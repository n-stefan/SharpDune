/* WSA */

using System;
using static System.Math;

namespace SharpDune
{
    /*
	 * The flags of a WSA Header.
	 */
    class WSAFlags
	{
		internal bool notmalloced;                          /*!< If the WSA is in memory of the caller. */
		internal bool malloced;                             /*!< If the WSA is malloc'd by us. */
		internal bool dataOnDisk;                           /*!< Only the header is in the memory. Rest is on disk. */
		internal bool dataInMemory;                         /*!< The whole WSA is in memory. */
		internal bool displayInBuffer;                      /*!< The output display is in the buffer. */
		internal bool noAnimation;                          /*!< If the WSA has animation or not. */
		internal bool hasNoFirstFrame;                      /*!< The WSA is the continuation of another one. */
		internal bool hasPalette;                           /*!< Indicates if the WSA has a palette stored. */
	}

	/*
	 * The header of a WSA file that is being read.
	 */
	class WSAHeader
	{
		internal ushort frameCurrent;                           /*!< Current frame displaying. */
		internal ushort frames;                                 /*!< Total frames in WSA. */
		internal ushort width;                                  /*!< Width of WSA. */
		internal ushort height;                                 /*!< Height of WSA. */
		internal ushort bufferLength;                           /*!< Length of the buffer. */
		internal CArray<byte> buffer;                           /*!< The buffer. */
		internal byte[] fileContent;                            /*!< The content of the file. */
		internal string filename; //char[13]                    /*!< Filename of WSA. */
		internal WSAFlags flags;                                /*!< Flags of WSA. */
	}

	/*
	 * The header of a WSA file as on the disk.
	 */
	class WSAFileHeader
	{
		/* 0000(2)   */
		internal ushort frames;             /*!< Amount of animation frames in this WSA. */
		/* 0002(2)   */
		internal ushort width;              /*!< Width of WSA. */
		/* 0004(2)   */
		internal ushort height;             /*!< Height of WSA. */
		/* 0006(2)   */
		internal ushort requiredBufferSize; /*!< The size the buffer has to be at least to process this WSA. */
		/* 0008(2)   */
		internal ushort hasPalette;         /*!< Indicates if the WSA has a palette stored. */
		/* 000A(4)   */
		internal uint firstFrameOffset;     /*!< Offset where animation starts. */
		/* 000E(4)   */
		internal uint secondFrameOffset;    /*!< Offset where animation ends. */
	}

	class WSAObject
	{
		internal WSAHeader header;
		internal CArray<byte> data;
	}

	class Wsa
	{
		const int WSAHeaderSize = 36;

		/*
         * Get the amount of frames a WSA has.
         */
		internal static ushort WSA_GetFrameCount(/*WSAObject*/(WSAHeader header, CArray<byte> buffer) wsa)
		{
			WSAHeader header = wsa.header;

			if (header == null) return 0;
			return header.frames;
		}

		/*
		 * Unload the WSA.
		 * @param wsa The pointer to the WSA.
		 */
		internal static void WSA_Unload(/*WSAObject*/(WSAHeader header, CArray<byte> buffer) wsa)
		{
			WSAHeader header = wsa.header;

			if (wsa == (null, null)) return;
			if (!header.flags.malloced) return;

			wsa = (null, null); //free(wsa);
		}

