/* Configuration and options load and save */

namespace SharpDune;

/*
 * This is the layout of options.cfg.
 */
class GameCfg
{
    internal ushort music;                      /*!< 0:Off, 1:On. */
    internal ushort sounds;                     /*!< 0:Off, 1:On. */
    internal ushort gameSpeed;                  /*!< 0:Slowest, 1:Slow, 2:Normal, 3:Fast, 4:Fastest. */
    internal ushort hints;                      /*!< 0:Off, 1:On. */
    internal ushort autoScroll;                 /*!< 0:Off, 1:On. */
}

/*
 * This is the layout of decoded dune.cfg.
 */
[StructLayout(LayoutKind.Explicit)]
class DuneCfg
{
    /* 0000(1)   PACK */
    [FieldOffset(0)] internal byte graphicDrv;        /*!< Graphic mode to use. */
    /* 0001(1)   PACK */
    [FieldOffset(1)] internal byte musicDrv;          /*!< Index into music drivers array. */
    /* 0002(1)   PACK */
    [FieldOffset(2)] internal byte soundDrv;          /*!< Index into sound drivers array. */
    /* 0003(1)   PACK */
    [FieldOffset(3)] internal byte voiceDrv;          /*!< Index into digitized sound drivers array. */
    /* 0004(1)   PACK */
    [FieldOffset(4)] internal bool useMouse;          /*!< Use Mouse. */
    /* 0005(1)   PACK */
    [FieldOffset(5)] internal bool useXMS;            /*!< Use Extended Memory. */
    /* 0006(1)   PACK */
    [FieldOffset(6)] internal byte variable_0006;     /*!< ?? */
    /* 0007(1)   PACK */
    [FieldOffset(7)] internal byte variable_0007;     /*!< ?? */
    /* 0008(1)   PACK */
    [FieldOffset(8)] internal byte language;          /*!< @see Language. */
    /* 0009(1)   PACK */
    [FieldOffset(9)] internal byte checksum;          /*!< Used to check validity on config data. See Config_Read(). */
}

static class Config
{
    internal static GameCfg g_gameConfig = new() { music = 1, sounds = 1, gameSpeed = 2, hints = 1, autoScroll = 0 };
    internal static DuneCfg g_config = new();
    internal static bool g_enableSoundMusic = true;
    internal static bool g_enableVoices = true;

    /*
     * Set a default config
     */
    internal static bool Config_Default(DuneCfg config)
    {
        if (config == null) return false;

        //memset(config, 0, sizeof(DuneCfg));
        config.graphicDrv = 1;
        config.musicDrv = 1;
        config.soundDrv = 1;
        config.voiceDrv = 1;
        config.useMouse = true; //1;
        config.useXMS = true; //1;
        config.language = (byte)Language.ENGLISH;
        return true;
    }

    /*
     * Loads the game options.
     *
     * @return True if loading is successful.
     */
    internal static bool GameOptions_Load()
    {
        var index = File_Open_Personal("OPTIONS.CFG", FileMode.FILE_MODE_READ);

        if (index == (byte)FileMode.FILE_INVALID) return false;

        g_gameConfig.music = File_Read_LE16(index);
        g_gameConfig.sounds = File_Read_LE16(index);
        g_gameConfig.gameSpeed = File_Read_LE16(index);
        g_gameConfig.hints = File_Read_LE16(index);
        g_gameConfig.autoScroll = File_Read_LE16(index);

        File_Close(index);

        return true;
    }

    /*
     * Saves the game options.
     */
    internal static void GameOptions_Save()
    {
        var index = File_Open_Personal("OPTIONS.CFG", FileMode.FILE_MODE_WRITE);

        if (index == (byte)FileMode.FILE_INVALID) return;

        File_Write_LE16(index, g_gameConfig.music);
        File_Write_LE16(index, g_gameConfig.sounds);
        File_Write_LE16(index, g_gameConfig.gameSpeed);
        File_Write_LE16(index, g_gameConfig.hints);
        File_Write_LE16(index, g_gameConfig.autoScroll);

        File_Close(index);

        if (g_gameConfig.music == 0) Music_Play(0);
    }

    /*
     * Reads and decode the config.
     *
     * @param filename The name of file containing config.
     * @param config The address where the config will be stored.
     * @return True if loading and decoding is successful.
     */
    internal static bool Config_Read(string filename, DuneCfg config)
    {
        FileStream f;
        int read;
        byte sum;
        byte[] buffer;
        int bufferPointer;
        sbyte i;

        f = FOpenDataDir(SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR, filename, "rb");
        if (f == null) return false;

        buffer = new byte[10];

        read = f.Read(buffer, 0, buffer.Length);
        f.Close();
        if (read != 10) return false;

        sum = 0;

        for (bufferPointer = 0, i = 7; i >= 0; bufferPointer++, i--)
        {
            buffer[bufferPointer] ^= 0xA5;
            buffer[bufferPointer] -= (byte)i;
            sum += buffer[bufferPointer];
        }

        config.graphicDrv = buffer[0];
        config.musicDrv = buffer[1];
        config.soundDrv = buffer[2];
        config.voiceDrv = buffer[3];
        config.useMouse = Convert.ToBoolean(buffer[4]);
        config.useXMS = Convert.ToBoolean(buffer[5]);
        config.variable_0006 = buffer[6];
        config.variable_0007 = buffer[7];
        config.language = buffer[8];
        config.checksum = buffer[9];

        sum ^= 0xA5;

        return (sum == config.checksum);
    }

    /*
     * encode and write the config
     *
     * @param filename The name of file containing config.
     * @param config The address where the config will be read.
     * @return True if successful.
     */
    internal static bool Config_Write(string filename, DuneCfg config)
    {
        FileStream f;
        byte sum;
        byte[] c1, c2;
        int c1Pointer, c2Pointer;
        sbyte i;

        f = FOpenDataDir(SearchDirectory.SEARCHDIR_PERSONAL_DATA_DIR, filename, "wb");
        if (f == null) return false;

        sum = 0;

        c1 = [ config.graphicDrv, config.musicDrv, config.soundDrv, config.voiceDrv, Convert.ToByte(config.useMouse),
                Convert.ToByte(config.useXMS), config.variable_0006, config.variable_0007, config.language, config.checksum ];
        c2 = new byte[10];

        for (c1Pointer = 0, c2Pointer = 0, i = 7; i >= 0; c1Pointer++, c2Pointer++, i--)
        {
            c2[c2Pointer] = (byte)((c1[c1Pointer] + i) ^ 0xA5);
            sum += c1[c1Pointer];
        }

        /* Language */
        c2[c2Pointer++] = c1[c1Pointer++];
        sum ^= 0xA5;
        c2[c2Pointer] = sum;

        f.Write(c2, 0, c2.Length);
        f.Close();

        return true;
    }
}
