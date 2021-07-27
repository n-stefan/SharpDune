﻿/* Unit */

using SharpDune.Audio;
using SharpDune.Gui;
using SharpDune.Os;
using SharpDune.Pool;
using System;
using System.Diagnostics;
using static SharpDune.Script.Script;
using static System.Math;

namespace SharpDune
{
    /*
	 * Types of Units available in the game.
	 */
    enum UnitType
	{
		UNIT_CARRYALL = 0,
		UNIT_ORNITHOPTER = 1,
		UNIT_INFANTRY = 2,
		UNIT_TROOPERS = 3,
		UNIT_SOLDIER = 4,
		UNIT_TROOPER = 5,
		UNIT_SABOTEUR = 6,
		UNIT_LAUNCHER = 7,
		UNIT_DEVIATOR = 8,
		UNIT_TANK = 9,
		UNIT_SIEGE_TANK = 10,
		UNIT_DEVASTATOR = 11,
		UNIT_SONIC_TANK = 12,
		UNIT_TRIKE = 13,
		UNIT_RAIDER_TRIKE = 14,
		UNIT_QUAD = 15,
		UNIT_HARVESTER = 16,
		UNIT_MCV = 17,
		UNIT_MISSILE_HOUSE = 18,
		UNIT_MISSILE_ROCKET = 19,
		UNIT_MISSILE_TURRET = 20,
		UNIT_MISSILE_DEVIATOR = 21,
		UNIT_MISSILE_TROOPER = 22,
		UNIT_BULLET = 23,
		UNIT_SONIC_BLAST = 24,
		UNIT_SANDWORM = 25,
		UNIT_FRIGATE = 26,

		UNIT_MAX = 27,
		UNIT_INVALID = 0xFF
	}

	/*
	 * Types of Movements available in the game.
	 */
	enum MovementType
	{
		MOVEMENT_FOOT = 0,
		MOVEMENT_TRACKED = 1,
		MOVEMENT_HARVESTER = 2,
		MOVEMENT_WHEELED = 3,
		MOVEMENT_WINGER = 4,
		MOVEMENT_SLITHER = 5,

		MOVEMENT_MAX = 6,
		MOVEMENT_INVALID = 0xFF
	}

	/*
	 * Types of Actions available in the game.
	 */
	enum ActionType
	{
		ACTION_ATTACK = 0,
		ACTION_MOVE = 1,
		ACTION_RETREAT = 2,
		ACTION_GUARD = 3,
		ACTION_AREA_GUARD = 4,
		ACTION_HARVEST = 5,
		ACTION_RETURN = 6,
		ACTION_STOP = 7,
		ACTION_AMBUSH = 8,
		ACTION_SABOTAGE = 9,
		ACTION_DIE = 10,
		ACTION_HUNT = 11,
		ACTION_DEPLOY = 12,
		ACTION_DESTRUCT = 13,

		ACTION_MAX = 14,
		ACTION_INVALID = 0xFF
	}

	/*
	 * Types of DisplayModes available in the game.
	 */
	enum DisplayMode
	{
		SINGLE_FRAME = 0,
		UNIT = 1,                                    /* Ground: N,NE,E,SE,S.  Air: N,NE,E. */
		ROCKET = 2,                                  /* N,NNE,NE,ENE,E. */
		INFANTRY_3_FRAMES = 3,                       /* N,E,S; 3 frames per direction. */
		INFANTRY_4_FRAMES = 4,                       /* N,E,S; 4 frames per direction. */
		ORNITHOPTER = 5                              /* N,NE,E; 3 frames per direction. */
	}

	/*
	 * Flags used to indicate units in a bitmask.
	 */
	[Flags]
	enum UnitFlag
	{
		FLAG_UNIT_CARRYALL = 1 << UnitType.UNIT_CARRYALL,         /* 0x______01 */
		FLAG_UNIT_ORNITHOPTER = 1 << UnitType.UNIT_ORNITHOPTER,      /* 0x______02 */
		FLAG_UNIT_INFANTRY = 1 << UnitType.UNIT_INFANTRY,         /* 0x______04 */
		FLAG_UNIT_TROOPERS = 1 << UnitType.UNIT_TROOPERS,         /* 0x______08 */
		FLAG_UNIT_SOLDIER = 1 << UnitType.UNIT_SOLDIER,          /* 0x______10 */
		FLAG_UNIT_TROOPER = 1 << UnitType.UNIT_TROOPER,          /* 0x______20 */
		FLAG_UNIT_SABOTEUR = 1 << UnitType.UNIT_SABOTEUR,         /* 0x______40 */
		FLAG_UNIT_LAUNCHER = 1 << UnitType.UNIT_LAUNCHER,         /* 0x______80 */
		FLAG_UNIT_DEVIATOR = 1 << UnitType.UNIT_DEVIATOR,         /* 0x____01__ */
		FLAG_UNIT_TANK = 1 << UnitType.UNIT_TANK,             /* 0x____02__ */
		FLAG_UNIT_SIEGE_TANK = 1 << UnitType.UNIT_SIEGE_TANK,       /* 0x____04__ */
		FLAG_UNIT_DEVASTATOR = 1 << UnitType.UNIT_DEVASTATOR,       /* 0x____08__ */
		FLAG_UNIT_SONIC_TANK = 1 << UnitType.UNIT_SONIC_TANK,       /* 0x____10__ */
		FLAG_UNIT_TRIKE = 1 << UnitType.UNIT_TRIKE,            /* 0x____20__ */
		FLAG_UNIT_RAIDER_TRIKE = 1 << UnitType.UNIT_RAIDER_TRIKE,     /* 0x____40__ */
		FLAG_UNIT_QUAD = 1 << UnitType.UNIT_QUAD,             /* 0x____80__ */
		FLAG_UNIT_HARVESTER = 1 << UnitType.UNIT_HARVESTER,        /* 0x__01____ */
		FLAG_UNIT_MCV = 1 << UnitType.UNIT_MCV,              /* 0x__02____ */
		FLAG_UNIT_MISSILE_HOUSE = 1 << UnitType.UNIT_MISSILE_HOUSE,    /* 0x__04____ */
		FLAG_UNIT_MISSILE_ROCKET = 1 << UnitType.UNIT_MISSILE_ROCKET,   /* 0x__08____ */
		FLAG_UNIT_MISSILE_TURRET = 1 << UnitType.UNIT_MISSILE_TURRET,   /* 0x__10____ */
		FLAG_UNIT_MISSILE_DEVIATOR = 1 << UnitType.UNIT_MISSILE_DEVIATOR, /* 0x__20____ */
		FLAG_UNIT_MISSILE_TROOPER = 1 << UnitType.UNIT_MISSILE_TROOPER,  /* 0x__40____ */
		FLAG_UNIT_BULLET = 1 << UnitType.UNIT_BULLET,           /* 0x__80____ */
		FLAG_UNIT_SONIC_BLAST = 1 << UnitType.UNIT_SONIC_BLAST,      /* 0x01______ */
		FLAG_UNIT_SANDWORM = 1 << UnitType.UNIT_SANDWORM,         /* 0x02______ */
		FLAG_UNIT_FRIGATE = 1 << UnitType.UNIT_FRIGATE,          /* 0x04______ */

		FLAG_UNIT_NONE = 0
	}

	/*
	 * Directional information
	 */
	class dir24
	{
		internal sbyte speed;                                    /*!< Speed of direction change. */
		internal sbyte target;                                   /*!< Target direction. */
		internal sbyte current;                                  /*!< Current direction. */
	}

	/*
	 * A Unit as stored in the memory.
	 */
	class Unit
	{
		internal Object o;                                       /*!< Common to Unit and Structures. */
		internal tile32 currentDestination;                      /*!< Where the Unit is currently going to. */
		internal ushort originEncoded;                           /*!< Encoded index, indicating the origin. */
		internal byte actionID;                                  /*!< Current action. */
		internal byte nextActionID;                              /*!< Next action. */
		internal ushort fireDelay;                               /*!< Delay between firing. In Dune2 this is an uint8. */
		internal ushort distanceToDestination;                   /*!< How much distance between where we are now and where currentDestination is. */
		internal ushort targetAttack;                            /*!< Target to attack (encoded index). */
		internal ushort targetMove;                              /*!< Target to move to (encoded index). */
		internal byte amount;                                    /*!< Meaning depends on type:
																 * - Sandworm : units to eat before disappearing.
																 * - Harvester : harvested spice.
																 */
		internal byte deviated;                                  /*!< Strength of deviation. Zero if unit is not deviated. */
		internal byte deviatedHouse;                             /*!< Which house it is deviated to. Only valid if 'deviated' is non-zero. */
		internal tile32 targetLast;                              /*!< The last position of the Unit. Carry-alls will return the Unit here. */
		internal tile32 targetPreLast;                           /*!< The position before the last position of the Unit. */
		internal dir24[] orientation;                            /*!< Orientation of the unit. [0] = base, [1] = top (turret, etc). */
		internal byte speedPerTick;                              /*!< Every tick this amount is added; if over 255 Unit is moved. */
		internal byte speedRemainder;                            /*!< Remainder of speedPerTick. */
		internal byte speed;                                     /*!< The amount to move when speedPerTick goes over 255. */
		internal byte movingSpeed;                               /*!< The speed of moving as last set. */
		internal byte wobbleIndex;                               /*!< At which wobble index the Unit currently is. */
		internal sbyte spriteOffset;                             /*!< Offset of the current sprite for Unit. */
		internal byte blinkCounter;                              /*!< If non-zero, it indicates how many more ticks this unit is blinking. */
		internal byte team;                                      /*!< If non-zero, unit is part of team. Value 1 means team 0, etc. */
		internal ushort timer;                                   /*!< Timer used in animation, to count down when to do the next step. */
		internal byte[] route = new byte[14];                    /*!< The current route the Unit is following. */

		internal Unit()
		{
			o = new Object();
			currentDestination = new tile32();
			orientation = new dir24[] { new(), new() };
		}
	}

	class UnitInfoFlags
	{
		internal bool isBullet;                         /*!< If true, Unit is a bullet / missile. */
		internal bool explodeOnDeath;                   /*!< If true, Unit exploses when dying. */
		internal bool sonicProtection;                  /*!< If true, Unit receives no damage of a sonic blast. */
		internal bool canWobble;                        /*!< If true, Unit will wobble around while moving on certain tiles. */
		internal bool isTracked;                        /*!< If true, Unit is tracked-based (and leaves marks in sand). */
		internal bool isGroundUnit;                     /*!< If true, Unit is ground-based. */
		internal bool mustStayInMap;                    /*!< Unit cannot leave the map and bounces off the border (air-based units). */
		internal bool firesTwice;                       /*!< If true, Unit fires twice. */
		internal bool impactOnSand;                     /*!< If true, hitting sand (as bullet / missile) makes an impact (crater-like). */
		internal bool isNotDeviatable;                  /*!< If true, Unit can't be deviated. */
		internal bool hasAnimationSet;                  /*!< If true, the Unit has two set of sprites for animation. */
		internal bool notAccurate;                      /*!< If true, Unit is a bullet and is not very accurate at hitting the target (rockets). */
		internal bool isNormalUnit;                     /*!< If true, Unit is a normal unit (not a bullet / missile, nor a sandworm / frigate). */
	}

	/*
	 * Static information per Unit type.
	 */
	class UnitInfo
	{
		internal ObjectInfo o;                                  /*!< Common to UnitInfo and StructureInfo. */
		internal ushort indexStart;                             /*!< At Unit create, between this and indexEnd (including) a free index is picked. */
		internal ushort indexEnd;                               /*!< At Unit create, between indexStart and this (including) a free index is picked. */
		internal UnitInfoFlags flags;                           /*!< General flags of the UnitInfo. */
		internal ushort dimension;                              /*!< The dimension of the Unit Sprite. */
		internal ushort movementType;                           /*!< MovementType of Unit. */
		internal ushort animationSpeed;                         /*!< Speed of sprite animation of Unit. */
		internal ushort movingSpeedFactor;                      /*!< Factor speed of movement of Unit, where 256 is full speed. */
		internal byte turningSpeed;                             /*!< Speed of orientation change of Unit. */
		internal ushort groundSpriteID;                         /*!< SpriteID for north direction. */
		internal ushort turretSpriteID;                         /*!< SpriteID of the turret for north direction. */
		internal ushort actionAI;                               /*!< Default action for AI units. */
		internal ushort displayMode;                            /*!< How to draw the Unit. */
		internal ushort destroyedSpriteID;                      /*!< SpriteID of burning Unit for north direction. Can be zero if no such animation. */
		internal ushort fireDelay;                              /*!< Time between firing at Normal speed. */
		internal ushort fireDistance;                           /*!< Maximal distance this Unit can fire from. */
		internal ushort damage;                                 /*!< Damage this Unit does to other Units. */
		internal ushort explosionType;                          /*!< Type of the explosion of Unit. */
		internal byte bulletType;                               /*!< Type of the bullets of Unit. */
		internal ushort bulletSound;                            /*!< Sound for the bullets. */
	}

	/*
	 * Static information per Action type.
	 */
	class ActionInfo
	{
		internal ushort stringID;                               /*!< StringID of Action name. */
		internal string name;                                   /*!< Name of Action. */
		internal ushort switchType;                             /*!< When going to new mode, how do we handle it? 0: queue if needed, 1: change immediately, 2: run via subroutine. */
		internal ushort selectionType;                          /*!< Selection type attached to this action. */
		internal ushort soundID;                                /*!< The sound played when unit is a Foot unit. */
	}

	class CUnit
	{
		internal static UnitInfo[] g_table_unitInfo;

