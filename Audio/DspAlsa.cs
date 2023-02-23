/* ALSA implementation of the DSP */

/*
 * MIT License
 * 
 * Copyright (c) 2019 Zhang Yuexin
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#if LINUX

namespace SharpDune.Audio;

partial class DspAlsa
{
    enum snd_pcm_stream_t { SND_PCM_STREAM_PLAYBACK = 0, SND_PCM_STREAM_CAPTURE, SND_PCM_STREAM_LAST = SND_PCM_STREAM_CAPTURE }

    enum snd_pcm_access_t
    {
        SND_PCM_ACCESS_MMAP_INTERLEAVED = 0, SND_PCM_ACCESS_MMAP_NONINTERLEAVED, SND_PCM_ACCESS_MMAP_COMPLEX, SND_PCM_ACCESS_RW_INTERLEAVED,
        SND_PCM_ACCESS_RW_NONINTERLEAVED, SND_PCM_ACCESS_LAST = SND_PCM_ACCESS_RW_NONINTERLEAVED
    }

    enum snd_pcm_format_t
    {
        SND_PCM_FORMAT_UNKNOWN = -1, SND_PCM_FORMAT_S8 = 0, SND_PCM_FORMAT_U8, SND_PCM_FORMAT_S16_LE,
        SND_PCM_FORMAT_S16_BE, SND_PCM_FORMAT_U16_LE, SND_PCM_FORMAT_U16_BE, SND_PCM_FORMAT_S24_LE,
        SND_PCM_FORMAT_S24_BE, SND_PCM_FORMAT_U24_LE, SND_PCM_FORMAT_U24_BE, SND_PCM_FORMAT_S32_LE,
        SND_PCM_FORMAT_S32_BE, SND_PCM_FORMAT_U32_LE, SND_PCM_FORMAT_U32_BE, SND_PCM_FORMAT_FLOAT_LE,
        SND_PCM_FORMAT_FLOAT_BE, SND_PCM_FORMAT_FLOAT64_LE, SND_PCM_FORMAT_FLOAT64_BE, SND_PCM_FORMAT_IEC958_SUBFRAME_LE,
        SND_PCM_FORMAT_IEC958_SUBFRAME_BE, SND_PCM_FORMAT_MU_LAW, SND_PCM_FORMAT_A_LAW, SND_PCM_FORMAT_IMA_ADPCM,
        SND_PCM_FORMAT_MPEG, SND_PCM_FORMAT_GSM, SND_PCM_FORMAT_S20_LE, SND_PCM_FORMAT_S20_BE,
        SND_PCM_FORMAT_U20_LE, SND_PCM_FORMAT_U20_BE, SND_PCM_FORMAT_SPECIAL = 31, SND_PCM_FORMAT_S24_3LE = 32,
        SND_PCM_FORMAT_S24_3BE, SND_PCM_FORMAT_U24_3LE, SND_PCM_FORMAT_U24_3BE, SND_PCM_FORMAT_S20_3LE,
        SND_PCM_FORMAT_S20_3BE, SND_PCM_FORMAT_U20_3LE, SND_PCM_FORMAT_U20_3BE, SND_PCM_FORMAT_S18_3LE,
        SND_PCM_FORMAT_S18_3BE, SND_PCM_FORMAT_U18_3LE, SND_PCM_FORMAT_U18_3BE, SND_PCM_FORMAT_G723_24,
        SND_PCM_FORMAT_G723_24_1B, SND_PCM_FORMAT_G723_40, SND_PCM_FORMAT_G723_40_1B, SND_PCM_FORMAT_DSD_U8,
        SND_PCM_FORMAT_DSD_U16_LE, SND_PCM_FORMAT_DSD_U32_LE, SND_PCM_FORMAT_DSD_U16_BE, SND_PCM_FORMAT_DSD_U32_BE,
        SND_PCM_FORMAT_LAST = SND_PCM_FORMAT_DSD_U32_BE, SND_PCM_FORMAT_S16 = SND_PCM_FORMAT_S16_LE, SND_PCM_FORMAT_U16 = SND_PCM_FORMAT_U16_LE, SND_PCM_FORMAT_S24 = SND_PCM_FORMAT_S24_LE,
        SND_PCM_FORMAT_U24 = SND_PCM_FORMAT_U24_LE, SND_PCM_FORMAT_S32 = SND_PCM_FORMAT_S32_LE, SND_PCM_FORMAT_U32 = SND_PCM_FORMAT_U32_LE, SND_PCM_FORMAT_FLOAT = SND_PCM_FORMAT_FLOAT_LE,
        SND_PCM_FORMAT_FLOAT64 = SND_PCM_FORMAT_FLOAT64_LE, SND_PCM_FORMAT_IEC958_SUBFRAME = SND_PCM_FORMAT_IEC958_SUBFRAME_LE, SND_PCM_FORMAT_S20 = SND_PCM_FORMAT_S20_LE, SND_PCM_FORMAT_U20 = SND_PCM_FORMAT_U20_LE
    }

    enum snd_pcm_state_t
    {
        SND_PCM_STATE_OPEN = 0, SND_PCM_STATE_SETUP, SND_PCM_STATE_PREPARED, SND_PCM_STATE_RUNNING,
        SND_PCM_STATE_XRUN, SND_PCM_STATE_DRAINING, SND_PCM_STATE_PAUSED, SND_PCM_STATE_SUSPENDED,
        SND_PCM_STATE_DISCONNECTED, SND_PCM_STATE_LAST = SND_PCM_STATE_DISCONNECTED, SND_PCM_STATE_PRIVATE1 = 1024
    }

    const int SND_PCM_NONBLOCK = 0x00000001;
    const int SND_PCM_ASYNC = 0x00000002;
    const string LibraryName = "libasound";

    static /* snd_pcm_t* */ nint s_dsp = nint.Zero;
    static /* snd_async_handler_t* */ nint s_dspAsync = nint.Zero;

    static bool s_init = false;
    static bool s_playing = false;

    static nint s_data = nint.Zero;
    static uint s_dataLen = 0;

    static nint s_buffer = nint.Zero;
    static uint s_bufferLen = 0;
    static uint s_bufferDone = 0;

    [LibraryImport(LibraryName)]
    private static partial int snd_pcm_close(nint pcm);

    [LibraryImport(LibraryName)]
    private static partial int snd_pcm_drop(nint pcm);

    [LibraryImport(LibraryName)]
    private static partial long snd_pcm_avail(nint pcm);

    [LibraryImport(LibraryName)]
    private static partial long snd_pcm_avail_update(nint pcm);

    [LibraryImport(LibraryName)]
    private static partial long snd_pcm_writei(nint pcm, nint buffer, ulong size);

    [LibraryImport(LibraryName)]
    private static partial snd_pcm_state_t snd_pcm_state(nint pcm);

    [LibraryImport(LibraryName)]
    private static partial int snd_pcm_hw_params(nint pcm, nint @params);

    [LibraryImport(LibraryName)]
    private static partial int snd_pcm_hw_params_set_rate(nint pcm, nint @params, uint val, int dir);

    [LibraryImport(LibraryName)]
    private static partial int snd_pcm_hw_params_set_channels(nint pcm, nint @params, uint val);

    [LibraryImport(LibraryName)]
    private static partial int snd_pcm_hw_params_set_format(nint pcm, nint @params, snd_pcm_format_t format);

    [LibraryImport(LibraryName)]
    private static partial int snd_pcm_hw_params_set_access(nint pcm, nint @params, snd_pcm_access_t access);

    [LibraryImport(LibraryName)]
    private static partial int snd_pcm_hw_params_any(nint pcm, nint @params);

    [LibraryImport(LibraryName)]
    private static partial int snd_pcm_hw_params_malloc(ref nint @params);

    [LibraryImport(LibraryName)]
    private static partial void snd_pcm_hw_params_free(nint @params);

    [LibraryImport(LibraryName)]
    unsafe private static partial int snd_async_add_pcm_handler(ref nint handler, nint pcm, delegate* unmanaged<nint, void> callback, nint private_data);

    [LibraryImport(LibraryName)]
    private static partial int snd_pcm_open(ref nint pcm, [MarshalAs(UnmanagedType.LPStr)] string name, snd_pcm_stream_t stream, int mode);

    [UnmanagedCallersOnly]
    static void DSP_Callback(/* snd_async_handler_t* */ nint ahandler)
    {
        uint len;

        if (!s_playing) return;

        /* Check how much we can buffer */
        len = (uint)snd_pcm_avail_update(s_dsp);
        if (len == 0) return;

        /* Check how much bytes we have left to write */
        if (len > s_bufferLen) len = s_bufferLen;
        if (len == 0) return;

        /* Queue as much as possible */
        snd_pcm_writei(s_dsp, s_buffer, len);
        nint.Add(s_buffer, (int)len);
        s_bufferLen -= len;
    }

    internal static void DSP_Stop()
    {
        if (s_dsp == nint.Zero) return;

        snd_pcm_drop(s_dsp);
        snd_pcm_close(s_dsp);

        s_dsp = nint.Zero;
        s_playing = false;
    }

    internal static void DSP_Uninit()
    {
        if (!s_init) return;

        DSP_Stop();

        Marshal.FreeHGlobal(s_data); s_data = nint.Zero;
        s_dataLen = 0;

        s_init = false;
    }

    internal static bool DSP_Init()
    {
        s_init = true;
        return true;
    }

    unsafe internal static void DSP_Play(byte[] data)
    {
        uint len;
        uint freq;
        /* snd_pcm_hw_params_t* */ nint dspParams = nint.Zero;
        var dataPointer = 0;

        DSP_Stop();

        dataPointer += Read_LE_UInt16(data.AsSpan(20));  /* skip Create Voice File header */

        /* first byte is Block Type :
	     * 0x00: Terminator
	     * 0x01: Sound data
	     * 0x02: Sound data continuation
	     * 0x03: Silence
	     * 0x04: Marker
	     * 0x05: Text
	     * 0x06: Repeat start
	     * 0x07: Repeat end
	     * 0x08: Extra info
	     * 0x09: Sound data (New format) */
        if (data[dataPointer] != 1) return;

        /* next 3 bytes are block size (not including the 1 block type and size 4 bytes) */
        len = (Read_LE_UInt32(data.AsSpan(dataPointer)) >> 8) - 2;
        dataPointer += 4;
        /* byte  0    frequency divisor
	     * byte  1    codec id : 0 is "8bits unsigned PCM"
	     * bytes 2..n audio data */

        s_data = Marshal.AllocHGlobal((int)len); //realloc(s_data, len);
        s_dataLen = len;

        Marshal.Copy(data, dataPointer + 2, s_data, (int)len); //memcpy(s_data, data + 2, len);

        freq = (uint)(1000000 / (256 - data[dataPointer]));
        if (data[dataPointer + 1] != 0) Trace.WriteLine($"WARNING: Unsupported VOC codec 0x{(int)data[dataPointer + 1]:X2}");

        /* Open device */
        if (snd_pcm_open(ref s_dsp, "default", snd_pcm_stream_t.SND_PCM_STREAM_PLAYBACK, SND_PCM_NONBLOCK | SND_PCM_ASYNC) < 0)
        {
            Trace.WriteLine("ERROR: Failed to initialize DSP");
            s_dsp = nint.Zero;
            return;
        }

        /* Set parameters */
        snd_pcm_hw_params_malloc(ref dspParams);
        Debug.Assert(dspParams != nint.Zero);
        if (snd_pcm_hw_params_any(s_dsp, dspParams) < 0) Trace.WriteLine("WARNING: snd_pcm_hw_params_any() failed");
        snd_pcm_hw_params_set_access(s_dsp, dspParams, snd_pcm_access_t.SND_PCM_ACCESS_RW_INTERLEAVED);
        snd_pcm_hw_params_set_format(s_dsp, dspParams, snd_pcm_format_t.SND_PCM_FORMAT_U8);
        if (snd_pcm_hw_params_set_channels(s_dsp, dspParams, 1) < 0) Trace.WriteLine("WARNING: snd_pcm_hw_params_set_channels() failed");
        if (snd_pcm_hw_params_set_rate(s_dsp, dspParams, freq, 0) < 0) Trace.WriteLine("WARNING: snd_pcm_hw_params_set_rate() failed");
        if (snd_pcm_hw_params(s_dsp, dspParams) < 0)
        {
            Trace.WriteLine("ERROR: Failed to set parameters for DSP");
            snd_pcm_hw_params_free(dspParams);
            snd_pcm_close(s_dsp);
            s_dsp = nint.Zero;
            return;
        }
        else
        {
            snd_pcm_hw_params_free(dspParams);
        }

        /* Prepare buffer */
        s_bufferLen = len;
        s_buffer = s_data;

        /* Create callback */
        if (snd_async_add_pcm_handler(ref s_dspAsync, s_dsp, &DSP_Callback, nint.Zero) >= 0)
        {
            s_bufferDone = 0;
        }
        else
        {
            /* Async callbacks not supported. Fallback on a more ugly way to detect end-of-stream */
            s_bufferDone = (uint)snd_pcm_avail(s_dsp);
            Trace.WriteLine($"WARNING: dsp_alsa: Async callbacks not supported. {(int)s_bufferDone} PCM byte available");
        }

        /* Write as much as we can to start playback */
        len = (uint)snd_pcm_writei(s_dsp, s_buffer, s_bufferLen);
        nint.Add(s_buffer, (int)len);
        s_bufferLen -= len;

        s_playing = true;
    }

    internal static byte DSP_GetStatus()
    {
        if (!s_playing) return 0;

        /* Check if have a buffer underrun. In that case we are done. */
        /* XXX -- In some weird cases the state switches to SETUP. So just
	     *  check if we are still running, and assume we are done playing in
	     *  all other cases */
        if (snd_pcm_state(s_dsp) != snd_pcm_state_t.SND_PCM_STATE_RUNNING)
        {
            Debug.Assert(s_bufferLen == 0);
            s_playing = false;
            return 0;
        }
        /* Some ALSA implementations seem to not support async, and also never
	     *  underrun, even if it runs out of samples. So we hack our way into
	     *  detecting when our sample is done playing */
        /* XXX -- For some reason it seems to never dequeue the last byte in
	     *  the buffer */
        if (s_bufferDone != 0 && snd_pcm_avail(s_dsp) == s_bufferDone - 1)
        {
            Debug.Assert(s_bufferLen == 0);
            s_playing = false;
            return 0;
        }

        return 2;
    }
}

#endif
