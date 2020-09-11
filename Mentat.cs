/* Mentat gui */

using System.Diagnostics;
using static System.Math;

namespace SharpDune
{
    class MentatInfo
	{
		internal byte[] notused = new byte[8];
		internal uint length;
	}

	class Mentat
	{
		/*
		 * Information about the mentat.
		 *
		 * eyeX, eyeY, mouthX, mouthY, otherX, otherY, shoulderX, shoulderY
		 */
		static byte[][] s_mentatSpritePositions = { //[6][8]
			new byte[] {0x20,0x58,0x20,0x68,0x00,0x00,0x80,0x68}, /* Harkonnen mentat. */
			new byte[] {0x28,0x50,0x28,0x60,0x48,0x98,0x80,0x80}, /* Atreides mentat. */
			new byte[] {0x10,0x50,0x10,0x60,0x58,0x90,0x80,0x80}, /* Ordos mentat. */
			new byte[] {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00},
			new byte[] {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00},
			new byte[] {0x40,0x50,0x38,0x60,0x00,0x00,0x00,0x00}  /* Intro houses (mercenaries) mentat. */
		};

		static byte s_otherLeft; /*!< Left of the other object (ring of Ordos mentat, book of atreides mentat). */
		static byte s_otherTop;  /*!< Top of the other object (ring of Ordos mentat, book of atreides mentat). */
		internal static bool g_disableOtherMovement; /*!< Disable moving of the other object. */

		static byte s_eyesLeft;    /*!< Left of the changing eyes. */
		static byte s_eyesTop;     /*!< Top of the changing eyes. */
		static byte s_eyesRight;   /*!< Right of the changing eyes. */
		static byte s_eyesBottom;  /*!< Bottom of the changing eyes. */

		static byte s_mouthLeft;   /*!< Left of the moving mouth. */
		static byte s_mouthTop;    /*!< Top of the moving mouth. */
		static byte s_mouthRight;  /*!< Right of the moving mouth. */
		static byte s_mouthBottom; /*!< Bottom of the moving mouth. */

		internal static byte g_shoulderLeft; /*!< Left of the right shoulder of the house mentats (to put them in front of the display in the background). */
		internal static byte g_shoulderTop;  /*!< Top of the right shoulder of the house mentats (to put them in front of the display in the background). */

		internal static bool g_interrogation;      /*!< Asking a security question (changes mentat eye movement). */
		internal static uint g_interrogationTimer; /*!< Speaking time-out for security question. */

		static byte[][][] s_mentatSprites = new byte[3][/*5*/][];

		static bool s_selectMentatHelp = false; /*!< Selecting from the list of in-game help subjects. */
		static byte[] s_helpSubjects;
		static int s_helpSubjectsPointer;

		static string s_mentatFilename; //char[13]
		static ushort s_topHelpList;
		static ushort s_selectedHelpSubject;
		static ushort s_numberHelpSubjects;

		/*
         * Show the briefing screen.
         */
		internal static void GUI_Mentat_ShowBriefing() =>
			GUI_Mentat_ShowDialog((byte)CHouse.g_playerHouseID, (ushort)(CSharpDune.g_campaignID * 4 + 4), CScenario.g_scenario.pictureBriefing, CHouse.g_table_houseInfo[(int)CHouse.g_playerHouseID].musicBriefing);

		/*
         * Show the win screen.
         */
		internal static void GUI_Mentat_ShowWin() =>
			GUI_Mentat_ShowDialog((byte)CHouse.g_playerHouseID, (ushort)(CSharpDune.g_campaignID * 4 + 5), CScenario.g_scenario.pictureWin, CHouse.g_table_houseInfo[(int)CHouse.g_playerHouseID].musicWin);

		/*
         * Show the lose screen.
         */
		internal static void GUI_Mentat_ShowLose() =>
			GUI_Mentat_ShowDialog((byte)CHouse.g_playerHouseID, (ushort)(CSharpDune.g_campaignID * 4 + 6), CScenario.g_scenario.pictureLose, CHouse.g_table_houseInfo[(int)CHouse.g_playerHouseID].musicLose);

		/*
		 * Show the Mentat screen with a dialog (Proceed / Repeat).
		 * @param houseID The house to show the mentat of.
		 * @param stringID The string to show.
		 * @param wsaFilename The WSA to show.
		 * @param musicID The Music to play.
		 */
		static void GUI_Mentat_ShowDialog(byte houseID, ushort stringID, string wsaFilename, ushort musicID)
		{
			Widget w1, w2;

			if (CSharpDune.g_debugSkipDialogs)
			{
				Debug.WriteLine("DEBUG: Skipping Mentat dialog...");
				return;
			}

			w1 = CWidget.GUI_Widget_Allocate(1, CWidget.GUI_Widget_GetShortcut((byte)CString.String_Get_ByIndex(Text.STR_PROCEED)[0]), 168, 168, 379, 0);
			w2 = CWidget.GUI_Widget_Allocate(2, CWidget.GUI_Widget_GetShortcut((byte)CString.String_Get_ByIndex(Text.STR_REPEAT)[0]), 240, 168, 381, 0);

			w1 = CWidget.GUI_Widget_Link(w1, w2);

			Sound.Sound_Output_Feedback(0xFFFE);

			CDriver.Driver_Voice_Play(null, 0xFF);

			Sound.Music_Play(musicID);

			stringID += (ushort)(Text.STR_HOUSE_HARKONNENFROM_THE_DARK_WORLD_OF_GIEDI_PRIME_THE_SAVAGE_HOUSE_HARKONNEN_HAS_SPREAD_ACROSS_THE_UNIVERSE_A_CRUEL_PEOPLE_THE_HARKONNEN_ARE_RUTHLESS_TOWARDS_BOTH_FRIEND_AND_FOE_IN_THEIR_FANATICAL_PURSUIT_OF_POWER + houseID * 40);

			string text;
			do
			{
				text = CString.String_Get_ByIndex(stringID);
				CSharpDune.g_readBuffer = CSharpDune.Encoding.GetBytes(text); //strncpy(g_readBuffer, String_Get_ByIndex(stringID), g_readBufferSize);
				Sleep.sleepIdle();
			} while (GUI_Mentat_Show(text, wsaFilename, w1) == 0x8002);

			w2 = null; //free(w2);
			w1 = null; //free(w1);

			if (musicID != 0xFFFF) CDriver.Driver_Music_FadeOut();
		}

