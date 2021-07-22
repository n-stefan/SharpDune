using SharpDune.Pool;
using System.IO;
using static SharpDune.SaveLoad.SaveLoad;

namespace SharpDune.SaveLoad
{
    class SaveLoadTeam
    {
		static readonly SaveLoadDesc[] s_saveTeam = {
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, nameof(Team.index)),
			SLD_ENTRY2(/*team,*/ SaveLoadType.SLDT_UINT16, nameof(Team.flags), SaveLoadType.SLDT_TEAMFLAGS),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, nameof(Team.members)),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, nameof(Team.minMembers)),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, nameof(Team.maxMembers)),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, nameof(Team.movementType)),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, nameof(Team.action)),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, nameof(Team.actionStart)),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT8, nameof(Team.houseID)),
			SLD_EMPTY2(SaveLoadType.SLDT_UINT8, 3),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, $"{nameof(Team.position)}.{nameof(tile32.x)}"),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, $"{nameof(Team.position)}.{nameof(tile32.y)}"),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, nameof(Team.targetTile)),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, nameof(Team.target)),
			SLD_SLD(/*team,*/ nameof(Team.script), SaveLoadScriptEngine.g_saveScriptEngine),
			SLD_END()
		};

		/*
		 * Load all Teams from a file.
		 * @param fp The file to load from.
		 * @param length The length of the data chunk.
		 * @return True if and only if all bytes were read successful.
		 */
		internal static bool Team_Load(BinaryReader fp, uint length)
		{
			while (length > 0)
			{
				Team tl;

				/* Read the next index from disk */
				var index = fp.ReadUInt16();

				/* Get the Team from the pool */
				tl = PoolTeam.Team_Get_ByIndex(index);
				if (tl == null) return false;

				fp.BaseStream.Seek(-2, SeekOrigin.Current);

				/* Read the next Team from disk */
				if (!SaveLoad_Load(s_saveTeam, fp, tl)) return false;

				length -= SaveLoad_GetLength(s_saveTeam);

				tl.script.scriptInfo = Script.g_scriptTeam;
			}
			if (length != 0) return false;

			PoolTeam.Team_Recount();

			return true;
		}

		/*
		 * Save all Teams to a file. It converts pointers to indices where needed.
		 * @param fp The file to save to.
		 * @return True if and only if all bytes were written successful.
		 */
		internal static bool Team_Save(BinaryWriter fp)
		{
			var find = new PoolFindStruct
			{
				houseID = (byte)HouseType.HOUSE_INVALID,
				type = 0xFFFF,
				index = 0xFFFF
			};

			while (true)
			{
				Team t;

				t = PoolTeam.Team_Find(find);
				if (t == null) break;

				if (!SaveLoad_Save(s_saveTeam, fp, t)) return false;
			}

			return true;
		}
	}
}
