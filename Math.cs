/* OS-independent inclusion of min/max/clamp */

using static System.Math;

namespace SharpDune
{
    class CMath
    {
        internal static short clamp(short a, short b, short c) =>
            Min(Max(a, b), c);

        internal static ushort clamp(ushort a, ushort b, ushort c) =>
            Min(Max(a, b), c);
    }
}
