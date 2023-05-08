namespace SharpDune.SaveLoad;

class SaveLoadMap
{
    /*
     * Load a Tile structure to a file (Little endian)
     *
     * @param t The tile to read
     * @param fp The stream
     * @return True if the tile was loaded successfully
     */
    static bool FRead_Tile(CTile t, FileStream fp)
    {
        var buffer = new byte[4];

        if (fp.Read(buffer, 0, 4) != 4) return false; //(fread(buffer, 1, 4, fp) != 4)

        t.groundTileID = (ushort)(buffer[0] | ((buffer[1] & 1) << 8));
        t.overlayTileID = (ushort)(buffer[1] >> 1);
        t.houseID = (byte)(buffer[2] & 0x07);
        t.isUnveiled = (buffer[2] & 0x08) == 0x08;
        t.hasUnit = (buffer[2] & 0x10) == 0x10;
        t.hasStructure = (buffer[2] & 0x20) == 0x20;
        t.hasAnimation = (buffer[2] & 0x40) == 0x40;
        t.hasExplosion = (buffer[2] & 0x80) == 0x80;
        t.index = buffer[3];
        return true;
    }

    /*
     * Save a Tile structure to a file (Little endian)
     *
     * @param t The tile to save
     * @param fp The stream
     * @return True if the tile was saved successfully
     */
    static bool FWrite_Tile(CTile t, BinaryWriter fp)
    {
        var value = t.houseID;
        if (t.isUnveiled) value |= 1 << 3;
        if (t.hasUnit) value |= 1 << 4;
        if (t.hasStructure) value |= 1 << 5;
        if (t.hasAnimation) value |= 1 << 6;
        if (t.hasExplosion) value |= 1 << 7;

        var buffer = new byte[4];
        buffer[0] = (byte)(t.groundTileID & 0xff);
        buffer[1] = (byte)((t.groundTileID >> 8) | (t.overlayTileID << 1));
        buffer[2] = value;
        buffer[3] = (byte)t.index;

        fp.Write(buffer); //if (fwrite(buffer, 1, 4, fp) != 4) return false;

        return true;

        //byte[] buffer = new byte[4];
        //buffer[0] = (byte)(t.groundTileID & 0xff);
        //buffer[1] = (byte)((t.groundTileID >> 8) | (t.overlayTileID << 1));
        //buffer[2] = (byte)(t.houseID | (Convert.ToByte(t.isUnveiled) << 3) | (Convert.ToByte(t.hasUnit) << 4) | (Convert.ToByte(t.hasStructure) << 5) | (Convert.ToByte(t.hasAnimation) << 6) | (Convert.ToByte(t.hasExplosion) << 7));
        //buffer[3] = (byte)t.index;

        //fp.Write(buffer); //if (fwrite(buffer, 1, 4, fp) != 4) return false;

        //return true;
    }

    /*
     * Load all Tiles from a file.
     * @param fp The file to load from.
     * @param length The length of the data chunk.
     * @return True if and only if all bytes were read successful.
     */
    internal static bool Map_Load(FileStream fp, uint length)
    {
        ushort i;

        for (i = 0; i < 0x1000; i++)
        {
            var t = g_map[i];

            t.isUnveiled = false;
            t.overlayTileID = g_veiledTileID;
        }

        while (length >= sizeof(ushort) + 4/*Common.SizeOf(typeof(Tile))*/)
        {
            CTile t;

            length -= sizeof(ushort) + 4/*Common.SizeOf(typeof(Tile))*/;

            if (!FRead_LE_UInt16(ref i, fp)) return false;
            if (i >= 0x1000) return false;

            t = g_map[i];
            if (!FRead_Tile(t, fp)) return false;

            if (g_mapTileID[i] != t.groundTileID)
            {
                g_mapTileID[i] |= 0x8000;
            }
        }
        if (length != 0) return false;

        return true;
    }

    /*
     * Save all Tiles to a file.
     * @param fp The file to save to.
     * @return True if and only if all bytes were written successful.
     */
    internal static bool Map_Save(BinaryWriter fp)
    {
        ushort i;

        for (i = 0; i < 0x1000; i++)
        {
            var tile = g_map[i];

            /* If there is nothing on the tile, not unveiled, and it is equal to the mapseed generated tile, don't store it */
            if (!tile.isUnveiled && !tile.hasStructure && !tile.hasUnit && !tile.hasAnimation && !tile.hasExplosion && (g_mapTileID[i] & 0x8000) == 0 && g_mapTileID[i] == tile.groundTileID) continue;

            /* Store the index, then the tile itself */
            if (!FWrite_LE_UInt16(i, fp)) return false;
            if (!FWrite_Tile(tile, fp)) return false;
        }

        return true;
    }
}
