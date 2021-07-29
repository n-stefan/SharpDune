/* Tile */

using SharpDune.Include;
using static System.Math;

namespace SharpDune
{
    class CTile
    {
        static readonly sbyte[] _stepX = {
               0,    3,    6,    9,   12,   15,   18,   21,   24,   27,   30,   33,   36,   39,   42,   45,
              48,   51,   54,   57,   59,   62,   65,   67,   70,   73,   75,   78,   80,   82,   85,   87,
              89,   91,   94,   96,   98,  100,  101,  103,  105,  107,  108,  110,  111,  113,  114,  116,
             117,  118,  119,  120,  121,  122,  123,  123,  124,  125,  125,  126,  126,  126,  126,  126,
             127,  126,  126,  126,  126,  126,  125,  125,  124,  123,  123,  122,  121,  120,  119,  118,
             117,  116,  114,  113,  112,  110,  108,  107,  105,  103,  102,  100,   98,   96,   94,   91,
              89,   87,   85,   82,   80,   78,   75,   73,   70,   67,   65,   62,   59,   57,   54,   51,
              48,   45,   42,   39,   36,   33,   30,   27,   24,   21,   18,   15,   12,    9,    6,    3,
               0,   -3,   -6,   -9,  -12,  -15,  -18,  -21,  -24,  -27,  -30,  -33,  -36,  -39,  -42,  -45,
             -48,  -51,  -54,  -57,  -59,  -62,  -65,  -67,  -70,  -73,  -75,  -78,  -80,  -82,  -85,  -87,
             -89,  -91,  -94,  -96,  -98, -100, -102, -103, -105, -107, -108, -110, -111, -113, -114, -116,
            -117, -118, -119, -120, -121, -122, -123, -123, -124, -125, -125, -126, -126, -126, -126, -126,
            -126, -126, -126, -126, -126, -126, -125, -125, -124, -123, -123, -122, -121, -120, -119, -118,
            -117, -116, -114, -113, -112, -110, -108, -107, -105, -103, -102, -100,  -98,  -96,  -94,  -91,
             -89,  -87,  -85,  -82,  -80,  -78,  -75,  -73,  -70,  -67,  -65,  -62,  -59,  -57,  -54,  -51,
             -48,  -45,  -42,  -39,  -36,  -33,  -30,  -27,  -24,  -21,  -18,  -15,  -12,   -9,   -6,   -3
        };

        static readonly sbyte[] _stepY = {
             127,  126,  126,  126,  126,  126,  125,  125,  124,  123,  123,  122,  121,  120,  119,  118,
             117,  116,  114,  113,  112,  110,  108,  107,  105,  103,  102,  100,   98,   96,   94,   91,
              89,   87,   85,   82,   80,   78,   75,   73,   70,   67,   65,   62,   59,   57,   54,   51,
              48,   45,   42,   39,   36,   33,   30,   27,   24,   21,   18,   15,   12,    9,    6,    3,
               0,   -3,   -6,   -9,  -12,  -15,  -18,  -21,  -24,  -27,  -30,  -33,  -36,  -39,  -42,  -45,
             -48,  -51,  -54,  -57,  -59,  -62,  -65,  -67,  -70,  -73,  -75,  -78,  -80,  -82,  -85,  -87,
             -89,  -91,  -94,  -96,  -98, -100, -102, -103, -105, -107, -108, -110, -111, -113, -114, -116,
            -117, -118, -119, -120, -121, -122, -123, -123, -124, -125, -125, -126, -126, -126, -126, -126,
            -126, -126, -126, -126, -126, -126, -125, -125, -124, -123, -123, -122, -121, -120, -119, -118,
            -117, -116, -114, -113, -112, -110, -108, -107, -105, -103, -102, -100,  -98,  -96,  -94,  -91,
             -89,  -87,  -85,  -82,  -80,  -78,  -75,  -73,  -70,  -67,  -65,  -62,  -59,  -57,  -54,  -51,
             -48,  -45,  -42,  -39,  -36,  -33,  -30,  -27,  -24,  -21,  -18,  -15,  -12,   -9,   -6,   -3,
               0,    3,    6,    9,   12,   15,   18,   21,   24,   27,   30,   33,   36,   39,   42,   45,
              48,   51,   54,   57,   59,   62,   65,   67,   70,   73,   75,   78,   80,   82,   85,   87,
              89,   91,   94,   96,   98,  100,  101,  103,  105,  107,  108,  110,  111,  113,  114,  116,
             117,  118,  119,  120,  121,  122,  123,  123,  124,  125,  125,  126,  126,  126,  126,  126
        };

