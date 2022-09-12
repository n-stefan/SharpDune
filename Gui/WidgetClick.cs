/* Widget clicking handling */

namespace SharpDune.Gui;

class WidgetClick
{
    static ushort s_savegameIndexBase;
    static ushort s_savegameCountOnDisk;                    /*!< Amount of savegames on disk. */

    internal static string[] g_savegameDesc = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty }; //[5][51]			/*!< Array of savegame descriptions for the SaveLoad window. */

    /*
     * Handles Click event for "Save Game" or "Load Game" button.
     *
     * @param save Wether to save or load.
     * @return True if a game has been saved or loaded, False otherwise.
     */
    internal static bool GUI_Widget_SaveLoad_Click(bool save)
    {
        var desc = g_saveLoadWindowDesc;
        bool loop;

        s_savegameCountOnDisk = GetSavegameCount();

        s_savegameIndexBase = (ushort)Math.Max(0, s_savegameCountOnDisk - (save ? 0 : 1));

        FillSavegameDesc(save);

        desc.stringID = (short)(save ? Text.STR_SELECT_A_POSITION_TO_SAVE_TO : Text.STR_SELECT_A_SAVED_GAME_TO_LOAD);

        GUI_Window_BackupScreen(desc);

        GUI_Window_Create(desc);

        UpdateArrows(save, true);

        for (loop = true; loop; SleepIdle())
        {
            var w = g_widgetLinkedListTail;
            var key = GUI_Widget_HandleEvents(w);

            UpdateArrows(save, false);

            if ((key & 0x8000) != 0)
            {
                CWidget w2;

                key &= 0x7FFF;
                w2 = GUI_Widget_Get_ByIndex(w, key);

                switch (key)
                {
                    case 0x25:
                        s_savegameIndexBase = (ushort)Math.Min(s_savegameCountOnDisk - (save ? 0 : 1), s_savegameIndexBase + 1);

                        FillSavegameDesc(save);

                        GUI_Widget_DrawAll(w);
                        break;

                    case 0x26:
                        s_savegameIndexBase = (ushort)Math.Max(0, s_savegameIndexBase - 1);

                        FillSavegameDesc(save);

                        GUI_Widget_DrawAll(w);
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
                                return SaveGame_LoadFile(GenerateSavegameFilename((ushort)(s_savegameIndexBase - key)));
                            }

                            if (GUI_Widget_Savegame_Click(key)) return true;

                            GUI_Window_BackupScreen(desc);

                            UpdateArrows(save, true);

                            GUI_Window_Create(desc);

                            UpdateArrows(save, true);
                        }
                        break;
                }

                GUI_Widget_MakeNormal(w2, false);
            }

            GUI_PaletteAnimate();
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
    internal static bool GUI_Widget_TextButton_Click(CWidget w)
    {
        UnitInfo ui;
        ActionInfo ai;
        ushort[] actions;
        ActionType action;
        CUnit u;
        //ushort found;
        ActionType unitAction;

        u = g_unitSelected;
        ui = g_table_unitInfo[u.o.type];

        actions = ui.o.actionsPlayer;
        if (Unit_GetHouseID(u) != (byte)g_playerHouseID && u.o.type != (byte)UnitType.UNIT_HARVESTER)
        {
            actions = g_table_actionsAI;
        }

        action = (ActionType)actions[w.index - 8];
        if (g_dune2_enhanced)
        {
            if (Input_Test(0x2c) != 0 || Input_Test(0x39) != 0)
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
            Unit_Deviation_Decrease(u, 5);
            if (u.deviated == 0)
            {
                GUI_Widget_MakeNormal(w, false);
                return true;
            }
        }

        GUI_Widget_MakeSelected(w, false);

        ai = g_table_actionInfo[(int)action];

        if (ai.selectionType != g_selectionType)
        {
            g_unitActive = g_unitSelected;
            g_activeAction = (ushort)action;
            GUI_ChangeSelectionType(ai.selectionType);

            return true;
        }

        Object_Script_Variable4_Clear(u.o);
        u.targetAttack = 0;
        u.targetMove = 0;
        u.route[0] = 0xFF;

        Unit_SetAction(u, action);

        if (ui.movementType == (ushort)MovementType.MOVEMENT_FOOT) Sound_StartSound(ai.soundID);

        if (unitAction == action) return true;

        var index = Array.FindIndex(actions, a => a == (ushort)unitAction);
        if (index == -1) return true;
        //found = memchr(actions, unitAction, 4);
        //if (found == NULL) return true;

        GUI_Widget_MakeNormal(GUI_Widget_Get_ByIndex(g_widgetLinkedListHead, (ushort)(/*found - actions + 8*/index + 8)), false);

        return true;
    }

    static ushort previousIndex;
    static void UpdateArrows(bool save, bool force)
    {
        CWidget w;

        if (!force && s_savegameIndexBase == previousIndex) return;

        previousIndex = s_savegameIndexBase;

        w = g_table_windowWidgets[8];
        if (s_savegameIndexBase >= 5)
        {
            GUI_Widget_MakeVisible(w);
        }
        else
        {
            GUI_Widget_MakeInvisible(w);
            GUI_Widget_Undraw(w, 233);
        }

        w = g_table_windowWidgets[7];
        if (s_savegameCountOnDisk - (save ? 0 : 1) > s_savegameIndexBase)
        {
            GUI_Widget_MakeVisible(w);
        }
        else
        {
            GUI_Widget_MakeInvisible(w);
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

            //g_savegameDesc[i] = string.Empty; //*desc = '\0';

            if (s_savegameIndexBase - i < 0) continue;

            if (s_savegameIndexBase - i == s_savegameCountOnDisk)
            {
                if (!save) continue;

                /*desc*/ g_savegameDesc[i] = String_Get_ByIndex(Text.STR_EMPTY_SLOT_); //strncpy(desc, String_Get_ByIndex(STR_EMPTY_SLOT_), 50);
                continue;
            }

            filename = GenerateSavegameFilename((ushort)(s_savegameIndexBase - i));

            fileId = ChunkFile_Open_Personal(filename);
            if (fileId == (byte)FileMode.FILE_INVALID) continue;
            ChunkFile_Read(fileId, HToBE32((uint)SharpDune.MultiChar[FourCC.NAME]), ref /*desc*/g_savegameDesc[i], 50);
            ChunkFile_Close(fileId);
            g_savegameDesc[i] = g_savegameDesc[i][..^1];
            continue;
        }
    }

    static void GUI_Widget_Undraw(CWidget w, byte colour)
    {
        ushort offsetX;
        ushort offsetY;
        ushort width;
        ushort height;

        if (w == null) return;

        offsetX = (ushort)(w.offsetX + (g_widgetProperties[w.parentID].xBase << 3));
        offsetY = (ushort)(w.offsetY + g_widgetProperties[w.parentID].yBase);
        width = w.width;
        height = w.height;

        if (GFX_Screen_IsActive(Screen.NO0))
        {
            GUI_Mouse_Hide_InRegion(offsetX, offsetY, (ushort)(offsetX + width), (ushort)(offsetY + height));
        }

        GUI_DrawFilledRectangle((short)offsetX, (short)offsetY, (short)(offsetX + width), (short)(offsetY + height), colour);

        if (GFX_Screen_IsActive(Screen.NO0))
        {
            GUI_Mouse_Show_InRegion();
        }
    }

    static void GUI_Window_BackupScreen(WindowDesc desc)
    {
        Widget_SetCurrentWidget(desc.index);

        GUI_Mouse_Hide_Safe();
        GFX_CopyToBuffer((short)(g_curWidgetXBase * 8), (short)g_curWidgetYBase, (ushort)(g_curWidgetWidth * 8), g_curWidgetHeight, GFX_Screen_Get_ByIndex(Screen.NO2));
        GUI_Mouse_Show_Safe();
    }

    static void GUI_Window_RestoreScreen(WindowDesc desc)
    {
        Widget_SetCurrentWidget(desc.index);

        GUI_Mouse_Hide_Safe();
        GFX_CopyFromBuffer((short)(g_curWidgetXBase * 8), (short)g_curWidgetYBase, (ushort)(g_curWidgetWidth * 8), g_curWidgetHeight, GFX_Screen_Get_ByIndex(Screen.NO2));
        GUI_Mouse_Show_Safe();
    }

    /*
     * Handles Click event for "Clear List" button.
     *
     * @param w The widget.
     * @return True, always.
     */
    internal static bool GUI_Widget_HOF_ClearList_Click(CWidget w)
    {
        /* "Are you sure you want to clear the high scores?" */
        if (GUI_YesNo((ushort)Text.STR_ARE_YOU_SURE_YOU_WANT_TO_CLEAR_THE_HIGH_SCORES))
        {
            var data = (HallOfFameStruct[])w.data;

            for (var i = 0; i < data.Length; i++) data[i] = new HallOfFameStruct(); //memset(data, 0, 128);

            if (File_Exists_Personal("SAVEFAME.DAT")) File_Delete_Personal("SAVEFAME.DAT");

            GUI_HallOfFame_DrawData(data, true);

            g_doQuitHOF = true;
        }

        GUI_Widget_MakeNormal(w, false);

        return true;
    }

    /*
     * Handles Click event for "Resume Game" button.
     *
     * @return True, always.
     */
    internal static bool GUI_Widget_HOF_Resume_Click(CWidget _)
    {
        g_doQuitHOF = true;

        return true;
    }

    /*
     * Handles Click event for "Cancel" button.
     *
     * @return True, always.
     */
    internal static bool GUI_Widget_Cancel_Click(CWidget _)
    {
        if (g_structureActiveType != 0xFFFF)
        {
            var s = Structure_Get_ByPackedTile(g_structureActivePosition);
            var s2 = g_structureActive;

            Debug.Assert(s2 != null);

            if (s != null)
            {
                s.o.linkedID = (byte)(s2.o.index & 0xFF);
            }
            else
            {
                Structure_Free(s2);
            }

            g_structureActive = null;
            g_structureActiveType = 0xFFFF;

            GUI_ChangeSelectionType((ushort)SelectionType.STRUCTURE);

            g_selectionState = 0; /* Invalid. */
        }

        if (g_unitActive == null) return true;

        g_unitActive = null;
        g_activeAction = 0xFFFF;
        g_cursorSpriteID = 0;

        Sprites_SetMouseSprite(0, 0, g_sprites[0]);

        GUI_ChangeSelectionType((ushort)SelectionType.UNIT);

        return true;
    }

    /*
     * Handles Click event for a sprite/text button.
     *
     * @param w The widget.
     * @return False, always.
     */
    internal static bool GUI_Widget_SpriteTextButton_Click(CWidget _)
    {
        CStructure s;

        s = Structure_Get_ByPackedTile(g_selectionPosition);

        switch ((Text)g_productionStringID)
        {
            default: break;

            case Text.STR_PLACE_IT:
                if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
                {
                    CStructure ns;

                    ns = Structure_Get_ByIndex(s.o.linkedID);
                    g_structureActive = ns;
                    g_structureActiveType = s.objectType;
                    g_selectionState = Structure_IsValidBuildLocation(g_selectionRectanglePosition, (StructureType)g_structureActiveType);
                    g_structureActivePosition = g_selectionPosition;
                    s.o.linkedID = (byte)StructureType.STRUCTURE_INVALID;

                    GUI_ChangeSelectionType((ushort)SelectionType.PLACE);
                }
                break;

            case Text.STR_ON_HOLD:
                s.o.flags.repairing = false;
                s.o.flags.onHold = false;
                s.o.flags.upgrading = false;
                break;

            case Text.STR_BUILD_IT:
                Structure_BuildObject(s, s.objectType);
                break;

            case Text.STR_LAUNCH:
            case Text.STR_FREMEN:
            case Text.STR_SABOTEUR:
                Structure_ActivateSpecial(s);
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
    internal static bool GUI_Widget_Name_Click(CWidget _)
    {
        CObject o;
        ushort packed;

        o = Object_GetByPackedTile(g_selectionPosition);

        if (o == null) return false;

        packed = Tile_PackTile(o.position);

        Map_SetViewportPosition(packed);
        Map_SetSelection(packed);

        return false;
    }

    /*
     * Handles Click event for current selection picture.
     *
     * @return False, always.
     */
    internal static bool GUI_Widget_Picture_Click(CWidget _)
    {
        CStructure s;

        if (g_unitSelected != null)
        {
            Unit_DisplayStatusText(g_unitSelected);

            return false;
        }

        s = Structure_Get_ByPackedTile(g_selectionPosition);

        if (s == null || !g_table_structureInfo[s.o.type].o.flags.factory) return false;

        Structure_BuildObject(s, 0xFFFF);

        return false;
    }

    /*
     * Handles Click event for "Repair/Upgrade" button.
     *
     * @param w The widget.
     * @return False, always.
     */
    internal static bool GUI_Widget_RepairUpgrade_Click(CWidget w)
    {
        CStructure s;

        s = Structure_Get_ByPackedTile(g_selectionPosition);

        if (Structure_SetRepairingState(s, -1, w)) return false;
        Structure_SetUpgradingState(s, -1, w);

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
        var desc = g_savegameNameWindowDesc;
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

        GFX_Screen_SetActive(Screen.NO0);

        Widget_SetCurrentWidget(15);

        GUI_Mouse_Hide_Safe();
        GUI_DrawBorder((ushort)((g_curWidgetXBase << 3) - 1), (ushort)(g_curWidgetYBase - 1), (ushort)((g_curWidgetWidth << 3) + 2), (ushort)(g_curWidgetHeight + 2), 4, false);
        GUI_Mouse_Show_Safe();

        for (loop = true; loop; SleepIdle())
        {
            ushort eventKey;
            var w = g_widgetLinkedListTail;

            GUI_DrawText_Wrapper(null, 0, 0, 232, 235, 0x22);

            eventKey = GUI_EditBox(ref /*saveDesc*/g_savegameDesc[index], 50, 15, g_widgetLinkedListTail, null, widgetPaint);
            widgetPaint = false;

            if ((eventKey & 0x8000) == 0) continue;

            GUI_Widget_MakeNormal(GUI_Widget_Get_ByIndex(w, (ushort)(eventKey & 0x7FFF)), false);

            switch (eventKey & 0x7FFF)
            {
                case 0x1E:  /* RETURN / Save Button */
                    if (/*saveDesc == 0*/g_savegameDesc[index].Length == 0) break;

                    SaveGame_SaveFile(GenerateSavegameFilename((ushort)(s_savegameIndexBase - index)), /*saveDesc*/g_savegameDesc[index]);
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
        var desc = g_yesNoWindowDesc;
        bool loop;
        var ret = false;

        desc.stringID = (short)stringID;

        GUI_Window_BackupScreen(desc);

        GUI_Window_Create(desc);

        for (loop = true; loop; SleepIdle())
        {
            var key = GUI_Widget_HandleEvents(g_widgetLinkedListTail);

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

            GUI_PaletteAnimate();
        }

        GUI_Window_RestoreScreen(desc);

        return ret;
    }

    static ushort GetSavegameCount()
    {
        ushort i;

        for (i = 0; ; i++)
        {
            if (!File_Exists_Personal(GenerateSavegameFilename(i))) return i;
        }
    }

    static void GUI_Window_Create(WindowDesc desc)
    {
        byte i;

        if (desc == null) return;

        g_widgetLinkedListTail = null;

        GFX_Screen_SetActive(Screen.NO1);

        Widget_SetCurrentWidget(desc.index);

        GUI_Widget_DrawBorder(g_curWidgetIndex, 2, true);

        if (GUI_String_Get_ByIndex(desc.stringID) != null)
        {
            GUI_DrawText_Wrapper(GUI_String_Get_ByIndex(desc.stringID), (short)((g_curWidgetXBase << 3) + (g_curWidgetWidth << 2)), (short)(g_curWidgetYBase + 6 + ((desc == g_yesNoWindowDesc) ? 2 : 0)), 238, 0, 0x122);
        }

        if (GUI_String_Get_ByIndex((short)desc.widgets[0].stringID) == null)
        {
            GUI_DrawText_Wrapper(String_Get_ByIndex(Text.STR_THERE_ARE_NO_SAVED_GAMES_TO_LOAD), (short)((g_curWidgetXBase + 2) << 3), (short)(g_curWidgetYBase + 42), 232, 0, 0x22);
        }

        for (i = 0; i < desc.widgetCount; i++)
        {
            var w = g_table_windowWidgets[i];

            if (GUI_String_Get_ByIndex((short)desc.widgets[i].stringID) == null) continue;

            w.next = null;
            w.offsetX = (short)desc.widgets[i].offsetX;
            w.offsetY = (short)desc.widgets[i].offsetY;
            w.width = desc.widgets[i].width;
            w.height = desc.widgets[i].height;
            w.shortcut = 0;
            w.shortcut2 = 0;

            if (desc != g_savegameNameWindowDesc)
            {
                w.shortcut = desc.widgets[i].labelStringId != (ushort)Text.STR_NULL
                    ? GUI_Widget_GetShortcut((byte)GUI_String_Get_ByIndex((short)desc.widgets[i].labelStringId)[0])
                    : GUI_Widget_GetShortcut((byte)GUI_String_Get_ByIndex((short)desc.widgets[i].stringID)[0]);
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
            w.drawParameterNormal.proc = GUI_Widget_TextButton_Draw;
            w.drawParameterSelected.proc = GUI_Widget_TextButton_Draw;
            w.drawParameterDown.proc = GUI_Widget_TextButton_Draw;
            w.parentID = desc.index;
            //memset(&w.state, 0, sizeof(w.state));

            g_widgetLinkedListTail = GUI_Widget_Link(g_widgetLinkedListTail, w);

            GUI_Widget_MakeVisible(w);
            GUI_Widget_MakeNormal(w, false);
            GUI_Widget_Draw(w);

            if (desc.widgets[i].labelStringId == (ushort)Text.STR_NULL) continue;

            if (g_config.language == (byte)Language.FRENCH)
            {
                GUI_DrawText_Wrapper(GUI_String_Get_ByIndex((short)desc.widgets[i].labelStringId), (short)((g_widgetProperties[w.parentID].xBase << 3) + 40), (short)(w.offsetY + g_widgetProperties[w.parentID].yBase + 3), 232, 0, 0x22);
            }
            else
            {
                GUI_DrawText_Wrapper(GUI_String_Get_ByIndex((short)desc.widgets[i].labelStringId), (short)(w.offsetX + (g_widgetProperties[w.parentID].xBase << 3) - 10), (short)(w.offsetY + g_widgetProperties[w.parentID].yBase + 3), 232, 0, 0x222);
            }
        }

        if (s_savegameCountOnDisk >= 5 && desc.addArrows)
        {
            var w = g_table_windowWidgets[7];

            w.drawParameterNormal.sprite = g_sprites[59];
            w.drawParameterSelected.sprite = g_sprites[60];
            w.drawParameterDown.sprite = g_sprites[60];
            w.next = null;
            w.parentID = desc.index;

            GUI_Widget_MakeNormal(w, false);
            GUI_Widget_MakeInvisible(w);
            GUI_Widget_Undraw(w, 233);

            g_widgetLinkedListTail = GUI_Widget_Link(g_widgetLinkedListTail, w);

            w = g_table_windowWidgets[8];

            w.drawParameterNormal.sprite = g_sprites[61];
            w.drawParameterSelected.sprite = g_sprites[62];
            w.drawParameterDown.sprite = g_sprites[62];
            w.next = null;
            w.parentID = desc.index;

            GUI_Widget_MakeNormal(w, false);
            GUI_Widget_MakeInvisible(w);
            GUI_Widget_Undraw(w, 233);

            g_widgetLinkedListTail = GUI_Widget_Link(g_widgetLinkedListTail, w);
        }

        GUI_Mouse_Hide_Safe();

        Widget_SetCurrentWidget(desc.index);

        GUI_Screen_Copy((short)g_curWidgetXBase, (short)g_curWidgetYBase, (short)g_curWidgetXBase, (short)g_curWidgetYBase, (short)g_curWidgetWidth, (short)g_curWidgetHeight, Screen.NO1, Screen.NO0);

        GUI_Mouse_Show_Safe();

        GFX_Screen_SetActive(Screen.NO0);
    }

    /*
     * Handles Click event for scrollbar up button.
     *
     * @param w The widget.
     * @return False, always.
     */
    internal static bool GUI_Widget_Scrollbar_ArrowUp_Click(CWidget w)
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
    internal static bool GUI_Widget_Scrollbar_ArrowDown_Click(CWidget w)
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

        GUI_Widget_Scrollbar_CalculatePosition(scrollbar);

        GUI_Widget_Scrollbar_Draw(scrollbar.parent);
    }

    /*
     * Handles Click event for scrollbar button.
     *
     * @param w The widget.
     * @return False, always.
     */
    internal static bool GUI_Widget_Scrollbar_Click(CWidget w)
    {
        WidgetScrollbar scrollbar;
        ushort positionX, positionY;

        scrollbar = (WidgetScrollbar)w.data;

        positionX = (ushort)w.offsetX;
        if (w.offsetX < 0) positionX += (ushort)(g_widgetProperties[w.parentID].width << 3);
        positionX += (ushort)(g_widgetProperties[w.parentID].xBase << 3);

        positionY = (ushort)w.offsetY;
        if (w.offsetY < 0) positionY += g_widgetProperties[w.parentID].height;
        positionY += g_widgetProperties[w.parentID].yBase;

        if ((w.state.buttonState & 0x44) != 0)
        {
            scrollbar.pressed = 0;
            GUI_Widget_Scrollbar_Draw(w);
        }

        if ((w.state.buttonState & 0x11) != 0)
        {
            short positionCurrent;
            short positionBegin;
            short positionEnd;

            scrollbar.pressed = 0;

            if (w.width > w.height)
            {
                positionCurrent = (short)g_mouseX;
                positionBegin = (short)(positionX + scrollbar.position + 1);
            }
            else
            {
                positionCurrent = (short)g_mouseY;
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
                position = (short)(g_mouseX - scrollbar.pressedPosition - positionX - 1);
            }
            else
            {
                size = (short)(w.height - 2 - scrollbar.size);
                position = (short)(g_mouseY - scrollbar.pressedPosition - positionY - 1);
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

            GUI_Widget_Scrollbar_CalculateScrollPosition(scrollbar);

            if (scrollbar.dirty != 0) GUI_Widget_Scrollbar_Draw(w);
        }

        return false;
    }

    /*
     * Handles Click event for the list in production window.
     *
     * @return True, always.
     */
    internal static bool GUI_Production_List_Click(CWidget w)
    {
        GUI_FactoryWindow_B495_0F30();

        g_factoryWindowSelected = (ushort)(w.index - 46);

        GUI_FactoryWindow_DrawDetails();

        GUI_FactoryWindow_UpdateSelection(true);

        return true;
    }

    /*
     * Handles Click event for the "Resume Game" button in production window.
     *
     * @return True, always.
     */
    internal static bool GUI_Production_ResumeGame_Click(CWidget w)
    {
        g_factoryWindowResult = FactoryResult.FACTORY_RESUME;

        if (g_factoryWindowStarport)
        {
            byte i = 0;
            var h = g_playerHouse;
            while (g_factoryWindowOrdered != 0)
            {
                if (g_factoryWindowItems[i].amount != 0)
                {
                    h.credits += (ushort)(g_factoryWindowItems[i].amount * g_factoryWindowItems[i].credits);
                    g_factoryWindowOrdered -= (ushort)g_factoryWindowItems[i].amount;
                    g_factoryWindowItems[i].amount = 0;
                }

                i++;

                GUI_DrawCredits((byte)g_playerHouseID, 0);
            }
        }

        if (w != null) GUI_Widget_MakeNormal(w, false);

        return true;
    }

    /*
     * Handles Click event for the "Build this" button in production window.
     *
     * @return True, always.
     */
    internal static bool GUI_Production_BuildThis_Click(CWidget w)
    {
        if (g_factoryWindowStarport)
        {
            if (g_factoryWindowOrdered == 0)
            {
                GUI_Widget_MakeInvisible(w);
                GUI_Purchase_ShowInvoice();
                GUI_Widget_MakeVisible(w);
            }
            else
            {
                g_factoryWindowResult = FactoryResult.FACTORY_BUY;
            }
        }
        else
        {
            FactoryWindowItem item;
            ObjectInfo oi;

            item = GUI_FactoryWindow_GetItem((short)g_factoryWindowSelected);
            oi = item.objectInfo;

            if (oi.available > 0)
            {
                item.amount = 1;
                g_factoryWindowResult = FactoryResult.FACTORY_BUY;
            }
        }

        GUI_Widget_MakeNormal(w, false);

        return true;
    }

    /*
     * Handles Click event for the "Down" button in production window.
     *
     * @return True, always.
     */
    internal static bool GUI_Production_Down_Click(CWidget w)
    {
        var drawDetails = false;

        if (g_factoryWindowSelected < 3 && (g_factoryWindowSelected + 1) < g_factoryWindowTotal)
        {
            g_timerTimeout = 10;
            GUI_FactoryWindow_B495_0F30();
            g_factoryWindowSelected++;

            GUI_FactoryWindow_UpdateSelection(true);

            drawDetails = true;
        }
        else
        {
            if (g_factoryWindowBase + 4 < g_factoryWindowTotal)
            {
                g_timerTimeout = 10;
                g_factoryWindowBase++;
                drawDetails = true;

                GUI_FactoryWindow_ScrollList(1);

                GUI_FactoryWindow_UpdateSelection(true);
            }
            else
            {
                GUI_FactoryWindow_DrawDetails();

                GUI_FactoryWindow_FailScrollList(1);
            }
        }

        for (; g_timerTimeout != 0; SleepIdle())
        {
            GUI_FactoryWindow_UpdateSelection(false);
        }

        if (drawDetails) GUI_FactoryWindow_DrawDetails();

        GUI_Widget_MakeNormal(w, false);

        return true;
    }

    /*
     * Handles Click event for the "Up" button in production window.
     *
     * @return True, always.
     */
    internal static bool GUI_Production_Up_Click(CWidget w)
    {
        var drawDetails = false;

        if (g_factoryWindowSelected != 0)
        {
            g_timerTimeout = 10;
            GUI_FactoryWindow_B495_0F30();
            g_factoryWindowSelected--;

            GUI_FactoryWindow_UpdateSelection(true);

            drawDetails = true;
        }
        else
        {
            if (g_factoryWindowBase != 0)
            {
                g_timerTimeout = 10;
                g_factoryWindowBase--;
                drawDetails = true;

                GUI_FactoryWindow_ScrollList(-1);

                GUI_FactoryWindow_UpdateSelection(true);
            }
            else
            {
                GUI_FactoryWindow_DrawDetails();

                GUI_FactoryWindow_FailScrollList(-1);
            }
        }

        for (; g_timerTimeout != 0; SleepIdle())
        {
            GUI_FactoryWindow_UpdateSelection(false);
        }

        if (drawDetails) GUI_FactoryWindow_DrawDetails();

        GUI_Widget_MakeNormal(w, false);

        return true;
    }

    static void GUI_FactoryWindow_ScrollList(short step)
    {
        ushort i;
        ushort y = 32;

        GUI_FactoryWindow_B495_0F30();

        GUI_Mouse_Hide_Safe();

        for (i = 0; i < 32; i++)
        {
            y += (ushort)step;
            GFX_Screen_Copy2(72, (short)y, 72, 16, 32, 136, Screen.NO1, Screen.NO0, false);
        }

        GUI_Mouse_Show_Safe();

        GUI_FactoryWindow_PrepareScrollList();

        GUI_FactoryWindow_UpdateSelection(true);
    }

    static void GUI_FactoryWindow_FailScrollList(short step)
    {
        ushort i;
        ushort y = 32;

        GUI_FactoryWindow_B495_0F30();

        GUI_Mouse_Hide_Safe();

        GUI_FactoryWindow_B495_0F30();

        for (i = 0; i < 6; i++)
        {
            y += (ushort)step;
            GFX_Screen_Copy2(72, (short)y, 72, 16, 32, 136, Screen.NO1, Screen.NO0, false);
        }

        for (i = 0; i < 6; i++)
        {
            y -= (ushort)step;
            GFX_Screen_Copy2(72, (short)y, 72, 16, 32, 136, Screen.NO1, Screen.NO0, false);
        }

        GUI_Mouse_Show_Safe();

        GUI_FactoryWindow_UpdateSelection(true);
    }

    /*
     * Handles Click event for the "Ugrade" button in production window.
     *
     * @return True, always.
     */
    internal static bool GUI_Production_Upgrade_Click(CWidget w)
    {
        GUI_Widget_MakeNormal(w, false);

        g_factoryWindowResult = FactoryResult.FACTORY_UPGRADE;

        return true;
    }

    /*
     * Handles Click event for the "+" button in starport window.
     *
     * @return True, always.
     */
    internal static bool GUI_Purchase_Plus_Click(CWidget w)
    {
        var item = GUI_FactoryWindow_GetItem((short)g_factoryWindowSelected);
        var oi = item.objectInfo;
        var h = g_playerHouse;
        var canCreateMore = true;
        var type = item.objectType;

        GUI_Widget_MakeNormal(w, false);

        if (g_table_unitInfo[type].movementType is not ((ushort)MovementType.MOVEMENT_WINGER) and not ((ushort)MovementType.MOVEMENT_SLITHER))
        {
            if (g_starPortEnforceUnitLimit && h.unitCount >= h.unitCountMax) canCreateMore = false;
        }

        if (item.amount < oi.available && item.credits <= h.credits && canCreateMore)
        {
            item.amount++;

            GUI_FactoryWindow_UpdateDetails(item);

            g_factoryWindowOrdered++;

            h.credits -= (ushort)item.credits;

            GUI_FactoryWindow_DrawCaption(null);
        }

        return true;
    }

    /*
     * Handles Click event for the "-" button in startport window.
     *
     * @return True, always.
     */
    internal static bool GUI_Purchase_Minus_Click(CWidget w)
    {
        FactoryWindowItem item;
        var h = g_playerHouse;

        GUI_Widget_MakeNormal(w, false);

        item = GUI_FactoryWindow_GetItem((short)g_factoryWindowSelected);

        if (item.amount != 0)
        {
            item.amount--;

            GUI_FactoryWindow_UpdateDetails(item);

            g_factoryWindowOrdered--;

            h.credits += (ushort)item.credits;

            GUI_FactoryWindow_DrawCaption(null);
        }

        return true;
    }

    /*
     * Handles Click event for the "Invoice" button in starport window.
     *
     * @return True, always.
     */
    internal static bool GUI_Purchase_Invoice_Click(CWidget w)
    {
        GUI_Widget_MakeInvisible(w);
        GUI_Purchase_ShowInvoice();
        GUI_Widget_MakeVisible(w);
        GUI_Widget_MakeNormal(w, false);
        return true;
    }

    static void GUI_Purchase_ShowInvoice()
    {
        var w = g_widgetInvoiceTail;
        Screen oldScreenID;
        ushort y = 48;
        ushort total = 0;
        ushort x;
        string textBuffer; //char[12]

        oldScreenID = GFX_Screen_SetActive(Screen.NO1);

        GUI_DrawFilledRectangle(128, 48, 311, 159, 20);

        GUI_DrawText_Wrapper(String_Get_ByIndex(Text.STR_ITEM_NAME_QTY_TOTAL), 128, (short)y, 12, 0, 0x11);

        y += 7;

        GUI_DrawLine(129, (short)y, 310, (short)y, 12);

        y += 2;

        if (g_factoryWindowOrdered != 0)
        {
            ushort i;

            for (i = 0; i < g_factoryWindowTotal; i++)
            {
                ObjectInfo oi;
                ushort amount;

                if (g_factoryWindowItems[i].amount == 0) continue;

                amount = (ushort)(g_factoryWindowItems[i].amount * g_factoryWindowItems[i].credits);
                total += amount;

                textBuffer = string.Format(Culture, "{0:D2} {1, 5}", g_factoryWindowItems[i].amount, amount); //snprintf(textBuffer, sizeof(textBuffer), "%02d %5d", g_factoryWindowItems[i].amount, amount);

                oi = g_factoryWindowItems[i].objectInfo;
                GUI_DrawText_Wrapper(String_Get_ByIndex(oi.stringID_full), 128, (short)y, 8, 0, 0x11);

                GUI_DrawText_Monospace(textBuffer, (ushort)(311 - (short)textBuffer.Length * 6), y, 15, 0, 6);

                y += 8;
            }
        }
        else
        {
            GUI_DrawText_Wrapper(String_Get_ByIndex(Text.STR_NO_UNITS_ON_ORDER), 220, 99, 6, 0, 0x112);
        }

        GUI_DrawLine(129, 148, 310, 148, 12);
        GUI_DrawLine(129, 150, 310, 150, 12);

        textBuffer = total.ToString("D", Culture); //snprintf(textBuffer, sizeof(textBuffer), "%d", total);

        x = (ushort)(311 - (short)textBuffer.Length * 6);

        /* "Total Cost :" */
        GUI_DrawText_Wrapper(GUI_String_Get_ByIndex((short)Text.STR_TOTAL_COST_), (short)(x - 3), 152, 11, 0, 0x211);
        GUI_DrawText_Monospace(textBuffer, x, 152, 11, 0, 6);

        GUI_Mouse_Hide_Safe();
        GUI_Screen_Copy(16, 48, 16, 48, 23, 112, Screen.NO1, Screen.NO0);
        GUI_Mouse_Show_Safe();

        GFX_Screen_SetActive(Screen.NO0);

        GUI_FactoryWindow_DrawCaption(String_Get_ByIndex(Text.STR_INVOICE_OF_UNITS_ON_ORDER));

        Input_History_Clear();

        for (; GUI_Widget_HandleEvents(w) == 0; SleepIdle())
        {
            GUI_DrawCredits((byte)g_playerHouseID, 0);

            GUI_FactoryWindow_UpdateSelection(false);

            GUI_PaletteAnimate();
        }

        GFX_Screen_SetActive(oldScreenID);

        w = GUI_Widget_Get_ByIndex(w, 10);

        if (w != null && Mouse_InsideRegion(w.offsetX, w.offsetY, (short)(w.offsetX + w.width), (short)(w.offsetY + w.height)) != 0)
        {
            while (Input_Test(0x41) != 0 || Input_Test(0x42) != 0) SleepIdle();
            Input_History_Clear();
        }

        if (g_factoryWindowResult == FactoryResult.FACTORY_CONTINUE) GUI_FactoryWindow_DrawDetails();
    }

    /*
     * Handles Click event for "Options" button.
     *
     * @param w The widget.
     * @return False, always.
     */
    internal static bool GUI_Widget_Options_Click(CWidget w)
    {
        var desc = g_optionsWindowDesc;
        var cursor = g_cursorSpriteID;
        bool loop;

        g_cursorSpriteID = 0;

        Sprites_SetMouseSprite(0, 0, g_sprites[0]);

        Sprites_UnloadTiles();

        Buffer.BlockCopy(g_paletteActive, 0, g_palette_998A, 0, 256 * 3); //memmove(g_palette_998A, g_paletteActive, 256 * 3);

        Driver_Voice_Play(null, 0xFF);

        Timer_SetTimer(TimerType.TIMER_GAME, false);

        GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x22);

        ShadeScreen();

        GUI_Window_BackupScreen(desc);

        GUI_Window_Create(desc);

        for (loop = true; loop; SleepIdle())
        {
            var w2 = g_widgetLinkedListTail;
            var key = GUI_Widget_HandleEvents(w2);

            if ((key & 0x8000) != 0)
            {
                w = GUI_Widget_Get_ByIndex(w2, key);

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
                        g_gameMode = GameMode.GM_RESTART;
                        break;

                    case 4:
                        /* "Are you sure you wish to pick a new house?" */
                        if (!GUI_YesNo((ushort)Text.STR_ARE_YOU_SURE_YOU_WISH_TO_PICK_A_NEW_HOUSE)) break;

                        loop = false;
                        Driver_Music_FadeOut();
                        g_gameMode = GameMode.GM_PICKHOUSE;
                        break;

                    case 5:
                        loop = false;
                        break;

                    case 6:
                        /* "Are you sure you want to quit playing?" */
                        loop = !GUI_YesNo((ushort)Text.STR_ARE_YOU_SURE_YOU_WANT_TO_QUIT_PLAYING);
                        g_running = loop;

                        Sound_Output_Feedback(0xFFFE);

                        while (Driver_Voice_IsPlaying()) SleepIdle();
                        break;

                    default: break;
                }

                if (g_running && loop)
                {
                    GUI_Window_BackupScreen(desc);

                    GUI_Window_Create(desc);
                }
            }

            GUI_PaletteAnimate();
        }

        g_textDisplayNeedsUpdate = true;

        Sprites_LoadTiles();
        GUI_DrawInterfaceAndRadar(Screen.NO0);

        UnshadeScreen();

        GUI_Widget_MakeSelected(w, false);

        Timer_SetTimer(TimerType.TIMER_GAME, true);

        GameOptions_Save();

        Structure_Recount();
        Unit_Recount();

        g_cursorSpriteID = cursor;

        Sprites_SetMouseSprite(0, 0, g_sprites[cursor]);

        return false;
    }

    /* shade everything except colors 231 to 238 */
    static void ShadeScreen()
    {
        ushort i;

        Buffer.BlockCopy(g_palette1, 0, g_palette_998A, 0, 256 * 3); //memmove(g_palette_998A, g_palette1, 256 * 3);

        for (i = 0; i < 231 * 3; i++) g_palette1[i] = (byte)(g_palette1[i] / 2);
        for (i = 239 * 3; i < 256 * 3; i++) g_palette1[i] = (byte)(g_palette1[i] / 2);

        GFX_SetPalette(g_palette_998A);
    }

    static void UnshadeScreen()
    {
        Buffer.BlockCopy(g_palette_998A, 0, g_palette1, 0, 256 * 3); //memmove(g_palette1, g_palette_998A, 256 * 3);

        GFX_SetPalette(g_palette1);
    }

    /*
     * Handles Click event for "Game controls" button.
     *
     * @param w The widget.
     */
    static void GUI_Widget_GameControls_Click(CWidget w)
    {
        var desc = g_gameControlWindowDesc;
        bool loop;

        GUI_Window_BackupScreen(desc);

        GUI_Window_Create(desc);

        for (loop = true; loop; SleepIdle())
        {
            var w2 = g_widgetLinkedListTail;
            var key = GUI_Widget_HandleEvents(w2);

            if ((key & 0x8000) != 0)
            {
                w = GUI_Widget_Get_ByIndex(w2, (ushort)(key & 0x7FFF));

                switch ((key & 0x7FFF) - 0x1E)
                {
                    case 0:
                        g_gameConfig.music ^= 0x1;
                        if (g_gameConfig.music == 0) Driver_Music_Stop();
                        break;

                    case 1:
                        g_gameConfig.sounds ^= 0x1;
                        if (g_gameConfig.sounds == 0) Driver_Sound_Stop();
                        break;

                    case 2:
                        if (++g_gameConfig.gameSpeed >= 5) g_gameConfig.gameSpeed = 0;
                        break;

                    case 3:
                        g_gameConfig.hints ^= 0x1;
                        break;

                    case 4:
                        g_gameConfig.autoScroll ^= 0x1;
                        break;

                    case 5:
                        loop = false;
                        break;

                    default: break;
                }

                GUI_Widget_MakeNormal(w, false);

                GUI_Widget_Draw(w);
            }

            GUI_PaletteAnimate();
        }

        GUI_Window_RestoreScreen(desc);
    }
}
