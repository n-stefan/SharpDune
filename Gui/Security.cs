/* Security */

namespace SharpDune.Gui;

static class Security
{
    /*
     * Ask the security question to the user. Give him 3 times. If he fails,
     *  return false, otherwise true.
     * @return True if and only if the user answered one of the three questions
     *   correct.
     */
    internal static bool GUI_Security_Show()
    {
        string wsaHouseFilename;
        ushort questionsCount;
        ushort oldCurrentWidget;
        Screen oldScreenID;
        ushort i;
        bool valid;
        string question;

        g_disableOtherMovement = true;
        g_interrogation = true;

        wsaHouseFilename = House_GetWSAHouseFilename((byte)g_playerHouseID);
        if (wsaHouseFilename == null) return true;

        GUI_SetPaletteAnimated(g_palette2, 15);

        GUI_Mentat_Display(wsaHouseFilename, (byte)g_playerHouseID);

        GUI_Mouse_Hide_Safe();
        GUI_Screen_Copy(0, 0, 0, 0, SCREEN_WIDTH / 8, (short)SCREEN_HEIGHT, Screen.NO1, Screen.NO0);
        GUI_Mouse_Show_Safe();

        GUI_SetPaletteAnimated(g_palette1, 15);

        //strncpy(g_readBuffer, String_Get_ByIndex(STR_SECURITY_TEXT_HARKONNEN + g_playerHouseID * 3), g_readBufferSize);
        question = String_Get_ByIndex((ushort)(Text.STR_SECURITY_TEXT_HARKONNEN + ((byte)g_playerHouseID * 3)));
        g_readBuffer = SharpDune.Encoding.GetBytes(question);
        GUI_Mentat_Loop(wsaHouseFilename, null, question/*CSharpDune.Encoding.GetString(g_readBuffer)*/, true, null);

        questionsCount = ushort.Parse(String_Get_ByIndex(Text.STR_SECURITY_COUNT), Culture);

        oldCurrentWidget = Widget_SetCurrentWidget(8);

        oldScreenID = GFX_Screen_SetActive(Screen.NO2);

        for (i = 0, valid = false; i < 3 && !valid; i++)
        {
            /* void* */ WSAStream wsa;
            ushort questionIndex;
            uint tickWaitTill;
            string buffer; //char[81];

            questionIndex = (ushort)((Tools_RandomLCG_Range(0, (ushort)(questionsCount - 1)) * 3) + Text.STR_SECURITY_QUESTIONS);

            Widget_SetCurrentWidget(8);

            wsa = WSA_LoadFile(String_Get_ByIndex((ushort)(questionIndex + 1)), GFX_Screen_Get_ByIndex(Screen.NO1), GFX_Screen_GetSize_ByIndex(Screen.NO1), false);
            WSA_DisplayFrame(wsa, 0, (ushort)(g_curWidgetXBase << 3), g_curWidgetYBase, Screen.NO2);
            WSA_Unload(wsa);

            GUI_DrawSprite(Screen.NO2, g_sprites[397 + ((byte)g_playerHouseID * 15)], g_shoulderLeft, g_shoulderTop, 0, 0);

            GUI_Mouse_Hide_InWidget(g_curWidgetIndex);
            GUI_Screen_Copy((short)g_curWidgetXBase, (short)g_curWidgetYBase, (short)g_curWidgetXBase, (short)g_curWidgetYBase, (short)g_curWidgetWidth, (short)g_curWidgetHeight, Screen.NO2, Screen.NO0);
            GUI_Mouse_Show_InWidget();

            question = String_Get_ByIndex(questionIndex);
            g_readBuffer = SharpDune.Encoding.GetBytes(question); //strncpy(g_readBuffer, String_Get_ByIndex(questionIndex), g_readBufferSize);
            GUI_Security_DrawText(question/*CSharpDune.Encoding.GetString(g_readBuffer)*/);

            g_interrogationTimer = g_timerGUI + ((uint)question.Length * 4); //(uint32)strlen(g_readBuffer) * 4

            Widget_SetCurrentWidget(9);

            GUI_Mouse_Hide_Safe();
            GUI_Screen_Copy((short)(g_curWidgetXBase - 1), (short)(g_curWidgetYBase - 8), 0, 0, (short)(g_curWidgetWidth + 2), (short)(g_curWidgetHeight + 16), Screen.NO0, Screen.NO2);
            GUI_Mouse_Show_Safe();

            GFX_Screen_SetActive(Screen.NO0);

            GUI_Mouse_Hide_Safe();
            GUI_DrawBorder((ushort)((g_curWidgetXBase << 3) - 6), (ushort)(g_curWidgetYBase - 6), (ushort)((g_curWidgetWidth << 3) + 12), (ushort)(g_curWidgetHeight + 12), 1, true);
            GUI_DrawBorder((ushort)((g_curWidgetXBase << 3) - 2), (ushort)(g_curWidgetYBase - 2), (ushort)((g_curWidgetWidth << 3) + 4), (ushort)(g_curWidgetHeight + 4), 2, false);
            GUI_Mouse_Show_Safe();

            Input_History_Clear();

            buffer = string.Empty;

            GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x22);

            var savedColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Trace.TraceInformation($"Answer : {String_Get_ByIndex((ushort)(questionIndex + 2))}");
            Console.ForegroundColor = savedColor;

            GUI_EditBox(ref buffer, (ushort)(buffer.Length - 1), 9, null, GUI_Mentat_Tick, false);

            GUI_Security_UndrawText();

            GUI_Mouse_Hide_Safe();
            GUI_Screen_Copy(0, 0, (short)(g_curWidgetXBase - 1), (short)(g_curWidgetYBase - 8), (short)(g_curWidgetWidth + 2), (short)(g_curWidgetHeight + 16), Screen.NO2, Screen.NO0);
            GUI_Mouse_Show_Safe();

            buffer = GUI_Security_NormaliseText(buffer);

            question = String_Get_ByIndex((ushort)(questionIndex + 2));
            g_readBuffer = SharpDune.Encoding.GetBytes(question); //strncpy(g_readBuffer, String_Get_ByIndex(questionIndex + 2), g_readBufferSize);
            question = GUI_Security_NormaliseText(question);

            if (!string.Equals(question/*CSharpDune.Encoding.GetString(g_readBuffer)*/, buffer, StringComparison.OrdinalIgnoreCase))
            { //if (strcasecmp(g_readBuffer, buffer) != 0) {
                g_readBuffer = SharpDune.Encoding.GetBytes(String_Get_ByIndex(Text.STR_SECURITY_WRONG_HARKONNEN + ((byte)g_playerHouseID * 3)));
                //strncpy(g_readBuffer, String_Get_ByIndex(STR_SECURITY_WRONG_HARKONNEN + g_playerHouseID * 3), g_readBufferSize);
            }
            else
            {
                g_readBuffer = SharpDune.Encoding.GetBytes(String_Get_ByIndex(Text.STR_SECURITY_CORRECT_HARKONNEN + ((byte)g_playerHouseID * 3)));
                //strncpy(g_readBuffer, String_Get_ByIndex(STR_SECURITY_CORRECT_HARKONNEN + g_playerHouseID * 3), g_readBufferSize);

                valid = true;
            }

            GUI_Security_DrawText(SharpDune.Encoding.GetString(g_readBuffer));

            tickWaitTill = g_timerGUI + ((uint)SharpDune.Encoding.GetString(g_readBuffer).Length * 4); //(uint32)strlen(g_readBuffer) * 4;

            Input_History_Clear();

            /* ENHANCEMENT -- In Dune2, the + 120 is on the other side, causing the 'You are wrong! / Well done.' screen to appear very short (close to invisible, so to say) */
            while (g_timerGUI + (g_dune2_enhanced ? 0 : 120) < tickWaitTill + (g_dune2_enhanced ? 120 : 0))
            {
                if (Input_Keyboard_NextKey() != 0) break;

                if (g_timerGUI < tickWaitTill)
                {
                    GUI_Mentat_Animation(1);
                }
                else
                {
                    GUI_Mentat_Animation(0);
                }
                SleepIdle();
            }

            GUI_Security_UndrawText();
        }

