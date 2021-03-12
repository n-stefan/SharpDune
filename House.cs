/* House management */

using System;
using System.Diagnostics;
using static System.Math;

namespace SharpDune
{
    /*
    * Types of Houses available in the game.
    */
    enum HouseType
    {
        HOUSE_HARKONNEN = 0,
        HOUSE_ATREIDES = 1,
        HOUSE_ORDOS = 2,
        HOUSE_FREMEN = 3,
        HOUSE_SARDAUKAR = 4,
        HOUSE_MERCENARY = 5,

        HOUSE_MAX = 6,
        HOUSE_INVALID = 0xFF
    }

    enum HouseIndex
    {
        HOUSE_INDEX_MAX = 6,                                    /*!< The highest possible index for any House.  */

        HOUSE_INDEX_INVALID = 0xFFFF
    }

    enum HouseAnimationType
    {
        HOUSEANIMATION_INTRO = 0,
        HOUSEANIMATION_LEVEL4_HARKONNEN = 1,
        HOUSEANIMATION_LEVEL4_ARTREIDES = 2,
        HOUSEANIMATION_LEVEL4_ORDOS = 3,
        HOUSEANIMATION_LEVEL8_HARKONNEN = 4,
        HOUSEANIMATION_LEVEL8_ARTREIDES = 5,
        HOUSEANIMATION_LEVEL8_ORDOS = 6,
        HOUSEANIMATION_LEVEL9_HARKONNEN = 7,
        HOUSEANIMATION_LEVEL9_ARTREIDES = 8,
        HOUSEANIMATION_LEVEL9_ORDOS = 9,

        HOUSEANIMATION_MAX = 10
    }

    /*
     * Types of special %House Weapons available in the game.
     */
    enum HouseWeapon
    {
        HOUSE_WEAPON_MISSILE = 1,
        HOUSE_WEAPON_FREMEN = 2,
        HOUSE_WEAPON_SABOTEUR = 3,

        HOUSE_WEAPON_INVALID = 0xFF
    }

    /*
     * Flags used to indicate houses in a bitmask.
     */
    [Flags]
    enum HouseFlag
    {
        FLAG_HOUSE_HARKONNEN = 1 << HouseType.HOUSE_HARKONNEN, /* 0x01 */
        FLAG_HOUSE_ATREIDES = 1 << HouseType.HOUSE_ATREIDES,  /* 0x02 */
        FLAG_HOUSE_ORDOS = 1 << HouseType.HOUSE_ORDOS,     /* 0x04 */
        FLAG_HOUSE_FREMEN = 1 << HouseType.HOUSE_FREMEN,    /* 0x08 */
        FLAG_HOUSE_SARDAUKAR = 1 << HouseType.HOUSE_SARDAUKAR, /* 0x10 */
        FLAG_HOUSE_MERCENARY = 1 << HouseType.HOUSE_MERCENARY, /* 0x20 */

        FLAG_HOUSE_ALL = FLAG_HOUSE_MERCENARY | FLAG_HOUSE_SARDAUKAR | FLAG_HOUSE_FREMEN | FLAG_HOUSE_ORDOS | FLAG_HOUSE_ATREIDES | FLAG_HOUSE_HARKONNEN
    }

    /*
     * The information for a single animation frame in House Animation. It is part
     *  of an array that stops when duration is 0.
     */
    class HouseAnimation_Animation
    {
        internal string str; //[8];                           /*!< Name of the WSA for this animation. */
        internal byte duration;                               /*!< Duration of this animation (in 1/10th sec). */
        internal byte frameCount;                             /*!< Amount of frames in this animation. */
        internal ushort flags;                                /*!< Flags of the animation. */
    }

    /*
     * Subtitle information part of House Information. It is part of an array that
     *  stops when stringID is 0xFFFF.
     */
    class HouseAnimation_Subtitle
    {
        internal ushort stringID;                             /*!< StringID for the subtitle. */
        internal ushort colour;                               /*!< Colour of the subtitle. */
        internal byte animationID;                            /*!< To which AnimationID this Subtitle belongs. */
        internal byte top;                                    /*!< The top of the subtitle, in pixels. */
        internal byte waitFadein;                             /*!< How long to wait before we fadein this Subtitle. */
        internal byte paletteFadein;                          /*!< How many ticks the palette update should take when appearing. */
        internal byte waitFadeout;                            /*!< How long to wait before we fadeout this Subtitle. */
        internal byte paletteFadeout;                         /*!< How many ticks the palette update should take when disappearing. */
    }

