/* Map */

namespace SharpDune
{
    /* Types of available landscapes. */
    enum LandscapeType
    {
        LST_NORMAL_SAND = 0,                       /*<! Flat sand. */
        LST_PARTIAL_ROCK = 1,                      /*!< Edge of a rocky area (mostly sand). */
        LST_ENTIRELY_DUNE = 2,                     /*!< Entirely sand dunes. */
        LST_PARTIAL_DUNE = 3,                      /*!< Partial sand dunes. */
        LST_ENTIRELY_ROCK = 4,                     /*!< Center part of rocky area. */
        LST_MOSTLY_ROCK = 5,                       /*!< Edge of a rocky area (mostly rocky). */
        LST_ENTIRELY_MOUNTAIN = 6,                 /*!< Center part of the mountain. */
        LST_PARTIAL_MOUNTAIN = 7,                  /*!< Edge of a mountain. */
        LST_SPICE = 8,                             /*!< Sand with spice. */
        LST_THICK_SPICE = 9,                       /*!< Sand with thick spice. */
        LST_CONCRETE_SLAB = 10,                    /*!< Concrete slab. */
        LST_WALL = 11,                             /*!< Wall. */
        LST_STRUCTURE = 12,                        /*!< Structure. */
        LST_DESTROYED_WALL = 13,                   /*!< Destroyed wall. */
        LST_BLOOM_FIELD = 14,                      /*!< Bloom field. */

        LST_MAX = 15
    }

    /*
	 * A Tile as stored in the memory in the map.
	 */
    [StructLayout(LayoutKind.Explicit)]
    class Tile
    {
        /* 0000 01FF PACK */
        [FieldOffset(0)] internal ushort groundTileID;  /*!< The "Icon" which is drawn on this Tile. */
        /* 0000 FE00 PACK */
        [FieldOffset(2)] internal ushort overlayTileID; /*!< The Overlay which is drawn over this Tile. */
        /* 0007 0000 PACK */
        [FieldOffset(4)] internal byte houseID;         /*!< Which House owns this Tile. */
        /* 0008 0000 PACK */
        [FieldOffset(5)] internal bool isUnveiled;      /*!< There is no fog on the Tile. */
        /* 0010 0000 PACK */
        [FieldOffset(6)] internal bool hasUnit;         /*!< There is a Unit on the Tile. */
        /* 0020 0000 PACK */
        [FieldOffset(7)] internal bool hasStructure;    /*!< There is a Structure on the Tile. */
        /* 0040 0000 PACK */
        [FieldOffset(8)] internal bool hasAnimation;    /*!< There is animation going on the Tile. */
        /* 0080 0000 PACK */
        [FieldOffset(9)] internal bool hasExplosion;    /*!< There is an explosion on the Tile. */
        /* FF00 0000 PACK */
        [FieldOffset(10)] internal ushort index;        /*!< Index of the Structure / Unit (index 1 is Structure/Unit 0, etc). */
    }

    /*
	 * Information about LandscapeType.
	 */
    class LandscapeInfo
    {
        internal byte[] movementSpeed = new byte[6];            /*!< Per MovementType the speed a Unit has on this LandscapeType. */
        internal bool letUnitWobble;                            /*!< True if a Unit on this LandscapeType should wobble around while moving on it. */
        internal bool isValidForStructure;                      /*!< True if a Structure with notOnConcrete false can be build on this LandscapeType. */
        internal bool isSand;                                   /*!< True if the LandscapeType is a sand tile (sand, dune, spice, thickspice, bloom). */
        internal bool isValidForStructure2;                     /*!< True if a Structure with notOnConcrete true can be build on this LandscapeType. */
        internal bool canBecomeSpice;                           /*!< True if the LandscapeType can become a spice tile. */
        internal byte craterType;                               /*!< Type of crater on tile; 0 for none, 1 for sand, 2 for concrete. */
        internal ushort radarColour;                            /*!< Colour used on radar for this LandscapeType. */
        internal ushort spriteID;                               /*!< Sprite used on map for this LandscapeType. */
    }

    /* Definition of the map size of a map scale. */
    class MapInfo
    {
        internal ushort minX;                                   /*!< Minimal X position of the map. */
        internal ushort minY;                                   /*!< Minimal Y position of the map. */
        internal ushort sizeX;                                  /*!< Width of the map. */
        internal ushort sizeY;                                  /*!< Height of the map. */
    }

    class Map
    {
        internal static ushort[] g_mapTileID = new ushort[64 * 64];

        internal static Tile[] g_map = new Tile[64 * 64];                                        /*!< All map data. */
        
        internal static byte[][] g_functions = { //[3][3]
            new byte[] {0, 1, 0}, new byte[] {2, 3, 0}, new byte[] {0, 1, 0}
        };

        internal static ushort g_changedTilesCount;
        internal static ushort[] g_changedTiles = new ushort[200];
        internal static byte[] g_changedTilesMap = new byte[512];

        static readonly bool s_debugNoExplosionDamage;               /*!< When non-zero, explosions do no damage to their surrounding. */

        internal static byte[] g_dirtyMinimap = new byte[512];
        internal static byte[] g_displayedMinimap = new byte[512];
        internal static byte[] g_dirtyViewport = new byte[512];
        internal static byte[] g_displayedViewport = new byte[512];

        /*
         * Map definitions.
         * Map sizes: [0] is 62x62, [1] is 32x32, [2] is 21x21.
         */
        internal static MapInfo[] g_mapInfos = { //[3]
	        new() { minX =  1, minY =  1, sizeX = 62, sizeY = 62 },
	        new() { minX = 16, minY = 16, sizeX = 32, sizeY = 32 },
	        new() { minX = 21, minY = 21, sizeX = 21, sizeY = 21 }
        };

        internal static ushort g_dirtyViewportCount;
        internal static bool g_selectionRectangleNeedRepaint;

        /*
		 * Type of landscape for the landscape sprites.
		 *
		 * 0=normal sand, 1=partial rock, 5=mostly rock, 4=entirely rock,
		 * 3=partial sand dunes, 2=entirely sand dunes, 7=partial mountain,
		 * 6=entirely mountain, 8=spice, 9=thick spice
		 * @see Map_GetLandscapeType
		 */
        static readonly ushort[] _landscapeSpriteMap = {
            0, 1, 1, 1, 5, 1, 5, 5, 5, 5, /* Sprites 127-136 */
			5, 5, 5, 5, 5, 5, 4, 3, 3, 3, /* Sprites 137-146 */
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3, /* Sprites 147-156 */
			3, 3, 2, 7, 7, 7, 7, 7, 7, 7, /* Sprites 157-166 */
			7, 7, 7, 7, 7, 7, 7, 7, 6, 8, /* Sprites 167-176 */
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8, /* Sprites 177-186 */
			8, 8, 8, 8, 8, 9, 9, 9, 9, 9, /* Sprites 187-196 */
			9, 9, 9, 9, 9, 9, 9, 9, 9, 9, /* Sprites 197-206 */
			9,                            /* Sprite  207 (bloom sprites 208 and 209 are already caught). */
		};

        static readonly ushort[][][] _offsetTable = { //[2][21][4]
	        new ushort[][] {
                new ushort[] {0, 0, 4, 0}, new ushort[] {4, 0, 4, 4}, new ushort[] {0, 0, 0, 4}, new ushort[] {0, 4, 4, 4}, new ushort[] {0, 0, 0, 2},
		        new ushort[] {0, 2, 0, 4}, new ushort[] {0, 0, 2, 0}, new ushort[] {2, 0, 4, 0}, new ushort[] {4, 0, 4, 2}, new ushort[] {4, 2, 4, 4},
		        new ushort[] {0, 4, 2, 4}, new ushort[] {2, 4, 4, 4}, new ushort[] {0, 0, 4, 4}, new ushort[] {2, 0, 2, 2}, new ushort[] {0, 0, 2, 2},
		        new ushort[] {4, 0, 2, 2}, new ushort[] {0, 2, 2, 2}, new ushort[] {2, 2, 4, 2}, new ushort[] {2, 2, 0, 4}, new ushort[] {2, 2, 4, 4},
                new ushort[] {2, 2, 2, 4}
	        },
            new ushort[][] {
                new ushort[] {0, 0, 4, 0}, new ushort[] {4, 0, 4, 4}, new ushort[] {0, 0, 0, 4}, new ushort[] {0, 4, 4, 4}, new ushort[] {0, 0, 0, 2},
		        new ushort[] {0, 2, 0, 4}, new ushort[] {0, 0, 2, 0}, new ushort[] {2, 0, 4, 0}, new ushort[] {4, 0, 4, 2}, new ushort[] {4, 2, 4, 4},
		        new ushort[] {0, 4, 2, 4}, new ushort[] {2, 4, 4, 4}, new ushort[] {4, 0, 0, 4}, new ushort[] {2, 0, 2, 2}, new ushort[] {0, 0, 2, 2},
		        new ushort[] {4, 0, 2, 2}, new ushort[] {0, 2, 2, 2}, new ushort[] {2, 2, 4, 2}, new ushort[] {2, 2, 0, 4}, new ushort[] {2, 2, 4, 4},
                new ushort[] {2, 2, 2, 4}
	        }
        };

