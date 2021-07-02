/* File access */

//TODO: Add using statements

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Math;

namespace SharpDune
{
    delegate bool ProcessFileCallback(string name, string path, uint size);

	enum ConvertCase
	{
		NO_CONVERT = 0,
		CONVERT_TO_UPPERCASE,
		CONVERT_TO_LOWERCASE
	}

	enum SearchDirectory
	{
		SEARCHDIR_ABSOLUTE,
		SEARCHDIR_GLOBAL_DATA_DIR,
		SEARCHDIR_CAMPAIGN_DIR,
		SEARCHDIR_PERSONAL_DATA_DIR
	}

	[Flags]
	enum FileMode
	{
		FILE_MODE_READ = 0x01,
		FILE_MODE_WRITE = 0x02,
		FILE_MODE_READ_WRITE = FILE_MODE_READ | FILE_MODE_WRITE,

		FILE_MAX = 8,
		FILE_INVALID = 0xFF
	}

	/*
	 * Static information about opened files.
	 */
	class File
	{
		internal FileStream/*FILE?*/ fp;
		internal uint size;
		internal uint start;
		internal uint position;
		internal FileInfo pakInfo;
	}

	class FileInfoFlags
	{
		bool inMemory;                                  /*!< File is loaded in alloc'd memory. */
		internal bool inPAKFile;                        /*!< File can be in other PAK file. */
	}

	/*
	 * Static information about files and their location.
	 */
	class FileInfo
	{
		internal string filename;                       /*!< Name of the file. */
		internal uint fileSize;                         /*!< The size of this file. */
		//IntPtr buffer;                                /*!< In case the file is read in the memory, this is the location of the data. */
		internal uint filePosition;                     /*!< Where in the file we currently are (doesn't have to start at zero when in PAK file). */
		internal FileInfoFlags flags;                   /*!< General flags of the FileInfo. */
	}

	/*
	 * Information about files in data/ directory
	 * and processed content of PAK files.
	 */
	class FileInfoLinkedElem
	{
		internal FileInfoLinkedElem next;
		internal FileInfo info;
		internal string filenamebuffer; //char[1]
	}

	class PakFileInfoLinkedElem
	{
		internal PakFileInfoLinkedElem next;
		internal FileInfo pak;
		internal FileInfo info;
		internal string filenamebuffer; //char[1]
	}

	class CFile
	{
		//readonly string SEARCHDIR_GLOBAL_DATA_DIR = Path.Combine(Environment.CurrentDirectory, "data");
		//readonly string SEARCHDIR_PERSONAL_DATA_DIR = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		static string g_dune_data_dir = @".\data";
		static string g_personal_data_dir = ".";

		/* In order to avoid to open/close the same .PAK file multiple time
		 * in a row, we cache the last opened PAK file.
		 * DUNE II code is very conservative about file access, and only open
		 * one file at once. */
		static /*FILE?*/FileStream s_currentPakFp = null;
		static FileInfo s_currentPakInfo = null;

		static FileInfoLinkedElem s_files_in_root = null;
		static PakFileInfoLinkedElem s_files_in_pak = null;

		static File[] s_file;

		static CFile()
		{
			s_file = new File[8];
			for (var i = 0; i < s_file.Length; i++) s_file[i] = new File();
		}

		static FileAccess FileAccessFromString(string access) =>
			access switch
			{
				"wb" => FileAccess.Write,
				"wb+" => FileAccess.ReadWrite,
				"rb" => FileAccess.Read,
				_ => FileAccess.Read
			};

		/*
		 * Find the FileInfo for the given filename.
		 *
		 * @param filename The filename to get the FileInfo for.
		 * @return The FileInfo pointer or NULL if not found.
		 */
		static FileInfo FileInfo_Find_ByName(string filename, ref FileInfo pakInfo)
		{
			FileInfoLinkedElem e;
			for (e = s_files_in_root; e != null; e = e.next)
			{
				if (string.Equals(e.info.filename, filename, StringComparison.OrdinalIgnoreCase))
				{ //!strcasecmp(e->info.filename, filename)) {
					/*if (pakInfo != null)*/
					pakInfo = null; //if (pakInfo) *pakInfo = NULL;
					return e.info;
				}
			}
			PakFileInfoLinkedElem pe;
			for (pe = s_files_in_pak; pe != null; pe = pe.next)
			{
				if (string.Equals(pe.info.filename, filename, StringComparison.OrdinalIgnoreCase))
				{ //!strcasecmp(e->info.filename, filename)) {
					/*if (pakInfo != null)*/
					pakInfo = pe.pak; //if (pakInfo) *pakInfo = e->pak;
					return pe.info;
				}
			}
			return null;
		}