    /*
     * Voice information part of House Information. It is part of an array that
     *  stops when voiceID is 0xFF.
     */
    class HouseAnimation_SoundEffect
    {
        internal byte animationID;                            /*!< The which AnimationID this SoundEffect belongs. */
        internal byte voiceID;                                /*!< The SoundEffect to play. */
        internal byte wait;                                   /*!< How long to wait before we play this SoundEffect. */
    }

    /*
    * Flags for House structure
    */
    class HouseFlags
    {
        internal bool used;                             /*!< The House is in use (no longer free in the pool). */
        internal bool human;                            /*!< The House is controlled by a human. */
        internal bool doneFullScaleAttack;              /*!< The House did his one time attack the human with everything we have. */
        internal bool isAIActive;                       /*!< The House has been seen by the human, and everything now becomes active (Team attack, house missiles, rebuilding, ..). */
        internal bool radarActivated;                   /*!< The radar is activated. */
        internal bool unused_0020;                      /*!< Unused */
        internal uint all
        {
            get
            {
                var value = 0U;
                if (used) value |= 1U << 0;
                if (human) value |= 1U << 1;
                if (doneFullScaleAttack) value |= 1U << 2;
                if (isAIActive) value |= 1U << 3;
                if (radarActivated) value |= 1U << 4;
                return value;
            }
        }
    }

    /*
    * A House as stored in the memory.
    */
    class House
    {
        internal byte index;                                       /*!< The index of the House in the array. */
        internal ushort harvestersIncoming;                        /*!< How many harvesters are waiting to be delivered. Only happens when we run out of Units to do it immediately. */
        internal HouseFlags flags;                                 /*!< General flags of the House. */
        internal ushort unitCount;                                 /*!< Amount of units owned by House. */
        internal ushort unitCountMax;                              /*!< Maximum amount of units this House is allowed to have. */
        internal ushort unitCountEnemy;                            /*!< Amount of units owned by enemy. */
        internal ushort unitCountAllied;                           /*!< Amount of units owned by allies. */
        internal uint structuresBuilt;                             /*!< The Nth bit active means the Nth structure type is built (one or more). */
        internal ushort credits;                                   /*!< Amount of credits the House currently has. */
        internal ushort creditsStorage;                            /*!< Amount of credits the House can store. */
        internal ushort powerProduction;                           /*!< Amount of power the House produces. */
        internal ushort powerUsage;                                /*!< Amount of power the House requires. */
        internal ushort windtrapCount;                             /*!< Amount of windtraps the House currently has. */
        internal ushort creditsQuota;                              /*!< Quota house has to reach to win the mission. */
        internal tile32 palacePosition;                            /*!< Position of the Palace. */
        internal ushort timerUnitAttack;                           /*!< Timer to count down when next 'unit approaching' message can be showed again. */
        internal ushort timerSandwormAttack;                       /*!< Timer to count down when next 'sandworm approaching' message can be showed again. */
        internal ushort timerStructureAttack;                      /*!< Timer to count down when next 'base is under attack' message can be showed again. */
        internal ushort starportTimeLeft;                          /*!< How much time is left before starport transport arrives. */
        internal ushort starportLinkedID;                          /*!< If there is a starport delivery, this indicates the first unit of the linked list. Otherwise it is 0xFFFF. */
        internal ushort[][] ai_structureRebuild; //[5][2]          /*!< An array for the AI which stores the type and position of a destroyed structure, for rebuilding. */
         
        internal House()
        {
            flags = new HouseFlags();
            palacePosition = new tile32();
            ai_structureRebuild = new ushort[][] { new ushort[2], new ushort[2], new ushort[2], new ushort[2], new ushort[2] };
        }
    }

    /*
     * Static information per House type.
     */
    class HouseInfo
    {
        internal string name;                                   /*!< Pointer to name of house. */
        internal ushort toughness;                              /*!< How though the House is. Gives probability of deviation and chance of retreating. */
        internal ushort degradingChance;                        /*!< On Unit create, this is the chance a Unit will be set to 'degrading'. */
        internal ushort degradingAmount;                        /*!< Amount of damage dealt to degrading Structures. */
        internal ushort minimapColor;                           /*!< The color used on the minimap. */
        internal ushort specialCountDown;                       /*!< Time between activation of Special Weapon. */
        internal ushort starportDeliveryTime;                   /*!< Time it takes for a starport delivery. */
        internal ushort prefixChar;                             /*!< Char used as prefix for some filenames. */
        internal ushort specialWeapon;                          /*!< Which Special Weapon this House has. @see HouseWeapon. */
        internal ushort musicWin;                               /*!< Music played when you won a mission. */
        internal ushort musicLose;                              /*!< Music played when you lose a mission. */
        internal ushort musicBriefing;                          /*!< Music played during initial briefing of mission. */
        internal string voiceFilename;                          /*!< Pointer to filename with the voices of the house. */
    }

