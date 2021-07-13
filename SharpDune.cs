/* Gameloop and other main routines */

// Master TODO list:
// - reduce casts
// - speed up array operations: Array.Copy => Buffer.BlockCopy(faster?), ...
// - clean up
// - use CArray where appropriate
// - prefix all top level class names with "C"?

//INT - BOOL: zero is false and all other values are true

using SharpDune;
using System;
using System.Diagnostics;
using System.Text;
using static System.Math;

enum GameMode
{
    GM_MENU = 0,
    GM_NORMAL = 1,
    GM_RESTART = 2,
    GM_PICKHOUSE = 3
}

/* X and Y coordinate. */
class XYPosition
{
    internal ushort x; /*!< X coordinate. */
    internal ushort y; /*!< Y coordinate. */
}

class CSharpDune
{
    internal const string window_caption = "SharpDUNE - v0.1";
    internal static bool g_dune2_enhanced = true; /*!< If false, the game acts exactly like the original Dune2, including bugs. */
    internal static bool g_starPortEnforceUnitLimit; /*!< If true, one cannot circumvent unit cap using starport */
    internal static bool g_unpackSHPonLoad = true; /*!< If true, Format80 encoded sprites from SHP files will be decoded on load. set to false to save memory */

    internal static uint g_hintsShown1; /*!< A bit-array to indicate which hints has been show already (0-31). */
    internal static uint g_hintsShown2; /*!< A bit-array to indicate which hints has been show already (32-63). */
    internal static GameMode g_gameMode = GameMode.GM_MENU;
    internal static ushort g_campaignID;
    internal static ushort g_scenarioID = 1;
    internal static ushort g_activeAction = 0xFFFF; /*!< Action the controlled unit will do. */
    internal static uint g_tickScenarioStart; /*!< The tick the scenario started in. */
    static readonly uint s_tickGameTimeout; /*!< The tick the game will timeout. */

    internal static bool g_debugGame; /*!< When true, you can control the AI. */
    internal static bool g_debugScenario; /*!< When true, you can review the scenario. There is no fog. The game is not running (no unit-movement, no structure-building, etc). You can click on individual tiles. */
    internal static bool g_debugSkipDialogs; /*!< When non-zero, you immediately go to house selection, and skip all intros. */

    internal static byte[] g_readBuffer;
    internal static uint g_readBufferSize;

    static bool s_debugForceWin; /*!< When true, you immediately win the level. */

    static byte s_enableLog; /*!< 0 = off, 1 = record game, 2 = playback game (stored in 'dune.log'). */

    internal static ushort g_validateStrictIfZero; /*!< 0 = strict validation, basically: no-cheat-mode. */
    internal static bool g_running = true; /*!< true if game needs to keep running; false to stop the game. */
    internal static ushort g_selectionType;
    internal static ushort g_selectionTypeNew;
    internal static bool g_viewport_forceRedraw; /*!< Force a full redraw of the screen. */
    internal static bool g_viewport_fadein; /*!< Fade in the screen. */

    internal static short g_musicInBattle; /*!< 0 = no battle, 1 = fight is going on, -1 = music of fight is going on is active. */

    internal static Encoding Encoding = Encoding.UTF7; //Encoding.ASCII

    internal static MultiChar MultiChar = new();

    /*
    * Check if a level is finished, based on the values in WinFlags.
    *
    * @return True if and only if the level has come to an end.
    */
    static bool GameLoop_IsLevelFinished()
    {
        var finish = false;

        if (s_debugForceWin) return true;

        /* You have to play at least 7200 ticks before you can win the game */
        if (Timer.g_timerGame - g_tickScenarioStart < 7200) return false;

        /* Check for structure counts hitting zero */
        if ((CScenario.g_scenario.winFlags & 0x3) != 0)
        {
            var find = new PoolFindStruct();
            ushort countStructureEnemy = 0;
            ushort countStructureFriendly = 0;

            find.houseID = (byte)HouseType.HOUSE_INVALID;
            find.type = 0xFFFF;
            find.index = 0xFFFF;

            /* Calculate how many structures are left on the map */
            while (true)
            {
                Structure s;

                s = CStructure.Structure_Find(find);
                if (s == null) break;

                if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;
                if (s.o.type == (byte)StructureType.STRUCTURE_TURRET) continue;
                if (s.o.type == (byte)StructureType.STRUCTURE_ROCKET_TURRET) continue;

                if (s.o.houseID == (byte)CHouse.g_playerHouseID)
                {
                    countStructureFriendly++;
                }
                else
                {
                    countStructureEnemy++;
                }
            }

            if ((CScenario.g_scenario.winFlags & 0x1) != 0 && countStructureEnemy == 0)
            {
                finish = true;
            }
            if ((CScenario.g_scenario.winFlags & 0x2) != 0 && countStructureFriendly == 0)
            {
                finish = true;
            }
        }

        /* Check for reaching spice quota */
        if ((CScenario.g_scenario.winFlags & 0x4) != 0 && CHouse.g_playerCredits != 0xFFFF)
        {
            if (CHouse.g_playerCredits >= CHouse.g_playerHouse.creditsQuota)
            {
                finish = true;
            }
        }

        /* Check for reaching timeout */
        if ((CScenario.g_scenario.winFlags & 0x8) != 0)
        {
            /* XXX -- This code was with '<' instead of '>=', which makes
             *  no sense. As it is unused, who knows what the intentions
             *  were. This at least makes it sensible. */
            if (Timer.g_timerGame >= s_tickGameTimeout)
            {
                finish = true;
            }
        }

        return finish;
    }

    /*
    * Check if a level is won, based on the values in LoseFlags.
    *
    * @return True if and only if the level has been won by the human.
    */
    static bool GameLoop_IsLevelWon()
    {
        var win = false;

        if (s_debugForceWin) return true;

        /* Check for structure counts hitting zero */
        if ((CScenario.g_scenario.loseFlags & 0x3) != 0)
        {
            var find = new PoolFindStruct();
            ushort countStructureEnemy = 0;
            ushort countStructureFriendly = 0;

            find.houseID = (byte)HouseType.HOUSE_INVALID;
            find.type = 0xFFFF;
            find.index = 0xFFFF;

            /* Calculate how many structures are left on the map */
            while (true)
            {
                Structure s;

                s = CStructure.Structure_Find(find);
                if (s == null) break;

                if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;
                if (s.o.type == (byte)StructureType.STRUCTURE_TURRET) continue;
                if (s.o.type == (byte)StructureType.STRUCTURE_ROCKET_TURRET) continue;

                if (s.o.houseID == (byte)CHouse.g_playerHouseID)
                {
                    countStructureFriendly++;
                }
                else
                {
                    countStructureEnemy++;
                }
            }

            win = true;
            if ((CScenario.g_scenario.loseFlags & 0x1) != 0)
            {
                win = win && (countStructureEnemy == 0);
            }
            if ((CScenario.g_scenario.loseFlags & 0x2) != 0)
            {
                win = win && (countStructureFriendly != 0);
            }
        }

        /* Check for reaching spice quota */
        if (!win && (CScenario.g_scenario.loseFlags & 0x4) != 0 && CHouse.g_playerCredits != 0xFFFF)
        {
            win = (CHouse.g_playerCredits >= CHouse.g_playerHouse.creditsQuota);
        }

        /* Check for reaching timeout */
        if (!win && (CScenario.g_scenario.loseFlags & 0x8) != 0)
        {
            win = (Timer.g_timerGame < s_tickGameTimeout);
        }

        return win;
    }

