﻿/* Sprite */

//#define Sprites_0
//#define Sprites_1

namespace SharpDune;

/*
 * The \c ICON.MAP contains indices only. An index can point either to another
 * index or to a spriteID in the tiles file, as follows.
 *  - Index 0 contain the number of icon groups (including the EOF entry).
 *  - Each index in 1 . . . number_of_icongroups-1 points to the first spriteID of a icon group.
 *  - Index number_of_icongroups is 0, meaning 'the index at EOF'.
 *
 * Icon group at index i contains sprite indices. The first one is pointed to by
 * index i, the last one is one entry before the start of icon group i+1 (where 0
 * means EOF, as explained already).
 */
enum IconMapEntries
{
    ICM_ICONGROUP_COUNT,                 /*!< Number of icon groups. */
    /* Icon groups. */
    ICM_ICONGROUP_ROCK_CRATERS = 1, /*!< Rock craters spriteIDs. */
    ICM_ICONGROUP_SAND_CRATERS = 2, /*!< Sand craters spriteIDs. */
    ICM_ICONGROUP_FLY_MACHINES_CRASH = 3, /*!< Carry-all / thopter crash and craters spriteIDs. */
    ICM_ICONGROUP_SAND_DEAD_BODIES = 4, /*!< Dead bodies in the sand spriteIDs. */
    ICM_ICONGROUP_SAND_TRACKS = 5, /*!< Tracks in the sand spriteIDs. */
    ICM_ICONGROUP_WALLS = 6, /*!< Wall parts spriteIDs. */
    ICM_ICONGROUP_FOG_OF_WAR = 7, /*!< Fog of war spriteIDs. */
    ICM_ICONGROUP_CONCRETE_SLAB = 8, /*!< Concrete slab spriteIDs. */
    ICM_ICONGROUP_LANDSCAPE = 9, /*!< Landscape spriteIDs. */
    ICM_ICONGROUP_SPICE_BLOOM = 10, /*!< Spice bloom spriteIDs. */
    ICM_ICONGROUP_HOUSE_PALACE = 11, /*!< Palace spriteIDs. */
    ICM_ICONGROUP_LIGHT_VEHICLE_FACTORY = 12, /*!< Light vehicles factory spriteIDs. */
    ICM_ICONGROUP_HEAVY_VEHICLE_FACTORY = 13, /*!< Heavy vehicles factory spriteIDs. */
    ICM_ICONGROUP_HI_TECH_FACTORY = 14, /*!< Hi-tech factory spriteIDs. */
    ICM_ICONGROUP_IX_RESEARCH = 15, /*!< IX Research facility spriteIDs. */
    ICM_ICONGROUP_WOR_TROOPER_FACILITY = 16, /*!< WOR trooper facility spriteIDs. */
    ICM_ICONGROUP_CONSTRUCTION_YARD = 17, /*!< Construction yard spriteIDs. */
    ICM_ICONGROUP_INFANTRY_BARRACKS = 18, /*!< Infantry barracks spriteIDs. */
    ICM_ICONGROUP_WINDTRAP_POWER = 19, /*!< Windtrap facility spriteIDs. */
    ICM_ICONGROUP_STARPORT_FACILITY = 20, /*!< Starport facility spriteIDs. */
    ICM_ICONGROUP_SPICE_REFINERY = 21, /*!< Spice refinery spriteIDs. */
    ICM_ICONGROUP_VEHICLE_REPAIR_CENTRE = 22, /*!< Repair center spriteIDs. */
    ICM_ICONGROUP_BASE_DEFENSE_TURRET = 23, /*!< Gun turret spriteIDs. */
    ICM_ICONGROUP_BASE_ROCKET_TURRET = 24, /*!< Rocket turret spriteIDs. */
    ICM_ICONGROUP_SPICE_STORAGE_SILO = 25, /*!< Spice storage spriteIDs. */
    ICM_ICONGROUP_RADAR_OUTPOST = 26, /*!< Radar outpost spriteIDs. */
    ICM_ICONGROUP_EOF = 27  /*!< End of file spriteIDs. */
}