    class CHouse
    {
        /*
         * HouseAnimation flags
         * see GameLoop_PlayAnimation()
         */
        internal const ushort HOUSEANIM_FLAGS_MODE0 = 0;	        /* no WSA, only text or voice */
        internal const ushort HOUSEANIM_FLAGS_MODE1 = 1;	        /* WSA Looping */
        internal const ushort HOUSEANIM_FLAGS_MODE2 = 2;	        /* WSA display from first to end frame*/
        internal const ushort HOUSEANIM_FLAGS_MODE3 = 3;	        /* display WSA unique frame (frameCount field) */
        internal const ushort HOUSEANIM_FLAGS_FADEINTEXT = 0x04;	/* fade in text at the beginning */
        internal const ushort HOUSEANIM_FLAGS_FADEOUTTEXT = 0x08;	/* fade out text at the end */
        internal const ushort HOUSEANIM_FLAGS_FADETOWHITE = 0x10;	/* Fade palette to all while at the end */
        internal const ushort HOUSEANIM_FLAGS_POS0_0 = 0x20;	    /* Position (0,0) - default is (8,24) */
        internal const ushort HOUSEANIM_FLAGS_DISPLAYFRAME = 0x40;  /* force display in frame buffer (not screen) */
        internal const ushort HOUSEANIM_FLAGS_FADEIN2 = 0x80;	    /*  */
        internal const ushort HOUSEANIM_FLAGS_FADEIN = 0x400;	    /*  */

        internal static HouseInfo[] g_table_houseInfo = { //[HOUSE_MAX]
            new HouseInfo {
		        name = "Harkonnen",
		        toughness = 200,
		        degradingChance = 85,
		        degradingAmount = 3,
		        minimapColor = 144,
		        specialCountDown = 600,
		        starportDeliveryTime = 10,
		        prefixChar = 'H',
		        specialWeapon = 1,
		        musicWin = 6,
		        musicLose = 3,
		        musicBriefing = 24,
		        voiceFilename = "nhark.voc"
            },

            new HouseInfo {
		        name = "Atreides",
		        toughness = 77,
		        degradingChance = 0,
		        degradingAmount = 1,
		        minimapColor = 160,
		        specialCountDown = 300,
		        starportDeliveryTime = 10,
		        prefixChar = 'A',
		        specialWeapon = 2,
		        musicWin = 7,
		        musicLose = 4,
		        musicBriefing = 25,
		        voiceFilename = "nattr.voc"
            },

            new HouseInfo {
		        name = "Ordos",
		        toughness = 128,
		        degradingChance = 10,
		        degradingAmount = 2,
		        minimapColor = 176,
		        specialCountDown = 300,
		        starportDeliveryTime = 10,
		        prefixChar = 'O',
		        specialWeapon = 3,
		        musicWin = 5,
		        musicLose = 2,
		        musicBriefing = 26,
		        voiceFilename = "nordo.voc"
            },

            new HouseInfo {
		        name = "Fremen",
		        toughness = 10,
		        degradingChance = 0,
		        degradingAmount = 1,
		        minimapColor = 192,
		        specialCountDown = 300,
		        starportDeliveryTime = 0,
		        prefixChar = 'O',
		        specialWeapon = 2,
		        musicWin = 5,
		        musicLose = 2,
		        musicBriefing = 65535,
		        voiceFilename = "afremen.voc"
            },

            new HouseInfo {
		        name = "Sardaukar",
		        toughness = 10,
		        degradingChance = 0,
		        degradingAmount = 1,
		        minimapColor = 208,
		        specialCountDown = 600,
		        starportDeliveryTime = 0,
		        prefixChar = 'H',
		        specialWeapon = 1,
		        musicWin = 6,
		        musicLose = 3,
		        musicBriefing = 65535,
		        voiceFilename = "asard.voc"
            },

            new HouseInfo {
		        name = "Mercenary",
		        toughness = 0,
		        degradingChance = 0,
		        degradingAmount = 1,
		        minimapColor = 224,
		        specialCountDown = 300,
		        starportDeliveryTime = 0,
		        prefixChar = 'M',
		        specialWeapon = 3,
		        musicWin = 7,
		        musicLose = 4,
		        musicBriefing = 65535,
		        voiceFilename = "amerc.voc"
            }
        };

