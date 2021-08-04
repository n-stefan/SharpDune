/* String */

namespace SharpDune
{
    /*
	 * Types of Language available in the game.
	 */
    enum Language
	{
		ENGLISH = 0,
		FRENCH = 1,
		GERMAN = 2,
		ITALIAN = 3,
		SPANISH = 4,

		MAX = 5,
		INVALID = 0xFF
	}

	class String
	{
		static string[] s_strings;

        static ushort s_stringsCount;

        internal static string[] g_languageSuffixes = new string[(int)Language.MAX] { "ENG", "FRE", "GER", "ITA", "SPA" };

		/*
         * Returns a pointer to the string at given index in current string file.
         *
         * @param stringID The index of the string.
         * @return The pointer to the string.
         */
		internal static string String_Get_ByIndex(ushort stringID) => s_strings[stringID];

		internal static string String_Get_ByIndex(Text stringID) => s_strings[(ushort)stringID];

		static string filename; //char[14]
		/*
		 * Appends ".(ENG|FRE|...)" to the given string.
		 *
		 * @param name The string to append extension to.
		 * @return The new string.
		 */
		internal static string String_GenerateFilename(string name)
		{
			Debug.Assert(g_config.language < g_languageSuffixes.Length);

			filename = $"{name}.{g_languageSuffixes[g_config.language]}"; //snprintf(filename, sizeof(filename), "%s.%s", name, g_languageSuffixes[g_config.language]);
			return filename;
		}

		static void String_Load(string filename, bool compressed, int start, int end)
		{
			byte[] buf;
			ushort count;
			ushort i;
			string decomp_buffer = null; //char[1024]
			ushort from, to, prev;
			Range range;

			buf = File_ReadWholeFile(String_GenerateFilename(filename));
			count = (ushort)(READ_LE_UINT16(buf) / 2);

			if (end < 0) end = start + count - 1;

			Array.Resize(ref s_strings, end + 1); //(char**)realloc(s_strings, (end + 1) * sizeof(char*));
			s_strings[s_stringsCount] = null;

			prev = READ_LE_UINT16(buf);
			for (i = 0; i <= count && s_stringsCount <= end; i++)
			{
				//from = Endian.READ_LE_UINT16(buf[(i * 2)..]);
				//to = (ushort)Min(Endian.READ_LE_UINT16(buf[((i + 1) * 2)..]), buf.Length - 1);
				//range = from == to ? from..to : from..(to - 1);
				from = prev;
				to = (ushort)Math.Min(READ_LE_UINT16(buf[(i * 2)..]), buf.Length - 1);
				prev = to;
				range = from == to ? from..to : from..(to - 1);

				var src = SharpDune.Encoding.GetString(buf[range]);
				string dst;

				if (compressed)
				{
					ushort len;
					len = String_Decompress(src, ref decomp_buffer, 1024);
					decomp_buffer = String_TranslateSpecial(decomp_buffer);
					decomp_buffer.Trim(); //String_Trim(decomp_buffer);
					dst = decomp_buffer; //strdup(decomp_buffer);
				}
				else
				{
					dst = src; //strdup(src);
					dst.Trim(); //String_Trim(dst);
				}

				if (dst.Length == 0 && s_strings[0] != null)
				{
					dst = null; //free(dst);
				}
				else
				{
					s_strings[s_stringsCount++] = dst;
				}
			}

			/* EU version has one more string in DUNE langfile. */
			if (s_stringsCount == (ushort)Text.STR_LOAD_GAME) s_strings[s_stringsCount++] = s_strings[(int)Text.STR_LOAD_A_GAME]; //strdup(s_strings[STR_LOAD_A_GAME]);

			while (s_stringsCount <= end)
			{
				s_strings[s_stringsCount++] = null;
			}

			buf = null; //free(buf);
		}

		static void String_Sanitize()
		{
			if (g_config.language == (byte)Language.ENGLISH)
            {
				s_strings[Array.FindIndex(s_strings, s => s == "Insufficient memory by %ld bytes.")] = "Insufficient memory by {0} bytes.";
				s_strings[Array.FindIndex(s_strings, s => s == "%s %s destroyed.")] = "{0} {1} destroyed.";
				s_strings[Array.FindIndex(s_strings, s => s == "Score: %d")] = "Score: {0}";
				s_strings[Array.FindIndex(s_strings, s => s == "Time: %dh %dm")] = "Time: {0}h {1}m";
				s_strings[Array.FindIndex(s_strings, s => s == "%d%% done")] = "{0}% done";
				s_strings[Array.FindIndex(s_strings, s => s == "Pick Target\rT-Minus: %d")] = "Pick Target\rT-Minus: {0}";
				s_strings[Array.FindIndex(s_strings, s => s == "   Spice\rholds: %4d\rmax: %4d")] = "   Spice\rholds: {0, 4}\rmax: {1, 4}";
				s_strings[Array.FindIndex(s_strings, s => s == " is %d percent full")] = " is {0} percent full";
				s_strings[Array.FindIndex(s_strings, s => s == " is %d percent full and harvesting")] = " is {0} percent full and harvesting";
				s_strings[Array.FindIndex(s_strings, s => s == " is %d percent full and heading back")] = " is {0} percent full and heading back";
				s_strings[Array.FindIndex(s_strings, s => s == " is %d percent full and awaiting pickup")] = " is {0} percent full and awaiting pickup";
				s_strings[Array.FindIndex(s_strings, s => s == "Can not place %s here.")] = "Can not place {0} here.";
				s_strings[Array.FindIndex(s_strings, s => s == "Production of %s has started.")] = "Production of {0} has started.";
				s_strings[Array.FindIndex(s_strings, s => s == "Upgrading\r%d%% done")] = "Upgrading\r{0}% done";
				s_strings[Array.FindIndex(s_strings, s => s == "Radar Scan\rFriend: %2d\rEnemy: %2d")] = "Radar Scan\rFriend: {0, 2}\rEnemy: {1, 2}";
				s_strings[Array.FindIndex(s_strings, s => s == "Frigate\rArrival in\rT-minus %d")] = "Frigate\rArrival in\rT-minus {0}";
				s_strings[Array.FindIndex(s_strings, s => s == "Cost: %3d")] = "Cost: {0, 3}";
				s_strings[Array.FindIndex(s_strings, s => s == "Qty: %2d")] = "Qty: {0, 2}";
				s_strings[Array.FindIndex(s_strings, s => s == "Upgrade Cost : %d")] = "Upgrade Cost : {0}";
            }
		}

