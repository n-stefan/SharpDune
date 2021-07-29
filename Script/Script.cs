/* Script routines */

using SharpDune.Include;
using SharpDune.Os;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using static SharpDune.Script.General;
using static SharpDune.Script.ScriptStructure;
using static SharpDune.Script.ScriptTeam;
using static SharpDune.Script.ScriptUnit;

namespace SharpDune.Script
{
    delegate ushort ScriptFunction(ScriptEngine script);

    /*
     * The valid types for command in ScriptInfo->start array.
     */
    enum ScriptCommand
    {
        SCRIPT_JUMP = 0,                     /*!< Jump to the instruction given by the parameter. */
        SCRIPT_SETRETURNVALUE = 1,           /*!< Set the return value to the value given by the parameter. */
        SCRIPT_PUSH_RETURN_OR_LOCATION = 2,  /*!< Push the return value (parameter = 0) or the location + framepointer (parameter = 1) on the stack. */
        SCRIPT_PUSH = 3,                     /*!< Push a value given by the parameter on the stack. */
        SCRIPT_PUSH2 = 4,                    /*!< Identical to SCRIPT_PUSH. */
        SCRIPT_PUSH_VARIABLE = 5,            /*!< Push a variable on the stack. */
        SCRIPT_PUSH_LOCAL_VARIABLE = 6,      /*!< Push a local variable on the stack. */
        SCRIPT_PUSH_PARAMETER = 7,           /*!< Push a parameter on the stack. */
        SCRIPT_POP_RETURN_OR_LOCATION = 8,   /*!< Pop the return value (parameter = 0) or the location + framepointer (parameter = 1) from the stack. */
        SCRIPT_POP_VARIABLE = 9,             /*!< Pop a variable from the stack. */
        SCRIPT_POP_LOCAL_VARIABLE = 10,      /*!< Pop a local variable from the stack. */
        SCRIPT_POP_PARAMETER = 11,           /*!< Pop a paramter from the stack. */
        SCRIPT_STACK_REWIND = 12,            /*!< Add a value given by the parameter to the stackpointer. */
        SCRIPT_STACK_FORWARD = 13,           /*!< Substract a value given by the parameter to the stackpointer. */
        SCRIPT_FUNCTION = 14,                /*!< Execute a function by its ID given by the parameter. */
        SCRIPT_JUMP_NE = 15,                 /*!< Jump to the instruction given by the parameter if the last entry on the stack is non-zero. */
        SCRIPT_UNARY = 16,                   /*!< Perform unary operations. */
        SCRIPT_BINARY = 17,                  /*!< Perform binary operations. */
        SCRIPT_RETURN = 18                   /*!< Return from a subroutine. */
    }

    /*
    * A ScriptEngine as stored in the memory.
    */
    class ScriptEngine
    {
        internal ushort delay;                                /*!< How many more ticks the script is suspended (or zero if not suspended). */
        internal /*CArray<ushort>*/ushort? script;            /*!< Pointer to current command in the script we are executing. */
        internal ScriptInfo scriptInfo;                       /*!< Pointer to a class with script information. */
        internal ushort returnValue;                          /*!< Return value from sub-routines. */
        internal byte framePointer;                           /*!< Frame pointer. */
        internal byte stackPointer;                           /*!< Stack pointer. */
        internal ushort[] variables = new ushort[5];          /*!< Variables to store values in (outside the stack). Accessed by all kinds of routines outside the scripts! */
        internal ushort[] stack = new ushort[15];             /*!< Stack of the script engine, starting at the end. */
        internal byte isSubroutine;                           /*!< The script we are executing is a subroutine. */
    }

    /*
    * A ScriptInfo as stored in the memory.
    */
    class ScriptInfo
    {
        //TODO: Change to ushort?
        internal ushort[] text;                               /*!< Pointer to TEXT section of the scripts. */
        internal /*CArray<ushort>*/ushort[] start;            /*!< Pointer to the begin of the scripts. */
        internal ushort[] offsets;                            /*!< Pointer to an array of offsets of where to start with a script for a typeID. */
        internal ushort offsetsCount;                         /*!< Number of words in offsets array. */
        internal ushort startCount;                           /*!< Number of words in start. */
        internal ScriptFunction[] functions;                  /*!< Pointer to an array of functions pointers which scripts with this scriptInfo can call. */
        internal ushort isAllocated;                          /*!< Memory has been allocated on load. */
    }