    internal static void GameLoop_Uninit()
    {
        while (CWidget.g_widgetLinkedListHead != null)
        {
            var w = CWidget.g_widgetLinkedListHead;
            CWidget.g_widgetLinkedListHead = w.next;

            w = null;
        }

        Script.Script_ClearInfo(Script.g_scriptStructure);
        Script.Script_ClearInfo(Script.g_scriptTeam);

        g_readBuffer = null;

        Gfx.g_palette1 = null;
        Gfx.g_palette2 = null;
        Gfx.g_paletteMapping1 = null;
        Gfx.g_paletteMapping2 = null;
    }

    static uint levelEndTimer;
    /*
    * Checks if the level comes to an end. If so, it shows all end-level stuff,
    * and prepares for the next level.
    */
    static void GameLoop_LevelEnd()
    {
        if (levelEndTimer >= Timer.g_timerGame && !s_debugForceWin) return;

        if (GameLoop_IsLevelFinished())
        {
            Sound.Music_Play(0);

            Gui.g_cursorSpriteID = 0;

            Sprites.Sprites_SetMouseSprite(0, 0, Sprites.g_sprites[0]);

            Sound.Sound_Output_Feedback(0xFFFE);

            Gui.GUI_ChangeSelectionType((ushort)SelectionType.SELECTIONTYPE_MENTAT);

            if (GameLoop_IsLevelWon())
            {
                Sound.Sound_Output_Feedback(40);

                Gui.GUI_DisplayModalMessage(CString.String_Get_ByIndex(Text.STR_YOU_HAVE_SUCCESSFULLY_COMPLETED_YOUR_MISSION), 0xFFFF);

                Mentat.GUI_Mentat_ShowWin();

                Sprites.Sprites_UnloadTiles();

                g_campaignID++;

                Gui.GUI_EndStats_Show(CScenario.g_scenario.killedAllied, CScenario.g_scenario.killedEnemy, CScenario.g_scenario.destroyedAllied, CScenario.g_scenario.destroyedEnemy,
                    CScenario.g_scenario.harvestedAllied, CScenario.g_scenario.harvestedEnemy, (short)CScenario.g_scenario.score, (byte)CHouse.g_playerHouseID);

                if (g_campaignID == 9)
                {
                    Gui.GUI_Mouse_Hide_Safe();

                    Gui.GUI_SetPaletteAnimated(Gfx.g_palette2, 15);
                    Gui.GUI_ClearScreen(Screen.SCREEN_0);
                    Cutscene.GameLoop_GameEndAnimation();
                    PrepareEnd();
                    Environment.Exit(0);
                }

                Gui.GUI_Mouse_Hide_Safe();
                Cutscene.GameLoop_LevelEndAnimation();
                Gui.GUI_Mouse_Show_Safe();

                CFile.File_ReadBlockFile("IBM.PAL", Gfx.g_palette1, 256 * 3);

                g_scenarioID = Gui.GUI_StrategicMap_Show(g_campaignID, true);

                Gui.GUI_SetPaletteAnimated(Gfx.g_palette2, 15);

                if (g_campaignID == 1 || g_campaignID == 7)
                {
                    if (!Security.GUI_Security_Show())
                    {
                        PrepareEnd();
                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                Sound.Sound_Output_Feedback(41);

                Gui.GUI_DisplayModalMessage(CString.String_Get_ByIndex(Text.STR_YOU_HAVE_FAILED_YOUR_MISSION), 0xFFFF);

                Mentat.GUI_Mentat_ShowLose();

                Sprites.Sprites_UnloadTiles();

                g_scenarioID = Gui.GUI_StrategicMap_Show(g_campaignID, false);
            }

            CHouse.g_playerHouse.flags.doneFullScaleAttack = false;

            Sprites.Sprites_LoadTiles();

            g_gameMode = GameMode.GM_RESTART;
            s_debugForceWin = false;
        }

        levelEndTimer = Timer.g_timerGame + 300;
    }

    static void GameLoop_DrawMenu(string[] strings)
    {
        WidgetProperties props;
        ushort left;
        ushort top;
        byte i;

        props = CWidget.g_widgetProperties[21];
        top = (ushort)(CWidget.g_curWidgetYBase + props.yBase);
        left = (ushort)((CWidget.g_curWidgetXBase + props.xBase) << 3);

        Gui.GUI_Mouse_Hide_Safe();

        for (i = 0; i < props.height; i++)
        {
            var pos = (ushort)(top + CFont.g_fontCurrent.height * i);

            if (i == props.fgColourBlink)
            {
                Gui.GUI_DrawText_Wrapper(strings[i], (short)left, (short)pos, props.fgColourSelected, 0, 0x22);
            }
            else
            {
                Gui.GUI_DrawText_Wrapper(strings[i], (short)left, (short)pos, props.fgColourNormal, 0, 0x22);
            }
        }

        Gui.GUI_Mouse_Show_Safe();

        Input.Input_History_Clear();
    }

    static void GameLoop_DrawText2(string str, ushort left, ushort top, byte fgColourNormal, byte fgColourSelected, byte bgColour)
    {
        byte i;

        for (i = 0; i < 3; i++)
        {
            Gui.GUI_Mouse_Hide_Safe();

            Gui.GUI_DrawText_Wrapper(str, (short)left, (short)top, fgColourSelected, bgColour, 0x22);
            Timer.Timer_Sleep(2);

            Gui.GUI_DrawText_Wrapper(str, (short)left, (short)top, fgColourNormal, bgColour, 0x22);
            Gui.GUI_Mouse_Show_Safe();
            Timer.Timer_Sleep(2);
        }
    }

    static bool GameLoop_IsInRange(ushort x, ushort y, ushort minX, ushort minY, ushort maxX, ushort maxY) =>
        x >= minX && x <= maxX && y >= minY && y <= maxY;

    static ushort GameLoop_HandleEvents(string[] strings)
    {
        byte last;
        ushort result;
        ushort key;
        ushort top;
        ushort left;
        ushort minX;
        ushort maxX;
        ushort minY;
        ushort maxY;
        ushort lineHeight;
        byte fgColourNormal;
        byte fgColourSelected;
        byte old;
        WidgetProperties props;
        byte current;

        props = CWidget.g_widgetProperties[21];

        last = (byte)(props.height - 1);
        old = (byte)(props.fgColourBlink % (last + 1));
        current = old;

        result = 0xFFFF;

        top = (ushort)(CWidget.g_curWidgetYBase + props.yBase);
        left = (ushort)((CWidget.g_curWidgetXBase + props.xBase) << 3);

        lineHeight = CFont.g_fontCurrent.height;

        minX = (ushort)((CWidget.g_curWidgetXBase << 3) + (CFont.g_fontCurrent.maxWidth * props.xBase));
        minY = (ushort)(CWidget.g_curWidgetYBase + props.yBase);
        maxX = (ushort)(minX + (CFont.g_fontCurrent.maxWidth * props.width) - 1);
        maxY = (ushort)(minY + (props.height * lineHeight) - 1);

        fgColourNormal = props.fgColourNormal;
        fgColourSelected = props.fgColourSelected;

        key = 0;
        if (Input.Input_IsInputAvailable() != 0)
        {
            key = (ushort)(Input.Input_Wait() & 0x8FF);
        }

        if (Mouse.g_mouseDisabled == 0)
        {
            var y = Mouse.g_mouseY;

            if (GameLoop_IsInRange(Mouse.g_mouseX, y, minX, minY, maxX, maxY))
            {
                current = (byte)((y - minY) / lineHeight);
            }
        }

        switch (key)
        {
            case 0x60: /* NUMPAD 8 / ARROW UP */
                if (current-- == 0) current = last;
                break;

            case 0x62: /* NUMPAD 2 / ARROW DOWN */
                if (current++ == last) current = 0;
                break;

            case 0x5B: /* NUMPAD 7 / HOME */
            case 0x65: /* NUMPAD 9 / PAGE UP */
                current = 0;
                break;

            case 0x5D: /* NUMPAD 1 / END */
            case 0x67: /* NUMPAD 3 / PAGE DOWN */
                current = last;
                break;

            case 0x41: /* MOUSE LEFT BUTTON */
            case 0x42: /* MOUSE RIGHT BUTTON */
                if (GameLoop_IsInRange(Mouse.g_mouseClickX, Mouse.g_mouseClickY, minX, minY, maxX, maxY))
                {
                    current = (byte)((Mouse.g_mouseClickY - minY) / lineHeight);
                    result = current;
                }
                break;

            case 0x2B: /* NUMPAD 5 / RETURN */
            case 0x3D: /* SPACE */
            case 0x61:
                result = current;
                break;

            default:
                {
                    byte i;

                    for (i = 0; i < props.height; i++)
                    {
                        char c1;
                        var c2 = (char)27;

                        c1 = char.ToUpper(strings[0][i]); //TODO: Or strings[i][0]?
                        var chr = (char)Input.Input_Keyboard_HandleKeys((ushort)(key & 0xFF));
                        if (char.IsLetterOrDigit(chr))
                            c2 = char.ToUpper(chr);

                        if (c1 == c2)
                        {
                            result = i;
                            current = i;
                            break;
                        }
                    }
                }
                break;
        }

        if (current != old)
        {
            Gui.GUI_Mouse_Hide_Safe();
            Gui.GUI_DrawText_Wrapper(strings[old], (short)left, (short)(top + (old * lineHeight)), fgColourNormal, 0, 0x22);
            Gui.GUI_DrawText_Wrapper(strings[current], (short)left, (short)(top + (current * lineHeight)), fgColourSelected, 0, 0x22);
            Gui.GUI_Mouse_Show_Safe();
        }

        props.fgColourBlink = current;

        if (result == 0xFFFF) return 0xFFFF;

        Gui.GUI_Mouse_Hide_Safe();
        GameLoop_DrawText2(strings[result], left, (ushort)(top + (current * lineHeight)), fgColourNormal, fgColourSelected, 0);
        Gui.GUI_Mouse_Show_Safe();

        return result;
    }

    static void Window_WidgetClick_Create()
    {
        WidgetInfo wi;
        int pointer;

        for (pointer = 0; CWidgetInfo.g_table_gameWidgetInfo[pointer].index >= 0; pointer++)
        {
            wi = CWidgetInfo.g_table_gameWidgetInfo[pointer];
            Widget w;

            w = CWidget.GUI_Widget_Allocate((ushort)wi.index, (ushort)wi.shortcut, wi.offsetX, wi.offsetY, (ushort)wi.spriteID, wi.stringID);

            if (wi.spriteID < 0)
            {
                w.width = wi.width;
                w.height = wi.height;
            }

            w.clickProc = wi.clickProc;
            w.flags.Set(wi.flags);

            CWidget.g_widgetLinkedListHead = CWidget.GUI_Widget_Insert(CWidget.g_widgetLinkedListHead, w);
        }
    }

    static void ReadProfileIni(string filename)
    {
        /* char * */
        byte[] source;
        /* char * */
        string key;
        /* char * */
        string keys; //char[]
        /* char[120] */
        string buffer = null; //char[120]
        string[] bufferStrings;
        string sourceString;
        ushort locsi;

        if (filename == null) return;
        if (!CFile.File_Exists(filename)) return;

        source = Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_1);

        Array.Fill<byte>(source, 0, 0, 32000); //memset(source, 0, 32000);

        CFile.File_ReadBlockFile(filename, source, Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_1));

        keys = Encoding.GetString(source[(source.Length + 5000)..]);
        //*keys = '\0';

        sourceString = Encoding.GetString(source);
        keys = Ini.Ini_GetString("construct", null, keys, /*ref keys,*/ 2000, sourceString);

        var keyPointer = 0;
        for (key = keys; key[keyPointer] != '\r'; keyPointer++)
        { //key += strlen(key) + 1) {
            ObjectInfo oi = null;
            //ushort count;
            byte type;
            ushort buildCredits;
            ushort buildTime;
            ushort fogUncoverRadius;
            ushort availableCampaign;
            ushort sortPriority = 0;
            ushort priorityBuild;
            ushort priorityTarget;
            ushort hitpoints;

            type = CUnit.Unit_StringToType(key);
            if (type != (byte)UnitType.UNIT_INVALID)
            {
                oi = CUnit.g_table_unitInfo[type].o;
            }
            else
            {
                type = CStructure.Structure_StringToType(key);
                if (type != (byte)StructureType.STRUCTURE_INVALID) oi = CStructure.g_table_structureInfo[type].o;
            }

            if (oi == null) continue;

            buffer = Ini.Ini_GetString("construct", key, buffer, /*ref buffer,*/ 120, sourceString);
            bufferStrings = buffer.Split(",");
            buildCredits = ushort.Parse(bufferStrings[0]);
            buildTime = ushort.Parse(bufferStrings[1]);
            hitpoints = ushort.Parse(bufferStrings[2]);
            fogUncoverRadius = ushort.Parse(bufferStrings[3]);
            availableCampaign = ushort.Parse(bufferStrings[4]);
            priorityBuild = ushort.Parse(bufferStrings[5]);
            priorityTarget = ushort.Parse(bufferStrings[6]);
            if (bufferStrings.Length > 7)
                sortPriority = ushort.Parse(bufferStrings[7]);
            //count = sscanf(buffer, "%hu,%hu,%hu,%hu,%hu,%hu,%hu,%hu", &buildCredits, &buildTime, &hitpoints, &fogUncoverRadius, &availableCampaign, &priorityBuild, &priorityTarget, &sortPriority);
            oi.buildCredits = buildCredits;
            oi.buildTime = buildTime;
            oi.hitpoints = hitpoints;
            oi.fogUncoverRadius = fogUncoverRadius;
            oi.availableCampaign = availableCampaign;
            oi.priorityBuild = priorityBuild;
            oi.priorityTarget = priorityTarget;
            if (bufferStrings.Length <= 7) continue;
            oi.sortPriority = (byte)sortPriority;
        }

        if (g_debugGame)
        {
            for (locsi = 0; locsi < (ushort)UnitType.UNIT_MAX; locsi++)
            {
                var oi = CUnit.g_table_unitInfo[locsi].o;

                buffer = $"{oi.name.PadRight(15 - oi.name.Length, ' ')}{oi.buildCredits},{oi.buildTime},{oi.hitpoints},{oi.fogUncoverRadius},{oi.availableCampaign},{oi.priorityBuild},{oi.priorityTarget},{oi.sortPriority}";
                //sprintf(buffer, "%*s%4d,%4d,%4d,%4d,%4d,%4d,%4d,%4d",
                //	15 - (int)strlen(oi->name), string.Empty, oi->buildCredits, oi->buildTime, oi->hitpoints, oi->fogUncoverRadius,
                //	oi->availableCampaign, oi->priorityBuild, oi->priorityTarget, oi->sortPriority);

                Ini.Ini_SetString("construct", oi.name, buffer, Encoding.GetString(source));
            }

            for (locsi = 0; locsi < (ushort)StructureType.STRUCTURE_MAX; locsi++)
            {
                var oi = CStructure.g_table_structureInfo[locsi].o;

                buffer = $"{oi.name.PadRight(15 - oi.name.Length, ' ')}{oi.buildCredits},{oi.buildTime},{oi.hitpoints},{oi.fogUncoverRadius},{oi.availableCampaign},{oi.priorityBuild},{oi.priorityTarget},{oi.sortPriority}";
                //sprintf(buffer, "%*s%4d,%4d,%4d,%4d,%4d,%4d,%4d,%4d",
                //	15 - (int)strlen(oi->name), string.Empty, oi->buildCredits, oi->buildTime, oi->hitpoints, oi->fogUncoverRadius,
                //	oi->availableCampaign, oi->priorityBuild, oi->priorityTarget, oi->sortPriority);

                Ini.Ini_SetString("construct", oi.name, buffer, Encoding.GetString(source));
            }
        }

        //*keys = '\0';

        keys = Ini.Ini_GetString("combat", null, keys, /*ref keys,*/ 2000, sourceString);

        for (key = keys; key[keyPointer] != '\r'; keyPointer++)
        { //key += strlen(key) + 1) {
            ushort damage;
            ushort movingSpeedFactor;
            ushort fireDelay;
            ushort fireDistance;

            buffer = Ini.Ini_GetString("combat", key, buffer, /*ref buffer,*/ 120, sourceString);

            bufferStrings = buffer.Trim().Split(","); //String_Trim(buffer);
            if (bufferStrings.Length < 4) continue;
            fireDistance = ushort.Parse(bufferStrings[0]);
            damage = ushort.Parse(bufferStrings[1]);
            fireDelay = ushort.Parse(bufferStrings[2]);
            movingSpeedFactor = ushort.Parse(bufferStrings[3]);
            //if (sscanf(buffer, "%hu,%hu,%hu,%hu", &fireDistance, &damage, &fireDelay, &movingSpeedFactor) < 4) continue;

            for (locsi = 0; locsi < (ushort)UnitType.UNIT_MAX; locsi++)
            {
                var ui = CUnit.g_table_unitInfo[locsi];

                if (!string.Equals(ui.o.name, key, StringComparison.OrdinalIgnoreCase)) continue;
                //if (strcasecmp(ui->o.name, key) != 0) continue;

                ui.damage = damage;
                ui.movingSpeedFactor = movingSpeedFactor;
                ui.fireDelay = fireDelay;
                ui.fireDistance = fireDistance;
                break;
            }
        }

        if (!g_debugGame) return;

        for (locsi = 0; locsi < (ushort)UnitType.UNIT_MAX; locsi++)
        {
            var ui = CUnit.g_table_unitInfo[locsi];

            buffer = $"{ui.o.name.PadRight(15 - ui.o.name.Length, ' ')}{ui.fireDistance},{ui.damage},{ui.fireDelay},{ui.movingSpeedFactor}";
            //sprintf(buffer, "%*s%4d,%4d,%4d,%4d", 15 - (int)strlen(ui->o.name), string.Empty, ui->fireDistance, ui->damage, ui->fireDelay, ui->movingSpeedFactor);
            Ini.Ini_SetString("combat", ui.o.name, buffer, Encoding.GetString(source));
        }
    }

