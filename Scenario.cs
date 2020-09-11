/* Scenario handling */

using System;

namespace SharpDune
{
	/*
    * Information about reinforcements in the scenario.
    */
	class Reinforcement
	{
		internal ushort unitID;                                 /*!< The Unit which is already created and ready to join the game. */
		internal ushort locationID;                             /*!< The location where the Unit will appear. */
		internal ushort timeLeft;                               /*!< In how many ticks the Unit will appear. */
		internal ushort timeBetween;                            /*!< In how many ticks the Unit will appear again if repeat is set. */
		internal ushort repeat;                                 /*!< If non-zero, the Unit will appear every timeBetween ticks. */
	}

	/*
    * Information about the current loaded scenario.
    */
	class Scenario
	{
		internal ushort score;                                          /*!< Base score. */
		internal ushort winFlags;                                       /*!< BASIC/WinFlags. */
		internal ushort loseFlags;                                      /*!< BASIC/LoseFlags. */
		internal uint mapSeed;                                          /*!< MAP/Seed. */
		internal ushort mapScale;                                       /*!< BASIC/MapScale. 0 is 62x62, 1 is 32x32, 2 is 21x21. */
		internal ushort timeOut;                                        /*!< BASIC/TimeOut. */
		internal string /*char[14]*/ pictureBriefing;                   /*!< BASIC/BriefPicture. */
		internal string /*char[14]*/ pictureWin;                        /*!< BASIC/WinPicture. */
		internal string /*char[14]*/ pictureLose;                       /*!< BASIC/LosePicture. */
		internal ushort killedAllied;                                   /*!< Number of units lost by "You". */
		internal ushort killedEnemy;                                    /*!< Number of units lost by "Enemy". */
		internal ushort destroyedAllied;                                /*!< Number of structures lost by "You". */
		internal ushort destroyedEnemy;                                 /*!< Number of structures lost by "Enemy". */
		internal ushort harvestedAllied;                                /*!< Total amount of spice harvested by "You". */
		internal ushort harvestedEnemy;                                 /*!< Total amount of spice harvested by "Enemy". */
		internal Reinforcement[] reinforcement = new Reinforcement[16]; /*!< Reinforcement information. */

		internal Scenario()
		{
			for (var i = 0; i < reinforcement.Length; i++) reinforcement[i] = new Reinforcement();
		}
	}

	class CScenario
	{
		internal static Scenario g_scenario = new Scenario();

		static /* void* */string s_scenarioBuffer = string.Empty;

		static void Scenario_Load_Chunk(string category, Action<string, string> ptr)
		{
			string value;

			var keys = Ini.Ini_GetString(category, null, null, null, s_scenarioBuffer);
			if (keys == null) return;

			foreach (var key in keys.Split('|'))
			{
				value = Ini.Ini_GetString(category, key, null, null, s_scenarioBuffer);
				ptr?.Invoke(key, value);
			}
		}

		static void Scenario_Load_General()
		{
			g_scenario.winFlags = (ushort)Ini.Ini_GetInteger("BASIC", "WinFlags", 0, s_scenarioBuffer);
			g_scenario.loseFlags = (ushort)Ini.Ini_GetInteger("BASIC", "LoseFlags", 0, s_scenarioBuffer);
			g_scenario.mapSeed = (uint)Ini.Ini_GetInteger("MAP", "Seed", 0, s_scenarioBuffer);
			g_scenario.timeOut = (ushort)Ini.Ini_GetInteger("BASIC", "TimeOut", 0, s_scenarioBuffer);
			Gui.g_minimapPosition = (ushort)Ini.Ini_GetInteger("BASIC", "TacticalPos", Gui.g_minimapPosition, s_scenarioBuffer);
			Gui.g_selectionRectanglePosition = (ushort)Ini.Ini_GetInteger("BASIC", "CursorPos", Gui.g_selectionRectanglePosition, s_scenarioBuffer);
			g_scenario.mapScale = (ushort)Ini.Ini_GetInteger("BASIC", "MapScale", 0, s_scenarioBuffer);

			g_scenario.pictureBriefing = Ini.Ini_GetString("BASIC", "BriefPicture", "HARVEST.WSA", /*ref g_scenario.pictureBriefing,*/ 14, s_scenarioBuffer);
			g_scenario.pictureWin = Ini.Ini_GetString("BASIC", "WinPicture", "WIN1.WSA", /*ref g_scenario.pictureWin,*/ 14, s_scenarioBuffer);
			g_scenario.pictureLose = Ini.Ini_GetString("BASIC", "LosePicture", "LOSTBILD.WSA", /*ref g_scenario.pictureLose,*/ 14, s_scenarioBuffer);

			Gui.g_viewportPosition = Gui.g_minimapPosition;
			Gui.g_selectionPosition = Gui.g_selectionRectanglePosition;
		}

