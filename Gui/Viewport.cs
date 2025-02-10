/* Viewport */

namespace SharpDune.Gui;

static class Viewport
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

        if (screenID == Screen.NO0) oldScreenID = GFX_Screen_SetActive(Screen.NO1);

        for (i = 0; i < 4096; i++) GUI_Widget_Viewport_DrawTile(i);

        Map_UpdateMinimapPosition(g_minimapPosition, true);

        if (screenID != Screen.NO0) return;

        GFX_Screen_SetActive(oldScreenID);

        GUI_Mouse_Hide_InWidget(3);
        GUI_Screen_Copy(32, 136, 32, 136, 8, 64, Screen.NO1, Screen.NO0);
        GUI_Mouse_Show_InWidget();
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

        if (Tile_IsOutOfMap(packed) || !Map_IsValidPosition(packed)) return false;

        mapScale = (ushort)(g_scenario.mapScale + 1);

        if (mapScale == 0 || BitArray_Test(g_displayedMinimap, packed)) return false;

        if ((g_map[packed].isUnveiled && g_playerHouse.flags.radarActivated) || g_debugScenario)
        {
            var type = Map_GetLandscapeType(packed);
            CUnit u;

            if (mapScale > 1)
            {
                spriteID = (ushort)(g_scenario.mapScale + g_table_landscapeInfo[type].spriteID - 1);
            }
            else
            {
                colour = g_table_landscapeInfo[type].radarColour;
            }

            if (g_table_landscapeInfo[type].radarColour == 0xFFFF)
            {
                if (mapScale > 1)
                {
                    spriteID = (ushort)(mapScale + (g_map[packed].houseID * 2) + 29);
                }
                else
                {
                    colour = g_table_houseInfo[g_map[packed].houseID].minimapColor;
                }
            }

            u = Unit_Get_ByPackedTile(packed);

            if (u != null)
            {
                if (mapScale > 1)
                {
                    spriteID = u.o.type == (byte)UnitType.UNIT_SANDWORM ? (ushort)(mapScale + 53) : (ushort)(mapScale + (Unit_GetHouseID(u) * 2) + 29);
                }
                else
                {
                    colour = u.o.type == (byte)UnitType.UNIT_SANDWORM ? (ushort)255 : g_table_houseInfo[Unit_GetHouseID(u)].minimapColor;
                }
            }
        }
        else
        {
            var s = Structure_Get_ByPackedTile(packed);

            if (s != null && s.o.houseID == (byte)g_playerHouseID)
            {
                if (mapScale > 1)
                {
                    spriteID = (ushort)(mapScale + (s.o.houseID * 2) + 29);
                }
                else
                {
                    colour = g_table_houseInfo[s.o.houseID].minimapColor;
                }
            }
            else
            {
                if (mapScale > 1)
                {
                    spriteID = (ushort)(g_scenario.mapScale + g_table_landscapeInfo[(int)LandscapeType.LST_ENTIRELY_MOUNTAIN].spriteID - 1);
                }
                else
                {
                    colour = 12;
                }
            }
        }

        x = Tile_GetPackedX(packed);
        y = Tile_GetPackedY(packed);

        x -= g_mapInfos[g_scenario.mapScale].minX;
        y -= g_mapInfos[g_scenario.mapScale].minY;

        if (spriteID != 0xFFFF)
        {
            x *= (ushort)(g_scenario.mapScale + 1);
            y *= (ushort)(g_scenario.mapScale + 1);
            GUI_DrawSprite(Screen.ACTIVE, g_sprites[spriteID], (short)x, (short)y, 3, DRAWSPRITE_FLAG_WIDGETPOS);
        }
        else
        {
            GFX_PutPixel((ushort)(x + 256), (ushort)(y + 136), (byte)(colour & 0xFF));
        }
        return true;
    }

    static readonly ushort[][] values_32A4 = [ //[8][2]	/* index, flag passed to GUI_DrawSprite() */
			[0, 0], [1, 0], [2, 0], [3, 0],
            [4, 0], [3, 1], [2, 1], [1, 1]
        ];
    static readonly ushort[][] values_32C4 = [ //[8][2]	/* index, flag */
			[0, 0], [1, 0], [1, 0], [1, 0],
            [2, 0], [1, 1], [1, 1], [1, 1]
        ];
    static readonly ushort[] values_334A = [0, 1, 0, 2];
    static readonly short[][] values_334E = [ //[8][2]
			[0, 7],  [-7,  6], [-14, 1], [-9, -6],
            [0, -9], [9, -6], [14, 1], [7,  6]
        ];
    static readonly short[][] values_336E = [ //[8][2]
			[0, -5], [0, -5], [2, -3], [2, -1],
            [-1, -3], [-2, -1], [-2, -3], [-1, -5]
        ];
    static readonly short[][] values_338E = [ //[8][2]
			[0, -4], [-1, -3], [2, -4], [0, -3],
            [-1, -3], [0, -3], [-2, -4], [1, -3]
        ];
    static readonly ushort[][] values_32E4 = [ //[8][2]
			[0, 0], [1, 0], [2, 0], [1, 2],
            [0, 2], [1, 3], [2, 1], [1, 1]
        ];
    static readonly ushort[][] values_3304 = [ //[16][2]
			[0, 0], [1, 0], [2, 0], [3, 0],
            [4, 0], [3, 2], [2, 2], [1, 2],
            [0, 2], [3, 3], [2, 3], [3, 3],
            [4, 1], [3, 1], [2, 1], [1, 1]
        ];
    static readonly ushort[] values_33AE = [2, 1, 0, 1];
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

        oldScreenID = GFX_Screen_SetActive(Screen.NO1);

        oldWidgetID = Widget_SetCurrentWidget(2);

        if (g_dirtyViewportCount != 0 || forceRedraw)
        {
            for (y = 0; y < 10; y++)
            {
                var top = (ushort)((y << 4) + 0x28); /* 40 */
                for (x = 0; x < (drawToMainScreen ? 15 : 16); x++)
                {
                    CTile t;
                    ushort left;

                    curPos = (ushort)(g_viewportPosition + Tile_PackXY(x, y));

                    if (x < 15 && !forceRedraw && BitArray_Test(g_dirtyViewport, curPos))
                    {
                        if (maxX[y] < x) maxX[y] = (short)x;
                        if (minX[y] > x) minX[y] = (short)x;
                        updateDisplay = true;
                    }

                    if (!BitArray_Test(g_dirtyMinimap, curPos) && !forceRedraw) continue;

                    BitArray_Set(g_dirtyViewport, curPos);

                    if (x < 15)
                    {
                        updateDisplay = true;
                        if (maxX[y] < x) maxX[y] = (short)x;
                        if (minX[y] > x) minX[y] = (short)x;
                    }

                    t = g_map[curPos];
                    left = (ushort)(x << 4);

                    if (!g_debugScenario && g_veiledTileID == t.overlayTileID)
                    {
                        /* draw a black rectangle */
                        GUI_DrawFilledRectangle((short)left, (short)top, (short)(left + 15), (short)(top + 15), 12);
                        continue;
                    }

                    GFX_DrawTile(t.groundTileID, left, top, t.houseID);

                    if (t.overlayTileID != 0 && !g_debugScenario)
                    {
                        GFX_DrawTile(t.overlayTileID, left, top, t.houseID);
                    }
                }
            }
            g_dirtyViewportCount = 0;
        }

        /* Draw Sandworm */
        find.type = (ushort)UnitType.UNIT_SANDWORM;
        find.index = 0xFFFF;
        find.houseID = (byte)HouseType.HOUSE_INVALID;

        while (true)
        {
            CUnit u;
            byte[] sprite;

            u = Unit_Find(find);

            if (u == null) break;

            if (!u.o.flags.isDirty && !forceRedraw) continue;
            u.o.flags.isDirty = false;

            if (!g_map[Tile_PackTile(u.o.position)].isUnveiled && !g_debugScenario) continue;

            sprite = g_sprites[g_table_unitInfo[u.o.type].groundSpriteID];
            GUI_Widget_Viewport_GetSprite_HousePalette(sprite, Unit_GetHouseID(u), paletteHouse);

            if (Map_IsPositionInViewport(u.o.position, out x, out y))
            {
                GUI_DrawSprite(Screen.ACTIVE, sprite, (short)x, (short)y, 2, DRAWSPRITE_FLAG_BLUR | DRAWSPRITE_FLAG_WIDGETPOS | DRAWSPRITE_FLAG_CENTER);
            }
            if (Map_IsPositionInViewport(u.targetLast, out x, out y))
            {
                GUI_DrawSprite(Screen.ACTIVE, sprite, (short)x, (short)y, 2, DRAWSPRITE_FLAG_BLUR | DRAWSPRITE_FLAG_WIDGETPOS | DRAWSPRITE_FLAG_CENTER);
            }
            if (Map_IsPositionInViewport(u.targetPreLast, out x, out y))
            {
                GUI_DrawSprite(Screen.ACTIVE, sprite, (short)x, (short)y, 2, DRAWSPRITE_FLAG_BLUR | DRAWSPRITE_FLAG_WIDGETPOS | DRAWSPRITE_FLAG_CENTER);
            }
            if (u == g_unitSelected && Map_IsPositionInViewport(u.o.position, out x, out y))
            {
                GUI_DrawSprite(Screen.ACTIVE, g_sprites[6], (short)x, (short)y, 2, DRAWSPRITE_FLAG_WIDGETPOS | DRAWSPRITE_FLAG_CENTER);
            }
        }

        if (g_unitSelected == null && (g_selectionRectangleNeedRepaint || hasScrolled) && (Structure_Get_ByPackedTile(g_selectionRectanglePosition) != null || g_selectionType == (ushort)SelectionType.PLACE || g_debugScenario))
        {
            var x1 = (ushort)((Tile_GetPackedX(g_selectionRectanglePosition) - Tile_GetPackedX(g_minimapPosition)) << 4);
            var y1 = (ushort)(((Tile_GetPackedY(g_selectionRectanglePosition) - Tile_GetPackedY(g_minimapPosition)) << 4) + 0x28);
            var x2 = (ushort)(x1 + (g_selectionWidth << 4) - 1);
            var y2 = (ushort)(y1 + (g_selectionHeight << 4) - 1);

            GUI_SetClippingArea(0, 40, 239, SCREEN_HEIGHT - 1);
            GUI_DrawWiredRectangle(x1, y1, x2, y2, 0xFF);

            if (g_selectionState == 0 && g_selectionType == (ushort)SelectionType.PLACE)
            {
                GUI_DrawLine((short)x1, (short)y1, (short)x2, (short)y2, 0xFF);
                GUI_DrawLine((short)x2, (short)y1, (short)x1, (short)y2, 0xFF);
            }

            GUI_SetClippingArea(0, 0, SCREEN_WIDTH - 1, SCREEN_HEIGHT - 1);

            g_selectionRectangleNeedRepaint = false;
        }

        /* Draw ground units */
        if (g_dirtyUnitCount != 0 || forceRedraw || updateDisplay)
        {
            find.type = 0xFFFF;
            find.index = 0xFFFF;
            find.houseID = (byte)HouseType.HOUSE_INVALID;

            while (true)
            {
                CUnit u;
                UnitInfo ui;
                ushort packed;
                byte orientation;
                ushort index;
                ushort spriteFlags = 0;

                u = Unit_Find(find);

                if (u == null) break;

                if (u.o.index is < 20 or > 101) continue;

                packed = Tile_PackTile(u.o.position);

                if ((!u.o.flags.isDirty || u.o.flags.isNotOnMap) && !forceRedraw && !BitArray_Test(g_dirtyViewport, packed)) continue;
                u.o.flags.isDirty = false;

                if (!g_map[packed].isUnveiled && !g_debugScenario) continue;

                ui = g_table_unitInfo[u.o.type];

                if (!Map_IsPositionInViewport(u.o.position, out x, out y)) continue;

                x += g_table_tilediff[0][u.wobbleIndex].x;
                y += g_table_tilediff[0][u.wobbleIndex].y;

                orientation = Orientation_Orientation256ToOrientation8((byte)u.orientation[0].current);

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

                if (u.o.type != (byte)UnitType.UNIT_SANDWORM && u.o.flags.isHighlighted) spriteFlags |= DRAWSPRITE_FLAG_REMAP;
                if (ui.o.flags.blurTile) spriteFlags |= DRAWSPRITE_FLAG_BLUR;

                spriteFlags |= DRAWSPRITE_FLAG_WIDGETPOS | DRAWSPRITE_FLAG_CENTER;

                if (GUI_Widget_Viewport_GetSprite_HousePalette(g_sprites[index], (u.deviated != 0) ? u.deviatedHouse : Unit_GetHouseID(u), paletteHouse))
                {
                    spriteFlags |= DRAWSPRITE_FLAG_PAL;
                    GUI_DrawSprite(Screen.ACTIVE, g_sprites[index], (short)x, (short)y, 2, spriteFlags, paletteHouse, g_paletteMapping2, (short)1);
                }
                else
                {
                    GUI_DrawSprite(Screen.ACTIVE, g_sprites[index], (short)x, (short)y, 2, spriteFlags, g_paletteMapping2, 1);
                }

                if (u.o.type == (byte)UnitType.UNIT_HARVESTER && u.actionID == (byte)ActionType.ACTION_HARVEST && u.spriteOffset >= 0 && (u.actionID == (byte)ActionType.ACTION_HARVEST || u.actionID == (byte)ActionType.ACTION_MOVE))
                {
                    var type = Map_GetLandscapeType(packed);
                    if (type is ((ushort)LandscapeType.LST_SPICE) or ((ushort)LandscapeType.LST_THICK_SPICE))
                    {
                        /*GUI_Widget_Viewport_GetSprite_HousePalette(. . ., Unit_GetHouseID(u), paletteHouse),*/
                        GUI_DrawSprite(Screen.ACTIVE,
                                       g_sprites[(u.spriteOffset % 3) + 0xDF + (values_32A4[orientation][0] * 3)],
                                       (short)(x + values_334E[orientation][0]), (short)(y + values_334E[orientation][1]),
                                       2, values_32A4[orientation][1] | DRAWSPRITE_FLAG_WIDGETPOS | DRAWSPRITE_FLAG_CENTER);
                    }
                }

                if (u.spriteOffset >= 0 && ui.turretSpriteID != 0xFFFF)
                {
                    short offsetX = 0;
                    short offsetY = 0;
                    var spriteID = ui.turretSpriteID;

                    orientation = Orientation_Orientation256ToOrientation8((byte)u.orientation[ui.o.flags.hasTurret ? 1 : 0].current);

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

                    if (GUI_Widget_Viewport_GetSprite_HousePalette(g_sprites[spriteID], Unit_GetHouseID(u), paletteHouse))
                    {
                        GUI_DrawSprite(Screen.ACTIVE, g_sprites[spriteID],
                                       (short)(x + offsetX), (short)(y + offsetY),
                                       2, values_32A4[orientation][1] | DRAWSPRITE_FLAG_WIDGETPOS | DRAWSPRITE_FLAG_CENTER | DRAWSPRITE_FLAG_PAL, paletteHouse);
                    }
                    else
                    {
                        GUI_DrawSprite(Screen.ACTIVE, g_sprites[spriteID],
                                       (short)(x + offsetX), (short)(y + offsetY),
                                       2, values_32A4[orientation][1] | DRAWSPRITE_FLAG_WIDGETPOS | DRAWSPRITE_FLAG_CENTER);
                    }
                }

                if (u.o.flags.isSmoking)
                {
                    var spriteID = (ushort)(180 + (u.spriteOffset & 3));
                    if (spriteID == 183) spriteID = 181;

                    GUI_DrawSprite(Screen.ACTIVE, g_sprites[spriteID], (short)x, (short)(y - 14), 2, DRAWSPRITE_FLAG_WIDGETPOS | DRAWSPRITE_FLAG_CENTER);
                }

                if (u != g_unitSelected) continue;

                GUI_DrawSprite(Screen.ACTIVE, g_sprites[6], (short)x, (short)y, 2, DRAWSPRITE_FLAG_WIDGETPOS | DRAWSPRITE_FLAG_CENTER);
            }

            g_dirtyUnitCount = 0;
        }

        /* draw explosions */
        for (i = 0; i < EXPLOSION_MAX; i++)
        {
            var e = Explosion_Get_ByIndex(i);

            curPos = Tile_PackTile(e.position);

            if (BitArray_Test(g_dirtyViewport, curPos)) e.isDirty = true;

            if (e.commands == null) continue;
            if (!e.isDirty && !forceRedraw) continue;
            if (e.spriteID == 0) continue;

            e.isDirty = false;

            if (!g_map[curPos].isUnveiled && !g_debugScenario) continue;
            if (!Map_IsPositionInViewport(e.position, out x, out y)) continue;

            /*GUI_Widget_Viewport_GetSprite_HousePalette(g_sprites[e->spriteID], e->houseID, paletteHouse);*/
            GUI_DrawSprite(Screen.ACTIVE, g_sprites[e.spriteID], (short)x, (short)y, 2, DRAWSPRITE_FLAG_WIDGETPOS | DRAWSPRITE_FLAG_CENTER/*, paletteHouse*/);
        }

        /* draw air units */
        if (g_dirtyAirUnitCount != 0 || forceRedraw || updateDisplay)
        {
            find.type = 0xFFFF;
            find.index = 0xFFFF;
            find.houseID = (byte)HouseType.HOUSE_INVALID;

            while (true)
            {
                CUnit u;
                UnitInfo ui;
                byte orientation;
                byte[] sprite;
                ushort index;
                ushort spriteFlags;

                u = Unit_Find(find);

                if (u == null) break;

                if (u.o.index > 15) continue;

                curPos = Tile_PackTile(u.o.position);

                if ((!u.o.flags.isDirty || u.o.flags.isNotOnMap) && !forceRedraw && !BitArray_Test(g_dirtyViewport, curPos)) continue;
                u.o.flags.isDirty = false;

                if (!g_map[curPos].isUnveiled && !g_debugScenario) continue;

                ui = g_table_unitInfo[u.o.type];

                if (!Map_IsPositionInViewport(u.o.position, out x, out y)) continue;

                index = ui.groundSpriteID;
                orientation = (byte)u.orientation[0].current;
                spriteFlags = DRAWSPRITE_FLAG_WIDGETPOS | DRAWSPRITE_FLAG_CENTER;

                switch ((DisplayMode)ui.displayMode)
                {
                    case DisplayMode.SINGLE_FRAME:
                        if (u.o.flags.bulletIsBig) index++;
                        break;

                    case DisplayMode.UNIT:
                        orientation = Orientation_Orientation256ToOrientation8(orientation);

                        index += values_32E4[orientation][0];
                        spriteFlags |= values_32E4[orientation][1];
                        break;

                    case DisplayMode.ROCKET:
                        {
                            orientation = Orientation_Orientation256ToOrientation16(orientation);

                            index += values_3304[orientation][0];
                            spriteFlags |= values_3304[orientation][1];
                        }
                        break;

                    case DisplayMode.ORNITHOPTER:
                        {
                            orientation = Orientation_Orientation256ToOrientation8(orientation);

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

                sprite = g_sprites[index];

                if (ui.o.flags.hasShadow)
                {
                    GUI_DrawSprite(Screen.ACTIVE, sprite, (short)(x + 1), (short)(y + 3), 2, (spriteFlags & ~DRAWSPRITE_FLAG_PAL) | DRAWSPRITE_FLAG_REMAP | DRAWSPRITE_FLAG_BLUR, g_paletteMapping1, (short)1);
                }
                if (ui.o.flags.blurTile) spriteFlags |= DRAWSPRITE_FLAG_BLUR;

                if (GUI_Widget_Viewport_GetSprite_HousePalette(sprite, Unit_GetHouseID(u), paletteHouse))
                {
                    GUI_DrawSprite(Screen.ACTIVE, sprite, (short)x, (short)y, 2, spriteFlags | DRAWSPRITE_FLAG_PAL, paletteHouse);
                }
                else
                {
                    GUI_DrawSprite(Screen.ACTIVE, sprite, (short)x, (short)y, 2, spriteFlags);
                }
            }

            g_dirtyAirUnitCount = 0;
        }

        if (updateDisplay)
        {
            Array.Fill<byte>(g_dirtyMinimap, 0, 0, g_dirtyMinimap.Length); //memset(g_dirtyMinimap, 0, sizeof(g_dirtyMinimap));
            Array.Fill<byte>(g_dirtyViewport, 0, 0, g_dirtyViewport.Length); //memset(g_dirtyViewport, 0, sizeof(g_dirtyViewport));
        }

        if (g_changedTilesCount != 0)
        {
            var init = false;
            var update = false;
            ushort minY = 0xffff;
            ushort maxY = 0;
            var oldScreenID2 = Screen.NO1;

            for (i = 0; i < g_changedTilesCount; i++)
            {
                curPos = g_changedTiles[i];
                BitArray_Clear(g_changedTilesMap, curPos);

                if (!init)
                {
                    init = true;

                    oldScreenID2 = GFX_Screen_SetActive(Screen.NO1);

                    GUI_Mouse_Hide_InWidget(3);
                }

                if (GUI_Widget_Viewport_DrawTile(curPos))
                {
                    y = (ushort)(Tile_GetPackedY(curPos) - g_mapInfos[g_scenario.mapScale].minY); /* +136 */
                    y *= (ushort)(g_scenario.mapScale + 1);
                    if (y > maxY) maxY = y;
                    if (y < minY) minY = y;
                }

                if (!update && BitArray_Test(g_displayedMinimap, curPos)) update = true;
            }

            if (update) Map_UpdateMinimapPosition(g_minimapPosition, true);

            if (init)
            {
                if (hasScrolled)
                {   /* force copy of the whole map (could be of the white rectangle) */
                    minY = 0;
                    maxY = (ushort)(63 - g_scenario.mapScale);
                }
                /* MiniMap : redraw only line that changed */
                if (minY < maxY) GUI_Screen_Copy(32, (short)(136 + minY), 32, (short)(136 + minY), 8, (short)(maxY + 1 + g_scenario.mapScale - minY), Screen.ACTIVE, Screen.NO0);

                GFX_Screen_SetActive(oldScreenID2);

                GUI_Mouse_Show_InWidget();
            }

            if (g_changedTilesCount == g_changedTiles.Length)
            {
                g_changedTilesCount = 0;

                for (i = 0; i < 4096; i++)
                {
                    if (!BitArray_Test(g_changedTilesMap, i)) continue;
                    g_changedTiles[g_changedTilesCount++] = i;
                    if (g_changedTilesCount == g_changedTiles.Length) break;
                }
            }
            else
            {
                g_changedTilesCount = 0;
            }
        }

        if ((g_viewportMessageCounter & 1) != 0 && g_viewportMessageText != null && (minX[6] <= 14 || maxX[6] >= 0 || hasScrolled || forceRedraw))
        {
            GUI_DrawText_Wrapper(g_viewportMessageText, 112, 139, 15, 0, 0x132);
            minX[6] = -1;
            maxX[6] = 14;
        }

        if (updateDisplay && !drawToMainScreen)
        {
            if (g_viewport_fadein)
            {
                GUI_Mouse_Hide_InWidget(g_curWidgetIndex);

                /* ENHANCEMENT -- When fading in the game on start, you don't see the fade as it is against the already drawn screen. */
                if (g_dune2_enhanced)
                {
                    var oldScreenID2 = GFX_Screen_SetActive(Screen.NO0);
                    GUI_DrawFilledRectangle((short)(g_curWidgetXBase << 3), (short)g_curWidgetYBase, (short)((g_curWidgetXBase + g_curWidgetWidth) << 3), (short)(g_curWidgetYBase + g_curWidgetHeight), 0);
                    GFX_Screen_SetActive(oldScreenID2);
                }

                GUI_Screen_FadeIn(g_curWidgetXBase, g_curWidgetYBase, g_curWidgetXBase, g_curWidgetYBase, g_curWidgetWidth, g_curWidgetHeight, Screen.ACTIVE, Screen.NO0);
                GUI_Mouse_Show_InWidget();

                g_viewport_fadein = false;
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
                        GUI_Mouse_Hide_InWidget(g_curWidgetIndex);

                        init = true;
                    }

                    GUI_Screen_Copy((short)x, (short)y, (short)x, (short)y, (short)width, (short)height, Screen.ACTIVE, Screen.NO0);
                }

                if (init) GUI_Mouse_Show_InWidget();
            }
        }

        GFX_Screen_SetActive(oldScreenID);

        Widget_SetCurrentWidget(oldWidgetID);
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

                if (v is >= 0x90 and <= 0x98)
                {
                    v += (byte)(houseID << 4);
                }

                paletteHouse[i] = v;
            }
        }
        return true;
    }

    /* HotSpots for different cursor types. */
    static readonly XYPosition[] cursorHotSpots = [
            new() { x = 0, y = 0 }, new() { x = 5, y = 0 }, new() { x = 8, y = 5 },
            new() { x = 5, y = 8 }, new() { x = 0, y = 5 }, new() { x = 8, y = 8 }
        ];
    /*
     * Handles the Click events for the Viewport widget.
     *
     * @param w The widget.
     */
    internal static bool GUI_Widget_Viewport_Click(CWidget w)
    {
        ushort direction;
        ushort x, y;
        ushort spriteID;
        ushort packed;
        bool click, drag;

        spriteID = g_cursorSpriteID;
        switch (w.index)
        {
            default: break;
            case 39: spriteID = 1; break;
            case 40: spriteID = 2; break;
            case 41: spriteID = 4; break;
            case 42: spriteID = 3; break;
            case 43: spriteID = g_cursorDefaultSpriteID; break;
            case 44: spriteID = g_cursorDefaultSpriteID; break;
            case 45: spriteID = 0; break;
        }

        if (spriteID != g_cursorSpriteID)
        {
            s_tickCursor = g_timerGame;

            Sprites_SetMouseSprite(cursorHotSpots[spriteID].x, cursorHotSpots[spriteID].y, g_sprites[spriteID]);

            g_cursorSpriteID = spriteID;
        }

        if (w.index == 45) return true;

        click = false;
        drag = false;

        if ((w.state.buttonState & 0x11) != 0)
        {
            click = true;
            g_var_37B8 = false;
        }
        else if ((w.state.buttonState & 0x22) != 0 && !g_var_37B8)
        {
            drag = true;
        }

        /* ENHANCEMENT -- Dune2 depends on slow CPUs to limit the rate mouse clicks are handled. */
        if (g_dune2_enhanced && (click || drag))
        {
            if (s_tickClick + 2 >= g_timerGame) return true;
            s_tickClick = g_timerGame;
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
                if (s_tickMapScroll + 10 >= g_timerGame || s_tickCursor + 20 >= g_timerGame) return true;
                /* Don't scroll if we have a structure/unit selected and don't want to autoscroll */
                if (g_gameConfig.autoScroll == 0 && (g_selectionType == (ushort)SelectionType.STRUCTURE || g_selectionType == (ushort)SelectionType.UNIT)) return true;
            }

            s_tickMapScroll = g_timerGame;

            Map_MoveDirection(direction);
            return true;
        }

        if (click)
        {
            x = g_mouseClickX;
            y = g_mouseClickY;
        }
        else
        {
            x = g_mouseX;
            y = g_mouseY;
        }

        if (w.index == 43)
        {
            x = (ushort)((x / 16) + Tile_GetPackedX(g_minimapPosition));
            y = (ushort)(((y - 40) / 16) + Tile_GetPackedY(g_minimapPosition));
        }
        else if (w.index == 44)
        {
            ushort mapScale;
            MapInfo mapInfo;

            mapScale = g_scenario.mapScale;
            mapInfo = g_mapInfos[mapScale];

            x = (ushort)(Math.Min((Math.Max(x, (ushort)256) - 256) / (mapScale + 1), mapInfo.sizeX - 1) + mapInfo.minX);
            y = (ushort)(Math.Min((Math.Max(y, (ushort)136) - 136) / (mapScale + 1), mapInfo.sizeY - 1) + mapInfo.minY);
        }

        packed = Tile_PackXY(x, y);

        if (click && g_selectionType == (ushort)SelectionType.TARGET)
        {
            CUnit u;
            ActionType action;
            ushort encoded;

            GUI_DisplayText(null, -1);

            if (g_unitHouseMissile != null)
            {
                Unit_LaunchHouseMissile(packed);
                return true;
            }

            u = g_unitActive;

            action = (ActionType)g_activeAction;

            Object_Script_Variable4_Clear(u.o);
            u.targetAttack = 0;
            u.targetMove = 0;
            u.route[0] = 0xFF;

            encoded = action is not ActionType.ACTION_MOVE and not ActionType.ACTION_HARVEST
                ? Tools_Index_Encode(Unit_FindTargetAround(packed), IndexType.IT_TILE)
                : Tools_Index_Encode(packed, IndexType.IT_TILE);

            Unit_SetAction(u, action);

            if (action == ActionType.ACTION_MOVE)
            {
                Unit_SetDestination(u, encoded);
            }
            else if (action == ActionType.ACTION_HARVEST)
            {
                u.targetMove = encoded;
            }
            else
            {
                CUnit target;

                Unit_SetTarget(u, encoded);
                target = Tools_Index_GetUnit(u.targetAttack);
                if (target != null) target.blinkCounter = 8;
            }

            if (!g_enableVoices)
            {
                Driver_Sound_Play(36, 0xFF);
            }
            else if (g_table_unitInfo[u.o.type].movementType == (ushort)MovementType.MOVEMENT_FOOT)
            {
                Sound_StartSound(g_table_actionInfo[(int)action].soundID);
            }
            else
            {
                Sound_StartSound((ushort)(((Tools_Random_256() & 0x1) == 0) ? 20 : 17));
            }

            g_unitActive = null;
            g_activeAction = 0xFFFF;

            GUI_ChangeSelectionType((ushort)SelectionType.UNIT);
            return true;
        }

        if (click && g_selectionType == (ushort)SelectionType.PLACE)
        {
            StructureInfo si;
            CStructure s;
            CHouse h;

            s = g_structureActive;
            si = g_table_structureInfo[g_structureActiveType];
            h = g_playerHouse;

            if (Structure_Place(s, g_selectionPosition))
            {
                Voice_Play(20);

                if (s.o.type == (byte)StructureType.STRUCTURE_PALACE) House_Get_ByIndex(s.o.houseID).palacePosition = s.o.position;

                if (g_structureActiveType == (ushort)StructureType.STRUCTURE_REFINERY && g_validateStrictIfZero == 0)
                {
                    CUnit u;

                    g_validateStrictIfZero++;
                    u = Unit_CreateWrapper((byte)g_playerHouseID, UnitType.UNIT_HARVESTER, Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE));
                    g_validateStrictIfZero--;

                    if (u == null)
                    {
                        h.harvestersIncoming++;
                    }
                    else
                    {
                        u.originEncoded = Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);
                    }
                }

                GUI_ChangeSelectionType((ushort)SelectionType.STRUCTURE);

                s = Structure_Get_ByPackedTile(g_structureActivePosition);
                if (s != null)
                {
                    if ((Structure_GetBuildable(s) & (1 << s.objectType)) == 0) Structure_BuildObject(s, 0xFFFE);
                }

                g_structureActiveType = 0xFFFF;
                g_structureActive = null;
                g_selectionState = 0; /* Invalid. */

                GUI_DisplayHint(si.o.hintStringID, si.o.spriteID);

                House_UpdateRadarState(h);

                if (h.powerProduction < h.powerUsage)
                {
                    if ((h.structuresBuilt & (1 << (byte)StructureType.STRUCTURE_OUTPOST)) != 0)
                    {
                        GUI_DisplayText(String_Get_ByIndex(Text.STR_NOT_ENOUGH_POWER_FOR_RADAR_BUILD_WINDTRAPS), 3);
                    }
                }
                return true;
            }

            Voice_Play(47);

            if (g_structureActiveType is ((ushort)StructureType.STRUCTURE_SLAB_1x1) or ((ushort)StructureType.STRUCTURE_SLAB_2x2))
            {
                GUI_DisplayText(String_Get_ByIndex(Text.STR_CAN_NOT_PLACE_FOUNDATION_HERE), 2);
            }
            else
            {
                GUI_DisplayHint((ushort)Text.STR_STRUCTURES_MUST_BE_PLACED_ON_CLEAR_ROCK_OR_CONCRETE_AND_ADJACENT_TO_ANOTHER_FRIENDLY_STRUCTURE, 0xFFFF);
                GUI_DisplayText(String_Get_ByIndex(Text.STR_CAN_NOT_PLACE_S_HERE), 2, String_Get_ByIndex(si.o.stringID_abbrev));
            }
            return true;
        }

        if (click && w.index == 43)
        {
            var position = g_debugScenario ? packed : Unit_FindTargetAround(packed);

            if (g_map[position].overlayTileID != g_veiledTileID || g_debugScenario)
            {
                if (Object_GetByPackedTile(position) != null || g_debugScenario)
                {
                    Map_SetSelection(position);
                    Unit_DisplayStatusText(g_unitSelected);
                }
            }

            if ((w.state.buttonState & 0x10) != 0) Map_SetViewportPosition(packed);

            return true;
        }

        if ((click || drag) && w.index == 44)
        {
            Map_SetViewportPosition(packed);
            return true;
        }

        if (g_selectionType == (ushort)SelectionType.TARGET)
        {
            Map_SetSelection(Unit_FindTargetAround(packed));
        }
        else if (g_selectionType == (ushort)SelectionType.PLACE)
        {
            Map_SetSelection(packed);
        }

        return true;
    }
}
