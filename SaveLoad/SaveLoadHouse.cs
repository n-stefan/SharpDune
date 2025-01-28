namespace SharpDune.SaveLoad;

static class SaveLoadHouse
{
    static readonly SaveLoadDesc[] s_saveHouse = [
        SLD_ENTRY2(SLDT_UINT16, nameof(CHouse.index), SLDT_UINT8),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.harvestersIncoming)),
        SLD_ENTRY2(SLDT_UINT16, nameof(CHouse.flags), SLDT_HOUSEFLAGS),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.unitCount)),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.unitCountMax)),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.unitCountEnemy)),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.unitCountAllied)),
        SLD_ENTRY(SLDT_UINT32, nameof(CHouse.structuresBuilt)),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.credits)),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.creditsStorage)),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.powerProduction)),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.powerUsage)),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.windtrapCount)),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.creditsQuota)),
        SLD_ENTRY(SLDT_UINT16, $"{nameof(CHouse.palacePosition)}.{nameof(Tile32.x)}"),
        SLD_ENTRY(SLDT_UINT16, $"{nameof(CHouse.palacePosition)}.{nameof(Tile32.y)}"),
        SLD_EMPTY(SLDT_UINT16),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.timerUnitAttack)),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.timerSandwormAttack)),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.timerStructureAttack)),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.starportTimeLeft)),
        SLD_ENTRY(SLDT_UINT16, nameof(CHouse.starportLinkedID)),
        SLD_ARRAY(SLDT_UINT16, nameof(CHouse.ai_structureRebuild), 10),
        SLD_END()
    ];

    /*
     * Load all Houses from a file.
     * @param fp The file to load from.
     * @param length The length of the data chunk.
     * @return True if and only if all bytes were read successful.
     */
    internal static bool House_Load(BinaryReader fp, uint length)
    {
        while (length > 0)
        {
            CHouse hl;

            /* Read the next index from disk */
            var index = fp.ReadUInt16();

            /* Create the House in the pool */
            hl = House_Allocate((byte)index);
            if (hl == null) return false;

            fp.BaseStream.Seek(-2, SeekOrigin.Current);

            /* Read the next House from disk */
            if (!SaveLoad_Load(s_saveHouse, fp, hl)) return false;

            length -= SaveLoad_GetLength(s_saveHouse);

            /* See if it is a human house */
            if (hl.flags.human)
            {
                g_playerHouseID = (HouseType)hl.index;
                g_playerHouse = hl;

                if (hl.starportLinkedID != 0xFFFF && hl.starportTimeLeft == 0) hl.starportTimeLeft = 1;
            }
        }
        if (length != 0) return false;

        return true;
    }

    /*
     * Load all Houses from a file.
     * @param fp The file to load from.
     * @param length The length of the data chunk.
     * @return True if and only if all bytes were read successful.
     */
    internal static bool House_LoadOld(BinaryReader fp, uint length)
    {
        while (length > 0)
        {
            CHouse hl = null;

            /* Read the next House from disk */
            if (!SaveLoad_Load(s_saveHouse, fp, hl)) return false;

            /* See if it is a human house */
            if (hl.flags.human)
            {
                g_playerHouseID = (HouseType)hl.index;
                break;
            }

            length -= SaveLoad_GetLength(s_saveHouse);
        }
        if (length == 0) return false;

        return true;
    }

    /*
     * Save all Houses to a file.
     * @param fp The file to save to.
     * @return True if and only if all bytes were written successful.
     */
    internal static bool House_Save(BinaryWriter fp)
    {
        var find = new PoolFindStruct
        {
            houseID = (byte)HouseType.HOUSE_INVALID,
            type = 0xFFFF,
            index = 0xFFFF
        };

        while (true)
        {
            CHouse h = House_Find(find);
            if (h == null) break;

            if (!SaveLoad_Save(s_saveHouse, fp, h)) return false;
        }

        return true;
    }
}
