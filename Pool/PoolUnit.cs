namespace SharpDune.Pool;

static class PoolUnit
{
    internal enum UnitIndex
    {
        UNIT_INDEX_MAX = 102,                                    /*!< The highest possible index for any Unit. */

        UNIT_INDEX_INVALID = 0xFFFF
    }

    static readonly CUnit[] g_unitArray = new CUnit[(int)UnitIndex.UNIT_INDEX_MAX];
    internal static readonly CUnit[] g_unitFindArray = new CUnit[(int)UnitIndex.UNIT_INDEX_MAX];
    internal static ushort g_unitFindCount;

    /*
     * Get a Unit from the pool with the indicated index.
     *
     * @param index The index of the Unit to get.
     * @return The Unit.
     */
    internal static CUnit Unit_Get_ByIndex(ushort index)
    {
        Debug.Assert(index < (ushort)UnitIndex.UNIT_INDEX_MAX);
        return g_unitArray[index];
    }

    //internal static void Unit_Set_ByIndex(Unit u)
    //{
    //	Debug.Assert(u.o.index < (ushort)UnitIndex.UNIT_INDEX_MAX);
    //	g_unitArray[u.o.index] = u;
    //}

    /*
     * Find the first matching Unit based on the PoolFindStruct filter data.
     *
     * @param find A pointer to a PoolFindStruct which contains filter data and
     * last known tried index. Calling this functions multiple times with the
     * same 'find' parameter walks over all possible values matching the filter.
     * @return The Unit, or NULL if nothing matches (anymore).
     */
    internal static CUnit Unit_Find(PoolFindStruct find)
    {
        if (find.index >= g_unitFindCount && find.index != 0xFFFF) return null;
        find.index++; /* First, we always go to the next index */

        for (; find.index < g_unitFindCount; find.index++)
        {
            var u = g_unitFindArray[find.index];
            if (u == null) continue;

            if (u.o.flags.isNotOnMap && g_validateStrictIfZero == 0) continue;
            if (find.houseID != (byte)HouseType.HOUSE_INVALID && find.houseID != Unit_GetHouseID(u)) continue;
            if (find.type != (ushort)UnitIndex.UNIT_INDEX_INVALID && find.type != u.o.type) continue;

            return u;
        }

        return null;
    }

    /*
     * Initialize the Unit array.
     */
    internal static void Unit_Init()
    {
        for (var i = 0; i < g_unitArray.Length; i++) g_unitArray[i] = new CUnit(); //memset(g_unitArray, 0, sizeof(g_unitArray));
        Array.Fill(g_unitFindArray, null, 0, g_unitFindArray.Length); //memset(g_unitFindArray, 0, sizeof(g_unitFindArray));
        g_unitFindCount = 0;
    }

    /*
     * Recount all Units, ignoring the cache array. Also set the unitCount
     *  of all houses to zero.
     */
    internal static void Unit_Recount()
    {
        ushort index;
        var find = new PoolFindStruct();
        unchecked { find.houseID = (byte)-1; find.type = (ushort)-1; find.index = (ushort)-1; }
        var h = House_Find(find);

        while (h != null)
        {
            h.unitCount = 0;
            h = House_Find(find);
        }

        g_unitFindCount = 0;

        for (index = 0; index < (ushort)UnitIndex.UNIT_INDEX_MAX; index++)
        {
            var u = Unit_Get_ByIndex(index);
            if (!u.o.flags.used) continue;

            h = House_Get_ByIndex(u.o.houseID);
            h.unitCount++;

            g_unitFindArray[g_unitFindCount++] = u;
        }
    }

    /*
     * Allocate a Unit.
     *
     * @param index The index to use, or UNIT_INDEX_INVALID to find an unused index.
     * @param typeID The type of the new Unit.
     * @param houseID The House of the new Unit.
     * @return The Unit allocated, or NULL on failure.
     */
    internal static CUnit Unit_Allocate(ushort index, byte type, byte houseID)
    {
        CHouse h;
        CUnit u = null;

        if (type == 0xFF || houseID == 0xFF) return null;

        h = House_Get_ByIndex(houseID);
        if (h.unitCount >= h.unitCountMax)
        {
            if (g_table_unitInfo[type].movementType is not ((ushort)MovementType.MOVEMENT_WINGER) and not ((ushort)MovementType.MOVEMENT_SLITHER))
            {
                if (g_validateStrictIfZero == 0) return null;
            }
        }

        if (index is 0 or ((ushort)UnitIndex.UNIT_INDEX_INVALID))
        {
            var indexStart = g_table_unitInfo[type].indexStart;
            var indexEnd = g_table_unitInfo[type].indexEnd;

            for (index = indexStart; index <= indexEnd; index++)
            {
                u = Unit_Get_ByIndex(index);
                if (!u.o.flags.used) break;
            }
            if (index > indexEnd) return null;
        }
        else
        {
            u = Unit_Get_ByIndex(index);
            if (u.o.flags.used) return null;
        }
        Debug.Assert(u != null);

        h.unitCount++;

        /* Initialize the Unit */
        //memset(u, 0, sizeof(Unit));
        u.o.index = index;
        u.o.type = type;
        u.o.houseID = houseID;
        u.o.linkedID = 0xFF;
        u.o.flags.used = true;
        u.o.flags.allocated = true;
        u.o.flags.isUnit = true;
        u.o.script.delay = 0;
        u.route[0] = 0xFF;
        if (type == (byte)UnitType.UNIT_SANDWORM) u.amount = 3;

        g_unitFindArray[g_unitFindCount++] = u;

        return u;
    }

    /*
     * Free a Unit.
     *
     * @param address The address of the Unit to free.
     */
    internal static void Unit_Free(CUnit u)
    {
        int i;

        u.o.flags = new ObjectFlags(); //memset(&u->o.flags, 0, sizeof(u->o.flags));

        Script_Reset(u.o.script, g_scriptUnit);

        /* Walk the array to find the Unit we are removing */
        for (i = 0; i < g_unitFindCount; i++)
        {
            if (g_unitFindArray[i] == u) break;
        }
        Debug.Assert(i < g_unitFindCount); /* We should always find an entry */

        g_unitFindCount--;

        {
            var h = House_Get_ByIndex(u.o.houseID);
            h.unitCount--;
        }

        /* If needed, close the gap */
        if (i == g_unitFindCount) return;

        Array.Copy(g_unitFindArray, i + 1, g_unitFindArray, i, g_unitFindCount - i); //memmove(&g_unitFindArray[i], &g_unitFindArray[i + 1], (g_unitFindCount - i) * sizeof(g_unitFindArray[0]));
    }
}