		static void Scenario_Load_House(byte houseID)
		{
			string houseName = CHouse.g_table_houseInfo[houseID].name;
			string houseType; //char*
			string buf; //char[128]
						//char[] b; //char*
			House h;

			/* Get the type of the House (CPU / Human) */
			buf = Ini.Ini_GetString(houseName, "Brain", "NONE", /*ref buf,*/ 127, s_scenarioBuffer);
			//b = buf.ToArray();
			//for (var i = 0; i < b.Length; i++) if (b[i] >= 'a' && b[i] <= 'z') unchecked { b[i] += (char)('A' - 'a'); }
			//for (b = buf; *b != '\0'; b++) if (*b >= 'a' && *b <= 'z') *b += 'A' - 'a';
			//buf = new string(b);
			//buf = buf.ToUpper();

			//strstr("HUMAN$CPU", buf);
			if (string.Equals(buf, "HUMAN", StringComparison.OrdinalIgnoreCase))
				houseType = "HUMAN";
			else if (string.Equals(buf, "CPU", StringComparison.OrdinalIgnoreCase))
				houseType = "CPU";
			else
				return;

			/* Create the house */
			h = CHouse.House_Allocate(houseID);

			h.credits = (ushort)Ini.Ini_GetInteger(houseName, "Credits", 0, s_scenarioBuffer);
			h.creditsQuota = (ushort)Ini.Ini_GetInteger(houseName, "Quota", 0, s_scenarioBuffer);
			h.unitCountMax = (ushort)Ini.Ini_GetInteger(houseName, "MaxUnit", 39, s_scenarioBuffer);

			/* For 'Brain = Human' we have to set a few additional things */
			if (houseType[0] != 'H') return;

			h.flags.human = true;

			CHouse.g_playerHouseID = (HouseType)houseID;
			CHouse.g_playerHouse = h;
			CHouse.g_playerCreditsNoSilo = h.credits;
		}

		static void Scenario_Load_Houses()
		{
			House h;
			byte houseID;

			for (houseID = 0; houseID < (byte)HouseType.HOUSE_MAX; houseID++)
			{
				Scenario_Load_House(houseID);
			}

			h = CHouse.g_playerHouse;
			/* In case there was no unitCountMax in the scenario, calculate
			 *  it based on values used for the AI controlled houses. */
			if (h.unitCountMax == 0)
			{
				PoolFindStruct find = new PoolFindStruct();
				byte max;
				House h2;

				find.houseID = (byte)HouseType.HOUSE_INVALID;
				find.index = 0xFFFF;
				find.type = 0xFFFF;

				max = 80;
				while ((h2 = CHouse.House_Find(find)) != null)
				{
					/* Skip the human controlled house */
					if (h2.flags.human) continue;
					max -= (byte)h2.unitCountMax;
				}

				h.unitCountMax = max;
			}
		}

