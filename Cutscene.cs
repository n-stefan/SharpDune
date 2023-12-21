/* Introduction movie and cutscenes */

namespace SharpDune;

/* Direction of change in the #GameLoop_PalettePart_Update function. */
enum PalettePartDirection
{
    PPD_STOPPED,        /*!< Not changing. */
    PPD_TO_NEW_PALETTE, /*!< Modifying towards #s_palettePartTarget */
    PPD_TO_BLACK        /*!< Modifying towards all black. */
}

struct CreditString
{
    internal ushort x;
    internal short y;
    internal string text;
    internal byte separator;
    internal byte charHeight;
    internal byte type;
}

class CreditPosition
{
    internal ushort x;
    internal ushort y;
}

class Cutscene
{
    internal static bool g_canSkipIntro; /*!< When true, you can skip the intro by pressing a key or clicking. */

    static HouseAnimation_Subtitle[] s_houseAnimation_subtitle;       /*!< Subtitle part of animation data. */
    static HouseAnimation_SoundEffect[] s_houseAnimation_soundEffect; /*!< Soundeffect part of animation data. */
    static ushort s_feedback_base_index = 0xFFFF;                     /*!< base index in g_feedback - used in Intro animation.*/
    static ushort s_subtitleIndex = 0xFFFF;                           /*!< Unknown animation data. */
    static ushort s_subtitleWait = 0xFFFF;                            /*!< Unknown animation data. */
    static ushort s_houseAnimation_currentSubtitle;               /*!< Current subtitle (index) part of animation. */
    static ushort s_houseAnimation_currentSoundEffect;            /* Current voice (index) part of animation. */
    static bool s_subtitleActive;                             /* Unknown animation data. */

    static PalettePartDirection s_palettePartDirection; /*!< Direction of change. @see PalettePartDirection */
    static uint s_paletteAnimationTimeout;              /*!< Timeout value for the next palette change. */
    static ushort s_palettePartCount;                   /*!< Number of steps left before the target palette is reached. */
    static readonly byte[] s_palettePartTarget = new byte[18];   /*!< Target palette part (6 colours). */
    static readonly byte[] s_palettePartCurrent = new byte[18];  /*!< Current value of the palette part (6 colours, updated each call to #GameLoop_PalettePart_Update). */
    static readonly byte[] s_palettePartChange = new byte[18];   /*!< Amount of change of each RGB colour of the palette part with each step. */