    class Script
    {
        static readonly ScriptInfo s_scriptStructure = new();
        static readonly ScriptInfo s_scriptTeam = new();
        static readonly ScriptInfo s_scriptUnit = new();
        internal static ScriptInfo g_scriptStructure = s_scriptStructure;
        internal static ScriptInfo g_scriptTeam = s_scriptTeam;
        internal static ScriptInfo g_scriptUnit = s_scriptUnit;

        internal static Object g_scriptCurrentObject;
        internal static Structure g_scriptCurrentStructure;
        internal static Unit g_scriptCurrentUnit;
        internal static Team g_scriptCurrentTeam;

        const int SCRIPT_FUNCTIONS_COUNT = 64;                            /*!< There are never more than 64 functions for a script category. */
        internal const int SCRIPT_UNIT_OPCODES_PER_TICK = 50;             /*!< The amount of opcodes a unit can execute per tick. */

        /*
         * Converted script functions for Structures.
         */
        internal static ScriptFunction[] g_scriptFunctionsStructure = {
	        /* 00 */ Script_General_Delay,
	        /* 01 */ Script_General_NoOperation,
	        /* 02 */ Script_Structure_Unknown0A81,
	        /* 03 */ Script_Structure_FindUnitByType,
	        /* 04 */ Script_Structure_SetState,
	        /* 05 */ Script_General_DisplayText,
	        /* 06 */ Script_Structure_Unknown11B9,
	        /* 07 */ Script_Structure_Unknown0C5A,
	        /* 08 */ Script_Structure_FindTargetUnit,
	        /* 09 */ Script_Structure_RotateTurret,
	        /* 0A */ Script_Structure_GetDirection,
	        /* 0B */ Script_Structure_Fire,
	        /* 0C */ Script_General_NoOperation,
	        /* 0D */ Script_Structure_GetState,
	        /* 0E */ Script_Structure_VoicePlay,
	        /* 0F */ Script_Structure_RemoveFogAroundTile,
	        /* 10 */ Script_General_NoOperation,
	        /* 11 */ Script_General_NoOperation,
	        /* 12 */ Script_General_NoOperation,
	        /* 13 */ Script_General_NoOperation,
	        /* 14 */ Script_General_NoOperation,
	        /* 15 */ Script_Structure_RefineSpice,
	        /* 16 */ Script_Structure_Explode,
	        /* 17 */ Script_Structure_Destroy,
	        /* 18 */ Script_General_NoOperation,
        };

        /*
         * Converted script functions for Teams.
         */
        internal static ScriptFunction[] g_scriptFunctionsTeam = { //ScriptFunction[SCRIPT_FUNCTIONS_COUNT]
	        /* 00 */ Script_General_Delay,
	        /* 01 */ Script_Team_DisplayText,
	        /* 02 */ Script_Team_GetMembers,
	        /* 03 */ Script_Team_AddClosestUnit,
	        /* 04 */ Script_Team_GetAverageDistance,
	        /* 05 */ Script_Team_Unknown0543,
	        /* 06 */ Script_Team_FindBestTarget,
	        /* 07 */ Script_Team_Unknown0788,
	        /* 08 */ Script_Team_Load,
	        /* 09 */ Script_Team_Load2,
	        /* 0A */ Script_General_DelayRandom,
	        /* 0B */ Script_General_DisplayModalMessage,
	        /* 0C */ Script_Team_GetVariable6,
	        /* 0D */ Script_Team_GetTarget,
	        /* 0E */ Script_General_NoOperation,
        };

