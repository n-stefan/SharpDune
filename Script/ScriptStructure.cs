/* Structure script routines */

namespace SharpDune.Script;

class ScriptStructure
{
    /*
     * Get the state of the current structure.
     *
     * Stack: *none*
     *
     * @param script The script engine to operate on.
     * @return State of current structure.
     */
    internal static ushort Script_Structure_GetState(ScriptEngine script)
    {
        CStructure s;

        s = g_scriptCurrentStructure;
        return (ushort)s.state;
    }

    /*
     * Set the state for the current structure.
     *
     * Stack: 1 - The state.
     *
     * @param script The script engine to operate on.
     * @return The value 0. Always.
     */
    internal static ushort Script_Structure_SetState(ScriptEngine script)
    {
        CStructure s;
        short state;

        s = g_scriptCurrentStructure;
        state = (short)STACK_PEEK(script, 1);

        if (state == (short)StructureState.STRUCTURE_STATE_DETECT)
        {
            if (s.o.linkedID == 0xFF)
            {
                state = (short)StructureState.STRUCTURE_STATE_IDLE;
            }
            else
            {
                state = s.countDown == 0
                    ? (short)StructureState.STRUCTURE_STATE_READY
                    : (short)StructureState.STRUCTURE_STATE_BUSY;
            }
        }

        Structure_SetState(s, state);

        return 0;
    }

    /*
     * Remove fog around the current structure.
     * Radius to uncover is taken from the current structure info.
     *
     * Stack: *none*
     *
     * @param script The script engine to operate on.
     * @return The value 0. Always.
     */
    internal static ushort Script_Structure_RemoveFogAroundTile(ScriptEngine script)
    {
        Structure_RemoveFog(g_scriptCurrentStructure);

        return 0;
    }

    /*
     * Refine spice in the current structure.
     *
     * Stack: *none*
     *
     * @param script The script engine to operate on.
     * @return 0 if there is no spice to refine, otherwise 1.
     */
    internal static ushort Script_Structure_RefineSpice(ScriptEngine script)
    {
        StructureInfo si;
        CStructure s;
        CUnit u;
        CHouse h;
        ushort harvesterStep, creditsStep;

        s = g_scriptCurrentStructure;

        if (s.o.linkedID == 0xFF)
        {
            Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_IDLE);
            return 0;
        }

        u = Unit_Get_ByIndex(s.o.linkedID);
        si = g_table_structureInfo[s.o.type];

        harvesterStep = (ushort)((s.o.hitpoints * 256 / si.o.hitpoints) * 3 / 256);

        if (u.amount < harvesterStep) harvesterStep = u.amount;
        if (u.amount != 0 && harvesterStep < 1) harvesterStep = 1;
        if (harvesterStep == 0) return 0;

        creditsStep = 7;
        if (u.o.houseID != (byte)g_playerHouseID)
        {
            creditsStep += (ushort)((Tools_Random_256() % 4) - 1);
        }

        creditsStep *= harvesterStep;

        if (House_AreAllied((byte)g_playerHouseID, s.o.houseID))
        {
            g_scenario.harvestedAllied += creditsStep;
            if (g_scenario.harvestedAllied > 65000) g_scenario.harvestedAllied = 65000;
        }
        else
        {
            g_scenario.harvestedEnemy += creditsStep;
            if (g_scenario.harvestedEnemy > 65000) g_scenario.harvestedEnemy = 65000;
        }

        h = House_Get_ByIndex(s.o.houseID);
        h.credits += creditsStep;
        u.amount -= (byte)harvesterStep;

