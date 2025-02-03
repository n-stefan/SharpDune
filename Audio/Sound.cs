/* Sound */

namespace SharpDune.Audio;

/* Information about sound files. */
class VoiceData
{
    internal string str;         /*!< Pointer to a string. */
    internal ushort priority;    /*!< priority */
}

/* Information about sound files. */
class MusicData
{
    internal string name;       /*!< Pointer to a string. */
    internal ushort index;      /*!< index */
}

/* Audio and visual feedback about events and commands.
 * in Intro, messageId is used as a flag (force drawing
 * of message if messageId == 1) */
class Feedback
{
    internal ushort[] voiceId = new ushort[NUM_SPEECH_PARTS]; /*!< English spoken text. */
    internal ushort messageId;                                      /*!< Message to display in the viewport when audio is disabled. */
    internal ushort soundId;                                        /*!< Sound. */
}

static class Sound
{
    /* Number of voices in the game. */
    const byte NUM_VOICES = 131;
    /* Maximal number of spoken audio fragments in one message. */
    internal const byte NUM_SPEECH_PARTS = 5;

    static readonly byte[][] g_voiceData = new byte[NUM_VOICES][];         /*!< Preloaded Voices sound data */
    static readonly uint[] g_voiceDataSize = new uint[NUM_VOICES];         /*!< Preloaded Voices sound data size in byte */
    static readonly ushort[] s_spokenWords = new ushort[NUM_SPEECH_PARTS]; /*!< Buffer with speech to play. */
    static short s_currentVoicePriority;                          /*!< Priority of the currently playing Speech */

    static string s_currentMusic;        /*!< Currently loaded music file. */

    static void Driver_Music_Play(short index, ushort volume)
    {
        var music = g_driverMusic;
        var musicBuffer = g_bufferMusic;

        if (index < 0 || index > 120 || g_gameConfig.music == 0) return;

        if (music.index == 0xFFFF) return;

        if (musicBuffer.index != 0xFFFF)
        {
            MPU_Stop(musicBuffer.index);
            MPU_ClearData(musicBuffer.index);
            musicBuffer.index = 0xFFFF;
        }

        musicBuffer.index = MPU_SetData(music.content, (ushort)index, musicBuffer.buffer[0]);

        MPU_Play(musicBuffer.index);
        MPU_SetVolume(musicBuffer.index, (ushort)((volume & 0xFF) * 90 / 256), 0);
    }

    static void Driver_Music_LoadFile(string musicName)
    {
        var music = g_driverMusic;
        var sound = g_driverSound;

        Driver_Music_Stop();

        if (music.index == 0xFFFF) return;

        if (music.content == sound.content)
        {
            music.content = null;
            music.filename = null; //"\0"
            music.contentMalloced = false;
        }
        else
        {
            Driver_UnloadFile(music);
        }

        if (sound.filename != null /*"\0"*/ && musicName != null && string.Equals(Drivers_GenerateFilename(musicName, music), sound.filename, StringComparison.OrdinalIgnoreCase))
        { //strcasecmp
            g_driverMusic.content = g_driverSound.content;
            g_driverMusic.filename = g_driverSound.filename; //memcpy(g_driverMusic->filename, g_driverSound->filename, sizeof(g_driverMusic->filename));
            g_driverMusic.contentMalloced = g_driverSound.contentMalloced;

            return;
        }

        Driver_LoadFile(musicName, music);
    }

    static ushort currentMusicID;
    /*
    * Plays a music.
    * @param index The index of the music to play.
    */
    internal static void Music_Play(ushort musicID)
    {
        if (musicID == 0xFFFF || musicID >= 38 || musicID == currentMusicID) return;

        currentMusicID = musicID;

        if (g_table_musics[musicID].name != s_currentMusic)
        {
            s_currentMusic = g_table_musics[musicID].name;

            Driver_Music_Stop();
            Driver_Voice_Play(null, 0xFF);
            Driver_Music_LoadFile(null);
            Driver_Sound_LoadFile(null);
            Driver_Music_LoadFile(s_currentMusic);
            Driver_Sound_LoadFile(s_currentMusic);
        }

        Driver_Music_Play((short)g_table_musics[musicID].index, 0xFF);
    }

