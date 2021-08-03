/* OS-independent inclusion of min/max/clamp */

namespace SharpDune.Os
{
    class CMath
    {
        internal static short clamp(short a, short b, short c) =>
            Math.Min(Math.Max(a, b), c);

        internal static ushort clamp(ushort a, ushort b, ushort c) =>
            Math.Min(Math.Max(a, b), c);
    }
}
