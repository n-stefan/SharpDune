/* String */

namespace SharpDune;

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

static class String
{
    static string[] s_strings;

    static ushort s_stringsCount;

    internal static string[] g_languageSuffixes = ["ENG", "FRE", "GER", "ITA", "SPA"];

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
     * Appends ".(ENG|FRE|. . .)" to the given string.
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
        Span<byte> buf;
        ushort count;
        ushort i;
        string decomp_buffer; //char[1024]
        ushort from, to;

        buf = File_ReadWholeFile(String_GenerateFilename(filename));
        count = (ushort)(Read_LE_UInt16(buf) / 2);

        if (end < 0) end = start + count - 1;

        Array.Resize(ref s_strings, end + 1); //(char**)realloc(s_strings, (end + 1) * sizeof(char*));
        s_strings[s_stringsCount] = null;

        to = Read_LE_UInt16(buf);
        for (i = 0; i <= count && s_stringsCount <= end; i++)
        {
            from = to;
            to = (ushort)Math.Min(Read_LE_UInt16(buf.Slice(i * 2)), buf.Length - 1);

            var src = from == to ? string.Empty : SharpDune.Encoding.GetString(buf.Slice(from, to - from - 1));
            string dst;

            if (compressed)
            {
                (decomp_buffer, _) = String_Decompress(src, 1024);
                decomp_buffer = String_TranslateSpecial(decomp_buffer);
                decomp_buffer = decomp_buffer.Trim(); //String_Trim(decomp_buffer);
                dst = decomp_buffer; //strdup(decomp_buffer);
            }
            else
            {
                dst = src; //strdup(src);
                //dst = dst.Trim(); //String_Trim(dst);
            }

            if (dst.Length == 0 && s_strings[0] != null)
            {
                //dst = null; //free(dst);
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

        //buf = null; //free(buf);
    }

    private static void String_Replace(int i, string term, int position)
    {
        var index = s_strings[i].IndexOf(term, Comparison);
        if (index == -1) return;
        s_strings[i] = char.IsDigit(term[1]) ?
            s_strings[i].Remove(index, term.Length).Insert(index, $"{{{position}, {term[1]}}}") :
            s_strings[i].Remove(index, term.Length).Insert(index, $"{{{position}}}");
    }

    static void String_Sanitize()
    {
        int index;
        for (var i = 0; s_strings[i] != null && i < s_strings.Length; i++)
        {
            String_Replace(i, "%ld", 0);
            String_Replace(i, "%d", 0);
            String_Replace(i, "%d", 1);
            String_Replace(i, "%2d", 0);
            String_Replace(i, "%2d", 1);
            String_Replace(i, "%3d", 0);
            String_Replace(i, "%4d", 0);
            String_Replace(i, "%4d", 1);
            String_Replace(i, "%s", 0);
            String_Replace(i, "%s", 1);
            index = s_strings[i].IndexOf("%%", Comparison);
            if (index != -1) s_strings[i] = s_strings[i].Remove(index, 1);
        }

        if (g_config.language == (byte)Language.GERMAN)
        {
            s_strings[Array.FindIndex(s_strings, s => string.Equals(s, "Ziel w\u0084hlen\rZeit-Minus: {0}", StringComparison.Ordinal))] = "Ziel w\u0084hlen\rZ-Minus: {0}";
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
    internal static (string, ushort) String_Decompress(string s, ushort destLen)
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
                c = (byte)couples[c + 16];    /* 2nd char */
            }
            dest[count++] = (char)c;
            if (count >= destLen - 1)
            {
                Trace.TraceWarning("WARNING: String_Decompress() : truncating output !");
                break;
            }
        }
        //dest[count] = '\0';

        var result = new string(dest, 0, count);
        return (result, count);
    }

    /*
     * Translates 0x1B 0xXX occurences into extended ASCII values (0x7F + 0xXX).
     *
     * @param source The untranslated string.
     * @param dest The translated string.
     */
    internal static string String_TranslateSpecial(ReadOnlySpan<char> str)
    {
        if (str == default) return null;

        var dest = new StringBuilder();
        var strPointer = 0;

        while (strPointer < str.Length)
        {
            var c = str[strPointer++];
            if (c == 0x1B)
            {
                c = (char)(0x7F + str[strPointer++]);
            }
            dest.Append(c);
        }
        return dest.ToString();
        //*dest = '\0';
    }

    /*
     * Go to the next string.
     * @param ptr Pointer to the current string.
     * @return Pointer to the next string.
     */
    internal static int String_NextString(Span<byte> text, int pointer)
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
    internal static int String_PrevString(Span<byte> text, int pointer)
    {
        do
        {
            pointer--;
        } while (text[pointer] == 0);
        pointer -= text[pointer] - 1;
        return pointer;
    }
}