    /*
     * Output feedback about events of the game.
     * @param index Feedback to provide (\c 0xFFFF means do nothing, \c 0xFFFE means stop, otherwise a feedback code).
     * @note If sound is disabled, the main viewport is used to display a message.
     */
    internal static void Sound_Output_Feedback(ushort index)
    {
        if (index == 0xFFFF) return;

        if (index == 0xFFFE)
        {
            byte i;

            /* Clear spoken audio. */
            for (i = 0; i < /*Common.lengthof<ushort>(s_spokenWords)*/s_spokenWords.Length; i++)
            {
                s_spokenWords[i] = 0xFFFF;
            }

            Driver_Voice_Stop();

            g_viewportMessageText = null;
            if ((g_viewportMessageCounter & 1) != 0)
            {
                g_viewport_forceRedraw = true;
                g_viewportMessageCounter = 0;
            }
            s_currentVoicePriority = 0;

            return;
        }

        if (!g_enableVoices || g_gameConfig.sounds == 0)
        {
            Driver_Sound_Play((short)g_feedback[index].soundId, 0xFF);

            g_viewportMessageText = String_Get_ByIndex(g_feedback[index].messageId);

            if ((g_viewportMessageCounter & 1) != 0)
            {
                g_viewport_forceRedraw = true;
            }

            g_viewportMessageCounter = 4;

            return;
        }

        /* If nothing is being said currently, load new words. */
        if (s_spokenWords[0] == 0xFFFF)
        {
            byte i;

            for (i = 0; i < /*Common.lengthof<ushort>(s_spokenWords)*/s_spokenWords.Length; i++)
            {
                s_spokenWords[i] = (g_config.language == (byte)Language.ENGLISH) ? g_feedback[index].voiceId[i] : g_translatedVoice[index][i];
            }
        }

        Sound_StartSpeech();
    }

    /*
     * Start speech.
     * Start a new speech fragment if possible.
     * @return Sound is produced.
     */
    internal static bool Sound_StartSpeech()
    {
        if (g_gameConfig.sounds == 0) return false;

        if (Driver_Voice_IsPlaying()) return true;

        s_currentVoicePriority = 0;

        if (s_spokenWords[0] == 0xFFFF) return false;

        Sound_StartSound(s_spokenWords[0]);
        /* Move speech parts one place. */
        Array.Copy(s_spokenWords, 1, s_spokenWords, 0, s_spokenWords.Length - 1); //(s_spokenWords.Length - s_spokenWords[0]) * 2); //memmove(&s_spokenWords[0], &s_spokenWords[1], sizeof(s_spokenWords) - sizeof(s_spokenWords[0]));
        s_spokenWords[/*Common.lengthof<ushort>(s_spokenWords)*/^1] = 0xFFFF;

        return true;
    }

    /*
     * Start playing a sound sample.
     * @param index Sample to play.
     */
    internal static void Sound_StartSound(ushort index)
    {
        if (index == 0xFFFF || g_gameConfig.sounds == 0 || (short)g_table_voices[index].priority < s_currentVoicePriority) return;

        s_currentVoicePriority = (short)g_table_voices[index].priority;

        if (g_voiceData[index] != null)
        {
            Driver_Voice_Play(g_voiceData[index], 0xFF);
        }
        else
        {
            ReadOnlySpan<char> filenameBuffer;

            var filename = g_table_voices[index].str;
            if (filename.StartsWith('?'))
            {
                //snprintf(filenameBuffer, sizeof(filenameBuffer), filename + 1, g_playerHouseID < HOUSE_MAX ? g_table_houseInfo[g_playerHouseID].prefixChar : ' ');
                filenameBuffer = filename.Replace("%c", g_playerHouseID < HouseType.HOUSE_MAX ? g_table_houseInfo[(int)g_playerHouseID].prefixChar.ToString(Culture) : " ", Comparison);

                if (g_readBuffer.Length < g_readBufferSize)
                    Array.Resize(ref g_readBuffer, (int)g_readBufferSize);

                Driver_Voice_LoadFile(filenameBuffer.Slice(1), g_readBuffer, g_readBufferSize);

                Driver_Voice_Play(g_readBuffer, 0xFF);
            }
        }
    }

