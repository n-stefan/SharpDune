/* Load/save routines for Info */

using System.IO;

namespace SharpDune
{
    class Info
    {
        static readonly SaveLoadDesc[] s_saveInfoOld = {
            SaveLoad.SLD_EMPTY2(SaveLoadType.SLDT_UINT8, 250),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT16, () => CSharpDune.g_scenarioID, (v, _) => CSharpDune.g_scenarioID = (ushort)v),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT16, () => CSharpDune.g_campaignID, (v, _) => CSharpDune.g_campaignID = (ushort)v),
            SaveLoad.SLD_END()
        };

        static readonly SaveLoadDesc[] s_saveInfo = {
            SaveLoad.SLD_GSLD(() => CScenario.g_scenario, SaveLoad.g_saveScenario),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT16, () => CHouse.g_playerCreditsNoSilo, (v, _) => CHouse.g_playerCreditsNoSilo = (ushort)v),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT16, () => Gui.Gui.g_minimapPosition, (v, _) => Gui.Gui.g_minimapPosition = (ushort)v),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT16, () => Gui.Gui.g_selectionRectanglePosition, (v, _) => Gui.Gui.g_selectionRectanglePosition = (ushort)v),
            SaveLoad.SLD_GCALLB(SaveLoadType.SLDT_INT8, () => CSharpDune.g_selectionType, SaveLoad_SelectionType),
            SaveLoad.SLD_GENTRY2(SaveLoadType.SLDT_INT8, SaveLoadType.SLDT_UINT16, () => CStructure.g_structureActiveType, (v, _) => CStructure.g_structureActiveType = (ushort)(sbyte)v),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT16, () => CStructure.g_structureActivePosition, (v, _) => CStructure.g_structureActivePosition = (ushort)v),
            SaveLoad.SLD_GCALLB(SaveLoadType.SLDT_UINT16, () => CStructure.g_structureActive, SaveLoad_StructureActive),
            SaveLoad.SLD_GCALLB(SaveLoadType.SLDT_UINT16, () => CUnit.g_unitSelected, SaveLoad_UnitSelected),
            SaveLoad.SLD_GCALLB(SaveLoadType.SLDT_UINT16, () => CUnit.g_unitActive, SaveLoad_UnitActive),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT16, () => CSharpDune.g_activeAction, (v, _) => CSharpDune.g_activeAction = (ushort)v),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT32, () => Gui.Gui.g_strategicRegionBits, (v, _) => Gui.Gui.g_strategicRegionBits = (uint)v),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT16, () => CSharpDune.g_scenarioID, (v, _) => CSharpDune.g_scenarioID = (ushort)v),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT16, () => CSharpDune.g_campaignID, (v, _) => CSharpDune.g_campaignID = (ushort)v),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT32, () => CSharpDune.g_hintsShown1, (v, _) => CSharpDune.g_hintsShown1 = (uint)v),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT32, () => CSharpDune.g_hintsShown2, (v, _) => CSharpDune.g_hintsShown2 = (uint)v),
            SaveLoad.SLD_GCALLB(SaveLoadType.SLDT_UINT32, () => CSharpDune.g_tickScenarioStart, SaveLoad_TickScenarioStart),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT16, () => CHouse.g_playerCreditsNoSilo, (v, _) => CHouse.g_playerCreditsNoSilo = (ushort)v),
            SaveLoad.SLD_GARRAY(SaveLoadType.SLDT_INT16, () => CUnit.g_starportAvailable, (v, i) => CUnit.g_starportAvailable[i] = (short)v, (ushort)UnitType.UNIT_MAX),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT16, () => CHouse.g_houseMissileCountdown, (v, _) => CHouse.g_houseMissileCountdown = (ushort)v),
            SaveLoad.SLD_GCALLB(SaveLoadType.SLDT_UINT16, () => CUnit.g_unitHouseMissile, SaveLoad_UnitHouseMissile),
            SaveLoad.SLD_GENTRY(SaveLoadType.SLDT_UINT16, () => CStructure.g_structureIndex, (v, _) => CStructure.g_structureIndex = (ushort)v),
            SaveLoad.SLD_END()
        };

        /*
         * Load all kinds of important info from a file.
         * @param fp The file to load from.
         * @param length The length of the data chunk.
         * @return True if and only if all bytes were read successful.
         */
        internal static bool Info_LoadOld(BinaryReader fp, uint length)
        {
            if (!SaveLoad.SaveLoad_Load(s_saveInfoOld, fp, null)) return false;

            return true;
        }

        /*
         * Load all kinds of important info from a file.
         * @param fp The file to load from.
         * @param length The length of the data chunk.
         * @return True if and only if all bytes were read successful.
         */
        internal static bool Info_Load(BinaryReader fp, uint length)
        {
            if (SaveLoad.SaveLoad_GetLength(s_saveInfo) != length) return false;
            if (!SaveLoad.SaveLoad_Load(s_saveInfo, fp, null)) return false;

            Gui.Gui.g_viewportPosition = Gui.Gui.g_minimapPosition;
            Gui.Gui.g_selectionPosition = Gui.Gui.g_selectionRectanglePosition;

            Sprites.Sprites_LoadTiles();

            Map.Map_CreateLandscape(CScenario.g_scenario.mapSeed);

            return true;
        }

        static uint SaveLoad_SelectionType(object obj, uint value, bool loading)
        {
            if (loading)
            {
                CSharpDune.g_selectionTypeNew = (ushort)value;
                return 0;
            }

            return CSharpDune.g_selectionType;
        }

        static uint SaveLoad_StructureActive(object obj, uint value, bool loading)
        {
            if (loading)
            {
                if ((ushort)value != 0xFFFF)
                {
                    CStructure.g_structureActive = CStructure.Structure_Get_ByIndex((ushort)value);
                }
                else
                {
                    CStructure.g_structureActive = null;
                }
                return 0;
            }

            if (CStructure.g_structureActiveType != 0xFFFF)
            {
                return CStructure.g_structureActive.o.index;
            }
            else
            {
                return 0xFFFF;
            }
        }

        static uint SaveLoad_UnitSelected(object obj, uint value, bool loading)
        {
            if (loading)
            {
                if ((ushort)value != 0xFFFF && value < (uint)UnitIndex.UNIT_INDEX_MAX)
                {
                    CUnit.g_unitSelected = CUnit.Unit_Get_ByIndex((ushort)value);
                }
                else
                {
                    CUnit.g_unitSelected = null;
                }
                return 0;
            }

            if (CUnit.g_unitSelected != null)
            {
                return CUnit.g_unitSelected.o.index;
            }
            else
            {
                return 0xFFFF;
            }
        }

        static uint SaveLoad_UnitActive(object obj, uint value, bool loading)
        {
            if (loading)
            {
                if ((ushort)value != 0xFFFF && value < (uint)UnitIndex.UNIT_INDEX_MAX)
                {
                    CUnit.g_unitActive = CUnit.Unit_Get_ByIndex((ushort)value);
                }
                else
                {
                    CUnit.g_unitActive = null;
                }
                return 0;
            }

            if (CUnit.g_unitActive != null)
            {
                return CUnit.g_unitActive.o.index;
            }
            else
            {
                return 0xFFFF;
            }
        }

        static uint SaveLoad_TickScenarioStart(object obj, uint value, bool loading)
        {
            if (loading)
            {
                CSharpDune.g_tickScenarioStart = Timer.g_timerGame - value;
                return 0;
            }

            return Timer.g_timerGame - CSharpDune.g_tickScenarioStart;
        }

        static uint SaveLoad_UnitHouseMissile(object obj, uint value, bool loading)
        {
            if (loading)
            {
                if ((ushort)value != 0xFFFF && value < (uint)UnitIndex.UNIT_INDEX_MAX)
                {
                    CUnit.g_unitHouseMissile = CUnit.Unit_Get_ByIndex((ushort)value);
                }
                else
                {
                    CUnit.g_unitHouseMissile = null;
                }
                return 0;
            }

            if (CUnit.g_unitHouseMissile != null)
            {
                return CUnit.g_unitHouseMissile.o.index;
            }
            else
            {
                return 0xFFFF;
            }
        }

        internal static uint SaveLoad_Script_Script(object obj, uint value, bool loading)
        {
            var script = (ScriptEngine)obj;

            if (loading)
            {
                script.script = (ushort)value;
                return 0;
            }

            if (script.script == null) return 0;
            return (uint)script.script;

            //ScriptEngine script = (ScriptEngine)obj;

            //if (loading)
            //{
            //    script.script.Arr = new[] { (ushort)value, (ushort)(value >> 16) };
            //    return 0;
            //}

            //if (script.script == null) return 0;
            //return (uint)(script.script.Ptr - script.scriptInfo.start.Ptr);
        }

        //static ushort savegameVersion = 0x0290;
        /*
         * Save all kinds of important info to the savegame.
         * @param fp The file to save to.
         * @return True if and only if all bytes were written successful.
         */
        internal static bool Info_Save(BinaryWriter fp)
        {
            ushort savegameVersion = 0x0290;

            if (!CFile.fwrite_le_uint16(savegameVersion, fp)) return false;

            if (!SaveLoad.SaveLoad_Save(s_saveInfo, fp, null)) return false;

            return true;
        }
    }
}
