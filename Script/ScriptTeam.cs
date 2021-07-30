/* Team script routines */

namespace SharpDune.Script
{
    class ScriptTeam
    {
		/*
		 * Gets the amount of members in the current team.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return Amount of members in current team.
		 */
		internal static ushort Script_Team_GetMembers(ScriptEngine script) =>
            g_scriptCurrentTeam.members;

		/*
		 * Gets the variable_06 of the current team.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The variable_06 of the current team.
		 */
		internal static ushort Script_Team_GetVariable6(ScriptEngine script) =>
            g_scriptCurrentTeam.minMembers;

		/*
		 * Gets the target for the current team.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The encoded target.
		 */
		internal static ushort Script_Team_GetTarget(ScriptEngine script) =>
            g_scriptCurrentTeam.target;

		/*
		 * Tries to add the closest unit to the current team.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The amount of space left in current team.
		 */
		internal static ushort Script_Team_AddClosestUnit(ScriptEngine script)
		{
			Team t;
			Unit closest = null;
			Unit closest2 = null;
			ushort minDistance = 0;
			ushort minDistance2 = 0;
			var find = new PoolFindStruct();

			t = g_scriptCurrentTeam;

			if (t.members >= t.maxMembers) return 0;

			find.houseID = t.houseID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			while (true)
			{
				Unit u;
				Team t2;
				ushort distance;

				u = PoolUnit.Unit_Find(find);
				if (u == null) break;
				if (!u.o.flags.byScenario) continue;
				if (u.o.type == (byte)UnitType.UNIT_SABOTEUR) continue;
				if (g_table_unitInfo[u.o.type].movementType != t.movementType) continue;
				if (u.team == 0)
				{
					distance = CTile.Tile_GetDistance(t.position, u.o.position);
					if (distance >= minDistance && minDistance != 0) continue;
					minDistance = distance;
					closest = u;
					continue;
				}

				t2 = PoolTeam.Team_Get_ByIndex((ushort)(u.team - 1));
				if (t2.members > t2.minMembers) continue;

				distance = CTile.Tile_GetDistance(t.position, u.o.position);
				if (distance >= minDistance2 && minDistance2 != 0) continue;
				minDistance2 = distance;
				closest2 = u;
			}

			if (closest == null) closest = closest2;
			if (closest == null) return 0;

			CUnit.Unit_RemoveFromTeam(closest);
			return CUnit.Unit_AddToTeam(closest, t);
		}

		/*
		 * Gets the average distance between current team members, and set the
		 *  position of the team to the average position.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The average distance.
		 */
		internal static ushort Script_Team_GetAverageDistance(ScriptEngine script)
		{
			ushort averageX = 0;
			ushort averageY = 0;
			ushort count = 0;
			ushort distance = 0;
			Team t;
			var find = new PoolFindStruct();

			t = g_scriptCurrentTeam;

			find.houseID = t.houseID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			while (true)
			{
				Unit u;

				u = PoolUnit.Unit_Find(find);
				if (u == null) break;
				if (t.index != u.team - 1) continue;
				count++;
				averageX += (ushort)((u.o.position.x >> 8) & 0x3f);
				averageY += (ushort)((u.o.position.y >> 8) & 0x3f);
			}

			if (count == 0) return 0;
			averageX /= count;
			averageY /= count;

			CTile.Tile_MakeXY(ref t.position, averageX, averageY);

			find.houseID = t.houseID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			while (true)
			{
				Unit u;

				u = PoolUnit.Unit_Find(find);
				if (u == null) break;
				if (t.index != u.team - 1) continue;
				distance += CTile.Tile_GetDistanceRoundedUp(u.o.position, t.position);
			}

			distance /= count;

			if (t.target == 0 || t.targetTile == 0) return distance;

			if (CTile.Tile_GetDistancePacked(CTile.Tile_PackXY(averageX, averageY), Tools.Tools_Index_GetPackedTile(t.target)) <= 10) t.targetTile = 2;

			return distance;
		}

		/*
		 * Unknown function 0543.
		 *
		 * Stack: 1 - A distance.
		 *
		 * @param script The script engine to operate on.
		 * @return The number of moving units.
		 */
		internal static ushort Script_Team_Unknown0543(ScriptEngine script)
		{
			Team t;
			ushort count = 0;
			ushort distance;
			var find = new PoolFindStruct();

			t = g_scriptCurrentTeam;
			distance = STACK_PEEK(script, 1);

			find.houseID = t.houseID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			while (true)
			{
				Unit u;
				tile32 tile;
				ushort distanceUnitDest;
				ushort distanceUnitTeam;
				ushort distanceTeamDest;

				u = PoolUnit.Unit_Find(find);
				if (u == null) break;
				if (t.index != u.team - 1) continue;

				tile = Tools.Tools_Index_GetTile(u.targetMove);
				distanceUnitTeam = CTile.Tile_GetDistanceRoundedUp(u.o.position, t.position);

				if (u.targetMove != 0)
				{
					distanceUnitDest = CTile.Tile_GetDistanceRoundedUp(u.o.position, tile);
					distanceTeamDest = CTile.Tile_GetDistanceRoundedUp(t.position, tile);
				}
				else
				{
					distanceUnitDest = 64;
					distanceTeamDest = 64;
				}

				if ((distanceUnitDest < distanceTeamDest && (distance + 2) < distanceUnitTeam) || (distanceUnitDest >= distanceTeamDest && distanceUnitTeam > distance))
				{
					CUnit.Unit_SetAction(u, ActionType.ACTION_MOVE);

					tile = CTile.Tile_MoveByRandom(t.position, (ushort)(distance << 4), true);

					CUnit.Unit_SetDestination(u, Tools.Tools_Index_Encode(CTile.Tile_PackTile(tile), IndexType.IT_TILE));
					count++;
					continue;
				}

				CUnit.Unit_SetAction(u, ActionType.ACTION_GUARD);
			}

			return count;
		}