		static CUnit()
		{
			unchecked
			{
				g_table_unitInfo = new UnitInfo[] { //[UNIT_MAX]
					new() { /* 0 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_CARRYALL,
						name = "Carryall",
						stringID_full = (ushort)Text.STR_ALLPURPOSE_CARRYALL,
						wsa = "carryall.wsa",
						flags = new ObjectInfoFlags {
						hasShadow = true,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = false,
						noMessageOnDeath = false,
						tabSelectable = false,
						scriptNoSlowdown = true,
						targetAir = false,
						priority = true
						},
						spawnChance = 0,
						hitpoints = 100,
						fogUncoverRadius = 0,
						spriteID = 89,
						buildCredits = 800,
						buildTime = 64,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 16,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 20,
						priorityTarget = 16,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
						},
						indexStart = 0,
						indexEnd = 10,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = false,
						sonicProtection = false,
						canWobble = false,
						isTracked = false,
						isGroundUnit = false,
						mustStayInMap = true,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = true,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 32,
						movementType = (ushort)MovementType.MOVEMENT_WINGER,
						animationSpeed = 0,
						movingSpeedFactor = 200,
						turningSpeed = 3,
						groundSpriteID = 283,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_STOP,
						displayMode = (ushort)DisplayMode.UNIT,
						destroyedSpriteID = 0,
						fireDelay = 0,
						fireDistance = 0,
						damage = 0,
						explosionType = (ushort)ExplosionType.EXPLOSION_INVALID,
						bulletType = (byte)UnitType.UNIT_INVALID,
						bulletSound = 42
					},

					new() { /* 1 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_THOPTER,
						name = "'Thopter",
						stringID_full = (ushort)Text.STR_ORNITHIPTER,
						wsa = "orni.wsa",
						flags = new ObjectInfoFlags {
						hasShadow = true,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = false,
						noMessageOnDeath = false,
						tabSelectable = false,
						scriptNoSlowdown = true,
						targetAir = false,
						priority = true
						},
						spawnChance = 0,
						hitpoints = 25,
						fogUncoverRadius = 5,
						spriteID = 97,
						buildCredits = 600,
						buildTime = 96,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_HOUSE_OF_IX,
						sortPriority = 28,
						upgradeLevelRequired = 1,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 75,
						priorityTarget = 30,
						availableHouse = (byte)(HouseFlag.FLAG_HOUSE_MERCENARY | HouseFlag.FLAG_HOUSE_SARDAUKAR | HouseFlag.FLAG_HOUSE_FREMEN | HouseFlag.FLAG_HOUSE_ORDOS | HouseFlag.FLAG_HOUSE_ATREIDES),
						},
						indexStart = 0,
						indexEnd = 10,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = true,
						sonicProtection = false,
						canWobble = false,
						isTracked = false,
						isGroundUnit = false,
						mustStayInMap = true,
						firesTwice = true,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 24,
						movementType = (ushort)MovementType.MOVEMENT_WINGER,
						animationSpeed = 7,
						movingSpeedFactor = 150,
						turningSpeed = 2,
						groundSpriteID = 289,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_STOP,
						displayMode = (ushort)DisplayMode.ORNITHOPTER,
						destroyedSpriteID = 0,
						fireDelay = 50,
						fireDistance = 50,
						damage = 50,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_SMALL,
						bulletType = (byte)UnitType.UNIT_MISSILE_TROOPER,
						bulletSound = 42
					},

					new() { /* 2 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_INFANTRY,
						name = "Infantry",
						stringID_full = (ushort)Text.STR_LIGHT_INFANTRY_SQUAD,
						wsa = "infantry.wsa",
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
						tabSelectable = true,
						scriptNoSlowdown = false,
						targetAir = false,
						priority = true
						},
						spawnChance = 0,
						hitpoints = 50,
						fogUncoverRadius = 1,
						spriteID = 93,
						buildCredits = 100,
						buildTime = 32,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 4,
						upgradeLevelRequired = 1,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETREAT, (ushort)ActionType.ACTION_GUARD },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 20,
						priorityTarget = 20,
						availableHouse = (byte)(HouseFlag.FLAG_HOUSE_MERCENARY | HouseFlag.FLAG_HOUSE_SARDAUKAR | HouseFlag.FLAG_HOUSE_FREMEN | HouseFlag.FLAG_HOUSE_ORDOS | HouseFlag.FLAG_HOUSE_ATREIDES),
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = false,
						sonicProtection = false,
						canWobble = false,
						isTracked = false,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = true,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 16,
						movementType = (ushort)MovementType.MOVEMENT_FOOT,
						animationSpeed = 15,
						movingSpeedFactor = 5,
						turningSpeed = 3,
						groundSpriteID = 329,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_HUNT,
						displayMode = (ushort)DisplayMode.INFANTRY_4_FRAMES,
						destroyedSpriteID = 0,
						fireDelay = 45,
						fireDistance = 2,
						damage = 3,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_SMALL,
						bulletType = (byte)UnitType.UNIT_BULLET,
						bulletSound = 58
					},

					new() { /* 3 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_TROOPERS,
						name = "Troopers",
						stringID_full = (ushort)Text.STR_HEAVY_TROOPER_SQUAD,
						wsa = "hyinfy.wsa",
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
						tabSelectable = true,
						scriptNoSlowdown = false,
						targetAir = true,
						priority = true
						},
						spawnChance = 0,
						hitpoints = 110,
						fogUncoverRadius = 1,
						spriteID = 103,
						buildCredits = 200,
						buildTime = 56,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 8,
						upgradeLevelRequired = 1,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETREAT, (ushort)ActionType.ACTION_GUARD },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 50,
						priorityTarget = 50,
						availableHouse = (byte)(HouseFlag.FLAG_HOUSE_MERCENARY | HouseFlag.FLAG_HOUSE_SARDAUKAR | HouseFlag.FLAG_HOUSE_FREMEN | HouseFlag.FLAG_HOUSE_ORDOS | HouseFlag.FLAG_HOUSE_HARKONNEN),
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = false,
						sonicProtection = false,
						canWobble = false,
						isTracked = false,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = true,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 16,
						movementType = (ushort)MovementType.MOVEMENT_FOOT,
						animationSpeed = 15,
						movingSpeedFactor = 10,
						turningSpeed = 3,
						groundSpriteID = 341,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_HUNT,
						displayMode = (ushort)DisplayMode.INFANTRY_4_FRAMES,
						destroyedSpriteID = 0,
						fireDelay = 50,
						fireDistance = 5,
						damage = 5,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_SMALL,
						bulletType = (byte)UnitType.UNIT_BULLET,
						bulletSound = 59
					},

					new() { /* 4 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_SOLDIER,
						name = "Soldier",
						stringID_full = (ushort)Text.STR_INFANTRY_SOLDIER,
						wsa = "infantry.wsa",
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
						tabSelectable = true,
						scriptNoSlowdown = false,
						targetAir = false,
						priority = true
						},
						spawnChance = 0,
						hitpoints = 20,
						fogUncoverRadius = 1,
						spriteID = 102,
						buildCredits = 60,
						buildTime = 32,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 2,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETREAT, (ushort)ActionType.ACTION_GUARD },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 10,
						priorityTarget = 10,
						availableHouse = (byte)(HouseFlag.FLAG_HOUSE_MERCENARY | HouseFlag.FLAG_HOUSE_SARDAUKAR | HouseFlag.FLAG_HOUSE_FREMEN | HouseFlag.FLAG_HOUSE_ORDOS | HouseFlag.FLAG_HOUSE_ATREIDES),
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = false,
						sonicProtection = false,
						canWobble = true,
						isTracked = false,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 16,
						movementType = (ushort)MovementType.MOVEMENT_FOOT,
						animationSpeed = 12,
						movingSpeedFactor = 8,
						turningSpeed = 3,
						groundSpriteID = 311,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_HUNT,
						displayMode = (ushort)DisplayMode.INFANTRY_3_FRAMES,
						destroyedSpriteID = 0,
						fireDelay = 45,
						fireDistance = 2,
						damage = 3,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_SMALL,
						bulletType = (byte)UnitType.UNIT_BULLET,
						bulletSound = 58
					},

					new() { /* 5 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_TROOPER,
						name = "Trooper",
						stringID_full = (ushort)Text.STR_HEAVY_TROOPER,
						wsa = "hyinfy.wsa",
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
						tabSelectable = true,
						scriptNoSlowdown = false,
						targetAir = true,
						priority = true
						},
						spawnChance = 0,
						hitpoints = 45,
						fogUncoverRadius = 1,
						spriteID = 88,
						buildCredits = 100,
						buildTime = 56,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 6,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETREAT, (ushort)ActionType.ACTION_GUARD },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 20,
						priorityTarget = 30,
						availableHouse = (byte)(HouseFlag.FLAG_HOUSE_MERCENARY | HouseFlag.FLAG_HOUSE_SARDAUKAR | HouseFlag.FLAG_HOUSE_FREMEN | HouseFlag.FLAG_HOUSE_ORDOS | HouseFlag.FLAG_HOUSE_HARKONNEN),
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = false,
						sonicProtection = false,
						canWobble = true,
						isTracked = false,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 16,
						movementType = (ushort)MovementType.MOVEMENT_FOOT,
						animationSpeed = 12,
						movingSpeedFactor = 15,
						turningSpeed = 3,
						groundSpriteID = 320,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_HUNT,
						displayMode = (ushort)DisplayMode.INFANTRY_3_FRAMES,
						destroyedSpriteID = 0,
						fireDelay = 50,
						fireDistance = 5,
						damage = 5,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_SMALL,
						bulletType = (byte)UnitType.UNIT_BULLET,
						bulletSound = 59
					},

					new() { /* 6 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_SABOTEUR,
						name = "Saboteur",
						stringID_full = (ushort)Text.STR_SABOTEUR,
						wsa = "saboture.wsa",
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
						tabSelectable = true,
						scriptNoSlowdown = true,
						targetAir = false,
						priority = true
						},
						spawnChance = 0,
						hitpoints = 10,
						fogUncoverRadius = 1,
						spriteID = 96,
						buildCredits = 120,
						buildTime = 48,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 0,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_SABOTAGE, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETREAT, (ushort)ActionType.ACTION_GUARD },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 0,
						priorityTarget = 700,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ORDOS,
						},
						indexStart = 20,
						indexEnd = 21,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = false,
						sonicProtection = false,
						canWobble = false,
						isTracked = false,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 8,
						movementType = (ushort)MovementType.MOVEMENT_FOOT,
						animationSpeed = 7,
						movingSpeedFactor = 40,
						turningSpeed = 3,
						groundSpriteID = 301,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_SABOTAGE,
						displayMode = (ushort)DisplayMode.INFANTRY_3_FRAMES,
						destroyedSpriteID = 0,
						fireDelay = 45,
						fireDistance = 2,
						damage = 2,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_SMALL,
						bulletType = (byte)UnitType.UNIT_BULLET,
						bulletSound = 58
					},

					new() { /* 7 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_LAUNCHER,
						name = "Launcher",
						stringID_full = (ushort)Text.STR_ROCKET_LAUNCHER,
						wsa = "rtank.wsa",
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = true,
						noMessageOnDeath = false,
						tabSelectable = true,
						scriptNoSlowdown = false,
						targetAir = true,
						priority = true
						},
						spawnChance = 64,
						hitpoints = 100,
						fogUncoverRadius = 5,
						spriteID = 85,
						buildCredits = 450,
						buildTime = 72,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 26,
						upgradeLevelRequired = 2,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETREAT, (ushort)ActionType.ACTION_GUARD },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 100,
						priorityTarget = 150,
						availableHouse = (byte)(HouseFlag.FLAG_HOUSE_MERCENARY | HouseFlag.FLAG_HOUSE_SARDAUKAR | HouseFlag.FLAG_HOUSE_FREMEN | HouseFlag.FLAG_HOUSE_ATREIDES | HouseFlag.FLAG_HOUSE_HARKONNEN),
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = true,
						sonicProtection = false,
						canWobble = false,
						isTracked = true,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = true,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 16,
						movementType = (ushort)MovementType.MOVEMENT_TRACKED,
						animationSpeed = 0,
						movingSpeedFactor = 30,
						turningSpeed = 1,
						groundSpriteID = 111,
						turretSpriteID = 146,
						actionAI = (ushort)ActionType.ACTION_HUNT,
						displayMode = (ushort)DisplayMode.UNIT,
						destroyedSpriteID = 162,
						fireDelay = 120,
						fireDistance = 9,
						damage = 75,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_EXPLODE,
						bulletType = (byte)UnitType.UNIT_MISSILE_ROCKET,
						bulletSound = (ushort)-1
					},

					new() { /* 8 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_DEVIATOR,
						name = "Deviator",
						stringID_full = (ushort)Text.STR_DEVIATOR_LAUNCHER,
						wsa = "ordrtank.wsa",
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = true,
						noMessageOnDeath = false,
						tabSelectable = true,
						scriptNoSlowdown = false,
						targetAir = false,
						priority = true
						},
						spawnChance = 64,
						hitpoints = 120,
						fogUncoverRadius = 5,
						spriteID = 98,
						buildCredits = 750,
						buildTime = 80,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_HOUSE_OF_IX,
						sortPriority = 30,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETREAT, (ushort)ActionType.ACTION_GUARD },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 50,
						priorityTarget = 175,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ORDOS,
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = true,
						sonicProtection = false,
						canWobble = false,
						isTracked = true,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 16,
						movementType = (ushort)MovementType.MOVEMENT_TRACKED,
						animationSpeed = 0,
						movingSpeedFactor = 30,
						turningSpeed = 1,
						groundSpriteID = 111,
						turretSpriteID = 146,
						actionAI = (ushort)ActionType.ACTION_HUNT,
						displayMode = (ushort)DisplayMode.UNIT,
						destroyedSpriteID = 162,
						fireDelay = 180,
						fireDistance = 7,
						damage = 0,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_EXPLODE,
						bulletType = (byte)UnitType.UNIT_MISSILE_DEVIATOR,
						bulletSound = (ushort)-1
					},

					new() { /* 9 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_TANK,
						name = "Tank",
						stringID_full = (ushort)Text.STR_COMBAT_TANK,
						wsa = "ltank.wsa",
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = true,
						conquerable = false,
						canBePickedUp = true,
						noMessageOnDeath = false,
						tabSelectable = true,
						scriptNoSlowdown = false,
						targetAir = false,
						priority = true
						},
						spawnChance = 64,
						hitpoints = 200,
						fogUncoverRadius = 3,
						spriteID = 90,
						buildCredits = 300,
						buildTime = 64,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 22,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETREAT, (ushort)ActionType.ACTION_GUARD },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 80,
						priorityTarget = 100,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = true,
						sonicProtection = false,
						canWobble = false,
						isTracked = true,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 16,
						movementType = (ushort)MovementType.MOVEMENT_TRACKED,
						animationSpeed = 0,
						movingSpeedFactor = 25,
						turningSpeed = 1,
						groundSpriteID = 111,
						turretSpriteID = 116,
						actionAI = (ushort)ActionType.ACTION_HUNT,
						displayMode = (ushort)DisplayMode.UNIT,
						destroyedSpriteID = 162,
						fireDelay = 80,
						fireDistance = 4,
						damage = 25,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_MEDIUM,
						bulletType = (byte)UnitType.UNIT_BULLET,
						bulletSound = 57
					},

					new() { /* 10 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_SIEGE_TANK,
						name = "Siege Tank",
						stringID_full = (ushort)Text.STR_HEAVY_SIEGE_TANK,
						wsa = "htank.wsa",
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = true,
						conquerable = false,
						canBePickedUp = true,
						noMessageOnDeath = false,
						tabSelectable = true,
						scriptNoSlowdown = false,
						targetAir = false,
						priority = true
						},
						spawnChance = 64,
						hitpoints = 300,
						fogUncoverRadius = 4,
						spriteID = 84,
						buildCredits = 600,
						buildTime = 96,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 24,
						upgradeLevelRequired = 3,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETREAT, (ushort)ActionType.ACTION_GUARD },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 130,
						priorityTarget = 150,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = true,
						sonicProtection = false,
						canWobble = false,
						isTracked = true,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = true,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 24,
						movementType = (ushort)MovementType.MOVEMENT_TRACKED,
						animationSpeed = 0,
						movingSpeedFactor = 20,
						turningSpeed = 1,
						groundSpriteID = 121,
						turretSpriteID = 126,
						actionAI = (ushort)ActionType.ACTION_HUNT,
						displayMode = (ushort)DisplayMode.UNIT,
						destroyedSpriteID = 162,
						fireDelay = 90,
						fireDistance = 5,
						damage = 30,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_MEDIUM,
						bulletType = (byte)UnitType.UNIT_BULLET,
						bulletSound = 57
					},

					new() { /* 11 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_DEVASTATOR,
						name = "Devastator",
						stringID_full = (ushort)Text.STR_DEVASTATOR_TANK,
						wsa = "harktank.wsa",
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = true,
						noMessageOnDeath = false,
						tabSelectable = true,
						scriptNoSlowdown = false,
						targetAir = false,
						priority = true
						},
						spawnChance = 64,
						hitpoints = 400,
						fogUncoverRadius = 4,
						spriteID = 87,
						buildCredits = 800,
						buildTime = 104,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_HOUSE_OF_IX,
						sortPriority = 32,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_DESTRUCT, (ushort)ActionType.ACTION_GUARD },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 175,
						priorityTarget = 180,
						availableHouse = (byte)(HouseFlag.FLAG_HOUSE_MERCENARY | HouseFlag.FLAG_HOUSE_SARDAUKAR | HouseFlag.FLAG_HOUSE_FREMEN | HouseFlag.FLAG_HOUSE_HARKONNEN),
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = true,
						sonicProtection = false,
						canWobble = false,
						isTracked = true,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = true,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 24,
						movementType = (ushort)MovementType.MOVEMENT_TRACKED,
						animationSpeed = 0,
						movingSpeedFactor = 10,
						turningSpeed = 1,
						groundSpriteID = 131,
						turretSpriteID = 136,
						actionAI = (ushort)ActionType.ACTION_HUNT,
						displayMode = (ushort)DisplayMode.UNIT,
						destroyedSpriteID = 165,
						fireDelay = 100,
						fireDistance = 5,
						damage = 40,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_MEDIUM,
						bulletType = (byte)UnitType.UNIT_BULLET,
						bulletSound = 57
					},

					new() { /* 12 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_SONIC_TANK,
						name = "Sonic Tank",
						stringID_full = (ushort)Text.STR_SONIC_WAVE_TANK,
						wsa = "stank.wsa",
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = true,
						noMessageOnDeath = false,
						tabSelectable = true,
						scriptNoSlowdown = false,
						targetAir = false,
						priority = true
						},
						spawnChance = 64,
						hitpoints = 110,
						fogUncoverRadius = 4,
						spriteID = 91,
						buildCredits = 600,
						buildTime = 104,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_HOUSE_OF_IX,
						sortPriority = 34,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETREAT, (ushort)ActionType.ACTION_GUARD },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 80,
						priorityTarget = 110,
						availableHouse = (byte)(HouseFlag.FLAG_HOUSE_MERCENARY | HouseFlag.FLAG_HOUSE_SARDAUKAR | HouseFlag.FLAG_HOUSE_FREMEN | HouseFlag.FLAG_HOUSE_ATREIDES),
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = true,
						sonicProtection = true,
						canWobble = false,
						isTracked = true,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 16,
						movementType = (ushort)MovementType.MOVEMENT_TRACKED,
						animationSpeed = 0,
						movingSpeedFactor = 30,
						turningSpeed = 1,
						groundSpriteID = 111,
						turretSpriteID = 141,
						actionAI = (ushort)ActionType.ACTION_HUNT,
						displayMode = (ushort)DisplayMode.UNIT,
						destroyedSpriteID = 162,
						fireDelay = 80,
						fireDistance = 8,
						damage = 60,
						explosionType = (ushort)ExplosionType.EXPLOSION_INVALID,
						bulletType = (byte)UnitType.UNIT_SONIC_BLAST,
						bulletSound = 43
					},

					new() { /* 13 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_TRIKE,
						name = "Trike",
						stringID_full = (ushort)Text.STR_LIGHT_ATTACK_TRIKE,
						wsa = "trike.wsa",
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = true,
						noMessageOnDeath = false,
						tabSelectable = true,
						scriptNoSlowdown = false,
						targetAir = false,
						priority = true
						},
						spawnChance = 64,
						hitpoints = 100,
						fogUncoverRadius = 2,
						spriteID = 92,
						buildCredits = 150,
						buildTime = 40,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 10,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETREAT, (ushort)ActionType.ACTION_GUARD },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 50,
						priorityTarget = 50,
						availableHouse = (byte)(HouseFlag.FLAG_HOUSE_MERCENARY | HouseFlag.FLAG_HOUSE_SARDAUKAR | HouseFlag.FLAG_HOUSE_FREMEN | HouseFlag.FLAG_HOUSE_ATREIDES),
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = true,
						sonicProtection = false,
						canWobble = true,
						isTracked = true,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = true,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 16,
						movementType = (ushort)MovementType.MOVEMENT_WHEELED,
						animationSpeed = 0,
						movingSpeedFactor = 45,
						turningSpeed = 2,
						groundSpriteID = 243,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_HUNT,
						displayMode = (ushort)DisplayMode.UNIT,
						destroyedSpriteID = 0,
						fireDelay = 50,
						fireDistance = 3,
						damage = 5,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_SMALL,
						bulletType = (byte)UnitType.UNIT_BULLET,
						bulletSound = 59
					},

					new() { /* 14 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_RAIDER_TRIKE,
						name = "Raider Trike",
						stringID_full = (ushort)Text.STR_FAST_RAIDER_TRIKE,
						wsa = "otrike.wsa",
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = true,
						noMessageOnDeath = false,
						tabSelectable = true,
						scriptNoSlowdown = false,
						targetAir = false,
						priority = true
						},
						spawnChance = 64,
						hitpoints = 80,
						fogUncoverRadius = 2,
						spriteID = 99,
						buildCredits = 150,
						buildTime = 40,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 12,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETREAT, (ushort)ActionType.ACTION_GUARD },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 55,
						priorityTarget = 60,
						availableHouse = (byte)(HouseFlag.FLAG_HOUSE_MERCENARY | HouseFlag.FLAG_HOUSE_SARDAUKAR | HouseFlag.FLAG_HOUSE_FREMEN | HouseFlag.FLAG_HOUSE_ORDOS),
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = true,
						sonicProtection = false,
						canWobble = true,
						isTracked = true,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = true,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 16,
						movementType = (ushort)MovementType.MOVEMENT_WHEELED,
						animationSpeed = 0,
						movingSpeedFactor = 60,
						turningSpeed = 2,
						groundSpriteID = 243,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_HUNT,
						displayMode = (ushort)DisplayMode.UNIT,
						destroyedSpriteID = 0,
						fireDelay = 50,
						fireDistance = 3,
						damage = 5,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_SMALL,
						bulletType = (byte)UnitType.UNIT_BULLET,
						bulletSound = 59
					},

					new() { /* 15 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_QUAD,
						name = "Quad",
						stringID_full = (ushort)Text.STR_HEAVY_ATTACK_QUAD,
						wsa = "quad.wsa",
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = true,
						noMessageOnDeath = false,
						tabSelectable = true,
						scriptNoSlowdown = false,
						targetAir = false,
						priority = true
						},
						spawnChance = 64,
						hitpoints = 130,
						fogUncoverRadius = 2,
						spriteID = 86,
						buildCredits = 200,
						buildTime = 48,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 14,
						upgradeLevelRequired = 1,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETREAT, (ushort)ActionType.ACTION_GUARD },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 60,
						priorityTarget = 60,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = true,
						sonicProtection = false,
						canWobble = true,
						isTracked = true,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = true,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 16,
						movementType = (ushort)MovementType.MOVEMENT_WHEELED,
						animationSpeed = 0,
						movingSpeedFactor = 40,
						turningSpeed = 2,
						groundSpriteID = 238,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_HUNT,
						displayMode = (ushort)DisplayMode.UNIT,
						destroyedSpriteID = 0,
						fireDelay = 50,
						fireDistance = 3,
						damage = 7,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_SMALL,
						bulletType = (byte)UnitType.UNIT_BULLET,
						bulletSound = 59
					},

					new() { /* 16 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_HARVESTER,
						name = "Harvester",
						stringID_full = (ushort)Text.STR_SPICE_HARVESTER,
						wsa = "harvest.wsa",
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = true,
						noMessageOnDeath = false,
						tabSelectable = true,
						scriptNoSlowdown = true,
						targetAir = false,
						priority = true
						},
						spawnChance = 128,
						hitpoints = 150,
						fogUncoverRadius = 2,
						spriteID = 100,
						buildCredits = 300,
						buildTime = 64,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 18,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_HARVEST, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETURN, (ushort)ActionType.ACTION_STOP },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 10,
						priorityTarget = 150,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = true,
						sonicProtection = false,
						canWobble = false,
						isTracked = true,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 24,
						movementType = (ushort)MovementType.MOVEMENT_HARVESTER,
						animationSpeed = 0,
						movingSpeedFactor = 20,
						turningSpeed = 1,
						groundSpriteID = 248,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_HARVEST,
						displayMode = (ushort)DisplayMode.UNIT,
						destroyedSpriteID = 165,
						fireDelay = 0,
						fireDistance = 0,
						damage = 0,
						explosionType = (ushort)ExplosionType.EXPLOSION_INVALID,
						bulletType = (byte)UnitType.UNIT_INVALID,
						bulletSound = 0
					},

					new() { /* 17 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_MCV,
						name = "MCV",
						stringID_full = (ushort)Text.STR_MOBILE_CONST_VEHICLE,
						wsa = "mcv.wsa",
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = true,
						noMessageOnDeath = false,
						tabSelectable = true,
						scriptNoSlowdown = false,
						targetAir = false,
						priority = true
						},
						spawnChance = 64,
						hitpoints = 150,
						fogUncoverRadius = 2,
						spriteID = 101,
						buildCredits = 900,
						buildTime = 80,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 20,
						upgradeLevelRequired = 1,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_DEPLOY, (ushort)ActionType.ACTION_MOVE, (ushort)ActionType.ACTION_RETREAT, (ushort)ActionType.ACTION_STOP },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 10,
						priorityTarget = 150,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
						},
						indexStart = 22,
						indexEnd = 101,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = true,
						sonicProtection = false,
						canWobble = false,
						isTracked = true,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = false,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = true
						},
						dimension = 24,
						movementType = (ushort)MovementType.MOVEMENT_TRACKED,
						animationSpeed = 0,
						movingSpeedFactor = 20,
						turningSpeed = 1,
						groundSpriteID = 253,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_HUNT,
						displayMode = (ushort)DisplayMode.UNIT,
						destroyedSpriteID = 0,
						fireDelay = 0,
						fireDistance = 0,
						damage = 0,
						explosionType = (ushort)ExplosionType.EXPLOSION_INVALID,
						bulletType = (byte)UnitType.UNIT_INVALID,
						bulletSound = 0
					},

					new() { /* 18 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_NULL,
						name = "Death Hand",
						stringID_full = (ushort)Text.STR_NULL,
						wsa = "gold-bb.wsa",
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = false,
						noMessageOnDeath = true,
						tabSelectable = false,
						scriptNoSlowdown = true,
						targetAir = false,
						priority = false
						},
						spawnChance = 0,
						hitpoints = 70,
						fogUncoverRadius = 0,
						spriteID = 0,
						buildCredits = 0,
						buildTime = 0,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 0,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 0,
						priorityTarget = 0,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_HARKONNEN,
						},
						indexStart = 12,
						indexEnd = 15,
						flags = new UnitInfoFlags {
						isBullet = true,
						explodeOnDeath = false,
						sonicProtection = false,
						canWobble = false,
						isTracked = false,
						isGroundUnit = false,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = true,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = false
						},
						dimension = 32,
						movementType = (ushort)MovementType.MOVEMENT_WINGER,
						animationSpeed = 0,
						movingSpeedFactor = 250,
						turningSpeed = 2,
						groundSpriteID = 278,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_INVALID,
						displayMode = (ushort)DisplayMode.ROCKET,
						destroyedSpriteID = 0,
						fireDelay = 0,
						fireDistance = 15,
						damage = 100,
						explosionType = (ushort)ExplosionType.EXPLOSION_DEATH_HAND,
						bulletType = (byte)UnitType.UNIT_INVALID,
						bulletSound = 42
					},

					new() { /* 19 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_NULL,
						name = "Rocket",
						stringID_full = (ushort)Text.STR_NULL,
						wsa = null,
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = false,
						noMessageOnDeath = true,
						tabSelectable = false,
						scriptNoSlowdown = true,
						targetAir = false,
						priority = false
						},
						spawnChance = 0,
						hitpoints = 70,
						fogUncoverRadius = 0,
						spriteID = 0,
						buildCredits = 0,
						buildTime = 0,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 0,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 0,
						priorityTarget = 0,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
						},
						indexStart = 12,
						indexEnd = 15,
						flags = new UnitInfoFlags {
						isBullet = true,
						explodeOnDeath = false,
						sonicProtection = false,
						canWobble = false,
						isTracked = false,
						isGroundUnit = false,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = true,
						isNotDeviatable = true,
						hasAnimationSet = true,
						notAccurate = true,
						isNormalUnit = false
						},
						dimension = 16,
						movementType = (ushort)MovementType.MOVEMENT_WINGER,
						animationSpeed = 7,
						movingSpeedFactor = 200,
						turningSpeed = 2,
						groundSpriteID = 258,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_INVALID,
						displayMode = (ushort)DisplayMode.ROCKET,
						destroyedSpriteID = 0,
						fireDelay = 0,
						fireDistance = 8,
						damage = 75,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_EXPLODE,
						bulletType = (byte)UnitType.UNIT_INVALID,
						bulletSound = 42
					},

					new() { /* 20 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_NULL,
						name = "ARocket",
						stringID_full = (ushort)Text.STR_NULL,
						wsa = null,
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = false,
						noMessageOnDeath = true,
						tabSelectable = false,
						scriptNoSlowdown = true,
						targetAir = false,
						priority = false
						},
						spawnChance = 0,
						hitpoints = 70,
						fogUncoverRadius = 0,
						spriteID = 0,
						buildCredits = 0,
						buildTime = 0,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 0,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 0,
						priorityTarget = 0,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
						},
						indexStart = 12,
						indexEnd = 15,
						flags = new UnitInfoFlags {
						isBullet = true,
						explodeOnDeath = false,
						sonicProtection = false,
						canWobble = false,
						isTracked = false,
						isGroundUnit = false,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = true,
						isNotDeviatable = true,
						hasAnimationSet = true,
						notAccurate = false,
						isNormalUnit = false
						},
						dimension = 16,
						movementType = (ushort)MovementType.MOVEMENT_WINGER,
						animationSpeed = 7,
						movingSpeedFactor = 160,
						turningSpeed = 8,
						groundSpriteID = 258,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_INVALID,
						displayMode = (ushort)DisplayMode.ROCKET,
						destroyedSpriteID = 0,
						fireDelay = 0,
						fireDistance = 60,
						damage = 75,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_EXPLODE,
						bulletType = (byte)UnitType.UNIT_INVALID,
						bulletSound = 42
					},

					new() { /* 21 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_NULL,
						name = "GRocket",
						stringID_full = (ushort)Text.STR_NULL,
						wsa = null,
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = false,
						noMessageOnDeath = true,
						tabSelectable = false,
						scriptNoSlowdown = true,
						targetAir = false,
						priority = false
						},
						spawnChance = 0,
						hitpoints = 70,
						fogUncoverRadius = 0,
						spriteID = 0,
						buildCredits = 0,
						buildTime = 0,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 0,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 0,
						priorityTarget = 0,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
						},
						indexStart = 12,
						indexEnd = 15,
						flags = new UnitInfoFlags {
						isBullet = true,
						explodeOnDeath = false,
						sonicProtection = false,
						canWobble = false,
						isTracked = false,
						isGroundUnit = false,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = true,
						hasAnimationSet = true,
						notAccurate = true,
						isNormalUnit = false
						},
						dimension = 16,
						movementType = (ushort)MovementType.MOVEMENT_WINGER,
						animationSpeed = 7,
						movingSpeedFactor = 200,
						turningSpeed = 2,
						groundSpriteID = 258,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_INVALID,
						displayMode = (ushort)DisplayMode.ROCKET,
						destroyedSpriteID = 0,
						fireDelay = 0,
						fireDistance = 7,
						damage = 75,
						explosionType = (ushort)ExplosionType.EXPLOSION_DEVIATOR_GAS,
						bulletType = (byte)UnitType.UNIT_INVALID,
						bulletSound = 42
					},

					new() { /* 22 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_NULL,
						name = "MiniRocket",
						stringID_full = (ushort)Text.STR_NULL,
						wsa = null,
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = false,
						noMessageOnDeath = true,
						tabSelectable = false,
						scriptNoSlowdown = true,
						targetAir = false,
						priority = false
						},
						spawnChance = 0,
						hitpoints = 70,
						fogUncoverRadius = 0,
						spriteID = 0,
						buildCredits = 0,
						buildTime = 0,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 0,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 0,
						priorityTarget = 0,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
						},
						indexStart = 12,
						indexEnd = 15,
						flags = new UnitInfoFlags {
						isBullet = true,
						explodeOnDeath = false,
						sonicProtection = false,
						canWobble = false,
						isTracked = false,
						isGroundUnit = false,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = true,
						hasAnimationSet = true,
						notAccurate = false,
						isNormalUnit = false
						},
						dimension = 8,
						movementType = (ushort)MovementType.MOVEMENT_WINGER,
						animationSpeed = 7,
						movingSpeedFactor = 180,
						turningSpeed = 5,
						groundSpriteID = 268,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_INVALID,
						displayMode = (ushort)DisplayMode.ROCKET,
						destroyedSpriteID = 0,
						fireDelay = 0,
						fireDistance = 3,
						damage = 0,
						explosionType = (ushort)ExplosionType.EXPLOSION_MINI_ROCKET,
						bulletType = (byte)UnitType.UNIT_INVALID,
						bulletSound = 64
					},

					new() { /* 23 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_NULL,
						name = "Bullet",
						stringID_full = (ushort)Text.STR_NULL,
						wsa = null,
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = false,
						noMessageOnDeath = true,
						tabSelectable = false,
						scriptNoSlowdown = true,
						targetAir = false,
						priority = false
						},
						spawnChance = 0,
						hitpoints = 1,
						fogUncoverRadius = 0,
						spriteID = 0,
						buildCredits = 0,
						buildTime = 0,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 0,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 0,
						priorityTarget = 0,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
						},
						indexStart = 12,
						indexEnd = 15,
						flags = new UnitInfoFlags {
						isBullet = true,
						explodeOnDeath = true,
						sonicProtection = false,
						canWobble = false,
						isTracked = false,
						isGroundUnit = false,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = true,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = false
						},
						dimension = 8,
						movementType = (ushort)MovementType.MOVEMENT_WINGER,
						animationSpeed = 0,
						movingSpeedFactor = 250,
						turningSpeed = 0,
						groundSpriteID = 174,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_INVALID,
						displayMode = (ushort)DisplayMode.SINGLE_FRAME,
						destroyedSpriteID = 0,
						fireDelay = 0,
						fireDistance = 0,
						damage = 0,
						explosionType = (ushort)ExplosionType.EXPLOSION_IMPACT_SMALL,
						bulletType = (byte)UnitType.UNIT_INVALID,
						bulletSound = (ushort)-1
					},

					new() { /* 24 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_NULL,
						name = "Sonic Blast",
						stringID_full = (ushort)Text.STR_NULL,
						wsa = null,
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = true,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = false,
						noMessageOnDeath = true,
						tabSelectable = false,
						scriptNoSlowdown = true,
						targetAir = false,
						priority = false
						},
						spawnChance = 0,
						hitpoints = 1,
						fogUncoverRadius = 0,
						spriteID = 0,
						buildCredits = 0,
						buildTime = 0,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 0,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 0,
						priorityTarget = 0,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
						},
						indexStart = 12,
						indexEnd = 15,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = false,
						sonicProtection = false,
						canWobble = false,
						isTracked = false,
						isGroundUnit = false,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = true,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = false
						},
						dimension = 32,
						movementType = (ushort)MovementType.MOVEMENT_WINGER,
						animationSpeed = 7,
						movingSpeedFactor = 200,
						turningSpeed = 0,
						groundSpriteID = 160,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_INVALID,
						displayMode = (ushort)DisplayMode.SINGLE_FRAME,
						destroyedSpriteID = 0,
						fireDelay = 0,
						fireDistance = 10,
						damage = 25,
						explosionType = (ushort)ExplosionType.EXPLOSION_INVALID,
						bulletType = (byte)UnitType.UNIT_INVALID,
						bulletSound = (ushort)-1
					},

					new() { /* 25 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_SANDWORM,
						name = "Sandworm",
						stringID_full = (ushort)Text.STR_SANDWORM2,
						wsa = null,
						flags = new ObjectInfoFlags {
						hasShadow = false,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = true,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = false,
						noMessageOnDeath = true,
						tabSelectable = true,
						scriptNoSlowdown = true,
						targetAir = false,
						priority = true
						},
						spawnChance = 0,
						hitpoints = 1000,
						fogUncoverRadius = 0,
						spriteID = 105,
						buildCredits = 0,
						buildTime = 0,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 0,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK, (ushort)ActionType.ACTION_ATTACK },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 0,
						priorityTarget = 0,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_FREMEN,
						},
						indexStart = 16,
						indexEnd = 17,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = false,
						sonicProtection = false,
						canWobble = false,
						isTracked = false,
						isGroundUnit = true,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = true,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = false
						},
						dimension = 24,
						movementType = (ushort)MovementType.MOVEMENT_SLITHER,
						animationSpeed = 0,
						movingSpeedFactor = 35,
						turningSpeed = 3,
						groundSpriteID = 161,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_INVALID,
						displayMode = (ushort)DisplayMode.UNIT,
						destroyedSpriteID = 0,
						fireDelay = 20,
						fireDistance = 0,
						damage = 300,
						explosionType = (ushort)ExplosionType.EXPLOSION_SANDWORM_SWALLOW,
						bulletType = (byte)UnitType.UNIT_SANDWORM,
						bulletSound = 63
					},

					new() { /* 26 */
						o = new ObjectInfo {
						stringID_abbrev = (ushort)Text.STR_NULL,
						name = "Frigate",
						stringID_full = (ushort)Text.STR_NULL,
						wsa = null,
						flags = new ObjectInfoFlags {
						hasShadow = true,
						factory = false,
						notOnConcrete = false,
						busyStateIsIncoming = false,
						blurTile = false,
						hasTurret = false,
						conquerable = false,
						canBePickedUp = false,
						noMessageOnDeath = true,
						tabSelectable = false,
						scriptNoSlowdown = true,
						targetAir = false,
						priority = true
						},
						spawnChance = 0,
						hitpoints = 100,
						fogUncoverRadius = 0,
						spriteID = 0,
						buildCredits = 0,
						buildTime = 0,
						availableCampaign = 0,
						structuresRequired = (uint)StructureFlag.FLAG_STRUCTURE_NONE,
						sortPriority = 0,
						upgradeLevelRequired = 0,
						actionsPlayer = new[] { (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP, (ushort)ActionType.ACTION_STOP },
						available = 0,
						hintStringID = (ushort)Text.STR_NULL,
						priorityBuild = 0,
						priorityTarget = 0,
						availableHouse = (byte)HouseFlag.FLAG_HOUSE_ALL,
						},
						indexStart = 11,
						indexEnd = 11,
						flags = new UnitInfoFlags {
						isBullet = false,
						explodeOnDeath = false,
						sonicProtection = false,
						canWobble = false,
						isTracked = false,
						isGroundUnit = false,
						mustStayInMap = false,
						firesTwice = false,
						impactOnSand = false,
						isNotDeviatable = true,
						hasAnimationSet = false,
						notAccurate = false,
						isNormalUnit = false
						},
						dimension = 32,
						movementType = (ushort)MovementType.MOVEMENT_WINGER,
						animationSpeed = 0,
						movingSpeedFactor = 130,
						turningSpeed = 2,
						groundSpriteID = 298,
						turretSpriteID = (ushort)-1,
						actionAI = (ushort)ActionType.ACTION_INVALID,
						displayMode = (ushort)DisplayMode.UNIT,
						destroyedSpriteID = 0,
						fireDelay = 0,
						fireDistance = 0,
						damage = 0,
						explosionType = (ushort)ExplosionType.EXPLOSION_INVALID,
						bulletType = (byte)UnitType.UNIT_INVALID,
						bulletSound = (ushort)-1
					}
				};
			}
		}

		static uint s_tickUnitMovement; /*!< Indicates next time the Movement function is executed. */
        static uint s_tickUnitRotation; /*!< Indicates next time the Rotation function is executed. */
        static uint s_tickUnitBlinking; /*!< Indicates next time the Blinking function is executed. */
        static uint s_tickUnitUnknown4; /*!< Indicates next time the Unknown4 function is executed. */
        static uint s_tickUnitScript; /*!< Indicates next time the Script function is executed. */
        static uint s_tickUnitUnknown5; /*!< Indicates next time the Unknown5 function is executed. */
        static uint s_tickUnitDeviation; /*!< Indicates next time the Deviation function is executed. */

        internal static Unit g_unitHouseMissile;
		internal static Unit g_unitSelected;

		internal static ushort[] g_table_actionsAI = { (ushort)ActionType.ACTION_HUNT, (ushort)ActionType.ACTION_AREA_GUARD, (ushort)ActionType.ACTION_AMBUSH, (ushort)ActionType.ACTION_GUARD };

		internal static ActionInfo[] g_table_actionInfo = { //[ACTION_MAX]
			new() { /* 0 */
				stringID = (ushort)Text.STR_ATTACK,
				name = "Attack",
				switchType = 0,
				selectionType = (ushort)SelectionType.TARGET,
				soundID = 21
			},

			new() { /* 1 */
				stringID = (ushort)Text.STR_MOVE,
				name = "Move",
				switchType = 0,
				selectionType = (ushort)SelectionType.TARGET,
				soundID = 22
			},

			new() { /* 2 */
				stringID = (ushort)Text.STR_RETREAT,
				name = "Retreat",
				switchType = 0,
				selectionType = (ushort)SelectionType.UNIT,
				soundID = 21
			},

			new() { /* 3 */
				stringID = (ushort)Text.STR_GUARD,
				name = "Guard",
				switchType = 0,
				selectionType = (ushort)SelectionType.UNIT,
				soundID = 21
			},

			new() { /* 4 */
				stringID = (ushort)Text.STR_AREA_GUARD,
				name = "Area Guard",
				switchType = 0,
				selectionType = (ushort)SelectionType.UNIT,
				soundID = 20
			},

			new() { /* 5 */
				stringID = (ushort)Text.STR_HARVEST,
				name = "Harvest",
				switchType = 0,
				selectionType = (ushort)SelectionType.TARGET,
				soundID = 20
			},

			new() { /* 6 */
				stringID = (ushort)Text.STR_RETURN,
				name = "Return",
				switchType = 0,
				selectionType = (ushort)SelectionType.UNIT,
				soundID = 21
			},

			new() { /* 7 */
				stringID = (ushort)Text.STR_STOP2,
				name = "Stop",
				switchType = 0,
				selectionType = (ushort)SelectionType.UNIT,
				soundID = 21
			},

			new() { /* 8 */
				stringID = (ushort)Text.STR_AMBUSH,
				name = "Ambush",
				switchType = 0,
				selectionType = (ushort)SelectionType.UNIT,
				soundID = 20
			},

			new() { /* 9 */
				stringID = (ushort)Text.STR_SABOTAGE,
				name = "Sabotage",
				switchType = 0,
				selectionType = (ushort)SelectionType.UNIT,
				soundID = 20
			},

			new() { /* 10 */
				stringID = (ushort)Text.STR_DIE,
				name = "Die",
				switchType = 1,
				selectionType = (ushort)SelectionType.UNIT,
				soundID = 0xFFFF
			},

			new() { /* 11 */
				stringID = (ushort)Text.STR_HUNT,
				name = "Hunt",
				switchType = 0,
				selectionType = (ushort)SelectionType.UNIT,
				soundID = 20
			},

			new() { /* 12 */
				stringID = (ushort)Text.STR_DEPLOY,
				name = "Deploy",
				switchType = 0,
				selectionType = (ushort)SelectionType.UNIT,
				soundID = 20
			},

			new() { /* 13 */
				stringID = (ushort)Text.STR_DESTRUCT,
				name = "Destruct",
				switchType = 1,
				selectionType = (ushort)SelectionType.UNIT,
				soundID = 20
			}
		};

		internal static Unit g_unitActive;

        internal static ushort g_dirtyUnitCount;
		internal static ushort g_dirtyAirUnitCount;

		/*
		 * Number of units of each type available at the starport.
		 * \c 0 means not available, \c -1 means \c 0 units, \c >0 means that number of units available.
		 */
		internal static short[] g_starportAvailable = new short[(int)UnitType.UNIT_MAX];

		/*
		 * Get the unit on the given packed tile.
		 *
		 * @param packed The packed tile to get the unit from.
		 * @return The unit.
		 */
		internal static Unit Unit_Get_ByPackedTile(ushort packed)
		{
			Tile tile;

			if (CTile.Tile_IsOutOfMap(packed)) return null;

			tile = Map.g_map[packed];
			if (!tile.hasUnit) return null;
			return PoolUnit.Unit_Get_ByIndex((ushort)(tile.index - 1));
		}

		/*
		 * Selects the given unit.
		 *
		 * @param unit The Unit to select.
		 */
		internal static void Unit_Select(Unit unit)
		{
			if (unit == g_unitSelected) return;

			if (unit != null && !unit.o.flags.allocated && !CSharpDune.g_debugGame)
			{
				unit = null;
			}

			if (unit != null && (unit.o.seenByHouses & (1 << (int)CHouse.g_playerHouseID)) == 0 && !CSharpDune.g_debugGame)
			{
				unit = null;
			}

			if (g_unitSelected != null) Unit_UpdateMap(2, g_unitSelected);

			if (unit == null)
			{
				g_unitSelected = null;

				Gui.Gui.GUI_ChangeSelectionType((ushort)SelectionType.STRUCTURE);
				return;
			}

			if (Unit_GetHouseID(unit) == (byte)CHouse.g_playerHouseID)
			{
				UnitInfo ui;

				ui = g_table_unitInfo[unit.o.type];

				/* Plays the 'reporting' sound file. */
				Sound.Sound_StartSound((ushort)(ui.movementType == (ushort)MovementType.MOVEMENT_FOOT ? 18 : 19));

				Gui.Gui.GUI_DisplayHint(ui.o.hintStringID, ui.o.spriteID);
			}

			if (g_unitSelected != null)
			{
				if (g_unitSelected != unit) Unit_DisplayStatusText(unit);

				g_unitSelected = unit;

				WidgetDraw.GUI_Widget_ActionPanel_Draw(true);
			}
			else
			{
				Unit_DisplayStatusText(unit);
				g_unitSelected = unit;

				Gui.Gui.GUI_ChangeSelectionType((ushort)SelectionType.UNIT);
			}

			Unit_UpdateMap(2, g_unitSelected);

			Map.Map_SetSelectionObjectPosition(0xFFFF);
		}

		/*
		 * Update the map around the Unit depending on the type (entering tile, leaving, staying).
		 * @param type The type of action on the map.
		 * @param unit The Unit doing the action.
		 */
		internal static void Unit_UpdateMap(ushort type, Unit unit)
		{
			UnitInfo ui;
			tile32 position;
			ushort packed;
			Tile t;
			ushort radius;

			if (unit == null || unit.o.flags.isNotOnMap || !unit.o.flags.used) return;

			ui = g_table_unitInfo[unit.o.type];

			if (ui.movementType == (ushort)MovementType.MOVEMENT_WINGER)
			{
				if (type != 0)
				{
					unit.o.flags.isDirty = true;
					g_dirtyAirUnitCount++;
				}

				Map.Map_UpdateAround(g_table_unitInfo[unit.o.type].dimension, unit.o.position, unit, Map.g_functions[0][type]);
				return;
			}

			position = unit.o.position;
			packed = CTile.Tile_PackTile(position);
			t = Map.g_map[packed];

			if (t.isUnveiled || unit.o.houseID == (byte)CHouse.g_playerHouseID)
			{
				Unit_HouseUnitCount_Add(unit, (byte)CHouse.g_playerHouseID);
			}
			else
			{
				Unit_HouseUnitCount_Remove(unit);
			}

			if (type == 1)
			{
				if (CHouse.House_AreAllied(Unit_GetHouseID(unit), (byte)CHouse.g_playerHouseID) && !Map.Map_IsPositionUnveiled(packed) && unit.o.type != (byte)UnitType.UNIT_SANDWORM)
				{
					CTile.Tile_RemoveFogInRadius(position, 1);
				}

				if (CObject.Object_GetByPackedTile(packed) == null)
				{
					t.index = (ushort)(unit.o.index + 1);
					t.hasUnit = true;
				}
			}

			if (type != 0)
			{
				unit.o.flags.isDirty = true;
				g_dirtyUnitCount++;
			}

			radius = (ushort)(ui.dimension + 3);

			if (unit.o.flags.bulletIsBig || unit.o.flags.isSmoking || (unit.o.type == (byte)UnitType.UNIT_HARVESTER && unit.actionID == (byte)ActionType.ACTION_HARVEST)) radius = 33;

			Map.Map_UpdateAround(radius, position, unit, Map.g_functions[1][type]);

			if (unit.o.type != (byte)UnitType.UNIT_HARVESTER) return;

			/* The harvester is the only 2x1 unit, so also update tiles in behind us. */
			Map.Map_UpdateAround(radius, unit.targetPreLast, unit, Map.g_functions[1][type]);
			Map.Map_UpdateAround(radius, unit.targetLast, unit, Map.g_functions[1][type]);
		}

		/*
		 * Get the HouseID of a unit. This is not always u->o.houseID, as a unit can be
		 *  deviated by the Ordos.
		 *
		 * @param u Unit to get the HouseID of.
		 * @return The HouseID of the unit, which might be deviated.
		 */
		internal static byte Unit_GetHouseID(Unit u)
		{
			if (u.deviated != 0)
			{
				/* ENHANCEMENT -- Deviated units always belong to Ordos, no matter who did the deviating. */
				if (CSharpDune.g_dune2_enhanced) return u.deviatedHouse;
				return (byte)HouseType.HOUSE_ORDOS;
			}
			return u.o.houseID;
		}

		/*
		 * Display status text for the given unit.
		 *
		 * @param unit The Unit to display status text for.
		 */
		internal static void Unit_DisplayStatusText(Unit unit)
		{
			UnitInfo ui;
			string buffer; //char[81]

			if (unit == null) return;

			ui = g_table_unitInfo[unit.o.type];

			if (unit.o.type == (byte)UnitType.UNIT_SANDWORM)
			{
				buffer = CStrings.String_Get_ByIndex(ui.o.stringID_abbrev); //snprintf(buffer, sizeof(buffer), "%s", String_Get_ByIndex(ui->o.stringID_abbrev));
			}
			else
			{
				var houseName = CHouse.g_table_houseInfo[Unit_GetHouseID(unit)].name;
				if (Config.g_config.language == (byte)Language.FRENCH)
				{
					buffer = $"{CStrings.String_Get_ByIndex(ui.o.stringID_abbrev)} {houseName}"; //snprintf(buffer, sizeof(buffer), "%s %s", String_Get_ByIndex(ui->o.stringID_abbrev), houseName);
				}
				else
				{
					buffer = $"{houseName} {CStrings.String_Get_ByIndex(ui.o.stringID_abbrev)}"; //snprintf(buffer, sizeof(buffer), "%s %s", houseName, String_Get_ByIndex(ui->o.stringID_abbrev));
				}
			}

			if (unit.o.type == (byte)UnitType.UNIT_HARVESTER)
			{
				ushort stringID;

				stringID = (ushort)Text.STR_IS_D_PERCENT_FULL;

				if (unit.actionID == (byte)ActionType.ACTION_HARVEST && unit.amount < 100)
				{
					var type = Map.Map_GetLandscapeType(CTile.Tile_PackTile(unit.o.position));

					if (type == (ushort)LandscapeType.LST_SPICE || type == (ushort)LandscapeType.LST_THICK_SPICE) stringID = (ushort)Text.STR_IS_D_PERCENT_FULL_AND_HARVESTING;
				}

				if (unit.actionID == (byte)ActionType.ACTION_MOVE && Tools.Tools_Index_GetStructure(unit.targetMove) != null)
				{
					stringID = (ushort)Text.STR_IS_D_PERCENT_FULL_AND_HEADING_BACK;
				}
				else
				{
					if (unit.o.script.variables[4] != 0)
					{
						stringID = (ushort)Text.STR_IS_D_PERCENT_FULL_AND_AWAITING_PICKUP;
					}
				}

				if (unit.amount == 0) stringID += 4;

				{
					//size_t len = strlen(buffer);
					//char* s = buffer + len;

					buffer = $"{buffer}{string.Format(CSharpDune.Culture, CStrings.String_Get_ByIndex(stringID), unit.amount)}"; //snprintf(s, sizeof(buffer) - len, String_Get_ByIndex(stringID), unit->amount);
				}
			}

			/* add a dot "." at the end of the buffer */
			//size_t len = strlen(buffer);
			//if (len < sizeof(buffer) - 1) {
			buffer = $"{buffer}.";
			//buffer[len + 1] = '\0';
			//}
			Gui.Gui.GUI_DisplayText(buffer, 2);
		}

		/*
		 * Removes the Unit from the given packed tile.
		 *
		 * @param unit The Unit to remove.
		 * @param packed The packed tile.
		 */
		internal static void Unit_RemoveFromTile(Unit unit, ushort packed)
		{
			var t = Map.g_map[packed];

			if (t.hasUnit && Unit_Get_ByPackedTile(packed) == unit && (packed != CTile.Tile_PackTile(unit.currentDestination) || unit.o.flags.bulletIsBig))
			{
				t.index = 0;
				t.hasUnit = false;
			}

			Map.Map_MarkTileDirty(packed);

			Map.Map_Update(packed, 0, false);
		}

		internal static void Unit_AddToTile(Unit unit, ushort packed)
		{
			Map.Map_UnveilTile(packed, Unit_GetHouseID(unit));
			Map.Map_MarkTileDirty(packed);
			Map.Map_Update(packed, 1, false);
		}

		/*
		 * This unit is about to appear on the map. So add it from the house
		 *  statistics about allies/enemies, and do some other logic.
		 * @param unit The unit to add.
		 * @param houseID The house registering the add.
		 */
		internal static void Unit_HouseUnitCount_Add(Unit unit, byte houseID)
		{
			UnitInfo ui;
			ushort houseIDBit;
			House hp;
			House h;

			if (unit == null) return;

			hp = PoolHouse.House_Get_ByIndex((byte)CHouse.g_playerHouseID);
			ui = g_table_unitInfo[unit.o.type];
			h = PoolHouse.House_Get_ByIndex(houseID);
			houseIDBit = (ushort)(1 << houseID);

			if (houseID == (byte)HouseType.HOUSE_ATREIDES && unit.o.type != (byte)UnitType.UNIT_SANDWORM)
			{
				houseIDBit |= 1 << (ushort)HouseType.HOUSE_FREMEN;
			}

			if ((unit.o.seenByHouses & houseIDBit) != 0 && h.flags.isAIActive)
			{
				unit.o.seenByHouses |= (byte)houseIDBit;
				return;
			}

			if (!ui.flags.isNormalUnit && unit.o.type != (byte)UnitType.UNIT_SANDWORM)
			{
				return;
			}

			if ((unit.o.seenByHouses & houseIDBit) == 0)
			{
				if (CHouse.House_AreAllied(houseID, Unit_GetHouseID(unit)))
				{
					h.unitCountAllied++;
				}
				else
				{
					h.unitCountEnemy++;
				}
			}

			if (ui.movementType != (ushort)MovementType.MOVEMENT_WINGER)
			{
				if (!CHouse.House_AreAllied(houseID, Unit_GetHouseID(unit)))
				{
					h.flags.isAIActive = true;
					var t = PoolHouse.House_Get_ByIndex(Unit_GetHouseID(unit));
					t.flags.isAIActive = true;
				}
			}

			if (houseID == (byte)CHouse.g_playerHouseID && CSharpDune.g_selectionType != (ushort)SelectionType.MENTAT)
			{
				if (unit.o.type == (byte)UnitType.UNIT_SANDWORM)
				{
					if (hp.timerSandwormAttack == 0)
					{
						if (CSharpDune.g_musicInBattle == 0) CSharpDune.g_musicInBattle = 1;

						Sound.Sound_Output_Feedback(37);

						if (Config.g_config.language == (byte)Language.ENGLISH)
						{
							Gui.Gui.GUI_DisplayHint((ushort)Text.STR_WARNING_SANDWORMS_SHAIHULUD_ROAM_DUNE_DEVOURING_ANYTHING_ON_THE_SAND, 105);
						}

						hp.timerSandwormAttack = 8;
					}
				}
				else if (!CHouse.House_AreAllied((byte)CHouse.g_playerHouseID, Unit_GetHouseID(unit)))
				{
					Team t;

					if (hp.timerUnitAttack == 0)
					{
						if (CSharpDune.g_musicInBattle == 0) CSharpDune.g_musicInBattle = 1;

						if (unit.o.type == (byte)UnitType.UNIT_SABOTEUR)
						{
							Sound.Sound_Output_Feedback(12);
						}
						else
						{
							if (CSharpDune.g_scenarioID < 3)
							{
								var find = new PoolFindStruct();
								Structure s;
								ushort feedbackID;

								find.houseID = (byte)CHouse.g_playerHouseID;
								find.index = 0xFFFF;
								find.type = (ushort)StructureType.STRUCTURE_CONSTRUCTION_YARD;

								s = PoolStructure.Structure_Find(find);
								if (s != null)
								{
									feedbackID = (ushort)(((CTile.Orientation_Orientation256ToOrientation8((byte)CTile.Tile_GetDirection(s.o.position, unit.o.position)) + 1) & 7) / 2 + 2);
								}
								else
								{
									feedbackID = 1;
								}

								Sound.Sound_Output_Feedback(feedbackID);
							}
							else
							{
								Sound.Sound_Output_Feedback((ushort)(unit.o.houseID + 6));
							}
						}

						hp.timerUnitAttack = 8;
					}

					t = PoolTeam.Team_Get_ByIndex(unit.team);
					if (t != null) t.script.variables[4] = 1;
				}
			}

			if (!CHouse.House_AreAllied(houseID, unit.o.houseID) && unit.actionID == (byte)ActionType.ACTION_AMBUSH) Unit_SetAction(unit, ActionType.ACTION_HUNT);

			if (unit.o.houseID == (byte)CHouse.g_playerHouseID || (unit.o.houseID == (byte)HouseType.HOUSE_FREMEN && CHouse.g_playerHouseID == HouseType.HOUSE_ATREIDES))
			{
				unit.o.seenByHouses = 0xFF;
			}
			else
			{
				unit.o.seenByHouses |= (byte)houseIDBit;
			}
		}

		/*
		 * This unit is about to disapear from the map. So remove it from the house
		 *  statistics about allies/enemies.
		 * @param unit The unit to remove.
		 */
		internal static void Unit_HouseUnitCount_Remove(Unit unit)
		{
			var find = new PoolFindStruct();

			if (unit == null) return;

			if (unit.o.seenByHouses == 0) return;

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			while (true)
			{
				House h;

				h = PoolHouse.House_Find(find);
				if (h == null) break;

				if ((unit.o.seenByHouses & (1 << h.index)) == 0) continue;

				if (!CHouse.House_AreAllied(h.index, Unit_GetHouseID(unit)))
				{
					h.unitCountEnemy--;
				}
				else
				{
					h.unitCountAllied--;
				}

				unit.o.seenByHouses &= (byte)~(1 << h.index);
			}

			if (CSharpDune.g_dune2_enhanced) unit.o.seenByHouses = 0;
		}

		/*
		 * Sets the action the given unit will execute.
		 *
		 * @param u The Unit to set the action for.
		 * @param action The action.
		 */
		internal static void Unit_SetAction(Unit u, ActionType action)
		{
			ActionInfo ai;

			if (u == null) return;

			if (u.actionID == (byte)ActionType.ACTION_DESTRUCT || u.actionID == (byte)ActionType.ACTION_DIE || action == ActionType.ACTION_INVALID) return;

			ai = g_table_actionInfo[(int)action];

			switch (ai.switchType)
			{
				case 0:
				case 1:
					if (ai.switchType == 0)
					{
						if (u.currentDestination.x != 0 || u.currentDestination.y != 0)
						{
							u.nextActionID = (byte)action;
							return;
						}
					}
					u.actionID = (byte)action;
					u.nextActionID = (byte)ActionType.ACTION_INVALID;
					u.currentDestination.x = 0;
					u.currentDestination.y = 0;
					u.o.script.delay = 0;
                    Script_Reset(u.o.script, g_scriptUnit);
					u.o.script.variables[0] = (ushort)action;
                    Script_Load(u.o.script, u.o.type);
					return;

				case 2:
					u.o.script.variables[0] = (ushort)action;
                    Script_LoadAsSubroutine(u.o.script, u.o.type);
					return;

				default: return;
			}
		}

		/*
		 * Find the best target, based on the score. Only considers units on sand.
		 *
		 * @param unit The unit to search a target for.
		 * @return A target Unit, or NULL if none is found.
		 */
		internal static Unit Unit_Sandworm_FindBestTarget(Unit unit)
		{
			Unit best = null;
			var find = new PoolFindStruct();
			ushort bestPriority = 0;

			if (unit == null) return null;

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.type = 0xFFFF;
			find.index = 0xFFFF;

			while (true)
			{
				Unit u;
				ushort priority;

				u = PoolUnit.Unit_Find(find);

				if (u == null) break;

				priority = Unit_Sandworm_GetTargetPriority(unit, u);

				if (priority >= bestPriority)
				{
					best = u;
					bestPriority = priority;
				}
			}

			if (bestPriority == 0) return null;

			return best;
		}

		/*
		 * Get the priority for a target. Various of things have influence on this score,
		 *  most noticeable the movementType of the target, his distance to you, and
		 *  if he is moving/firing.
		 * @note It only considers units on sand.
		 *
		 * @param unit The Unit that is requesting the score.
		 * @param target The Unit that is being targeted.
		 * @return The priority of the target.
		 */
		static ushort Unit_Sandworm_GetTargetPriority(Unit unit, Unit target)
		{
			ushort res;
			ushort distance;

			if (unit == null || target == null) return 0;
			if (!Map.Map_IsPositionUnveiled(CTile.Tile_PackTile(target.o.position))) return 0;
			if (!Map.g_table_landscapeInfo[Map.Map_GetLandscapeType(CTile.Tile_PackTile(target.o.position))].isSand) return 0;

			switch ((MovementType)g_table_unitInfo[target.o.type].movementType)
			{
				case MovementType.MOVEMENT_FOOT: res = 0x64; break;
				case MovementType.MOVEMENT_TRACKED: res = 0x3E8; break;
				case MovementType.MOVEMENT_HARVESTER: res = 0x3E8; break;
				case MovementType.MOVEMENT_WHEELED: res = 0x1388; break;
				default: res = 0; break;
			}

			if (target.speed != 0 || target.fireDelay != 0) res *= 4;

			distance = CTile.Tile_GetDistanceRoundedUp(unit.o.position, target.o.position);

			if (distance != 0 && res != 0) res /= distance;
			if (distance < 2) res *= 2;

			return res;
		}

		/*
		 * Initiate the first movement of a Unit when the pathfinder has found a route.
		 *
		 * @param unit The Unit to operate on.
		 * @return True if movement was initiated (not blocked etc).
		 */
		internal static bool Unit_StartMovement(Unit unit)
		{
			UnitInfo ui;
			sbyte orientation;
			ushort packed;
			ushort type;
			tile32 position;
			ushort speed;
			short score;

			if (unit == null) return false;

			ui = g_table_unitInfo[unit.o.type];

			orientation = (sbyte)((unit.orientation[0].current + 16) & 0xE0);

			Unit_SetOrientation(unit, orientation, true, 0);
			Unit_SetOrientation(unit, orientation, false, 1);

			position = CTile.Tile_MoveByOrientation(unit.o.position, (byte)orientation);

			packed = CTile.Tile_PackTile(position);

			unit.distanceToDestination = 0x7FFF;

			score = Unit_GetTileEnterScore(unit, packed, (ushort)(orientation / 32));

			if (score > 255 || score == -1) return false;

			type = Map.Map_GetLandscapeType(packed);
			if (type == (ushort)LandscapeType.LST_STRUCTURE) type = (ushort)LandscapeType.LST_CONCRETE_SLAB;

			speed = Map.g_table_landscapeInfo[type].movementSpeed[ui.movementType];

			if (unit.o.type == (byte)UnitType.UNIT_SABOTEUR && type == (ushort)LandscapeType.LST_WALL) speed = 255;
			unit.o.flags.isSmoking = false;

			/* ENHANCEMENT -- the flag is never set to false in original Dune2; in result, once the wobbling starts, it never stops. */
			if (CSharpDune.g_dune2_enhanced)
			{
				unit.o.flags.isWobbling = Map.g_table_landscapeInfo[type].letUnitWobble;
			}
			else
			{
				if (Map.g_table_landscapeInfo[type].letUnitWobble) unit.o.flags.isWobbling = true;
			}

			if ((ui.o.hitpoints / 2) > unit.o.hitpoints && ui.movementType != (ushort)MovementType.MOVEMENT_WINGER) speed -= (ushort)(speed / 4);

			Unit_SetSpeed(unit, speed);

			if (ui.movementType != (ushort)MovementType.MOVEMENT_SLITHER)
			{
				tile32 positionOld;

				positionOld = unit.o.position;
				unit.o.position = position;

				Unit_UpdateMap(1, unit);

				unit.o.position = positionOld;
			}

			unit.currentDestination = position;

			Unit_Deviation_Decrease(unit, 10);

			return true;
		}

		/*
		 * Get the score of entering this tile from a direction.
		 *
		 * @param unit The Unit to operate on.
		 * @param packed The packed tile.
		 * @param direction The direction entering this tile from.
		 * @return 256 if tile is not accessable, -1 when it is an accessable structure,
		 *   or a score to enter the tile otherwise.
		 */
		internal static short Unit_GetTileEnterScore(Unit unit, ushort packed, ushort orient8)
		{
			UnitInfo ui;
			Unit u;
			Structure s;
			ushort type;
			ushort res;

			if (unit == null) return 0;

			ui = g_table_unitInfo[unit.o.type];

			if (!Map.Map_IsValidPosition(packed) && ui.movementType != (ushort)MovementType.MOVEMENT_WINGER) return 256;

			u = Unit_Get_ByPackedTile(packed);
			if (u != null && u != unit && unit.o.type != (byte)UnitType.UNIT_SANDWORM)
			{
				if (unit.o.type == (byte)UnitType.UNIT_SABOTEUR && unit.targetMove == Tools.Tools_Index_Encode(u.o.index, IndexType.IT_UNIT)) return 0;

				if (CHouse.House_AreAllied(Unit_GetHouseID(u), Unit_GetHouseID(unit))) return 256;
				if (g_table_unitInfo[u.o.type].movementType != (ushort)MovementType.MOVEMENT_FOOT || (ui.movementType != (ushort)MovementType.MOVEMENT_TRACKED && ui.movementType != (ushort)MovementType.MOVEMENT_HARVESTER)) return 256;
			}

			s = CStructure.Structure_Get_ByPackedTile(packed);
			if (s != null)
			{
				res = Unit_IsValidMovementIntoStructure(unit, s);
				if (res == 0) return 256;
				return (short)-res;
			}

			type = Map.Map_GetLandscapeType(packed);

			if (CSharpDune.g_dune2_enhanced)
			{
				res = (ushort)(Map.g_table_landscapeInfo[type].movementSpeed[ui.movementType] * ui.movingSpeedFactor / 256);
			}
			else
			{
				res = Map.g_table_landscapeInfo[type].movementSpeed[ui.movementType];
			}

			if (unit.o.type == (byte)UnitType.UNIT_SABOTEUR && type == (ushort)LandscapeType.LST_WALL)
			{
				if (!CHouse.House_AreAllied(Map.g_map[packed].houseID, Unit_GetHouseID(unit))) res = 255;
			}

			if (res == 0) return 256;

			/* Check if the unit is travelling diagonally. */
			if ((orient8 & 1) != 0)
			{
				res -= (ushort)(res / 4 + res / 8);
			}

			/* 'Invert' the speed to get a rough estimate of the time taken. */
			res ^= 0xFF;

			return (short)res;
		}

		/*
		 * ?? Sorts unit array and count enemy/allied units.
		 */
		internal static void Unit_Sort()
		{
			House h;
			ushort i;

			h = CHouse.g_playerHouse;
			h.unitCountEnemy = 0;
			h.unitCountAllied = 0;

			for (i = 0; i < PoolUnit.g_unitFindCount - 1; i++)
			{
				Unit u1;
				Unit u2;
				ushort y1;
				ushort y2;

				u1 = PoolUnit.g_unitFindArray[i];
				u2 = PoolUnit.g_unitFindArray[i + 1];
				y1 = CTile.Tile_GetY(u1.o.position);
				y2 = CTile.Tile_GetY(u2.o.position);
				if (g_table_unitInfo[u1.o.type].movementType == (ushort)MovementType.MOVEMENT_FOOT) y1 -= 0x100;
				if (g_table_unitInfo[u2.o.type].movementType == (ushort)MovementType.MOVEMENT_FOOT) y2 -= 0x100;

				if ((short)y1 > (short)y2)
				{
					PoolUnit.g_unitFindArray[i] = u2;
					PoolUnit.g_unitFindArray[i + 1] = u1;
				}
			}

			for (i = 0; i < PoolUnit.g_unitFindCount; i++)
			{
				Unit u;

				u = PoolUnit.g_unitFindArray[i];
				if ((u.o.seenByHouses & (1 << (byte)CHouse.g_playerHouseID)) != 0 && !u.o.flags.isNotOnMap)
				{
					if (CHouse.House_AreAllied(u.o.houseID, (byte)CHouse.g_playerHouseID))
					{
						h.unitCountAllied++;
					}
					else
					{
						h.unitCountEnemy++;
					}
				}
			}
		}

		/*
		 * Remove the Unit from the game, doing all required administration for it, like
		 *  deselecting it, remove it from the radar count, stopping scripts, ..
		 *
		 * @param u The Unit to remove.
		 */
		internal static void Unit_Remove(Unit u)
		{
			if (u == null) return;

			u.o.flags.allocated = true;
			Unit_UntargetMe(u);

			if (u == g_unitSelected) Unit_Select(null);

			u.o.flags.bulletIsBig = true;
			Unit_UpdateMap(0, u);

			Unit_HouseUnitCount_Remove(u);

            Script_Reset(u.o.script, g_scriptUnit);

			PoolUnit.Unit_Free(u);
		}

		/*
		 * Gets the team of the given unit.
		 *
		 * @param u The unit to get the team of.
		 * @return The team.
		 */
		internal static Team Unit_GetTeam(Unit u)
		{
			if (u == null) return null;
			if (u.team == 0) return null;
			return PoolTeam.Team_Get_ByIndex((ushort)(u.team - 1));
		}

		/*
		 * Untarget the given Unit.
		 *
		 * @param unit The Unit to untarget.
		 */
		static void Unit_UntargetMe(Unit unit)
		{
			var find = new PoolFindStruct();
			var encoded = Tools.Tools_Index_Encode(unit.o.index, IndexType.IT_UNIT);

			CObject.Object_Script_Variable4_Clear(unit.o);

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.type = 0xFFFF;
			find.index = 0xFFFF;

			while (true)
			{
				Unit u;

				u = PoolUnit.Unit_Find(find);
				if (u == null) break;

				if (u.targetMove == encoded) u.targetMove = 0;
				if (u.targetAttack == encoded) u.targetAttack = 0;
				if (u.o.script.variables[4] == encoded) CObject.Object_Script_Variable4_Clear(u.o);
			}

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.type = 0xFFFF;
			find.index = 0xFFFF;

			while (true)
			{
				Structure s;

				s = PoolStructure.Structure_Find(find);
				if (s == null) break;

				if (s.o.type != (byte)StructureType.STRUCTURE_TURRET && s.o.type != (byte)StructureType.STRUCTURE_ROCKET_TURRET) continue;
				if (s.o.script.variables[2] == encoded) s.o.script.variables[2] = 0;
			}

			Unit_RemoveFromTeam(unit);

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.type = 0xFFFF;
			find.index = 0xFFFF;

			while (true)
			{
				Team t;

				t = PoolTeam.Team_Find(find);
				if (t == null) break;

				if (t.target == encoded) t.target = 0;
			}
		}

		/*
		 * Removes the specified unit from its team.
		 *
		 * @param u The unit to remove from the team it is in.
		 * @return Amount of space left in the team.
		 */
		internal static ushort Unit_RemoveFromTeam(Unit u)
		{
			Team t;

			if (u == null) return 0;
			if (u.team == 0) return 0;

			t = PoolTeam.Team_Get_ByIndex((ushort)(u.team - 1));

			t.members--;
			u.team = 0;

			return (ushort)(t.maxMembers - t.members);
		}

		/*
		 * Applies damages to the given unit.
		 *
		 * @param unit The Unit to apply damages on.
		 * @param damage The amount of damage to apply.
		 * @param range ??.
		 * @return True if and only if the unit has no hitpoints left.
		 */
		internal static bool Unit_Damage(Unit unit, ushort damage, ushort range)
		{
			UnitInfo ui;
			var alive = false;
			byte houseID;

			if (unit == null || !unit.o.flags.allocated) return false;

			ui = g_table_unitInfo[unit.o.type];

			if (!ui.flags.isNormalUnit && unit.o.type != (byte)UnitType.UNIT_SANDWORM) return false;

			if (unit.o.hitpoints != 0) alive = true;

			if (unit.o.hitpoints >= damage)
			{
				unit.o.hitpoints -= damage;
			}
			else
			{
				unit.o.hitpoints = 0;
			}

			Unit_Deviation_Decrease(unit, 0);

			houseID = Unit_GetHouseID(unit);

			if (unit.o.hitpoints == 0)
			{
				Unit_RemovePlayer(unit);

				if (unit.o.type == (byte)UnitType.UNIT_HARVESTER) Map.Map_FillCircleWithSpice(CTile.Tile_PackTile(unit.o.position), (ushort)(unit.amount / 32));

				if (unit.o.type == (byte)UnitType.UNIT_SABOTEUR)
				{
					Sound.Sound_Output_Feedback(20);
				}
				else
				{
					if (!ui.o.flags.noMessageOnDeath && alive)
					{
						Sound.Sound_Output_Feedback((ushort)((houseID == (byte)CHouse.g_playerHouseID || CSharpDune.g_campaignID > 3) ? houseID + 14 : 13));
					}
				}

				Unit_SetAction(unit, ActionType.ACTION_DIE);
				return true;
			}

			if (range != 0)
			{
				Map.Map_MakeExplosion((ushort)((damage < 25) ? ExplosionType.EXPLOSION_IMPACT_SMALL : ExplosionType.EXPLOSION_IMPACT_MEDIUM), unit.o.position, 0, 0);
			}

			if (houseID != (byte)CHouse.g_playerHouseID && unit.actionID == (byte)ActionType.ACTION_AMBUSH && unit.o.type != (byte)UnitType.UNIT_HARVESTER)
			{
				Unit_SetAction(unit, ActionType.ACTION_ATTACK);
			}

			if (unit.o.hitpoints >= ui.o.hitpoints / 2) return false;

			if (unit.o.type == (byte)UnitType.UNIT_SANDWORM)
			{
				Unit_SetAction(unit, ActionType.ACTION_DIE);
			}

			if (unit.o.type == (byte)UnitType.UNIT_TROOPERS || unit.o.type == (byte)UnitType.UNIT_INFANTRY)
			{
				unit.o.type += 2;
				ui = g_table_unitInfo[unit.o.type];
				unit.o.hitpoints = ui.o.hitpoints;

				Unit_UpdateMap(2, unit);

				if (Tools.Tools_Random_256() < CHouse.g_table_houseInfo[unit.o.houseID].toughness)
				{
					Unit_SetAction(unit, ActionType.ACTION_RETREAT);
				}
			}

			if (ui.movementType != (ushort)MovementType.MOVEMENT_TRACKED && ui.movementType != (ushort)MovementType.MOVEMENT_HARVESTER && ui.movementType != (ushort)MovementType.MOVEMENT_WHEELED) return false;

			unit.o.flags.isSmoking = true;
			unit.spriteOffset = 0;
			unit.timer = 0;

			return false;
		}

		/*
		 * Check if the Unit belonged the the current human, and do some extra tasks.
		 *
		 * @param unit The Unit to operate on.
		 */
		internal static void Unit_RemovePlayer(Unit unit)
		{
			if (unit == null) return;
			if (Unit_GetHouseID(unit) != (byte)CHouse.g_playerHouseID) return;
			if (!unit.o.flags.allocated) return;

			unit.o.flags.allocated = false;
			Unit_RemoveFromTeam(unit);

			if (unit != g_unitSelected) return;

			if (CSharpDune.g_selectionType == (ushort)SelectionType.TARGET)
			{
				g_unitActive = null;
				CSharpDune.g_activeAction = 0xFFFF;

				Gui.Gui.GUI_ChangeSelectionType((ushort)SelectionType.STRUCTURE);
			}

			Unit_Select(null);
		}

		/*
		 * Decrease deviation counter for the given unit.
		 *
		 * @param unit The Unit to decrease counter for.
		 * @param amount The amount to decrease.
		 * @return True if and only if the unit lost deviation.
		 */
		internal static bool Unit_Deviation_Decrease(Unit unit, ushort amount)
		{
			UnitInfo ui;

			if (unit == null || unit.deviated == 0) return false;

			ui = g_table_unitInfo[unit.o.type];

			if (!ui.flags.isNormalUnit) return false;

			if (amount == 0)
			{
				amount = CHouse.g_table_houseInfo[unit.o.houseID].toughness;
			}

			if (unit.deviated > amount)
			{
				unit.deviated -= (byte)amount;
				return false;
			}

			unit.deviated = 0;

			unit.o.flags.bulletIsBig = true;
			Unit_UpdateMap(2, unit);
			unit.o.flags.bulletIsBig = false;

			if (unit.o.houseID == (byte)CHouse.g_playerHouseID)
			{
				Unit_SetAction(unit, (ActionType)ui.o.actionsPlayer[3]);
			}
			else
			{
				Unit_SetAction(unit, (ActionType)ui.actionAI);
			}

			Unit_UntargetMe(unit);
			unit.targetAttack = 0;
			unit.targetMove = 0;

			return true;
		}

		/*
		 * Set the target for the given unit.
		 *
		 * @param unit The Unit to set the target for.
		 * @param encoded The encoded index of the target.
		 */
		internal static void Unit_SetTarget(Unit unit, ushort encoded)
		{
			if (unit == null || !Tools.Tools_Index_IsValid(encoded)) return;
			if (unit.targetAttack == encoded) return;

			if (Tools.Tools_Index_GetType(encoded) == IndexType.IT_TILE)
			{
				ushort packed;
				Unit u;

				packed = Tools.Tools_Index_Decode(encoded);

				u = Unit_Get_ByPackedTile(packed);
				if (u != null)
				{
					encoded = Tools.Tools_Index_Encode(u.o.index, IndexType.IT_UNIT);
				}
				else
				{
					Structure s;

					s = CStructure.Structure_Get_ByPackedTile(packed);
					if (s != null)
					{
						encoded = Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);
					}
				}
			}

			if (Tools.Tools_Index_Encode(unit.o.index, IndexType.IT_UNIT) == encoded)
			{
				encoded = Tools.Tools_Index_Encode(CTile.Tile_PackTile(unit.o.position), IndexType.IT_TILE);
			}

			unit.targetAttack = encoded;

			if (!g_table_unitInfo[unit.o.type].o.flags.hasTurret)
			{
				unit.targetMove = encoded;
				unit.route[0] = 0xFF;
			}
		}