        Widget_SetCurrentWidget(oldCurrentWidget);

        GFX_Screen_SetActive(oldScreenID);

        Input_History_Clear();

        Load_Palette_Mercenaries();

        g_disableOtherMovement = false;
        g_interrogation = false;

        return valid;
    }

    static void GUI_Security_DrawText(string text)
    {
        var oldScreenID = GFX_Screen_SetActive(Screen.NO2);

        GUI_Mouse_Hide_InRegion(0, 0, SCREEN_WIDTH, 40);
        GUI_Screen_Copy(0, 0, 0, 0, SCREEN_WIDTH / 8, 40, Screen.NO0, Screen.NO2);
        GUI_Mouse_Show_InRegion();

        GUI_Screen_Copy(0, 0, 0, 160, SCREEN_WIDTH / 8, 40, Screen.NO2, Screen.NO2);

        var textLines = GUI_Mentat_SplitText(text, 304);

        GUI_DrawText_Wrapper(textLines[0], 4, 1, g_curWidgetFGColourBlink, 0, 0x32);

        GUI_Mouse_Hide_InRegion(0, 0, SCREEN_WIDTH, 40);
        GUI_Screen_Copy(0, 0, 0, 0, SCREEN_WIDTH / 8, 40, Screen.NO2, Screen.NO0);
        GUI_Mouse_Show_InRegion();

        GFX_Screen_SetActive(oldScreenID);
    }

    static void GUI_Security_UndrawText()
    {
        GUI_Mouse_Hide_Safe();
        GUI_Screen_Copy(0, 160, 0, 0, SCREEN_WIDTH / 8, 40, Screen.NO2, Screen.NO0);
        GUI_Mouse_Show_Safe();
    }

    static string GUI_Security_NormaliseText(string str) =>
        str.ToUpper(Culture);
}