        /*
         * Check if a packed tile is out of map. Useful after additional or substraction.
         * @param packed The packed tile to check.
         * @return True if and only if the tile is out of map.
         */
        /*extern bool Tile_IsOutOfMap(uint16 packed);*/
        internal static bool Tile_IsOutOfMap(ushort packed) => (packed & 0xF000) != 0;

        /*
         * Unpacks a 12 bits packed tile and retrieves the X-position.
         *
         * @param packed The uint16 containing the 12 bits packed tile.
         * @return The unpacked X-position.
         */
        /*extern uint8 Tile_GetPackedX(uint16 packed);*/
        internal static ushort Tile_GetPackedX(ushort packed) => (ushort)(packed & 0x3F);

        /*
         * Unpacks a 12 bits packed tile and retrieves the Y-position.
         *
         * @param packed The uint16 containing the 12 bits packed tile.
         * @return The unpacked Y-position.
         */
        /*extern uint8 Tile_GetPackedY(uint16 packed);*/
        internal static ushort Tile_GetPackedY(ushort packed) => (ushort)((packed >> 6) & 0x3F);

        /*
         * Returns the X-position of the tile.
         *
         * @param tile The tile32 to get the X-position from.
         * @return The X-position of the tile.
         */
        /*extern uint8 Tile_GetPosX(tile32 tile);*/
        internal static byte Tile_GetPosX(tile32 tile) => (byte)((tile.x >> 8) & 0x3f);

        /*
         * Returns the Y-position of the tile.
         *
         * @param tile The tile32 to get the Y-position from.
         * @return The Y-position of the tile.
         */
        /*extern uint8 Tile_GetPosY(tile32 tile);*/
        internal static byte Tile_GetPosY(tile32 tile) => (byte)((tile.y >> 8) & 0x3f);

        /*
         * Packs a 32 bits tile class into a 12 bits packed tile.
         *
         * @param tile The tile32 to get it's Y-position from.
         * @return The tile packed into 12 bits.
         */
        /*extern uint16 Tile_PackTile(tile32 tile);*/
        internal static ushort Tile_PackTile(tile32 tile) => (ushort)((Tile_GetPosY(tile) << 6) | Tile_GetPosX(tile));

        /*
         * Packs an x and y coordinate into a 12 bits packed tile.
         *
         * @param x The X-coordinate from.
         * @param x The Y-coordinate from.
         * @return The coordinates packed into 12 bits.
         */
        /*extern uint16 Tile_PackXY(uint16 x, uint16 y);*/
        internal static ushort Tile_PackXY(ushort x, ushort y) => (ushort)((y << 6) | x);

        /*
         * Check whether a tile is valid.
         *
         * @param tile The tile32 to check for validity.
         * @return True if valid, false if not.
         */
        /*extern bool Tile_IsValid(tile32 tile);*/
        /*#define Tile_IsValid(tile) (((tile).x & 0xc000) == 0 && ((tile).y & 0xc000) == 0)*/
        internal static bool Tile_IsValid(tile32 tile) => ((tile.x | tile.y) & 0xc000) == 0;

        /*
         * Adds two tiles together.
         *
         * @param from The origin.
         * @param diff The difference.
         * @return The new coordinates.
         */
        internal static tile32 Tile_AddTileDiff(tile32 from, tile32 diff)
        {
            var result = new tile32
            {
                x = (ushort)(from.x + diff.x),
                y = (ushort)(from.y + diff.y)
            };

            return result;
        }

