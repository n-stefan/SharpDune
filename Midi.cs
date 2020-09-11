/* Windows implementation of the MIDI. Uses midiOut functions from the Windows API, which contain a softsynth and handles all MIDI output for us. */

/*
 * Copyright (c) 2010 Atsushi Eno
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
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SharpDune
{
    class Midi
	{
		const string LibraryName = "winmm";
		const int MaxPNameLen = 32;
		const int MMSYSERR_NOERROR = 0;

		static /*HMIDIOUT*/IntPtr s_midi = IntPtr.Zero;

		[Flags]
		public enum MidiOutOpenFlags
		{
			Null,
			Function,
			Thread,
			Window,
			Event
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		struct MidiOutCaps
		{
			public short Mid;
			public short Pid;
			public int DriverVersion;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPNameLen)]
			public string Name;
			public short Technology;
			public short Voices;
			public short Notes;
			public short ChannelMask;
			public int Support;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct MidiHdr
		{
			public IntPtr Data;
			public int BufferLength;
			public int BytesRecorded;
			public IntPtr User;
			public int Flags;
			public IntPtr Next; // of MidiHdr
			public IntPtr Reserved;
			public int Offset;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			private int[] reservedArray;
		}

		delegate void MidiOutProc(IntPtr midiOut, uint msg, IntPtr instance, IntPtr param1, IntPtr param2);

		[DllImport(LibraryName)]
		static extern int midiOutGetNumDevs();

		[DllImport(LibraryName)]
		static extern int midiOutGetDevCaps(UIntPtr uDeviceID, out MidiOutCaps midiOutCaps, uint sizeOfMidiOutCaps);

		[DllImport(LibraryName)]
		static extern int midiOutOpen(out IntPtr midiIn, uint deviceID, MidiOutProc callback, IntPtr callbackInstance, MidiOutOpenFlags flags);

		[DllImport(LibraryName)]
		static extern int midiOutShortMsg(IntPtr handle, uint msg);

		[DllImport(LibraryName)]
		static extern int midiOutReset(IntPtr handle);

		[DllImport(LibraryName)]
		static extern int midiOutClose(IntPtr midiIn);

		[DllImport(LibraryName)]
		static extern int midiOutLongMsg(IntPtr handle, IntPtr midiOutHdr, int midiOutHdrSize);

		[DllImport(LibraryName)]
		static extern int midiOutPrepareHeader(IntPtr handle, IntPtr midiOutHdr, int midiOutHdrSize);

		[DllImport(LibraryName)]
		static extern int midiOutUnprepareHeader(IntPtr handle, IntPtr headerPtr, int sizeOfMidiHeader);

		internal static bool midi_init()
		{
			uint devID = 0;
			uint i;
			int numDevs = midiOutGetNumDevs();
			if (IniFile.IniFile_GetInteger("mt32midi", 0) != 0)
			{
				for (i = 0; i < numDevs; i++)
				{
					if (midiOutGetDevCaps((UIntPtr)i, out MidiOutCaps caps, (uint)Marshal.SizeOf<MidiOutCaps>()) == MMSYSERR_NOERROR)
					{
						Debug.WriteLine($"DEBUG: MidiOutdevice #{i}: {caps.Mid}:{caps.Pid} v{caps.DriverVersion >> 8}.{caps.DriverVersion & 0xff}" +
										  $" voices={caps.Voices} notes={caps.Notes} channels={caps.ChannelMask} {caps.Name}");
						/* select this device if its description contains "MT-32" */
						if (caps.Name.Contains("MT-32")) devID = i;
					}
				}
			}

			/* No callback to process messages related to the progress of the playback */
			if (midiOutOpen(out s_midi, devID, null, IntPtr.Zero, MidiOutOpenFlags.Null) != MMSYSERR_NOERROR)
			{
				Trace.WriteLine($"ERROR: Failed to initialize MIDI (Device ID = {devID})");
				s_midi = IntPtr.Zero;
				return false;
			}

			return true;
		}

		internal static void midi_uninit()
		{
			if (s_midi == IntPtr.Zero) return;

			midiOutReset(s_midi);
			midiOutClose(s_midi);

			s_midi = IntPtr.Zero;
		}

		internal static void midi_send(uint data)
		{
			if (s_midi == IntPtr.Zero) return;

			midiOutShortMsg(s_midi, data);
		}

		internal static void midi_reset()
		{
			if (s_midi == IntPtr.Zero) return;

			midiOutReset(s_midi);
		}

		internal static ushort midi_send_string(byte[] data, ushort len)
		{
			if (s_midi == IntPtr.Zero) return len;

			var header = new MidiHdr();
			var hdrSize = Marshal.SizeOf(typeof(MidiHdr));
			var prepared = false;
			var ptr = IntPtr.Zero;

			try
			{
				header.Data = Marshal.AllocHGlobal(len);
				header.BufferLength = len;
				Marshal.Copy(data, 0, header.Data, header.BufferLength); //TODO: Use Unsafe.CopyBlock?
				ptr = Marshal.AllocHGlobal(hdrSize);
				Marshal.StructureToPtr(header, ptr, false);

				if (midiOutPrepareHeader(s_midi, ptr, hdrSize) != MMSYSERR_NOERROR)
					Trace.WriteLine("ERROR: midiOutPrepareHeader() failed");
				else
				{
					prepared = true;
					Debug.Assert(midiOutLongMsg(s_midi, ptr, hdrSize) == MMSYSERR_NOERROR);
				}
			}
			finally
			{
				if (prepared)
					if (midiOutUnprepareHeader(s_midi, ptr, hdrSize) != MMSYSERR_NOERROR)
						Trace.WriteLine("ERROR: midiOutUnprepareHeader() failed");

				if (header.Data != IntPtr.Zero)
					Marshal.FreeHGlobal(header.Data);

				if (ptr != IntPtr.Zero)
					Marshal.FreeHGlobal(ptr);
			}
			
			return len;
		}
	}
}
