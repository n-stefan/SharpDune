/* Team */

using SharpDune.Include;
using SharpDune.Pool;
using SharpDune.Script;
using System;
using static SharpDune.Script.Script;
using static SharpDune.Table.TableTeamAction;

namespace SharpDune
{
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
		static uint s_tickTeamGameLoop; /*!< Indicates next time the GameLoop function is executed. */

		/*
		 * Loop over all teams, performing various of tasks.
		 */
		internal static void GameLoop_Team()
		{
			var find = new PoolFindStruct();

			if (s_tickTeamGameLoop > Timer.g_timerGame) return;
			s_tickTeamGameLoop = (uint)(Timer.g_timerGame + (Tools.Tools_Random_256() & 7) + 5);

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

            g_scriptCurrentObject = null;
            g_scriptCurrentUnit = null;
            g_scriptCurrentStructure = null;

			while (true)
			{
				Team t;
				House h;

				t = PoolTeam.Team_Find(find);
				if (t == null) break;

				h = PoolHouse.House_Get_ByIndex(t.houseID);

                g_scriptCurrentTeam = t;

				if (!h.flags.isAIActive) continue;

				if (t.script.delay != 0)
				{
					t.script.delay--;
					continue;
				}

				if (!Script_IsLoaded(t.script)) continue;

				if (!Script_Run(t.script))
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
				if (string.Equals(g_table_teamActionName[type], name, StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(g_table_teamActionName[type], name) == 0)
					return type;
			}

			return (byte)TeamActionType.TEAM_ACTION_INVALID;
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

			t = PoolTeam.Team_Allocate(0xFFFF);

			if (t == null) return null;
			t.flags.used = true;
			t.houseID = houseID;
			t.action = teamActionType;
			t.actionStart = teamActionType;
			t.movementType = movementType;
			t.minMembers = minMembers;
			t.maxMembers = maxMembers;

            Script_Reset(t.script, g_scriptTeam);
            Script_Load(t.script, teamActionType);

			t.script.delay = 0;

			return t;
		}
	}
}