        internal static House g_playerHouse;
        internal static HouseType g_playerHouseID = HouseType.HOUSE_INVALID;
        internal static ushort g_houseMissileCountdown = 0;
        internal static ushort g_playerCreditsNoSilo = 0;
        internal static ushort g_playerCredits = 0; /*!< Credits shown to player as 'current'. */
        internal static uint g_tickHousePowerMaintenance = 0;

        static uint s_tickHouseHouse = 0;
        static uint s_tickHouseStarport = 0;
        static uint s_tickHouseReinforcement = 0;
        static uint s_tickHouseMissileCountdown = 0;
        static uint s_tickHouseStarportAvailability = 0;

        static House[] g_houseArray = new House[(int)HouseIndex.HOUSE_INDEX_MAX];
        static House[] g_houseFindArray = new House[(int)HouseIndex.HOUSE_INDEX_MAX];
        static ushort g_houseFindCount;

        /*
         * Get a House from the pool with the indicated index.
         *
         * @param index The index of the House to get.
         * @return The House.
         */
        internal static House House_Get_ByIndex(byte index)
        {
            Debug.Assert(index < (byte)HouseIndex.HOUSE_INDEX_MAX);
            return g_houseArray[index];
        }

        //internal static void House_Set_ByIndex(House h)
        //{
        //    Debug.Assert(h.index < (byte)HouseIndex.HOUSE_INDEX_MAX);
        //    g_houseArray[h.index] = h;
        //}

        /*
         * Checks if two houses are allied.
         *
         * @param houseID1 The index of the first house.
         * @param houseID2 The index of the second house.
         * @return True if and only if the two houses are allies of eachother.
         */
        internal static bool House_AreAllied(byte houseID1, byte houseID2)
        {
            if (houseID1 == (byte)HouseType.HOUSE_INVALID || houseID2 == (byte)HouseType.HOUSE_INVALID) return false;

            if (houseID1 == houseID2) return true;

            if (houseID1 == (byte)HouseType.HOUSE_FREMEN || houseID2 == (byte)HouseType.HOUSE_FREMEN)
            {
                return (houseID1 == (byte)HouseType.HOUSE_ATREIDES || houseID2 == (byte)HouseType.HOUSE_ATREIDES);
            }

            return (houseID1 != (byte)g_playerHouseID && houseID2 != (byte)g_playerHouseID);
        }

        /*
         * Find the first matching House based on the PoolFindStruct filter data.
         *
         * @param find A pointer to a PoolFindStruct which contains filter data and
         *   last known tried index. Calling this functions multiple times with the
         *   same 'find' parameter walks over all possible values matching the filter.
         * @return The House, or NULL if nothing matches (anymore).
         */
        internal static House House_Find(PoolFindStruct find)
        {
            if (find.index >= g_houseFindCount && find.index != 0xFFFF) return null;
            find.index++; /* First, we always go to the next index */

            for (; find.index < g_houseFindCount; find.index++)
            {
                var h = g_houseFindArray[find.index];
                if (h != null) return h;
            }

            return null;
        }

        /*
         * Update the CreditsStorage by walking over all structures and checking what
         *  they can hold.
         * @param houseID The house to check the storage for.
         */
        internal static void House_UpdateCreditsStorage(byte houseID)
        {
            var find = new PoolFindStruct();
            uint creditsStorage;

            var oldValidateStrictIfZero = CSharpDune.g_validateStrictIfZero;
            CSharpDune.g_validateStrictIfZero = 0;

            find.houseID = houseID;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            creditsStorage = 0;
            while (true)
            {
                StructureInfo si;
                Structure s;

                s = CStructure.Structure_Find(find);
                if (s == null) break;

                si = CStructure.g_table_structureInfo[s.o.type];
                creditsStorage += si.creditsStorage;
            }

            if (creditsStorage > 32000) creditsStorage = 32000;

            House_Get_ByIndex(houseID).creditsStorage = (ushort)creditsStorage;

            CSharpDune.g_validateStrictIfZero = oldValidateStrictIfZero;
        }

