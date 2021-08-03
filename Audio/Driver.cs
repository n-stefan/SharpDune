/* Driver */

namespace SharpDune.Audio
{
    class Driver
    {
        internal ushort index;                                  /*!< Index of the loaded driver. */
        internal /*char[4]*/string extension;                   /*!< Extension used for music file names. */
        internal /*IntPtr*/byte[] content;                      /*!< Pointer to the file to play content. */
        internal /*char[14]*/string filename;                   /*!< Name of file to play. */
        internal bool contentMalloced;                          /*!< Wether content pointer is the result of a malloc. */

        internal Driver Clone() =>
            (Driver)MemberwiseClone();
    }

    class MSBuffer
    {
        internal ushort index;                                  /*!< ?? */
        internal MSData[] buffer;                               /*!< ?? */
    }

    class CDriver
    {
        static readonly bool[] s_driverInstalled = new bool[16];
        static readonly bool[] s_driverLoaded = new bool[16];

        static readonly Driver s_driverMusic = new();
        static readonly Driver s_driverSound = new();
        static readonly Driver s_driverVoice = new();

        internal static Driver g_driverMusic = s_driverMusic;
        internal static Driver g_driverSound = s_driverSound;
        static readonly Driver g_driverVoice = s_driverVoice;

        static readonly MSBuffer s_bufferMusic = new();
        internal static MSBuffer g_bufferMusic = s_bufferMusic;

        static readonly MSBuffer[] s_bufferSound = { new(), new(), new(), new() }; //new MSBuffer[4];
        static readonly MSBuffer[] g_bufferSound = { s_bufferSound[0], s_bufferSound[1], s_bufferSound[2], s_bufferSound[3] };

        static byte s_bufferSoundIndex;

        internal static void Driver_LoadFile(string musicName, Driver driver)
        {
            string filename;
            //int len;

            filename = Drivers_GenerateFilename(musicName, driver);

            if (filename == null) return;

            Driver_UnloadFile(driver);

            //len = filename.Length; // + 1; /* String length including terminating \0 */
            //Debug.Assert(len <= driver.filename.Length);
            driver.filename = filename; //memcpy(driver->filename, filename, len);

            driver.content = File_ReadWholeFile(filename);
            driver.contentMalloced = true;
            Debug.WriteLine($"DEBUG: Driver_LoadFile({musicName}, {driver}): {filename} loaded");
        }

        internal static void Driver_UnloadFile(Driver driver)
        {
            if (driver.contentMalloced)
            {
                driver.content = null; //free(driver->content);
            }

            driver.filename = null; //"\0";
            driver.content = null; //IntPtr.Zero;
            driver.contentMalloced = false;
        }

        internal static void Driver_Music_Stop()
        {
            var music = g_driverMusic;
            var musicBuffer = g_bufferMusic;

            if (music.index == 0xFFFF) return;
            if (musicBuffer.index == 0xFFFF) return;

            MPU_Stop(musicBuffer.index);
            MPU_ClearData(musicBuffer.index);
            musicBuffer.index = 0xFFFF;
        }

        internal static bool Driver_Voice_IsPlaying()
        {
            if (g_driverVoice.index == 0xFFFF) return false;
            return DSP_GetStatus() == 2;
        }

        internal static bool Driver_Music_IsPlaying()
        {
            var buffer = g_bufferMusic;

            if (g_driverMusic.index == 0xFFFF) return false;
            if (buffer.index == 0xFFFF) return false;

            return MPU_IsPlaying(buffer.index) == 1;
        }

        internal static void Driver_Voice_Stop()
        {
            var voice = g_driverVoice;

            if (Driver_Voice_IsPlaying()) DSP_Stop();

            if (voice.contentMalloced)
            {
                //Marshal.FreeHGlobal(voice.content); //free(voice->content);
                voice.contentMalloced = false;
            }

            voice.content = null; //IntPtr.Zero;
        }

        static short l_currentPriority = -1;   /* priority of sound currently playing */
        internal static void Driver_Voice_Play(byte[] data, short priority)
        {
            var voice = g_driverVoice;

            if (g_gameConfig.sounds == 0 || voice.index == 0xFFFF) return;

            if (data == null)
            {
                priority = 0x100;
            }
            else if (priority >= 0x100)
            {
                priority = 0xFF;
            }

            if (!Driver_Voice_IsPlaying()) l_currentPriority = -1;

            if (priority < l_currentPriority) return;

            Driver_Voice_Stop();

            if (data == null) return;

            l_currentPriority = priority;

            if (data == null) return;

            DSP_Play(data);
        }

