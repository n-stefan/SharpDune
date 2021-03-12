/* Widget clicking handling */

using System;
using System.Diagnostics;
using static System.Math;

namespace SharpDune
{
	class WidgetClick
	{
		static ushort s_savegameIndexBase = 0;
		static ushort s_savegameCountOnDisk = 0;                    /*!< Amount of savegames on disk. */

		internal static string[] g_savegameDesc; //[5][51]			/*!< Array of savegame descriptions for the SaveLoad window. */

        static WidgetClick()
        {
			g_savegameDesc = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
		}

		/*
		 * Handles Click event for "Save Game" or "Load Game" button.
		 *
		 * @param save Wether to save or load.
		 * @return True if a game has been saved or loaded, False otherwise.
		 */
		internal static bool GUI_Widget_SaveLoad_Click(bool save)
		{
			var desc = CWindowDesc.g_saveLoadWindowDesc;
			bool loop;

			s_savegameCountOnDisk = GetSavegameCount();

			s_savegameIndexBase = (ushort)Max(0, s_savegameCountOnDisk - (save ? 0 : 1));

			FillSavegameDesc(save);

			desc.stringID = (short)(save ? Text.STR_SELECT_A_POSITION_TO_SAVE_TO : Text.STR_SELECT_A_SAVED_GAME_TO_LOAD);

			GUI_Window_BackupScreen(desc);

			GUI_Window_Create(desc);

			UpdateArrows(save, true);

			for (loop = true; loop; Sleep.sleepIdle())
			{
				var w = CWidget.g_widgetLinkedListTail;
				var key = CWidget.GUI_Widget_HandleEvents(w);

				UpdateArrows(save, false);

				if ((key & 0x8000) != 0)
				{
					Widget w2;

					key &= 0x7FFF;
					w2 = CWidget.GUI_Widget_Get_ByIndex(w, key);

					switch (key)
					{
						case 0x25:
							s_savegameIndexBase = (ushort)Min(s_savegameCountOnDisk - (save ? 0 : 1), s_savegameIndexBase + 1);

							FillSavegameDesc(save);

							WidgetDraw.GUI_Widget_DrawAll(w);
							break;

						case 0x26:
							s_savegameIndexBase = (ushort)Max(0, s_savegameIndexBase - 1);

							FillSavegameDesc(save);

							WidgetDraw.GUI_Widget_DrawAll(w);
							break;

						case 0x23:
							loop = false;
							break;

						default:
							{
								GUI_Window_RestoreScreen(desc);

								key -= 0x1E;

								if (!save)
								{
									return Load.SaveGame_LoadFile(GenerateSavegameFilename((ushort)(s_savegameIndexBase - key)));
								}

								if (GUI_Widget_Savegame_Click(key)) return true;

								GUI_Window_BackupScreen(desc);

								UpdateArrows(save, true);

								GUI_Window_Create(desc);

								UpdateArrows(save, true);
							}
							break;
					}

					CWidget.GUI_Widget_MakeNormal(w2, false);
				}

				Gui.GUI_PaletteAnimate();
			}

			GUI_Window_RestoreScreen(desc);

			return false;
		}

		/*
		 * Handles Click event for unit commands button.
		 *
		 * @param w The widget.
		 * @return True, always.
		 */
		internal static bool GUI_Widget_TextButton_Click(Widget w)
		{
			UnitInfo ui;
			ActionInfo ai;
			ushort[] actions;
			ActionType action;
			Unit u;
			//ushort found;
			ActionType unitAction;

			u = CUnit.g_unitSelected;
			ui = CUnit.g_table_unitInfo[u.o.type];

			actions = ui.o.actionsPlayer;
			if (CUnit.Unit_GetHouseID(u) != (byte)CHouse.g_playerHouseID && u.o.type != (byte)UnitType.UNIT_HARVESTER)
			{
				actions = CUnit.g_table_actionsAI;
			}

			action = (ActionType)actions[w.index - 8];
			if (CSharpDune.g_dune2_enhanced)
			{
				if (Input.Input_Test(0x2c) != 0 || Input.Input_Test(0x39) != 0)
				{   /* LSHIFT or RSHIFT is pressed */
					if (action == ActionType.ACTION_GUARD) action = ActionType.ACTION_AREA_GUARD;   /* AREA GUARD instead of GUARD */
					else if (action == ActionType.ACTION_ATTACK) action = ActionType.ACTION_AMBUSH; /* AMBUSH instead of ATTACK */
				}
				Debug.WriteLine($"DEBUG: GUI_Widget_TextButton_Click({w} index={w.index}) action={action}");
			}

			unitAction = (ActionType)u.nextActionID;
			if (unitAction == ActionType.ACTION_INVALID)
			{
				unitAction = (ActionType)u.actionID;
			}

			if (u.deviated != 0)
			{
				CUnit.Unit_Deviation_Decrease(u, 5);
				if (u.deviated == 0)
				{
					CWidget.GUI_Widget_MakeNormal(w, false);
					return true;
				}
			}

			CWidget.GUI_Widget_MakeSelected(w, false);

			ai = CUnit.g_table_actionInfo[(int)action];

			if (ai.selectionType != CSharpDune.g_selectionType)
			{
				CUnit.g_unitActive = CUnit.g_unitSelected;
				CSharpDune.g_activeAction = (ushort)action;
				Gui.GUI_ChangeSelectionType(ai.selectionType);

				return true;
			}

			CObject.Object_Script_Variable4_Clear(u.o);
			u.targetAttack = 0;
			u.targetMove = 0;
			u.route[0] = 0xFF;

			CUnit.Unit_SetAction(u, action);

			if (ui.movementType == (ushort)MovementType.MOVEMENT_FOOT) Sound.Sound_StartSound(ai.soundID);

			if (unitAction == action) return true;

			var index = Array.FindIndex(actions[..4], a => a == (ushort)unitAction);
			if (index == -1) return true;
			//found = memchr(actions, unitAction, 4);
			//if (found == NULL) return true;

			CWidget.GUI_Widget_MakeNormal(CWidget.GUI_Widget_Get_ByIndex(CWidget.g_widgetLinkedListHead, (ushort)(/*found - actions + 8*/index + 8)), false);

			return true;
		}