		/*
         * Display a frame.
         * @param wsa The pointer to the WSA.
         * @param frameNext The next frame to display.
         * @param posX The X-position of the WSA.
         * @param posY The Y-position of the WSA.
         * @param screenID The screenID to draw on.
         * @return False on failure, true on success.
         */
		internal static bool WSA_DisplayFrame(/*WSAObject*/(WSAHeader header, CArray<byte> buffer) wsa, ushort frameNext, ushort posX, ushort posY, Screen screenID)
		{
			WSAHeader header = wsa.header;
			CArray<byte> dst;

			ushort i;
			ushort frame;
			short frameDiff;
			short direction;
			short frameCount;

			if (wsa == (null, null)) return false;
			if (frameNext >= header.frames) return false;

			if (header.flags.displayInBuffer)
			{
				dst = new CArray<byte> { Arr = wsa.buffer.Arr, Ptr = wsa.buffer.Ptr + WSAHeaderSize };
			}
			else
			{
				dst = new CArray<byte> { Arr = (byte[])Gfx.GFX_Screen_Get_ByIndex(screenID) };
				dst += (ushort)(posX + posY * Gfx.SCREEN_WIDTH); //dst.Ptr += (ushort)(posX + posY * Gfx.SCREEN_WIDTH);
			}

			if (header.frameCurrent == header.frames)
			{
				if (!header.flags.hasNoFirstFrame)
				{
					if (!header.flags.displayInBuffer)
					{
						Format40.Format40_Decode_ToScreen(dst.Arr, header.buffer.Arr, header.width, dst.Ptr, header.buffer.Ptr);
					}
					else
					{
						Format40.Format40_Decode(new CArray<byte>(dst), new CArray<byte>(header.buffer));
					}
				}

				header.frameCurrent = 0;
			}

			frameDiff = Abs((short)(header.frameCurrent - frameNext));
			direction = 1;

			if (frameNext > header.frameCurrent)
			{
				frameCount = (short)(header.frames - frameNext + header.frameCurrent);

				if (frameCount < frameDiff && !header.flags.noAnimation)
				{
					direction = -1;
				}
				else
				{
					frameCount = frameDiff;
				}
			}
			else
			{
				frameCount = (short)(header.frames - header.frameCurrent + frameNext);

				if (frameCount < frameDiff && !header.flags.noAnimation)
				{
				}
				else
				{
					direction = -1;
					frameCount = frameDiff;
				}
			}

			frame = header.frameCurrent;
			if (direction > 0)
			{
				for (i = 0; i < frameCount; i++)
				{
					frame += (ushort)direction;

					WSA_GotoNextFrame(wsa, frame, dst);

					if (frame == header.frames) frame = 0;
				}
			}
			else
			{
				for (i = 0; i < frameCount; i++)
				{
					if (frame == 0) frame = header.frames;

					WSA_GotoNextFrame(wsa, frame, dst);

					frame += (ushort)direction;
				}
			}

			header.frameCurrent = frameNext;

			if (header.flags.displayInBuffer)
			{
				WSA_DrawFrame((short)posX, (short)posY, (short)header.width, (short)header.height, 0, dst, screenID);
			}

			Gfx.GFX_Screen_SetDirty(screenID, posX, posY, (ushort)(posX + header.width), (ushort)(posY + header.height));
			return true;
		}

