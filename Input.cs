/* Input */

using System;
using System.Diagnostics;

namespace SharpDune
{
	/*
	 * There are three different mouse modes.
	 * It looks like only the first (normal) mode is ever used.
	 */
	enum InputMouseMode
	{
		INPUT_MOUSE_MODE_NORMAL = 0,                          /*!< Normal mouse mode. */
		INPUT_MOUSE_MODE_RECORD = 1,                          /*!< Record mouse events to a file. */
		INPUT_MOUSE_MODE_PLAY = 2                             /*!< Plays mouse events from a file. */
	}

	/*
	 * Several flags for input handling.
	 */
	[Flags]
	enum InputFlagsEnum
	{
		INPUT_FLAG_KEY_REPEAT = 0x0001,                       /*!< Allow repeated input of the same key. */
		INPUT_FLAG_NO_TRANSLATE = 0x0002,                     /*!< Don't translate a key. */
		INPUT_FLAG_UNKNOWN_0004 = 0x0004,                     /*!< ?? */
		INPUT_FLAG_UNKNOWN_0008 = 0x0008,                     /*!< ?? */
		INPUT_FLAG_UNKNOWN_0010 = 0x0010,                     /*!< ?? */
		INPUT_FLAG_UNKNOWN_0020 = 0x0020,                     /*!< ?? */
		INPUT_FLAG_UNKNOWN_0040 = 0x0040,                     /*!< ?? */
		INPUT_FLAG_UNKNOWN_0080 = 0x0080,                     /*!< ?? */
		INPUT_FLAG_UNKNOWN_0100 = 0x0100,                     /*!< ?? */
		INPUT_FLAG_UNKNOWN_0200 = 0x0200,                     /*!< ?? */
		INPUT_FLAG_UNKNOWN_0400 = 0x0400,                     /*!< ?? */
		INPUT_FLAG_KEY_RELEASE = 0x0800,                      /*!< Record release of keys (not for buttons). */
		INPUT_FLAG_NO_CLICK = 0x1000,                         /*!< Don't record mouse button clicks. */
		INPUT_FLAG_UNKNOWN_2000 = 0x2000                      /*!< ?? */
	}

	class Input
	{
		static ushort[] s_history = new ushort[128];                /*!< History of input commands. */
		static ushort s_historyHead = 0;                            /*!< The current head inside the #s_history array. */
		static ushort s_historyTail = 0;                            /*!< The current tail inside the #s_history array. */
		static bool s_input_extendedKey;                            /*!< If we are currently actively reading an extended key. */
		static byte[] s_activeInputMap = new byte[16];              /*!< A 96 bit array, where each active bit means that the Nth key is pressed. */

		/* Dune II key codes :
		 * 0x00        : invalid
		 * 0x01 - 0x37 : ASCII characters (except 0x0e, 0x1e, 0x2a, 0x2c)
		 * 0x1e        : CAPS LOCK
		 * 0x2b        : ENTER
		 * 0x2c        : LSHIFT
		 * 0x2d        : MOUSE MOVE
		 * 0x39        : RSHIFT
		 * 0x3a        : LCTRL
		 * 0x3c        : LALT
		 * 0x3d        : SPACE (printable)
		 * 0x3e        : RALT
		 * 0x40        : RCTRL
		 * 0x41        : LEFT MOUSE BUTTON
		 * 0x42        : RIGHT MOUSE BUTTON
		 * 0x4b - 0x56 : INSERT DELETE LEFT HOME END UP DOWN PGUP PGDOWN RIGHT
		 * 0x59        : KP / (?)
		 * 0x5a - 0x5d : NUMLOCK KP 7 4 1
		 * 0x5f - 0x6a : KP / 8 5 2 0 * 9 6 3 . - +
		 * 0x6c        : KP ENTER
		 * 0x6e        : ESCAPE
		 * 0x70 - 0x7b : F1-F12
		 * 0x7d        : SCROLL LOCK
		 */

