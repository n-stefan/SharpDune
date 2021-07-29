/* Structure handling */

using SharpDune.Audio;
using SharpDune.Gui;
using SharpDune.Include;
using SharpDune.Pool;
using System;
using System.Diagnostics;
using static SharpDune.Script.Script;
using static SharpDune.Table.TableAnimation;
using static SharpDune.Table.TableHouseInfo;
using static SharpDune.Table.TableLandscapeInfo;
using static SharpDune.Table.TableStrings;
using static SharpDune.Table.TableStructureInfo;
using static SharpDune.Table.TableTileDiff;
using static SharpDune.Table.TableUnitInfo;
using static System.Math;

namespace SharpDune
{
    /*
    * Types of Structures available in the game.
    */
    enum StructureType
    {
        STRUCTURE_SLAB_1x1 = 0,
        STRUCTURE_SLAB_2x2 = 1,
        STRUCTURE_PALACE = 2,
        STRUCTURE_LIGHT_VEHICLE = 3,
        STRUCTURE_HEAVY_VEHICLE = 4,
        STRUCTURE_HIGH_TECH = 5,
        STRUCTURE_HOUSE_OF_IX = 6,
        STRUCTURE_WOR_TROOPER = 7,
        STRUCTURE_CONSTRUCTION_YARD = 8,
        STRUCTURE_WINDTRAP = 9,
        STRUCTURE_BARRACKS = 10,
        STRUCTURE_STARPORT = 11,
        STRUCTURE_REFINERY = 12,
        STRUCTURE_REPAIR = 13,
        STRUCTURE_WALL = 14,
        STRUCTURE_TURRET = 15,
        STRUCTURE_ROCKET_TURRET = 16,
        STRUCTURE_SILO = 17,
        STRUCTURE_OUTPOST = 18,

        STRUCTURE_MAX = 19,
        STRUCTURE_INVALID = 0xFF
    }

    /* Available structure layouts. */
    enum StructureLayout
    {
        STRUCTURE_LAYOUT_1x1 = 0,
        STRUCTURE_LAYOUT_2x1 = 1,
        STRUCTURE_LAYOUT_1x2 = 2,
        STRUCTURE_LAYOUT_2x2 = 3,
        STRUCTURE_LAYOUT_2x3 = 4,
        STRUCTURE_LAYOUT_3x2 = 5,
        STRUCTURE_LAYOUT_3x3 = 6,

        STRUCTURE_LAYOUT_MAX = 7
    }

    /* States a structure can be in */
    enum StructureState
    {
        STRUCTURE_STATE_DETECT = -2,                            /*!< Used when setting state, meaning to detect which state it has by looking at other properties. */
        STRUCTURE_STATE_JUSTBUILT = -1,                         /*!< This shows you the building animation etc. */
        STRUCTURE_STATE_IDLE = 0,                               /*!< Structure is doing nothing. */
        STRUCTURE_STATE_BUSY = 1,                               /*!< Structure is busy (harvester in refinery, unit in repair, .. */
        STRUCTURE_STATE_READY = 2                               /*!< Structure is ready and unit will be deployed soon. */
    }

    /*
     * Flags used to indicate structures in a bitmask.
     */
    [Flags]
    enum StructureFlag
    {
        FLAG_STRUCTURE_SLAB_1x1 = 1 << StructureType.STRUCTURE_SLAB_1x1,          /* 0x____01 */
        FLAG_STRUCTURE_SLAB_2x2 = 1 << StructureType.STRUCTURE_SLAB_2x2,          /* 0x____02 */
        FLAG_STRUCTURE_PALACE = 1 << StructureType.STRUCTURE_PALACE,            /* 0x____04 */
        FLAG_STRUCTURE_LIGHT_VEHICLE = 1 << StructureType.STRUCTURE_LIGHT_VEHICLE,     /* 0x____08 */
        FLAG_STRUCTURE_HEAVY_VEHICLE = 1 << StructureType.STRUCTURE_HEAVY_VEHICLE,     /* 0x____10 */
        FLAG_STRUCTURE_HIGH_TECH = 1 << StructureType.STRUCTURE_HIGH_TECH,         /* 0x____20 */
        FLAG_STRUCTURE_HOUSE_OF_IX = 1 << StructureType.STRUCTURE_HOUSE_OF_IX,       /* 0x____40 */
        FLAG_STRUCTURE_WOR_TROOPER = 1 << StructureType.STRUCTURE_WOR_TROOPER,       /* 0x____80 */
        FLAG_STRUCTURE_CONSTRUCTION_YARD = 1 << StructureType.STRUCTURE_CONSTRUCTION_YARD, /* 0x__01__ */
        FLAG_STRUCTURE_WINDTRAP = 1 << StructureType.STRUCTURE_WINDTRAP,          /* 0x__02__ */
        FLAG_STRUCTURE_BARRACKS = 1 << StructureType.STRUCTURE_BARRACKS,          /* 0x__04__ */
        FLAG_STRUCTURE_STARPORT = 1 << StructureType.STRUCTURE_STARPORT,          /* 0x__08__ */
        FLAG_STRUCTURE_REFINERY = 1 << StructureType.STRUCTURE_REFINERY,          /* 0x__10__ */
        FLAG_STRUCTURE_REPAIR = 1 << StructureType.STRUCTURE_REPAIR,            /* 0x__20__ */
        FLAG_STRUCTURE_WALL = 1 << StructureType.STRUCTURE_WALL,              /* 0x__40__ */
        FLAG_STRUCTURE_TURRET = 1 << StructureType.STRUCTURE_TURRET,            /* 0x__80__ */
        FLAG_STRUCTURE_ROCKET_TURRET = 1 << StructureType.STRUCTURE_ROCKET_TURRET,     /* 0x01____ */
        FLAG_STRUCTURE_SILO = 1 << StructureType.STRUCTURE_SILO,              /* 0x02____ */
        FLAG_STRUCTURE_OUTPOST = 1 << StructureType.STRUCTURE_OUTPOST,           /* 0x04____ */

        FLAG_STRUCTURE_NONE = 0,
        FLAG_STRUCTURE_NEVER = -1                                /*!< Special flag to mark that certain buildings can never be built on a Construction Yard. */
    }

    /*
    * A Structure as stored in the memory.
    */
    class Structure
    {
        internal Object o;                                      /*!< Common to Unit and Structures. */
        internal ushort creatorHouseID;                         /*!< The Index of the House who created this Structure. Required in case of take-overs. */
        internal ushort rotationSpriteDiff;                     /*!< Which sprite to show for the current rotation of Turrets etc. */
        internal ushort objectType;                             /*!< Type of Unit/Structure we are building. */
        internal byte upgradeLevel;                             /*!< The current level of upgrade of the Structure. */
        internal byte upgradeTimeLeft;                          /*!< Time left before upgrade is complete, or 0 if no upgrade available. */
        internal ushort countDown;                              /*!< General countdown for various of functions. */
        internal ushort buildCostRemainder;                     /*!< The remainder of the buildCost for next tick. */
        internal short state;                                   /*!< The state of the structure. @see StructureState. */
        internal ushort hitpointsMax;                           /*!< Max amount of hitpoints. */

        internal Structure() =>
            o = new Object();
    }

    /*
     * Static information per Structure type.
     */
    class StructureInfo
    {
        internal ObjectInfo o;                                  /*!< Common to UnitInfo and StructureInfo. */
        internal uint enterFilter;                              /*!< Bitfield determining which unit is allowed to enter the structure. If bit n is set, then units of type n may enter */
        internal ushort creditsStorage;                         /*!< How many credits this Structure can store. */
        internal short powerUsage;                              /*!< How much power this Structure uses (positive value) or produces (negative value). */
        internal ushort layout;                                 /*!< Layout type of Structure. */
        internal ushort iconGroup;                              /*!< In which IconGroup the sprites of the Structure belongs. */
        internal byte[] animationIndex = new byte[3];           /*!< The index inside g_table_animation_structure for the Animation of the Structure. */
        internal byte[] buildableUnits = new byte[8];           /*!< Which units this structure can produce. */
        internal ushort[] upgradeCampaign = new ushort[3];      /*!< Minimum campaign for upgrades. */
    }

    /* X/Y pair defining a 2D size. */
    class XYSize
    {
        internal ushort width;  /*!< Horizontal length. */
        internal ushort height; /*!< Vertical length. */
    }

    class CStructure
    {
        internal static ushort g_structureActivePosition;
        internal static ushort g_structureActiveType;

        internal static Structure g_structureActive;

        static readonly bool s_debugInstantBuild; /*!< When non-zero, constructions are almost instant. */
        static uint s_tickStructureDegrade; /*!< Indicates next time Degrade function is executed. */
        static uint s_tickStructureStructure; /*!< Indicates next time Structures function is executed. */
        static uint s_tickStructureScript; /*!< Indicates next time Script function is executed. */
        static uint s_tickStructurePalace; /*!< Indicates next time Palace function is executed. */

        internal static ushort g_structureIndex;