		/*
		 * Go to the next frame in the animation.
		 * @param wsa WSA pointer.
		 * @param frame Frame number to go to.
		 * @param dst Destination buffer to write the animation to.
		 * @return 1 on success, 0 on failure.
		 */
		static ushort WSA_GotoNextFrame(/*WSAObject*/(WSAHeader header, CArray<byte> buffer) wsa, ushort frame, CArray<byte> dst)
		{
			WSAHeader header = wsa.header;
			ushort lengthPalette;
			byte[] buffer;
			int bufferPointer = 0;

			lengthPalette = (ushort)(header.flags.hasPalette ? 0x300 : 0);

			buffer = header.buffer.Arr[header.buffer.Ptr..];

			if (header.flags.dataInMemory)
			{
				uint positionStart;
				uint positionEnd;
				uint length;
				byte[] positionFrame;

				positionStart = WSA_GetFrameOffset_FromMemory(header, frame);
				positionEnd = WSA_GetFrameOffset_FromMemory(header, (ushort)(frame + 1));
				length = positionEnd - positionStart;

				positionFrame = header.fileContent[(int)positionStart..];
				bufferPointer += (ushort)(header.bufferLength - length);

				Array.Copy(positionFrame, 0, buffer, bufferPointer, length); //memmove(buffer, positionFrame, length);
			}
			else if (header.flags.dataOnDisk)
			{
				byte fileno;
				uint positionStart;
				uint positionEnd;
				uint length;
				uint res;

				fileno = CFile.File_Open(header.filename, FileMode.FILE_MODE_READ);

				positionStart = WSA_GetFrameOffset_FromDisk(fileno, frame);
				positionEnd = WSA_GetFrameOffset_FromDisk(fileno, (ushort)(frame + 1));
				length = positionEnd - positionStart;

				if (positionStart == 0 || positionEnd == 0 || length == 0)
				{
					CFile.File_Close(fileno);
					return 0;
				}

				bufferPointer += (ushort)(header.bufferLength - length);

				CFile.File_Seek(fileno, (int)(positionStart + lengthPalette), 0);
				res = CFile.File_Read(fileno, ref buffer, length, bufferPointer);
				CFile.File_Close(fileno);

				if (res != length) return 0;
			}

			Format80.Format80_Decode(header.buffer.Arr, buffer, header.bufferLength, header.buffer.Ptr, bufferPointer);

			if (header.flags.displayInBuffer)
			{
				Format40.Format40_Decode(new CArray<byte>(dst), new CArray<byte>(header.buffer));
			}
			else
			{
				Format40.Format40_Decode_XorToScreen(dst.Arr, header.buffer.Arr, header.width, dst.Ptr, header.buffer.Ptr);
			}

			return 1;
		}

