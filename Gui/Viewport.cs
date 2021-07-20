/* Viewport */

using SharpDune.Audio;
using SharpDune.Input;
using System;
using static System.Math;

namespace SharpDune.Gui
{
	class Viewport
	{
		static uint s_tickCursor;                                 /*!< Stores last time Viewport changed the cursor spriteID. */
		static uint s_tickMapScroll;                              /*!< Stores last time Viewport ran MapScroll function. */
		static uint s_tickClick;                                  /*!< Stores last time Viewport handled a click. */

		/*
		 * Redraw the whole map.
		 *
		 * @param screenID To which screen we should draw the map. Can only be NO0 or NO1. Any non-zero is forced to NO1.
		 */
		internal static void GUI_Widget_Viewport_RedrawMap(Screen screenID)
		{
			var oldScreenID = Screen.NO1;
			ushort i;

			if (screenID == Screen.NO0) oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);

			for (i = 0; i < 4096; i++) GUI_Widget_Viewport_DrawTile(i);

			Map.Map_UpdateMinimapPosition(Gui.g_minimapPosition, true);

			if (screenID != Screen.NO0) return;

			Gfx.GFX_Screen_SetActive(oldScreenID);

			Gui.GUI_Mouse_Hide_InWidget(3);
			Gui.GUI_Screen_Copy(32, 136, 32, 136, 8, 64, Screen.NO1, Screen.NO0);
			Gui.GUI_Mouse_Show_InWidget();
		}

