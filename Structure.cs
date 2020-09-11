/* Structure handling */

using System;
using System.Diagnostics;
using static System.Math;

namespace SharpDune
{
    /*
    * Types of Structures available in the game.
    */
    enum StructureType
    {
        STRUCTURE_SLAB_1x1 = 0,
        STRUCTURE_SLAB_2x2 = 1,
        STRUCTURE_PALACE = 2,
        STRUCTURE_LIGHT_VEHICLE = 3,
        STRUCTURE_HEAVY_VEHICLE = 4,
        STRUCTURE_HIGH_TECH = 5,
        STRUCTURE_HOUSE_OF_IX = 6,
        STRUCTURE_WOR_TROOPER = 7,
        STRUCTURE_CONSTRUCTION_YARD = 8,
        STRUCTURE_WINDTRAP = 9,
        STRUCTURE_BARRACKS = 10,
        STRUCTURE_STARPORT = 11,
        STRUCTURE_REFINERY = 12,
        STRUCTURE_REPAIR = 13,
        STRUCTURE_WALL = 14,
        STRUCTURE_TURRET = 15,
        STRUCTURE_ROCKET_TURRET = 16,
        STRUCTURE_SILO = 17,
        STRUCTURE_OUTPOST = 18,

        STRUCTURE_MAX = 19,
        STRUCTURE_INVALID = 0xFF
    }

    enum StructureIndex
    {
        STRUCTURE_INDEX_MAX_SOFT = 79,                          /*!< The highest possible index for normal Structure. */
        STRUCTURE_INDEX_MAX_HARD = 82,                          /*!< The highest possible index for any Structure. */

        STRUCTURE_INDEX_WALL = 79,                              /*!< All walls are are put under index 79. */
        STRUCTURE_INDEX_SLAB_2x2 = 80,                          /*!< All 2x2 slabs are put under index 80. */
        STRUCTURE_INDEX_SLAB_1x1 = 81,                          /*!< All 1x1 slabs are put under index 81. */

        STRUCTURE_INDEX_INVALID = 0xFFFF
    }

    /* Available structure layouts. */
    enum StructureLayout
    {
        STRUCTURE_LAYOUT_1x1 = 0,
        STRUCTURE_LAYOUT_2x1 = 1,
        STRUCTURE_LAYOUT_1x2 = 2,
        STRUCTURE_LAYOUT_2x2 = 3,
        STRUCTURE_LAYOUT_2x3 = 4,
        STRUCTURE_LAYOUT_3x2 = 5,
        STRUCTURE_LAYOUT_3x3 = 6,

        STRUCTURE_LAYOUT_MAX = 7
    }

    /* States a structure can be in */
    enum StructureState
    {
        STRUCTURE_STATE_DETECT = -2,                            /*!< Used when setting state, meaning to detect which state it has by looking at other properties. */
        STRUCTURE_STATE_JUSTBUILT = -1,                         /*!< This shows you the building animation etc. */
        STRUCTURE_STATE_IDLE = 0,                               /*!< Structure is doing nothing. */
        STRUCTURE_STATE_BUSY = 1,                               /*!< Structure is busy (harvester in refinery, unit in repair, .. */
        STRUCTURE_STATE_READY = 2                               /*!< Structure is ready and unit will be deployed soon. */
    }

    /*
     * Flags used to indicate structures in a bitmask.
     */
    [Flags]
    enum StructureFlag
    {
        FLAG_STRUCTURE_SLAB_1x1 = 1 << StructureType.STRUCTURE_SLAB_1x1,          /* 0x____01 */
        FLAG_STRUCTURE_SLAB_2x2 = 1 << StructureType.STRUCTURE_SLAB_2x2,          /* 0x____02 */
        FLAG_STRUCTURE_PALACE = 1 << StructureType.STRUCTURE_PALACE,            /* 0x____04 */
        FLAG_STRUCTURE_LIGHT_VEHICLE = 1 << StructureType.STRUCTURE_LIGHT_VEHICLE,     /* 0x____08 */
        FLAG_STRUCTURE_HEAVY_VEHICLE = 1 << StructureType.STRUCTURE_HEAVY_VEHICLE,     /* 0x____10 */
        FLAG_STRUCTURE_HIGH_TECH = 1 << StructureType.STRUCTURE_HIGH_TECH,         /* 0x____20 */
        FLAG_STRUCTURE_HOUSE_OF_IX = 1 << StructureType.STRUCTURE_HOUSE_OF_IX,       /* 0x____40 */
        FLAG_STRUCTURE_WOR_TROOPER = 1 << StructureType.STRUCTURE_WOR_TROOPER,       /* 0x____80 */
        FLAG_STRUCTURE_CONSTRUCTION_YARD = 1 << StructureType.STRUCTURE_CONSTRUCTION_YARD, /* 0x__01__ */
        FLAG_STRUCTURE_WINDTRAP = 1 << StructureType.STRUCTURE_WINDTRAP,          /* 0x__02__ */
        FLAG_STRUCTURE_BARRACKS = 1 << StructureType.STRUCTURE_BARRACKS,          /* 0x__04__ */
        FLAG_STRUCTURE_STARPORT = 1 << StructureType.STRUCTURE_STARPORT,          /* 0x__08__ */
        FLAG_STRUCTURE_REFINERY = 1 << StructureType.STRUCTURE_REFINERY,          /* 0x__10__ */
        FLAG_STRUCTURE_REPAIR = 1 << StructureType.STRUCTURE_REPAIR,            /* 0x__20__ */
        FLAG_STRUCTURE_WALL = 1 << StructureType.STRUCTURE_WALL,              /* 0x__40__ */
        FLAG_STRUCTURE_TURRET = 1 << StructureType.STRUCTURE_TURRET,            /* 0x__80__ */
        FLAG_STRUCTURE_ROCKET_TURRET = 1 << StructureType.STRUCTURE_ROCKET_TURRET,     /* 0x01____ */
        FLAG_STRUCTURE_SILO = 1 << StructureType.STRUCTURE_SILO,              /* 0x02____ */
        FLAG_STRUCTURE_OUTPOST = 1 << StructureType.STRUCTURE_OUTPOST,           /* 0x04____ */

        FLAG_STRUCTURE_NONE = 0,
        FLAG_STRUCTURE_NEVER = -1                                /*!< Special flag to mark that certain buildings can never be built on a Construction Yard. */
    }

    /*
    * A Structure as stored in the memory.
    */
    class Structure
    {
        internal Object o;                                      /*!< Common to Unit and Structures. */
        internal ushort creatorHouseID;                         /*!< The Index of the House who created this Structure. Required in case of take-overs. */
        internal ushort rotationSpriteDiff;                     /*!< Which sprite to show for the current rotation of Turrets etc. */
        internal ushort objectType;                             /*!< Type of Unit/Structure we are building. */
        internal byte upgradeLevel;                             /*!< The current level of upgrade of the Structure. */
        internal byte upgradeTimeLeft;                          /*!< Time left before upgrade is complete, or 0 if no upgrade available. */
        internal ushort countDown;                              /*!< General countdown for various of functions. */
        internal ushort buildCostRemainder;                     /*!< The remainder of the buildCost for next tick. */
        internal short state;                                   /*!< The state of the structure. @see StructureState. */
        internal ushort hitpointsMax;                           /*!< Max amount of hitpoints. */

        internal Structure()
        {
            o = new Object();
        }
    }

    /*
     * Static information per Structure type.
     */
    class StructureInfo
    {
        internal ObjectInfo o;                                  /*!< Common to UnitInfo and StructureInfo. */
        internal uint enterFilter;                              /*!< Bitfield determining which unit is allowed to enter the structure. If bit n is set, then units of type n may enter */
        internal ushort creditsStorage;                         /*!< How many credits this Structure can store. */
        internal short powerUsage;                              /*!< How much power this Structure uses (positive value) or produces (negative value). */
        internal ushort layout;                                 /*!< Layout type of Structure. */
        internal ushort iconGroup;                              /*!< In which IconGroup the sprites of the Structure belongs. */
        internal byte[] animationIndex = new byte[3];           /*!< The index inside g_table_animation_structure for the Animation of the Structure. */
        internal byte[] buildableUnits = new byte[8];           /*!< Which units this structure can produce. */
        internal ushort[] upgradeCampaign = new ushort[3];      /*!< Minimum campaign for upgrades. */
    }

    /* X/Y pair defining a 2D size. */
    class XYSize
    {
        internal ushort width;  /*!< Horizontal length. */
        internal ushort height; /*!< Vertical length. */
    }

    class CStructure
    {
        static ushort g_structureFindCount;
        static Structure[] g_structureArray = new Structure[(int)StructureIndex.STRUCTURE_INDEX_MAX_HARD];
        static Structure[] g_structureFindArray = new Structure[(int)StructureIndex.STRUCTURE_INDEX_MAX_SOFT];

        internal static StructureInfo[] g_table_structureInfo;

