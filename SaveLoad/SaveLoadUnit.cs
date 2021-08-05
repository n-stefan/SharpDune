namespace SharpDune.SaveLoad
{
    class SaveLoadUnit
    {
		static readonly SaveLoadDesc[] s_saveUnitOrientation = {
			SLD_ENTRY(/*dir24,*/ SLDT_INT8, nameof(Dir24.speed)),
			SLD_ENTRY(/*dir24,*/ SLDT_INT8, nameof(Dir24.target)),
			SLD_ENTRY(/*dir24,*/ SLDT_INT8, nameof(Dir24.current)),
			SLD_END()
		};

		static readonly SaveLoadDesc[] s_saveUnit = {
			SLD_SLD(/*unit,*/ nameof(CUnit.o), g_saveObject),
			SLD_EMPTY(SLDT_UINT16),
			SLD_ENTRY(/*unit,*/ SLDT_UINT16, $"{nameof(CUnit.currentDestination)}.{nameof(Tile32.x)}"),
			SLD_ENTRY(/*unit,*/ SLDT_UINT16, $"{nameof(CUnit.currentDestination)}.{nameof(Tile32.y)}"),
			SLD_ENTRY(/*unit,*/ SLDT_UINT16, nameof(CUnit.originEncoded)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT8, nameof(CUnit.actionID)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT8, nameof(CUnit.nextActionID)),
			SLD_ENTRY2(/*unit,*/ SLDT_UINT8, nameof(CUnit.fireDelay), SLDT_UINT16),
			SLD_ENTRY(/*unit,*/ SLDT_UINT16, nameof(CUnit.distanceToDestination)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT16, nameof(CUnit.targetAttack)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT16, nameof(CUnit.targetMove)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT8, nameof(CUnit.amount)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT8, nameof(CUnit.deviated)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT16, $"{nameof(CUnit.targetLast)}.{nameof(Tile32.x)}"),
			SLD_ENTRY(/*unit,*/ SLDT_UINT16, $"{nameof(CUnit.targetLast)}.{nameof(Tile32.y)}"),
			SLD_ENTRY(/*unit,*/ SLDT_UINT16, $"{nameof(CUnit.targetPreLast)}.{nameof(Tile32.x)}"),
			SLD_ENTRY(/*unit,*/ SLDT_UINT16, $"{nameof(CUnit.targetPreLast)}.{nameof(Tile32.y)}"),
			SLD_SLD2(/*unit,*/ nameof(CUnit.orientation), s_saveUnitOrientation, 2),
			SLD_ENTRY(/*unit,*/ SLDT_UINT8, nameof(CUnit.speedPerTick)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT8, nameof(CUnit.speedRemainder)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT8, nameof(CUnit.speed)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT8, nameof(CUnit.movingSpeed)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT8, nameof(CUnit.wobbleIndex)),
			SLD_ENTRY(/*unit,*/ SLDT_INT8, nameof(CUnit.spriteOffset)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT8, nameof(CUnit.blinkCounter)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT8, nameof(CUnit.team)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT16, nameof(CUnit.timer)),
			SLD_ARRAY(/*unit,*/ SLDT_UINT8, nameof(CUnit.route), 14),
			SLD_END()
		};

		static readonly SaveLoadDesc[] s_saveUnitNewIndex = {
			SLD_ENTRY(/*obj,*/ SLDT_UINT16, nameof(CObject.index)),
			SLD_END()
		};

		static readonly SaveLoadDesc[] s_saveUnitNew = {
			SLD_ENTRY(/*unit,*/ SLDT_UINT16, nameof(CUnit.fireDelay)),
			SLD_ENTRY(/*unit,*/ SLDT_UINT8, nameof(CUnit.deviatedHouse)),
			SLD_EMPTY(SLDT_UINT8),
			SLD_EMPTY2(SLDT_UINT16, 6),
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
				CUnit ul;

				/* Read the next index from disk */
				var index = fp.ReadUInt16();

				/* Get the Unit from the pool */
				ul = Unit_Get_ByIndex(index);
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

            Unit_Recount();

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
				CUnit u;

				u = Unit_Find(find);
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
				CUnit u;
				var o = new CObject();

				/* Read the next index from disk */
				if (!SaveLoad_Load(s_saveUnitNewIndex, fp, o)) return false;

				length -= SaveLoad_GetLength(s_saveUnitNewIndex);

				/* Get the Unit from the pool */
				u = Unit_Get_ByIndex(o.index);
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
				CUnit u;

				u = Unit_Find(find);
				if (u == null) break;

				if (!SaveLoad_Save(s_saveUnitNewIndex, fp, u.o)) return false;
				if (!SaveLoad_Save(s_saveUnitNew, fp, u)) return false;
			}

			return true;
		}
	}
}