		/*
		 * Show the Mentat screen.
		 * @param spriteBuffer The buffer of the strings.
		 * @param wsaFilename The WSA to show.
		 * @param w The widgets to handle. Can be NULL for no widgets.
		 * @return Return value of GUI_Widget_HandleEvents() or f__B4DA_0AB8_002A_AAB2() (latter when no widgets).
		 */
		internal static ushort GUI_Mentat_Show(string stringBuffer, string wsaFilename, Widget w)
		{
			ushort ret;

			Sprites.Sprites_UnloadTiles();

			GUI_Mentat_Display(wsaFilename, (byte)CHouse.g_playerHouseID);

			Gfx.GFX_Screen_SetActive(Screen.SCREEN_1);

			CWidget.Widget_SetAndPaintCurrentWidget(8);

			if (wsaFilename != null)
			{
				/*WSAObject*/
				(WSAHeader header, CArray<byte> buffer) wsa;

				wsa = Wsa.WSA_LoadFile(wsaFilename, (byte[])Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_2), Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_2), false);
				Wsa.WSA_DisplayFrame(wsa, 0, (ushort)(CWidget.g_curWidgetXBase * 8), CWidget.g_curWidgetYBase, Screen.SCREEN_1);
				Wsa.WSA_Unload(wsa);
			}

			Gui.GUI_DrawSprite(Screen.SCREEN_1, Sprites.g_sprites[397 + (int)CHouse.g_playerHouseID * 15], g_shoulderLeft, g_shoulderTop, 0, 0);
			Gfx.GFX_Screen_SetActive(Screen.SCREEN_0);