		/*
		 * Convert the name of a unit to the type value of that unit, or
		 *  UNIT_INVALID if not found.
		 */
		internal static byte Unit_StringToType(string name)
		{
			byte type;
			if (name == null) return (byte)UnitType.UNIT_INVALID;

			for (type = 0; type < (byte)UnitType.UNIT_MAX; type++)
			{
				if (string.Equals(g_table_unitInfo[type].o.name, name, StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(g_table_unitInfo[type].o.name, name) == 0)
					return type;
			}

			return (byte)UnitType.UNIT_INVALID;
		}

		/*
		 * Remove fog arount the given unit.
		 *
		 * @param unit The Unit to remove fog around.
		 */
		internal static void Unit_RemoveFog(Unit unit)
		{
			ushort fogUncoverRadius;

			if (unit == null) return;
			if (unit.o.flags.isNotOnMap) return;
			if ((unit.o.position.x == 0xFFFF && unit.o.position.y == 0xFFFF) || (unit.o.position.x == 0 && unit.o.position.y == 0)) return;
			if (!CHouse.House_AreAllied(Unit_GetHouseID(unit), (byte)CHouse.g_playerHouseID)) return;

			fogUncoverRadius = g_table_unitInfo[unit.o.type].o.fogUncoverRadius;

			if (fogUncoverRadius == 0) return;

			CTile.Tile_RemoveFogInRadius(unit.o.position, fogUncoverRadius);
		}

		/*
		 * Loop over all units, performing various of tasks.
		 */
		internal static void GameLoop_Unit()
		{
			var find = new PoolFindStruct();
			var tickMovement = false;
			var tickRotation = false;
			var tickBlinking = false;
			var tickUnknown4 = false;
			var tickScript = false;
			var tickUnknown5 = false;
			var tickDeviation = false;

			if (CSharpDune.g_debugScenario) return;

			if (s_tickUnitMovement <= Timer.g_timerGame)
			{
				tickMovement = true;
				s_tickUnitMovement = Timer.g_timerGame + 3;
			}

			if (s_tickUnitRotation <= Timer.g_timerGame)
			{
				tickRotation = true;
				s_tickUnitRotation = Timer.g_timerGame + Tools.Tools_AdjustToGameSpeed(4, 2, 8, true);
			}

			if (s_tickUnitBlinking <= Timer.g_timerGame)
			{
				tickBlinking = true;
				s_tickUnitBlinking = Timer.g_timerGame + 3;
			}

			if (s_tickUnitUnknown4 <= Timer.g_timerGame)
			{
				tickUnknown4 = true;
				s_tickUnitUnknown4 = Timer.g_timerGame + 20;
			}

			if (s_tickUnitScript <= Timer.g_timerGame)
			{
				tickScript = true;
				s_tickUnitScript = Timer.g_timerGame + 5;
			}

			if (s_tickUnitUnknown5 <= Timer.g_timerGame)
			{
				tickUnknown5 = true;
				s_tickUnitUnknown5 = Timer.g_timerGame + 5;
			}

			if (s_tickUnitDeviation <= Timer.g_timerGame)
			{
				tickDeviation = true;
				s_tickUnitDeviation = Timer.g_timerGame + 60;
			}

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			while (true)
			{
				UnitInfo ui;
				Unit u;

				u = PoolUnit.Unit_Find(find);
				if (u == null) break;

				ui = g_table_unitInfo[u.o.type];

                g_scriptCurrentObject = u.o;
                g_scriptCurrentStructure = null;
                g_scriptCurrentUnit = u;
                g_scriptCurrentTeam = null;

				if (u.o.flags.isNotOnMap) continue;

				if (tickUnknown4 && u.targetAttack != 0 && ui.o.flags.hasTurret)
				{
					tile32 tile;

					tile = Tools.Tools_Index_GetTile(u.targetAttack);

					Unit_SetOrientation(u, CTile.Tile_GetDirection(u.o.position, tile), false, 1);
				}

				if (tickMovement)
				{
					Unit_MovementTick(u);

					if (u.fireDelay != 0)
					{
						if (ui.movementType == (ushort)MovementType.MOVEMENT_WINGER && !ui.flags.isNormalUnit)
						{
							tile32 tile;

							tile = u.currentDestination;

							if (Tools.Tools_Index_GetType(u.targetAttack) == IndexType.IT_UNIT && g_table_unitInfo[Tools.Tools_Index_GetUnit(u.targetAttack).o.type].movementType == (ushort)MovementType.MOVEMENT_WINGER)
							{
								tile = Tools.Tools_Index_GetTile(u.targetAttack);
							}

							Unit_SetOrientation(u, CTile.Tile_GetDirection(u.o.position, tile), false, 0);
						}

						u.fireDelay--;
					}
				}

				if (tickRotation)
				{
					Unit_Rotate(u, 0);
					if (ui.o.flags.hasTurret) Unit_Rotate(u, 1);
				}

				if (tickBlinking && u.blinkCounter != 0)
				{
					u.blinkCounter--;
					if ((u.blinkCounter % 2) != 0)
					{
						u.o.flags.isHighlighted = true;
					}
					else
					{
						u.o.flags.isHighlighted = false;
					}

					Unit_UpdateMap(2, u);
				}

				if (tickDeviation) Unit_Deviation_Decrease(u, 1);

				if (ui.movementType != (ushort)MovementType.MOVEMENT_WINGER && CObject.Object_GetByPackedTile(CTile.Tile_PackTile(u.o.position)) == null) Unit_UpdateMap(1, u);

				if (tickUnknown5)
				{
					if (u.timer == 0)
					{
						if ((ui.movementType == (ushort)MovementType.MOVEMENT_FOOT && u.speed != 0) || u.o.flags.isSmoking)
						{
							if (u.spriteOffset >= 0)
							{
								u.spriteOffset &= 0x3F;
								u.spriteOffset++;

								Unit_UpdateMap(2, u);

								u.timer = (ushort)(ui.animationSpeed / 5);
								if (u.o.flags.isSmoking)
								{
									u.timer = 3;
									if (u.spriteOffset > 32)
									{
										u.o.flags.isSmoking = false;
										u.spriteOffset = 0;
									}
								}
							}
						}

						if (u.o.type == (byte)UnitType.UNIT_ORNITHOPTER && u.o.flags.allocated && u.spriteOffset >= 0)
						{
							u.spriteOffset &= 0x3F;
							u.spriteOffset++;

							Unit_UpdateMap(2, u);

							u.timer = 1;
						}

						if (u.o.type == (byte)UnitType.UNIT_HARVESTER)
						{
							if (u.actionID == (byte)ActionType.ACTION_HARVEST || u.o.flags.isSmoking)
							{
								u.spriteOffset &= 0x3F;
								u.spriteOffset++;

								Unit_UpdateMap(2, u);

								u.timer = 4;
							}
							else
							{
								if (u.spriteOffset != 0)
								{
									Unit_UpdateMap(2, u);

									u.spriteOffset = 0;
								}
							}
						}
					}
					else
					{
						u.timer--;
					}
				}

				if (tickScript)
				{
					if (u.o.script.delay == 0)
					{
						if (Script_IsLoaded(u.o.script))
						{
							var opcodesLeft = SCRIPT_UNIT_OPCODES_PER_TICK + 2;
							if (!ui.o.flags.scriptNoSlowdown && !Map.Map_IsPositionInViewport(u.o.position, out _, out _))
							{
								opcodesLeft = 3;
							}

							u.o.script.variables[3] = (ushort)CHouse.g_playerHouseID;

							for (; opcodesLeft > 0 && u.o.script.delay == 0; opcodesLeft--)
							{
								if (!Script_Run(u.o.script)) break;
							}
						}
					}
					else
					{
						u.o.script.delay--;
					}
				}

				if (u.nextActionID == (byte)ActionType.ACTION_INVALID) continue;
				if (u.currentDestination.x != 0 || u.currentDestination.y != 0) continue;

				Unit_SetAction(u, (ActionType)u.nextActionID);
				u.nextActionID = (byte)ActionType.ACTION_INVALID;
			}
		}

		/*
		 * Set the new orientation of the unit.
		 *
		 * @param unit The Unit to operate on.
		 * @param orientation The new orientation of the unit.
		 * @param rotateInstantly If true, rotation is instant. Else the unit turns over the next few ticks slowly.
		 * @param level 0 = base, 1 = top (turret etc).
		 */
		internal static void Unit_SetOrientation(Unit unit, sbyte orientation, bool rotateInstantly, ushort level)
		{
			short diff;

			Debug.Assert(level == 0 || level == 1);

			if (unit == null) return;

			unit.orientation[level].speed = 0;
			unit.orientation[level].target = orientation;

			if (rotateInstantly)
			{
				unit.orientation[level].current = orientation;
				return;
			}

			if (unit.orientation[level].current == orientation) return;

			unit.orientation[level].speed = (sbyte)(g_table_unitInfo[unit.o.type].turningSpeed * 4);

			diff = (short)(orientation - unit.orientation[level].current);

			if ((diff > -128 && diff < 0) || diff > 128)
			{
				unit.orientation[level].speed = (sbyte)-unit.orientation[level].speed;
			}
		}

		/*
		 * Sets the position of the given unit.
		 *
		 * @param u The Unit to set the position for.
		 * @position The position.
		 * @return True if and only if the position changed.
		 */
		internal static bool Unit_SetPosition(Unit u, tile32 position)
		{
			UnitInfo ui;

			if (u == null) return false;

			ui = g_table_unitInfo[u.o.type];
			u.o.flags.isNotOnMap = false;

			u.o.position = CTile.Tile_Center(position);

			if (u.originEncoded == 0) Unit_FindClosestRefinery(u);

			u.o.script.variables[4] = 0;

			if (Unit_IsTileOccupied(u))
			{
				u.o.flags.isNotOnMap = true;
				return false;
			}

			u.currentDestination.x = 0;
			u.currentDestination.y = 0;
			u.targetMove = 0;
			u.targetAttack = 0;

			if (Map.g_map[CTile.Tile_PackTile(u.o.position)].isUnveiled)
			{
				/* A new unit being delivered fresh from the factory; force a seenByHouses
				 *  update and add it to the statistics etc. */
				u.o.seenByHouses &= (byte)~(1 << u.o.houseID);
				Unit_HouseUnitCount_Add(u, (byte)CHouse.g_playerHouseID);
			}

			if (u.o.houseID != (byte)CHouse.g_playerHouseID || u.o.type == (byte)UnitType.UNIT_HARVESTER || u.o.type == (byte)UnitType.UNIT_SABOTEUR)
			{
				Unit_SetAction(u, (ActionType)ui.actionAI);
			}
			else
			{
				Unit_SetAction(u, (ActionType)ui.o.actionsPlayer[3]);
			}

			u.spriteOffset = 0;

			Unit_UpdateMap(1, u);

			return true;
		}

		/*
		 * Call a specified type of unit owned by the house to you.
		 *
		 * @param type The type of the Unit to find.
		 * @param houseID The houseID of the Unit to find.
		 * @param target To where the found Unit should move.
		 * @param createCarryall Create a carryall if none found.
		 * @return The found Unit, or NULL if none found.
		 */
		internal static Unit Unit_CallUnitByType(UnitType type, byte houseID, ushort target, bool createCarryall)
		{
			var find = new PoolFindStruct();
			Unit unit = null;

			find.houseID = houseID;
			find.type = (ushort)type;
			find.index = 0xFFFF;

			while (true)
			{
				Unit u;

				u = PoolUnit.Unit_Find(find);
				if (u == null) break;
				if (u.o.linkedID != 0xFF) continue;
				if (u.targetMove != 0) continue;
				unit = u;
			}

			if (createCarryall && unit == null && type == UnitType.UNIT_CARRYALL)
			{
				var position = new tile32();

				CSharpDune.g_validateStrictIfZero++;
				position.x = 0;
				position.y = 0;
				unit = Unit_Create((ushort)PoolUnit.UnitIndex.UNIT_INDEX_INVALID, (byte)type, houseID, position, 96);
				CSharpDune.g_validateStrictIfZero--;

				if (unit != null) unit.o.flags.byScenario = true;
			}

			if (unit != null)
			{
				unit.targetMove = target;

				CObject.Object_Script_Variable4_Set(unit.o, target);
			}

			return unit;
		}

		/*
		 * Create a new Unit.
		 *
		 * @param index The new index of the Unit, or UNIT_INDEX_INVALID to assign one.
		 * @param typeID The type of the new Unit.
		 * @param houseID The House of the new Unit.
		 * @param position To where on the map this Unit should be transported, or TILE_INVALID for not on the map yet.
		 * @param orientation Orientation of the Unit.
		 * @return The new created Unit, or NULL if something failed.
		 */
		internal static Unit Unit_Create(ushort index, byte typeID, byte houseID, tile32 position, sbyte orientation)
		{
			UnitInfo ui;
			Unit u;

			if (houseID >= (byte)HouseType.HOUSE_MAX) return null;
			if (typeID >= (byte)UnitType.UNIT_MAX) return null;

			ui = g_table_unitInfo[typeID];
			u = PoolUnit.Unit_Allocate(index, typeID, houseID);
			if (u == null) return null;

			u.o.houseID = houseID;

			Unit_SetOrientation(u, orientation, true, 0);
			Unit_SetOrientation(u, orientation, true, 1);

			Unit_SetSpeed(u, 0);

			u.o.position = position;
			u.o.hitpoints = ui.o.hitpoints;
			u.currentDestination.x = 0;
			u.currentDestination.y = 0;
			u.originEncoded = 0x0000;
			u.route[0] = 0xFF;

			if (position.x != 0xFFFF || position.y != 0xFFFF)
			{
				u.originEncoded = Unit_FindClosestRefinery(u);
				u.targetLast = position;
				u.targetPreLast = position;
			}

			u.o.linkedID = 0xFF;
			u.o.script.delay = 0;
			u.actionID = (byte)ActionType.ACTION_GUARD;
			u.nextActionID = (byte)ActionType.ACTION_INVALID;
			u.fireDelay = 0;
			u.distanceToDestination = 0x7FFF;
			u.targetMove = 0x0000;
			u.amount = 0;
			u.wobbleIndex = 0;
			u.spriteOffset = 0;
			u.blinkCounter = 0;
			u.timer = 0;

            Script_Reset(u.o.script, g_scriptUnit);

			u.o.flags.allocated = true;

			if (ui.movementType == (ushort)MovementType.MOVEMENT_TRACKED)
			{
				if (Tools.Tools_Random_256() < CHouse.g_table_houseInfo[houseID].degradingChance)
				{
					u.o.flags.degrades = true;
				}
			}

			if (ui.movementType == (ushort)MovementType.MOVEMENT_WINGER)
			{
				Unit_SetSpeed(u, 255);
			}
			else
			{
				if ((position.x != 0xFFFF || position.y != 0xFFFF) && Unit_IsTileOccupied(u))
				{
					PoolUnit.Unit_Free(u);
					return null;
				}
			}

			if ((position.x == 0xFFFF) && (position.y == 0xFFFF))
			{
				u.o.flags.isNotOnMap = true;
				return u;
			}

			Unit_UpdateMap(1, u);

			Unit_SetAction(u, (ActionType)((houseID == (byte)CHouse.g_playerHouseID) ? ui.o.actionsPlayer[3] : ui.actionAI));

			return u;
		}

		/*
		 * Finds the closest refinery a harvester can go to.
		 *
		 * @param unit The unit to find the closest refinery for.
		 * @return 1 if unit->originEncoded was not 0, else 0.
		 */
		internal static ushort Unit_FindClosestRefinery(Unit unit)
		{
			ushort res;
			Structure s = null;
			ushort mind = 0;
			Structure s2;
			ushort d;
			var find = new PoolFindStruct();

			res = (ushort)((unit.originEncoded == 0) ? 0 : 1);

			if (unit.o.type != (byte)UnitType.UNIT_HARVESTER)
			{
				unit.originEncoded = Tools.Tools_Index_Encode(CTile.Tile_PackTile(unit.o.position), IndexType.IT_TILE);
				return res;
			}

			find.type = (ushort)StructureType.STRUCTURE_REFINERY;
			find.houseID = Unit_GetHouseID(unit);
			find.index = 0xFFFF;

			while (true)
			{
				s2 = PoolStructure.Structure_Find(find);
				if (s2 == null) break;
				if (s2.state != (short)StructureState.STRUCTURE_STATE_BUSY) continue;
				d = CTile.Tile_GetDistance(unit.o.position, s2.o.position);
				if (mind != 0 && d >= mind) continue;
				mind = d;
				s = s2;
			}

			if (s == null)
			{
				find.type = (ushort)StructureType.STRUCTURE_REFINERY;
				find.houseID = Unit_GetHouseID(unit);
				find.index = 0xFFFF;

				while (true)
				{
					s2 = PoolStructure.Structure_Find(find);
					if (s2 == null) break;
					d = CTile.Tile_GetDistance(unit.o.position, s2.o.position);
					if (mind != 0 && d >= mind) continue;
					mind = d;
					s = s2;
				}
			}

			if (s != null) unit.originEncoded = Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);

			return res;
		}