		/*
		 * Gets the best target for the current team.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The encoded index of the best target or 0 if none found.
		 */
		internal static ushort Script_Team_FindBestTarget(ScriptEngine script)
		{
			Team t;
			var find = new PoolFindStruct();

			t = g_scriptCurrentTeam;

			find.houseID = t.houseID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			while (true)
			{
				Unit u;
				ushort target;

				u = PoolUnit.Unit_Find(find);
				if (u == null) break;
				if (u.team - 1 != t.index) continue;
				target = CUnit.Unit_FindBestTargetEncoded(u, (ushort)(t.action == (ushort)TeamActionType.TEAM_ACTION_KAMIKAZE ? 4 : 0));
				if (target == 0) continue;
				if (t.target == target) return target;

				t.target = target;
				t.targetTile = CTile.Tile_GetTileInDirectionOf(CTile.Tile_PackTile(u.o.position), Tools.Tools_Index_GetPackedTile(target));
				return target;
			}

			return 0;
		}

		/*
		 * Loads a new script for the current team.
		 *
		 * Stack: 1 - The script type.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Team_Load(ScriptEngine script)
		{
			Team t;
			ushort type;

			t = g_scriptCurrentTeam;
			type = STACK_PEEK(script, 1);

			if (t.action == type) return 0;

			t.action = type;

			Script_Reset(t.script, g_scriptTeam);
			Script_Load(t.script, (byte)(type & 0xFF));

			return 0;
		}

		/*
		 * Loads a new script for the current team.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Team_Load2(ScriptEngine script)
		{
			Team t;
			ushort type;

			t = g_scriptCurrentTeam;
			type = t.actionStart;

			if (t.action == type) return 0;

			t.action = type;

			Script_Reset(t.script, g_scriptTeam);
			Script_Load(t.script, (byte)(type & 0xFF));

			return 0;
		}

		/*
		 * Unknown function 0788.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Team_Unknown0788(ScriptEngine script)
		{
			Team t;
			tile32 tile;
			var find = new PoolFindStruct();

			t = g_scriptCurrentTeam;
			if (t.target == 0) return 0;

			tile = Tools.Tools_Index_GetTile(t.target);

			find.houseID = t.houseID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			while (true)
			{
				Unit u;
				ushort distance;
				ushort packed;
				short orientation;

				u = PoolUnit.Unit_Find(find);
				if (u == null) break;
				if (u.team - 1 != t.index) continue;
				if (t.target == 0)
				{
					CUnit.Unit_SetAction(u, ActionType.ACTION_GUARD);
					continue;
				}

				distance = (ushort)(g_table_unitInfo[u.o.type].fireDistance << 8);
				if (u.actionID == (byte)ActionType.ACTION_ATTACK && u.targetAttack == t.target)
				{
					if (u.targetMove != 0) continue;
					if (CTile.Tile_GetDistance(u.o.position, tile) >= distance) continue;
				}

				if (u.actionID != (byte)ActionType.ACTION_ATTACK) CUnit.Unit_SetAction(u, ActionType.ACTION_ATTACK);

				orientation = (short)((CTile.Tile_GetDirection(tile, u.o.position) & 0xC0) + Tools.Tools_RandomLCG_Range(0, 127));
				if (orientation < 0) orientation += 256;

				packed = CTile.Tile_PackTile(CTile.Tile_MoveByDirection(tile, orientation, distance));

				if (CObject.Object_GetByPackedTile(packed) == null)
				{
					CUnit.Unit_SetDestination(u, Tools.Tools_Index_Encode(packed, IndexType.IT_TILE));
				}
				else
				{
					CUnit.Unit_SetDestination(u, Tools.Tools_Index_Encode(CTile.Tile_PackTile(tile), IndexType.IT_TILE));
				}

				CUnit.Unit_SetTarget(u, t.target);
			}

			return 0;
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
		internal static ushort Script_Team_DisplayText(ScriptEngine script)
		{
			Team t;
			string text; //char *
			ushort offset;

			t = g_scriptCurrentTeam;
			if (t.houseID == (byte)CHouse.g_playerHouseID) return 0;

			offset = Endian.BETOH16(script.scriptInfo.text[STACK_PEEK(script, 1)]);
			text = script.scriptInfo.text[offset..].Cast<char>().ToString();

			Gui.Gui.GUI_DisplayText(text, 0, STACK_PEEK(script, 2), STACK_PEEK(script, 3), STACK_PEEK(script, 4));

			return 0;
		}
	}
}
