/* WSA */

namespace SharpDune;

/*
 * The flags of a WSA Header.
 */
class WSAFlags
{
    internal bool notmalloced;                                   /*!< If the WSA is in memory of the caller. */
    internal bool malloced;                                      /*!< If the WSA is malloc'd by us. */
    internal bool dataOnDisk;                                    /*!< Only the header is in the memory. Rest is on disk. */
    internal bool dataInMemory;                                  /*!< The whole WSA is in memory. */
    internal bool displayInBuffer;                               /*!< The output display is in the buffer. */
    internal bool noAnimation;                                   /*!< If the WSA has animation or not. */
    internal bool hasNoFirstFrame;                               /*!< The WSA is the continuation of another one. */
    internal bool hasPalette;                                    /*!< Indicates if the WSA has a palette stored. */
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
    internal Array<byte> buffer;                            /*!< The buffer. */
    internal Memory<byte> fileContent;                      /*!< The content of the file. */
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

//class WSAObject
//{
//    internal WSAHeader header;
//    internal Array<byte> data;
//}

static class Wsa
{
    const int WSAHeaderSize = 36;

    /*
     * Get the amount of frames a WSA has.
     */
    internal static ushort WSA_GetFrameCount(/*WSAObject*/(WSAHeader header, Array<byte> buffer) wsa)
    {
        var header = wsa.header;

        if (header == null) return 0;
        return header.frames;
    }

