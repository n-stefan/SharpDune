namespace SharpDune.SaveLoad
{
    class SaveLoadHouse
    {
		static readonly SaveLoadDesc[] s_saveHouse = {
			SLD_ENTRY2(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.index), SaveLoadType.SLDT_UINT8),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.harvestersIncoming)),
			SLD_ENTRY2(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.flags), SaveLoadType.SLDT_HOUSEFLAGS),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.unitCount)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.unitCountMax)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.unitCountEnemy)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.unitCountAllied)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT32, nameof(House.structuresBuilt)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.credits)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.creditsStorage)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.powerProduction)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.powerUsage)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.windtrapCount)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.creditsQuota)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, $"{nameof(House.palacePosition)}.{nameof(tile32.x)}"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, $"{nameof(House.palacePosition)}.{nameof(tile32.y)}"),
			SLD_EMPTY(SaveLoadType.SLDT_UINT16),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.timerUnitAttack)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.timerSandwormAttack)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.timerStructureAttack)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.starportTimeLeft)),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.starportLinkedID)),
			SLD_ARRAY(/*house,*/ SaveLoadType.SLDT_UINT16, nameof(House.ai_structureRebuild), 10),
			SLD_END()
		};

		/*
		 * Load all Houses from a file.
		 * @param fp The file to load from.
		 * @param length The length of the data chunk.
		 * @return True if and only if all bytes were read successful.
		 */
		internal static bool House_Load(BinaryReader fp, uint length)
		{
			while (length > 0)
			{
				House hl;

				/* Read the next index from disk */
				var index = fp.ReadUInt16();

				/* Create the House in the pool */
				hl = PoolHouse.House_Allocate((byte)index);
				if (hl == null) return false;

				fp.BaseStream.Seek(-2, SeekOrigin.Current);

				/* Read the next House from disk */
				if (!SaveLoad_Load(s_saveHouse, fp, hl)) return false;

				length -= SaveLoad_GetLength(s_saveHouse);

				/* See if it is a human house */
				if (hl.flags.human)
				{
                    g_playerHouseID = (HouseType)hl.index;
                    g_playerHouse = hl;

					if (hl.starportLinkedID != 0xFFFF && hl.starportTimeLeft == 0) hl.starportTimeLeft = 1;
				}
			}
			if (length != 0) return false;

			return true;
		}

		/*
		 * Load all Houses from a file.
		 * @param fp The file to load from.
		 * @param length The length of the data chunk.
		 * @return True if and only if all bytes were read successful.
		 */
		internal static bool House_LoadOld(BinaryReader fp, uint length)
		{
			while (length > 0)
			{
				House hl = null;

				/* Read the next House from disk */
				if (!SaveLoad_Load(s_saveHouse, fp, hl)) return false;

				/* See if it is a human house */
				if (hl.flags.human)
				{
                    g_playerHouseID = (HouseType)hl.index;
					break;
				}

				length -= SaveLoad_GetLength(s_saveHouse);
			}
			if (length == 0) return false;

			return true;
		}

		/*
		 * Save all Houses to a file.
		 * @param fp The file to save to.
		 * @return True if and only if all bytes were written successful.
		 */
		internal static bool House_Save(BinaryWriter fp)
		{
			var find = new PoolFindStruct
			{
				houseID = (byte)HouseType.HOUSE_INVALID,
				type = 0xFFFF,
				index = 0xFFFF
			};

			while (true)
			{
				House h;

				h = PoolHouse.House_Find(find);
				if (h == null) break;

				if (!SaveLoad_Save(s_saveHouse, fp, h)) return false;
			}

			return true;
		}
	}
}