		static ushort previousIndex = 0;
		static void UpdateArrows(bool save, bool force)
		{
			Widget w;

			if (!force && s_savegameIndexBase == previousIndex) return;

			previousIndex = s_savegameIndexBase;

			w = CWidget.g_table_windowWidgets[8];
			if (s_savegameIndexBase >= 5)
			{
				CWidget.GUI_Widget_MakeVisible(w);
			}
			else
			{
				CWidget.GUI_Widget_MakeInvisible(w);
				GUI_Widget_Undraw(w, 233);
			}

			w = CWidget.g_table_windowWidgets[7];
			if (s_savegameCountOnDisk - (save ? 0 : 1) > s_savegameIndexBase)
			{
				CWidget.GUI_Widget_MakeVisible(w);
			}
			else
			{
				CWidget.GUI_Widget_MakeInvisible(w);
				GUI_Widget_Undraw(w, 233);
			}
		}

		static string filename; //char[13]
		static string GenerateSavegameFilename(ushort number)
		{
			filename = $"_save{number:D3}.dat"; //sprintf(filename, "_save%03d.dat", number);
			return filename;
		}

		static void FillSavegameDesc(bool save)
		{
			byte i;

			for (i = 0; i < 5; i++)
			{
				//string desc = g_savegameDesc[i]; //char*
				string filename; //char*
				byte fileId;

				g_savegameDesc[i] = string.Empty; //*desc = '\0';

				if (s_savegameIndexBase - i < 0) continue;

				if (s_savegameIndexBase - i == s_savegameCountOnDisk)
				{
					if (!save) continue;

					/*desc*/g_savegameDesc[i] = CString.String_Get_ByIndex(Text.STR_EMPTY_SLOT_); //strncpy(desc, String_Get_ByIndex(STR_EMPTY_SLOT_), 50);
					continue;
				}

				filename = GenerateSavegameFilename((ushort)(s_savegameIndexBase - i));

				fileId = CFile.ChunkFile_Open_Personal(filename);
				if (fileId == (byte)FileMode.FILE_INVALID) continue;
				CFile.ChunkFile_Read(fileId, Endian.HTOBE32((uint)CSharpDune.MultiChar[FourCC.NAME]), ref /*desc*/g_savegameDesc[i], 50);
				g_savegameDesc[i] = g_savegameDesc[i][..^1];
				CFile.ChunkFile_Close(fileId);
				continue;
			}
		}

		static void GUI_Widget_Undraw(Widget w, byte colour)
		{
			ushort offsetX;
			ushort offsetY;
			ushort width;
			ushort height;

			if (w == null) return;

			offsetX = (ushort)(w.offsetX + (CWidget.g_widgetProperties[w.parentID].xBase << 3));
			offsetY = (ushort)(w.offsetY + CWidget.g_widgetProperties[w.parentID].yBase);
			width = w.width;
			height = w.height;

			if (Gfx.GFX_Screen_IsActive(Screen.SCREEN_0))
			{
				Gui.GUI_Mouse_Hide_InRegion(offsetX, offsetY, (ushort)(offsetX + width), (ushort)(offsetY + height));
			}

			Gui.GUI_DrawFilledRectangle((short)offsetX, (short)offsetY, (short)(offsetX + width), (short)(offsetY + height), colour);

			if (Gfx.GFX_Screen_IsActive(Screen.SCREEN_0))
			{
				Gui.GUI_Mouse_Show_InRegion();
			}
		}

		static void GUI_Window_BackupScreen(WindowDesc desc)
		{
			CWidget.Widget_SetCurrentWidget(desc.index);

			Gui.GUI_Mouse_Hide_Safe();
			Gfx.GFX_CopyToBuffer((short)(CWidget.g_curWidgetXBase * 8), (short)CWidget.g_curWidgetYBase, (ushort)(CWidget.g_curWidgetWidth * 8), CWidget.g_curWidgetHeight, (byte[])Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_2));
			Gui.GUI_Mouse_Show_Safe();
		}

