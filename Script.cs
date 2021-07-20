/* Generic script */

using SharpDune.Audio;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SharpDune
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
	        /* 02 */ CStructure.Script_Structure_Unknown0A81,
	        /* 03 */ CStructure.Script_Structure_FindUnitByType,
	        /* 04 */ CStructure.Script_Structure_SetState,
	        /* 05 */ Script_General_DisplayText,
	        /* 06 */ CStructure.Script_Structure_Unknown11B9,
	        /* 07 */ CStructure.Script_Structure_Unknown0C5A,
	        /* 08 */ CStructure.Script_Structure_FindTargetUnit,
	        /* 09 */ CStructure.Script_Structure_RotateTurret,
	        /* 0A */ CStructure.Script_Structure_GetDirection,
	        /* 0B */ CStructure.Script_Structure_Fire,
	        /* 0C */ Script_General_NoOperation,
	        /* 0D */ CStructure.Script_Structure_GetState,
	        /* 0E */ CStructure.Script_Structure_VoicePlay,
	        /* 0F */ CStructure.Script_Structure_RemoveFogAroundTile,
	        /* 10 */ Script_General_NoOperation,
	        /* 11 */ Script_General_NoOperation,
	        /* 12 */ Script_General_NoOperation,
	        /* 13 */ Script_General_NoOperation,
	        /* 14 */ Script_General_NoOperation,
	        /* 15 */ CStructure.Script_Structure_RefineSpice,
	        /* 16 */ CStructure.Script_Structure_Explode,
	        /* 17 */ CStructure.Script_Structure_Destroy,
	        /* 18 */ Script_General_NoOperation,
        };

        /*
         * Converted script functions for Teams.
         */
        internal static ScriptFunction[] g_scriptFunctionsTeam = { //ScriptFunction[SCRIPT_FUNCTIONS_COUNT]
	        /* 00 */ Script_General_Delay,
	        /* 01 */ CTeam.Script_Team_DisplayText,
	        /* 02 */ CTeam.Script_Team_GetMembers,
	        /* 03 */ CTeam.Script_Team_AddClosestUnit,
	        /* 04 */ CTeam.Script_Team_GetAverageDistance,
	        /* 05 */ CTeam.Script_Team_Unknown0543,
	        /* 06 */ CTeam.Script_Team_FindBestTarget,
	        /* 07 */ CTeam.Script_Team_Unknown0788,
	        /* 08 */ CTeam.Script_Team_Load,
	        /* 09 */ CTeam.Script_Team_Load2,
	        /* 0A */ Script_General_DelayRandom,
	        /* 0B */ Script_General_DisplayModalMessage,
	        /* 0C */ CTeam.Script_Team_GetVariable6,
	        /* 0D */ CTeam.Script_Team_GetTarget,
	        /* 0E */ Script_General_NoOperation,
        };

        /*
         * Converted script functions for Units.
         */
        internal static ScriptFunction[] g_scriptFunctionsUnit = { //ScriptFunction[SCRIPT_FUNCTIONS_COUNT]
	        /* 00 */ CUnit.Script_Unit_GetInfo,
	        /* 01 */ CUnit.Script_Unit_SetAction,
	        /* 02 */ Script_General_DisplayText,
	        /* 03 */ Script_General_GetDistanceToTile,
	        /* 04 */ CUnit.Script_Unit_StartAnimation,
	        /* 05 */ CUnit.Script_Unit_SetDestination,
	        /* 06 */ CUnit.Script_Unit_GetOrientation,
	        /* 07 */ CUnit.Script_Unit_SetOrientation,
	        /* 08 */ CUnit.Script_Unit_Fire,
	        /* 09 */ CUnit.Script_Unit_MCVDeploy,
	        /* 0A */ CUnit.Script_Unit_SetActionDefault,
	        /* 0B */ CUnit.Script_Unit_Blink,
	        /* 0C */ CUnit.Script_Unit_CalculateRoute,
	        /* 0D */ Script_General_IsEnemy,
	        /* 0E */ CUnit.Script_Unit_ExplosionSingle,
	        /* 0F */ CUnit.Script_Unit_Die,
	        /* 10 */ Script_General_Delay,
	        /* 11 */ Script_General_IsFriendly,
	        /* 12 */ CUnit.Script_Unit_ExplosionMultiple,
	        /* 13 */ CUnit.Script_Unit_SetSprite,
	        /* 14 */ CUnit.Script_Unit_TransportDeliver,
	        /* 15 */ Script_General_NoOperation,
	        /* 16 */ CUnit.Script_Unit_MoveToTarget,
	        /* 17 */ Script_General_RandomRange,
	        /* 18 */ Script_General_FindIdle,
	        /* 19 */ CUnit.Script_Unit_SetDestinationDirect,
	        /* 1A */ CUnit.Script_Unit_Stop,
	        /* 1B */ CUnit.Script_Unit_SetSpeed,
	        /* 1C */ CUnit.Script_Unit_FindBestTarget,
	        /* 1D */ CUnit.Script_Unit_GetTargetPriority,
	        /* 1E */ CUnit.Script_Unit_MoveToStructure,
	        /* 1F */ CUnit.Script_Unit_IsInTransport,
	        /* 20 */ CUnit.Script_Unit_GetAmount,
	        /* 21 */ CUnit.Script_Unit_RandomSoldier,
	        /* 22 */ CUnit.Script_Unit_Pickup,
	        /* 23 */ CUnit.Script_Unit_CallUnitByType,
	        /* 24 */ CUnit.Script_Unit_Unknown2552,
	        /* 25 */ CUnit.Script_Unit_FindStructure,
	        /* 26 */ Script_General_VoicePlay,
	        /* 27 */ CUnit.Script_Unit_DisplayDestroyedText,
	        /* 28 */ CUnit.Script_Unit_RemoveFog,
	        /* 29 */ Script_General_SearchSpice,
	        /* 2A */ CUnit.Script_Unit_Harvest,
	        /* 2B */ Script_General_NoOperation,
	        /* 2C */ Script_General_GetLinkedUnitType,
	        /* 2D */ Script_General_GetIndexType,
	        /* 2E */ Script_General_DecodeIndex,
	        /* 2F */ CUnit.Script_Unit_IsValidDestination,
	        /* 30 */ CUnit.Script_Unit_GetRandomTile,
	        /* 31 */ CUnit.Script_Unit_IdleAction,
	        /* 32 */ Script_General_UnitCount,
	        /* 33 */ CUnit.Script_Unit_GoToClosestStructure,
	        /* 34 */ Script_General_NoOperation,
	        /* 35 */ Script_General_NoOperation,
	        /* 36 */ CUnit.Script_Unit_Sandworm_GetBestTarget,
	        /* 37 */ CUnit.Script_Unit_Unknown2BD5,
	        /* 38 */ Script_General_GetOrientation,
	        /* 39 */ Script_General_NoOperation,
	        /* 3A */ CUnit.Script_Unit_SetTarget,
	        /* 3B */ Script_General_Unknown0288,
	        /* 3C */ Script_General_DelayRandom,
	        /* 3D */ CUnit.Script_Unit_Rotate,
	        /* 3E */ Script_General_GetDistanceToObject,
	        /* 3F */ Script_General_NoOperation,
        };

        /*
         * Suspend the script execution for a set amount of ticks.
         *
         * Stack: 1 - delay value in ticks.
         *
         * @param script The script engine to operate on.
         * @return Amount of ticks the script will be suspended, divided by 5.
         *
         * @note Scripts are executed every 5 ticks, so the delay is divided by 5. You
         *  can't delay your script for 4 ticks or less.
         */
        static ushort Script_General_Delay(ScriptEngine script)
        {
            ushort delay;

            delay = (ushort)(STACK_PEEK(script, 1) / 5);

            script.delay = delay;

            return delay;
        }

        /*
         * Do nothing. This function has absolutely no functionality other than
         *  returning the value 0.
         *
         * Stack: *none*
         *
         * @param script The script engine to operate on
         * @return The value 0. Always.
         */
        static ushort Script_General_NoOperation(ScriptEngine script) =>
            0;

        /*
         * Suspend the script execution for a randomized amount of ticks, with an
         *  upper limit given.
         *
         * Stack: 1 - maximum amount of delay in ticks.
         *
         * @param script The script engine to operate on.
         * @return Amount of ticks the script will be suspended, divided by 5.
         */
        static ushort Script_General_DelayRandom(ScriptEngine script)
        {
            ushort delay;

            delay = (ushort)(Tools.Tools_Random_256() * STACK_PEEK(script, 1) / 256);
            delay /= 5;

            script.delay = delay;

            return delay;
        }

        /*
         * Display a modal message.
         *
         * Stack: 1 - The index of a string.
         *
         * @param script The script engine to operate on.
         * @return unknown.
         */
        static ushort Script_General_DisplayModalMessage(ScriptEngine script)
        {
            string text; //char *
            ushort offset;

            offset = Endian.BETOH16(script.scriptInfo.text[STACK_PEEK(script, 1)]);
            text = script.scriptInfo.text[offset..].Cast<char>().ToString();

            return Gui.GUI_DisplayModalMessage(text, 0xFFFF);
        }

        /*
         * Draws a string.
         *
         * Stack: 1 - The index of the string to draw.
         *        2-4 - The arguments for the string.
         *
         * @param script The script engine to operate on.
         * @return The value 0. Always.
         */
        static ushort Script_General_DisplayText(ScriptEngine script)
        {
            string text; //char *
            ushort offset;

            offset = Endian.BETOH16(script.scriptInfo.text[STACK_PEEK(script, 1)]);
            text = script.scriptInfo.text[offset..].Cast<char>().ToString();

            Gui.GUI_DisplayText(text, 0, STACK_PEEK(script, 2), STACK_PEEK(script, 3), STACK_PEEK(script, 4));

            return 0;
        }

        /*
         * Get the distance from the current unit/structure to the tile.
         *
         * Stack: 1 - An encoded tile index.
         *
         * @param script The script engine to operate on.
         * @return Distance to it, where distance is (longest(x,y) + shortest(x,y) / 2).
         */
        static ushort Script_General_GetDistanceToTile(ScriptEngine script)
        {
            Object o;
            ushort encoded;

            encoded = STACK_PEEK(script, 1);
            o = g_scriptCurrentObject;

            if (!Tools.Tools_Index_IsValid(encoded)) return 0xFFFF;

            return CTile.Tile_GetDistance(o.position, Tools.Tools_Index_GetTile(encoded));
        }

        /*
         * Check if a Unit/Structure is an enemy.
         *
         * Stack: 1 - An encoded index.
         *
         * @param script The script engine to operate on.
         * @return Zero if and only if the Unit/Structure is friendly.
         */
        static ushort Script_General_IsEnemy(ScriptEngine script)
        {
            byte houseID;
            ushort index;

            index = STACK_PEEK(script, 1);

            if (!Tools.Tools_Index_IsValid(index)) return 0;

            houseID = (g_scriptCurrentUnit != null) ? CUnit.Unit_GetHouseID(g_scriptCurrentUnit) : g_scriptCurrentObject.houseID;

            switch (Tools.Tools_Index_GetType(index))
            {
                case IndexType.IT_UNIT: return (ushort)((CUnit.Unit_GetHouseID(Tools.Tools_Index_GetUnit(index)) != houseID) ? 1 : 0);
                case IndexType.IT_STRUCTURE: return (ushort)((Tools.Tools_Index_GetStructure(index).o.houseID != houseID) ? 1 : 0);
                default: return 0;
            }
        }

        /*
         * Check if a Unit/Structure is a friend.
         *
         * Stack: 1 - An encoded index.
         *
         * @param script The script engine to operate on.
         * @return Either 1 (friendly) or -1 (enemy).
         */
        static ushort Script_General_IsFriendly(ScriptEngine script)
        {
            ushort index;
            Object o;
            ushort res;

            index = STACK_PEEK(script, 1);

            o = Tools.Tools_Index_GetObject(index);

            if (o == null || o.flags.isNotOnMap || !o.flags.used) return 0;

            res = Script_General_IsEnemy(script);

            return (ushort)((res == 0) ? 1 : -1);
        }

        /*
         * Get a random value between min and max.
         *
         * Stack: 1 - The minimum value.
         *        2 - The maximum value.
         *
         * @param script The script engine to operate on.
         * @return The random value.
         */
        static ushort Script_General_RandomRange(ScriptEngine script) =>
            Tools.Tools_RandomLCG_Range(STACK_PEEK(script, 1), STACK_PEEK(script, 2));

        /*
         * Two sided function. If the parameter is an index, it will return 1 if and
         *  only if the structure indicated is idle. If the parameter is not an index,
         *  it is a structure type, and this function will return the first structure
         *  that is of that type and idle.
         *
         * Stack: 1 - An encoded index or a Structure type.
         *
         * @param script The script engine to operate on.
         * @return Zero or one to indicate idle, or the index of the structure which is idle, depending on the input parameter.
         */
        static ushort Script_General_FindIdle(ScriptEngine script)
        {
            byte houseID;
            ushort index;
            Structure s;
            var find = new PoolFindStruct();

            index = STACK_PEEK(script, 1);

            houseID = g_scriptCurrentObject.houseID;

            if (Tools.Tools_Index_GetType(index) == IndexType.IT_UNIT) return 0;
            if (Tools.Tools_Index_GetType(index) == IndexType.IT_TILE) return 0;

            if (Tools.Tools_Index_GetType(index) == IndexType.IT_STRUCTURE)
            {
                s = Tools.Tools_Index_GetStructure(index);
                if (s.o.houseID != houseID) return 0;
                if (s.state != (short)StructureState.STRUCTURE_STATE_IDLE) return 0;
                return 1;
            }

            find.houseID = houseID;
            find.index = 0xFFFF;
            find.type = index;

            while (true)
            {
                s = CStructure.Structure_Find(find);
                if (s == null) return 0;
                if (s.state != (short)StructureState.STRUCTURE_STATE_IDLE) continue;
                return Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);
            }
        }

        /*
         * Play a voice.
         *
         * Stack: 1 - The VoiceID to play.
         *
         * @param script The script engine to operate on.
         * @return The value 0. Always.
         */
        static ushort Script_General_VoicePlay(ScriptEngine script)
        {
            tile32 position;

            position = g_scriptCurrentObject.position;

            Sound.Voice_PlayAtTile((short)STACK_PEEK(script, 1), position);

            return 0;
        }

        /*
         * Search for spice nearby.
         *
         * Stack: 1 - Radius of the search.
         *
         * @param script The script engine to operate on.
         * @return Encoded position with spice, or \c 0 if no spice nearby.
         */
        static ushort Script_General_SearchSpice(ScriptEngine script)
        {
            tile32 position;
            ushort packedSpicePos;

            position = g_scriptCurrentObject.position;

            packedSpicePos = Map.Map_SearchSpice(CTile.Tile_PackTile(position), STACK_PEEK(script, 1));

            if (packedSpicePos == 0) return 0;
            return Tools.Tools_Index_Encode(packedSpicePos, IndexType.IT_TILE);
        }

        /*
         * Gets the type of the current object's linked unit.
         *
         * Stack: *none*.
         *
         * @param script The script engine to operate on.
         * @return The type, or 0xFFFF if no linked unit.
         */
        static ushort Script_General_GetLinkedUnitType(ScriptEngine script)
        {
            ushort linkedID;

            linkedID = g_scriptCurrentObject.linkedID;

            if (linkedID == 0xFF) return 0xFFFF;

            return CUnit.Unit_Get_ByIndex(linkedID).o.type;
        }

        /*
         * Gets the type of the given encoded index.
         *
         * Stack: 1 - An encoded index.
         *
         * @param script The script engine to operate on.
         * @return The type, or 0xFFFF if invalid.
         */
        static ushort Script_General_GetIndexType(ScriptEngine script)
        {
            ushort index;

            index = STACK_PEEK(script, 1);

            if (!Tools.Tools_Index_IsValid(index)) return 0xFFFF;

            return (ushort)Tools.Tools_Index_GetType(index);
        }

        /*
         * Decodes the given encoded index.
         *
         * Stack: 1 - An encoded index.
         *
         * @param script The script engine to operate on.
         * @return The decoded index, or 0xFFFF if invalid.
         */
        static ushort Script_General_DecodeIndex(ScriptEngine script)
        {
            ushort index;

            index = STACK_PEEK(script, 1);

            if (!Tools.Tools_Index_IsValid(index)) return 0xFFFF;

            return Tools.Tools_Index_Decode(index);
        }

        /*
         * Counts how many unit of the given type are owned by current object's owner.
         *
         * Stack: 1 - An unit type.
         *
         * @param script The script engine to operate on.
         * @return The count.
         */
        static ushort Script_General_UnitCount(ScriptEngine script)
        {
            ushort count = 0;
            var find = new PoolFindStruct
            {
                houseID = g_scriptCurrentObject.houseID,
                type = STACK_PEEK(script, 1),
                index = 0xFFFF
            };

            while (true)
            {
                var u = CUnit.Unit_Find(find);
                if (u == null) break;
                count++;
            }

            return count;
        }

        /*
         * Get orientation of a unit.
         *
         * Stack: 1 - An encoded index.
         *
         * @param script The script engine to operate on.
         * @return The orientation of the unit.
         */
        static ushort Script_General_GetOrientation(ScriptEngine script)
        {
            Unit u;

            u = Tools.Tools_Index_GetUnit(STACK_PEEK(script, 1));

            if (u == null) return 128;

            return (ushort)u.orientation[0].current;
        }

        /*
         * Unknown function 0288.
         *
         * Stack: 1 - An encoded index.
         *
         * @param script The script engine to operate on.
         * @return unknown.
         */
        static ushort Script_General_Unknown0288(ScriptEngine script)
        {
            ushort index;
            Structure s;

            index = STACK_PEEK(script, 1);
            s = Tools.Tools_Index_GetStructure(index);

            if (s != null && Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE) != index) return 1;

            return (ushort)((Tools.Tools_Index_GetObject(index) == null) ? 1 : 0);
        }

        /*
         * Get the distance from the current unit/structure to the unit/structure.
         *
         * Stack: 1 - An encoded unit/structure index.
         *
         * @param script The script engine to operate on.
         * @return Distance to it, where distance is (longest(x,y) + shortest(x,y) / 2).
         */
        static ushort Script_General_GetDistanceToObject(ScriptEngine script)
        {
            ushort index;

            index = STACK_PEEK(script, 1);

            if (!Tools.Tools_Index_IsValid(index)) return 0xFFFF;

            return CObject.Object_GetDistanceToEncoded(g_scriptCurrentObject, index);
        }

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