		static string File_MakeCompleteFilename(int len, SearchDirectory dir, string filename, ConvertCase convert)
		{
			char[] buf = null;
			int j;
			int i;

			if (dir == SearchDirectory.SEARCHDIR_GLOBAL_DATA_DIR || dir == SearchDirectory.SEARCHDIR_CAMPAIGN_DIR)
			{
				/* Note: campaign specific data directory not implemented. */
				buf = $@"{g_dune_data_dir}\{filename}".ToCharArray();
			}
			else if (dir == SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR)
			{
				buf = $@"{g_personal_data_dir}\{filename}".ToCharArray();
			}
			i = buf.Length;

			//buf[len - 1] = '\0';

			if (i > len)
			{
				Trace.WriteLine($"WARNING: output truncated : {new string(buf)} ({filename})");
				i = len;
			}
			if (convert != ConvertCase.NO_CONVERT)
			{
				for (j = i - 1; j >= 0; j--)
				{
					if (buf[j] == '/' || buf[j] == '\\')
						break;
					if (convert == ConvertCase.CONVERT_TO_LOWERCASE)
					{
						if ('A' <= buf[j] && buf[j] <= 'Z')
							buf[j] = (char)(buf[j] + 'a' - 'A');
					}
					else if (convert == ConvertCase.CONVERT_TO_UPPERCASE)
					{
						if ('a' <= buf[j] && buf[j] <= 'z')
							buf[j] = (char)(buf[j] - 'a' + 'A');
					}
				}
			}
			return new string(buf);
		}

		/*
		 * Open a file from the data/ directory
		 */
		internal static FileStream fopendatadir(SearchDirectory dir, string name, string mode)
		{
			string filenameComplete; //char[1024]
			FileInfo fileInfo;
			string filename;
			FileInfo pakInfo = null;

			var fileAccess = FileAccessFromString(mode);
			Debug.WriteLine($"DEBUG: fopendatadir({dir}, {name}, {mode})");
			if (dir != SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR)
			{
				fileInfo = FileInfo_Find_ByName(name, ref pakInfo);
				if (fileInfo != null)
				{
					/* Take the filename from the FileInfo structure, as it was read
					 * from the data/ directory */
					filename = fileInfo.filename;
				}
				else
				{
					filename = name;
				}
				filenameComplete = File_MakeCompleteFilename(1024, dir, filename, ConvertCase.NO_CONVERT);
				return new FileStream(filenameComplete, System.IO.FileMode.Open, fileAccess); //fopen(filenameComplete, mode);
			}
			else
			{
				if (fileAccess == FileAccess.Read)
				{
					/* try both in lower and upper case */
					filenameComplete = File_MakeCompleteFilename(1024, dir, name, ConvertCase.CONVERT_TO_UPPERCASE);
					if (System.IO.File.Exists(filenameComplete))
						return new FileStream(filenameComplete, System.IO.FileMode.Open, fileAccess); //fopen(filenameComplete, mode);
					filenameComplete = File_MakeCompleteFilename(1024, dir, name, ConvertCase.CONVERT_TO_LOWERCASE);
					if (System.IO.File.Exists(filenameComplete))
						return new FileStream(filenameComplete, System.IO.FileMode.Open, fileAccess); //fopen(filenameComplete, mode);
					return null;
				}
				else
				{
					filenameComplete = File_MakeCompleteFilename(1024, dir, name, ConvertCase.CONVERT_TO_UPPERCASE);
					return new FileStream(filenameComplete, System.IO.FileMode.Create, fileAccess); //fopen(filenameComplete, mode);
				}
			}
		}

		/*
		 * Internal function to truly open a file.
		 *
		 * @param filename The name of the file to open.
		 * @param mode The mode to open the file in. Bit 1 means reading, bit 2 means writing.
		 * @return An index value refering to the opened file, or FILE_INVALID.
		 */
		static byte _File_Open(SearchDirectory dir, string filename, byte mode)
		{
			byte fileIndex;
			FileInfo fileInfo;
			FileInfo pakInfo = null;

			if (((FileMode)mode & FileMode.FILE_MODE_READ_WRITE) == FileMode.FILE_MODE_READ_WRITE) return (byte)FileMode.FILE_INVALID;

			/* Find a free spot in our limited array */
			for (fileIndex = 0; fileIndex < (byte)FileMode.FILE_MAX; fileIndex++)
			{
				if (s_file[fileIndex].fp == null) break;
			}
			if (fileIndex >= (byte)FileMode.FILE_MAX)
			{
				Trace.WriteLine($"WARNING: Limit of {FileMode.FILE_MAX} open files reached.");
				return (byte)FileMode.FILE_INVALID;
			}

			if (mode == (byte)FileMode.FILE_MODE_READ && dir != SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR)
			{
				/* Look in PAK only for READ only files, and not Personnal files */
				fileInfo = FileInfo_Find_ByName(filename, ref pakInfo);
				if (fileInfo == null) return (byte)FileMode.FILE_INVALID;
				if (pakInfo == null)
				{
					/* Check if we can find the file outside any PAK file */
					s_file[fileIndex].fp = fopendatadir(dir, filename, "rb");
					if (s_file[fileIndex].fp == null) return (byte)FileMode.FILE_INVALID;

					s_file[fileIndex].start = 0;
					s_file[fileIndex].position = 0;
					s_file[fileIndex].fp.Seek(0, SeekOrigin.End); //fseek(s_file[fileIndex].fp, 0, SEEK_END);
					s_file[fileIndex].size = (uint)s_file[fileIndex].fp.Position; //ftell(s_file[fileIndex].fp);
					s_file[fileIndex].fp.Seek(0, SeekOrigin.Begin); //fseek(s_file[fileIndex].fp, 0, SEEK_SET);
				}
				else
				{
					/* file is found in PAK */
					if (pakInfo != s_currentPakInfo)
					{
						if (s_currentPakFp != null) s_currentPakFp.Close();
						s_currentPakFp = fopendatadir(dir, pakInfo.filename, "rb");
						s_currentPakInfo = pakInfo;
					}
					s_file[fileIndex].fp = s_currentPakFp;
					if (s_file[fileIndex].fp == null) return (byte)FileMode.FILE_INVALID;

					s_file[fileIndex].start = fileInfo.filePosition;
					s_file[fileIndex].position = 0;
					s_file[fileIndex].size = fileInfo.fileSize;

					/* Go to the start of the file now */
					s_file[fileIndex].fp.Seek(s_file[fileIndex].start, SeekOrigin.Begin); //fseek(s_file[fileIndex].fp, s_file[fileIndex].start, SEEK_SET);
				}
				s_file[fileIndex].pakInfo = pakInfo;
				return fileIndex;
			}

			/* Check if we can find the file outside any PAK file */
			s_file[fileIndex].fp = fopendatadir(dir, filename, (mode == (byte)FileMode.FILE_MODE_WRITE) ? "wb" : ((mode == (byte)FileMode.FILE_MODE_READ_WRITE) ? "wb+" : "rb"));
			if (s_file[fileIndex].fp != null)
			{
				s_file[fileIndex].start = 0;
				s_file[fileIndex].position = 0;
				s_file[fileIndex].size = 0;
				s_file[fileIndex].pakInfo = null;

				/* We can only check the size of the file if we are reading (or appending) */
				if ((mode & (byte)FileMode.FILE_MODE_READ) != 0)
				{
					s_file[fileIndex].fp.Seek(0, SeekOrigin.End); //fseek(s_file[fileIndex].fp, 0, SEEK_END);
					s_file[fileIndex].size = (uint)s_file[fileIndex].fp.Position; //ftell(s_file[fileIndex].fp);
					s_file[fileIndex].fp.Seek(0, SeekOrigin.Begin); //fseek(s_file[fileIndex].fp, 0, SEEK_SET);
				}

				return fileIndex;
			}
			return (byte)FileMode.FILE_INVALID;
		}