		/*
		 * Load a WSA file.
		 * @param filename Name of the file.
		 * @param wsa Data buffer for the WSA.
		 * @param wsaSize Current size of buffer.
		 * @param reserveDisplayFrame True if we need to reserve the display frame.
		 * @return Address of loaded WSA file, or NULL.
		 */
		internal static /*WSAObject*/(WSAHeader, CArray<byte>) WSA_LoadFile(string filename, /*WSAObject*/byte[] wsa, uint wsaSize, bool reserveDisplayFrame)
		{
			WSAFlags flags = new WSAFlags();
			WSAFileHeader fileheader = new WSAFileHeader();
			WSAHeader header = new WSAHeader();
			//WSAObject result = new WSAObject();
			uint bufferSizeMinimal;
			uint bufferSizeOptimal;
			ushort lengthHeader;
			byte fileno;
			ushort lengthPalette;
			ushort lengthFirstFrame;
			uint lengthFileContent;
			uint displaySize;
			int wsaPointer;

			//memset(&flags, 0, sizeof(flags));

			fileno = CFile.File_Open(filename, FileMode.FILE_MODE_READ);
			fileheader.frames = CFile.File_Read_LE16(fileno);
			fileheader.width = CFile.File_Read_LE16(fileno);
			fileheader.height = CFile.File_Read_LE16(fileno);
			fileheader.requiredBufferSize = CFile.File_Read_LE16(fileno);
			fileheader.hasPalette = CFile.File_Read_LE16(fileno);           /* has palette */
			fileheader.firstFrameOffset = CFile.File_Read_LE32(fileno);		/* Offset of 1st frame */
			fileheader.secondFrameOffset = CFile.File_Read_LE32(fileno);    /* Offset of 2nd frame (end of 1st frame) */

			lengthPalette = 0;
			if (fileheader.hasPalette != 0)
			{
				flags.hasPalette = true;

				lengthPalette = 0x300;  /* length of a 256 color RGB palette */
			}

			lengthFileContent = CFile.File_Seek(fileno, 0, 2);

			lengthFirstFrame = 0;
			if (fileheader.firstFrameOffset != 0)
			{
				lengthFirstFrame = (ushort)(fileheader.secondFrameOffset - fileheader.firstFrameOffset);
			}
			else
			{
				flags.hasNoFirstFrame = true;   /* is the continuation of another WSA */
			}

			lengthFileContent -= (uint)(lengthPalette + lengthFirstFrame + 10);

			displaySize = 0;
			if (reserveDisplayFrame)
			{
				flags.displayInBuffer = true;
				displaySize = (uint)(fileheader.width * fileheader.height);
			}

			bufferSizeMinimal = displaySize + fileheader.requiredBufferSize - 33 + WSAHeaderSize;
			bufferSizeOptimal = bufferSizeMinimal + lengthFileContent;

			if (wsaSize > 1 && wsaSize < bufferSizeMinimal)
			{
				CFile.File_Close(fileno);

				return (null, null);
			}
			if (wsaSize == 0) wsaSize = bufferSizeOptimal;
			if (wsaSize == 1) wsaSize = bufferSizeMinimal;

			if (wsa == null)
			{
				if (wsaSize == 0)
				{
					wsaSize = bufferSizeOptimal;
				}
				else if (wsaSize == 1)
				{
					wsaSize = bufferSizeMinimal;
				}
				else if (wsaSize >= bufferSizeOptimal)
				{
					wsaSize = bufferSizeOptimal;
				}
				else
				{
					wsaSize = bufferSizeMinimal;
				}

				//result.data = new CArray<byte> { Array = new byte[wsaSize] }; //calloc(1, wsaSize);
				wsa = new byte[wsaSize]; //calloc(1, wsaSize);
				flags.malloced = true;
			}
			else
			{
				flags.notmalloced = true;
			}

            //header = (WSAHeader*)wsa;
            //result.header = header;
            wsaPointer = WSAHeaderSize;

            header.flags = flags;

			if (reserveDisplayFrame)
			{
				Array.Fill<byte>(wsa, 0, wsaPointer, (int)displaySize); //memset(buffer, 0, displaySize);
			}

			wsaPointer += reserveDisplayFrame ? (ushort)displaySize : 1100; //TODO: Revisit

			if ((fileheader.frames & 0x8000) != 0)
			{
				fileheader.frames &= 0x7FFF;
			}

			header.frameCurrent = fileheader.frames;
			header.frames = fileheader.frames;
			header.width = fileheader.width;
			header.height = fileheader.height;
			header.bufferLength = (ushort)(fileheader.requiredBufferSize + 33 - WSAHeaderSize);
			header.buffer = new CArray<byte> { Arr = wsa, Ptr = wsaPointer };
			header.filename = filename; //strncpy(header->filename, filename, sizeof(header->filename));

			lengthHeader = (ushort)((fileheader.frames + 2) * 4);

			if (wsaSize >= bufferSizeOptimal)
			{
				header.fileContent = wsa[(wsaPointer + header.bufferLength)..];

				CFile.File_Seek(fileno, 10, 0);
				CFile.File_Read(fileno, ref header.fileContent, lengthHeader);
				CFile.File_Seek(fileno, lengthFirstFrame + lengthPalette, 1);
				CFile.File_Read(fileno, ref header.fileContent, lengthFileContent - lengthHeader, lengthHeader);

				header.flags.dataInMemory = true;
				if (WSA_GetFrameOffset_FromMemory(header, (ushort)(header.frames + 1)) == 0) header.flags.noAnimation = true;
			}
			else
			{
				header.flags.dataOnDisk = true;
				if (WSA_GetFrameOffset_FromDisk(fileno, (ushort)(header.frames + 1)) == 0) header.flags.noAnimation = true;
			}

			byte[] b = wsa[(wsaPointer + header.bufferLength - lengthFirstFrame)..];

			CFile.File_Seek(fileno, lengthHeader + lengthPalette + 10, 0);
			CFile.File_Read(fileno, ref b, lengthFirstFrame);
			CFile.File_Close(fileno);
			
			Format80.Format80_Decode(wsa, b, header.bufferLength, wsaPointer, 0);

			//result.data = new CArray<byte> { Array = wsa, Pointer = wsaPointer };

			return (header, new CArray<byte> { Arr = wsa });
		}

