namespace SharpDune.SaveLoad
{
    class SaveLoadUnit
    {
		static readonly SaveLoadDesc[] s_saveUnitOrientation = {
			SLD_ENTRY(/*dir24,*/ SaveLoadType.SLDT_INT8, nameof(dir24.speed)),
			SLD_ENTRY(/*dir24,*/ SaveLoadType.SLDT_INT8, nameof(dir24.target)),
			SLD_ENTRY(/*dir24,*/ SaveLoadType.SLDT_INT8, nameof(dir24.current)),
			SLD_END()
		};

		static readonly SaveLoadDesc[] s_saveUnit = {
			SLD_SLD(/*unit,*/ nameof(Unit.o), SaveLoadObject.g_saveObject),
			SLD_EMPTY(SaveLoadType.SLDT_UINT16),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, $"{nameof(Unit.currentDestination)}.{nameof(tile32.x)}"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, $"{nameof(Unit.currentDestination)}.{nameof(tile32.y)}"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, nameof(Unit.originEncoded)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, nameof(Unit.actionID)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, nameof(Unit.nextActionID)),
			SLD_ENTRY2(/*unit,*/ SaveLoadType.SLDT_UINT8, nameof(Unit.fireDelay), SaveLoadType.SLDT_UINT16),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, nameof(Unit.distanceToDestination)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, nameof(Unit.targetAttack)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, nameof(Unit.targetMove)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, nameof(Unit.amount)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, nameof(Unit.deviated)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, $"{nameof(Unit.targetLast)}.{nameof(tile32.x)}"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, $"{nameof(Unit.targetLast)}.{nameof(tile32.y)}"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, $"{nameof(Unit.targetPreLast)}.{nameof(tile32.x)}"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, $"{nameof(Unit.targetPreLast)}.{nameof(tile32.y)}"),
			SLD_SLD2(/*unit,*/ nameof(Unit.orientation), s_saveUnitOrientation, 2),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, nameof(Unit.speedPerTick)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, nameof(Unit.speedRemainder)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, nameof(Unit.speed)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, nameof(Unit.movingSpeed)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, nameof(Unit.wobbleIndex)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_INT8, nameof(Unit.spriteOffset)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, nameof(Unit.blinkCounter)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, nameof(Unit.team)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, nameof(Unit.timer)),
			SLD_ARRAY(/*unit,*/ SaveLoadType.SLDT_UINT8, nameof(Unit.route), 14),
			SLD_END()
		};

		static readonly SaveLoadDesc[] s_saveUnitNewIndex = {
			SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT16, nameof(Object.index)),
			SLD_END()
		};

		static readonly SaveLoadDesc[] s_saveUnitNew = {
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, nameof(Unit.fireDelay)),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, nameof(Unit.deviatedHouse)),
			SLD_EMPTY(SaveLoadType.SLDT_UINT8),
			SLD_EMPTY2(SaveLoadType.SLDT_UINT16, 6),
			SLD_END()
		};

		/*
		 * Load all Units from a file.
		 * @param fp The file to load from.
		 * @param length The length of the data chunk.
		 * @return True if and only if all bytes were read successful.
		 */
		internal static bool Unit_Load(BinaryReader fp, uint length)
		{
			while (length > 0)
			{
				Unit ul;

				/* Read the next index from disk */
				var index = fp.ReadUInt16();

				/* Get the Unit from the pool */
				ul = PoolUnit.Unit_Get_ByIndex(index);
				if (ul == null) return false;

				fp.BaseStream.Seek(-2, SeekOrigin.Current);

				/* Read the next Unit from disk */
				if (!SaveLoad_Load(s_saveUnit, fp, ul)) return false;

				length -= SaveLoad_GetLength(s_saveUnit);

				ul.o.script.scriptInfo = g_scriptUnit;
				ul.o.script.delay = 0;
				ul.timer = 0;
				ul.o.seenByHouses |= (byte)(1 << ul.o.houseID);

				/* In case the new ODUN chunk is not available, Ordos is always the one who deviated */
				if (ul.deviated != 0) ul.deviatedHouse = (byte)HouseType.HOUSE_ORDOS;

				/* ENHANCEMENT -- Due to wrong parameter orders of Unit_Create in original Dune2,
				 *  it happened that units exists with houseID 13. This in fact are Trikes with
				 *  the wrong houseID. So remove those units completely from the savegame. */
				if (ul.o.houseID == 13) continue;
			}
			if (length != 0) return false;

			PoolUnit.Unit_Recount();

			return true;
		}

		/*
		 * Save all Units to a file. It converts pointers to indices where needed.
		 * @param fp The file to save to.
		 * @return True if and only if all bytes were written successful.
		 */
		internal static bool Unit_Save(BinaryWriter fp)
		{
			var find = new PoolFindStruct
			{
				houseID = (byte)HouseType.HOUSE_INVALID,
				type = 0xFFFF,
				index = 0xFFFF
			};

			while (true)
			{
				Unit u;

				u = PoolUnit.Unit_Find(find);
				if (u == null) break;

				if (!SaveLoad_Save(s_saveUnit, fp, u)) return false;
			}

			return true;
		}

		/*
		 * Load all new information of Units from a file.
		 * @param fp The file to load from.
		 * @param length The length of the data chunk.
		 * @return True if and only if all bytes were read successful.
		 */
		internal static bool UnitNew_Load(BinaryReader fp, uint length)
		{
			while (length > 0)
			{
				Unit u;
				var o = new Object();

				/* Read the next index from disk */
				if (!SaveLoad_Load(s_saveUnitNewIndex, fp, o)) return false;

				length -= SaveLoad_GetLength(s_saveUnitNewIndex);

				/* Get the Unit from the pool */
				u = PoolUnit.Unit_Get_ByIndex(o.index);
				if (u == null) return false;

				/* Read the "new" information for this unit */
				if (!SaveLoad_Load(s_saveUnitNew, fp, u)) return false;

				length -= SaveLoad_GetLength(s_saveUnitNew);
			}
			if (length != 0) return false;

			return true;
		}

		/*
		 * Save all new Units information to a file. It converts pointers to indices
		 *   where needed.
		 * @param fp The file to save to.
		 * @return True if and only if all bytes were written successful.
		 */
		internal static bool UnitNew_Save(BinaryWriter fp)
		{
			var find = new PoolFindStruct
			{
				houseID = (byte)HouseType.HOUSE_INVALID,
				type = 0xFFFF,
				index = 0xFFFF
			};

			while (true)
			{
				Unit u;

				u = PoolUnit.Unit_Find(find);
				if (u == null) break;

				if (!SaveLoad_Save(s_saveUnitNewIndex, fp, u.o)) return false;
				if (!SaveLoad_Save(s_saveUnitNew, fp, u)) return false;
			}

			return true;
		}
	}
}