        /*
         * Checks if the given position is a valid location for the given structure type.
         *
         * @param position The (packed) tile to check.
         * @param type The structure type to check the position for.
         * @return 0 if the position is not valid, 1 if the position is valid and have enough slabs, <0 if the position is valid but miss some slabs.
         */
        internal static short Structure_IsValidBuildLocation(ushort position, StructureType type)
        {
            StructureInfo si;
            ushort[] layoutTile;
            byte i;
            ushort neededSlabs;
            bool isValid;
            ushort curPos;

            si = g_table_structureInfo[(int)type];
            layoutTile = g_table_structure_layoutTiles[si.layout];

            isValid = true;
            neededSlabs = 0;
            for (i = 0; i < g_table_structure_layoutTileCount[si.layout]; i++)
            {
                ushort lst;

                curPos = (ushort)(position + layoutTile[i]);

                lst = Map.Map_GetLandscapeType(curPos);

                if (CSharpDune.g_debugScenario)
                {
                    if (!g_table_landscapeInfo[lst].isValidForStructure2)
                    {
                        isValid = false;
                        break;
                    }
                }
                else
                {
                    if (!Map.Map_IsValidPosition(curPos))
                    {
                        isValid = false;
                        break;
                    }

                    if (si.o.flags.notOnConcrete)
                    {
                        if (!g_table_landscapeInfo[lst].isValidForStructure2 && CSharpDune.g_validateStrictIfZero == 0)
                        {
                            isValid = false;
                            break;
                        }
                    }
                    else
                    {
                        if (!g_table_landscapeInfo[lst].isValidForStructure && CSharpDune.g_validateStrictIfZero == 0)
                        {
                            isValid = false;
                            break;
                        }
                        if (lst != (ushort)LandscapeType.LST_CONCRETE_SLAB) neededSlabs++;
                    }
                }

                if (CObject.Object_GetByPackedTile(curPos) != null)
                {
                    isValid = false;
                    break;
                }
            }

            if (CSharpDune.g_validateStrictIfZero == 0 && isValid && type != StructureType.STRUCTURE_CONSTRUCTION_YARD && !CSharpDune.g_debugScenario)
            {
                isValid = false;
                for (i = 0; i < 16; i++)
                {
                    ushort offset, lst;
                    Structure s;

                    offset = (ushort)g_table_structure_layoutTilesAround[si.layout][i];
                    if (offset == 0) break;

                    curPos = (ushort)(position + offset);
                    s = Structure_Get_ByPackedTile(curPos);
                    if (s != null)
                    {
                        if (s.o.houseID != (byte)CHouse.g_playerHouseID) continue;
                        isValid = true;
                        break;
                    }

                    lst = Map.Map_GetLandscapeType(curPos);
                    if (lst != (ushort)LandscapeType.LST_CONCRETE_SLAB && lst != (ushort)LandscapeType.LST_WALL) continue;
                    if (Map.g_map[curPos].houseID != (byte)CHouse.g_playerHouseID) continue;

                    isValid = true;
                    break;
                }
            }

            if (!isValid) return 0;
            if (neededSlabs == 0) return 1;
            return (short)-neededSlabs;
        }

        /*
         * Get the structure on the given packed tile.
         *
         * @param packed The packed tile to get the structure from.
         * @return The structure.
         */
        internal static Structure Structure_Get_ByPackedTile(ushort packed)
        {
            Tile tile;

            if (CTile.Tile_IsOutOfMap(packed)) return null;

            tile = Map.g_map[packed];
            if (!tile.hasStructure) return null;
            return PoolStructure.Structure_Get_ByIndex((ushort)(tile.index - 1));
        }

        /*
         * Update the map with the right data for this structure.
         * @param s The structure to update on the map.
         */
        internal static void Structure_UpdateMap(Structure s)
        {
            StructureInfo si;
            ushort layoutSize;
            ushort[] layout;
            ushort[] iconMap;
            int i;

            if (s == null) return;
            if (!s.o.flags.used) return;
            if (s.o.flags.isNotOnMap) return;

            si = g_table_structureInfo[s.o.type];

            layout = g_table_structure_layoutTiles[si.layout];
            layoutSize = g_table_structure_layoutTileCount[si.layout];

            iconMap = Sprites.g_iconMap[(Sprites.g_iconMap[si.iconGroup] + layoutSize + layoutSize)..];

            for (i = 0; i < layoutSize; i++)
            {
                ushort position;
                Tile t;

                position = (ushort)(CTile.Tile_PackTile(s.o.position) + layout[i]);

                t = Map.g_map[position];
                t.houseID = s.o.houseID;
                t.hasStructure = true;
                t.index = (ushort)(s.o.index + 1);

                t.groundTileID = (ushort)(iconMap[i] + s.rotationSpriteDiff);

                if (Sprites.Tile_IsUnveiled(t.overlayTileID)) t.overlayTileID = 0;

                Map.Map_Update(position, 0, false);
            }

            s.o.flags.isDirty = true;

            if (s.state >= (short)StructureState.STRUCTURE_STATE_IDLE)
            {
                var animationIndex = (ushort)((s.state > (short)StructureState.STRUCTURE_STATE_READY) ? (short)StructureState.STRUCTURE_STATE_READY : s.state);

                if (si.animationIndex[animationIndex] == 0xFF)
                {
                    CAnimation.Animation_Start(null, s.o.position, si.layout, s.o.houseID, (byte)si.iconGroup);
                }
                else
                {
                    var animationID = si.animationIndex[animationIndex];

                    Debug.Assert(animationID < 29);
                    CAnimation.Animation_Start(g_table_animation_structure[animationID], s.o.position, si.layout, s.o.houseID, (byte)si.iconGroup);
                }
            }
            else
            {
                CAnimation.Animation_Start(g_table_animation_structure[1], s.o.position, si.layout, s.o.houseID, (byte)si.iconGroup);
            }
        }

        /*
         * Check wether the given structure is upgradable.
         *
         * @param s The Structure to check.
         * @return True if and only if the structure is upgradable.
         */
        internal static bool Structure_IsUpgradable(Structure s)
        {
            StructureInfo si;

            if (s == null) return false;

            si = g_table_structureInfo[s.o.type];

            if (s.o.houseID == (byte)HouseType.HOUSE_HARKONNEN && s.o.type == (byte)StructureType.STRUCTURE_HIGH_TECH) return false;
            if (s.o.houseID == (byte)HouseType.HOUSE_ORDOS && s.o.type == (byte)StructureType.STRUCTURE_HEAVY_VEHICLE && s.upgradeLevel == 1 && si.upgradeCampaign[2] > CSharpDune.g_campaignID) return false;

            if (s.upgradeLevel < si.upgradeCampaign.Length && si.upgradeCampaign[s.upgradeLevel] != 0 && si.upgradeCampaign[s.upgradeLevel] <= CSharpDune.g_campaignID + 1)
            {
                House h;

                if (s.o.type != (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD) return true;
                if (s.upgradeLevel != 1) return true;

                h = PoolHouse.House_Get_ByIndex(s.o.houseID);
                if ((h.structuresBuilt & g_table_structureInfo[(int)StructureType.STRUCTURE_ROCKET_TURRET].o.structuresRequired) == g_table_structureInfo[(int)StructureType.STRUCTURE_ROCKET_TURRET].o.structuresRequired) return true;

                return false;
            }

            if (s.o.houseID == (byte)HouseType.HOUSE_HARKONNEN && s.o.type == (byte)StructureType.STRUCTURE_WOR_TROOPER && s.upgradeLevel == 0 && CSharpDune.g_campaignID > 3) return true;
            return false;
        }

        /*
         * Get the unit linked to this structure, or NULL if there is no.
         * @param s The structure to get the linked unit from.
         * @return The linked unit, or NULL if there was none.
         */
        internal static Unit Structure_GetLinkedUnit(Structure s)
        {
            if (s.o.linkedID == 0xFF) return null;
            return PoolUnit.Unit_Get_ByIndex(s.o.linkedID);
        }

        /*
         * Set the state for the given structure.
         *
         * @param s The structure to set the state of.
         * @param state The new sate value.
         */
        internal static void Structure_SetState(Structure s, short state)
        {
            if (s == null) return;
            s.state = state;

            Structure_UpdateMap(s);
        }

        /*
         * The house is under attack in the form of a structure being hit.
         * @param houseID The house who is being attacked.
         */
        internal static void Structure_HouseUnderAttack(byte houseID)
        {
            var find = new PoolFindStruct();
            House h;

            h = PoolHouse.House_Get_ByIndex(houseID);

            if (houseID != (byte)CHouse.g_playerHouseID && h.flags.doneFullScaleAttack) return;
            h.flags.doneFullScaleAttack = true;

            if (h.flags.human)
            {
                if (h.timerStructureAttack != 0) return;

                Sound.Sound_Output_Feedback(48);

                h.timerStructureAttack = 8;
                return;
            }

            /* ENHANCEMENT -- Dune2 originally only searches for units with type 0 (Carry-all). In result, the rest of this function does nothing. */
            if (!CSharpDune.g_dune2_enhanced) return;

            find.houseID = houseID;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            while (true)
            {
                UnitInfo ui;
                Unit u;

                u = PoolUnit.Unit_Find(find);
                if (u == null) break;

                ui = g_table_unitInfo[u.o.type];

                if (ui.bulletType == (byte)UnitType.UNIT_INVALID) continue;

                /* XXX -- Dune2 does something odd here. What was their intention? */
                if ((u.actionID == (byte)ActionType.ACTION_GUARD && u.actionID == (byte)ActionType.ACTION_AMBUSH) || u.actionID == (byte)ActionType.ACTION_AREA_GUARD) CUnit.Unit_SetAction(u, ActionType.ACTION_HUNT);
            }
        }

        /*
         * Damage the structure, and bring the surrounding to an explosion if needed.
         *
         * @param s The structure to damage.
         * @param damage The damage to deal to the structure.
         * @param range The range in which an explosion should be possible.
         * @return True if and only if the structure is now destroyed.
         */
        internal static bool Structure_Damage(Structure s, ushort damage, ushort range)
        {
            StructureInfo si;

            if (s == null) return false;
            if (damage == 0) return false;
            if (s.o.script.variables[0] == 1) return false;

            si = g_table_structureInfo[s.o.type];

            if (s.o.hitpoints >= damage)
            {
                s.o.hitpoints -= damage;
            }
            else
            {
                s.o.hitpoints = 0;
            }

            if (s.o.hitpoints == 0)
            {
                ushort score;

                score = (ushort)(si.o.buildCredits / 100);
                if (score < 1) score = 1;

                if (CHouse.House_AreAllied((byte)CHouse.g_playerHouseID, s.o.houseID))
                {
                    CScenario.g_scenario.destroyedAllied++;
                    CScenario.g_scenario.score -= score;
                }
                else
                {
                    CScenario.g_scenario.destroyedEnemy++;
                    CScenario.g_scenario.score += score;
                }

                Structure_Destroy(s);

                if ((byte)CHouse.g_playerHouseID == s.o.houseID)
                {
                    ushort index;

                    switch ((HouseType)s.o.houseID)
                    {
                        case HouseType.HOUSE_HARKONNEN: index = 22; break;
                        case HouseType.HOUSE_ATREIDES: index = 23; break;
                        case HouseType.HOUSE_ORDOS: index = 24; break;
                        default: index = 0xFFFF; break;
                    }

                    Sound.Sound_Output_Feedback(index);
                }
                else
                {
                    Sound.Sound_Output_Feedback(21);
                }

                Structure_UntargetMe(s);
                return true;
            }

            if (range == 0) return false;

            Map.Map_MakeExplosion((ushort)ExplosionType.EXPLOSION_IMPACT_LARGE, CTile.Tile_AddTileDiff(s.o.position, g_table_structure_layoutTileDiff[si.layout]), 0, 0);
            return false;
        }

        /*
         * Untarget the given Structure.
         *
         * @param unit The Structure to untarget.
         */
        internal static void Structure_UntargetMe(Structure s)
        {
            var find = new PoolFindStruct();
            var encoded = Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);

            CObject.Object_Script_Variable4_Clear(s.o);

            find.houseID = (byte)HouseType.HOUSE_INVALID;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            while (true)
            {
                Unit u;

                u = PoolUnit.Unit_Find(find);
                if (u == null) break;

                if (u.targetMove == encoded) u.targetMove = 0;
                if (u.targetAttack == encoded) u.targetAttack = 0;
                if (u.o.script.variables[4] == encoded) CObject.Object_Script_Variable4_Clear(u.o);
            }

            find.houseID = (byte)HouseType.HOUSE_INVALID;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            while (true)
            {
                Team t;

                t = PoolTeam.Team_Find(find);
                if (t == null) break;

                if (t.target == encoded) t.target = 0;
            }
        }

