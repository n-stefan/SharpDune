namespace SharpDune.Pool
{
    class PoolTeam
    {
		enum TeamIndex
		{
			TEAM_INDEX_MAX = 16,                                 /*!< The highest possible index for any Team.  */

			TEAM_INDEX_INVALID = 0xFFFF
		}

		static readonly Team[] g_teamArray = new Team[(int)TeamIndex.TEAM_INDEX_MAX];
		static readonly Team[] g_teamFindArray = new Team[(int)TeamIndex.TEAM_INDEX_MAX];
		static ushort g_teamFindCount;

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
				var t = g_teamFindArray[find.index];
				if (t == null) continue;

				if (find.houseID == (byte)HouseType.HOUSE_INVALID || find.houseID == t.houseID) return t;
			}

			return null;
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
				var t = Team_Get_ByIndex(index);
				if (t.flags.used) g_teamFindArray[g_teamFindCount++] = t;
			}
		}

		/*
		 * Allocate a Team.
		 *
		 * @param index The index to use, or TEAM_INDEX_INVALID to find an unused index.
		 * @return The Team allocated, or NULL on failure.
		 */
		internal static Team Team_Allocate(ushort index)
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
	}
}
