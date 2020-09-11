/* Basic definitions and data types */

namespace SharpDune
{
    /*
    * bits 0 to 7 are the offset in the tile.
    * bits 8 to 13 are the position on the map.
    * bits 14 and 15 are never used (or should never be used).
    */
    struct tile32
    {
        internal ushort x;
        internal ushort y;
    }
}