		static void Scenario_Load_Reinforcement(string key, string settings)
		{
			byte index, houseType, unitType, locationID;
			ushort timeBetween;
			tile32 position;
			bool repeat;
			Unit u;
			string[] split;

			key = key.Replace(";", "");
			index = byte.Parse(key);
			index--;

			/* The value should have 4 values separated by a ',' */
			split = settings.Split(','); //strchr(settings, ',');
			if (split == null || split.Length == 0) return;
			//*split = '\0';

			/* First value is the House type */
			houseType = CHouse.House_StringToType(split[0]);
			if (houseType == (byte)HouseType.HOUSE_INVALID) return;

			/* Find the next value in the ',' separated list */
			//settings = split + 1;
			//split = strchr(settings, ',');
			//if (split == NULL) return;
			//*split = '\0';

			/* Second value is the Unit type */
			unitType = CUnit.Unit_StringToType(split[1]);
			if (unitType == (byte)UnitType.UNIT_INVALID) return;

			/* Find the next value in the ',' separated list */
			//settings = split + 1;
			//split = strchr(settings, ',');
			//if (split == NULL) return;
			//*split = '\0';

			/* Third value is the location of the reinforcement */
			if (string.Equals(split[2], "NORTH", StringComparison.OrdinalIgnoreCase)) locationID = 0; //if (strcasecmp(settings, "NORTH") == 0)
			else if (string.Equals(split[2], "EAST", StringComparison.OrdinalIgnoreCase)) locationID = 1; //else if (strcasecmp(settings, "EAST") == 0)
			else if (string.Equals(split[2], "SOUTH", StringComparison.OrdinalIgnoreCase)) locationID = 2; //else if (strcasecmp(settings, "SOUTH") == 0)
			else if (string.Equals(split[2], "WEST", StringComparison.OrdinalIgnoreCase)) locationID = 3; //else if (strcasecmp(settings, "WEST") == 0)
			else if (string.Equals(split[2], "AIR", StringComparison.OrdinalIgnoreCase)) locationID = 4; //else if (strcasecmp(settings, "AIR") == 0)
			else if (string.Equals(split[2], "VISIBLE", StringComparison.OrdinalIgnoreCase)) locationID = 5; //else if (strcasecmp(settings, "VISIBLE") == 0)
			else if (string.Equals(split[2], "ENEMYBASE", StringComparison.OrdinalIgnoreCase)) locationID = 6; //else if (strcasecmp(settings, "ENEMYBASE") == 0)
			else if (string.Equals(split[2], "HOMEBASE", StringComparison.OrdinalIgnoreCase)) locationID = 7; //else if (strcasecmp(settings, "HOMEBASE") == 0)
			else return;

			/* Fourth value is the time between reinforcement */
			//settings = split + 1;
			timeBetween = (ushort)(ushort.Parse(split[3]) * 6 + 1);
			repeat = split[3].EndsWith('+'); //settings[strlen(settings) - 1] == '+') ? true : false;
			/* ENHANCEMENT -- Dune2 makes a mistake in reading the '+', causing repeat to be always false */
			if (!CSharpDune.g_dune2_enhanced) repeat = false;

			position = new tile32();
			position.x = 0xFFFF;
			position.y = 0xFFFF;
			u = CUnit.Unit_Create((ushort)UnitIndex.UNIT_INDEX_INVALID, unitType, houseType, position, 0);
			if (u == null) return;

			g_scenario.reinforcement[index].unitID = u.o.index;
			g_scenario.reinforcement[index].locationID = locationID;
			g_scenario.reinforcement[index].timeLeft = timeBetween;
			g_scenario.reinforcement[index].timeBetween = timeBetween;
			g_scenario.reinforcement[index].repeat = (ushort)(repeat ? 1 : 0);
		}