			Gui.GUI_Mouse_Hide_Safe();
			Gui.GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, Screen.SCREEN_1, Screen.SCREEN_0);
			Gui.GUI_Mouse_Show_Safe();

			Gui.GUI_SetPaletteAnimated(Gfx.g_palette1, 15);

			ret = GUI_Mentat_Loop(wsaFilename, null, stringBuffer, true, null);

			if (w != null)
			{
				do
				{
					WidgetDraw.GUI_Widget_DrawAll(w);
					ret = CWidget.GUI_Widget_HandleEvents(w);

					Gui.GUI_PaletteAnimate();
					GUI_Mentat_Animation(0);

					Sleep.sleepIdle();
				} while ((ret & 0x8000) == 0);
			}

			Input.Input_History_Clear();

			if (w != null)
			{
				/* reset palette and tiles */
				Load.Load_Palette_Mercenaries();
				Sprites.Sprites_LoadTiles();
			}

			return ret;
		}

		/*
		 * Display a mentat.
		 * @param houseFilename Filename of the house.
		 * @param houseID ID of the house.
		 */
		internal static void GUI_Mentat_Display(string wsaFilename, byte houseID)
		{
			string textBuffer; //char[16]
			Screen oldScreenID;
			int i;

			textBuffer = $"MENTAT{CHouse.g_table_houseInfo[houseID].name[0]}.CPS"; //snprintf(textBuffer, sizeof(textBuffer), "MENTAT%c.CPS", g_table_houseInfo[houseID].name[0]);
			Sprites.Sprites_LoadImage(textBuffer, Screen.SCREEN_1, Gui.g_palette_998A);

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.SCREEN_1);

			if (houseID == (byte)HouseType.HOUSE_MERCENARY)
			{
				CFile.File_ReadBlockFile("BENE.PAL", Gfx.g_palette1, 256 * 3);
			}

			//memset(s_mentatSprites, 0, sizeof(s_mentatSprites));
			s_mentatSprites[0] = new byte[5][];
			s_mentatSprites[1] = new byte[5][];
			s_mentatSprites[2] = new byte[5][];

			s_eyesLeft = s_eyesRight = s_mentatSpritePositions[houseID][0];
			s_eyesTop = s_eyesBottom = s_mentatSpritePositions[houseID][1];

			for (i = 0; i < 5; i++)
			{
				s_mentatSprites[0][i] = Sprites.g_sprites[387 + houseID * 15 + i];
			}

			s_eyesRight += Sprites.Sprite_GetWidth(s_mentatSprites[0][0]);
			s_eyesBottom += Sprites.Sprite_GetHeight(s_mentatSprites[0][0]);

			s_mouthLeft = s_mouthRight = s_mentatSpritePositions[houseID][2];
			s_mouthTop = s_mouthBottom = s_mentatSpritePositions[houseID][3];

			for (i = 0; i < 5; i++)
			{
				s_mentatSprites[1][i] = Sprites.g_sprites[392 + houseID * 15 + i];
			}

			s_mouthRight += Sprites.Sprite_GetWidth(s_mentatSprites[1][0]);
			s_mouthBottom += Sprites.Sprite_GetHeight(s_mentatSprites[1][0]);

			s_otherLeft = s_mentatSpritePositions[houseID][4];
			s_otherTop = s_mentatSpritePositions[houseID][5];

			for (i = 0; i < 4; i++)
			{
				s_mentatSprites[2][i] = Sprites.g_sprites[398 + houseID * 15 + i];
			}

			g_shoulderLeft = s_mentatSpritePositions[houseID][6];
			g_shoulderTop = s_mentatSpritePositions[houseID][7];

			CWidget.Widget_SetAndPaintCurrentWidget(8);

			if (wsaFilename != null)
			{
				/*WSAObject*/
				(WSAHeader header, CArray<byte> buffer) wsa;

				wsa = Wsa.WSA_LoadFile(wsaFilename, (byte[])Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_2), Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_2), false);
				Wsa.WSA_DisplayFrame(wsa, 0, (ushort)(CWidget.g_curWidgetXBase * 8), CWidget.g_curWidgetYBase, Screen.SCREEN_1);
				Wsa.WSA_Unload(wsa);
			}

			Gui.GUI_DrawSprite(Screen.SCREEN_1, Sprites.g_sprites[397 + houseID * 15], g_shoulderLeft, g_shoulderTop, 0, 0);
			Gfx.GFX_Screen_SetActive(oldScreenID);
		}

		internal static ushort GUI_Mentat_Tick()
		{
			GUI_Mentat_Animation((ushort)((g_interrogationTimer < Timer.g_timerGUI) ? 0 : 1));

			return 0;
		}

		static uint movingEyesTimer = 0;      /* Timer when to change the eyes sprite. */
		static ushort movingEyesSprite = 0;     /* Index in _mentatSprites of the displayed moving eyes. */
		static ushort movingEyesNextSprite = 0; /* If not 0, it decides the movingEyesNextSprite */

		static uint movingMouthTimer = 0;
		static ushort movingMouthSprite = 0;

		static uint movingOtherTimer = 0;
		static short otherSprite = 0;
		/*
		 * Draw sprites and handle mouse in a mentat screen.
		 * @param speakingMode If \c 1, the mentat is speaking.
		 */
		internal static void GUI_Mentat_Animation(ushort speakingMode)
		{
			bool partNeedsRedraw;
			ushort i;

			if (movingOtherTimer < Timer.g_timerGUI && !g_disableOtherMovement)
			{
				if (movingOtherTimer != 0)
				{
					byte[] sprite;

					if (s_mentatSprites[2][1 + Abs(otherSprite)] == null)
					{
						otherSprite = (short)(1 - otherSprite);
					}
					else
					{
						otherSprite++;
					}

					sprite = s_mentatSprites[2][Abs(otherSprite)];

					Gui.GUI_Mouse_Hide_InRegion(s_otherLeft, s_otherTop, (ushort)(s_otherLeft + Sprites.Sprite_GetWidth(sprite)), (ushort)(s_otherTop + Sprites.Sprite_GetHeight(sprite)));
					Gui.GUI_DrawSprite(Screen.SCREEN_0, sprite, s_otherLeft, s_otherTop, 0, 0);
					Gui.GUI_Mouse_Show_InRegion();
				}

				switch (CHouse.g_playerHouseID)
				{
					case HouseType.HOUSE_HARKONNEN:
						movingOtherTimer = Timer.g_timerGUI + 300 * 60;
						break;
					case HouseType.HOUSE_ATREIDES:
						movingOtherTimer = (uint)(Timer.g_timerGUI + 60 * Tools.Tools_RandomLCG_Range(1, 3));
						break;
					case HouseType.HOUSE_ORDOS:
						if (otherSprite != 0)
						{
							movingOtherTimer = Timer.g_timerGUI + 6;
						}
						else
						{
							movingOtherTimer = (uint)(Timer.g_timerGUI + 60 * Tools.Tools_RandomLCG_Range(10, 19));
						}
						break;
					default:
						break;
				}
			}

			if (speakingMode == 1)
			{
				if (movingMouthTimer < Timer.g_timerGUI)
				{
					byte[] sprite;

					movingMouthSprite = Tools.Tools_RandomLCG_Range(0, 4);
					sprite = s_mentatSprites[1][movingMouthSprite];

					Gui.GUI_Mouse_Hide_InRegion(s_mouthLeft, s_mouthTop, (ushort)(s_mouthLeft + Sprites.Sprite_GetWidth(sprite)), (ushort)(s_mouthTop + Sprites.Sprite_GetHeight(sprite)));
					Gui.GUI_DrawSprite(Screen.SCREEN_0, sprite, s_mouthLeft, s_mouthTop, 0, 0);
					Gui.GUI_Mouse_Show_InRegion();

					switch (movingMouthSprite)
					{
						case 0:
							movingMouthTimer = Timer.g_timerGUI + Tools.Tools_RandomLCG_Range(7, 30);
							break;
						case 1:
						case 2:
						case 3:
							movingMouthTimer = Timer.g_timerGUI + Tools.Tools_RandomLCG_Range(6, 10);
							break;
						case 4:
							movingMouthTimer = Timer.g_timerGUI + Tools.Tools_RandomLCG_Range(5, 6);
							break;
						default:
							break;
					}
				}
			}
			else
			{
				partNeedsRedraw = false;

				if (Input.Input_Test(0x41) == 0 && Input.Input_Test(0x42) == 0)
				{
					if (movingMouthSprite != 0)
					{
						movingMouthSprite = 0;
						movingMouthTimer = 0;
						partNeedsRedraw = true;
					}
				}
				else if (Mouse.Mouse_InsideRegion(s_mouthLeft, s_mouthTop, s_mouthRight, s_mouthBottom) != 0)
				{
					if (movingMouthTimer != 0xFFFFFFFF)
					{
						movingMouthTimer = 0xFFFFFFFF;
						movingMouthSprite = Tools.Tools_RandomLCG_Range(1, 4);
						partNeedsRedraw = true;
					}
				}
				else
				{
					if (movingMouthSprite != 0)
					{
						movingMouthSprite = 0;
						movingMouthTimer = 0;
						partNeedsRedraw = true;
					}
				}

				if (partNeedsRedraw)
				{
					byte[] sprite;

					sprite = s_mentatSprites[1][movingMouthSprite];

					Gui.GUI_Mouse_Hide_InRegion(s_mouthLeft, s_mouthTop, (ushort)(s_mouthLeft + Sprites.Sprite_GetWidth(sprite)), (ushort)(s_mouthTop + Sprites.Sprite_GetHeight(sprite)));
					Gui.GUI_DrawSprite(Screen.SCREEN_0, sprite, s_mouthLeft, s_mouthTop, 0, 0);
					Gui.GUI_Mouse_Show_InRegion();
				}
			}

			partNeedsRedraw = false;

			if (Input.Input_Test(0x41) != 0 || Input.Input_Test(0x42) != 0)
			{
				if (Mouse.Mouse_InsideRegion(s_eyesLeft, s_eyesTop, s_eyesRight, s_eyesBottom) != 0)
				{
					if (movingEyesSprite != 0x4)
					{
						partNeedsRedraw = true;
						movingEyesSprite = (ushort)((movingEyesSprite == 3) ? 4 : 3);
						movingEyesNextSprite = 0;
						movingEyesTimer = 0;
					}

					if (partNeedsRedraw)
					{
						byte[] sprite;

						sprite = s_mentatSprites[0][movingEyesSprite];

						Gui.GUI_Mouse_Hide_InRegion(s_eyesLeft, s_eyesTop, (ushort)(s_eyesLeft + Sprites.Sprite_GetWidth(sprite)), (ushort)(s_eyesTop + Sprites.Sprite_GetHeight(sprite)));
						Gui.GUI_DrawSprite(Screen.SCREEN_0, sprite, s_eyesLeft, s_eyesTop, 0, 0);
						Gui.GUI_Mouse_Show_InRegion();
					}

					return;
				}
			}

			if (Mouse.Mouse_InsideRegion((short)(s_eyesLeft - 16), (short)(s_eyesTop - 8), (short)(s_eyesRight + 16), (short)(s_eyesBottom + 24)) != 0)
			{
				if (Mouse.Mouse_InsideRegion((short)(s_eyesLeft - 8), s_eyesBottom, (short)(s_eyesRight + 8), Gfx.SCREEN_HEIGHT - 1) != 0)
				{
					i = 3;
				}
				else
				{
					if (Mouse.Mouse_InsideRegion(s_eyesRight, (short)(s_eyesTop - 8), (short)(s_eyesRight + 16), (short)(s_eyesBottom + 8)) != 0)
					{
						i = 2;
					}
					else
					{
						i = (ushort)((Mouse.Mouse_InsideRegion((short)(s_eyesLeft - 16), (short)(s_eyesTop - 8), s_eyesLeft, (short)(s_eyesBottom + 8)) == 0) ? 0 : 1);
					}
				}

				if (i != movingEyesSprite)
				{
					partNeedsRedraw = true;
					movingEyesSprite = i;
					movingEyesNextSprite = 0;
					movingEyesTimer = Timer.g_timerGUI;
				}
			}
			else
			{
				if (movingEyesTimer >= Timer.g_timerGUI) return;

				partNeedsRedraw = true;
				if (movingEyesNextSprite != 0)
				{
					movingEyesSprite = movingEyesNextSprite;
					movingEyesNextSprite = 0;

					if (movingEyesSprite != 4)
					{
						movingEyesTimer = Timer.g_timerGUI + Tools.Tools_RandomLCG_Range(20, 180);
					}
					else
					{
						movingEyesTimer = Timer.g_timerGUI + Tools.Tools_RandomLCG_Range(12, 30);
					}
				}
				else
				{
					i = 0;
					switch (speakingMode)
					{
						case 0:
							i = Tools.Tools_RandomLCG_Range(0, 7);
							if (i > 5)
							{
								i = 1;
							}
							else
							{
								if (i == 5)
								{
									i = 4;
								}
							}
							break;

						case 1:
							if (movingEyesSprite != ((!g_interrogation) ? 0 : 3))
							{
								i = 0;
							}
							else
							{
								i = Tools.Tools_RandomLCG_Range(0, 17);
								if (i > 9)
								{
									i = 0;
								}
								else
								{
									if (i >= 5)
									{
										i = 4;
									}
								}
							}
							break;

						default:
							i = Tools.Tools_RandomLCG_Range(0, 15);
							if (i > 10)
							{
								i = 2;
							}
							else
							{
								if (i >= 5)
								{
									i = 4;
								}
							}
							break;
					}

					if ((i == 2 && movingEyesSprite == 1) || (i == 1 && movingEyesSprite == 2))
					{
						movingEyesNextSprite = i;
						movingEyesSprite = 0;
						movingEyesTimer = Timer.g_timerGUI + Tools.Tools_RandomLCG_Range(1, 5);
					}
					else
					{
						if (i != movingEyesSprite && (i == 4 || movingEyesSprite == 4))
						{
							movingEyesNextSprite = i;
							movingEyesSprite = 3;
							movingEyesTimer = Timer.g_timerGUI;
						}
						else
						{
							movingEyesSprite = i;
							if (i != 4)
							{
								movingEyesTimer = Timer.g_timerGUI + Tools.Tools_RandomLCG_Range(15, 180);
							}
							else
							{
								movingEyesTimer = Timer.g_timerGUI + Tools.Tools_RandomLCG_Range(6, 60);
							}
						}
					}

					if (g_interrogation && movingEyesSprite == 0) movingEyesSprite = 3;
				}
			}

			if (partNeedsRedraw)
			{
				byte[] sprite;

				sprite = s_mentatSprites[0][movingEyesSprite];

				Gui.GUI_Mouse_Hide_InRegion(s_eyesLeft, s_eyesTop, (ushort)(s_eyesLeft + Sprites.Sprite_GetWidth(sprite)), (ushort)(s_eyesTop + Sprites.Sprite_GetHeight(sprite)));
				Gui.GUI_DrawSprite(Screen.SCREEN_0, sprite, s_eyesLeft, s_eyesTop, 0, 0);
				Gui.GUI_Mouse_Show_InRegion();
			}
		}

		internal static ushort GUI_Mentat_Loop(string wsaFilename, string pictureDetails, string text, bool loopAnimation, Widget w)
		{
			Screen oldScreenID;
			ushort oldWidgetID;
			/*WSAObject*/
			(WSAHeader header, CArray<byte> buffer) wsa;
			ushort descLines;
			bool dirty;
			bool done;
			bool textDone;
			ushort frame;
			uint descTick;
			ushort mentatSpeakingMode;
			ushort result;
			uint textTick;
			uint textDelay;
			ushort lines;
			ushort step;
			int partsPointer = 0;

			dirty = false;
			textTick = 0;
			textDelay = 0;

			oldWidgetID = CWidget.Widget_SetCurrentWidget(8);
			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.SCREEN_2);

			wsa = (null, null);

			if (wsaFilename != null)
			{
				wsa = Wsa.WSA_LoadFile(wsaFilename, (byte[])Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_1), Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_1), false);
			}

			step = 0;
			if (wsa == (null, null))
			{
				CWidget.Widget_PaintCurrentWidget();
				step = 1;
			}

			Gui.GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x31);

			descLines = Gui.GUI_SplitText(ref pictureDetails, (ushort)((CWidget.g_curWidgetWidth << 3) + 10), '\r');

			Gui.GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x32);

			var parts/*(parts, textLines)*/ = GUI_Mentat_SplitText(text, 304);
			var textLines = parts?.Length ?? 0;

			mentatSpeakingMode = 2;
			lines = 0;
			frame = 0;
			Timer.g_timerTimeout = 0;
			descTick = Timer.g_timerGUI + 30;

			Input.Input_History_Clear();

			textDone = false;
			result = 0;
			for (done = false; !done; Sleep.sleepIdle())
			{
				ushort key;

				Gfx.GFX_Screen_SetActive(Screen.SCREEN_0);

				key = CWidget.GUI_Widget_HandleEvents(w);

				Gui.GUI_PaletteAnimate();

				if (key != 0)
				{
					if ((key & 0x800) == 0)
					{
						if (w != null)
						{
							if ((key & 0x8000) != 0 && result == 0) result = key;
						}
						else
						{
							if (textDone) result = key;
						}
					}
					else
					{
						key = 0;
					}
				}

				switch (step)
				{
					case 0:
					case 1:
						if (step == 0)
						{
							if (key == 0) break;
							step = 1;
						}

						if (key != 0)
						{
							if (result != 0)
							{
								step = 5;
								break;
							}
							lines = descLines;
							dirty = true;
						}
						else
						{
							if (Timer.g_timerGUI > descTick)
							{
								descTick = Timer.g_timerGUI + 15;
								lines++;
								dirty = true;
							}
						}

						if (lines < descLines && lines <= 12) break;

						step = (ushort)((parts?[partsPointer]/*text*/ != null) ? 2 : 4);
						lines = descLines;
						break;

					case 2:
					case 3:
						if (step == 2)
						{
							Gui.GUI_Mouse_Hide_InRegion(0, 0, Gfx.SCREEN_WIDTH, 40);
							Gui.GUI_Screen_Copy(0, 0, 0, 160, Gfx.SCREEN_WIDTH / 8, 40, Screen.SCREEN_0, Screen.SCREEN_2);
							Gui.GUI_Mouse_Show_InRegion();

							step = 3;
							key = 1;
						}

						if (mentatSpeakingMode == 2 && textTick < Timer.g_timerGUI) key = 1;

						if ((key != 0 && textDone) || result != 0)
						{
							Gui.GUI_Mouse_Hide_InRegion(0, 0, Gfx.SCREEN_WIDTH, 40);
							Gui.GUI_Screen_Copy(0, 160, 0, 0, Gfx.SCREEN_WIDTH / 8, 40, Screen.SCREEN_2, Screen.SCREEN_0);
							Gui.GUI_Mouse_Show_InRegion();

							step = 4;
							mentatSpeakingMode = 0;
							break;
						}

						if (key != 0)
						{
							Gui.GUI_Screen_Copy(0, 160, 0, 0, Gfx.SCREEN_WIDTH / 8, 40, Screen.SCREEN_2, Screen.SCREEN_2);

							if (textLines-- != 0)
							{
								Gfx.GFX_Screen_SetActive(Screen.SCREEN_2);
								Gui.GUI_DrawText_Wrapper(parts[partsPointer]/*text*/, 4, 1, CWidget.g_curWidgetFGColourBlink, 0, 0x32);
								mentatSpeakingMode = 1;
								textDelay = (uint)(parts[partsPointer]/*text*/.Length * 4);
								textTick = Timer.g_timerGUI + textDelay;

								//int textPointer = 0;
								if (textLines != 0)
								{
									partsPointer++;
									//while (text[textPointer++] != '\r') {}
								}
								else
								{
									textDone = true;
								}

								Gfx.GFX_Screen_SetActive(Screen.SCREEN_0);
							}

							Gui.GUI_Mouse_Hide_InRegion(0, 0, Gfx.SCREEN_WIDTH, 40);
							Gui.GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, 40, Screen.SCREEN_2, Screen.SCREEN_0);
							Gui.GUI_Mouse_Show_InRegion();
							break;
						}

						if (mentatSpeakingMode == 0 || textTick > Timer.g_timerGUI) break;

						mentatSpeakingMode = 2;
						textTick += textDelay + textDelay / 2;
						break;

					case 4:
						if (result != 0 || w == null) step = 5;
						break;

					case 5:
						dirty = true;
						done = true;
						break;

					default: break;
				}

				GUI_Mentat_Animation(mentatSpeakingMode);

				if (wsa != (null, null) && Timer.g_timerTimeout == 0)
				{
					Timer.g_timerTimeout = 7;

					do
					{
						if (step == 0 && frame > 4) step = 1;

						if (!Wsa.WSA_DisplayFrame(wsa, frame++, (ushort)(CWidget.g_curWidgetXBase << 3), CWidget.g_curWidgetYBase, Screen.SCREEN_2))
						{
							if (step == 0) step = 1;

							if (loopAnimation)
							{
								frame = 0;
							}
							else
							{
								Wsa.WSA_Unload(wsa);
								wsa = (null, null);
							}
						}
					} while (frame == 0);
					dirty = true;
				}

				if (!dirty) continue;

				GUI_Mentat_DrawInfo(pictureDetails, (ushort)((CWidget.g_curWidgetXBase << 3) + 5), (ushort)(CWidget.g_curWidgetYBase + 3), 8, 0, (short)lines, 0x31);

				Gui.GUI_DrawSprite(Screen.SCREEN_2, Sprites.g_sprites[397 + (byte)CHouse.g_playerHouseID * 15], g_shoulderLeft, g_shoulderTop, 0, 0);
				Gui.GUI_Mouse_Hide_InWidget(CWidget.g_curWidgetIndex);
				Gui.GUI_Screen_Copy((short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetWidth, (short)CWidget.g_curWidgetHeight, Screen.SCREEN_2, Screen.SCREEN_0);
				Gui.GUI_Mouse_Show_InWidget();
				dirty = false;
			}

			if (wsa != (null, null)) Wsa.WSA_Unload(wsa);

			Gfx.GFX_Screen_SetActive(Screen.SCREEN_2);
			Gui.GUI_DrawSprite(Screen.SCREEN_2, Sprites.g_sprites[397 + (byte)CHouse.g_playerHouseID * 15], g_shoulderLeft, g_shoulderTop, 0, 0);
			Gui.GUI_Mouse_Hide_InWidget(CWidget.g_curWidgetIndex);
			Gui.GUI_Screen_Copy((short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetWidth, (short)CWidget.g_curWidgetHeight, Screen.SCREEN_2, Screen.SCREEN_0);
			Gui.GUI_Mouse_Show_InWidget();
			CWidget.Widget_SetCurrentWidget(oldWidgetID);
			Gfx.GFX_Screen_SetActive(oldScreenID);

			Input.Input_History_Clear();

			return result;
		}

		internal static /*(string[] parts, ushort count)*/string[] GUI_Mentat_SplitText(string text, ushort maxWidth)
		{
			//ushort lines = 0;
			ushort height = 0;
			char[] str;
			int i = 0;

			if (text == null) return null; //(null, 0);

			str = text.ToCharArray();

			while (i < str.Length && str[i] != '\0')
			{
				ushort width = 0;

				while (width < maxWidth && str[i] != '.' && str[i] != '!' && str[i] != '?' && str[i] != '\0' && str[i] != '\r')
				{
					width += CFont.Font_GetCharWidth(str[i++]);
				}

				if (width >= maxWidth)
				{
					while (str[i] != ' ') width -= CFont.Font_GetCharWidth(str[i--]);
				}

				height++;

				if ((str[i] != '\0' && (str[i] == '.' || str[i] == '!' || str[i] == '?' || str[i] == '\r')) || height >= 3)
				{
					while (i < str.Length && str[i] != '\0' && (str[i] == ' ' || str[i] == '.' || str[i] == '!' || str[i] == '?' || str[i] == '\r')) i++;

					if (i < str.Length && str[i] != '\0') str[i - 1] = '\0';
					height = 0;
					//lines++;
					continue;
				}

				if (str[i] == '\0')
				{
					//lines++;
					height = 0;
					continue;
				}

				str[i++] = '\r';
			}

			return new string(str).Split('\0'); //(new string(str).Split('\0'), lines);
		}

		/*
		 * Handle clicks on the Mentat widget.
		 * @return True, always.
		 */
		internal static bool GUI_Widget_Mentat_Click(Widget w)
		{
			//VARIABLE_NOT_USED(w);

			Gui.g_cursorSpriteID = 0;

			Sprites.Sprites_SetMouseSprite(0, 0, Sprites.g_sprites[0]);

			Sound.Sound_Output_Feedback(0xFFFE);

			CDriver.Driver_Voice_Play(null, 0xFF);

			Sound.Music_Play(CHouse.g_table_houseInfo[(int)CHouse.g_playerHouseID].musicBriefing);

			Sprites.Sprites_UnloadTiles();

			Timer.Timer_SetTimer(TimerType.TIMER_GAME, false);

			GUI_Mentat_ShowHelpList(false);

			Timer.Timer_SetTimer(TimerType.TIMER_GAME, true);

			CDriver.Driver_Sound_Play(1, 0xFF);

			Sprites.Sprites_LoadTiles();

			Gui.g_textDisplayNeedsUpdate = true;

			Gui.GUI_DrawInterfaceAndRadar(Screen.SCREEN_0);

			Sound.Music_Play((ushort)(Tools.Tools_RandomLCG_Range(0, 5) + 8));

			return true;
		}

		static bool GUI_Mentat_DrawInfo(string text, ushort left, ushort top, ushort height, ushort skip, short lines, ushort flags)
		{
			Screen oldScreenID;
			string[] split;
			int index = 0;

			if (lines <= 0) return false;

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.SCREEN_2);

			split = text.Split('\r');

			while (skip-- != 0) index++; //text += strlen(text) + 1;

			while (lines-- != 0)
			{
				if (!string.IsNullOrWhiteSpace(split[index])) Gui.GUI_DrawText_Wrapper(split[index], (short)left, (short)top, CWidget.g_curWidgetFGColourBlink, 0, flags);
				top += height;
				index++; //text += strlen(text) + 1;
			}

			Gfx.GFX_Screen_SetActive(oldScreenID);

			return true;
		}

		/*
		 * Shows the Help window.
		 * @param proceed Display a "Proceed" button if true, "Exit" otherwise.
		 */
		static void GUI_Mentat_ShowHelpList(bool proceed)
		{
			Screen oldScreenID;

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.SCREEN_1);

			/* ENHANCEMENT -- After visiting Mentat (the help) window, auto-repeat of keys gets disabled. */
			if (!CSharpDune.g_dune2_enhanced) Input.Input_Flags_SetBits((ushort)InputFlagsEnum.INPUT_FLAG_KEY_REPEAT);
			Input.Input_History_Clear();

			GUI_Mentat_Display(null, (byte)CHouse.g_playerHouseID);

			CWidget.g_widgetMentatFirst = CWidget.GUI_Widget_Allocate(1, CWidget.GUI_Widget_GetShortcut((byte)CString.String_Get_ByIndex(Text.STR_EXIT)[0]), 200, 168, (ushort)(proceed ? 379 : 377), 5);
			CWidget.g_widgetMentatFirst.shortcut2 = 'n';

			GUI_Mentat_Create_HelpScreen_Widgets();

			Gui.GUI_Mouse_Hide_Safe();
			Gui.GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, Screen.SCREEN_1, Screen.SCREEN_0);
			Gui.GUI_Mouse_Show_Safe();

			GUI_Mentat_LoadHelpSubjects(true);

			GUI_Mentat_Draw(true);

			Gfx.GFX_Screen_SetActive(Screen.SCREEN_0);

			GUI_Mentat_HelpListLoop();

			CWidget.g_widgetMentatFirst = null; //free(g_widgetMentatFirst);

			Load.Load_Palette_Mercenaries();

			CWidget.GUI_Widget_Free_WithScrollbar(CWidget.g_widgetMentatScrollbar);
			CWidget.g_widgetMentatScrollbar = null;

			CWidget.g_widgetMentatScrollUp = null; //free(g_widgetMentatScrollUp);
			CWidget.g_widgetMentatScrollDown = null; //free(g_widgetMentatScrollDown);

			/* ENHANCEMENT -- After visiting Mentat (the help) window, auto-repeat of keys gets disabled. */
			if (!CSharpDune.g_dune2_enhanced) Input.Input_Flags_ClearBits((ushort)InputFlagsEnum.INPUT_FLAG_KEY_REPEAT);

			Gfx.GFX_Screen_SetActive(oldScreenID);
		}

		static string empty = string.Empty; //char[2]
		/* Create the widgets of the mentat help screen. */
		static void GUI_Mentat_Create_HelpScreen_Widgets()
		{
			ushort ypos;
			Widget[] w;
			int i;

			if (CWidget.g_widgetMentatScrollbar != null)
			{
				CWidget.GUI_Widget_Free_WithScrollbar(CWidget.g_widgetMentatScrollbar);
				CWidget.g_widgetMentatScrollbar = null;
			}

			CWidget.g_widgetMentatScrollUp = null; //free(g_widgetMentatScrollUp);
			CWidget.g_widgetMentatScrollDown = null; //free(g_widgetMentatScrollDown);

			CWidget.g_widgetMentatTail = null;
			ypos = 8;

			//w = (Widget[])Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_2);

			w = new Widget[13];

			for (i = 0; i < 13; i++)
			{
				w[i] = new Widget(); //memset(w, 0, 13 * sizeof(Widget));

				w[i].index = (ushort)(i + 2);

				//memset(&w->flags, 0, sizeof(w->flags));
				w[i].flags.buttonFilterLeft = 9;
				w[i].flags.buttonFilterRight = 1;

				w[i].clickProc = GUI_Mentat_List_Click;

				w[i].drawParameterDown.text = empty;
				w[i].drawParameterSelected.text = empty;
				w[i].drawParameterNormal.text = empty;

				w[i].drawModeNormal = (byte)DrawMode.DRAW_MODE_TEXT;

				//memset(&w->state, 0, sizeof(w->state));

				w[i].offsetX = 24;
				w[i].offsetY = (short)ypos;
				w[i].width = 0x88;
				w[i].height = 8;
				w[i].parentID = 8;

				if (CWidget.g_widgetMentatTail != null)
				{
					CWidget.g_widgetMentatTail = CWidget.GUI_Widget_Link(CWidget.g_widgetMentatTail, w[i]);
				}
				else
				{
					CWidget.g_widgetMentatTail = w[i];
				}

				ypos += 8;
				//w++;
			}

			CWidget.GUI_Widget_MakeInvisible(CWidget.g_widgetMentatTail);
			CWidget.GUI_Widget_MakeInvisible(w[i - 1]);

			CWidget.g_widgetMentatScrollbar = CWidget.GUI_Widget_Allocate_WithScrollbar(15, 8, 168, 24, 8, 72, GUI_Mentat_ScrollBar_Draw);

			CWidget.g_widgetMentatTail = CWidget.GUI_Widget_Link(CWidget.g_widgetMentatTail, CWidget.g_widgetMentatScrollbar);

			CWidget.g_widgetMentatScrollDown = CWidget.GUI_Widget_AllocateScrollBtn(16, 0, 168, 96, Sprites.g_sprites[385], Sprites.g_sprites[386], CWidget.GUI_Widget_Get_ByIndex(CWidget.g_widgetMentatTail, 15), true);
			CWidget.g_widgetMentatScrollDown.shortcut = 0;
			CWidget.g_widgetMentatScrollDown.shortcut2 = 0;
			CWidget.g_widgetMentatScrollDown.parentID = 8;
			CWidget.g_widgetMentatTail = CWidget.GUI_Widget_Link(CWidget.g_widgetMentatTail, CWidget.g_widgetMentatScrollDown);

			CWidget.g_widgetMentatScrollUp = CWidget.GUI_Widget_AllocateScrollBtn(17, 0, 168, 16, Sprites.g_sprites[383], Sprites.g_sprites[384], CWidget.GUI_Widget_Get_ByIndex(CWidget.g_widgetMentatTail, 15), false);
			CWidget.g_widgetMentatScrollUp.shortcut = 0;
			CWidget.g_widgetMentatScrollUp.shortcut2 = 0;
			CWidget.g_widgetMentatScrollUp.parentID = 8;
			CWidget.g_widgetMentatTail = CWidget.GUI_Widget_Link(CWidget.g_widgetMentatTail, CWidget.g_widgetMentatScrollUp);

			CWidget.g_widgetMentatTail = CWidget.GUI_Widget_Link(CWidget.g_widgetMentatTail, CWidget.g_widgetMentatFirst);

			CWidget.GUI_Widget_Draw(CWidget.g_widgetMentatFirst);
		}

		/*
		 * Handles Click event for list in mentat window.
		 *
		 * @param w The widget.
		 */
		static bool GUI_Mentat_List_Click(Widget w)
		{
			ushort index;
			Widget w2;

			index = (ushort)(s_selectedHelpSubject + 3);

			if (w.index != index)
			{
				w2 = CWidget.GUI_Widget_Get_ByIndex(CWidget.g_widgetMentatTail, index);

				CWidget.GUI_Widget_MakeNormal(w, false);
				CWidget.GUI_Widget_MakeNormal(w2, false);

				if (w2.stringID == 0x31)
				{
					w2.fgColourDown = 15;
					w2.fgColourNormal = 15;

					CWidget.GUI_Widget_Draw(w2);
				}

				if (w.stringID == 0x31)
				{
					w.fgColourDown = 8;
					w.fgColourNormal = 8;

					CWidget.GUI_Widget_Draw(w);
				}

				s_selectedHelpSubject = (ushort)(w.index - 3);
				return true;
			}

			if ((w.state.buttonState & 0x11) == 0 && !s_selectMentatHelp) return true;

			if (w.stringID != 0x31) return true;

			CWidget.GUI_Widget_MakeNormal(w, false);

			GUI_Mentat_ShowHelp();

			GUI_Mentat_Draw(true);

			Input.Input_HandleInput(0x841);
			Input.Input_HandleInput(0x842);
			return false;
		}

		static ushort displayedHelpSubject = 0;
		static void GUI_Mentat_Draw(bool force)
		{
			Screen oldScreenID;
			Widget line;
			Widget w = CWidget.g_widgetMentatTail;
			byte[] helpSubjects = s_helpSubjects;
			ushort i;
			string text;
			int helpSubjectsPointer = s_helpSubjectsPointer;

			if (!force && s_topHelpList == displayedHelpSubject) return;

			displayedHelpSubject = s_topHelpList;

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.SCREEN_1);

			CWidget.Widget_SetAndPaintCurrentWidget(8);

			Gui.GUI_DrawSprite(Screen.SCREEN_1, Sprites.g_sprites[397 + (byte)CHouse.g_playerHouseID * 15], g_shoulderLeft, g_shoulderTop, 0, 0);

			Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_SELECT_SUBJECT), (short)((CWidget.g_curWidgetXBase << 3) + 16), (short)(CWidget.g_curWidgetYBase + 2), 12, 0, 0x12);
			Gui.GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x11);

			line = CWidget.GUI_Widget_Get_ByIndex(w, 3);
			for (i = 0; i < 11; i++)
			{
				text = CSharpDune.Encoding.GetString(helpSubjects[(helpSubjectsPointer + 7)..]);
				text = text[..text.IndexOf('\0')];
				line.drawParameterDown.text = text;
				line.drawParameterSelected.text = text;
				line.drawParameterNormal.text = text;

				if (helpSubjects[helpSubjectsPointer + 6] == '0')
				{
					line.offsetX = 16;
					line.fgColourSelected = 11;
					line.fgColourDown = 11;
					line.fgColourNormal = 11;
					line.stringID = 0x30;
				}
				else
				{
					byte colour = (byte)((i == s_selectedHelpSubject) ? 8 : 15);
					line.offsetX = 24;
					line.fgColourSelected = colour;
					line.fgColourDown = colour;
					line.fgColourNormal = colour;
					line.stringID = 0x31;
				}

				CWidget.GUI_Widget_MakeNormal(line, false);
				CWidget.GUI_Widget_Draw(line);

				line = CWidget.GUI_Widget_GetNext(line);
				helpSubjectsPointer = CString.String_NextString(helpSubjects, helpSubjectsPointer);
			}

			CWidget.GUI_Widget_Scrollbar_Init(CWidget.GUI_Widget_Get_ByIndex(w, 15), (short)s_numberHelpSubjects, 11, (short)s_topHelpList);

			CWidget.GUI_Widget_Draw(CWidget.GUI_Widget_Get_ByIndex(w, 16));
			CWidget.GUI_Widget_Draw(CWidget.GUI_Widget_Get_ByIndex(w, 17));

			Gui.GUI_Mouse_Hide_Safe();
			Gui.GUI_Screen_Copy((short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetWidth, (short)CWidget.g_curWidgetHeight, Screen.SCREEN_1, Screen.SCREEN_0);
			Gui.GUI_Mouse_Show_Safe();
			Gfx.GFX_Screen_SetActive(oldScreenID);
		}

		static void GUI_Mentat_HelpListLoop()
		{
			ushort key;

			for (key = 0; key != 0x8001; Sleep.sleepIdle())
			{
				Widget w = CWidget.g_widgetMentatTail;

				GUI_Mentat_Animation(0);

				key = CWidget.GUI_Widget_HandleEvents(w);

				if ((key & 0x800) != 0) key = 0;

				if (key == 0x8001) break;

				key &= 0x80FF;

				s_selectMentatHelp = true;

				switch (key)
				{
					case 0x0053:
					case 0x0060: /* NUMPAD 8 / ARROW UP */
					case 0x0453:
					case 0x0460:
						if (s_selectedHelpSubject != 0)
						{
							GUI_Mentat_List_Click(CWidget.GUI_Widget_Get_ByIndex(w, (ushort)(s_selectedHelpSubject + 2)));
							break;
						}

						WidgetClick.GUI_Widget_Scrollbar_ArrowUp_Click(CWidget.g_widgetMentatScrollbar);
						break;

					case 0x0054:
					case 0x0062: /* NUMPAD 2 / ARROW DOWN */
					case 0x0454:
					case 0x0462:
						if (s_selectedHelpSubject < 10)
						{
							GUI_Mentat_List_Click(CWidget.GUI_Widget_Get_ByIndex(w, (ushort)(s_selectedHelpSubject + 4)));
							break;
						}

						WidgetClick.GUI_Widget_Scrollbar_ArrowDown_Click(CWidget.g_widgetMentatScrollbar);
						break;

					case 0x0055:
					case 0x0065: /* NUMPAD 9 / PAGE UP */
					case 0x0455:
					case 0x0465:
						{
							byte i;
							for (i = 0; i < 11; i++) WidgetClick.GUI_Widget_Scrollbar_ArrowUp_Click(CWidget.g_widgetMentatScrollbar);
						}
						break;

					case 0x0056:
					case 0x0067: /* NUMPAD 3 / PAGE DOWN */
					case 0x0456:
					case 0x0467:
						{
							byte i;
							for (i = 0; i < 11; i++) WidgetClick.GUI_Widget_Scrollbar_ArrowDown_Click(CWidget.g_widgetMentatScrollbar);
						}
						break;

					case 0x0041: /* MOUSE LEFT BUTTON */
						break;

					case 0x002B: /* NUMPAD 5 / RETURN */
					case 0x003D: /* SPACE */
					case 0x042B:
					case 0x043D:
						GUI_Mentat_List_Click(CWidget.GUI_Widget_Get_ByIndex(w, (ushort)(s_selectedHelpSubject + 3)));
						break;

					default: break;
				}

				s_selectMentatHelp = false;
			}
		}

		static byte[] helpDataList = null;
		static void GUI_Mentat_LoadHelpSubjects(bool init)
		{
			byte fileID;
			uint length;
			uint counter;
			byte[] helpSubjects;
			ushort i;
			int helpSubjectsPointer = 0;

			if (init)
			{
				helpDataList = (byte[])Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_1);

				s_topHelpList = 0;
				s_selectedHelpSubject = 0;

				//sprintf(s_mentatFilename, "MENTAT%c", g_table_houseInfo[g_playerHouseID].name[0]);
				s_mentatFilename = $"MENTAT{CHouse.g_table_houseInfo[(int)CHouse.g_playerHouseID].name[0]}";
				//strncpy(s_mentatFilename, String_GenerateFilename(s_mentatFilename), sizeof(s_mentatFilename));
				s_mentatFilename = CString.String_GenerateFilename(s_mentatFilename);
			}

			fileID = CFile.ChunkFile_Open(s_mentatFilename);
			length = CFile.ChunkFile_Read(fileID, Endian.HTOBE32((uint)CSharpDune.MultiChar[FourCC.NAME]), ref helpDataList, Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_1));
			CFile.ChunkFile_Close(fileID);

			s_numberHelpSubjects = 0;

			helpSubjects = helpDataList;
			//helpSubjectsPointer = 0;

			counter = 0;
			while (counter < length)
			{
				byte size = helpSubjects[helpSubjectsPointer];

				counter += size;

				if (helpSubjects[helpSubjectsPointer + size - 1] > CSharpDune.g_campaignID + 1)
				{
					while (size-- != 0) helpSubjects[helpSubjectsPointer++] = (byte)'\0';
					continue;
				}

				helpSubjects[helpSubjectsPointer + size - 1] = size;
				helpSubjectsPointer += size;
				s_numberHelpSubjects++;
			}

			//helpSubjects = helpDataList;
			helpSubjectsPointer = 0;

			while (helpSubjects[helpSubjectsPointer] == '\0') helpSubjectsPointer++;

			for (i = 0; i < s_topHelpList; i++) helpSubjectsPointer = CString.String_NextString(helpSubjects, helpSubjectsPointer);

			s_helpSubjects = helpSubjects;
			s_helpSubjectsPointer = helpSubjectsPointer;
		}

		static void GUI_Mentat_ScrollBar_Draw(Widget w)
		{
			GUI_Mentat_SelectHelpSubject((short)(CWidget.GUI_Get_Scrollbar_Position(w) - s_topHelpList));
			GUI_Mentat_Draw(false);
		}

		/*
		 * Select a new subject, move the list of help subjects displayed, if necessary.
		 * @param difference Number of subjects to jump.
		 */
		static void GUI_Mentat_SelectHelpSubject(short difference)
		{
			if (difference > 0)
			{
				if (difference + s_topHelpList + 11 > s_numberHelpSubjects)
				{
					difference = (short)(s_numberHelpSubjects - (s_topHelpList + 11));
				}
				s_topHelpList += (ushort)difference;

				while (difference-- != 0)
				{
					s_helpSubjectsPointer = CString.String_NextString(s_helpSubjects, s_helpSubjectsPointer);
				}
				return;
			}

			if (difference < 0)
			{
				difference = (short)-difference;

				if ((short)s_topHelpList < difference)
				{
					difference = (short)s_topHelpList;
				}

				s_topHelpList -= (ushort)difference;

				while (difference-- != 0)
				{
					s_helpSubjectsPointer = CString.String_PrevString(s_helpSubjects, s_helpSubjectsPointer);
				}
				return;
			}
		}

		static void GUI_Mentat_ShowHelp()
		{
			byte[] subject;
			ushort i;
			bool noDesc;
			byte fileID;
			uint offset;
			byte[] compressedText; //char *
			string desc; //char *
			string picture; //char *
			string text; //char *
			bool loopAnimation;
			MentatInfo info;
			int subjectPointer;
			int textPointer = 0;

			info = new MentatInfo();
			subject = s_helpSubjects;
			subjectPointer = s_helpSubjectsPointer;

			for (i = 0; i < s_selectedHelpSubject; i++) subjectPointer = CString.String_NextString(subject, subjectPointer);

			noDesc = (subject[subjectPointer + 5] == '0');  /* or no WSA file ? */
			offset = Endian.HTOBE32(Endian.READ_LE_UINT32(subject[(subjectPointer + 1)..]));

			fileID = CFile.ChunkFile_Open(s_mentatFilename);
			CFile.ChunkFile_Read(fileID, Endian.HTOBE32((uint)CSharpDune.MultiChar[FourCC.INFO]), ref info, 12);
			CFile.ChunkFile_Close(fileID);

			info.length = Endian.HTOBE32(info.length);

			text = null; //CSharpDune.Encoding.GetString(CSharpDune.g_readBuffer);
			compressedText = new byte[info.length]; //(byte[])Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_1);

			fileID = CFile.File_Open(s_mentatFilename, FileMode.FILE_MODE_READ);
			CFile.File_Seek(fileID, (int)offset, 0);
			CFile.File_Read(fileID, ref compressedText, info.length);
			CString.String_Decompress(CSharpDune.Encoding.GetString(compressedText), ref text, (ushort)CSharpDune.g_readBufferSize);
			text = CString.String_TranslateSpecial(text);
			CFile.File_Close(fileID);

			//TODO: Remove textPointer
			/* skip WSA file name (or string index) */
			while (text[textPointer] != '*' && text[textPointer] != '?') textPointer++;

			loopAnimation = (text[textPointer] == '*');

			textPointer++; //*text++ = '\0';

			if (noDesc)
			{
				picture = CScenario.g_scenario.pictureBriefing;
				desc = null;

				textPointer = 0;
				var index = (ushort)(text[textPointer] - 44 + CSharpDune.g_campaignID * 4 + 405 + (byte)CHouse.g_playerHouseID * 40);
				text = $"{CString.String_Get_ByIndex(index)}\0"; //strncpy(g_readBuffer, String_Get_ByIndex(index), g_readBufferSize);
			}
			else
			{
				picture = text[..text.IndexOf('*')];
				desc = text[textPointer..text.IndexOf('\f')];

				while (text[textPointer] != '\0' && text[textPointer] != 0xC) textPointer++;
				if (text[textPointer] != '\0') textPointer++; //*text++ = '\0';
			}

			GUI_Mentat_Loop(picture, desc, text[textPointer..text.IndexOf('\0')], loopAnimation, CWidget.g_widgetMentatFirst);

			CWidget.GUI_Widget_MakeNormal(CWidget.g_widgetMentatFirst, false);

			GUI_Mentat_LoadHelpSubjects(false);

			GUI_Mentat_Create_HelpScreen_Widgets();

			GUI_Mentat_Draw(true);
		}
	}
}