        /*
		 * Get type of landscape of a tile.
		 *
		 * @param packed The packed tile to examine.
		 * @return The type of landscape at the tile.
		 */
        internal static ushort Map_GetLandscapeType(ushort packed)
        {
            Tile t;
            short spriteOffset;

            t = g_map[packed];

            if (t.groundTileID == g_builtSlabTileID) return (ushort)LandscapeType.LST_CONCRETE_SLAB;

            if (t.groundTileID == g_bloomTileID || t.groundTileID == g_bloomTileID + 1) return (ushort)LandscapeType.LST_BLOOM_FIELD;

            if (t.groundTileID > g_wallTileID && t.groundTileID < g_wallTileID + 75) return (ushort)LandscapeType.LST_WALL;

            if (t.overlayTileID == g_wallTileID) return (ushort)LandscapeType.LST_DESTROYED_WALL;

            if (Structure_Get_ByPackedTile(packed) != null) return (ushort)LandscapeType.LST_STRUCTURE;

            spriteOffset = (short)(t.groundTileID - g_landscapeTileID); /* Offset in the landscape icon group. */
            if (spriteOffset < 0 || spriteOffset > 80) return (ushort)LandscapeType.LST_ENTIRELY_ROCK;

            return _landscapeSpriteMap[spriteOffset];
        }

        /*
         * Sets the selection on given packed tile.
         *
         * @param packed The packed tile to set the selection on.
         */
        internal static void Map_SetSelection(ushort packed)
        {
            if (g_selectionType == (ushort)SelectionType.TARGET) return;

            if (g_selectionType == (ushort)SelectionType.PLACE)
            {
                g_selectionState = Structure_IsValidBuildLocation(packed, (StructureType)g_structureActiveType);
                g_selectionPosition = packed;
                return;
            }

            if ((packed != 0xFFFF && g_map[packed].overlayTileID != g_veiledTileID) || g_debugScenario)
            {
                Structure s;

                s = Structure_Get_ByPackedTile(packed);
                if (s != null)
                {
                    StructureInfo si;

                    si = g_table_structureInfo[s.o.type];
                    if (s.o.houseID == (byte)g_playerHouseID && g_selectionType != (ushort)SelectionType.MENTAT)
                    {
                        GUI_DisplayHint(si.o.hintStringID, si.o.spriteID);
                    }

                    packed = Tile_PackTile(s.o.position);

                    Map_SetSelectionSize(si.layout);

                    Structure_UpdateMap(s);
                }
                else
                {
                    Map_SetSelectionSize((ushort)StructureLayout.STRUCTURE_LAYOUT_1x1);
                }

                if (g_selectionType != (ushort)SelectionType.TARGET)
                {
                    Unit u;

                    u = Unit_Get_ByPackedTile(packed);
                    if (u != null)
                    {
                        if (u.o.type != (byte)UnitType.UNIT_CARRYALL)
                        {
                            Unit_Select(u);
                        }
                    }
                    else
                    {
                        if (g_unitSelected != null)
                        {
                            Unit_Select(null);
                        }
                    }
                }
                g_selectionPosition = packed;
                return;
            }

            Map_SetSelectionSize((ushort)StructureLayout.STRUCTURE_LAYOUT_1x1);
            g_selectionPosition = packed;
            return;
        }

        /*
		 * Checks if the given position is inside the map.
		 *
		 * @param position The tile (packed) to check.
		 * @return True if the position is valid.
		 */
        internal static bool Map_IsValidPosition(ushort position)
        {
            ushort x, y;
            MapInfo mapInfo;

            if ((position & 0xC000) != 0) return false;

            x = Tile_GetPackedX(position);
            y = Tile_GetPackedY(position);

            mapInfo = g_mapInfos[g_scenario.mapScale];

            return (mapInfo.minX <= x && x < (mapInfo.minX + mapInfo.sizeX) && mapInfo.minY <= y && y < (mapInfo.minY + mapInfo.sizeY));
        }

        static ushort selectionLayout;
        /*
         * Sets the selection size for the given layout.
         *
         * @param layout The layout to determine selection size from.
         * @return The previous layout.
         * @see StructureLayout
         */
        internal static ushort Map_SetSelectionSize(ushort layout)
        {
            ushort oldLayout;
            ushort oldPosition;

            oldLayout = selectionLayout;
            if ((layout & 0x80) != 0) return oldLayout; //TODO: Check

            oldPosition = Map_SetSelectionObjectPosition(0xFFFF);

            selectionLayout = layout;
            g_selectionWidth = g_table_structure_layoutSize[layout].width;
            g_selectionHeight = g_table_structure_layoutSize[layout].height;

            Map_SetSelectionObjectPosition(oldPosition);

            return oldLayout;
        }

        static ushort selectionPosition = 0xFFFF;
        /*
		 * Sets the selection object to the given position.
		 *
		 * @param packed The position to set.
		 * @return The previous position.
		 */
        internal static ushort Map_SetSelectionObjectPosition(ushort packed)
        {
            ushort oldPacked;

            oldPacked = selectionPosition;

            if (packed == oldPacked) return oldPacked;

            Map_InvalidateSelection(oldPacked, false);

            if (packed != 0xFFFF) Map_InvalidateSelection(packed, true);

            selectionPosition = packed;

            return oldPacked;
        }

        static void Map_InvalidateSelection(ushort packed, bool enable)
        {
            ushort x, y;

            if (packed == 0xFFFF) return;

            for (y = 0; y < g_selectionHeight; y++)
            {
                for (x = 0; x < g_selectionWidth; x++)
                {
                    ushort curPacked;

                    curPacked = (ushort)(packed + Tile_PackXY(x, y));

                    Map_Update(curPacked, 0, false);

                    if (enable)
                    {
                        BitArray_Set(g_displayedViewport, curPacked);
                    }
                    else
                    {
                        BitArray_Clear(g_displayedViewport, curPacked);
                    }
                }
            }
        }

        static readonly short[] offsets = {
            -64, /* up */
            -63, /* up right */
            1,   /* right */
            65,  /* down rigth */
            64,  /* down */
            63,  /* down left */
            -1,  /* left */
            -65, /* up left */
            0
        };
        /*
		 * Updates ??.
		 *
		 * @param packed The packed tile.
		 * @param type The type of update.
		 * @param ignoreInvisible Wether to ignore tile visibility check.
		 */
        internal static void Map_Update(ushort packed, ushort type, bool ignoreInvisible)
        {
            if (!ignoreInvisible && !Map_IsTileVisible(packed)) return;

            switch (type)
            {
                default:
                case 0:
                    {
                        byte i;
                        ushort curPacked = 0;

                        if (BitArray_Test(g_dirtyMinimap, packed)) return;

                        g_dirtyViewportCount++;

                        for (i = 0; i < 9; i++)
                        {
                            curPacked = (ushort)((packed + offsets[i]) & 0xFFF);
                            BitArray_Set(g_dirtyViewport, curPacked);
                            if (BitArray_Test(g_displayedViewport, curPacked)) g_selectionRectangleNeedRepaint = true;
                        }

                        BitArray_Set(g_dirtyMinimap, curPacked);
                        return;
                    }

                case 1:
                case 2:
                case 3:
                    BitArray_Set(g_dirtyViewport, packed);
                    return;
            }
        }

