/* Dummy implementation of the MIDI */

namespace SharpDune.Audio;

static class MidiNone
{
    internal static bool Midi_Init() => true;

    internal static void Midi_Uninit() { }

    internal static void Midi_Reset() { }

    internal static void Midi_Send(uint _) { }

    internal static ushort Midi_Send_String(byte[] _, ushort len) => len;
}