        /*
         * Calculate the power usage and production, and the credits storage.
         *
         * @param h The house to calculate the numbers for.
         */
        internal static void House_CalculatePowerAndCredit(House h)
        {
            var find = new PoolFindStruct();

            if (h == null) return;

            h.powerUsage = 0;
            h.powerProduction = 0;
            h.creditsStorage = 0;

            find.houseID = h.index;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            while (true)
            {
                StructureInfo si;
                Structure s;

                s = CStructure.Structure_Find(find);
                if (s == null) break;
                /* ENHANCEMENT -- Only count structures that are placed on the map, not ones we are building. */
                if (CSharpDune.g_dune2_enhanced && s.o.flags.isNotOnMap) continue;

                si = CStructure.g_table_structureInfo[s.o.type];

                h.creditsStorage += si.creditsStorage;

                /* Positive values means usage */
                if (si.powerUsage >= 0)
                {
                    h.powerUsage += (ushort)si.powerUsage;
                    continue;
                }

                /* Negative value and full health means everything goes to production */
                if (s.o.hitpoints >= si.o.hitpoints)
                {
                    h.powerProduction += (ushort)-si.powerUsage;
                    continue;
                }

                /* Negative value and partial health, calculate how much should go to production (capped at 50%) */
                /* ENHANCEMENT -- The 50% cap of Dune2 is silly and disagress with the GUI. If your hp is 10%, so should the production. */
                if (!CSharpDune.g_dune2_enhanced && s.o.hitpoints <= si.o.hitpoints / 2)
                {
                    h.powerProduction += (ushort)((-si.powerUsage) / 2);
                    continue;
                }
                h.powerProduction += (ushort)((-si.powerUsage) * s.o.hitpoints / si.o.hitpoints);
            }

            /* Check if we are low on power */
            if (h.index == (byte)g_playerHouseID && h.powerUsage > h.powerProduction)
            {
                Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_INSUFFICIENT_POWER_WINDTRAP_IS_NEEDED), 1);
            }