    /*
     * Unload the WSA.
     * @param wsa The pointer to the WSA.
     */
    internal static void WSA_Unload(/*WSAObject*/(WSAHeader header, Array<byte> buffer) wsa)
    {
        var header = wsa.header;

        if (wsa == (null, null)) return;
        if (!header.flags.malloced) return;

        wsa.header = null;
        wsa.buffer = null; //free(wsa);
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
    internal static bool WSA_DisplayFrame(/*WSAObject*/(WSAHeader header, Array<byte> buffer) wsa, ushort frameNext, ushort posX, ushort posY, Screen screenID)
    {
        var header = wsa.header;
        Array<byte> dst;

        ushort i;
        ushort frame;
        short frameDiff;
        short direction;
        short frameCount;

        if (wsa == (null, null)) return false;
        if (frameNext >= header.frames) return false;

        if (header.flags.displayInBuffer)
        {
            dst = new Array<byte>(wsa.buffer.Arr, wsa.buffer.Ptr + WSAHeaderSize);
        }
        else
        {
            dst = new Array<byte>(GFX_Screen_Get_ByIndex(screenID));
            dst += (ushort)(posX + (posY * SCREEN_WIDTH));
        }

        if (header.frameCurrent == header.frames)
        {
            if (!header.flags.hasNoFirstFrame)
            {
                if (!header.flags.displayInBuffer)
                {
                    Format40_Decode_ToScreen(new Array<byte>(dst), new Array<byte>(header.buffer), header.width);
                }
                else
                {
                    Format40_Decode(new Array<byte>(dst), new Array<byte>(header.buffer));
                }
            }

            header.frameCurrent = 0;
        }

        frameDiff = Math.Abs((short)(header.frameCurrent - frameNext));
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

            if (frameCount >= frameDiff || header.flags.noAnimation)
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

        GFX_Screen_SetDirty(screenID, posX, posY, (ushort)(posX + header.width), (ushort)(posY + header.height));
        return true;
    }

    /*
     * Go to the next frame in the animation.
     * @param wsa WSA pointer.
     * @param frame Frame number to go to.
     * @param dst Destination buffer to write the animation to.
     * @return 1 on success, 0 on failure.
     */
    static ushort WSA_GotoNextFrame(/*WSAObject*/(WSAHeader header, Array<byte> buffer) wsa, ushort frame, Array<byte> dst)
    {
        var header = wsa.header;
        ushort lengthPalette;
        Array<byte> buffer;

        lengthPalette = (ushort)(header.flags.hasPalette ? 0x300 : 0);

        buffer = new Array<byte>(header.buffer.Arr, header.buffer.Ptr);

        if (header.flags.dataInMemory)
        {
            uint positionStart;
            uint positionEnd;
            uint length;
            Memory<byte> positionFrame;

            positionStart = WSA_GetFrameOffset_FromMemory(header, frame);
            positionEnd = WSA_GetFrameOffset_FromMemory(header, (ushort)(frame + 1));
            length = positionEnd - positionStart;

            positionFrame = header.fileContent.Slice((int)positionStart);
            buffer += (int)(header.bufferLength - length);

            positionFrame.Slice(0, (int)length).CopyTo(buffer.Arr.Slice(buffer.Ptr)); //memmove(buffer, positionFrame, length);
        }
        else if (header.flags.dataOnDisk)
        {
            byte fileno;
            uint positionStart;
            uint positionEnd;
            uint length;
            uint res;

            fileno = File_Open(header.filename, FileMode.FILE_MODE_READ);

            positionStart = WSA_GetFrameOffset_FromDisk(fileno, frame);
            positionEnd = WSA_GetFrameOffset_FromDisk(fileno, (ushort)(frame + 1));

            length = positionEnd - positionStart;

            if (positionStart == 0 || positionEnd == 0 || length == 0)
            {
                File_Close(fileno);
                return 0;
            }

            buffer += (int)(header.bufferLength - length);

            File_Seek(fileno, (int)(positionStart + lengthPalette), 0);
            res = File_Read(fileno, ref buffer, length);
            File_Close(fileno);

            if (res != length) return 0;
        }

        Format80_Decode(header.buffer.Arr.Span, buffer.Arr.Span, header.bufferLength, header.buffer.Ptr, buffer.Ptr);

        if (header.flags.displayInBuffer)
        {
            Format40_Decode(new Array<byte>(dst), new Array<byte>(header.buffer));
        }
        else
        {
            Format40_Decode_XorToScreen(new Array<byte>(dst), new Array<byte>(header.buffer), header.width);
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
    internal static /*WSAObject*/(WSAHeader, Array<byte>) WSA_LoadFile(string filename, /*WSAObject*/Memory<byte> wsa, uint wsaSize, bool reserveDisplayFrame)
    {
        var flags = new WSAFlags();
        var fileheader = new WSAFileHeader();
        var header = new WSAHeader();
        //WSAObject result = new WSAObject();
        uint bufferSizeMinimal;
        uint bufferSizeOptimal;
        ushort lengthHeader;
        byte fileno;
        ushort lengthPalette;
        ushort lengthFirstFrame;
        uint lengthFileContent;
        uint displaySize;
        Memory<byte> buffer;

        //memset(&flags, 0, sizeof(flags));

        fileno = File_Open(filename, FileMode.FILE_MODE_READ);
        fileheader.frames = File_Read_LE16(fileno);
        fileheader.width = File_Read_LE16(fileno);
        fileheader.height = File_Read_LE16(fileno);
        fileheader.requiredBufferSize = File_Read_LE16(fileno);
        fileheader.hasPalette = File_Read_LE16(fileno);         /* has palette */
        fileheader.firstFrameOffset = File_Read_LE32(fileno);   /* Offset of 1st frame */
        fileheader.secondFrameOffset = File_Read_LE32(fileno);  /* Offset of 2nd frame (end of 1st frame) */

        lengthPalette = 0;
        if (fileheader.hasPalette != 0)
        {
            flags.hasPalette = true;

            lengthPalette = 0x300;  /* length of a 256 color RGB palette */
        }

        lengthFileContent = File_Seek(fileno, 0, 2);

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
            File_Close(fileno);

            return (null, null);
        }
        if (wsaSize == 0) wsaSize = bufferSizeOptimal;
        if (wsaSize == 1) wsaSize = bufferSizeMinimal;

        if (wsa.IsEmpty)
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

            wsa = new byte[wsaSize]; //calloc(1, wsaSize);
            flags.malloced = true;
        }
        else
        {
            flags.notmalloced = true;
        }

        //header = (WSAHeader *)wsa;
        buffer = wsa.Slice(WSAHeaderSize); //(uint8 *)wsa + sizeof(WSAHeader);

        header.flags = flags;

        if (reserveDisplayFrame)
        {
            buffer.Span.Slice(0, (int)displaySize).Clear(); //memset(buffer, 0, displaySize);
        }

        buffer = buffer.Slice((int)displaySize);

        if ((fileheader.frames & 0x8000) != 0)
        {
            fileheader.frames &= 0x7FFF;
        }

        header.frameCurrent = fileheader.frames;
        header.frames = fileheader.frames;
        header.width = fileheader.width;
        header.height = fileheader.height;
        header.bufferLength = (ushort)(fileheader.requiredBufferSize + 33 - WSAHeaderSize);
        header.buffer = new Array<byte>(buffer);
        header.filename = filename; //strncpy(header->filename, filename, sizeof(header->filename));

        lengthHeader = (ushort)((fileheader.frames + 2) * 4);

        if (wsaSize >= bufferSizeOptimal)
        {
            header.fileContent = buffer.Slice(header.bufferLength);

            File_Seek(fileno, 10, 0);
            File_Read(fileno, ref header.fileContent, lengthHeader);
            File_Seek(fileno, lengthFirstFrame + lengthPalette, 1);
            File_Read(fileno, ref header.fileContent, lengthFileContent - lengthHeader, lengthHeader);

            header.flags.dataInMemory = true;
            if (WSA_GetFrameOffset_FromMemory(header, (ushort)(header.frames + 1)) == 0) header.flags.noAnimation = true;
        }
        else
        {
            header.flags.dataOnDisk = true;
            if (WSA_GetFrameOffset_FromDisk(fileno, (ushort)(header.frames + 1)) == 0) header.flags.noAnimation = true;
        }

        {
            var b = buffer.Slice(header.bufferLength - lengthFirstFrame);

            File_Seek(fileno, lengthHeader + lengthPalette + 10, 0);
            File_Read(fileno, ref b, lengthFirstFrame);
            File_Close(fileno);

            Format80_Decode(buffer.Span, b.Span, header.bufferLength);
        }
        return (header, new Array<byte>(wsa));
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
    static void WSA_DrawFrame(short x, short y, short width, short height, ushort windowID, Array<byte> src, Screen screenID)
    {
        short left;
        short right;
        short top;
        short bottom;
        short skipBefore;
        short skipAfter;
        var dst = new Array<byte>(GFX_Screen_Get_ByIndex(screenID));

        left = (short)(g_widgetProperties[windowID].xBase << 3);
        right = (short)(left + (g_widgetProperties[windowID].width << 3));
        top = (short)g_widgetProperties[windowID].yBase;
        bottom = (short)(top + g_widgetProperties[windowID].height);

        if (y < top)
        {
            if (y - top + height <= 0) return;
            height += (short)(y - top);
            src += (top - y) * width;
            y += (short)(top - y);
        }

        if (bottom <= y) return;
        height = Math.Min((short)(bottom - y), height);

        skipBefore = 0;
        if (x < left)
        {
            skipBefore = (short)(left - x);
            x += skipBefore;
            width -= skipBefore;
        }

        skipAfter = 0;
        if (right <= x) return;
        if (right - x < width)
        {
            skipAfter = (short)(width - right + x);
            width = (short)(right - x);
        }

        dst += (y * SCREEN_WIDTH) + x;

        while (height-- != 0)
        {
            src += skipBefore;
            src.Arr.Slice(src.Ptr, width).CopyTo(dst.Arr.Slice(dst.Ptr)); //memcpy(dst, src, width);
            src += width + skipAfter;
            dst += SCREEN_WIDTH;
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

        animationFrame = Read_LE_UInt32(header.fileContent.Span.Slice(frame * 4));

        if (animationFrame == 0) return 0;

        animation0 = Read_LE_UInt32(header.fileContent.Span);
        if (animation0 != 0)
        {
            lengthAnimation = (ushort)(Read_LE_UInt32(header.fileContent.Span.Slice(4)) - animation0);
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
        File_Seek(fileno, (frame * 4) + 10, 0);
        return File_Read_LE32(fileno);
    }
}