        /*
         * Handles destroying of a structure.
         *
         * @param s The Structure.
         */
        static void Structure_Destroy(Structure s)
        {
            StructureInfo si;
            byte linkedID;
            House h;

            if (s == null) return;

            if (CSharpDune.g_debugScenario)
            {
                Structure_Remove(s);
                return;
            }

            s.o.script.variables[0] = 1;
            s.o.flags.allocated = false;
            s.o.flags.repairing = false;
            s.o.script.delay = 0;

            Script_Reset(s.o.script, g_scriptStructure);
            Script_Load(s.o.script, s.o.type);

            Sound.Voice_PlayAtTile(44, s.o.position);

            linkedID = s.o.linkedID;

            if (linkedID != 0xFF)
            {
                if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
                {
                    Structure_Destroy(PoolStructure.Structure_Get_ByIndex(linkedID));
                    s.o.linkedID = 0xFF;
                }
                else
                {
                    while (linkedID != 0xFF)
                    {
                        var u = PoolUnit.Unit_Get_ByIndex(linkedID);

                        linkedID = u.o.linkedID;

                        CUnit.Unit_Remove(u);
                    }
                }
            }

            h = PoolHouse.House_Get_ByIndex(s.o.houseID);
            si = g_table_structureInfo[s.o.type];

            h.credits -= (ushort)((h.creditsStorage == 0) ? h.credits : Min(h.credits, (h.credits * 256 / h.creditsStorage) * si.creditsStorage / 256));

            if (s.o.houseID != (byte)CHouse.g_playerHouseID) h.credits += (ushort)(si.o.buildCredits + (CSharpDune.g_campaignID > 7 ? si.o.buildCredits / 2 : 0));

            if (s.o.type != (byte)StructureType.STRUCTURE_WINDTRAP) return;

            h.windtrapCount--;
        }

        /*
         * Remove the structure from the map, free it, and clean up after it.
         * @param s The structure to remove.
         */
        internal static void Structure_Remove(Structure s)
        {
            StructureInfo si;
            ushort packed;
            ushort i;
            House h;

            if (s == null) return;

            si = g_table_structureInfo[s.o.type];
            packed = CTile.Tile_PackTile(s.o.position);

            for (i = 0; i < g_table_structure_layoutTileCount[si.layout]; i++)
            {
                Tile t;
                var curPacked = (ushort)(packed + g_table_structure_layoutTiles[si.layout][i]);

                CAnimation.Animation_Stop_ByTile(curPacked);

                t = Map.g_map[curPacked];
                t.hasStructure = false;

                if (CSharpDune.g_debugScenario)
                {
                    t.groundTileID = (ushort)(Map.g_mapTileID[curPacked] & 0x1FF);
                    t.overlayTileID = 0;
                }
            }

            if (!CSharpDune.g_debugScenario)
            {
                CAnimation.Animation_Start(g_table_animation_structure[0], s.o.position, si.layout, s.o.houseID, (byte)si.iconGroup);
            }

            h = PoolHouse.House_Get_ByIndex(s.o.houseID);

            for (i = 0; i < 5; i++)
            {
                if (h.ai_structureRebuild[i][0] != 0) continue;
                h.ai_structureRebuild[i][0] = s.o.type;
                h.ai_structureRebuild[i][1] = packed;
                break;
            }

            PoolStructure.Structure_Free(s);
            Structure_UntargetMe(s);

            h.structuresBuilt = Structure_GetStructuresBuilt(h);

            CHouse.House_UpdateCreditsStorage(s.o.houseID);

            if (CSharpDune.g_debugScenario) return;

            switch ((StructureType)s.o.type)
            {
                case StructureType.STRUCTURE_WINDTRAP:
                    CHouse.House_CalculatePowerAndCredit(h);
                    break;

                case StructureType.STRUCTURE_OUTPOST:
                    CHouse.House_UpdateRadarState(h);
                    break;

                default: break;
            }
        }

        /*
         * Get a bitmask of all built structure types for the given House.
         *
         * @param h The house to get built structures for.
         * @return The bitmask.
         */
        internal static uint Structure_GetStructuresBuilt(House h)
        {
            var find = new PoolFindStruct();
            uint result;

            if (h == null) return 0;

            result = 0;
            find.houseID = h.index;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            /* Recount windtraps after capture or loading old saved games. */
            h.windtrapCount = 0;

            while (true)
            {
                Structure s;

                s = PoolStructure.Structure_Find(find);
                if (s == null) break;
                if (s.o.flags.isNotOnMap) continue;
                if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;
                result |= (uint)(1 << s.o.type);

                if (s.o.type == (byte)StructureType.STRUCTURE_WINDTRAP) h.windtrapCount++;
            }

            return result;
        }

        static readonly byte[] wall = {
             0,  3,  1,  2,  3,  3,  4,  5,  1,  6,  1,  7,  8,  9, 10, 11,
             1, 12,  1, 19,  1, 16,  1, 31,  1, 28,  1, 52,  1, 45,  1, 59,
             3,  3, 13, 20,  3,  3, 22, 32,  3,  3, 13, 53,  3,  3, 38, 60,
             5,  6,  7, 21,  5,  6,  7, 33,  5,  6,  7, 54,  5,  6,  7, 61,
             9,  9,  9,  9, 17, 17, 23, 34,  9,  9,  9,  9, 25, 46, 39, 62,
            11, 12, 11, 12, 13, 18, 13, 35, 11, 12, 11, 12, 13, 47, 13, 63,
            15, 15, 16, 16, 17, 17, 24, 36, 15, 15, 16, 16, 17, 17, 40, 64,
            19, 20, 21, 22, 23, 24, 25, 37, 19, 20, 21, 22, 23, 24, 25, 65,
            27, 27, 27, 27, 27, 27, 27, 27, 14, 29, 14, 55, 26, 48, 41, 66,
            29, 30, 29, 30, 29, 30, 29, 30, 31, 30, 31, 56, 31, 49, 31, 67,
            33, 33, 34, 34, 33, 33, 34, 34, 35, 35, 15, 57, 35, 35, 42, 68,
            37, 38, 39, 40, 37, 38, 39, 40, 41, 42, 43, 58, 41, 42, 43, 69,
            45, 45, 45, 45, 46, 46, 46, 46, 47, 47, 47, 47, 27, 50, 43, 70,
            49, 50, 49, 50, 51, 52, 51, 52, 53, 54, 53, 54, 55, 51, 55, 71,
            57, 57, 58, 58, 59, 59, 60, 60, 61, 61, 62, 62, 63, 63, 44, 72,
            65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 73
        };
        /*
         * Connect walls around the given position.
         *
         * @param position The packed position.
         * @param recurse Wether to recurse.
         * @return True if and only if a change happened.
         */
        internal static bool Structure_ConnectWall(ushort position, bool recurse)
        {
            ushort bits = 0;
            ushort tileID;
            bool isDestroyedWall;
            byte i;
            Tile tile;

            isDestroyedWall = Map.Map_GetLandscapeType(position) == (ushort)LandscapeType.LST_DESTROYED_WALL;

            for (i = 0; i < 4; i++)
            {
                var curPos = (ushort)(position + g_table_mapDiff[i]);

                if (recurse && Map.Map_GetLandscapeType(curPos) == (ushort)LandscapeType.LST_WALL) Structure_ConnectWall(curPos, false);

                if (isDestroyedWall) continue;

                var landscapeType = (LandscapeType)Map.Map_GetLandscapeType(curPos);
                if (landscapeType == LandscapeType.LST_DESTROYED_WALL)
                    bits |= (ushort)(1 << (i + 4));
                if (landscapeType == LandscapeType.LST_WALL)
                    bits |= (ushort)(1 << i);

                //switch (Map_GetLandscapeType(curPos)) {
                //    case LST_DESTROYED_WALL: bits |= (1 << (i + 4));
                //        /* FALL-THROUGH */
                //    case LST_WALL: bits |= (1 << i);
                //        /* FALL-THROUGH */
                //    default:  break;
                //}
            }

            if (isDestroyedWall) return false;

            tileID = (ushort)(Sprites.g_wallTileID + wall[bits] + 1);

            tile = Map.g_map[position];
            if (tile.groundTileID == tileID) return false;

            tile.groundTileID = tileID;
            Map.g_mapTileID[position] |= 0x8000;
            Map.Map_Update(position, 0, false);

            return true;
        }

