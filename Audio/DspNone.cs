﻿/* Dummy implementation of the DSP */

namespace SharpDune.Audio;

static class DspNone
{
    internal static bool DSP_Init() => true;

    internal static void DSP_Uninit() { }

    internal static byte DSP_GetStatus() => 0;

    internal static void DSP_Play(byte[] _) { }

    internal static void DSP_Stop() { }
}
