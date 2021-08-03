/* Save */

namespace SharpDune
{
    delegate bool SaveProc(BinaryWriter bw);

	class Save
	{
		/*
		 * Save the game to a filename
		 *
		 * @param fp The filename of the savegame.
		 * @param description The description of the savegame.
		 * @return True if and only if all bytes were written successful.
		 */
		internal static bool SaveGame_SaveFile(string filename, string description)
		{
			FileStream fp;
			bool res;

			/* In debug-scenario mode, the whole map is uncovered. Cover it now in
			 *  the savegame based on the current position of the units and
			 *  structures. */
			if (g_debugScenario)
			{
				var find = new PoolFindStruct();
				ushort i;

				/* Add fog of war for all tiles on the map */
				for (i = 0; i < 0x1000; i++)
				{
					var tile = Map.g_map[i];
					tile.isUnveiled = false;
					tile.overlayTileID = Sprites.g_veiledTileID;
				}

				find.houseID = (byte)HouseType.HOUSE_INVALID;
				find.type = 0xFFFF;
				find.index = 0xFFFF;

				/* Remove the fog of war for all units */
				while (true)
				{
					Unit u;

					u = PoolUnit.Unit_Find(find);
					if (u == null) break;

                    Unit_RemoveFog(u);
				}

				find.houseID = (byte)HouseType.HOUSE_INVALID;
				find.type = 0xFFFF;
				find.index = 0xFFFF;

				/* Remove the fog of war for all structures */
				while (true)
				{
					Structure s;

					s = PoolStructure.Structure_Find(find);
					if (s == null) break;
					if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;

                    Structure_RemoveFog(s);
				}
			}

			fp = CFile.fopendatadir(SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR, filename, "wb");
			if (fp == null)
			{
				Trace.WriteLine($"ERROR: Failed to open file '{filename}' for writing.");
				return false;
			}

            g_validateStrictIfZero++;
			res = Save_Main(fp, description);
            g_validateStrictIfZero--;

			fp.Close();

			if (!res)
			{
				/* TODO -- Also remove the savegame now */
				Trace.WriteLine($"ERROR: Error while writing savegame '{filename}'.");
				return false;
			}

			return true;
		}

		/*
		 * Save the game for real. It creates all the required chunks and stores them
		 *  to the file. It updates the field lengths where needed.
		 *
		 * @param fp The file to save to.
		 * @param description The description of the savegame.
		 * @return True if and only if all bytes were written successful.
		 */
		static bool Save_Main(FileStream fp, string description)
		{
			uint length;
			uint lengthSwapped;

			try
			{
				using var bw = new BinaryWriter(fp);
				/* Write the 'FORM' chunk (in which all other chunks are) */
				bw.Write("FORM".ToArray()); //if (fwrite("FORM", 4, 1, fp) != 1) return false;

				/* Write zero length for now. We come back to this value before closing */
				length = 0;
				bw.Write(length); //if (fwrite(&length, 4, 1, fp) != 1) return false;

				/* Write the 'SCEN' chunk. Never contains content. */
				bw.Write("SCEN".ToArray()); //if (fwrite("SCEN", 4, 1, fp) != 1) return false;

				/* Write the 'NAME' chunk. Keep ourself word-aligned. */
				bw.Write("NAME".ToArray()); //if (fwrite("NAME", 4, 1, fp) != 1) return false;
				length = Min(255, (uint)description.Length + 1);
				lengthSwapped = Endian.HTOBE32(length);
				bw.Write(lengthSwapped); //if (fwrite(&lengthSwapped, 4, 1, fp) != 1) return false;
				bw.Write($"{description}\0".ToArray()); //if (fwrite(description, length, 1, fp) != 1) return false;
				/* Ensure we are word aligned */
				if ((length & 1) == 1)
				{
					byte empty = 0;
					bw.Write(empty); //if (fwrite(&empty, 1, 1, fp) != 1) return false;
				}

				/* Store all additional chunks */
				if (!Save_Chunk(bw, "INFO", SaveLoadInfo.Info_Save)) return false;
				if (!Save_Chunk(bw, "PLYR", SaveLoadHouse.House_Save)) return false;
				if (!Save_Chunk(bw, "UNIT", SaveLoadUnit.Unit_Save)) return false;
				if (!Save_Chunk(bw, "BLDG", SaveLoadStructure.Structure_Save)) return false;
				if (!Save_Chunk(bw, "MAP ", SaveLoadMap.Map_Save)) return false;
				if (!Save_Chunk(bw, "TEAM", SaveLoadTeam.Team_Save)) return false;
				if (!Save_Chunk(bw, "ODUN", SaveLoadUnit.UnitNew_Save)) return false;

				/* Write the total length of all data in the FORM chunk */
				length = (uint)fp.Position - 8; //length = ftell(fp) - 8;
				fp.Seek(4, SeekOrigin.Begin); //fseek(fp, 4, SEEK_SET);
				lengthSwapped = Endian.HTOBE32(length);
				bw.Write(lengthSwapped); //if (fwrite(&lengthSwapped, 4, 1, fp) != 1) return false;

				return true;
			}
			catch (Exception e)
			{
				Trace.WriteLine($"ERROR: {e.Message}");
				return false;
			}
		}

		/*
		 * Save a chunk of data.
		 * @param fp The file to save to.
		 * @param header The chunk identification string (4 chars, always).
		 * @param saveProc The proc to call to generate the content of the chunk.
		 * @return True if and only if all bytes were written successful.
		 */
		static bool Save_Chunk(BinaryWriter fp, string header, SaveProc saveProc)
		{
			uint position;
			uint length;
			uint lengthSwapped;

			fp.Write(header.ToArray()); //if (fwrite(header, 4, 1, fp) != 1) return false;

			/* Reserve the length field */
			length = 0;
			fp.Write(length); //if (fwrite(&length, 4, 1, fp) != 1) return false;

			/* Store the content of the chunk, and remember the length */
			position = (uint)fp.BaseStream.Position; //ftell(fp);
			if (!saveProc(fp)) return false;
			length = (uint)fp.BaseStream.Position - position; //ftell(fp)

			/* Ensure we are word aligned */
			if ((length & 1) == 1)
			{
				byte empty = 0;
				fp.Write(empty); //if (fwrite(&empty, 1, 1, fp) != 1) return false;
			}

			/* Write back the chunk size */
			fp.Seek((int)(position - 4), SeekOrigin.Begin); //fseek(fp, position - 4, SEEK_SET);
			lengthSwapped = Endian.HTOBE32(length);
			fp.Write(lengthSwapped); //if (fwrite(&lengthSwapped, 4, 1, fp) != 1) return false;
			fp.Seek(0, SeekOrigin.End); //fseek(fp, 0, SEEK_END);

			return true;
		}
	}
}
