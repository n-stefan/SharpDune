namespace SharpDune.SaveLoad
{
    class SaveLoadStructure
    {
		static readonly SaveLoadDesc[] s_saveStructure = {
			SLD_SLD(/*structure,*/ nameof(CStructure.o), g_saveObject),
			SLD_ENTRY(/*structure,*/ SLDT_UINT16, nameof(CStructure.creatorHouseID)),
			SLD_ENTRY(/*structure,*/ SLDT_UINT16, nameof(CStructure.rotationSpriteDiff)),
			SLD_EMPTY(SLDT_UINT8),
			SLD_ENTRY(/*structure,*/ SLDT_UINT16, nameof(CStructure.objectType)),
			SLD_ENTRY(/*structure,*/ SLDT_UINT8, nameof(CStructure.upgradeLevel)),
			SLD_ENTRY(/*structure,*/ SLDT_UINT8, nameof(CStructure.upgradeTimeLeft)),
			SLD_ENTRY(/*structure,*/ SLDT_UINT16, nameof(CStructure.countDown)),
			SLD_ENTRY(/*structure,*/ SLDT_UINT16, nameof(CStructure.buildCostRemainder)),
			SLD_ENTRY(/*structure,*/ SLDT_INT16, nameof(CStructure.state)),
			SLD_ENTRY(/*structure,*/ SLDT_UINT16, nameof(CStructure.hitpointsMax)),
			SLD_END()
		};

		/*
		 * Load all Structures from a file.
		 * @param fp The file to load from.
		 * @param length The length of the data chunk.
		 * @return True if and only if all bytes were read successful.
		 */
		internal static bool Structure_Load(BinaryReader fp, uint length)
		{
			while (length > 0)
			{
				CStructure sl;

				/* Read the next index from disk */
				var index = fp.ReadUInt16();

				/* Get the Structure from the pool */
				sl = Structure_Get_ByIndex(index);
				if (sl == null) return false;

				fp.BaseStream.Seek(-2, SeekOrigin.Current);

				/* Read the next Structure from disk */
				if (!SaveLoad_Load(s_saveStructure, fp, sl)) return false;

				length -= SaveLoad_GetLength(s_saveStructure);

				sl.o.script.scriptInfo = g_scriptStructure;
				if (sl.upgradeTimeLeft == 0) sl.upgradeTimeLeft = (byte)(Structure_IsUpgradable(sl) ? 100 : 0);
			}
			if (length != 0) return false;

            Structure_Recount();

			return true;
		}

		/*
		 * Save all Structures to a file. It converts pointers to indices where needed.
		 * @param fp The file to save to.
		 * @return True if and only if all bytes were written successful.
		 */
		internal static bool Structure_Save(BinaryWriter fp)
		{
			var find = new PoolFindStruct
			{
				houseID = (byte)HouseType.HOUSE_INVALID,
				type = 0xFFFF,
				index = 0xFFFF
			};

			while (true)
			{
				CStructure s;

				s = Structure_Find(find);
				if (s == null) break;

				if (!SaveLoad_Save(s_saveStructure, fp, s)) return false;
			}

			return true;
		}
	}
}
