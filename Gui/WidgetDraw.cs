/* Widget drawing */

namespace SharpDune.Gui
{
    class WidgetDraw
	{
		/*
		 * Draw the panel on the right side of the screen, with the actions of the
		 *  selected item.
		 *
		 * @param forceDraw Wether to draw the panel even if nothing changed.
		 */
		internal static void GUI_Widget_ActionPanel_Draw(bool forceDraw)
		{
			StructureInfo si;
			ObjectInfo oi;
			UnitInfo ui;
			ushort actionType;
			Screen oldScreenID;
			ushort oldWidgetID;
			bool isNotPlayerOwned;
			Object o;
			Unit u;
			Structure s;
			House h;
			var buttons = new Widget[4];
			Widget widget24 = null, widget28 = null, widget2C = null, widget30 = null, widget34 = null;

			o = null;
			u = null;
			s = null;
			h = null;

			oi = null;
			ui = null;
			si = null;
			isNotPlayerOwned = false;

			actionType = GUI_Widget_ActionPanel_GetActionType(forceDraw);

			switch (actionType)
			{
				case 2:
					{ /* Unit */
						u = CUnit.g_unitSelected;
						ui = g_table_unitInfo[u.o.type];

						o = u.o;
						oi = ui.o;

						isNotPlayerOwned = (byte)CHouse.g_playerHouseID != CUnit.Unit_GetHouseID(u);

						h = PoolHouse.House_Get_ByIndex(u.o.houseID);
					}
					break;

				case 3:
					{ /* Structure */
						s = CStructure.Structure_Get_ByPackedTile(Gui.g_selectionPosition);
						si = g_table_structureInfo[s.o.type];

						o = s.o;
						oi = si.o;

						isNotPlayerOwned = (byte)CHouse.g_playerHouseID != s.o.houseID;

						h = PoolHouse.House_Get_ByIndex(s.o.houseID);

						if (s.upgradeTimeLeft == 0 && CStructure.Structure_IsUpgradable(s)) s.upgradeTimeLeft = 100;
						Gui.GUI_UpdateProductionStringID();
					}
					break;

				case 7:
					{ /* Placement */
						si = g_table_structureInfo[CStructure.g_structureActiveType];

						o = null;
						oi = si.o;

						isNotPlayerOwned = false;

						h = PoolHouse.House_Get_ByIndex((byte)CHouse.g_playerHouseID);
					}
					break;

				case 8:
					{ /* House Missile */
						u = CUnit.g_unitHouseMissile;
						ui = g_table_unitInfo[u.o.type];

						o = u.o;
						oi = ui.o;

						isNotPlayerOwned = (byte)CHouse.g_playerHouseID != CUnit.Unit_GetHouseID(u);

						h = PoolHouse.House_Get_ByIndex((byte)CHouse.g_playerHouseID);
					}
					break;

				case 4: /* Attack */
				case 5: /* Movement */
				case 6: /* Harvest */
				default: /* Default */
					break;

			}

			oldScreenID = Screen.ACTIVE;
			oldWidgetID = CWidget.g_curWidgetIndex;

			if (actionType != 0)
			{
				var w = CWidget.g_widgetLinkedListHead;
				int i;

				oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);
				oldWidgetID = CWidget.Widget_SetCurrentWidget(6);

				widget30 = CWidget.GUI_Widget_Get_ByIndex(w, 7);
				CWidget.GUI_Widget_MakeInvisible(widget30);

				widget24 = CWidget.GUI_Widget_Get_ByIndex(w, 4);
				CWidget.GUI_Widget_MakeInvisible(widget24);

				widget28 = CWidget.GUI_Widget_Get_ByIndex(w, 6);
				CWidget.GUI_Widget_MakeInvisible(widget28);

				widget2C = CWidget.GUI_Widget_Get_ByIndex(w, 5);
				CWidget.GUI_Widget_MakeInvisible(widget2C);

				widget34 = CWidget.GUI_Widget_Get_ByIndex(w, 3);
				CWidget.GUI_Widget_MakeInvisible(widget34);

				/* Create the 4 buttons */
				for (i = 0; i < 4; i++)
				{
					buttons[i] = CWidget.GUI_Widget_Get_ByIndex(w, (ushort)(i + 8));
					CWidget.GUI_Widget_MakeInvisible(buttons[i]);
				}

				GUI_Widget_DrawBorder(CWidget.g_curWidgetIndex, 0, false /*0*/);
			}

			if (actionType > 1)
			{
				var stringID = (ushort)Text.STR_NULL;
				ushort spriteID = 0xFFFF;

				switch (actionType)
				{
					case 4: stringID = (ushort)Text.STR_TARGET; break; /* Attack */
					case 5: stringID = (ushort)Text.STR_MOVEMENT; break; /* Movement */
					case 6: stringID = (ushort)Text.STR_HARVEST; break; /* Harvest */

					case 2: /* Unit */
					case 3: /* Structure */
					case 7: /* Placement */
					case 8: /* House Missile */
						stringID = oi.stringID_abbrev;
						break;

					default: break;
				}

				if (stringID != (ushort)Text.STR_NULL) Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(stringID), 288, 43, 29, 0, 0x111);

