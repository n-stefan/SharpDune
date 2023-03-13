/* ALSA implementation of the MIDI */

/*
 * Copyright(c) 2010 Atsushi Eno
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

#if LINUX

namespace SharpDune.Audio;

partial class MidiAlsa
{
    const int SND_SEQ_OPEN_OUTPUT = 1;
    const int SND_SEQ_PORT_CAP_READ = 1 << 0;
    const int SND_SEQ_PORT_CAP_SUBS_READ = 1 << 5;
    const int SND_SEQ_PORT_CAP_WRITE = 1 << 1;
    const int SND_SEQ_PORT_CAP_SUBS_WRITE = 1 << 6;
    const int SND_SEQ_PORT_TYPE_MIDI_GENERIC = 1 << 1;
    const int SND_SEQ_QUEUE_DIRECT = 253;
    const int SND_SEQ_ADDRESS_UNKNOWN = 253;
    const int SND_SEQ_ADDRESS_SUBSCRIBERS = 254;
    const string LibraryName = "libasound";

    static /* snd_seq_t* */ nint s_midi = nint.Zero;
    static /* snd_midi_event_t* */ nint s_midiCoder = nint.Zero;
    static /* snd_seq_port_subscribe_t* */ nint s_midiSubscription = nint.Zero;
    static int s_midiPort = -1;
    static string s_midiCaption = "SharpDUNE MIDI Port";

    [StructLayout(LayoutKind.Sequential)]
    struct snd_seq_addr_t
    {
        public byte client;
        public byte port;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct snd_seq_real_time_t
    {
        public uint tv_sec;
        public uint tv_nsec;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct snd_seq_timestamp_t
    {
        [FieldOffset(0)]
        public uint tick;
        [FieldOffset(0)]
        public snd_seq_real_time_t time;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct snd_seq_event_t
    {
        public byte type;
        public byte flags;
        public byte tag;
        public byte queue;
        public snd_seq_timestamp_t time;
        public snd_seq_addr_t source;
        public snd_seq_addr_t dest;
    }

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_close(nint handle);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_delete_port(nint handle, int port);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial void snd_midi_event_init(nint dev);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_midi_event_new(uint bufsize, ref nint rdev);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_client_id(nint handle);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_port_info_get_client(nint info);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_port_info_get_port(nint info);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_client_info_get_client(nint info);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_set_client_name(nint seq, nint name);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial void snd_seq_client_info_set_client(nint info, int client);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_query_next_client(nint handle, nint info);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial void snd_seq_port_info_set_client(nint info, int client);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial void snd_seq_port_info_set_port(nint info, int port);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_query_next_port(nint handle, nint info);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial uint snd_seq_port_info_get_capability(nint info);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial nint snd_seq_port_info_get_name(nint info);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_create_simple_port(nint seq, nint name, uint caps, uint type);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_port_subscribe_malloc(ref nint ptr);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial void snd_seq_port_subscribe_set_sender(nint info, nint addr);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial void snd_seq_port_subscribe_set_dest(nint info, nint addr);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial void snd_seq_port_subscribe_set_time_update(nint info, int val);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial void snd_seq_port_subscribe_set_time_real(nint info, int val);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_subscribe_port(nint handle, nint sub);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_port_info_malloc(ref nint port);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial void snd_seq_port_info_free(nint port);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_client_info_malloc(ref nint port);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial void snd_seq_client_info_free(nint port);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_open(ref nint handle, nint name, int streams, int mode);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial void snd_midi_event_free(nint dev);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial void snd_seq_port_subscribe_free(nint ptr);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial void snd_midi_event_reset_encode(nint dev);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_event_output(nint handle, nint ev);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_seq_drain_output(nint handle);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial long snd_midi_event_encode(nint dev, nint buf, long count, nint ev);

    [LibraryImport(LibraryName), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial int snd_midi_event_encode_byte(nint dev, int c, nint ev);

    internal static bool Midi_Init()
    {
        snd_seq_addr_t sender, receiver;
        /* snd_seq_port_info_t* */ var pinfo = nint.Zero;
        /* snd_seq_client_info_t* */ var cinfo = nint.Zero;
        var found = false;
        var name = Marshal.StringToHGlobalAuto(s_midiCaption);

        if (snd_seq_open(ref s_midi, Marshal.StringToHGlobalAuto("default"), SND_SEQ_OPEN_OUTPUT, 0) < 0)
        {
            Trace.WriteLine("ERROR: Failed to initialize MIDI");
            s_midi = nint.Zero;
            return false;
        }
        snd_seq_set_client_name(s_midi, name);

        /* Create a port to work on */
        s_midiPort = snd_seq_create_simple_port(s_midi, name, SND_SEQ_PORT_CAP_READ | SND_SEQ_PORT_CAP_SUBS_READ, SND_SEQ_PORT_TYPE_MIDI_GENERIC);
        if (s_midiPort < 0)
        {
            Trace.WriteLine("ERROR: Failed to initialize MIDI");
            snd_seq_close(s_midi);
            s_midi = nint.Zero;
            return false;
        }

        /* Try to find a MIDI out */
        snd_seq_port_info_malloc(ref pinfo);
        snd_seq_client_info_malloc(ref cinfo);
        snd_seq_client_info_set_client(cinfo, -1);

        /* Walk all clients and ports, and see if one matches our demands */
        while (snd_seq_query_next_client(s_midi, cinfo) >= 0 && !found)
        {
            int client;

            client = snd_seq_client_info_get_client(cinfo);
            if (client == 0) continue;

            snd_seq_port_info_set_client(pinfo, client);
            snd_seq_port_info_set_port(pinfo, -1);
            while (snd_seq_query_next_port(s_midi, pinfo) >= 0)
            {
                if ((snd_seq_port_info_get_capability(pinfo) & (SND_SEQ_PORT_CAP_WRITE | SND_SEQ_PORT_CAP_SUBS_WRITE)) != (SND_SEQ_PORT_CAP_WRITE | SND_SEQ_PORT_CAP_SUBS_WRITE)) continue;
                /* Most linux installations come with a Midi Through Port.
                 *  This is 'hardware' support that mostly ends up on your serial, which
                 *  you most likely do not have connected. So we skip it by default. */
                if (Marshal.PtrToStringAuto(snd_seq_port_info_get_name(pinfo)).StartsWith("Midi Through Port")) continue;
                found = true;
                break;
            }
        }

        if (!found)
        {
            Trace.WriteLine("ERROR: No valid MIDI output ports.\n  Please install and start Timidity++ like: timidity -iA");
            snd_seq_delete_port(s_midi, s_midiPort);
            snd_seq_close(s_midi);
            s_midi = nint.Zero;
            return false;
        }

        /* Subscribe ourself to the port */
        receiver.client = (byte)snd_seq_port_info_get_client(pinfo);
        receiver.port = (byte)snd_seq_port_info_get_port(pinfo);
        sender.client = (byte)snd_seq_client_id(s_midi);
        sender.port = (byte)s_midiPort;

        var senderPtr = Marshal.AllocHGlobal(Marshal.SizeOf(sender));
        Marshal.StructureToPtr(sender, senderPtr, true);
        var receiverPtr = Marshal.AllocHGlobal(Marshal.SizeOf(receiver));
        Marshal.StructureToPtr(receiver, receiverPtr, true);

        snd_seq_port_subscribe_malloc(ref s_midiSubscription);
        snd_seq_port_subscribe_set_sender(s_midiSubscription, senderPtr);
        snd_seq_port_subscribe_set_dest(s_midiSubscription, receiverPtr);
        snd_seq_port_subscribe_set_time_update(s_midiSubscription, 1);
        snd_seq_port_subscribe_set_time_real(s_midiSubscription, 1);
        if (snd_seq_subscribe_port(s_midi, s_midiSubscription) < 0)
        {
            Trace.WriteLine("ERROR: Failed to subscribe to MIDI output");
            snd_seq_delete_port(s_midi, s_midiPort);
            snd_seq_close(s_midi);
            s_midi = nint.Zero;
            return false;
        }

        /* Start the MIDI decoder */
        if (snd_midi_event_new(4, ref s_midiCoder) < 0)
        {
            Trace.WriteLine("ERROR: Failed to initialize MIDI decoder");
            snd_seq_delete_port(s_midi, s_midiPort);
            snd_seq_close(s_midi);
            s_midi = nint.Zero;
            return false;
        }
        snd_midi_event_init(s_midiCoder);

        snd_seq_port_info_free(pinfo);
        snd_seq_client_info_free(cinfo);
        Marshal.FreeHGlobal(senderPtr);
        Marshal.FreeHGlobal(receiverPtr);

        return true;
    }

    internal static void Midi_Uninit()
    {
        if (s_midi == nint.Zero) return;

        snd_midi_event_free(s_midiCoder);
        snd_seq_port_subscribe_free(s_midiSubscription);
        snd_seq_delete_port(s_midi, s_midiPort);
        snd_seq_close(s_midi);

        s_midi = nint.Zero;
    }

    internal static void Midi_Reset()
    {
        if (s_midi == nint.Zero) return;

        snd_midi_event_reset_encode(s_midiCoder);
    }

    internal static void Midi_Send(uint data)
    {
        if (s_midi == nint.Zero) return;

        var ev = new snd_seq_event_t();
        int r;

        //snd_seq_ev_clear(ev);
        snd_seq_ev_set_source(ref ev, (byte)s_midiPort);
        snd_seq_ev_set_subs(ref ev);
        snd_seq_ev_set_direct(ref ev);

        var evPtr = Marshal.AllocHGlobal(Marshal.SizeOf(ev));
        Marshal.StructureToPtr(ev, evPtr, true);

        r = snd_midi_event_encode_byte(s_midiCoder, (int)(data & 0xff), evPtr);  /* status byte */
        if (r < 0)
        {
            Trace.WriteLine($"WARNING: snd_midi_event_encode_byte() failed to send status byte {data & 0xff:X2}. err={r}");
        }
        else if (r == 0)
        {
            /* snd_midi_event_encode_byte() returns 0 when more bytes are required to complete the event */
            r = snd_midi_event_encode_byte(s_midiCoder, (int)((data >> 8) & 0xff), evPtr);   /* 1st data byte */
            if (r < 0)
            {
                Trace.WriteLine($"WARNING: snd_midi_event_encode_byte() failed to send 1st data byte {(data >> 8) & 0xff:X2}. err={r}");
            }
            else if (r == 0)
            {
                r = snd_midi_event_encode_byte(s_midiCoder, (int)((data >> 16) & 0xff), evPtr);  /* 2nd data byte */
                if (r < 0)
                {
                    Trace.WriteLine($"WARNING: snd_midi_event_encode_byte() failed to send 2nd data byte {(data >> 16) & 0xff:X2}. err={r}");
                }
                else if (r == 0)
                {
                    Trace.WriteLine($"WARNING: midi_send() no more data byte to send : {data & 0xff:X2} {(data >> 8) & 0xff:X2} {(data >> 16) & 0xff:X2}");
                }
            }
        }

        snd_seq_event_output(s_midi, evPtr);
        snd_seq_drain_output(s_midi);

        Marshal.FreeHGlobal(evPtr);
    }

    internal static ushort Midi_Send_String(byte[] data, ushort len)
    {
        if (s_midi == nint.Zero) return len;

        var ev = new snd_seq_event_t();

        //snd_seq_ev_clear(ev);
        snd_seq_ev_set_source(ref ev, (byte)s_midiPort);
        snd_seq_ev_set_subs(ref ev);
        snd_seq_ev_set_direct(ref ev);

        var evPtr = Marshal.AllocHGlobal(Marshal.SizeOf(ev));
        Marshal.StructureToPtr(ev, evPtr, true);

        var dataPtr = Marshal.AllocHGlobal(len);
        Marshal.Copy(data, 0, dataPtr, len);

        snd_midi_event_encode(s_midiCoder, dataPtr, len, evPtr);

        snd_seq_event_output(s_midi, evPtr);
        snd_seq_drain_output(s_midi);

        Marshal.FreeHGlobal(evPtr);
        Marshal.FreeHGlobal(dataPtr);

        return len;
    }

    private static void snd_seq_ev_set_source(ref snd_seq_event_t ev, byte port) =>
        ev.source.port = port;

    private static void snd_seq_ev_set_subs(ref snd_seq_event_t ev)
    {
        ev.dest.client = SND_SEQ_ADDRESS_SUBSCRIBERS;
        ev.dest.port = SND_SEQ_ADDRESS_UNKNOWN;
    }

    private static void snd_seq_ev_set_direct(ref snd_seq_event_t ev) =>
        ev.queue = SND_SEQ_QUEUE_DIRECT;

    //#define snd_seq_ev_clear(ev) memset(ev, 0, sizeof(snd_seq_event_t))
}

#endif