		/*
		 * Loads the language files in the memory, which is used after that with String_GetXXX_ByIndex().
		 */
		internal static void String_Init()
		{
			String_Load("DUNE", false, 1, 339);
			String_Load("MESSAGE", false, 340, 367);
			String_Load("INTRO", false, 368, 404);
			String_Load("TEXTH", true, 405, 444);
			String_Load("TEXTA", true, 445, 484);
			String_Load("TEXTO", true, 485, 524);
			String_Load("PROTECT", true, 525, -1);

			String_Sanitize();
		}

		/*
		 * Unloads the language files in the memory.
		 */
		internal static void String_Uninit()
		{
			ushort i;

			for (i = 0; i < s_stringsCount; i++) s_strings[i] = null; //free(s_strings[i]);
			s_strings = null; //free(s_strings);
		}

		static readonly char[] couples =
			(" etainosrlhcdupm" +       /* 1st char */
			"tasio wb" +                /* <SPACE>? */
			" rnsdalm" +                /* e? */
			"h ieoras" +                /* t? */
			"nrtlc sy" +                /* a? */
			"nstcloer" +                /* i? */
			" dtgesio" +                /* n? */
			"nr ufmsw" +                /* o? */
			" tep.ica" +                /* s? */
			"e oiadur" +                /* r? */
			" laeiyod" +                /* l? */
			"eia otru" +                /* h? */
			"etoakhlr" +                /* c? */
			" eiu,.oa" +                /* d? */
			"nsrctlai" +                /* u? */
			"leoiratp" +                /* p? */
			"eaoip bm").ToCharArray();  /* m? */
		/*
		 * Decompress a string.
		 *
		 * Characters values >= 0x80 (1AAAABBB) are unpacked to 2 characters
		 * from the table. AAAA gives the 1st characted.
		 * BBB the 2nd one (from a sub-table depending on AAAA)
		 *
		 * @param source The compressed string.
		 * @param dest The decompressed string.
		 * @return The length of decompressed string.
		 */
		internal static ushort String_Decompress(string s, ref string result, ushort destLen)
		{
			ushort count;
			var sPointer = 0;
			var dest = new char[destLen];

			for (count = 0; sPointer < s.Length; sPointer++)
			{
				var c = (byte)s[sPointer];
				if ((c & 0x80) != 0)
				{
					c &= 0x7F;
					dest[count++] = couples[c >> 3];    /* 1st char */
					c = (byte)couples[c + 16];  /* 2nd char */
				}
				dest[count++] = (char)c;
				if (count >= destLen - 1)
				{
					Trace.WriteLine($"WARNING: String_Decompress() : truncating output !");
					break;
				}
			}
			//dest[count] = '\0';

			result = new string(dest, 0, count);
			return count;
		}

		/*
		 * Translates 0x1B 0xXX occurences into extended ASCII values (0x7F + 0xXX).
		 *
		 * @param source The untranslated string.
		 * @param dest The translated string.
		 */
		internal static string String_TranslateSpecial(string str)
		{
			if (str == null) return null;

			char[] dest;
			var strPointer = 0;
			var destPointer = 0;

			dest = str.ToCharArray();
			while (strPointer < str.Length)
			{
				var c = str[strPointer++];
				if (c == 0x1B)
				{
					c = (char)(0x7F + str[strPointer++]);
				}
				dest[destPointer++] = c;
			}
			return new string(dest);
			//*dest = '\0';
		}

		/*
		 * Go to the next string.
		 * @param ptr Pointer to the current string.
		 * @return Pointer to the next string.
		 */
		internal static int String_NextString(byte[] text, int pointer)
		{
			pointer += text[pointer];
			while (text[pointer] == 0) pointer++;
			return pointer;
		}

		/*
		 * Go to the previous string.
		 * @param ptr Pointer to the current string.
		 * @return Pointer to the previous string.
		 */
		internal static int String_PrevString(byte[] text, int pointer)
		{
			do
			{
				pointer--;
			} while (text[pointer] == 0);
			pointer -= text[pointer] - 1;
			return pointer;
		}
	}
}