        static CStructure()
        {
            unchecked
            {
                g_table_structureInfo = new StructureInfo[] { //[STRUCTURE_MAX]
                    new StructureInfo { /* 0 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_CONCRETE,
                            name = "Concrete",
                            stringID_full = (ushort)Text.STR_SMALL_CONCRETE_SLAB,
                            wsa = "slab.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = false,
                                notOnConcrete = true,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = false,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 0,
                            hitpoints = 20,
                            fogUncoverRadius = 1,
                            spriteID = 65,
                            buildCredits = 5,
                            buildTime = 16,
                            availableCampaign = 1,
                            structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
                            sortPriority = 2,
                            upgradeLevelRequired  = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_CONCRETE_USE_CONCRETE_TO_MAKE_A_STURDY_FOUNDATION_FOR_YOUR_STRUCTURES,
                            priorityBuild = 0,
                            priorityTarget = 5,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 0,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_1x1,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_CONCRETE_SLAB,
                        animationIndex = new byte[] {
                            2,
                            2,
                            2
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            0,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 1 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_CONCRETE_4,
                            name = "Concrete4",
                            stringID_full = (ushort)Text.STR_LARGE_CONCRETE_SLAB,
                            wsa = "4slab.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = false,
                                notOnConcrete = true,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = false,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 0,
                            hitpoints = 20,
                            fogUncoverRadius = 1,
                            spriteID = 83,
                            buildCredits = 20,
                            buildTime = 16,
                            availableCampaign = 4,
                            structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
                            sortPriority = 4,
                            upgradeLevelRequired = 1,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_CONCRETE_USE_CONCRETE_TO_MAKE_A_STURDY_FOUNDATION_FOR_YOUR_STRUCTURES,
                            priorityBuild = 0,
                            priorityTarget = 10,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 0,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_2x2,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_CONCRETE_SLAB,
                        animationIndex = new byte[] {
                            2,
                            2,
                            2
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            0,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 2 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_PALACE,
                            name = "Palace",
                            stringID_full = (ushort)Text.STR_HOUSE_PALACE,
                            wsa = "palace.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = false,
                                notOnConcrete = false,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = false,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 128,
                            hitpoints = 1000,
                            fogUncoverRadius = 5,
                            spriteID = 66,
                            buildCredits = 999,
                            buildTime = 130,
                            availableCampaign = 8,
                            structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_STARPORT,
                            sortPriority = 5,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_PALACE_THIS_IS_YOUR_PALACE,
                            priorityBuild = 0,
                            priorityTarget = 400,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 80,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_3x3,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_HOUSE_PALACE,
                        animationIndex = new byte[] {
                            4,
                            4,
                            4
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            0,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 3 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_LIGHT_FCTRY,
                            name = "Light Fctry",
                            stringID_full = (ushort)Text.STR_LIGHT_VEHICLE_FACTORY,
                            wsa = "liteftry.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = true,
                                notOnConcrete = false,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = true,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 64,
                            hitpoints = 350,
                            fogUncoverRadius = 3,
                            spriteID = 67,
                            buildCredits = 400,
                            buildTime = 96,
                            availableCampaign = 3,
                            structuresRequired = (uint)(StructureFlag.FLAG_STRUCTURE_REFINERY | StructureFlag.FLAG_STRUCTURE_WINDTRAP),
                            sortPriority = 14,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_LIGHT_FACTORY_THE_LIGHT_FACTORY_PRODUCES_LIGHT_ATTACK_VEHICLES,
                            priorityBuild = 0,
                            priorityTarget = 200,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 20,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_2x2,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_LIGHT_VEHICLE_FACTORY,
                        animationIndex = new byte[] {
                            14,
                            15,
                            16
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_TRIKE,
                            (byte)UnitType.UNIT_QUAD,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            3,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 4 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_HEAVY_FCTRY,
                            name = "Heavy Fctry",
                            stringID_full = (ushort)Text.STR_HEAVY_VEHICLE_FACTORY,
                            wsa = "hvyftry.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = true,
                                notOnConcrete = false,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = true,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 64,
                            hitpoints = 200,
                            fogUncoverRadius = 3,
                            spriteID = 68,
                            buildCredits = 600,
                            buildTime = 144,
                            availableCampaign = 4,
                            structuresRequired = (uint)(StructureFlag.FLAG_STRUCTURE_OUTPOST | StructureFlag.FLAG_STRUCTURE_WINDTRAP | StructureFlag.FLAG_STRUCTURE_LIGHT_VEHICLE),
                            sortPriority = 28,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_HEAVY_FACTORY_THE_HEAVY_FACTORY_PRODUCES_TRACKED_VEHICLES,
                            priorityBuild = 0,
                            priorityTarget = 600,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 35,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_3x2,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_HEAVY_VEHICLE_FACTORY,
                        animationIndex = new byte[] {
                            11,
                            12,
                            13
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_SIEGE_TANK,
                            (byte)UnitType.UNIT_LAUNCHER,
                            (byte)UnitType.UNIT_HARVESTER,
                            (byte)UnitType.UNIT_TANK,
                            (byte)UnitType.UNIT_DEVASTATOR,
                            (byte)UnitType.UNIT_DEVIATOR,
                            (byte)UnitType.UNIT_MCV,
                            (byte)UnitType.UNIT_SONIC_TANK,
                        },
                        upgradeCampaign = new ushort[] {
                            4,
                            5,
                            6,
                        },
                    },

                    new StructureInfo { /* 5 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_HITECH,
                            name = "Hi-Tech",
                            stringID_full = (ushort)Text.STR_HITECH_FACTORY,
                            wsa = "hitcftry.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = true,
                                notOnConcrete = false,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = true,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 64,
                            hitpoints = 400,
                            fogUncoverRadius = 3,
                            spriteID = 69,
                            buildCredits = 500,
                            buildTime = 120,
                            availableCampaign = 5,
                            structuresRequired = (uint)(StructureFlag.FLAG_STRUCTURE_OUTPOST | StructureFlag.FLAG_STRUCTURE_WINDTRAP | StructureFlag.FLAG_STRUCTURE_LIGHT_VEHICLE),
                            sortPriority = 30,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_HITECH_FACTORY_THE_HITECH_FACTORY_PRODUCES_FLYING_VEHICLES,
                            priorityBuild = 0,
                            priorityTarget = 200,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 35,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_3x2,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_HI_TECH_FACTORY,
                        animationIndex = new byte[] {
                            8,
                            9,
                            10
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_CARRYALL,
                            (byte)UnitType.UNIT_ORNITHOPTER,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            7,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 6 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_IX,
                            name = "IX",
                            stringID_full = (ushort)Text.STR_HOUSE_OF_IX,
                            wsa = "ix.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = false,
                                notOnConcrete = false,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = false,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 192,
                            hitpoints = 400,
                            fogUncoverRadius = 3,
                            spriteID = 70,
                            buildCredits = 500,
                            buildTime = 120,
                            availableCampaign = 7,
                            structuresRequired = (uint)(StructureFlag.FLAG_STRUCTURE_REFINERY | StructureFlag.FLAG_STRUCTURE_STARPORT | StructureFlag.FLAG_STRUCTURE_WINDTRAP),
                            sortPriority = 34,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_HOUSE_IX_THE_IX_RESEARCH_FACILITY_ADVANCES_YOUR_HOUSES_TECHNOLOGY,
                            priorityBuild = 0,
                            priorityTarget = 100,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 40,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_2x2,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_IX_RESEARCH,
                        animationIndex = new byte[] {
                            20,
                            20,
                            20
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            0,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 7 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_WOR,
                            name = "WOR",
                            stringID_full = (ushort)Text.STR_WOR_TROOPER_FACILITY,
                            wsa = "wor.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = true,
                                notOnConcrete = false,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = false,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 128,
                            hitpoints = 400,
                            fogUncoverRadius = 3,
                            spriteID = 71,
                            buildCredits = 400,
                            buildTime = 104,
                            availableCampaign = 5,
                            structuresRequired = (uint)(StructureFlag.FLAG_STRUCTURE_OUTPOST | StructureFlag.FLAG_STRUCTURE_BARRACKS | StructureFlag.FLAG_STRUCTURE_WINDTRAP),
                            sortPriority = 20,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_WOR_WOR_IS_USED_TO_TRAIN_YOUR_HEAVY_INFANTRY,
                            priorityBuild = 0,
                            priorityTarget = 175,
                            availableHouse = (byte)(HouseFlag.FLAG_HOUSE_MERCENARY | HouseFlag.FLAG_HOUSE_SARDAUKAR | HouseFlag.FLAG_HOUSE_FREMEN | HouseFlag.FLAG_HOUSE_ORDOS | HouseFlag.FLAG_HOUSE_HARKONNEN),
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 20,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_2x2,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_WOR_TROOPER_FACILITY,
                        animationIndex = new byte[] {
                            21,
                            21,
                            21
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_TROOPER,
                            (byte)UnitType.UNIT_TROOPERS,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            6,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 8 */
		                o = new ObjectInfo{
                            stringID_abbrev = (ushort)Text.STR_CONST_YARD,
                            name = "Const Yard",
                            stringID_full = (ushort)Text.STR_CONSTRUCTION_YARD,
                            wsa = "construc.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = true,
                                notOnConcrete = true,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = true,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 64,
                            hitpoints = 400,
                            fogUncoverRadius = 3,
                            spriteID = 72,
                            buildCredits = 400,
                            buildTime = 80,
                            availableCampaign = 99,
                            structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NEVER,
                            sortPriority = 0,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_CONSTRUCTION_FACILITY_ALL_STRUCTURES_ARE_BUILT_BY_THE_CONSTRUCTION_FACILITY,
                            priorityBuild = 0,
                            priorityTarget = 300,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 0,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_2x2,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_CONSTRUCTION_YARD,
                        animationIndex = new byte[] {
                            22,
                            22,
                            22
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            4,
                            6,
                            0,
                        },
                    },

                    new StructureInfo { /* 9 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_WINDTRAP,
                            name = "Windtrap",
                            stringID_full = (ushort)Text.STR_WINDTRAP_POWER_CENTER,
                            wsa = "windtrap.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = false,
                                notOnConcrete = false,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = true,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 64,
                            hitpoints = 200,
                            fogUncoverRadius = 2,
                            spriteID = 73,
                            buildCredits = 300,
                            buildTime = 48,
                            availableCampaign = 1,
                            structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
                            sortPriority = 6,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_WINDTRAP_THE_WINDTRAP_SUPPLIES_POWER_TO_YOUR_BASE_WITHOUT_POWER_YOUR_STRUCTURES_WILL_DECAY,
                            priorityBuild = 0,
                            priorityTarget = 300,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = -100,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_2x2,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_WINDTRAP_POWER,
                        animationIndex = new byte[] {
                            26,
                            26,
                            26
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            0,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 10 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_BARRACKS,
                            name = "Barracks",
                            stringID_full = (ushort)Text.STR_INFANTRY_BARRACKS,
                            wsa = "barrac.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = true,
                                notOnConcrete = false,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = false,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 128,
                            hitpoints = 300,
                            fogUncoverRadius = 2,
                            spriteID = 74,
                            buildCredits = 300,
                            buildTime = 72,
                            availableCampaign = 2,
                            structuresRequired = (uint)(StructureFlag.FLAG_STRUCTURE_OUTPOST | StructureFlag.FLAG_STRUCTURE_WINDTRAP),
                            sortPriority = 18,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_BARRACKS_THE_BARRACKS_IS_USED_TO_TRAIN_YOUR_LIGHT_INFANTRY,
                            priorityBuild = 0,
                            priorityTarget = 100,
                            availableHouse = (byte)(HouseFlag.FLAG_HOUSE_MERCENARY | HouseFlag.FLAG_HOUSE_SARDAUKAR | HouseFlag.FLAG_HOUSE_FREMEN | HouseFlag.FLAG_HOUSE_ORDOS | HouseFlag.FLAG_HOUSE_ATREIDES),
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 10,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_2x2,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_INFANTRY_BARRACKS,
                        animationIndex = new byte[] {
                            28,
                            28,
                            28
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_SOLDIER,
                            (byte)UnitType.UNIT_INFANTRY,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            2,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 11 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_STARPORT,
                            name = "Starport",
                            stringID_full = (ushort)Text.STR_STARPORT_FACILITY,
                            wsa = "starport.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = true,
                                notOnConcrete = false,
                                busyStateIsIncoming = true,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = true,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 128,
                            hitpoints = 500,
                            fogUncoverRadius = 6,
                            spriteID = 75,
                            buildCredits = 500,
                            buildTime = 120,
                            availableCampaign = 6,
                            structuresRequired = (uint)(StructureFlag.FLAG_STRUCTURE_REFINERY | StructureFlag.FLAG_STRUCTURE_WINDTRAP),
                            sortPriority = 32,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_STARTPORT_THE_STARPORT_IS_USED_TO_ORDER_AND_RECEIVE_SHIPMENTS_FROM_CHOAM,
                            priorityBuild = 0,
                            priorityTarget = 250,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 50,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_3x3,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_STARPORT_FACILITY,
                        animationIndex = new byte[] {
                            5,
                            6,
                            7
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            0,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 12 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_REFINERY,
                            name = "Refinery",
                            stringID_full = (ushort)Text.STR_SPICE_REFINERY,
                            wsa = "refinery.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = false,
                                notOnConcrete = false,
                                busyStateIsIncoming = true,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = true,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 128,
                            hitpoints = 450,
                            fogUncoverRadius = 4,
                            spriteID = 76,
                            buildCredits = 400,
                            buildTime = 80,
                            availableCampaign = 1,
                            structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_WINDTRAP,
                            sortPriority = 8,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_SPICE_REFINERY_THE_REFINERY_CONVERTS_SPICE_INTO_CREDITS,
                            priorityBuild = 0,
                            priorityTarget = 300,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_HARVESTER,
                        creditsStorage = 1005,
                        powerUsage = 30,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_3x2,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_SPICE_REFINERY,
                        animationIndex = new byte[] {
                            17,
                            18,
                            19
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            0,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 13 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_REPAIR2,
                            name = "Repair",
                            stringID_full = (ushort)Text.STR_REPAIR_FACILITY,
                            wsa = "repair.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = false,
                                notOnConcrete = false,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = true,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 128,
                            hitpoints = 200,
                            fogUncoverRadius = 3,
                            spriteID = 77,
                            buildCredits = 700,
                            buildTime = 80,
                            availableCampaign = 5,
                            structuresRequired = (uint)(StructureFlag.FLAG_STRUCTURE_OUTPOST | StructureFlag.FLAG_STRUCTURE_WINDTRAP | StructureFlag.FLAG_STRUCTURE_LIGHT_VEHICLE),
                            sortPriority = 24,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_REPAIR_FACILITY_THE_REPAIR_FACILITY_IS_USED_TO_REPAIR_YOUR_VEHICLES,
                            priorityBuild = 0,
                            priorityTarget = 600,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)(UnitFlag.FLAG_UNIT_HARVESTER | UnitFlag.FLAG_UNIT_QUAD | UnitFlag.FLAG_UNIT_RAIDER_TRIKE | UnitFlag.FLAG_UNIT_TRIKE | UnitFlag.FLAG_UNIT_SONIC_TANK | UnitFlag.FLAG_UNIT_DEVASTATOR | UnitFlag.FLAG_UNIT_SIEGE_TANK | UnitFlag.FLAG_UNIT_TANK | UnitFlag.FLAG_UNIT_DEVIATOR | UnitFlag.FLAG_UNIT_LAUNCHER),
                        creditsStorage = 0,
                        powerUsage = 20,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_3x2,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_VEHICLE_REPAIR_CENTRE,
                        animationIndex = new byte[] {
                            23,
                            24,
                            25
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            0,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 14 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_WALL,
                            name = "Wall",
                            stringID_full = (ushort)Text.STR_BASE_DEFENSE_WALL,
                            wsa = "wall.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = false,
                                notOnConcrete = false,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = false,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 0,
                            hitpoints = 50,
                            fogUncoverRadius = 1,
                            spriteID = 78,
                            buildCredits = 50,
                            buildTime = 40,
                            availableCampaign = 4,
                            structuresRequired = (uint)(StructureFlag.FLAG_STRUCTURE_OUTPOST | StructureFlag.FLAG_STRUCTURE_WINDTRAP),
                            sortPriority = 16,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_WALL_THE_WALL_IS_USED_FOR_PASSIVE_DEFENSE,
                            priorityBuild = 0,
                            priorityTarget = 30,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 0,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_1x1,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_WALLS,
                        animationIndex = new byte[] {
                            0xFF,
                            0xFF,
                            0xFF
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            0,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 15 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_TURRET,
                            name = "Turret",
                            stringID_full = (ushort)Text.STR_CANNON_TURRET,
                            wsa = "turret.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = false,
                                notOnConcrete = false,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = true,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 64,
                            hitpoints = 200,
                            fogUncoverRadius = 2,
                            spriteID = 79,
                            buildCredits = 125,
                            buildTime = 64,
                            availableCampaign = 5,
                            structuresRequired = (uint)(StructureFlag.FLAG_STRUCTURE_OUTPOST | StructureFlag.FLAG_STRUCTURE_WINDTRAP),
                            sortPriority = 22,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_GUN_TURRET_THE_CANNON_TURRET_IS_USED_FOR_SHORT_RANGE_ACTIVE_DEFENSE,
                            priorityBuild = 75,
                            priorityTarget = 150,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 10,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_1x1,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_BASE_DEFENSE_TURRET,
                        animationIndex = new byte[] {
                            0xFF,
                            0xFF,
                            0xFF
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            0,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 16 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_RTURRET,
                            name = "R-Turret",
                            stringID_full = (ushort)Text.STR_ROCKET_TURRET,
                            wsa = "rturret.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = false,
                                notOnConcrete = false,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = true,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 64,
                            hitpoints = 200,
                            fogUncoverRadius = 5,
                            spriteID = 80,
                            buildCredits = 250,
                            buildTime = 96,
                            availableCampaign = 0,
                            structuresRequired = (uint)(StructureFlag.FLAG_STRUCTURE_OUTPOST | StructureFlag.FLAG_STRUCTURE_WINDTRAP),
                            sortPriority = 26,
                            upgradeLevelRequired = 2,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_ROCKET_TURRET_THE_ROCKETCANNON_TURRET_IS_USED_FOR_BOTH_SHORT_AND_MEDIUM_RANGE_ACTIVE_DEFENSE,
                            priorityBuild = 100,
                            priorityTarget = 75,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 25,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_1x1,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_BASE_ROCKET_TURRET,
                        animationIndex = new byte[] {
                            0xFF,
                            0xFF,
                            0xFF
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            0,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 17 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_SPICE_SILO,
                            name = "Spice Silo",
                            stringID_full = (ushort)Text.STR_SPICE_STORAGE_SILO,
                            wsa = "storage.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = false,
                                notOnConcrete = false,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = true,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 0,
                            hitpoints = 150,
                            fogUncoverRadius = 2,
                            spriteID = 81,
                            buildCredits = 150,
                            buildTime = 48,
                            availableCampaign = 2,
                            structuresRequired = (uint)(StructureFlag.FLAG_STRUCTURE_REFINERY | StructureFlag.FLAG_STRUCTURE_WINDTRAP),
                            sortPriority = 12,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_SPICE_SILO_THE_SPICE_SILO_IS_USED_TO_STORE_REFINED_SPICE,
                            priorityBuild = 0,
                            priorityTarget = 150,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 1000,
                        powerUsage = 5,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_2x2,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_SPICE_STORAGE_SILO,
                        animationIndex = new byte[] {
                            27,
                            27,
                            27
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            0,
                            0,
                            0,
                        },
                    },

                    new StructureInfo { /* 18 */
		                o = new ObjectInfo {
                            stringID_abbrev = (ushort)Text.STR_OUTPOST,
                            name = "Outpost",
                            stringID_full = (ushort)Text.STR_RADAR_OUTPOST,
                            wsa = "headqrts.wsa",
                            flags = new ObjectInfoFlags {
                                hasShadow = false,
                                factory = false,
                                notOnConcrete = false,
                                busyStateIsIncoming = false,
                                blurTile = false,
                                hasTurret = false,
                                conquerable = false,
                                canBePickedUp = false,
                                noMessageOnDeath = false,
                                tabSelectable = false,
                                scriptNoSlowdown = false,
                                targetAir = false,
                                priority = false
                            },
                            spawnChance = 128,
                            hitpoints = 500,
                            fogUncoverRadius = 10,
                            spriteID = 82,
                            buildCredits = 400,
                            buildTime = 80,
                            availableCampaign = 2,
                            structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_WINDTRAP,
                            sortPriority = 10,
                            upgradeLevelRequired = 0,
                            actionsPlayer = new ushort[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
                            available = 0,
                            hintStringID = (ushort)Text.STR_OUTPOST_THE_OUTPOST_PROVIDES_RADAR_AND_AIDS_CONTROL_OF_DISTANT_VEHICLES,
                            priorityBuild = 0,
                            priorityTarget = 275,
                            availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
                        },
                        enterFilter = (uint)UnitFlag.FLAG_UNIT_NONE,
                        creditsStorage = 0,
                        powerUsage = 30,
                        layout = (ushort)StructureLayout.STRUCTURE_LAYOUT_2x2,
                        iconGroup = (ushort)IconMapEntries.ICM_ICONGROUP_RADAR_OUTPOST,
                        animationIndex = new byte[] {
                            3,
                            3,
                            3
                        },
                        buildableUnits = new byte[] {
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                            (byte)UnitType.UNIT_INVALID,
                        },
                        upgradeCampaign = new ushort[] {
                            0,
                            0,
                            0,
                        },
                    }
                };
            }
        }

        /* Array with number of tiles in a layout. */
        internal static ushort[] g_table_structure_layoutTileCount = { //[STRUCTURE_LAYOUT_MAX]
            1, /* STRUCTURE_LAYOUT_1x1 */
	        2, /* STRUCTURE_LAYOUT_2x1 */
	        2, /* STRUCTURE_LAYOUT_1x2 */
	        4, /* STRUCTURE_LAYOUT_2x2 */
	        6, /* STRUCTURE_LAYOUT_2x3 */
	        6, /* STRUCTURE_LAYOUT_3x2 */
	        9, /* STRUCTURE_LAYOUT_3x3 */
        };

        /* Array with TileDiff of a layout. */
        internal static tile32[] g_table_structure_layoutTileDiff = { //[STRUCTURE_LAYOUT_MAX]
            new tile32 { x = 0x0080, y = 0x0080 }, /* STRUCTURE_LAYOUT_1x1 */
	        new tile32 { x = 0x0100, y = 0x0080 }, /* STRUCTURE_LAYOUT_2x1 */
	        new tile32 { x = 0x0080, y = 0x0100 }, /* STRUCTURE_LAYOUT_1x2 */
	        new tile32 { x = 0x0100, y = 0x0100 }, /* STRUCTURE_LAYOUT_2x2 */
	        new tile32 { x = 0x0100, y = 0x0180 }, /* STRUCTURE_LAYOUT_2x3 */
	        new tile32 { x = 0x0280, y = 0x0100 }, /* STRUCTURE_LAYOUT_3x2 */
	        new tile32 { x = 0x0180, y = 0x0180 }, /* STRUCTURE_LAYOUT_3x3 */
        };

        /* Array with size of a layout. */
        internal static XYSize[] g_table_structure_layoutSize = { //[STRUCTURE_LAYOUT_MAX]
            new XYSize { width = 1, height = 1 }, /* STRUCTURE_LAYOUT_1x1 */
	        new XYSize { width = 2, height = 1 }, /* STRUCTURE_LAYOUT_2x1 */
	        new XYSize { width = 1, height = 2 }, /* STRUCTURE_LAYOUT_1x2 */
	        new XYSize { width = 2, height = 2 }, /* STRUCTURE_LAYOUT_2x2 */
	        new XYSize { width = 2, height = 3 }, /* STRUCTURE_LAYOUT_2x3 */
	        new XYSize { width = 3, height = 2 }, /* STRUCTURE_LAYOUT_3x2 */
	        new XYSize { width = 3, height = 3 }, /* STRUCTURE_LAYOUT_3x3 */
        };

        /* Array with position offset per tile in a structure layout. */
        internal static ushort[][] g_table_structure_layoutTiles = { //[STRUCTURE_LAYOUT_MAX][9]
	        new ushort[] {0,    0,    0,    0,     0,     0,     0,     0,     0}, /* STRUCTURE_LAYOUT_1x1 */
	        new ushort[] {0,    1,    0,    0,     0,     0,     0,     0,     0}, /* STRUCTURE_LAYOUT_2x1 */
	        new ushort[] {0, 64+0,    0,    0,     0,     0,     0,     0,     0}, /* STRUCTURE_LAYOUT_1x2 */
	        new ushort[] {0,    1, 64+0, 64+1,     0,     0,     0,     0,     0}, /* STRUCTURE_LAYOUT_2x2 */
	        new ushort[] {0,    1, 64+0, 64+1, 128+0, 128+1,     0,     0,     0}, /* STRUCTURE_LAYOUT_2x3 */
	        new ushort[] {0,    1,    2, 64+0,  64+1,  64+2,     0,     0,     0}, /* STRUCTURE_LAYOUT_3x2 */
	        new ushort[] {0,    1,    2, 64+0,  64+1,  64+2, 128+0, 128+1, 128+2}  /* STRUCTURE_LAYOUT_3x3 */
        };

        /* Array with position offset per tile around a structure layout. */
        static short[][] g_table_structure_layoutTilesAround = { //[STRUCTURE_LAYOUT_MAX][16]
	        new short[] {-64, -64+1,     1,  64+1,  64+0,  64-1,    -1, -64-1,     0,     0,     0,     0,     0,     0,  0,     0}, /* STRUCTURE_LAYOUT_1x1 */
	        new short[] {-64, -64+1, -64+2,     2,  64+2,  64+1,  64+0,  64-1,    -1, -64-1,     0,     0,     0,     0,  0,     0}, /* STRUCTURE_LAYOUT_2x1 */
	        new short[] {-64, -64+1,     1,  64+1, 128+1, 128+0, 128-1,  64-1,    -1, -64-1,     0,     0,     0,     0,  0,     0}, /* STRUCTURE_LAYOUT_1x2 */
	        new short[] {-64, -64+1, -64+2,     2,  64+2, 128+2, 128+1, 128+0, 128-1,  64-1,    -1, -64-1,     0,     0,  0,     0}, /* STRUCTURE_LAYOUT_2x2 */
	        new short[] {-64, -64+1, -64+2,     2,  64+2, 128+2, 192+2, 192+1, 192+0, 192-1, 128-1,  64-1,    -1, -64-1,  0,     0}, /* STRUCTURE_LAYOUT_2x3 */
	        new short[] {-64, -64+1, -64+2, -64+3,     3,  64+3, 128+3, 128+2, 128+1, 128+0, 128-1,  64-1,    -1, -64-1,  0,     0}, /* STRUCTURE_LAYOUT_3x2 */
	        new short[] {-64, -64+1, -64+2, -64+3,     3,  64+3, 128+3, 192+3, 192+2, 192+1, 192+0, 192-1, 128-1,  64-1, -1, -64-1}, /* STRUCTURE_LAYOUT_3x3 */
        };

        /* Array with position offset of edge tiles in a structure layout. */
        internal static ushort[][] g_table_structure_layoutEdgeTiles = { //[STRUCTURE_LAYOUT_MAX][8]
	        new ushort[] {0, 0,    0,     0,     0,     0,     0, 0}, /* STRUCTURE_LAYOUT_1x1 */
	        new ushort[] {0, 1,    1,     1,     1,     0,     0, 0}, /* STRUCTURE_LAYOUT_2x1 */
	        new ushort[] {0, 0,    0,  64+0,  64+0,  64+0,     0, 0}, /* STRUCTURE_LAYOUT_1x2 */
	        new ushort[] {0, 1,    1,  64+1,  64+1,  64+0,  64+0, 0}, /* STRUCTURE_LAYOUT_2x2 */
	        new ushort[] {0, 1, 64+1, 128+1, 128+1, 128+0,  64+0, 0}, /* STRUCTURE_LAYOUT_2x3 */
	        new ushort[] {1, 2,    2,  64+2,  64+1,  64+0,     0, 0}, /* STRUCTURE_LAYOUT_3x2 */
	        new ushort[] {1, 2, 64+2, 128+2, 128+1, 128+0,  64+0, 0}, /* STRUCTURE_LAYOUT_3x3 */
        };

        internal static ushort g_structureActivePosition;
        internal static ushort g_structureActiveType;

        internal static Structure g_structureActive = null;

        static bool s_debugInstantBuild = false; /*!< When non-zero, constructions are almost instant. */
        static uint s_tickStructureDegrade = 0; /*!< Indicates next time Degrade function is executed. */
        static uint s_tickStructureStructure = 0; /*!< Indicates next time Structures function is executed. */
        static uint s_tickStructureScript = 0; /*!< Indicates next time Script function is executed. */
        static uint s_tickStructurePalace = 0; /*!< Indicates next time Palace function is executed. */

        internal static ushort g_structureIndex;

        /*
        * Get a Structure from the pool with the indicated index.
        * @param index The index of the Structure to get.
        * @return The Structure.
        */
        internal static Structure Structure_Get_ByIndex(ushort index)
        {
            Debug.Assert(index < (ushort)StructureIndex.STRUCTURE_INDEX_MAX_HARD);
            return g_structureArray[index];
        }

        internal static void Structure_Set_ByIndex(Structure s)
        {
            Debug.Assert(s.o.index < (ushort)StructureIndex.STRUCTURE_INDEX_MAX_HARD);
            g_structureArray[s.o.index] = s;
        }

        /*
        * Find the first matching Structure based on the PoolFindStruct filter data.
        *
        * @param find A pointer to a PoolFindStruct which contains filter data and
        *   last known tried index. Calling this functions multiple times with the
        *   same 'find' parameter walks over all possible values matching the filter.
        * @return The Structure, or NULL if nothing matches (anymore).
        */
        internal static Structure Structure_Find(PoolFindStruct find)
        {
            if (find.index >= g_structureFindCount + 3 && find.index != 0xFFFF) return null;
            find.index++; /* First, we always go to the next index */

            Debug.Assert(g_structureFindCount <= (ushort)StructureIndex.STRUCTURE_INDEX_MAX_SOFT);
            for (; find.index < g_structureFindCount + 3; find.index++)
            {
                Structure s = null;

                if (find.index < g_structureFindCount)
                {
                    s = g_structureFindArray[find.index];
                }
                else
                {
                    /* There are 3 special structures that are never in the Find array */
                    Debug.Assert(find.index - g_structureFindCount < 3);
                    switch (find.index - g_structureFindCount)
                    {
                        case 0:
                            s = Structure_Get_ByIndex((ushort)StructureIndex.STRUCTURE_INDEX_WALL);
                            if (s.o.index != (ushort)StructureIndex.STRUCTURE_INDEX_WALL) continue;
                            break;

                        case 1:
                            s = Structure_Get_ByIndex((ushort)StructureIndex.STRUCTURE_INDEX_SLAB_2x2);
                            if (s.o.index != (ushort)StructureIndex.STRUCTURE_INDEX_SLAB_2x2) continue;
                            break;

                        case 2:
                            s = Structure_Get_ByIndex((ushort)StructureIndex.STRUCTURE_INDEX_SLAB_1x1);
                            if (s.o.index != (ushort)StructureIndex.STRUCTURE_INDEX_SLAB_1x1) continue;
                            break;
                    }
                }
                if (s == null) continue;

                if (s.o.flags.isNotOnMap && CSharpDune.g_validateStrictIfZero == 0) continue;
                if (find.houseID != (byte)HouseType.HOUSE_INVALID && find.houseID != s.o.houseID) continue;
                if (find.type != (ushort)StructureIndex.STRUCTURE_INDEX_INVALID && find.type != s.o.type) continue;

                return s;
            }

            return null;
        }

        /*
         * Checks if the given position is a valid location for the given structure type.
         *
         * @param position The (packed) tile to check.
         * @param type The structure type to check the position for.
         * @return 0 if the position is not valid, 1 if the position is valid and have enough slabs, <0 if the position is valid but miss some slabs.
         */
        internal static short Structure_IsValidBuildLocation(ushort position, StructureType type)
        {
            StructureInfo si;
            ushort[] layoutTile;
            byte i;
            ushort neededSlabs;
            bool isValid;
            ushort curPos;

            si = g_table_structureInfo[(int)type];
            layoutTile = g_table_structure_layoutTiles[si.layout];

            isValid = true;
            neededSlabs = 0;
            for (i = 0; i < g_table_structure_layoutTileCount[si.layout]; i++)
            {
                ushort lst;

                curPos = (ushort)(position + layoutTile[i]);

                lst = Map.Map_GetLandscapeType(curPos);

                if (CSharpDune.g_debugScenario)
                {
                    if (!Map.g_table_landscapeInfo[lst].isValidForStructure2)
                    {
                        isValid = false;
                        break;
                    }
                }
                else
                {
                    if (!Map.Map_IsValidPosition(curPos))
                    {
                        isValid = false;
                        break;
                    }

                    if (si.o.flags.notOnConcrete)
                    {
                        if (!Map.g_table_landscapeInfo[lst].isValidForStructure2 && CSharpDune.g_validateStrictIfZero == 0)
                        {
                            isValid = false;
                            break;
                        }
                    }
                    else
                    {
                        if (!Map.g_table_landscapeInfo[lst].isValidForStructure && CSharpDune.g_validateStrictIfZero == 0)
                        {
                            isValid = false;
                            break;
                        }
                        if (lst != (ushort)LandscapeType.LST_CONCRETE_SLAB) neededSlabs++;
                    }
                }

                if (CObject.Object_GetByPackedTile(curPos) != null)
                {
                    isValid = false;
                    break;
                }
            }

            if (CSharpDune.g_validateStrictIfZero == 0 && isValid && type != StructureType.STRUCTURE_CONSTRUCTION_YARD && !CSharpDune.g_debugScenario)
            {
                isValid = false;
                for (i = 0; i < 16; i++)
                {
                    ushort offset, lst;
                    Structure s;

                    offset = (ushort)g_table_structure_layoutTilesAround[si.layout][i];
                    if (offset == 0) break;

                    curPos = (ushort)(position + offset);
                    s = Structure_Get_ByPackedTile(curPos);
                    if (s != null)
                    {
                        if (s.o.houseID != (byte)CHouse.g_playerHouseID) continue;
                        isValid = true;
                        break;
                    }

                    lst = Map.Map_GetLandscapeType(curPos);
                    if (lst != (ushort)LandscapeType.LST_CONCRETE_SLAB && lst != (ushort)LandscapeType.LST_WALL) continue;
                    if (Map.g_map[curPos].houseID != (byte)CHouse.g_playerHouseID) continue;

                    isValid = true;
                    break;
                }
            }

            if (!isValid) return 0;
            if (neededSlabs == 0) return 1;
            return (short)-neededSlabs;
        }

        /*
         * Get the structure on the given packed tile.
         *
         * @param packed The packed tile to get the structure from.
         * @return The structure.
         */
        internal static Structure Structure_Get_ByPackedTile(ushort packed)
        {
            Tile tile;

            if (CTile.Tile_IsOutOfMap(packed)) return null;

            tile = Map.g_map[packed];
            if (!tile.hasStructure) return null;
            return Structure_Get_ByIndex((ushort)(tile.index - 1));
        }

        /*
         * Update the map with the right data for this structure.
         * @param s The structure to update on the map.
         */
        internal static void Structure_UpdateMap(Structure s)
        {
            StructureInfo si;
            ushort layoutSize;
            ushort[] layout;
            ushort[] iconMap;
            int i;

            if (s == null) return;
            if (!s.o.flags.used) return;
            if (s.o.flags.isNotOnMap) return;

            si = g_table_structureInfo[s.o.type];

            layout = g_table_structure_layoutTiles[si.layout];
            layoutSize = g_table_structure_layoutTileCount[si.layout];

            iconMap = Sprites.g_iconMap[(Sprites.g_iconMap[si.iconGroup] + layoutSize + layoutSize)..];

            for (i = 0; i < layoutSize; i++)
            {
                ushort position;
                Tile t;

                position = (ushort)(CTile.Tile_PackTile(s.o.position) + layout[i]);

                t = Map.g_map[position];
                t.houseID = s.o.houseID;
                t.hasStructure = true;
                t.index = (ushort)(s.o.index + 1);

                t.groundTileID = (ushort)(iconMap[i] + s.rotationSpriteDiff);

                if (Sprites.Tile_IsUnveiled(t.overlayTileID)) t.overlayTileID = 0;

                Map.Map_Update(position, 0, false);
            }

            s.o.flags.isDirty = true;

            if (s.state >= (short)StructureState.STRUCTURE_STATE_IDLE)
            {
                ushort animationIndex = (ushort)((s.state > (short)StructureState.STRUCTURE_STATE_READY) ? (short)StructureState.STRUCTURE_STATE_READY : s.state);

                if (si.animationIndex[animationIndex] == 0xFF)
                {
                    CAnimation.Animation_Start(null, s.o.position, si.layout, s.o.houseID, (byte)si.iconGroup);
                }
                else
                {
                    byte animationID = si.animationIndex[animationIndex];

                    Debug.Assert(animationID < 29);
                    CAnimation.Animation_Start(CAnimation.g_table_animation_structure[animationID], s.o.position, si.layout, s.o.houseID, (byte)si.iconGroup);
                }
            }
            else
            {
                CAnimation.Animation_Start(CAnimation.g_table_animation_structure[1], s.o.position, si.layout, s.o.houseID, (byte)si.iconGroup);
            }
        }

        /*
         * Check wether the given structure is upgradable.
         *
         * @param s The Structure to check.
         * @return True if and only if the structure is upgradable.
         */
        internal static bool Structure_IsUpgradable(Structure s)
        {
            StructureInfo si;

            if (s == null) return false;

            si = g_table_structureInfo[s.o.type];

            if (s.o.houseID == (byte)HouseType.HOUSE_HARKONNEN && s.o.type == (byte)StructureType.STRUCTURE_HIGH_TECH) return false;
            if (s.o.houseID == (byte)HouseType.HOUSE_ORDOS && s.o.type == (byte)StructureType.STRUCTURE_HEAVY_VEHICLE && s.upgradeLevel == 1 && si.upgradeCampaign[2] > CSharpDune.g_campaignID) return false;

            if (si.upgradeCampaign[s.upgradeLevel] != 0 && si.upgradeCampaign[s.upgradeLevel] <= CSharpDune.g_campaignID + 1)
            {
                House h;

                if (s.o.type != (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD) return true;
                if (s.upgradeLevel != 1) return true;

                h = CHouse.House_Get_ByIndex(s.o.houseID);
                if ((h.structuresBuilt & g_table_structureInfo[(int)StructureType.STRUCTURE_ROCKET_TURRET].o.structuresRequired) == g_table_structureInfo[(int)StructureType.STRUCTURE_ROCKET_TURRET].o.structuresRequired) return true;

                return false;
            }

            if (s.o.houseID == (byte)HouseType.HOUSE_HARKONNEN && s.o.type == (byte)StructureType.STRUCTURE_WOR_TROOPER && s.upgradeLevel == 0 && CSharpDune.g_campaignID > 3) return true;
            return false;
        }

        /*
         * Get the unit linked to this structure, or NULL if there is no.
         * @param s The structure to get the linked unit from.
         * @return The linked unit, or NULL if there was none.
         */
        internal static Unit Structure_GetLinkedUnit(Structure s)
        {
            if (s.o.linkedID == 0xFF) return null;
            return CUnit.Unit_Get_ByIndex(s.o.linkedID);
        }

        /*
         * Set the state for the given structure.
         *
         * @param s The structure to set the state of.
         * @param state The new sate value.
         */
        internal static void Structure_SetState(Structure s, short state)
        {
            if (s == null) return;
            s.state = state;

            Structure_UpdateMap(s);
        }

        /*
         * The house is under attack in the form of a structure being hit.
         * @param houseID The house who is being attacked.
         */
        internal static void Structure_HouseUnderAttack(byte houseID)
        {
            PoolFindStruct find = new PoolFindStruct();
            House h;

            h = CHouse.House_Get_ByIndex(houseID);

            if (houseID != (byte)CHouse.g_playerHouseID && h.flags.doneFullScaleAttack) return;
            h.flags.doneFullScaleAttack = true;

            if (h.flags.human)
            {
                if (h.timerStructureAttack != 0) return;

                Sound.Sound_Output_Feedback(48);

                h.timerStructureAttack = 8;
                return;
            }

            /* ENHANCEMENT -- Dune2 originally only searches for units with type 0 (Carry-all). In result, the rest of this function does nothing. */
            if (!CSharpDune.g_dune2_enhanced) return;

            find.houseID = houseID;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            while (true)
            {
                UnitInfo ui;
                Unit u;

                u = CUnit.Unit_Find(find);
                if (u == null) break;

                ui = CUnit.g_table_unitInfo[u.o.type];

                if (ui.bulletType == (byte)UnitType.UNIT_INVALID) continue;

                /* XXX -- Dune2 does something odd here. What was their intention? */
                if ((u.actionID == (byte)ActionType.ACTION_GUARD && u.actionID == (byte)ActionType.ACTION_AMBUSH) || u.actionID == (byte)ActionType.ACTION_AREA_GUARD) CUnit.Unit_SetAction(u, ActionType.ACTION_HUNT);
            }
        }

        /*
         * Damage the structure, and bring the surrounding to an explosion if needed.
         *
         * @param s The structure to damage.
         * @param damage The damage to deal to the structure.
         * @param range The range in which an explosion should be possible.
         * @return True if and only if the structure is now destroyed.
         */
        internal static bool Structure_Damage(Structure s, ushort damage, ushort range)
        {
            StructureInfo si;

            if (s == null) return false;
            if (damage == 0) return false;
            if (s.o.script.variables[0] == 1) return false;

            si = g_table_structureInfo[s.o.type];

            if (s.o.hitpoints >= damage)
            {
                s.o.hitpoints -= damage;
            }
            else
            {
                s.o.hitpoints = 0;
            }

            if (s.o.hitpoints == 0)
            {
                ushort score;

                score = (ushort)(si.o.buildCredits / 100);
                if (score < 1) score = 1;

                if (CHouse.House_AreAllied((byte)CHouse.g_playerHouseID, s.o.houseID))
                {
                    CScenario.g_scenario.destroyedAllied++;
                    CScenario.g_scenario.score -= score;
                }
                else
                {
                    CScenario.g_scenario.destroyedEnemy++;
                    CScenario.g_scenario.score += score;
                }

                Structure_Destroy(s);

                if ((byte)CHouse.g_playerHouseID == s.o.houseID)
                {
                    ushort index;

                    switch ((HouseType)s.o.houseID)
                    {
                        case HouseType.HOUSE_HARKONNEN: index = 22; break;
                        case HouseType.HOUSE_ATREIDES: index = 23; break;
                        case HouseType.HOUSE_ORDOS: index = 24; break;
                        default: index = 0xFFFF; break;
                    }

                    Sound.Sound_Output_Feedback(index);
                }
                else
                {
                    Sound.Sound_Output_Feedback(21);
                }

                Structure_UntargetMe(s);
                return true;
            }

            if (range == 0) return false;

            Map.Map_MakeExplosion((ushort)ExplosionType.EXPLOSION_IMPACT_LARGE, CTile.Tile_AddTileDiff(s.o.position, g_table_structure_layoutTileDiff[si.layout]), 0, 0);
            return false;
        }

        /*
         * Untarget the given Structure.
         *
         * @param unit The Structure to untarget.
         */
        internal static void Structure_UntargetMe(Structure s)
        {
            PoolFindStruct find = new PoolFindStruct();
            ushort encoded = Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);

            CObject.Object_Script_Variable4_Clear(s.o);

            find.houseID = (byte)HouseType.HOUSE_INVALID;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            while (true)
            {
                Unit u;

                u = CUnit.Unit_Find(find);
                if (u == null) break;

                if (u.targetMove == encoded) u.targetMove = 0;
                if (u.targetAttack == encoded) u.targetAttack = 0;
                if (u.o.script.variables[4] == encoded) CObject.Object_Script_Variable4_Clear(u.o);
            }

            find.houseID = (byte)HouseType.HOUSE_INVALID;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            while (true)
            {
                Team t;

                t = CTeam.Team_Find(find);
                if (t == null) break;

                if (t.target == encoded) t.target = 0;
            }
        }

        /*
         * Handles destroying of a structure.
         *
         * @param s The Structure.
         */
        static void Structure_Destroy(Structure s)
        {
            StructureInfo si;
            byte linkedID;
            House h;

            if (s == null) return;

            if (CSharpDune.g_debugScenario)
            {
                Structure_Remove(s);
                return;
            }

            s.o.script.variables[0] = 1;
            s.o.flags.allocated = false;
            s.o.flags.repairing = false;
            s.o.script.delay = 0;

            Script.Script_Reset(s.o.script, Script.g_scriptStructure);
            Script.Script_Load(s.o.script, s.o.type);

            Sound.Voice_PlayAtTile(44, s.o.position);

            linkedID = s.o.linkedID;

            if (linkedID != 0xFF)
            {
                if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
                {
                    Structure_Destroy(Structure_Get_ByIndex(linkedID));
                    s.o.linkedID = 0xFF;
                }
                else
                {
                    while (linkedID != 0xFF)
                    {
                        Unit u = CUnit.Unit_Get_ByIndex(linkedID);

                        linkedID = u.o.linkedID;

                        CUnit.Unit_Remove(u);
                    }
                }
            }

            h = CHouse.House_Get_ByIndex(s.o.houseID);
            si = g_table_structureInfo[s.o.type];

            h.credits -= (ushort)((h.creditsStorage == 0) ? h.credits : Min(h.credits, (h.credits * 256 / h.creditsStorage) * si.creditsStorage / 256));

            if (s.o.houseID != (byte)CHouse.g_playerHouseID) h.credits += (ushort)(si.o.buildCredits + (CSharpDune.g_campaignID > 7 ? si.o.buildCredits / 2 : 0));

            if (s.o.type != (byte)StructureType.STRUCTURE_WINDTRAP) return;

            h.windtrapCount--;
        }

        /*
         * Remove the structure from the map, free it, and clean up after it.
         * @param s The structure to remove.
         */
        static void Structure_Remove(Structure s)
        {
            StructureInfo si;
            ushort packed;
            ushort i;
            House h;

            if (s == null) return;

            si = g_table_structureInfo[s.o.type];
            packed = CTile.Tile_PackTile(s.o.position);

            for (i = 0; i < g_table_structure_layoutTileCount[si.layout]; i++)
            {
                Tile t;
                ushort curPacked = (ushort)(packed + g_table_structure_layoutTiles[si.layout][i]);

                CAnimation.Animation_Stop_ByTile(curPacked);

                t = Map.g_map[curPacked];
                t.hasStructure = false;

                if (CSharpDune.g_debugScenario)
                {
                    t.groundTileID = (ushort)(Map.g_mapTileID[curPacked] & 0x1FF);
                    t.overlayTileID = 0;
                }
            }

            if (!CSharpDune.g_debugScenario)
            {
                CAnimation.Animation_Start(CAnimation.g_table_animation_structure[0], s.o.position, si.layout, s.o.houseID, (byte)si.iconGroup);
            }

            h = CHouse.House_Get_ByIndex(s.o.houseID);

            for (i = 0; i < 5; i++)
            {
                if (h.ai_structureRebuild[i][0] != 0) continue;
                h.ai_structureRebuild[i][0] = s.o.type;
                h.ai_structureRebuild[i][1] = packed;
                break;
            }

            Structure_Free(s);
            Structure_UntargetMe(s);

            h.structuresBuilt = Structure_GetStructuresBuilt(h);

            CHouse.House_UpdateCreditsStorage(s.o.houseID);

            if (CSharpDune.g_debugScenario) return;

            switch ((StructureType)s.o.type)
            {
                case StructureType.STRUCTURE_WINDTRAP:
                    CHouse.House_CalculatePowerAndCredit(h);
                    break;

                case StructureType.STRUCTURE_OUTPOST:
                    CHouse.House_UpdateRadarState(h);
                    break;

                default: break;
            }
        }

        /*
         * Free a Structure.
         *
         * @param address The address of the Structure to free.
         */
        internal static void Structure_Free(Structure s)
        {
            int i;

            s.o.flags = new ObjectFlags(); //memset(&s->o.flags, 0, sizeof(s->o.flags));

            Script.Script_Reset(s.o.script, Script.g_scriptStructure);

            if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) return;

            /* Walk the array to find the Structure we are removing */
            Debug.Assert(g_structureFindCount <= (ushort)StructureIndex.STRUCTURE_INDEX_MAX_SOFT);
            for (i = 0; i < g_structureFindCount; i++)
            {
                if (g_structureFindArray[i] == s) break;
            }
            Debug.Assert(i < g_structureFindCount); /* We should always find an entry */

            g_structureFindCount--;

            /* If needed, close the gap */
            if (i == g_structureFindCount) return;
            Array.Copy(g_structureFindArray, i + 1, g_structureFindArray, i, g_structureFindCount - i); //memmove(&g_structureFindArray[i], &g_structureFindArray[i + 1], (g_structureFindCount - i) * sizeof(g_structureFindArray[0]));
        }

        /*
         * Get a bitmask of all built structure types for the given House.
         *
         * @param h The house to get built structures for.
         * @return The bitmask.
         */
        internal static uint Structure_GetStructuresBuilt(House h)
        {
            PoolFindStruct find = new PoolFindStruct();
            uint result;

            if (h == null) return 0;

            result = 0;
            find.houseID = h.index;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            /* Recount windtraps after capture or loading old saved games. */
            h.windtrapCount = 0;

            while (true)
            {
                Structure s;

                s = Structure_Find(find);
                if (s == null) break;
                if (s.o.flags.isNotOnMap) continue;
                if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;
                result |= (ushort)(1 << s.o.type);

                if (s.o.type == (byte)StructureType.STRUCTURE_WINDTRAP) h.windtrapCount++;
            }

            return result;
        }

        static byte[] wall = {
             0,  3,  1,  2,  3,  3,  4,  5,  1,  6,  1,  7,  8,  9, 10, 11,
             1, 12,  1, 19,  1, 16,  1, 31,  1, 28,  1, 52,  1, 45,  1, 59,
             3,  3, 13, 20,  3,  3, 22, 32,  3,  3, 13, 53,  3,  3, 38, 60,
             5,  6,  7, 21,  5,  6,  7, 33,  5,  6,  7, 54,  5,  6,  7, 61,
             9,  9,  9,  9, 17, 17, 23, 34,  9,  9,  9,  9, 25, 46, 39, 62,
            11, 12, 11, 12, 13, 18, 13, 35, 11, 12, 11, 12, 13, 47, 13, 63,
            15, 15, 16, 16, 17, 17, 24, 36, 15, 15, 16, 16, 17, 17, 40, 64,
            19, 20, 21, 22, 23, 24, 25, 37, 19, 20, 21, 22, 23, 24, 25, 65,
            27, 27, 27, 27, 27, 27, 27, 27, 14, 29, 14, 55, 26, 48, 41, 66,
            29, 30, 29, 30, 29, 30, 29, 30, 31, 30, 31, 56, 31, 49, 31, 67,
            33, 33, 34, 34, 33, 33, 34, 34, 35, 35, 15, 57, 35, 35, 42, 68,
            37, 38, 39, 40, 37, 38, 39, 40, 41, 42, 43, 58, 41, 42, 43, 69,
            45, 45, 45, 45, 46, 46, 46, 46, 47, 47, 47, 47, 27, 50, 43, 70,
            49, 50, 49, 50, 51, 52, 51, 52, 53, 54, 53, 54, 55, 51, 55, 71,
            57, 57, 58, 58, 59, 59, 60, 60, 61, 61, 62, 62, 63, 63, 44, 72,
            65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 73
        };
        /*
         * Connect walls around the given position.
         *
         * @param position The packed position.
         * @param recurse Wether to recurse.
         * @return True if and only if a change happened.
         */
        internal static bool Structure_ConnectWall(ushort position, bool recurse)
        {
            ushort bits = 0;
            ushort tileID;
            bool isDestroyedWall;
            byte i;
            Tile tile;

            isDestroyedWall = Map.Map_GetLandscapeType(position) == (ushort)LandscapeType.LST_DESTROYED_WALL;

            for (i = 0; i < 4; i++)
            {
                ushort curPos = (ushort)(position + Map.g_table_mapDiff[i]);

                if (recurse && Map.Map_GetLandscapeType(curPos) == (ushort)LandscapeType.LST_WALL) Structure_ConnectWall(curPos, false);

                if (isDestroyedWall) continue;

                LandscapeType landscapeType = (LandscapeType)Map.Map_GetLandscapeType(curPos);
                if (landscapeType == LandscapeType.LST_DESTROYED_WALL)
                    bits |= (ushort)(1 << (i + 4));
                if (landscapeType == LandscapeType.LST_WALL)
                    bits |= (ushort)(1 << i);

                //switch (Map_GetLandscapeType(curPos)) {
                //    case LST_DESTROYED_WALL: bits |= (1 << (i + 4));
                //        /* FALL-THROUGH */
                //    case LST_WALL: bits |= (1 << i);
                //        /* FALL-THROUGH */
                //    default:  break;
                //}
            }

            if (isDestroyedWall) return false;

            tileID = (ushort)(Sprites.g_wallTileID + wall[bits] + 1);

            tile = Map.g_map[position];
            if (tile.groundTileID == tileID) return false;

            tile.groundTileID = tileID;
            Map.g_mapTileID[position] |= 0x8000;
            Map.Map_Update(position, 0, false);

            return true;
        }

        /*
         * Convert the name of a structure to the type value of that structure, or
         *  STRUCTURE_INVALID if not found.
         */
        internal static byte Structure_StringToType(string name)
        {
            byte type;
            if (name == null) return (byte)StructureType.STRUCTURE_INVALID;

            for (type = 0; type < (byte)StructureType.STRUCTURE_MAX; type++)
            {
                if (string.Equals(g_table_structureInfo[type].o.name, name, StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(g_table_structureInfo[type].o.name, name) == 0)
                    return type;
            }

            return (byte)StructureType.STRUCTURE_INVALID;
        }

        /*
         * Initialize the Structure array.
         *
         * @param address If non-zero, the new location of the Structure array.
         */
        internal static void Structure_Init()
        {
            for (var i = 0; i < g_structureArray.Length; i++) g_structureArray[i] = new Structure(); //memset(g_structureArray, 0, sizeof(g_structureArray));
            Array.Fill(g_structureFindArray, null, 0, g_structureFindArray.Length); //memset(g_structureFindArray, 0, sizeof(g_structureFindArray));
            g_structureFindCount = 0;
        }

        /*
         * Remove the fog around a structure.
         *
         * @param s The Structure.
         */
        internal static void Structure_RemoveFog(Structure s)
        {
            StructureInfo si;
            tile32 position;

            if (s == null || s.o.houseID != (byte)CHouse.g_playerHouseID) return;

            si = g_table_structureInfo[s.o.type];

            position = s.o.position;

            /* ENHANCEMENT -- Fog is removed around the top left corner instead of the center of a structure. */
            if (CSharpDune.g_dune2_enhanced)
            {
                position.x += (ushort)(256 * (g_table_structure_layoutSize[si.layout].width - 1) / 2);
                position.y += (ushort)(256 * (g_table_structure_layoutSize[si.layout].height - 1) / 2);
            }

            CTile.Tile_RemoveFogInRadius(position, si.o.fogUncoverRadius);
        }

        /*
         * Recount all Structures, ignoring the cache array. Also set the structureCount
         *  of all houses to zero.
         */
        internal static void Structure_Recount()
        {
            ushort index;
            PoolFindStruct find = new PoolFindStruct();
            unchecked { find.houseID = (byte)-1; find.type = (ushort)-1; find.index = (ushort)-1; }
            House h = CHouse.House_Find(find);

            while (h != null)
            {
                h.unitCount = 0;
                h = CHouse.House_Find(find);
            }

            g_structureFindCount = 0;

            for (index = 0; index < (ushort)StructureIndex.STRUCTURE_INDEX_MAX_SOFT; index++)
            {
                Structure s = Structure_Get_ByIndex(index);
                if (s.o.flags.used) g_structureFindArray[g_structureFindCount++] = s;
            }
        }

        /*
         * Loop over all structures, preforming various of tasks.
         */
        internal static void GameLoop_Structure()
        {
            PoolFindStruct find = new PoolFindStruct();
            bool tickDegrade = false;
            bool tickStructure = false;
            bool tickScript = false;
            bool tickPalace = false;

            if (s_tickStructureDegrade <= Timer.g_timerGame && CSharpDune.g_campaignID > 1)
            {
                tickDegrade = true;
                s_tickStructureDegrade = Timer.g_timerGame + Tools.Tools_AdjustToGameSpeed(10800, 5400, 21600, true);
            }

            if (s_tickStructureStructure <= Timer.g_timerGame || s_debugInstantBuild)
            {
                tickStructure = true;
                s_tickStructureStructure = Timer.g_timerGame + Tools.Tools_AdjustToGameSpeed(30, 15, 60, true);
            }

            if (s_tickStructureScript <= Timer.g_timerGame)
            {
                tickScript = true;
                s_tickStructureScript = Timer.g_timerGame + 5;
            }

            if (s_tickStructurePalace <= Timer.g_timerGame)
            {
                tickPalace = true;
                s_tickStructurePalace = Timer.g_timerGame + 60;
            }

            find.houseID = (byte)HouseType.HOUSE_INVALID;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            if (CSharpDune.g_debugScenario) return;

            while (true)
            {
                StructureInfo si;
                HouseInfo hi;
                Structure s;
                House h;

                s = Structure_Find(find);
                if (s == null) break;
                if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;

                si = g_table_structureInfo[s.o.type];
                h = CHouse.House_Get_ByIndex(s.o.houseID);
                hi = CHouse.g_table_houseInfo[h.index];

                Script.g_scriptCurrentObject = s.o;
                Script.g_scriptCurrentStructure = s;
                Script.g_scriptCurrentUnit = null;
                Script.g_scriptCurrentTeam = null;

                if (tickPalace && s.o.type == (byte)StructureType.STRUCTURE_PALACE)
                {
                    if (s.countDown != 0)
                    {
                        s.countDown--;

                        if (s.o.houseID == (byte)CHouse.g_playerHouseID)
                        {
                            WidgetDraw.GUI_Widget_ActionPanel_Draw(true);
                        }
                    }

                    /* Check if we have to fire the weapon for the AI immediately */
                    if (s.countDown == 0 && !h.flags.human && h.flags.isAIActive)
                    {
                        Structure_ActivateSpecial(s);
                    }
                }

                if (tickDegrade && s.o.flags.degrades && s.o.hitpoints > si.o.hitpoints / 2)
                {
                    Structure_Damage(s, hi.degradingAmount, 0);
                }

                if (tickStructure)
                {
                    if (s.o.flags.upgrading)
                    {
                        ushort upgradeCost = (ushort)(si.o.buildCredits / 40);

                        if (upgradeCost <= h.credits)
                        {
                            h.credits -= upgradeCost;

                            if (s.upgradeTimeLeft > 5)
                            {
                                s.upgradeTimeLeft -= 5;
                            }
                            else
                            {
                                s.upgradeLevel++;
                                s.o.flags.upgrading = false;

                                /* Ordos Heavy Vehicle gets the last upgrade for free */
                                if (s.o.houseID == (byte)HouseType.HOUSE_ORDOS && s.o.type == (byte)StructureType.STRUCTURE_HEAVY_VEHICLE && s.upgradeLevel == 2) s.upgradeLevel = 3;

                                s.upgradeTimeLeft = (byte)(Structure_IsUpgradable(s) ? 100 : 0);
                            }
                        }
                        else
                        {
                            s.o.flags.upgrading = false;
                        }
                    }
                    else if (s.o.flags.repairing)
                    {
                        ushort repairCost;

                        /* ENHANCEMENT -- The calculation of the repaircost is a bit unfair in Dune2, because of rounding errors (they use a 256 float-resolution, which is not sufficient) */
                        if (CSharpDune.g_dune2_enhanced)
                        {
                            repairCost = (ushort)(si.o.buildCredits * 2 / si.o.hitpoints);
                        }
                        else
                        {
                            repairCost = (ushort)(((2 * 256 / si.o.hitpoints) * si.o.buildCredits + 128) / 256);
                        }

                        if (repairCost <= h.credits)
                        {
                            h.credits -= repairCost;

                            /* AIs repair in early games slower than in later games */
                            if (s.o.houseID == (byte)CHouse.g_playerHouseID || CSharpDune.g_campaignID >= 3)
                            {
                                s.o.hitpoints += 5;
                            }
                            else
                            {
                                s.o.hitpoints += 3;
                            }

                            if (s.o.hitpoints > si.o.hitpoints)
                            {
                                s.o.hitpoints = si.o.hitpoints;
                                s.o.flags.repairing = false;
                                s.o.flags.onHold = false;
                            }
                        }
                        else
                        {
                            s.o.flags.repairing = false;
                        }
                    }
                    else
                    {
                        if (!s.o.flags.onHold && s.countDown != 0 && s.o.linkedID != 0xFF && s.state == (short)StructureState.STRUCTURE_STATE_BUSY && si.o.flags.factory)
                        {
                            ObjectInfo oi;
                            ushort buildSpeed;
                            ushort buildCost;

                            if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
                            {
                                oi = g_table_structureInfo[s.objectType].o;
                            }
                            else if (s.o.type == (byte)StructureType.STRUCTURE_REPAIR)
                            {
                                oi = CUnit.g_table_unitInfo[CUnit.Unit_Get_ByIndex(s.o.linkedID).o.type].o;
                            }
                            else
                            {
                                oi = CUnit.g_table_unitInfo[s.objectType].o;
                            }

                            buildSpeed = 256;
                            if (s.o.hitpoints < si.o.hitpoints)
                            {
                                buildSpeed = (ushort)(s.o.hitpoints * 256 / si.o.hitpoints);
                            }

                            /* For AIs, we slow down building speed in all but the last campaign */
                            if ((byte)CHouse.g_playerHouseID != s.o.houseID)
                            {
                                if (buildSpeed > CSharpDune.g_campaignID * 20 + 95) buildSpeed = (ushort)(CSharpDune.g_campaignID * 20 + 95);
                            }

                            buildCost = (ushort)(oi.buildCredits * 256 / oi.buildTime);

                            if (buildSpeed < 256)
                            {
                                buildCost = (ushort)(buildSpeed * buildCost / 256);
                            }

                            if (s.o.type == (byte)StructureType.STRUCTURE_REPAIR && buildCost > 4)
                            {
                                buildCost /= 4;
                            }

                            buildCost += s.buildCostRemainder;

                            if (buildCost / 256 <= h.credits)
                            {
                                s.buildCostRemainder = (ushort)(buildCost & 0xFF);
                                h.credits -= (ushort)(buildCost / 256);

                                if (buildSpeed < s.countDown)
                                {
                                    s.countDown -= buildSpeed;
                                }
                                else
                                {
                                    s.countDown = 0;
                                    s.buildCostRemainder = 0;

                                    Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_READY);

                                    if (s.o.houseID == (byte)CHouse.g_playerHouseID)
                                    {
                                        if (s.o.type != (byte)StructureType.STRUCTURE_BARRACKS && s.o.type != (byte)StructureType.STRUCTURE_WOR_TROOPER)
                                        {
                                            ushort stringID = (ushort)Text.STR_IS_COMPLETED_AND_AWAITING_ORDERS;
                                            if (s.o.type == (byte)StructureType.STRUCTURE_HIGH_TECH) stringID = (ushort)Text.STR_IS_COMPLETE;
                                            if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD) stringID = (ushort)Text.STR_IS_COMPLETED_AND_READY_TO_PLACE;

                                            Gui.GUI_DisplayText("{0} {1}", 0, CString.String_Get_ByIndex(oi.stringID_full), CString.String_Get_ByIndex(stringID));

                                            Sound.Sound_Output_Feedback(0);
                                        }
                                    }
                                    else if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
                                    {
                                        /* An AI immediately places the structure when it is done building */
                                        Structure ns;
                                        byte i;

                                        ns = Structure_Get_ByIndex(s.o.linkedID);
                                        s.o.linkedID = 0xFF;

                                        /* The AI places structures which are operational immediately */
                                        Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_IDLE);

                                        /* Find the position to place the structure */
                                        for (i = 0; i < 5; i++)
                                        {
                                            if (ns.o.type != h.ai_structureRebuild[i][0]) continue;

                                            if (!Structure_Place(ns, h.ai_structureRebuild[i][1])) continue;

                                            h.ai_structureRebuild[i][0] = 0;
                                            h.ai_structureRebuild[i][1] = 0;
                                            break;
                                        }

                                        /* If the AI no longer had in memory where to store the structure, free it and forget about it */
                                        if (i == 5)
                                        {
                                            StructureInfo nsi = g_table_structureInfo[ns.o.type];

                                            h.credits += nsi.o.buildCredits;

                                            Structure_Free(ns);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                /* Out of money means the building gets put on hold */
                                if (s.o.houseID == (byte)CHouse.g_playerHouseID)
                                {
                                    s.o.flags.onHold = true;
                                    Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_INSUFFICIENT_FUNDS_CONSTRUCTION_IS_HALTED), 0);
                                }
                            }
                        }

                        if (s.o.type == (byte)StructureType.STRUCTURE_REPAIR)
                        {
                            if (!s.o.flags.onHold && s.countDown != 0 && s.o.linkedID != 0xFF)
                            {
                                UnitInfo ui;
                                ushort repairSpeed;
                                ushort repairCost;

                                ui = CUnit.g_table_unitInfo[CUnit.Unit_Get_ByIndex(s.o.linkedID).o.type];

                                repairSpeed = 256;
                                if (s.o.hitpoints < si.o.hitpoints)
                                {
                                    repairSpeed = (ushort)(s.o.hitpoints * 256 / si.o.hitpoints);
                                }

                                /* XXX -- This is highly unfair. Repairing becomes more expensive if your structure is more damaged */
                                repairCost = (ushort)(2 * ui.o.buildCredits / 256);

                                if (repairCost < h.credits)
                                {
                                    h.credits -= repairCost;

                                    if (repairSpeed < s.countDown)
                                    {
                                        s.countDown -= repairSpeed;
                                    }
                                    else
                                    {
                                        s.countDown = 0;

                                        Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_READY);

                                        if (s.o.houseID == (byte)CHouse.g_playerHouseID) Sound.Sound_Output_Feedback((ushort)(CHouse.g_playerHouseID + 55));
                                    }
                                }
                            }
                            else if (h.credits != 0)
                            {
                                /* Automaticly resume repairing when there is money again */
                                s.o.flags.onHold = false;
                            }
                        }

                        /* AI maintenance on structures */
                        if (h.flags.isAIActive && s.o.flags.allocated && s.o.houseID != (byte)CHouse.g_playerHouseID && h.credits != 0)
                        {
                            /* When structure is below 50% hitpoints, start repairing */
                            if (s.o.hitpoints < si.o.hitpoints / 2)
                            {
                                Structure_SetRepairingState(s, 1, null);
                            }

                            /* If the structure is not doing something, but can build stuff, see if there is stuff to build */
                            if (si.o.flags.factory && s.countDown == 0 && s.o.linkedID == 0xFF)
                            {
                                ushort type = Structure_AI_PickNextToBuild(s);

                                if (type != 0xFFFF) Structure_BuildObject(s, type);
                            }
                        }
                    }
                }

                if (tickScript)
                {
                    if (s.o.script.delay != 0)
                    {
                        s.o.script.delay--;
                    }
                    else
                    {
                        if (Script.Script_IsLoaded(s.o.script))
                        {
                            byte i;

                            /* Run the script 3 times in a row */
                            for (i = 0; i < 3; i++)
                            {
                                if (!Script.Script_Run(s.o.script)) break;
                            }

                            /* ENHANCEMENT -- Dune2 aborts all other structures if one gives a script error. This doesn't seem correct */
                            if (!CSharpDune.g_dune2_enhanced && i != 3) return;
                        }
                        else
                        {
                            Script.Script_Reset(s.o.script, s.o.script.scriptInfo);
                            Script.Script_Load(s.o.script, s.o.type);
                        }
                    }
                }
            }
        }

