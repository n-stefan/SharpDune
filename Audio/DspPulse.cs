/* PulseAudio implementation of the DSP */

#if LINUX

namespace SharpDune.Audio;

unsafe class DspPulse
{
    static pa_mainloop_api* s_mainloop_api;
    static /* pa_mainloop* */ nint s_mainloop = nint.Zero;
    static /* pa_context* */ nint s_context = nint.Zero;
    static /* pa_stream* */ nint s_stream = nint.Zero;
    static uint s_current_freq;
    static bool s_playing;
    static nint s_data = nint.Zero;

    [UnmanagedCallersOnly]
    static void DSP_context_event_cb(/* pa_context* */ nint c, sbyte* name, /* pa_proplist* */ nint p, void* userdata) =>
        Debug.WriteLine($"DEBUG: DSP_context_event_cb({c}, {Marshal.PtrToStringAuto((nint)name)}, {p}, {(nint)userdata})");

    [UnmanagedCallersOnly]
    static void DSP_stream_state_cb(/* pa_stream* */ nint p, void* userdata)
    {
        /*pa_operation * op;*/
        var state = pa_stream_get_state(p);
        Debug.WriteLine($"DEBUG: DSP_stream_state_cb({p}, {(nint)userdata}) state={(int)state}");
        switch (state)
        {
            case pa_stream_state.PA_STREAM_READY:
                Debug.WriteLine("DEBUG: PA_STREAM_READY");
//#if 0
//			if (pa_stream_write(p, s_data, s_dataLen, NULL, 0, PA_SEEK_RELATIVE) < 0) {
//				Error("pa_stream_write() failed\n");
//			}
//			/*op = pa_stream_cork(p, 0, DSP_stream_success_cb, p);*/
//			op = pa_stream_trigger(p, DSP_stream_success_cb, NULL);
//			pa_operation_unref(op);
//#endif
                break;
            case pa_stream_state.PA_STREAM_FAILED:
                Trace.WriteLine("WARNING: PA_STREAM_FAILED");
                break;
            case pa_stream_state.PA_STREAM_TERMINATED:
                Debug.WriteLine("DEBUG: PA_STREAM_TERMINATED");
                break;
            default:
            case pa_stream_state.PA_STREAM_UNCONNECTED:
            case pa_stream_state.PA_STREAM_CREATING:
                break;
        }
    }

    [UnmanagedCallersOnly]
    static void DSP_stream_underflow_cb(/* pa_stream* */ nint p, void* userdata)
    {
        Debug.WriteLine($"DEBUG: DSP_stream_underflow_cb({p}, {(nint)userdata})");
        s_playing = false;
    }

    [UnmanagedCallersOnly]
    static void DSP_stream_flush_cb(/* pa_stream* */ nint p, int success, void* userdata) =>
        Debug.WriteLine($"DEBUG: DSP_stream_flush_cb({p}, {success}, {(nint)userdata})");

    [UnmanagedCallersOnly]
    static void DSP_stream_update_rate_cb(/* pa_stream* */ nint p, int success, void* userdata) =>
        Debug.WriteLine($"DEBUG: DSP_stream_update_rate_cb({p}, {success}, {(nint)userdata})");

    static void DSP_PulseAudio_Tick()
    {
        int retval;
        pa_mainloop_iterate(s_mainloop, 0, &retval);    /* non blocking */
    }

