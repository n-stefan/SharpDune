/* Linux implementation of the DSP */

namespace SharpDune.Audio;

class DspNone
{
    internal static bool DSP_Init() => true;

    internal static void DSP_Uninit() { }

    internal static byte DSP_GetStatus() => 0;

    internal static void DSP_Play(byte[] data) { }

    internal static void DSP_Stop() { }
}