		/*
		 * Check if the position the unit is on is already occupied.
		 *
		 * @param unit The Unit to operate on.
		 * @return True if and only if the position of the unit is already occupied.
		 */
		internal static bool Unit_IsTileOccupied(Unit unit)
		{
			UnitInfo ui;
			ushort packed;
			Unit unit2;
			ushort speed;

			if (unit == null) return true;

			ui = g_table_unitInfo[unit.o.type];
			packed = CTile.Tile_PackTile(unit.o.position);

			speed = Map.g_table_landscapeInfo[Map.Map_GetLandscapeType(packed)].movementSpeed[ui.movementType];
			if (speed == 0) return true;

			if (unit.o.type == (byte)UnitType.UNIT_SANDWORM || ui.movementType == (ushort)MovementType.MOVEMENT_WINGER) return false;

			unit2 = Unit_Get_ByPackedTile(packed);
			if (unit2 != null && unit2 != unit)
			{
				if (CHouse.House_AreAllied(Unit_GetHouseID(unit2), Unit_GetHouseID(unit))) return true;
				if (ui.movementType != (ushort)MovementType.MOVEMENT_TRACKED) return true;
				if (g_table_unitInfo[unit2.o.type].movementType != (ushort)MovementType.MOVEMENT_FOOT) return true;
			}

			return CStructure.Structure_Get_ByPackedTile(packed) != null;
		}