        /*
		 * Check if a position is unveiled (the fog is removed).
		 *
		 * @param position For which position to check.
		 * @return True if and only if the position is unveiled.
		 */
        internal static bool Map_IsPositionUnveiled(ushort position)
        {
            Tile t;

            if (g_debugScenario) return true;

            t = g_map[position];

            if (!t.isUnveiled) return false;
            if (!Tile_IsUnveiled(t.overlayTileID)) return false;

            return true;
        }

        /*
		 * Checks wether a packed tile is visible in the viewport.
		 *
		 * @param packed The packed tile.
		 * @return True if the tile is visible.
		 */
        static bool Map_IsTileVisible(ushort packed)
        {
            byte x, y;
            byte x2, y2;

            x = (byte)Tile_GetPackedX(packed);
            y = (byte)Tile_GetPackedY(packed);
            x2 = (byte)Tile_GetPackedX(g_minimapPosition);
            y2 = (byte)Tile_GetPackedY(g_minimapPosition);

            return x >= x2 && x < x2 + 15 && y >= y2 && y < y2 + 10;
        }

        static readonly ushort[] tileOffsets = {
            0x0080, 0x0088, 0x0090, 0x0098,
            0x00A0, 0x00A8, 0x00B0, 0x00B8,
            0x00C0, 0x00C8, 0x00D0, 0x00D8,
            0x00E0, 0x00E8, 0x00F0, 0x00F8,
            0x0100, 0x0180
        };
        /*
         * Around a position, run a certain function for all tiles within a certain radius.
         *
         * @note Radius is in a 1/4th of a tile unit.
         *
         * @param radius The radius of the to-update tiles.
         * @param position The position to go from.
         * @param unit The unit to update for (can be NULL if function < 2).
         * @param function The function to call.
         */
        internal static void Map_UpdateAround(ushort radius, tile32 position, Unit unit, byte function)
        {
            short i, j;
            var diff = new tile32();
            ushort lastPacked;

            if (radius == 0 || (position.x == 0 && position.y == 0)) return;

            radius--;

            /* If radius is bigger or equal than 32, update all tiles in a 5x5 grid around the unit. */
            if (radius >= 32)
            {
                ushort x = Tile_GetPosX(position);
                ushort y = Tile_GetPosY(position);

                for (i = -2; i <= 2; i++)
                {
                    for (j = -2; j <= 2; j++)
                    {
                        ushort curPacked;

                        if (x + i < 0 || x + i >= 64 || y + j < 0 || y + j >= 64) continue;

                        curPacked = Tile_PackXY((ushort)(x + i), (ushort)(y + j));
                        BitArray_Set(g_dirtyViewport, curPacked);
                        g_dirtyViewportCount++;

                        switch (function)
                        {
                            case 0: Map_Update(curPacked, 0, false); break;
                            case 1: Map_Update(curPacked, 3, false); break;
                            case 2: Unit_RemoveFromTile(unit, curPacked); break;
                            case 3: Unit_AddToTile(unit, curPacked); break;
                            default: break;
                        }
                    }
                }
                return;
            }

            radius = Math.Max(radius, (ushort)15);
            position.x -= tileOffsets[radius - 15];
            position.y -= tileOffsets[radius - 15];

            diff.x = 0;
            diff.y = 0;
            lastPacked = 0;

            i = 0;
            do
            {
                var curTile = Tile_AddTileDiff(position, diff);

                if (Tile_IsValid(curTile))
                {
                    var curPacked = Tile_PackTile(curTile);

                    if (curPacked != lastPacked)
                    {
                        BitArray_Set(g_dirtyViewport, curPacked);
                        g_dirtyViewportCount++;

                        switch (function)
                        {
                            case 0: Map_Update(curPacked, 0, false); break;
                            case 1: Map_Update(curPacked, 3, false); break;
                            case 2: Unit_RemoveFromTile(unit, curPacked); break;
                            case 3: Unit_AddToTile(unit, curPacked); break;
                            default: break;
                        }

                        lastPacked = curPacked;
                    }
                }

                if (i == 8) break;
                diff = g_table_tilediff[radius + 1][i++];
            } while ((diff.x != 0) || (diff.y != 0));
        }

        /*
         * Mark a specific tile as dirty, so it gets a redrawn next time.
         *
         * @param packed The tile to mark as dirty.
         */
        internal static void Map_MarkTileDirty(ushort packed)
        {
            if (BitArray_Test(g_displayedMinimap, packed) && g_scenario.mapScale + 1 == 0) return;

            BitArray_Set(g_changedTilesMap, packed);
            if (g_changedTilesCount < g_changedTiles.Length /*Common.lengthof<ushort>(g_changedTiles)*/) g_changedTiles[g_changedTilesCount++] = packed;
        }

        /*
         * Unveil a tile for a House.
         * @param packed The tile to unveil.
         * @param houseID The house to unveil for.
         * @return True if tile was freshly unveiled.
         */
        internal static bool Map_UnveilTile(ushort packed, byte houseID)
        {
            Structure s;
            Unit u;
            Tile t;

            if (houseID != (byte)g_playerHouseID) return false;
            if (Tile_IsOutOfMap(packed)) return false;

            t = g_map[packed];

            if (t.isUnveiled && Tile_IsUnveiled(t.overlayTileID)) return false;
            t.isUnveiled = true;

            Map_MarkTileDirty(packed);

            u = Unit_Get_ByPackedTile(packed);
            if (u != null) Unit_HouseUnitCount_Add(u, houseID);

            s = Structure_Get_ByPackedTile(packed);
            if (s != null)
            {
                var sv = s;
                sv.o.seenByHouses |= (byte)(1 << houseID);
                if (houseID == (byte)HouseType.HOUSE_ATREIDES) sv.o.seenByHouses |= 1 << (byte)HouseType.HOUSE_FREMEN;
            }

            Map_UnveilTile_Neighbour(packed);
            Map_UnveilTile_Neighbour((ushort)(packed + 1));
            Map_UnveilTile_Neighbour((ushort)(packed - 1));
            Map_UnveilTile_Neighbour((ushort)(packed - 64));
            Map_UnveilTile_Neighbour((ushort)(packed + 64));

            return true;
        }

        /*
         * After unveiling, check neighbour tiles. This function handles one neighbour.
         * @param packed The neighbour tile of an unveiled tile.
         */
        static void Map_UnveilTile_Neighbour(ushort packed)
        {
            ushort tileID;
            Tile t;

            if (Tile_IsOutOfMap(packed)) return;

            t = g_map[packed];

            tileID = 15;
            if (t.isUnveiled)
            {
                int i;

                if (g_validateStrictIfZero == 0 && Tile_IsUnveiled(t.overlayTileID)) return;

                tileID = 0;

                for (i = 0; i < 4; i++)
                {
                    var neighbour = (ushort)(packed + g_table_mapDiff[i]);

                    if (Tile_IsOutOfMap(neighbour) || !g_map[neighbour].isUnveiled)
                    {
                        tileID |= (ushort)(1 << i);
                    }
                }
            }

            if (tileID != 0)
            {
                if (tileID != 15)
                {
                    var u = Unit_Get_ByPackedTile(packed);
                    if (u != null) Unit_HouseUnitCount_Add(u, (byte)g_playerHouseID);
                }

                tileID = g_iconMap[g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_FOG_OF_WAR] + tileID];
            }

            t.overlayTileID = tileID;

            Map_Update(packed, 0, false);
        }