		/*
		 * Open a file for reading/writing/appending.
		 *
		 * @param filename The name of the file to open.
		 * @param mode The mode to open the file in. Bit 1 means reading, bit 2 means writing.
		 * @return An index value refering to the opened file, or FILE_INVALID.
		 */
		static byte File_Open_Ex(SearchDirectory dir, string filename, byte mode)
		{
			byte res;

			res = _File_Open(dir, filename, mode);

			if (res == (byte)FileMode.FILE_INVALID)
			{
				if (dir == SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR)
				{
					Trace.WriteLine($"WARNING: Unable to open file '{filename}'.");
				}
				else
				{
					Trace.WriteLine($"ERROR: Unable to open file '{filename}'.");
					Environment.Exit(1);
				}
			}

			return res;
		}

		/*
		 * Check if a file exists either in a PAK or on the disk.
		 *
		 * @param dir directory for this file
		 * @param filename The filename to check for.
		 * @param fileSize Filled with the file size if the file exists
		 * @return True if and only if the file can be found.
		 */
		static bool File_Exists_Ex(SearchDirectory dir, string filename, out uint fileSize)
		{
			var exists = false;
			FileInfo pakInfo = null;
			fileSize = 0;

			if (dir != SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR)
			{
				FileInfo fileInfo;
				fileInfo = FileInfo_Find_ByName(filename, ref pakInfo);
				if (fileInfo != null)
				{
					exists = true;
					/*if (fileSize != null)*/
					fileSize = fileInfo.fileSize;
				}
			}
			else
			{
				byte index;
				index = _File_Open(dir, filename, (byte)FileMode.FILE_MODE_READ);
				if (index != (byte)FileMode.FILE_INVALID)
				{
					exists = true;
					/*if (fileSize != null)*/
					fileSize = File_GetSize(index);
					File_Close(index);
				}
			}

			return exists;
		}

		internal static bool File_Exists(string filename) =>
			File_Exists_Ex(SearchDirectory.SEARCHDIR_GLOBAL_DATA_DIR, filename, out _);

		internal static bool File_Exists_Personal(string filename) =>
			File_Exists_Ex(SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR, filename, out _);

		internal static byte File_Open(string filename, FileMode mode) =>
			File_Open_Ex(SearchDirectory.SEARCHDIR_GLOBAL_DATA_DIR, filename, (byte)mode);

		internal static uint File_ReadBlockFile<T>(string filename, /*byte[]*/T buffer, uint length) =>
			File_ReadBlockFile_Ex(SearchDirectory.SEARCHDIR_GLOBAL_DATA_DIR, filename, buffer, length);

		internal static byte File_Open_Personal(string filename, FileMode mode) =>
			File_Open_Ex(SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR, filename, (byte)mode);

		internal static uint File_ReadBlockFile_Personal(string filename, byte[] buffer, uint length) =>
			File_ReadBlockFile_Ex(SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR, filename, buffer, length);

		internal static bool File_Exists_GetSize(string filename, out uint filesize) =>
			File_Exists_Ex(SearchDirectory.SEARCHDIR_GLOBAL_DATA_DIR, filename, out filesize);

		internal static byte ChunkFile_Open(string filename) =>
			ChunkFile_Open_Ex(SearchDirectory.SEARCHDIR_GLOBAL_DATA_DIR, filename);

		internal static byte ChunkFile_Open_Personal(string filename) =>
			ChunkFile_Open_Ex(SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR, filename);