        /*
         * Convert the name of a structure to the type value of that structure, or
         *  STRUCTURE_INVALID if not found.
         */
        internal static byte Structure_StringToType(string name)
        {
            byte type;
            if (name == null) return (byte)StructureType.STRUCTURE_INVALID;

            for (type = 0; type < (byte)StructureType.STRUCTURE_MAX; type++)
            {
                if (string.Equals(g_table_structureInfo[type].o.name, name, StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(g_table_structureInfo[type].o.name, name) == 0)
                    return type;
            }

            return (byte)StructureType.STRUCTURE_INVALID;
        }

        /*
         * Remove the fog around a structure.
         *
         * @param s The Structure.
         */
        internal static void Structure_RemoveFog(Structure s)
        {
            StructureInfo si;
            tile32 position;

            if (s == null || s.o.houseID != (byte)CHouse.g_playerHouseID) return;

            si = g_table_structureInfo[s.o.type];

            position = s.o.position;

            /* ENHANCEMENT -- Fog is removed around the top left corner instead of the center of a structure. */
            if (CSharpDune.g_dune2_enhanced)
            {
                position.x += (ushort)(256 * (g_table_structure_layoutSize[si.layout].width - 1) / 2);
                position.y += (ushort)(256 * (g_table_structure_layoutSize[si.layout].height - 1) / 2);
            }

            CTile.Tile_RemoveFogInRadius(position, si.o.fogUncoverRadius);
        }

        /*
         * Loop over all structures, preforming various of tasks.
         */
        internal static void GameLoop_Structure()
        {
            var find = new PoolFindStruct();
            var tickDegrade = false;
            var tickStructure = false;
            var tickScript = false;
            var tickPalace = false;

            if (s_tickStructureDegrade <= Timer.g_timerGame && CSharpDune.g_campaignID > 1)
            {
                tickDegrade = true;
                s_tickStructureDegrade = Timer.g_timerGame + Tools.Tools_AdjustToGameSpeed(10800, 5400, 21600, true);
            }

            if (s_tickStructureStructure <= Timer.g_timerGame || s_debugInstantBuild)
            {
                tickStructure = true;
                s_tickStructureStructure = Timer.g_timerGame + Tools.Tools_AdjustToGameSpeed(30, 15, 60, true);
            }

            if (s_tickStructureScript <= Timer.g_timerGame)
            {
                tickScript = true;
                s_tickStructureScript = Timer.g_timerGame + 5;
            }

            if (s_tickStructurePalace <= Timer.g_timerGame)
            {
                tickPalace = true;
                s_tickStructurePalace = Timer.g_timerGame + 60;
            }

            find.houseID = (byte)HouseType.HOUSE_INVALID;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            if (CSharpDune.g_debugScenario) return;

            while (true)
            {
                StructureInfo si;
                HouseInfo hi;
                Structure s;
                House h;

                s = PoolStructure.Structure_Find(find);
                if (s == null) break;
                if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;

                si = g_table_structureInfo[s.o.type];
                h = PoolHouse.House_Get_ByIndex(s.o.houseID);
                hi = g_table_houseInfo[h.index];

                g_scriptCurrentObject = s.o;
                g_scriptCurrentStructure = s;
                g_scriptCurrentUnit = null;
                g_scriptCurrentTeam = null;

                if (tickPalace && s.o.type == (byte)StructureType.STRUCTURE_PALACE)
                {
                    if (s.countDown != 0)
                    {
                        s.countDown--;

                        if (s.o.houseID == (byte)CHouse.g_playerHouseID)
                        {
                            WidgetDraw.GUI_Widget_ActionPanel_Draw(true);
                        }
                    }

                    /* Check if we have to fire the weapon for the AI immediately */
                    if (s.countDown == 0 && !h.flags.human && h.flags.isAIActive)
                    {
                        Structure_ActivateSpecial(s);
                    }
                }

                if (tickDegrade && s.o.flags.degrades && s.o.hitpoints > si.o.hitpoints / 2)
                {
                    Structure_Damage(s, hi.degradingAmount, 0);
                }

                if (tickStructure)
                {
                    if (s.o.flags.upgrading)
                    {
                        var upgradeCost = (ushort)(si.o.buildCredits / 40);

                        if (upgradeCost <= h.credits)
                        {
                            h.credits -= upgradeCost;

                            if (s.upgradeTimeLeft > 5)
                            {
                                s.upgradeTimeLeft -= 5;
                            }
                            else
                            {
                                s.upgradeLevel++;
                                s.o.flags.upgrading = false;

                                /* Ordos Heavy Vehicle gets the last upgrade for free */
                                if (s.o.houseID == (byte)HouseType.HOUSE_ORDOS && s.o.type == (byte)StructureType.STRUCTURE_HEAVY_VEHICLE && s.upgradeLevel == 2) s.upgradeLevel = 3;

                                s.upgradeTimeLeft = (byte)(Structure_IsUpgradable(s) ? 100 : 0);
                            }
                        }
                        else
                        {
                            s.o.flags.upgrading = false;
                        }
                    }
                    else if (s.o.flags.repairing)
                    {
                        ushort repairCost;

                        /* ENHANCEMENT -- The calculation of the repaircost is a bit unfair in Dune2, because of rounding errors (they use a 256 float-resolution, which is not sufficient) */
                        if (CSharpDune.g_dune2_enhanced)
                        {
                            repairCost = (ushort)(si.o.buildCredits * 2 / si.o.hitpoints);
                        }
                        else
                        {
                            repairCost = (ushort)(((2 * 256 / si.o.hitpoints) * si.o.buildCredits + 128) / 256);
                        }

                        if (repairCost <= h.credits)
                        {
                            h.credits -= repairCost;

                            /* AIs repair in early games slower than in later games */
                            if (s.o.houseID == (byte)CHouse.g_playerHouseID || CSharpDune.g_campaignID >= 3)
                            {
                                s.o.hitpoints += 5;
                            }
                            else
                            {
                                s.o.hitpoints += 3;
                            }

                            if (s.o.hitpoints > si.o.hitpoints)
                            {
                                s.o.hitpoints = si.o.hitpoints;
                                s.o.flags.repairing = false;
                                s.o.flags.onHold = false;
                            }
                        }
                        else
                        {
                            s.o.flags.repairing = false;
                        }
                    }
                    else
                    {
                        if (!s.o.flags.onHold && s.countDown != 0 && s.o.linkedID != 0xFF && s.state == (short)StructureState.STRUCTURE_STATE_BUSY && si.o.flags.factory)
                        {
                            ObjectInfo oi;
                            ushort buildSpeed;
                            ushort buildCost;

                            if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
                            {
                                oi = g_table_structureInfo[s.objectType].o;
                            }
                            else if (s.o.type == (byte)StructureType.STRUCTURE_REPAIR)
                            {
                                oi = g_table_unitInfo[PoolUnit.Unit_Get_ByIndex(s.o.linkedID).o.type].o;
                            }
                            else
                            {
                                oi = g_table_unitInfo[s.objectType].o;
                            }

                            buildSpeed = 256;
                            if (s.o.hitpoints < si.o.hitpoints)
                            {
                                buildSpeed = (ushort)(s.o.hitpoints * 256 / si.o.hitpoints);
                            }

                            /* For AIs, we slow down building speed in all but the last campaign */
                            if ((byte)CHouse.g_playerHouseID != s.o.houseID)
                            {
                                if (buildSpeed > CSharpDune.g_campaignID * 20 + 95) buildSpeed = (ushort)(CSharpDune.g_campaignID * 20 + 95);
                            }

                            buildCost = (ushort)(oi.buildCredits * 256 / oi.buildTime);

                            if (buildSpeed < 256)
                            {
                                buildCost = (ushort)(buildSpeed * buildCost / 256);
                            }

                            if (s.o.type == (byte)StructureType.STRUCTURE_REPAIR && buildCost > 4)
                            {
                                buildCost /= 4;
                            }

                            buildCost += s.buildCostRemainder;

                            if (buildCost / 256 <= h.credits)
                            {
                                s.buildCostRemainder = (ushort)(buildCost & 0xFF);
                                h.credits -= (ushort)(buildCost / 256);

                                if (buildSpeed < s.countDown)
                                {
                                    s.countDown -= buildSpeed;
                                }
                                else
                                {
                                    s.countDown = 0;
                                    s.buildCostRemainder = 0;

                                    Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_READY);

                                    if (s.o.houseID == (byte)CHouse.g_playerHouseID)
                                    {
                                        if (s.o.type != (byte)StructureType.STRUCTURE_BARRACKS && s.o.type != (byte)StructureType.STRUCTURE_WOR_TROOPER)
                                        {
                                            var stringID = (ushort)Text.STR_IS_COMPLETED_AND_AWAITING_ORDERS;
                                            if (s.o.type == (byte)StructureType.STRUCTURE_HIGH_TECH) stringID = (ushort)Text.STR_IS_COMPLETE;
                                            if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD) stringID = (ushort)Text.STR_IS_COMPLETED_AND_READY_TO_PLACE;

                                            Gui.Gui.GUI_DisplayText("{0} {1}", 0, CString.String_Get_ByIndex(oi.stringID_full), CString.String_Get_ByIndex(stringID));

                                            Sound.Sound_Output_Feedback(0);
                                        }
                                    }
                                    else if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
                                    {
                                        /* An AI immediately places the structure when it is done building */
                                        Structure ns;
                                        byte i;

                                        ns = PoolStructure.Structure_Get_ByIndex(s.o.linkedID);
                                        s.o.linkedID = 0xFF;

                                        /* The AI places structures which are operational immediately */
                                        Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_IDLE);

                                        /* Find the position to place the structure */
                                        for (i = 0; i < 5; i++)
                                        {
                                            if (ns.o.type != h.ai_structureRebuild[i][0]) continue;

                                            if (!Structure_Place(ns, h.ai_structureRebuild[i][1])) continue;

                                            h.ai_structureRebuild[i][0] = 0;
                                            h.ai_structureRebuild[i][1] = 0;
                                            break;
                                        }

                                        /* If the AI no longer had in memory where to store the structure, free it and forget about it */
                                        if (i == 5)
                                        {
                                            var nsi = g_table_structureInfo[ns.o.type];

                                            h.credits += nsi.o.buildCredits;

                                            PoolStructure.Structure_Free(ns);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                /* Out of money means the building gets put on hold */
                                if (s.o.houseID == (byte)CHouse.g_playerHouseID)
                                {
                                    s.o.flags.onHold = true;
                                    Gui.Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_INSUFFICIENT_FUNDS_CONSTRUCTION_IS_HALTED), 0);
                                }
                            }
                        }

                        if (s.o.type == (byte)StructureType.STRUCTURE_REPAIR)
                        {
                            if (!s.o.flags.onHold && s.countDown != 0 && s.o.linkedID != 0xFF)
                            {
                                UnitInfo ui;
                                ushort repairSpeed;
                                ushort repairCost;

                                ui = g_table_unitInfo[PoolUnit.Unit_Get_ByIndex(s.o.linkedID).o.type];

                                repairSpeed = 256;
                                if (s.o.hitpoints < si.o.hitpoints)
                                {
                                    repairSpeed = (ushort)(s.o.hitpoints * 256 / si.o.hitpoints);
                                }

                                /* XXX -- This is highly unfair. Repairing becomes more expensive if your structure is more damaged */
                                repairCost = (ushort)(2 * ui.o.buildCredits / 256);

                                if (repairCost < h.credits)
                                {
                                    h.credits -= repairCost;

                                    if (repairSpeed < s.countDown)
                                    {
                                        s.countDown -= repairSpeed;
                                    }
                                    else
                                    {
                                        s.countDown = 0;

                                        Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_READY);

                                        if (s.o.houseID == (byte)CHouse.g_playerHouseID) Sound.Sound_Output_Feedback((ushort)(CHouse.g_playerHouseID + 55));
                                    }
                                }
                            }
                            else if (h.credits != 0)
                            {
                                /* Automaticly resume repairing when there is money again */
                                s.o.flags.onHold = false;
                            }
                        }

                        /* AI maintenance on structures */
                        if (h.flags.isAIActive && s.o.flags.allocated && s.o.houseID != (byte)CHouse.g_playerHouseID && h.credits != 0)
                        {
                            /* When structure is below 50% hitpoints, start repairing */
                            if (s.o.hitpoints < si.o.hitpoints / 2)
                            {
                                Structure_SetRepairingState(s, 1, null);
                            }

                            /* If the structure is not doing something, but can build stuff, see if there is stuff to build */
                            if (si.o.flags.factory && s.countDown == 0 && s.o.linkedID == 0xFF)
                            {
                                var type = Structure_AI_PickNextToBuild(s);

                                if (type != 0xFFFF) Structure_BuildObject(s, type);
                            }
                        }
                    }
                }