        /*
         * Converted script functions for Units.
         */
        internal static ScriptFunction[] g_scriptFunctionsUnit = { //ScriptFunction[SCRIPT_FUNCTIONS_COUNT]
	        /* 00 */ Script_Unit_GetInfo,
	        /* 01 */ Script_Unit_SetAction,
	        /* 02 */ Script_General_DisplayText,
	        /* 03 */ Script_General_GetDistanceToTile,
	        /* 04 */ Script_Unit_StartAnimation,
	        /* 05 */ Script_Unit_SetDestination,
	        /* 06 */ Script_Unit_GetOrientation,
	        /* 07 */ Script_Unit_SetOrientation,
	        /* 08 */ Script_Unit_Fire,
	        /* 09 */ Script_Unit_MCVDeploy,
	        /* 0A */ Script_Unit_SetActionDefault,
	        /* 0B */ Script_Unit_Blink,
	        /* 0C */ Script_Unit_CalculateRoute,
	        /* 0D */ Script_General_IsEnemy,
	        /* 0E */ Script_Unit_ExplosionSingle,
	        /* 0F */ Script_Unit_Die,
	        /* 10 */ Script_General_Delay,
	        /* 11 */ Script_General_IsFriendly,
	        /* 12 */ Script_Unit_ExplosionMultiple,
	        /* 13 */ Script_Unit_SetSprite,
	        /* 14 */ Script_Unit_TransportDeliver,
	        /* 15 */ Script_General_NoOperation,
	        /* 16 */ Script_Unit_MoveToTarget,
	        /* 17 */ Script_General_RandomRange,
	        /* 18 */ Script_General_FindIdle,
	        /* 19 */ Script_Unit_SetDestinationDirect,
	        /* 1A */ Script_Unit_Stop,
	        /* 1B */ Script_Unit_SetSpeed,
	        /* 1C */ Script_Unit_FindBestTarget,
	        /* 1D */ Script_Unit_GetTargetPriority,
	        /* 1E */ Script_Unit_MoveToStructure,
	        /* 1F */ Script_Unit_IsInTransport,
	        /* 20 */ Script_Unit_GetAmount,
	        /* 21 */ Script_Unit_RandomSoldier,
	        /* 22 */ Script_Unit_Pickup,
	        /* 23 */ Script_Unit_CallUnitByType,
	        /* 24 */ Script_Unit_Unknown2552,
	        /* 25 */ Script_Unit_FindStructure,
	        /* 26 */ Script_General_VoicePlay,
	        /* 27 */ Script_Unit_DisplayDestroyedText,
	        /* 28 */ Script_Unit_RemoveFog,
	        /* 29 */ Script_General_SearchSpice,
	        /* 2A */ Script_Unit_Harvest,
	        /* 2B */ Script_General_NoOperation,
	        /* 2C */ Script_General_GetLinkedUnitType,
	        /* 2D */ Script_General_GetIndexType,
	        /* 2E */ Script_General_DecodeIndex,
	        /* 2F */ Script_Unit_IsValidDestination,
	        /* 30 */ Script_Unit_GetRandomTile,
	        /* 31 */ Script_Unit_IdleAction,
	        /* 32 */ Script_General_UnitCount,
	        /* 33 */ Script_Unit_GoToClosestStructure,
	        /* 34 */ Script_General_NoOperation,
	        /* 35 */ Script_General_NoOperation,
	        /* 36 */ Script_Unit_Sandworm_GetBestTarget,
	        /* 37 */ Script_Unit_Unknown2BD5,
	        /* 38 */ Script_General_GetOrientation,
	        /* 39 */ Script_General_NoOperation,
	        /* 3A */ Script_Unit_SetTarget,
	        /* 3B */ Script_General_Unknown0288,
	        /* 3C */ Script_General_DelayRandom,
	        /* 3D */ Script_Unit_Rotate,
	        /* 3E */ Script_General_GetDistanceToObject,
	        /* 3F */ Script_General_NoOperation,
        };

        /*
        * Clears the given scriptInfo.
        *
        * @param scriptInfo The scriptInfo to clear.
        */
        internal static void Script_ClearInfo(ScriptInfo scriptInfo)
        {
            if (scriptInfo == null) return;

            //if (scriptInfo.isAllocated != 0)
            //{
            //    free(scriptInfo->text);
            //    free(scriptInfo->offsets);
            //    free(scriptInfo->start);
            //}

            scriptInfo.text = null;
            scriptInfo.offsets = null;
            scriptInfo.start = null;
        }

