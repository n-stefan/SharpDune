/* Gameloop and other main routines */

// Master TODO list:
// - reduce casts
// - speed up array operations: Array.Copy => Buffer.BlockCopy(faster?), . . .
// - clean up
// - use Array<T> where appropriate

//INT - BOOL: zero is false and all other values are true

namespace SharpDune;

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

static class SharpDune
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

    internal static CultureInfo Culture = CultureInfo.InvariantCulture;

    internal static StringComparison Comparison = StringComparison.Ordinal;

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
        if (g_timerGame - g_tickScenarioStart < 7200) return false;

        /* Check for structure counts hitting zero */
        if ((g_scenario.winFlags & 0x3) != 0)
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
                var s = Structure_Find(find);
                if (s == null) break;

                if (s.o.type is ((byte)StructureType.STRUCTURE_SLAB_1x1) or ((byte)StructureType.STRUCTURE_SLAB_2x2) or ((byte)StructureType.STRUCTURE_WALL)) continue;
                if (s.o.type == (byte)StructureType.STRUCTURE_TURRET) continue;
                if (s.o.type == (byte)StructureType.STRUCTURE_ROCKET_TURRET) continue;

                if (s.o.houseID == (byte)g_playerHouseID)
                {
                    countStructureFriendly++;
                }
                else
                {
                    countStructureEnemy++;
                }
            }

            if ((g_scenario.winFlags & 0x1) != 0 && countStructureEnemy == 0)
            {
                finish = true;
            }
            if ((g_scenario.winFlags & 0x2) != 0 && countStructureFriendly == 0)
            {
                finish = true;
            }
        }

        /* Check for reaching spice quota */
        if ((g_scenario.winFlags & 0x4) != 0 && g_playerCredits != 0xFFFF)
        {
            if (g_playerCredits >= g_playerHouse.creditsQuota)
            {
                finish = true;
            }
        }

        /* Check for reaching timeout */
        if ((g_scenario.winFlags & 0x8) != 0)
        {
            /* XXX -- This code was with '<' instead of '>=', which makes
             *  no sense. As it is unused, who knows what the intentions
             *  were. This at least makes it sensible. */
            if (g_timerGame >= s_tickGameTimeout)
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
        if ((g_scenario.loseFlags & 0x3) != 0)
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
                var s = Structure_Find(find);
                if (s == null) break;

                if (s.o.type is ((byte)StructureType.STRUCTURE_SLAB_1x1) or ((byte)StructureType.STRUCTURE_SLAB_2x2) or ((byte)StructureType.STRUCTURE_WALL)) continue;
                if (s.o.type == (byte)StructureType.STRUCTURE_TURRET) continue;
                if (s.o.type == (byte)StructureType.STRUCTURE_ROCKET_TURRET) continue;

                if (s.o.houseID == (byte)g_playerHouseID)
                {
                    countStructureFriendly++;
                }
                else
                {
                    countStructureEnemy++;
                }
            }

            win = true;
            if ((g_scenario.loseFlags & 0x1) != 0)
            {
                win = win && (countStructureEnemy == 0);
            }
            if ((g_scenario.loseFlags & 0x2) != 0)
            {
                win = win && (countStructureFriendly != 0);
            }
        }

        /* Check for reaching spice quota */
        if (!win && (g_scenario.loseFlags & 0x4) != 0 && g_playerCredits != 0xFFFF)
        {
            win = g_playerCredits >= g_playerHouse.creditsQuota;
        }

        /* Check for reaching timeout */
        if (!win && (g_scenario.loseFlags & 0x8) != 0)
        {
            win = g_timerGame < s_tickGameTimeout;
        }

        return win;
    }

    internal static void GameLoop_Uninit()
    {
        while (g_widgetLinkedListHead != null)
        {
            var w = g_widgetLinkedListHead;
            g_widgetLinkedListHead = w.next;

            //w = null;
        }

        Script_ClearInfo(g_scriptStructure);
        Script_ClearInfo(g_scriptTeam);

        g_readBuffer = null;

        g_palette1 = null;
        g_palette2 = null;
        g_paletteMapping1 = null;
        g_paletteMapping2 = null;
    }

    static uint levelEndTimer;
    /*
    * Checks if the level comes to an end. If so, it shows all end-level stuff,
    * and prepares for the next level.
    */
    static void GameLoop_LevelEnd()
    {
        if (levelEndTimer >= g_timerGame && !s_debugForceWin) return;

        if (GameLoop_IsLevelFinished())
        {
            Music_Play(0);

            g_cursorSpriteID = 0;

            Sprites_SetMouseSprite(0, 0, g_sprites[0]);

            Sound_Output_Feedback(0xFFFE);

            GUI_ChangeSelectionType((ushort)SelectionType.MENTAT);

            if (GameLoop_IsLevelWon())
            {
                Sound_Output_Feedback(40);

                GUI_DisplayModalMessage(String_Get_ByIndex(Text.STR_YOU_HAVE_SUCCESSFULLY_COMPLETED_YOUR_MISSION), 0xFFFF);

                GUI_Mentat_ShowWin();

                Sprites_UnloadTiles();

                g_campaignID++;

                GUI_EndStats_Show(g_scenario.killedAllied, g_scenario.killedEnemy, g_scenario.destroyedAllied, g_scenario.destroyedEnemy,
                    g_scenario.harvestedAllied, g_scenario.harvestedEnemy, (short)g_scenario.score, (byte)g_playerHouseID);

                if (g_campaignID == 9)
                {
                    GUI_Mouse_Hide_Safe();

                    GUI_SetPaletteAnimated(g_palette2, 15);
                    GUI_ClearScreen(Screen.NO0);
                    GameLoop_GameEndAnimation();
                    PrepareEnd();
                    Environment.Exit(0);
                }

                GUI_Mouse_Hide_Safe();
                GameLoop_LevelEndAnimation();
                GUI_Mouse_Show_Safe();

                File_ReadBlockFile("IBM.PAL", g_palette1, 256 * 3);

                g_scenarioID = GUI_StrategicMap_Show(g_campaignID, true);

                GUI_SetPaletteAnimated(g_palette2, 15);

                if (g_campaignID is 1 or 7)
                {
                    if (!GUI_Security_Show())
                    {
                        PrepareEnd();
                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                Sound_Output_Feedback(41);

                GUI_DisplayModalMessage(String_Get_ByIndex(Text.STR_YOU_HAVE_FAILED_YOUR_MISSION), 0xFFFF);

                GUI_Mentat_ShowLose();

                Sprites_UnloadTiles();

                g_scenarioID = GUI_StrategicMap_Show(g_campaignID, false);
            }

            g_playerHouse.flags.doneFullScaleAttack = false;

            Sprites_LoadTiles();

            g_gameMode = GameMode.GM_RESTART;
            s_debugForceWin = false;
        }

        levelEndTimer = g_timerGame + 300;
    }

    static void GameLoop_DrawMenu(string[] strings)
    {
        WidgetProperties props;
        ushort left;
        ushort top;
        byte i;

        props = g_widgetProperties[21];
        top = (ushort)(g_curWidgetYBase + props.yBase);
        left = (ushort)((g_curWidgetXBase + props.xBase) << 3);

        GUI_Mouse_Hide_Safe();

        for (i = 0; i < props.height; i++)
        {
            var pos = (ushort)(top + g_fontCurrent.height * i);

            if (i == props.fgColourBlink)
            {
                GUI_DrawText_Wrapper(strings[i], (short)left, (short)pos, props.fgColourSelected, 0, 0x22);
            }
            else
            {
                GUI_DrawText_Wrapper(strings[i], (short)left, (short)pos, props.fgColourNormal, 0, 0x22);
            }
        }

        GUI_Mouse_Show_Safe();

        Input_History_Clear();
    }

    static void GameLoop_DrawText2(string str, ushort left, ushort top, byte fgColourNormal, byte fgColourSelected, byte bgColour)
    {
        byte i;

        for (i = 0; i < 3; i++)
        {
            GUI_Mouse_Hide_Safe();

            GUI_DrawText_Wrapper(str, (short)left, (short)top, fgColourSelected, bgColour, 0x22);
            Timer_Sleep(2);

            GUI_DrawText_Wrapper(str, (short)left, (short)top, fgColourNormal, bgColour, 0x22);
            GUI_Mouse_Show_Safe();
            Timer_Sleep(2);
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

        props = g_widgetProperties[21];

        last = (byte)(props.height - 1);
        old = (byte)(props.fgColourBlink % (last + 1));
        current = old;

        result = 0xFFFF;

        top = (ushort)(g_curWidgetYBase + props.yBase);
        left = (ushort)((g_curWidgetXBase + props.xBase) << 3);

        lineHeight = g_fontCurrent.height;

        minX = (ushort)((g_curWidgetXBase << 3) + (g_fontCurrent.maxWidth * props.xBase));
        minY = (ushort)(g_curWidgetYBase + props.yBase);
        maxX = (ushort)(minX + (g_fontCurrent.maxWidth * props.width) - 1);
        maxY = (ushort)(minY + (props.height * lineHeight) - 1);

        fgColourNormal = props.fgColourNormal;
        fgColourSelected = props.fgColourSelected;

        key = 0;
        if (Input_IsInputAvailable() != 0)
        {
            key = (ushort)(Input_Wait() & 0x8FF);
        }

        if (g_mouseDisabled == 0)
        {
            var y = g_mouseY;

            if (GameLoop_IsInRange(g_mouseX, y, minX, minY, maxX, maxY))
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
                if (GameLoop_IsInRange(g_mouseClickX, g_mouseClickY, minX, minY, maxX, maxY))
                {
                    current = (byte)((g_mouseClickY - minY) / lineHeight);
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
                        char c2;

                        c1 = char.ToUpper(strings[i][0], Culture);
                        c2 = char.ToUpper((char)Input_Keyboard_HandleKeys((ushort)(key & 0xFF)), Culture);

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
            GUI_Mouse_Hide_Safe();
            GUI_DrawText_Wrapper(strings[old], (short)left, (short)(top + (old * lineHeight)), fgColourNormal, 0, 0x22);
            GUI_DrawText_Wrapper(strings[current], (short)left, (short)(top + (current * lineHeight)), fgColourSelected, 0, 0x22);
            GUI_Mouse_Show_Safe();
        }

        props.fgColourBlink = current;

        if (result == 0xFFFF) return 0xFFFF;

        GUI_Mouse_Hide_Safe();
        GameLoop_DrawText2(strings[result], left, (ushort)(top + (current * lineHeight)), fgColourNormal, fgColourSelected, 0);
        GUI_Mouse_Show_Safe();

        return result;
    }

    static void Window_WidgetClick_Create()
    {
        WidgetInfo wi;
        int pointer;

        for (pointer = 0; g_table_gameWidgetInfo[pointer].index >= 0; pointer++)
        {
            wi = g_table_gameWidgetInfo[pointer];
            var w = GUI_Widget_Allocate((ushort)wi.index, (ushort)wi.shortcut, wi.offsetX, wi.offsetY, (ushort)wi.spriteID, wi.stringID);

            if (wi.spriteID < 0)
            {
                w.width = wi.width;
                w.height = wi.height;
            }

            w.clickProc = wi.clickProc;
            w.flags.Set(wi.flags);

            g_widgetLinkedListHead = GUI_Widget_Insert(g_widgetLinkedListHead, w);
        }
    }

    static void ReadProfileIni(string filename)
    {
        /* char * */
        Memory<byte> source;
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
        if (!File_Exists(filename)) return;

        source = GFX_Screen_Get_ByIndex(Screen.NO1);

        source.Span.Slice(0, 32000).Clear(); //memset(source, 0, 32000);

        File_ReadBlockFile(filename, source, GFX_Screen_GetSize_ByIndex(Screen.NO1));

        keys = Encoding.GetString(source.Span.Slice(source.Length + 5000));
        //*keys = '\0';

        sourceString = Encoding.GetString(source.Span);
        keys = Ini_GetString("construct", null, keys, sourceString);

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

            type = Unit_StringToType(key);
            if (type != (byte)UnitType.UNIT_INVALID)
            {
                oi = g_table_unitInfo[type].o;
            }
            else
            {
                type = Structure_StringToType(key);
                if (type != (byte)StructureType.STRUCTURE_INVALID) oi = g_table_structureInfo[type].o;
            }

            if (oi == null) continue;

            buffer = Ini_GetString("construct", key, buffer, sourceString);
            bufferStrings = buffer.Split(",");
            buildCredits = ushort.Parse(bufferStrings[0], Culture);
            buildTime = ushort.Parse(bufferStrings[1], Culture);
            hitpoints = ushort.Parse(bufferStrings[2], Culture);
            fogUncoverRadius = ushort.Parse(bufferStrings[3], Culture);
            availableCampaign = ushort.Parse(bufferStrings[4], Culture);
            priorityBuild = ushort.Parse(bufferStrings[5], Culture);
            priorityTarget = ushort.Parse(bufferStrings[6], Culture);
            if (bufferStrings.Length > 7)
                sortPriority = ushort.Parse(bufferStrings[7], Culture);
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
                var oi = g_table_unitInfo[locsi].o;

                buffer = $"{oi.name.PadRight(15 - oi.name.Length, ' ')}{oi.buildCredits},{oi.buildTime},{oi.hitpoints},{oi.fogUncoverRadius},{oi.availableCampaign},{oi.priorityBuild},{oi.priorityTarget},{oi.sortPriority}";
                //sprintf(buffer, "%*s%4d,%4d,%4d,%4d,%4d,%4d,%4d,%4d",
                //	15 - (int)strlen(oi->name), string.Empty, oi->buildCredits, oi->buildTime, oi->hitpoints, oi->fogUncoverRadius,
                //	oi->availableCampaign, oi->priorityBuild, oi->priorityTarget, oi->sortPriority);

                Ini_SetString("construct", oi.name, buffer, Encoding.GetString(source.Span));
            }

            for (locsi = 0; locsi < (ushort)StructureType.STRUCTURE_MAX; locsi++)
            {
                var oi = g_table_structureInfo[locsi].o;

                buffer = $"{oi.name.PadRight(15 - oi.name.Length, ' ')}{oi.buildCredits},{oi.buildTime},{oi.hitpoints},{oi.fogUncoverRadius},{oi.availableCampaign},{oi.priorityBuild},{oi.priorityTarget},{oi.sortPriority}";
                //sprintf(buffer, "%*s%4d,%4d,%4d,%4d,%4d,%4d,%4d,%4d",
                //	15 - (int)strlen(oi->name), string.Empty, oi->buildCredits, oi->buildTime, oi->hitpoints, oi->fogUncoverRadius,
                //	oi->availableCampaign, oi->priorityBuild, oi->priorityTarget, oi->sortPriority);

                Ini_SetString("construct", oi.name, buffer, Encoding.GetString(source.Span));
            }
        }

        //*keys = '\0';

        keys = Ini_GetString("combat", null, keys, sourceString);

        for (key = keys; key[keyPointer] != '\r'; keyPointer++)
        { //key += strlen(key) + 1) {
            ushort damage;
            ushort movingSpeedFactor;
            ushort fireDelay;
            ushort fireDistance;

            buffer = Ini_GetString("combat", key, buffer, sourceString);

            bufferStrings = buffer.Trim().Split(","); //String_Trim(buffer);
            if (bufferStrings.Length < 4) continue;
            fireDistance = ushort.Parse(bufferStrings[0], Culture);
            damage = ushort.Parse(bufferStrings[1], Culture);
            fireDelay = ushort.Parse(bufferStrings[2], Culture);
            movingSpeedFactor = ushort.Parse(bufferStrings[3], Culture);
            //if (sscanf(buffer, "%hu,%hu,%hu,%hu", &fireDistance, &damage, &fireDelay, &movingSpeedFactor) < 4) continue;

            for (locsi = 0; locsi < (ushort)UnitType.UNIT_MAX; locsi++)
            {
                var ui = g_table_unitInfo[locsi];

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
            var ui = g_table_unitInfo[locsi];

            buffer = $"{ui.o.name.PadRight(15 - ui.o.name.Length, ' ')}{ui.fireDistance},{ui.damage},{ui.fireDelay},{ui.movingSpeedFactor}";
            //sprintf(buffer, "%*s%4d,%4d,%4d,%4d", 15 - (int)strlen(ui->o.name), string.Empty, ui->fireDistance, ui->damage, ui->fireDelay, ui->movingSpeedFactor);
            Ini_SetString("combat", ui.o.name, buffer, Encoding.GetString(source.Span));
        }
    }

    static bool drawMenu = true;
    static ushort stringID = (ushort)Text.STR_REPLAY_INTRODUCTION;
    static bool hasSave;
    static bool hasFame;
    static readonly string[] strings = new string[6];
    static ushort index = 0xFFFF;
    static readonly ushort[][] mainMenuStrings = [ //[][6]
			[(ushort)Text.STR_PLAY_A_GAME, (ushort)Text.STR_REPLAY_INTRODUCTION, (ushort)Text.STR_EXIT_GAME, (ushort)Text.STR_NULL,         (ushort)Text.STR_NULL,         (ushort)Text.STR_NULL], /* Neither HOF nor save. */
			[(ushort)Text.STR_PLAY_A_GAME, (ushort)Text.STR_REPLAY_INTRODUCTION, (ushort)Text.STR_LOAD_GAME, (ushort)Text.STR_EXIT_GAME,    (ushort)Text.STR_NULL,         (ushort)Text.STR_NULL], /* Has a save game. */
			[(ushort)Text.STR_PLAY_A_GAME, (ushort)Text.STR_REPLAY_INTRODUCTION, (ushort)Text.STR_EXIT_GAME, (ushort)Text.STR_HALL_OF_FAME, (ushort)Text.STR_NULL,         (ushort)Text.STR_NULL], /* Has a HOF. */
			[(ushort)Text.STR_PLAY_A_GAME, (ushort)Text.STR_REPLAY_INTRODUCTION, (ushort)Text.STR_LOAD_GAME, (ushort)Text.STR_EXIT_GAME,    (ushort)Text.STR_HALL_OF_FAME, (ushort)Text.STR_NULL]  /* Has a HOF and a save game. */
		];
    /*
     * Intro menu.
     */
    static void GameLoop_GameIntroAnimationMenu()
    {
        var loadGame = false;
        ushort maxWidth;

        if (index == 0xFFFF)
        {
            hasSave = File_Exists_Personal("_save000.dat");
            hasFame = File_Exists_Personal("SAVEFAME.DAT");
            index = (ushort)((hasFame ? 2 : 0) + (hasSave ? 1 : 0));
        }

        if (!g_canSkipIntro)
        {
            if (hasSave) g_canSkipIntro = true;
        }

        switch ((Text)stringID)
        {
            case Text.STR_REPLAY_INTRODUCTION:
                Music_Play(0);

                g_readBuffer = null; //free(g_readBuffer);
                g_readBufferSize = (uint)(!g_enableVoices ? 12000 : 28000);
                g_readBuffer = new byte[g_readBufferSize]; //calloc(1, g_readBufferSize);

                GUI_Mouse_Hide_Safe();

                Driver_Music_FadeOut();

                GameLoop_GameIntroAnimation();

                Sound_Output_Feedback(0xFFFE);

                File_ReadBlockFile("IBM.PAL", g_palette1, 256 * 3);

                if (!g_canSkipIntro)
                {
                    File_Create_Personal("ONETIME.DAT");
                    g_canSkipIntro = true;
                }

                Music_Play(0);

                g_readBuffer = null; //free(g_readBuffer);
                g_readBufferSize = (uint)(!g_enableVoices ? 12000 : 20000);
                g_readBuffer = new byte[g_readBufferSize]; //calloc(1, g_readBufferSize);

                GUI_Mouse_Show_Safe();

                Music_Play(28);

                drawMenu = true;
                break;

            case Text.STR_EXIT_GAME:
                g_running = false;
                return;

            case Text.STR_HALL_OF_FAME:
                GUI_HallOfFame_Show(0xFFFF);

                GFX_SetPalette(g_palette2);

                hasFame = File_Exists_Personal("SAVEFAME.DAT");
                drawMenu = true;
                break;

            case Text.STR_LOAD_GAME:
                GUI_Mouse_Hide_Safe();
                GUI_SetPaletteAnimated(g_palette2, 30);
                GUI_ClearScreen(Screen.NO0);
                GUI_Mouse_Show_Safe();

                GFX_SetPalette(g_palette1);

                if (GUI_Widget_SaveLoad_Click(false))
                {
                    loadGame = true;
                    if (g_gameMode == GameMode.GM_RESTART) break;
                    g_gameMode = GameMode.GM_NORMAL;
                }
                else
                {
                    GFX_SetPalette(g_palette2);

                    drawMenu = true;
                }
                break;

            default: break;
        }

        if (drawMenu)
        {
            ushort i;

            g_widgetProperties[21].height = 0;

            for (i = 0; i < 6; i++)
            {
                strings[i] = null;

                if (mainMenuStrings[index][i] == 0)
                {
                    if (g_widgetProperties[21].height == 0) g_widgetProperties[21].height = i;
                    continue;
                }

                strings[i] = String_Get_ByIndex(mainMenuStrings[index][i]);
            }

            GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x22);

            maxWidth = 0;

            for (i = 0; i < g_widgetProperties[21].height; i++)
            {
                if (Font_GetStringWidth(strings[i]) <= maxWidth) continue;
                maxWidth = Font_GetStringWidth(strings[i]);
            }

            maxWidth += 7;

            g_widgetProperties[21].width = (ushort)(maxWidth >> 3);
            g_widgetProperties[13].width = (ushort)(g_widgetProperties[21].width + 2);
            g_widgetProperties[13].xBase = (ushort)(19 - (maxWidth >> 4));
            g_widgetProperties[13].yBase = (ushort)(160 - ((g_widgetProperties[21].height * g_fontCurrent.height) >> 1));
            g_widgetProperties[13].height = (ushort)((g_widgetProperties[21].height * g_fontCurrent.height) + 11);

            Sprites_LoadImage(String_GenerateFilename("TITLE"), Screen.NO1, null);

            GUI_Mouse_Hide_Safe();

            GUI_ClearScreen(Screen.NO0);

            GUI_Screen_Copy(0, 0, 0, 0, SCREEN_WIDTH / 8, (short)SCREEN_HEIGHT, Screen.NO1, Screen.NO0);

            GUI_SetPaletteAnimated(g_palette1, 30);

            GUI_DrawText_Wrapper("V1.07", 319, 192, 133, 0, 0x231, 0x39);
            GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x22);

            Widget_SetCurrentWidget(13);

            GUI_Widget_DrawBorder(13, 2, true/*1*/);

            GameLoop_DrawMenu(strings);

            GUI_Mouse_Show_Safe();

            drawMenu = false;
        }

        if (loadGame) return;

        stringID = GameLoop_HandleEvents(strings);

        if (stringID != 0xFFFF) stringID = mainMenuStrings[index][stringID];

        GUI_PaletteAnimate();

        if (stringID == (ushort)Text.STR_PLAY_A_GAME) g_gameMode = GameMode.GM_PICKHOUSE;
    }

    static void InGame_Numpad_Move(ushort key)
    {
        if (key == 0) return;

        switch (key)
        {
            case 0x0010: /* TAB */
                Map_SelectNext(true);
                return;

            case 0x0110: /* SHIFT TAB */
                Map_SelectNext(false);
                return;

            case 0x005C: /* NUMPAD 4 / ARROW LEFT */
            case 0x045C:
            case 0x055C:
                Map_MoveDirection(6);
                return;

            case 0x0066: /* NUMPAD 6 / ARROW RIGHT */
            case 0x0466:
            case 0x0566:
                Map_MoveDirection(2);
                return;

            case 0x0060: /* NUMPAD 8 / ARROW UP */
            case 0x0460:
            case 0x0560:
                Map_MoveDirection(0);
                return;

            case 0x0062: /* NUMPAD 2 / ARROW DOWN */
            case 0x0462:
            case 0x0562:
                Map_MoveDirection(4);
                return;

            case 0x005B: /* NUMPAD 7 / HOME */
            case 0x045B:
            case 0x055B:
                Map_MoveDirection(7);
                return;

            case 0x005D: /* NUMPAD 1 / END */
            case 0x045D:
            case 0x055D:
                Map_MoveDirection(5);
                return;

            case 0x0065: /* NUMPAD 9 / PAGE UP */
            case 0x0465:
            case 0x0565:
                Map_MoveDirection(1);
                return;

            case 0x0067: /* NUMPAD 3 / PAGE DOWN */
            case 0x0467:
            case 0x0567:
                Map_MoveDirection(3);
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

        String_Init();
        Sprites_Init();

        if (IniFile_GetInteger("mt32midi", 0) != 0) Music_InitMT32();

        Input_Flags_SetBits((ushort)(InputFlagsEnum.INPUT_FLAG_KEY_REPEAT | InputFlagsEnum.INPUT_FLAG_UNKNOWN_0010 | InputFlagsEnum.INPUT_FLAG_UNKNOWN_0200 |
                                           InputFlagsEnum.INPUT_FLAG_UNKNOWN_2000));
        Input_Flags_ClearBits((ushort)(InputFlagsEnum.INPUT_FLAG_KEY_RELEASE | InputFlagsEnum.INPUT_FLAG_UNKNOWN_0400 | InputFlagsEnum.INPUT_FLAG_UNKNOWN_0100 |
                                             InputFlagsEnum.INPUT_FLAG_UNKNOWN_0080 | InputFlagsEnum.INPUT_FLAG_UNKNOWN_0040 | InputFlagsEnum.INPUT_FLAG_UNKNOWN_0020 |
                                             InputFlagsEnum.INPUT_FLAG_UNKNOWN_0008 | InputFlagsEnum.INPUT_FLAG_UNKNOWN_0004 | InputFlagsEnum.INPUT_FLAG_NO_TRANSLATE));

        Timer_SetTimer(TimerType.TIMER_GAME, true);
        Timer_SetTimer(TimerType.TIMER_GUI, true);

        g_campaignID = 0;
        g_scenarioID = 1;
        g_playerHouseID = HouseType.HOUSE_INVALID;
        g_selectionType = (ushort)SelectionType.MENTAT;
        g_selectionTypeNew = (ushort)SelectionType.MENTAT;

        if (g_palette1 != null) Trace.WriteLine("WARNING: g_palette1");
        else g_palette1 = new byte[256 * 3]; //calloc(1, 256 * 3);
        if (g_palette2 != null) Trace.WriteLine("WARNING: g_palette2");
        else g_palette2 = new byte[256 * 3]; //calloc(1, 256 * 3);

        g_readBufferSize = 12000;
        g_readBuffer = new byte[g_readBufferSize]; //calloc(1, g_readBufferSize);

        ReadProfileIni("PROFILE.INI");

        g_readBuffer = null; //free(g_readBuffer);

        File_ReadBlockFile("IBM.PAL", g_palette1, 256 * 3);

        GUI_ClearScreen(Screen.NO0);

        Video_SetPalette(g_palette1, 0, 256);

        GFX_SetPalette(g_palette1);
        GFX_SetPalette(g_palette2);

        g_paletteMapping1 = new byte[256]; //malloc(256);
        g_paletteMapping2 = new byte[256]; //malloc(256);

        GUI_Palette_CreateMapping(g_palette1, g_paletteMapping1, 0xC, 0x55);
        g_paletteMapping1[0xFF] = 0xFF;
        g_paletteMapping1[0xDF] = 0xDF;
        g_paletteMapping1[0xEF] = 0xEF;

        GUI_Palette_CreateMapping(g_palette1, g_paletteMapping2, 0xF, 0x55);
        g_paletteMapping2[0xFF] = 0xFF;
        g_paletteMapping2[0xDF] = 0xDF;
        g_paletteMapping2[0xEF] = 0xEF;

        Script_LoadFromFile("TEAM.EMC", g_scriptTeam, g_scriptFunctionsTeam, null);
        Script_LoadFromFile("BUILD.EMC", g_scriptStructure, g_scriptFunctionsStructure, null);

        GUI_Palette_CreateRemap((byte)HouseType.HOUSE_MERCENARY);

        g_cursorSpriteID = 0;

        Sprites_SetMouseSprite(0, 0, g_sprites[0]);

        while (g_mouseHiddenDepth > 1)
        {
            GUI_Mouse_Show_Safe();
        }

        Window_WidgetClick_Create();
        GameOptions_Load();
        Unit_Init();
        Team_Init();
        House_Init();
        Structure_Init();

        GUI_Mouse_Show_Safe();

        g_canSkipIntro = File_Exists_Personal("ONETIME.DAT");

        for (; ; SleepIdle())
        {
            if (g_gameMode == GameMode.GM_MENU)
            {
                GameLoop_GameIntroAnimationMenu();

                if (!g_running) break;
                if (g_gameMode == GameMode.GM_MENU) continue;

                GUI_Mouse_Hide_Safe();

                GUI_DrawFilledRectangle((short)(g_curWidgetXBase << 3), (short)g_curWidgetYBase, (short)((g_curWidgetXBase + g_curWidgetWidth) << 3), (short)(g_curWidgetYBase + g_curWidgetHeight), 12);

                Input_History_Clear();

                if (s_enableLog != 0) Mouse_SetMouseMode(s_enableLog, "DUNE.LOG");

                GFX_SetPalette(g_palette1);

                GUI_Mouse_Show_Safe();
            }

            if (g_gameMode == GameMode.GM_PICKHOUSE)
            {
                Music_Play(28);

                g_playerHouseID = HouseType.HOUSE_MERCENARY;
                g_playerHouseID = (HouseType)GUI_PickHouse();

                GUI_Mouse_Hide_Safe();

                GFX_ClearBlock(Screen.NO0);

                Sprites_LoadTiles();

                GUI_Palette_CreateRemap((byte)g_playerHouseID);

                Voice_LoadVoices((ushort)g_playerHouseID);

                GUI_Mouse_Show_Safe();

                g_gameMode = GameMode.GM_RESTART;
                g_scenarioID = 1;
                g_campaignID = 0;
                g_strategicRegionBits = 0;
            }

            if (g_selectionTypeNew != g_selectionType)
            {
                GUI_ChangeSelectionType(g_selectionTypeNew);
            }

            GUI_PaletteAnimate();

            if (g_gameMode == GameMode.GM_RESTART)
            {
                GUI_ChangeSelectionType((ushort)SelectionType.MENTAT);

                Game_LoadScenario((byte)g_playerHouseID, g_scenarioID);
                if (!g_debugScenario && !g_debugSkipDialogs)
                {
                    GUI_Mentat_ShowBriefing();
                }
                else
                {
                    Debug.WriteLine("DEBUG: Skipping GUI_Mentat_ShowBriefing()");
                }

                g_gameMode = GameMode.GM_NORMAL;

                GUI_ChangeSelectionType((ushort)(g_debugScenario ? SelectionType.DEBUG : SelectionType.STRUCTURE));

                Music_Play((ushort)(Tools_RandomLCG_Range(0, 8) + 8));
                l_timerNext = g_timerGUI + 300;
            }

            if (l_selectionState != g_selectionState)
            {
                Map_SetSelectionObjectPosition(0xFFFF);
                Map_SetSelectionObjectPosition(g_selectionRectanglePosition);
                l_selectionState = g_selectionState;
            }

            if (!Driver_Voice_IsPlaying() && !Sound_StartSpeech())
            {
                if (g_gameConfig.music == 0)
                {
                    Music_Play(2);

                    g_musicInBattle = 0;
                }
                else if (g_musicInBattle > 0)
                {
                    Music_Play((ushort)(Tools_RandomLCG_Range(0, 5) + 17));
                    l_timerNext = g_timerGUI + 300;
                    g_musicInBattle = -1;
                }
                else
                {
                    g_musicInBattle = 0;
                    if (g_enableSoundMusic && g_timerGUI > l_timerNext)
                    {
                        if (!Driver_Music_IsPlaying())
                        {
                            Music_Play((ushort)(Tools_RandomLCG_Range(0, 8) + 8));
                            l_timerNext = g_timerGUI + 300;
                        }
                    }
                }
            }

            GFX_Screen_SetActive(Screen.NO0);

            key = GUI_Widget_HandleEvents(g_widgetLinkedListHead);

            if (g_selectionType is ((ushort)SelectionType.TARGET) or ((ushort)SelectionType.PLACE) or ((ushort)SelectionType.UNIT) or ((ushort)SelectionType.STRUCTURE))
            {
                if (g_unitSelected != null)
                {
                    if (l_timerUnitStatus < g_timerGame)
                    {
                        Unit_DisplayStatusText(g_unitSelected);
                        l_timerUnitStatus = g_timerGame + 300;
                    }

                    if (g_selectionType != (ushort)SelectionType.TARGET)
                    {
                        g_selectionPosition = Tile_PackTile(Tile_Center(g_unitSelected.o.position));
                    }
                }

                GUI_Widget_ActionPanel_Draw(false);

                InGame_Numpad_Move(key);

                GUI_DrawCredits((byte)g_playerHouseID, 0);

                GameLoop_Team();
                GameLoop_Unit();
                GameLoop_Structure();
                GameLoop_House();

                GUI_DrawScreen(Screen.NO0);
            }

            GUI_DisplayText(null, 0);

            if (g_running && !g_debugScenario)
            {
                GameLoop_LevelEnd();
            }

            if (!g_running) break;
        }

        GUI_Mouse_Hide_Safe();

        if (s_enableLog != 0) Mouse_SetMouseMode((byte)InputMouseMode.INPUT_MOUSE_MODE_NORMAL, "DUNE.LOG");

        GUI_Mouse_Hide_Safe();

        Widget_SetCurrentWidget(0);

        GFX_Screen_SetActive(Screen.NO1);

        GFX_ClearScreen(Screen.NO1);

        GUI_Screen_FadeIn(g_curWidgetXBase, g_curWidgetYBase, g_curWidgetXBase, g_curWidgetYBase, g_curWidgetWidth, g_curWidgetHeight, Screen.NO1, Screen.NO0);
    }

    /*
     * Initialize Timer, Video, Mouse, GFX, Fonts, Random number generator
     * and current Widget
     */
    static bool SharpDune_Init(int screen_magnification, VideoScaleFilter filter, int frame_rate)
    {
        if (!Font_Init())
        {
            Trace.WriteLine("ERROR: --------------------------");
            Trace.WriteLine("ERROR LOADING DATA FILE");
            Trace.WriteLine("Did you copy the Dune2 1.07eu data files into the data directory ?");

            return false;
        }

        Timer_Init();

        if (!Video_Init(screen_magnification, filter)) return false;

        Mouse_Init();

        /* Add the general tickers */
        Timer_Add(Timer_Tick, 1000000 / 60, false);
        Timer_Add(Video_Tick, (uint)(1000000 / frame_rate), true);

        unchecked { g_mouseDisabled = (byte)-1; }

        GFX_Init();
        GFX_ClearScreen(Screen.ACTIVE);

        Font_Select(g_fontNew8p);

        g_palette_998A = new byte[256 * 3]; //calloc(256 * 3, sizeof(uint8));

        Array.Fill<byte>(g_palette_998A, 63, 45, 3); //memset(&g_palette_998A[45], 63, 3);	/* Set color 15 to WHITE */

        GFX_SetPalette(g_palette_998A);

        Tools_RandomLCG_Seed((ushort)DateTime.UnixEpoch.Ticks); //(unsigned)time(NULL));

        Widget_SetCurrentWidget(0);

        return true;
    }

    static int Main() //int main(int argc, char **argv)
    {
        var commit_dune_cfg = false;
        var scale_filter = VideoScaleFilter.FILTER_NEAREST_NEIGHBOR;
        int scaling_factor; // = 2;
        int frame_rate; // = 60;
        string filter_text; //char[64]

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

        CrashLog_Init();

        /* Load sharpdune.ini file */
        Load_IniFile();

        /* set globals according to sharpdune.ini */
        g_dune2_enhanced = IniFile_GetInteger("dune2_enhanced", 1) != 0;
        g_debugGame = IniFile_GetInteger("debug_game", 0) != 0;
        g_debugScenario = IniFile_GetInteger("debug_scenario", 0) != 0;
        g_debugSkipDialogs = IniFile_GetInteger("debug_skip_dialogs", 0) != 0;
        s_enableLog = (byte)IniFile_GetInteger("debug_log_game", 0);
        g_starPortEnforceUnitLimit = IniFile_GetInteger("startport_unit_cap", 0) != 0;

        Debug.WriteLine("DEBUG: Globals :");
        Debug.WriteLine($"DEBUG:  g_dune2_enhanced = {g_dune2_enhanced}");
        Debug.WriteLine($"DEBUG:  g_debugGame = {g_debugGame}");
        Debug.WriteLine($"DEBUG:  g_debugScenario = {g_debugScenario}");
        Debug.WriteLine($"DEBUG:  g_debugSkipDialogs = {g_debugSkipDialogs}");
        Debug.WriteLine($"DEBUG:  s_enableLog = {s_enableLog}");
        Debug.WriteLine($"DEBUG:  g_starPortEnforceUnitLimit = {g_starPortEnforceUnitLimit}");

        if (!File_Init())
        {
            return 1;
        }

        /* Loading config from dune.cfg */
        if (!Config_Read("dune.cfg", g_config))
        {
            Config_Default(g_config);
            commit_dune_cfg = true;
        }
        /* reading config from sharpdune.ini which prevail over dune.cfg */
        SetLanguage_From_IniFile(g_config);

        /* Writing config to dune.cfg */
        if (commit_dune_cfg && !Config_Write("dune.cfg", g_config))
        {
            Trace.WriteLine("ERROR: Error writing to dune.cfg file.");
            return 1;
        }

        Input_Init();

        Drivers_All_Init();

        scaling_factor = IniFile_GetInteger("scalefactor", 2);
        if ((filter_text = IniFile_GetString("scalefilter", null)) != null)
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

        frame_rate = IniFile_GetInteger("framerate", 60);

        if (!SharpDune_Init(scaling_factor, scale_filter, frame_rate)) Environment.Exit(1);

        g_mouseDisabled = 0;

        GameLoop_Main();

        Trace.WriteLine(String_Get_ByIndex(Text.STR_THANK_YOU_FOR_PLAYING_DUNE_II));

        PrepareEnd();
        Free_IniFile();

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
        CTile[] t;
        var tPointer = 0;
        int i;

        g_validateStrictIfZero++;

        oldSelectionType = g_selectionType;
        g_selectionType = (ushort)SelectionType.MENTAT;

        Structure_Recount();
        Unit_Recount();
        Team_Recount();

        t = g_map; //[0];
        for (i = 0; i < 64 * 64; i++, tPointer++)
        {
            CStructure s;
            var u = Unit_Get_ByPackedTile((ushort)i);
            s = Structure_Get_ByPackedTile((ushort)i);

            if (u?.o.flags.used == false) t[tPointer].hasUnit = false;
            if (s?.o.flags.used == false) t[tPointer].hasStructure = false;
            if (t[tPointer].isUnveiled) Map_UnveilTile((ushort)i, (byte)g_playerHouseID);
        }

        find.houseID = (byte)HouseType.HOUSE_INVALID;
        find.index = 0xFFFF;
        find.type = 0xFFFF;

        while (true)
        {
            var u = Unit_Find(find);
            if (u == null) break;

            if (u.o.flags.isNotOnMap) continue;

            Unit_RemoveFog(u);
            Unit_UpdateMap(1, u);
        }

        find.houseID = (byte)HouseType.HOUSE_INVALID;
        find.index = 0xFFFF;
        find.type = 0xFFFF;

        while (true)
        {
            var s = Structure_Find(find);
            if (s == null) break;
            if (s.o.type is ((byte)StructureType.STRUCTURE_SLAB_1x1) or ((byte)StructureType.STRUCTURE_SLAB_2x2) or ((byte)StructureType.STRUCTURE_WALL)) continue;

            if (s.o.flags.isNotOnMap) continue;

            Structure_RemoveFog(s);

            if (s.o.type == (byte)StructureType.STRUCTURE_STARPORT && s.o.linkedID != 0xFF)
            {
                var u = Unit_Get_ByIndex(s.o.linkedID);

                if (!u.o.flags.used || !u.o.flags.isNotOnMap)
                {
                    s.o.linkedID = 0xFF;
                    s.countDown = 0;
                }
                else
                {
                    Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_READY);
                }
            }

            Script_Load(s.o.script, s.o.type);

            if (s.o.type == (byte)StructureType.STRUCTURE_PALACE)
            {
                House_Get_ByIndex(s.o.houseID).palacePosition = s.o.position;
            }

            if ((House_Get_ByIndex(s.o.houseID).palacePosition.x != 0) || (House_Get_ByIndex(s.o.houseID).palacePosition.y != 0)) continue;
            House_Get_ByIndex(s.o.houseID).palacePosition = s.o.position;
        }

        find.houseID = (byte)HouseType.HOUSE_INVALID;
        find.index = 0xFFFF;
        find.type = 0xFFFF;

        while (true)
        {
            var h = House_Find(find);
            if (h == null) break;

            h.structuresBuilt = Structure_GetStructuresBuilt(h);
            House_UpdateCreditsStorage(h.index);
            House_CalculatePowerAndCredit(h);
        }

        GUI_Palette_CreateRemap((byte)g_playerHouseID);

        Map_SetSelection(g_selectionPosition);

        if (g_structureActiveType != 0xFFFF)
        {
            Map_SetSelectionSize(g_table_structureInfo[g_structureActiveType].layout);
        }
        else
        {
            var s = Structure_Get_ByPackedTile(g_selectionPosition);

            if (s != null) Map_SetSelectionSize(g_table_structureInfo[s.o.type].layout);
        }

        Voice_LoadVoices((ushort)g_playerHouseID);

        g_tickHousePowerMaintenance = Math.Max(g_timerGame + 70, g_tickHousePowerMaintenance);
        g_viewport_forceRedraw = true;
        g_playerCredits = 0xFFFF;

        g_selectionType = oldSelectionType;
        g_validateStrictIfZero--;
    }

    /*
     * Initialize a game, by setting most variables to zero, cleaning the map, etc
     * etc.
     */
    internal static void Game_Init()
    {
        Unit_Init();
        Structure_Init();
        Team_Init();
        House_Init();

        Animation_Init();
        Explosion_Init();
        for (var i = 0; i < g_map.Length; i++) g_map[i] = new CTile(); //memset(g_map, 0, 64 * 64 * sizeof(Tile));

        Array.Fill<byte>(g_displayedViewport, 0, 0, g_displayedViewport.Length); //memset(g_displayedViewport, 0, sizeof(g_displayedViewport));
        Array.Fill<byte>(g_displayedMinimap, 0, 0, g_displayedMinimap.Length); //memset(g_displayedMinimap, 0, sizeof(g_displayedMinimap));
        Array.Fill<byte>(g_changedTilesMap, 0, 0, g_changedTilesMap.Length); //memset(g_changedTilesMap, 0, sizeof(g_changedTilesMap));
        Array.Fill<byte>(g_dirtyViewport, 0, 0, g_dirtyViewport.Length); //memset(g_dirtyViewport, 0, sizeof(g_dirtyViewport));
        Array.Fill<byte>(g_dirtyMinimap, 0, 0, g_dirtyMinimap.Length); //memset(g_dirtyMinimap, 0, sizeof(g_dirtyMinimap));

        Array.Fill<ushort>(g_mapTileID, 0, 0, 64 * 64); //memset(g_mapTileID, 0, 64 * 64 * sizeof(uint16));
        Array.Fill<short>(g_starportAvailable, 0, 0, g_starportAvailable.Length); //memset(g_starportAvailable, 0, sizeof(g_starportAvailable));

        Sound_Output_Feedback(0xFFFE);

        g_playerCreditsNoSilo = 0;
        g_houseMissileCountdown = 0;
        g_selectionState = 0; /* Invalid. */
        g_structureActivePosition = 0;

        g_unitHouseMissile = null;
        g_unitActive = null;
        g_structureActive = null;

        g_activeAction = 0xFFFF;
        g_structureActiveType = 0xFFFF;

        GUI_DisplayText(null, -1);

        SleepIdle();  /* let the game a chance to update screen, etc. */
    }

    /*
     * Load a scenario in a safe way, and prepare the game.
     * @param houseID The House which is going to play the game.
     * @param scenarioID The Scenario to load.
     */
    static void Game_LoadScenario(byte houseID, ushort scenarioID)
    {
        Sound_Output_Feedback(0xFFFE);

        Game_Init();

        g_validateStrictIfZero++;

        if (!Scenario_Load(scenarioID, houseID))
        {
            GUI_DisplayModalMessage("No more scenarios!", 0xFFFF);

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
        g_palette_998A = null; //free(g_palette_998A);

        GameLoop_Uninit();

        String_Uninit();
        Sprites_Uninit();
        Font_Uninit();
        Voice_UnloadVoices();

        Drivers_All_Uninit();

        if (g_mouseFileID != 0xFF) Mouse_SetMouseMode((byte)InputMouseMode.INPUT_MOUSE_MODE_NORMAL, null);

        File_Uninit();
        Timer_Uninit();
        GFX_Uninit();
        Video_Uninit();
    }
}
