/* Generic script routines */

namespace SharpDune.Script
{
    class General
    {
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
        internal static ushort Script_General_Delay(ScriptEngine script)
        {
            ushort delay;

            delay = (ushort)(STACK_PEEK(script, 1) / 5);

            script.delay = delay;

            return delay;
        }

        /*
         * Suspend the script execution for a randomized amount of ticks, with an
         *  upper limit given.
         *
         * Stack: 1 - maximum amount of delay in ticks.
         *
         * @param script The script engine to operate on.
         * @return Amount of ticks the script will be suspended, divided by 5.
         */
        internal static ushort Script_General_DelayRandom(ScriptEngine script)
        {
            ushort delay;

            delay = (ushort)(Tools.Tools_Random_256() * STACK_PEEK(script, 1) / 256);
            delay /= 5;

            script.delay = delay;

            return delay;
        }

        /*
         * Get the distance from the current unit/structure to the tile.
         *
         * Stack: 1 - An encoded tile index.
         *
         * @param script The script engine to operate on.
         * @return Distance to it, where distance is (longest(x,y) + shortest(x,y) / 2).
         */
        internal static ushort Script_General_GetDistanceToTile(ScriptEngine script)
        {
            Object o;
            ushort encoded;

            encoded = STACK_PEEK(script, 1);
            o = g_scriptCurrentObject;

            if (!Tools.Tools_Index_IsValid(encoded)) return 0xFFFF;

            return CTile.Tile_GetDistance(o.position, Tools.Tools_Index_GetTile(encoded));
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
        internal static ushort Script_General_NoOperation(ScriptEngine script) =>
            0;

        /*
         * Draws a string.
         *
         * Stack: 1 - The index of the string to draw.
         *        2-4 - The arguments for the string.
         *
         * @param script The script engine to operate on.
         * @return The value 0. Always.
         */
        internal static ushort Script_General_DisplayText(ScriptEngine script)
        {
            string text; //char *
            ushort offset;

            offset = Endian.BETOH16(script.scriptInfo.text[STACK_PEEK(script, 1)]);
            text = script.scriptInfo.text[offset..].Cast<char>().ToString();

            Gui.Gui.GUI_DisplayText(text, 0, STACK_PEEK(script, 2), STACK_PEEK(script, 3), STACK_PEEK(script, 4));

            return 0;
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
        internal static ushort Script_General_RandomRange(ScriptEngine script) =>
            Tools.Tools_RandomLCG_Range(STACK_PEEK(script, 1), STACK_PEEK(script, 2));

        /*
         * Display a modal message.
         *
         * Stack: 1 - The index of a string.
         *
         * @param script The script engine to operate on.
         * @return unknown.
         */
        internal static ushort Script_General_DisplayModalMessage(ScriptEngine script)
        {
            string text; //char *
            ushort offset;

            offset = Endian.BETOH16(script.scriptInfo.text[STACK_PEEK(script, 1)]);
            text = script.scriptInfo.text[offset..].Cast<char>().ToString();

            return Gui.Gui.GUI_DisplayModalMessage(text, 0xFFFF);
        }

        /*
         * Get the distance from the current unit/structure to the unit/structure.
         *
         * Stack: 1 - An encoded unit/structure index.
         *
         * @param script The script engine to operate on.
         * @return Distance to it, where distance is (longest(x,y) + shortest(x,y) / 2).
         */
        internal static ushort Script_General_GetDistanceToObject(ScriptEngine script)
        {
            ushort index;

            index = STACK_PEEK(script, 1);

            if (!Tools.Tools_Index_IsValid(index)) return 0xFFFF;

            return CObject.Object_GetDistanceToEncoded(g_scriptCurrentObject, index);
        }

        /*
         * Unknown function 0288.
         *
         * Stack: 1 - An encoded index.
         *
         * @param script The script engine to operate on.
         * @return unknown.
         */
        internal static ushort Script_General_Unknown0288(ScriptEngine script)
        {
            ushort index;
            Structure s;

            index = STACK_PEEK(script, 1);
            s = Tools.Tools_Index_GetStructure(index);

            if (s != null && Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE) != index) return 1;

            return (ushort)((Tools.Tools_Index_GetObject(index) == null) ? 1 : 0);
        }

        /*
         * Get orientation of a unit.
         *
         * Stack: 1 - An encoded index.
         *
         * @param script The script engine to operate on.
         * @return The orientation of the unit.
         */
        internal static ushort Script_General_GetOrientation(ScriptEngine script)
        {
            Unit u;

            u = Tools.Tools_Index_GetUnit(STACK_PEEK(script, 1));

            if (u == null) return 128;

            return (ushort)u.orientation[0].current;
        }

        /*
         * Counts how many unit of the given type are owned by current object's owner.
         *
         * Stack: 1 - An unit type.
         *
         * @param script The script engine to operate on.
         * @return The count.
         */
        internal static ushort Script_General_UnitCount(ScriptEngine script)
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
                var u = PoolUnit.Unit_Find(find);
                if (u == null) break;
                count++;
            }