static class Sprites
{
    internal static /* uint8** */byte[][] g_sprites;
    static ushort s_spritesCount;

    internal static /* void * */byte[] g_mouseSprite;
    internal static /* void * */byte[] g_mouseSpriteBuffer;

    static ushort s_mouseSpriteSize;
    static ushort s_mouseSpriteBufferSize;

    internal static ushort g_veiledTileID;     /*!< TileID of the veiled tile, at the end of the partily veiled tiles. */
    internal static ushort g_bloomTileID;      /*!< First bloom field ID. */
    internal static ushort g_landscapeTileID;  /*!< First landscape ID. */
    internal static ushort g_builtSlabTileID;  /*!< built concrete slab. */
    internal static ushort g_wallTileID;       /*!< First wall. */

    /* for tiles loaded from ICON.ICN */
    internal static byte[] g_iconRTBL;  /* table to give spriteID => palette index*/
    internal static byte[] g_iconRPAL;  /* palettes for "icons" tiles. Each palette is 16 colors (8bits index into the main palette) */
    internal static byte[] g_tilesPixels;   /* 2 pixels per byte */

    internal static ushort[] g_iconMap;

    static bool s_iconLoaded;

    internal static Memory<byte> g_fileRgnclkCPS;
    internal static string g_fileRegionINI;
    internal static ushort[] g_regions;

    internal static void Sprites_SetMouseSprite(ushort hotSpotX, ushort hotSpotY, Span<byte> sprite)
    {
        ushort size;
        var spritePointer = 0;

        if (sprite == default || g_mouseDisabled != 0) return;

        while (g_mouseLock != 0) SleepIdle();

        g_mouseLock++;

        GUI_Mouse_Hide();

        size = GFX_GetSize((short)(Read_LE_UInt16(sprite.Slice(3)) + 16), sprite[5]);

        if (s_mouseSpriteBufferSize < size)
        {
            Array.Resize(ref g_mouseSpriteBuffer, size); //g_mouseSpriteBuffer = realloc(g_mouseSpriteBuffer, size);
            s_mouseSpriteBufferSize = size;
        }

        size = (ushort)(Read_LE_UInt16(sprite.Slice(8)) + 10);
        if ((sprite[spritePointer] & 0x1) != 0) size += 16;

        if (s_mouseSpriteSize < size)
        {
            Array.Resize(ref g_mouseSprite, size); //g_mouseSprite = realloc(g_mouseSprite, size);
            s_mouseSpriteSize = size;
        }

        if ((sprite[spritePointer] & 0x2) != 0)
        {
            sprite.Slice(spritePointer, Read_LE_UInt16(sprite.Slice(6))).CopyTo(g_mouseSprite); //memcpy(g_mouseSprite, sprite, READ_LE_UINT16(sprite + 6));
        }
        else
        {
            var dst = g_mouseSprite.AsSpan();
            var flags = (byte)(sprite[0] | 0x2);
            var dstPointer = 0;

            dst[0] = flags;
            dst[1] = sprite[1];
            dstPointer += 2;
            spritePointer += 2;

            sprite.Slice(spritePointer, 6).CopyTo(dst.Slice(dstPointer)); //memcpy(dst, sprite, 6);
            dstPointer += 6;
            spritePointer += 6;

            size = Read_LE_UInt16(sprite);
            dst[0] = sprite[0];
            dst[1] = sprite[1];
            dstPointer += 2;
            spritePointer += 2;

            if ((flags & 0x1) != 0)
            {
                sprite.Slice(spritePointer, 16).CopyTo(dst.Slice(dstPointer)); //memcpy(dst, sprite, 16);
                dstPointer += 16;
                spritePointer += 16;
            }

            Format80_Decode(dst, sprite, size, dstPointer, spritePointer);
        }

        g_mouseSpriteHotspotX = hotSpotX;
        g_mouseSpriteHotspotY = hotSpotY;

        sprite = g_mouseSprite;
        g_mouseHeight = sprite[5];
        g_mouseWidth = (ushort)((Read_LE_UInt16(sprite.Slice(3)) >> 3) + 2);

        GUI_Mouse_Show();

        g_mouseLock--;
    }