        /* Border tiles of the viewport relative to the top-left. */
        static readonly ushort[] viewportBorder = {
            0x0000, 0x0001, 0x0002, 0x0003, 0x0004, 0x0005, 0x0006, 0x0007, 0x0008, 0x0009, 0x000A, 0x000B, 0x000C, 0x000D, 0x000E,
            0x0040, 0x004E,
            0x0080, 0x008E,
            0x00C0, 0x00CE,
            0x0100, 0x010E,
            0x0140, 0x014E,
            0x0180, 0x018E,
            0x01C0, 0x01CE,
            0x0200, 0x020E,
            0x0240, 0x0241, 0x0242, 0x0243, 0x0244, 0x0245, 0x0246, 0x0247, 0x0248, 0x0249, 0x024A, 0x024B, 0x024C, 0x024D, 0x024E,
            0xFFFF
        };
        static ushort minimapPreviousPosition;
        /*
         * Update the minimap position.
         *
         * @param packed The new position.
         * @param forceUpdate If true force the update even if the position didn't change.
         */
        internal static void Map_UpdateMinimapPosition(ushort packed, bool forceUpdate)
        {
            bool cleared;
            Screen oldScreenID;

            if (packed != 0xFFFF && packed == minimapPreviousPosition && !forceUpdate) return;
            if (g_selectionType == (ushort)SelectionType.MENTAT) return;

            oldScreenID = GFX_Screen_SetActive(Screen.NO1);

            cleared = false;

            if (minimapPreviousPosition != 0xFFFF && minimapPreviousPosition != packed)
            {
                var m = viewportBorder;
                var mPointer = 0;

                cleared = true;

                for (mPointer = 0; m[mPointer] != 0xFFFF; mPointer++)
                {
                    ushort curPacked;

                    curPacked = (ushort)(minimapPreviousPosition + m[mPointer]);
                    BitArray_Clear(g_displayedMinimap, curPacked);

                    GUI_Widget_Viewport_DrawTile(curPacked);
                }
            }

            if (packed != 0xFFFF && (packed != minimapPreviousPosition || forceUpdate))
            {
                var m = viewportBorder;
                var mPointer = 0;
                ushort mapScale;
                MapInfo mapInfo;
                ushort left, top, right, bottom;

                mapScale = g_scenario.mapScale;
                mapInfo = g_mapInfos[mapScale];

                left = (ushort)((Tile_GetPackedX(packed) - mapInfo.minX) * (mapScale + 1) + 256);
                right = (ushort)(left + mapScale * 15 + 14);
                top = (ushort)((Tile_GetPackedY(packed) - mapInfo.minY) * (mapScale + 1) + 136);
                bottom = (ushort)(top + mapScale * 10 + 9);

                GUI_DrawWiredRectangle(left, top, right, bottom, 15);

                for (mPointer = 0; m[mPointer] != 0xFFFF; mPointer++)
                {
                    ushort curPacked;

                    curPacked = (ushort)(packed + m[mPointer]);
                    BitArray_Set(g_displayedMinimap, curPacked);
                }
            }

            if (cleared && oldScreenID == Screen.NO0)
            {
                GUI_Mouse_Hide_Safe();
                GUI_Screen_Copy(32, 136, 32, 136, 8, 64, Screen.NO1, Screen.NO0);
                GUI_Mouse_Show_Safe();
            }

            GFX_Screen_SetActive(oldScreenID);

            minimapPreviousPosition = packed;
        }

        /*
         * Perform a bloom explosion, filling the area with spice.
         * @param packed Center position.
         * @param houseID %House causing the explosion.
         */
        internal static void Map_Bloom_ExplodeSpice(ushort packed, byte houseID)
        {
            if (g_validateStrictIfZero == 0)
            {
                var u = Unit_Get_ByPackedTile(packed);
                Unit_Remove(u);
                g_map[packed].groundTileID = (ushort)(g_mapTileID[packed] & 0x1FF);
                Map_MakeExplosion((ushort)ExplosionType.EXPLOSION_SPICE_BLOOM_TREMOR, Tile_UnpackTile(packed), 0, 0);
            }

            if (houseID == (byte)g_playerHouseID) Sound_Output_Feedback(36);

            Map_FillCircleWithSpice(packed, 5);
        }

        /*
         * Change amount of spice at \a packed position.
         * @param packed Position in the world to modify.
         * @param dir Direction of change, > 0 means add spice, < 0 means remove spice.
         */
        internal static void Map_ChangeSpiceAmount(ushort packed, short dir)
        {
            ushort type;
            ushort spriteID;

            if (dir == 0) return;

            type = Map_GetLandscapeType(packed);

            if (type == (ushort)LandscapeType.LST_THICK_SPICE && dir > 0) return;
            if (type != (ushort)LandscapeType.LST_SPICE && type != (ushort)LandscapeType.LST_THICK_SPICE && dir < 0) return;
            if (type != (ushort)LandscapeType.LST_NORMAL_SAND && type != (ushort)LandscapeType.LST_ENTIRELY_DUNE && type != (ushort)LandscapeType.LST_SPICE && dir > 0) return;

            if (dir > 0)
            {
                type = (type == (ushort)LandscapeType.LST_SPICE) ? (ushort)LandscapeType.LST_THICK_SPICE : (ushort)LandscapeType.LST_SPICE;
            }
            else
            {
                type = (type == (ushort)LandscapeType.LST_THICK_SPICE) ? (ushort)LandscapeType.LST_SPICE : (ushort)LandscapeType.LST_NORMAL_SAND;
            }

            spriteID = 0;
            if (type == (ushort)LandscapeType.LST_SPICE) spriteID = 49;
            if (type == (ushort)LandscapeType.LST_THICK_SPICE) spriteID = 65;

            spriteID = (ushort)(g_iconMap[g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_LANDSCAPE] + spriteID] & 0x1FF);
            g_mapTileID[packed] = (ushort)(0x8000 | spriteID);
            g_map[packed].groundTileID = spriteID;

