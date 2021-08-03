namespace SharpDune.Pool
{
    class PoolHouse
    {
        enum HouseIndex
        {
            HOUSE_INDEX_MAX = 6,                                    /*!< The highest possible index for any House.  */

            HOUSE_INDEX_INVALID = 0xFFFF
        }

        static readonly House[] g_houseArray = new House[(int)HouseIndex.HOUSE_INDEX_MAX];
        static readonly House[] g_houseFindArray = new House[(int)HouseIndex.HOUSE_INDEX_MAX];
        static ushort g_houseFindCount;

        /*
         * Get a House from the pool with the indicated index.
         *
         * @param index The index of the House to get.
         * @return The House.
         */
        internal static House House_Get_ByIndex(byte index)
        {
            Debug.Assert(index < (byte)HouseIndex.HOUSE_INDEX_MAX);
            return g_houseArray[index];
        }

        //internal static void House_Set_ByIndex(House h)
        //{
        //    Debug.Assert(h.index < (byte)HouseIndex.HOUSE_INDEX_MAX);
        //    g_houseArray[h.index] = h;
        //}

        /*
         * Find the first matching House based on the PoolFindStruct filter data.
         *
         * @param find A pointer to a PoolFindStruct which contains filter data and
         *   last known tried index. Calling this functions multiple times with the
         *   same 'find' parameter walks over all possible values matching the filter.
         * @return The House, or NULL if nothing matches (anymore).
         */
        internal static House House_Find(PoolFindStruct find)
        {
            if (find.index >= g_houseFindCount && find.index != 0xFFFF) return null;
            find.index++; /* First, we always go to the next index */

            for (; find.index < g_houseFindCount; find.index++)
            {
                var h = g_houseFindArray[find.index];
                if (h != null) return h;
            }

            return null;
        }

        /*
         * Initialize the House array.
         *
         * @param address If non-zero, the new location of the House array.
         */
        internal static void House_Init()
        {
            for (var i = 0; i < g_houseArray.Length; i++) g_houseArray[i] = new House(); //memset(g_houseArray, 0, sizeof(g_houseArray));
            Array.Fill(g_houseFindArray, null, 0, g_houseFindArray.Length); //memset(g_houseFindArray, 0, sizeof(g_houseFindArray));
            g_houseFindCount = 0;
        }

        /*
         * Allocate a House.
         *
         * @param index The index to use.
         * @return The House allocated, or NULL on failure.
         */
        internal static House House_Allocate(byte index)
        {
            House h;

            if (index >= (byte)HouseIndex.HOUSE_INDEX_MAX) return null;

            h = House_Get_ByIndex(index);
            if (h.flags.used) return null;

            /* Initialize the House */
            //memset(h, 0, sizeof(House));
            h.index = index;
            h.flags.used = true;
            h.starportLinkedID = (ushort)UnitIndex.UNIT_INDEX_INVALID;

            g_houseFindArray[g_houseFindCount++] = h;

            return h;
        }

        //internal static void House_Allocate(House h)
        //{
        //    if (h.index >= (byte)HouseIndex.HOUSE_INDEX_MAX) return;

        //    House_Set_ByIndex(h);

        //    g_houseFindArray[g_houseFindCount++] = h;
        //}
    }
}
