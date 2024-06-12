namespace SharpDune.SaveLoad;

static class SaveLoadTeam
{
    static readonly SaveLoadDesc[] s_saveTeam = [
        SLD_ENTRY(SLDT_UINT16, nameof(CTeam.index)),
        SLD_ENTRY2(SLDT_UINT16, nameof(CTeam.flags), SLDT_TEAMFLAGS),
        SLD_ENTRY(SLDT_UINT16, nameof(CTeam.members)),
        SLD_ENTRY(SLDT_UINT16, nameof(CTeam.minMembers)),
        SLD_ENTRY(SLDT_UINT16, nameof(CTeam.maxMembers)),
        SLD_ENTRY(SLDT_UINT16, nameof(CTeam.movementType)),
        SLD_ENTRY(SLDT_UINT16, nameof(CTeam.action)),
        SLD_ENTRY(SLDT_UINT16, nameof(CTeam.actionStart)),
        SLD_ENTRY(SLDT_UINT8, nameof(CTeam.houseID)),
        SLD_EMPTY2(SLDT_UINT8, 3),
        SLD_ENTRY(SLDT_UINT16, $"{nameof(CTeam.position)}.{nameof(Tile32.x)}"),
        SLD_ENTRY(SLDT_UINT16, $"{nameof(CTeam.position)}.{nameof(Tile32.y)}"),
        SLD_ENTRY(SLDT_UINT16, nameof(CTeam.targetTile)),
        SLD_ENTRY(SLDT_UINT16, nameof(CTeam.target)),
        SLD_SLD(nameof(CTeam.script), g_saveScriptEngine),
        SLD_END()
    ];

    /*
     * Load all Teams from a file.
     * @param fp The file to load from.
     * @param length The length of the data chunk.
     * @return True if and only if all bytes were read successful.
     */
    internal static bool Team_Load(BinaryReader fp, uint length)
    {
        while (length > 0)
        {
            CTeam tl;

            /* Read the next index from disk */
            var index = fp.ReadUInt16();

            /* Get the Team from the pool */
            tl = Team_Get_ByIndex(index);
            if (tl == null) return false;

            fp.BaseStream.Seek(-2, SeekOrigin.Current);

            /* Read the next Team from disk */
            if (!SaveLoad_Load(s_saveTeam, fp, tl)) return false;

            length -= SaveLoad_GetLength(s_saveTeam);

            tl.script.scriptInfo = g_scriptTeam;
        }
        if (length != 0) return false;

        Team_Recount();

        return true;
    }

    /*
     * Save all Teams to a file. It converts pointers to indices where needed.
     * @param fp The file to save to.
     * @return True if and only if all bytes were written successful.
     */
    internal static bool Team_Save(BinaryWriter fp)
    {
        var find = new PoolFindStruct
        {
            houseID = (byte)HouseType.HOUSE_INVALID,
            type = 0xFFFF,
            index = 0xFFFF
        };

        while (true)
        {
            CTeam t;

            t = Team_Find(find);
            if (t == null) break;

            if (!SaveLoad_Save(s_saveTeam, fp, t)) return false;
        }

        return true;
    }
}
