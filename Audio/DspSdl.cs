/* SDL implementation of the DSP */

namespace SharpDune.Audio;

class DspSdl
{
    static nint s_buffer;
    static uint s_bufferLen;
    static byte s_status;
    static nint s_data;
    static uint s_dataLen;

    static SDL_AudioSpec s_spec;

    internal static void DSP_Stop()
    {
        SDL_PauseAudio(1);

        s_bufferLen = 0;
        s_buffer = nint.Zero;
        s_status = 0;
    }

    /*
     * In Dune2, the frequency of the VOC files are all over the place. SDL really
     * dislikes it when we close/open the audio driver a lot. So, we convert all
     * audio to one frequency, which resolves all issues. Sadly, our knowledge of
     * audio is not really good, so this is a linear scaler.
     */
    unsafe static void DSP_ConvertAudio(uint freq)
    {
        var newlen = (uint)(s_bufferLen * s_spec.freq / freq);
        byte* r, w;
        uint i, j;

        Debug.Assert((int)freq < s_spec.freq);
        
        if (s_dataLen < newlen)
        {
            s_data = Marshal.ReAllocHGlobal(s_data, (int)newlen);
            s_dataLen = newlen;
        }

        w = (byte*)(s_data + newlen - 1);
        r = (byte*)(s_data + s_bufferLen - 1);
        j = 0;
        for (i = 0; i < s_bufferLen; i++)
        {
            do
            {
                *w-- = *r;
                j++;
            } while (j <= i * s_spec.freq / freq);
            r--;
        }
        r++;
        while (j < i * s_spec.freq / freq)
        {
            *w-- = *r;
            j++;
        }
        w++;
        
        Debug.Assert(w == (byte*)s_data);
        Debug.Assert(r == (byte*)s_data);
        
        s_bufferLen = newlen;
    }

    internal static void DSP_Play(byte[] data)
    {
        var dataPointer = 0;

        DSP_Stop();

        dataPointer += Read_LE_UInt16(data.AsSpan(20));

        if (data[dataPointer] != 1) return;

        s_bufferLen = (Read_LE_UInt32(data.AsSpan(dataPointer)) >> 8) - 2;

        if (s_data == nint.Zero)
        {
            s_data = Marshal.AllocHGlobal((int)s_bufferLen);
        }
        else if (s_dataLen < s_bufferLen)
        {
            s_data = Marshal.ReAllocHGlobal(s_data, (int)s_bufferLen);
        }
        s_dataLen = s_bufferLen;

        Marshal.Copy(data, dataPointer + 6, s_data, (int)s_bufferLen);
        DSP_ConvertAudio((uint)(1000000 / (256 - data[dataPointer + 4])));

        s_buffer = s_data;
        s_status = 2;
        SDL_PauseAudio(0);
    }

    internal static byte DSP_GetStatus() =>
        SDL_GetAudioStatus() != 0 ? s_status : (byte)0;

    internal static bool DSP_Init()
    {
        if (SDL_InitSubSystem(SDL_INIT_AUDIO) != 0) return false;

        s_spec.freq = 22050;
        s_spec.format = AUDIO_U8;
        s_spec.channels = 1;
        s_spec.samples = 512;
        s_spec.callback = DSP_Callback;

        s_bufferLen = 0;
        s_buffer = nint.Zero;
        s_status = 0;
        s_data = nint.Zero;
        s_dataLen = 0;

        if (SDL_OpenAudio(ref s_spec, out s_spec) != 0) return false;

        return SDL_GetAudioStatus() != 0;
    }

    internal static void DSP_Uninit()
    {
        if (SDL_WasInit(SDL_INIT_AUDIO) == 0) return;

        DSP_Stop();
        SDL_CloseAudio();

        Marshal.FreeHGlobal(s_data); s_data = nint.Zero;
        s_dataLen = 0;

        SDL_QuitSubSystem(SDL_INIT_AUDIO);
    }

    unsafe static void DSP_Callback(nint userdata, nint stream, int len)
    {
        if (s_status == 0 || s_bufferLen == 0 || s_buffer == nint.Zero)
        {
            /* no more sample to play : */
            NativeMemory.Fill((void*)stream, (nuint)len, 0x80); //memset(stream, 0x80, len);  /* fill buffer with silence */
            SDL_PauseAudio(1);  /* stop playback */
            return;
        }

        if (len <= (int)s_bufferLen)
        {
            NativeMemory.Copy((void*)s_buffer, (void*)stream, (nuint)len); //memcpy(stream, s_buffer, len);
            s_bufferLen -= (uint)len;
            s_buffer += len;
        }
        else
        {
            NativeMemory.Copy((void*)s_buffer, (void*)stream, s_bufferLen); //memcpy(stream, s_buffer, s_bufferLen);
            NativeMemory.Fill((void*)(stream + (int)s_bufferLen), (nuint)(len - s_bufferLen), 0x80); //memset(stream + s_bufferLen, 0x80, len - s_bufferLen);  /* fill buffer end with silence */
            s_bufferLen = 0;
            s_buffer = nint.Zero;
            s_status = 0;
        }
    }
}
