/* Security */

using System;
using System.Diagnostics;

namespace SharpDune.Gui
{
	class Security
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

			Mentat.g_disableOtherMovement = true;
			Mentat.g_interrogation = true;

			wsaHouseFilename = CHouse.House_GetWSAHouseFilename((byte)CHouse.g_playerHouseID);
			if (wsaHouseFilename == null) return true;

			Gui.GUI_SetPaletteAnimated(Gfx.g_palette2, 15);

			Mentat.GUI_Mentat_Display(wsaHouseFilename, (byte)CHouse.g_playerHouseID);

			Gui.GUI_Mouse_Hide_Safe();
			Gui.GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, Screen.NO1, Screen.NO0);
			Gui.GUI_Mouse_Show_Safe();

			Gui.GUI_SetPaletteAnimated(Gfx.g_palette1, 15);

			//strncpy(g_readBuffer, String_Get_ByIndex(STR_SECURITY_TEXT_HARKONNEN + g_playerHouseID * 3), g_readBufferSize);
			question = CString.String_Get_ByIndex((ushort)(Text.STR_SECURITY_TEXT_HARKONNEN + (byte)CHouse.g_playerHouseID * 3));
			CSharpDune.g_readBuffer = CSharpDune.Encoding.GetBytes(question);
			Mentat.GUI_Mentat_Loop(wsaHouseFilename, null, question/*CSharpDune.Encoding.GetString(CSharpDune.g_readBuffer)*/, true, null);

			questionsCount = ushort.Parse(CString.String_Get_ByIndex(Text.STR_SECURITY_COUNT), CSharpDune.Culture);

			oldCurrentWidget = CWidget.Widget_SetCurrentWidget(8);

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO2);

			for (i = 0, valid = false; i < 3 && !valid; i++)
			{
				/* void * *//*WSAObject*/
				(WSAHeader header, CArray<byte> buffer) wsa;
				ushort questionIndex;
				uint tickWaitTill;
				string buffer; //char[81];

				questionIndex = (ushort)(Tools.Tools_RandomLCG_Range(0, (ushort)(questionsCount - 1)) * 3 + Text.STR_SECURITY_QUESTIONS);

				CWidget.Widget_SetCurrentWidget(8);

				wsa = Wsa.WSA_LoadFile(CString.String_Get_ByIndex((ushort)(questionIndex + 1)), Gfx.GFX_Screen_Get_ByIndex(Screen.NO1), Gfx.GFX_Screen_GetSize_ByIndex(Screen.NO1), false);
				Wsa.WSA_DisplayFrame(wsa, 0, (ushort)(CWidget.g_curWidgetXBase << 3), CWidget.g_curWidgetYBase, Screen.NO2);
				Wsa.WSA_Unload(wsa);

				Gui.GUI_DrawSprite(Screen.NO2, Sprites.g_sprites[397 + (byte)CHouse.g_playerHouseID * 15], Mentat.g_shoulderLeft, Mentat.g_shoulderTop, 0, 0);

				Gui.GUI_Mouse_Hide_InWidget(CWidget.g_curWidgetIndex);
				Gui.GUI_Screen_Copy((short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetWidth, (short)CWidget.g_curWidgetHeight, Screen.NO2, Screen.NO0);
				Gui.GUI_Mouse_Show_InWidget();

				question = CString.String_Get_ByIndex(questionIndex);
				CSharpDune.g_readBuffer = CSharpDune.Encoding.GetBytes(question); //strncpy(g_readBuffer, String_Get_ByIndex(questionIndex), g_readBufferSize);
				GUI_Security_DrawText(question/*CSharpDune.Encoding.GetString(CSharpDune.g_readBuffer)*/);

				Mentat.g_interrogationTimer = Timer.g_timerGUI + (uint)question.Length * 4; //(uint32)strlen(g_readBuffer) * 4

				CWidget.Widget_SetCurrentWidget(9);

				Gui.GUI_Mouse_Hide_Safe();
				Gui.GUI_Screen_Copy((short)(CWidget.g_curWidgetXBase - 1), (short)(CWidget.g_curWidgetYBase - 8), 0, 0, (short)(CWidget.g_curWidgetWidth + 2), (short)(CWidget.g_curWidgetHeight + 16), Screen.NO0, Screen.NO2);
				Gui.GUI_Mouse_Show_Safe();

				Gfx.GFX_Screen_SetActive(Screen.NO0);

				Gui.GUI_Mouse_Hide_Safe();
				Gui.GUI_DrawBorder((ushort)((CWidget.g_curWidgetXBase << 3) - 6), (ushort)(CWidget.g_curWidgetYBase - 6), (ushort)((CWidget.g_curWidgetWidth << 3) + 12), (ushort)(CWidget.g_curWidgetHeight + 12), 1, true);
				Gui.GUI_DrawBorder((ushort)((CWidget.g_curWidgetXBase << 3) - 2), (ushort)(CWidget.g_curWidgetYBase - 2), (ushort)((CWidget.g_curWidgetWidth << 3) + 4), (ushort)(CWidget.g_curWidgetHeight + 4), 2, false);
				Gui.GUI_Mouse_Show_Safe();

				Input.Input_History_Clear();

				buffer = string.Empty;

				Gui.GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x22);

				var savedColor = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Red;
				Trace.WriteLine($"Answer : {CString.String_Get_ByIndex((ushort)(questionIndex + 2))}");
				Console.ForegroundColor = savedColor;

				EditBox.GUI_EditBox(ref buffer, (ushort)(buffer.Length - 1), 9, null, Mentat.GUI_Mentat_Tick, false);

				GUI_Security_UndrawText();

				Gui.GUI_Mouse_Hide_Safe();
				Gui.GUI_Screen_Copy(0, 0, (short)(CWidget.g_curWidgetXBase - 1), (short)(CWidget.g_curWidgetYBase - 8), (short)(CWidget.g_curWidgetWidth + 2), (short)(CWidget.g_curWidgetHeight + 16), Screen.NO2, Screen.NO0);
				Gui.GUI_Mouse_Show_Safe();

				GUI_Security_NormaliseText(ref buffer);

				question = CString.String_Get_ByIndex((ushort)(questionIndex + 2));
				CSharpDune.g_readBuffer = CSharpDune.Encoding.GetBytes(question); //strncpy(g_readBuffer, String_Get_ByIndex(questionIndex + 2), g_readBufferSize);
				GUI_Security_NormaliseText(ref question);

				if (!string.Equals(question/*CSharpDune.Encoding.GetString(CSharpDune.g_readBuffer)*/, buffer, StringComparison.OrdinalIgnoreCase))
				{ //if (strcasecmp(g_readBuffer, buffer) != 0) {
					CSharpDune.g_readBuffer = CSharpDune.Encoding.GetBytes(CString.String_Get_ByIndex(Text.STR_SECURITY_WRONG_HARKONNEN + (byte)CHouse.g_playerHouseID * 3));
					//strncpy(g_readBuffer, String_Get_ByIndex(STR_SECURITY_WRONG_HARKONNEN + g_playerHouseID * 3), g_readBufferSize);
				}
				else
				{
					CSharpDune.g_readBuffer = CSharpDune.Encoding.GetBytes(CString.String_Get_ByIndex(Text.STR_SECURITY_CORRECT_HARKONNEN + (byte)CHouse.g_playerHouseID * 3));
					//strncpy(g_readBuffer, String_Get_ByIndex(STR_SECURITY_CORRECT_HARKONNEN + g_playerHouseID * 3), g_readBufferSize);

					valid = true;
				}

				GUI_Security_DrawText(CSharpDune.Encoding.GetString(CSharpDune.g_readBuffer));

				tickWaitTill = Timer.g_timerGUI + (uint)CSharpDune.Encoding.GetString(CSharpDune.g_readBuffer).Length * 4; //(uint32)strlen(g_readBuffer) * 4;

				Input.Input_History_Clear();

				/* ENHANCEMENT -- In Dune2, the + 120 is on the other side, causing the 'You are wrong! / Well done.' screen to appear very short (close to invisible, so to say) */
				while (Timer.g_timerGUI + (CSharpDune.g_dune2_enhanced ? 0 : 120) < tickWaitTill + (CSharpDune.g_dune2_enhanced ? 120 : 0))
				{
					if (Input.Input_Keyboard_NextKey() != 0) break;

					if (Timer.g_timerGUI < tickWaitTill)
					{
						Mentat.GUI_Mentat_Animation(1);
					}
					else
					{
						Mentat.GUI_Mentat_Animation(0);
					}
					Sleep.sleepIdle();
				}

				GUI_Security_UndrawText();
			}

			CWidget.Widget_SetCurrentWidget(oldCurrentWidget);

			Gfx.GFX_Screen_SetActive(oldScreenID);

			Input.Input_History_Clear();

			Load.Load_Palette_Mercenaries();

			Mentat.g_disableOtherMovement = false;
			Mentat.g_interrogation = false;

			return valid;
		}

		static void GUI_Security_DrawText(string text)
		{
			Screen oldScreenID;

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO2);

			Gui.GUI_Mouse_Hide_InRegion(0, 0, Gfx.SCREEN_WIDTH, 40);
			Gui.GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, 40, Screen.NO0, Screen.NO2);
			Gui.GUI_Mouse_Show_InRegion();

			Gui.GUI_Screen_Copy(0, 0, 0, 160, Gfx.SCREEN_WIDTH / 8, 40, Screen.NO2, Screen.NO2);

			var parts/*(parts, _)*/ = Mentat.GUI_Mentat_SplitText(text, 304);

			Gui.GUI_DrawText_Wrapper(parts[0], 4, 1, CWidget.g_curWidgetFGColourBlink, 0, 0x32);

			Gui.GUI_Mouse_Hide_InRegion(0, 0, Gfx.SCREEN_WIDTH, 40);
			Gui.GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, 40, Screen.NO2, Screen.NO0);
			Gui.GUI_Mouse_Show_InRegion();

			Gfx.GFX_Screen_SetActive(oldScreenID);
		}

		static void GUI_Security_UndrawText()
		{
			Gui.GUI_Mouse_Hide_Safe();
			Gui.GUI_Screen_Copy(0, 160, 0, 0, Gfx.SCREEN_WIDTH / 8, 40, Screen.NO2, Screen.NO0);
			Gui.GUI_Mouse_Show_Safe();
		}

		static void GUI_Security_NormaliseText(ref string str)
		{
			var s = str.ToCharArray();

			for (var i = 0; i < s.Length; i++)
				if (char.IsLetterOrDigit(s[i]) && char.IsLower(s[i]))
					s[i] = char.ToUpper(s[i], CSharpDune.Culture);

			str = new string(s);
		}
	}
}