		internal static bool Scenario_Load(ushort scenarioID, byte houseID)
		{
			string filename; //char[14];
			int i;

			if (houseID >= (byte)HouseType.HOUSE_MAX) return false;

			CSharpDune.g_scenarioID = scenarioID;

			/* Load scenario file */
			filename = $"SCEN{CHouse.g_table_houseInfo[houseID].name[0]}{scenarioID:D3}.INI"; //sprintf(filename, "SCEN%c%03hu.INI", g_table_houseInfo[houseID].name[0], scenarioID);
			if (!CFile.File_Exists(filename)) return false;
			CFile.File_ReadWholeFile(filename, ref s_scenarioBuffer); //s_scenarioBuffer = CFile.File_ReadWholeFile(filename);

			//memset(&g_scenario, 0, sizeof(Scenario));

			Scenario_Load_General();
			Sprites.Sprites_LoadTiles();
			Map.Map_CreateLandscape(g_scenario.mapSeed);

			for (i = 0; i < 16; i++)
			{
				g_scenario.reinforcement[i].unitID = (ushort)UnitIndex.UNIT_INDEX_INVALID;
			}

			Scenario_Load_Houses();

			Scenario_Load_Chunk("UNITS", Scenario_Load_Unit);
			Scenario_Load_Chunk("STRUCTURES", Scenario_Load_Structure);
			Scenario_Load_Chunk("MAP", Scenario_Load_Map);
			Scenario_Load_Chunk("REINFORCEMENTS", Scenario_Load_Reinforcement);
			Scenario_Load_Chunk("TEAMS", Scenario_Load_Team);
			Scenario_Load_Chunk("CHOAM", Scenario_Load_Choam);

			Scenario_Load_MapParts("Bloom", Scenario_Load_Map_Bloom);
			Scenario_Load_MapParts("Field", Scenario_Load_Map_Field);
			Scenario_Load_MapParts("Special", Scenario_Load_Map_Special);

			CSharpDune.g_tickScenarioStart = Timer.g_timerGame;

			s_scenarioBuffer = string.Empty; //free(s_scenarioBuffer);
			return true;
		}

		static void Scenario_Load_MapParts(string key, Action<ushort, Tile> ptr)
		{
			string[] s; //char*
			string buf; //char[128]

			buf = Ini.Ini_GetString("MAP", key, string.Empty, /*ref buf,*/ 127, s_scenarioBuffer);
			if (buf == string.Empty) return;

			s = buf.Split(","); //strtok(buf, ",\r\n");
			for (int i = 0; i < s.Length; i++) //while (s != NULL) {
			{
				ushort packed;
				Tile t;

				packed = ushort.Parse(s[i]); //atoi(s);
				t = Map.g_map[packed];

				ptr?.Invoke(packed, t);

				//s = strtok(NULL, ",\r\n");
			}
		}

		/*
		 * Initialize a unit count of the starport.
		 * @param key Unit type to set.
		 * @param settings Count to set.
		 */
		static void Scenario_Load_Choam(string key, string settings)
		{
			byte unitType;

			unitType = CUnit.Unit_StringToType(key);
			if (unitType == (byte)UnitType.UNIT_INVALID) return;

			CUnit.g_starportAvailable[unitType] = short.Parse(settings);
		}

		static void Scenario_Load_Map(string key, string settings)
		{
			Tile t;
			ushort packed;
			ushort value;
			string[] s;
			string posY; //char[3]

			if (key[0] != 'C') return;

			posY = key[4..6]; //memcpy(posY, key + 4, 2);
							  //posY[2] = '\0';

			packed = (ushort)(CTile.Tile_PackXY(ushort.Parse(posY), ushort.Parse(key[6..])) & 0xFFF);
			t = Map.g_map[packed];

			s = settings.Split(",\r\n"); //strtok(settings, ",\r\n");
			value = ushort.Parse(s[0]);
			t.houseID = (byte)(value & 0x07);
			t.isUnveiled = (value & 0x08) != 0;
			t.hasUnit = (value & 0x10) != 0;
			t.hasStructure = (value & 0x20) != 0;
			t.hasAnimation = (value & 0x40) != 0;
			t.hasExplosion = (value & 0x80) != 0;

			//s = strtok(NULL, ",\r\n");
			t.groundTileID = (ushort)(ushort.Parse(s[1]) & 0x01FF);
			if (Map.g_mapTileID[packed] != t.groundTileID) Map.g_mapTileID[packed] |= 0x8000;

			if (!t.isUnveiled) t.overlayTileID = Sprites.g_veiledTileID;
		}

		static void Scenario_Load_Map_Bloom(ushort packed, Tile t)
		{
			t.groundTileID = Sprites.g_bloomTileID;
			Map.g_mapTileID[packed] |= 0x8000;
		}

		static void Scenario_Load_Map_Field(ushort packed, Tile t)
		{
			Map.Map_Bloom_ExplodeSpice(packed, (byte)HouseType.HOUSE_INVALID);

			/* Show where a field started in the preview mode by making it an odd looking sprite */
			if (CSharpDune.g_debugScenario)
			{
				t.groundTileID = 0x01FF;
			}
		}