    /*
     * Play a voice. Volume is based on distance to position.
     * @param voiceID Which voice to play.
     * @param position Which position to play it on.
     */
    internal static void Voice_PlayAtTile(short voiceID, Tile32 position)
    {
        ushort index;
        ushort volume;

        if (voiceID is < 0 or >= 120) return;
        if (g_gameConfig.sounds == 0) return;

        volume = 255;
        if (position.x != 0 || position.y != 0)
        {
            volume = Tile_GetDistancePacked(g_minimapPosition, Tile_PackTile(position));
            if (volume > 64) volume = 64;

            volume = (ushort)(255 - (volume * 255 / 80));
        }

        index = g_table_voiceMapping[voiceID];

        if (g_enableVoices && index != 0xFFFF && g_voiceData[index] != null && g_table_voices[index].priority >= s_currentVoicePriority)
        {
            s_currentVoicePriority = (short)g_table_voices[index].priority;

            //g_readBuffer = new byte[g_voiceDataSize[index]];
            //Array.Copy(g_voiceData[index], g_readBuffer, g_voiceDataSize[index]); //memmove(g_readBuffer, g_voiceData[index], g_voiceDataSize[index]);

            Driver_Voice_Play(/*g_readBuffer*/g_voiceData[index], s_currentVoicePriority);
        }
        else
        {
            Driver_Sound_Play(voiceID, (short)volume);
        }
    }

    /*
     * Play a voice.
     * @param voiceID The voice to play.
     */
    internal static void Voice_Play(short voiceID)
    {
        var tile = new Tile32
        {
            x = 0,
            y = 0
        };
        Voice_PlayAtTile(voiceID, tile);
    }

    /*
     * Initialises the MT-32.
     * @param index The index of the music to play.
     */
    internal static void Music_InitMT32()
    {
        ushort left = 0;

        Driver_Music_LoadFile("DUNEINIT");

        Driver_Music_Play(0, 0xFF);

        GUI_DrawText(String_Get_ByIndex(15), 0, 0, 15, 12); /* "Initializing the MT-32" */

        while (Driver_Music_IsPlaying())
        {
            Timer_Sleep(60);

            left += 6;
            GUI_DrawText(".", (short)left, 10, 15, 12);
        }
    }