		/*
		 * Draw a frame on the buffer.
		 * @param x The X-position to start drawing.
		 * @param y The Y-position to start drawing.
		 * @param width The width of the image.
		 * @param height The height of the image.
		 * @param windowID The windowID.
		 * @param screenID the screen to write to
		 * @param src The source for the frame.
		 */
		static void WSA_DrawFrame(short x, short y, short width, short height, ushort windowID, /*byte[]*/CArray<byte> src, Screen screenID)
		{
			short left;
			short right;
			short top;
			short bottom;
			short skipBefore;
			short skipAfter;
			byte[] dst;
			//int srcPointer = 0;
			int dstPointer = 0;

			dst = (byte[])Gfx.GFX_Screen_Get_ByIndex(screenID);

			left = (short)(CWidget.g_widgetProperties[windowID].xBase << 3);
			right = (short)(left + (CWidget.g_widgetProperties[windowID].width << 3));
			top = (short)CWidget.g_widgetProperties[windowID].yBase;
			bottom = (short)(top + CWidget.g_widgetProperties[windowID].height);

			if (y - top < 0)
			{
				if (y - top + height <= 0) return;
				height += (short)(y - top);
				/*srcPointer*/src += (top - y) * width; //src.Ptr += (top - y) * width;
				y += (short)(top - y);
			}

			if (bottom - y <= 0) return;
			height = Min((short)(bottom - y), height);

			skipBefore = 0;
			if (x - left < 0)
			{
				skipBefore = (short)(left - x);
				x += skipBefore;
				width -= skipBefore;
			}

			skipAfter = 0;
			if (right - x <= 0) return;
			if (right - x < width)
			{
				skipAfter = (short)(width - right + x);
				width = (short)(right - x);
			}

			dstPointer += y * Gfx.SCREEN_WIDTH + x;

			while (height-- != 0)
			{
				/*srcPointer*/src += skipBefore; //src.Ptr += skipBefore;
				Array.Copy(src.Arr, /*srcPointer*/src.Ptr, dst, dstPointer, width); //memcpy(dst, src, width);

				//src.Arr.AsSpan(src.Ptr, width).CopyTo(dst.AsSpan(dstPointer, width));

				/*srcPointer*/src += width + skipAfter; //src.Ptr += width + skipAfter;
				dstPointer += Gfx.SCREEN_WIDTH;
			}
		}

		/*
		 * Get the offset in the fileContent which stores the animation data for a
		 *  given frame.
		 * @param header The header of the WSA.
		 * @param frame The frame of animation.
		 * @return The offset for the animation from the beginning of the fileContent.
		 */
		static uint WSA_GetFrameOffset_FromMemory(WSAHeader header, ushort frame)
		{
			ushort lengthAnimation = 0;
			uint animationFrame;
			uint animation0;

			animationFrame = Endian.READ_LE_UINT32(header.fileContent[(frame * 4)..]);

			if (animationFrame == 0) return 0;

			animation0 = Endian.READ_LE_UINT32(header.fileContent);
			if (animation0 != 0)
			{
				lengthAnimation = (ushort)(Endian.READ_LE_UINT32(header.fileContent[4..]) - animation0);
			}

			return animationFrame - lengthAnimation - 10;
		}

		/*
		 * Get the offset in the file which stores the animation data for a given
		 *  frame.
		 * @param fileno The fileno of an opened WSA.
		 * @param frame The frame of animation.
		 * @return The offset for the animation from the beginning of the file.
		 */
		static uint WSA_GetFrameOffset_FromDisk(byte fileno, ushort frame)
		{
			uint offset;

			CFile.File_Seek(fileno, frame * 4 + 10, 0);
			offset = CFile.File_Read_LE32(fileno);

			return offset;
		}
	}
}
