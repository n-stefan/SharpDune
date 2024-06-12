/* OS-independent inclusion of min/max/clamp */

namespace SharpDune.Os;

static class Math
{
    internal static short Clamp(short a, short b, short c) =>
        System.Math.Min(System.Math.Max(a, b), c);

    internal static ushort Clamp(ushort a, ushort b, ushort c) =>
        System.Math.Min(System.Math.Max(a, b), c);
}