    static bool drawMenu = true;
    static ushort stringID = (ushort)Text.STR_REPLAY_INTRODUCTION;
    static bool hasSave;
    static bool hasFame;
    static readonly string[] strings = new string[6];
    static ushort index = 0xFFFF;
    static readonly ushort[][] mainMenuStrings = { //[][6]
			new[] { (ushort)Text.STR_PLAY_A_GAME, (ushort)Text.STR_REPLAY_INTRODUCTION, (ushort)Text.STR_EXIT_GAME, (ushort)Text.STR_NULL,         (ushort)Text.STR_NULL,         (ushort)Text.STR_NULL }, /* Neither HOF nor save. */
			new[] { (ushort)Text.STR_PLAY_A_GAME, (ushort)Text.STR_REPLAY_INTRODUCTION, (ushort)Text.STR_LOAD_GAME, (ushort)Text.STR_EXIT_GAME,    (ushort)Text.STR_NULL,         (ushort)Text.STR_NULL }, /* Has a save game. */
			new[] { (ushort)Text.STR_PLAY_A_GAME, (ushort)Text.STR_REPLAY_INTRODUCTION, (ushort)Text.STR_EXIT_GAME, (ushort)Text.STR_HALL_OF_FAME, (ushort)Text.STR_NULL,         (ushort)Text.STR_NULL }, /* Has a HOF. */
			new[] { (ushort)Text.STR_PLAY_A_GAME, (ushort)Text.STR_REPLAY_INTRODUCTION, (ushort)Text.STR_LOAD_GAME, (ushort)Text.STR_EXIT_GAME,    (ushort)Text.STR_HALL_OF_FAME, (ushort)Text.STR_NULL }  /* Has a HOF and a save game. */
		};
    /*
     * Intro menu.
     */
    static void GameLoop_GameIntroAnimationMenu()
    {
        var loadGame = false;
        ushort maxWidth;

        if (index == 0xFFFF)
        {
            hasSave = CFile.File_Exists_Personal("_save000.dat");
            hasFame = CFile.File_Exists_Personal("SAVEFAME.DAT");
            index = (ushort)((hasFame ? 2 : 0) + (hasSave ? 1 : 0));
        }

        if (!Cutscene.g_canSkipIntro)
        {
            if (hasSave) Cutscene.g_canSkipIntro = true;
        }

        switch ((Text)stringID)
        {
            case Text.STR_REPLAY_INTRODUCTION:
                Sound.Music_Play(0);

                g_readBuffer = null; //free(g_readBuffer);
                g_readBufferSize = (uint)(!Config.g_enableVoices ? 12000 : 28000);
                g_readBuffer = new byte[g_readBufferSize]; //calloc(1, g_readBufferSize);

                Gui.GUI_Mouse_Hide_Safe();

                CDriver.Driver_Music_FadeOut();

                Cutscene.GameLoop_GameIntroAnimation();

                Sound.Sound_Output_Feedback(0xFFFE);

                CFile.File_ReadBlockFile("IBM.PAL", Gfx.g_palette1, 256 * 3);

                if (!Cutscene.g_canSkipIntro)
                {
                    CFile.File_Create_Personal("ONETIME.DAT");
                    Cutscene.g_canSkipIntro = true;
                }

                Sound.Music_Play(0);

                g_readBuffer = null; //free(g_readBuffer);
                g_readBufferSize = (uint)(!Config.g_enableVoices ? 12000 : 20000);
                g_readBuffer = new byte[g_readBufferSize]; //calloc(1, g_readBufferSize);

                Gui.GUI_Mouse_Show_Safe();

                Sound.Music_Play(28);

                drawMenu = true;
                break;

            case Text.STR_EXIT_GAME:
                g_running = false;
                return;

            case Text.STR_HALL_OF_FAME:
                Gui.GUI_HallOfFame_Show(0xFFFF);

                Gfx.GFX_SetPalette(Gfx.g_palette2);

                hasFame = CFile.File_Exists_Personal("SAVEFAME.DAT");
                drawMenu = true;
                break;

            case Text.STR_LOAD_GAME:
                Gui.GUI_Mouse_Hide_Safe();
                Gui.GUI_SetPaletteAnimated(Gfx.g_palette2, 30);
                Gui.GUI_ClearScreen(Screen.SCREEN_0);
                Gui.GUI_Mouse_Show_Safe();

                Gfx.GFX_SetPalette(Gfx.g_palette1);

                if (WidgetClick.GUI_Widget_SaveLoad_Click(false))
                {
                    loadGame = true;
                    if (g_gameMode == GameMode.GM_RESTART) break;
                    g_gameMode = GameMode.GM_NORMAL;
                }
                else
                {
                    Gfx.GFX_SetPalette(Gfx.g_palette2);

                    drawMenu = true;
                }
                break;

            default: break;
        }

        if (drawMenu)
        {
            ushort i;

            CWidget.g_widgetProperties[21].height = 0;

            for (i = 0; i < 6; i++)
            {
                strings[i] = null;

                if (mainMenuStrings[index][i] == 0)
                {
                    if (CWidget.g_widgetProperties[21].height == 0) CWidget.g_widgetProperties[21].height = i;
                    continue;
                }

                strings[i] = CString.String_Get_ByIndex(mainMenuStrings[index][i]);
            }

            Gui.GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x22);

            maxWidth = 0;

            for (i = 0; i < CWidget.g_widgetProperties[21].height; i++)
            {
                if (CFont.Font_GetStringWidth(strings[i]) <= maxWidth) continue;
                maxWidth = CFont.Font_GetStringWidth(strings[i]);
            }

            maxWidth += 7;

            CWidget.g_widgetProperties[21].width = (ushort)(maxWidth >> 3);
            CWidget.g_widgetProperties[13].width = (ushort)(CWidget.g_widgetProperties[21].width + 2);
            CWidget.g_widgetProperties[13].xBase = (ushort)(19 - (maxWidth >> 4));
            CWidget.g_widgetProperties[13].yBase = (ushort)(160 - ((CWidget.g_widgetProperties[21].height * CFont.g_fontCurrent.height) >> 1));
            CWidget.g_widgetProperties[13].height = (ushort)((CWidget.g_widgetProperties[21].height * CFont.g_fontCurrent.height) + 11);

            Sprites.Sprites_LoadImage(CString.String_GenerateFilename("TITLE"), Screen.SCREEN_1, null);

            Gui.GUI_Mouse_Hide_Safe();

            Gui.GUI_ClearScreen(Screen.SCREEN_0);

            Gui.GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, Screen.SCREEN_1, Screen.SCREEN_0);

