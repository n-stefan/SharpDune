/* Linux implementation of the MIDI */

namespace SharpDune.Audio;

class MidiNone
{
    internal static bool Midi_Init()
    {
        return true;
    }

    internal static void Midi_Uninit()
    {
    }

    internal static void Midi_Reset()
    {
    }

    internal static void Midi_Send(uint data)
    {
    }

    internal static ushort Midi_Send_String(byte[] data, ushort len)
    {
        return len;
    }
}