    /*
     * Check if a spriteID is part of the veiling sprites.
     * @param spriteID The sprite to check for.
     * @return True if and only if the spriteID is part of the veiling sprites.
     */
    internal static bool Tile_IsUnveiled(ushort tileID)
    {
        if (tileID > g_veiledTileID) return true;
        if (tileID < g_veiledTileID - 15) return true;

        return false;
    }

    /*
     * Loads an image.
     *
     * @param filename The name of the file to load.
     * @param memory1 The index of a memory block where to store loaded data.
     * @param memory2 The index of a memory block where to store loaded data.
     * @param palette Where to store the palette, if any.
     * @return The size of the loaded image.
     */
    internal static ushort Sprites_LoadImage(string filename, Screen screenID, byte[] palette) =>
#if Sprites_0
        byte index;
        byte[] header = default;

        index = CFile.File_Open(filename, FileMode.FILE_MODE_READ);
        if (index == (byte)FileMode.FILE_INVALID) return 0;

        CFile.File_Read(index, header, 4);
        CFile.File_Close(index);
#endif
        (ushort)(Sprites_LoadCPSFile(filename, screenID, palette) / 8000);

    /*
     * Loads a CPS file.
     *
     * @param filename The name of the file to load.
     * @param screenID The index of a memory block where to store loaded data.
     * @param palette Where to store the palette, if any.
     * @return The size of the loaded image.
     */
    static uint Sprites_LoadCPSFile(string filename, Screen screenID, byte[] palette)
    {
        byte index;
        ushort size;
        Memory<byte> buffer;
        Memory<byte> buffer2;
        var buffer2Pointer = 0;
        ushort paletteSize;

        buffer = GFX_Screen_Get_ByIndex(screenID);

        index = File_Open(filename, FileMode.FILE_MODE_READ);

        size = File_Read_LE16(index);

        File_Read(index, ref buffer, 8);

        size -= 8;

        paletteSize = Read_LE_UInt16(buffer.Span.Slice(6));

        if (palette != null && paletteSize != 0)
        {
            File_Read(index, ref palette, paletteSize);
        }
        else
        {
            File_Seek(index, paletteSize, 1);
        }

        buffer.Span[6] = 0;  /* dont read palette next time */
        buffer.Span[7] = 0;
        size -= paletteSize;

        buffer2 = GFX_Screen_Get_ByIndex(screenID);
        buffer2Pointer += (ushort)(GFX_Screen_GetSize_ByIndex(screenID) - size - 8);

        buffer.Slice(0, 8).CopyTo(buffer2.Slice(buffer2Pointer)); //memmove(buffer2, buffer, 8);
        File_Read(index, ref buffer2, size, buffer2Pointer + 8);

        File_Close(index);

        return Sprites_Decode(buffer2.Span.Slice(buffer2Pointer), buffer.Span);
    }

    /*
     * Decodes an image.
     *
     * @param source The encoded image.
     * @param dest The place the decoded image will be.
     * @return The size of the decoded image.
     */
    static uint Sprites_Decode(Span<byte> source, Span<byte> dest)
    {
        uint size = 0;
        var sourcePointer = 0;

        switch (source[0])
        {
            case 0x0:
                sourcePointer += 2;
                size = Read_LE_UInt32(source.Slice(sourcePointer));
                sourcePointer += 4;
                sourcePointer += Read_LE_UInt16(source.Slice(sourcePointer));
                sourcePointer += 2;
                source.Slice(sourcePointer, (int)size).CopyTo(dest); //memmove(dest, source, size);
                break;

            case 0x4:
                sourcePointer += 6;
                sourcePointer += Read_LE_UInt16(source.Slice(sourcePointer));
                sourcePointer += 2;
                size = Format80_Decode(dest, source, 0xFFFF, 0, sourcePointer);
                break;

            default: break;
        }

        return size;
    }