        internal static void Driver_Voice_LoadFile(string filename, byte[] buffer, uint length)
        {
            Debug.Assert(buffer != null);

            if (filename == null) return;
            if (g_driverVoice.index == 0xFFFF) return;

            File_ReadBlockFile(filename, buffer, length);
        }

        static string filename; //= new char[14];
        internal static string Drivers_GenerateFilename(string name, Driver driver)
        {
            string basefilename; //= new char[14];

            if (name == null || driver == null || driver.index == 0xFFFF) return null;

            /* Remove extension if there is one */
            name = Path.GetFileNameWithoutExtension(name); //if (strrchr(basefilename, '.') != NULL) *strrchr(basefilename, '.') = '\0';
            basefilename = name; //strncpy(basefilename, name, sizeof(basefilename));
            //basefilename[sizeof(basefilename) - 1] = '\0';

            filename = $"{basefilename}.{driver.extension}"; //snprintf(filename, sizeof(filename), "%s.%s", basefilename, driver->extension);

            if (File_Exists(filename)) return filename;

            if (driver.index == 0xFFFF) return null;

            filename = $"{basefilename}.XMI"; //snprintf(filename, sizeof(filename), "%s.XMI", basefilename);

            if (File_Exists(filename)) return filename;

            return null;
        }

        internal static void Driver_Sound_Stop()
        {
            var sound = g_driverSound;
            byte i;

            if (sound.index == 0xFFFF) return;

            for (i = 0; i < 4; i++)
            {
                var soundBuffer = g_bufferSound[i];
                if (soundBuffer.index == 0xFFFF) continue;

                MPU_Stop(soundBuffer.index);
                MPU_ClearData(soundBuffer.index);
                soundBuffer.index = 0xFFFF;
            }
        }

        internal static void Driver_Sound_LoadFile(string musicName)
        {
            var sound = g_driverSound;
            var music = g_driverMusic;

            Driver_Sound_Stop();

            if (sound.index == 0xFFFF) return;

            if (sound.content == music.content)
            {
                sound.content = null;
                sound.filename = null; //[0] = '\0';
                sound.contentMalloced = false;
            }
            else
            {
                Driver_UnloadFile(sound);
            }

            if (music.filename != null)
            { //[0] != '\0') {
                string filename;

                filename = Drivers_GenerateFilename(musicName, sound);

                if (filename != null && string.Equals(filename, music.filename, StringComparison.OrdinalIgnoreCase))
                { //strcasecmp(filename, music->filename) == 0) {
                    sound.content = music.content;
                    sound.filename = music.filename; //memcpy(sound->filename, music->filename, sizeof(music->filename));
                    sound.contentMalloced = music.contentMalloced;
                    return;
                }
            }

            Driver_LoadFile(musicName, sound);
        }

        internal static void Driver_Sound_Play(short index, short volume)
        {
            var sound = g_driverSound;
            var soundBuffer = g_bufferSound[s_bufferSoundIndex];

            if (index < 0 || index >= 120) return;

            if (g_gameConfig.sounds == 0 && index > 1) return;

            if (sound.index == 0xFFFF) return;

            if (soundBuffer.index != 0xFFFF)
            {
                MPU_Stop(soundBuffer.index);
                MPU_ClearData(soundBuffer.index);
                soundBuffer.index = 0xFFFF;
            }

            soundBuffer.index = MPU_SetData(sound.content, (ushort)index, soundBuffer.buffer[0]);

            MPU_Play(soundBuffer.index);
            MPU_SetVolume(soundBuffer.index, (ushort)(((volume & 0xFF) * 90) / 256), 0);

            s_bufferSoundIndex = (byte)((s_bufferSoundIndex + 1) % 4);
        }

        internal static void Drivers_All_Init()
        {
            Drivers_Reset();

            g_enableSoundMusic = Drivers_SoundMusic_Init(g_enableSoundMusic);
            g_enableVoices = Drivers_Voice_Init(g_enableVoices);
        }

        internal static void Drivers_All_Uninit()
        {
            Drivers_SoundMusic_Uninit();
            Drivers_Voice_Uninit();
        }

        internal static void Driver_Music_FadeOut()
        {
            var music = g_driverMusic;
            var musicBuffer = g_bufferMusic;

            if (music.index == 0xFFFF) return;
            if (musicBuffer.index == 0xFFFF) return;

            MPU_SetVolume(musicBuffer.index, 0, 2000);
        }

