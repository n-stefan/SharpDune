﻿/* Various routines */

namespace SharpDune;

/*
 * Types of encoded Index.
 */
enum IndexType
{
    IT_NONE = 0,
    IT_TILE = 1,
    IT_UNIT = 2,
    IT_STRUCTURE = 3
}

static class Tools
{
    static readonly byte[] s_randomSeed = new byte[4];
    static uint s_randomLCG;

    /*
     * Test a bit in a bit array.
     * @param array Bit array.
     * @param index Index in the array.
     * @return value of the bit.
     */
    internal static bool BitArray_Test(byte[] array, ushort index)
    {
        var normalizedIndex = index >> 3;
        if (normalizedIndex >= array.Length) return false;
        return (array[normalizedIndex] & (1 << (index & 7))) != 0;
    }

    /*
     * Set a bit in a bit array.
     * @param array Bit array.
     * @param index Index in the array.
     */
    internal static void BitArray_Set(byte[] array, ushort index)
    {
        var normalizedIndex = index >> 3;
        if (normalizedIndex >= array.Length) return;
        array[normalizedIndex] |= (byte)(1 << (index & 7));
    }

    /*
     * Clear a bit in a bit array.
     * @param array Bit array.
     * @param index Index in the array.
     */
    internal static void BitArray_Clear(byte[] array, ushort index)
    {
        var normalizedIndex = index >> 3;
        if (normalizedIndex >= array.Length) return;
        array[normalizedIndex] &= (byte)~(1 << (index & 7));
    }

    /*
     * Get the type of the given encoded index.
     *
     * @param id The encoded index to get the type of.
     * @return The type
     */
    internal static IndexType Tools_Index_GetType(ushort encoded) =>
    (encoded & 0xC000) switch
    {
        0x4000 => IndexType.IT_UNIT,
        0x8000 => IndexType.IT_STRUCTURE,
        0xC000 => IndexType.IT_TILE,
        _ => IndexType.IT_NONE,
    };

    /*
     * Decode the given encoded index.
     *
     * @param id The encoded index to decode.
     * @return The decoded index.
     */
    internal static ushort Tools_Index_Decode(ushort encoded)
    {
        if (Tools_Index_GetType(encoded) == IndexType.IT_TILE) return Tile_PackXY((ushort)((encoded >> 1) & 0x3F), (ushort)((encoded >> 8) & 0x3F));
        return (ushort)(encoded & 0x3FFF);
    }

    /*
     * Gets the Structure corresponding to the given encoded index.
     *
     * @param id The encoded index to get the Structure of.
     * @return The Structure.
     */
    internal static CStructure Tools_Index_GetStructure(ushort encoded)
    {
        ushort index;

        if (Tools_Index_GetType(encoded) != IndexType.IT_STRUCTURE) return null;

        index = Tools_Index_Decode(encoded);
        return (index < (ushort)StructureIndex.STRUCTURE_INDEX_MAX_HARD) ? Structure_Get_ByIndex(index) : null;
    }

    /*
     * Get a random value from the LCG.
     */
    static short Tools_RandomLCG()
    {
        /* Borland C/C++ 'a' and 'b' value, bits 30. . .16, as used by Dune2 */
        s_randomLCG = (0x015A4E35 * s_randomLCG) + 1;
        return (short)((s_randomLCG >> 16) & 0x7FFF);
    }

    /*
     * Get a random value between the given values.
     *
     * @param min The minimum value.
     * @param max The maximum value.
     * @return The random value.
     */
    internal static ushort Tools_RandomLCG_Range(ushort min, ushort max)
    {
        ushort ret;

        if (min > max)
        {
            (max, min) = (min, max);
        }

        do
        {
            var value = (ushort)((Tools_RandomLCG() * (max - min + 1) / 0x8000) + min);
            ret = value;
        } while (ret > max);

        return ret;
    }

    /*
     * Get a random value between 0 and 255.
     *
     * @return The random value.
     */
    internal static byte Tools_Random_256()
    {
        ushort val16;
        byte val8;

        val16 = (ushort)((s_randomSeed[1] << 8) | s_randomSeed[2]);
        val8 = (byte)(((val16 ^ 0x8000) >> 15) & 1);
        val16 = (ushort)((val16 << 1) | ((s_randomSeed[0] >> 1) & 1));
        val8 = (byte)((s_randomSeed[0] >> 2) - s_randomSeed[0] - val8);
        s_randomSeed[0] = (byte)((val8 << 7) | (s_randomSeed[0] >> 1));
        s_randomSeed[1] = (byte)(val16 >> 8);
        s_randomSeed[2] = (byte)(val16 & 0xFF);

        return (byte)(s_randomSeed[0] ^ s_randomSeed[1]);
    }

    /*
     * Gets the Object corresponding to the given encoded index.
     *
     * @param id The encoded index to get the Object of.
     * @return The Object.
     */
    internal static CObject Tools_Index_GetObject(ushort encoded)
    {
        ushort index;

        switch (Tools_Index_GetType(encoded))
        {
            case IndexType.IT_UNIT:
                index = Tools_Index_Decode(encoded);
                return (index < (ushort)UnitIndex.UNIT_INDEX_MAX) ? Unit_Get_ByIndex(index).o : null;

            case IndexType.IT_STRUCTURE:
                index = Tools_Index_Decode(encoded);
                return (index < (ushort)StructureIndex.STRUCTURE_INDEX_MAX_HARD) ? Structure_Get_ByIndex(index).o : null;

            default: return null;
        }
    }