    static ushort currentVoiceSet = 0xFFFE;
    /*
     * Load voices.
     * voiceSet 0xFFFE is for Game Intro.
     * voiceSet 0xFFFF is for Game End.
     * @param voiceSet Voice set to load : either a HouseID, or special values 0xFFFE or 0xFFFF.
     */
    internal static void Voice_LoadVoices(ushort voiceSet)
    {
        char prefixChar; // = ' ';
        ushort voice;

        if (!g_enableVoices/* == 0*/) return;

        for (voice = 0; voice < NUM_VOICES; voice++)
        {
            switch (g_table_voices[voice].str[0])
            {
                case '%':
                    if (g_config.language != (byte)Language.ENGLISH || currentVoiceSet == voiceSet)
                    {
                        if (voiceSet is not 0xFFFF and not 0xFFFE) break;
                    }

                    g_voiceData[voice] = null; //free(g_voiceData[voice]);
                    break;

                case '+':
                    if (voiceSet is not 0xFFFF and not 0xFFFE) break;

                    g_voiceData[voice] = null; //free(g_voiceData[voice]);
                    break;

                case '-':
                    if (voiceSet == 0xFFFF) break;

                    g_voiceData[voice] = null; //free(g_voiceData[voice]);
                    break;

                case '/':
                    if (voiceSet != 0xFFFE) break;

                    g_voiceData[voice] = null; //free(g_voiceData[voice]);
                    break;

                case '?':
                    if (voiceSet == 0xFFFF) break;

                    /* No free() as there was never a malloc(). */
                    g_voiceData[voice] = null;
                    break;

                default:
                    break;
            }
        }

        if (currentVoiceSet == voiceSet) return;

        for (voice = 0; voice < NUM_VOICES; voice++)
        {
            ReadOnlySpan<char> filename; //char[16]
            var str = g_table_voices[voice].str;
            SleepIdle();  /* let a chance to update screen, etc. */
            switch (str[0])
            {
                case '%':
                    if (g_voiceData[voice] != null || currentVoiceSet == voiceSet || voiceSet == 0xFFFF || voiceSet == 0xFFFE) break;

                    prefixChar = (Language)g_config.language switch
                    {
                        Language.FRENCH => 'F',
                        Language.GERMAN => 'G',
                        _ => g_table_houseInfo[voiceSet].prefixChar,
                    };
                    filename = str.Replace("%c", prefixChar.ToString(), Comparison); //snprintf(filename, sizeof(filename), str, prefixChar);

                    g_voiceData[voice] = Sound_LoadVoc(filename, out g_voiceDataSize[voice]);
                    break;

                case '+':
                    if (voiceSet == 0xFFFF || g_voiceData[voice] != null) break;

                    prefixChar = (Language)g_config.language switch
                    {
                        Language.FRENCH => 'F',
                        Language.GERMAN => 'G',
                        _ => 'Z',
                    };
                    filename = str.Replace("%c", prefixChar.ToString(), Comparison).AsSpan(1); //snprintf(filename, sizeof(filename), str + 1, prefixChar);

                    /* XXX - In the 1.07us datafiles, a few files are named differently:
                     *
                     *  moveout.voc
                     *  overout.voc
                     *  report1.voc
                     *  report2.voc
                     *  report3.voc
                     *
                     * They come without letter in front of them. To make things a bit
                     *  easier, just check if the file exists, then remove the first
                     *  letter and see if it works then.
                     */
                    if (!File_Exists(filename))
                    {
                        filename = filename.Slice(1); //memmove(filename, filename + 1, strlen(filename));
                    }

                    g_voiceData[voice] = Sound_LoadVoc(filename, out g_voiceDataSize[voice]);
                    break;

                case '-':
                    if (voiceSet != 0xFFFF || g_voiceData[voice] != null) break;

                    g_voiceData[voice] = Sound_LoadVoc(str.AsSpan(1), out g_voiceDataSize[voice]);
                    break;

                case '/':
                    if (voiceSet != 0xFFFE) break;

                    g_voiceData[voice] = Sound_LoadVoc(str.AsSpan(1), out g_voiceDataSize[voice]);
                    break;

                case '?':
                    break;

                default:
                    if (g_voiceData[voice] != null) break;

                    g_voiceData[voice] = Sound_LoadVoc(str, out g_voiceDataSize[voice]);
                    break;
            }
        }
        currentVoiceSet = voiceSet;
    }

    /*
     * Unload voices.
     */
    internal static void Voice_UnloadVoices()
    {
        ushort voice;

        for (voice = 0; voice < NUM_VOICES; voice++)
        {
            //free(g_voiceData[voice]);
            g_voiceData[voice] = null;
        }
    }

    /*
     * Load a voice file to a malloc'd buffer.
     * @param filename The name of the file to load.
     * @return Where the file is loaded.
     */
    static byte[] Sound_LoadVoc(/*string*/ReadOnlySpan<char> filename, out uint retFileSize)
    {
        byte[] res;

        retFileSize = 0;

        if (filename == null) return null;
        if (!File_Exists_GetSize(filename, out var fileSize)) return null;

        fileSize++;
        fileSize &= 0xFFFFFFFE;

        retFileSize = fileSize;
        res = new byte[fileSize]; //malloc(fileSize);
        Driver_Voice_LoadFile(filename, res, fileSize);

        return res;
    }
}