        if (u.amount == 0) u.o.flags.inTransport = false;
        s.o.script.delay = 6;
        return 1;
    }

    /*
     * Unknown function 0A81.
     *
     * Stack: *none*
     *
     * @param script The script engine to operate on.
     * @return unknown.
     */
    internal static ushort Script_Structure_Unknown0A81(ScriptEngine script)
    {
        ushort structureIndex;
        CStructure s;
        CUnit u;

        s = g_scriptCurrentStructure;

        structureIndex = Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);

        u = Tools_Index_GetUnit(s.o.script.variables[4]);
        if (u != null)
        {
            if (structureIndex == u.o.script.variables[4]) return s.o.script.variables[4];
            Object_Script_Variable4_Clear(u.o);
        }

        Object_Script_Variable4_Clear(s.o);

        return 0;
    }

    /*
     * Find a UnitType and make it go to the current structure. In general, type
     *  should be a Carry-All for this to make any sense.
     *
     * Stack: 1 - An unit type.
     *
     * @param script The script engine to operate on.
     * @return unknown.
     */
    internal static ushort Script_Structure_FindUnitByType(ScriptEngine script)
    {
        CStructure s;
        CUnit u;
        CUnit carryall;
        ushort type;
        ushort position;
        ushort carryallIndex;

        s = g_scriptCurrentStructure;

        if (s.state != (short)StructureState.STRUCTURE_STATE_READY) return (ushort)IndexType.IT_NONE;
        if (s.o.linkedID == 0xFF) return (ushort)IndexType.IT_NONE;

        type = STACK_PEEK(script, 1);

        position = Structure_FindFreePosition(s, false);

        u = Unit_Get_ByIndex(s.o.linkedID);

        if ((byte)g_playerHouseID == s.o.houseID && u.o.type == (byte)UnitType.UNIT_HARVESTER && (u.targetLast.x == 0 && u.targetLast.y == 0) && position != 0)
        {
            return (ushort)IndexType.IT_NONE;
        }

        carryall = Unit_CallUnitByType((UnitType)type, s.o.houseID, Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE), position == 0);

        if (carryall == null) return (ushort)IndexType.IT_NONE;

        carryallIndex = Tools_Index_Encode(carryall.o.index, IndexType.IT_UNIT);
        Object_Script_Variable4_Set(s.o, carryallIndex);

        return carryallIndex;
    }

    /*
     * Unknown function 0C5A.
     *
     * Stack: *none*
     *
     * @param script The script engine to operate on.
     * @return unknown.
     */
    internal static ushort Script_Structure_Unknown0C5A(ScriptEngine script)
    {
        Tile32 tile;
        CStructure s;
        CUnit u;
        ushort position;

        s = g_scriptCurrentStructure;

        if (s.o.linkedID == 0xFF) return 0;

        u = Unit_Get_ByIndex(s.o.linkedID);

        if (g_table_unitInfo[u.o.type].movementType == (ushort)MovementType.MOVEMENT_WINGER && Unit_SetPosition(u, s.o.position))
        {
            s.o.linkedID = u.o.linkedID;
            u.o.linkedID = 0xFF;

            if (s.o.linkedID == 0xFF) Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_IDLE);
            Object_Script_Variable4_Clear(s.o);

            if (s.o.houseID == (byte)g_playerHouseID) Sound_Output_Feedback((ushort)(g_playerHouseID + 49));

            return 1;
        }

        position = Structure_FindFreePosition(s, u.o.type == (byte)UnitType.UNIT_HARVESTER);
        if (position == 0) return 0;

        u.o.seenByHouses |= s.o.seenByHouses;

        tile = Tile_Center(Tile_UnpackTile(position));

        if (!Unit_SetPosition(u, tile)) return 0;

        s.o.linkedID = u.o.linkedID;
        u.o.linkedID = 0xFF;

        Unit_SetOrientation(u, (sbyte)(Tile_GetDirection(s.o.position, u.o.position) & 0xE0), true, 0);
        Unit_SetOrientation(u, u.orientation[0].current, true, 1);

        if (u.o.houseID == (byte)g_playerHouseID && u.o.type == (byte)UnitType.UNIT_HARVESTER)
        {
            GUI_DisplayHint((ushort)Text.STR_SEARCH_FOR_SPICE_FIELDS_TO_HARVEST, 0x6A);
        }

        if (s.o.linkedID == 0xFF) Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_IDLE);
        Object_Script_Variable4_Clear(s.o);

        if (s.o.houseID != (byte)g_playerHouseID) return 1;
        if (s.o.type == (byte)StructureType.STRUCTURE_REPAIR) return 1;

        Sound_Output_Feedback((ushort)(g_playerHouseID + ((u.o.type == (byte)UnitType.UNIT_HARVESTER) ? 68 : 30)));

        return 1;
    }

    /*
     * Find a Unit which is within range and not an ally.
     *
     * Stack: 1 - Range to find a target in (amount of tiles multiplied with 256).
     *
     * @param script The script engine to operate on.
     * @return The Unit Index of the closest unit within range and not friendly,
     *   or 0 if none exists.
     */
    internal static ushort Script_Structure_FindTargetUnit(ScriptEngine script)
    {
        var find = new PoolFindStruct();
        CStructure s;
        CUnit u;
        uint distanceCurrent;
        uint targetRange;
        Tile32 position;

        s = g_scriptCurrentStructure;
        targetRange = STACK_PEEK(script, 1);
        distanceCurrent = 32000;
        u = null;

        find.houseID = (byte)HouseType.HOUSE_INVALID;
        find.index = 0xFFFF;
        find.type = 0xFFFF;

        /* ENHANCEMENT -- The original code calculated distances from the top-left corner of the structure. */
        position = g_dune2_enhanced ? Tile_Center(s.o.position) : s.o.position;

        while (true)
        {
            ushort distance;
            CUnit uf;

            uf = Unit_Find(find);
            if (uf == null) break;

            if (House_AreAllied(s.o.houseID, Unit_GetHouseID(uf))) continue;

            if (uf.o.type != (byte)UnitType.UNIT_ORNITHOPTER)
            {
                if ((uf.o.seenByHouses & (1 << s.o.houseID)) == 0) continue;
            }

            distance = Tile_GetDistance(uf.o.position, position);
            if (distance >= distanceCurrent) continue;

            if (g_dune2_enhanced)
            {
                if (uf.o.type == (byte)UnitType.UNIT_ORNITHOPTER)
                {
                    if (distance > targetRange * 3) continue;
                }
                else
                {
                    if (distance > targetRange) continue;
                }
            }
            else
            {
                if (uf.o.type == (byte)UnitType.UNIT_ORNITHOPTER)
                {
                    if (distance >= targetRange * 3) continue;
                }
                else
                {
                    if (distance >= targetRange) continue;
                }
            }

            /* ENHANCEMENT -- The original code swapped the assignment, making it do nothing, Now it finds the closest unit to shoot at, what seems to be the intention */
            if (g_dune2_enhanced) distanceCurrent = distance;
            u = uf;
        }

        if (u == null) return (ushort)IndexType.IT_NONE;
        return Tools_Index_Encode(u.o.index, IndexType.IT_UNIT);
    }

    /*
     * Rotate the turret to look at a tile.
     *
     * Stack: 1 - Tile to look at.
     *
     * @param script The script engine to operate on.
     * @return 0 if looking at target, otherwise 1.
     */
    internal static ushort Script_Structure_RotateTurret(ScriptEngine script)
    {
        CStructure s;
        Tile32 lookAt;
        CTile tile;
        ushort baseTileID;
        ushort encoded;
        short rotation;
        short rotationNeeded;
        short rotateDiff;

        encoded = STACK_PEEK(script, 1);

        if (encoded == 0) return 0;

        s = g_scriptCurrentStructure;
        lookAt = Tools_Index_GetTile(encoded);
        tile = g_map[Tile_PackTile(s.o.position)];

        /* Find the base sprite of the structure */
        baseTileID = s.o.type == (byte)StructureType.STRUCTURE_ROCKET_TURRET
            ? g_iconMap[g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_BASE_ROCKET_TURRET] + 2]
            : g_iconMap[g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_BASE_DEFENSE_TURRET] + 2];

        rotation = (short)(tile.groundTileID - baseTileID);
        if (rotation < 0 || rotation > 7) return 1;

        /* Find what rotation we should have to look at the target */
        rotationNeeded = Orientation_Orientation256ToOrientation8((byte)Tile_GetDirection(s.o.position, lookAt));

        /* Do we need to rotate */
        if (rotationNeeded == rotation) return 0;

        /* Find the fastest way to rotate to the correct rotation */
        rotateDiff = (short)(rotationNeeded - rotation);
        if (rotateDiff < 0) rotateDiff += 8;

        if (rotateDiff < 4)
        {
            rotation++;
        }
        else
        {
            rotation--;
        }
        rotation &= 0x7;

        /* Set the new sprites */
        tile.groundTileID = (ushort)(baseTileID + rotation);
        s.rotationSpriteDiff = (ushort)rotation;

        Map_Update(Tile_PackTile(s.o.position), 0, false);

        return 1;
    }

    /*
     * Find the direction a tile is, seen from the structure. If the tile is
     *  invalid it gives the direction the structure is currently looking at.
     *
     * Stack: 1 - Tile to get the direction to, or the current direction of the
     *   structure in case the tile is invalid.
     *
     * @param script The script engine to operate on.
     * @return The direction (value between 0 and 7, shifted to the left with 5).
     */
    internal static ushort Script_Structure_GetDirection(ScriptEngine script)
    {
        CStructure s;
        Tile32 tile;
        ushort encoded;

        s = g_scriptCurrentStructure;
        encoded = STACK_PEEK(script, 1);

        if (!Tools_Index_IsValid(encoded)) return (ushort)(s.rotationSpriteDiff << 5);

        tile = Tools_Index_GetTile(encoded);

        return (ushort)(Orientation_Orientation256ToOrientation8((byte)Tile_GetDirection(s.o.position, tile)) << 5);
    }

    /*
     * Unknown function 11B9.
     *
     * Stack: 1 - Encoded tile.
     *
     * @param script The script engine to operate on.
     * @return unknown.
     */
    internal static ushort Script_Structure_Unknown11B9(ScriptEngine script)
    {
        ushort encoded;
        CUnit u;

        encoded = STACK_PEEK(script, 1);

        if (!Tools_Index_IsValid(encoded)) return 0;
        if (Tools_Index_GetType(encoded) != IndexType.IT_UNIT) return 0;

        u = Tools_Index_GetUnit(encoded);
        if (u == null) return 0;

        Object_Script_Variable4_Clear(u.o);
        u.targetMove = 0;

        return 0;
    }

    /*
     * Play a voice on the structure.
     *
     * Stack: 1 - The VoiceID to play.
     *
     * @param script The script engine to operate on.
     * @return unknown.
     */
    internal static ushort Script_Structure_VoicePlay(ScriptEngine script)
    {
        CStructure s;

        s = g_scriptCurrentStructure;

        if (s.o.houseID != (byte)g_playerHouseID) return 0;

        Voice_PlayAtTile((short)STACK_PEEK(script, 1), s.o.position);

        return 0;
    }

    /*
     * Fire a bullet or missile from a (rocket) turret.
     *
     * Stack: *none*
     * Variables: 2 - Target to shoot at.
     *
     * @param script The script engine to operate on.
     * @return The time between this and the next time firing.
     */
    internal static ushort Script_Structure_Fire(ScriptEngine script)
    {
        CStructure s;
        CUnit u;
        var position = new Tile32();
        ushort target;
        ushort damage;
        ushort fireDelay;
        ushort type;

        s = g_scriptCurrentStructure;

        target = script.variables[2];
        if (target == 0) return 0;

        if (s.o.type == (byte)StructureType.STRUCTURE_ROCKET_TURRET && Tile_GetDistance(Tools_Index_GetTile(target), s.o.position) >= 0x300)
        {
            type = (ushort)UnitType.UNIT_MISSILE_TURRET;
            damage = 30;
            fireDelay = Tools_AdjustToGameSpeed(g_table_unitInfo[(ushort)UnitType.UNIT_LAUNCHER].fireDelay, 1, 0xFFFF, true);
        }
        else
        {
            type = (ushort)UnitType.UNIT_BULLET;
            damage = 20;
            fireDelay = Tools_AdjustToGameSpeed(g_table_unitInfo[(ushort)UnitType.UNIT_TANK].fireDelay, 1, 0xFFFF, true);
        }

        position.x = (ushort)(s.o.position.x + 0x80);
        position.y = (ushort)(s.o.position.y + 0x80);
        u = Unit_CreateBullet(position, (UnitType)type, s.o.houseID, damage, target);

        if (u == null) return 0;

        u.originEncoded = Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);

        return fireDelay;
    }

    /*
     * Make the structure explode.
     *
     * Stack: *none*
     *
     * @param script The script engine to operate on.
     * @return unknown.
     */
    internal static ushort Script_Structure_Explode(ScriptEngine script)
    {
        CStructure s;
        ushort position;
        ushort layout;
        ushort i;

        s = g_scriptCurrentStructure;
        layout = g_table_structureInfo[s.o.type].layout;
        position = Tile_PackTile(s.o.position);

        for (i = 0; i < g_table_structure_layoutTileCount[layout]; i++)
        {
            Tile32 tile;

            tile = Tile_UnpackTile((ushort)(position + g_table_structure_layoutTiles[layout][i]));

            Map_MakeExplosion((ushort)ExplosionType.EXPLOSION_STRUCTURE, tile, 0, 0);
        }

        return 0;
    }

    /*
     * Destroy a structure and spawn soldiers around the place.
     *
     * Stack: *none*
     *
     * @param script The script engine to operate on.
     * @return Always 0.
     */
    internal static ushort Script_Structure_Destroy(ScriptEngine script)
    {
        CStructure s;
        ushort position;
        ushort layout;
        ushort i;

        s = g_scriptCurrentStructure;
        layout = g_table_structureInfo[s.o.type].layout;
        position = Tile_PackTile(s.o.position);

        Structure_Remove(s);

        for (i = 0; i < g_table_structure_layoutTileCount[layout]; i++)
        {
            Tile32 tile;
            CUnit u;

            tile = Tile_UnpackTile((ushort)(position + g_table_structure_layoutTiles[layout][i]));

            if (g_table_structureInfo[s.o.type].o.spawnChance < Tools_Random_256()) continue;

            u = Unit_Create((ushort)UnitIndex.UNIT_INDEX_INVALID, (byte)UnitType.UNIT_SOLDIER, s.o.houseID, tile, (sbyte)Tools_Random_256());
            if (u == null) continue;

            u.o.hitpoints = (ushort)(g_table_unitInfo[(int)UnitType.UNIT_SOLDIER].o.hitpoints * (Tools_Random_256() & 3) / 256);

            if (s.o.houseID != (byte)g_playerHouseID)
            {
                Unit_SetAction(u, ActionType.ACTION_ATTACK);
                continue;
            }

            Unit_SetAction(u, ActionType.ACTION_MOVE);

            tile = Tile_MoveByRandom(u.o.position, 32, true);

            u.targetMove = Tools_Index_Encode(Tile_PackTile(tile), IndexType.IT_TILE);
        }

        if (g_debugScenario) return 0;
        if (s.o.houseID != (byte)g_playerHouseID) return 0;

        if (g_config.language == (byte)Language.FRENCH)
        {
            GUI_DisplayText("{0} {1} {2}", 0, String_Get_ByIndex(g_table_structureInfo[s.o.type].o.stringID_full), g_table_houseInfo[s.o.houseID].name, String_Get_ByIndex(Text.STR_IS_DESTROYED));
        }
        else
        {
            GUI_DisplayText("{0} {1} {2}", 0, g_table_houseInfo[s.o.houseID].name, String_Get_ByIndex(g_table_structureInfo[s.o.type].o.stringID_full), String_Get_ByIndex(Text.STR_IS_DESTROYED));
        }

        return 0;
    }
}