		/*
		 * Read bytes from a file into a buffer.
		 *
		 * @param index The index given by File_Open() of the file.
		 * @param buffer The buffer to read into.
		 * @param length The amount of bytes to read.
		 * @return The amount of bytes truly read, or 0 if there was a failure.
		 */
		internal static uint File_Read<T>(byte index, ref T buffer, uint length, int offset = 0)
		{
			if (index >= (byte)FileMode.FILE_MAX) return 0;
			if (s_file[index].fp == null) return 0;
			if (s_file[index].position >= s_file[index].size) return 0;
			if (length == 0) return 0;

			if (length > s_file[index].size - s_file[index].position) length = s_file[index].size - s_file[index].position;

			if (buffer is byte[] bufferByteArray)
			{
				if (s_file[index].fp.Read(bufferByteArray, offset, (int)length) == 0) //fread(buffer, length, 1, s_file[index].fp) != 1) {
				{
					Trace.WriteLine("ERROR: Read error");
					File_Close(index);
					length = 0;
				}
			}
			else if (buffer is ushort[] _)
			{
				var bytes = new byte[length]; //[bufferUshortArray.Length * 2]
				if (s_file[index].fp.Read(bytes, offset, bytes.Length) == 0)
				{
					Trace.WriteLine("ERROR: Read error");
					File_Close(index);
					length = 0;
				}
				else
				{
					buffer = (T)(object)Common.FromByteArrayToUshortArray(bytes);
				}
			}
			else if (buffer is char[] bufferCharArray)
			{
				var sr = new StreamReader(s_file[index].fp);
				if (sr.ReadBlock(bufferCharArray, offset, (int)length) == 0)
				{
					Trace.WriteLine("ERROR: Read error");
					File_Close(index);
					length = 0;
				}
			}
			else if (buffer is uint _)
			{
				var br = new BinaryReader(s_file[index].fp);
				try
				{
					buffer = (T)(object)br.ReadUInt32();
				}
				catch (Exception e)
				{
					Trace.WriteLine($"ERROR: Read error - {e.Message}");
					File_Close(index);
					length = 0;
				}
			}
			else if (buffer is string _)
			{
				var sr = new StreamReader(s_file[index].fp);
				try
				{
					var chars = new char[length];
					sr.ReadBlock(chars, offset, (int)length);
					buffer = (T)(object)new string(chars);
				}
				catch (Exception e)
				{
					Trace.WriteLine($"ERROR: Read error - {e.Message}");
					File_Close(index);
					length = 0;
				}
			}
			else if (buffer is MentatInfo mentatInfo)
			{
				if (s_file[index].fp.Read(mentatInfo.notused, 0, mentatInfo.notused.Length) == 0)
				{
					Trace.WriteLine("ERROR: Read error");
					File_Close(index);
					length = 0;
				}
				else
				{
					var br = new BinaryReader(s_file[index].fp);
					try
					{
						mentatInfo.length = br.ReadUInt32();
					}
					catch (Exception e)
					{
						Trace.WriteLine($"ERROR: Read error - {e.Message}");
						File_Close(index);
						length = 0;
					}
				}
			}
			else
			{
				throw new Exception($"ERROR: File_Read - no implementation for type '{buffer.GetType()}'!");
			}

			s_file[index].position += length;
			return length;
		}

		/*
		 * Reads length bytes from filename into buffer.
		 *
		 * @param filename Then name of the file to read.
		 * @param buffer The buffer to read into.
		 * @param length The amount of bytes to read.
		 * @return The amount of bytes truly read, or 0 if there was a failure.
		 */
		static uint File_ReadBlockFile_Ex<T>(SearchDirectory dir, string filename, /*byte[]*/T buffer, uint length)
		{
			byte index;

			index = File_Open_Ex(dir, filename, (byte)FileMode.FILE_MODE_READ);
			if (index == (byte)FileMode.FILE_INVALID) return 0;
			length = File_Read(index, ref buffer, length);
			File_Close(index);
			return length;
		}

		/*
		 * Reads the whole file in the memory.
		 *
		 * @param filename The name of the file to open.
		 * @param mallocFlags The type of memory to allocate.
		 * @return The pointer to allocated memory where the file has been read.
		 */
		internal static /*IntPtr*/byte[] File_ReadWholeFile(string filename)
		{
			byte index;
			uint length;
			byte[] buffer;

			index = File_Open(filename, FileMode.FILE_MODE_READ);
			if (index == (byte)FileMode.FILE_INVALID) return null;
			length = File_GetSize(index);

			buffer = new byte[length]; //malloc(length + 1);
			File_Read(index, ref buffer, length);

			/* In case of text-files it can be very important to have a \0 at the end */
			//((char *)buffer)[length] = '\0';

			File_Close(index);

			return buffer;
		}

		internal static uint File_ReadWholeFile<T>(string filename, ref T buffer)
		{
			byte index;
			uint length;

			index = File_Open(filename, FileMode.FILE_MODE_READ);
			if (index == (byte)FileMode.FILE_INVALID) return 0;
			length = File_GetSize(index);

			File_Read(index, ref buffer, length);

			/* In case of text-files it can be very important to have a \0 at the end */
			//((char *)buffer)[length] = '\0';

			File_Close(index);

			return length;
		}

		/*
		 * Get the size of a file.
		 *
		 * @param index The index given by File_Open() of the file.
		 * @return The size of the file.
		 */
		static uint File_GetSize(byte index)
		{
			if (index >= (byte)FileMode.FILE_MAX) return 0;
			if (s_file[index].fp == null) return 0;

			return s_file[index].size;
		}