        static void Drivers_Reset()
        {
            Array.Clear(s_driverInstalled, 0, s_driverInstalled.Length); //memset(s_driverInstalled, 0, sizeof(s_driverInstalled));
            Array.Clear(s_driverLoaded, 0, s_driverLoaded.Length); //memset(s_driverLoaded, 0, sizeof(s_driverLoaded));
        }

        static bool Drivers_SoundMusic_Init(bool enable)
        {
            Driver sound;
            Driver music;
            uint size;
            byte i;

            sound = g_driverSound;
            music = g_driverMusic;

            sound.index = 0xFFFF;
            music.index = 0xFFFF;

            if (!enable) return false;

            if (!MPU_Init()) return false;

            if (!Drivers_Init(sound, (IniFile_GetInteger("mt32midi", 0) != 0) ? "XMI" : "C55")) return false;

            music = sound.Clone(); //music = sound; //memcpy(music, sound, sizeof(Driver));
            g_driverMusic = sound.Clone();

            //#if _WIN32
            MPU_StartThread(1000000 / 120);
            //#else
            //  Timer_Add(MPU_Interrupt, 1000000 / 120, false);
            //#endif

            size = MPU_GetDataSize();

            for (i = 0; i < 4; i++)
            {
                var buf = g_bufferSound[i];
                buf.buffer = new MSData[size]; //calloc(1, size);
                for (var j = 0; j < size; j++) buf.buffer[j] = new MSData();
                buf.index = 0xFFFF;
            }
            s_bufferSoundIndex = 0;

            g_bufferMusic.buffer = new MSData[] { new() }; //new MSData[size]; //calloc(1, size);
            g_bufferMusic.index = 0xFFFF;

            return true;
        }

        static bool Drivers_Voice_Init(bool enable)
        {
            Driver voice;

            voice = g_driverVoice;

            voice.index = 0xFFFF;

            if (!enable) return false;

            if (!DSP_Init()) return false;

            if (!Drivers_Init(voice, "VOC")) return false;

            return true;
        }

        static void Drivers_SoundMusic_Uninit()
        {
            var sound = g_driverSound;
            var music = g_driverMusic;

            if (g_enableSoundMusic)
            {
                byte i;
                MSBuffer buffer = null;

                for (i = 0; i < 4; i++)
                {
                    buffer = g_bufferSound[i];
                    if (buffer.index != 0xFFFF)
                    {
                        MPU_Stop(buffer.index);
                        MPU_ClearData(buffer.index);
                        buffer.index = 0xFFFF;
                    }
                }

                buffer = g_bufferMusic;
                if (buffer.index != 0xFFFF)
                {
                    MPU_Stop(buffer.index);
                    MPU_ClearData(buffer.index);
                    buffer.index = 0xFFFF;
                }

                //#if _WIN32
                MPU_StopThread();
                //#else
                //Timer_Remove(MPU_Interrupt);
                //#endif

                buffer.buffer = null;

                for (i = 0; i < 4; i++)
                {
                    buffer = g_bufferSound[i];
                    buffer.buffer = null;
                }
            }

            if (music.content != sound.content) Driver_UnloadFile(music);
            Driver_UnloadFile(sound);

            /* Only Uninit() sound, as music is a copy of sound. */
            Drivers_Uninit(sound);
            music = sound; //memcpy(music, sound, sizeof(Driver));

            MPU_Uninit();
        }

        static void Drivers_Voice_Uninit()
        {
            Drivers_Uninit(g_driverVoice);

            DSP_Uninit();
        }

        static bool Drivers_Init(Driver driver, string extension)
        {
            driver.index = Driver_Install();
            if (driver.index == 0xFFFF) return false;

            Driver_Init(driver.index);

            driver.extension = extension; //strncpy(driver->extension, extension, 4);

            return true;
        }

        static void Drivers_Uninit(Driver driver)
        {
            if (driver == null) return;

            Driver_Uninit(driver.index);

            Driver_Uninstall(driver.index);

            driver.index = 0xFFFF;
        }

        static void Driver_Init(ushort driver)
        {
            if (driver >= 16) return;

            s_driverLoaded[driver] = true;
        }

        static void Driver_Uninit(ushort driver)
        {
            if (driver >= 16 || !s_driverLoaded[driver]) return;
            s_driverLoaded[driver] = false;
        }

        static ushort Driver_Install()
        {
            ushort index;

            for (index = 0; index < 16; index++)
            {
                if (!s_driverInstalled[index]) break;
            }
            if (index == 16) return 0xFFFF;
            s_driverInstalled[index] = true;

            return index;
        }

        static void Driver_Uninstall(ushort driver)
        {
            if (driver >= 16) return;

            s_driverInstalled[driver] = false;
        }
    }
}