            Map_FixupSpiceEdges(packed);
            Map_FixupSpiceEdges((ushort)(packed + 1));
            Map_FixupSpiceEdges((ushort)(packed - 1));
            Map_FixupSpiceEdges((ushort)(packed - 64));
            Map_FixupSpiceEdges((ushort)(packed + 64));
        }

        /*
         * Fixes edges of spice / thick spice to show sand / normal spice for better looks.
         * @param packed Position to check and possible fix edges of.
         */
        static void Map_FixupSpiceEdges(ushort packed)
        {
            ushort type;
            ushort spriteID;

            packed &= 0xFFF;
            type = Map_GetLandscapeType(packed);
            spriteID = 0;

            if (type == (ushort)LandscapeType.LST_SPICE || type == (ushort)LandscapeType.LST_THICK_SPICE)
            {
                byte i;

                for (i = 0; i < 4; i++)
                {
                    var curPacked = (ushort)(packed + g_table_mapDiff[i]);
                    ushort curType;

                    if (Tile_IsOutOfMap(curPacked))
                    {
                        if (type == (ushort)LandscapeType.LST_SPICE || type == (ushort)LandscapeType.LST_THICK_SPICE) spriteID |= (ushort)(1 << i);
                        continue;
                    }

                    curType = Map_GetLandscapeType(curPacked);

                    if (type == (ushort)LandscapeType.LST_SPICE)
                    {
                        if (curType == (ushort)LandscapeType.LST_SPICE || curType == (ushort)LandscapeType.LST_THICK_SPICE) spriteID |= (ushort)(1 << i);
                        continue;
                    }

                    if (curType == (ushort)LandscapeType.LST_THICK_SPICE) spriteID |= (ushort)(1 << i);
                }

                spriteID += (ushort)((type == (ushort)LandscapeType.LST_SPICE) ? 49 : 65);

                spriteID = (ushort)(g_iconMap[g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_LANDSCAPE] + spriteID] & 0x1FF);
                g_mapTileID[packed] = (ushort)(0x8000 | spriteID);
                g_map[packed].groundTileID = spriteID;
            }

            Map_Update(packed, 0, false);
        }

        /*
         * Make an explosion on the given position, of a certain type. All units in the
         *  neighbourhoud get an amount of damage related to their distance to the
         *  explosion.
         * @param type The type of explosion.
         * @param position The position of the explosion.
         * @param hitpoints The amount of hitpoints to give people in the neighbourhoud.
         * @param unitOriginEncoded The unit that fired the bullet.
         */
        internal static void Map_MakeExplosion(ushort type, tile32 position, ushort hitpoints, ushort unitOriginEncoded)
        {
            var reactionDistance = (ushort)((type == (ushort)ExplosionType.EXPLOSION_DEATH_HAND) ? 32 : 16);
            var positionPacked = Tile_PackTile(position);

            if (!s_debugNoExplosionDamage && hitpoints != 0)
            {
                var find = new PoolFindStruct
                {
                    houseID = (byte)HouseType.HOUSE_INVALID,
                    index = 0xFFFF,
                    type = 0xFFFF
                };

                while (true)
                {
                    UnitInfo ui;
                    ushort distance;
                    Team t;
                    Unit u;
                    Unit us;
                    Unit attack;

                    u = Unit_Find(find);
                    if (u == null) break;

                    ui = g_table_unitInfo[u.o.type];

                    distance = (ushort)(Tile_GetDistance(position, u.o.position) >> 4);
                    if (distance >= reactionDistance) continue;

                    if (!(u.o.type == (byte)UnitType.UNIT_SANDWORM && type == (ushort)ExplosionType.EXPLOSION_SANDWORM_SWALLOW) && u.o.type != (byte)UnitType.UNIT_FRIGATE)
                    {
                        Unit_Damage(u, (ushort)(hitpoints >> (distance >> 2)), 0);
                    }

                    if (u.o.houseID == (byte)g_playerHouseID) continue;

                    us = Tools_Index_GetUnit(unitOriginEncoded);
                    if (us == null) continue;
                    if (us == u) continue;
                    if (House_AreAllied(Unit_GetHouseID(u), Unit_GetHouseID(us))) continue;

                    t = Unit_GetTeam(u);
                    if (t != null)
                    {
                        UnitInfo targetInfo;
                        Unit target;

                        if (t.action == (ushort)TeamActionType.TEAM_ACTION_STAGING)
                        {
                            Unit_RemoveFromTeam(u);
                            Unit_SetAction(u, ActionType.ACTION_HUNT);
                            continue;
                        }

                        target = Tools_Index_GetUnit(t.target);
                        if (target == null) continue;

                        targetInfo = g_table_unitInfo[target.o.type];
                        if (targetInfo.bulletType == (byte)UnitType.UNIT_INVALID) t.target = unitOriginEncoded;
                        continue;
                    }

                    if (u.o.type == (byte)UnitType.UNIT_HARVESTER)
                    {
                        var uis = g_table_unitInfo[us.o.type];

                        if (uis.movementType == (ushort)MovementType.MOVEMENT_FOOT && u.targetMove == 0)
                        {
                            if (u.actionID != (byte)ActionType.ACTION_MOVE) Unit_SetAction(u, ActionType.ACTION_MOVE);
                            u.targetMove = unitOriginEncoded;
                            continue;
                        }
                    }

                    if (ui.bulletType == (byte)UnitType.UNIT_INVALID) continue;

                    if (u.actionID == (byte)ActionType.ACTION_GUARD && u.o.flags.byScenario)
                    {
                        Unit_SetAction(u, ActionType.ACTION_HUNT);
                    }

                    if (u.targetAttack != 0 && u.actionID != (byte)ActionType.ACTION_HUNT) continue;

                    attack = Tools_Index_GetUnit(u.targetAttack);
                    if (attack != null)
                    {
                        var packed = Tile_PackTile(u.o.position);
                        if (Tile_GetDistancePacked(Tools_Index_GetPackedTile(u.targetAttack), packed) <= ui.fireDistance) continue;
                    }

                    Unit_SetTarget(u, unitOriginEncoded);
                }
            }

            if (!s_debugNoExplosionDamage && hitpoints != 0)
            {
                var s = Structure_Get_ByPackedTile(positionPacked);

                if (s != null)
                {
                    if (type == (ushort)ExplosionType.EXPLOSION_IMPACT_LARGE)
                    {
                        var si = g_table_structureInfo[s.o.type];

                        if (si.o.hitpoints / 2 > s.o.hitpoints)
                        {
                            type = (ushort)ExplosionType.EXPLOSION_SMOKE_PLUME;
                        }
                    }

                    Structure_HouseUnderAttack(s.o.houseID);
                    Structure_Damage(s, hitpoints, 0);
                }
            }

            if (Map_GetLandscapeType(positionPacked) == (ushort)LandscapeType.LST_WALL && hitpoints != 0)
            {
                if ((g_table_structureInfo[(int)StructureType.STRUCTURE_WALL].o.hitpoints <= hitpoints) ||
                    (Tools_Random_256() <= (hitpoints * 256 / g_table_structureInfo[(int)StructureType.STRUCTURE_WALL].o.hitpoints)))
                {
                    Map_UpdateWall(positionPacked);
                }
            }

            Explosion_Start(type, position);
        }

        /*
         * Fill a circular area with spice.
         * @param packed Center position of the area.
         * @param radius Radius of the circle.
         */
        internal static void Map_FillCircleWithSpice(ushort packed, ushort radius)
        {
            ushort x;
            ushort y;
            int i;
            int j;

            if (radius == 0) return;

            x = Tile_GetPackedX(packed);
            y = Tile_GetPackedY(packed);

            for (i = -radius; i <= radius; i++)
            {
                for (j = -radius; j <= radius; j++)
                {
                    var curPacked = Tile_PackXY((ushort)(x + j), (ushort)(y + i));
                    var distance = Tile_GetDistancePacked(packed, curPacked);

                    if (distance > radius) continue;

                    if (distance == radius && (Tools_Random_256() & 1) == 0) continue;

                    if (Map_GetLandscapeType(curPacked) == (ushort)LandscapeType.LST_SPICE) continue;

                    Map_ChangeSpiceAmount(curPacked, 1);

                    if (g_debugScenario)
                    {
                        Map_MarkTileDirty(curPacked);
                    }
                }
            }

            Map_ChangeSpiceAmount(packed, 1);
        }

        static bool Map_UpdateWall(ushort packed)
        {
            Tile t;

            if (Map_GetLandscapeType(packed) != (ushort)LandscapeType.LST_WALL) return false;

            t = g_map[packed];

            t.groundTileID = (ushort)(g_mapTileID[packed] & 0x1FF);

            if (Map_IsPositionUnveiled(packed)) t.overlayTileID = g_wallTileID;

            Structure_ConnectWall(packed, true);
            Map_Update(packed, 0, false);

            return true;
        }

        /*
         * Check if a position is in the viewport.
         *
         * @param position For which position to check.
         * @param retX Pointer to X inside the viewport.
         * @param retY Pointer to Y inside the viewport.
         * @return True if and only if the position is in the viewport.
         */
        internal static bool Map_IsPositionInViewport(tile32 position, out ushort retX, out ushort retY)
        {
            short x, y;

            x = (short)((position.x >> 4) - (Tile_GetPackedX(g_viewportPosition) << 4));
            y = (short)((position.y >> 4) - (Tile_GetPackedY(g_viewportPosition) << 4));

            /*if (retX != null)*/ retX = (ushort)x;
            /*if (retY != null)*/ retY = (ushort)y;

            return x >= -16 && x <= 256 && y >= -16 && y <= 176;
        }

        static XYPosition[] mapScrollOffset;
        /*
         * Move the viewport position in the given direction.
         *
         * @param direction The direction to move in.
         * @return The new viewport position.
        */
        internal static ushort Map_MoveDirection(ushort direction)
        {
            unchecked
            {
                mapScrollOffset = new XYPosition[] {
                    new() { x = 0, y = (ushort)-1 }, new() { x = 1, y = (ushort)-1 }, new() { x = 1, y = 0 }, new() { x = 1, y = 1 },
                    new() { x = 0, y = 1 }, new() { x = (ushort)-1, y = 1 }, new() { x = (ushort)-1, y = 0 }, new() { x = (ushort)-1, y = (ushort)-1 }
                };
            }

            ushort x, y;
            MapInfo mapInfo;

            x = (ushort)(Tile_GetPackedX(g_minimapPosition) + mapScrollOffset[direction].x);
            y = (ushort)(Tile_GetPackedY(g_minimapPosition) + mapScrollOffset[direction].y);

            mapInfo = g_mapInfos[g_scenario.mapScale];

            x = Math.Max(x, mapInfo.minX);
            y = Math.Max(y, mapInfo.minY);

            x = (ushort)Math.Min(x, mapInfo.minX + mapInfo.sizeX - 15);
            y = (ushort)Math.Min(y, mapInfo.minY + mapInfo.sizeY - 10);

            g_viewportPosition = Tile_PackXY(x, y);
            return g_viewportPosition;
        }

        internal static void Map_SelectNext(bool getNext)
        {
            var find = new PoolFindStruct();
            Object selected = null;
            Object previous = null;
            Object next = null;
            Object first = null;
            Object last = null;
            var hasPrevious = false;
            var hasNext = false;

            if (g_unitSelected != null)
            {
                if (Map_IsTileVisible(Tile_PackTile(g_unitSelected.o.position))) selected = g_unitSelected.o;
            }
            else
            {
                Structure s;

                s = Structure_Get_ByPackedTile(g_selectionPosition);

                if (s != null && Map_IsTileVisible(Tile_PackTile(s.o.position))) selected = s.o;
            }

            find.houseID = (byte)HouseType.HOUSE_INVALID;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            while (true)
            {
                Unit u;

                u = Unit_Find(find);
                if (u == null) break;

                if (!g_table_unitInfo[u.o.type].o.flags.tabSelectable) continue;

                if (!Map_IsTileVisible(Tile_PackTile(u.o.position))) continue;

                if ((u.o.seenByHouses & (1 << (byte)g_playerHouseID)) == 0) continue;

                if (first == null) first = u.o;
                last = u.o;
                if (selected == null) selected = u.o;

                if (selected == u.o)
                {
                    hasPrevious = true;
                    continue;
                }

                if (!hasPrevious)
                {
                    previous = u.o;
                    continue;
                }

                if (!hasNext)
                {
                    next = u.o;
                    hasNext = true;
                }
            }

            find.houseID = (byte)HouseType.HOUSE_INVALID;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            while (true)
            {
                Structure s;

                s = Structure_Find(find);
                if (s == null) break;

                if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;

                if (!Map_IsTileVisible(Tile_PackTile(s.o.position))) continue;

                if ((s.o.seenByHouses & (1 << (byte)g_playerHouseID)) == 0) continue;

                if (first == null) first = s.o;
                last = s.o;
                if (selected == null) selected = s.o;

                if (selected == s.o)
                {
                    hasPrevious = true;
                    continue;
                }

                if (!hasPrevious)
                {
                    previous = s.o;
                    continue;
                }

                if (!hasNext)
                {
                    next = s.o;
                    hasNext = true;
                }
            }

            if (previous == null) previous = last;
            if (next == null) next = first;
            if (previous == null) previous = next;
            if (next == null) next = previous;

            selected = getNext ? next : previous;

            if (selected == null) return;

            Map_SetSelection(Tile_PackTile(selected.position));
        }

        static readonly short[] mapBase = { 1, -2, -2 };
        /*
         * Find a tile close the a LocationID described position (North, Enemy Base, ..).
         *
         * @param locationID Value between 0 and 7 to indicate where the tile should be.
         * @param houseID The HouseID looking for a tile (to get an idea of Enemy Base).
         * @return The tile requested.
         */
        internal static ushort Map_FindLocationTile(ushort locationID, byte houseID)
        {
            var mapInfo = g_mapInfos[g_scenario.mapScale];
            var mapOffset = (ushort)mapBase[g_scenario.mapScale];

            ushort ret = 0;

            if (locationID == 6)
            { /* Enemy Base */
                var find = new PoolFindStruct
                {
                    houseID = (byte)HouseType.HOUSE_INVALID,
                    index = 0xFFFF,
                    type = 0xFFFF
                };

                /* Find the house of an enemy */
                while (true)
                {
                    Structure s;

                    s = Structure_Find(find);
                    if (s == null) break;
                    if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;

                    if (s.o.houseID == houseID) continue;

                    houseID = s.o.houseID;
                    break;
                }
            }

            while (ret == 0)
            {
                switch (locationID)
                {
                    case 0: /* North */
                        ret = Tile_PackXY((ushort)(mapInfo.minX + Tools_RandomLCG_Range(0, (ushort)(mapInfo.sizeX - 2))), (ushort)(mapInfo.minY + mapOffset));
                        break;

                    case 1: /* East */
                        ret = Tile_PackXY((ushort)(mapInfo.minX + mapInfo.sizeX - mapOffset), (ushort)(mapInfo.minY + Tools_RandomLCG_Range(0, (ushort)(mapInfo.sizeY - 2))));
                        break;

                    case 2: /* South */
                        ret = Tile_PackXY((ushort)(mapInfo.minX + Tools_RandomLCG_Range(0, (ushort)(mapInfo.sizeX - 2))), (ushort)(mapInfo.minY + mapInfo.sizeY - mapOffset));
                        break;

                    case 3: /* West */
                        ret = Tile_PackXY((ushort)(mapInfo.minX + mapOffset), (ushort)(mapInfo.minY + Tools_RandomLCG_Range(0, (ushort)(mapInfo.sizeY - 2))));
                        break;

                    case 4: /* Air */
                        ret = Tile_PackXY((ushort)(mapInfo.minX + Tools_RandomLCG_Range(0, mapInfo.sizeX)), (ushort)(mapInfo.minY + Tools_RandomLCG_Range(0, mapInfo.sizeY)));
                        if (houseID == (byte)g_playerHouseID && !Map_IsValidPosition(ret)) ret = 0;
                        break;

                    case 5: /* Visible */
                        ret = Tile_PackXY((ushort)(Tile_GetPackedX(g_minimapPosition) + Tools_RandomLCG_Range(0, 14)), (ushort)(Tile_GetPackedY(g_minimapPosition) + Tools_RandomLCG_Range(0, 9)));
                        if (houseID == (byte)g_playerHouseID && !Map_IsValidPosition(ret)) ret = 0;
                        break;

                    case 6: /* Enemy Base */
                    case 7:
                        { /* Home Base */
                            var find = new PoolFindStruct();
                            Structure s;

                            find.houseID = houseID;
                            find.index = 0xFFFF;
                            find.type = 0xFFFF;

                            s = Structure_Find(find);

                            if (s != null)
                            {
                                ret = Tile_PackTile(Tile_MoveByRandom(s.o.position, 120, true));
                            }
                            else
                            {
                                Unit u;

                                find.houseID = houseID;
                                find.index = 0xFFFF;
                                find.type = 0xFFFF;

                                u = Unit_Find(find);

                                if (u != null)
                                {
                                    ret = Tile_PackTile(Tile_MoveByRandom(u.o.position, 120, true));
                                }
                                else
                                {
                                    ret = Tile_PackXY((ushort)(mapInfo.minX + Tools_RandomLCG_Range(0, mapInfo.sizeX)), (ushort)(mapInfo.minY + Tools_RandomLCG_Range(0, mapInfo.sizeY)));
                                }
                            }

                            if (houseID == (byte)g_playerHouseID && !Map_IsValidPosition(ret)) ret = 0;
                            break;
                        }

                    default: return 0;
                }

                ret &= 0xFFF;
                if (ret != 0 && Object_GetByPackedTile(ret) != null) ret = 0;
            }

            return ret;
        }

        /*
         * Search for spice around a position. Thick spice is preferred if it is not too far away.
         * @param packed Center position.
         * @param radius Radius of the search.
         * @return Best position with spice, or \c 0 if no spice found.
         */
        internal static ushort Map_SearchSpice(ushort packed, ushort radius)
        {
            ushort radius1;
            ushort radius2;
            ushort packed1;
            ushort packed2;
            ushort xmin;
            ushort xmax;
            ushort ymin;
            ushort ymax;
            MapInfo mapInfo;
            ushort x;
            ushort y;
            bool found;

            radius1 = (ushort)(radius + 1);
            radius2 = (ushort)(radius + 1);
            packed1 = packed;
            packed2 = packed;

            found = false;

            mapInfo = g_mapInfos[g_scenario.mapScale];

            xmin = Math.Max((ushort)(Tile_GetPackedX(packed) - radius), mapInfo.minX);
            xmax = Math.Min((ushort)(Tile_GetPackedX(packed) + radius), (ushort)(mapInfo.minX + mapInfo.sizeX - 1));
            ymin = Math.Max((ushort)(Tile_GetPackedY(packed) - radius), mapInfo.minY);
            ymax = Math.Min((ushort)(Tile_GetPackedY(packed) + radius), (ushort)(mapInfo.minY + mapInfo.sizeY - 1));

            for (y = ymin; y <= ymax; y++)
            {
                for (x = xmin; x <= xmax; x++)
                {
                    var curPacked = Tile_PackXY(x, y);
                    ushort type;
                    ushort distance;

                    if (!Map_IsValidPosition(curPacked)) continue;
                    if (g_map[curPacked].hasStructure) continue;
                    if (Unit_Get_ByPackedTile(curPacked) != null) continue;

                    type = Map_GetLandscapeType(curPacked);
                    distance = Tile_GetDistancePacked(curPacked, packed);

                    if (type == (ushort)LandscapeType.LST_THICK_SPICE && distance < 4)
                    {
                        found = true;

                        if (distance <= radius2)
                        {
                            radius2 = distance;
                            packed2 = curPacked;
                        }
                    }

                    if (type == (ushort)LandscapeType.LST_SPICE)
                    {
                        found = true;

                        if (distance <= radius1)
                        {
                            radius1 = distance;
                            packed1 = curPacked;
                        }
                    }
                }
            }

            if (!found) return 0;

            return (radius2 <= radius) ? packed2 : packed1;
        }

        /*
         * A unit drove over a special bloom, which can either give credits, a friendly
         *  Trike, an enemy Trike, or an enemy Infantry.
         * @param packed The tile where the bloom is on.
         * @param houseID The HouseID that is driving over the bloom.
         */
        internal static void Map_Bloom_ExplodeSpecial(ushort packed, byte houseID)
        {
            House h;
            var find = new PoolFindStruct();
            byte enemyHouseID;

            h = House_Get_ByIndex(houseID);

            g_map[packed].groundTileID = g_landscapeTileID;
            g_mapTileID[packed] = (ushort)(0x8000 | g_landscapeTileID);

            Map_Update(packed, 0, false);

            enemyHouseID = houseID;

            find.houseID = (byte)HouseType.HOUSE_INVALID;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            /* Find a house that belongs to the enemy */
            while (true)
            {
                Unit u;

                u = Unit_Find(find);
                if (u == null) break;

                if (u.o.houseID == houseID) continue;

                enemyHouseID = u.o.houseID;
                break;
            }

            switch (Tools_Random_256() & 0x3)
            {
                case 0:
                    h.credits += Tools_RandomLCG_Range(150, 400);
                    break;

                case 1:
                    {
                        var position = Tile_UnpackTile(packed);

                        position = Tile_MoveByRandom(position, 16, true);

                        /* ENHANCEMENT -- Dune2 inverted houseID and typeID arguments. */
                        Unit_Create((ushort)UnitIndex.UNIT_INDEX_INVALID, (byte)UnitType.UNIT_TRIKE, houseID, position, (sbyte)Tools_Random_256());
                        break;
                    }

                case 2:
                    {
                        var position = Tile_UnpackTile(packed);
                        Unit u;

                        position = Tile_MoveByRandom(position, 16, true);

                        /* ENHANCEMENT -- Dune2 inverted houseID and typeID arguments. */
                        u = Unit_Create((ushort)UnitIndex.UNIT_INDEX_INVALID, (byte)UnitType.UNIT_TRIKE, enemyHouseID, position, (sbyte)Tools_Random_256());

                        if (u != null) Unit_SetAction(u, ActionType.ACTION_HUNT);
                        break;
                    }

                case 3:
                    {
                        var position = Tile_UnpackTile(packed);
                        Unit u;

                        position = Tile_MoveByRandom(position, 16, true);

                        /* ENHANCEMENT -- Dune2 inverted houseID and typeID arguments. */
                        u = Unit_Create((ushort)UnitIndex.UNIT_INDEX_INVALID, (byte)UnitType.UNIT_INFANTRY, enemyHouseID, position, (sbyte)Tools_Random_256());

                        if (u != null) Unit_SetAction(u, ActionType.ACTION_HUNT);
                        break;
                    }

                default: break;
            }
        }

        /*
         * Sets the viewport position.
         *
         * @param packed The packed position.
        */
        internal static void Map_SetViewportPosition(ushort packed)
        {
            short x;
            short y;
            MapInfo mapInfo;

            x = (short)(Tile_GetPackedX(packed) - 7);
            y = (short)(Tile_GetPackedY(packed) - 5);

            mapInfo = g_mapInfos[g_scenario.mapScale];

            x = (short)Math.Max(mapInfo.minX, Math.Min(mapInfo.minX + mapInfo.sizeX - 15, x));
            y = (short)Math.Max(mapInfo.minY, Math.Min(mapInfo.minY + mapInfo.sizeY - 10, y));

            g_viewportPosition = Tile_PackXY((ushort)x, (ushort)y);
        }

        static readonly sbyte[] around = { 0, -1, 1, -16, 16, -17, 17, -15, 15, -2, 2, -32, 32, -4, 4, -64, 64, -30, 30, -34, 34 };
        /*
         * Creates the landscape using the given seed.
         * @param seed The seed.
         */
        internal static void Map_CreateLandscape(uint seed)
        {
            ushort i;
            ushort j;
            ushort k;
            var memory = new byte[273];
            var currentRow = new ushort[64];
            var previousRow = new ushort[64];
            ushort spriteID1;
            ushort spriteID2;
            ushort[] iconMap;

            Tools_Random_Seed(seed);

            /* Place random data on a 4x4 grid. */
            for (i = 0; i < 272; i++)
            {
                memory[i] = (byte)(Tools_Random_256() & 0xF);
                if (memory[i] > 0xA) memory[i] = 0xA;
            }

            i = (ushort)((Tools_Random_256() & 0xF) + 1);
            while (i-- != 0)
            {
                short baseline = Tools_Random_256();

                for (j = 0; j < around.Length; j++)
                {
                    var index = (short)Math.Min(Math.Max(0, baseline + around[j]), 272);

                    memory[index] = (byte)((memory[index] + (Tools_Random_256() & 0xF)) & 0xF);
                }
            }

            i = (ushort)((Tools_Random_256() & 0x3) + 1);
            while (i-- != 0)
            {
                short baseline = Tools_Random_256();

                for (j = 0; j < around.Length; j++)
                {
                    var index = (short)Math.Min(Math.Max(0, baseline + around[j]), 272);

                    memory[index] = (byte)(Tools_Random_256() & 0x3);
                }
            }

            for (j = 0; j < 16; j++)
            {
                for (i = 0; i < 16; i++)
                {
                    g_map[Tile_PackXY((ushort)(i * 4), (ushort)(j * 4))].groundTileID = memory[j * 16 + i];
                }
            }

            /* Average around the 4x4 grid. */
            for (j = 0; j < 16; j++)
            {
                for (i = 0; i < 16; i++)
                {
                    for (k = 0; k < 21; k++)
                    {
                        var offsets = _offsetTable[(i + 1) % 2][k];
                        ushort packed1;
                        ushort packed2;
                        ushort packed;
                        ushort sprite2;

                        packed1 = Tile_PackXY((ushort)(i * 4 + offsets[0]), (ushort)(j * 4 + offsets[1]));
                        packed2 = Tile_PackXY((ushort)(i * 4 + offsets[2]), (ushort)(j * 4 + offsets[3]));
                        packed = (ushort)((packed1 + packed2) / 2);

                        if (Tile_IsOutOfMap(packed)) continue;

                        packed1 = Tile_PackXY((ushort)((i * 4 + offsets[0]) & 0x3F), (ushort)(j * 4 + offsets[1]));
                        packed2 = Tile_PackXY((ushort)((i * 4 + offsets[2]) & 0x3F), (ushort)(j * 4 + offsets[3]));
                        Debug.Assert(packed1 < 64 * 64);

                        /* ENHANCEMENT -- use groundTileID=0 when out-of-bounds to generate the original maps. */
                        if (packed2 < 64 * 64)
                        {
                            sprite2 = g_map[packed2].groundTileID;
                        }
                        else
                        {
                            sprite2 = 0;
                        }

                        g_map[packed].groundTileID = (ushort)((g_map[packed1].groundTileID + sprite2 + 1) / 2);
                    }
                }
            }

            Array.Fill<ushort>(currentRow, 0, 0, 64); //memset(currentRow, 0, 128);

            /* Average each tile with its neighbours. */
            for (j = 0; j < 64; j++)
            {
                var t = g_map[(j * 64)..];
                //Span<Tile> t = g_map.AsSpan(j * 64);

                Buffer.BlockCopy(currentRow, 0, previousRow, 0, 128); //memcpy(previousRow, currentRow, 128);

                for (i = 0; i < 64; i++) currentRow[i] = t[i].groundTileID;

                for (i = 0; i < 64; i++)
                {
                    var neighbours = new ushort[9];
                    ushort total = 0;

                    neighbours[0] = (i == 0 || j == 0) ? currentRow[i] : previousRow[i - 1];
                    neighbours[1] = (j == 0) ? currentRow[i] : previousRow[i];
                    neighbours[2] = (i == 63 || j == 0) ? currentRow[i] : previousRow[i + 1];
                    neighbours[3] = (i == 0) ? currentRow[i] : currentRow[i - 1];
                    neighbours[4] = currentRow[i];
                    neighbours[5] = (i == 63) ? currentRow[i] : currentRow[i + 1];
                    neighbours[6] = (i == 0 || j == 63) ? currentRow[i] : t[i + 63].groundTileID;
                    neighbours[7] = (j == 63) ? currentRow[i] : t[i + 64].groundTileID;
                    neighbours[8] = (i == 63 || j == 63) ? currentRow[i] : t[i + 65].groundTileID;

                    for (k = 0; k < 9; k++) total += neighbours[k];
                    t[i].groundTileID = (ushort)(total / 9);
                }
            }

            /* Filter each tile to determine its final type. */
            spriteID1 = (ushort)(Tools_Random_256() & 0xF);
            if (spriteID1 < 0x8) spriteID1 = 0x8;
            if (spriteID1 > 0xC) spriteID1 = 0xC;

            spriteID2 = (ushort)((Tools_Random_256() & 0x3) - 1);
            if (spriteID2 > spriteID1 - 3) spriteID2 = (ushort)(spriteID1 - 3);

            for (i = 0; i < 4096; i++)
            {
                var spriteID = g_map[i].groundTileID;

                if (spriteID > spriteID1 + 4)
                {
                    spriteID = (ushort)LandscapeType.LST_ENTIRELY_MOUNTAIN;
                }
                else if (spriteID >= spriteID1)
                {
                    spriteID = (ushort)LandscapeType.LST_ENTIRELY_ROCK;
                }
                else if (spriteID <= spriteID2)
                {
                    spriteID = (ushort)LandscapeType.LST_ENTIRELY_DUNE;
                }
                else
                {
                    spriteID = (ushort)LandscapeType.LST_NORMAL_SAND;
                }

                g_map[i].groundTileID = spriteID;
            }

            /* Add some spice. */
            i = (ushort)(Tools_Random_256() & 0x2F);
            while (i-- != 0)
            {
                tile32 tile;
                ushort packed;

                while (true)
                {
                    packed = (ushort)(Tools_Random_256() & 0x3F);
                    packed = Tile_PackXY((ushort)(Tools_Random_256() & 0x3F), packed);

                    if (g_table_landscapeInfo[g_map[packed].groundTileID].canBecomeSpice) break;
                }

                tile = Tile_UnpackTile(packed);

                j = (ushort)(Tools_Random_256() & 0x1F);
                while (j-- != 0)
                {
                    while (true)
                    {
                        //packed = CTile.Tile_PackTile(CTile.Tile_MoveByRandom(tile, (ushort)(Tools.Tools_Random_256() & 0x3F), true));
                        var unpacked = Tile_MoveByRandom(tile, (ushort)(Tools_Random_256() & 0x3F), true);
                        packed = Tile_PackTile(unpacked);

                        if (!Tile_IsOutOfMap(packed)) break;
                    }

                    Map_AddSpiceOnTile(packed);
                }
            }

            /* Make everything smoother and use the right sprite indexes. */
            for (j = 0; j < 64; j++)
            {
                var t = g_map[(j * 64)..];
                //Span<Tile> t = g_map.AsSpan(j * 64);

                Buffer.BlockCopy(currentRow, 0, previousRow, 0, 128); //memcpy(previousRow, currentRow, 128);

                for (i = 0; i < 64; i++) currentRow[i] = t[i].groundTileID;

                for (i = 0; i < 64; i++)
                {
                    var current = t[i].groundTileID;
                    var up = (j == 0) ? current : previousRow[i];
                    var right = (i == 63) ? current : currentRow[i + 1];
                    var down = (j == 63) ? current : t[i + 64].groundTileID;
                    var left = (i == 0) ? current : currentRow[i - 1];
                    ushort spriteID = 0;

                    if (up == current) spriteID |= 1;
                    if (right == current) spriteID |= 2;
                    if (down == current) spriteID |= 4;
                    if (left == current) spriteID |= 8;

                    switch ((LandscapeType)current)
                    {
                        case LandscapeType.LST_NORMAL_SAND:
                            spriteID = 0;
                            break;
                        case LandscapeType.LST_ENTIRELY_ROCK:
                            if (up == (ushort)LandscapeType.LST_ENTIRELY_MOUNTAIN) spriteID |= 1;
                            if (right == (ushort)LandscapeType.LST_ENTIRELY_MOUNTAIN) spriteID |= 2;
                            if (down == (ushort)LandscapeType.LST_ENTIRELY_MOUNTAIN) spriteID |= 4;
                            if (left == (ushort)LandscapeType.LST_ENTIRELY_MOUNTAIN) spriteID |= 8;
                            spriteID++;
                            break;
                        case LandscapeType.LST_ENTIRELY_DUNE:
                            spriteID += 17;
                            break;
                        case LandscapeType.LST_ENTIRELY_MOUNTAIN:
                            spriteID += 33;
                            break;
                        case LandscapeType.LST_SPICE:
                            if (up == (ushort)LandscapeType.LST_THICK_SPICE) spriteID |= 1;
                            if (right == (ushort)LandscapeType.LST_THICK_SPICE) spriteID |= 2;
                            if (down == (ushort)LandscapeType.LST_THICK_SPICE) spriteID |= 4;
                            if (left == (ushort)LandscapeType.LST_THICK_SPICE) spriteID |= 8;
                            spriteID += 49;
                            break;
                        case LandscapeType.LST_THICK_SPICE:
                            spriteID += 65;
                            break;
                        default: break;
                    }

                    t[i].groundTileID = spriteID;
                }
            }

            /* Finalise the tiles with the real sprites. */
            iconMap = g_iconMap[g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_LANDSCAPE]..];

            for (i = 0; i < 4096; i++)
            {
                var t = g_map[i];

                t.groundTileID = iconMap[t.groundTileID];
                t.overlayTileID = g_veiledTileID;
                t.houseID = (byte)HouseType.HOUSE_HARKONNEN;
                t.isUnveiled = false;
                t.hasUnit = false;
                t.hasStructure = false;
                t.hasAnimation = false;
                t.hasExplosion = false;
                t.index = 0;
            }

            for (i = 0; i < 4096; i++)
                g_mapTileID[i] = g_map[i].groundTileID;
        }

        /*
         * Add spice on the given tile.
         * @param packed The tile.
         */
        static void Map_AddSpiceOnTile(ushort packed)
        {
            Tile t;

            t = g_map[packed];

            switch ((LandscapeType)t.groundTileID)
            {
                case LandscapeType.LST_SPICE:
                    t.groundTileID = (ushort)LandscapeType.LST_THICK_SPICE;
                    Map_AddSpiceOnTile(packed);
                    return;

                case LandscapeType.LST_THICK_SPICE:
                    {
                        sbyte i;
                        sbyte j;

                        for (j = -1; j <= 1; j++)
                        {
                            for (i = -1; i <= 1; i++)
                            {
                                Tile t2;
                                var packed2 = Tile_PackXY((ushort)(Tile_GetPackedX(packed) + i), (ushort)(Tile_GetPackedY(packed) + j));

                                if (Tile_IsOutOfMap(packed2)) continue;

                                t2 = g_map[packed2];

                                if (!g_table_landscapeInfo[t2.groundTileID].canBecomeSpice)
                                {
                                    t.groundTileID = (ushort)LandscapeType.LST_SPICE;
                                    continue;
                                }

                                if (t2.groundTileID != (ushort)LandscapeType.LST_THICK_SPICE) t2.groundTileID = (ushort)LandscapeType.LST_SPICE;
                            }
                        }
                        return;
                    }

                default:
                    if (g_table_landscapeInfo[t.groundTileID].canBecomeSpice) t.groundTileID = (ushort)LandscapeType.LST_SPICE;
                    return;
            }
        }
    }
}