    /*
     * Encode the given index.
     *
     * @param id The index to encode.
     * @param type The type of the encoded Index.
     * @return The encoded Index.
     */
    internal static ushort Tools_Index_Encode(ushort index, IndexType type)
    {
        switch (type)
        {
            case IndexType.IT_TILE:
                {
                    var ret = (ushort)(((Tile_GetPackedX(index) << 1) + 1) << 0);
                    ret |= (ushort)(((Tile_GetPackedY(index) << 1) + 1) << 7);
                    return (ushort)(ret | 0xC000);
                }
            case IndexType.IT_UNIT:
                {
                    if (index >= (ushort)UnitIndex.UNIT_INDEX_MAX || !Unit_Get_ByIndex(index).o.flags.allocated) return 0;
                    return (ushort)(index | 0x4000);
                }
            case IndexType.IT_STRUCTURE: return (ushort)(index | 0x8000);
            default: return 0;
        }
    }

    /*
     * Gets the Unit corresponding to the given encoded index.
     *
     * @param id The encoded index to get the Unit of.
     * @return The Unit.
     */
    internal static CUnit Tools_Index_GetUnit(ushort encoded)
    {
        ushort index;

        if (Tools_Index_GetType(encoded) != IndexType.IT_UNIT) return null;

        index = Tools_Index_Decode(encoded);
        return (index < (ushort)UnitIndex.UNIT_INDEX_MAX) ? Unit_Get_ByIndex(index) : null;
    }

    /*
     * Gets the packed tile corresponding to the given encoded index.
     *
     * @param id The encoded index to get the packed tile of.
     * @return The packed tile.
     */
    internal static ushort Tools_Index_GetPackedTile(ushort encoded)
    {
        var index = Tools_Index_Decode(encoded);

        return Tools_Index_GetType(encoded) switch
        {
            IndexType.IT_TILE => index,
            IndexType.IT_UNIT => (ushort)((index < (ushort)UnitIndex.UNIT_INDEX_MAX) ? Tile_PackTile(Unit_Get_ByIndex(index).o.position) : 0),
            IndexType.IT_STRUCTURE => (ushort)((index < (ushort)StructureIndex.STRUCTURE_INDEX_MAX_HARD) ? Tile_PackTile(Structure_Get_ByIndex(index).o.position) : 0),
            _ => 0,
        };
    }

    /*
     * Check whether an encoded index is valid.
     *
     * @param id The encoded index to check for validity.
     * @return True if valid, false if not.
     */
    internal static bool Tools_Index_IsValid(ushort encoded)
    {
        ushort index;

        if (encoded == 0) return false;

        index = Tools_Index_Decode(encoded);

        switch (Tools_Index_GetType(encoded))
        {
            case IndexType.IT_UNIT:
                if (index >= (ushort)UnitIndex.UNIT_INDEX_MAX) return false;
                return Unit_Get_ByIndex(index).o.flags.used && Unit_Get_ByIndex(index).o.flags.allocated;

            case IndexType.IT_STRUCTURE:
                if (index >= (ushort)StructureIndex.STRUCTURE_INDEX_MAX_HARD) return false;
                return Structure_Get_ByIndex(index).o.flags.used;

            case IndexType.IT_TILE: return true;

            default: return false;
        }
    }

    /*
     * Set the seed for the LCG randomizer.
     */
    internal static void Tools_RandomLCG_Seed(ushort seed) =>
        s_randomLCG = seed;

    /*
     * Set the seed for the Tools_Random_256().
     * @param seed The seed to set the randomizer to.
     */
    internal static void Tools_Random_Seed(uint seed)
    {
        s_randomSeed[0] = (byte)((seed >> 0) & 0xFF);
        s_randomSeed[1] = (byte)((seed >> 8) & 0xFF);
        s_randomSeed[2] = (byte)((seed >> 16) & 0xFF);
        s_randomSeed[3] = (byte)((seed >> 24) & 0xFF);
    }

    internal static ushort Tools_AdjustToGameSpeed(ushort normal, ushort minimum, ushort maximum, bool inverseSpeed)
    {
        var gameSpeed = g_gameConfig.gameSpeed;

        if (gameSpeed == 2) return normal;
        if (gameSpeed > 4) return normal;

        if (maximum > normal * 2) maximum = (ushort)(normal * 2);
        if (minimum < normal / 2) minimum = (ushort)(normal / 2);

        if (inverseSpeed) gameSpeed = (ushort)(4 - gameSpeed);

        return gameSpeed switch
        {
            0 => minimum,
            1 => (ushort)(normal - ((normal - minimum) / 2)),
            3 => (ushort)(normal + ((maximum - normal) / 2)),
            4 => maximum,

            /* Never reached, but avoids compiler errors */
            _ => normal,
        };
    }

    /*
     * Gets the tile corresponding to the given encoded index.
     *
     * @param id The encoded index to get the tile of.
     * @return The tile.
     */
    internal static Tile32 Tools_Index_GetTile(ushort encoded)
    {
        ushort index;
        var tile = new Tile32();

        index = Tools_Index_Decode(encoded);
        tile.x = 0;
        tile.y = 0;

        switch (Tools_Index_GetType(encoded))
        {
            case IndexType.IT_TILE: return Tile_UnpackTile(index);
            case IndexType.IT_UNIT: return (index < (ushort)UnitIndex.UNIT_INDEX_MAX) ? Unit_Get_ByIndex(index).o.position : tile;
            case IndexType.IT_STRUCTURE:
                {
                    StructureInfo si;
                    CStructure s;

                    if (index >= (ushort)StructureIndex.STRUCTURE_INDEX_MAX_HARD) return tile;

                    s = Structure_Get_ByIndex(index);
                    si = g_table_structureInfo[s.o.type];

                    return Tile_AddTileDiff(s.o.position, g_table_structure_layoutTileDiff[si.layout]);
                }
            default: return tile;
        }
    }
}
