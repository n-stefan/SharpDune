/* INI file reading */

namespace SharpDune;

static partial class Ini
{
    [GeneratedRegex("^(.*)[\t ]*=[\t ]*.*$", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex IniRegex();

    internal static string Ini_GetString(ReadOnlySpan<char> category, ReadOnlySpan<char> key, string defaultValue, ReadOnlySpan<char> source)
    {
        var result = defaultValue;

        if (source == default) return result;

        var start = source.IndexOf($"[{category}]", StringComparison.OrdinalIgnoreCase);
        if (start == -1) return result;
        start += category.Length + 2;
        var section = source.Slice(start);
        var end = section.IndexOf('[');
        if (end != -1) section = section.Slice(0, end);

        if (key != default)
        {
            var pattern = $"^{key}[\t ]*=[\t ]*(.*)$";
            var match = Regex.Match(section.ToString(), pattern, RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (match.Success) result = match.Groups[1].Value.Trim().TrimEnd(',');
        }
        else
        {
            var matches = IniRegex().Matches(section.ToString());
            if (matches.Count != 0) result = string.Join('|', matches.Select(m => m.Groups[1].Value));
        }

        return result;
    }

    internal static int Ini_GetInteger(string category, string key, int defaultValue, string source)
    {
        string value; //char[16];
        string buffer; //char[16]

        value = defaultValue.ToString(Culture); //sprintf(value, "%d", defaultValue);

        buffer = Ini_GetString(category, key, value, source);
        return int.Parse(buffer, Culture); //atoi(buffer);
    }

    internal static void Ini_SetString(string category, string key, string value, string source)
    {
        string s;
        string buffer; //char[120];

        if (source == null || category == null) return;

        s = Ini_GetString(category, null, null, source);
        if (s == null && key != null)
        {
            buffer = $"{Environment.NewLine}[{category}]{Environment.NewLine}"; //sprintf(buffer, "\r\n[%s]\r\n", category);
            source += buffer; //strcat(source, buffer);
        }

        s = Ini_GetString(category, key, null, source);
        if (s != null)
        {
            //TODO: Check and try to simplify
            var count = s.IndexOfAny(['\r', '\n']); //uint16 count = (uint16)strcspn(s, "\r\n");
            if (count != 0)
            {
                /* Drop first line if not empty */
                var len = s.Length + count + 1 + 1; //size_t len = strlen(s + count + 1) + 1;
                var array = s.ToCharArray();
                Array.Copy(array, count + 1, array, 0, len); //memmove(s, s + count + 1, len);
                s = new string(array);
            }
            if (s[0] == '\n')
            {
                /* Drop first line if empty */
                var len = s.Length + 1 + 1; //size_t len = strlen(s + 1) + 1;
                var array = s.ToCharArray();
                Array.Copy(array, 1, array, 0, len); //memmove(s, s + 1, len);
                s = new string(array);
            }
        }
        else
        {
            s = Ini_GetString(category, null, null, source);
        }

        if (value != null)
        {
            buffer = $"{key}={value}{Environment.NewLine}"; //sprintf(buffer, "%s=%s\r\n", key, value);
            var array = s.ToCharArray();
            Array.Copy(array, 0, array, buffer.Length, s.Length + 1); //memmove(s + strlen(buffer), s, strlen(s) + 1);
            s = new string(array);
            s += buffer; //memcpy(s, buffer, strlen(buffer));
        }
    }
}