		/*
		 * Close an opened file.
		 *
		 * @param index The index given by File_Open() of the file.
		 */
		internal static void File_Close(byte index)
		{
			if (index >= (byte)FileMode.FILE_MAX) return;
			if (s_file[index].fp == null) return;

			if (s_file[index].pakInfo != null)
			{
				s_file[index].fp = null;    /* do not close PAK file */
				return;
			}

			s_file[index].fp.Close();
			s_file[index].fp = null;
		}

		/*
		 * Write bytes from a buffer to a file.
		 *
		 * @param index The index given by File_Open() of the file.
		 * @param buffer The buffer to write from.
		 * @param length The amount of bytes to write.
		 * @return The amount of bytes truly written, or 0 if there was a failure.
		 */
		internal static uint File_Write(byte index, byte[] buffer, uint length)
		{
			if (index >= (byte)FileMode.FILE_MAX) return 0;
			if (s_file[index].fp == null) return 0;

			try
			{
				s_file[index].fp.Write(buffer, 0, (int)length);
			}
			catch (Exception e)
			{
				Trace.WriteLine($"ERROR: Write error: {e.Message}");
				File_Close(index);

				length = 0;
			}

			s_file[index].position += length;
			if (s_file[index].position > s_file[index].size) s_file[index].size = s_file[index].position;
			return length;
		}

		/*
		 * Seek inside a file.
		 *
		 * @param index The index given by File_Open() of the file.
		 * @param position Position to fix to.
		 * @param mode Mode of seeking. 0 = SEEK_SET, 1 = SEEK_CUR, 2 = SEEK_END.
		 * @return The new position inside the file, relative from the start.
		 */
		internal static uint File_Seek(byte index, int position, byte mode)
		{
			if (index >= (byte)FileMode.FILE_MAX) return 0;
			if (s_file[index].fp == null) return 0;
			if (mode > 2) { File_Close(index); return 0; }

			switch (mode)
			{
				case 0:
					s_file[index].fp.Seek(s_file[index].start + position, SeekOrigin.Begin); //fseek(s_file[index].fp, s_file[index].start + position, SEEK_SET);
					s_file[index].position = (uint)position;
					break;
				case 1:
					s_file[index].fp.Seek(position, SeekOrigin.Current); //fseek(s_file[index].fp, (int32)position, SEEK_CUR);
					s_file[index].position += (uint)position;
					break;
				case 2:
					s_file[index].fp.Seek(s_file[index].start + s_file[index].size - position, SeekOrigin.Begin); //fseek(s_file[index].fp, s_file[index].start + s_file[index].size - position, SEEK_SET);
					s_file[index].position = (uint)(s_file[index].size - position);
					break;
			}

			return s_file[index].position;
		}

		/*
		 * Read a 16bit unsigned from the file (written on disk in Little endian)
		 *
		 * @param index The index given by File_Open() of the file.
		 * @return The integer read.
		 */
		internal static ushort File_Read_LE16(byte index)
		{
			var buffer = new byte[2];
			File_Read(index, ref buffer, (uint)buffer.Length);
			return Endian.READ_LE_UINT16(buffer);
		}

		/*
		 * Read a 32bit unsigned from the file (written on disk in Little endian)
		 *
		 * @param index The index given by File_Open() of the file.
		 * @return The integer read.
		 */
		internal static uint File_Read_LE32(byte index)
		{
			var buffer = new byte[4];
			File_Read(index, ref buffer, (uint)buffer.Length);
			return Endian.READ_LE_UINT32(buffer);
		}

		/*
		 * Write a 16bit unsigned to the file (written on disk in Little endian)
		 *
		 * @param index The index given by File_Open() of the file.
		 * @param value The 16bit unsigned integer
		 * @return true if the operation succeeded
		 */
		internal static bool File_Write_LE16(byte index, ushort value)
		{
			var buffer = new byte[2];
			Endian.WRITE_LE_UINT16(buffer, value);
			return (File_Write(index, buffer, 2) == 2);
		}

		//internal static bool fread_le_int16(ref short p, FileStream f)
		//{
		//	var value = (ushort)p;
		//	return fread_le_uint16(ref value, f);
		//}

		/*
		 * Read a uint16 value from a little endian file.
		 */
		internal static bool fread_le_uint16(ref ushort value, FileStream stream)
		{
			var buffer = new byte[2];
			//if (value == null) return false;
			if (stream.Read(buffer, 0, 2) != 2) return false; //fread(buffer, 1, 2, stream) != 2)
			value = Endian.READ_LE_UINT16(buffer);
			return true;
		}

		//internal static bool fread_le_int32(ref int p, FileStream f)
		//{
		//	var value = (uint)p;
		//	return fread_le_uint32(ref value, f);
		//}

		/*
		 * Read a uint32 value from a little endian file.
		 */
		internal static bool fread_le_uint32(ref uint value, FileStream stream)
		{
			var buffer = new byte[4];
			//if (value == null) return false;
			if (stream.Read(buffer, 0, 4) != 4) return false; //fread(buffer, 1, 4, stream) != 4)
			value = Endian.READ_LE_UINT32(buffer);
			return true;
		}

		//TODO: Consolidate fwrite_le_... methods
		//internal static bool fwrite_le_int16(short value, BinaryWriter stream) =>
		//	fwrite_le_uint16((ushort)value, stream);

