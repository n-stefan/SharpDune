﻿/* Windows implementation of the DSP */

/*
 * Copyright © 2010 John Gietzen
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
 * BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
 * ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#if !LINUX

namespace SharpDune.Audio;

partial class DspWin
{
    const string LibraryName = "winmm";
    const int WaveOutMapperDeviceId = -1;
    static bool s_playing;
    static bool s_init;
    static uint s_dataLen;
    static nint s_data;
    static nint s_waveOut = nint.Zero;
    static nint s_waveHdrAddr;
    static readonly WaveOutProc DSP_Callback_Del = DSP_Callback;

    /// <summary>
    /// Used as a return result from many of the WinMM calls.
    /// </summary>
    internal enum MMSYSERROR
    {
        /// <summary>
        /// No Error. (Success)
        /// </summary>
        MMSYSERR_NOERROR = 0,

        /// <summary>
        /// Unspecified Error.
        /// </summary>
        MMSYSERR_ERROR = 1,

        /// <summary>
        /// Device ID out of range.
        /// </summary>
        MMSYSERR_BADDEVICEID = 2,

        /// <summary>
        /// Driver failed enable.
        /// </summary>
        MMSYSERR_NOTENABLED = 3,

        /// <summary>
        /// Device is already allocated.
        /// </summary>
        MMSYSERR_ALLOCATED = 4,

        /// <summary>
        /// Device handle is invalid.
        /// </summary>
        MMSYSERR_INVALHANDLE = 5,

        /// <summary>
        /// No device driver is present.
        /// </summary>
        MMSYSERR_NODRIVER = 6,

        /// <summary>
        /// In sufficient memory, or memory allocation error.
        /// </summary>
        MMSYSERR_NOMEM = 7,

        /// <summary>
        /// Unsupported function.
        /// </summary>
        MMSYSERR_NOTSUPPORTED = 8,

        /// <summary>
        /// Error value out of range.
        /// </summary>
        MMSYSERR_BADERRNUM = 9,

        /// <summary>
        /// Invalid flag passed.
        /// </summary>
        MMSYSERR_INVALFLAG = 10,

        /// <summary>
        /// Invalid parameter passed.
        /// </summary>
        MMSYSERR_INVALPARAM = 11,

        /// <summary>
        /// Handle being used simultaneously on another thread.
        /// </summary>
        MMSYSERR_HANDLEBUSY = 12,

        /// <summary>
        /// Specified alias not found.
        /// </summary>
        MMSYSERR_INVALIDALIAS = 13,

        /// <summary>
        /// Bad registry database.
        /// </summary>
        MMSYSERR_BADDB = 14,

        /// <summary>
        /// Registry key not found.
        /// </summary>
        MMSYSERR_KEYNOTFOUND = 15,

        /// <summary>
        /// Registry read error.
        /// </summary>
        MMSYSERR_READERROR = 16,

        /// <summary>
        /// Registry write error.
        /// </summary>
        MMSYSERR_WRITEERROR = 17,

        /// <summary>
        /// Registry delete error.
        /// </summary>
        MMSYSERR_DELETEERROR = 18,

        /// <summary>
        /// Registry value not found.
        /// </summary>
        MMSYSERR_VALNOTFOUND = 19,

        /// <summary>
        /// Driver does not call DriverCallback.
        /// </summary>
        MMSYSERR_NODRIVERCB = 20,

        /// <summary>
        /// More data to be returned.
        /// </summary>
        MMSYSERR_MOREDATA = 21
    }

    /// <summary>
    /// Flags supplying information about the buffer. The following values are defined:
    /// </summary>
    [Flags]
    internal enum WaveHeaderFlags
    {
        /// <summary>
        /// Set by the device driver to indicate that it is finished with the buffer and is returning it to the application.
        /// </summary>
        Done = 0x00000001,

        /// <summary>
        /// Set by Windows to indicate that the buffer has been prepared with the waveInPrepareHeader or waveOutPrepareHeader function.
        /// </summary>
        Prepared = 0x00000002,

        /// <summary>
        /// This buffer is the first buffer in a loop.  This flag is used only with output buffers.
        /// </summary>
        BeginLoop = 0x00000004,

        /// <summary>
        /// This buffer is the last buffer in a loop.  This flag is used only with output buffers.
        /// </summary>
        EndLoop = 0x00000008,

        /// <summary>
        /// Set by Windows to indicate that the buffer is queued for playback.
        /// </summary>
        InQueue = 0x00000010
    }

    /// <summary>
    /// Indicates a wave data sample format.
    /// </summary>
    internal enum WaveFormatTag
    {
        /// <summary>
        /// Indicates an invalid sample format.
        /// </summary>
        Invalid = 0x00,

        /// <summary>
        /// Indicates raw Pulse Code Modulation data.
        /// </summary>
        Pcm = 0x01,

        /// <summary>
        /// Indicates Adaptive Differential Pulse Code Modulation data.
        /// </summary>
        Adpcm = 0x02,

        /// <summary>
        /// Indicates IEEE-Float data.
        /// </summary>
        Float = 0x03,

        /// <summary>
        /// Indicates a-law companded data.
        /// </summary>
        ALaw = 0x06,

        /// <summary>
        /// Indicates μ-law companded data.
        /// </summary>
        MuLaw = 0x07,
    }

    /// <summary>
    /// Used with the <see cref="NativeMethods.waveOutOpen"/> command.
    /// </summary>
    [Flags]
    internal enum WaveOpenFlags
    {
        /// <summary>
        /// No callback mechanism. This is the default setting.
        /// </summary>
        CALLBACK_NULL = 0x00000,

        /// <summary>
        /// If this flag is specified, <see cref="NativeMethods.waveOutOpen"/> queries the device to determine if it supports the given format, but the device is not actually opened.
        /// </summary>
        WAVE_FORMAT_QUERY = 0x00001,

        /// <summary>
        /// If this flag is specified, a synchronous waveform-audio device can be opened. If this flag is not specified while opening a synchronous driver, the device will fail to open.
        /// </summary>
        WAVE_ALLOWSYNC = 0x00002,

        /// <summary>
        /// If this flag is specified, the uDeviceID parameter specifies a waveform-audio device to be mapped to by the wave mapper.
        /// </summary>
        WAVE_MAPPED = 0x00004,

        /// <summary>
        /// If this flag is specified, the ACM driver does not perform conversions on the audio data.
        /// </summary>
        WAVE_FORMAT_DIRECT = 0x00008,

        /// <summary>
        /// Indicates the dwCallback parameter is a window handle.
        /// </summary>
        CALLBACK_WINDOW = 0x10000,

        /// <summary>
        /// The dwCallback parameter is a thread identifier.
        /// </summary>
        CALLBACK_THREAD = 0x20000,

        /// <summary>
        /// The dwCallback parameter is a thread identifier.
        /// </summary>
        [Obsolete]
        CALLBACK_TASK = 0x20000,

        /// <summary>
        /// The dwCallback parameter is a callback procedure address.
        /// </summary>
        CALLBACK_FUNCTION = 0x30000
    }

    /// <summary>
    /// Indicates a WaveOut message.
    /// </summary>
    internal enum WaveOutMessage
    {
        /// <summary>
        /// Not Used. Indicates that there is no message.
        /// </summary>
        None = 0x000,

        /// <summary>
        /// Indicates that the device has been opened.
        /// </summary>
        DeviceOpened = 0x3BB,

        /// <summary>
        /// Indicates that the device has been closed.
        /// </summary>
        DeviceClosed = 0x3BC,

        /// <summary>
        /// Indicates that playback of a write operation has been completed.
        /// </summary>
        WriteDone = 0x3BD
    }

    /// <summary>
    /// Describes the full format of a wave formatted stream.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct WAVEFORMATEX
    {
        /// <summary>
        /// The wave format of the stream.
        /// </summary>
        public short wFormatTag;

        /// <summary>
        /// The number of channels.
        /// </summary>
        public short nChannels;

        /// <summary>
        /// The number of samples per second.
        /// </summary>
        public int nSamplesPerSec;

        /// <summary>
        /// The average bytes per second.
        /// </summary>
        public int nAvgBytesPerSec;

        /// <summary>
        /// The smallest atomic data size.
        /// </summary>
        public short nBlockAlign;

        /// <summary>
        /// The number of bits per sample.
        /// </summary>
        public short wBitsPerSample;

        /// <summary>
        /// The remaining header size. (Must be zero in this struct format.)
        /// </summary>
        public short cbSize;
    }

    /// <summary>
    /// The WAVEHDR structure defines the header used to identify a waveform-audio buffer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct WAVEHDR
    {
        /// <summary>
        /// Pointer to the waveform buffer.
        /// </summary>
        public nint lpData;

        /// <summary>
        /// Length, in bytes, of the buffer.
        /// </summary>
        public int dwBufferLength;

        /// <summary>
        /// When the header is used in input, this member specifies how much data is in the buffer.
        /// </summary>
        public int dwBytesRecorded;

        /// <summary>
        /// User data.
        /// </summary>
        public nint dwUser;

        /// <summary>
        /// Flags supplying information about the buffer. The following values are defined:
        /// </summary>
        public WaveHeaderFlags dwFlags;

        /// <summary>
        /// Number of times to play the loop. This member is used only with output buffers.
        /// </summary>
        public int dwLoops;

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        public nint lpNext;

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        public int reserved;
    }

    internal delegate void WaveOutProc(nint handle, WaveOutMessage uMsg, nint dwInstance, nint dwParam1, nint dwParam2);

    [LibraryImport(LibraryName, SetLastError = true), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial MMSYSERROR waveOutReset(nint handle);

    [LibraryImport(LibraryName, SetLastError = true), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial MMSYSERROR waveOutUnprepareHeader(nint handle, nint pwh, int cbwh);

    [LibraryImport(LibraryName, SetLastError = true), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial MMSYSERROR waveOutClose(nint handle);

    [LibraryImport(LibraryName, SetLastError = true), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial MMSYSERROR waveOutWrite(nint handle, nint pwh, int cbwh);

    [LibraryImport(LibraryName, SetLastError = true), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial MMSYSERROR waveOutPrepareHeader(nint handle, nint pwh, int cbwh);

    [LibraryImport(LibraryName, SetLastError = true), DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    private static partial MMSYSERROR waveOutOpen(ref nint handle, int uDeviceID, ref WAVEFORMATEX pwfx, WaveOutProc dwCallback, nint dwCallbackInstance, WaveOpenFlags dwFlags);

    internal static byte DSP_GetStatus() =>
        (byte)(s_playing ? 2 : 0);

    internal static void DSP_Stop()
    {
        if (s_waveOut == nint.Zero) return;

        waveOutReset(s_waveOut);
        waveOutUnprepareHeader(s_waveOut, s_waveHdrAddr, Marshal.SizeOf(typeof(WAVEHDR)));
        waveOutClose(s_waveOut);

        s_waveOut = nint.Zero;
        s_playing = false;

        s_dataLen = 0;

        if (s_data != nint.Zero)
            Marshal.FreeHGlobal(s_data);

        if (s_waveHdrAddr != nint.Zero)
            Marshal.FreeHGlobal(s_waveHdrAddr);
    }

    internal static bool DSP_Init()
    {
        s_init = true;
        return true;
    }

    internal static void DSP_Uninit()
    {
        if (!s_init) return;

        DSP_Stop();

        //free(s_data);

        s_init = false;
    }

    internal static void DSP_Play(byte[] data)
    {
        int freq;
        MMSYSERROR res;
        var dataPointer = 0;

        DSP_Stop();

        dataPointer += Read_LE_UInt16(data.AsSpan(20));

        if (data[dataPointer] != 1) return;

        s_dataLen = (Read_LE_UInt32(data.AsSpan(dataPointer)) >> 8) - 2;

        s_data = Marshal.AllocHGlobal((nint)s_dataLen); //s_data = realloc(s_data, len);

        Marshal.Copy(data, dataPointer + 6, s_data, (int)s_dataLen); //memcpy(s_data, data + 6, len);

        freq = 1000000 / (256 - data[dataPointer + 4]);

        var waveFormat = new WAVEFORMATEX
        {
            wFormatTag = (short)WaveFormatTag.Pcm, //WAVE_FORMAT_PCM;
            nChannels = 1,
            nSamplesPerSec = freq,
            nAvgBytesPerSec = freq,
            nBlockAlign = 1,
            wBitsPerSample = 8,
            cbSize = 0 //sizeof(WAVEFORMATEX);
        };

        res = waveOutOpen(ref s_waveOut, WaveOutMapperDeviceId/*WAVE_MAPPER*/, ref waveFormat, DSP_Callback_Del, nint.Zero, WaveOpenFlags.CALLBACK_FUNCTION/*CALLBACK_FUNCTION*/);
        if (res != MMSYSERROR.MMSYSERR_NOERROR)
        {
            Trace.WriteLine($"ERROR: waveOutOpen failed ({res})");
            s_waveOut = nint.Zero;
            return;
        }

        var s_waveHdr = new WAVEHDR
        {
            lpData = s_data,
            dwBufferLength = (int)s_dataLen,
            dwFlags = 0,
            dwLoops = 0
        };

        s_waveHdrAddr = Marshal.AllocHGlobal(Marshal.SizeOf(s_waveHdr));
        Marshal.StructureToPtr(s_waveHdr, s_waveHdrAddr, false);

        res = waveOutPrepareHeader(s_waveOut, s_waveHdrAddr, Marshal.SizeOf(typeof(WAVEHDR)));
        if (res != MMSYSERROR.MMSYSERR_NOERROR)
        {
            Trace.WriteLine($"ERROR: waveOutPrepareHeader failed ({res})");
            return;
        }

        res = waveOutWrite(s_waveOut, s_waveHdrAddr, Marshal.SizeOf(typeof(WAVEHDR)));
        if (res != MMSYSERROR.MMSYSERR_NOERROR)
        {
            Trace.WriteLine($"ERROR: waveOutWrite failed ({res})");
            return;
        }

        s_playing = true;
    }

    static void DSP_Callback(nint handle, WaveOutMessage uMsg, nint dwInstance, nint dwParam1, nint dwParam2) =>
        s_playing = false;
}

#endif