        /*
         * Remove fog in the radius around the given tile.
         *
         * @param tile The tile to remove fog around.
         * @param radius The radius to remove fog around.
         */
        internal static void Tile_RemoveFogInRadius(tile32 tile, ushort radius)
        {
            ushort packed;
            ushort x, y;
            short i, j;

            /* TODO this code could be simplified */
            packed = Tile_PackTile(tile);

            if (!Map.Map_IsValidPosition(packed)) return;

            /* setting tile from its packed position equals removing the
	         * non integer part */
            x = Tile_GetPackedX(packed);
            y = Tile_GetPackedY(packed);
            Tile_MakeXY(ref tile, x, y);

            for (i = (short)-radius; i <= radius; i++)
            {
                for (j = (short)-radius; j <= radius; j++)
                {
                    var t = new tile32();

                    if ((x + i) < 0 || (x + i) >= 64) continue;
                    if ((y + j) < 0 || (y + j) >= 64) continue;

                    var xi = (ushort)(x + i);
                    var yj = (ushort)(y + j);

                    packed = Tile_PackXY(xi, yj);
                    Tile_MakeXY(ref t, xi, yj);

                    if (Tile_GetDistanceRoundedUp(tile, t) > radius) continue;

                    Map.Map_UnveilTile(packed, (byte)CHouse.g_playerHouseID);
                }
            }
        }

        /*
         * Make a tile32 from an X- and Y-position.
         *
         * @param x The X-position.
         * @param y The Y-position.
         * @return A tile32 at the top-left corner of the X- and Y-position.
         */
        /*extern tile32 Tile_MakeXY(uint16 x, uint16 y);*/
        internal static void/*tile32*/ Tile_MakeXY(ref tile32 tile, ushort X, ushort Y)
        {
            tile.x = (ushort)(X << 8);
            tile.y = (ushort)(Y << 8);
            //return tile;
        }

        /*
         * Calculates the rounded up distance between the two given packed tiles.
         *
         * @param from The origin.
         * @param to The destination.
         * @return The longest distance between the X or Y coordinates, plus half the shortest.
         */
        internal static ushort Tile_GetDistanceRoundedUp(tile32 from, tile32 to) => (ushort)((Tile_GetDistance(from, to) + 0x80) >> 8);

        /*
         * Calculates the distance between the two given tiles.
         *
         * @param from The origin.
         * @param to The destination.
         * @return The longest distance between the X or Y coordinates, plus half the shortest.
         */
        internal static ushort Tile_GetDistance(tile32 from, tile32 to)
        {
            var distance_x = (ushort)Abs(from.x - to.x);
            var distance_y = (ushort)Abs(from.y - to.y);

            if (distance_x > distance_y) return (ushort)(distance_x + (distance_y / 2));
            return (ushort)(distance_y + (distance_x / 2));
        }

        static readonly ushort[] orientationOffsets = { 0x40, 0x80, 0x0, 0xC0 };
        static readonly int[] directions = {
            0x3FFF, 0x28BC, 0x145A, 0xD8E,  0xA27, 0x81B, 0x6BD, 0x5C3,  0x506, 0x474, 0x3FE, 0x39D,  0x34B, 0x306, 0x2CB, 0x297,
            0x26A,  0x241,  0x21D,  0x1FC,  0x1DE, 0x1C3, 0x1AB, 0x194,  0x17F, 0x16B, 0x159, 0x148,  0x137, 0x128, 0x11A, 0x10C
        };
        /*
         * Get to direction to follow to go from \a from to \a to.
         *
         * @param from The origin.
         * @param to The destination.
         * @return The direction.
         */
        internal static sbyte Tile_GetDirection(tile32 from, tile32 to)
        {
            int dx;
            int dy;
            ushort i;
            int gradient;
            ushort baseOrientation;
            bool invert;
            ushort quadrant = 0;

            dx = to.x - from.x;
            dy = to.y - from.y;

            if (Abs(dx) + Abs(dy) > 8000)
            {
                dx /= 2;
                dy /= 2;
            }

            if (dy <= 0)
            {
                quadrant |= 0x2;
                dy = -dy;
            }

            if (dx < 0)
            {
                quadrant |= 0x1;
                dx = -dx;
            }

            baseOrientation = orientationOffsets[quadrant];
            invert = false;
            gradient = 0x7FFF;

            if (dx >= dy)
            {
                if (dy != 0) gradient = (dx << 8) / dy;
            }
            else
            {
                invert = true;
                if (dx != 0) gradient = (dy << 8) / dx;
            }

            for (i = 0; i < directions.Length /*lengthof(directions)*/; i++)
            {
                if (directions[i] <= gradient) break;
            }

            if (!invert) i = (ushort)(64 - i);

            if (quadrant == 0 || quadrant == 3) return (sbyte)((baseOrientation + 64 - i) & 0xFF);

            return (sbyte)((baseOrientation + i) & 0xFF);
        }