    /*
     * Loads the sprites for tiles.
     */
    internal static void Sprites_LoadTiles()
    {
        if (s_iconLoaded) return;

        s_iconLoaded = true;

        Tiles_LoadICNFile("ICON.ICN");

        g_iconMap = null; //free(g_iconMap);
        g_iconMap = File_ReadWholeFileLE16("ICON.MAP");

        g_veiledTileID = g_iconMap[g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_FOG_OF_WAR] + 16];
        g_bloomTileID = g_iconMap[g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_SPICE_BLOOM]];
        g_builtSlabTileID = g_iconMap[g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_CONCRETE_SLAB] + 2];
        g_landscapeTileID = g_iconMap[g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_LANDSCAPE]];
        g_wallTileID = g_iconMap[g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_WALLS]];

        Script_LoadFromFile("UNIT.EMC", g_scriptUnit, g_scriptFunctionsUnit, GFX_Screen_Get_ByIndex(Screen.NO2));
    }

    /*
     * Unloads the sprites for tiles.
     */
    internal static void Sprites_UnloadTiles() =>
        s_iconLoaded = false;

    /*
     * Loads the sprites.
     *
     * @param index The index of the list of sprite files to load.
     * @param sprites The array where to store CSIP for each loaded sprite.
     */
    static void Sprites_Load(string filename)
    {
        byte[] buffer;
        ushort count;
        ushort i;
        ushort size;

        buffer = File_ReadWholeFile(filename);

        count = Read_LE_UInt16(buffer);

        s_spritesCount += count;
        Array.Resize(ref g_sprites, s_spritesCount); //g_sprites = (uint8 **)realloc(g_sprites, s_spritesCount * sizeof(uint8 *));

        for (i = 0; i < count; i++)
        {
            var src = Sprites_GetSprite(buffer, i);
            byte[] dst = null;

            Debug.WriteLine($"DEBUG: Sprites {filename} {i} : {Read_LE_UInt16(src)} {Read_LE_UInt16(src.Slice(3)) /* flags */} {src[2] /* width */} {src[5] /* height */} {Read_LE_UInt16(src.Slice(6)) /* packed size */} {Read_LE_UInt16(src.Slice(8)) /* decoded size */}");
            if (src != default)
            {
                if (g_unpackSHPonLoad && (src[0] & 0x2) == 0)
                {
                    size = (ushort)(Read_LE_UInt16(src.Slice(8)) + 10);
                    if ((Read_LE_UInt16(src) & 0x1) != 0)
                    {
                        size += 16; /* 16 bytes more for the palette */
                    }
                    dst = new byte[size]; //(uint8 *)malloc(size);
                    var encoded_data = src;
                    var decoded_data = dst.AsSpan();
                    var encoded_dataPointer = 0;
                    var decoded_dataPointer = 0;
                    decoded_data[decoded_dataPointer++] = (byte)(encoded_data[encoded_dataPointer++] | 0x2);    /* the sprite is not Format80 encoded any more */
                    encoded_data.Slice(encoded_dataPointer, 5).CopyTo(decoded_data.Slice(decoded_dataPointer)); //memcpy(decoded_data, encoded_data, 5);
                    decoded_dataPointer += 5;
                    Write_LE_UInt16(decoded_data, size, decoded_dataPointer);  /* new packed size */
                    decoded_dataPointer += 2;
                    encoded_dataPointer += 7;
                    decoded_data[decoded_dataPointer++] = encoded_data[encoded_dataPointer++];    /* copy pixel size */
                    decoded_data[decoded_dataPointer++] = encoded_data[encoded_dataPointer++];
                    if ((Read_LE_UInt16(src) & 0x1) != 0)
                    {
                        encoded_data.Slice(encoded_dataPointer, 16).CopyTo(decoded_data.Slice(decoded_dataPointer)); //memcpy(decoded_data, encoded_data, 16);	/* copy palette */
                        decoded_dataPointer += 16;
                        encoded_dataPointer += 16;
                    }
                    Format80_Decode(decoded_data, encoded_data, Read_LE_UInt16(src.Slice(8)), decoded_dataPointer, encoded_dataPointer);
                }
                else
                {
                    size = Read_LE_UInt16(src.Slice(6)); /* "packed" size */
                    dst = new byte[size]; //(uint8 *)malloc(size);
                    src.Slice(0, size).CopyTo(dst); //memcpy(dst, src, size);
                }
            }

            g_sprites[s_spritesCount - count + i] = dst;
        }

        //buffer = null; //free(buffer);
    }

    internal static void Sprites_Init()
    {
        Sprites_Load("MOUSE.SHP");                               /*   0 -   6 */
        Sprites_Load(String_GenerateFilename("BTTN"));           /*   7 -  11 */
        Sprites_Load("SHAPES.SHP");                              /*  12 - 110 */
        Sprites_Load("UNITS2.SHP");                              /* 111 - 150 */
        Sprites_Load("UNITS1.SHP");                              /* 151 - 237 */
        Sprites_Load("UNITS.SHP");                               /* 238 - 354 */
        Sprites_Load(String_GenerateFilename("CHOAM"));          /* 355 - 372 */
        Sprites_Load(String_GenerateFilename("MENTAT"));         /* 373 - 386 */
        Sprites_Load("MENSHPH.SHP");                             /* 387 - 401 */
        Sprites_Load("MENSHPA.SHP");                             /* 402 - 416 */
        Sprites_Load("MENSHPO.SHP");                             /* 417 - 431 */
        Sprites_Load("MENSHPM.SHP");                             /* 432 - 446 (Placeholder - Fremen) */
        Sprites_Load("MENSHPM.SHP");                             /* 447 - 461 (Placeholder - Sardaukar) */
        Sprites_Load("MENSHPM.SHP");                             /* 462 - 476 */
        Sprites_Load("PIECES.SHP");                              /* 477 - 504 */
        Sprites_Load("ARROWS.SHP");                              /* 505 - 513 */
        Sprites_Load("CREDIT1.SHP");                             /* 514 */
        Sprites_Load("CREDIT2.SHP");                             /* 515 */
        Sprites_Load("CREDIT3.SHP");                             /* 516 */
        Sprites_Load("CREDIT4.SHP");                             /* 517 */
        Sprites_Load("CREDIT5.SHP");                             /* 518 */
        Sprites_Load("CREDIT6.SHP");                             /* 519 */
        Sprites_Load("CREDIT7.SHP");                             /* 520 */
        Sprites_Load("CREDIT8.SHP");                             /* 521 */
        Sprites_Load("CREDIT9.SHP");                             /* 522 */
        Sprites_Load("CREDIT10.SHP");                            /* 523 */
        Sprites_Load("CREDIT11.SHP");                            /* 524 */
    }

    internal static void Sprites_Uninit()
    {
        ushort i;

        for (i = 0; i < s_spritesCount; i++) g_sprites[i] = null; //free(g_sprites[i]);
        g_sprites = null; //free(g_sprites);

        g_mouseSpriteBuffer = null; //free(g_mouseSpriteBuffer);
        g_mouseSprite = null; //free(g_mouseSprite);

        g_tilesPixels = null; //free(g_tilesPixels);
        g_iconRTBL = null; //free(g_iconRTBL);
        g_iconRPAL = null; //free(g_iconRPAL);

        g_iconMap = null; //free(g_iconMap);
    }

    /*
     * Gets the width of the given sprite.
     *
     * @param sprite The sprite.
     * @return The width.
     */
    internal static byte Sprite_GetWidth(byte[] sprite)
    {
        if (sprite == null) return 0;

        return sprite[3];
    }

    /*
     * Gets the height of the given sprite.
     *
     * @param sprite The sprite.
     * @return The height.
     */
    internal static byte Sprite_GetHeight(byte[] sprite)
    {
        if (sprite == null) return 0;

        return sprite[2];
    }

    static void InitRegions()
    {
        //ushort[] regions = g_regions;
        ushort i;
        var textBuffer = Ini_GetString("INFO", "TOTAL REGIONS", null, g_fileRegionINI); //char[81]

        g_regions[0] = ushort.Parse(textBuffer, Culture); //sscanf(textBuffer, "%hu", &regions[0]);

        for (i = 0; i < g_regions[0]; i++) g_regions[i + 1] = 0xFFFF;
    }

    internal static void Sprites_CPS_LoadRegionClick()
    {
        Memory<byte> buf;
        byte i;
        string filename; //char[16];
        var bufPointer = 0;

        buf = GFX_Screen_Get_ByIndex(Screen.NO2);

        g_fileRgnclkCPS = buf;
        Sprites_LoadCPSFile("RGNCLK.CPS", Screen.NO2, null);
        for (i = 0; i < 120; i++) buf.Slice(7688 + (i * 320), 304).CopyTo(buf.Slice(i * 304)); //memcpy(buf + (i * 304), buf + 7688 + (i * 320), 304);
        bufPointer += 120 * 304;

        filename = $"REGION{g_table_houseInfo[(int)g_playerHouseID].name[0]}.INI"; //snprintf(filename, sizeof(filename), "REGION%c.INI", g_table_houseInfo[g_playerHouseID].name[0]);
        var length = (int)File_ReadFile(filename, buf, bufPointer);
        g_fileRegionINI = SharpDune.Encoding.GetString(buf.Span.Slice(bufPointer, length));
        bufPointer += length;

        g_regions = FromByteArrayToUshortArray(buf.Span.Slice(bufPointer));

        InitRegions();
    }

    /*
     * Gets the given sprite inside the given buffer.
     *
     * @param buffer The buffer containing sprites.
     * @param index The index of the sprite to get.
     * @return The sprite.
     */
    static Span<byte> Sprites_GetSprite(Span<byte> buffer, ushort index)
    {
        uint offset;
        var bufferPointer = 0;

        if (buffer == default) return null;
        if (Read_LE_UInt16(buffer) <= index) return null;

        bufferPointer += 2;

        offset = Read_LE_UInt32(buffer.Slice(bufferPointer + (4 * index)));

        if (offset == 0) return null;

        return buffer.Slice(bufferPointer + (int)offset);
    }

    /*
     * Loads an ICN file.
     * NOTE : should be called "tiles"
     *
     * @param filename The name of the file to load.
     * @param screenID The index of a memory block where to store loaded sprites.
     */
    static void Tiles_LoadICNFile(string filename)
    {
        byte fileIndex;

        uint tilesDataLength;
        uint tableLength;
        uint paletteLength;
        var info = new sbyte[4];

        fileIndex = ChunkFile_Open(filename);

        /* Get the length of the chunks */
        tilesDataLength = ChunkFile_Seek(fileIndex, HToBE32((uint)SharpDune.MultiChar[FourCC.SSET]));
        tableLength = ChunkFile_Seek(fileIndex, HToBE32((uint)SharpDune.MultiChar[FourCC.RTBL]));
        paletteLength = ChunkFile_Seek(fileIndex, HToBE32((uint)SharpDune.MultiChar[FourCC.RPAL]));

        /* Read the header information */
        ChunkFile_Read(fileIndex, HToBE32((uint)SharpDune.MultiChar[FourCC.SINF]), ref info, 4);
        GFX_Init_TilesInfo((ushort)info[0], (ushort)info[1]);

        /* Get the SpritePixels chunk */
        g_tilesPixels = new byte[tilesDataLength]; //calloc(1, tilesDataLength);
        ChunkFile_Read(fileIndex, HToBE32((uint)SharpDune.MultiChar[FourCC.SSET]), ref g_tilesPixels, tilesDataLength);
        /*tilesDataLength = */
        Sprites_Decode(g_tilesPixels, g_tilesPixels);
        /*g_tilesPixels = realloc(g_tilesPixels, tilesDataLength);*/

        /* Get the Table chunk */
        g_iconRTBL = new byte[tableLength]; //calloc(1, tableLength);
        ChunkFile_Read(fileIndex, HToBE32((uint)SharpDune.MultiChar[FourCC.RTBL]), ref g_iconRTBL, tableLength);

        /* Get the Palette chunk */
        g_iconRPAL = new byte[paletteLength]; //calloc(1, paletteLength);
        ChunkFile_Read(fileIndex, HToBE32((uint)SharpDune.MultiChar[FourCC.RPAL]), ref g_iconRPAL, paletteLength);

        ChunkFile_Close(fileIndex);
    }
}