		/*
		 * Write a uint16 value from a little endian file.
		 */
		internal static bool fwrite_le_uint16(ushort value, BinaryWriter stream)
		{
			stream.Write(value);
			//stream.Write((/*char*/sbyte)(value & 0xff)); //if (putc(value & 0xff, stream) == EOF) return false;
			//stream.Write((/*char*/sbyte)((value >> 8) & 0xff)); //if (putc((value >> 8) & 0xff, stream) == EOF) return false;
			//TODO: Or do it this way?
			//byte[] bytes = new byte[2];
			//Endian.WRITE_LE_UINT16(bytes, value);
			//stream.Write(bytes);
			return true;
		}

		//internal static bool fwrite_le_int32(int value, BinaryWriter stream) =>
		//	fwrite_le_uint32((uint)value, stream);

		/*
		 * Write a uint32 value from a little endian file.
		 */
		internal static bool fwrite_le_uint32(uint value, BinaryWriter stream)
		{
			stream.Write(value);
			//TODO: Use Endian.WRITE_LE_UINT32?
			//stream.Write((/*char*/sbyte)(value & 0xff)); //if (putc(value & 0xff, stream) == EOF) return false;
			//stream.Write((/*char*/sbyte)((value >> 8) & 0xff)); //if (putc((value >> 8) & 0xff, stream) == EOF) return false;
			//stream.Write((/*char*/sbyte)((value >> 16) & 0xff)); //if (putc((value >> 16) & 0xff, stream) == EOF) return false;
			//stream.Write((/*char*/sbyte)((value >> 24) & 0xff)); //if (putc((value >> 24) & 0xff, stream) == EOF) return false;
			return true;
		}

		/*
		 * Create a file on the disk.
		 *
		 * @param filename The filename to create.
		 */
		internal static void File_Create_Personal(string filename)
		{
			byte index;

			index = _File_Open(SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR, filename, (byte)FileMode.FILE_MODE_WRITE);
			if (index != (byte)FileMode.FILE_INVALID) File_Close(index);
		}

		/*
		 * Free all ressources loaded in memory.
		 */
		internal static void File_Uninit()
		{
			if (s_currentPakFp != null) s_currentPakFp.Close();
			s_currentPakFp = null;
			s_currentPakInfo = null;
			while (s_files_in_root != null)
			{
				var e = s_files_in_root;
				s_files_in_root = e.next;
				e = null; //free(e);
			}

			while (s_files_in_pak != null)
			{
				var e = s_files_in_pak;
				s_files_in_pak = e.next;
				e = null; //free(e);
			}
		}

		/*
		 * Initialize the personal and global data directories, and the file tables.
		 *
		 * @return True if and only if everything was ok.
		 */
		internal static bool File_Init()
		{
			var buf = string.Empty; //char[1024]

			if (IniFile.IniFile_GetString("savedir", null, buf, (ushort)(buf == null ? 0 : buf.Length)) != null)
			{
				/* savedir is defined in opendune.ini */
				g_personal_data_dir = buf; //strncpy(g_personal_data_dir, buf, sizeof(g_personal_data_dir));
			}
			else
			{
				/* %APPDATA%/SharpDUNE */
				if ((buf = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) == string.Empty) //SHGetFolderPath( NULL, CSIDL_APPDATA/*CSIDL_COMMON_APPDATA*/, NULL, 0, buf ) != S_OK)
				{
					Trace.WriteLine("WARNING: Cannot find AppData directory.");
					g_personal_data_dir = "."; //snprintf(g_personal_data_dir, sizeof(g_personal_data_dir), ".");
				}
				else
				{
					buf = Path.Combine(buf, "SharpDUNE");
					g_personal_data_dir = buf; //strncpy(g_personal_data_dir, buf, sizeof(g_personal_data_dir));
				}
			}

			if (!File_MakeDirectory(g_personal_data_dir))
			{
				Trace.WriteLine($"ERROR: Cannot open personal data directory {g_personal_data_dir}. Do you have sufficient permissions?");
				return false;
			}

			if (IniFile.IniFile_GetString("datadir", null, buf, (ushort)buf.Length) != null)
			{
				/* datadir is defined in opendune.ini */
				g_dune_data_dir += buf; //strncpy(g_dune_data_dir, buf, sizeof(g_dune_data_dir));
			}

			buf = File_MakeCompleteFilename(buf.Length, SearchDirectory.SEARCHDIR_GLOBAL_DATA_DIR, string.Empty, ConvertCase.NO_CONVERT);

			if (!ReadDir_ProcessAllFiles(buf, _File_Init_Callback))
			{
				Trace.WriteLine($"ERROR: Cannot initialize files. Does the directory {buf} exist?");
				return false;
			}

			return true;
		}

		/*
		 * Reads the whole file into buffer.
		 *
		 * @param filename The name of the file to open.
		 * @param buf The buffer to read into.
		 * @return The length of the file.
		 */
		internal static uint File_ReadFile(string filename, byte[] buf, int offset = 0)
		{
			byte index;
			uint length;

			index = File_Open(filename, FileMode.FILE_MODE_READ);
			length = File_GetSize(index);
			File_Read(index, ref buf, length, offset);
			File_Close(index);

			return length;
		}