		static void Scenario_Load_Map_Special(ushort packed, Tile t)
		{
			t.groundTileID = (ushort)(Sprites.g_bloomTileID + 1);
			Map.g_mapTileID[packed] |= 0x8000;
		}

		static void Scenario_Load_Team(string key, string settings)
		{
			byte houseType, teamActionType, movementType;
			ushort minMembers, maxMembers;
			string[] split;

			//VARIABLE_NOT_USED(key);

			/* The value should have 5 values separated by a ',' */
			split = settings.Split(',');

			/* First value is the House type */
			houseType = CHouse.House_StringToType(split[0]);
			if (houseType == (byte)HouseType.HOUSE_INVALID) return;

			/* Second value is the teamAction type */
			teamActionType = CTeam.Team_ActionStringToType(split[1]);
			if (teamActionType == (byte)TeamActionType.TEAM_ACTION_INVALID) return;

			/* Third value is the movement type */
			movementType = CUnit.Unit_MovementStringToType(split[2]);
			if (movementType == (byte)MovementType.MOVEMENT_INVALID) return;

			/* Fourth value is minimum amount of members in team */
			minMembers = ushort.Parse(split[3]);

			/* Fifth value is maximum amount of members in team */
			maxMembers = ushort.Parse(split[4]);

			CTeam.Team_Create(houseType, teamActionType, movementType, minMembers, maxMembers);
		}

		static void Scenario_Load_Unit(string key, string settings)
		{
			byte houseType, unitType, actionType;
			sbyte orientation;
			ushort hitpoints;
			tile32 position;
			Unit u;
			string[] split;

			//VARIABLE_NOT_USED(key);

			/* The value should have 6 values separated by a ',' */
			split = settings.Split(','); //strchr(settings, ',');
			if (split == null || split.Length == 0) return;
			//*split = '\0';

			/* First value is the House type */
			houseType = CHouse.House_StringToType(split[0]);
			if (houseType == (byte)HouseType.HOUSE_INVALID) return;

			/* Find the next value in the ',' separated list */
			//settings = split + 1;
			//split = strchr(settings, ',');
			//if (split == NULL) return;
			//*split = '\0';

			/* Second value is the Unit type */
			unitType = CUnit.Unit_StringToType(split[1]);
			if (unitType == (byte)UnitType.UNIT_INVALID) return;

			/* Find the next value in the ',' separated list */
			//settings = split + 1;
			//split = strchr(settings, ',');
			//if (split == NULL) return;
			//*split = '\0';

			/* Third value is the Hitpoints in percent (in base 256) */
			hitpoints = ushort.Parse(split[2]);

			/* Find the next value in the ',' separated list */
			//settings = split + 1;
			//split = strchr(settings, ',');
			//if (split == NULL) return;
			//*split = '\0';

			/* Fourth value is the position on the map */
			position = CTile.Tile_UnpackTile(ushort.Parse(split[3]));

			/* Find the next value in the ',' separated list */
			//settings = split + 1;
			//split = strchr(settings, ',');
			//if (split == NULL) return;
			//*split = '\0';

			/* Fifth value is orientation */
			orientation = (sbyte)byte.Parse(split[4]);

			/* Sixth value is the current state of the unit */
			//settings = split + 1;
			actionType = CUnit.Unit_ActionStringToType(split[5]);
			if (actionType == (byte)ActionType.ACTION_INVALID) return;

			u = CUnit.Unit_Allocate((ushort)UnitIndex.UNIT_INDEX_INVALID, unitType, houseType);
			if (u == null) return;
			u.o.flags.byScenario = true;

			u.o.hitpoints = (ushort)(hitpoints * CUnit.g_table_unitInfo[unitType].o.hitpoints / 256);
			u.o.position = position;
			u.orientation[0].current = orientation;
			u.actionID = actionType;
			u.nextActionID = (byte)ActionType.ACTION_INVALID;

			/* In case the above function failed and we are passed campaign 2, don't add the unit */
			if (!Map.Map_IsValidPosition(CTile.Tile_PackTile(u.o.position)) && CSharpDune.g_campaignID > 2)
			{
				CUnit.Unit_Free(u);
				return;
			}

			/* XXX -- There is no way this is ever possible, as the beingBuilt flag is unset by Unit_Allocate() */
			if (!u.o.flags.isNotOnMap) CUnit.Unit_SetAction(u, (ActionType)u.actionID);

			u.o.seenByHouses = 0x00;

			CUnit.Unit_HouseUnitCount_Add(u, u.o.houseID);

			CUnit.Unit_SetOrientation(u, u.orientation[0].current, true, 0);
			CUnit.Unit_SetOrientation(u, u.orientation[0].current, true, 1);
			CUnit.Unit_SetSpeed(u, 0);
		}

