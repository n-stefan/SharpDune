﻿/* sharpdune.ini file handling */

namespace SharpDune;

static class IniFile
{
    static string g_sharpduneini;

    internal static int IniFile_GetInteger(string key, int defaultValue)
    {
        if (g_sharpduneini == null) return defaultValue;
        return Ini_GetInteger("sharpdune", key, defaultValue, g_sharpduneini);
    }

    /*
     * Set language depending on value in sharpdune.ini
     *
     * @param config dune config to modify
     * @return False in case of error
     */
    internal static bool SetLanguage_From_IniFile(DuneCfg config)
    {
        string language; //char[16];

        if (config == null || g_sharpduneini == null) return false;
        if ((language = IniFile_GetString("language", null)) == null)
        {
            return false;
        }
        if (string.Equals(language, "ENGLISH", StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(language, "ENGLISH") == 0)
            config.language = (byte)Language.ENGLISH;
        else if (string.Equals(language, "FRENCH", StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(language, "FRENCH") == 0)
            config.language = (byte)Language.FRENCH;
        else if (string.Equals(language, "GERMAN", StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(language, "GERMAN") == 0)
            config.language = (byte)Language.GERMAN;
        else if (string.Equals(language, "ITALIAN", StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(language, "ITALIAN") == 0)
            config.language = (byte)Language.ITALIAN;
        else if (string.Equals(language, "SPANISH", StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(language, "SPANISH") == 0)
            config.language = (byte)Language.SPANISH;
        return true;
    }

    /*
     * Release sharpdune.ini malloc'd buffer
     */
    internal static void Free_IniFile() =>
        g_sharpduneini = null; //free(g_sharpduneini);

    /*
     * Find and read the sharpdune.ini file
     *
     * @return True if and only if sharpdune.ini file was found and read.
     */
    internal static bool Load_IniFile()
    {
        FileStream f;
        long fileSize;

        /* look for sharpdune.ini in the following locations:
           1) %APPDATA%/SharpDUNE
           2) current directory
           3) data/ dir
        */
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SharpDUNE", "sharpdune.ini"); //SHGetFolderPath( NULL, CSIDL_APPDATA/*CSIDL_COMMON_APPDATA*/, NULL, 0, path ) != S_OK)
        if (System.IO.File.Exists(path))
        {
            f = new FileStream(path, System.IO.FileMode.Open); //fopen(path, "rb");
        }
        else
        {
            path = Path.Combine(".", "sharpdune.ini");
            if (System.IO.File.Exists(path))
            {
                f = new FileStream(path, System.IO.FileMode.Open); //fopen("sharpdune.ini", "rb");
            }
            else
            {
                path = Path.Combine(".", "data", "sharpdune.ini");
                if (System.IO.File.Exists(path))
                {
                    f = new FileStream(path, System.IO.FileMode.Open); //fopen("data/sharpdune.ini", "rb");
                }
                else
                {
                    Trace.WriteLine("WARNING: sharpdune.ini file not found.");
                    return false;
                }
            }
        }
        if (f.Seek(0, SeekOrigin.End) < 0)
        {
            Trace.WriteLine("ERROR: Cannot get sharpdune.ini file size.");
            f.Close();
            return false;
        }
        fileSize = f.Position;
        if (fileSize < 0)
        {
            Trace.WriteLine("ERROR: Cannot get sharpdune.ini file size.");
            f.Close();
            return false;
        }
        f.Seek(0, SeekOrigin.Begin);
        //g_sharpduneini = malloc(fileSize + 1);
        //if (g_sharpduneini == NULL) {
        //	Error("Cannot allocate %ld bytes\n", fileSize + 1);
        //	fclose(f);
        //	return false;
        //}
        using var s = new StreamReader(f);
        g_sharpduneini = s.ReadToEnd();
        if (g_sharpduneini.Length != fileSize)
        {
            Trace.WriteLine("ERROR: Failed to read sharpdune.ini");
            f.Close();
            g_sharpduneini = null;
            return false;
        }
        //g_sharpduneini[fileSize] = '\0';
        f.Close();
        return true;
    }

    internal static string IniFile_GetString(string key, string defaultValue)
    {
        /* if g_sharpduneini is NULL, Ini_GetString() still does what we expect */
        var p = Ini_GetString("sharpdune", key, defaultValue, g_sharpduneini);
        if (!string.IsNullOrEmpty(p))
        {
            /* Trim space from the beginning of the dest */
            p = p.TrimStart();
        }
        return p;
    }
}