		/*
		 * Reads the whole file in the memory. The file should contain little endian
		 * 16bits unsigned integers. It is converted to host byte ordering if needed.
		 *
		 * @param filename The name of the file to open.
		 * @param mallocFlags The type of memory to allocate.
		 * @return The pointer to allocated memory where the file has been read.
		 */
		internal static ushort[] File_ReadWholeFileLE16(string filename)
		{
			byte index;
			uint count;
			ushort[] buffer;
			//#if __BYTE_ORDER == __BIG_ENDIAN
			//	uint32 i;
			//#endif

			index = File_Open(filename, FileMode.FILE_MODE_READ);
			count = File_GetSize(index) / 2;

			buffer = new ushort[count];

			if (File_Read(index, ref buffer, count * 2) != count * 2)
			{
				buffer = null; //free(buffer);
				return null;
			}

			File_Close(index);

			//#if __BYTE_ORDER == __BIG_ENDIAN
			//	for(i = 0; i < count; i++) {
			//		buffer[i] = LETOH16(buffer[i]);
			//	}
			//#endif

			return buffer;
		}

		/*
		 * Close an opened chunk file.
		 *
		 * @param index The index given by ChunkFile_Open() of the file.
		 */
		internal static void ChunkFile_Close(byte index)
		{
			if (index == (byte)FileMode.FILE_INVALID) return;

			File_Close(index);
		}

		/*
		 * Seek to the given chunk inside a chunk file.
		 *
		 * @param index The index given by ChunkFile_Open() of the file.
		 * @param chunk The chunk to seek to.
		 * @return The length of the chunk (0 if not found).
		 */
		internal static uint ChunkFile_Seek(byte index, uint chunk)
		{
			uint value = 0;
			uint length = 0;
			var first = true;

			while (true)
			{
				if (File_Read(index, ref value, 4) != 4 && !first) return 0;

				if (value == 0 && File_Read(index, ref value, 4) != 4 && !first) return 0;

				if (File_Read(index, ref length, 4) != 4 && !first) return 0;

				length = Endian.HTOBE32(length);

				if (value == chunk)
				{
					File_Seek(index, -8, 1);
					return length;
				}

				if (first)
				{
					File_Seek(index, 12, 0);
					first = false;
					continue;
				}

				length += 1;
				length &= 0xFFFFFFFE;
				File_Seek(index, (int)length, 1);
			}
		}

		/*
		 * Read bytes from a chunk file into a buffer.
		 *
		 * @param index The index given by ChunkFile_Open() of the file.
		 * @param chunk The chunk to read from.
		 * @param buffer The buffer to read into.
		 * @param length The amount of bytes to read.
		 * @return The amount of bytes truly read, or 0 if there was a failure.
		 */
		internal static uint ChunkFile_Read<T>(byte index, uint chunk, /* void* */ref T buffer, uint buflen)
		{
			uint value = 0;
			uint length = 0;
			var first = true;

			while (true)
			{
				if (File_Read(index, ref value, 4) != 4 && !first) return 0;

				if (value == 0 && File_Read(index, ref value, 4) != 4 && !first) return 0;

				if (File_Read(index, ref length, 4) != 4 && !first) return 0;

				length = Endian.HTOBE32(length);

				if (value == chunk)
				{
					buflen = Min(buflen, length);

					File_Read(index, ref buffer, buflen);

					length += 1;
					length &= 0xFFFFFFFE;

					if (buflen < length) File_Seek(index, (int)(length - buflen), 1);

					return buflen;
				}

				if (first)
				{
					File_Seek(index, 12, 0);
					first = false;
					continue;
				}

				length += 1;
				length &= 0xFFFFFFFE;
				File_Seek(index, (int)length, 1);
			}
		}

		/*
		 * Open a chunk file (starting with FORM) for reading.
		 *
		 * @param filename The name of the file to open.
		 * @return An index value refering to the opened file, or FILE_INVALID.
		 */
		static byte ChunkFile_Open_Ex(SearchDirectory dir, string filename)
		{
			byte index;
			uint header = 0;

			index = File_Open_Ex(dir, filename, (byte)FileMode.FILE_MODE_READ);

			if (index == (byte)FileMode.FILE_INVALID) return index;

			File_Read(index, ref header, 4);

			if (header != Endian.HTOBE32((uint)CSharpDune.MultiChar[FourCC.FORM]))
			{
				File_Close(index);
				return (byte)FileMode.FILE_INVALID;
			}

			File_Seek(index, 4, 1);

			return index;
		}

		/*
		 * Delete a file from the disk.
		 *
		 * @param filename The filename to remove.
		 */
		internal static void File_Delete_Personal(string filename)
		{
			string filenameComplete; //char[1024]

			filenameComplete = File_MakeCompleteFilename(12/*filenameComplete.Length*/, SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR, filename, ConvertCase.CONVERT_TO_LOWERCASE);
			try
			{
				System.IO.File.Delete(filenameComplete);
			}
			catch (Exception)
			{
				//if (unlink(filenameComplete) < 0)
				/* try with the upper case file name */
				filenameComplete = File_MakeCompleteFilename(12/*filenameComplete.Length*/, SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR, filename, ConvertCase.CONVERT_TO_UPPERCASE);
				try
				{
					System.IO.File.Delete(filenameComplete);
				}
				catch (Exception e)
				{
					Trace.WriteLine($"ERROR: Couldn't delete file '{filenameComplete}'. Details: {e.Message}");
				}
			}
		}