            return count;
        }

        /*
         * Decodes the given encoded index.
         *
         * Stack: 1 - An encoded index.
         *
         * @param script The script engine to operate on.
         * @return The decoded index, or 0xFFFF if invalid.
         */
        internal static ushort Script_General_DecodeIndex(ScriptEngine script)
        {
            ushort index;

            index = STACK_PEEK(script, 1);

            if (!Tools.Tools_Index_IsValid(index)) return 0xFFFF;

            return Tools.Tools_Index_Decode(index);
        }

        /*
         * Gets the type of the given encoded index.
         *
         * Stack: 1 - An encoded index.
         *
         * @param script The script engine to operate on.
         * @return The type, or 0xFFFF if invalid.
         */
        internal static ushort Script_General_GetIndexType(ScriptEngine script)
        {
            ushort index;

            index = STACK_PEEK(script, 1);

            if (!Tools.Tools_Index_IsValid(index)) return 0xFFFF;

            return (ushort)Tools.Tools_Index_GetType(index);
        }

        /*
         * Gets the type of the current object's linked unit.
         *
         * Stack: *none*.
         *
         * @param script The script engine to operate on.
         * @return The type, or 0xFFFF if no linked unit.
         */
        internal static ushort Script_General_GetLinkedUnitType(ScriptEngine script)
        {
            ushort linkedID;

            linkedID = g_scriptCurrentObject.linkedID;

            if (linkedID == 0xFF) return 0xFFFF;

            return PoolUnit.Unit_Get_ByIndex(linkedID).o.type;
        }

        /*
         * Play a voice.
         *
         * Stack: 1 - The VoiceID to play.
         *
         * @param script The script engine to operate on.
         * @return The value 0. Always.
         */
        internal static ushort Script_General_VoicePlay(ScriptEngine script)
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
        internal static ushort Script_General_SearchSpice(ScriptEngine script)
        {
            tile32 position;
            ushort packedSpicePos;

            position = g_scriptCurrentObject.position;

            packedSpicePos = Map.Map_SearchSpice(CTile.Tile_PackTile(position), STACK_PEEK(script, 1));

            if (packedSpicePos == 0) return 0;
            return Tools.Tools_Index_Encode(packedSpicePos, IndexType.IT_TILE);
        }

        /*
         * Check if a Unit/Structure is a friend.
         *
         * Stack: 1 - An encoded index.
         *
         * @param script The script engine to operate on.
         * @return Either 1 (friendly) or -1 (enemy).
         */
        internal static ushort Script_General_IsFriendly(ScriptEngine script)
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
         * Check if a Unit/Structure is an enemy.
         *
         * Stack: 1 - An encoded index.
         *
         * @param script The script engine to operate on.
         * @return Zero if and only if the Unit/Structure is friendly.
         */
        internal static ushort Script_General_IsEnemy(ScriptEngine script)
        {
            byte houseID;
            ushort index;

            index = STACK_PEEK(script, 1);

            if (!Tools.Tools_Index_IsValid(index)) return 0;

            houseID = (g_scriptCurrentUnit != null) ? Unit_GetHouseID(g_scriptCurrentUnit) : g_scriptCurrentObject.houseID;

            switch (Tools.Tools_Index_GetType(index))
            {
                case IndexType.IT_UNIT: return (ushort)((Unit_GetHouseID(Tools.Tools_Index_GetUnit(index)) != houseID) ? 1 : 0);
                case IndexType.IT_STRUCTURE: return (ushort)((Tools.Tools_Index_GetStructure(index).o.houseID != houseID) ? 1 : 0);
                default: return 0;
            }
        }

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
        internal static ushort Script_General_FindIdle(ScriptEngine script)
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
                s = PoolStructure.Structure_Find(find);
                if (s == null) return 0;
                if (s.state != (short)StructureState.STRUCTURE_STATE_IDLE) continue;
                return Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);
            }
        }
    }
}