                if (tickScript)
                {
                    if (s.o.script.delay != 0)
                    {
                        s.o.script.delay--;
                    }
                    else
                    {
                        if (Script_IsLoaded(s.o.script))
                        {
                            byte i;

                            /* Run the script 3 times in a row */
                            for (i = 0; i < 3; i++)
                            {
                                if (!Script_Run(s.o.script)) break;
                            }

                            /* ENHANCEMENT -- Dune2 aborts all other structures if one gives a script error. This doesn't seem correct */
                            if (!CSharpDune.g_dune2_enhanced && i != 3) return;
                        }
                        else
                        {
                            Script_Reset(s.o.script, s.o.script.scriptInfo);
                            Script_Load(s.o.script, s.o.type);
                        }
                    }
                }
            }
        }

        /*
         * Calculate the power usage and production, and the credits storage.
         *
         * @param h The house to calculate the numbers for.
         */
        internal static void Structure_CalculateHitpointsMax(House h)
        {
            var find = new PoolFindStruct();
            ushort power = 0;

            if (h == null) return;

            if (h.index == (byte)CHouse.g_playerHouseID) CHouse.House_UpdateRadarState(h);

            if (h.powerUsage == 0)
            {
                power = 256;
            }
            else
            {
                power = (ushort)Min(h.powerProduction * 256 / h.powerUsage, 256);
            }

            find.houseID = h.index;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            while (true)
            {
                StructureInfo si;
                Structure s;

                s = PoolStructure.Structure_Find(find);
                if (s == null) return;
                if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;

                si = g_table_structureInfo[s.o.type];

                s.hitpointsMax = (ushort)(si.o.hitpoints * power / 256);
                s.hitpointsMax = (ushort)Max(s.hitpointsMax, si.o.hitpoints / 2);

                if (s.hitpointsMax >= s.o.hitpoints) continue;
                Structure_Damage(s, 1, 0);
            }
        }

        /*
         * Find a free spot for units next to a structure.
         * @param s Structure that needs a free spot.
         * @param checkForSpice Spot should be as close to spice as possible.
         * @return Position of the free spot, or \c 0 if no free spot available.
         */
        internal static ushort Structure_FindFreePosition(Structure s, bool checkForSpice)
        {
            StructureInfo si;
            ushort packed;
            ushort spicePacked;  /* Position of the spice, or 0 if not used or if no spice. */
            ushort bestPacked;
            ushort bestDistance; /* If > 0, distance to the spice from bestPacked. */
            ushort i, j;

            if (s == null) return 0;

            si = g_table_structureInfo[s.o.type];
            packed = CTile.Tile_PackTile(CTile.Tile_Center(s.o.position));

            spicePacked = (ushort)(checkForSpice ? Map.Map_SearchSpice(packed, 10) : 0);
            bestPacked = 0;
            bestDistance = 0;

            i = (ushort)(Tools.Tools_Random_256() & 0xF);
            for (j = 0; j < 16; j++, i = (ushort)((i + 1) & 0xF))
            {
                ushort offset;
                ushort curPacked;
                ushort type;
                Tile t;

                offset = (ushort)g_table_structure_layoutTilesAround[si.layout][i];
                if (offset == 0) continue;

                curPacked = (ushort)(packed + offset);
                if (!Map.Map_IsValidPosition(curPacked)) continue;

                type = Map.Map_GetLandscapeType(curPacked);
                if (type == (ushort)LandscapeType.LST_WALL || type == (ushort)LandscapeType.LST_ENTIRELY_MOUNTAIN || type == (ushort)LandscapeType.LST_PARTIAL_MOUNTAIN) continue;

                t = Map.g_map[curPacked];
                if (t.hasUnit || t.hasStructure) continue;

                if (!checkForSpice) return curPacked;

                if (bestDistance == 0 || CTile.Tile_GetDistancePacked(curPacked, spicePacked) < bestDistance)
                {
                    bestPacked = curPacked;
                    bestDistance = CTile.Tile_GetDistancePacked(curPacked, spicePacked);
                }
            }

            return bestPacked;
        }

        /*
         * Sets or toggle the repairing state of the given Structure.
         *
         * @param s The Structure.
         * @param value The repairing state, -1 to toggle.
         * @param w The widget.
         * @return True if and only if the state changed.
         */
        internal static bool Structure_SetRepairingState(Structure s, sbyte state, Widget w)
        {
            var ret = false;

            if (s == null) return false;

            /* ENHANCEMENT -- If a structure gets damaged during upgrading, pressing the "Upgrading" button silently starts the repair of the structure, and doesn't cancel upgrading. */
            if (CSharpDune.g_dune2_enhanced && s.o.flags.upgrading) return false;

            if (!s.o.flags.allocated) state = 0;

            if (state == -1) state = (sbyte)(s.o.flags.repairing ? 0 : 1);

            if (state == 0 && s.o.flags.repairing)
            {
                if (s.o.houseID == (byte)CHouse.g_playerHouseID)
                {
                    Gui.Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_REPAIRING_STOPS), 2);
                }

                s.o.flags.repairing = false;
                s.o.flags.onHold = false;

                CWidget.GUI_Widget_MakeNormal(w, false);

                ret = true;
            }

            if (state == 0 || s.o.flags.repairing || s.o.hitpoints == g_table_structureInfo[s.o.type].o.hitpoints) return ret;

            if (s.o.houseID == (byte)CHouse.g_playerHouseID)
            {
                Gui.Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_REPAIRING_STARTS), 2);
            }

            s.o.flags.onHold = true;
            s.o.flags.repairing = true;

            CWidget.GUI_Widget_MakeSelected(w, false);

            return true;
        }

        /*
         * Find the next object to build.
         * @param s The structure in which we can build something.
         * @return The type (either UnitType or StructureType) of what we should build next.
         */
        static ushort Structure_AI_PickNextToBuild(Structure s)
        {
            var find = new PoolFindStruct();
            /*ushort*/
            int buildable;
            ushort type;
            House h;
            int i;

            if (s == null) return 0xFFFF;

            h = PoolHouse.House_Get_ByIndex(s.o.houseID);
            buildable = (int)Structure_GetBuildable(s);

            if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
            {
                for (i = 0; i < 5; i++)
                {
                    type = h.ai_structureRebuild[i][0];

                    if (type == 0) continue;
                    if ((buildable & (1 << type)) == 0) continue;

                    return type;
                }

                return 0xFFFF;
            }

            if (s.o.type == (byte)StructureType.STRUCTURE_HIGH_TECH)
            {
                find.houseID = s.o.houseID;
                find.index = 0xFFFF;
                find.type = (ushort)UnitType.UNIT_CARRYALL;

                while (true)
                {
                    Unit u;

                    u = PoolUnit.Unit_Find(find);
                    if (u == null) break;

                    buildable &= (int)~UnitFlag.FLAG_UNIT_CARRYALL;
                }
            }

            if (s.o.type == (byte)StructureType.STRUCTURE_HEAVY_VEHICLE)
            {
                buildable &= (int)~UnitFlag.FLAG_UNIT_HARVESTER;
                buildable &= (int)~UnitFlag.FLAG_UNIT_MCV;
            }

            type = 0xFFFF;
            for (i = 0; i < (int)UnitType.UNIT_MAX; i++)
            {
                if ((buildable & (1 << i)) == 0) continue;

                if ((Tools.Tools_Random_256() % 4) == 0) type = (ushort)i;

                if (type != 0xFFFF)
                {
                    if (g_table_unitInfo[i].o.priorityBuild <= g_table_unitInfo[type].o.priorityBuild) continue;
                }

                type = (ushort)i;
            }

            return type;
        }

        /*
         * Activate the special weapon of a house.
         *
         * @param s The structure which launches the weapon. Has to be the Palace.
         */
        internal static void Structure_ActivateSpecial(Structure s)
        {
            House h;

            if (s == null) return;
            if (s.o.type != (byte)StructureType.STRUCTURE_PALACE) return;

            h = PoolHouse.House_Get_ByIndex(s.o.houseID);
            if (!h.flags.used) return;

            switch ((HouseWeapon)g_table_houseInfo[s.o.houseID].specialWeapon)
            {
                case HouseWeapon.HOUSE_WEAPON_MISSILE:
                    {
                        Unit u;
                        var position = new tile32
                        {
                            x = 0xFFFF,
                            y = 0xFFFF
                        };

                        CSharpDune.g_validateStrictIfZero++;
                        u = CUnit.Unit_Create((ushort)PoolUnit.UnitIndex.UNIT_INDEX_INVALID, (byte)UnitType.UNIT_MISSILE_HOUSE, s.o.houseID, position, (sbyte)Tools.Tools_Random_256());
                        CSharpDune.g_validateStrictIfZero--;

                        CUnit.g_unitHouseMissile = u;
                        if (u == null) break;

                        s.countDown = g_table_houseInfo[s.o.houseID].specialCountDown;

                        if (!h.flags.human)
                        {
                            var find = new PoolFindStruct
                            {
                                houseID = (byte)HouseType.HOUSE_INVALID,
                                type = 0xFFFF,
                                index = 0xFFFF
                            };

                            /* For the AI, try to find the first structure which is not ours, and launch missile to there */
                            while (true)
                            {
                                Structure sf;

                                sf = PoolStructure.Structure_Find(find);
                                if (sf == null) break;
                                if (sf.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || sf.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || sf.o.type == (byte)StructureType.STRUCTURE_WALL) continue;

                                if (CHouse.House_AreAllied(s.o.houseID, sf.o.houseID)) continue;

                                CUnit.Unit_LaunchHouseMissile(CTile.Tile_PackTile(sf.o.position));

                                return;
                            }

                            /* We failed to find a target, so remove the missile */
                            PoolUnit.Unit_Free(u);
                            CUnit.g_unitHouseMissile = null;

                            return;
                        }

                        /* Give the user 7 seconds to select their target */
                        CHouse.g_houseMissileCountdown = 7;

                        Gui.Gui.GUI_ChangeSelectionType((ushort)SelectionType.TARGET);
                    }
                    break;

                case HouseWeapon.HOUSE_WEAPON_FREMEN:
                    {
                        ushort location;
                        ushort i;

                        /* Find a random location to appear */
                        location = Map.Map_FindLocationTile(4, (byte)HouseType.HOUSE_INVALID);

                        for (i = 0; i < 5; i++)
                        {
                            Unit u;
                            tile32 position;
                            ushort orientation;
                            ushort unitType;

                            Tools.Tools_Random_256();

                            position = CTile.Tile_UnpackTile(location);
                            position = CTile.Tile_MoveByRandom(position, 32, true);

                            orientation = Tools.Tools_RandomLCG_Range(0, 3);
                            unitType = (ushort)((orientation == 1) ? UnitType.UNIT_TROOPER : UnitType.UNIT_TROOPERS);

                            CSharpDune.g_validateStrictIfZero++;
                            u = CUnit.Unit_Create((ushort)PoolUnit.UnitIndex.UNIT_INDEX_INVALID, (byte)unitType, (byte)HouseType.HOUSE_FREMEN, position, (sbyte)orientation);
                            CSharpDune.g_validateStrictIfZero--;

                            if (u == null) continue;

                            CUnit.Unit_SetAction(u, ActionType.ACTION_HUNT);
                        }

                        s.countDown = g_table_houseInfo[s.o.houseID].specialCountDown;
                    }
                    break;

                case HouseWeapon.HOUSE_WEAPON_SABOTEUR:
                    {
                        Unit u;
                        ushort position;

                        /* Find a spot next to the structure */
                        position = Structure_FindFreePosition(s, false);

                        /* If there is no spot, reset countdown */
                        if (position == 0)
                        {
                            s.countDown = 1;
                            return;
                        }

                        CSharpDune.g_validateStrictIfZero++;
                        u = CUnit.Unit_Create((ushort)PoolUnit.UnitIndex.UNIT_INDEX_INVALID, (byte)UnitType.UNIT_SABOTEUR, s.o.houseID, CTile.Tile_UnpackTile(position), (sbyte)Tools.Tools_Random_256());
                        CSharpDune.g_validateStrictIfZero--;

                        if (u == null) return;

                        CUnit.Unit_SetAction(u, ActionType.ACTION_SABOTAGE);

                        s.countDown = g_table_houseInfo[s.o.houseID].specialCountDown;
                    }
                    break;

                default: break;
            }

            if (s.o.houseID == (byte)CHouse.g_playerHouseID)
            {
                WidgetDraw.GUI_Widget_ActionPanel_Draw(true);
            }
        }

        internal static uint Structure_GetBuildable(Structure s)
        {
            StructureInfo si;
            uint structuresBuilt;
            uint ret = 0;
            int i;

            if (s == null) return 0;

            si = g_table_structureInfo[s.o.type];

            structuresBuilt = PoolHouse.House_Get_ByIndex(s.o.houseID).structuresBuilt;

            switch ((StructureType)s.o.type)
            {
                case StructureType.STRUCTURE_LIGHT_VEHICLE:
                case StructureType.STRUCTURE_HEAVY_VEHICLE:
                case StructureType.STRUCTURE_HIGH_TECH:
                case StructureType.STRUCTURE_WOR_TROOPER:
                case StructureType.STRUCTURE_BARRACKS:
                    for (i = 0; i < (int)UnitType.UNIT_MAX; i++)
                    {
                        g_table_unitInfo[i].o.available = 0;
                    }

                    for (i = 0; i < 8; i++)
                    {
                        UnitInfo ui;
                        ushort upgradeLevelRequired;
                        var unitType = si.buildableUnits[i];

                        if (unitType == (byte)UnitType.UNIT_INVALID) continue;

                        if (unitType == (byte)UnitType.UNIT_TRIKE && s.creatorHouseID == (ushort)HouseType.HOUSE_ORDOS) unitType = (byte)UnitType.UNIT_RAIDER_TRIKE;

                        ui = g_table_unitInfo[unitType];
                        upgradeLevelRequired = ui.o.upgradeLevelRequired;

                        if (unitType == (byte)UnitType.UNIT_SIEGE_TANK && s.creatorHouseID == (ushort)HouseType.HOUSE_ORDOS) upgradeLevelRequired--;

                        if ((structuresBuilt & ui.o.structuresRequired) != ui.o.structuresRequired) continue;
                        if ((ui.o.availableHouse & (1 << s.creatorHouseID)) == 0) continue;

                        if (s.upgradeLevel >= upgradeLevelRequired)
                        {
                            ui.o.available = 1;

                            ret |= (uint)(1 << unitType);
                            continue;
                        }

                        if (s.upgradeTimeLeft != 0 && s.upgradeLevel + 1 >= upgradeLevelRequired)
                        {
                            ui.o.available = -1;
                        }
                    }
                    return ret;

                case StructureType.STRUCTURE_CONSTRUCTION_YARD:
                    for (i = 0; i < (int)StructureType.STRUCTURE_MAX; i++)
                    {
                        var localsi = g_table_structureInfo[i];
                        ushort availableCampaign;
                        uint structuresRequired;

                        localsi.o.available = 0;

                        availableCampaign = localsi.o.availableCampaign;
                        structuresRequired = localsi.o.structuresRequired;

                        if (i == (int)StructureType.STRUCTURE_WOR_TROOPER && s.o.houseID == (byte)HouseType.HOUSE_HARKONNEN && CSharpDune.g_campaignID >= 1)
                        {
                            structuresRequired &= ~((uint)1 << (byte)StructureType.STRUCTURE_BARRACKS); //TODO: Check
                            availableCampaign = 2;
                        }

                        if ((structuresBuilt & structuresRequired) == structuresRequired || s.o.houseID != (byte)CHouse.g_playerHouseID)
                        {
                            if (s.o.houseID != (byte)HouseType.HOUSE_HARKONNEN && i == (int)StructureType.STRUCTURE_LIGHT_VEHICLE)
                            {
                                availableCampaign = 2;
                            }

                            if (CSharpDune.g_campaignID >= availableCampaign - 1 && (localsi.o.availableHouse & (1 << s.o.houseID)) != 0)
                            {
                                if (s.upgradeLevel >= localsi.o.upgradeLevelRequired || s.o.houseID != (byte)CHouse.g_playerHouseID)
                                {
                                    localsi.o.available = 1;

                                    ret |= (uint)(1 << i);
                                }
                                else if (s.upgradeTimeLeft != 0 && s.upgradeLevel + 1 >= localsi.o.upgradeLevelRequired)
                                {
                                    localsi.o.available = -1;
                                }
                            }
                        }
                    }
                    return ret;

                case StructureType.STRUCTURE_STARPORT:
                    unchecked { return (uint)-1; }

                default:
                    return 0;
            }
        }

        /*
         * Make the given Structure build an object.
         *
         * @param s The Structure.
         * @param objectType The type of the object to build or a special value (0xFFFD, 0xFFFE, 0xFFFF).
         * @return ??.
         */
        internal static bool Structure_BuildObject(Structure s, ushort objectType)
        {
            StructureInfo si;
            string str;
            Object o;
            ObjectInfo oi;

            if (s == null) return false;

            si = g_table_structureInfo[s.o.type];

            if (!si.o.flags.factory) return false;

            Structure_SetRepairingState(s, 0, null);

            if (objectType == 0xFFFD)
            {
                Structure_SetUpgradingState(s, 1, null);
                return false;
            }

            if (objectType == 0xFFFF || objectType == 0xFFFE)
            {
                ushort upgradeCost = 0;
                uint buildable;

                if (Structure_IsUpgradable(s) && si.o.hitpoints == s.o.hitpoints)
                {
                    upgradeCost = (ushort)((si.o.buildCredits + (si.o.buildCredits >> 15)) / 2);
                }

                if (upgradeCost != 0 && s.o.type == (byte)StructureType.STRUCTURE_HIGH_TECH && s.o.houseID == (byte)HouseType.HOUSE_HARKONNEN) upgradeCost = 0;
                if (s.o.type == (byte)StructureType.STRUCTURE_STARPORT) upgradeCost = 0;

                buildable = Structure_GetBuildable(s);

                if (buildable == 0)
                {
                    s.objectType = 0;
                    return false;
                }

                if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
                {
                    byte i;

                    Gui.Gui.g_factoryWindowConstructionYard = true;

                    for (i = 0; i < (byte)StructureType.STRUCTURE_MAX; i++)
                    {
                        if ((buildable & (1 << i)) == 0) continue;
                        g_table_structureInfo[i].o.available = 1;
                        if (objectType != 0xFFFE) continue;
                        s.objectType = i;
                        return false;
                    }
                }
                else
                {
                    Gui.Gui.g_factoryWindowConstructionYard = false;

                    if (s.o.type == (byte)StructureType.STRUCTURE_STARPORT)
                    {
                        byte linkedID = 0xFF;
                        var availableUnits = new short[(int)UnitType.UNIT_MAX];
                        Unit u;
                        bool loop;

                        //memset(availableUnits, 0, sizeof(availableUnits));

                        do
                        {
                            byte i;

                            loop = false;

                            for (i = 0; i < (byte)UnitType.UNIT_MAX; i++)
                            {
                                var unitsAtStarport = CUnit.g_starportAvailable[i];

                                if (unitsAtStarport == 0)
                                {
                                    g_table_unitInfo[i].o.available = 0;
                                }
                                else if (unitsAtStarport < 0)
                                {
                                    g_table_unitInfo[i].o.available = -1;
                                }
                                else if (unitsAtStarport > availableUnits[i])
                                {
                                    CSharpDune.g_validateStrictIfZero++;
                                    u = PoolUnit.Unit_Allocate((ushort)PoolUnit.UnitIndex.UNIT_INDEX_INVALID, i, s.o.houseID);
                                    CSharpDune.g_validateStrictIfZero--;

                                    if (u != null)
                                    {
                                        loop = true;
                                        u.o.linkedID = linkedID;
                                        linkedID = (byte)(u.o.index & 0xFF);
                                        availableUnits[i]++;
                                        g_table_unitInfo[i].o.available = (sbyte)availableUnits[i];
                                    }
                                    else if (availableUnits[i] == 0) g_table_unitInfo[i].o.available = -1;
                                }
                            }
                        } while (loop);

                        while (linkedID != 0xFF)
                        {
                            u = PoolUnit.Unit_Get_ByIndex(linkedID);
                            linkedID = u.o.linkedID;
                            PoolUnit.Unit_Free(u);
                        }
                    }
                    else
                    {
                        byte i;

                        for (i = 0; i < (byte)UnitType.UNIT_MAX; i++)
                        {
                            if ((buildable & (1 << i)) == 0) continue;
                            g_table_unitInfo[i].o.available = 1;
                            if (objectType != 0xFFFE) continue;
                            s.objectType = i;
                            return false;
                        }
                    }
                }

                if (objectType == 0xFFFF)
                {
                    FactoryResult res;

                    Sprites.Sprites_UnloadTiles();

                    Buffer.BlockCopy(Gfx.g_paletteActive, 0, Gfx.g_palette1, 0, 256 * 3); //memmove(g_palette1, g_paletteActive, 256 * 3);

                    Gui.Gui.GUI_ChangeSelectionType((ushort)SelectionType.MENTAT);

                    Timer.Timer_SetTimer(TimerType.TIMER_GAME, false);

                    res = Gui.Gui.GUI_DisplayFactoryWindow(Gui.Gui.g_factoryWindowConstructionYard, s.o.type == (byte)StructureType.STRUCTURE_STARPORT, upgradeCost);

                    Timer.Timer_SetTimer(TimerType.TIMER_GAME, true);

                    Sprites.Sprites_LoadTiles();

                    Gfx.GFX_SetPalette(Gfx.g_palette1);

                    Gui.Gui.GUI_ChangeSelectionType((ushort)SelectionType.STRUCTURE);

                    if (res == FactoryResult.FACTORY_RESUME) return false;

                    if (res == FactoryResult.FACTORY_UPGRADE)
                    {
                        Structure_SetUpgradingState(s, 1, null);
                        return false;
                    }

                    if (res == FactoryResult.FACTORY_BUY)
                    {
                        House h;
                        byte i;

                        h = PoolHouse.House_Get_ByIndex(s.o.houseID);

                        for (i = 0; i < 25; i++)
                        {
                            Unit u;

                            if (Gui.Gui.g_factoryWindowItems[i].amount == 0) continue;
                            objectType = Gui.Gui.g_factoryWindowItems[i].objectType;

                            if (s.o.type != (byte)StructureType.STRUCTURE_STARPORT)
                            {
                                Structure_CancelBuild(s);

                                s.objectType = objectType;

                                if (!Gui.Gui.g_factoryWindowConstructionYard) continue;

                                if (Structure_CheckAvailableConcrete(objectType, s.o.houseID)) continue;

                                if (Gui.Gui.GUI_DisplayHint((ushort)Text.STR_THERE_ISNT_ENOUGH_OPEN_CONCRETE_TO_PLACE_THIS_STRUCTURE_YOU_MAY_PROCEED_BUT_WITHOUT_ENOUGH_CONCRETE_THE_BUILDING_WILL_NEED_REPAIRS, g_table_structureInfo[objectType].o.spriteID) == 0) continue;

                                s.objectType = objectType;

                                return false;
                            }

                            CSharpDune.g_validateStrictIfZero++;
                            {
                                var tile = new tile32 { x = 0xFFFF, y = 0xFFFF };
                                u = CUnit.Unit_Create((ushort)PoolUnit.UnitIndex.UNIT_INDEX_INVALID, (byte)objectType, s.o.houseID, tile, 0);
                            }
                            CSharpDune.g_validateStrictIfZero--;

                            if (u == null)
                            {
                                h.credits += g_table_unitInfo[(int)UnitType.UNIT_CARRYALL].o.buildCredits;
                                if (s.o.houseID != (byte)CHouse.g_playerHouseID) continue;
                                Gui.Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_UNABLE_TO_CREATE_MORE), 2);
                                continue;
                            }

                            g_structureIndex = s.o.index;

                            if (h.starportTimeLeft == 0) h.starportTimeLeft = g_table_houseInfo[h.index].starportDeliveryTime;

                            u.o.linkedID = (byte)(h.starportLinkedID & 0xFF);
                            h.starportLinkedID = u.o.index;

                            CUnit.g_starportAvailable[objectType]--;
                            if (CUnit.g_starportAvailable[objectType] <= 0) CUnit.g_starportAvailable[objectType] = -1;

                            Gui.Gui.g_factoryWindowItems[i].amount--;
                            if (Gui.Gui.g_factoryWindowItems[i].amount != 0) i--;
                        }
                    }
                }
                else
                {
                    s.objectType = objectType;
                }
            }

            if (s.o.type == (byte)StructureType.STRUCTURE_STARPORT) return true;

            if (s.objectType != objectType) Structure_CancelBuild(s);

            if (s.o.linkedID != 0xFF || objectType == 0xFFFF) return false;

            if (s.o.type != (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
            {
                var tile = new tile32 { x = 0xFFFF, y = 0xFFFF };

                oi = g_table_unitInfo[objectType].o;
                o = CUnit.Unit_Create((ushort)PoolUnit.UnitIndex.UNIT_INDEX_INVALID, (byte)objectType, s.o.houseID, tile, 0)?.o;
                str = CString.String_Get_ByIndex(g_table_unitInfo[objectType].o.stringID_full);
            }
            else
            {
                oi = g_table_structureInfo[objectType].o;
                o = Structure_Create((ushort)PoolStructure.StructureIndex.STRUCTURE_INDEX_INVALID, (byte)objectType, s.o.houseID, 0xFFFF).o;
                str = CString.String_Get_ByIndex(g_table_structureInfo[objectType].o.stringID_full);
            }

            s.o.flags.onHold = false;

            if (o != null)
            {
                s.o.linkedID = (byte)(o.index & 0xFF);
                s.objectType = objectType;
                s.countDown = (ushort)(oi.buildTime << 8);

                Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_BUSY);

                if (s.o.houseID != (byte)CHouse.g_playerHouseID) return true;

                Gui.Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_PRODUCTION_OF_S_HAS_STARTED), 2, str);

                return true;
            }

            if (s.o.houseID != (byte)CHouse.g_playerHouseID) return false;

            Gui.Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_UNABLE_TO_CREATE_MORE), 2);

            return false;
        }

        /*
         * Sets or toggle the upgrading state of the given Structure.
         *
         * @param s The Structure.
         * @param value The upgrading state, -1 to toggle.
         * @param w The widget.
         * @return True if and only if the state changed.
         */
        internal static bool Structure_SetUpgradingState(Structure s, sbyte state, Widget w)
        {
            var ret = false;

            if (s == null) return false;

            if (state == -1) state = (sbyte)(s.o.flags.upgrading ? 0 : 1);

            if (state == 0 && s.o.flags.upgrading)
            {
                if (s.o.houseID == (byte)CHouse.g_playerHouseID)
                {
                    Gui.Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_UPGRADING_STOPS), 2);
                }

                s.o.flags.upgrading = false;
                s.o.flags.onHold = false;

                CWidget.GUI_Widget_MakeNormal(w, false);

                ret = true;
            }

            if (state == 0 || s.o.flags.upgrading || s.upgradeTimeLeft == 0) return ret;

            if (s.o.houseID == (byte)CHouse.g_playerHouseID)
            {
                Gui.Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_UPGRADING_STARTS), 2);
            }

            s.o.flags.onHold = true;
            s.o.flags.repairing = false;
            s.o.flags.upgrading = true;

            CWidget.GUI_Widget_MakeSelected(w, false);

            return true;
        }

        /*
         * Cancel the building of object for given structure.
         *
         * @param s The Structure.
         */
        static void Structure_CancelBuild(Structure s)
        {
            ObjectInfo oi;

            if (s == null || s.o.linkedID == 0xFF) return;

            if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
            {
                var s2 = PoolStructure.Structure_Get_ByIndex(s.o.linkedID);
                oi = g_table_structureInfo[s2.o.type].o;
                PoolStructure.Structure_Free(s2);
            }
            else
            {
                var u = PoolUnit.Unit_Get_ByIndex(s.o.linkedID);
                oi = g_table_unitInfo[u.o.type].o;
                PoolUnit.Unit_Free(u);
            }

            PoolHouse.House_Get_ByIndex(s.o.houseID).credits += (ushort)(((oi.buildTime - (s.countDown >> 8)) * 256 / oi.buildTime) * oi.buildCredits / 256);

            s.o.flags.onHold = false;
            s.countDown = 0;
            s.o.linkedID = 0xFF;
        }

        /*
         * Check if requested structureType can be build on the map with concrete below.
         *
         * @param structureType The type of structure to check for.
         * @param houseID The house to check for.
         * @return True if and only if there are enough slabs available on the map to
         *  build requested structure.
         */
        static bool Structure_CheckAvailableConcrete(ushort structureType, byte houseID)
        {
            StructureInfo si;
            short tileCount;
            short i;

            si = g_table_structureInfo[structureType];

            tileCount = (short)g_table_structure_layoutTileCount[si.layout];

            if (structureType == (ushort)StructureType.STRUCTURE_SLAB_1x1 || structureType == (ushort)StructureType.STRUCTURE_SLAB_2x2) return true;

            for (i = 0; i < 4096; i++)
            {
                var stop = true;
                ushort j;

                for (j = 0; j < tileCount; j++)
                {
                    var packed = (ushort)(i + g_table_structure_layoutTiles[si.layout][j]);
                    /* XXX -- This can overflow, and we should check for that */

                    if (Map.Map_GetLandscapeType(packed) == (ushort)LandscapeType.LST_CONCRETE_SLAB && Map.g_map[packed].houseID == houseID) continue;

                    stop = false;
                    break;
                }

                if (stop) return true;
            }

            return false;
        }

        /*
         * Place a structure on the map.
         *
         * @param structure The structure to place on the map.
         * @param position The (packed) tile to place the struction on.
         * @return True if and only if the structure is placed on the map.
         */
        internal static bool Structure_Place(Structure s, ushort position)
        {
            StructureInfo si;
            short validBuildLocation;

            if (s == null) return false;
            if (position == 0xFFFF) return false;

            si = g_table_structureInfo[s.o.type];

            switch ((StructureType)s.o.type)
            {
                case StructureType.STRUCTURE_WALL:
                    {
                        Tile t;

                        if (Structure_IsValidBuildLocation(position, StructureType.STRUCTURE_WALL) == 0) return false;

                        t = Map.g_map[position];
                        t.groundTileID = (ushort)(Sprites.g_wallTileID + 1);
                        /* ENHANCEMENT -- Dune2 wrongfully only removes the lower 2 bits, where the lower 3 bits are the owner. This is no longer visible. */
                        t.houseID = s.o.houseID;

                        Map.g_mapTileID[position] |= 0x8000;

                        if (s.o.houseID == (byte)CHouse.g_playerHouseID) CTile.Tile_RemoveFogInRadius(CTile.Tile_UnpackTile(position), 1);

                        if (Map.Map_IsPositionUnveiled(position)) t.overlayTileID = 0;

                        Structure_ConnectWall(position, true);
                        PoolStructure.Structure_Free(s);

                    }
                    return true;

                case StructureType.STRUCTURE_SLAB_1x1:
                case StructureType.STRUCTURE_SLAB_2x2:
                    {
                        ushort i, result;

                        result = 0;

                        for (i = 0; i < g_table_structure_layoutTileCount[si.layout]; i++)
                        {
                            var curPos = (ushort)(position + g_table_structure_layoutTiles[si.layout][i]);
                            var t = Map.g_map[curPos];

                            if (Structure_IsValidBuildLocation(curPos, StructureType.STRUCTURE_SLAB_1x1) == 0) continue;

                            t.groundTileID = Sprites.g_builtSlabTileID;
                            t.houseID = s.o.houseID;

                            Map.g_mapTileID[curPos] |= 0x8000;

                            if (s.o.houseID == (byte)CHouse.g_playerHouseID) CTile.Tile_RemoveFogInRadius(CTile.Tile_UnpackTile(curPos), 1);

                            if (Map.Map_IsPositionUnveiled(curPos)) t.overlayTileID = 0;

                            Map.Map_Update(curPos, 0, false);

                            result = 1;
                        }

                        /* XXX -- Dirt hack -- Parts of the 2x2 slab can be outside the building area, so by doing the same loop twice it will build for sure */
                        if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2)
                        {
                            for (i = 0; i < g_table_structure_layoutTileCount[si.layout]; i++)
                            {
                                var curPos = (ushort)(position + g_table_structure_layoutTiles[si.layout][i]);
                                var t = Map.g_map[curPos];

                                if (Structure_IsValidBuildLocation(curPos, StructureType.STRUCTURE_SLAB_1x1) == 0) continue;

                                t.groundTileID = Sprites.g_builtSlabTileID;
                                t.houseID = s.o.houseID;

                                Map.g_mapTileID[curPos] |= 0x8000;

                                if (s.o.houseID == (byte)CHouse.g_playerHouseID)
                                {
                                    CTile.Tile_RemoveFogInRadius(CTile.Tile_UnpackTile(curPos), 1);
                                    t.overlayTileID = 0;
                                }

                                Map.Map_Update(curPos, 0, false);

                                result = 1;
                            }
                        }

                        if (result == 0) return false;

                        PoolStructure.Structure_Free(s);
                    }
                    return true;
            }

            validBuildLocation = Structure_IsValidBuildLocation(position, (StructureType)s.o.type);
            if (validBuildLocation == 0 && s.o.houseID == (byte)CHouse.g_playerHouseID && !CSharpDune.g_debugScenario && CSharpDune.g_validateStrictIfZero == 0) return false;

            /* ENHANCEMENT -- In Dune2, it only removes the fog around the top-left tile of a structure, leaving for big structures the right in the fog. */
            if (!CSharpDune.g_dune2_enhanced && s.o.houseID == (byte)CHouse.g_playerHouseID) CTile.Tile_RemoveFogInRadius(CTile.Tile_UnpackTile(position), 2);

            s.o.seenByHouses |= (byte)(1 << s.o.houseID);
            if (s.o.houseID == (byte)CHouse.g_playerHouseID) s.o.seenByHouses |= 0xFF;

            s.o.flags.isNotOnMap = false;

            s.o.position = CTile.Tile_UnpackTile(position);
            s.o.position.x &= 0xFF00;
            s.o.position.y &= 0xFF00;

            s.rotationSpriteDiff = 0;
            s.o.hitpoints = si.o.hitpoints;
            s.hitpointsMax = si.o.hitpoints;

            /* If the return value is negative, there are tiles without slab. This gives a penalty to the hitpoints. */
            if (validBuildLocation < 0)
            {
                var tilesWithoutSlab = (ushort)-validBuildLocation;
                var structureTileCount = g_table_structure_layoutTileCount[si.layout];

                s.o.hitpoints -= (ushort)((si.o.hitpoints / 2) * tilesWithoutSlab / structureTileCount);

                s.o.flags.degrades = true;
            }
            else
            {
                /* ENHANCEMENT -- When you build a structure completely on slabs, it should not degrade */
                if (!CSharpDune.g_dune2_enhanced)
                {
                    s.o.flags.degrades = true;
                }
            }

            Script_Reset(s.o.script, g_scriptStructure);

            s.o.script.variables[0] = 0;
            s.o.script.variables[4] = 0;

            /* XXX -- Weird .. if 'position' enters with 0xFFFF it is returned immediately .. how can this ever NOT happen? */
            if (position != 0xFFFF)
            {
                s.o.script.delay = 0;
                Script_Reset(s.o.script, s.o.script.scriptInfo);
                Script_Load(s.o.script, s.o.type);
            }

            {
                ushort i;

                for (i = 0; i < g_table_structure_layoutTileCount[si.layout]; i++)
                {
                    var curPos = (ushort)(position + g_table_structure_layoutTiles[si.layout][i]);
                    Unit u;

                    u = CUnit.Unit_Get_ByPackedTile(curPos);

                    CUnit.Unit_Remove(u);

                    /* ENHANCEMENT -- In Dune2, it only removes the fog around the top-left tile of a structure, leaving for big structures the right in the fog. */
                    if (CSharpDune.g_dune2_enhanced && s.o.houseID == (byte)CHouse.g_playerHouseID) CTile.Tile_RemoveFogInRadius(CTile.Tile_UnpackTile(curPos), 2);
                }
            }

            if (s.o.type == (byte)StructureType.STRUCTURE_WINDTRAP)
            {
                House h;

                h = PoolHouse.House_Get_ByIndex(s.o.houseID);
                h.windtrapCount += 1;
            }

            if (CSharpDune.g_validateStrictIfZero == 0)
            {
                House h;

                h = PoolHouse.House_Get_ByIndex(s.o.houseID);
                CHouse.House_CalculatePowerAndCredit(h);
            }

            Structure_UpdateMap(s);

            {
                House h;
                h = PoolHouse.House_Get_ByIndex(s.o.houseID);
                h.structuresBuilt = Structure_GetStructuresBuilt(h);
            }

            return true;
        }

        /*
         * Create a new Structure.
         *
         * @param index The new index of the Structure, or STRUCTURE_INDEX_INVALID to assign one.
         * @param typeID The type of the new Structure.
         * @param houseID The House of the new Structure.
         * @param position The packed position where to place the Structure. If 0xFFFF, the Structure is not placed.
         * @return The new created Structure, or NULL if something failed.
         */
        internal static Structure Structure_Create(ushort index, byte typeID, byte houseID, ushort position)
        {
            StructureInfo si;
            Structure s;

            if (houseID >= (byte)HouseType.HOUSE_MAX) return null;
            if (typeID >= (byte)StructureType.STRUCTURE_MAX) return null;

            si = g_table_structureInfo[typeID];
            s = PoolStructure.Structure_Allocate(index, typeID);
            if (s == null) return null;

            s.o.houseID = houseID;
            s.creatorHouseID = houseID;
            s.o.flags.isNotOnMap = true;
            s.o.position.x = 0;
            s.o.position.y = 0;
            s.o.linkedID = 0xFF;
            s.state = (short)(CSharpDune.g_debugScenario ? StructureState.STRUCTURE_STATE_IDLE : StructureState.STRUCTURE_STATE_JUSTBUILT);

            if (typeID == (byte)StructureType.STRUCTURE_TURRET)
            {
                s.rotationSpriteDiff = Sprites.g_iconMap[Sprites.g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_BASE_DEFENSE_TURRET] + 1];
            }
            if (typeID == (byte)StructureType.STRUCTURE_ROCKET_TURRET)
            {
                s.rotationSpriteDiff = Sprites.g_iconMap[Sprites.g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_BASE_ROCKET_TURRET] + 1];
            }

            s.o.hitpoints = si.o.hitpoints;
            s.hitpointsMax = si.o.hitpoints;

            if (houseID == (byte)HouseType.HOUSE_HARKONNEN && typeID == (byte)StructureType.STRUCTURE_LIGHT_VEHICLE)
            {
                s.upgradeLevel = 1;
            }

            /* Check if there is an upgrade available */
            if (si.o.flags.factory)
            {
                s.upgradeTimeLeft = (byte)(Structure_IsUpgradable(s) ? 100 : 0);
            }

            s.objectType = 0xFFFF;

            Structure_BuildObject(s, 0xFFFE);

            s.countDown = 0;

            /* AIs get the full upgrade immediately */
            if (houseID != (byte)CHouse.g_playerHouseID)
            {
                while (true)
                {
                    if (!Structure_IsUpgradable(s)) break;
                    s.upgradeLevel++;
                }
                s.upgradeTimeLeft = 0;
            }

            if (position != 0xFFFF && !Structure_Place(s, position))
            {
                PoolStructure.Structure_Free(s);
                return null;
            }

            return s;
        }
    }
}