				switch (actionType)
				{
					case 3: /* Structure */
					case 2: /* Unit */
					case 7: /* Placement */
						if (actionType == 3)
						{
							if (oi.flags.factory && !isNotPlayerOwned)
							{
								CWidget.GUI_Widget_MakeVisible(widget28);
								break;
							}
						}
						spriteID = oi.spriteID;
						break;

					case 5: /* Movement */
					case 6: /* Harvest */
						spriteID = 0x1D;
						break;

					case 4: /* Attack */
						spriteID = 0x1C;
						break;

					case 8: /* House Missile */
						spriteID = 0x1E;
						break;

					default:
						spriteID = 0xFFFF;
						break;
				}

				if (spriteID != 0xFFFF)
				{
					Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[spriteID], 258, 51, 0, 0);
				}

				/* Unit / Structure */
				if (actionType == 2 || actionType == 3)
				{
					Gui.GUI_DrawProgressbar(o.hitpoints, oi.hitpoints);
					Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[27], 292, 60, 0, 0);
					Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_DMG), 296, 65, 29, 0, 0x11);
				}

				if (!isNotPlayerOwned || CSharpDune.g_debugGame)
				{
					switch (actionType)
					{
						case 2: /* Unit */
							{
								ushort[] actions;
								ushort actionCurrent;
								int i;

								CWidget.GUI_Widget_MakeVisible(widget34);

								actionCurrent = (u.nextActionID != (byte)ActionType.ACTION_INVALID) ? u.nextActionID : u.actionID;

								actions = oi.actionsPlayer;
								if (isNotPlayerOwned && o.type != (byte)UnitType.UNIT_HARVESTER) actions = g_table_actionsAI;

								for (i = 0; i < 4; i++)
								{
									buttons[i].stringID = g_table_actionInfo[actions[i]].stringID;
									buttons[i].shortcut = CWidget.GUI_Widget_GetShortcut((byte)CString.String_Get_ByIndex(buttons[i].stringID)[0]);

									if (Config.g_config.language == (byte)Language.FRENCH)
									{
										if (buttons[i].stringID == (ushort)Text.STR_MOVE) buttons[i].shortcut2 = 0x27;  /* L key */
										else if (buttons[i].stringID == (ushort)Text.STR_RETURN) buttons[i].shortcut2 = 0x13;   /* E key */
									}
									else if (Config.g_config.language == (byte)Language.GERMAN)
									{
										if (buttons[i].stringID == (ushort)Text.STR_GUARD) buttons[i].shortcut2 = 0x17; /* U key */
									}

									CWidget.GUI_Widget_MakeVisible(buttons[i]);

									if (actions[i] == actionCurrent)
									{
										CWidget.GUI_Widget_MakeSelected(buttons[i], false);
									}
									else
									{
										CWidget.GUI_Widget_MakeNormal(buttons[i], false);
									}
								}
							}
							break;

						case 3: /* Structure */
							{
								CWidget.GUI_Widget_MakeVisible(widget34);

								if (o.flags.upgrading)
								{
									widget24.stringID = (ushort)Text.STR_UPGRADING;

									CWidget.GUI_Widget_MakeVisible(widget24);
									CWidget.GUI_Widget_MakeSelected(widget24, false);
								}
								else if (o.hitpoints != oi.hitpoints)
								{
									if (o.flags.repairing)
									{
										widget24.stringID = (ushort)Text.STR_REPAIRING;

										CWidget.GUI_Widget_MakeVisible(widget24);
										CWidget.GUI_Widget_MakeSelected(widget24, false);
									}
									else
									{
										widget24.stringID = (ushort)Text.STR_REPAIR;

										CWidget.GUI_Widget_MakeVisible(widget24);
										CWidget.GUI_Widget_MakeNormal(widget24, false);
									}
								}
								else if (s.upgradeTimeLeft != 0)
								{
									widget24.stringID = (ushort)Text.STR_UPGRADE;

									CWidget.GUI_Widget_MakeVisible(widget24);
									CWidget.GUI_Widget_MakeNormal(widget24, false);
								}

								if (o.type != (byte)StructureType.STRUCTURE_STARPORT)
								{
									if (oi.flags.factory || (o.type == (byte)StructureType.STRUCTURE_PALACE && s.countDown == 0))
									{
										CWidget.GUI_Widget_MakeVisible(widget2C);
										CWidget.GUI_Widget_Draw(widget2C);
									}
								}

								switch (o.type)
								{
									case (byte)StructureType.STRUCTURE_SLAB_1x1: break;
									case (byte)StructureType.STRUCTURE_SLAB_2x2: break;
									case (byte)StructureType.STRUCTURE_PALACE: break;
									case (byte)StructureType.STRUCTURE_LIGHT_VEHICLE: break;
									case (byte)StructureType.STRUCTURE_HEAVY_VEHICLE: break;
									case (byte)StructureType.STRUCTURE_HIGH_TECH: break;
									case (byte)StructureType.STRUCTURE_HOUSE_OF_IX: break;
									case (byte)StructureType.STRUCTURE_WOR_TROOPER: break;
									case (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD: break;
									case (byte)StructureType.STRUCTURE_BARRACKS: break;
									case (byte)StructureType.STRUCTURE_WALL: break;
									case (byte)StructureType.STRUCTURE_TURRET: break;
									case (byte)StructureType.STRUCTURE_ROCKET_TURRET: break;

									case (byte)StructureType.STRUCTURE_REPAIR:
										{
											ushort percent;
											ushort steps;
											Unit u2;

											u2 = CStructure.Structure_GetLinkedUnit(s);
											if (u2 == null) break;

											Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[g_table_unitInfo[u2.o.type].o.spriteID], 260, 89, 0, 0);

											steps = (ushort)(g_table_unitInfo[u2.o.type].o.buildTime / 4);
											percent = (ushort)((steps - (s.countDown >> 8)) * 100 / steps);

											Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_D_DONE), 258, 116, 29, 0, 0x11, percent);
										}
										break;

									case (byte)StructureType.STRUCTURE_WINDTRAP:
										{
											var powerOutput = (ushort)(o.hitpoints * -si.powerUsage / oi.hitpoints);
											var powerAverage = (ushort)((h.windtrapCount == 0) ? 0 : h.powerUsage / h.windtrapCount);

											Gui.GUI_DrawLine(261, 95, 312, 95, 16);
											Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_POWER_INFONEEDEDOUTPUT), 258, 88, 29, 0, 0x11);
											Gui.GUI_DrawText_Wrapper("{0, 3}", 292, (short)(CFont.g_fontCurrent.height * 2 + 80), 29, 0, 0x11, powerAverage);
											Gui.GUI_DrawText_Wrapper("{0, 3}", 292, (short)(CFont.g_fontCurrent.height * 3 + 80), (byte)((powerOutput >= powerAverage) ? 29 : 6), 0, 0x11, powerOutput);
										}
										break;

									case (byte)StructureType.STRUCTURE_STARPORT:
										{
											if (h.starportLinkedID != 0xFFFF)
											{
												Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_FRIGATEARRIVAL_INTMINUS_D), 258, 88, 29, 0, 0x11, h.starportTimeLeft);
											}
											else
											{
												Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_FRIGATE_INORBIT_ANDAWAITINGORDER), 258, 88, 29, 0, 0x11);
											}
										}
										break;

									case (byte)StructureType.STRUCTURE_REFINERY:
									case (byte)StructureType.STRUCTURE_SILO:
										{
											ushort creditsStored;

											creditsStored = (ushort)(h.credits * si.creditsStorage / h.creditsStorage);
											if (h.credits > h.creditsStorage) creditsStored = si.creditsStorage;

											Gui.GUI_DrawLine(261, 95, 312, 95, 16);
											Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_SPICEHOLDS_4DMAX_4D), 258, 88, 29, 0, 0x11, creditsStored, (si.creditsStorage <= 1000) ? si.creditsStorage : 1000);
										}
										break;

									case (byte)StructureType.STRUCTURE_OUTPOST:
										{
											Gui.GUI_DrawLine(261, 95, 312, 95, 16);
											Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_RADAR_SCANFRIEND_2DENEMY_2D), 258, 88, 29, 0, 0x11, h.unitCountAllied, h.unitCountEnemy);
										}
										break;
								}
							}
							break;

						case 4: /* Attack */
							CWidget.GUI_Widget_MakeVisible(widget30);
							Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_SELECTTARGET), 259, 76, CWidget.g_curWidgetFGColourBlink, 0, 0x11);
							break;

						case 5: /* Movement */
							CWidget.GUI_Widget_MakeVisible(widget30);
							Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_SELECTDESTINATION), 259, 76, CWidget.g_curWidgetFGColourBlink, 0, 0x11);
							break;

						case 6: /* Harvest */
							CWidget.GUI_Widget_MakeVisible(widget30);
							Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_SELECTPLACE_TOHARVEST), 259, 76, CWidget.g_curWidgetFGColourBlink, 0, 0x11);
							break;

						case 7: /* Placement */
							CWidget.GUI_Widget_MakeVisible(widget30);
							Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_SELECTLOCATION_TOBUILD), 259, 84, CWidget.g_curWidgetFGColourBlink, 0, 0x11);
							break;

						case 8: /* House Missile */
							{
								var count = (short)(CHouse.g_houseMissileCountdown - 1);
								if (count <= 0) count = 0;

								Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_PICK_TARGETTMINUS_D), 259, 84, CWidget.g_curWidgetFGColourBlink, 0, 0x11, count);
							}
							break;

						default:
							break;
					}
				}
			}

			if (actionType != 0)
			{
				Gui.GUI_Mouse_Hide_InWidget(6);
				Gui.GUI_Screen_Copy((short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetWidth, (short)CWidget.g_curWidgetHeight, Screen.ACTIVE, Screen.NO0);
				Gui.GUI_Mouse_Show_InWidget();
			}

			if (actionType > 1)
			{
				CWidget.Widget_SetCurrentWidget(oldWidgetID);
				Gfx.GFX_Screen_SetActive(oldScreenID);
			}
		}

		static readonly byte[][] borderIndexSize = { //[][2]
			new byte[] {0, 0}, new byte[] {2, 4}, new byte[] {1, 1}, new byte[] {2, 1}
		};
		/*
		 * Draw the border around a widget.
		 * @param widgetIndex The widget index to draw the border around.
		 * @param borderType The type of border. 0 = normal, 1 = thick depth, 2 = double, 3 = thin depth.
		 * @param pressed True if the button is pressed.
		 */
		internal static void GUI_Widget_DrawBorder(ushort widgetIndex, ushort borderType, bool pressed)
		{
			var left = (ushort)(CWidget.g_widgetProperties[widgetIndex].xBase << 3);
			var top = CWidget.g_widgetProperties[widgetIndex].yBase;
			var width = (ushort)(CWidget.g_widgetProperties[widgetIndex].width << 3);
			var height = CWidget.g_widgetProperties[widgetIndex].height;

			var colourSchemaIndex = (ushort)(pressed ? 2 : 0);
			ushort size;

			if (Gfx.GFX_Screen_IsActive(Screen.NO0))
			{
				Gui.GUI_Mouse_Hide_InRegion(left, top, (ushort)(left + width), (ushort)(top + height));
			}

			Gui.GUI_DrawBorder(left, top, width, height, (ushort)(colourSchemaIndex + 1), true);

			size = borderIndexSize[borderType][1];

			if (size != 0)
			{
				colourSchemaIndex += borderIndexSize[borderType][0];
				Gui.GUI_DrawBorder((ushort)(left + size), (ushort)(top + size), (ushort)(width - (size * 2)), (ushort)(height - (size * 2)), colourSchemaIndex, false);
			}

			if (Gfx.GFX_Screen_IsActive(Screen.NO0))
			{
				Gui.GUI_Mouse_Show_InRegion();
			}
		}

		static ushort displayedActionType = 1;
		static ushort displayedHitpoints = 0xFFFF;
		static ushort displayedIndex = 0xFFFF;
		static ushort displayedCountdown = 0xFFFF;
		static ushort displayedObjectType = 0xFFFF;
		static ushort displayedStructureFlags;
        static ushort displayedLinkedID = 0xFFFF;
		static ushort displayedHouseID = 0xFFFF;
		static ushort displayedActiveAction = 0xFFFF;
		static ushort displayedMissileCountdown;
        static ushort displayedUpgradeTime = 0xFFFF;
		static ushort displayedStarportTime = 0xFFFF;
		/*
		 * Gets the action type used to determine how to draw the panel on the right side of the screen.
		 *
		 * @param forceDraw Wether to draw the panel even if nothing changed.
		 */
		static ushort GUI_Widget_ActionPanel_GetActionType(bool forceDraw)
		{
			ushort actionType = 0;
			Structure s = null;
			Unit u = null;

			if (CSharpDune.g_selectionType == (ushort)SelectionType.PLACE)
			{
				if (displayedActionType != 7 || forceDraw) actionType = 7; /* Placement */
			}
			else if (CUnit.g_unitHouseMissile != null)
			{
				if (displayedMissileCountdown != CHouse.g_houseMissileCountdown || forceDraw) actionType = 8; /* House Missile */
			}
			else if (CUnit.g_unitSelected != null)
			{
				if (CSharpDune.g_selectionType == (ushort)SelectionType.TARGET)
				{
					var activeAction = CSharpDune.g_activeAction;

					if (activeAction != displayedActiveAction || forceDraw)
					{
						switch (activeAction)
						{
							case (ushort)ActionType.ACTION_ATTACK: actionType = 4; break; /* Attack */
							case (ushort)ActionType.ACTION_MOVE: actionType = 5; break; /* Movement */
							default: actionType = 6; break; /* Harvest */
						}
					}

					if (actionType == displayedActionType && !forceDraw) actionType = 0;
				}
				else
				{
					u = CUnit.g_unitSelected;

					if (forceDraw
						|| u.o.index != displayedIndex
						|| u.o.hitpoints != displayedHitpoints
						|| u.o.houseID != displayedHouseID
						|| u.actionID != displayedActiveAction)
					{
						actionType = 2; /* Unit */
					}
				}
			}
			else if (!CTile.Tile_IsOutOfMap(Gui.g_selectionPosition) && (Map.g_map[Gui.g_selectionPosition].isUnveiled || CSharpDune.g_debugScenario))
			{
				if (Map.Map_GetLandscapeType(Gui.g_selectionPosition) == (ushort)LandscapeType.LST_STRUCTURE)
				{
					s = CStructure.Structure_Get_ByPackedTile(Gui.g_selectionPosition);

					if (forceDraw
						|| s.o.hitpoints != displayedHitpoints
						|| s.o.index != displayedIndex
						|| s.countDown != displayedCountdown
						|| s.upgradeTimeLeft != displayedUpgradeTime
						|| s.o.linkedID != displayedLinkedID
						|| s.objectType != displayedObjectType
						|| s.o.houseID != displayedHouseID
						|| PoolHouse.House_Get_ByIndex(s.o.houseID).starportTimeLeft != displayedStarportTime
						|| s.o.flags.all != displayedStructureFlags)
					{
						Gui.g_structureHighHealth = (s.o.hitpoints > (g_table_structureInfo[s.o.type].o.hitpoints / 2));
						actionType = 3; /* Structure */
					}
				}
				else
				{
					actionType = 1;
				}
			}
			else
			{
				actionType = 1;
			}

			switch (actionType)
			{
				case 8: /* House Missile */
					displayedMissileCountdown = CHouse.g_houseMissileCountdown;
					displayedIndex = 0xFFFF;
					break;

				case 1:
				case 4: /* Attack */
				case 5: /* Movement */
				case 6: /* Harvest */
				case 7: /* Placement */
					displayedIndex = 0xFFFF;
					displayedMissileCountdown = 0xFFFF;
					if (!forceDraw && actionType == displayedActionType) actionType = 0;
					break;

				case 2: /* Unit */
					displayedHitpoints = u.o.hitpoints;
					displayedIndex = u.o.index;
					displayedActiveAction = u.actionID;
					displayedMissileCountdown = 0xFFFF;
					displayedHouseID = u.o.houseID;
					break;

				case 3: /* Structure */
					displayedHitpoints = s.o.hitpoints;
					displayedIndex = s.o.index;
					displayedObjectType = s.objectType;
					displayedCountdown = s.countDown;
					displayedUpgradeTime = s.upgradeTimeLeft;
					displayedLinkedID = s.o.linkedID;
					displayedStructureFlags = (ushort)s.o.flags.all;
					displayedHouseID = s.o.houseID;
					displayedMissileCountdown = 0xFFFF;
					displayedStarportTime = PoolHouse.House_Get_ByIndex(s.o.houseID).starportTimeLeft;
					break;

				case 0:
				default:
					break;
			}

			if (actionType != 0) displayedActionType = actionType;

			return actionType;
		}

		/*
		 * Draw all widgets, starting with the one given by the parameters.
		 * @param w First widget of the chain to draw.
		 */
		internal static void GUI_Widget_DrawAll(Widget w)
		{
			while (w != null)
			{
				CWidget.GUI_Widget_Draw(w);
				w = CWidget.GUI_Widget_GetNext(w);
			}
		}

		/*
		 * Draw a sprite button widget to the display, relative to 0,0.
		 *
		 * @param w The widget (which is a button) to draw.
		 */
		internal static void GUI_Widget_SpriteButton_Draw(Widget w)
		{
			Screen oldScreenID;
			ushort positionX, positionY;
			ushort width, height;
			ushort spriteID;
			bool buttonDown;

			if (w == null) return;

			spriteID = 0;
			if (CUnit.g_unitSelected != null)
			{
				UnitInfo ui;

				ui = g_table_unitInfo[CUnit.g_unitSelected.o.type];

				spriteID = ui.o.spriteID;
			}
			else
			{
				StructureInfo si;
				Structure s;

				s = CStructure.Structure_Get_ByPackedTile(Gui.g_selectionPosition);
				if (s == null) return;
				si = g_table_structureInfo[s.o.type];

				spriteID = si.o.spriteID;
			}

			oldScreenID = Screen.ACTIVE;
			if (Gfx.GFX_Screen_IsActive(Screen.NO0))
			{
				oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);
			}

			buttonDown = w.state.hover2;

			positionX = (ushort)w.offsetX;
			positionY = (ushort)w.offsetY;
			width = w.width;
			height = w.height;

			Gui.GUI_DrawWiredRectangle((ushort)(positionX - 1), (ushort)(positionY - 1), (ushort)(positionX + width), (ushort)(positionY + height), 12);

			Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[spriteID], (short)positionX, (short)positionY, 0, Gui.DRAWSPRITE_FLAG_REMAP, Gfx.g_paletteMapping1, (short)(buttonDown ? 1 : 0));

			Gui.GUI_DrawBorder(positionX, positionY, width, height, (ushort)(buttonDown ? 0 : 1), false);

			if (oldScreenID != Screen.NO0) return;

			Gui.GUI_Mouse_Hide_InRegion((ushort)(positionX - 1), (ushort)(positionY - 1), (ushort)(positionX + width + 1), (ushort)(positionY + height + 1));
			Gfx.GFX_Screen_Copy2((short)(positionX - 1), (short)(positionY - 1), (short)(positionX - 1), (short)(positionY - 1), (short)(width + 2), (short)(height + 2), Screen.NO1, Screen.NO0, false);
			Gui.GUI_Mouse_Show_InRegion();

			Gfx.GFX_Screen_SetActive(Screen.NO0);
		}

		/*
		 * Draw a text button widget to the display, relative to 0,0.
		 *
		 * @param w The widget (which is a button) to draw.
		 */
		internal static void GUI_Widget_TextButton2_Draw(Widget w)
		{
			Screen oldScreenID;
			ushort stringID;
			ushort positionX, positionY;
			ushort width, height;
			byte colour;
			bool buttonSelected;
			bool buttonDown;

			if (w == null) return;

			oldScreenID = Screen.ACTIVE;
			if (Gfx.GFX_Screen_IsActive(Screen.NO0))
			{
				oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);
			}

			stringID = w.stringID;

			buttonSelected = w.state.selected;
			buttonDown = w.state.hover2;

			positionX = (ushort)w.offsetX;
			positionY = (ushort)w.offsetY;
			width = w.width;
			height = w.height;

			Gui.GUI_DrawWiredRectangle((ushort)(positionX - 1), (ushort)(positionY - 1), (ushort)(positionX + width), (ushort)(positionY + height), 12);
			Gui.GUI_DrawBorder(positionX, positionY, width, height, (ushort)(buttonDown ? 0 : 1), true);

			colour = 0xF;
			if (buttonSelected)
			{
				colour = 0x6;
			}
			else if (buttonDown)
			{
				colour = 0xE;
			}

			if (!buttonDown && stringID == (ushort)Text.STR_REPAIR)
			{
				colour = 0xEF;
			}

			Gui.GUI_DrawText_Wrapper(CString.String_Get_ByIndex(stringID), (short)(positionX + width / 2), (short)(positionY + 1), colour, 0, 0x121);

			w.shortcut = CWidget.GUI_Widget_GetShortcut((byte)CString.String_Get_ByIndex(stringID)[0]);

			if (oldScreenID != Screen.NO0) return;

			Gui.GUI_Mouse_Hide_InRegion((ushort)(positionX - 1), (ushort)(positionY - 1), (ushort)(positionX + width + 1), (ushort)(positionY + height + 1));
			Gfx.GFX_Screen_Copy2((short)(positionX - 1), (short)(positionY - 1), (short)(positionX - 1), (short)(positionY - 1), (short)(width + 2), (short)(height + 2), Screen.NO1, Screen.NO0, false);
			Gui.GUI_Mouse_Show_InRegion();

			Gfx.GFX_Screen_SetActive(Screen.NO0);
		}

		/*
		 * Draw a sprite/text button widget to the display, relative to 0,0.
		 *
		 * @param w The widget (which is a button) to draw.
		 */
		internal static void GUI_Widget_SpriteTextButton_Draw(Widget w)
		{
			Screen oldScreenID;
			Structure s;
			ushort positionX, positionY;
			ushort width, height;
			ushort spriteID;
			ushort percentDone;
			bool buttonDown;

			if (w == null) return;

			spriteID = 0;
			percentDone = 0;

			s = CStructure.Structure_Get_ByPackedTile(Gui.g_selectionPosition);
			if (s == null) return;

			Gui.GUI_UpdateProductionStringID();

			oldScreenID = Screen.ACTIVE;
			if (Gfx.GFX_Screen_IsActive(Screen.NO0))
			{
				oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);
			}

			buttonDown = w.state.hover2;

			positionX = (ushort)w.offsetX;
			positionY = (ushort)w.offsetY;
			width = w.width;
			height = w.height;

			Gui.GUI_DrawWiredRectangle((ushort)(positionX - 1), (ushort)(positionY - 1), (ushort)(positionX + width), (ushort)(positionY + height), 12);
			Gui.GUI_DrawBorder(positionX, positionY, width, height, (ushort)(buttonDown ? 0 : 1), true);

			switch ((Text)Gui.g_productionStringID)
			{
				case Text.STR_LAUNCH:
					spriteID = 0x1E;
					break;

				case Text.STR_FREMEN:
					spriteID = 0x5E;
					break;

				case Text.STR_SABOTEUR:
					spriteID = 0x60;
					break;

				case Text.STR_UPGRADINGD_DONE:
				default:
					spriteID = 0x0;
					break;

				case Text.STR_PLACE_IT:
				case Text.STR_COMPLETED:
				case Text.STR_ON_HOLD:
				case Text.STR_BUILD_IT:
				case Text.STR_D_DONE:
					if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
					{
						StructureInfo si;
						ushort spriteWidth;
						ushort x, y;
						byte[] sprite;

						Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[63], (short)(positionX + 37), (short)(positionY + 5), 0, Gui.DRAWSPRITE_FLAG_REMAP, Gfx.g_paletteMapping1, (short)(buttonDown ? 2 : 0));

						sprite = Sprites.g_sprites[24];
						spriteWidth = (ushort)(Sprites.Sprite_GetWidth(sprite) + 1);

						si = g_table_structureInfo[s.objectType];

						for (y = 0; y < g_table_structure_layoutSize[si.layout].height; y++)
						{
							for (x = 0; x < g_table_structure_layoutSize[si.layout].width; x++)
							{
								Gui.GUI_DrawSprite(Screen.ACTIVE, sprite, (short)(positionX + x * spriteWidth + 38), (short)(positionY + y * spriteWidth + 6), 0, 0);
							}
						}

						spriteID = si.o.spriteID;
					}
					else
					{
						UnitInfo ui;

						ui = g_table_unitInfo[s.objectType];
						spriteID = ui.o.spriteID;
					}
					break;
			}

			if (spriteID != 0) Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[spriteID], (short)(positionX + 2), (short)(positionY + 2), 0, Gui.DRAWSPRITE_FLAG_REMAP, Gfx.g_paletteMapping1, (short)(buttonDown ? 1 : 0));

			if (Gui.g_productionStringID == (ushort)Text.STR_D_DONE)
			{
				ushort buildTime;
				ushort timeLeft;

				if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
				{
					StructureInfo si;

					si = g_table_structureInfo[s.objectType];
					buildTime = si.o.buildTime;
				}
				else if (s.o.type == (byte)StructureType.STRUCTURE_REPAIR)
				{
					UnitInfo ui;

					if (s.o.linkedID == 0xFF) return;

					ui = g_table_unitInfo[PoolUnit.Unit_Get_ByIndex(s.o.linkedID).o.type];
					buildTime = ui.o.buildTime;
				}
				else
				{
					UnitInfo ui;

					ui = g_table_unitInfo[s.objectType];
					buildTime = ui.o.buildTime;
				}

				timeLeft = (ushort)(buildTime - (s.countDown + 255) / 256);
				percentDone = (ushort)(100 * timeLeft / buildTime);
			}

			if (Gui.g_productionStringID == (ushort)Text.STR_UPGRADINGD_DONE)
			{
				percentDone = (ushort)(100 - s.upgradeTimeLeft);

				Gui.GUI_DrawText_Wrapper(
					CString.String_Get_ByIndex(Gui.g_productionStringID),
					(short)(positionX + 1),
					(short)(positionY + height - 19),
					(byte)(buttonDown ? 0xE : 0xF),
					0,
					0x021,
					percentDone
				);
			}
			else
			{
				Gui.GUI_DrawText_Wrapper(
					CString.String_Get_ByIndex(Gui.g_productionStringID),
					(short)(positionX + width / 2),
					(short)(positionY + height - 9),
					(byte)((Gui.g_productionStringID == (ushort)Text.STR_PLACE_IT) ? 0xEF : (buttonDown ? 0xE : 0xF)),
					0,
					0x121,
					percentDone
				);
			}

			if (Gui.g_productionStringID == (ushort)Text.STR_D_DONE || Gui.g_productionStringID == (ushort)Text.STR_UPGRADINGD_DONE)
			{
				w.shortcut = CWidget.GUI_Widget_GetShortcut((byte)CString.String_Get_ByIndex(Text.STR_ON_HOLD)[0]);
			}
			else
			{
				w.shortcut = CWidget.GUI_Widget_GetShortcut((byte)CString.String_Get_ByIndex(Gui.g_productionStringID)[0]);
			}

			if (oldScreenID != Screen.NO0) return;

			Gui.GUI_Mouse_Hide_InRegion((ushort)(positionX - 1), (ushort)(positionY - 1), (ushort)(positionX + width + 1), (ushort)(positionY + height + 1));
			Gfx.GFX_Screen_Copy2((short)(positionX - 1), (short)(positionY - 1), (short)(positionX - 1), (short)(positionY - 1), (short)(width + 2), (short)(height + 2), Screen.NO1, Screen.NO0, false);
			Gui.GUI_Mouse_Show_InRegion();

			Gfx.GFX_Screen_SetActive(Screen.NO0);
		}

		/*
		 * Draw a text button widget to the display, relative to its parent.
		 *
		 * @param w The widget (which is a button) to draw.
		 */
		internal static void GUI_Widget_TextButton_Draw(Widget w)
		{
			Screen oldScreenID;
			ushort positionX, positionY;
			ushort width, height;
			ushort state;
			byte colour;

			if (w == null) return;

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);

			positionX = (ushort)(w.offsetX + (CWidget.g_widgetProperties[w.parentID].xBase << 3));
			positionY = (ushort)(w.offsetY + CWidget.g_widgetProperties[w.parentID].yBase);
			width = w.width;
			height = w.height;

			CWidget.g_widgetProperties[19].xBase = (ushort)(positionX >> 3);
			CWidget.g_widgetProperties[19].yBase = positionY;
			CWidget.g_widgetProperties[19].width = (ushort)(width >> 3);
			CWidget.g_widgetProperties[19].height = height;

			state = (ushort)(w.state.selected ? 0 : 2);
			colour = (byte)(w.state.hover2 ? 231 : 232);

			GUI_Widget_DrawBorder(19, state, true/*1*/);

			if (w.stringID == (ushort)Text.STR_CANCEL || w.stringID == (ushort)Text.STR_PREVIOUS || w.stringID == (ushort)Text.STR_YES || w.stringID == (ushort)Text.STR_NO)
			{
				Gui.GUI_DrawText_Wrapper(Gui.GUI_String_Get_ByIndex((short)w.stringID), (short)(positionX + (width / 2)), (short)(positionY + 2), colour, 0, 0x122);
			}
			else
			{
				Gui.GUI_DrawText_Wrapper(Gui.GUI_String_Get_ByIndex((short)w.stringID), (short)(positionX + 3), (short)(positionY + 2), colour, 0, 0x22);
			}

			if (oldScreenID == Screen.NO0)
			{
				Gui.GUI_Mouse_Hide_InRegion(positionX, positionY, (ushort)(positionX + width), (ushort)(positionY + height));
				Gui.GUI_Screen_Copy((short)(positionX >> 3), (short)positionY, (short)(positionX >> 3), (short)positionY, (short)(width >> 3), (short)height, Screen.NO1, Screen.NO0);
				Gui.GUI_Mouse_Show_InRegion();
			}

			Gfx.GFX_Screen_SetActive(oldScreenID);
		}

		/*
		 * Draw a scrollbar widget to the display, relative to its parent.
		 *
		 * @param w The widget (which is a scrollbar) to draw.
		 */
		internal static void GUI_Widget_Scrollbar_Draw(Widget w)
		{
			WidgetScrollbar scrollbar;
			ushort positionX, positionY;
			ushort width, height;
			ushort scrollLeft, scrollTop;
			ushort scrollRight, scrollBottom;

			if (w == null) return;
			if (w.flags.invisible) return;

			scrollbar = (WidgetScrollbar)w.data;

			width = w.width;
			height = w.height;

			positionX = (ushort)w.offsetX;
			if (w.offsetX < 0) positionX += (ushort)(CWidget.g_widgetProperties[w.parentID].width << 3);
			positionX += (ushort)(CWidget.g_widgetProperties[w.parentID].xBase << 3);

			positionY = (ushort)w.offsetY;
			if (w.offsetY < 0) positionY += CWidget.g_widgetProperties[w.parentID].height;
			positionY += CWidget.g_widgetProperties[w.parentID].yBase;

			if (width > height)
			{
				scrollLeft = (ushort)(scrollbar.position + 1);
				scrollTop = 1;
				scrollRight = (ushort)(scrollLeft + scrollbar.size - 1);
				scrollBottom = (ushort)(height - 2);
			}
			else
			{
				scrollLeft = 1;
				scrollTop = (ushort)(scrollbar.position + 1);
				scrollRight = (ushort)(width - 2);
				scrollBottom = (ushort)(scrollTop + scrollbar.size - 1);
			}

			if (Gfx.GFX_Screen_IsActive(Screen.NO0))
			{
				Gui.GUI_Mouse_Hide_InRegion(positionX, positionY, (ushort)(positionX + width - 1), (ushort)(positionY + height - 1));
			}

			/* Draw background */
			Gui.GUI_DrawFilledRectangle((short)positionX, (short)positionY, (short)(positionX + width - 1), (short)(positionY + height - 1), w.bgColourNormal);

			/* Draw where we currently are */
			Gui.GUI_DrawFilledRectangle((short)(positionX + scrollLeft), (short)(positionY + scrollTop), (short)(positionX + scrollRight), (short)(positionY + scrollBottom), (scrollbar.pressed == 0) ? w.fgColourNormal : w.fgColourSelected);

			if (Gfx.GFX_Screen_IsActive(Screen.NO0))
			{
				Gui.GUI_Mouse_Show_InRegion();
			}

			/* Call custom callback function if set */
			scrollbar.drawProc?.Invoke(w);

			scrollbar.dirty = 0;
		}
	}
}