		/* Key translation table.
		 * From XT/AT scancodes to Dune II codes :
		 * 0x00 => 0x7f
		 * 0x01 => 0x6e            ESC
		 * [0x02:0x0d] => n        1 2 3 4 5 6 7 8 9 0 - =
		 * [0x0e:0x1b] => n+1      BACKSPACE TAB q w e r t y u i o p [ ]
		 * 0x1c => 0x2b            ENTER
		 * 0x1d => 0x3a            LCTRL
		 * [0x1e:0x28] => n+1      a s d f g h j k l ; '
		 * 0x29 => 0x01            ~' (key left to 1)
		 * 0x2a => 0x2c            LSHIFT
		 * 0x2b => 0x1d            | (key left to Z)
		 * [0x2c:0x35] => n+2      z x c v b n m , . /
		 * 0x36 => 0x39            RSHIFT
		 * 0x37 => 0x64            KP *
		 * 0x38 => 0x3c            LALT
		 * 0x39 => 0x3d            SPACE
		 * 0x3a => 0x1e            CAPS LOCK
		 * [0x3b:0x44] => [0x70:0x79] (n+0x35) F1-F10
		 * 0x45 => 0x5a            NUMLOCK
		 * 0x46 => 0x7d            SCROLL LOCK
		 * 0x47 => 0x5b            KP 7 HOME
		 * 0x48 => 0x60            KP 8 UP
		 * 0x49 => 0x65            KP 9 PGUP
		 * 0x4a => 0x69            KP -
		 * 0x4b => 0x5c            KP 4 LEFT
		 * 0x4c => 0x61            KP 5
		 * 0x4d => 0x66            KP 6 RIGHT
		 * 0x4e => 0x6a            KP +
		 * 0x4f => 0x5d            KP 1 END
		 * 0x50 => 0x62            KP 2 DOWN
		 * 0x51 => 0x67            KP 3 PGDN
		 * 0x52 => 0x63            KP 0 INS
		 * 0x53 => 0x68            KP . DEL
		 * [0x54:0x56] => 0x7f
		 * 0x57 => 0x7a            F11
		 * 0x58 => 0x7b            F12
		 */
		static byte[] s_keyTranslate = {
			127, (byte)'n', 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, /* 0x00 - 0x0f */
			17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, (byte)'+', (byte)':', 31, (byte)' ', /* 0x10 - 0x1f */
			(byte)'!', (byte)'"', (byte)'#', (byte)'$', (byte)'%', (byte)'&', (byte)'\'', (byte)'(', (byte)')', 1, (byte)',', 29, (byte)'.', (byte)'/', (byte)'0', (byte)'1', /* 0x20 - 0x2f */
			(byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'9', (byte)'d', (byte)'<', (byte)'=', 30, (byte)'p', (byte)'q', (byte)'r', (byte)'s', (byte)'t', /* 0x30 - 0x3f */
			(byte)'u', (byte)'v', (byte)'w', (byte)'x', (byte)'y', (byte)'Z', (byte)'}', (byte)'[', (byte)'`', (byte)'e', (byte)'i', (byte)'\\', (byte)'a', (byte)'f', (byte)'j', (byte)']', /* 0x40 - 0x4f */
			(byte)'b', (byte)'g', (byte)'c', (byte)'h', 127, 127, 127, (byte)'z', (byte)'{' /* 0x50 - 0x58 */
		};

		static byte[] s_keymapIgnore = { 30, (byte)',', (byte)'9', (byte)':', (byte)'<', (byte)'>', (byte)'@', (byte)'Z', 128 }; /*!< Keys to ignore when reading. */
		/* Dune II codes                    0x1e, 0x2c, 0x39, 0x3a, 0x3c, 0x3e, 0x40, 0x5a, 0x80 */
		/*                                  CAPS  LSHFT RSHFT LCTRL LALT  RALT  RCTRL NUMLK */

		/* Per bit, mask which keys are letters and special and should be done &= 0x1F when ALT is pressed (or CTLR ?) */
		static byte[] s_keymapSpecialMask = { 0x00, 0x00, 0xFE, 0x87, 0xFF, 0xC0, 0x1F, 0x00 };
		/* 0x11-0x1b : qwertyuiop
		 * 0x1f-0x27 : asdfghjkl
		 * 0x2e-0x34 : zxcvbnm
		 */

		/* Keymap to convert Dune II codes to ASCII with capslock off and shift released. */
		static byte[] s_keymapNormal = {
			0, (byte)'`', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'0', (byte)'-', (byte)'=', 0, 8, /* 0x00 - 0x0f */
			(byte)'\t', (byte)'q', (byte)'w', (byte)'e', (byte)'r', (byte)'t', (byte)'y', (byte)'u', (byte)'i', (byte)'o', (byte)'p', (byte)'[', (byte)']', (byte)'\\', 0, (byte)'a', /* 0x10 - 0x1f */
			(byte)'s', (byte)'d', (byte)'f', (byte)'g', (byte)'h', (byte)'j', (byte)'k', (byte)'l', (byte)';', (byte)'\'', 0, (byte)'\r', 0, (byte)'-', (byte)'z', (byte)'x', /* 0x20 - 0x2f */
			(byte)'c', (byte)'v', (byte)'b', (byte)'n', (byte)'m', (byte)',', (byte)'.', (byte)'/', 0, 0, 0, 0, 0, (byte)' ' /* 0x30 - 0x3d */
		};

		/* Keymap to convert Dune II codes to ASCII with capslock off and shift pressed. */
		static byte[] s_keymapShift = {
			0, (byte)'~', (byte)'!', (byte)'@', (byte)'#', (byte)'$', (byte)'%', (byte)'^', (byte)'&', (byte)'*', (byte)'(', (byte)')', (byte)'_', (byte)'+', 0, 8,
			(byte)'\t', (byte)'Q', (byte)'W', (byte)'E', (byte)'R', (byte)'T', (byte)'Y', (byte)'U', (byte)'I', (byte)'O', (byte)'P', (byte)'{', (byte)'}', (byte)'|', 0, (byte)'A',
			(byte)'S', (byte)'D', (byte)'F', (byte)'G', (byte)'H', (byte)'J', (byte)'K', (byte)'L', (byte)':', (byte)'"', 0, (byte)'\r', 0, (byte)'-', (byte)'Z', (byte)'X',
			(byte)'C', (byte)'V', (byte)'B', (byte)'N', (byte)'M', (byte)'<', (byte)'>', (byte)'?', 0, 0, 0, 0, 0, (byte)' '
		};

		/* Keymap to convert scancode to for numpad with numlock on. */
		static byte[] s_keymapNumlock = {0, (byte)'7', (byte)'4', (byte)'1', 0, (byte)'/', (byte)'8', (byte)'5', (byte)'2', (byte)'0', (byte)'*', (byte)'9', (byte)'6', (byte)'3',
			(byte)'.', (byte)'-', (byte)'+', 0, (byte)'\r', 0};

		/* Some kind of translation map. */
		static byte[] s_translateMap = { (byte)'>', (byte)'@', (byte)'K', (byte)'L', (byte)'O', (byte)'P', (byte)'Q', (byte)'S', (byte)'T', (byte)'U', (byte)'V', (byte)'Y', (byte)'_', (byte)'l', (byte)'|', 0 };
		/* DuneII codes :  0x3e 0x40 0x4b 0x4c  0x4f 0x50 0x51 0x53  0x54 0x55 0x56 0x59  0x5f 0x6c 0x7c 0x00 */
		/*                 RALT RCTL                                                      KP / KPENTER        */

		/* when escaped with 0xE0, scancodes in s_translateExtendedMap are translated to s_translateMap */
		/* Some kind of translation map for extended keys. */
		static byte[] s_translateExtendedMap = { (byte)'8', 29, (byte)'R', (byte)'S', (byte)'K', (byte)'G', (byte)'O', (byte)'H', (byte)'P', (byte)'I', (byte)'Q', (byte)'M', (byte)'5', 28, (byte)'7', (byte)'F' };
		/* scancodes : 0x38, 0x1d, 0x52, 0x53,  0x4b, 0x47, 0x4f, 0x48,  0x50, 0x49, 0x51, 0x4d,  0x35, 0x1c, 0x37, 0x46 */
		/* 0xE0 keys : RALT RCTRL  INSRT DEL    LEFT  7HOME 1END  8UP    DOWN  9PGUP 3PGDN 6RGHT  KP/  ENTR *PRNSCN SCLOCK */

		/* To what a match in #s_translateMap translates. */
		static byte[] s_translateTo = { (byte)'<', (byte)':', (byte)'c', (byte)'h', (byte)'\\', (byte)'[', (byte)']', (byte)'`', (byte)'b', (byte)'e', (byte)'g', (byte)'f', (byte)'7', (byte)'+', (byte)'|', 0 };
		/* DuneII codes :  0x3c 0x3a 0x63 0x68  0x5c 0x5b 0x5d 0x60  0x62 0x65 0x67 0x66  0x37 0x2b 0x7c 0x00 */
		/*                 LALT LCTL KP 0 KP .  KP 4 KP 7 KP 1 KP 8  KP 2 KP 9 KP 3 KP 6   /  ENTER           */

		/* Clear the history buffer. */
		internal static void Input_History_Clear() => s_historyTail = s_historyHead;

		/*
		 * Wait for valid input.
		 * @return Read input. (ASCII VALUE or > 0x80)
		 */
		internal static ushort Input_WaitForValidInput()
		{
			ushort index = 0;
			ushort value, i;

			do
			{
				for (; ; Sleep.sleepIdle())
				{
					if (Mouse.g_mouseMode == (byte)InputMouseMode.INPUT_MOUSE_MODE_PLAY) break;

					index = s_historyHead;
					if (index != s_historyTail) break;
				}

				value = Input_ReadHistory(index);
				for (i = 0; i < s_keymapIgnore.Length; i++)
				{
					if ((value & 0xFF) == s_keymapIgnore[i]) break;
				}
			} while (i < s_keymapIgnore.Length || (value & 0x800) != 0 || (value & 0xFF) >= 0x7A);

			value = Input_Keyboard_HandleKeys(value);
			Input_ReadInputFromFile();
			return (ushort)(value & 0xFF);
		}

		/*
		 * Get an input from the history buffer.
		 * @param index Current index in the history.
		 * @return Read input.
		 * @note Provided \a index gets updated and written to #historyHead
		 */
		static ushort Input_ReadHistory(ushort index)
		{
			ushort value;

			if (Mouse.g_mouseMode != (byte)InputMouseMode.INPUT_MOUSE_MODE_PLAY) Mouse.g_mouseInputValue = s_history[index / 2];
			value = Mouse.g_mouseInputValue;
			index = (ushort)((index + 2) & 0xFF);

			if ((value & 0xFF) >= 0x41)
			{
				if ((value & 0xFF) <= 0x42)
				{
					if (Mouse.g_mouseMode != (byte)InputMouseMode.INPUT_MOUSE_MODE_PLAY) Mouse.g_mouseRecordedX = s_history[index / 2];
					Mouse.g_mouseClickX = Mouse.g_mouseRecordedX;
					index = (ushort)((index + 2) & 0xFF);

					if (Mouse.g_mouseMode != (byte)InputMouseMode.INPUT_MOUSE_MODE_PLAY) Mouse.g_mouseRecordedY = s_history[index / 2];
					Mouse.g_mouseClickY = Mouse.g_mouseRecordedY;
					index = (ushort)((index + 2) & 0xFF);
				}
				else if ((value & 0xFF) <= 0x44)
				{
					if (Mouse.g_mouseMode != (byte)InputMouseMode.INPUT_MOUSE_MODE_PLAY) Mouse.g_mouseRecordedX = s_history[index / 2];
					index = (ushort)((index + 2) & 0xFF);

					if (Mouse.g_mouseMode != (byte)InputMouseMode.INPUT_MOUSE_MODE_PLAY) Mouse.g_mouseRecordedY = s_history[index / 2];
					index = (ushort)((index + 2) & 0xFF);
				}
			}
			if (Mouse.g_mouseMode != (byte)InputMouseMode.INPUT_MOUSE_MODE_PLAY) s_historyHead = index;
			return value;
		}

		/* Read input event from file. */
		static void Input_ReadInputFromFile()
		{
			ushort value;
			ushort[] mouseBuffer = new ushort[2];

			if (Mouse.g_mouseMode == (byte)InputMouseMode.INPUT_MOUSE_MODE_NORMAL || Mouse.g_mouseMode != (byte)InputMouseMode.INPUT_MOUSE_MODE_PLAY) return;

			byte[] byteMouseBuffer = Array.ConvertAll(mouseBuffer, x => (byte)x);

			if (CFile.File_Read(Mouse.g_mouseFileID, ref byteMouseBuffer, 4) != 4)
			{
				Trace.WriteLine("WARNING: Input_ReadInputFromFile(): File_Read() error.");
				return;
			}
			Debug.WriteLine($"DEBUG:  time={mouseBuffer[1]} value={mouseBuffer[0]}");

			Mouse.g_mouseRecordedTimer = mouseBuffer[1];
			value = Mouse.g_mouseInputValue = mouseBuffer[0];

			if ((value & 0xFF) != 0x2D)
			{
				byte idx, bit;

				idx = (byte)((value & 0xFF) >> 3);
				bit = (byte)(1 << (value & 7));

				s_activeInputMap[idx] &= (byte)~bit;
				if ((value & 0x800) == 0) s_activeInputMap[idx] |= bit;

				if ((value & 0xFF) < 0x41 || (value & 0xFF) > 0x44)
				{
					Timer.g_timerInput = 0;
					return;
				}

				value -= 0x41;
				if ((value & 0xFF) <= 0x2)
				{
					Mouse.g_prevButtonState &= (byte)~(1 << (value & 0xFF));
					Mouse.g_prevButtonState |= (byte)((((value & 0x800) >> (3 + 8)) ^ 1) << (value & 0xFF));
				}
			}

			if (CFile.File_Read(Mouse.g_mouseFileID, ref byteMouseBuffer, 4) != 4)
			{
				Trace.WriteLine("WARNING: Input_ReadInputFromFile(): File_Read() error.");
				return;
			}
			Debug.WriteLine($"DEBUG:  mouseX={mouseBuffer[0]} mouseY={mouseBuffer[1]}");

			Mouse.g_mouseX = Mouse.g_mouseRecordedX = mouseBuffer[0];
			value = Mouse.g_mouseY = Mouse.g_mouseRecordedY = mouseBuffer[1];

			Mouse.Mouse_HandleMovementIfMoved(value);
			Timer.g_timerInput = 0;
		}

		/*
		 * Handle keyboard input.
		 * @param value Combined keycode and modifier flags.
		 * @return key value and modifier flags. (ASCII value or > 0x80)
		 * @todo Most users seem to ignore the returned high byte, perhaps make it a
		 *      uint8 at some time in the future?
		 */
		internal static ushort Input_Keyboard_HandleKeys(ushort value)
		{
			ushort keyFlags;

			keyFlags = (ushort)(value & 0xFF00);
			value &= 0x00FF;

			if ((keyFlags & 0x8000) != 0 || (keyFlags & 0x800) != 0)
			{
				return 0;   /* return on key up */
			}

			if (value == 0x6E)
			{
				return (ushort)(keyFlags | 0x1B);   /* ESCAPE */
			}

			if (value < 0x3E)
			{   /* Printables Chars */
				byte keySave = (byte)(value & 0x3F);
				ushort asciiValue;

				if ((keyFlags & 0x100) != 0)
				{
					asciiValue = s_keymapShift[keySave];
				}
				else
				{
					asciiValue = s_keymapNormal[keySave];
				}

				if ((keyFlags & 0x200) != 0)
				{   /* ALT */
					if ((s_keymapSpecialMask[keySave >> 3] & (1 << (keySave & 7))) != 0)
					{
						asciiValue &= 0x1F;
					}
				}
				return (ushort)(keyFlags | asciiValue);
			}

			if (value < 0x4B)
			{
				/* 0x3E - 0x4A : unused codes ? */
				if (value >= 0x41)
				{
					return (ushort)(keyFlags | (value + 0x85)); /* 0x41-0x4A => 0xC6-0xCF */
				}
				return (ushort)(keyFlags | value | 0x80);   /* 0x3E 0x3F 0x40 => 0xBE 0xBF 0xC0 */
			}

			if (value < 0x6E)
			{
				/* 0x4B - 0x6D : Keypad + grey edit keys */
				if (value >= 0x5A)
				{
					return s_keymapNumlock[value - 0x5A];
				}
				else
				{
					/*return s_keymapNumpad[value - 0x4B]; CODE has been removed in 1b81370b006 */
				}
			}

			if (value < 0x70 || value > 0x79)
			{
				return (ushort)(keyFlags | value | 0x80);
			}
			value -= 0x70;  /* F1-F10 Function keys */
			if ((keyFlags & 0x700) != 0)
			{
				if ((keyFlags & 0x400) == 0)
				{   /* ? */
					if ((keyFlags & 0x200) == 0)
					{   /* ALT */
						return (ushort)(keyFlags | (0xAC - value)); /* 0xA3 - 0xAC */
					}
					return (ushort)(keyFlags | (0xA2 - value)); /* ? : 0x99 - 0xA2 */
				}
				return (ushort)(keyFlags | (0x98 - value)); /* SHIFT or ALT : 0x8F - 0x98 */
			}
			return (ushort)(keyFlags | (0xC5 - value)); /* No modifier : 0xBC - 0xC5 */
		}

		/* Copied from 29E8:0A9C - 29E8:0AB6 */
		static sbyte[][] data_0A9C = //[13][2]
		{
			new sbyte[] {-1, -1}, new sbyte[] {-1,  0}, new sbyte[] {-1,  1}, new sbyte[] { 0,  0},
			new sbyte[] { 0,  0}, new sbyte[] { 0, -1}, new sbyte[] { 0,  0}, new sbyte[] { 0,  1},
			new sbyte[] { 0,  0}, new sbyte[] { 0,  0}, new sbyte[] { 1, -1}, new sbyte[] { 1,  0},
			new sbyte[] { 1,  1}
		};
		/* Copied from 29E8:0AB6 - 29E8:0AD6 */
		static ushort[] data_0AB6 = { 8, 2, 8, 6, 4, 3, 8, 5, 8, 8, 8, 8, 0, 1, 8, 7 };
		/* Copied from 29E8:000A - 29E8:002E */
		static XYPosition[] mousePos = {
			new XYPosition { x = 0x0a0, y = 0x000}, new XYPosition { x = 0x13f, y = 0x000}, new XYPosition { x = 0x13f, y = 0x045},
			new XYPosition { x = 0x13f, y = 0x089}, new XYPosition { x = 0x0a0, y = 0x089}, new XYPosition { x = 0x000, y = 0x089},
			new XYPosition { x = 0x000, y = 0x045}, new XYPosition { x = 0x000, y = 0x000}, new XYPosition { x = 0x0a0, y = 0x045}
		};
		static sbyte[] offsetSmall = { -1, 0, 1 };
		static sbyte[] offsetBig = { -16, 0, 16 };
		/*
		 * Handle input.
		 * @param input New input.
		 * Upper byte is flags, lower byte is Dune II keycode
		 * Flags :
		 *   0x01 SHIFT
		 *   0x02 ALT
		 *   0x04 ?
		 *   0x08 KEYUP / RELEASE
		 */
		internal static void Input_HandleInput(ushort input)
		{
			ushort oldTail;
			ushort saveSize = 0;

			ushort index;
			ushort value;
			byte bit_value;

			ushort inputMouseX;
			ushort inputMouseY;
			ushort[] tempBuffer = new ushort[4];
			ushort flags; /* Mask for allowed input types. See InputFlagsEnum. */

			flags = Mouse.g_inputFlags;
			inputMouseX = Mouse.g_mouseX;
			inputMouseY = Mouse.g_mouseY;

			if (Mouse.g_mouseMode == (byte)InputMouseMode.INPUT_MOUSE_MODE_RECORD)
			{
				saveSize = 4;
			}

			if (input == 0) return;

			value = (ushort)(input & 0xFF);
			if ((flags & (ushort)InputFlagsEnum.INPUT_FLAG_NO_CLICK) != 0 && (input & 0x400) == 0)
			{

				if (((flags & (ushort)InputFlagsEnum.INPUT_FLAG_UNKNOWN_2000) != 0 && (value == 0x2B || value == 0x3D || value == 0x6C)) || value == 0x63)
				{
					input = (ushort)(0x41 | (input & 0xFF00));
					Mouse.g_prevButtonState |= 1;
					if ((input & 0x800) != 0)
					{
						Mouse.g_prevButtonState &= 0xFE; /* ~1 */
					}
				}
				else if (value == 0x68)
				{
					input = (ushort)(0x42 | (input & 0xFF00));
					Mouse.g_prevButtonState |= 2;
					if ((input & 0x800) != 0)
					{
						Mouse.g_prevButtonState &= 0xFD; /* ~2 */
					}
				}
				else if ((input & 0x800) == 0 && (value == 0x61 || (value >= 0x5B && value <= 0x67 &&
					  (value <= 0x5D || value >= 0x65 || value == 0x60 || value == 0x62))))
				{

					sbyte dx, dy;

					dx = data_0A9C[value - 0x5B][0];
					dy = data_0A9C[value - 0x5B][1];

					if ((input & 0x200) != 0)
					{
						XYPosition xy;

						xy = mousePos[data_0AB6[((dy & 3) << 2) | (dx & 3)]];
						inputMouseX = xy.x;
						inputMouseY = xy.y;
					}
					else
					{
						sbyte[] change;

						change = ((input & 0x100) == 0) ? offsetSmall[1..] : offsetBig[1..];

						inputMouseX += (ushort)change[dx];
						inputMouseY += (ushort)change[dy];
						if (inputMouseX >= 0x8000) inputMouseX = 0;
						if (inputMouseY >= 0x8000) inputMouseY = 0;
						if ((short)inputMouseX > Gfx.SCREEN_WIDTH - 1) inputMouseX = Gfx.SCREEN_WIDTH - 1;
						if ((short)inputMouseY > Gfx.SCREEN_HEIGHT - 1) inputMouseY = Gfx.SCREEN_HEIGHT - 1;
					}

					Mouse.g_mouseX = inputMouseX;
					Mouse.g_mouseY = inputMouseY;
					if (Mouse.g_mouseLock == 0)
					{
						/* Move mouse pointer */
						Gui.GUI_Mouse_Hide();
						Gui.GUI_Mouse_Show();
					}
					input = 0x2D;
				}
			}

			oldTail = s_historyTail;

			if (Input_History_Add(input) != 0)
			{
				s_historyTail = oldTail;
				return;
			}

			value = (ushort)(input & 0xFF);
			if (value == 0x2D || value == 0x41 || value == 0x42)
			{
				/* mouse buttons : 0x2D : no change
								   0x41 : change for 1st button
								   0x42 : change for 2nd button */
				if ((Input_History_Add(inputMouseX) != 0) || (Input_History_Add(inputMouseY) != 0))
				{
					s_historyTail = oldTail;
					return;
				}

				tempBuffer[2] = inputMouseX;
				tempBuffer[3] = inputMouseY;
				saveSize += 4;
			}

			bit_value = 1;
			if (value != 0x2D && value != 0x7F && (input & 0x800) != 0) bit_value = 0;
			/* bitvalue = 0 if release, 1 for key press */

			if (value == 0x2D || value == 0x7F ||
					((input & 0x800) != 0 && (flags & (ushort)InputFlagsEnum.INPUT_FLAG_KEY_RELEASE) == 0 && value != 0x41 && value != 0x42))
			{
				s_historyTail = oldTail;
			}

			index = (ushort)((value & 0x7F) >> 3);
			bit_value <<= (value & 7);
			if ((bit_value & s_activeInputMap[index]) != 0 && (flags & (ushort)InputFlagsEnum.INPUT_FLAG_KEY_REPEAT) == 0)
			{
				s_historyTail = oldTail;
			}
			s_activeInputMap[index] &= (byte)((1 << (value & 7)) ^ 0xFF);   /* Clear bit */
			s_activeInputMap[index] |= bit_value;                   /* set bit */

			if (Mouse.g_mouseMode != (byte)InputMouseMode.INPUT_MOUSE_MODE_RECORD || value == 0x7D) return;

			tempBuffer[0] = input;
			tempBuffer[1] = (ushort)Timer.g_timerInput;
			CFile.File_Write(Mouse.g_mouseFileID, Array.ConvertAll(tempBuffer, x => (byte)x), saveSize);
			Timer.g_timerInput = 0;
		}

		/*
		 * Add a value to the history buffer.
		 * @param value New value to add.
		 * @return \c 1 if adding fails, \c 0 if successful.
		 */
		static ushort Input_History_Add(ushort value)
		{
			ushort index;

			index = (ushort)((s_historyTail + 2) & 0xFF);
			if (index == s_historyHead) return 1;

			s_history[s_historyTail / 2] = value;
			s_historyTail = index;
			return 0;
		}

		/*
		 * Get input, and add it to the history.
		 * @param value Old value.
		 * @return Added input value.
		 */
		static ushort Input_AddHistory(ushort value)
		{
			if (Mouse.g_mouseMode == (byte)InputMouseMode.INPUT_MOUSE_MODE_NORMAL || Mouse.g_mouseMode == (byte)InputMouseMode.INPUT_MOUSE_MODE_RECORD) return value;

			if (Mouse.g_mouseNoRecordedValue)
			{
				value = 0;
			}
			else if (Timer.g_timerInput < Mouse.g_mouseRecordedTimer)
			{
				value = 0;
			}
			else if (Mouse.g_mouseInputValue == 0x2D)
			{   /* 0x2D == '-' */
				Input_ReadInputFromFile();
				value = 0;
			}
			else
			{
				value = Mouse.g_mouseInputValue;
			}

			s_history[s_historyHead / 2] = value;
			return value;
		}

		/*
		 * Is input available?
		 * @return \c 0 if no input, else a value.
		 */
		internal static ushort Input_IsInputAvailable()
		{
			ushort value;

			value = (ushort)(s_historyHead ^ s_historyTail);

			return Input_AddHistory(value);
		}

		/*
		 * Wait for input, and return the read event.
		 * @return New input.
		 */
		internal static ushort Input_Wait()
		{
			ushort value = 0;

			for (; ; Sleep.sleepIdle())
			{
				if (Mouse.g_mouseMode == (byte)InputMouseMode.INPUT_MOUSE_MODE_PLAY) break;

				value = s_historyHead;
				if (value != s_historyTail) break;
			}

			value = Input_ReadHistory(value);

			Input_ReadInputFromFile();
			return value;
		}

		internal static void Input_Init()
		{
			byte i;

			for (i = 0; i < s_activeInputMap.Length; i++) s_activeInputMap[i] = 0;
		}

		/*
		 * Sets the given bits in input flags.
		 *
		 * @param bits The bits to set.
		 * @return The new value of input flags.
		 */
		internal static ushort Input_Flags_SetBits(ushort bits)
		{
			Mouse.g_inputFlags |= bits;

			if ((Mouse.g_inputFlags & (ushort)InputFlagsEnum.INPUT_FLAG_KEY_RELEASE) != 0)
			{
				byte i;
				for (i = 0; i < s_activeInputMap.Length; i++) s_activeInputMap[i] = 0;
			}

			return Mouse.g_inputFlags;
		}

		/*
		 * Clears the given bits in input flags.
		 *
		 * @param bits The bits to clear.
		 * @return The new value of input flags.
		 */
		internal static ushort Input_Flags_ClearBits(ushort bits)
		{
			Mouse.g_inputFlags &= (ushort)~bits;
			return Mouse.g_inputFlags;
		}

		/*
		 * Get the next key.
		 * @return Next key.
		 */
		internal static ushort Input_Keyboard_NextKey()
		{
			ushort i;
			ushort value;

			Input_AddHistory(0);

			for (; ; Sleep.sleepIdle())
			{
				ushort index;

				index = s_historyHead;
				if (Mouse.g_mouseMode != (byte)InputMouseMode.INPUT_MOUSE_MODE_PLAY && index == s_historyTail)
				{
					value = 0;
					break;
				}

				value = s_history[index / 2];
				if (Mouse.g_mouseMode == (byte)InputMouseMode.INPUT_MOUSE_MODE_PLAY && value == 0) break;

				for (i = 0; i < s_keymapIgnore.Length; i++)
				{
					if (s_keymapIgnore[i] == (value & 0xFF)) break;
				}

				if (i == s_keymapIgnore.Length && (value & 0x800) == 0 && (value & 0xFF) < 0x7A) break;

				if ((value & 0xFF) >= 0x41 && (value & 0xFF) <= 0x44) index += 4;

				s_historyHead = (ushort)(index + 2);
			}

			if (value != 0)
			{
				value = (ushort)(Input_Keyboard_HandleKeys(value) & 0xFF);
			}

			return value;
		}

		/*
		 * Translate keyboard input.
		 * @param keyValue Entered key value.
		 * @return Translated key value.
		 */
		static ushort Input_Keyboard_Translate(ushort keyValue)
		{
			ushort i;

			if ((Mouse.g_inputFlags & (ushort)InputFlagsEnum.INPUT_FLAG_NO_TRANSLATE) != 0) return keyValue;

			for (i = 0; i < s_translateMap.Length; i++)
			{
				if (s_translateMap[i] != (byte)(keyValue & 0xFF)) continue;

				return (ushort)(s_translateTo[i] | (keyValue & 0xFF00));
			}

			return keyValue;
		}

		/*
		 * Test whether \a value is available in the input.
		 * @param value Input value.
		 * @return \c 0 means not available.
		 */
		internal static ushort Input_Test(ushort value)
		{
			Input_AddHistory(value);
			value = Input_Keyboard_Translate(value);

			return (ushort)(s_activeInputMap[value >> 3] & (1 << (value & 7)));
		}

		/* Receive the keyboard scancodes from the AT keyboard */
		internal static void Input_EventHandler(byte key)
		{
			byte state;
			byte i;

			state = 0;

			/* escape code for escaped scancodes */
			if (key == 0xE0)
			{
				s_input_extendedKey = true;
				return;
			}

			/* Key up */
			if ((key & 0x80) != 0)
			{
				key &= 0x7F;
				state |= 0x08;  /* KEYUP */
			}

			if (s_input_extendedKey)
			{
				s_input_extendedKey = false;

				for (i = 0; i < s_translateExtendedMap.Length; i++)
				{
					if (s_translateExtendedMap[i] == key)
					{
						key = s_translateMap[i];
						break;
					}
				}
				if (i == 16) return;
			}
			else if (key == 0x7A)
			{
				key = 0x80;
			}
			else
			{
				key = s_keyTranslate[key & 0x7F];
			}

			if ((s_activeInputMap[7] & 0x4) != 0) return;   /* 0x3a : LCTRL */
			if ((s_activeInputMap[7] & 0x50) != 0) state |= 0x04;   /* 0x3c 0x3e : LALT RALT => ALT */

			key = (byte)(Input_Keyboard_Translate(key) & 0xFF);

			if ((s_activeInputMap[7] & 0x2) != 0) state |= 0x01;    /* 0x39 : RSHIFT */

			if (state == 0x06 && key == 0x68) return;   /* state == ALT+??? && key == KP DEL */
			if (state == 0x06 && key == 0x4C) return;   /* state == ALT+??? && key == DELETE */

			Input_HandleInput((ushort)((state << 8) | key));
		}
	}
}