    internal static bool DSP_Init()
    {
        int retval;
        pa_context_state state;
        pa_sample_spec sample_spec;

        if (s_context != nint.Zero)
            return true;

        Debug.WriteLine($"DEBUG: DSP_Init() PulseAudio version {Marshal.PtrToStringAuto((nint)pa_get_library_version())}");
        s_mainloop = pa_mainloop_new();
        if (s_mainloop == nint.Zero)
        {
            Trace.WriteLine("ERROR: PulseAudio pa_mainloop_new() failed");
            return false;
        }
        s_mainloop_api = pa_mainloop_get_api(s_mainloop);
        /*
            if (pa_signal_init(s_mainloop_api) != 0) {
                Error("PulseAudio pa_signal_init() failed\n");
                return false;
            }
        */
        s_context = pa_context_new_with_proplist(s_mainloop_api, (sbyte*)Marshal.StringToHGlobalAuto("SharpDUNE"), nint.Zero/*proplist*/);
        if (s_context == nint.Zero)
        {
            Trace.WriteLine("ERROR: PulseAudio pa_context_new_with_proplist() failed");
            return false;
        }
        /*pa_context_set_state_callback(s_context, DSP_context_state_cb, NULL);*/
        pa_context_set_event_callback(s_context, &DSP_context_event_cb, null/*userdata*/);
        if (pa_context_connect(s_context, null, 0/*PA_CONTEXT_NOAUTOSPAWN*/, null) != 0)
        {
            Trace.WriteLine("ERROR: PulseAudio pa_context_connect() failed");
            return false;
        }
        /* Wait for context to be ready */
        do
        {
            pa_mainloop_iterate(s_mainloop, 1, &retval);
            state = pa_context_get_state(s_context);
            if (state is pa_context_state.PA_CONTEXT_FAILED or pa_context_state.PA_CONTEXT_TERMINATED) return false;
        } while (state != pa_context_state.PA_CONTEXT_READY);
        /* create stream */
        s_current_freq = 10989; /* default value */
        sample_spec.format = pa_sample_format.PA_SAMPLE_U8;
        sample_spec.rate = s_current_freq;
        sample_spec.channels = 1;
        s_stream = pa_stream_new(s_context, (sbyte*)Marshal.StringToHGlobalAuto("DuneSpeech"), &sample_spec, null);
        if (s_stream == nint.Zero)
        {
            Trace.WriteLine("ERROR: pa_stream_new() failed");
            return false;
        }
        pa_stream_set_state_callback(s_stream, &DSP_stream_state_cb, null);
        pa_stream_set_underflow_callback(s_stream, &DSP_stream_underflow_cb, null);
        if (pa_stream_connect_playback(s_stream, null, null, pa_stream_flags.PA_STREAM_START_UNMUTED | pa_stream_flags.PA_STREAM_VARIABLE_RATE | pa_stream_flags.PA_STREAM_ADJUST_LATENCY, null, nint.Zero) < 0)
        {
            Trace.WriteLine("ERROR: pa_stream_connect_playback() failed");
            return false;
        }
        Timer_Add(DSP_PulseAudio_Tick, 1000000 / 100, false);
        return true;
    }

    internal static void DSP_Uninit()
    {
        int retval;

        pa_stream_disconnect(s_stream);
        pa_stream_unref(s_stream);
        s_stream = nint.Zero;
        pa_context_unref(s_context);
        s_context = nint.Zero;
        pa_mainloop_quit(s_mainloop, 42);
        pa_mainloop_run(s_mainloop, &retval);
        pa_mainloop_free(s_mainloop);
        s_mainloop = nint.Zero;
    }

    internal static byte DSP_GetStatus() =>
        (byte)(s_playing ? 2 : 0);

    internal static void DSP_Play(byte[] data)
    {
        /*pa_sample_spec sample_spec;
        pa_stream * stream;*/
        uint freq;
        uint len;
        /*pa_buffer_attr attr;*/
        var dataPointer = 0;

        dataPointer += Read_LE_UInt16(data.AsSpan(20));  /* Skip VOC header */

        if (data[dataPointer] != 1) return;   /* if not a Sound Data block, return */
        len = (uint)(data[dataPointer + 1] | (data[dataPointer + 2] << 8) | (data[dataPointer + 3] << 16));
        len -= 2;
        freq = (uint)(1000000 / (256 - data[dataPointer + 4]));
        /* data[dataPointer + 5] = codec */
        dataPointer += 6;

        s_data = Marshal.AllocHGlobal((nint)len);
        Marshal.Copy(data, dataPointer, s_data, (int)len);

        Debug.WriteLine($"DEBUG: DSP_Play() data={s_data} freq={freq}");

        if (s_playing)
        {
            /* pa_operation* */ var operation = pa_stream_flush(s_stream, &DSP_stream_flush_cb, null);
            pa_operation_unref(operation);
        }

        if (freq != s_current_freq)
        {
            /* pa_operation* */ nint operation;
            operation = pa_stream_update_sample_rate(s_stream, freq, &DSP_stream_update_rate_cb, null);
            s_current_freq = freq;
            pa_operation_unref(operation);
        }

        if (pa_stream_write(s_stream, (void*)s_data, len, null, 0, pa_seek_mode.PA_SEEK_RELATIVE) < 0)
        {
            Trace.WriteLine("ERROR: pa_stream_write() failed");
        }
        s_playing = true;
    }

    internal static void DSP_Stop()
    {
        Debug.WriteLine("DEBUG: DSP_Stop()");
        if (s_playing)
        {
            /* pa_operation* */ var operation = pa_stream_flush(s_stream, &DSP_stream_flush_cb, null);
            pa_operation_unref(operation);
            s_playing = false;
        }

        if (s_data != nint.Zero)
            Marshal.FreeHGlobal(s_data);
    }
}

#endif
