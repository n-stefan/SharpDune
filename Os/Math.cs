/* OS-independent inclusion of min/max/clamp */

namespace SharpDune.Os
{
    class Math
    {
        internal static short clamp(short a, short b, short c) =>
            System.Math.Min(System.Math.Max(a, b), c);

        internal static ushort clamp(ushort a, ushort b, ushort c) =>
            System.Math.Min(System.Math.Max(a, b), c);
    }
}
