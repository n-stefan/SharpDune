/* Load/save routines for Info */

namespace SharpDune.SaveLoad;

static class SaveLoadInfo
{
    static readonly SaveLoadDesc[] s_saveInfo = [
        SLD_GSLD(() => g_scenario, g_saveScenario),
        SLD_GENTRY(SLDT_UINT16, () => g_playerCreditsNoSilo, (v, _) => g_playerCreditsNoSilo = (ushort)v),
        SLD_GENTRY(SLDT_UINT16, () => g_minimapPosition, (v, _) => g_minimapPosition = (ushort)v),
        SLD_GENTRY(SLDT_UINT16, () => g_selectionRectanglePosition, (v, _) => g_selectionRectanglePosition = (ushort)v),
        SLD_GCALLB(SLDT_INT8, () => g_selectionType, SaveLoad_SelectionType),
        SLD_GENTRY2(SLDT_INT8, SLDT_UINT16, () => g_structureActiveType, (v, _) => g_structureActiveType = (ushort)(sbyte)v),
        SLD_GENTRY(SLDT_UINT16, () => g_structureActivePosition, (v, _) => g_structureActivePosition = (ushort)v),
        SLD_GCALLB(SLDT_UINT16, () => g_structureActive, SaveLoad_StructureActive),
        SLD_GCALLB(SLDT_UINT16, () => g_unitSelected, SaveLoad_UnitSelected),
        SLD_GCALLB(SLDT_UINT16, () => g_unitActive, SaveLoad_UnitActive),
        SLD_GENTRY(SLDT_UINT16, () => g_activeAction, (v, _) => g_activeAction = (ushort)v),
        SLD_GENTRY(SLDT_UINT32, () => g_strategicRegionBits, (v, _) => g_strategicRegionBits = (uint)v),
        SLD_GENTRY(SLDT_UINT16, () => g_scenarioID, (v, _) => g_scenarioID = (ushort)v),
        SLD_GENTRY(SLDT_UINT16, () => g_campaignID, (v, _) => g_campaignID = (ushort)v),
        SLD_GENTRY(SLDT_UINT32, () => g_hintsShown1, (v, _) => g_hintsShown1 = (uint)v),
        SLD_GENTRY(SLDT_UINT32, () => g_hintsShown2, (v, _) => g_hintsShown2 = (uint)v),
        SLD_GCALLB(SLDT_UINT32, () => g_tickScenarioStart, SaveLoad_TickScenarioStart),
        SLD_GENTRY(SLDT_UINT16, () => g_playerCreditsNoSilo, (v, _) => g_playerCreditsNoSilo = (ushort)v),
        SLD_GARRAY(SLDT_INT16, () => g_starportAvailable, (v, i) => g_starportAvailable[i] = (short)v, (ushort)UnitType.UNIT_MAX),
        SLD_GENTRY(SLDT_UINT16, () => g_houseMissileCountdown, (v, _) => g_houseMissileCountdown = (ushort)v),
        SLD_GCALLB(SLDT_UINT16, () => g_unitHouseMissile, SaveLoad_UnitHouseMissile),
        SLD_GENTRY(SLDT_UINT16, () => g_structureIndex, (v, _) => g_structureIndex = (ushort)v),
        SLD_END()
    ];

    static readonly SaveLoadDesc[] s_saveInfoOld = [
        SLD_EMPTY2(SLDT_UINT8, 250),
        SLD_GENTRY(SLDT_UINT16, () => g_scenarioID, (v, _) => g_scenarioID = (ushort)v),
        SLD_GENTRY(SLDT_UINT16, () => g_campaignID, (v, _) => g_campaignID = (ushort)v),
        SLD_END()
    ];

    static uint SaveLoad_SelectionType(object obj, uint value, bool loading)
    {
        if (loading)
        {
            g_selectionTypeNew = (ushort)value;
            return 0;
        }

        return g_selectionType;
    }

    static uint SaveLoad_StructureActive(object obj, uint value, bool loading)
    {
        if (loading)
        {
            g_structureActive = (ushort)value != 0xFFFF ? Structure_Get_ByIndex((ushort)value) : null;
            return 0;
        }

        return g_structureActiveType != 0xFFFF ? g_structureActive.o.index : (uint)0xFFFF;
    }

    static uint SaveLoad_UnitSelected(object obj, uint value, bool loading)
    {
        if (loading)
        {
            g_unitSelected = (ushort)value != 0xFFFF && value < (uint)UnitIndex.UNIT_INDEX_MAX ? Unit_Get_ByIndex((ushort)value) : null;
            return 0;
        }

        return g_unitSelected != null ? g_unitSelected.o.index : (uint)0xFFFF;
    }

    static uint SaveLoad_UnitActive(object obj, uint value, bool loading)
    {
        if (loading)
        {
            g_unitActive = (ushort)value != 0xFFFF && value < (uint)UnitIndex.UNIT_INDEX_MAX ? Unit_Get_ByIndex((ushort)value) : null;
            return 0;
        }

        return g_unitActive != null ? g_unitActive.o.index : (uint)0xFFFF;
    }

    static uint SaveLoad_TickScenarioStart(object obj, uint value, bool loading)
    {
        if (loading)
        {
            g_tickScenarioStart = g_timerGame - value;
            return 0;
        }

        return g_timerGame - g_tickScenarioStart;
    }

    static uint SaveLoad_UnitHouseMissile(object obj, uint value, bool loading)
    {
        if (loading)
        {
            g_unitHouseMissile = (ushort)value != 0xFFFF && value < (uint)UnitIndex.UNIT_INDEX_MAX ? Unit_Get_ByIndex((ushort)value) : null;
            return 0;
        }

        return g_unitHouseMissile != null ? g_unitHouseMissile.o.index : (uint)0xFFFF;
    }

    /*
     * Load all kinds of important info from a file.
     * @param fp The file to load from.
     * @param length The length of the data chunk.
     * @return True if and only if all bytes were read successful.
     */
    internal static bool Info_Load(BinaryReader fp, uint length)
    {
        if (SaveLoad_GetLength(s_saveInfo) != length) return false;
        if (!SaveLoad_Load(s_saveInfo, fp, null)) return false;

        g_viewportPosition = g_minimapPosition;
        g_selectionPosition = g_selectionRectanglePosition;

        Sprites_LoadTiles();

        Map_CreateLandscape(g_scenario.mapSeed);

        return true;
    }

    /*
     * Load all kinds of important info from a file.
     * @param fp The file to load from.
     * @param length The length of the data chunk.
     * @return True if and only if all bytes were read successful.
     */
    internal static bool Info_LoadOld(BinaryReader fp)
    {
        return SaveLoad_Load(s_saveInfoOld, fp, null);
    }

    //static ushort savegameVersion = 0x0290;
    /*
     * Save all kinds of important info to the savegame.
     * @param fp The file to save to.
     * @return True if and only if all bytes were written successful.
     */
    internal static bool Info_Save(BinaryWriter fp)
    {
        const ushort savegameVersion = 0x0290;

        if (!FWrite_LE_UInt16(savegameVersion, fp)) return false;

        if (!SaveLoad_Save(s_saveInfo, fp, null)) return false;

        return true;
    }
}