    /*
     * The Intro.
     */
    internal static void GameLoop_GameIntroAnimation()
    {
        GUI_ChangeSelectionType((ushort)SelectionType.INTRO);

        GameLoop_Logos();

        if (Input_Keyboard_NextKey() == 0 || !g_canSkipIntro)
        {
            var animation = g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_INTRO];
            var subtitle = g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_INTRO];
            var soundEffect = g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_INTRO];

            Music_Play(0x1B);

            /* 0x4A = 74 = Intro feedback base index */
            GameLoop_PrepareAnimation(subtitle, 0x4A, soundEffect);

            GameLoop_PlayAnimation(animation);

            Driver_Music_FadeOut();

            GameLoop_FinishAnimation();
        }

        GUI_ChangeSelectionType((ushort)SelectionType.MENTAT);
    }

    /*
     * Shows the end game "movie"
     */
    internal static void GameLoop_GameEndAnimation()
    {
        HouseAnimation_Animation[] animation;
        HouseAnimation_Subtitle[] subtitle;
        HouseAnimation_SoundEffect[] soundEffect;
        ushort sound;

        Voice_LoadVoices(0xFFFE);

        switch (g_playerHouseID)
        {
            case HouseType.HOUSE_HARKONNEN:
                animation = g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_HARKONNEN];
                subtitle = g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_HARKONNEN];
                soundEffect = g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_HARKONNEN];
                sound = 0x1E;
                break;

            default:
            case HouseType.HOUSE_ATREIDES:
                animation = g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_ARTREIDES];
                subtitle = g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_ARTREIDES];
                soundEffect = g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_ARTREIDES];
                sound = 0x1F;
                break;

            case HouseType.HOUSE_ORDOS:
                animation = g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_ORDOS];
                subtitle = g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_ORDOS];
                soundEffect = g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL9_ORDOS];
                sound = 0x20;
                break;
        }

        GameLoop_PrepareAnimation(subtitle, 0xFFFF, soundEffect);

        Music_Play(sound);

        GameLoop_PlayAnimation(animation);

        Driver_Music_FadeOut();

        GameLoop_FinishAnimation();

        GameLoop_GameCredits();
    }

    internal static void GameLoop_LevelEndAnimation()
    {
        HouseAnimation_Animation[] animation;
        HouseAnimation_Subtitle[] subtitle;
        HouseAnimation_SoundEffect[] soundEffect;

        Input_History_Clear();

        switch (g_campaignID)
        {
            case 4:
                switch (g_playerHouseID)
                {
                    case HouseType.HOUSE_HARKONNEN:
                        animation = g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_HARKONNEN];
                        subtitle = g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_HARKONNEN];
                        soundEffect = g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_HARKONNEN];
                        break;

                    case HouseType.HOUSE_ATREIDES:
                        animation = g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_ARTREIDES];
                        subtitle = g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_ARTREIDES];
                        soundEffect = g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_ARTREIDES];
                        break;

                    case HouseType.HOUSE_ORDOS:
                        animation = g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_ORDOS];
                        subtitle = g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_ORDOS];
                        soundEffect = g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL4_ORDOS];
                        break;

                    default: return;
                }
                break;

            case 8:
                switch (g_playerHouseID)
                {
                    case HouseType.HOUSE_HARKONNEN:
                        animation = g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_HARKONNEN];
                        subtitle = g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_HARKONNEN];
                        soundEffect = g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_HARKONNEN];
                        break;

                    case HouseType.HOUSE_ATREIDES:
                        animation = g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_ARTREIDES];
                        subtitle = g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_ARTREIDES];
                        soundEffect = g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_ARTREIDES];
                        break;

                    case HouseType.HOUSE_ORDOS:
                        animation = g_table_houseAnimation_animation[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_ORDOS];
                        subtitle = g_table_houseAnimation_subtitle[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_ORDOS];
                        soundEffect = g_table_houseAnimation_soundEffect[(int)HouseAnimationType.HOUSEANIMATION_LEVEL8_ORDOS];
                        break;

                    default: return;
                }
                break;

            default: return;
        }

        GameLoop_PrepareAnimation(subtitle, 0xFFFF, soundEffect);

        Music_Play(0x22);

        GameLoop_PlayAnimation(animation);

        Driver_Music_FadeOut();

        GameLoop_FinishAnimation();
    }

    static void GameLoop_FinishAnimation()
    {
        GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x1);
        GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x2);

        GUI_SetPaletteAnimated(g_palette2, 60);

        GUI_ClearScreen(Screen.NO0);

        Input_History_Clear();

        GFX_ClearBlock(Screen.NO3);
    }

    static void GameLoop_PrepareAnimation(HouseAnimation_Subtitle[] subtitle, ushort feedback_base_index, HouseAnimation_SoundEffect[] soundEffect)
    {
        byte i;
        var colors = new byte[16];

        s_houseAnimation_subtitle = subtitle;
        s_houseAnimation_soundEffect = soundEffect;

        s_houseAnimation_currentSubtitle = 0;
        s_houseAnimation_currentSoundEffect = 0;

        g_fontCharOffset = 0;

        s_feedback_base_index = feedback_base_index;
        s_subtitleIndex = 0;
        s_subtitleWait = 0xFFFF;
        s_subtitleActive = false;

        s_palettePartDirection = PalettePartDirection.PPD_STOPPED;
        s_palettePartCount = 0;
        s_paletteAnimationTimeout = 0;

        GFX_ClearScreen(Screen.ACTIVE);

        File_ReadBlockFile("INTRO.PAL", g_palette1, 256 * 3);

        Buffer.BlockCopy(g_palette1, 0, g_palette_998A, 0, 256 * 3); //memcpy(g_palette_998A, g_palette1, 256 * 3);

        Font_Select(g_fontIntro);

        GFX_Screen_SetActive(Screen.NO0);

        Buffer.BlockCopy(g_palette1, (144 + s_houseAnimation_subtitle[0].colour * 16) * 3, s_palettePartTarget, 0, 6 * 3); //memcpy(s_palettePartTarget, &g_palette1[(144 + s_houseAnimation_subtitle->colour * 16) * 3], 6 * 3);

        Array.Fill<byte>(g_palette1, 0, 215 * 3, 6 * 3); //memset(&g_palette1[215 * 3], 0, 6 * 3);

        Buffer.BlockCopy(s_palettePartTarget, 0, s_palettePartCurrent, 0, 6 * 3); //memcpy(s_palettePartCurrent, s_palettePartTarget, 6 * 3);

        Array.Fill<byte>(s_palettePartChange, 0, 0, 6 * 3); //memset(s_palettePartChange, 0, 6 * 3);

        colors[0] = 0;
        for (i = 0; i < 6; i++) colors[i + 1] = (byte)(215 + i);

        GUI_InitColors(colors, 0, 15);
    }

    static void GameLoop_PlayAnimation(HouseAnimation_Animation[] animation)
    {
        byte animationStep = 0;
        var pointer = 0;

        while (animation[pointer].duration != 0)
        {
            ushort frameCount;
            ushort posX;
            ushort posY;
            var timeout = (uint)(g_timerGUI + animation[pointer].duration * 6);
            var timeout2 = timeout + 30;   /* timeout + 0.5 s */
            uint timeLeftForFrame;
            uint timeLeft;
            var mode = (ushort)(animation[pointer].flags & 0x3);
            ushort addFrameCount;   /* additional frame count */
            ushort frame;
            /*WSAObject*/
            (WSAHeader, Array<byte>) wsa;

            if ((animation[pointer].flags & HOUSEANIM_FLAGS_POS0_0) != 0)
            {
                posX = 0;
                posY = 0;
            }
            else
            {
                posX = 8;
                posY = 24;
            }

            s_subtitleIndex = 0;

            Debug.WriteLine($"DEBUG: GameLoop_PlayAnimation() {animationStep} {animation[pointer].str} mode={mode} flags={animation[pointer].flags & ~3:X3}");
            if (mode == 0)
            {
                wsa = (null, null);
                frame = 0;
            }
            else
            {
                string filenameBuffer; //char[16]
                Memory<byte> wsaData;
                uint wsaSize;
                bool wsaReservedDisplayFrame;

                if (mode == 3)
                {
                    frame = animation[pointer].frameCount;
                    wsaReservedDisplayFrame = true;
                }
                else
                {
                    frame = 0;
                    wsaReservedDisplayFrame = (animation[pointer].flags & HOUSEANIM_FLAGS_DISPLAYFRAME) != 0;
                }

                if ((animation[pointer].flags & (HOUSEANIM_FLAGS_FADEIN2 | HOUSEANIM_FLAGS_FADEIN)) != 0)
                {
                    GUI_ClearScreen(Screen.NO1);

                    //wsa = new WSAObject();
                    wsaData = GFX_Screen_Get_ByIndex(Screen.NO2);

                    wsaSize = (uint)(GFX_Screen_GetSize_ByIndex(Screen.NO2) + GFX_Screen_GetSize_ByIndex(Screen.NO3));
                    wsaReservedDisplayFrame = false;
                }
                else
                {
                    //wsa = new WSAObject();
                    wsaData = GFX_Screen_Get_ByIndex(Screen.NO1);

                    wsaSize = (uint)(GFX_Screen_GetSize_ByIndex(Screen.NO1) + GFX_Screen_GetSize_ByIndex(Screen.NO2) + GFX_Screen_GetSize_ByIndex(Screen.NO3));
                }

                filenameBuffer = $"{animation[pointer].str}.WSA"; //snprintf(filenameBuffer, sizeof(filenameBuffer), "%.8s.WSA", animation.string);
                wsa = WSA_LoadFile(filenameBuffer, wsaData, wsaSize, wsaReservedDisplayFrame);
            }

            addFrameCount = 0;
            if ((animation[pointer].flags & HOUSEANIM_FLAGS_FADEOUTTEXT) != 0)
            {
                timeout -= 45;
                addFrameCount++;
            }
            else if ((animation[pointer].flags & HOUSEANIM_FLAGS_FADETOWHITE) != 0)
            {
                timeout -= 15;
                addFrameCount++;
            }

            if ((animation[pointer].flags & HOUSEANIM_FLAGS_FADEINTEXT) != 0)
            {
                GameLoop_PlaySubtitle(animationStep);
                WSA_DisplayFrame(wsa, frame++, posX, posY, Screen.NO0);
                GameLoop_PalettePart_Update(true);

                Buffer.BlockCopy(s_palettePartCurrent, 0, g_palette1, 215 * 3, 18); //memcpy(&g_palette1[215 * 3], s_palettePartCurrent, 18);

                GUI_SetPaletteAnimated(g_palette1, 45);

                addFrameCount++;
            }
            else if ((animation[pointer].flags & (HOUSEANIM_FLAGS_FADEIN2 | HOUSEANIM_FLAGS_FADEIN)) != 0)
            {
                GameLoop_PlaySubtitle(animationStep);
                WSA_DisplayFrame(wsa, frame++, posX, posY, Screen.NO1);
                addFrameCount++;

                if ((animation[pointer].flags & (HOUSEANIM_FLAGS_FADEIN2 | HOUSEANIM_FLAGS_FADEIN)) == HOUSEANIM_FLAGS_FADEIN2)
                {
                    GUI_Screen_FadeIn2(8, 24, 304, 120, Screen.NO1, Screen.NO0, 1, false);
                }
                else if ((animation[pointer].flags & (HOUSEANIM_FLAGS_FADEIN2 | HOUSEANIM_FLAGS_FADEIN)) == HOUSEANIM_FLAGS_FADEIN)
                {
                    GUI_Screen_FadeIn(1, 24, 1, 24, 38, 120, Screen.NO1, Screen.NO0);
                }
            }

            timeLeft = timeout - g_timerGUI;
            timeLeftForFrame = 0;
            frameCount = 1;

            switch (mode)
            {
                case 0:
                    frameCount = (ushort)(animation[pointer].frameCount - addFrameCount);
                    timeLeftForFrame = timeLeft / frameCount;
                    break;

                case 1:
                    frameCount = WSA_GetFrameCount(wsa);
                    timeLeftForFrame = timeLeft / animation[pointer].frameCount;
                    break;

                case 2:
                    frameCount = (ushort)(WSA_GetFrameCount(wsa) - addFrameCount);
                    timeLeftForFrame = timeLeft / frameCount;
                    timeout -= timeLeftForFrame;
                    break;

                case 3:
                    frame = animation[pointer].frameCount;
                    frameCount = 1;
                    timeLeftForFrame = timeLeft / 20;
                    break;

                default:
                    PrepareEnd();
                    Trace.WriteLine($"ERROR: Bad mode in animation #{animationStep}.");
                    Environment.Exit(0);
                    break;
            }

            while (timeout > g_timerGUI)
            {
                g_timerTimeout = timeLeftForFrame;

                GameLoop_PlaySubtitle(animationStep);
                WSA_DisplayFrame(wsa, frame++, posX, posY, Screen.NO0);

                if (mode == 1 && frame == frameCount)
                {
                    frame = 0;
                }
                else if (mode == 3)
                {
                    frame--;
                }

                if (Input_Keyboard_NextKey() != 0 && g_canSkipIntro)
                {
                    WSA_Unload(wsa);
                    return;
                }

                do
                {
                    GameLoop_PalettePart_Update(false);
                    SleepIdle();
                } while (g_timerTimeout != 0 && timeout > g_timerGUI);
            }

            if (mode == 2)
            {
                bool displayed;
                do
                {
                    GameLoop_PlaySubtitle(animationStep);
                    displayed = WSA_DisplayFrame(wsa, frame++, posX, posY, Screen.NO0);
                    SleepIdle();
                } while (displayed);
            }

            if ((animation[pointer].flags & HOUSEANIM_FLAGS_FADETOWHITE) != 0)
            {
                Array.Fill<byte>(g_palette_998A, 63, 3 * 1, 255 * 3); //memset(&g_palette_998A[3 * 1], 63, 255 * 3);

                Buffer.BlockCopy(s_palettePartCurrent, 0, g_palette_998A, 215 * 3, 18); //memcpy(&g_palette_998A[215 * 3], s_palettePartCurrent, 18);

                GUI_SetPaletteAnimated(g_palette_998A, 15);

                Buffer.BlockCopy(g_palette1, 0, g_palette_998A, 0, 256 * 3); //memcpy(g_palette_998A, g_palette1, 256 * 3);
            }

            if ((animation[pointer].flags & HOUSEANIM_FLAGS_FADEOUTTEXT) != 0)
            {
                GameLoop_PalettePart_Update(true);

                Buffer.BlockCopy(s_palettePartCurrent, 0, g_palette_998A, 215 * 3, 18); //memcpy(&g_palette_998A[215 * 3], s_palettePartCurrent, 18);

                GUI_SetPaletteAnimated(g_palette_998A, 45);
            }

            WSA_Unload(wsa);

            animationStep++;
            pointer++;

            while (timeout2 > g_timerGUI) SleepIdle();
        }
    }

    /*
     * Update part of the palette one step.
     * @param finishNow Finish all steps now.
     * @return Direction of change for the next call.
     * @note If \a finishNow, the new palette is not written to the screen.
     * @see PalettePartDirection
     */
    static ushort GameLoop_PalettePart_Update(bool finishNow)
    {
        Sound_StartSpeech();

        if (s_palettePartDirection == PalettePartDirection.PPD_STOPPED) return 0;

        if (s_paletteAnimationTimeout >= g_timerGUI && !finishNow) return (ushort)s_palettePartDirection;

        s_paletteAnimationTimeout = g_timerGUI + 7;
        if (--s_palettePartCount == 0 || finishNow)
        {
            if (s_palettePartDirection == PalettePartDirection.PPD_TO_NEW_PALETTE)
            {
                Buffer.BlockCopy(s_palettePartTarget, 0, s_palettePartCurrent, 0, 18); //memcpy(s_palettePartCurrent, s_palettePartTarget, 18);
            }
            else
            {
                Array.Fill<byte>(s_palettePartCurrent, 0, 0, 18); //memset(s_palettePartCurrent, 0, 18);
            }

            s_palettePartDirection = PalettePartDirection.PPD_STOPPED;
        }
        else
        {
            byte i;

            for (i = 0; i < 18; i++)
            {
                s_palettePartCurrent[i] = s_palettePartDirection == PalettePartDirection.PPD_TO_NEW_PALETTE
                    ? (byte)Math.Min(s_palettePartCurrent[i] + s_palettePartChange[i], s_palettePartTarget[i])
                    : (byte)Math.Max(s_palettePartCurrent[i] - s_palettePartChange[i], 0);
            }
        }

        if (finishNow) return (ushort)s_palettePartDirection;

        Buffer.BlockCopy(s_palettePartCurrent, 0, g_palette_998A, 215 * 3, 18); //memcpy(&g_palette_998A[215 * 3], s_palettePartCurrent, 18);

        GFX_SetPalette(g_palette_998A);

        return (ushort)s_palettePartDirection;
    }

    static void GameLoop_PlaySubtitle(byte animation)
    {
        HouseAnimation_Subtitle subtitle;
        byte i;
        var colors = new byte[16];

        s_subtitleIndex++;

        GameLoop_PlaySoundEffect(animation);

        subtitle = s_houseAnimation_subtitle[s_houseAnimation_currentSubtitle];

        if (subtitle.stringID == 0xFFFF || subtitle.animationID > animation) return;

        if (s_subtitleActive)
        {
            if (s_subtitleWait == 0xFFFF) s_subtitleWait = subtitle.waitFadeout;
            if (s_subtitleWait-- != 0) return;

            s_subtitleActive = false;
            s_houseAnimation_currentSubtitle++;
            s_palettePartDirection = PalettePartDirection.PPD_TO_BLACK;

            if (subtitle.paletteFadeout != 0)
            {
                s_palettePartCount = subtitle.paletteFadeout;

                for (i = 0; i < 18; i++)
                {
                    s_palettePartChange[i] = (byte)(s_palettePartTarget[i] / s_palettePartCount);
                    if (s_palettePartChange[i] == 0) s_palettePartChange[i] = 1;
                }

                return;
            }

            Buffer.BlockCopy(s_palettePartTarget, 0, s_palettePartChange, 0, 18); //memcpy(s_palettePartChange, s_palettePartTarget, 18);
            s_palettePartCount = 1;
            return;
        }

        if (s_subtitleWait == 0xFFFF) s_subtitleWait = subtitle.waitFadein;
        if (s_subtitleWait-- != 0) return;

        Buffer.BlockCopy(g_palette1, (144 + (subtitle.colour * 16)) * 3, s_palettePartTarget, 0, 18); //memcpy(s_palettePartTarget, &g_palette1[(144 + (subtitle.colour * 16)) * 3], 18);

        s_subtitleActive = true;

        GUI_DrawFilledRectangle(0, (short)(subtitle.top == 85 ? 0 : subtitle.top), SCREEN_WIDTH - 1, SCREEN_HEIGHT - 1, 0);

        if (g_enableVoices && s_feedback_base_index != 0xFFFF && s_houseAnimation_currentSubtitle != 0 && g_config.language == (byte)Language.ENGLISH)
        {
            /* specific code for Intro
             * @see GameLoop_GameIntroAnimation() */
            var feedback_index = (ushort)(s_feedback_base_index + s_houseAnimation_currentSubtitle);

            Sound_Output_Feedback(feedback_index);

            //if (g_feedback[feedback_index].messageId != 0)
            //{
            /* force drawing of subtitle */
            GameLoop_DrawText(String_Get_ByIndex(subtitle.stringID), subtitle.top);
            //}
        }
        else
        {
            if (subtitle.stringID != (ushort)Text.STR_NULL)
            {
                GameLoop_DrawText(String_Get_ByIndex(subtitle.stringID), subtitle.top);
            }
        }

        s_palettePartDirection = PalettePartDirection.PPD_TO_NEW_PALETTE;

        if (subtitle.paletteFadein != 0)
        {
            s_palettePartCount = subtitle.paletteFadein;

            for (i = 0; i < 18; i++)
            {
                s_palettePartChange[i] = (byte)(s_palettePartTarget[i] / s_palettePartCount);
                if (s_palettePartChange[i] == 0) s_palettePartChange[i] = 1;
            }
        }
        else
        {
            Buffer.BlockCopy(s_palettePartTarget, 0, s_palettePartChange, 0, 18); //memcpy(s_palettePartChange, s_palettePartTarget, 18);
            s_palettePartCount = 1;
        }

        if (g_playerHouseID != HouseType.HOUSE_INVALID || s_houseAnimation_currentSubtitle != 2) return;

        GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x21);

        GUI_DrawText_Wrapper("Copyright (c) 1992 Westwood Studios, Inc.", 160, 189, 215, 0, 0x112);

        g_fontCharOffset = 0;

        colors[0] = 0;
        for (i = 0; i < 6; i++) colors[i + 1] = (byte)(215 + i);

        GUI_InitColors(colors, 0, 15);

        Font_Select(g_fontIntro);
    }

    static void GameLoop_DrawText(string str, ushort top)
    {
        var lines = str.Split('\r');

        GUI_DrawText_Wrapper(lines[0], 160, (short)top, 215, 0, 0x100);

        if (lines.Length == 1) return;

        GUI_DrawText_Wrapper(lines[1], 160, (short)(top + 18), 215, 0, 0x100);
    }

    static void GameLoop_PlaySoundEffect(byte animation)
    {
        var soundEffect = s_houseAnimation_soundEffect[s_houseAnimation_currentSoundEffect];

        if (soundEffect.animationID > animation || soundEffect.wait > s_subtitleIndex) return;

        Voice_Play(soundEffect.voiceID);

        s_houseAnimation_currentSoundEffect++;
    }

    /*
     * Logos at begin of intro.
     */
    static void GameLoop_Logos()
    {
        Screen oldScreenID;
        /* void* *//*WSAObject*/
        (WSAHeader, Array<byte>) wsa;
        ushort frame;

        oldScreenID = GFX_Screen_SetActive(Screen.NO0);

        GFX_SetPalette(g_palette2);
        GFX_ClearScreen(Screen.NO0);

        File_ReadBlockFile("WESTWOOD.PAL", g_palette_998A, 256 * 3);

        frame = 0;
        wsa = WSA_LoadFile("WESTWOOD.WSA", GFX_Screen_Get_ByIndex(Screen.NO1), (uint)(GFX_Screen_GetSize_ByIndex(Screen.NO1) + GFX_Screen_GetSize_ByIndex(Screen.NO2) + GFX_Screen_GetSize_ByIndex(Screen.NO3)), true);
        WSA_DisplayFrame(wsa, frame++, 0, 0, Screen.NO0);

        GUI_SetPaletteAnimated(g_palette_998A, 60);

        Music_Play(0x24);

        g_timerTimeout = 360;

        while (WSA_DisplayFrame(wsa, frame++, 0, 0, Screen.NO0)) Timer_Sleep(6);

        WSA_Unload(wsa);

        if (Input_Keyboard_NextKey() != 0 && g_canSkipIntro)
        {
            LogosExit();
            return;
        }

        Voice_LoadVoices(0xFFFF);

        for (; g_timerTimeout != 0; SleepIdle())
        {
            if (Input_Keyboard_NextKey() != 0 && g_canSkipIntro)
            {
                LogosExit();
                return;
            }
        }

        GUI_SetPaletteAnimated(g_palette2, 60);

        while (Driver_Music_IsPlaying()) SleepIdle();

        GUI_SetPaletteAnimated(g_palette2, 60);

        GFX_ClearScreen(Screen.ACTIVE);

        Sprites_LoadImage(String_GenerateFilename("AND"), Screen.NO1, g_palette_998A);

        GUI_Screen_Copy(0, 0, 0, 0, SCREEN_WIDTH / 8, (short)SCREEN_HEIGHT, Screen.NO1, Screen.NO0);

        GUI_SetPaletteAnimated(g_palette_998A, 30);

        for (g_timerTimeout = 60; g_timerTimeout != 0; SleepIdle())
        {
            if (Input_Keyboard_NextKey() != 0 && g_canSkipIntro)
            {
                LogosExit();
                return;
            }
        }

        GUI_SetPaletteAnimated(g_palette2, 30);

        GUI_ClearScreen(Screen.NO0);

        Sprites_LoadImage("VIRGIN.CPS", Screen.NO1, g_palette_998A);

        GUI_Screen_Copy(0, 0, 0, 0, SCREEN_WIDTH / 8, (short)SCREEN_HEIGHT, Screen.NO1, Screen.NO0);

        GUI_SetPaletteAnimated(g_palette_998A, 30);

        for (g_timerTimeout = 180; g_timerTimeout != 0; SleepIdle())
        {
            if (Input_Keyboard_NextKey() != 0 && g_canSkipIntro)
            {
                break;
            }
        }

        LogosExit();

        void LogosExit()
        {
            GUI_SetPaletteAnimated(g_palette2, 30);

            GUI_ClearScreen(Screen.NO0);

            GFX_Screen_SetActive(oldScreenID);
        }
    }

    static readonly byte[] colours = [0, 0, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0];
    /*
     * Shows the game credits.
     */
    static void GameLoop_GameCredits()
    {
        ushort i;
        var remap = new byte[256];
        Memory<byte> credits_buffer;
        int credits_bufferPointer;

        GUI_Mouse_Hide_Safe();

        Widget_SetCurrentWidget(20);

        Sprites_LoadImage("BIGPLAN.CPS", Screen.NO1, g_palette_998A);

        GUI_ClearScreen(Screen.NO0);

        GUI_Screen_Copy((short)g_curWidgetXBase, (short)g_curWidgetYBase, (short)g_curWidgetXBase, (short)g_curWidgetYBase, (short)g_curWidgetWidth, (short)g_curWidgetHeight, Screen.NO1, Screen.NO0);

        GUI_SetPaletteAnimated(g_palette_998A, 60);

        Music_Play(0);

        GameLoop_Uninit();

        Music_Play(33);

        /*memory = GFX_Screen_Get_ByIndex(NO2);*/

        for (i = 0; i < 256; i++)
        {
            byte high, low;    /* high / low nibble */

            remap[i] = (byte)i;

            high = (byte)(i >> 4);
            low = (byte)(i & 15);

            /* map colors 144-150 to the one of the player House */
            if (high == 9 && low <= 6)
            {
                remap[i] = (byte)((((byte)g_playerHouseID + 9) << 4) + low);
                Debug.WriteLine($"DEBUG: GameLoop_GameCredits() remap color {i} to {remap[i]}");
            }
        }

        Sprites_LoadImage("MAPPLAN.CPS", Screen.NO1, g_palette_998A);

        GUI_Palette_RemapScreen((ushort)(g_curWidgetXBase << 3), g_curWidgetYBase, (ushort)(g_curWidgetWidth << 3), g_curWidgetHeight, Screen.NO1, remap);

        GUI_Screen_FadeIn2((short)(g_curWidgetXBase << 3), (short)g_curWidgetYBase, (short)(g_curWidgetWidth << 3), (short)g_curWidgetHeight, Screen.NO1, Screen.NO0, 1, false);

        GameCredits_LoadPalette();

        credits_buffer = GFX_Screen_Get_ByIndex(Screen.NO3);
        credits_bufferPointer = SCREEN_WIDTH * g_curWidgetHeight;
        Debug.WriteLine($"DEBUG: GameLoop_GameCredits() credit buffer is {g_curWidgetHeight} lines in NO3 buffer");

        GUI_Mouse_Hide_Safe();

        GUI_InitColors(colours, 0, (byte)(colours.Length - 1));

        g_fontCharOffset = -1;

        GFX_SetPalette(g_palette1);

        for (; ; SleepIdle())
        {
            File_ReadBlockFile(String_GenerateFilename("CREDITS"), credits_buffer, GFX_Screen_GetSize_ByIndex(Screen.NO3), credits_bufferPointer);

            GameCredits_Play(credits_buffer.Span, credits_bufferPointer, 20, Screen.NO1, Screen.NO2, 6);

            if (Input_Keyboard_NextKey() != 0) break;

            Music_Play(33);
        }

        GUI_SetPaletteAnimated(g_palette2, 60);

        Driver_Music_FadeOut();

        GFX_ClearScreen(Screen.ACTIVE);
    }

    static void GameCredits_LoadPalette()
    {
        ushort i;
        int p;

        if (g_palette1 != null) Trace.WriteLine($"WARNING: g_palette1 already allocated");
        else g_palette1 = new byte[256 * 3 * 10]; //malloc(256 * 3 * 10);
        if (g_palette2 != null) Trace.WriteLine($"WARNING: g_palette2 already allocated");
        else g_palette2 = new byte[256 * 3]; //calloc(1, 256 * 3);

        File_ReadBlockFile("IBM.PAL", g_palette1, 256 * 3);

        /* Create 10 fadein/fadeout palettes */
        p = 0;
        for (i = 0; i < 10; i++)
        {
            ushort j;

            for (j = 0; j < 255 * 3; j++) g_palette1[p++] = (byte)(g_palette1[j] * (9 - i) / 9); //*p++ = *pr++ * (9 - i) / 9;

            g_palette1[p++] = 0x3F;
            g_palette1[p++] = 0x3F;
            g_palette1[p++] = 0x3F;
        }
    }

    static void GameCredits_Play(Span<byte> data, int dataPointer, ushort windowID, Screen spriteScreenID, Screen backScreenID, ushort delay)
    {
        ushort i;
        ushort stringCount = 0;
        uint timetoWait;
        ushort spriteID = 514;
        var textEnd = false;
        ushort spriteX;
        ushort spriteY;
        ushort spritePos = 0;
        var strings = new CreditString[33];
        var positions = new CreditPosition[6];
        ushort stage = 4;
        ushort counter = 60;

        for (i = 0; i < strings.Length; i++) strings[i] = new CreditString();
        for (i = 0; i < positions.Length; i++) positions[i] = new CreditPosition();

        Widget_SetCurrentWidget(windowID);

        spriteX = (ushort)((g_curWidgetWidth << 3) - Sprite_GetWidth(g_sprites[spriteID]));
        spriteY = (ushort)(g_curWidgetHeight - Sprite_GetHeight(g_sprites[spriteID]));

        positions[0].x = spriteX;
        positions[0].y = 0;
        positions[1].x = 0;
        positions[1].y = (ushort)(spriteY / 2);
        positions[2].x = spriteX;
        positions[2].y = spriteY;
        positions[3].x = 0;
        positions[3].y = 0;
        positions[4].x = spriteX;
        positions[4].y = (ushort)(spriteY / 2);
        positions[5].x = 0;
        positions[5].y = spriteY;

        /* initialize */
        GUI_Screen_Copy(0, 0, 0, 0, SCREEN_WIDTH / 8, (short)SCREEN_HEIGHT, Screen.NO0, spriteScreenID);
        GUI_Screen_Copy(0, 0, 0, 0, SCREEN_WIDTH / 8, (short)SCREEN_HEIGHT, spriteScreenID, backScreenID);

        GameCredits_SwapScreen((ushort)(g_curWidgetYBase + 24), g_curWidgetHeight, spriteScreenID, Screen.NO3);

        GFX_Screen_SetActive(Screen.NO0);
        timetoWait = g_timerSleep;

        Input_History_Clear();

        while ((!textEnd || stage != 0) && (Input_Keyboard_NextKey() == 0))
        {
            while (timetoWait > g_timerSleep) SleepIdle();

            timetoWait = g_timerSleep + delay;

            while ((g_curWidgetHeight / 6) + 2 > stringCount && data[dataPointer] != 0)
            {
                ReadOnlySpan<char> text; //char *
                ushort y;

                if (stringCount != 0)
                {
                    /* below or next to the previous string */
                    y = (ushort)strings[stringCount - 1].y;
                    if (strings[stringCount - 1].separator != 5)
                    {
                        y += (ushort)(strings[stringCount - 1].charHeight + strings[stringCount - 1].charHeight / 8);
                    }
                }
                else
                {
                    y = g_curWidgetHeight;
                }

                text = SharpDune.Encoding.GetString(data.Slice(dataPointer, 50));

                var index = text.IndexOfAny(['\x05', '\r']);
                if (index != -1)
                {
                    dataPointer += index; //strpbrk(data, "\x05\r");
                    text = text.Slice(0, index);
                }
                else
                {
                    data = new[] { (byte)0 }; //strchr(text, '\0');
                }

                strings[stringCount].separator = data[dataPointer];
                data[dataPointer] = 0; //*data = '\0';
                if (strings[stringCount].separator != 0) dataPointer++;
                strings[stringCount].type = 0;

                if (text.Length > 0)
                {
                    switch ((byte)text[0])
                    {
                        case 1:
                            text = text.Slice(1);
                            Font_Select(g_fontNew6p);
                            break;
                        case 2:
                            text = text.Slice(1);
                            Font_Select(g_fontNew8p);
                            break;
                        case 3:
                        case 4:
                            strings[stringCount].type = (byte)text[0];
                            text = text.Slice(1);
                            break;
                    }
                }

                strings[stringCount].charHeight = g_fontCurrent.height;

                strings[stringCount].x = strings[stringCount].type switch
                {
                    /* "xxx by:" text : on the left */
                    3 => (ushort)(157 - Font_GetStringWidth(text)),
                    /* names on the right */
                    4 => 161,
                    /* centered strings */
                    _ => (ushort)(1 + (SCREEN_WIDTH - Font_GetStringWidth(text)) / 2),
                };

                strings[stringCount].y = (short)y;
                strings[stringCount].text = text.ToString();

                stringCount++;
            }

            switch (stage)
            {
                case 0: /* 0 : clear */
                    GUI_ClearScreen(spriteScreenID);

                    if (spriteID == 514) GUI_ClearScreen(backScreenID);

                    stage++;
                    counter = 2;
                    break;

                case 1:
                case 4: /* 1, 4 : Wait */
                    if (counter == 0)
                    {
                        counter = 0;
                        stage++;
                    }
                    else
                    {
                        counter--;
                    }
                    break;

                case 2: /* 2 : display new picture */
                    if (spriteID == 525) spriteID = 514;

                    GUI_DrawSprite(spriteScreenID, g_sprites[spriteID], (short)positions[spritePos].x, (short)positions[spritePos].y, windowID, DRAWSPRITE_FLAG_WIDGETPOS);

                    counter = 8;
                    stage++;
                    spriteID++;
                    if (++spritePos > 5) spritePos = 0;
                    break;

                case 3: /* 3 : fade from black */
                    if (counter < 8) GFX_SetPalette(g_palette1.AsSpan(256 * 3 * counter));

                    if (counter-- == 0)
                    {
                        stage++;
                        counter = 20;
                    }
                    break;

                case 5: /* 5 : fade to black */
                    if (counter > 0) GFX_SetPalette(g_palette1.AsSpan(256 * 3 * counter));

                    if (counter++ >= 8) stage = 0;
                    break;

                default: break;
            }

            /* copy sprite (image) to back buffer */
            GUI_Screen_Copy((short)g_curWidgetXBase, (short)g_curWidgetYBase, (short)g_curWidgetXBase, (short)g_curWidgetYBase, (short)g_curWidgetWidth, (short)g_curWidgetHeight, spriteScreenID, backScreenID);

            /* draw all strings on back buffer and scroll them 1 pixel up */
            for (i = 0; i < stringCount; i++)
            {
                if (strings[i].y < g_curWidgetHeight)
                {
                    GFX_Screen_SetActive(backScreenID);

                    Font_Select(g_fontNew8p);

                    if (strings[i].charHeight != g_fontCurrent.height) Font_Select(g_fontNew6p);

                    GUI_DrawText(strings[i].text, (short)strings[i].x, (short)(strings[i].y + g_curWidgetYBase), 255, 0);

                    GFX_Screen_SetActive(Screen.NO0);
                }

                strings[i].y--;
            }

            /* display what we just draw on back buffer */
            GameCredits_SwapScreen((ushort)(g_curWidgetYBase + 24), g_curWidgetHeight, backScreenID, Screen.NO3);

            if (strings[0].y < -10)
            {
                /* remove 1st string and shift the other */
                //strings[0].text += strlen(strings[0].text);
                strings[0].text = ((char)strings[0].separator).ToString();
                stringCount--;
                Array.Copy(strings, 1, strings, 0, stringCount); //memmove(&strings[0], &strings[1], stringCount * sizeof(*strings));
            }

            if ((g_curWidgetHeight / 6 + 2) > stringCount)
            {
                if (strings[stringCount - 1].y + strings[stringCount - 1].charHeight < g_curWidgetYBase + g_curWidgetHeight) textEnd = true;
            }
        }

        /* fade to black */
        GUI_SetPaletteAnimated(g_palette2, 120);

        GUI_ClearScreen(Screen.NO0);
        GUI_ClearScreen(spriteScreenID);
        GUI_ClearScreen(backScreenID);
    }

    static void GameCredits_SwapScreen(ushort top, ushort height, Screen srcScreenID, Screen dstScreenID)
    {
        var bPointer = 0;
        var screen1Pointer = 0;
        var screen2Pointer = 0;

        var b = GFX_Screen_Get_ByIndex(dstScreenID);   /* destination */
        var screen1 = GFX_Screen_Get_ByIndex(srcScreenID).Slice(top * SCREEN_WIDTH / 2);  /* source */
        var screen2 = GFX_Screen_Get_ByIndex(Screen.NO0).Slice(top * SCREEN_WIDTH / 2);   /* secondary destination : Video RAM*/

        for (var count = height * SCREEN_WIDTH / 2; count > 0; count--)
        {
            if (b[bPointer] != screen1[screen1Pointer] || b[bPointer + 1] != screen1[screen1Pointer + 1])
            {
                b[bPointer] = screen1[screen1Pointer];
                b[bPointer + 1] = screen1[screen1Pointer + 1];
                screen2[screen2Pointer] = screen1[screen1Pointer];
                screen2[screen2Pointer + 1] = screen1[screen1Pointer + 1];
            }
            bPointer += 2;
            screen1Pointer += 2;
            screen2Pointer += 2;
        }
    }
}
