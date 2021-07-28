/* SDL implementation of the DSP */

using SDL2;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Vanara.Extensions;

namespace SharpDune.Audio
{
	class DspSdl
	{
		static /*byte[]*/CArray<byte> s_buffer;
		static uint s_bufferLen;
		static byte s_status;
		static byte[] s_data;
		static uint s_dataLen;

		static SDL.SDL_AudioSpec s_spec;

		internal static void DSP_Stop()
		{
			SDL.SDL_PauseAudio(1);

			s_bufferLen = 0;
			s_buffer = null;
			s_status = 0;
		}

		/*
		 * In Dune2, the frequency of the VOC files are all over the place. SDL really
		 * dislikes it when we close/open the audio driver a lot. So, we convert all
		 * audio to one frequency, which resolves all issues. Sadly, our knowledge of
		 * audio is not really good, so this is a linear scaler.
		 */
		static void DSP_ConvertAudio(uint freq)
		{
			uint newlen = (uint)(s_bufferLen * s_spec.freq / freq);
			uint r;
			uint w;
			uint i, j;

			Debug.Assert((int)freq < s_spec.freq);

			if (s_dataLen < newlen)
			{
				Array.Resize(ref s_data, (int)newlen); //s_data = realloc(s_data, newlen);
				s_dataLen = newlen;
			}

			w = newlen - 1;
			r = s_bufferLen - 1;
			j = 0;
			for (i = 0; i < s_bufferLen; i++)
			{
				do
				{
					s_data[w--] = s_data[r];
					j++;
				} while (j <= i * s_spec.freq / freq);
				r--;
			}
			r++;
			while (j < i * s_spec.freq / freq)
			{
				s_data[w--] = s_data[r];
				j++;
			}
			w++;

			Debug.Assert(w == 0);
			Debug.Assert(r == 0);

			s_bufferLen = newlen;
		}

		internal static void DSP_Play(/* uint8 * */byte[] data)
		{
			int dataPointer = 0;

			DSP_Stop();

			dataPointer += Endian.READ_LE_UINT16(data[20..]);

			if (data[dataPointer] != 1) return;

			s_bufferLen = (Endian.READ_LE_UINT32(data[dataPointer..]) >> 8) - 2;

			if (s_dataLen < s_bufferLen)
			{
				Array.Resize(ref s_data, (int)s_bufferLen); //realloc(s_data, s_bufferLen);
				s_dataLen = s_bufferLen;
			}

			Array.Copy(data, dataPointer + 6, s_data, 0, s_bufferLen); //memcpy(s_data, data + 6, s_bufferLen);

			DSP_ConvertAudio((uint)(1000000 / (256 - data[dataPointer + 4])));

			s_buffer = new CArray<byte> { Arr = s_data };
			s_status = 2;
			SDL.SDL_PauseAudio(0);
		}

		internal static byte DSP_GetStatus() =>
			(SDL.SDL_GetAudioStatus() != 0) ? s_status : (byte)0;

		internal static bool DSP_Init()
		{
			if (SDL.SDL_InitSubSystem(SDL.SDL_INIT_AUDIO) != 0) return false;

			s_spec.freq = 22050;
			s_spec.format = SDL.AUDIO_U8;
			s_spec.channels = 1;
			s_spec.samples = 512;
			s_spec.callback = DSP_Callback;

			s_bufferLen = 0;
			s_buffer = null;
			s_status = 0;
			s_data = null;
			s_dataLen = 0;

			if (SDL.SDL_OpenAudio(ref s_spec, out s_spec) != 0) return false;

			return SDL.SDL_GetAudioStatus() != 0;
		}

		internal static void DSP_Uninit()
		{
			if (SDL.SDL_WasInit(SDL.SDL_INIT_AUDIO) == 0) return;

			DSP_Stop();
			SDL.SDL_CloseAudio();

			s_data = null;
			s_dataLen = 0;

			SDL.SDL_QuitSubSystem(SDL.SDL_INIT_AUDIO);
		}

		static void DSP_Callback(IntPtr userdata, IntPtr stream, int len)
		{
			if (s_status == 0 || s_bufferLen == 0 || s_buffer == null)
			{
				/* no more sample to play : */
				stream.FillMemory(0x80, len); //memset(stream, 0x80, len);  /* fill buffer with silence */
				SDL.SDL_PauseAudio(1);  /* stop playback */
				return;
			}

			if (len <= (int)s_bufferLen)
			{
				Marshal.Copy(s_buffer.Arr, s_buffer.Ptr, stream, len); //memcpy(stream, s_buffer, len);
				s_bufferLen -= (uint)len;
				s_buffer += len;
			}
			else
			{
				Marshal.Copy(s_buffer.Arr, s_buffer.Ptr, stream, (int)s_bufferLen); //memcpy(stream, s_buffer, s_bufferLen);
				stream.Offset(s_bufferLen).FillMemory(0x80, len - s_bufferLen); //memset(stream + s_bufferLen, 0x80, len - s_bufferLen);  /* fill buffer end with silence */
				s_bufferLen = 0;
				s_buffer = null;
				s_status = 0;
			}
		}
	}
}