        /*
         * Convert an orientation that goes from 0 .. 255 to one that goes from 0 .. 7.
         * @param orientation The 256-based orientation.
         * @return A 8-based orientation.
         */
        internal static byte Orientation_Orientation256ToOrientation8(byte orientation) => (byte)(((orientation + 16) / 32) & 0x7);

        /*
         * Convert an orientation that goes from 0 .. 255 to one that goes from 0 .. 15.
         * @param orientation The 256-based orientation.
         * @return A 16-based orientation.
         */
        internal static byte Orientation_Orientation256ToOrientation16(byte orientation) => (byte)(((orientation + 8) / 16) & 0xF);

        /*
         * Unpacks a 12 bits packed tile to a 32 bits tile class.
         *
         * @param packed The uint16 containing the 12 bits packed tile information.
         * @return The unpacked tile.
         */
        internal static tile32 Tile_UnpackTile(ushort packed)
        {
            var tile = new tile32
            {
                x = (ushort)((((packed >> 0) & 0x3F) << 8) | 0x80),
                y = (ushort)((((packed >> 6) & 0x3F) << 8) | 0x80)
            };

            return tile;
        }

        /*
         * Calculates the distance between the two given packed tiles.
         *
         * @param packed_from The origin.
         * @param packed_to The destination.
         * @return The longest distance between the X or Y coordinates, plus half the shortest.
         */
        internal static ushort Tile_GetDistancePacked(ushort packed_from, ushort packed_to)
        {
            var from = Tile_UnpackTile(packed_from);
            var to = Tile_UnpackTile(packed_to);

            return (ushort)(Tile_GetDistance(from, to) >> 8);
        }

        /*
         * Returns the X-position of the tile.
         *
         * @param tile The tile32 to get the X-position from.
         * @return The X-position of the tile.
         */
        /*extern uint16 Tile_GetX(tile32 tile);*/
        internal static ushort Tile_GetX(tile32 tile) => tile.x;

        /*
         * Returns the Y-position of the tile.
         *
         * @param tile The tile32 to get the Y-position from.
         * @return The Y-position of the tile.
         */
        /*extern uint16 Tile_GetY(tile32 tile);*/
        internal static ushort Tile_GetY(tile32 tile) => tile.y;

        /*
         * Centers the offset of the tile.
         *
         * @param tile The tile to center.
         */
        internal static tile32 Tile_Center(tile32 tile)
        {
            tile32 result;

            result = tile;
            result.x = (ushort)((result.x & 0xff00) | 0x80);
            result.y = (ushort)((result.y & 0xff00) | 0x80);

            return result;
        }

        /*
         * Get the tile from given tile at given maximum distance in random direction.
         *
         * @param tile The origin.
         * @param distance The distance maximum.
         * @param center Wether to center the offset of the tile.
         * @return The tile.
         */
        internal static tile32 Tile_MoveByRandom(tile32 tile, ushort distance, bool center)
        {
            ushort x;
            ushort y;
            var ret = new tile32();
            byte orientation;
            ushort newDistance;

            if (distance == 0) return tile;

            x = Tile_GetX(tile);
            y = Tile_GetY(tile);

            newDistance = Tools.Tools_Random_256();
            while (newDistance > distance) newDistance /= 2;
            distance = newDistance;

            orientation = Tools.Tools_Random_256();
            x += (ushort)(((_stepX[orientation] * distance) / 128) * 16);
            y -= (ushort)(((_stepY[orientation] * distance) / 128) * 16);

            if (x > 16384 || y > 16384) return tile;

            ret.x = x;
            ret.y = y;

            return center ? Tile_Center(ret) : ret;
        }

