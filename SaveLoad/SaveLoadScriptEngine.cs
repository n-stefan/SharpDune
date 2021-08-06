namespace SharpDune.SaveLoad
{
    class SaveLoadScriptEngine
    {
        internal static readonly SaveLoadDesc[] g_saveScriptEngine = {
            SLD_ENTRY(SLDT_UINT16, nameof(ScriptEngine.delay)),
            SLD_CALLB(SLDT_UINT32, nameof(ScriptEngine.script), SaveLoad_Script_Script),
            SLD_EMPTY(SLDT_UINT32),
            SLD_ENTRY(SLDT_UINT16, nameof(ScriptEngine.returnValue)),
            SLD_ENTRY(SLDT_UINT8, nameof(ScriptEngine.framePointer)),
            SLD_ENTRY(SLDT_UINT8, nameof(ScriptEngine.stackPointer)),
            SLD_ARRAY(SLDT_UINT16, nameof(ScriptEngine.variables), 5),
            SLD_ARRAY(SLDT_UINT16, nameof(ScriptEngine.stack), 15),
            SLD_ENTRY(SLDT_UINT8, nameof(ScriptEngine.isSubroutine)),
            SLD_END()
        };

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
    }
}
