/* Team */

using System;
using System.Diagnostics;
using System.Linq;

namespace SharpDune
{
	enum TeamIndex
	{
		TEAM_INDEX_MAX = 16,                                 /*!< The highest possible index for any Team.  */

		TEAM_INDEX_INVALID = 0xFFFF
	}

	/*
	 * Types of TeamActions available in the game.
	 */
	enum TeamActionType
	{
		TEAM_ACTION_NORMAL = 0,
		TEAM_ACTION_STAGING = 1,
		TEAM_ACTION_FLEE = 2,
		TEAM_ACTION_KAMIKAZE = 3,
		TEAM_ACTION_GUARD = 4,

		TEAM_ACTION_MAX = 5,
		TEAM_ACTION_INVALID = 0xFF
	}

	/*
	 * flags for Team structure
	 */
	class TeamFlags
	{
		internal bool used;                                   /*!< The Team is in use (no longer free in the pool). */
		internal bool notused_0002;                           /*!< Never used - remaining bits. */
	}

	/*
	 * An Team as stored in the memory.
	 */
	class Team
	{
		internal ushort index;                                  /*!< The index of the Team in the array. */
		internal TeamFlags flags;                               /*!< General flags of the Team. */
		internal ushort members;                                /*!< Amount of members in Team. */
		internal ushort minMembers;                             /*!< Minimum amount of members in Team. */
		internal ushort maxMembers;                             /*!< Maximum amount of members in Team. */
		internal ushort movementType;                           /*!< MovementType of Team. */
		internal ushort action;                                 /*!< Current TeamActionType of Team. */
		internal ushort actionStart;                            /*!< The TeamActionType Team starts with. */
		internal byte houseID;                                  /*!< House of Team. */
		internal tile32 position;                               /*!< Position on the map. */
		internal ushort targetTile;                             /*!< Current target tile around the target. Only used as a bool, so either set or not. */
		internal ushort target;                                 /*!< Current target of team (encoded index). */
		internal ScriptEngine script;                           /*!< The script engine instance of this Team. */

		internal Team()
		{
			flags = new TeamFlags();
			script = new ScriptEngine();
		}
	}

	class CTeam
	{
		static Team[] g_teamArray = new Team[(int)TeamIndex.TEAM_INDEX_MAX];
		static Team[] g_teamFindArray = new Team[(int)TeamIndex.TEAM_INDEX_MAX];
		static ushort g_teamFindCount;

		static uint s_tickTeamGameLoop = 0; /*!< Indicates next time the GameLoop function is executed. */

		/*
		 * Get a Team from the pool with the indicated index.
		 *
		 * @param index The index of the Team to get.
		 * @return The Team.
		 */
		internal static Team Team_Get_ByIndex(ushort index)
		{
			Debug.Assert(index < (ushort)TeamIndex.TEAM_INDEX_MAX);
			return g_teamArray[index];
		}

		//internal static void Team_Set_ByIndex(Team t)
		//{
		//	Debug.Assert(t.index < (ushort)TeamIndex.TEAM_INDEX_MAX);
		//	g_teamArray[t.index] = t;
		//}

		/*
		 * Find the first matching Team based on the PoolFindStruct filter data.
		 *
		 * @param find A pointer to a PoolFindStruct which contains filter data and
		 *   last known tried index. Calling this functions multiple times with the
		 *   same 'find' parameter walks over all possible values matching the filter.
		 * @return The Team, or NULL if nothing matches (anymore).
		 */
		internal static Team Team_Find(PoolFindStruct find)
		{
			if (find.index >= g_teamFindCount && find.index != 0xFFFF) return null;
			find.index++; /* First, we always go to the next index */

			for (; find.index < g_teamFindCount; find.index++)
			{
				Team t = g_teamFindArray[find.index];
				if (t == null) continue;

				if (find.houseID == (byte)HouseType.HOUSE_INVALID || find.houseID == t.houseID) return t;
			}

			return null;
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

			t = Script.g_scriptCurrentTeam;
			if (t.houseID == (byte)CHouse.g_playerHouseID) return 0;

			offset = Endian.BETOH16(script.scriptInfo.text[Script.STACK_PEEK(script, 1)]);
			text = script.scriptInfo.text[offset..].Cast<char>().ToString();

			Gui.GUI_DisplayText(text, 0, Script.STACK_PEEK(script, 2), Script.STACK_PEEK(script, 3), Script.STACK_PEEK(script, 4));

			return 0;
		}