            /* If there are no buildings left, you lose your right on 'credits without storage' */
            if (h.index == (byte)g_playerHouseID && h.structuresBuilt == 0 && CSharpDune.g_validateStrictIfZero == 0)
            {
                g_playerCreditsNoSilo = 0;
            }
        }

        /*
         * Updates the radar state for the given house.
         * @param h The house.
         * @return True if and only if the radar has been activated.
         */
        internal static bool House_UpdateRadarState(House h)
        {
            /*WSAObject*/(WSAHeader header, CArray<byte> buffer) wsa;
            ushort frame;
            ushort frameCount;
            bool activate;

            if (h == null || h.index != (byte)g_playerHouseID) return false;

            wsa = (null, null);

            activate = h.flags.radarActivated;

            if (h.flags.radarActivated)
            {
                /* Deactivate radar */
                if ((h.structuresBuilt & (1 << (byte)StructureType.STRUCTURE_OUTPOST)) == 0 || h.powerProduction < h.powerUsage) activate = false;
            }
            else
            {
                /* Activate radar */
                if ((h.structuresBuilt & (1 << (byte)StructureType.STRUCTURE_OUTPOST)) != 0 && h.powerProduction >= h.powerUsage) activate = true;
            }

            if (h.flags.radarActivated == activate) return false;

            wsa = Wsa.WSA_LoadFile("STATIC.WSA", (byte[])Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_1), Gfx.GFX_Screen_GetSize_ByIndex(Screen.SCREEN_1), true);
            frameCount = Wsa.WSA_GetFrameCount(wsa);

            Gui.g_textDisplayNeedsUpdate = true;

            Gui.GUI_Mouse_Hide_Safe();

            while (CDriver.Driver_Voice_IsPlaying()) Sleep.sleepIdle();

            Sound.Voice_Play(62);

            Sound.Sound_Output_Feedback((ushort)(activate ? 28 : 29));

            frameCount = Wsa.WSA_GetFrameCount(wsa);

            for (frame = 0; frame < frameCount; frame++)
            {
                Wsa.WSA_DisplayFrame(wsa, (ushort)(activate ? frameCount - frame : frame), 256, 136, Screen.SCREEN_0);
                Gui.GUI_PaletteAnimate();

                Timer.Timer_Sleep(3);
            }

            h.flags.radarActivated = activate;

            Wsa.WSA_Unload(wsa);

            CSharpDune.g_viewport_forceRedraw = true;

            Gui.GUI_Mouse_Show_Safe();

            Viewport.GUI_Widget_Viewport_RedrawMap(Screen.SCREEN_0);

            return activate;
        }

        /*
         * Initialize the House array.
         *
         * @param address If non-zero, the new location of the House array.
         */
        internal static void House_Init()
        {
            for (var i = 0; i < g_houseArray.Length; i++) g_houseArray[i] = new House(); //memset(g_houseArray, 0, sizeof(g_houseArray));
            Array.Fill(g_houseFindArray, null, 0, g_houseFindArray.Length); //memset(g_houseFindArray, 0, sizeof(g_houseFindArray));
            g_houseFindCount = 0;
        }

        /*
         * Loop over all houses, preforming various of tasks.
         */
        internal static void GameLoop_House()
        {
            var find = new PoolFindStruct();
            House h; // = NULL;
            var tickHouse = false;
            var tickPowerMaintenance = false;
            var tickStarport = false;
            var tickReinforcement = false;
            var tickMissileCountdown = false;
            var tickStarportAvailability = false;

            if (CSharpDune.g_debugScenario) return;

            if (s_tickHouseHouse <= Timer.g_timerGame)
            {
                tickHouse = true;
                s_tickHouseHouse = Timer.g_timerGame + 900;
            }

            if (g_tickHousePowerMaintenance <= Timer.g_timerGame)
            {
                tickPowerMaintenance = true;
                g_tickHousePowerMaintenance = Timer.g_timerGame + 10800;
            }

            if (s_tickHouseStarport <= Timer.g_timerGame)
            {
                tickStarport = true;
                s_tickHouseStarport = Timer.g_timerGame + 180;
            }

            if (s_tickHouseReinforcement <= Timer.g_timerGame)
            {
                tickReinforcement = true;
                s_tickHouseReinforcement = (uint)(Timer.g_timerGame + (CSharpDune.g_debugGame ? 60 : 600));
            }

            if (s_tickHouseMissileCountdown <= Timer.g_timerGame)
            {
                tickMissileCountdown = true;
                s_tickHouseMissileCountdown = Timer.g_timerGame + 60;
            }

            if (s_tickHouseStarportAvailability <= Timer.g_timerGame)
            {
                tickStarportAvailability = true;
                s_tickHouseStarportAvailability = Timer.g_timerGame + 1800;
            }

            if (tickMissileCountdown && g_houseMissileCountdown != 0)
            {
                g_houseMissileCountdown--;
                Sound.Sound_Output_Feedback((ushort)(g_houseMissileCountdown + 41));

                if (g_houseMissileCountdown == 0) CUnit.Unit_LaunchHouseMissile(Map.Map_FindLocationTile(4, (byte)g_playerHouseID));
            }

            if (tickStarportAvailability)
            {
                ushort type;

                /* Pick a random unit to increase starport availability */
                type = Tools.Tools_RandomLCG_Range(0, (ushort)(UnitType.UNIT_MAX - 1));

                /* Increase how many of this unit is available via starport by one */
                if (CUnit.g_starportAvailable[type] != 0 && CUnit.g_starportAvailable[type] < 10)
                {
                    if (CUnit.g_starportAvailable[type] == -1)
                    {
                        CUnit.g_starportAvailable[type] = 1;
                    }
                    else
                    {
                        CUnit.g_starportAvailable[type]++;
                    }
                }
            }

            if (tickReinforcement)
            {
                Unit nu = null;
                int i;

                for (i = 0; i < 16; i++)
                {
                    ushort locationID;
                    bool deployed;
                    Unit u;

                    if (CScenario.g_scenario.reinforcement[i].unitID == (ushort)UnitIndex.UNIT_INDEX_INVALID) continue;
                    if (CScenario.g_scenario.reinforcement[i].timeLeft == 0) continue;
                    if (--CScenario.g_scenario.reinforcement[i].timeLeft != 0) continue;

                    u = CUnit.Unit_Get_ByIndex(CScenario.g_scenario.reinforcement[i].unitID);

                    locationID = CScenario.g_scenario.reinforcement[i].locationID;
                    deployed = false;

                    if (locationID >= 4)
                    {
                        if (nu == null)
                        {
                            nu = CUnit.Unit_Create((ushort)UnitIndex.UNIT_INDEX_INVALID, (byte)UnitType.UNIT_CARRYALL, u.o.houseID, CTile.Tile_UnpackTile(Map.Map_FindLocationTile((ushort)(Tools.Tools_Random_256() & 3), u.o.houseID)), 100);

                            if (nu != null)
                            {
                                nu.o.flags.byScenario = true;
                                CUnit.Unit_SetDestination(nu, Tools.Tools_Index_Encode(Map.Map_FindLocationTile(locationID, u.o.houseID), IndexType.IT_TILE));
                            }
                        }

                        if (nu != null)
                        {
                            u.o.linkedID = nu.o.linkedID;
                            nu.o.linkedID = (byte)u.o.index;
                            nu.o.flags.inTransport = true;
                            CScenario.g_scenario.reinforcement[i].unitID = (ushort)UnitIndex.UNIT_INDEX_INVALID;
                            deployed = true;
                        }
                        else
                        {
                            /* Failed to create carry-all, try again in a short moment */
                            CScenario.g_scenario.reinforcement[i].timeLeft = 1;
                        }
                    }
                    else
                    {
                        deployed = CUnit.Unit_SetPosition(u, CTile.Tile_UnpackTile(Map.Map_FindLocationTile(locationID, u.o.houseID)));
                    }

                    if (deployed && CScenario.g_scenario.reinforcement[i].repeat != 0)
                    {
                        var tile = new tile32();
                        tile.x = 0xFFFF;
                        tile.y = 0xFFFF;

                        CSharpDune.g_validateStrictIfZero++;
                        u = CUnit.Unit_Create((ushort)UnitIndex.UNIT_INDEX_INVALID, u.o.type, u.o.houseID, tile, 0);
                        CSharpDune.g_validateStrictIfZero--;

                        if (u != null)
                        {
                            CScenario.g_scenario.reinforcement[i].unitID = u.o.index;
                            CScenario.g_scenario.reinforcement[i].timeLeft = CScenario.g_scenario.reinforcement[i].timeBetween;
                        }
                    }
                }
            }

            find.houseID = (byte)HouseType.HOUSE_INVALID;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            while (true)
            {
                h = House_Find(find);
                if (h == null) break;

                if (tickHouse)
                {
                    /* ENHANCEMENT -- Originally this code was outside the house loop, which seems very odd.
			         *  This problem is considered to be so bad, that the original code has been removed. */
                    if (h.index != (byte)g_playerHouseID)
                    {
                        if (h.creditsStorage < h.credits)
                        {
                            h.credits = h.creditsStorage;
                        }
                    }
                    else
                    {
                        var maxCredits = Max(h.creditsStorage, g_playerCreditsNoSilo);
                        if (h.credits > maxCredits)
                        {
                            h.credits = maxCredits;

                            Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_INSUFFICIENT_SPICE_STORAGE_AVAILABLE_SPICE_IS_LOST), 1);
                        }
                    }

                    if (h.index == (byte)g_playerHouseID)
                    {
                        if (h.creditsStorage > g_playerCreditsNoSilo)
                        {
                            g_playerCreditsNoSilo = 0;
                        }

                        if (g_playerCreditsNoSilo == 0 && CSharpDune.g_campaignID > 1 && h.credits != 0)
                        {
                            if (h.creditsStorage != 0 && ((h.credits * 256 / h.creditsStorage) > 200))
                            {
                                Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_SPICE_STORAGE_CAPACITY_LOW_BUILD_SILOS), 0);
                            }
                        }

                        if (h.credits < 100 && g_playerCreditsNoSilo != 0)
                        {
                            Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_CREDITS_ARE_LOW_HARVEST_SPICE_FOR_MORE_CREDITS), 0);
                        }
                    }
                }

                if (tickHouse) House_EnsureHarvesterAvailable(h.index);

                if (tickStarport && h.starportLinkedID != (ushort)UnitIndex.UNIT_INDEX_INVALID)
                {
                    Unit u = null;

                    h.starportTimeLeft--;
                    if ((short)h.starportTimeLeft < 0) h.starportTimeLeft = 0;

                    if (h.starportTimeLeft == 0)
                    {
                        Structure s;

                        s = CStructure.Structure_Get_ByIndex(CStructure.g_structureIndex);
                        if (s.o.type == (byte)StructureType.STRUCTURE_STARPORT && s.o.houseID == h.index)
                        {
                            u = CUnit.Unit_CreateWrapper(h.index, UnitType.UNIT_FRIGATE, Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE));
                        }
                        else
                        {
                            var find2 = new PoolFindStruct();

                            find2.houseID = h.index;
                            find2.index = 0xFFFF;
                            find2.type = (ushort)StructureType.STRUCTURE_STARPORT;

                            while (true)
                            {
                                s = CStructure.Structure_Find(find2);
                                if (s == null) break;
                                if (s.o.linkedID != 0xFF) continue;

                                u = CUnit.Unit_CreateWrapper(h.index, UnitType.UNIT_FRIGATE, Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE));
                                break;
                            }
                        }

                        if (u != null)
                        {
                            u.o.linkedID = (byte)h.starportLinkedID;
                            h.starportLinkedID = (ushort)UnitIndex.UNIT_INDEX_INVALID;
                            u.o.flags.inTransport = true;

                            Sound.Sound_Output_Feedback(38);
                        }

                        h.starportTimeLeft = (ushort)((u != null) ? g_table_houseInfo[h.index].starportDeliveryTime : 1);
                    }
                }

                if (tickHouse)
                {
                    House_CalculatePowerAndCredit(h);
                    CStructure.Structure_CalculateHitpointsMax(h);

                    if (h.timerUnitAttack != 0) h.timerUnitAttack--;
                    if (h.timerSandwormAttack != 0) h.timerSandwormAttack--;
                    if (h.timerStructureAttack != 0) h.timerStructureAttack--;
                    if (h.harvestersIncoming > 0 && CUnit.Unit_CreateWrapper(h.index, UnitType.UNIT_HARVESTER, 0) != null) h.harvestersIncoming--;
                }

                if (tickPowerMaintenance)
                {
                    var powerMaintenanceCost = (ushort)((h.powerUsage / 32) + 1);
                    h.credits -= Min(h.credits, powerMaintenanceCost);
                }
            }
        }

        static string[] houseWSAFileNames = { "FHARK.WSA", "FARTR.WSA", "FORDOS.WSA" };
        internal static string House_GetWSAHouseFilename(byte houseID)
        {
            if (houseID >= 3) return null;
            return houseWSAFileNames[houseID];
        }

        /*
         * Gives a harvester to the given house if it has a refinery and no harvesters.
         *
         * @param houseID The index of the house to give a harvester to.
         */
        static void House_EnsureHarvesterAvailable(byte houseID)
        {
            var find = new PoolFindStruct();
            Structure s;

            find.houseID = houseID;
            find.type = 0xFFFF;
            find.index = 0xFFFF;

            while (true)
            {
                s = CStructure.Structure_Find(find);
                if (s == null) break;
                /* ENHANCEMENT -- Dune2 checked the wrong type to skip. LinkedID is a structure for a Construction Yard */
                if (!CSharpDune.g_dune2_enhanced && s.o.type == (byte)StructureType.STRUCTURE_HEAVY_VEHICLE) continue;
                if (CSharpDune.g_dune2_enhanced && s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD) continue;
                if (s.o.linkedID == (byte)UnitType.UNIT_INVALID) continue;
                if (CUnit.Unit_Get_ByIndex(s.o.linkedID).o.type == (byte)UnitType.UNIT_HARVESTER) return;
            }

            find.houseID = houseID;
            find.type = (ushort)UnitType.UNIT_CARRYALL;
            find.index = 0xFFFF;

            while (true)
            {
                Unit u;

                u = CUnit.Unit_Find(find);
                if (u == null) break;
                if (u.o.linkedID == (byte)UnitType.UNIT_INVALID) continue;
                if (CUnit.Unit_Get_ByIndex(u.o.linkedID).o.type == (byte)UnitType.UNIT_HARVESTER) return;
            }

            if (CUnit.Unit_IsTypeOnMap(houseID, (byte)UnitType.UNIT_HARVESTER)) return;

            find.houseID = houseID;
            find.type = (ushort)StructureType.STRUCTURE_REFINERY;
            find.index = 0xFFFF;

            s = CStructure.Structure_Find(find);
            if (s == null) return;

            if (CUnit.Unit_CreateWrapper(houseID, UnitType.UNIT_HARVESTER, Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE)) == null) return;

            if (houseID != (byte)g_playerHouseID) return;

            Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_HARVESTER_IS_HEADING_TO_REFINERY), 0);
        }

        /*
         * Convert the name of a house to the type value of that house, or
         *  HOUSE_INVALID if not found.
         */
        internal static byte House_StringToType(string name)
        {
            byte index;
            if (name == null) return (byte)HouseType.HOUSE_INVALID;

            for (index = 0; index < 6; index++)
            {
                if (string.Equals(g_table_houseInfo[index].name, name, StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(g_table_houseInfo[index].name, name) == 0)
                    return index;
            }

            return (byte)HouseType.HOUSE_INVALID;
        }

        /*
         * Allocate a House.
         *
         * @param index The index to use.
         * @return The House allocated, or NULL on failure.
         */
        internal static House House_Allocate(byte index)
        {
            House h;

            if (index >= (byte)HouseIndex.HOUSE_INDEX_MAX) return null;

            h = House_Get_ByIndex(index);
            if (h.flags.used) return null;

            /* Initialize the House */
            //memset(h, 0, sizeof(House));
            h.index = index;
            h.flags.used = true;
            h.starportLinkedID = (ushort)UnitIndex.UNIT_INDEX_INVALID;

            g_houseFindArray[g_houseFindCount++] = h;

            return h;
        }

        //internal static void House_Allocate(House h)
        //{
        //    if (h.index >= (byte)HouseIndex.HOUSE_INDEX_MAX) return;

        //    House_Set_ByIndex(h);

        //    g_houseFindArray[g_houseFindCount++] = h;
        //}
    }
}