        /*
         * Get the tile from given tile at given distance in given direction.
         *
         * @param tile The origin.
         * @param orientation The direction to follow.
         * @param distance The distance.
         * @return The tile.
         */
        internal static tile32 Tile_MoveByDirection(tile32 tile, short orientation, ushort distance)
        {
            int diffX, diffY;
            int roundingOffsetX, roundingOffsetY;

            distance = Min(distance, (ushort)0xFF);

            if (distance == 0) return tile;

            diffX = _stepX[orientation & 0xFF];
            diffY = _stepY[orientation & 0xFF];

            /* Always round away from zero */
            roundingOffsetX = diffX < 0 ? -64 : 64;
            roundingOffsetY = diffY < 0 ? -64 : 64;

            tile.x += (ushort)((diffX * distance + roundingOffsetX) / 128);
            tile.y -= (ushort)((diffY * distance + roundingOffsetY) / 128);

            return tile;
        }

        /*
         * Get a tile in the direction of a destination, randomized a bit.
         *
         * @param packed_from The origin.
         * @param packed_to The destination.
         * @return A packed tile.
         */
        internal static ushort Tile_GetTileInDirectionOf(ushort packed_from, ushort packed_to)
        {
            short distance;
            byte direction;

            if (packed_from == 0 || packed_to == 0) return 0;

            distance = (short)Tile_GetDistancePacked(packed_from, packed_to);
            direction = Tile_GetDirectionPacked(packed_to, packed_from);

            if (distance <= 10) return 0;

            while (true)
            {
                short dir;
                tile32 position;
                ushort packed;

                dir = (short)(31 + (Tools.Tools_Random_256() & 0x3F));

                if ((Tools.Tools_Random_256() & 1) != 0) dir = (short)-dir;

                position = Tile_UnpackTile(packed_to);
                position = Tile_MoveByDirection(position, (short)(direction + dir), (ushort)(Min(distance, (short)20) << 8));
                packed = Tile_PackTile(position);

                if (Map.Map_IsValidPosition(packed)) return packed;
            }

            //return 0;
        }

        static readonly byte[] returnValues = { 0x20, 0x40, 0x20, 0x00, 0xE0, 0xC0, 0xE0, 0x00, 0x60, 0x40, 0x60, 0x80, 0xA0, 0xC0, 0xA0, 0x80 };
        /*
         * Get to direction to follow to go from packed_from to packed_to.
         *
         * @param packed_from The origin.
         * @param packed_to The destination.
         * @return The direction.
         */
        internal static byte Tile_GetDirectionPacked(ushort packed_from, ushort packed_to)
        {
            short x1, y1, x2, y2;
            short dx, dy;
            ushort index;

            x1 = (short)Tile_GetPackedX(packed_from);
            y1 = (short)Tile_GetPackedY(packed_from);
            x2 = (short)Tile_GetPackedX(packed_to);
            y2 = (short)Tile_GetPackedY(packed_to);

            index = 0;

            dy = (short)(y1 - y2);
            if (dy < 0)
            {
                index |= 0x8;
                dy = (short)-dy;
            }

            dx = (short)(x2 - x1);
            if (dx < 0)
            {
                index |= 0x4;
                dx = (short)-dx;
            }

            if (dx >= dy)
            {
                if (((dx + 1) / 2) > dy) index |= 0x1;
            }
            else
            {
                index |= 0x2;
                if (((dy + 1) / 2) > dx) index |= 0x1;
            }

            return returnValues[index];
        }

        /*
         * Move to the given orientation looking from the current position.
         * @note returns input position when going out-of-bounds.
         * @param position The position to move from.
         * @param orientation The orientation to move in.
         * @return The new position, or the old in case of out-of-bounds.
         */
        internal static tile32 Tile_MoveByOrientation(tile32 position, byte orientation)
        {
            short[] xOffsets = { 0, 256, 256, 256, 0, -256, -256, -256 }; //[8]
            short[] yOffsets = { -256, -256, 0, 256, 256, 256, 0, -256 }; //[8]
            ushort x;
            ushort y;

            x = Tile_GetX(position);
            y = Tile_GetY(position);

            orientation = Orientation_Orientation256ToOrientation8(orientation);

            x += (ushort)xOffsets[orientation];
            y += (ushort)yOffsets[orientation];

            if (x > 16384 || y > 16384) return position;

            position.x = x;
            position.y = y;

            return position;
        }
    }
}