            Gui.GUI_SetPaletteAnimated(Gfx.g_palette1, 30);

            Gui.GUI_DrawText_Wrapper("V1.07", 319, 192, 133, 0, 0x231, 0x39);
            Gui.GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x22);

            CWidget.Widget_SetCurrentWidget(13);

            WidgetDraw.GUI_Widget_DrawBorder(13, 2, true/*1*/);

            GameLoop_DrawMenu(strings);

            Gui.GUI_Mouse_Show_Safe();

            drawMenu = false;
        }

        if (loadGame) return;

        stringID = GameLoop_HandleEvents(strings);

        if (stringID != 0xFFFF) stringID = mainMenuStrings[index][stringID];

        Gui.GUI_PaletteAnimate();

        if (stringID == (ushort)Text.STR_PLAY_A_GAME) g_gameMode = GameMode.GM_PICKHOUSE;
    }

    static void InGame_Numpad_Move(ushort key)
    {
        if (key == 0) return;

        switch (key)
        {
            case 0x0010: /* TAB */
                Map.Map_SelectNext(true);
                return;

            case 0x0110: /* SHIFT TAB */
                Map.Map_SelectNext(false);
                return;

            case 0x005C: /* NUMPAD 4 / ARROW LEFT */
            case 0x045C:
            case 0x055C:
                Map.Map_MoveDirection(6);
                return;

            case 0x0066: /* NUMPAD 6 / ARROW RIGHT */
            case 0x0466:
            case 0x0566:
                Map.Map_MoveDirection(2);
                return;

            case 0x0060: /* NUMPAD 8 / ARROW UP */
            case 0x0460:
            case 0x0560:
                Map.Map_MoveDirection(0);
                return;

            case 0x0062: /* NUMPAD 2 / ARROW DOWN */
            case 0x0462:
            case 0x0562:
                Map.Map_MoveDirection(4);
                return;

            case 0x005B: /* NUMPAD 7 / HOME */
            case 0x045B:
            case 0x055B:
                Map.Map_MoveDirection(7);
                return;

            case 0x005D: /* NUMPAD 1 / END */
            case 0x045D:
            case 0x055D:
                Map.Map_MoveDirection(5);
                return;

            case 0x0065: /* NUMPAD 9 / PAGE UP */
            case 0x0465:
            case 0x0565:
                Map.Map_MoveDirection(1);
                return;

            case 0x0067: /* NUMPAD 3 / PAGE DOWN */
            case 0x0467:
            case 0x0567:
                Map.Map_MoveDirection(3);
                return;

            default: return;
        }
    }

    static uint l_timerNext;
    static uint l_timerUnitStatus;
    static short l_selectionState = -2;
    /*
     * Main game loop.
     */
    static void GameLoop_Main()
    {
        ushort key;

        CString.String_Init();
        Sprites.Sprites_Init();

        if (IniFile.IniFile_GetInteger("mt32midi", 0) != 0) Sound.Music_InitMT32();

        Input.Input_Flags_SetBits((ushort)(InputFlagsEnum.INPUT_FLAG_KEY_REPEAT | InputFlagsEnum.INPUT_FLAG_UNKNOWN_0010 | InputFlagsEnum.INPUT_FLAG_UNKNOWN_0200 |
                                           InputFlagsEnum.INPUT_FLAG_UNKNOWN_2000));
        Input.Input_Flags_ClearBits((ushort)(InputFlagsEnum.INPUT_FLAG_KEY_RELEASE | InputFlagsEnum.INPUT_FLAG_UNKNOWN_0400 | InputFlagsEnum.INPUT_FLAG_UNKNOWN_0100 |
                                             InputFlagsEnum.INPUT_FLAG_UNKNOWN_0080 | InputFlagsEnum.INPUT_FLAG_UNKNOWN_0040 | InputFlagsEnum.INPUT_FLAG_UNKNOWN_0020 |
                                             InputFlagsEnum.INPUT_FLAG_UNKNOWN_0008 | InputFlagsEnum.INPUT_FLAG_UNKNOWN_0004 | InputFlagsEnum.INPUT_FLAG_NO_TRANSLATE));

        Timer.Timer_SetTimer(TimerType.TIMER_GAME, true);
        Timer.Timer_SetTimer(TimerType.TIMER_GUI, true);

        g_campaignID = 0;
        g_scenarioID = 1;
        CHouse.g_playerHouseID = HouseType.HOUSE_INVALID;
        g_selectionType = (ushort)SelectionType.SELECTIONTYPE_MENTAT;
        g_selectionTypeNew = (ushort)SelectionType.SELECTIONTYPE_MENTAT;

        if (Gfx.g_palette1 != null) Trace.WriteLine("WARNING: g_palette1");
        else Gfx.g_palette1 = new byte[256 * 3]; //calloc(1, 256 * 3);
        if (Gfx.g_palette2 != null) Trace.WriteLine("WARNING: g_palette2");
        else Gfx.g_palette2 = new byte[256 * 3]; //calloc(1, 256 * 3);

        g_readBufferSize = 12000;
        g_readBuffer = new byte[g_readBufferSize]; //calloc(1, g_readBufferSize);

        ReadProfileIni("PROFILE.INI");

        g_readBuffer = null; //free(g_readBuffer);

        CFile.File_ReadBlockFile("IBM.PAL", Gfx.g_palette1, 256 * 3);

        Gui.GUI_ClearScreen(Screen.SCREEN_0);

        Sdl2Video.Video_SetPalette(Gfx.g_palette1, 0, 256);

        Gfx.GFX_SetPalette(Gfx.g_palette1);
        Gfx.GFX_SetPalette(Gfx.g_palette2);

        Gfx.g_paletteMapping1 = new byte[256]; //malloc(256);
        Gfx.g_paletteMapping2 = new byte[256]; //malloc(256);

        Gui.GUI_Palette_CreateMapping(Gfx.g_palette1, Gfx.g_paletteMapping1, 0xC, 0x55);
        Gfx.g_paletteMapping1[0xFF] = 0xFF;
        Gfx.g_paletteMapping1[0xDF] = 0xDF;
        Gfx.g_paletteMapping1[0xEF] = 0xEF;

        Gui.GUI_Palette_CreateMapping(Gfx.g_palette1, Gfx.g_paletteMapping2, 0xF, 0x55);
        Gfx.g_paletteMapping2[0xFF] = 0xFF;
        Gfx.g_paletteMapping2[0xDF] = 0xDF;
        Gfx.g_paletteMapping2[0xEF] = 0xEF;

        Script.Script_LoadFromFile("TEAM.EMC", Script.g_scriptTeam, Script.g_scriptFunctionsTeam, null);
        Script.Script_LoadFromFile("BUILD.EMC", Script.g_scriptStructure, Script.g_scriptFunctionsStructure, null);

        Gui.GUI_Palette_CreateRemap((byte)HouseType.HOUSE_MERCENARY);

        Gui.g_cursorSpriteID = 0;

        Sprites.Sprites_SetMouseSprite(0, 0, Sprites.g_sprites[0]);

        while (Mouse.g_mouseHiddenDepth > 1)
        {
            Gui.GUI_Mouse_Show_Safe();
        }

        Window_WidgetClick_Create();
        Config.GameOptions_Load();
        CUnit.Unit_Init();
        CTeam.Team_Init();
        CHouse.House_Init();
        CStructure.Structure_Init();

        Gui.GUI_Mouse_Show_Safe();

        Cutscene.g_canSkipIntro = CFile.File_Exists_Personal("ONETIME.DAT");

        for (; ; Sleep.sleepIdle())
        {
            if (g_gameMode == GameMode.GM_MENU)
            {
                GameLoop_GameIntroAnimationMenu();

                if (!g_running) break;
                if (g_gameMode == GameMode.GM_MENU) continue;

                Gui.GUI_Mouse_Hide_Safe();

                Gui.GUI_DrawFilledRectangle((short)(CWidget.g_curWidgetXBase << 3), (short)CWidget.g_curWidgetYBase, (short)((CWidget.g_curWidgetXBase + CWidget.g_curWidgetWidth) << 3), (short)(CWidget.g_curWidgetYBase + CWidget.g_curWidgetHeight), 12);

                Input.Input_History_Clear();

                if (s_enableLog != 0) Mouse.Mouse_SetMouseMode(s_enableLog, "DUNE.LOG");

                Gfx.GFX_SetPalette(Gfx.g_palette1);

                Gui.GUI_Mouse_Show_Safe();
            }

            if (g_gameMode == GameMode.GM_PICKHOUSE)
            {
                Sound.Music_Play(28);

                CHouse.g_playerHouseID = HouseType.HOUSE_MERCENARY;
                CHouse.g_playerHouseID = (HouseType)Gui.GUI_PickHouse();

                Gui.GUI_Mouse_Hide_Safe();

                Gfx.GFX_ClearBlock(Screen.SCREEN_0);

                Sprites.Sprites_LoadTiles();

                Gui.GUI_Palette_CreateRemap((byte)CHouse.g_playerHouseID);

                Sound.Voice_LoadVoices((ushort)CHouse.g_playerHouseID);

                Gui.GUI_Mouse_Show_Safe();

                g_gameMode = GameMode.GM_RESTART;
                g_scenarioID = 1;
                g_campaignID = 0;
                Gui.g_strategicRegionBits = 0;
            }

            if (g_selectionTypeNew != g_selectionType)
            {
                Gui.GUI_ChangeSelectionType(g_selectionTypeNew);
            }

            Gui.GUI_PaletteAnimate();

            if (g_gameMode == GameMode.GM_RESTART)
            {
                Gui.GUI_ChangeSelectionType((ushort)SelectionType.SELECTIONTYPE_MENTAT);

                Game_LoadScenario((byte)CHouse.g_playerHouseID, g_scenarioID);
                if (!g_debugScenario && !g_debugSkipDialogs)
                {
                    Mentat.GUI_Mentat_ShowBriefing();
                }
                else
                {
                    Debug.WriteLine("DEBUG: Skipping GUI_Mentat_ShowBriefing()");
                }

                g_gameMode = GameMode.GM_NORMAL;

                Gui.GUI_ChangeSelectionType((ushort)(g_debugScenario ? SelectionType.SELECTIONTYPE_DEBUG : SelectionType.SELECTIONTYPE_STRUCTURE));

                Sound.Music_Play((ushort)(Tools.Tools_RandomLCG_Range(0, 8) + 8));
                l_timerNext = Timer.g_timerGUI + 300;
            }

            if (l_selectionState != Gui.g_selectionState)
            {
                Map.Map_SetSelectionObjectPosition(0xFFFF);
                Map.Map_SetSelectionObjectPosition(Gui.g_selectionRectanglePosition);
                l_selectionState = Gui.g_selectionState;
            }

            if (!CDriver.Driver_Voice_IsPlaying() && !Sound.Sound_StartSpeech())
            {
                if (Config.g_gameConfig.music == 0)
                {
                    Sound.Music_Play(2);

                    g_musicInBattle = 0;
                }
                else if (g_musicInBattle > 0)
                {
                    Sound.Music_Play((ushort)(Tools.Tools_RandomLCG_Range(0, 5) + 17));
                    l_timerNext = Timer.g_timerGUI + 300;
                    g_musicInBattle = -1;
                }
                else
                {
                    g_musicInBattle = 0;
                    if (Config.g_enableSoundMusic && Timer.g_timerGUI > l_timerNext)
                    {
                        if (!CDriver.Driver_Music_IsPlaying())
                        {
                            Sound.Music_Play((ushort)(Tools.Tools_RandomLCG_Range(0, 8) + 8));
                            l_timerNext = Timer.g_timerGUI + 300;
                        }
                    }
                }
            }

            Gfx.GFX_Screen_SetActive(Screen.SCREEN_0);

            key = CWidget.GUI_Widget_HandleEvents(CWidget.g_widgetLinkedListHead);

            if (g_selectionType == (ushort)SelectionType.SELECTIONTYPE_TARGET || g_selectionType == (ushort)SelectionType.SELECTIONTYPE_PLACE || g_selectionType == (ushort)SelectionType.SELECTIONTYPE_UNIT || g_selectionType == (ushort)SelectionType.SELECTIONTYPE_STRUCTURE)
            {
                if (CUnit.g_unitSelected != null)
                {
                    if (l_timerUnitStatus < Timer.g_timerGame)
                    {
                        CUnit.Unit_DisplayStatusText(CUnit.g_unitSelected);
                        l_timerUnitStatus = Timer.g_timerGame + 300;
                    }

                    if (g_selectionType != (ushort)SelectionType.SELECTIONTYPE_TARGET)
                    {
                        Gui.g_selectionPosition = CTile.Tile_PackTile(CTile.Tile_Center(CUnit.g_unitSelected.o.position));
                    }
                }

                WidgetDraw.GUI_Widget_ActionPanel_Draw(false);

                InGame_Numpad_Move(key);

                Gui.GUI_DrawCredits((byte)CHouse.g_playerHouseID, 0);

                CTeam.GameLoop_Team();
                CUnit.GameLoop_Unit();
                CStructure.GameLoop_Structure();
                CHouse.GameLoop_House();

                Gui.GUI_DrawScreen(Screen.SCREEN_0);
            }

            Gui.GUI_DisplayText(null, 0);

            if (g_running && !g_debugScenario)
            {
                GameLoop_LevelEnd();
            }

            if (!g_running) break;
        }

        Gui.GUI_Mouse_Hide_Safe();

        if (s_enableLog != 0) Mouse.Mouse_SetMouseMode((byte)InputMouseMode.INPUT_MOUSE_MODE_NORMAL, "DUNE.LOG");

        Gui.GUI_Mouse_Hide_Safe();

        CWidget.Widget_SetCurrentWidget(0);

        Gfx.GFX_Screen_SetActive(Screen.SCREEN_1);

        Gfx.GFX_ClearScreen(Screen.SCREEN_1);

        Gui.GUI_Screen_FadeIn(CWidget.g_curWidgetXBase, CWidget.g_curWidgetYBase, CWidget.g_curWidgetXBase, CWidget.g_curWidgetYBase, CWidget.g_curWidgetWidth, CWidget.g_curWidgetHeight, Screen.SCREEN_1, Screen.SCREEN_0);
    }

    /*
     * Initialize Timer, Video, Mouse, GFX, Fonts, Random number generator
     * and current Widget
     */
    static bool OpenDune_Init(int screen_magnification, VideoScaleFilter filter, int frame_rate)
    {
        if (!CFont.Font_Init())
        {
            Trace.WriteLine("ERROR: --------------------------");
            Trace.WriteLine("ERROR LOADING DATA FILE");
            Trace.WriteLine("Did you copy the Dune2 1.07eu data files into the data directory ?");

            return false;
        }

        Timer.Timer_Init();

        if (!Sdl2Video.Video_Init(screen_magnification, filter)) return false;

        Mouse.Mouse_Init();

        /* Add the general tickers */
        Timer.Timer_Add(Timer.Timer_Tick, 1000000 / 60, false);
        Timer.Timer_Add(Sdl2Video.Video_Tick, (uint)(1000000 / frame_rate), true);

        unchecked { Mouse.g_mouseDisabled = (byte)-1; }

        Gfx.GFX_Init();
        Gfx.GFX_ClearScreen(Screen.SCREEN_ACTIVE);

        CFont.Font_Select(CFont.g_fontNew8p);

        Gui.g_palette_998A = new byte[256 * 3]; //calloc(256 * 3, sizeof(uint8));

        Array.Fill<byte>(Gui.g_palette_998A, 63, 45, 3); //memset(&g_palette_998A[45], 63, 3);	/* Set color 15 to WHITE */

        Gfx.GFX_SetPalette(Gui.g_palette_998A);

        Tools.Tools_RandomLCG_Seed((ushort)DateTime.UnixEpoch.Ticks); //(unsigned)time(NULL));

        CWidget.Widget_SetCurrentWidget(0);

        return true;
    }

    static int Main(string[] args) //int main(int argc, char **argv)
    {
        var commit_dune_cfg = false;
        var scale_filter = VideoScaleFilter.FILTER_NEAREST_NEIGHBOR;
        var scaling_factor = 2;
        var frame_rate = 60;
        string filter_text = null; //char[64]

        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

        //#if defined(_WIN32)
        //	#if defined(__MINGW32__) && defined(__STRICT_ANSI__)
        //	#if 0 /* NOTE : disabled because it generates warnings when cross compiling
        //	       * for MinGW32 under linux */
        //		int __cdecl __MINGW_NOTHROW _fileno (FILE*);
        //	#endif
        //	#endif
        //	FILE *err = fopen("error.log", "w");
        //	FILE *out = fopen("output.log", "w");

        //	#if defined(_MSC_VER)
        //		_CrtSetDbgFlag( _CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF );
        //	#endif

        //	if (err != NULL) _dup2(_fileno(err), _fileno(stderr));
        //	if (out != NULL) _dup2(_fileno(out), _fileno(stdout));
        //	FreeConsole();
        //#endif /* _WIN32 */

        CrashLog.CrashLog_Init();

        /* Load opendune.ini file */
        IniFile.Load_IniFile();

        /* set globals according to opendune.ini */
        g_dune2_enhanced = (IniFile.IniFile_GetInteger("dune2_enhanced", 1) != 0) ? true : false;
        g_debugGame = (IniFile.IniFile_GetInteger("debug_game", 0) != 0) ? true : false;
        g_debugScenario = (IniFile.IniFile_GetInteger("debug_scenario", 0) != 0) ? true : false;
        g_debugSkipDialogs = (IniFile.IniFile_GetInteger("debug_skip_dialogs", 0) != 0) ? true : false;
        s_enableLog = (byte)IniFile.IniFile_GetInteger("debug_log_game", 0);
        g_starPortEnforceUnitLimit = (IniFile.IniFile_GetInteger("startport_unit_cap", 0) != 0) ? true : false;

        Debug.WriteLine("DEBUG: Globals :");
        Debug.WriteLine($"DEBUG:  g_dune2_enhanced = {g_dune2_enhanced}");
        Debug.WriteLine($"DEBUG:  g_debugGame = {g_debugGame}");
        Debug.WriteLine($"DEBUG:  g_debugScenario = {g_debugScenario}");
        Debug.WriteLine($"DEBUG:  g_debugSkipDialogs = {g_debugSkipDialogs}");
        Debug.WriteLine($"DEBUG:  s_enableLog = {s_enableLog}");
        Debug.WriteLine($"DEBUG:  g_starPortEnforceUnitLimit = {g_starPortEnforceUnitLimit}");

        if (!CFile.File_Init())
        {
            return 1;
        }

        /* Loading config from dune.cfg */
        if (!Config.Config_Read("dune.cfg", Config.g_config))
        {
            Config.Config_Default(Config.g_config);
            commit_dune_cfg = true;
        }
        /* reading config from opendune.ini which prevail over dune.cfg */
        IniFile.SetLanguage_From_IniFile(Config.g_config);

        /* Writing config to dune.cfg */
        if (commit_dune_cfg && !Config.Config_Write("dune.cfg", Config.g_config))
        {
            Trace.WriteLine("ERROR: Error writing to dune.cfg file.");
            return 1;
        }

        Input.Input_Init();

        CDriver.Drivers_All_Init();

        scaling_factor = IniFile.IniFile_GetInteger("scalefactor", 2);
        if (IniFile.IniFile_GetString("scalefilter", null, filter_text, (ushort)(filter_text == null ? 0 : filter_text.Length)) != null)
        {
            if (string.Equals(filter_text, "nearest", StringComparison.OrdinalIgnoreCase))
            { //if (strcasecmp(filter_text, "nearest") == 0) {
                scale_filter = VideoScaleFilter.FILTER_NEAREST_NEIGHBOR;
            }
            else if (string.Equals(filter_text, "scale2x", StringComparison.OrdinalIgnoreCase))
            { //if (strcasecmp(filter_text, "scale2x") == 0) {
                scale_filter = VideoScaleFilter.FILTER_SCALE2X;
            }
            else if (string.Equals(filter_text, "hqx", StringComparison.OrdinalIgnoreCase))
            { //if (strcasecmp(filter_text, "hqx") == 0) {
                scale_filter = VideoScaleFilter.FILTER_HQX;
            }
            else
            {
                Trace.WriteLine($"ERROR: unrecognized scalefilter value '{filter_text}'");
            }
        }

        frame_rate = IniFile.IniFile_GetInteger("framerate", 60);

        if (!OpenDune_Init(scaling_factor, scale_filter, frame_rate)) Environment.Exit(1);

        Mouse.g_mouseDisabled = 0;

        GameLoop_Main();

        Trace.WriteLine(CString.String_Get_ByIndex(Text.STR_THANK_YOU_FOR_PLAYING_DUNE_II));

        PrepareEnd();
        IniFile.Free_IniFile();

        return 0;
    }

    /*
     * Prepare the map (after loading scenario or savegame). Does some basic
     * sanity-check and corrects stuff all over the place.
     */
    internal static void Game_Prepare()
    {
        var find = new PoolFindStruct();
        ushort oldSelectionType;
        Tile[] t;
        var tPointer = 0;
        int i;

        g_validateStrictIfZero++;

        oldSelectionType = g_selectionType;
        g_selectionType = (ushort)SelectionType.SELECTIONTYPE_MENTAT;

        CStructure.Structure_Recount();
        CUnit.Unit_Recount();
        CTeam.Team_Recount();

        t = Map.g_map; //[0];
        for (i = 0; i < 64 * 64; i++, tPointer++)
        {
            Structure s;
            Unit u;

            u = CUnit.Unit_Get_ByPackedTile((ushort)i);
            s = CStructure.Structure_Get_ByPackedTile((ushort)i);

            if (u == null || !u.o.flags.used) t[tPointer].hasUnit = false;
            if (s == null || !s.o.flags.used) t[tPointer].hasStructure = false;
            if (t[tPointer].isUnveiled) Map.Map_UnveilTile((ushort)i, (byte)CHouse.g_playerHouseID);
        }

        find.houseID = (byte)HouseType.HOUSE_INVALID;
        find.index = 0xFFFF;
        find.type = 0xFFFF;

        while (true)
        {
            Unit u;

            u = CUnit.Unit_Find(find);
            if (u == null) break;

            if (u.o.flags.isNotOnMap) continue;

            CUnit.Unit_RemoveFog(u);
            CUnit.Unit_UpdateMap(1, u);
        }

        find.houseID = (byte)HouseType.HOUSE_INVALID;
        find.index = 0xFFFF;
        find.type = 0xFFFF;

        while (true)
        {
            Structure s;

            s = CStructure.Structure_Find(find);
            if (s == null) break;
            if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;

            if (s.o.flags.isNotOnMap) continue;

            CStructure.Structure_RemoveFog(s);

            if (s.o.type == (byte)StructureType.STRUCTURE_STARPORT && s.o.linkedID != 0xFF)
            {
                var u = CUnit.Unit_Get_ByIndex(s.o.linkedID);

                if (!u.o.flags.used || !u.o.flags.isNotOnMap)
                {
                    s.o.linkedID = 0xFF;
                    s.countDown = 0;
                }
                else
                {
                    CStructure.Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_READY);
                }
            }

            Script.Script_Load(s.o.script, s.o.type);

            if (s.o.type == (byte)StructureType.STRUCTURE_PALACE)
            {
                CHouse.House_Get_ByIndex(s.o.houseID).palacePosition = s.o.position;
            }

            if ((CHouse.House_Get_ByIndex(s.o.houseID).palacePosition.x != 0) || (CHouse.House_Get_ByIndex(s.o.houseID).palacePosition.y != 0)) continue;
            CHouse.House_Get_ByIndex(s.o.houseID).palacePosition = s.o.position;
        }

        find.houseID = (byte)HouseType.HOUSE_INVALID;
        find.index = 0xFFFF;
        find.type = 0xFFFF;

        while (true)
        {
            House h;

            h = CHouse.House_Find(find);
            if (h == null) break;

            h.structuresBuilt = CStructure.Structure_GetStructuresBuilt(h);
            CHouse.House_UpdateCreditsStorage(h.index);
            CHouse.House_CalculatePowerAndCredit(h);
        }

        Gui.GUI_Palette_CreateRemap((byte)CHouse.g_playerHouseID);

        Map.Map_SetSelection(Gui.g_selectionPosition);

        if (CStructure.g_structureActiveType != 0xFFFF)
        {
            Map.Map_SetSelectionSize(CStructure.g_table_structureInfo[CStructure.g_structureActiveType].layout);
        }
        else
        {
            var s = CStructure.Structure_Get_ByPackedTile(Gui.g_selectionPosition);

            if (s != null) Map.Map_SetSelectionSize(CStructure.g_table_structureInfo[s.o.type].layout);
        }

        Sound.Voice_LoadVoices((ushort)CHouse.g_playerHouseID);

        CHouse.g_tickHousePowerMaintenance = Max(Timer.g_timerGame + 70, CHouse.g_tickHousePowerMaintenance);
        g_viewport_forceRedraw = true;
        CHouse.g_playerCredits = 0xFFFF;

        g_selectionType = oldSelectionType;
        g_validateStrictIfZero--;
    }

    /*
     * Initialize a game, by setting most variables to zero, cleaning the map, etc
     * etc.
     */
    internal static void Game_Init()
    {
        CUnit.Unit_Init();
        CStructure.Structure_Init();
        CTeam.Team_Init();
        CHouse.House_Init();

        CAnimation.Animation_Init();
        CExplosion.Explosion_Init();
        for (var i = 0; i < Map.g_map.Length; i++) Map.g_map[i] = new Tile(); //memset(g_map, 0, 64 * 64 * sizeof(Tile));

        Array.Fill<byte>(Map.g_displayedViewport, 0, 0, Map.g_displayedViewport.Length); //memset(g_displayedViewport, 0, sizeof(g_displayedViewport));
        Array.Fill<byte>(Map.g_displayedMinimap, 0, 0, Map.g_displayedMinimap.Length); //memset(g_displayedMinimap, 0, sizeof(g_displayedMinimap));
        Array.Fill<byte>(Map.g_changedTilesMap, 0, 0, Map.g_changedTilesMap.Length); //memset(g_changedTilesMap, 0, sizeof(g_changedTilesMap));
        Array.Fill<byte>(Map.g_dirtyViewport, 0, 0, Map.g_dirtyViewport.Length); //memset(g_dirtyViewport, 0, sizeof(g_dirtyViewport));
        Array.Fill<byte>(Map.g_dirtyMinimap, 0, 0, Map.g_dirtyMinimap.Length); //memset(g_dirtyMinimap, 0, sizeof(g_dirtyMinimap));

        Array.Fill<ushort>(Map.g_mapTileID, 0, 0, 64 * 64); //memset(g_mapTileID, 0, 64 * 64 * sizeof(uint16));
        Array.Fill<short>(CUnit.g_starportAvailable, 0, 0, CUnit.g_starportAvailable.Length); //memset(g_starportAvailable, 0, sizeof(g_starportAvailable));

        Sound.Sound_Output_Feedback(0xFFFE);

        CHouse.g_playerCreditsNoSilo = 0;
        CHouse.g_houseMissileCountdown = 0;
        Gui.g_selectionState = 0; /* Invalid. */
        CStructure.g_structureActivePosition = 0;

        CUnit.g_unitHouseMissile = null;
        CUnit.g_unitActive = null;
        CStructure.g_structureActive = null;

        g_activeAction = 0xFFFF;
        CStructure.g_structureActiveType = 0xFFFF;

        Gui.GUI_DisplayText(null, -1);

        Sleep.sleepIdle();  /* let the game a chance to update screen, etc. */
    }

    /*
     * Load a scenario in a safe way, and prepare the game.
     * @param houseID The House which is going to play the game.
     * @param scenarioID The Scenario to load.
     */
    static void Game_LoadScenario(byte houseID, ushort scenarioID)
    {
        Sound.Sound_Output_Feedback(0xFFFE);

        Game_Init();

        g_validateStrictIfZero++;

        if (!CScenario.Scenario_Load(scenarioID, houseID))
        {
            Gui.GUI_DisplayModalMessage("No more scenarios!", 0xFFFF);

            PrepareEnd();
            Environment.Exit(0);
        }

        Game_Prepare();

        if (scenarioID < 5)
        {
            g_hintsShown1 = 0;
            g_hintsShown2 = 0;
        }

        g_validateStrictIfZero--;
    }

    /*
     * Close down facilities used by the program. Always called just before the
     * program terminates.
     */
    internal static void PrepareEnd()
    {
        Gui.g_palette_998A = null; //free(g_palette_998A);

        GameLoop_Uninit();

        CString.String_Uninit();
        Sprites.Sprites_Uninit();
        CFont.Font_Uninit();
        Sound.Voice_UnloadVoices();

        CDriver.Drivers_All_Uninit();

        if (Mouse.g_mouseFileID != 0xFF) Mouse.Mouse_SetMouseMode((byte)InputMouseMode.INPUT_MOUSE_MODE_NORMAL, null);

        CFile.File_Uninit();
        Timer.Timer_Uninit();
        Gfx.GFX_Uninit();
        Sdl2Video.Video_Uninit();
    }
}