		/*
		 * Set the speed of a Unit.
		 *
		 * @param unit The Unit to operate on.
		 * @param speed The new speed of the unit (a percent value between 0 and 255).
		 */
		internal static void Unit_SetSpeed(Unit unit, ushort speed)
		{
			ushort speedPerTick;

			Debug.Assert(unit != null);

			speedPerTick = 0;

			unit.speed = 0;
			unit.speedRemainder = 0;
			unit.speedPerTick = 0;

			if (unit.o.type == (byte)UnitType.UNIT_HARVESTER)
			{
				speed = (ushort)(((255 - unit.amount) * speed) / 256);
			}

			if (speed == 0 || speed >= 256)
			{
				unit.movingSpeed = 0;
				return;
			}

			unit.movingSpeed = (byte)(speed & 0xFF);
			speed = (ushort)(g_table_unitInfo[unit.o.type].movingSpeedFactor * speed / 256);

			/* Units in the air don't feel the effect of gameSpeed */
			if (g_table_unitInfo[unit.o.type].movementType != (ushort)MovementType.MOVEMENT_WINGER)
			{
				speed = Tools.Tools_AdjustToGameSpeed(speed, 1, 255, false);
			}

			speedPerTick = (ushort)(speed << 4);
			speed = (ushort)(speed >> 4);

			if (speed != 0)
			{
				speedPerTick = 255;
			}
			else
			{
				speed = 1;
			}

			unit.speed = (byte)(speed & 0xFF);
			unit.speedPerTick = (byte)(speedPerTick & 0xFF);
		}