		/*
		 * Gets the amount of members in the current team.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return Amount of members in current team.
		 */
		internal static ushort Script_Team_GetMembers(ScriptEngine script)
		{
			//VARIABLE_NOT_USED(script);
			return Script.g_scriptCurrentTeam.members;
		}

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
			PoolFindStruct find = new PoolFindStruct();

			//VARIABLE_NOT_USED(script);

			t = Script.g_scriptCurrentTeam;

			if (t.members >= t.maxMembers) return 0;

			find.houseID = t.houseID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			while (true)
			{
				Unit u;
				Team t2;
				ushort distance;

				u = CUnit.Unit_Find(find);
				if (u == null) break;
				if (!u.o.flags.byScenario) continue;
				if (u.o.type == (byte)UnitType.UNIT_SABOTEUR) continue;
				if (CUnit.g_table_unitInfo[u.o.type].movementType != t.movementType) continue;
				if (u.team == 0)
				{
					distance = CTile.Tile_GetDistance(t.position, u.o.position);
					if (distance >= minDistance && minDistance != 0) continue;
					minDistance = distance;
					closest = u;
					continue;
				}

				t2 = Team_Get_ByIndex((ushort)(u.team - 1));
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
			PoolFindStruct find = new PoolFindStruct();

			//VARIABLE_NOT_USED(script);

			t = Script.g_scriptCurrentTeam;

			find.houseID = t.houseID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			while (true)
			{
				Unit u;

				u = CUnit.Unit_Find(find);
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

				u = CUnit.Unit_Find(find);
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
			PoolFindStruct find = new PoolFindStruct();

			t = Script.g_scriptCurrentTeam;
			distance = Script.STACK_PEEK(script, 1);

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

				u = CUnit.Unit_Find(find);
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
			PoolFindStruct find = new PoolFindStruct();

			//VARIABLE_NOT_USED(script);

			t = Script.g_scriptCurrentTeam;

			find.houseID = t.houseID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			while (true)
			{
				Unit u;
				ushort target;

				u = CUnit.Unit_Find(find);
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

			t = Script.g_scriptCurrentTeam;
			type = Script.STACK_PEEK(script, 1);

			if (t.action == type) return 0;

			t.action = type;

			Script.Script_Reset(t.script, Script.g_scriptTeam);
			Script.Script_Load(t.script, (byte)(type & 0xFF));

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

			//VARIABLE_NOT_USED(script);

			t = Script.g_scriptCurrentTeam;
			type = t.actionStart;

			if (t.action == type) return 0;

			t.action = type;

			Script.Script_Reset(t.script, Script.g_scriptTeam);
			Script.Script_Load(t.script, (byte)(type & 0xFF));

			return 0;
		}

		/*
		 * Gets the variable_06 of the current team.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The variable_06 of the current team.
		 */
		internal static ushort Script_Team_GetVariable6(ScriptEngine script)
		{
			//VARIABLE_NOT_USED(script);
			return Script.g_scriptCurrentTeam.minMembers;
		}

		/*
		 * Gets the target for the current team.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The encoded target.
		 */
		internal static ushort Script_Team_GetTarget(ScriptEngine script)
		{
			//VARIABLE_NOT_USED(script);
			return Script.g_scriptCurrentTeam.target;
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
			PoolFindStruct find = new PoolFindStruct();

			//VARIABLE_NOT_USED(script);

			t = Script.g_scriptCurrentTeam;
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

				u = CUnit.Unit_Find(find);
				if (u == null) break;
				if (u.team - 1 != t.index) continue;
				if (t.target == 0)
				{
					CUnit.Unit_SetAction(u, ActionType.ACTION_GUARD);
					continue;
				}

				distance = (ushort)(CUnit.g_table_unitInfo[u.o.type].fireDistance << 8);
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
		 * Initialize the Team array.
		 *
		 * @param address If non-zero, the new location of the Team array.
		 */
		internal static void Team_Init()
		{
			for (var i = 0; i < g_teamArray.Length; i++) g_teamArray[i] = new Team(); //memset(g_teamArray, 0, sizeof(g_teamArray));
			Array.Fill(g_teamFindArray, null, 0, g_teamFindArray.Length); //memset(g_teamFindArray, 0, sizeof(g_teamFindArray));
			g_teamFindCount = 0;
		}

		/*
		 * Recount all Teams, ignoring the cache array.
		 */
		internal static void Team_Recount()
		{
			ushort index;

			g_teamFindCount = 0;

			for (index = 0; index < (ushort)TeamIndex.TEAM_INDEX_MAX; index++)
			{
				Team t = Team_Get_ByIndex(index);
				if (t.flags.used) g_teamFindArray[g_teamFindCount++] = t;
			}
		}

		/*
		 * Loop over all teams, performing various of tasks.
		 */
		internal static void GameLoop_Team()
		{
			PoolFindStruct find = new PoolFindStruct();

			if (s_tickTeamGameLoop > Timer.g_timerGame) return;
			s_tickTeamGameLoop = (uint)(Timer.g_timerGame + (Tools.Tools_Random_256() & 7) + 5);

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			Script.g_scriptCurrentObject = null;
			Script.g_scriptCurrentUnit = null;
			Script.g_scriptCurrentStructure = null;

			while (true)
			{
				Team t;
				House h;

				t = Team_Find(find);
				if (t == null) break;

				h = CHouse.House_Get_ByIndex(t.houseID);

				Script.g_scriptCurrentTeam = t;

				if (!h.flags.isAIActive) continue;

				if (t.script.delay != 0)
				{
					t.script.delay--;
					continue;
				}

				if (!Script.Script_IsLoaded(t.script)) continue;

				if (!Script.Script_Run(t.script))
				{
					/* ENHANCEMENT -- Dune2 aborts all other teams if one gives a script error. This doesn't seem correct */
					if (CSharpDune.g_dune2_enhanced) continue;
					break;
				}
			}
		}

		/*
		 * Convert the name of a team action to the type value of that team action, or
		 *  TEAM_ACTION_INVALID if not found.
		 */
		internal static byte Team_ActionStringToType(string name)
		{
			byte type;
			if (name == null) return (byte)TeamActionType.TEAM_ACTION_INVALID;

			for (type = 0; type < (byte)TeamActionType.TEAM_ACTION_MAX; type++)
			{
				if (string.Equals(TeamAction.g_table_teamActionName[type], name, StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(g_table_teamActionName[type], name) == 0)
					return type;
			}

			return (byte)TeamActionType.TEAM_ACTION_INVALID;
		}

		/*
		 * Allocate a Team.
		 *
		 * @param index The index to use, or TEAM_INDEX_INVALID to find an unused index.
		 * @return The Team allocated, or NULL on failure.
		 */
		static Team Team_Allocate(ushort index)
		{
			Team t = null;

			if (index == (ushort)TeamIndex.TEAM_INDEX_INVALID)
			{
				/* Find the first unused index */
				for (index = 0; index < (ushort)TeamIndex.TEAM_INDEX_MAX; index++)
				{
					t = Team_Get_ByIndex(index);
					if (!t.flags.used) break;
				}
				if (index == (ushort)TeamIndex.TEAM_INDEX_MAX) return null;
			}
			else
			{
				t = Team_Get_ByIndex(index);
				if (t.flags.used) return null;
			}
			Debug.Assert(t != null);

			/* Initialize the Team */
			//memset(t, 0, sizeof(Team));
			t.index = index;
			t.flags.used = true;

			g_teamFindArray[g_teamFindCount++] = t;

			return t;
		}

		/*
		 * Create a new Team.
		 *
		 * @param houseID The House of the new Team.
		 * @param teamActionType The teamActionType of the new Team.
		 * @param movementType The movementType of the new Team.
		 * @param minMembers The minimum amount of members in the new Team.
		 * @param maxMembers The maximum amount of members in the new Team.
		 * @return The new created Team, or NULL if something failed.
		 */
		internal static Team Team_Create(byte houseID, byte teamActionType, byte movementType, ushort minMembers, ushort maxMembers)
		{
			Team t;

			t = Team_Allocate(0xFFFF);

			if (t == null) return null;
			t.flags.used = true;
			t.houseID = houseID;
			t.action = teamActionType;
			t.actionStart = teamActionType;
			t.movementType = movementType;
			t.minMembers = minMembers;
			t.maxMembers = maxMembers;

			Script.Script_Reset(t.script, Script.g_scriptTeam);
			Script.Script_Load(t.script, teamActionType);

			t.script.delay = 0;

			return t;
		}
	}
}
