/* Font */

using System.Linq;

namespace SharpDune
{
    class FontChar
	{
		internal byte width;
		internal byte unusedLines;
		internal byte usedLines;
		internal byte[] data;
	}

	class Font
	{
		internal byte height;
		internal byte maxWidth;
		internal byte count;
		internal FontChar[] chars;
	}

	class CFont
	{
		internal static Font g_fontIntro;
		internal static Font g_fontNew6p;
		internal static Font g_fontNew8p;

		internal static sbyte g_fontCharOffset;

		internal static Font g_fontCurrent;

		/*
		 * Select a font.
		 *
		 * @param font The pointer of the font to use.
		 */
		internal static void Font_Select(Font f)
		{
			if (f == null) return;

			g_fontCurrent = f;
		}

		/*
		 * Get the width of the string in pixels.
		 *
		 * @param string The string to get the width of.
		 * @return The width of the string in pixels.
		 */
		internal static ushort Font_GetStringWidth(string str) =>
			(ushort)((str == null) ? 0 : str.Sum(c => Font_GetCharWidth(c)));

		/*
		 * Get the width of a char in pixels.
		 *
		 * @param c The char to get the width of.
		 * @return The width of the char in pixels.
		 */
		internal static ushort Font_GetCharWidth(char c) =>
			(ushort)(g_fontCurrent.chars[c].width + g_fontCharOffset);

		internal static bool Font_Init()
		{
			g_fontIntro = Font_LoadFile("INTRO.FNT");
			if ((Config.g_config.language == (byte)Language.LANGUAGE_GERMAN) && CFile.File_Exists("new6pg.fnt"))
			{
				g_fontNew6p = Font_LoadFile("new6pg.fnt");
			}
			else
			{
				g_fontNew6p = Font_LoadFile("new6p.fnt");
			}
			g_fontNew8p = Font_LoadFile("new8p.fnt");

			return g_fontNew8p != null;
		}

		internal static void Font_Uninit()
		{
			Font_Unload(g_fontIntro); g_fontIntro = null;
			Font_Unload(g_fontNew6p); g_fontNew6p = null;
			Font_Unload(g_fontNew8p); g_fontNew8p = null;
		}

		/*
		 * Load a font file.
		 *
		 * @param filename The name of the font file.
		 * @return The pointer of the allocated memory where the file has been read.
		 */
		static Font Font_LoadFile(string filename)
		{
			byte[] buf;
			Font f;
			byte i;
			ushort start;
			ushort dataStart;
			ushort widthList;
			ushort lineList;

			if (!CFile.File_Exists(filename)) return null;

			buf = CFile.File_ReadWholeFile(filename);

			if (buf[2] != 0x00 || buf[3] != 0x05)
			{
				buf = null; //free(buf);
				return null;
			}

			f = new Font(); //(Font*) calloc(1, sizeof(Font));
			start = Endian.READ_LE_UINT16(buf[4..]);
			dataStart = Endian.READ_LE_UINT16(buf[6..]);
			widthList = Endian.READ_LE_UINT16(buf[8..]);
			lineList = Endian.READ_LE_UINT16(buf[12..]);
			f.height = buf[start + 4];
			f.maxWidth = buf[start + 5];
			f.count = (byte)(Endian.READ_LE_UINT16(buf[10..]) - widthList);
			f.chars = new FontChar[f.count]; //(FontChar*) calloc(f->count, sizeof(FontChar));
			for (i = 0; i < f.chars.Length; i++) f.chars[i] = new FontChar();

			for (i = 0; i < f.count; i++)
			{
				FontChar fc = f.chars[i];
				ushort dataOffset;
				byte x;
				byte y;

				fc.width = buf[widthList + i];
				fc.unusedLines = buf[lineList + i * 2];
				fc.usedLines = buf[lineList + i * 2 + 1];

				dataOffset = Endian.READ_LE_UINT16(buf[(dataStart + i * 2)..]);
				if (dataOffset == 0) continue;

				fc.data = new byte[fc.usedLines * fc.width]; //(uint8*) malloc(fc->usedLines* fc->width);

				for (y = 0; y < fc.usedLines; y++)
				{
					for (x = 0; x < fc.width; x++)
					{
						byte data = buf[dataOffset + y * ((fc.width + 1) / 2) + x / 2];
						if (x % 2 != 0) data >>= 4;
						fc.data[y * fc.width + x] = (byte)(data & 0xF);
					}
				}
			}

			buf = null; //free(buf);

			return f;
		}

		static void Font_Unload(Font f)
		{
			byte i;

			for (i = 0; i < f.count; i++) f.chars[i].data = null; //free(f->chars[i].data);
			f.chars = null; //free(f->chars);
			f = null; //free(f);
		}
	}
}