        /*
         * Reset a script engine. It forgets the correct script it was executing,
         *  and resets stack and frame pointer. It also loads in the scriptInfo given
         *  by the parameter.
         *
         * @param script The script engine to reset.
         * @param scriptInfo The scriptInfo to load in the script. Can be NULL.
         */
        internal static void Script_Reset(ScriptEngine script, ScriptInfo scriptInfo)
        {
            if (script == null) return;
            if (scriptInfo == null) return;

            script.script = null;
            script.scriptInfo = scriptInfo;
            script.isSubroutine = 0;
            script.framePointer = 17;
            script.stackPointer = 15;
        }

        /*
         * Load a script in an engine. As script->scriptInfo already defines most
         *  of the information needed to load such script, all it needs is the type
         *  it needs to load the script for.
         *
         * @param script The script engine to load a script for.
         * @param typeID The typeID for which we want to load a script.
         */
        internal static void Script_Load(ScriptEngine script, byte typeID)
        {
            ScriptInfo scriptInfo;

            if (script == null) return;

            if (script.scriptInfo == null) return;
            scriptInfo = script.scriptInfo;

            Script_Reset(script, scriptInfo);

            script.script = scriptInfo.offsets[typeID];
            //script.script = new CArray<ushort> { Arr = scriptInfo.start.Arr, Ptr = scriptInfo.offsets[typeID] };
            //script.script = new CArray<ushort> { Arr = scriptInfo.start.Arr[scriptInfo.offsets[typeID]..] };
        }

        /*
         * Load a script in an engine without removing the previously loaded script.
         *
         * @param script The script engine to run.
         * @param typeID The typeID for which we want to load a script.
         */
        internal static void Script_LoadAsSubroutine(ScriptEngine script, byte typeID)
        {
            ScriptInfo scriptInfo;

            if (!Script_IsLoaded(script)) return;
            if (script.isSubroutine != 0) return;

            scriptInfo = script.scriptInfo;
            script.isSubroutine = 1;

            STACK_PUSH(script, (ushort)script.script);
            //STACK_PUSH(script, (ushort)(script.script.Ptr - scriptInfo.start.Ptr));
            STACK_PUSH(script, script.returnValue);

            script.script = scriptInfo.offsets[typeID];
            //script.script = new CArray<ushort> { Arr = scriptInfo.start.Arr[scriptInfo.offsets[typeID]..] };
        }

        /*
         * Check if a script is loaded in an engine. If returning true it means that
         *  the engine is actively executing a script.
         *
         * @param script The script engine to check on.
         * @return Returns true if and only if the script engine is actively executing a script.
         */
        internal static bool Script_IsLoaded(ScriptEngine script)
        {
            if (script == null) return false;
            if (script.script == null) return false;
            if (script.scriptInfo == null) return false;

            return true;
        }

#if DEBUG
        static void STACK_PUSH(ScriptEngine script, ushort value, [CallerFilePath]string filename = default, [CallerLineNumber]int lineno = default) =>
            Script_Stack_Push(script, value, filename, lineno);
        static ushort STACK_POP(ScriptEngine script, [CallerFilePath]string filename = default, [CallerLineNumber]int lineno = default) =>
            Script_Stack_Pop(script, filename, lineno);
        internal static ushort STACK_PEEK(ScriptEngine script, int position, [CallerFilePath]string filename = default, [CallerLineNumber]int lineno = default) =>
            Script_Stack_Peek(script, position, filename, lineno);
#else
        static void STACK_PUSH(ScriptEngine script, ushort value) =>
            Script_Stack_Push(script, value);
        static ushort STACK_POP(ScriptEngine script) =>
            Script_Stack_Pop(script);
        internal static ushort STACK_PEEK(ScriptEngine script, int position) =>
            Script_Stack_Peek(script, position);
#endif