		static bool File_MakeDirectory(string dir)
		{
			if (!Directory.Exists(dir))
			{
				try
				{
					Directory.CreateDirectory(dir);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}
			else
			{
				return true;
			}
		}

		/*
		 * Process all files in the directory
		 *
		 * @param dirpath path to directory to traverse
		 * @param cb function called back for each file found
		 * @return True if and only if everything was ok
		 */
		static bool ReadDir_ProcessAllFiles(string dirpath, ProcessFileCallback cb)
		{
			try
			{
				var directory = new DirectoryInfo(dirpath);
				var files = directory.GetFiles();
				files.ToList().ForEach(f => cb?.Invoke(f.Name, f.FullName, (uint)f.Length));
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/*
		 * Callback for processing files found in data/ directory.
		 *
		 * @param name The name of the file.
		 * @param path The relative path of the file.
		 * @param size The file size (bytes).
		 * @return True if the processing went OK.
		 */
		static bool _File_Init_Callback(string name, string path, uint size)
		{
			string ext;
			FileInfo fileInfo;

			fileInfo = _File_Init_AddFileInRootDir(name, size);
			if (fileInfo == null) return false;
			ext = Path.GetExtension(path);
			//ext = strrchr(path, '.');
			//if (ext != null) {
			if (string.Equals(ext, ".pak", StringComparison.OrdinalIgnoreCase))
			{ //(strcasecmp(ext, ".pak") == 0)
				if (!_File_Init_ProcessPak(path, size, fileInfo))
				{
					Trace.WriteLine($"WARNING: Failed to process PAK file {path}");
					return false;
				}
			}
			//}
			return true;
		}

		/*
		 * Memorize a file from the data/ directory.
		 *
		 * @param filename The name of the file.
		 * @param filesize The size of the file.
		 * @return A pointer to the newly created FileInfo.
		 */
		static FileInfo _File_Init_AddFileInRootDir(string filename, uint filesize)
		{
			FileInfoLinkedElem elem;
            //size_t size;

            //size = sizeof(FileInfoLinkedElem) + strlen(filename);
            elem = new FileInfoLinkedElem
            {
                //if (new == NULL) {
                //	Error("cannot allocate %u bytes of memory\n", size);
                //	return NULL;
                //}
                next = s_files_in_root,
                filenamebuffer = filename //memcpy(new->filenamebuffer, filename, strlen(filename) + 1);
            }; //new = malloc(size);
            elem.info = new FileInfo
            {
                filename = elem.filenamebuffer,
                fileSize = filesize,
                filePosition = 0
            }; //memset(&new->info, 0, sizeof(FileInfo));
            s_files_in_root = elem;
			return elem.info;
		}

		/*
		 * Process (parse) a PAK file.
		 *
		 * @param pakpath real path to open PAK file.
		 * @param paksize size (bytes) of the PAK file.
		 * @param pakInfo pointer to the FileInfo for PAK file.
		 * @return True if PAK processing was ok.
		 */
		static bool _File_Init_ProcessPak(string pakpath, uint paksize, FileInfo pakInfo)
		{
			FileStream f;
			uint position;
			uint nextposition = 0;
			uint size;
			var filename = new char[256];
			uint i;

			f = new FileStream(pakpath, System.IO.FileMode.Open, FileAccessFromString("rb")); //fopen(pakpath, "rb");
			if (f == null)
			{
				Trace.WriteLine($"ERROR: Failed to open {pakpath}");
				return false;
			}
			if (!fread_le_uint32(ref nextposition, f))
			{
				f.Close();
				return false;
			}
			while (nextposition != 0)
			{
				position = nextposition;
				for (i = 0; i < filename.Length; i++)
				{
					if ((filename[i] = (char)f.ReadByte()) == -1) //fread(filename + i, 1, 1, f) != 1)
					{
						f.Close();
						return false;
					}
					if (filename[i] == '\0') break;
				}
				if (i == filename.Length)
				{
					f.Close();
					return false;
				}
				if (!fread_le_uint32(ref nextposition, f))
				{
					f.Close();
					return false;
				}
				size = (nextposition != 0) ? nextposition - position : paksize - position;
				if (_File_Init_AddFileInPak(new string(filename, 0, (int)i), size, position, pakInfo) == null)
				{
					f.Close();
					return false;
				}
			}
			f.Close();
			return true;
		}

		/*
		 * Memorize a file inside a PAK file.
		 *
		 * @param filename the filename as indicated in PAK header.
		 * @param filesize the size as calculated from PAK header.
		 * @param position the position of the file from the start of the PAK file.
		 * @param pakInfo FileInfo pointer for the PAK file.
		 * @return A pointer to the newly created FileInfo.
		 */
		static FileInfo _File_Init_AddFileInPak(string filename, uint filesize, uint position, FileInfo pakInfo)
		{
			PakFileInfoLinkedElem elem;
            //size_t size;

            //size = sizeof(PakFileInfoLinkedElem) + strlen(filename);
            elem = new PakFileInfoLinkedElem
            {
                //if (new == NULL) {
                //	Error("cannot allocate %u bytes of memory\n", size);
                //	return NULL;
                //}
                next = s_files_in_pak,
                pak = pakInfo,
                filenamebuffer = filename //memcpy(new->filenamebuffer, filename, strlen(filename) + 1);
            }; //new = malloc(size);
            elem.info = new FileInfo
            {
                filename = elem.filenamebuffer,
                fileSize = filesize,
                filePosition = position,
                flags = new FileInfoFlags
                {
                    inPAKFile = true
                }
            }; //memset(&new->info, 0, sizeof(FileInfo));
            s_files_in_pak = elem;
			return elem.info;
		}
	}
}
