/* Editbox */

using System;
using System.Text;

namespace SharpDune
{
	class EditBox
	{
		static uint tickEditBox = 0;           /* Ticker for cursor blinking. */
		static bool editBoxShowCursor = false; /* Cursor is active. */
		/*
		 * Draw a blinking cursor, used inside the EditBox.
		 *
		 * @param positionX Where to draw the cursor on the X position.
		 * @param resetBlink If true, the blinking is reset and restarted.
		 */
		static void GUI_EditBox_BlinkCursor(ushort positionX, bool resetBlink)
		{
			if (resetBlink)
			{
				tickEditBox = 0;
				editBoxShowCursor = true;
			}

			if (tickEditBox > Timer.g_timerGUI) return;
			if (!resetBlink)
			{
				tickEditBox = Timer.g_timerGUI + 20;
			}

			editBoxShowCursor = !editBoxShowCursor;

			Gui.GUI_Mouse_Hide_Safe();
			Gui.GUI_DrawFilledRectangle((short)positionX, (short)CWidget.g_curWidgetYBase, (short)(positionX + CFont.Font_GetCharWidth('W')),
				(short)(CWidget.g_curWidgetYBase + CWidget.g_curWidgetHeight - 1), editBoxShowCursor ? CWidget.g_curWidgetFGColourBlink : CWidget.g_curWidgetFGColourNormal);
			Gui.GUI_Mouse_Show_Safe();
		}

		/*
		 * Show an EditBox and handles the input.
		 * @param text The text to edit. Uses the pointer to make the modifications.
		 * @param maxLength The maximum length of the text.
		 * @param widgetID the widget in which the EditBox is displayed.
		 * @param w The widget this editbox is attached to (for input events).
		 * @param tickProc The function to call every tick, for animation etc.
		 * @param paint Flag indicating if the widget need to be repainted.
		 * @return Key code / Button press code.
		 */
		internal static ushort GUI_EditBox(ref string text, ushort maxLength, ushort widgetID, Widget w, Func<ushort> tickProc, bool paint)
		{
			Screen oldScreenID;
			ushort oldWidgetID;
			ushort positionX;
			ushort maxWidth;
			ushort textWidth;
			ushort textLength;
			ushort returnValue;
			StringBuilder t = new StringBuilder();

			/* Initialize */
			{
				Input.Input_Flags_SetBits((ushort)InputFlagsEnum.INPUT_FLAG_NO_TRANSLATE);
				Input.Input_Flags_ClearBits((ushort)InputFlagsEnum.INPUT_FLAG_UNKNOWN_2000);

				oldScreenID = Gfx.GFX_Screen_SetActive(Screen.SCREEN_0);

				oldWidgetID = CWidget.Widget_SetCurrentWidget(widgetID);

				returnValue = 0x0;
			}

			positionX = (ushort)(CWidget.g_curWidgetXBase << 3);

			textWidth = 0;
			textLength = 0;
			maxWidth = (ushort)((CWidget.g_curWidgetWidth << 3) - CFont.Font_GetCharWidth('W') - 1);
			t.Append(text);

			/* Calculate the length and width of the current string */
			for (int i = 0; i < t.Length; i++)
			{
				textWidth += CFont.Font_GetCharWidth(t[i]);
				textLength++;

				if (textWidth >= maxWidth) break;
			}
			//*t = '\0';

			Gui.GUI_Mouse_Hide_Safe();

			if (paint) CWidget.Widget_PaintCurrentWidget();

			Gui.GUI_DrawText_Wrapper(text, (short)positionX, (short)CWidget.g_curWidgetYBase, CWidget.g_curWidgetFGColourBlink, CWidget.g_curWidgetFGColourNormal, 0);

			GUI_EditBox_BlinkCursor((ushort)(positionX + textWidth), false);

			Gui.GUI_Mouse_Show_Safe();

			for (; ; Sleep.sleepIdle())
			{
				ushort keyWidth;
				ushort key;

				if (tickProc != null)
				{
					returnValue = tickProc();
					if (returnValue != 0) break;
				}

				key = CWidget.GUI_Widget_HandleEvents(w);

				GUI_EditBox_BlinkCursor((ushort)(positionX + textWidth), false);

				if (key == 0x0) continue;

				if ((key & 0x8000) != 0)
				{
					returnValue = key;
					break;
				}
				if (key == 0x2B)
				{
					returnValue = 0x2B;
					break;
				}
				if (key == 0x6E)
				{
					//*t = '\0';
					returnValue = 0x6B;
					break;
				}

				/* Handle backspace */
				if (key == 0x0F)
				{
					if (textLength == 0) continue;

					GUI_EditBox_BlinkCursor((ushort)(positionX + textWidth), true);

					textWidth -= CFont.Font_GetCharWidth(t[^1]);
					textLength--;
					t.Remove(t.Length - 1, 1);
					//*(--t) = '\0';

					GUI_EditBox_BlinkCursor((ushort)(positionX + textWidth), false);
					continue;
				}

				key = (ushort)(Input.Input_Keyboard_HandleKeys(key) & 0xFF);

				/* Names can't start with a space, and should be alpha-numeric */
				if ((key == 0x20 && textLength == 0) || key < 0x20 || key > 0x7E) continue;

				keyWidth = CFont.Font_GetCharWidth((char)(key & 0xFF));

				if (textWidth + keyWidth >= maxWidth || textLength >= maxLength) continue;

				/* Add char to the text */
				t.Append((char)(key & 0xFF));
				//*(++t) = '\0';

				textLength++;

				Gui.GUI_Mouse_Hide_Safe();

				GUI_EditBox_BlinkCursor((ushort)(positionX + textWidth), true);

				/* Draw new character */
				Gui.GUI_DrawText_Wrapper(/*text + textLength - 1*/t[^1].ToString(), (short)(positionX + textWidth), (short)CWidget.g_curWidgetYBase, CWidget.g_curWidgetFGColourBlink, CWidget.g_curWidgetFGColourNormal, 0x020);

				Gui.GUI_Mouse_Show_Safe();

				textWidth += keyWidth;

				GUI_EditBox_BlinkCursor((ushort)(positionX + textWidth), false);
			}

			/* Deinitialize */
			{
				Input.Input_Flags_ClearBits((ushort)InputFlagsEnum.INPUT_FLAG_NO_TRANSLATE);
				Input.Input_Flags_SetBits((ushort)InputFlagsEnum.INPUT_FLAG_UNKNOWN_2000);

				CWidget.Widget_SetCurrentWidget(oldWidgetID);

				Gfx.GFX_Screen_SetActive(oldScreenID);
			}

			text = t.ToString();

			return returnValue;
		}
	}
}