		/*
		 * Rotate a unit (or his top).
		 *
		 * @param unit The Unit to operate on.
		 * @param level 0 = base, 1 = top (turret etc).
		 */
		static void Unit_Rotate(Unit unit, ushort level)
		{
			sbyte target;
			sbyte current;
			sbyte newCurrent;
			short diff;

			Debug.Assert(level == 0 || level == 1);

			if (unit.orientation[level].speed == 0) return;

			target = unit.orientation[level].target;
			current = unit.orientation[level].current;
			diff = (short)(target - current);

			if (diff > 128) diff -= 256;
			if (diff < -128) diff += 256;
			diff = Abs(diff);

			newCurrent = (sbyte)(current + unit.orientation[level].speed);

			if (Abs(unit.orientation[level].speed) >= diff)
			{
				unit.orientation[level].speed = 0;
				newCurrent = target;
			}

			unit.orientation[level].current = newCurrent;

			if (CTile.Orientation_Orientation256ToOrientation16((byte)newCurrent) == CTile.Orientation_Orientation256ToOrientation16((byte)current) &&
				CTile.Orientation_Orientation256ToOrientation8((byte)newCurrent) == CTile.Orientation_Orientation256ToOrientation8((byte)current)) return;

			Unit_UpdateMap(2, unit);
		}

