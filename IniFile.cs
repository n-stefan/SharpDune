/* opendune.ini file handling */

using System;
using System.Diagnostics;
using System.IO;

namespace SharpDune
{
    class IniFile
	{
		static string g_openduneini;

        internal static int IniFile_GetInteger(string key, int defaultValue)
		{
			if (g_openduneini == null) return defaultValue;
			return Ini.Ini_GetInteger("opendune", key, defaultValue, g_openduneini);
		}

		/*
		 * Set language depending on value in opendune.ini
		 *
		 * @param config dune config to modify
		 * @return False in case of error
		 */
		internal static bool SetLanguage_From_IniFile(DuneCfg config)
		{
			string language = null; //char[16];

			if (config == null || g_openduneini == null) return false;
			if (IniFile_GetString("language", null, language, (ushort)language.Length) == null)
			{
				return false;
			}
			if (string.Equals(language, "ENGLISH", StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(language, "ENGLISH") == 0)
				config.language = (byte)Language.LANGUAGE_ENGLISH;
			else if (string.Equals(language, "FRENCH", StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(language, "FRENCH") == 0)
				config.language = (byte)Language.LANGUAGE_FRENCH;
			else if (string.Equals(language, "GERMAN", StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(language, "GERMAN") == 0)
				config.language = (byte)Language.LANGUAGE_GERMAN;
			else if (string.Equals(language, "ITALIAN", StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(language, "ITALIAN") == 0)
				config.language = (byte)Language.LANGUAGE_ITALIAN;
			else if (string.Equals(language, "SPANISH", StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(language, "SPANISH") == 0)
				config.language = (byte)Language.LANGUAGE_SPANISH;
			return true;
		}

		/*
		 * Release opendune.ini malloc'd buffer
		 */
		internal static void Free_IniFile() =>
			g_openduneini = null; //free(g_openduneini);

		/*
		 * Find and read the opendune.ini file
		 *
		 * @return True if and only if opendune.ini file was found and read.
		 */
		internal static bool Load_IniFile()
		{
			FileStream f;
			StreamReader s;
			long fileSize;
			string path;

			/* look for opendune.ini in the following locations:
			   1) %APPDATA%/SharpDUNE
			   2) current directory
			   3) data/ dir
			*/
			path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SharpDUNE\opendune.ini"); //SHGetFolderPath( NULL, CSIDL_APPDATA/*CSIDL_COMMON_APPDATA*/, NULL, 0, path ) != S_OK)

			if (System.IO.File.Exists(path))
			{
				f = new FileStream(path, System.IO.FileMode.Open); //fopen(path, "rb");
			}
			else if (System.IO.File.Exists(@".\opendune.ini"))
			{
				f = new FileStream(@".\opendune.ini", System.IO.FileMode.Open); //fopen("opendune.ini", "rb");
			}
			else if (System.IO.File.Exists(@".\data\opendune.ini"))
			{
				f = new FileStream(@".\data\opendune.ini", System.IO.FileMode.Open); //fopen("data/opendune.ini", "rb");
			}
			else
			{
				Trace.WriteLine("WARNING: opendune.ini file not found.");
				return false;
			}
			if (f.Seek(0, SeekOrigin.End) < 0)
			{
				Trace.WriteLine("ERROR: Cannot get opendune.ini file size.");
				f.Close();
				return false;
			}
			fileSize = f.Position;
			if (fileSize < 0)
			{
				Trace.WriteLine("ERROR: Cannot get opendune.ini file size.");
				f.Close();
				return false;
			}
			f.Seek(0, SeekOrigin.Begin);
			//g_openduneini = malloc(fileSize + 1);
			//if (g_openduneini == NULL) {
			//	Error("Cannot allocate %ld bytes\n", fileSize + 1);
			//	fclose(f);
			//	return false;
			//}
			s = new StreamReader(f);
			g_openduneini = s.ReadToEnd();
			if (g_openduneini.Length != fileSize)
			{
				Trace.WriteLine("ERROR: Failed to read opendune.ini");
				f.Close();
				g_openduneini = null;
				return false;
			}
			//g_openduneini[fileSize] = '\0';
			f.Close();
			return true;
		}

		internal static string IniFile_GetString(string key, string defaultValue, string dest, ushort length)
		{
			string p;
			//ushort i;
			/* if g_openduneini is NULL, Ini_GetString() still does what we expect */
			p = Ini.Ini_GetString("opendune", key, defaultValue, /*ref dest?.ToArray(),*/ length, g_openduneini);
			//TODO: Check
			if (!string.IsNullOrEmpty(p))
			{
				/* Trim space from the beginning of the dest */
				dest.TrimStart();
				//for (i = 0; i < length && (dest[i] == ' ' || dest[i] == '\t') && (dest[i] != '\0'); i++);
				//if (i > 0 && i < length) memmove(dest, dest+i, length - i);
			}
			return p;
		}
	}
}