		/*
		 * Draw a single tile on the screen.
		 *
		 * @param packed The tile to draw.
		 */
		internal static bool GUI_Widget_Viewport_DrawTile(ushort packed)
		{
			ushort x;
			ushort y;
			ushort colour;
			ushort spriteID;
			ushort mapScale;

			colour = 12;
			spriteID = 0xFFFF;

			if (CTile.Tile_IsOutOfMap(packed) || !Map.Map_IsValidPosition(packed)) return false;

			mapScale = (ushort)(CScenario.g_scenario.mapScale + 1);

			if (mapScale == 0 || Tools.BitArray_Test(Map.g_displayedMinimap, packed)) return false;

			if ((Map.g_map[packed].isUnveiled && CHouse.g_playerHouse.flags.radarActivated) || CSharpDune.g_debugScenario)
			{
				var type = Map.Map_GetLandscapeType(packed);
				Unit u;

				if (mapScale > 1)
				{
					spriteID = (ushort)(CScenario.g_scenario.mapScale + Map.g_table_landscapeInfo[type].spriteID - 1);
				}
				else
				{
					colour = Map.g_table_landscapeInfo[type].radarColour;
				}

				if (Map.g_table_landscapeInfo[type].radarColour == 0xFFFF)
				{
					if (mapScale > 1)
					{
						spriteID = (ushort)(mapScale + Map.g_map[packed].houseID * 2 + 29);
					}
					else
					{
						colour = CHouse.g_table_houseInfo[Map.g_map[packed].houseID].minimapColor;
					}
				}

				u = CUnit.Unit_Get_ByPackedTile(packed);

				if (u != null)
				{
					if (mapScale > 1)
					{
						if (u.o.type == (byte)UnitType.UNIT_SANDWORM)
						{
							spriteID = (ushort)(mapScale + 53);
						}
						else
						{
							spriteID = (ushort)(mapScale + CUnit.Unit_GetHouseID(u) * 2 + 29);
						}
					}
					else
					{
						if (u.o.type == (byte)UnitType.UNIT_SANDWORM)
						{
							colour = 255;
						}
						else
						{
							colour = CHouse.g_table_houseInfo[CUnit.Unit_GetHouseID(u)].minimapColor;
						}
					}
				}
			}
			else
			{
				Structure s;

				s = CStructure.Structure_Get_ByPackedTile(packed);

				if (s != null && s.o.houseID == (byte)CHouse.g_playerHouseID)
				{
					if (mapScale > 1)
					{
						spriteID = (ushort)(mapScale + s.o.houseID * 2 + 29);
					}
					else
					{
						colour = CHouse.g_table_houseInfo[s.o.houseID].minimapColor;
					}
				}
				else
				{
					if (mapScale > 1)
					{
						spriteID = (ushort)(CScenario.g_scenario.mapScale + Map.g_table_landscapeInfo[(int)LandscapeType.LST_ENTIRELY_MOUNTAIN].spriteID - 1);
					}
					else
					{
						colour = 12;
					}
				}
			}

			x = CTile.Tile_GetPackedX(packed);
			y = CTile.Tile_GetPackedY(packed);

			x -= Map.g_mapInfos[CScenario.g_scenario.mapScale].minX;
			y -= Map.g_mapInfos[CScenario.g_scenario.mapScale].minY;

			if (spriteID != 0xFFFF)
			{
				x *= (ushort)(CScenario.g_scenario.mapScale + 1);
				y *= (ushort)(CScenario.g_scenario.mapScale + 1);
				Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[spriteID], (short)x, (short)y, 3, Gui.DRAWSPRITE_FLAG_WIDGETPOS);
			}
			else
			{
				Gfx.GFX_PutPixel((ushort)(x + 256), (ushort)(y + 136), (byte)(colour & 0xFF));
			}
			return true;
		}

		static readonly ushort[][] values_32A4 = { //[8][2]	/* index, flag passed to GUI_DrawSprite() */
			new ushort[] {0, 0}, new ushort[] {1, 0}, new ushort[] {2, 0}, new ushort[] {3, 0},
			new ushort[] {4, 0}, new ushort[] {3, 1}, new ushort[] {2, 1}, new ushort[] {1, 1}
		};
		static readonly ushort[][] values_32C4 = { //[8][2]	/* index, flag */
			new ushort[] {0, 0}, new ushort[] {1, 0}, new ushort[] {1, 0}, new ushort[] {1, 0},
			new ushort[] {2, 0}, new ushort[] {1, 1}, new ushort[] {1, 1}, new ushort[] {1, 1}
		};
		static readonly ushort[] values_334A = { 0, 1, 0, 2 };
		static readonly short[][] values_334E = { //[8][2]
			new short[] {0, 7},  new short[] {-7,  6}, new short[] {-14, 1}, new short[] {-9, -6},
			new short[] {0, -9}, new short[] { 9, -6}, new short[] { 14, 1}, new short[] { 7,  6}
		};
		static readonly short[][] values_336E = { //[8][2]
			new short[] { 0, -5}, new short[] { 0, -5}, new short[] { 2, -3}, new short[] { 2, -1},
			new short[] {-1, -3}, new short[] {-2, -1}, new short[] {-2, -3}, new short[] {-1, -5}
		};
		static readonly short[][] values_338E = { //[8][2]
			new short[] { 0, -4}, new short[] {-1, -3}, new short[] { 2, -4}, new short[] {0, -3},
			new short[] {-1, -3}, new short[] { 0, -3}, new short[] {-2, -4}, new short[] {1, -3}
		};
		static readonly ushort[][] values_32E4 = { //[8][2]
			new ushort[] {0, 0}, new ushort[] {1, 0}, new ushort[] {2, 0}, new ushort[] {1, 2},
			new ushort[] {0, 2}, new ushort[] {1, 3}, new ushort[] {2, 1}, new ushort[] {1, 1}
		};
		static readonly ushort[][] values_3304 = { //[16][2]
			new ushort[] {0, 0}, new ushort[] {1, 0}, new ushort[] {2, 0}, new ushort[] {3, 0},
			new ushort[] {4, 0}, new ushort[] {3, 2}, new ushort[] {2, 2}, new ushort[] {1, 2},
			new ushort[] {0, 2}, new ushort[] {3, 3}, new ushort[] {2, 3}, new ushort[] {3, 3},
			new ushort[] {4, 1}, new ushort[] {3, 1}, new ushort[] {2, 1}, new ushort[] {1, 1}
		};
		static readonly ushort[] values_33AE = { 2, 1, 0, 1 };
		/*
		 * Redraw parts of the viewport that require redrawing.
		 *
		 * @param forceRedraw If true, dirty flags are ignored, and everything is drawn.
		 * @param hasScrolled Viewport position has changed
		 * @param drawToMainScreen True if and only if we are drawing to the main screen and not some buffer screen.
		 */
		internal static void GUI_Widget_Viewport_Draw(bool forceRedraw, bool hasScrolled, bool drawToMainScreen)
		{
			var paletteHouse = new byte[16]; /*!< Used for palette manipulation to get housed coloured units etc. */
			paletteHouse[0] = 0;
			ushort x;
			ushort y;
			ushort i;
			ushort curPos;
			bool updateDisplay;
			Screen oldScreenID;
			ushort oldWidgetID;
			var minX = new short[10];
			var maxX = new short[10];

			var find = new PoolFindStruct();

			updateDisplay = forceRedraw;

			Array.Fill<short>(minX, 0xF, 0, minX.Length); //memset(minX, 0xF, sizeof(minX));
			//Array.Fill<short>(maxX, 0,   0, minX.Length); //memset(maxX, 0,   sizeof(minX));

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);

			oldWidgetID = CWidget.Widget_SetCurrentWidget(2);

			if (Map.g_dirtyViewportCount != 0 || forceRedraw)
			{
				for (y = 0; y < 10; y++)
				{
					var top = (ushort)((y << 4) + 0x28); /* 40 */
					for (x = 0; x < (drawToMainScreen ? 15 : 16); x++)
					{
						Tile t;
						ushort left;

						curPos = (ushort)(Gui.g_viewportPosition + CTile.Tile_PackXY(x, y));

						if (x < 15 && !forceRedraw && Tools.BitArray_Test(Map.g_dirtyViewport, curPos))
						{
							if (maxX[y] < x) maxX[y] = (short)x;
							if (minX[y] > x) minX[y] = (short)x;
							updateDisplay = true;
						}

						if (!Tools.BitArray_Test(Map.g_dirtyMinimap, curPos) && !forceRedraw) continue;

						Tools.BitArray_Set(Map.g_dirtyViewport, curPos);

						if (x < 15)
						{
							updateDisplay = true;
							if (maxX[y] < x) maxX[y] = (short)x;
							if (minX[y] > x) minX[y] = (short)x;
						}

						t = Map.g_map[curPos];
						left = (ushort)(x << 4);

						if (!CSharpDune.g_debugScenario && Sprites.g_veiledTileID == t.overlayTileID)
						{
							/* draw a black rectangle */
							Gui.GUI_DrawFilledRectangle((short)left, (short)top, (short)(left + 15), (short)(top + 15), 12);
							continue;
						}

						Gfx.GFX_DrawTile(t.groundTileID, left, top, t.houseID);

						if (t.overlayTileID != 0 && !CSharpDune.g_debugScenario)
						{
							Gfx.GFX_DrawTile(t.overlayTileID, left, top, t.houseID);
						}
					}
				}
				Map.g_dirtyViewportCount = 0;
			}

			/* Draw Sandworm */
			find.type = (ushort)UnitType.UNIT_SANDWORM;
			find.index = 0xFFFF;
			find.houseID = (byte)HouseType.HOUSE_INVALID;

			while (true)
			{
				Unit u;
				byte[] sprite;

				u = CUnit.Unit_Find(find);

				if (u == null) break;

				if (!u.o.flags.isDirty && !forceRedraw) continue;
				u.o.flags.isDirty = false;

				if (!Map.g_map[CTile.Tile_PackTile(u.o.position)].isUnveiled && !CSharpDune.g_debugScenario) continue;

				sprite = Sprites.g_sprites[CUnit.g_table_unitInfo[u.o.type].groundSpriteID];
				GUI_Widget_Viewport_GetSprite_HousePalette(sprite, CUnit.Unit_GetHouseID(u), paletteHouse);

				if (Map.Map_IsPositionInViewport(u.o.position, out x, out y))
				{
					Gui.GUI_DrawSprite(Screen.ACTIVE, sprite, (short)x, (short)y, 2, Gui.DRAWSPRITE_FLAG_BLUR | Gui.DRAWSPRITE_FLAG_WIDGETPOS | Gui.DRAWSPRITE_FLAG_CENTER);
				}
				if (Map.Map_IsPositionInViewport(u.targetLast, out x, out y))
				{
					Gui.GUI_DrawSprite(Screen.ACTIVE, sprite, (short)x, (short)y, 2, Gui.DRAWSPRITE_FLAG_BLUR | Gui.DRAWSPRITE_FLAG_WIDGETPOS | Gui.DRAWSPRITE_FLAG_CENTER);
				}
				if (Map.Map_IsPositionInViewport(u.targetPreLast, out x, out y))
				{
					Gui.GUI_DrawSprite(Screen.ACTIVE, sprite, (short)x, (short)y, 2, Gui.DRAWSPRITE_FLAG_BLUR | Gui.DRAWSPRITE_FLAG_WIDGETPOS | Gui.DRAWSPRITE_FLAG_CENTER);
				}
				if (u == CUnit.g_unitSelected && Map.Map_IsPositionInViewport(u.o.position, out x, out y))
				{
					Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[6], (short)x, (short)y, 2, Gui.DRAWSPRITE_FLAG_WIDGETPOS | Gui.DRAWSPRITE_FLAG_CENTER);
				}
			}

			if (CUnit.g_unitSelected == null && (Map.g_selectionRectangleNeedRepaint || hasScrolled) && (CStructure.Structure_Get_ByPackedTile(Gui.g_selectionRectanglePosition) != null || CSharpDune.g_selectionType == (ushort)SelectionType.PLACE || CSharpDune.g_debugScenario))
			{
				var x1 = (ushort)((CTile.Tile_GetPackedX(Gui.g_selectionRectanglePosition) - CTile.Tile_GetPackedX(Gui.g_minimapPosition)) << 4);
				var y1 = (ushort)(((CTile.Tile_GetPackedY(Gui.g_selectionRectanglePosition) - CTile.Tile_GetPackedY(Gui.g_minimapPosition)) << 4) + 0x28);
				var x2 = (ushort)(x1 + (Gui.g_selectionWidth << 4) - 1);
				var y2 = (ushort)(y1 + (Gui.g_selectionHeight << 4) - 1);

				Gui.GUI_SetClippingArea(0, 40, 239, Gfx.SCREEN_HEIGHT - 1);
				Gui.GUI_DrawWiredRectangle(x1, y1, x2, y2, 0xFF);

				if (Gui.g_selectionState == 0 && CSharpDune.g_selectionType == (ushort)SelectionType.PLACE)
				{
					Gui.GUI_DrawLine((short)x1, (short)y1, (short)x2, (short)y2, 0xFF);
					Gui.GUI_DrawLine((short)x2, (short)y1, (short)x1, (short)y2, 0xFF);
				}

				Gui.GUI_SetClippingArea(0, 0, Gfx.SCREEN_WIDTH - 1, Gfx.SCREEN_HEIGHT - 1);

				Map.g_selectionRectangleNeedRepaint = false;
			}

			/* Draw ground units */
			if (CUnit.g_dirtyUnitCount != 0 || forceRedraw || updateDisplay)
			{
				find.type = 0xFFFF;
				find.index = 0xFFFF;
				find.houseID = (byte)HouseType.HOUSE_INVALID;

				while (true)
				{
					Unit u;
					UnitInfo ui;
					ushort packed;
					byte orientation;
					ushort index;
					ushort spriteFlags = 0;

					u = CUnit.Unit_Find(find);

					if (u == null) break;

					if (u.o.index < 20 || u.o.index > 101) continue;

					packed = CTile.Tile_PackTile(u.o.position);

					if ((!u.o.flags.isDirty || u.o.flags.isNotOnMap) && !forceRedraw && !Tools.BitArray_Test(Map.g_dirtyViewport, packed)) continue;
					u.o.flags.isDirty = false;

					if (!Map.g_map[packed].isUnveiled && !CSharpDune.g_debugScenario) continue;

					ui = CUnit.g_table_unitInfo[u.o.type];

					if (!Map.Map_IsPositionInViewport(u.o.position, out x, out y)) continue;

					x += Map.g_table_tilediff[0][u.wobbleIndex].x;
					y += Map.g_table_tilediff[0][u.wobbleIndex].y;

					orientation = CTile.Orientation_Orientation256ToOrientation8((byte)u.orientation[0].current);

					if (u.spriteOffset >= 0 || ui.destroyedSpriteID == 0)
					{
						index = ui.groundSpriteID;

						switch ((DisplayMode)ui.displayMode)
						{
							case DisplayMode.UNIT:
							case DisplayMode.ROCKET:
								if (ui.movementType == (ushort)MovementType.MOVEMENT_SLITHER) break;
								index += values_32A4[orientation][0];
								spriteFlags = values_32A4[orientation][1];
								break;

							case DisplayMode.INFANTRY_3_FRAMES:
								{
									index += (ushort)(values_32C4[orientation][0] * 3); //[orientation][0]
									index += values_334A[u.spriteOffset & 3];
									spriteFlags = values_32C4[orientation][1]; //[orientation][1]
								}
								break;

							case DisplayMode.INFANTRY_4_FRAMES:
								index += (ushort)(values_32C4[orientation][0] * 4); //[orientation][0]
								index += (ushort)(u.spriteOffset & 3);
								spriteFlags = values_32C4[orientation][1]; //[orientation][1]
								break;

							default:
								spriteFlags = 0;
								break;
						}
					}
					else
					{
						index = (ushort)(ui.destroyedSpriteID - u.spriteOffset - 1);
						spriteFlags = 0;
					}

					if (u.o.type != (byte)UnitType.UNIT_SANDWORM && u.o.flags.isHighlighted) spriteFlags |= Gui.DRAWSPRITE_FLAG_REMAP;
					if (ui.o.flags.blurTile) spriteFlags |= Gui.DRAWSPRITE_FLAG_BLUR;

					spriteFlags |= Gui.DRAWSPRITE_FLAG_WIDGETPOS | Gui.DRAWSPRITE_FLAG_CENTER;

					if (GUI_Widget_Viewport_GetSprite_HousePalette(Sprites.g_sprites[index], (u.deviated != 0) ? u.deviatedHouse : CUnit.Unit_GetHouseID(u), paletteHouse))
					{
						spriteFlags |= Gui.DRAWSPRITE_FLAG_PAL;
						Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[index], (short)x, (short)y, 2, spriteFlags, paletteHouse, Gfx.g_paletteMapping2, (short)1);
					}
					else
					{
						Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[index], (short)x, (short)y, 2, spriteFlags, Gfx.g_paletteMapping2, 1);
					}

					if (u.o.type == (byte)UnitType.UNIT_HARVESTER && u.actionID == (byte)ActionType.ACTION_HARVEST && u.spriteOffset >= 0 && (u.actionID == (byte)ActionType.ACTION_HARVEST || u.actionID == (byte)ActionType.ACTION_MOVE))
					{
						var type = Map.Map_GetLandscapeType(packed);
						if (type == (ushort)LandscapeType.LST_SPICE || type == (ushort)LandscapeType.LST_THICK_SPICE)
						{
							/*GUI_Widget_Viewport_GetSprite_HousePalette(..., Unit_GetHouseID(u), paletteHouse),*/
							Gui.GUI_DrawSprite(Screen.ACTIVE,
										   Sprites.g_sprites[(u.spriteOffset % 3) + 0xDF + (values_32A4[orientation][0] * 3)],
										   (short)(x + values_334E[orientation][0]), (short)(y + values_334E[orientation][1]),
										   2, values_32A4[orientation][1] | Gui.DRAWSPRITE_FLAG_WIDGETPOS | Gui.DRAWSPRITE_FLAG_CENTER);
						}
					}

					if (u.spriteOffset >= 0 && ui.turretSpriteID != 0xFFFF)
					{
						short offsetX = 0;
						short offsetY = 0;
						var spriteID = ui.turretSpriteID;

						orientation = CTile.Orientation_Orientation256ToOrientation8((byte)u.orientation[ui.o.flags.hasTurret ? 1 : 0].current);

						switch (ui.turretSpriteID)
						{
							case 0x8D: /* sonic tank */
								offsetY = -2;
								break;

							case 0x92: /* rocket launcher */
								offsetY = -3;
								break;

							case 0x7E:
								{ /* siege tank */
									offsetX = values_336E[orientation][0];
									offsetY = values_336E[orientation][1];
								}
								break;

							case 0x88:
								{ /* devastator */
									offsetX = values_338E[orientation][0];
									offsetY = values_338E[orientation][1];
								}
								break;

							default:
								break;
						}

						spriteID += values_32A4[orientation][0];

						if (GUI_Widget_Viewport_GetSprite_HousePalette(Sprites.g_sprites[spriteID], CUnit.Unit_GetHouseID(u), paletteHouse))
						{
							Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[spriteID],
										   (short)(x + offsetX), (short)(y + offsetY),
										   2, values_32A4[orientation][1] | Gui.DRAWSPRITE_FLAG_WIDGETPOS | Gui.DRAWSPRITE_FLAG_CENTER | Gui.DRAWSPRITE_FLAG_PAL, paletteHouse);
						}
						else
						{
							Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[spriteID],
										   (short)(x + offsetX), (short)(y + offsetY),
										   2, values_32A4[orientation][1] | Gui.DRAWSPRITE_FLAG_WIDGETPOS | Gui.DRAWSPRITE_FLAG_CENTER);
						}
					}

					if (u.o.flags.isSmoking)
					{
						var spriteID = (ushort)(180 + (u.spriteOffset & 3));
						if (spriteID == 183) spriteID = 181;

						Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[spriteID], (short)x, (short)(y - 14), 2, Gui.DRAWSPRITE_FLAG_WIDGETPOS | Gui.DRAWSPRITE_FLAG_CENTER);
					}

					if (u != CUnit.g_unitSelected) continue;

					Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[6], (short)x, (short)y, 2, Gui.DRAWSPRITE_FLAG_WIDGETPOS | Gui.DRAWSPRITE_FLAG_CENTER);
				}

				CUnit.g_dirtyUnitCount = 0;
			}

			/* draw explosions */
			for (i = 0; i < CExplosion.EXPLOSION_MAX; i++)
			{
				var e = CExplosion.Explosion_Get_ByIndex(i);

				curPos = CTile.Tile_PackTile(e.position);

				if (Tools.BitArray_Test(Map.g_dirtyViewport, curPos)) e.isDirty = true;

				if (e.commands == null) continue;
				if (!e.isDirty && !forceRedraw) continue;
				if (e.spriteID == 0) continue;

				e.isDirty = false;

				if (!Map.g_map[curPos].isUnveiled && !CSharpDune.g_debugScenario) continue;
				if (!Map.Map_IsPositionInViewport(e.position, out x, out y)) continue;

				/*GUI_Widget_Viewport_GetSprite_HousePalette(g_sprites[e->spriteID], e->houseID, paletteHouse);*/
				Gui.GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[e.spriteID], (short)x, (short)y, 2, Gui.DRAWSPRITE_FLAG_WIDGETPOS | Gui.DRAWSPRITE_FLAG_CENTER/*, paletteHouse*/);
			}

			/* draw air units */
			if (CUnit.g_dirtyAirUnitCount != 0 || forceRedraw || updateDisplay)
			{
				find.type = 0xFFFF;
				find.index = 0xFFFF;
				find.houseID = (byte)HouseType.HOUSE_INVALID;

				while (true)
				{
					Unit u;
					UnitInfo ui;
					byte orientation;
					byte[] sprite;
					ushort index;
					ushort spriteFlags;

					u = CUnit.Unit_Find(find);

					if (u == null) break;

					if (u.o.index > 15) continue;

					curPos = CTile.Tile_PackTile(u.o.position);

					if ((!u.o.flags.isDirty || u.o.flags.isNotOnMap) && !forceRedraw && !Tools.BitArray_Test(Map.g_dirtyViewport, curPos)) continue;
					u.o.flags.isDirty = false;

					if (!Map.g_map[curPos].isUnveiled && !CSharpDune.g_debugScenario) continue;

					ui = CUnit.g_table_unitInfo[u.o.type];

					if (!Map.Map_IsPositionInViewport(u.o.position, out x, out y)) continue;

					index = ui.groundSpriteID;
					orientation = (byte)u.orientation[0].current;
					spriteFlags = Gui.DRAWSPRITE_FLAG_WIDGETPOS | Gui.DRAWSPRITE_FLAG_CENTER;

					switch ((DisplayMode)ui.displayMode)
					{
						case DisplayMode.SINGLE_FRAME:
							if (u.o.flags.bulletIsBig) index++;
							break;

						case DisplayMode.UNIT:
							orientation = CTile.Orientation_Orientation256ToOrientation8(orientation);

							index += values_32E4[orientation][0];
							spriteFlags |= values_32E4[orientation][1];
							break;

						case DisplayMode.ROCKET:
							{
								orientation = CTile.Orientation_Orientation256ToOrientation16(orientation);

								index += values_3304[orientation][0];
								spriteFlags |= values_3304[orientation][1];
							}
							break;

						case DisplayMode.ORNITHOPTER:
							{
								orientation = CTile.Orientation_Orientation256ToOrientation8(orientation);

								index += (ushort)((values_32E4[orientation][0] * 3) + values_33AE[u.spriteOffset & 3]);
								spriteFlags |= values_32E4[orientation][1];
							}
							break;

						default:
							spriteFlags = 0x0;
							break;
					}

					if (ui.flags.hasAnimationSet && u.o.flags.animationFlip) index += 5;
					if (u.o.type == (byte)UnitType.UNIT_CARRYALL && u.o.flags.inTransport) index += 3;

					sprite = Sprites.g_sprites[index];

					if (ui.o.flags.hasShadow)
					{
						Gui.GUI_DrawSprite(Screen.ACTIVE, sprite, (short)(x + 1), (short)(y + 3), 2, (spriteFlags & ~Gui.DRAWSPRITE_FLAG_PAL) | Gui.DRAWSPRITE_FLAG_REMAP | Gui.DRAWSPRITE_FLAG_BLUR, Gfx.g_paletteMapping1, (short)1);
					}
					if (ui.o.flags.blurTile) spriteFlags |= Gui.DRAWSPRITE_FLAG_BLUR;

					if (GUI_Widget_Viewport_GetSprite_HousePalette(sprite, CUnit.Unit_GetHouseID(u), paletteHouse))
					{
						Gui.GUI_DrawSprite(Screen.ACTIVE, sprite, (short)x, (short)y, 2, spriteFlags | Gui.DRAWSPRITE_FLAG_PAL, paletteHouse);
					}
					else
					{
						Gui.GUI_DrawSprite(Screen.ACTIVE, sprite, (short)x, (short)y, 2, spriteFlags);
					}
				}

				CUnit.g_dirtyAirUnitCount = 0;
			}

			if (updateDisplay)
			{
				Array.Fill<byte>(Map.g_dirtyMinimap, 0, 0, Map.g_dirtyMinimap.Length); //memset(g_dirtyMinimap, 0, sizeof(g_dirtyMinimap));
				Array.Fill<byte>(Map.g_dirtyViewport, 0, 0, Map.g_dirtyViewport.Length); //memset(g_dirtyViewport, 0, sizeof(g_dirtyViewport));
			}

			if (Map.g_changedTilesCount != 0)
			{
				var init = false;
				var update = false;
				ushort minY = 0xffff;
				ushort maxY = 0;
				var oldScreenID2 = Screen.NO1;

				for (i = 0; i < Map.g_changedTilesCount; i++)
				{
					curPos = Map.g_changedTiles[i];
					Tools.BitArray_Clear(Map.g_changedTilesMap, curPos);

					if (!init)
					{
						init = true;

						oldScreenID2 = Gfx.GFX_Screen_SetActive(Screen.NO1);

						Gui.GUI_Mouse_Hide_InWidget(3);
					}

					if (GUI_Widget_Viewport_DrawTile(curPos))
					{
						y = (ushort)(CTile.Tile_GetPackedY(curPos) - Map.g_mapInfos[CScenario.g_scenario.mapScale].minY); /* +136 */
						y *= (ushort)(CScenario.g_scenario.mapScale + 1);
						if (y > maxY) maxY = y;
						if (y < minY) minY = y;
					}

					if (!update && Tools.BitArray_Test(Map.g_displayedMinimap, curPos)) update = true;
				}

				if (update) Map.Map_UpdateMinimapPosition(Gui.g_minimapPosition, true);

				if (init)
				{
					if (hasScrolled)
					{   /* force copy of the whole map (could be of the white rectangle) */
						minY = 0;
						maxY = (ushort)(63 - CScenario.g_scenario.mapScale);
					}
					/* MiniMap : redraw only line that changed */
					if (minY < maxY) Gui.GUI_Screen_Copy(32, (short)(136 + minY), 32, (short)(136 + minY), 8, (short)(maxY + 1 + CScenario.g_scenario.mapScale - minY), Screen.ACTIVE, Screen.NO0);

					Gfx.GFX_Screen_SetActive(oldScreenID2);

					Gui.GUI_Mouse_Show_InWidget();
				}

				if (Map.g_changedTilesCount == Map.g_changedTiles.Length)
				{
					Map.g_changedTilesCount = 0;

					for (i = 0; i < 4096; i++)
					{
						if (!Tools.BitArray_Test(Map.g_changedTilesMap, i)) continue;
						Map.g_changedTiles[Map.g_changedTilesCount++] = i;
						if (Map.g_changedTilesCount == Map.g_changedTiles.Length) break;
					}
				}
				else
				{
					Map.g_changedTilesCount = 0;
				}
			}

			if ((Gui.g_viewportMessageCounter & 1) != 0 && Gui.g_viewportMessageText != null && (minX[6] <= 14 || maxX[6] >= 0 || hasScrolled || forceRedraw))
			{
				Gui.GUI_DrawText_Wrapper(Gui.g_viewportMessageText, 112, 139, 15, 0, 0x132);
				minX[6] = -1;
				maxX[6] = 14;
			}

			if (updateDisplay && !drawToMainScreen)
			{
				if (CSharpDune.g_viewport_fadein)
				{
					Gui.GUI_Mouse_Hide_InWidget(CWidget.g_curWidgetIndex);

					/* ENHANCEMENT -- When fading in the game on start, you don't see the fade as it is against the already drawn screen. */
					if (CSharpDune.g_dune2_enhanced)
					{
						var oldScreenID2 = Gfx.GFX_Screen_SetActive(Screen.NO0);
						Gui.GUI_DrawFilledRectangle((short)(CWidget.g_curWidgetXBase << 3), (short)CWidget.g_curWidgetYBase, (short)((CWidget.g_curWidgetXBase + CWidget.g_curWidgetWidth) << 3), (short)(CWidget.g_curWidgetYBase + CWidget.g_curWidgetHeight), 0);
						Gfx.GFX_Screen_SetActive(oldScreenID2);
					}

					Gui.GUI_Screen_FadeIn(CWidget.g_curWidgetXBase, CWidget.g_curWidgetYBase, CWidget.g_curWidgetXBase, CWidget.g_curWidgetYBase, CWidget.g_curWidgetWidth, CWidget.g_curWidgetHeight, Screen.ACTIVE, Screen.NO0);
					Gui.GUI_Mouse_Show_InWidget();

					CSharpDune.g_viewport_fadein = false;
				}
				else
				{
					var init = false;

					for (i = 0; i < 10; i++)
					{
						ushort width;
						ushort height;

						if (hasScrolled)
						{
							minX[i] = 0;
							maxX[i] = 14;
						}

						if (maxX[i] < minX[i]) continue;

						x = (ushort)(minX[i] * 2);
						y = (ushort)((i << 4) + 0x28);
						width = (ushort)((maxX[i] - minX[i] + 1) * 2);
						height = 16;

						if (!init)
						{
							Gui.GUI_Mouse_Hide_InWidget(CWidget.g_curWidgetIndex);

							init = true;
						}

						Gui.GUI_Screen_Copy((short)x, (short)y, (short)x, (short)y, (short)width, (short)height, Screen.ACTIVE, Screen.NO0);
					}

					if (init) Gui.GUI_Mouse_Show_InWidget();
				}
			}

			Gfx.GFX_Screen_SetActive(oldScreenID);

			CWidget.Widget_SetCurrentWidget(oldWidgetID);
		}

		/*
		 * Get palette house of sprite for the viewport
		 *
		 * @param sprite The sprite
		 * @param houseID The House to recolour it with.
		 * @param paletteHouse the palette to set
		 */
		static bool GUI_Widget_Viewport_GetSprite_HousePalette(byte[] sprite, byte houseID, byte[] paletteHouse)
		{
			int i;

			if (sprite == null) return false;

			/* flag 0x1 indicates if the sprite has a palette */
			if ((sprite[0] & 0x1) == 0) return false;

			if (houseID == 0)
			{
				Array.Copy(sprite, 10, paletteHouse, 0, 16); //memcpy(paletteHouse, sprite + 10, 16);
			}
			else
			{
				for (i = 0; i < 16; i++)
				{
					var v = sprite[10 + i];

					if (v >= 0x90 && v <= 0x98)
					{
						v += (byte)(houseID << 4);
					}

					paletteHouse[i] = v;
				}
			}
			return true;
		}

		/* HotSpots for different cursor types. */
		static readonly XYPosition[] cursorHotSpots = {
			new() { x = 0, y = 0 }, new() { x = 5, y = 0 }, new() { x = 8, y = 5 },
			new() { x = 5, y = 8 }, new() { x = 0, y = 5 }, new() { x = 8, y = 8 }
		};
		/*
		 * Handles the Click events for the Viewport widget.
		 *
		 * @param w The widget.
		 */
		internal static bool GUI_Widget_Viewport_Click(Widget w)
		{
			ushort direction;
			ushort x, y;
			ushort spriteID;
			ushort packed;
			bool click, drag;

			spriteID = Gui.g_cursorSpriteID;
			switch (w.index)
			{
				default: break;
				case 39: spriteID = 1; break;
				case 40: spriteID = 2; break;
				case 41: spriteID = 4; break;
				case 42: spriteID = 3; break;
				case 43: spriteID = Gui.g_cursorDefaultSpriteID; break;
				case 44: spriteID = Gui.g_cursorDefaultSpriteID; break;
				case 45: spriteID = 0; break;
			}

			if (spriteID != Gui.g_cursorSpriteID)
			{
				s_tickCursor = Timer.g_timerGame;

				Sprites.Sprites_SetMouseSprite(cursorHotSpots[spriteID].x, cursorHotSpots[spriteID].y, Sprites.g_sprites[spriteID]);

				Gui.g_cursorSpriteID = spriteID;
			}

			if (w.index == 45) return true;

			click = false;
			drag = false;

			if ((w.state.buttonState & 0x11) != 0)
			{
				click = true;
				Gui.g_var_37B8 = false;
			}
			else if ((w.state.buttonState & 0x22) != 0 && !Gui.g_var_37B8)
			{
				drag = true;
			}

			/* ENHANCEMENT -- Dune2 depends on slow CPUs to limit the rate mouse clicks are handled. */
			if (CSharpDune.g_dune2_enhanced && (click || drag))
			{
				if (s_tickClick + 2 >= Timer.g_timerGame) return true;
				s_tickClick = Timer.g_timerGame;
			}

			direction = 0xFFFF;
			switch (w.index)
			{
				default: break;
				case 39: direction = 0; break;
				case 40: direction = 2; break;
				case 41: direction = 6; break;
				case 42: direction = 4; break;
			}

			if (direction != 0xFFFF)
			{
				/* Always scroll if we have a click or a drag */
				if (!click && !drag)
				{
					/* Wait for either one of the timers */
					if (s_tickMapScroll + 10 >= Timer.g_timerGame || s_tickCursor + 20 >= Timer.g_timerGame) return true;
					/* Don't scroll if we have a structure/unit selected and don't want to autoscroll */
					if (Config.g_gameConfig.autoScroll == 0 && (CSharpDune.g_selectionType == (ushort)SelectionType.STRUCTURE || CSharpDune.g_selectionType == (ushort)SelectionType.UNIT)) return true;
				}

				s_tickMapScroll = Timer.g_timerGame;

				Map.Map_MoveDirection(direction);
				return true;
			}

			if (click)
			{
				x = Mouse.g_mouseClickX;
				y = Mouse.g_mouseClickY;
			}
			else
			{
				x = Mouse.g_mouseX;
				y = Mouse.g_mouseY;
			}

			if (w.index == 43)
			{
				x = (ushort)(x / 16 + CTile.Tile_GetPackedX(Gui.g_minimapPosition));
				y = (ushort)((y - 40) / 16 + CTile.Tile_GetPackedY(Gui.g_minimapPosition));
			}
			else if (w.index == 44)
			{
				ushort mapScale;
				MapInfo mapInfo;

				mapScale = CScenario.g_scenario.mapScale;
				mapInfo = Map.g_mapInfos[mapScale];

				x = (ushort)(Min((Max(x, (ushort)256) - 256) / (mapScale + 1), mapInfo.sizeX - 1) + mapInfo.minX);
				y = (ushort)(Min((Max(y, (ushort)136) - 136) / (mapScale + 1), mapInfo.sizeY - 1) + mapInfo.minY);
			}

			packed = CTile.Tile_PackXY(x, y);

			if (click && CSharpDune.g_selectionType == (ushort)SelectionType.TARGET)
			{
				Unit u;
				ActionType action;
				ushort encoded;

				Gui.GUI_DisplayText(null, -1);

				if (CUnit.g_unitHouseMissile != null)
				{
					CUnit.Unit_LaunchHouseMissile(packed);
					return true;
				}

				u = CUnit.g_unitActive;

				action = (ActionType)CSharpDune.g_activeAction;

				CObject.Object_Script_Variable4_Clear(u.o);
				u.targetAttack = 0;
				u.targetMove = 0;
				u.route[0] = 0xFF;

				if (action != ActionType.ACTION_MOVE && action != ActionType.ACTION_HARVEST)
				{
					encoded = Tools.Tools_Index_Encode(CUnit.Unit_FindTargetAround(packed), IndexType.IT_TILE);
				}
				else
				{
					encoded = Tools.Tools_Index_Encode(packed, IndexType.IT_TILE);
				}

				CUnit.Unit_SetAction(u, action);

				if (action == ActionType.ACTION_MOVE)
				{
					CUnit.Unit_SetDestination(u, encoded);
				}
				else if (action == ActionType.ACTION_HARVEST)
				{
					u.targetMove = encoded;
				}
				else
				{
					Unit target;

					CUnit.Unit_SetTarget(u, encoded);
					target = Tools.Tools_Index_GetUnit(u.targetAttack);
					if (target != null) target.blinkCounter = 8;
				}

				if (!Config.g_enableVoices)
				{
					CDriver.Driver_Sound_Play(36, 0xFF);
				}
				else if (CUnit.g_table_unitInfo[u.o.type].movementType == (ushort)MovementType.MOVEMENT_FOOT)
				{
					Sound.Sound_StartSound(CUnit.g_table_actionInfo[(int)action].soundID);
				}
				else
				{
					Sound.Sound_StartSound((ushort)(((Tools.Tools_Random_256() & 0x1) == 0) ? 20 : 17));
				}

				CUnit.g_unitActive = null;
				CSharpDune.g_activeAction = 0xFFFF;

				Gui.GUI_ChangeSelectionType((ushort)SelectionType.UNIT);
				return true;
			}

			if (click && CSharpDune.g_selectionType == (ushort)SelectionType.PLACE)
			{
				StructureInfo si;
				Structure s;
				House h;

				s = CStructure.g_structureActive;
				si = CStructure.g_table_structureInfo[CStructure.g_structureActiveType];
				h = CHouse.g_playerHouse;

				if (CStructure.Structure_Place(s, Gui.g_selectionPosition))
				{
					Sound.Voice_Play(20);

					if (s.o.type == (byte)StructureType.STRUCTURE_PALACE) CHouse.House_Get_ByIndex(s.o.houseID).palacePosition = s.o.position;

					if (CStructure.g_structureActiveType == (ushort)StructureType.STRUCTURE_REFINERY && CSharpDune.g_validateStrictIfZero == 0)
					{
						Unit u;

						CSharpDune.g_validateStrictIfZero++;
						u = CUnit.Unit_CreateWrapper((byte)CHouse.g_playerHouseID, UnitType.UNIT_HARVESTER, Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE));
						CSharpDune.g_validateStrictIfZero--;

						if (u == null)
						{
							h.harvestersIncoming++;
						}
						else
						{
							u.originEncoded = Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);
						}
					}

					Gui.GUI_ChangeSelectionType((ushort)SelectionType.STRUCTURE);

					s = CStructure.Structure_Get_ByPackedTile(CStructure.g_structureActivePosition);
					if (s != null)
					{
						if ((CStructure.Structure_GetBuildable(s) & (1 << s.objectType)) == 0) CStructure.Structure_BuildObject(s, 0xFFFE);
					}

					CStructure.g_structureActiveType = 0xFFFF;
					CStructure.g_structureActive = null;
					Gui.g_selectionState = 0; /* Invalid. */

					Gui.GUI_DisplayHint(si.o.hintStringID, si.o.spriteID);

					CHouse.House_UpdateRadarState(h);

					if (h.powerProduction < h.powerUsage)
					{
						if ((h.structuresBuilt & (1 << (byte)StructureType.STRUCTURE_OUTPOST)) != 0)
						{
							Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_NOT_ENOUGH_POWER_FOR_RADAR_BUILD_WINDTRAPS), 3);
						}
					}
					return true;
				}

				Sound.Voice_Play(47);

				if (CStructure.g_structureActiveType == (ushort)StructureType.STRUCTURE_SLAB_1x1 || CStructure.g_structureActiveType == (ushort)StructureType.STRUCTURE_SLAB_2x2)
				{
					Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_CAN_NOT_PLACE_FOUNDATION_HERE), 2);
				}
				else
				{
					Gui.GUI_DisplayHint((ushort)Text.STR_STRUCTURES_MUST_BE_PLACED_ON_CLEAR_ROCK_OR_CONCRETE_AND_ADJACENT_TO_ANOTHER_FRIENDLY_STRUCTURE, 0xFFFF);
					Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_CAN_NOT_PLACE_S_HERE), 2, CString.String_Get_ByIndex(si.o.stringID_abbrev));
				}
				return true;
			}

			if (click && w.index == 43)
			{
				ushort position;

				if (CSharpDune.g_debugScenario)
				{
					position = packed;
				}
				else
				{
					position = CUnit.Unit_FindTargetAround(packed);
				}

				if (Map.g_map[position].overlayTileID != Sprites.g_veiledTileID || CSharpDune.g_debugScenario)
				{
					if (CObject.Object_GetByPackedTile(position) != null || CSharpDune.g_debugScenario)
					{
						Map.Map_SetSelection(position);
						CUnit.Unit_DisplayStatusText(CUnit.g_unitSelected);
					}
				}

				if ((w.state.buttonState & 0x10) != 0) Map.Map_SetViewportPosition(packed);

				return true;
			}

			if ((click || drag) && w.index == 44)
			{
				Map.Map_SetViewportPosition(packed);
				return true;
			}

			if (CSharpDune.g_selectionType == (ushort)SelectionType.TARGET)
			{
				Map.Map_SetSelection(CUnit.Unit_FindTargetAround(packed));
			}
			else if (CSharpDune.g_selectionType == (ushort)SelectionType.PLACE)
			{
				Map.Map_SetSelection(packed);
			}

			return true;
		}
	}
}
