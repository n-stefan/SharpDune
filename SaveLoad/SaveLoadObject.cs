using static SharpDune.SaveLoad.SaveLoad;

namespace SharpDune.SaveLoad
{
    class SaveLoadObject
    {
		internal static readonly SaveLoadDesc[] g_saveObject = {
            SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT16, nameof(Object.index)),
            SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT8, nameof(Object.type)),
            SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT8, nameof(Object.linkedID)),
            SLD_ENTRY2(/*obj,*/ SaveLoadType.SLDT_UINT32, nameof(Object.flags), SaveLoadType.SLDT_OBJECTFLAGS),
            SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT8, nameof(Object.houseID)),
            SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT8, nameof(Object.seenByHouses)),
            SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT16, $"{nameof(Object.position)}.{nameof(tile32.x)}"),
            SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT16, $"{nameof(Object.position)}.{nameof(tile32.y)}"),
            SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT16, nameof(Object.hitpoints)),
            SLD_SLD(/*obj,*/ nameof(Object.script), SaveLoadScriptEngine.g_saveScriptEngine),
            SLD_END()
		};
	}
}
