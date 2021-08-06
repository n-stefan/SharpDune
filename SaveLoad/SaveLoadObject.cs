namespace SharpDune.SaveLoad
{
    class SaveLoadObject
    {
		internal static readonly SaveLoadDesc[] g_saveObject = {
            SLD_ENTRY(SLDT_UINT16, nameof(CObject.index)),
            SLD_ENTRY(SLDT_UINT8, nameof(CObject.type)),
            SLD_ENTRY(SLDT_UINT8, nameof(CObject.linkedID)),
            SLD_ENTRY2(SLDT_UINT32, nameof(CObject.flags), SLDT_OBJECTFLAGS),
            SLD_ENTRY(SLDT_UINT8, nameof(CObject.houseID)),
            SLD_ENTRY(SLDT_UINT8, nameof(CObject.seenByHouses)),
            SLD_ENTRY(SLDT_UINT16, $"{nameof(CObject.position)}.{nameof(Tile32.x)}"),
            SLD_ENTRY(SLDT_UINT16, $"{nameof(CObject.position)}.{nameof(Tile32.y)}"),
            SLD_ENTRY(SLDT_UINT16, nameof(CObject.hitpoints)),
            SLD_SLD(nameof(CObject.script), g_saveScriptEngine),
            SLD_END()
		};
	}
}
