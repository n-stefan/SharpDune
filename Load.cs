/* Load */

namespace SharpDune
{
    class Load
	{
		/*
         * In case the current house is Mercenary, another palette is loaded.
         */
		internal static void Load_Palette_Mercenaries()
		{
			if (CHouse.g_playerHouseID == HouseType.HOUSE_MERCENARY)
			{
				CFile.File_ReadBlockFile("IBM.PAL", Gfx.g_palette1, 256 * 3);
			}
		}

		internal static bool SaveGame_LoadFile(string filename)
		{
			FileStream fp;
			bool res;

			Sound.Sound_Output_Feedback(0xFFFE);

			CSharpDune.Game_Init();

			fp = CFile.fopendatadir(SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR, filename, "rb");
			if (fp == null)
			{
				Trace.WriteLine($"ERROR: Failed to open file '{filename}' for reading.");
				return false;
			}

			Sprites.Sprites_LoadTiles();

			CSharpDune.g_validateStrictIfZero++;
			res = Load_Main(fp);
			CSharpDune.g_validateStrictIfZero--;

			fp.Close();

			if (!res)
			{
				Trace.WriteLine($"ERROR: Error while loading savegame '{filename}'.");
				return false;
			}

			if (CSharpDune.g_gameMode != GameMode.GM_RESTART) CSharpDune.Game_Prepare();

			return true;
		}

		static bool Load_Main(FileStream fp)
		{
			uint position;
			uint length;
			uint header;
			ushort version;

			try
			{
				using var br = new BinaryReader(fp);
				/* All SharpDUNE / Dune2 savegames should start with 'FORM' */
				header = br.ReadUInt32(); //if (fread(&header, sizeof(uint32), 1, fp) != 1) return false;
				if (Endian.BETOH32(header) != (uint)CSharpDune.MultiChar[FourCC.FORM])
				{
					Trace.WriteLine("ERROR: Invalid magic header in savegame. Not a SharpDUNE / Dune2 savegame.");
					return false;
				}

				/* The total length field, which is ignored */
				length = br.ReadUInt32(); //if (fread(&length, sizeof(uint32), 1, fp) != 1) return false;

				/* The next 'chunk' is fake, and has no length field */
				header = br.ReadUInt32(); //if (fread(&header, sizeof(uint32), 1, fp) != 1) return false;
				if (Endian.BETOH32(header) != (uint)CSharpDune.MultiChar[FourCC.SCEN]) return false;

				position = (uint)fp.Position; //ftell(fp);

				/* Find the 'INFO' chunk, as it contains the savegame version */
				version = 0;
				length = Load_FindChunk(br, (uint)CSharpDune.MultiChar[FourCC.INFO]);
				if (length == 0) return false;

				/* Read the savegame version */
				if (!CFile.fread_le_uint16(ref version, fp)) return false;
				length -= 2;
				if (version == 0) return false;

				if (version != 0x0290)
				{
					/* Get the scenarioID / campaignID */
					if (!SaveLoadInfo.Info_LoadOld(br)) return false;

					CSharpDune.g_gameMode = GameMode.GM_RESTART;

					/* Find the 'PLYR' chunk */
					fp.Seek(position, SeekOrigin.Begin); //fseek(fp, position, SEEK_SET);
					length = Load_FindChunk(br, (uint)CSharpDune.MultiChar[FourCC.PLYR]);
					if (length == 0) return false;

					/* Find the human player */
					if (!SaveLoadHouse.House_LoadOld(br, length)) return false;

					Gui.Gui.GUI_DisplayModalMessage(CString.String_Get_ByIndex(Text.STR_WARNING_ORIGINAL_SAVED_GAMES_ARE_INCOMPATABLE_WITH_THE_NEW_VERSION_THE_BATTLE_WILL_BE_RESTARTED), 0xFFFF);

					return true;
				}

				/* Load the 'INFO' chunk'. It has to be the first chunk loaded */
				if (!SaveLoadInfo.Info_Load(br, length)) return false;

				/* Rewind, and read other chunks */
				fp.Seek(position, SeekOrigin.Begin); //fseek(fp, position, SEEK_SET);

				while (true) //(fread(&header, sizeof(uint32), 1, fp) == 1)
				{
					header = br.ReadUInt32();
					length = br.ReadUInt32(); //(fread(&length, sizeof(uint32), 1, fp) != 1)
					length = Endian.BETOH32(length);

					var headerValue = Endian.BETOH32(header);
					if (headerValue == CSharpDune.MultiChar[FourCC.NAME])
					{
						/* 'NAME' chunk is of no interest to us */
					}
					else if (headerValue == CSharpDune.MultiChar[FourCC.INFO])
					{
						/* 'INFO' chunk is already read */
					}
					else if (headerValue == CSharpDune.MultiChar[FourCC.MAP])
                    {
						if (!SaveLoadMap.Map_Load(fp, length)) return false;
					}
					else if (headerValue == CSharpDune.MultiChar[FourCC.PLYR])
                    {
						if (!SaveLoadHouse.House_Load(br, length)) return false;
					}
					else if (headerValue == CSharpDune.MultiChar[FourCC.UNIT])
                    {
						if (!SaveLoadUnit.Unit_Load(br, length)) return false;
					}
					else if (headerValue == CSharpDune.MultiChar[FourCC.BLDG])
                    {
						if (!SaveLoadStructure.Structure_Load(br, length)) return false;
					}
					else if (headerValue == CSharpDune.MultiChar[FourCC.TEAM])
                    {
						if (!SaveLoadTeam.Team_Load(br, length)) return false;
					}
					else if (headerValue == CSharpDune.MultiChar[FourCC.ODUN])
                    {
						if (!SaveLoadUnit.UnitNew_Load(br, length)) return false;
					}
					else
                    {
						Trace.WriteLine($"ERROR: Unknown chunk in savegame: {header}{header >> 8}{header >> 16}{header >> 24} (length: {length}). Skipped.");
                    }

					/* Savegames are word aligned */
					position += length + 8 + (length & 1);
					fp.Seek(position, SeekOrigin.Begin); //fseek(fp, position, SEEK_SET);
				}
			}
			catch (EndOfStreamException)
            {
				return true;
			}
			catch (Exception e)
			{
				Trace.WriteLine($"ERROR: {e.Message}");
				return false;
			}
		}

		static uint Load_FindChunk(BinaryReader br, uint chunk)
		{
			uint header;
			uint length;

			try
            {
				while (true) //(fread(&header, sizeof(uint32), 1, fp) == 1)
				{
					header = br.ReadUInt32();
					length = br.ReadUInt32(); //(fread(&length, sizeof(uint32), 1, fp) != 1)
					length = Endian.BETOH32(length);
					if (Endian.BETOH32(header) != chunk)
					{
						br.BaseStream.Seek(length + (length & 1), SeekOrigin.Current); //fseek(fp, length + (length & 1), SEEK_CUR);
						continue;
					}
					return length;
				}
			}
			catch (Exception)
            {
				return 0;
			}
		}
	}
}
