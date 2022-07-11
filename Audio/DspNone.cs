/* Linux implementation of the DSP */

namespace SharpDune.Audio;

class DspNone
{
    internal static bool DSP_Init()
    {
        return true;
    }

    internal static void DSP_Uninit()
    {
    }

    internal static byte DSP_GetStatus()
    {
        return 0;
    }

    internal static void DSP_Play(byte[] data)
    {
    }

    internal static void DSP_Stop()
    {
    }
}