        /*
         * Push a value on the stack.
         * @param value The value to push.
         * @note Use SCRIPT_PUSH(position) to use; do not use this function directly.
         */
#if DEBUG
        static void Script_Stack_Push(ScriptEngine script, ushort value, string filename = default, int lineno = default)
#else
        static void Script_Stack_Push(ScriptEngine script, ushort value)
#endif
        {
            if (script.stackPointer == 0)
            {
#if DEBUG
                Script_Error("Stack Overflow at {0}:{1}", filename, lineno);
#else
		        Script_Error("Stack Overflow");
#endif
                script.script = null;
                return;
            }

            script.stack[--script.stackPointer] = value;
        }

        /*
         * Pop a value from the stack.
         * @return The value that was on the stack.
         * @note Use SCRIPT_POP(position) to use; do not use this function directly.
         */
#if DEBUG
        static ushort Script_Stack_Pop(ScriptEngine script, string filename = default, int lineno = default)
#else
        static ushort Script_Stack_Pop(ScriptEngine script)
#endif
        {
            if (script.stackPointer >= 15)
            {
#if DEBUG
                Script_Error("Stack Overflow at {0}:{1}", filename, lineno);
#else
		        Script_Error("Stack Overflow");
#endif
                script.script = null;
                return 0;
            }

            return script.stack[script.stackPointer++];
        }

        /*
         * Peek a value from the stack.
         * @param position At which position you want to peek (1 = current, ..).
         * @return The value that was on the stack.
         * @note Use SCRIPT_PEEK(position) to use; do not use this function directly.
         */
#if DEBUG
        static ushort Script_Stack_Peek(ScriptEngine script, int position, string filename = default, int lineno = default)
#else
        static ushort Script_Stack_Peek(ScriptEngine script, int position)
#endif
        {
            Debug.Assert(position > 0);

            if (script.stackPointer >= 16 - position)
            {
#if DEBUG
                Script_Error("Stack Overflow at {0}:{1}", filename, lineno);
#else
		        Script_Error("Stack Overflow");
#endif
                script.script = null;
                return 0;
            }

            return script.stack[script.stackPointer + position - 1];
        }

        static readonly string[] l_types = { "Unit", "Structure", "Team", "Unknown" };
        /*
         * Show a script error with additional information (Type, Index, ..).
         * @param error The error to show.
         */
        static void Script_Error(string error, params object[] va)
        {
            var type = l_types[3];
            string buffer; //char[256]

            if (g_scriptCurrentUnit != null) type = l_types[0];
            if (g_scriptCurrentStructure != null) type = l_types[1];
            if (g_scriptCurrentTeam != null) type = l_types[2];

            buffer = new StringBuilder().AppendFormat(CSharpDune.Culture, error, va).ToString(); //vsnprintf(buffer, sizeof(buffer), error, va);

            Trace.WriteLine($"ERROR: [SCRIPT] {buffer}; Type: {type}; Index: {g_scriptCurrentObject.index}; Type: {g_scriptCurrentObject.type}");
        }