		static void Scenario_Load_Structure(string key, string settings)
		{
			byte index, houseType, structureType;
			ushort hitpoints, position;
			string[] split;

			/* 'GEN' marked keys are Slabs and Walls, where the number following indicates the position on the map */

			if (key.StartsWith("GEN", StringComparison.OrdinalIgnoreCase)) //(strncasecmp(key, "GEN", 3) == 0)
			{
				/* Position on the map is in the key */
				position = ushort.Parse(key[3..]);

				/* The value should have two values separated by a ',' */
				split = settings.Split(','); //strchr(settings, ',');
				if (split == null || split.Length == 0) return;
				//*split = '\0';

				/* First value is the House type */
				houseType = CHouse.House_StringToType(split[0]);
				if (houseType == (byte)HouseType.HOUSE_INVALID) return;

				/* Second value is the Structure type */
				//settings = split + 1;
				structureType = CStructure.Structure_StringToType(split[1]);
				if (structureType == (byte)StructureType.STRUCTURE_INVALID) return;

				CStructure.Structure_Create((ushort)StructureIndex.STRUCTURE_INDEX_INVALID, structureType, houseType, position);
				return;
			}

			/* The key should start with 'ID', followed by the index */
			index = byte.Parse(key[2..]);

			/* The value should have four values separated by a ',' */
			split = settings.Split(','); //strchr(settings, ',');
			if (split == null || split.Length == 0) return;
			//*split = '\0';

			/* First value is the House type */
			houseType = CHouse.House_StringToType(split[0]);
			if (houseType == (byte)HouseType.HOUSE_INVALID) return;

			/* Find the next value in the ',' separated list */
			//settings = split + 1;
			//split = strchr(settings, ',');
			//if (split == NULL) return;
			//*split = '\0';

			/* Second value is the Structure type */
			structureType = CStructure.Structure_StringToType(split[1]);
			if (structureType == (byte)StructureType.STRUCTURE_INVALID) return;

			/* Find the next value in the ',' separated list */
			//settings = split + 1;
			//split = strchr(settings, ',');
			//if (split == NULL) return;
			//*split = '\0';

			/* Third value is the Hitpoints in percent (in base 256) */
			hitpoints = ushort.Parse(split[2]);
			/* ENHANCEMENT -- Dune2 ignores the % hitpoints read from the scenario */
			if (!CSharpDune.g_dune2_enhanced) hitpoints = 256;
			else if (hitpoints > 256) hitpoints = 256;
			/* this is pointless to have more than 100% hitpoint, however ONE scenario
			 * file has such "bug" : SCENH006.INI
			 * ID001=Ordos,Const Yard,8421,936
			 * ID000=Ordos,Light Fctry,14058,1064     */

			/* Fourth value is the position of the structure */
			//settings = split + 1;
			position = ushort.Parse(split[3]);

			/* Ensure nothing is already on the tile */
			/* XXX -- DUNE2 BUG? -- This only checks the top-left corner? Not really a safety, is it? */
			if (CStructure.Structure_Get_ByPackedTile(position) != null) return;

			{
				Structure s;

				s = CStructure.Structure_Create(index, structureType, houseType, position);
				if (s == null) return;

				s.o.hitpoints = (ushort)(hitpoints * CStructure.g_table_structureInfo[s.o.type].o.hitpoints / 256);
				s.o.flags.degrades = false;
				s.state = (short)StructureState.STRUCTURE_STATE_IDLE;
			}
		}
	}
}