		/*
		  * Create a unit (and a carryall if needed).
		  *
		  * @param houseID The House of the new Unit.
		  * @param typeID The type of the new Unit.
		  * @param destination To where on the map this Unit should move.
		  * @return The new created Unit, or NULL if something failed.
		  */
		internal static Unit Unit_CreateWrapper(byte houseID, UnitType typeID, ushort destination)
		{
			tile32 tile;
			House h;
			sbyte orientation;
			Unit unit;
			Unit carryall;

			tile = CTile.Tile_UnpackTile(Map.Map_FindLocationTile((ushort)(Tools.Tools_Random_256() & 3), houseID));

			h = PoolHouse.House_Get_ByIndex(houseID);

			{
                var t = new tile32
                {
                    x = 0x2000,
                    y = 0x2000
                };
                orientation = CTile.Tile_GetDirection(tile, t);
			}

			if (g_table_unitInfo[(int)typeID].movementType == (ushort)MovementType.MOVEMENT_WINGER)
			{
				CSharpDune.g_validateStrictIfZero++;
				unit = Unit_Create((ushort)PoolUnit.UnitIndex.UNIT_INDEX_INVALID, (byte)typeID, houseID, tile, orientation);
				CSharpDune.g_validateStrictIfZero--;

				if (unit == null) return null;

				unit.o.flags.byScenario = true;

				if (destination != 0)
				{
					Unit_SetDestination(unit, destination);
				}

				return unit;
			}

			CSharpDune.g_validateStrictIfZero++;
			carryall = Unit_Create((ushort)PoolUnit.UnitIndex.UNIT_INDEX_INVALID, (byte)UnitType.UNIT_CARRYALL, houseID, tile, orientation);
			CSharpDune.g_validateStrictIfZero--;

			if (carryall == null)
			{
				if (typeID == UnitType.UNIT_HARVESTER && h.harvestersIncoming == 0) h.harvestersIncoming++;
				return null;
			}

			if (CHouse.House_AreAllied(houseID, (byte)CHouse.g_playerHouseID) || Unit_IsTypeOnMap(houseID, (byte)UnitType.UNIT_CARRYALL))
			{
				carryall.o.flags.byScenario = true;
			}

			tile.x = 0xFFFF;
			tile.y = 0xFFFF;

			CSharpDune.g_validateStrictIfZero++;
			unit = Unit_Create((ushort)PoolUnit.UnitIndex.UNIT_INDEX_INVALID, (byte)typeID, houseID, tile, 0);
			CSharpDune.g_validateStrictIfZero--;

			if (unit == null)
			{
				Unit_Remove(carryall);
				if (typeID == UnitType.UNIT_HARVESTER && h.harvestersIncoming == 0) h.harvestersIncoming++;
				return null;
			}

			carryall.o.flags.inTransport = true;
			carryall.o.linkedID = (byte)(unit.o.index & 0xFF);
			if (typeID == UnitType.UNIT_HARVESTER) unit.amount = 1;

			if (destination != 0)
			{
				Unit_SetDestination(carryall, destination);
			}

			return unit;
		}

		/*
		 * Sets the destination for the given unit.
		 *
		 * @param u The unit to set the destination for.
		 * @param destination The destination (encoded index).
		 */
		internal static void Unit_SetDestination(Unit u, ushort destination)
		{
			Structure s;

			if (u == null) return;
			if (!Tools.Tools_Index_IsValid(destination)) return;
			if (u.targetMove == destination) return;

			if (Tools.Tools_Index_GetType(destination) == IndexType.IT_TILE)
			{
				Unit u2;
				ushort packed;

				packed = Tools.Tools_Index_Decode(destination);

				u2 = Unit_Get_ByPackedTile(packed);
				if (u2 != null)
				{
					if (u != u2) destination = Tools.Tools_Index_Encode(u2.o.index, IndexType.IT_UNIT);
				}
				else
				{
					s = CStructure.Structure_Get_ByPackedTile(packed);
					if (s != null) destination = Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);
				}
			}

			s = Tools.Tools_Index_GetStructure(destination);
			if (s != null && s.o.houseID == Unit_GetHouseID(u))
			{
				if (Unit_IsValidMovementIntoStructure(u, s) == 1 || g_table_unitInfo[u.o.type].movementType == (ushort)MovementType.MOVEMENT_WINGER)
				{
					CObject.Object_Script_Variable4_Link(Tools.Tools_Index_Encode(u.o.index, IndexType.IT_UNIT), destination);
				}
			}

			u.targetMove = destination;
			u.route[0] = 0xFF;
		}

		/*
		 * Checks if a Unit is on the map.
		 *
		 * @param houseID The House of the Unit.
		 * @param typeID The type of the Unit.
		 * @return Returns true if and only if a Unit with the given attributes is on the map.
		 */
		internal static bool Unit_IsTypeOnMap(byte houseID, byte typeID)
		{
			ushort i;

			for (i = 0; i < PoolUnit.g_unitFindCount; i++)
			{
				Unit u;

				u = PoolUnit.g_unitFindArray[i];
				if (houseID != (byte)HouseType.HOUSE_INVALID && Unit_GetHouseID(u) != houseID) continue;
				if (typeID != (byte)UnitType.UNIT_INVALID && u.o.type != typeID) continue;
				if (CSharpDune.g_validateStrictIfZero == 0 && u.o.flags.isNotOnMap) continue;

				return true;
			}
			return false;
		}

		/*
		 * Determines whether a move order into the given structure is OK for
		 * a particular unit.
		 *
		 * It handles orders to invade enemy buildings as well as going into
		 * a friendly structure (e.g. refinery, repair facility).
		 *
		 * @param unit The Unit to operate on.
		 * @param s The Structure to operate on.
		 * @return
		 * 0 - invalid movement
		 * 1 - valid movement, will try to get close to the structure
		 * 2 - valid movement, will attempt to damage/conquer the structure
		 */
		internal static ushort Unit_IsValidMovementIntoStructure(Unit unit, Structure s)
		{
			StructureInfo si;
			UnitInfo ui;
			ushort unitEnc;
			ushort structEnc;

			if (unit == null || s == null) return 0;

			si = CStructure.g_table_structureInfo[s.o.type];
			ui = g_table_unitInfo[unit.o.type];

			unitEnc = Tools.Tools_Index_Encode(unit.o.index, IndexType.IT_UNIT);
			structEnc = Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);

			/* Movement into structure of other owner. */
			if (Unit_GetHouseID(unit) != s.o.houseID)
			{
				/* Saboteur can always enter houses */
				if (unit.o.type == (byte)UnitType.UNIT_SABOTEUR && unit.targetMove == structEnc) return 2;
				/* Entering houses is only possible for foot-units and if the structure is conquerable.
				 * Everyone else can only move close to the building. */
				if (ui.movementType == (ushort)MovementType.MOVEMENT_FOOT && si.o.flags.conquerable) return (ushort)(unit.targetMove == structEnc ? 2 : 1);
				return 0;
			}

			/* Prevent movement if target structure does not accept the unit type. */
			if ((si.enterFilter & (1 << unit.o.type)) == 0) return 0;

			/* TODO -- Not sure. */
			if (s.o.script.variables[4] == unitEnc) return 2;