        /*
         * Clears the given scriptInfo.
         *
         * @param filename The name of the file to load.
         * @param scriptInfo The scriptInfo to load in the script.
         * @param functions Pointer to the functions to call via script.
         * @param data Pointer to preallocated space to load data.
         */
        internal static ushort Script_LoadFromFile(string filename, ScriptInfo scriptInfo, ScriptFunction[] functions, byte[] data)
        {
            uint total = 0;
            uint length;
            byte index;
            short i;
            var dataPointer = 0;

            if (scriptInfo == null) return 0;
            if (filename == null) return 0;

            Script_ClearInfo(scriptInfo);

            scriptInfo.isAllocated = (ushort)((data == null) ? 1 : 0);

            scriptInfo.functions = functions;

            if (!CFile.File_Exists(filename)) return 0;

            index = CFile.ChunkFile_Open(filename);

            length = CFile.ChunkFile_Seek(index, Endian.HTOBE32((uint)CSharpDune.MultiChar[FourCC.TEXT]));
            total += length;

            if (length != 0)
            {
                if (data != null)
                {
                    scriptInfo.text = Common.FromByteArrayToUshortArray(data[dataPointer..(dataPointer + (int)length)]);
                    dataPointer += (int)length;
                }
                else
                {
                    scriptInfo.text = new ushort[length / 2]; //calloc(1, length);
                }

                CFile.ChunkFile_Read(index, Endian.HTOBE32((uint)CSharpDune.MultiChar[FourCC.TEXT]), ref scriptInfo.text, length);
            }

            length = CFile.ChunkFile_Seek(index, Endian.HTOBE32((uint)CSharpDune.MultiChar[FourCC.ORDR]));
            total += length;

            if (length == 0)
            {
                Script_ClearInfo(scriptInfo);
                CFile.ChunkFile_Close(index);
                return 0;
            }

            if (data != null)
            {
                scriptInfo.offsets = Common.FromByteArrayToUshortArray(data[dataPointer..(dataPointer + (int)length)]);
                dataPointer += (int)length;
            }
            else
            {
                scriptInfo.offsets = new ushort[length / 2]; //calloc(1, length);
            }

            scriptInfo.offsetsCount = (ushort)((length >> 1) & 0xFFFF);
            CFile.ChunkFile_Read(index, Endian.HTOBE32((uint)CSharpDune.MultiChar[FourCC.ORDR]), ref scriptInfo.offsets, length);

            for (i = 0; i < (short)((length >> 1) & 0xFFFF); i++)
            {
                scriptInfo.offsets[i] = Endian.BETOH16(scriptInfo.offsets[i]);
            }

            length = CFile.ChunkFile_Seek(index, Endian.HTOBE32((uint)CSharpDune.MultiChar[FourCC.DATA]));
            total += length;

            if (length == 0)
            {
                Script_ClearInfo(scriptInfo);
                CFile.ChunkFile_Close(index);
                return 0;
            }

            if (data != null)
            {
                scriptInfo.start = Common.FromByteArrayToUshortArray(data[dataPointer..(dataPointer + (int)length)]);
                dataPointer += (int)length;
            }
            else
            {
                scriptInfo.start = new ushort[length / 2]; //calloc(1, length);
            }

            scriptInfo.startCount = (ushort)((length >> 1) & 0xFFFF);

            CFile.ChunkFile_Read(index, Endian.HTOBE32((uint)CSharpDune.MultiChar[FourCC.DATA]), ref scriptInfo.start, length);

            CFile.ChunkFile_Close(index);

            return (ushort)(total & 0xFFFF);
        }

