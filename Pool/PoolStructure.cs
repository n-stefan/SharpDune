namespace SharpDune.Pool
{
    class PoolStructure
    {
        internal enum StructureIndex
        {
            STRUCTURE_INDEX_MAX_SOFT = 79,                          /*!< The highest possible index for normal Structure. */
            STRUCTURE_INDEX_MAX_HARD = 82,                          /*!< The highest possible index for any Structure. */

            STRUCTURE_INDEX_WALL = 79,                              /*!< All walls are are put under index 79. */
            STRUCTURE_INDEX_SLAB_2x2 = 80,                          /*!< All 2x2 slabs are put under index 80. */
            STRUCTURE_INDEX_SLAB_1x1 = 81,                          /*!< All 1x1 slabs are put under index 81. */

            STRUCTURE_INDEX_INVALID = 0xFFFF
        }

        static readonly Structure[] g_structureArray = new Structure[(int)StructureIndex.STRUCTURE_INDEX_MAX_HARD];
        static readonly Structure[] g_structureFindArray = new Structure[(int)StructureIndex.STRUCTURE_INDEX_MAX_SOFT];
        static ushort g_structureFindCount;

        /*
        * Get a Structure from the pool with the indicated index.
        * @param index The index of the Structure to get.
        * @return The Structure.
        */
        internal static Structure Structure_Get_ByIndex(ushort index)
        {
            Debug.Assert(index < (ushort)StructureIndex.STRUCTURE_INDEX_MAX_HARD);
            return g_structureArray[index];
        }

        //internal static void Structure_Set_ByIndex(Structure s)
        //{
        //    Debug.Assert(s.o.index < (ushort)StructureIndex.STRUCTURE_INDEX_MAX_HARD);
        //    g_structureArray[s.o.index] = s;
        //}

        /*
        * Find the first matching Structure based on the PoolFindStruct filter data.
        *
        * @param find A pointer to a PoolFindStruct which contains filter data and
        *   last known tried index. Calling this functions multiple times with the
        *   same 'find' parameter walks over all possible values matching the filter.
        * @return The Structure, or NULL if nothing matches (anymore).
        */
        internal static Structure Structure_Find(PoolFindStruct find)
        {
            if (find.index >= g_structureFindCount + 3 && find.index != 0xFFFF) return null;
            find.index++; /* First, we always go to the next index */

            Debug.Assert(g_structureFindCount <= (ushort)StructureIndex.STRUCTURE_INDEX_MAX_SOFT);
            for (; find.index < g_structureFindCount + 3; find.index++)
            {
                Structure s = null;

                if (find.index < g_structureFindCount)
                {
                    s = g_structureFindArray[find.index];
                }
                else
                {
                    /* There are 3 special structures that are never in the Find array */
                    Debug.Assert(find.index - g_structureFindCount < 3);
                    switch (find.index - g_structureFindCount)
                    {
                        case 0:
                            s = Structure_Get_ByIndex((ushort)StructureIndex.STRUCTURE_INDEX_WALL);
                            if (s.o.index != (ushort)StructureIndex.STRUCTURE_INDEX_WALL) continue;
                            break;

                        case 1:
                            s = Structure_Get_ByIndex((ushort)StructureIndex.STRUCTURE_INDEX_SLAB_2x2);
                            if (s.o.index != (ushort)StructureIndex.STRUCTURE_INDEX_SLAB_2x2) continue;
                            break;

                        case 2:
                            s = Structure_Get_ByIndex((ushort)StructureIndex.STRUCTURE_INDEX_SLAB_1x1);
                            if (s.o.index != (ushort)StructureIndex.STRUCTURE_INDEX_SLAB_1x1) continue;
                            break;
                    }
                }
                if (s == null) continue;

                if (s.o.flags.isNotOnMap && g_validateStrictIfZero == 0) continue;
                if (find.houseID != (byte)HouseType.HOUSE_INVALID && find.houseID != s.o.houseID) continue;
                if (find.type != (ushort)StructureIndex.STRUCTURE_INDEX_INVALID && find.type != s.o.type) continue;

                return s;
            }

            return null;
        }

        /*
         * Initialize the Structure array.
         *
         * @param address If non-zero, the new location of the Structure array.
         */
        internal static void Structure_Init()
        {
            for (var i = 0; i < g_structureArray.Length; i++) g_structureArray[i] = new Structure(); //memset(g_structureArray, 0, sizeof(g_structureArray));
            Array.Fill(g_structureFindArray, null, 0, g_structureFindArray.Length); //memset(g_structureFindArray, 0, sizeof(g_structureFindArray));
            g_structureFindCount = 0;
        }

        /*
         * Recount all Structures, ignoring the cache array. Also set the structureCount
         *  of all houses to zero.
         */
        internal static void Structure_Recount()
        {
            ushort index;
            var find = new PoolFindStruct();
            unchecked { find.houseID = (byte)-1; find.type = (ushort)-1; find.index = (ushort)-1; }
            var h = PoolHouse.House_Find(find);

            while (h != null)
            {
                h.unitCount = 0;
                h = PoolHouse.House_Find(find);
            }

            g_structureFindCount = 0;

            for (index = 0; index < (ushort)StructureIndex.STRUCTURE_INDEX_MAX_SOFT; index++)
            {
                var s = Structure_Get_ByIndex(index);
                if (s.o.flags.used) g_structureFindArray[g_structureFindCount++] = s;
            }
        }

        /*
         * Allocate a Structure.
         *
         * @param index The index to use, or STRUCTURE_INDEX_INVALID to find an unused index.
         * @param typeID The type of the new Structure.
         * @return The Structure allocated, or NULL on failure.
         */
        internal static Structure Structure_Allocate(ushort index, byte type)
        {
            Structure s = null;

            switch ((StructureType)type)
            {
                case StructureType.STRUCTURE_SLAB_1x1:
                    index = (ushort)StructureIndex.STRUCTURE_INDEX_SLAB_1x1;
                    s = Structure_Get_ByIndex(index);
                    break;

                case StructureType.STRUCTURE_SLAB_2x2:
                    index = (ushort)StructureIndex.STRUCTURE_INDEX_SLAB_2x2;
                    s = Structure_Get_ByIndex(index);
                    break;

                case StructureType.STRUCTURE_WALL:
                    index = (ushort)StructureIndex.STRUCTURE_INDEX_WALL;
                    s = Structure_Get_ByIndex(index);
                    break;

                default:
                    if (index == (ushort)StructureIndex.STRUCTURE_INDEX_INVALID)
                    {
                        /* Find the first unused index */
                        for (index = 0; index < (ushort)StructureIndex.STRUCTURE_INDEX_MAX_SOFT; index++)
                        {
                            s = Structure_Get_ByIndex(index);
                            if (!s.o.flags.used) break;
                        }
                        if (index == (ushort)StructureIndex.STRUCTURE_INDEX_MAX_SOFT) return null;
                    }
                    else
                    {
                        s = Structure_Get_ByIndex(index);
                        if (s.o.flags.used) return null;
                    }

                    g_structureFindArray[g_structureFindCount++] = s;
                    break;
            }
            Debug.Assert(s != null);

            /* Initialize the Structure */
            //memset(s, 0, sizeof(Structure));
            s.o.index = index;
            s.o.type = type;
            s.o.linkedID = 0xFF;
            s.o.flags.used = true;
            s.o.flags.allocated = true;
            s.o.script.delay = 0;

            return s;
        }

        /*
         * Free a Structure.
         *
         * @param address The address of the Structure to free.
         */
        internal static void Structure_Free(Structure s)
        {
            int i;

            s.o.flags = new ObjectFlags(); //memset(&s->o.flags, 0, sizeof(s->o.flags));

            Script_Reset(s.o.script, g_scriptStructure);

            if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) return;

            /* Walk the array to find the Structure we are removing */
            Debug.Assert(g_structureFindCount <= (ushort)StructureIndex.STRUCTURE_INDEX_MAX_SOFT);
            for (i = 0; i < g_structureFindCount; i++)
            {
                if (g_structureFindArray[i] == s) break;
            }
            Debug.Assert(i < g_structureFindCount); /* We should always find an entry */

            g_structureFindCount--;

            /* If needed, close the gap */
            if (i == g_structureFindCount) return;
            Array.Copy(g_structureFindArray, i + 1, g_structureFindArray, i, g_structureFindCount - i); //memmove(&g_structureFindArray[i], &g_structureFindArray[i + 1], (g_structureFindCount - i) * sizeof(g_structureFindArray[0]));
        }
    }
}