			/* Enter only if structure not linked to any other unit already. */
			return (ushort)(s.o.linkedID == 0xFF ? 1 : 0);
		}

		static void Unit_MovementTick(Unit unit)
		{
			ushort speed;

			if (unit.speed == 0) return;

			speed = unit.speedRemainder;

			/* Units in the air don't feel the effect of gameSpeed */
			if (g_table_unitInfo[unit.o.type].movementType != (ushort)MovementType.MOVEMENT_WINGER)
			{
				speed += Tools.Tools_AdjustToGameSpeed(unit.speedPerTick, 1, 255, false);
			}
			else
			{
				speed += unit.speedPerTick;
			}

			if ((speed & 0xFF00) != 0)
			{
				Unit_Move(unit, (ushort)Min(unit.speed * 16, CTile.Tile_GetDistance(unit.o.position, unit.currentDestination) + 16));
			}

			unit.speedRemainder = (byte)(speed & 0xFF);
		}

		/*
		 * Create a new bullet Unit.
		 *
		 * @param position Where on the map this bullet Unit is created.
		 * @param typeID The type of the new bullet Unit.
		 * @param houseID The House of the new bullet Unit.
		 * @param damage The hitpoints of the new bullet Unit.
		 * @param target The target of the new bullet Unit.
		 * @return The new created Unit, or NULL if something failed.
		 */
		internal static Unit Unit_CreateBullet(tile32 position, UnitType type, byte houseID, ushort damage, ushort target)
		{
			UnitInfo ui;
			tile32 tile;

			if (!Tools.Tools_Index_IsValid(target)) return null;

			ui = g_table_unitInfo[(int)type];
			tile = Tools.Tools_Index_GetTile(target);

			switch (type)
			{
				case UnitType.UNIT_MISSILE_HOUSE:
				case UnitType.UNIT_MISSILE_ROCKET:
				case UnitType.UNIT_MISSILE_TURRET:
				case UnitType.UNIT_MISSILE_DEVIATOR:
				case UnitType.UNIT_MISSILE_TROOPER:
					{
						sbyte orientation;
						Unit bullet;
						Unit u;

						orientation = CTile.Tile_GetDirection(position, tile);

						bullet = Unit_Create((ushort)PoolUnit.UnitIndex.UNIT_INDEX_INVALID, (byte)type, houseID, position, orientation);
						if (bullet == null) return null;

						Sound.Voice_PlayAtTile((short)ui.bulletSound, position);

						bullet.targetAttack = target;
						bullet.o.hitpoints = damage;
						bullet.currentDestination = tile;

						if (ui.flags.notAccurate)
						{
							bullet.currentDestination = CTile.Tile_MoveByRandom(tile, (ushort)((Tools.Tools_Random_256() & 0xF) != 0 ? CTile.Tile_GetDistance(position, tile) / 256 + 8 : Tools.Tools_Random_256() + 8), false);
						}

						bullet.fireDelay = (ushort)(ui.fireDistance & 0xFF);

						u = Tools.Tools_Index_GetUnit(target);
						if (u != null && g_table_unitInfo[u.o.type].movementType == (ushort)MovementType.MOVEMENT_WINGER)
						{
							bullet.fireDelay <<= 1;
						}

						if (type == UnitType.UNIT_MISSILE_HOUSE || (bullet.o.seenByHouses & (1 << (byte)CHouse.g_playerHouseID)) != 0) return bullet;

						CTile.Tile_RemoveFogInRadius(bullet.o.position, 2);

						return bullet;
					}

				case UnitType.UNIT_BULLET:
				case UnitType.UNIT_SONIC_BLAST:
					{
						sbyte orientation;
						tile32 t;
						Unit bullet;

						orientation = CTile.Tile_GetDirection(position, tile);

						t = CTile.Tile_MoveByDirection(CTile.Tile_MoveByDirection(position, 0, 32), orientation, 128);

						bullet = Unit_Create((ushort)PoolUnit.UnitIndex.UNIT_INDEX_INVALID, (byte)type, houseID, t, orientation);
						if (bullet == null) return null;

						if (type == UnitType.UNIT_SONIC_BLAST)
						{
							bullet.fireDelay = (ushort)(ui.fireDistance & 0xFF);
						}

						bullet.currentDestination = tile;
						bullet.o.hitpoints = damage;

						if (damage > 15) bullet.o.flags.bulletIsBig = true;

						if ((bullet.o.seenByHouses & (1 << (byte)CHouse.g_playerHouseID)) != 0) return bullet;

						CTile.Tile_RemoveFogInRadius(bullet.o.position, 2);

						return bullet;
					}

				default: return null;
			}
		}

		/*
		 * Adds the specified unit to the specified team.
		 *
		 * @param u The unit to add to the team.
		 * @param t The team to add the unit to.
		 * @return Amount of space left in the team.
		 */
		internal static ushort Unit_AddToTeam(Unit u, Team t)
		{
			if (t == null || u == null) return 0;

			u.team = (byte)(t.index + 1);
			t.members++;

			return (ushort)(t.maxMembers - t.members);
		}

		internal static void Unit_LaunchHouseMissile(ushort packed)
		{
			tile32 tile;
			bool isAI;
			House h;

			if (g_unitHouseMissile == null) return;

			h = PoolHouse.House_Get_ByIndex(g_unitHouseMissile.o.houseID);

			tile = CTile.Tile_UnpackTile(packed);
			tile = CTile.Tile_MoveByRandom(tile, 160, false);

			packed = CTile.Tile_PackTile(tile);

			isAI = g_unitHouseMissile.o.houseID != (byte)CHouse.g_playerHouseID;

			PoolUnit.Unit_Free(g_unitHouseMissile);

			Sound.Sound_Output_Feedback(0xFFFE);

			Unit_CreateBullet(h.palacePosition, (UnitType)g_unitHouseMissile.o.type, g_unitHouseMissile.o.houseID, 0x1F4, Tools.Tools_Index_Encode(packed, IndexType.IT_TILE));

			CHouse.g_houseMissileCountdown = 0;
			g_unitHouseMissile = null;

			if (isAI)
			{
				Sound.Sound_Output_Feedback(39);
				return;
			}

			Gui.Gui.GUI_ChangeSelectionType((ushort)SelectionType.STRUCTURE);
		}

		/*
		 * Gets the best target for the given unit.
		 *
		 * @param unit The Unit to get the best target for.
		 * @param mode How to determine the best target.
		 * @return The encoded index of the best target or 0 if none found.
		 */
		internal static ushort Unit_FindBestTargetEncoded(Unit unit, ushort mode)
		{
			Structure s;
			Unit target;

			if (unit == null) return 0;

			s = null;

			if (mode == 4)
			{
				s = Unit_FindBestTargetStructure(unit, mode);

				if (s != null) return Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);

				target = Unit_FindBestTargetUnit(unit, mode);

				if (target == null) return 0;
				return Tools.Tools_Index_Encode(target.o.index, IndexType.IT_UNIT);
			}

			target = Unit_FindBestTargetUnit(unit, mode);

			if (unit.o.type != (byte)UnitType.UNIT_DEVIATOR) s = Unit_FindBestTargetStructure(unit, mode);

			if (target != null && s != null)
			{
				ushort priority;

				priority = Unit_GetTargetUnitPriority(unit, target);

				if (Unit_GetTargetStructurePriority(unit, s) >= priority) return Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);
				return Tools.Tools_Index_Encode(target.o.index, IndexType.IT_UNIT);
			}

			if (target != null) return Tools.Tools_Index_Encode(target.o.index, IndexType.IT_UNIT);
			if (s != null) return Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);

			return 0;
		}

		/*
		 * Gets the best target unit for the given unit.
		 *
		 * @param u The Unit to get the best target for.
		 * @param mode How to determine the best target.
		 * @return The best target or NULL if none found.
		 */
		static Unit Unit_FindBestTargetUnit(Unit u, ushort mode)
		{
			tile32 position;
			ushort distance;
			var find = new PoolFindStruct();
			Unit best = null;
			ushort bestPriority = 0;

			if (u == null) return null;

			position = u.o.position;
			if (u.originEncoded == 0)
			{
				u.originEncoded = Tools.Tools_Index_Encode(CTile.Tile_PackTile(position), IndexType.IT_TILE);
			}
			else
			{
				position = Tools.Tools_Index_GetTile(u.originEncoded);
			}

			distance = (ushort)(g_table_unitInfo[u.o.type].fireDistance << 8);
			if (mode == 2) distance <<= 1;

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.type = 0xFFFF;
			find.index = 0xFFFF;

			while (true)
			{
				Unit target;
				ushort priority;

				target = PoolUnit.Unit_Find(find);

				if (target == null) break;

				if (mode != 0 && mode != 4)
				{
					if (mode == 1)
					{
						if (CTile.Tile_GetDistance(u.o.position, target.o.position) > distance) continue;
					}
					if (mode == 2)
					{
						if (CTile.Tile_GetDistance(position, target.o.position) > distance) continue;
					}
				}

				priority = Unit_GetTargetUnitPriority(u, target);

				if ((short)priority > (short)bestPriority)
				{
					best = target;
					bestPriority = priority;
				}
			}

			if (bestPriority == 0) return null;

			return best;
		}

		/*
		 * Gets the best target structure for the given unit.
		 *
		 * @param unit The Unit to get the best target for.
		 * @param mode How to determine the best target.
		 * @return The best target or NULL if none found.
		 */
		static Structure Unit_FindBestTargetStructure(Unit unit, ushort mode)
		{
			Structure best = null;
			ushort bestPriority = 0;
			tile32 position;
			ushort distance;
			var find = new PoolFindStruct();

			if (unit == null) return null;

			position = Tools.Tools_Index_GetTile(unit.originEncoded);
			distance = (ushort)(g_table_unitInfo[unit.o.type].fireDistance << 8);

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			while (true)
			{
				Structure s;
				var curPosition = new tile32();
				ushort priority;

				s = PoolStructure.Structure_Find(find);
				if (s == null) break;
				if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;

				curPosition.x = (ushort)(s.o.position.x + CStructure.g_table_structure_layoutTileDiff[CStructure.g_table_structureInfo[s.o.type].layout].x);
				curPosition.y = (ushort)(s.o.position.y + CStructure.g_table_structure_layoutTileDiff[CStructure.g_table_structureInfo[s.o.type].layout].y);

				if (mode != 0 && mode != 4)
				{
					if (mode == 1)
					{
						if (CTile.Tile_GetDistance(unit.o.position, curPosition) > distance) continue;
					}
					else
					{
						if (mode != 2) continue;
						if (CTile.Tile_GetDistance(position, curPosition) > distance * 2) continue;
					}
				}

				priority = Unit_GetTargetStructurePriority(unit, s);

				if (priority >= bestPriority)
				{
					best = s;
					bestPriority = priority;
				}
			}

			if (bestPriority == 0) return null;

			return best;
		}

		/*
		 * Get the priority a target unit has for a given unit. The higher the value,
		 *  the more serious it should look at the target.
		 *
		 * @param unit The unit looking at a target.
		 * @param target The unit to look at.
		 * @return The priority of the target.
		 */
		internal static ushort Unit_GetTargetUnitPriority(Unit unit, Unit target)
		{
			UnitInfo targetInfo;
			UnitInfo unitInfo;
			ushort distance;
			ushort priority;

			if (unit == null || target == null) return 0;
			if (unit == target) return 0;

			if (!target.o.flags.allocated) return 0;
			if ((target.o.seenByHouses & (1 << Unit_GetHouseID(unit))) == 0) return 0;

			if (CHouse.House_AreAllied(Unit_GetHouseID(unit), Unit_GetHouseID(target))) return 0;

			unitInfo = g_table_unitInfo[unit.o.type];
			targetInfo = g_table_unitInfo[target.o.type];

			if (!targetInfo.o.flags.priority) return 0;

			if (targetInfo.movementType == (ushort)MovementType.MOVEMENT_WINGER)
			{
				if (!unitInfo.o.flags.targetAir) return 0;
				if (target.o.houseID == (byte)CHouse.g_playerHouseID && !Map.Map_IsPositionUnveiled(CTile.Tile_PackTile(target.o.position))) return 0;
			}

			if (!Map.Map_IsValidPosition(CTile.Tile_PackTile(target.o.position))) return 0;

			distance = CTile.Tile_GetDistanceRoundedUp(unit.o.position, target.o.position);

			if (!Map.Map_IsValidPosition(CTile.Tile_PackTile(unit.o.position)))
			{
				if (targetInfo.fireDistance >= distance) return 0;
			}

			priority = (ushort)(targetInfo.o.priorityTarget + targetInfo.o.priorityBuild);
			if (distance != 0) priority = (ushort)((priority / distance) + 1);

			if (priority > 0x7D00) return 0x7D00;
			return priority;
		}

		/*
		 * Get the priority a target structure has for a given unit. The higher the value,
		 *  the more serious it should look at the target.
		 *
		 * @param unit The unit looking at a target.
		 * @param target The structure to look at.
		 * @return The priority of the target.
		 */
		internal static ushort Unit_GetTargetStructurePriority(Unit unit, Structure target)
		{
			StructureInfo si;
			ushort priority;
			ushort distance;

			if (unit == null || target == null) return 0;

			if (CHouse.House_AreAllied(Unit_GetHouseID(unit), target.o.houseID)) return 0;
			if ((target.o.seenByHouses & (1 << Unit_GetHouseID(unit))) == 0) return 0;

			si = CStructure.g_table_structureInfo[target.o.type];
			priority = (ushort)(si.o.priorityBuild + si.o.priorityTarget);
			distance = CTile.Tile_GetDistanceRoundedUp(unit.o.position, target.o.position);
			if (distance != 0) priority /= distance;

			return Min(priority, (ushort)32000);
		}

		static readonly short[] offsetX = { 0, 0, 200, 256, 200, 0, -200, -256, -200, 0, 400, 512, 400, 0, -400, -512, -400 };
		static readonly short[] offsetY = { 0, -256, -200, 0, 200, 256, 200, 0, -200, -512, -400, 0, 400, 512, 400, 0, -400 };
		/*
		 * Moves the given unit.
		 *
		 * @param unit The Unit to move.
		 * @param distance The maximum distance to pass through.
		 * @return ??.
		 */
		static bool Unit_Move(Unit unit, ushort distance)
		{
			UnitInfo ui;
			ushort d;
			ushort packed;
			tile32 newPosition;
			bool ret;
			tile32 currentDestination;
			var isSpiceBloom = false;
			var isSpecialBloom = false;

			if (unit == null || !unit.o.flags.used) return false;

			ui = g_table_unitInfo[unit.o.type];

			newPosition = CTile.Tile_MoveByDirection(unit.o.position, unit.orientation[0].current, distance);

			if ((newPosition.x == unit.o.position.x) && (newPosition.y == unit.o.position.y)) return false;

			if (!CTile.Tile_IsValid(newPosition))
			{
				if (!ui.flags.mustStayInMap)
				{
					Unit_Remove(unit);
					return true;
				}

				if (unit.o.flags.byScenario && unit.o.linkedID == 0xFF && unit.o.script.variables[4] == 0)
				{
					Unit_Remove(unit);
					return true;
				}

				newPosition = unit.o.position;
				Unit_SetOrientation(unit, (sbyte)(unit.orientation[0].current + (Tools.Tools_Random_256() & 0xF)), false, 0);
			}

			unit.wobbleIndex = 0;
			if (ui.flags.canWobble && unit.o.flags.isWobbling)
			{
				unit.wobbleIndex = (byte)(Tools.Tools_Random_256() & 7);
			}

			d = CTile.Tile_GetDistance(newPosition, unit.currentDestination);
			packed = CTile.Tile_PackTile(newPosition);

			if (ui.flags.isTracked && d < 48)
			{
				Unit u;
				u = Unit_Get_ByPackedTile(packed);

				/* Driving over a foot unit */
				if (u != null && g_table_unitInfo[u.o.type].movementType == (ushort)MovementType.MOVEMENT_FOOT && u.o.flags.allocated)
				{
					if (u == g_unitSelected) Unit_Select(null);

					Unit_UntargetMe(u);
					u.o.script.variables[1] = 1;
					Unit_SetAction(u, ActionType.ACTION_DIE);
				}
				else
				{
					var type = Map.Map_GetLandscapeType(packed);
					/* Produce tracks in the sand */
					if ((type == (ushort)LandscapeType.LST_NORMAL_SAND || type == (ushort)LandscapeType.LST_ENTIRELY_DUNE) && Map.g_map[packed].overlayTileID == 0)
					{
						var animationID = CTile.Orientation_Orientation256ToOrientation8((byte)unit.orientation[0].current);

						Debug.Assert(animationID < 8);
						CAnimation.Animation_Start(CAnimation.g_table_animation_unitMove[animationID], unit.o.position, 0, unit.o.houseID, 5);
					}
				}
			}

			Unit_UpdateMap(0, unit);

			if (ui.movementType == (ushort)MovementType.MOVEMENT_WINGER)
			{
				unit.o.flags.animationFlip = !unit.o.flags.animationFlip;
			}

			currentDestination = unit.currentDestination;
			distance = CTile.Tile_GetDistance(newPosition, currentDestination);

			if (unit.o.type == (byte)UnitType.UNIT_SONIC_BLAST)
			{
				Unit u;
				ushort damage;

				damage = (ushort)((unit.o.hitpoints / 4) + 1);
				ret = false;

				u = Unit_Get_ByPackedTile(packed);

				if (u != null)
				{
					if (!g_table_unitInfo[u.o.type].flags.sonicProtection)
					{
						Unit_Damage(u, damage, 0);
					}
				}
				else
				{
					Structure s;

					s = CStructure.Structure_Get_ByPackedTile(packed);

					if (s != null)
					{
						/* ENHANCEMENT -- make sonic blast trigger counter attack, but
						 * do not warn about base under attack (original behaviour). */
						if (CSharpDune.g_dune2_enhanced && s.o.houseID != (byte)CHouse.g_playerHouseID && !CHouse.House_AreAllied(unit.o.houseID, s.o.houseID))
						{
							CStructure.Structure_HouseUnderAttack(s.o.houseID);
						}

						CStructure.Structure_Damage(s, damage, 0);
					}
					else
					{
						if (Map.Map_GetLandscapeType(packed) == (ushort)LandscapeType.LST_WALL && CStructure.g_table_structureInfo[(int)StructureType.STRUCTURE_WALL].o.hitpoints > damage) Tools.Tools_Random_256();
					}
				}

				if (unit.o.hitpoints < (ui.damage / 2))
				{
					unit.o.flags.bulletIsBig = true;
				}

				if (--unit.o.hitpoints == 0 || unit.fireDelay == 0)
				{
					Unit_Remove(unit);
				}
			}
			else
			{
				if (unit.o.type == (byte)UnitType.UNIT_BULLET)
				{
					var type = Map.Map_GetLandscapeType(CTile.Tile_PackTile(newPosition));
					if (type == (ushort)LandscapeType.LST_WALL || type == (ushort)LandscapeType.LST_STRUCTURE)
					{
						if (Tools.Tools_Index_GetType(unit.originEncoded) == IndexType.IT_STRUCTURE)
						{
							if (Map.g_map[CTile.Tile_PackTile(newPosition)].houseID == unit.o.houseID)
							{
								type = (ushort)LandscapeType.LST_NORMAL_SAND;
							}
						}
					}

					if (type == (ushort)LandscapeType.LST_WALL || type == (ushort)LandscapeType.LST_STRUCTURE || type == (ushort)LandscapeType.LST_ENTIRELY_MOUNTAIN)
					{
						unit.o.position = newPosition;

						Map.Map_MakeExplosion((ushort)((ui.explosionType + unit.o.hitpoints / 10) & 3), unit.o.position, unit.o.hitpoints, unit.originEncoded);

						Unit_Remove(unit);
						return true;
					}
				}

				ret = (unit.distanceToDestination < distance || distance < 16);

				if (ret)
				{
					if (ui.flags.isBullet)
					{
						if (unit.fireDelay == 0 || unit.o.type == (byte)UnitType.UNIT_MISSILE_TURRET)
						{
							if (unit.o.type == (byte)UnitType.UNIT_MISSILE_HOUSE)
							{
								byte i;

								for (i = 0; i < 17; i++)
								{
									var p = newPosition;
									p.y += (ushort)offsetY[i];
									p.x += (ushort)offsetX[i];

									if (CTile.Tile_IsValid(p))
									{
										Map.Map_MakeExplosion(ui.explosionType, p, 200, 0);
									}
								}
							}
							else if (ui.explosionType != 0xFFFF)
							{
								if (ui.flags.impactOnSand && Map.g_map[CTile.Tile_PackTile(unit.o.position)].index == 0 && Map.Map_GetLandscapeType(CTile.Tile_PackTile(unit.o.position)) == (ushort)LandscapeType.LST_NORMAL_SAND)
								{
									Map.Map_MakeExplosion((ushort)ExplosionType.EXPLOSION_SAND_BURST, newPosition, unit.o.hitpoints, unit.originEncoded);
								}
								else if (unit.o.type == (byte)UnitType.UNIT_MISSILE_DEVIATOR)
								{
									Map_DeviateArea(ui.explosionType, newPosition, 32, unit.o.houseID);
								}
								else
								{
									Map.Map_MakeExplosion((ushort)((ui.explosionType + unit.o.hitpoints / 20) & 3), newPosition, unit.o.hitpoints, unit.originEncoded);
								}
							}

							Unit_Remove(unit);
							return true;
						}
					}
					else if (ui.flags.isGroundUnit)
					{
						if (currentDestination.x != 0 || currentDestination.y != 0) newPosition = currentDestination;
						unit.targetPreLast = unit.targetLast;
						unit.targetLast = unit.o.position;
						unit.currentDestination.x = 0;
						unit.currentDestination.y = 0;

						if (unit.o.flags.degrades && (Tools.Tools_Random_256() & 3) == 0)
						{
							Unit_Damage(unit, 1, 0);
						}

						if (unit.o.type == (byte)UnitType.UNIT_SABOTEUR)
						{
							var detonate = (Map.Map_GetLandscapeType(CTile.Tile_PackTile(newPosition)) == (ushort)LandscapeType.LST_WALL);

							if (!detonate)
							{
								/* ENHANCEMENT -- Saboteurs tend to forget their goal, depending on terrain and game speed: to blow up on reaching their destination. */
								if (CSharpDune.g_dune2_enhanced)
								{
									detonate = (unit.targetMove != 0 && CTile.Tile_GetDistance(newPosition, Tools.Tools_Index_GetTile(unit.targetMove)) < 16);
								}
								else
								{
									detonate = (unit.targetMove != 0 && CTile.Tile_GetDistance(unit.o.position, Tools.Tools_Index_GetTile(unit.targetMove)) < 32);
								}
							}

							if (detonate)
							{
								Map.Map_MakeExplosion((ushort)ExplosionType.EXPLOSION_SABOTEUR_DEATH, newPosition, 500, 0);

								Unit_Remove(unit);
								return true;
							}
						}

						Unit_SetSpeed(unit, 0);

						if (unit.targetMove == Tools.Tools_Index_Encode(packed, IndexType.IT_TILE))
						{
							unit.targetMove = 0;
						}

						{
							Structure s;

							s = CStructure.Structure_Get_ByPackedTile(packed);
							if (s != null)
							{
								unit.targetPreLast.x = 0;
								unit.targetPreLast.y = 0;
								unit.targetLast.x = 0;
								unit.targetLast.y = 0;
								Unit_EnterStructure(unit, s);
								return true;
							}
						}

						if (unit.o.type != (byte)UnitType.UNIT_SANDWORM)
						{
							if (Map.g_map[packed].groundTileID == Sprites.g_bloomTileID || Map.g_map[packed].groundTileID == Sprites.g_bloomTileID + 1)
							{
								isSpiceBloom = true;
							}
						}
					}
				}
			}

			unit.distanceToDestination = distance;
			unit.o.position = newPosition;

			Unit_UpdateMap(1, unit);

			if (isSpecialBloom) Map.Map_Bloom_ExplodeSpecial(packed, Unit_GetHouseID(unit));
			if (isSpiceBloom) Map.Map_Bloom_ExplodeSpice(packed, Unit_GetHouseID(unit));

			return ret;
		}

		/*
		 * Make a deviator missile explosion on the given position, of a certain type. All units in the
		 *  given radius may become deviated.
		 * @param type The type of explosion.
		 * @param position The position of the explosion.
		 * @param radius The radius.
		 * @param houseID House controlling the deviator.
		 */
		static void Map_DeviateArea(ushort type, tile32 position, ushort radius, byte houseID)
		{
			var find = new PoolFindStruct();

			CExplosion.Explosion_Start(type, position);

			find.type = 0xFFFF;
			find.index = 0xFFFF;
			find.houseID = (byte)HouseType.HOUSE_INVALID;

			while (true)
			{
				Unit u;

				u = PoolUnit.Unit_Find(find);

				if (u == null) break;
				if (CTile.Tile_GetDistance(position, u.o.position) / 16 >= radius) continue;

				Unit_Deviate(u, 0, houseID);
			}
		}

		/*
		 * Deviate the given unit.
		 *
		 * @param unit The Unit to deviate.
		 * @param probability The probability for deviation to succeed.
		 * @param houseID House controlling the deviator.
		 * @return True if and only if the unit beacame deviated.
		 */
		static bool Unit_Deviate(Unit unit, ushort probability, byte houseID)
		{
			UnitInfo ui;

			if (unit == null) return false;

			ui = g_table_unitInfo[unit.o.type];

			if (!ui.flags.isNormalUnit) return false;
			if (unit.deviated != 0) return false;
			if (ui.flags.isNotDeviatable) return false;

			if (probability == 0) probability = CHouse.g_table_houseInfo[unit.o.houseID].toughness;

			if (unit.o.houseID != (byte)CHouse.g_playerHouseID)
			{
				probability -= (ushort)(probability / 8);
			}

			if (Tools.Tools_Random_256() >= probability) return false;

			unit.deviated = 120;
			unit.deviatedHouse = houseID;

			Unit_UpdateMap(2, unit);

			if ((byte)CHouse.g_playerHouseID == unit.deviatedHouse)
			{
				Unit_SetAction(unit, (ActionType)ui.o.actionsPlayer[3]);
			}
			else
			{
				Unit_SetAction(unit, (ActionType)ui.actionAI);
			}

			Unit_UntargetMe(unit);
			unit.targetAttack = 0;
			unit.targetMove = 0;

			return true;
		}

		/*
		 * Handles what happens when the given unit enters into the given structure.
		 *
		 * @param unit The Unit.
		 * @param s The Structure.
		 */
		internal static void Unit_EnterStructure(Unit unit, Structure s)
		{
			StructureInfo si;
			UnitInfo ui;

			if (unit == null || s == null) return;

			if (unit == g_unitSelected)
			{
				/* ENHANCEMENT -- When a Unit enters a Structure, the last tile the Unit was on becomes selected rather than the entire Structure. */
				if (CSharpDune.g_dune2_enhanced)
				{
					Map.Map_SetSelection(CTile.Tile_PackTile(s.o.position));
				}
				else
				{
					Unit_Select(null);
				}
			}

			ui = g_table_unitInfo[unit.o.type];
			si = CStructure.g_table_structureInfo[s.o.type];

			if (!unit.o.flags.allocated || s.o.hitpoints == 0)
			{
				Unit_Remove(unit);
				return;
			}

			unit.o.seenByHouses |= s.o.seenByHouses;
			Unit_Hide(unit);

			if (CHouse.House_AreAllied(s.o.houseID, Unit_GetHouseID(unit)))
			{
				CStructure.Structure_SetState(s, (short)(si.o.flags.busyStateIsIncoming ? StructureState.STRUCTURE_STATE_READY : StructureState.STRUCTURE_STATE_BUSY));

				if (s.o.type == (byte)StructureType.STRUCTURE_REPAIR)
				{
					ushort countDown;

					countDown = (ushort)(((ui.o.hitpoints - unit.o.hitpoints) * 256 / ui.o.hitpoints) * (ui.o.buildTime << 6) / 256);

					if (countDown > 1)
					{
						s.countDown = countDown;
					}
					else
					{
						s.countDown = 1;
					}
					unit.o.hitpoints = ui.o.hitpoints;
					unit.o.flags.isSmoking = false;
					unit.spriteOffset = 0;
				}
				unit.o.linkedID = s.o.linkedID;
				s.o.linkedID = (byte)(unit.o.index & 0xFF);
				return;
			}

			if (unit.o.type == (byte)UnitType.UNIT_SABOTEUR)
			{
				CStructure.Structure_Damage(s, 500, 1);
				Unit_Remove(unit);
				return;
			}

			/* Take over the building when low on hitpoints */
			if (s.o.hitpoints < si.o.hitpoints / 4)
			{
				House h;

				h = PoolHouse.House_Get_ByIndex(s.o.houseID);
				s.o.houseID = Unit_GetHouseID(unit);
				h.structuresBuilt = CStructure.Structure_GetStructuresBuilt(h);

				/* ENHANCEMENT -- recalculate the power and credits for the house losing the structure. */
				if (CSharpDune.g_dune2_enhanced) CHouse.House_CalculatePowerAndCredit(h);

				h = PoolHouse.House_Get_ByIndex(s.o.houseID);
				h.structuresBuilt = CStructure.Structure_GetStructuresBuilt(h);

				if (s.o.linkedID != 0xFF)
				{
					var u = PoolUnit.Unit_Get_ByIndex(s.o.linkedID);
					if (u != null) u.o.houseID = Unit_GetHouseID(unit);
				}

				CHouse.House_CalculatePowerAndCredit(PoolHouse.House_Get_ByIndex(s.o.houseID));
				CStructure.Structure_UpdateMap(s);

				/* ENHANCEMENT -- When taking over a structure, untarget it. Else you will destroy the structure you just have taken over very easily */
				if (CSharpDune.g_dune2_enhanced) CStructure.Structure_UntargetMe(s);

				/* ENHANCEMENT -- When taking over a structure, unveil the fog around the structure. */
				if (CSharpDune.g_dune2_enhanced) CStructure.Structure_RemoveFog(s);
			}
			else
			{
				CStructure.Structure_Damage(s, (ushort)Min(unit.o.hitpoints * 2, s.o.hitpoints / 2), 1);
			}

			CObject.Object_Script_Variable4_Clear(s.o);

			Unit_Remove(unit);
		}

		/*
		 * Hide a unit from the viewport. Happens when a unit enters a structure or
		 *  gets picked up by a carry-all.
		 *
		 * @param unit The Unit to hide.
		 */
		internal static void Unit_Hide(Unit unit)
		{
			if (unit == null) return;

			unit.o.flags.bulletIsBig = true;
			Unit_UpdateMap(0, unit);
			unit.o.flags.bulletIsBig = false;

            Script_Reset(unit.o.script, g_scriptUnit);
			Unit_UntargetMe(unit);

			unit.o.flags.isNotOnMap = true;
			Unit_HouseUnitCount_Remove(unit);
		}

		static readonly short[] around = { 0, -1, 1, -64, 64, -65, -63, 65, 63 };
		/*
		 * Find a target around the given packed tile.
		 *
		 * @param packed The packed tile around where to look.
		 * @return A packed tile where a Unit/Structure is, or the given packed tile if nothing found.
		 */
		internal static ushort Unit_FindTargetAround(ushort packed)
		{
			byte i;

			if (CSharpDune.g_selectionType == (ushort)SelectionType.PLACE) return packed;

			if (CStructure.Structure_Get_ByPackedTile(packed) != null) return packed;

			if (Map.Map_GetLandscapeType(packed) == (ushort)LandscapeType.LST_BLOOM_FIELD) return packed;

			for (i = 0; i < around.Length; i++)
			{
				Unit u;

				u = Unit_Get_ByPackedTile((ushort)(packed + around[i]));
				if (u == null) continue;

				return CTile.Tile_PackTile(u.o.position);
			}

			return packed;
		}

		/*
		 * Convert the name of a movement to the type value of that movement, or
		 *  MOVEMENT_INVALID if not found.
		 */
		internal static byte Unit_MovementStringToType(string name)
		{
			byte type;
			if (name == null) return (byte)MovementType.MOVEMENT_INVALID;

			for (type = 0; type < (byte)MovementType.MOVEMENT_MAX; type++)
			{
				if (string.Equals(CMovementType.g_table_movementTypeName[type], name, StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(g_table_movementTypeName[type], name) == 0)
					return type;
			}

			return (byte)MovementType.MOVEMENT_INVALID;
		}

		/*
		 * Convert the name of an action to the type value of that action, or
		 *  ACTION_INVALID if not found.
		 */
		internal static byte Unit_ActionStringToType(string name)
		{
			byte type;
			if (name == null) return (byte)ActionType.ACTION_INVALID;

			for (type = 0; type < (byte)ActionType.ACTION_MAX; type++)
			{
				if (string.Equals(g_table_actionInfo[type].name, name, StringComparison.OrdinalIgnoreCase)) //if (strcasecmp(g_table_actionInfo[type].name, name) == 0) return type;
					return type;
			}

			return (byte)ActionType.ACTION_INVALID;
		}
	}
}