        /*
         * Unknown function 0A81.
         *
         * Stack: *none*
         *
         * @param script The script engine to operate on.
         * @return unknown.
         */
        internal static ushort Script_Structure_Unknown0A81(ScriptEngine script)
        {
            ushort structureIndex;
            Structure s;
            Unit u;

            //VARIABLE_NOT_USED(script);

            s = Script.g_scriptCurrentStructure;

            structureIndex = Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);

            u = Tools.Tools_Index_GetUnit(s.o.script.variables[4]);
            if (u != null)
            {
                if (structureIndex == u.o.script.variables[4]) return s.o.script.variables[4];
                CObject.Object_Script_Variable4_Clear(u.o);
            }

            CObject.Object_Script_Variable4_Clear(s.o);

            return 0;
        }

        /*
         * Find a UnitType and make it go to the current structure. In general, type
         *  should be a Carry-All for this to make any sense.
         *
         * Stack: 1 - An unit type.
         *
         * @param script The script engine to operate on.
         * @return unknown.
         */
        internal static ushort Script_Structure_FindUnitByType(ScriptEngine script)
        {
            Structure s;
            Unit u;
            Unit carryall;
            ushort type;
            ushort position;
            ushort carryallIndex;

            s = Script.g_scriptCurrentStructure;

            if (s.state != (short)StructureState.STRUCTURE_STATE_READY) return (ushort)IndexType.IT_NONE;
            if (s.o.linkedID == 0xFF) return (ushort)IndexType.IT_NONE;

            type = Script.STACK_PEEK(script, 1);

            position = Structure_FindFreePosition(s, false);

            u = CUnit.Unit_Get_ByIndex(s.o.linkedID);

            if ((byte)CHouse.g_playerHouseID == s.o.houseID && u.o.type == (byte)UnitType.UNIT_HARVESTER && (u.targetLast.x == 0 && u.targetLast.y == 0) && position != 0)
            {
                return (ushort)IndexType.IT_NONE;
            }

            carryall = CUnit.Unit_CallUnitByType((UnitType)type, s.o.houseID, Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE), position == 0);

            if (carryall == null) return (ushort)IndexType.IT_NONE;

            carryallIndex = Tools.Tools_Index_Encode(carryall.o.index, IndexType.IT_UNIT);
            CObject.Object_Script_Variable4_Set(s.o, carryallIndex);

            return carryallIndex;
        }

        /*
         * Set the state for the current structure.
         *
         * Stack: 1 - The state.
         *
         * @param script The script engine to operate on.
         * @return The value 0. Always.
         */
        internal static ushort Script_Structure_SetState(ScriptEngine script)
        {
            Structure s;
            short state;

            s = Script.g_scriptCurrentStructure;
            state = (short)Script.STACK_PEEK(script, 1);

            if (state == (short)StructureState.STRUCTURE_STATE_DETECT)
            {
                if (s.o.linkedID == 0xFF)
                {
                    state = (short)StructureState.STRUCTURE_STATE_IDLE;
                }
                else
                {
                    if (s.countDown == 0)
                    {
                        state = (short)StructureState.STRUCTURE_STATE_READY;
                    }
                    else
                    {
                        state = (short)StructureState.STRUCTURE_STATE_BUSY;
                    }
                }
            }

            Structure_SetState(s, state);

            return 0;
        }

        /*
         * Unknown function 11B9.
         *
         * Stack: 1 - Encoded tile.
         *
         * @param script The script engine to operate on.
         * @return unknown.
         */
        internal static ushort Script_Structure_Unknown11B9(ScriptEngine script)
        {
            ushort encoded;
            Unit u;

            encoded = Script.STACK_PEEK(script, 1);

            if (!Tools.Tools_Index_IsValid(encoded)) return 0;
            if (Tools.Tools_Index_GetType(encoded) != IndexType.IT_UNIT) return 0;

            u = Tools.Tools_Index_GetUnit(encoded);
            if (u == null) return 0;

            CObject.Object_Script_Variable4_Clear(u.o);
            u.targetMove = 0;

            return 0;
        }

        /*
         * Unknown function 0C5A.
         *
         * Stack: *none*
         *
         * @param script The script engine to operate on.
         * @return unknown.
         */
        internal static ushort Script_Structure_Unknown0C5A(ScriptEngine script)
        {
            tile32 tile;
            Structure s;
            Unit u;
            ushort position;

            //VARIABLE_NOT_USED(script);

            s = Script.g_scriptCurrentStructure;

            if (s.o.linkedID == 0xFF) return 0;

            u = CUnit.Unit_Get_ByIndex(s.o.linkedID);

            if (CUnit.g_table_unitInfo[u.o.type].movementType == (ushort)MovementType.MOVEMENT_WINGER && CUnit.Unit_SetPosition(u, s.o.position))
            {
                s.o.linkedID = u.o.linkedID;
                u.o.linkedID = 0xFF;

                if (s.o.linkedID == 0xFF) Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_IDLE);
                CObject.Object_Script_Variable4_Clear(s.o);

                if (s.o.houseID == (byte)CHouse.g_playerHouseID) Sound.Sound_Output_Feedback((ushort)(CHouse.g_playerHouseID + 49));

                return 1;
            }

            position = Structure_FindFreePosition(s, u.o.type == (byte)UnitType.UNIT_HARVESTER);
            if (position == 0) return 0;

            u.o.seenByHouses |= s.o.seenByHouses;

            tile = CTile.Tile_Center(CTile.Tile_UnpackTile(position));

            if (!CUnit.Unit_SetPosition(u, tile)) return 0;

            s.o.linkedID = u.o.linkedID;
            u.o.linkedID = 0xFF;

            CUnit.Unit_SetOrientation(u, (sbyte)(CTile.Tile_GetDirection(s.o.position, u.o.position) & 0xE0), true, 0);
            CUnit.Unit_SetOrientation(u, u.orientation[0].current, true, 1);

            if (u.o.houseID == (byte)CHouse.g_playerHouseID && u.o.type == (byte)UnitType.UNIT_HARVESTER)
            {
                Gui.GUI_DisplayHint((ushort)Text.STR_SEARCH_FOR_SPICE_FIELDS_TO_HARVEST, 0x6A);
            }

            if (s.o.linkedID == 0xFF) Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_IDLE);
            CObject.Object_Script_Variable4_Clear(s.o);

            if (s.o.houseID != (byte)CHouse.g_playerHouseID) return 1;
            if (s.o.type == (byte)StructureType.STRUCTURE_REPAIR) return 1;

            Sound.Sound_Output_Feedback((ushort)(CHouse.g_playerHouseID + ((u.o.type == (byte)UnitType.UNIT_HARVESTER) ? 68 : 30)));

            return 1;
        }

        /*
         * Find a Unit which is within range and not an ally.
         *
         * Stack: 1 - Range to find a target in (amount of tiles multiplied with 256).
         *
         * @param script The script engine to operate on.
         * @return The Unit Index of the closest unit within range and not friendly,
         *   or 0 if none exists.
         */
        internal static ushort Script_Structure_FindTargetUnit(ScriptEngine script)
        {
            PoolFindStruct find = new PoolFindStruct();
            Structure s;
            Unit u;
            uint distanceCurrent;
            uint targetRange;
            tile32 position;

            s = Script.g_scriptCurrentStructure;
            targetRange = Script.STACK_PEEK(script, 1);
            distanceCurrent = 32000;
            u = null;

            find.houseID = (byte)HouseType.HOUSE_INVALID;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            /* ENHANCEMENT -- The original code calculated distances from the top-left corner of the structure. */
            if (CSharpDune.g_dune2_enhanced)
            {
                position = CTile.Tile_Center(s.o.position);
            }
            else
            {
                position = s.o.position;
            }

            while (true)
            {
                ushort distance;
                Unit uf;

                uf = CUnit.Unit_Find(find);
                if (uf == null) break;

                if (CHouse.House_AreAllied(s.o.houseID, CUnit.Unit_GetHouseID(uf))) continue;

                if (uf.o.type != (byte)UnitType.UNIT_ORNITHOPTER)
                {
                    if ((uf.o.seenByHouses & (1 << s.o.houseID)) == 0) continue;
                }

                distance = CTile.Tile_GetDistance(uf.o.position, position);
                if (distance >= distanceCurrent) continue;

                if (CSharpDune.g_dune2_enhanced)
                {
                    if (uf.o.type == (byte)UnitType.UNIT_ORNITHOPTER)
                    {
                        if (distance > targetRange * 3) continue;
                    }
                    else
                    {
                        if (distance > targetRange) continue;
                    }
                }
                else
                {
                    if (uf.o.type == (byte)UnitType.UNIT_ORNITHOPTER)
                    {
                        if (distance >= targetRange * 3) continue;
                    }
                    else
                    {
                        if (distance >= targetRange) continue;
                    }
                }

                /* ENHANCEMENT -- The original code swapped the assignment, making it do nothing, Now it finds the closest unit to shoot at, what seems to be the intention */
                if (CSharpDune.g_dune2_enhanced) distanceCurrent = distance;
                u = uf;
            }

            if (u == null) return (ushort)IndexType.IT_NONE;
            return Tools.Tools_Index_Encode(u.o.index, IndexType.IT_UNIT);
        }

        /*
         * Rotate the turret to look at a tile.
         *
         * Stack: 1 - Tile to look at.
         *
         * @param script The script engine to operate on.
         * @return 0 if looking at target, otherwise 1.
         */
        internal static ushort Script_Structure_RotateTurret(ScriptEngine script)
        {
            Structure s;
            tile32 lookAt;
            Tile tile;
            ushort baseTileID;
            ushort encoded;
            short rotation;
            short rotationNeeded;
            short rotateDiff;

            encoded = Script.STACK_PEEK(script, 1);

            if (encoded == 0) return 0;

            s = Script.g_scriptCurrentStructure;
            lookAt = Tools.Tools_Index_GetTile(encoded);
            tile = Map.g_map[CTile.Tile_PackTile(s.o.position)];

            /* Find the base sprite of the structure */
            if (s.o.type == (byte)StructureType.STRUCTURE_ROCKET_TURRET)
            {
                baseTileID = Sprites.g_iconMap[Sprites.g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_BASE_ROCKET_TURRET] + 2];
            }
            else
            {
                baseTileID = Sprites.g_iconMap[Sprites.g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_BASE_DEFENSE_TURRET] + 2];
            }

            rotation = (short)(tile.groundTileID - baseTileID);
            if (rotation < 0 || rotation > 7) return 1;

            /* Find what rotation we should have to look at the target */
            rotationNeeded = CTile.Orientation_Orientation256ToOrientation8((byte)CTile.Tile_GetDirection(s.o.position, lookAt));

            /* Do we need to rotate */
            if (rotationNeeded == rotation) return 0;

            /* Find the fastest way to rotate to the correct rotation */
            rotateDiff = (short)(rotationNeeded - rotation);
            if (rotateDiff < 0) rotateDiff += 8;

            if (rotateDiff < 4)
            {
                rotation++;
            }
            else
            {
                rotation--;
            }
            rotation &= 0x7;

            /* Set the new sprites */
            tile.groundTileID = (ushort)(baseTileID + rotation);
            s.rotationSpriteDiff = (ushort)rotation;

            Map.Map_Update(CTile.Tile_PackTile(s.o.position), 0, false);

            return 1;
        }

        /*
         * Find the direction a tile is, seen from the structure. If the tile is
         *  invalid it gives the direction the structure is currently looking at.
         *
         * Stack: 1 - Tile to get the direction to, or the current direction of the
         *   structure in case the tile is invalid.
         *
         * @param script The script engine to operate on.
         * @return The direction (value between 0 and 7, shifted to the left with 5).
         */
        internal static ushort Script_Structure_GetDirection(ScriptEngine script)
        {
            Structure s;
            tile32 tile;
            ushort encoded;

            s = Script.g_scriptCurrentStructure;
            encoded = Script.STACK_PEEK(script, 1);

            if (!Tools.Tools_Index_IsValid(encoded)) return (ushort)(s.rotationSpriteDiff << 5);

            tile = Tools.Tools_Index_GetTile(encoded);

            return (ushort)(CTile.Orientation_Orientation256ToOrientation8((byte)CTile.Tile_GetDirection(s.o.position, tile)) << 5);
        }

        /*
         * Fire a bullet or missile from a (rocket) turret.
         *
         * Stack: *none*
         * Variables: 2 - Target to shoot at.
         *
         * @param script The script engine to operate on.
         * @return The time between this and the next time firing.
         */
        internal static ushort Script_Structure_Fire(ScriptEngine script)
        {
            Structure s;
            Unit u;
            tile32 position = new tile32();
            ushort target;
            ushort damage;
            ushort fireDelay;
            ushort type;

            s = Script.g_scriptCurrentStructure;

            target = script.variables[2];
            if (target == 0) return 0;

            if (s.o.type == (byte)StructureType.STRUCTURE_ROCKET_TURRET && CTile.Tile_GetDistance(Tools.Tools_Index_GetTile(target), s.o.position) >= 0x300)
            {
                type = (ushort)UnitType.UNIT_MISSILE_TURRET;
                damage = 30;
                fireDelay = Tools.Tools_AdjustToGameSpeed(CUnit.g_table_unitInfo[(ushort)UnitType.UNIT_LAUNCHER].fireDelay, 1, 0xFFFF, true);
            }
            else
            {
                type = (ushort)UnitType.UNIT_BULLET;
                damage = 20;
                fireDelay = Tools.Tools_AdjustToGameSpeed(CUnit.g_table_unitInfo[(ushort)UnitType.UNIT_TANK].fireDelay, 1, 0xFFFF, true);
            }

            position.x = (ushort)(s.o.position.x + 0x80);
            position.y = (ushort)(s.o.position.y + 0x80);
            u = CUnit.Unit_CreateBullet(position, (UnitType)type, s.o.houseID, damage, target);

            if (u == null) return 0;

            u.originEncoded = Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);

            return fireDelay;
        }

        /*
         * Get the state of the current structure.
         *
         * Stack: *none*
         *
         * @param script The script engine to operate on.
         * @return State of current structure.
         */
        internal static ushort Script_Structure_GetState(ScriptEngine script)
        {
            Structure s;

            //VARIABLE_NOT_USED(script);

            s = Script.g_scriptCurrentStructure;
            return (ushort)s.state;
        }

        /*
         * Play a voice on the structure.
         *
         * Stack: 1 - The VoiceID to play.
         *
         * @param script The script engine to operate on.
         * @return unknown.
         */
        internal static ushort Script_Structure_VoicePlay(ScriptEngine script)
        {
            Structure s;

            s = Script.g_scriptCurrentStructure;

            if (s.o.houseID != (byte)CHouse.g_playerHouseID) return 0;

            Sound.Voice_PlayAtTile((short)Script.STACK_PEEK(script, 1), s.o.position);

            return 0;
        }

        /*
         * Remove fog around the current structure.
         * Radius to uncover is taken from the current structure info.
         *
         * Stack: *none*
         *
         * @param script The script engine to operate on.
         * @return The value 0. Always.
         */
        internal static ushort Script_Structure_RemoveFogAroundTile(ScriptEngine script)
        {
            //VARIABLE_NOT_USED(script);

            Structure_RemoveFog(Script.g_scriptCurrentStructure);

            return 0;
        }

        /*
         * Refine spice in the current structure.
         *
         * Stack: *none*
         *
         * @param script The script engine to operate on.
         * @return 0 if there is no spice to refine, otherwise 1.
         */
        internal static ushort Script_Structure_RefineSpice(ScriptEngine script)
        {
            StructureInfo si;
            Structure s;
            Unit u;
            House h;
            ushort harvesterStep, creditsStep;

            //VARIABLE_NOT_USED(script);

            s = Script.g_scriptCurrentStructure;

            if (s.o.linkedID == 0xFF)
            {
                Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_IDLE);
                return 0;
            }

            u = CUnit.Unit_Get_ByIndex(s.o.linkedID);
            si = g_table_structureInfo[s.o.type];

            harvesterStep = (ushort)((s.o.hitpoints * 256 / si.o.hitpoints) * 3 / 256);

            if (u.amount < harvesterStep) harvesterStep = u.amount;
            if (u.amount != 0 && harvesterStep < 1) harvesterStep = 1;
            if (harvesterStep == 0) return 0;

            creditsStep = 7;
            if (u.o.houseID != (byte)CHouse.g_playerHouseID)
            {
                creditsStep += (ushort)((Tools.Tools_Random_256() % 4) - 1);
            }

            creditsStep *= harvesterStep;

            if (CHouse.House_AreAllied((byte)CHouse.g_playerHouseID, s.o.houseID))
            {
                CScenario.g_scenario.harvestedAllied += creditsStep;
                if (CScenario.g_scenario.harvestedAllied > 65000) CScenario.g_scenario.harvestedAllied = 65000;
            }
            else
            {
                CScenario.g_scenario.harvestedEnemy += creditsStep;
                if (CScenario.g_scenario.harvestedEnemy > 65000) CScenario.g_scenario.harvestedEnemy = 65000;
            }

            h = CHouse.House_Get_ByIndex(s.o.houseID);
            h.credits += creditsStep;
            u.amount -= (byte)harvesterStep;

            if (u.amount == 0) u.o.flags.inTransport = false;
            s.o.script.delay = 6;
            return 1;
        }

        /*
         * Make the structure explode.
         *
         * Stack: *none*
         *
         * @param script The script engine to operate on.
         * @return unknown.
         */
        internal static ushort Script_Structure_Explode(ScriptEngine script)
        {
            Structure s;
            ushort position;
            ushort layout;
            ushort i;

            //VARIABLE_NOT_USED(script);

            s = Script.g_scriptCurrentStructure;
            layout = g_table_structureInfo[s.o.type].layout;
            position = CTile.Tile_PackTile(s.o.position);

            for (i = 0; i < g_table_structure_layoutTileCount[layout]; i++)
            {
                tile32 tile;

                tile = CTile.Tile_UnpackTile((ushort)(position + g_table_structure_layoutTiles[layout][i]));

                Map.Map_MakeExplosion((ushort)ExplosionType.EXPLOSION_STRUCTURE, tile, 0, 0);
            }

            return 0;
        }

        /*
         * Destroy a structure and spawn soldiers around the place.
         *
         * Stack: *none*
         *
         * @param script The script engine to operate on.
         * @return Always 0.
         */
        internal static ushort Script_Structure_Destroy(ScriptEngine script)
        {
            Structure s;
            ushort position;
            ushort layout;
            ushort i;

            //VARIABLE_NOT_USED(script);

            s = Script.g_scriptCurrentStructure;
            layout = g_table_structureInfo[s.o.type].layout;
            position = CTile.Tile_PackTile(s.o.position);

            Structure_Remove(s);

            for (i = 0; i < g_table_structure_layoutTileCount[layout]; i++)
            {
                tile32 tile;
                Unit u;

                tile = CTile.Tile_UnpackTile((ushort)(position + g_table_structure_layoutTiles[layout][i]));

                if (g_table_structureInfo[s.o.type].o.spawnChance < Tools.Tools_Random_256()) continue;

                u = CUnit.Unit_Create((ushort)UnitIndex.UNIT_INDEX_INVALID, (byte)UnitType.UNIT_SOLDIER, s.o.houseID, tile, (sbyte)Tools.Tools_Random_256());
                if (u == null) continue;

                u.o.hitpoints = (ushort)(CUnit.g_table_unitInfo[(int)UnitType.UNIT_SOLDIER].o.hitpoints * (Tools.Tools_Random_256() & 3) / 256);

                if (s.o.houseID != (byte)CHouse.g_playerHouseID)
                {
                    CUnit.Unit_SetAction(u, ActionType.ACTION_ATTACK);
                    continue;
                }

                CUnit.Unit_SetAction(u, ActionType.ACTION_MOVE);

                tile = CTile.Tile_MoveByRandom(u.o.position, 32, true);

                u.targetMove = Tools.Tools_Index_Encode(CTile.Tile_PackTile(tile), IndexType.IT_TILE);
            }

            if (CSharpDune.g_debugScenario) return 0;
            if (s.o.houseID != (byte)CHouse.g_playerHouseID) return 0;

            if (Config.g_config.language == (byte)Language.LANGUAGE_FRENCH)
            {
                Gui.GUI_DisplayText("{0} {1} {2}", 0, CString.String_Get_ByIndex(g_table_structureInfo[s.o.type].o.stringID_full), CHouse.g_table_houseInfo[s.o.houseID].name, CString.String_Get_ByIndex(Text.STR_IS_DESTROYED));
            }
            else
            {
                Gui.GUI_DisplayText("{0} {1} {2}", 0, CHouse.g_table_houseInfo[s.o.houseID].name, CString.String_Get_ByIndex(g_table_structureInfo[s.o.type].o.stringID_full), CString.String_Get_ByIndex(Text.STR_IS_DESTROYED));
            }

            return 0;
        }

        /*
         * Calculate the power usage and production, and the credits storage.
         *
         * @param h The house to calculate the numbers for.
         */
        internal static void Structure_CalculateHitpointsMax(House h)
        {
            PoolFindStruct find = new PoolFindStruct();
            ushort power = 0;

            if (h == null) return;

            if (h.index == (byte)CHouse.g_playerHouseID) CHouse.House_UpdateRadarState(h);

            if (h.powerUsage == 0)
            {
                power = 256;
            }
            else
            {
                power = (ushort)Min(h.powerProduction * 256 / h.powerUsage, 256);
            }

            find.houseID = h.index;
            find.index = 0xFFFF;
            find.type = 0xFFFF;

            while (true)
            {
                StructureInfo si;
                Structure s;

                s = Structure_Find(find);
                if (s == null) return;
                if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;

                si = g_table_structureInfo[s.o.type];

                s.hitpointsMax = (ushort)(si.o.hitpoints * power / 256);
                s.hitpointsMax = (ushort)Max(s.hitpointsMax, si.o.hitpoints / 2);

                if (s.hitpointsMax >= s.o.hitpoints) continue;
                Structure_Damage(s, 1, 0);
            }
        }

        /*
         * Find a free spot for units next to a structure.
         * @param s Structure that needs a free spot.
         * @param checkForSpice Spot should be as close to spice as possible.
         * @return Position of the free spot, or \c 0 if no free spot available.
         */
        static ushort Structure_FindFreePosition(Structure s, bool checkForSpice)
        {
            StructureInfo si;
            ushort packed;
            ushort spicePacked;  /* Position of the spice, or 0 if not used or if no spice. */
            ushort bestPacked;
            ushort bestDistance; /* If > 0, distance to the spice from bestPacked. */
            ushort i, j;

            if (s == null) return 0;

            si = g_table_structureInfo[s.o.type];
            packed = CTile.Tile_PackTile(CTile.Tile_Center(s.o.position));

            spicePacked = (ushort)(checkForSpice ? Map.Map_SearchSpice(packed, 10) : 0);
            bestPacked = 0;
            bestDistance = 0;

            i = (ushort)(Tools.Tools_Random_256() & 0xF);
            for (j = 0; j < 16; j++, i = (ushort)((i + 1) & 0xF))
            {
                ushort offset;
                ushort curPacked;
                ushort type;
                Tile t;

                offset = (ushort)g_table_structure_layoutTilesAround[si.layout][i];
                if (offset == 0) continue;

                curPacked = (ushort)(packed + offset);
                if (!Map.Map_IsValidPosition(curPacked)) continue;

                type = Map.Map_GetLandscapeType(curPacked);
                if (type == (ushort)LandscapeType.LST_WALL || type == (ushort)LandscapeType.LST_ENTIRELY_MOUNTAIN || type == (ushort)LandscapeType.LST_PARTIAL_MOUNTAIN) continue;

                t = Map.g_map[curPacked];
                if (t.hasUnit || t.hasStructure) continue;

                if (!checkForSpice) return curPacked;

                if (bestDistance == 0 || CTile.Tile_GetDistancePacked(curPacked, spicePacked) < bestDistance)
                {
                    bestPacked = curPacked;
                    bestDistance = CTile.Tile_GetDistancePacked(curPacked, spicePacked);
                }
            }

            return bestPacked;
        }

        /*
         * Sets or toggle the repairing state of the given Structure.
         *
         * @param s The Structure.
         * @param value The repairing state, -1 to toggle.
         * @param w The widget.
         * @return True if and only if the state changed.
         */
        internal static bool Structure_SetRepairingState(Structure s, sbyte state, Widget w)
        {
            bool ret = false;

            if (s == null) return false;

            /* ENHANCEMENT -- If a structure gets damaged during upgrading, pressing the "Upgrading" button silently starts the repair of the structure, and doesn't cancel upgrading. */
            if (CSharpDune.g_dune2_enhanced && s.o.flags.upgrading) return false;

            if (!s.o.flags.allocated) state = 0;

            if (state == -1) state = (sbyte)(s.o.flags.repairing ? 0 : 1);

            if (state == 0 && s.o.flags.repairing)
            {
                if (s.o.houseID == (byte)CHouse.g_playerHouseID)
                {
                    Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_REPAIRING_STOPS), 2);
                }

                s.o.flags.repairing = false;
                s.o.flags.onHold = false;

                CWidget.GUI_Widget_MakeNormal(w, false);

                ret = true;
            }

            if (state == 0 || s.o.flags.repairing || s.o.hitpoints == g_table_structureInfo[s.o.type].o.hitpoints) return ret;

            if (s.o.houseID == (byte)CHouse.g_playerHouseID)
            {
                Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_REPAIRING_STARTS), 2);
            }

            s.o.flags.onHold = true;
            s.o.flags.repairing = true;

            CWidget.GUI_Widget_MakeSelected(w, false);

            return true;
        }

        /*
         * Find the next object to build.
         * @param s The structure in which we can build something.
         * @return The type (either UnitType or StructureType) of what we should build next.
         */
        static ushort Structure_AI_PickNextToBuild(Structure s)
        {
            PoolFindStruct find = new PoolFindStruct();
            /*ushort*/
            int buildable;
            ushort type;
            House h;
            int i;

            if (s == null) return 0xFFFF;

            h = CHouse.House_Get_ByIndex(s.o.houseID);
            buildable = (int)Structure_GetBuildable(s);

            if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
            {
                for (i = 0; i < 5; i++)
                {
                    type = h.ai_structureRebuild[i][0];

                    if (type == 0) continue;
                    if ((buildable & (1 << type)) == 0) continue;

                    return type;
                }

                return 0xFFFF;
            }

            if (s.o.type == (byte)StructureType.STRUCTURE_HIGH_TECH)
            {
                find.houseID = s.o.houseID;
                find.index = 0xFFFF;
                find.type = (ushort)UnitType.UNIT_CARRYALL;

                while (true)
                {
                    Unit u;

                    u = CUnit.Unit_Find(find);
                    if (u == null) break;

                    buildable &= (int)~UnitFlag.FLAG_UNIT_CARRYALL;
                }
            }

            if (s.o.type == (byte)StructureType.STRUCTURE_HEAVY_VEHICLE)
            {
                buildable &= (int)~UnitFlag.FLAG_UNIT_HARVESTER;
                buildable &= (int)~UnitFlag.FLAG_UNIT_MCV;
            }

            type = 0xFFFF;
            for (i = 0; i < (int)UnitType.UNIT_MAX; i++)
            {
                if ((buildable & (1 << i)) == 0) continue;

                if ((Tools.Tools_Random_256() % 4) == 0) type = (ushort)i;

                if (type != 0xFFFF)
                {
                    if (CUnit.g_table_unitInfo[i].o.priorityBuild <= CUnit.g_table_unitInfo[type].o.priorityBuild) continue;
                }

                type = (ushort)i;
            }

            return type;
        }

        /*
         * Activate the special weapon of a house.
         *
         * @param s The structure which launches the weapon. Has to be the Palace.
         */
        internal static void Structure_ActivateSpecial(Structure s)
        {
            House h;

            if (s == null) return;
            if (s.o.type != (byte)StructureType.STRUCTURE_PALACE) return;

            h = CHouse.House_Get_ByIndex(s.o.houseID);
            if (!h.flags.used) return;

            switch ((HouseWeapon)CHouse.g_table_houseInfo[s.o.houseID].specialWeapon)
            {
                case HouseWeapon.HOUSE_WEAPON_MISSILE:
                    {
                        Unit u;
                        tile32 position = new tile32();

                        position.x = 0xFFFF;
                        position.y = 0xFFFF;

                        CSharpDune.g_validateStrictIfZero++;
                        u = CUnit.Unit_Create((ushort)UnitIndex.UNIT_INDEX_INVALID, (byte)UnitType.UNIT_MISSILE_HOUSE, s.o.houseID, position, (sbyte)Tools.Tools_Random_256());
                        CSharpDune.g_validateStrictIfZero--;

                        CUnit.g_unitHouseMissile = u;
                        if (u == null) break;

                        s.countDown = CHouse.g_table_houseInfo[s.o.houseID].specialCountDown;

                        if (!h.flags.human)
                        {
                            PoolFindStruct find = new PoolFindStruct();

                            find.houseID = (byte)HouseType.HOUSE_INVALID;
                            find.type = 0xFFFF;
                            find.index = 0xFFFF;

                            /* For the AI, try to find the first structure which is not ours, and launch missile to there */
                            while (true)
                            {
                                Structure sf;

                                sf = Structure_Find(find);
                                if (sf == null) break;
                                if (sf.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || sf.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || sf.o.type == (byte)StructureType.STRUCTURE_WALL) continue;

                                if (CHouse.House_AreAllied(s.o.houseID, sf.o.houseID)) continue;

                                CUnit.Unit_LaunchHouseMissile(CTile.Tile_PackTile(sf.o.position));

                                return;
                            }

                            /* We failed to find a target, so remove the missile */
                            CUnit.Unit_Free(u);
                            CUnit.g_unitHouseMissile = null;

                            return;
                        }

                        /* Give the user 7 seconds to select their target */
                        CHouse.g_houseMissileCountdown = 7;

                        Gui.GUI_ChangeSelectionType((ushort)SelectionType.SELECTIONTYPE_TARGET);
                    }
                    break;

                case HouseWeapon.HOUSE_WEAPON_FREMEN:
                    {
                        ushort location;
                        ushort i;

                        /* Find a random location to appear */
                        location = Map.Map_FindLocationTile(4, (byte)HouseType.HOUSE_INVALID);

                        for (i = 0; i < 5; i++)
                        {
                            Unit u;
                            tile32 position;
                            ushort orientation;
                            ushort unitType;

                            Tools.Tools_Random_256();

                            position = CTile.Tile_UnpackTile(location);
                            position = CTile.Tile_MoveByRandom(position, 32, true);

                            orientation = Tools.Tools_RandomLCG_Range(0, 3);
                            unitType = (ushort)((orientation == 1) ? UnitType.UNIT_TROOPER : UnitType.UNIT_TROOPERS);

                            CSharpDune.g_validateStrictIfZero++;
                            u = CUnit.Unit_Create((ushort)UnitIndex.UNIT_INDEX_INVALID, (byte)unitType, (byte)HouseType.HOUSE_FREMEN, position, (sbyte)orientation);
                            CSharpDune.g_validateStrictIfZero--;

                            if (u == null) continue;

                            CUnit.Unit_SetAction(u, ActionType.ACTION_HUNT);
                        }

                        s.countDown = CHouse.g_table_houseInfo[s.o.houseID].specialCountDown;
                    }
                    break;

                case HouseWeapon.HOUSE_WEAPON_SABOTEUR:
                    {
                        Unit u;
                        ushort position;

                        /* Find a spot next to the structure */
                        position = Structure_FindFreePosition(s, false);

                        /* If there is no spot, reset countdown */
                        if (position == 0)
                        {
                            s.countDown = 1;
                            return;
                        }

                        CSharpDune.g_validateStrictIfZero++;
                        u = CUnit.Unit_Create((ushort)UnitIndex.UNIT_INDEX_INVALID, (byte)UnitType.UNIT_SABOTEUR, s.o.houseID, CTile.Tile_UnpackTile(position), (sbyte)Tools.Tools_Random_256());
                        CSharpDune.g_validateStrictIfZero--;

                        if (u == null) return;

                        CUnit.Unit_SetAction(u, ActionType.ACTION_SABOTAGE);

                        s.countDown = CHouse.g_table_houseInfo[s.o.houseID].specialCountDown;
                    }
                    break;

                default: break;
            }

            if (s.o.houseID == (byte)CHouse.g_playerHouseID)
            {
                WidgetDraw.GUI_Widget_ActionPanel_Draw(true);
            }
        }

        internal static uint Structure_GetBuildable(Structure s)
        {
            StructureInfo si;
            uint structuresBuilt;
            uint ret = 0;
            int i;

            if (s == null) return 0;

            si = g_table_structureInfo[s.o.type];

            structuresBuilt = CHouse.House_Get_ByIndex(s.o.houseID).structuresBuilt;

            switch ((StructureType)s.o.type)
            {
                case StructureType.STRUCTURE_LIGHT_VEHICLE:
                case StructureType.STRUCTURE_HEAVY_VEHICLE:
                case StructureType.STRUCTURE_HIGH_TECH:
                case StructureType.STRUCTURE_WOR_TROOPER:
                case StructureType.STRUCTURE_BARRACKS:
                    for (i = 0; i < (int)UnitType.UNIT_MAX; i++)
                    {
                        CUnit.g_table_unitInfo[i].o.available = 0;
                    }

                    for (i = 0; i < 8; i++)
                    {
                        UnitInfo ui;
                        ushort upgradeLevelRequired;
                        byte unitType = si.buildableUnits[i];

                        if (unitType == (byte)UnitType.UNIT_INVALID) continue;

                        if (unitType == (byte)UnitType.UNIT_TRIKE && s.creatorHouseID == (ushort)HouseType.HOUSE_ORDOS) unitType = (byte)UnitType.UNIT_RAIDER_TRIKE;

                        ui = CUnit.g_table_unitInfo[unitType];
                        upgradeLevelRequired = ui.o.upgradeLevelRequired;

                        if (unitType == (byte)UnitType.UNIT_SIEGE_TANK && s.creatorHouseID == (ushort)HouseType.HOUSE_ORDOS) upgradeLevelRequired--;

                        if ((structuresBuilt & ui.o.structuresRequired) != ui.o.structuresRequired) continue;
                        if ((ui.o.availableHouse & (1 << s.creatorHouseID)) == 0) continue;

                        if (s.upgradeLevel >= upgradeLevelRequired)
                        {
                            ui.o.available = 1;

                            ret |= (uint)(1 << unitType);
                            continue;
                        }

                        if (s.upgradeTimeLeft != 0 && s.upgradeLevel + 1 >= upgradeLevelRequired)
                        {
                            ui.o.available = -1;
                        }
                    }
                    return ret;

                case StructureType.STRUCTURE_CONSTRUCTION_YARD:
                    for (i = 0; i < (int)StructureType.STRUCTURE_MAX; i++)
                    {
                        StructureInfo localsi = g_table_structureInfo[i];
                        ushort availableCampaign;
                        uint structuresRequired;

                        localsi.o.available = 0;

                        availableCampaign = localsi.o.availableCampaign;
                        structuresRequired = localsi.o.structuresRequired;

                        if (i == (int)StructureType.STRUCTURE_WOR_TROOPER && s.o.houseID == (byte)HouseType.HOUSE_HARKONNEN && CSharpDune.g_campaignID >= 1)
                        {
                            structuresRequired &= ~((uint)1 << (byte)StructureType.STRUCTURE_BARRACKS); //TODO: Check
                            availableCampaign = 2;
                        }

                        if ((structuresBuilt & structuresRequired) == structuresRequired || s.o.houseID != (byte)CHouse.g_playerHouseID)
                        {
                            if (s.o.houseID != (byte)HouseType.HOUSE_HARKONNEN && i == (int)StructureType.STRUCTURE_LIGHT_VEHICLE)
                            {
                                availableCampaign = 2;
                            }

                            if (CSharpDune.g_campaignID >= availableCampaign - 1 && (localsi.o.availableHouse & (1 << s.o.houseID)) != 0)
                            {
                                if (s.upgradeLevel >= localsi.o.upgradeLevelRequired || s.o.houseID != (byte)CHouse.g_playerHouseID)
                                {
                                    localsi.o.available = 1;

                                    ret |= (uint)(1 << i);
                                }
                                else if (s.upgradeTimeLeft != 0 && s.upgradeLevel + 1 >= localsi.o.upgradeLevelRequired)
                                {
                                    localsi.o.available = -1;
                                }
                            }
                        }
                    }
                    return ret;

                case StructureType.STRUCTURE_STARPORT:
                    unchecked { return (uint)-1; }

                default:
                    return 0;
            }
        }

        /*
         * Make the given Structure build an object.
         *
         * @param s The Structure.
         * @param objectType The type of the object to build or a special value (0xFFFD, 0xFFFE, 0xFFFF).
         * @return ??.
         */
        internal static bool Structure_BuildObject(Structure s, ushort objectType)
        {
            StructureInfo si;
            string str;
            Object o;
            ObjectInfo oi;

            if (s == null) return false;

            si = g_table_structureInfo[s.o.type];

            if (!si.o.flags.factory) return false;

            Structure_SetRepairingState(s, 0, null);

            if (objectType == 0xFFFD)
            {
                Structure_SetUpgradingState(s, 1, null);
                return false;
            }

            if (objectType == 0xFFFF || objectType == 0xFFFE)
            {
                ushort upgradeCost = 0;
                uint buildable;

                if (Structure_IsUpgradable(s) && si.o.hitpoints == s.o.hitpoints)
                {
                    upgradeCost = (ushort)((si.o.buildCredits + (si.o.buildCredits >> 15)) / 2);
                }

                if (upgradeCost != 0 && s.o.type == (byte)StructureType.STRUCTURE_HIGH_TECH && s.o.houseID == (byte)HouseType.HOUSE_HARKONNEN) upgradeCost = 0;
                if (s.o.type == (byte)StructureType.STRUCTURE_STARPORT) upgradeCost = 0;

                buildable = Structure_GetBuildable(s);

                if (buildable == 0)
                {
                    s.objectType = 0;
                    return false;
                }

                if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
                {
                    byte i;

                    Gui.g_factoryWindowConstructionYard = true;

                    for (i = 0; i < (byte)StructureType.STRUCTURE_MAX; i++)
                    {
                        if ((buildable & (1 << i)) == 0) continue;
                        g_table_structureInfo[i].o.available = 1;
                        if (objectType != 0xFFFE) continue;
                        s.objectType = i;
                        return false;
                    }
                }
                else
                {
                    Gui.g_factoryWindowConstructionYard = false;

                    if (s.o.type == (byte)StructureType.STRUCTURE_STARPORT)
                    {
                        byte linkedID = 0xFF;
                        short[] availableUnits = new short[(int)UnitType.UNIT_MAX];
                        Unit u;
                        bool loop;

                        //memset(availableUnits, 0, sizeof(availableUnits));

                        do
                        {
                            byte i;

                            loop = false;

                            for (i = 0; i < (byte)UnitType.UNIT_MAX; i++)
                            {
                                short unitsAtStarport = CUnit.g_starportAvailable[i];

                                if (unitsAtStarport == 0)
                                {
                                    CUnit.g_table_unitInfo[i].o.available = 0;
                                }
                                else if (unitsAtStarport < 0)
                                {
                                    CUnit.g_table_unitInfo[i].o.available = -1;
                                }
                                else if (unitsAtStarport > availableUnits[i])
                                {
                                    CSharpDune.g_validateStrictIfZero++;
                                    u = CUnit.Unit_Allocate((ushort)UnitIndex.UNIT_INDEX_INVALID, i, s.o.houseID);
                                    CSharpDune.g_validateStrictIfZero--;

                                    if (u != null)
                                    {
                                        loop = true;
                                        u.o.linkedID = linkedID;
                                        linkedID = (byte)(u.o.index & 0xFF);
                                        availableUnits[i]++;
                                        CUnit.g_table_unitInfo[i].o.available = (sbyte)availableUnits[i];
                                    }
                                    else if (availableUnits[i] == 0) CUnit.g_table_unitInfo[i].o.available = -1;
                                }
                            }
                        } while (loop);

                        while (linkedID != 0xFF)
                        {
                            u = CUnit.Unit_Get_ByIndex(linkedID);
                            linkedID = u.o.linkedID;
                            CUnit.Unit_Free(u);
                        }
                    }
                    else
                    {
                        byte i;

                        for (i = 0; i < (byte)UnitType.UNIT_MAX; i++)
                        {
                            if ((buildable & (1 << i)) == 0) continue;
                            CUnit.g_table_unitInfo[i].o.available = 1;
                            if (objectType != 0xFFFE) continue;
                            s.objectType = i;
                            return false;
                        }
                    }
                }

                if (objectType == 0xFFFF)
                {
                    FactoryResult res;

                    Sprites.Sprites_UnloadTiles();

                    Buffer.BlockCopy(Gfx.g_paletteActive, 0, Gfx.g_palette1, 0, 256 * 3); //memmove(g_palette1, g_paletteActive, 256 * 3);

                    Gui.GUI_ChangeSelectionType((ushort)SelectionType.SELECTIONTYPE_MENTAT);

                    Timer.Timer_SetTimer(TimerType.TIMER_GAME, false);

                    res = Gui.GUI_DisplayFactoryWindow(Gui.g_factoryWindowConstructionYard, s.o.type == (byte)StructureType.STRUCTURE_STARPORT, upgradeCost);

                    Timer.Timer_SetTimer(TimerType.TIMER_GAME, true);

                    Sprites.Sprites_LoadTiles();

                    Gfx.GFX_SetPalette(Gfx.g_palette1);

                    Gui.GUI_ChangeSelectionType((ushort)SelectionType.SELECTIONTYPE_STRUCTURE);

                    if (res == FactoryResult.FACTORY_RESUME) return false;

                    if (res == FactoryResult.FACTORY_UPGRADE)
                    {
                        Structure_SetUpgradingState(s, 1, null);
                        return false;
                    }

                    if (res == FactoryResult.FACTORY_BUY)
                    {
                        House h;
                        byte i;

                        h = CHouse.House_Get_ByIndex(s.o.houseID);

                        for (i = 0; i < 25; i++)
                        {
                            Unit u;

                            if (Gui.g_factoryWindowItems[i].amount == 0) continue;
                            objectType = Gui.g_factoryWindowItems[i].objectType;

                            if (s.o.type != (byte)StructureType.STRUCTURE_STARPORT)
                            {
                                Structure_CancelBuild(s);

                                s.objectType = objectType;

                                if (!Gui.g_factoryWindowConstructionYard) continue;

                                if (Structure_CheckAvailableConcrete(objectType, s.o.houseID)) continue;

                                if (Gui.GUI_DisplayHint((ushort)Text.STR_THERE_ISNT_ENOUGH_OPEN_CONCRETE_TO_PLACE_THIS_STRUCTURE_YOU_MAY_PROCEED_BUT_WITHOUT_ENOUGH_CONCRETE_THE_BUILDING_WILL_NEED_REPAIRS, g_table_structureInfo[objectType].o.spriteID) == 0) continue;

                                s.objectType = objectType;

                                return false;
                            }

                            CSharpDune.g_validateStrictIfZero++;
                            {
                                tile32 tile = new tile32 { x = 0xFFFF, y = 0xFFFF };
                                u = CUnit.Unit_Create((ushort)UnitIndex.UNIT_INDEX_INVALID, (byte)objectType, s.o.houseID, tile, 0);
                            }
                            CSharpDune.g_validateStrictIfZero--;

                            if (u == null)
                            {
                                h.credits += CUnit.g_table_unitInfo[(int)UnitType.UNIT_CARRYALL].o.buildCredits;
                                if (s.o.houseID != (byte)CHouse.g_playerHouseID) continue;
                                Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_UNABLE_TO_CREATE_MORE), 2);
                                continue;
                            }

                            g_structureIndex = s.o.index;

                            if (h.starportTimeLeft == 0) h.starportTimeLeft = CHouse.g_table_houseInfo[h.index].starportDeliveryTime;

                            u.o.linkedID = (byte)(h.starportLinkedID & 0xFF);
                            h.starportLinkedID = u.o.index;

                            CUnit.g_starportAvailable[objectType]--;
                            if (CUnit.g_starportAvailable[objectType] <= 0) CUnit.g_starportAvailable[objectType] = -1;

                            Gui.g_factoryWindowItems[i].amount--;
                            if (Gui.g_factoryWindowItems[i].amount != 0) i--;
                        }
                    }
                }
                else
                {
                    s.objectType = objectType;
                }
            }

            if (s.o.type == (byte)StructureType.STRUCTURE_STARPORT) return true;

            if (s.objectType != objectType) Structure_CancelBuild(s);

            if (s.o.linkedID != 0xFF || objectType == 0xFFFF) return false;

            if (s.o.type != (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
            {
                tile32 tile = new tile32 { x = 0xFFFF, y = 0xFFFF };

                oi = CUnit.g_table_unitInfo[objectType].o;
                o = CUnit.Unit_Create((ushort)UnitIndex.UNIT_INDEX_INVALID, (byte)objectType, s.o.houseID, tile, 0).o;
                str = CString.String_Get_ByIndex(CUnit.g_table_unitInfo[objectType].o.stringID_full);
            }
            else
            {
                oi = g_table_structureInfo[objectType].o;
                o = Structure_Create((ushort)StructureIndex.STRUCTURE_INDEX_INVALID, (byte)objectType, s.o.houseID, 0xFFFF).o;
                str = CString.String_Get_ByIndex(g_table_structureInfo[objectType].o.stringID_full);
            }

            s.o.flags.onHold = false;

            if (o != null)
            {
                s.o.linkedID = (byte)(o.index & 0xFF);
                s.objectType = objectType;
                s.countDown = (ushort)(oi.buildTime << 8);

                Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_BUSY);

                if (s.o.houseID != (byte)CHouse.g_playerHouseID) return true;

                Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_PRODUCTION_OF_S_HAS_STARTED), 2, str);

                return true;
            }

            if (s.o.houseID != (byte)CHouse.g_playerHouseID) return false;

            Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_UNABLE_TO_CREATE_MORE), 2);

            return false;
        }

        /*
         * Sets or toggle the upgrading state of the given Structure.
         *
         * @param s The Structure.
         * @param value The upgrading state, -1 to toggle.
         * @param w The widget.
         * @return True if and only if the state changed.
         */
        internal static bool Structure_SetUpgradingState(Structure s, sbyte state, Widget w)
        {
            bool ret = false;

            if (s == null) return false;

            if (state == -1) state = (sbyte)(s.o.flags.upgrading ? 0 : 1);

            if (state == 0 && s.o.flags.upgrading)
            {
                if (s.o.houseID == (byte)CHouse.g_playerHouseID)
                {
                    Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_UPGRADING_STOPS), 2);
                }

                s.o.flags.upgrading = false;
                s.o.flags.onHold = false;

                CWidget.GUI_Widget_MakeNormal(w, false);

                ret = true;
            }

            if (state == 0 || s.o.flags.upgrading || s.upgradeTimeLeft == 0) return ret;

            if (s.o.houseID == (byte)CHouse.g_playerHouseID)
            {
                Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_UPGRADING_STARTS), 2);
            }

            s.o.flags.onHold = true;
            s.o.flags.repairing = false;
            s.o.flags.upgrading = true;

            CWidget.GUI_Widget_MakeSelected(w, false);

            return true;
        }

        /*
         * Cancel the building of object for given structure.
         *
         * @param s The Structure.
         */
        static void Structure_CancelBuild(Structure s)
        {
            ObjectInfo oi;

            if (s == null || s.o.linkedID == 0xFF) return;

            if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
            {
                Structure s2 = Structure_Get_ByIndex(s.o.linkedID);
                oi = g_table_structureInfo[s2.o.type].o;
                Structure_Free(s2);
            }
            else
            {
                Unit u = CUnit.Unit_Get_ByIndex(s.o.linkedID);
                oi = CUnit.g_table_unitInfo[u.o.type].o;
                CUnit.Unit_Free(u);
            }

            CHouse.House_Get_ByIndex(s.o.houseID).credits += (ushort)(((oi.buildTime - (s.countDown >> 8)) * 256 / oi.buildTime) * oi.buildCredits / 256);

            s.o.flags.onHold = false;
            s.countDown = 0;
            s.o.linkedID = 0xFF;
        }

        /*
         * Check if requested structureType can be build on the map with concrete below.
         *
         * @param structureType The type of structure to check for.
         * @param houseID The house to check for.
         * @return True if and only if there are enough slabs available on the map to
         *  build requested structure.
         */
        static bool Structure_CheckAvailableConcrete(ushort structureType, byte houseID)
        {
            StructureInfo si;
            short tileCount;
            short i;

            si = g_table_structureInfo[structureType];

            tileCount = (short)g_table_structure_layoutTileCount[si.layout];

            if (structureType == (ushort)StructureType.STRUCTURE_SLAB_1x1 || structureType == (ushort)StructureType.STRUCTURE_SLAB_2x2) return true;

            for (i = 0; i < 4096; i++)
            {
                bool stop = true;
                ushort j;

                for (j = 0; j < tileCount; j++)
                {
                    ushort packed = (ushort)(i + g_table_structure_layoutTiles[si.layout][j]);
                    /* XXX -- This can overflow, and we should check for that */

                    if (Map.Map_GetLandscapeType(packed) == (ushort)LandscapeType.LST_CONCRETE_SLAB && Map.g_map[packed].houseID == houseID) continue;

                    stop = false;
                    break;
                }

                if (stop) return true;
            }

            return false;
        }

        /*
         * Place a structure on the map.
         *
         * @param structure The structure to place on the map.
         * @param position The (packed) tile to place the struction on.
         * @return True if and only if the structure is placed on the map.
         */
        internal static bool Structure_Place(Structure s, ushort position)
        {
            StructureInfo si;
            short validBuildLocation;

            if (s == null) return false;
            if (position == 0xFFFF) return false;

            si = g_table_structureInfo[s.o.type];

            switch ((StructureType)s.o.type)
            {
                case StructureType.STRUCTURE_WALL:
                    {
                        Tile t;

                        if (Structure_IsValidBuildLocation(position, StructureType.STRUCTURE_WALL) == 0) return false;

                        t = Map.g_map[position];
                        t.groundTileID = (ushort)(Sprites.g_wallTileID + 1);
                        /* ENHANCEMENT -- Dune2 wrongfully only removes the lower 2 bits, where the lower 3 bits are the owner. This is no longer visible. */
                        t.houseID = s.o.houseID;

                        Map.g_mapTileID[position] |= 0x8000;

                        if (s.o.houseID == (byte)CHouse.g_playerHouseID) CTile.Tile_RemoveFogInRadius(CTile.Tile_UnpackTile(position), 1);

                        if (Map.Map_IsPositionUnveiled(position)) t.overlayTileID = 0;

                        Structure_ConnectWall(position, true);
                        Structure_Free(s);

                    }
                    return true;

                case StructureType.STRUCTURE_SLAB_1x1:
                case StructureType.STRUCTURE_SLAB_2x2:
                    {
                        ushort i, result;

                        result = 0;

                        for (i = 0; i < g_table_structure_layoutTileCount[si.layout]; i++)
                        {
                            ushort curPos = (ushort)(position + g_table_structure_layoutTiles[si.layout][i]);
                            Tile t = Map.g_map[curPos];

                            if (Structure_IsValidBuildLocation(curPos, StructureType.STRUCTURE_SLAB_1x1) == 0) continue;

                            t.groundTileID = Sprites.g_builtSlabTileID;
                            t.houseID = s.o.houseID;

                            Map.g_mapTileID[curPos] |= 0x8000;

                            if (s.o.houseID == (byte)CHouse.g_playerHouseID) CTile.Tile_RemoveFogInRadius(CTile.Tile_UnpackTile(curPos), 1);

                            if (Map.Map_IsPositionUnveiled(curPos)) t.overlayTileID = 0;

                            Map.Map_Update(curPos, 0, false);

                            result = 1;
                        }

                        /* XXX -- Dirt hack -- Parts of the 2x2 slab can be outside the building area, so by doing the same loop twice it will build for sure */
                        if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2)
                        {
                            for (i = 0; i < g_table_structure_layoutTileCount[si.layout]; i++)
                            {
                                ushort curPos = (ushort)(position + g_table_structure_layoutTiles[si.layout][i]);
                                Tile t = Map.g_map[curPos];

                                if (Structure_IsValidBuildLocation(curPos, StructureType.STRUCTURE_SLAB_1x1) == 0) continue;

                                t.groundTileID = Sprites.g_builtSlabTileID;
                                t.houseID = s.o.houseID;

                                Map.g_mapTileID[curPos] |= 0x8000;

                                if (s.o.houseID == (byte)CHouse.g_playerHouseID)
                                {
                                    CTile.Tile_RemoveFogInRadius(CTile.Tile_UnpackTile(curPos), 1);
                                    t.overlayTileID = 0;
                                }

                                Map.Map_Update(curPos, 0, false);

                                result = 1;
                            }
                        }

                        if (result == 0) return false;

                        Structure_Free(s);
                    }
                    return true;
            }

            validBuildLocation = Structure_IsValidBuildLocation(position, (StructureType)s.o.type);
            if (validBuildLocation == 0 && s.o.houseID == (byte)CHouse.g_playerHouseID && !CSharpDune.g_debugScenario && CSharpDune.g_validateStrictIfZero == 0) return false;

            /* ENHANCEMENT -- In Dune2, it only removes the fog around the top-left tile of a structure, leaving for big structures the right in the fog. */
            if (!CSharpDune.g_dune2_enhanced && s.o.houseID == (byte)CHouse.g_playerHouseID) CTile.Tile_RemoveFogInRadius(CTile.Tile_UnpackTile(position), 2);

            s.o.seenByHouses |= (byte)(1 << s.o.houseID);
            if (s.o.houseID == (byte)CHouse.g_playerHouseID) s.o.seenByHouses |= 0xFF;

            s.o.flags.isNotOnMap = false;

            s.o.position = CTile.Tile_UnpackTile(position);
            s.o.position.x &= 0xFF00;
            s.o.position.y &= 0xFF00;

            s.rotationSpriteDiff = 0;
            s.o.hitpoints = si.o.hitpoints;
            s.hitpointsMax = si.o.hitpoints;

            /* If the return value is negative, there are tiles without slab. This gives a penalty to the hitpoints. */
            if (validBuildLocation < 0)
            {
                ushort tilesWithoutSlab = (ushort)-validBuildLocation;
                ushort structureTileCount = g_table_structure_layoutTileCount[si.layout];

                s.o.hitpoints -= (ushort)((si.o.hitpoints / 2) * tilesWithoutSlab / structureTileCount);

                s.o.flags.degrades = true;
            }
            else
            {
                /* ENHANCEMENT -- When you build a structure completely on slabs, it should not degrade */
                if (!CSharpDune.g_dune2_enhanced)
                {
                    s.o.flags.degrades = true;
                }
            }

            Script.Script_Reset(s.o.script, Script.g_scriptStructure);

            s.o.script.variables[0] = 0;
            s.o.script.variables[4] = 0;

            /* XXX -- Weird .. if 'position' enters with 0xFFFF it is returned immediately .. how can this ever NOT happen? */
            if (position != 0xFFFF)
            {
                s.o.script.delay = 0;
                Script.Script_Reset(s.o.script, s.o.script.scriptInfo);
                Script.Script_Load(s.o.script, s.o.type);
            }

            {
                ushort i;

                for (i = 0; i < g_table_structure_layoutTileCount[si.layout]; i++)
                {
                    ushort curPos = (ushort)(position + g_table_structure_layoutTiles[si.layout][i]);
                    Unit u;

                    u = CUnit.Unit_Get_ByPackedTile(curPos);

                    CUnit.Unit_Remove(u);

                    /* ENHANCEMENT -- In Dune2, it only removes the fog around the top-left tile of a structure, leaving for big structures the right in the fog. */
                    if (CSharpDune.g_dune2_enhanced && s.o.houseID == (byte)CHouse.g_playerHouseID) CTile.Tile_RemoveFogInRadius(CTile.Tile_UnpackTile(curPos), 2);

                }
            }

            if (s.o.type == (byte)StructureType.STRUCTURE_WINDTRAP)
            {
                House h;

                h = CHouse.House_Get_ByIndex(s.o.houseID);
                h.windtrapCount += 1;
            }

            if (CSharpDune.g_validateStrictIfZero == 0)
            {
                House h;

                h = CHouse.House_Get_ByIndex(s.o.houseID);
                CHouse.House_CalculatePowerAndCredit(h);
            }

            Structure_UpdateMap(s);

            {
                House h;
                h = CHouse.House_Get_ByIndex(s.o.houseID);
                h.structuresBuilt = Structure_GetStructuresBuilt(h);
            }

            return true;
        }

        /*
         * Create a new Structure.
         *
         * @param index The new index of the Structure, or STRUCTURE_INDEX_INVALID to assign one.
         * @param typeID The type of the new Structure.
         * @param houseID The House of the new Structure.
         * @param position The packed position where to place the Structure. If 0xFFFF, the Structure is not placed.
         * @return The new created Structure, or NULL if something failed.
         */
        internal static Structure Structure_Create(ushort index, byte typeID, byte houseID, ushort position)
        {
            StructureInfo si;
            Structure s;

            if (houseID >= (byte)HouseType.HOUSE_MAX) return null;
            if (typeID >= (byte)StructureType.STRUCTURE_MAX) return null;

            si = g_table_structureInfo[typeID];
            s = Structure_Allocate(index, typeID);
            if (s == null) return null;

            s.o.houseID = houseID;
            s.creatorHouseID = houseID;
            s.o.flags.isNotOnMap = true;
            s.o.position.x = 0;
            s.o.position.y = 0;
            s.o.linkedID = 0xFF;
            s.state = (short)(CSharpDune.g_debugScenario ? StructureState.STRUCTURE_STATE_IDLE : StructureState.STRUCTURE_STATE_JUSTBUILT);

            if (typeID == (byte)StructureType.STRUCTURE_TURRET)
            {
                s.rotationSpriteDiff = Sprites.g_iconMap[Sprites.g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_BASE_DEFENSE_TURRET] + 1];
            }
            if (typeID == (byte)StructureType.STRUCTURE_ROCKET_TURRET)
            {
                s.rotationSpriteDiff = Sprites.g_iconMap[Sprites.g_iconMap[(int)IconMapEntries.ICM_ICONGROUP_BASE_ROCKET_TURRET] + 1];
            }

            s.o.hitpoints = si.o.hitpoints;
            s.hitpointsMax = si.o.hitpoints;

            if (houseID == (byte)HouseType.HOUSE_HARKONNEN && typeID == (byte)StructureType.STRUCTURE_LIGHT_VEHICLE)
            {
                s.upgradeLevel = 1;
            }

            /* Check if there is an upgrade available */
            if (si.o.flags.factory)
            {
                s.upgradeTimeLeft = (byte)(Structure_IsUpgradable(s) ? 100 : 0);
            }

            s.objectType = 0xFFFF;

            Structure_BuildObject(s, 0xFFFE);

            s.countDown = 0;

            /* AIs get the full upgrade immediately */
            if (houseID != (byte)CHouse.g_playerHouseID)
            {
                while (true)
                {
                    if (!Structure_IsUpgradable(s)) break;
                    s.upgradeLevel++;
                }
                s.upgradeTimeLeft = 0;
            }

            if (position != 0xFFFF && !Structure_Place(s, position))
            {
                Structure_Free(s);
                return null;
            }

            return s;
        }

        /*
         * Allocate a Structure.
         *
         * @param index The index to use, or STRUCTURE_INDEX_INVALID to find an unused index.
         * @param typeID The type of the new Structure.
         * @return The Structure allocated, or NULL on failure.
         */
        static Structure Structure_Allocate(ushort index, byte type)
        {
            Structure s = null;

            switch ((StructureType)type)
            {
                case StructureType.STRUCTURE_SLAB_1x1:
                    index = (ushort)StructureIndex.STRUCTURE_INDEX_SLAB_1x1;
                    s = Structure_Get_ByIndex(index);
                    break;

                case StructureType.STRUCTURE_SLAB_2x2:
                    index = (ushort)StructureIndex.STRUCTURE_INDEX_SLAB_2x2;
                    s = Structure_Get_ByIndex(index);
                    break;

                case StructureType.STRUCTURE_WALL:
                    index = (ushort)StructureIndex.STRUCTURE_INDEX_WALL;
                    s = Structure_Get_ByIndex(index);
                    break;

                default:
                    if (index == (ushort)StructureIndex.STRUCTURE_INDEX_INVALID)
                    {
                        /* Find the first unused index */
                        for (index = 0; index < (ushort)StructureIndex.STRUCTURE_INDEX_MAX_SOFT; index++)
                        {
                            s = Structure_Get_ByIndex(index);
                            if (!s.o.flags.used) break;
                        }
                        if (index == (ushort)StructureIndex.STRUCTURE_INDEX_MAX_SOFT) return null;
                    }
                    else
                    {
                        s = Structure_Get_ByIndex(index);
                        if (s.o.flags.used) return null;
                    }

                    g_structureFindArray[g_structureFindCount++] = s;
                    break;
            }
            Debug.Assert(s != null);

            /* Initialize the Structure */
            //memset(s, 0, sizeof(Structure));
            s.o.index = index;
            s.o.type = type;
            s.o.linkedID = 0xFF;
            s.o.flags.used = true;
            s.o.flags.allocated = true;
            s.o.script.delay = 0;

            return s;
        }
    }
}