        /*
         * Run the next opcode of a script.
         *
         * @param script The script engine to run.
         * @return Returns false if and only if there was an scripting error, like
         *   invalid opcode.
         */
        internal static bool Script_Run(ScriptEngine script, [CallerFilePath]string filename = default, [CallerLineNumber]int lineno = default)
        {
            ScriptInfo scriptInfo;
            ushort current, parameter;
            byte opcode;

            if (!Script_IsLoaded(script)) return false;
            scriptInfo = script.scriptInfo;

            current = Endian.BETOH16(scriptInfo.start[script.script.Value]);
            script.script++;

            opcode = (byte)((current >> 8) & 0x1F);
            parameter = 0;

            if ((current & 0x8000) != 0)
            {
                /* When this flag is set, the instruction is a GOTO with a 13bit address */
                opcode = 0;
                parameter = (ushort)(current & 0x7FFF);
            }
            else if ((current & 0x4000) != 0)
            {
                /* When this flag is set, the parameter is part of the instruction */
                parameter = (ushort)/*(short)*/(sbyte)(current & 0xFF);
            }
            else if ((current & 0x2000) != 0)
            {
                /* When this flag is set, the parameter is in the next opcode */
                parameter = Endian.BETOH16(scriptInfo.start[script.script.Value]);
                script.script++;
            }

            switch ((ScriptCommand)opcode)
            {
                case ScriptCommand.SCRIPT_JUMP:
                    {
                        script.script = parameter;
                        return true;
                    }

                case ScriptCommand.SCRIPT_SETRETURNVALUE:
                    {
                        script.returnValue = parameter;
                        return true;
                    }

                case ScriptCommand.SCRIPT_PUSH_RETURN_OR_LOCATION:
                    {
                        if (parameter == 0)
                        { /* PUSH RETURNVALUE */
                            STACK_PUSH(script, script.returnValue);
                            return true;
                        }

                        if (parameter == 1)
                        { /* PUSH NEXT LOCATION + FRAMEPOINTER */
                            uint location;
                            location = (uint)script.script + 1;

                            STACK_PUSH(script, (ushort)location);
                            STACK_PUSH(script, script.framePointer);
                            script.framePointer = (byte)(script.stackPointer + 2);

                            return true;
                        }

                        Script_Error("Unknown parameter {0} for opcode 2", parameter);
                        script.script = null;
                        return false;
                    }

                case ScriptCommand.SCRIPT_PUSH:
                case ScriptCommand.SCRIPT_PUSH2:
                    {
                        STACK_PUSH(script, parameter);
                        return true;
                    }

                case ScriptCommand.SCRIPT_PUSH_VARIABLE:
                    {
                        STACK_PUSH(script, script.variables[parameter]);
                        return true;
                    }

                case ScriptCommand.SCRIPT_PUSH_LOCAL_VARIABLE:
                    {
                        if (script.framePointer - parameter - 2 >= 15)
                        {
#if DEBUG
                            Script_Error("Stack Overflow at {0}:{1}", filename, lineno);
#else
                            Script_Error("Stack Overflow");
#endif
                            script.script = null;
                            return false;
                        }

                        STACK_PUSH(script, script.stack[script.framePointer - parameter - 2]);
                        return true;
                    }

                case ScriptCommand.SCRIPT_PUSH_PARAMETER:
                    {
                        if (script.framePointer + parameter - 1 >= 15)
                        {
#if DEBUG
                            Script_Error("Stack Overflow at {0}:{1}", filename, lineno);
#else
                            Script_Error("Stack Overflow");
#endif
                            script.script = null;
                            return false;
                        }

                        STACK_PUSH(script, script.stack[script.framePointer + parameter - 1]);
                        return true;
                    }

                case ScriptCommand.SCRIPT_POP_RETURN_OR_LOCATION:
                    {
                        if (parameter == 0)
                        { /* POP RETURNVALUE */
                            script.returnValue = STACK_POP(script);
                            return true;
                        }
                        if (parameter == 1)
                        { /* POP FRAMEPOINTER + LOCATION */
                            STACK_PEEK(script, 2); if (script.script == null) return false;

                            script.framePointer = (byte)STACK_POP(script);
                            script.script = STACK_POP(script);
                            return true;
                        }

                        Script_Error("Unknown parameter {0} for opcode 8", parameter);
                        script.script = null;
                        return false;
                    }

                case ScriptCommand.SCRIPT_POP_VARIABLE:
                    {
                        script.variables[parameter] = STACK_POP(script);
                        return true;
                    }

                case ScriptCommand.SCRIPT_POP_LOCAL_VARIABLE:
                    {
                        if (script.framePointer - parameter - 2 >= 15)
                        {
#if DEBUG
                            Script_Error("Stack Overflow at {0}:{1}", filename, lineno);
#else
                            Script_Error("Stack Overflow");
#endif
                            script.script = null;
                            return false;
                        }

                        script.stack[script.framePointer - parameter - 2] = STACK_POP(script);
                        return true;
                    }

                case ScriptCommand.SCRIPT_POP_PARAMETER:
                    {
                        if (script.framePointer + parameter - 1 >= 15)
                        {
#if DEBUG
                            Script_Error("Stack Overflow at {0}:{1}", filename, lineno);
#else
                            Script_Error("Stack Overflow");
#endif
                            script.script = null;
                            return false;
                        }

                        script.stack[script.framePointer + parameter - 1] = STACK_POP(script);
                        return true;
                    }

                case ScriptCommand.SCRIPT_STACK_REWIND:
                    {
                        script.stackPointer += (byte)parameter;
                        return true;
                    }

                case ScriptCommand.SCRIPT_STACK_FORWARD:
                    {
                        script.stackPointer -= (byte)parameter;
                        return true;
                    }

                case ScriptCommand.SCRIPT_FUNCTION:
                    {
                        parameter &= 0xFF;

                        if (parameter >= SCRIPT_FUNCTIONS_COUNT || scriptInfo.functions[parameter] == null)
                        {
                            Script_Error("Unknown function {0} for opcode 14", parameter);
                            return false;
                        }

                        script.returnValue = scriptInfo.functions[parameter](script);
                        return true;
                    }

                case ScriptCommand.SCRIPT_JUMP_NE:
                    {
                        STACK_PEEK(script, 1); if (script.script == null) return false;

                        if (STACK_POP(script) != 0) return true;

                        script.script = (ushort?)(parameter & 0x7FFF);
                        return true;
                    }

                case ScriptCommand.SCRIPT_UNARY:
                    {
                        if (parameter == 0)
                        { /* STACK = !STACK */
                            STACK_PUSH(script, (ushort)(STACK_POP(script) == 0 ? 1 : 0));
                            return true;
                        }
                        if (parameter == 1)
                        { /* STACK = -STACK */
                            STACK_PUSH(script, (ushort)-STACK_POP(script));
                            return true;
                        }
                        if (parameter == 2)
                        { /* STACK = ~STACK */
                            STACK_PUSH(script, (ushort)~STACK_POP(script));
                            return true;
                        }

                        Script_Error("Unknown parameter {0} for opcode 16", parameter);
                        script.script = null;
                        return false;
                    }

                case ScriptCommand.SCRIPT_BINARY:
                    {
                        var right = (short)STACK_POP(script);
                        var left = (short)STACK_POP(script);

                        switch (parameter)
                        {
                            case 0: STACK_PUSH(script, (ushort)(Convert.ToBoolean(left) && Convert.ToBoolean(right) ? 1 : 0)); break; /* left && right */
                            case 1: STACK_PUSH(script, (ushort)(Convert.ToBoolean(left) || Convert.ToBoolean(right) ? 1 : 0)); break; /* left || right */
                            case 2: STACK_PUSH(script, (ushort)(left == right ? 1 : 0)); break; /* left == right */
                            case 3: STACK_PUSH(script, (ushort)(left != right ? 1 : 0)); break; /* left != right */
                            case 4: STACK_PUSH(script, (ushort)(left < right ? 1 : 0)); break; /* left <  right */
                            case 5: STACK_PUSH(script, (ushort)(left <= right ? 1 : 0)); break; /* left <= right */
                            case 6: STACK_PUSH(script, (ushort)(left > right ? 1 : 0)); break; /* left >  right */
                            case 7: STACK_PUSH(script, (ushort)(left >= right ? 1 : 0)); break; /* left >= right */
                            case 8: STACK_PUSH(script, (ushort)(left + right)); break; /* left +  right */
                            case 9: STACK_PUSH(script, (ushort)(left - right)); break; /* left -  right */
                            case 10: STACK_PUSH(script, (ushort)(left * right)); break; /* left *  right */
                            case 11: STACK_PUSH(script, (ushort)(left / right)); break; /* left /  right */
                            case 12: STACK_PUSH(script, (ushort)(left >> right)); break; /* left >> right */
                            case 13: STACK_PUSH(script, (ushort)(left << right)); break; /* left << right */
                            case 14: STACK_PUSH(script, (ushort)(left & right)); break; /* left &  right */
                            case 15: STACK_PUSH(script, (ushort)(left | right)); break; /* left |  right */
                            case 16: STACK_PUSH(script, (ushort)(left % right)); break; /* left %  right */
                            case 17: STACK_PUSH(script, (ushort)(left ^ right)); break; /* left ^  right */

                            default:
                                Script_Error("Unknown parameter {0} for opcode 17", parameter);
                                script.script = null;
                                return false;
                        }

                        return true;
                    }
                case ScriptCommand.SCRIPT_RETURN:
                    {
                        STACK_PEEK(script, 2); if (script.script == null) return false;

                        script.returnValue = STACK_POP(script);
                        script.script = STACK_POP(script);

                        script.isSubroutine = 0;
                        return true;
                    }

                default:
                    Script_Error("Unknown opcode {0}", opcode);
                    script.script = null;
                    return false;
            }
        }
    }
}