		static void GUI_Window_RestoreScreen(WindowDesc desc)
		{
			CWidget.Widget_SetCurrentWidget(desc.index);

			Gui.GUI_Mouse_Hide_Safe();
			Gfx.GFX_CopyFromBuffer((short)(CWidget.g_curWidgetXBase * 8), (short)CWidget.g_curWidgetYBase, (ushort)(CWidget.g_curWidgetWidth * 8), CWidget.g_curWidgetHeight, (byte[])Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_2));
			Gui.GUI_Mouse_Show_Safe();
		}

		/*
		 * Handles Click event for "Clear List" button.
		 *
		 * @param w The widget.
		 * @return True, always.
		 */
		internal static bool GUI_Widget_HOF_ClearList_Click(Widget w)
		{
			/* "Are you sure you want to clear the high scores?" */
			if (GUI_YesNo((ushort)Text.STR_ARE_YOU_SURE_YOU_WANT_TO_CLEAR_THE_HIGH_SCORES))
			{
				var data = (HallOfFameStruct[])w.data;

				for (var i = 0; i < data.Length; i++) data[i] = new HallOfFameStruct(); //memset(data, 0, 128);

				if (CFile.File_Exists_Personal("SAVEFAME.DAT")) CFile.File_Delete_Personal("SAVEFAME.DAT");

				Gui.GUI_HallOfFame_DrawData(data, true);

				Gui.g_doQuitHOF = true;
			}

			CWidget.GUI_Widget_MakeNormal(w, false);

			return true;
		}

		/*
		 * Handles Click event for "Resume Game" button.
		 *
		 * @return True, always.
		 */
		internal static bool GUI_Widget_HOF_Resume_Click(Widget w)
		{
			//VARIABLE_NOT_USED(w);

			Gui.g_doQuitHOF = true;

			return true;
		}

		/*
		 * Handles Click event for "Cancel" button.
		 *
		 * @return True, always.
		 */
		internal static bool GUI_Widget_Cancel_Click(Widget w)
		{
			//VARIABLE_NOT_USED(w);

			if (CStructure.g_structureActiveType != 0xFFFF)
			{
				var s = CStructure.Structure_Get_ByPackedTile(CStructure.g_structureActivePosition);
				var s2 = CStructure.g_structureActive;

				Debug.Assert(s2 != null);

				if (s != null)
				{
					s.o.linkedID = (byte)(s2.o.index & 0xFF);
				}
				else
				{
					CStructure.Structure_Free(s2);
				}

				CStructure.g_structureActive = null;
				CStructure.g_structureActiveType = 0xFFFF;

				Gui.GUI_ChangeSelectionType((ushort)SelectionType.SELECTIONTYPE_STRUCTURE);

				Gui.g_selectionState = 0; /* Invalid. */
			}

			if (CUnit.g_unitActive == null) return true;

			CUnit.g_unitActive = null;
			CSharpDune.g_activeAction = 0xFFFF;
			Gui.g_cursorSpriteID = 0;

			Sprites.Sprites_SetMouseSprite(0, 0, Sprites.g_sprites[0]);

			Gui.GUI_ChangeSelectionType((ushort)SelectionType.SELECTIONTYPE_UNIT);

			return true;
		}

		/*
		 * Handles Click event for a sprite/text button.
		 *
		 * @param w The widget.
		 * @return False, always.
		 */
		internal static bool GUI_Widget_SpriteTextButton_Click(Widget w)
		{
			Structure s;

			//VARIABLE_NOT_USED(w);

			s = CStructure.Structure_Get_ByPackedTile(Gui.g_selectionPosition);

			switch ((Text)Gui.g_productionStringID)
			{
				default: break;

				case Text.STR_PLACE_IT:
					if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
					{
						Structure ns;

						ns = CStructure.Structure_Get_ByIndex(s.o.linkedID);
						CStructure.g_structureActive = ns;
						CStructure.g_structureActiveType = s.objectType;
						Gui.g_selectionState = CStructure.Structure_IsValidBuildLocation(Gui.g_selectionRectanglePosition, (StructureType)CStructure.g_structureActiveType);
						CStructure.g_structureActivePosition = Gui.g_selectionPosition;
						s.o.linkedID = (byte)StructureType.STRUCTURE_INVALID;

						Gui.GUI_ChangeSelectionType((ushort)SelectionType.SELECTIONTYPE_PLACE);
					}
					break;

				case Text.STR_ON_HOLD:
					s.o.flags.repairing = false;
					s.o.flags.onHold = false;
					s.o.flags.upgrading = false;
					break;

				case Text.STR_BUILD_IT:
					CStructure.Structure_BuildObject(s, s.objectType);
					break;

				case Text.STR_LAUNCH:
				case Text.STR_FREMEN:
				case Text.STR_SABOTEUR:
					CStructure.Structure_ActivateSpecial(s);
					break;

				case Text.STR_D_DONE:
					s.o.flags.onHold = true;
					break;
			}
			return false;
		}

		/*
		 * Handles Click event for current selection name.
		 *
		 * @return False, always.
		 */
		internal static bool GUI_Widget_Name_Click(Widget w)
		{
			Object o;
			ushort packed;

			//VARIABLE_NOT_USED(w);

			o = CObject.Object_GetByPackedTile(Gui.g_selectionPosition);

			if (o == null) return false;

			packed = CTile.Tile_PackTile(o.position);

			Map.Map_SetViewportPosition(packed);
			Map.Map_SetSelection(packed);

			return false;
		}

		/*
		 * Handles Click event for current selection picture.
		 *
		 * @return False, always.
		 */
		internal static bool GUI_Widget_Picture_Click(Widget w)
		{
			Structure s;

			//VARIABLE_NOT_USED(w);

			if (CUnit.g_unitSelected != null)
			{
				CUnit.Unit_DisplayStatusText(CUnit.g_unitSelected);

				return false;
			}

			s = CStructure.Structure_Get_ByPackedTile(Gui.g_selectionPosition);

			if (s == null || !CStructure.g_table_structureInfo[s.o.type].o.flags.factory) return false;

			CStructure.Structure_BuildObject(s, 0xFFFF);

			return false;
		}

		/*
		 * Handles Click event for "Repair/Upgrade" button.
		 *
		 * @param w The widget.
		 * @return False, always.
		 */
		internal static bool GUI_Widget_RepairUpgrade_Click(Widget w)
		{
			Structure s;

			s = CStructure.Structure_Get_ByPackedTile(Gui.g_selectionPosition);

			if (CStructure.Structure_SetRepairingState(s, -1, w)) return false;
			CStructure.Structure_SetUpgradingState(s, -1, w);

			return false;
		}

		/*
		 * Handles Click event for savegame button.
		 *
		 * @param index The index of the clicked button.
		 * @return True if a game has been saved, False otherwise.
		 */
		static bool GUI_Widget_Savegame_Click(ushort index)
		{
			var desc = CWindowDesc.g_savegameNameWindowDesc;
			bool loop;
			//string saveDesc = g_savegameDesc[index];
			bool widgetPaint;
			bool ret;

			if (/*saveDesc[0] == '['*/g_savegameDesc[index].StartsWith('[')) /*saveDesc*/g_savegameDesc[index] = string.Empty;

			GUI_Window_BackupScreen(desc);

			GUI_Window_Create(desc);

			ret = false;
			widgetPaint = true;

			if (/*saveDesc[0] == '['*/g_savegameDesc[index].StartsWith('[')) index = s_savegameCountOnDisk;

			Gfx.GFX_Screen_SetActive(Screen.SCREEN_0);

			CWidget.Widget_SetCurrentWidget(15);

			Gui.GUI_Mouse_Hide_Safe();
			Gui.GUI_DrawBorder((ushort)((CWidget.g_curWidgetXBase << 3) - 1), (ushort)(CWidget.g_curWidgetYBase - 1), (ushort)((CWidget.g_curWidgetWidth << 3) + 2), (ushort)(CWidget.g_curWidgetHeight + 2), 4, false);
			Gui.GUI_Mouse_Show_Safe();

			for (loop = true; loop; Sleep.sleepIdle())
			{
				ushort eventKey;
				var w = CWidget.g_widgetLinkedListTail;

				Gui.GUI_DrawText_Wrapper(null, 0, 0, 232, 235, 0x22);

				eventKey = EditBox.GUI_EditBox(ref /*saveDesc*/g_savegameDesc[index], 50, 15, CWidget.g_widgetLinkedListTail, null, widgetPaint);
				widgetPaint = false;

				if ((eventKey & 0x8000) == 0) continue;

				CWidget.GUI_Widget_MakeNormal(CWidget.GUI_Widget_Get_ByIndex(w, (ushort)(eventKey & 0x7FFF)), false);

				switch (eventKey & 0x7FFF)
				{
					case 0x1E:  /* RETURN / Save Button */
						if (/*saveDesc == 0*/g_savegameDesc[index] == string.Empty) break;

						Save.SaveGame_SaveFile(GenerateSavegameFilename((ushort)(s_savegameIndexBase - index)), /*saveDesc*/g_savegameDesc[index]);
						loop = false;
						ret = true;
						break;

					case 0x1F:  /* ESCAPE / Cancel Button */
						loop = false;
						ret = false;
						FillSavegameDesc(true);
						break;

					default: break;
				}
			}

			GUI_Window_RestoreScreen(desc);

			return ret;
		}

		static bool GUI_YesNo(ushort stringID)
		{
			var desc = CWindowDesc.g_yesNoWindowDesc;
			bool loop;
			var ret = false;

			desc.stringID = (short)stringID;

			GUI_Window_BackupScreen(desc);

			GUI_Window_Create(desc);

			for (loop = true; loop; Sleep.sleepIdle())
			{
				var key = CWidget.GUI_Widget_HandleEvents(CWidget.g_widgetLinkedListTail);

				if ((key & 0x8000) != 0)
				{
					switch (key & 0x7FFF)
					{
						case 0x1E: ret = true; break;
						case 0x1F: ret = false; break;
						default: break;
					}
					loop = false;
				}

				Gui.GUI_PaletteAnimate();
			}

			GUI_Window_RestoreScreen(desc);

			return ret;
		}

		static ushort GetSavegameCount()
		{
			ushort i;

			for (i = 0; ; i++)
			{
				if (!CFile.File_Exists_Personal(GenerateSavegameFilename(i))) return i;
			}
		}

		static void GUI_Window_Create(WindowDesc desc)
		{
			byte i;

			if (desc == null) return;

			CWidget.g_widgetLinkedListTail = null;

			Gfx.GFX_Screen_SetActive(Screen.SCREEN_1);

			CWidget.Widget_SetCurrentWidget(desc.index);

			WidgetDraw.GUI_Widget_DrawBorder(CWidget.g_curWidgetIndex, 2, true);

			if (Gui.GUI_String_Get_ByIndex(desc.stringID) != null)
			{
				Gui.GUI_DrawText_Wrapper(Gui.GUI_String_Get_ByIndex(desc.stringID), (short)((CWidget.g_curWidgetXBase << 3) + (CWidget.g_curWidgetWidth << 2)), (short)(CWidget.g_curWidgetYBase + 6 + ((desc == CWindowDesc.g_yesNoWindowDesc) ? 2 : 0)), 238, 0, 0x122);
			}

			if (Gui.GUI_String_Get_ByIndex((short)desc.widgets[0].stringID) == null)
			{
				Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_THERE_ARE_NO_SAVED_GAMES_TO_LOAD), (short)((CWidget.g_curWidgetXBase + 2) << 3), (short)(CWidget.g_curWidgetYBase + 42), 232, 0, 0x22);
			}

			for (i = 0; i < desc.widgetCount; i++)
			{
				var w = CWidget.g_table_windowWidgets[i];

				if (Gui.GUI_String_Get_ByIndex((short)desc.widgets[i].stringID) == null) continue;

				w.next = null;
				w.offsetX = (short)desc.widgets[i].offsetX;
				w.offsetY = (short)desc.widgets[i].offsetY;
				w.width = desc.widgets[i].width;
				w.height = desc.widgets[i].height;
				w.shortcut = 0;
				w.shortcut2 = 0;

				if (desc != CWindowDesc.g_savegameNameWindowDesc)
				{
					if (desc.widgets[i].labelStringId != (ushort)Text.STR_NULL)
					{
						w.shortcut = CWidget.GUI_Widget_GetShortcut((byte)Gui.GUI_String_Get_ByIndex((short)desc.widgets[i].labelStringId)[0]);
					}
					else
					{
						w.shortcut = CWidget.GUI_Widget_GetShortcut((byte)Gui.GUI_String_Get_ByIndex((short)desc.widgets[i].stringID)[0]);
					}
				}

				w.shortcut2 = desc.widgets[i].shortcut2;
				if (w.shortcut == 0x1B)
				{
					w.shortcut2 = 0x13;
				}

				w.stringID = desc.widgets[i].stringID;
				w.drawModeNormal = (byte)DrawMode.DRAW_MODE_CUSTOM_PROC;
				w.drawModeSelected = (byte)DrawMode.DRAW_MODE_CUSTOM_PROC;
				w.drawModeDown = (byte)DrawMode.DRAW_MODE_CUSTOM_PROC;
				w.drawParameterNormal.proc = WidgetDraw.GUI_Widget_TextButton_Draw;
				w.drawParameterSelected.proc = WidgetDraw.GUI_Widget_TextButton_Draw;
				w.drawParameterDown.proc = WidgetDraw.GUI_Widget_TextButton_Draw;
				w.parentID = desc.index;
				//memset(&w.state, 0, sizeof(w.state));

				CWidget.g_widgetLinkedListTail = CWidget.GUI_Widget_Link(CWidget.g_widgetLinkedListTail, w);

				CWidget.GUI_Widget_MakeVisible(w);
				CWidget.GUI_Widget_MakeNormal(w, false);
				CWidget.GUI_Widget_Draw(w);

				if (desc.widgets[i].labelStringId == (ushort)Text.STR_NULL) continue;

				if (Config.g_config.language == (byte)Language.LANGUAGE_FRENCH)
				{
					Gui.GUI_DrawText_Wrapper(Gui.GUI_String_Get_ByIndex((short)desc.widgets[i].labelStringId), (short)((CWidget.g_widgetProperties[w.parentID].xBase << 3) + 40), (short)(w.offsetY + CWidget.g_widgetProperties[w.parentID].yBase + 3), 232, 0, 0x22);
				}
				else
				{
					Gui.GUI_DrawText_Wrapper(Gui.GUI_String_Get_ByIndex((short)desc.widgets[i].labelStringId), (short)(w.offsetX + (CWidget.g_widgetProperties[w.parentID].xBase << 3) - 10), (short)(w.offsetY + CWidget.g_widgetProperties[w.parentID].yBase + 3), 232, 0, 0x222);
				}
			}

			if (s_savegameCountOnDisk >= 5 && desc.addArrows)
			{
				var w = CWidget.g_table_windowWidgets[7];

				w.drawParameterNormal.sprite = Sprites.g_sprites[59];
				w.drawParameterSelected.sprite = Sprites.g_sprites[60];
				w.drawParameterDown.sprite = Sprites.g_sprites[60];
				w.next = null;
				w.parentID = desc.index;

				CWidget.GUI_Widget_MakeNormal(w, false);
				CWidget.GUI_Widget_MakeInvisible(w);
				GUI_Widget_Undraw(w, 233);

				CWidget.g_widgetLinkedListTail = CWidget.GUI_Widget_Link(CWidget.g_widgetLinkedListTail, w);

				w = CWidget.g_table_windowWidgets[8];

				w.drawParameterNormal.sprite = Sprites.g_sprites[61];
				w.drawParameterSelected.sprite = Sprites.g_sprites[62];
				w.drawParameterDown.sprite = Sprites.g_sprites[62];
				w.next = null;
				w.parentID = desc.index;

				CWidget.GUI_Widget_MakeNormal(w, false);
				CWidget.GUI_Widget_MakeInvisible(w);
				GUI_Widget_Undraw(w, 233);

				CWidget.g_widgetLinkedListTail = CWidget.GUI_Widget_Link(CWidget.g_widgetLinkedListTail, w);
			}

			Gui.GUI_Mouse_Hide_Safe();

			CWidget.Widget_SetCurrentWidget(desc.index);

			Gui.GUI_Screen_Copy((short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetWidth, (short)CWidget.g_curWidgetHeight, Screen.SCREEN_1, Screen.SCREEN_0);

			Gui.GUI_Mouse_Show_Safe();

			Gfx.GFX_Screen_SetActive(Screen.SCREEN_0);
		}

		/*
		 * Handles Click event for scrollbar up button.
		 *
		 * @param w The widget.
		 * @return False, always.
		 */
		internal static bool GUI_Widget_Scrollbar_ArrowUp_Click(Widget w)
		{
			ushort temp;
			unchecked { temp = (ushort)-1; }
			GUI_Widget_Scrollbar_Scroll((WidgetScrollbar)w.data, temp);
			return false;
		}

		/*
		 * Handles Click event for scrollbar down button.
		 *
		 * @param w The widget.
		 * @return False, always.
		 */
		internal static bool GUI_Widget_Scrollbar_ArrowDown_Click(Widget w)
		{
			GUI_Widget_Scrollbar_Scroll((WidgetScrollbar)w.data, 1);

			return false;
		}

		/*
		 * Handles scrolling of a scrollbar.
		 *
		 * @param scrollbar The scrollbar.
		 * @param scroll The amount of scrolling.
		 */
		static void GUI_Widget_Scrollbar_Scroll(WidgetScrollbar scrollbar, ushort scroll)
		{
			scrollbar.scrollPosition += scroll;

			if ((short)scrollbar.scrollPosition >= scrollbar.scrollMax - scrollbar.scrollPageSize)
			{
				scrollbar.scrollPosition = (ushort)(scrollbar.scrollMax - scrollbar.scrollPageSize);
			}

			if ((short)scrollbar.scrollPosition <= 0) scrollbar.scrollPosition = 0;

			CWidget.GUI_Widget_Scrollbar_CalculatePosition(scrollbar);

			WidgetDraw.GUI_Widget_Scrollbar_Draw(scrollbar.parent);
		}

		/*
		 * Handles Click event for scrollbar button.
		 *
		 * @param w The widget.
		 * @return False, always.
		 */
		internal static bool GUI_Widget_Scrollbar_Click(Widget w)
		{
			WidgetScrollbar scrollbar;
			ushort positionX, positionY;

			scrollbar = (WidgetScrollbar)w.data;

			positionX = (ushort)w.offsetX;
			if (w.offsetX < 0) positionX += (ushort)(CWidget.g_widgetProperties[w.parentID].width << 3);
			positionX += (ushort)(CWidget.g_widgetProperties[w.parentID].xBase << 3);

			positionY = (ushort)w.offsetY;
			if (w.offsetY < 0) positionY += CWidget.g_widgetProperties[w.parentID].height;
			positionY += CWidget.g_widgetProperties[w.parentID].yBase;

			if ((w.state.buttonState & 0x44) != 0)
			{
				scrollbar.pressed = 0;
				WidgetDraw.GUI_Widget_Scrollbar_Draw(w);
			}

			if ((w.state.buttonState & 0x11) != 0)
			{
				short positionCurrent;
				short positionBegin;
				short positionEnd;

				scrollbar.pressed = 0;

				if (w.width > w.height)
				{
					positionCurrent = (short)Mouse.g_mouseX;
					positionBegin = (short)(positionX + scrollbar.position + 1);
				}
				else
				{
					positionCurrent = (short)Mouse.g_mouseY;
					positionBegin = (short)(positionY + scrollbar.position + 1);
				}

				positionEnd = (short)(positionBegin + scrollbar.size);

				if (positionCurrent <= positionEnd && positionCurrent >= positionBegin)
				{
					scrollbar.pressed = 1;
					scrollbar.pressedPosition = (ushort)(positionCurrent - positionBegin);
				}
				else
				{
					GUI_Widget_Scrollbar_Scroll(scrollbar, (ushort)(positionCurrent < positionBegin ? -scrollbar.scrollPageSize : scrollbar.scrollPageSize));
				}
			}

			if ((w.state.buttonState & 0x22) != 0 && scrollbar.pressed != 0)
			{
				short position, size;

				if (w.width > w.height)
				{
					size = (short)(w.width - 2 - scrollbar.size);
					position = (short)(Mouse.g_mouseX - scrollbar.pressedPosition - positionX - 1);
				}
				else
				{
					size = (short)(w.height - 2 - scrollbar.size);
					position = (short)(Mouse.g_mouseY - scrollbar.pressedPosition - positionY - 1);
				}

				if (position < 0)
				{
					position = 0;
				}
				else if (position > size)
				{
					position = size;
				}

				if (scrollbar.position != position)
				{
					scrollbar.position = (ushort)position;
					scrollbar.dirty = 1;
				}

				CWidget.GUI_Widget_Scrollbar_CalculateScrollPosition(scrollbar);

				if (scrollbar.dirty != 0) WidgetDraw.GUI_Widget_Scrollbar_Draw(w);
			}

			return false;
		}

		/*
		 * Handles Click event for the list in production window.
		 *
		 * @return True, always.
		 */
		internal static bool GUI_Production_List_Click(Widget w)
		{
			Gui.GUI_FactoryWindow_B495_0F30();

			Gui.g_factoryWindowSelected = (ushort)(w.index - 46);

			Gui.GUI_FactoryWindow_DrawDetails();

			Gui.GUI_FactoryWindow_UpdateSelection(true);

			return true;
		}

		/*
		 * Handles Click event for the "Resume Game" button in production window.
		 *
		 * @return True, always.
		 */
		internal static bool GUI_Production_ResumeGame_Click(Widget w)
		{
			Gui.g_factoryWindowResult = FactoryResult.FACTORY_RESUME;

			if (Gui.g_factoryWindowStarport)
			{
				byte i = 0;
				var h = CHouse.g_playerHouse;
				while (Gui.g_factoryWindowOrdered != 0)
				{
					if (Gui.g_factoryWindowItems[i].amount != 0)
					{
						h.credits += (ushort)(Gui.g_factoryWindowItems[i].amount * Gui.g_factoryWindowItems[i].credits);
						Gui.g_factoryWindowOrdered -= (ushort)Gui.g_factoryWindowItems[i].amount;
						Gui.g_factoryWindowItems[i].amount = 0;
					}

					i++;

					Gui.GUI_DrawCredits((byte)CHouse.g_playerHouseID, 0);
				}
			}

			if (w != null) CWidget.GUI_Widget_MakeNormal(w, false);

			return true;
		}

		/*
		 * Handles Click event for the "Build this" button in production window.
		 *
		 * @return True, always.
		 */
		internal static bool GUI_Production_BuildThis_Click(Widget w)
		{
			if (Gui.g_factoryWindowStarport)
			{
				if (Gui.g_factoryWindowOrdered == 0)
				{
					CWidget.GUI_Widget_MakeInvisible(w);
					GUI_Purchase_ShowInvoice();
					CWidget.GUI_Widget_MakeVisible(w);
				}
				else
				{
					Gui.g_factoryWindowResult = FactoryResult.FACTORY_BUY;
				}
			}
			else
			{
				FactoryWindowItem item;
				ObjectInfo oi;

				item = Gui.GUI_FactoryWindow_GetItem((short)Gui.g_factoryWindowSelected);
				oi = item.objectInfo;

				if (oi.available > 0)
				{
					item.amount = 1;
					Gui.g_factoryWindowResult = FactoryResult.FACTORY_BUY;
				}
			}

			CWidget.GUI_Widget_MakeNormal(w, false);

			return true;
		}

		/*
		 * Handles Click event for the "Down" button in production window.
		 *
		 * @return True, always.
		 */
		internal static bool GUI_Production_Down_Click(Widget w)
		{
			var drawDetails = false;

			if (Gui.g_factoryWindowSelected < 3 && (Gui.g_factoryWindowSelected + 1) < Gui.g_factoryWindowTotal)
			{
				Timer.g_timerTimeout = 10;
				Gui.GUI_FactoryWindow_B495_0F30();
				Gui.g_factoryWindowSelected++;

				Gui.GUI_FactoryWindow_UpdateSelection(true);

				drawDetails = true;
			}
			else
			{
				if (Gui.g_factoryWindowBase + 4 < Gui.g_factoryWindowTotal)
				{
					Timer.g_timerTimeout = 10;
					Gui.g_factoryWindowBase++;
					drawDetails = true;

					GUI_FactoryWindow_ScrollList(1);

					Gui.GUI_FactoryWindow_UpdateSelection(true);
				}
				else
				{
					Gui.GUI_FactoryWindow_DrawDetails();

					GUI_FactoryWindow_FailScrollList(1);
				}
			}

			for (; Timer.g_timerTimeout != 0; Sleep.sleepIdle())
			{
				Gui.GUI_FactoryWindow_UpdateSelection(false);
			}

			if (drawDetails) Gui.GUI_FactoryWindow_DrawDetails();

			CWidget.GUI_Widget_MakeNormal(w, false);

			return true;
		}

		/*
		 * Handles Click event for the "Up" button in production window.
		 *
		 * @return True, always.
		 */
		internal static bool GUI_Production_Up_Click(Widget w)
		{
			var drawDetails = false;

			if (Gui.g_factoryWindowSelected != 0)
			{
				Timer.g_timerTimeout = 10;
				Gui.GUI_FactoryWindow_B495_0F30();
				Gui.g_factoryWindowSelected--;

				Gui.GUI_FactoryWindow_UpdateSelection(true);

				drawDetails = true;
			}
			else
			{
				if (Gui.g_factoryWindowBase != 0)
				{
					Timer.g_timerTimeout = 10;
					Gui.g_factoryWindowBase--;
					drawDetails = true;

					GUI_FactoryWindow_ScrollList(-1);

					Gui.GUI_FactoryWindow_UpdateSelection(true);
				}
				else
				{
					Gui.GUI_FactoryWindow_DrawDetails();

					GUI_FactoryWindow_FailScrollList(-1);
				}
			}

			for (; Timer.g_timerTimeout != 0; Sleep.sleepIdle())
			{
				Gui.GUI_FactoryWindow_UpdateSelection(false);
			}

			if (drawDetails) Gui.GUI_FactoryWindow_DrawDetails();

			CWidget.GUI_Widget_MakeNormal(w, false);

			return true;
		}

		static void GUI_FactoryWindow_ScrollList(short step)
		{
			ushort i;
			ushort y = 32;

			Gui.GUI_FactoryWindow_B495_0F30();

			Gui.GUI_Mouse_Hide_Safe();

			for (i = 0; i < 32; i++)
			{
				y += (ushort)step;
				Gfx.GFX_Screen_Copy2(72, (short)y, 72, 16, 32, 136, Screen.SCREEN_1, Screen.SCREEN_0, false);
			}

			Gui.GUI_Mouse_Show_Safe();

			Gui.GUI_FactoryWindow_PrepareScrollList();

			Gui.GUI_FactoryWindow_UpdateSelection(true);
		}

		static void GUI_FactoryWindow_FailScrollList(short step)
		{
			ushort i;
			ushort y = 32;

			Gui.GUI_FactoryWindow_B495_0F30();

			Gui.GUI_Mouse_Hide_Safe();

			Gui.GUI_FactoryWindow_B495_0F30();

			for (i = 0; i < 6; i++)
			{
				y += (ushort)step;
				Gfx.GFX_Screen_Copy2(72, (short)y, 72, 16, 32, 136, Screen.SCREEN_1, Screen.SCREEN_0, false);
			}

			for (i = 0; i < 6; i++)
			{
				y -= (ushort)step;
				Gfx.GFX_Screen_Copy2(72, (short)y, 72, 16, 32, 136, Screen.SCREEN_1, Screen.SCREEN_0, false);
			}

			Gui.GUI_Mouse_Show_Safe();

			Gui.GUI_FactoryWindow_UpdateSelection(true);
		}

		/*
		 * Handles Click event for the "Ugrade" button in production window.
		 *
		 * @return True, always.
		 */
		internal static bool GUI_Production_Upgrade_Click(Widget w)
		{
			CWidget.GUI_Widget_MakeNormal(w, false);

			Gui.g_factoryWindowResult = FactoryResult.FACTORY_UPGRADE;

			return true;
		}

		/*
		 * Handles Click event for the "+" button in starport window.
		 *
		 * @return True, always.
		 */
		internal static bool GUI_Purchase_Plus_Click(Widget w)
		{
			var item = Gui.GUI_FactoryWindow_GetItem((short)Gui.g_factoryWindowSelected);
			var oi = item.objectInfo;
			var h = CHouse.g_playerHouse;
			var canCreateMore = true;
			var type = item.objectType;

			CWidget.GUI_Widget_MakeNormal(w, false);

			if (CUnit.g_table_unitInfo[type].movementType != (ushort)MovementType.MOVEMENT_WINGER && CUnit.g_table_unitInfo[type].movementType != (ushort)MovementType.MOVEMENT_SLITHER)
			{
				if (CSharpDune.g_starPortEnforceUnitLimit && h.unitCount >= h.unitCountMax) canCreateMore = false;
			}

			if (item.amount < oi.available && item.credits <= h.credits && canCreateMore)
			{
				item.amount++;

				Gui.GUI_FactoryWindow_UpdateDetails(item);

				Gui.g_factoryWindowOrdered++;

				h.credits -= (ushort)item.credits;

				Gui.GUI_FactoryWindow_DrawCaption(null);
			}

			return true;
		}

		/*
		 * Handles Click event for the "-" button in startport window.
		 *
		 * @return True, always.
		 */
		internal static bool GUI_Purchase_Minus_Click(Widget w)
		{
			FactoryWindowItem item;
			var h = CHouse.g_playerHouse;

			CWidget.GUI_Widget_MakeNormal(w, false);

			item = Gui.GUI_FactoryWindow_GetItem((short)Gui.g_factoryWindowSelected);

			if (item.amount != 0)
			{
				item.amount--;

				Gui.GUI_FactoryWindow_UpdateDetails(item);

				Gui.g_factoryWindowOrdered--;

				h.credits += (ushort)item.credits;

				Gui.GUI_FactoryWindow_DrawCaption(null);
			}

			return true;
		}

		/*
		 * Handles Click event for the "Invoice" button in starport window.
		 *
		 * @return True, always.
		 */
		internal static bool GUI_Purchase_Invoice_Click(Widget w)
		{
			CWidget.GUI_Widget_MakeInvisible(w);
			GUI_Purchase_ShowInvoice();
			CWidget.GUI_Widget_MakeVisible(w);
			CWidget.GUI_Widget_MakeNormal(w, false);
			return true;
		}

		static void GUI_Purchase_ShowInvoice()
		{
			var w = CWidget.g_widgetInvoiceTail;
			Screen oldScreenID;
			ushort y = 48;
			ushort total = 0;
			ushort x;
			string textBuffer; //char[12]

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.SCREEN_1);

			Gui.GUI_DrawFilledRectangle(128, 48, 311, 159, 20);

			Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_ITEM_NAME_QTY_TOTAL), 128, (short)y, 12, 0, 0x11);

			y += 7;

			Gui.GUI_DrawLine(129, (short)y, 310, (short)y, 12);

			y += 2;

			if (Gui.g_factoryWindowOrdered != 0)
			{
				ushort i;

				for (i = 0; i < Gui.g_factoryWindowTotal; i++)
				{
					ObjectInfo oi;
					ushort amount;

					if (Gui.g_factoryWindowItems[i].amount == 0) continue;

					amount = (ushort)(Gui.g_factoryWindowItems[i].amount * Gui.g_factoryWindowItems[i].credits);
					total += amount;

					textBuffer = string.Format("{0:D2} {1, 5}", Gui.g_factoryWindowItems[i].amount, amount); //snprintf(textBuffer, sizeof(textBuffer), "%02d %5d", g_factoryWindowItems[i].amount, amount);

					oi = Gui.g_factoryWindowItems[i].objectInfo;
					Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(oi.stringID_full), 128, (short)y, 8, 0, 0x11);

					Gui.GUI_DrawText_Monospace(textBuffer, (ushort)(311 - (short)textBuffer.Length * 6), y, 15, 0, 6);

					y += 8;
				}
			}
			else
			{
				Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_NO_UNITS_ON_ORDER), 220, 99, 6, 0, 0x112);
			}

			Gui.GUI_DrawLine(129, 148, 310, 148, 12);
			Gui.GUI_DrawLine(129, 150, 310, 150, 12);

			textBuffer = total.ToString("D"); //snprintf(textBuffer, sizeof(textBuffer), "%d", total);

			x = (ushort)(311 - (short)textBuffer.Length * 6);

			/* "Total Cost :" */
			Gui.GUI_DrawText_Wrapper(Gui.GUI_String_Get_ByIndex((short)Text.STR_TOTAL_COST_), (short)(x - 3), 152, 11, 0, 0x211);
			Gui.GUI_DrawText_Monospace(textBuffer, x, 152, 11, 0, 6);

			Gui.GUI_Mouse_Hide_Safe();
			Gui.GUI_Screen_Copy(16, 48, 16, 48, 23, 112, Screen.SCREEN_1, Screen.SCREEN_0);
			Gui.GUI_Mouse_Show_Safe();

			Gfx.GFX_Screen_SetActive(Screen.SCREEN_0);

			Gui.GUI_FactoryWindow_DrawCaption(CString.String_Get_ByIndex(Text.STR_INVOICE_OF_UNITS_ON_ORDER));

			Input.Input_History_Clear();

			for (; CWidget.GUI_Widget_HandleEvents(w) == 0; Sleep.sleepIdle())
			{
				Gui.GUI_DrawCredits((byte)CHouse.g_playerHouseID, 0);

				Gui.GUI_FactoryWindow_UpdateSelection(false);

				Gui.GUI_PaletteAnimate();
			}

			Gfx.GFX_Screen_SetActive(oldScreenID);

			w = CWidget.GUI_Widget_Get_ByIndex(w, 10);

			if (w != null && Mouse.Mouse_InsideRegion(w.offsetX, w.offsetY, (short)(w.offsetX + w.width), (short)(w.offsetY + w.height)) != 0)
			{
				while (Input.Input_Test(0x41) != 0 || Input.Input_Test(0x42) != 0) Sleep.sleepIdle();
				Input.Input_History_Clear();
			}

			if (Gui.g_factoryWindowResult == FactoryResult.FACTORY_CONTINUE) Gui.GUI_FactoryWindow_DrawDetails();
		}

		/*
		 * Handles Click event for "Options" button.
		 *
		 * @param w The widget.
		 * @return False, always.
		 */
		internal static bool GUI_Widget_Options_Click(Widget w)
		{
			var desc = CWindowDesc.g_optionsWindowDesc;
			var cursor = Gui.g_cursorSpriteID;
			bool loop;

			Gui.g_cursorSpriteID = 0;

			Sprites.Sprites_SetMouseSprite(0, 0, Sprites.g_sprites[0]);

			Sprites.Sprites_UnloadTiles();

			Buffer.BlockCopy(Gfx.g_paletteActive, 0, Gui.g_palette_998A, 0, 256 * 3); //memmove(g_palette_998A, g_paletteActive, 256 * 3);

			CDriver.Driver_Voice_Play(null, 0xFF);

			Timer.Timer_SetTimer(TimerType.TIMER_GAME, false);

			Gui.GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x22);

			ShadeScreen();

			GUI_Window_BackupScreen(desc);

			GUI_Window_Create(desc);

			for (loop = true; loop; Sleep.sleepIdle())
			{
				var w2 = CWidget.g_widgetLinkedListTail;
				var key = CWidget.GUI_Widget_HandleEvents(w2);

				if ((key & 0x8000) != 0)
				{
					w = CWidget.GUI_Widget_Get_ByIndex(w2, key);

					GUI_Window_RestoreScreen(desc);

					switch ((key & 0x7FFF) - 0x1E)
					{
						case 0:
							if (GUI_Widget_SaveLoad_Click(false)) loop = false;
							break;

						case 1:
							if (GUI_Widget_SaveLoad_Click(true)) loop = false;
							break;

						case 2:
							GUI_Widget_GameControls_Click(w);
							break;

						case 3:
							/* "Are you sure you wish to restart?" */
							if (!GUI_YesNo((ushort)Text.STR_ARE_YOU_SURE_YOU_WISH_TO_RESTART)) break;

							loop = false;
							CSharpDune.g_gameMode = GameMode.GM_RESTART;
							break;

						case 4:
							/* "Are you sure you wish to pick a new house?" */
							if (!GUI_YesNo((ushort)Text.STR_ARE_YOU_SURE_YOU_WISH_TO_PICK_A_NEW_HOUSE)) break;

							loop = false;
							CDriver.Driver_Music_FadeOut();
							CSharpDune.g_gameMode = GameMode.GM_PICKHOUSE;
							break;

						case 5:
							loop = false;
							break;

						case 6:
							/* "Are you sure you want to quit playing?" */
							loop = !GUI_YesNo((ushort)Text.STR_ARE_YOU_SURE_YOU_WANT_TO_QUIT_PLAYING);
							CSharpDune.g_running = loop;

							Sound.Sound_Output_Feedback(0xFFFE);

							while (CDriver.Driver_Voice_IsPlaying()) Sleep.sleepIdle();
							break;

						default: break;
					}

					if (CSharpDune.g_running && loop)
					{
						GUI_Window_BackupScreen(desc);

						GUI_Window_Create(desc);
					}
				}

				Gui.GUI_PaletteAnimate();
			}

			Gui.g_textDisplayNeedsUpdate = true;

			Sprites.Sprites_LoadTiles();
			Gui.GUI_DrawInterfaceAndRadar(Screen.SCREEN_0);

			UnshadeScreen();

			CWidget.GUI_Widget_MakeSelected(w, false);

			Timer.Timer_SetTimer(TimerType.TIMER_GAME, true);

			Config.GameOptions_Save();

			CStructure.Structure_Recount();
			CUnit.Unit_Recount();

			Gui.g_cursorSpriteID = cursor;

			Sprites.Sprites_SetMouseSprite(0, 0, Sprites.g_sprites[cursor]);

			return false;
		}

		/* shade everything except colors 231 to 238 */
		static void ShadeScreen()
		{
			ushort i;

			Buffer.BlockCopy(Gfx.g_palette1, 0, Gui.g_palette_998A, 0, 256 * 3); //memmove(g_palette_998A, g_palette1, 256 * 3);

			for (i = 0; i < 231 * 3; i++) Gfx.g_palette1[i] = (byte)(Gfx.g_palette1[i] / 2);
			for (i = 239 * 3; i < 256 * 3; i++) Gfx.g_palette1[i] = (byte)(Gfx.g_palette1[i] / 2);

			Gfx.GFX_SetPalette(Gui.g_palette_998A);
		}

		static void UnshadeScreen()
		{
			Buffer.BlockCopy(Gui.g_palette_998A, 0, Gfx.g_palette1, 0, 256 * 3); //memmove(g_palette1, g_palette_998A, 256 * 3);

			Gfx.GFX_SetPalette(Gfx.g_palette1);
		}

		/*
		 * Handles Click event for "Game controls" button.
		 *
		 * @param w The widget.
		 */
		static void GUI_Widget_GameControls_Click(Widget w)
		{
			var desc = CWindowDesc.g_gameControlWindowDesc;
			bool loop;

			GUI_Window_BackupScreen(desc);

			GUI_Window_Create(desc);

			for (loop = true; loop; Sleep.sleepIdle())
			{
				var w2 = CWidget.g_widgetLinkedListTail;
				var key = CWidget.GUI_Widget_HandleEvents(w2);

				if ((key & 0x8000) != 0)
				{
					w = CWidget.GUI_Widget_Get_ByIndex(w2, (ushort)(key & 0x7FFF));

					switch ((key & 0x7FFF) - 0x1E)
					{
						case 0:
							Config.g_gameConfig.music ^= 0x1;
							if (Config.g_gameConfig.music == 0) CDriver.Driver_Music_Stop();
							break;

						case 1:
							Config.g_gameConfig.sounds ^= 0x1;
							if (Config.g_gameConfig.sounds == 0) CDriver.Driver_Sound_Stop();
							break;

						case 2:
							if (++Config.g_gameConfig.gameSpeed >= 5) Config.g_gameConfig.gameSpeed = 0;
							break;

						case 3:
							Config.g_gameConfig.hints ^= 0x1;
							break;

						case 4:
							Config.g_gameConfig.autoScroll ^= 0x1;
							break;

						case 5:
							loop = false;
							break;

						default: break;
					}

					CWidget.GUI_Widget_MakeNormal(w, false);

					CWidget.GUI_Widget_Draw(w);
				}

				Gui.GUI_PaletteAnimate();
			}

			GUI_Window_RestoreScreen(desc);
		}
	}
}
