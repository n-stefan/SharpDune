﻿/* Strings table */

namespace SharpDune.Table;

static class TableStrings
{
    internal enum Text
    {
        STR_NULL = 0,
        STR_ATTACK = 1,
        STR_MOVE = 2,
        STR_RETREAT = 3,
        STR_GUARD = 4,
        STR_AREA_GUARD = 5,
        STR_HARVEST = 6,
        STR_RETURN = 7,
        STR_STOP = 8,
        STR_AMBUSH = 9,
        STR_SABOTAGE = 10,
        STR_DIE = 11,
        STR_HUNT = 12,
        STR_TARGET = 13,
        STR_MOVEMENT = 14,
        STR_INITIALIZING_THE_MT32 = 15,
        STR_OLD_SAVE_GAME_FILE_IS_INCOMPATABLE_WITH_LATEST_VERSION = 16,
        STR_INSUFFICIENT_MEMORY_BY_LD_BYTES = 17,
        STR_THE_SETUP_PROGRAM_MUST_BE_RUN_FIRST = 18,
        STR_S_S_DESTROYED = 19,
        STR_UNIT_IS_UNABLE_TO_DEPLOY_HERE = 20,
        STR_SCORE_D = 21,
        STR_TIME_DH_DM = 22,
        STR_YOU_HAVE_ATTAINED_THE_RANK_OF = 23,
        STR_UNITS_DESTROYED_BY = 24,
        STR_BUILDINGS_DESTROYED_BY = 25,
        STR_SPICE_HARVESTED_BY = 26,
        STR_PLAY_A_GAME = 27,
        STR_REPLAY_INTRODUCTION = 28,
        STR_EXIT_GAME = 29,
        STR_CANCEL = 30,
        STR_DEPLOY = 31,
        STR_LOAD = 32,
        STR_NEXT = 33,
        STR_REPAIR = 34,
        STR_REPAIRING = 35,
        STR_SAVE = 36,
        STR_STOP2 = 37,
        STR_PLACE_IT = 38,
        STR_COMPLETED = 39,
        STR_ON_HOLD = 40,
        STR_BUILD_IT = 41,
        STR_LAUNCH = 42,
        STR_FREMEN = 43,
        STR_SABOTEUR = 44,
        STR_PRESPICE_MOUND = 45,
        STR_D_DONE = 46,
        STR_MENTAT_BRIEFING = 47,
        STR_SELECT_SUBJECT = 48,
        STR_DMG = 49,
        STR_HARVESTER_IS_HEADING_TO_REFINERY = 50,
        STR_CONSTRUCTION_COMPLETE = 51,
        STR_ENEMY_UNIT_APPROACHING = 52,
        STR_HARKONNEN_APPROACHING = 53,
        STR_ATREIDES_APPROACHING = 54,
        STR_ORDOS_APPROACHING = 55,
        STR_FREMEN_APPROACHING = 56,
        STR_SARDAUKAR_APPROACHING = 57,
        STR_MERCENARY_APPROACHING = 58,
        STR_SABOTEUR_APPROACHING = 59,
        STR_HARKONNEN_UNIT_DESTROYED = 60,
        STR_ATREIDES_UNIT_DESTROYED = 61,
        STR_ORDOS_UNIT_DESTROYED = 62,
        STR_FREMEN_UNIT_DESTROYED = 63,
        STR_SARDAUKAR_UNIT_DESTROYED = 64,
        STR_MERCENARY_UNIT_DESTROYED = 65,
        STR_HARKONNEN_STRUCTURE_DESTROYED = 66,
        STR_ATREIDES_STRUCTURE_DESTROYED = 67,
        STR_ORDOS_STRUCTURE_DESTROYED = 68,
        STR_FREMEN_STRUCTURE_DESTROYED = 69,
        STR_SARDAUKAR_STRUCTURE_DESTROYED = 70,
        STR_MERCENARY_STRUCTURE_DESTROYED = 71,
        STR_HARKONNEN_UNIT_DEPLOYED = 72,
        STR_ATREIDES_UNIT_DEPLOYED = 73,
        STR_ORDOS_UNIT_DEPLOYED = 74,
        STR_FREMEN_UNIT_DEPLOYED = 75,
        STR_SARDAUKAR_UNIT_DEPLOYED = 76,
        STR_MERCENARY_UNIT_DEPLOYED = 77,
        STR_WORMSIGN = 78,
        STR_SABOTEUR_APPROACHING2 = 79,
        STR_FRIGATE_ARRIVED = 80,
        STR_MISSILE_APPROACHING = 81,
        STR_YOU_HAVE_SUCCESSFULLY_COMPLETED_YOUR_MISSION = 82,
        STR_YOU_HAVE_FAILED_YOUR_MISSION = 83,
        STR_SELECTLOCATION_TOBUILD = 84,
        STR_PICK_TARGETTMINUS_D = 85,
        STR_SELECTTARGET = 86,
        STR_SELECTDESTINATION = 87,
        STR_SPICEHOLDS_4DMAX_4D = 88,
        STR_POWER_INFONEEDEDOUTPUT = 89,
        STR_BASE_IS_UNDER_ATTACK = 90,
        STR_DUNE_II_THE_BATTLE_FOR_ARRAKIS = 91,
        STR_LOAD_A_GAME = 92,
        STR_SAVE_THIS_GAME = 93,
        STR_GAME_CONTROLS = 94,
        STR_QUIT_PLAYING = 95,
        STR_CONTINUE_GAME = 96,
        STR_SELECT_A_SAVED_GAME_TO_LOAD = 97,
        STR_SELECT_A_POSITION_TO_SAVE_TO = 98,
        STR_EMPTY_SLOT_ = 99,
        STR_ENTER_A_DESCRIPTION_OF_YOUR_SAVED_GAME = 100,
        STR_ARE_YOU_SURE_YOU_WANT_TO_QUIT_PLAYING = 101,
        STR_PREVIOUS = 102,
        STR_RESTART_SCENARIO = 103,
        STR_PICK_ANOTHER_HOUSE = 104,
        STR_ON = 105,
        STR_OFF = 106,
        STR_YES = 107,
        STR_NO = 108,
        STR_HINTS_ARE = 109,
        STR_GAME_SPEED = 110,
        STR_MUSIC_IS = 111,
        STR_SOUNDS_ARE = 112,
        STR_SLOWEST = 113,
        STR_SLOW = 114,
        STR_NORMAL = 115,
        STR_FAST = 116,
        STR_FASTEST = 117,
        STR_ARE_YOU_SURE_YOU_WISH_TO_RESTART = 118,
        STR_ARE_YOU_SURE_YOU_WISH_TO_PICK_A_NEW_HOUSE = 119,
        STR_AUTO_SCROLL_IS = 120,
        STR_IS_D_PERCENT_FULL = 121,
        STR_IS_D_PERCENT_FULL_AND_HARVESTING = 122,
        STR_IS_D_PERCENT_FULL_AND_HEADING_BACK = 123,
        STR_IS_D_PERCENT_FULL_AND_AWAITING_PICKUP = 124,
        STR_IS_EMPTY = 125,
        STR_IS_EMPTY_AND_HARVESTING = 126,
        STR_IS_EMPTY_AND_HEADING_BACK = 127,
        STR_IS_EMPTY_AND_AWAITING_PICKUP = 128,
        STR_IS_COMPLETE = 129,
        STR_IS_COMPLETED_AND_READY_TO_PLACE = 130,
        STR_IS_COMPLETED_AND_AWAITING_ORDERS = 131,
        STR_INSUFFICIENT_FUNDS_CONSTRUCTION_IS_HALTED = 132,
        STR_IS_DESTROYED = 133,
        STR_CAN_NOT_PLACE_S_HERE = 134,
        STR_CAN_NOT_PLACE_FOUNDATION_HERE = 135,
        STR_UNABLE_TO_CREATE_MORE = 136,
        STR_PRODUCTION_OF_S_HAS_STARTED = 137,
        STR_REPAIRING_STOPS = 138,
        STR_REPAIRING_STARTS = 139,
        STR_UPGRADING_STOPS = 140,
        STR_UPGRADING_STARTS = 141,
        STR_UPGRADE = 142,
        STR_UPGRADING = 143,
        STR_UPGRADINGD_DONE = 144,
        STR_INSUFFICIENT_SPICE_STORAGE_AVAILABLE_SPICE_IS_LOST = 145,
        STR_RADAR_SCANFRIEND_2DENEMY_2D = 146,
        STR_HARKONNEN_HARVESTER_DEPLOYED = 147,
        STR_ATREIDES_HARVESTER_DEPLOYED = 148,
        STR_ORDOS_HARVESTER_DEPLOYED = 149,
        STR_FREMEN_HARVESTER_DEPLOYED = 150,
        STR_SARDAUKAR_HARVESTER_DEPLOYED = 151,
        STR_MERCENARY_HARVESTER_DEPLOYED = 152,
        STR_DESTRUCT = 153,
        STR_HARKONNEN_UNIT_LAUNCHED = 154,
        STR_ATREIDES_UNIT_LAUNCHED = 155,
        STR_ORDOS_UNIT_LAUNCHED = 156,
        STR_FREMEN_UNIT_LAUNCHED = 157,
        STR_SARDAUKAR_UNIT_LAUNCHED = 158,
        STR_MERCENARY_UNIT_LAUNCHED = 159,
        STR_FRIGATE_INORBIT_ANDAWAITINGORDER = 160,
        STR_FRIGATEARRIVAL_INTMINUS_D = 161,
        STR_HARKONNEN_VEHICLE_REPAIRED = 162,
        STR_ATREIDES_VEHICLE_REPAIRED = 163,
        STR_ORDOS_VEHICLE_REPAIRED = 164,
        STR_FREMEN_VEHICLE_REPAIRED = 165,
        STR_SARDAUKAR_VEHICLE_REPAIRED = 166,
        STR_MERCENARY_VEHICLE_REPAIRED = 167,
        STR_SAND = 168,
        STR_ROCK = 169,
        STR_DUNE = 170,
        STR_MOUNTAIN = 171,
        STR_SPICE = 172,
        STR_SPICE_BLOOM = 173,
        STR_STRUCTURE = 174,
        STR_PROCEED = 175,
        STR_REPEAT = 176,
        STR_COST_3D = 177,
        STR_COST_999 = 178,
        STR_QTY_2D = 179,
        STR_QTY_99 = 180,
        STR_NO_UNITS_ON_ORDER = 181,
        STR_ITEM_NAME_QTY_TOTAL = 182,
        STR_INVOICE_OF_UNITS_ON_ORDER = 183,
        STR_TOTAL_COST_ = 184,
        STR_OUT_OF_STOCK = 185,
        STR_NEED_STRUCTURE_UPGRADE = 186,
        STR_UPGRADE_COST_D = 187,
        STR_RESUME_GAME = 188,
        STR_UPGRADE2 = 189,
        STR_SEND_ORDER = 190,
        STR_BUILD_THIS = 191,
        STR_INVOICE = 192,
        STR_EXIT = 193,
        STR_CARRYALL = 194,
        STR_ALLPURPOSE_CARRYALL = 195,
        STR_THOPTER = 196,
        STR_ORNITHIPTER = 197,
        STR_INFANTRY = 198,
        STR_LIGHT_INFANTRY_SQUAD = 199,
        STR_TROOPERS = 200,
        STR_HEAVY_TROOPER_SQUAD = 201,
        STR_SOLDIER = 202,
        STR_INFANTRY_SOLDIER = 203,
        STR_TROOPER = 204,
        STR_HEAVY_TROOPER = 205,
        STR_SABOTEUR2 = 206,
        STR_INSIDIOUS_SABOTEUR = 207,
        STR_LAUNCHER = 208,
        STR_ROCKET_LAUNCHER = 209,
        STR_DEVIATOR = 210,
        STR_DEVIATOR_LAUNCHER = 211,
        STR_TANK = 212,
        STR_COMBAT_TANK = 213,
        STR_SIEGE_TANK = 214,
        STR_HEAVY_SIEGE_TANK = 215,
        STR_DEVASTATOR = 216,
        STR_DEVASTATOR_TANK = 217,
        STR_SONIC_TANK = 218,
        STR_SONIC_WAVE_TANK = 219,
        STR_TRIKE = 220,
        STR_LIGHT_ATTACK_TRIKE = 221,
        STR_RAIDER_TRIKE = 222,
        STR_FAST_RAIDER_TRIKE = 223,
        STR_QUAD = 224,
        STR_HEAVY_ATTACK_QUAD = 225,
        STR_HARVESTER = 226,
        STR_SPICE_HARVESTER = 227,
        STR_MCV = 228,
        STR_MOBILE_CONST_VEHICLE = 229,
        STR_SANDWORM = 230,
        STR_SANDWORM2 = 231,
        STR_CONCRETE = 232,
        STR_SMALL_CONCRETE_SLAB = 233,
        STR_CONCRETE_4 = 234,
        STR_LARGE_CONCRETE_SLAB = 235,
        STR_PALACE = 236,
        STR_HOUSE_PALACE = 237,
        STR_LIGHT_FCTRY = 238,
        STR_LIGHT_VEHICLE_FACTORY = 239,
        STR_HEAVY_FCTRY = 240,
        STR_HEAVY_VEHICLE_FACTORY = 241,
        STR_HITECH = 242,
        STR_HITECH_FACTORY = 243,
        STR_IX = 244,
        STR_HOUSE_OF_IX = 245,
        STR_WOR = 246,
        STR_WOR_TROOPER_FACILITY = 247,
        STR_CONST_YARD = 248,
        STR_CONSTRUCTION_YARD = 249,
        STR_WINDTRAP = 250,
        STR_WINDTRAP_POWER_CENTER = 251,
        STR_BARRACKS = 252,
        STR_INFANTRY_BARRACKS = 253,
        STR_STARPORT = 254,
        STR_STARPORT_FACILITY = 255,
        STR_REFINERY = 256,
        STR_SPICE_REFINERY = 257,
        STR_REPAIR2 = 258,
        STR_REPAIR_FACILITY = 259,
        STR_WALL = 260,
        STR_BASE_DEFENSE_WALL = 261,
        STR_TURRET = 262,
        STR_CANNON_TURRET = 263,
        STR_RTURRET = 264,
        STR_ROCKET_TURRET = 265,
        STR_SPICE_SILO = 266,
        STR_SPICE_STORAGE_SILO = 267,
        STR_OUTPOST = 268,
        STR_RADAR_OUTPOST = 269,
        STR_INSUFFICIENT_POWER_WINDTRAP_IS_NEEDED = 270,
        STR_SAND_FLEA = 271,
        STR_SAND_SNAKE = 272,
        STR_DESERT_MONGOOSE = 273,
        STR_SAND_WARRIOR = 274,
        STR_DUNE_TROOPER = 275,
        STR_SQUAD_LEADER = 276,
        STR_OUTPOST_COMMANDER = 277,
        STR_BASE_COMMANDER = 278,
        STR_WARLORD = 279,
        STR_CHIEF_WARLORD = 280,
        STR_RULER_OF_ARRAKIS = 281,
        STR_EMPEROR = 282,
        STR_THREE_HOUSES_HAVE_COME_TO_DUNE = 283,
        STR_TO_TAKE_CONTROL_OF_THE_LAND = 284,
        STR_THAT_HAS_BECOME_DIVIDED = 285,
        STR_SELECT_YOUR_NEXT_REGION = 286,
        STR_AT_THE_EMPERORS_PALACE = 287,
        STR_YOU_OF_ALL_PEOPLE_SHOULDUNDERSTAND_THE_IMPORTANCE_OF_VICTORY = 288,
        STR_YES_YOUR_EXCELLENCY_I = 289,
        STR_YOU_LET_THE_ATREIDES_DEFEATYOU_AND_MY_SARDAUKAR = 290,
        STR_I_DID_NOT_LET = 291,
        STR_I_WILL_NOT_ALLOW_IT_TO_HAPPEN_AGAIN = 292,
        STR_AT_THE_EMPERORS_PALACEON_DUNE = 293,
        STR_FOOLSI_GAVE_YOU_WEAPONS_AND_TROOPS = 294,
        STR_AND_STILL_YOU_FAILTO_DEFEAT_THE_ATREIDES = 295,
        STR_BUT_EXCELL = 296,
        STR_ENOUGH_TOGETHER_WE_MUST_MAKESURE_THE_ATREIDES_DO_NOT_SUCCEED = 297,
        STR_AT_THE_EMPERORS_PALACE2 = 298,
        STR_I_CANNOT_BELIEVE_THEINCOMPETENCE_I_SEE_BEFORE_ME = 299,
        STR_YOUR_HIGHNESS = 300,
        STR_I_GIVE_YOU_MY_SARDAUKAR_TO_ASSISTAGAINST_THE_ORDOS_AND_YOU_FAILED_ME = 301,
        STR_WE = 302,
        STR_DO_NOT_FAIL_ME_AGAINYOU_ARE_DISMISSED = 303,
        STR_AT_THE_EMPERORS_PALACEON_DUNE2 = 304,
        STR_THE_ORDOS_WERE_NOTSUPPOSED_TO_GET_THIS_FAR = 305,
        STR_YOUR_HIGHNESS_LET_US_EXPLAIN = 306,
        STR_NO_MORE_EXPLANATIONSYOU_ARE_TO_DEFEND_MY_PALACE = 307,
        STR_ONLY_TOGETHER_WILLWE_DEFEAT_THE_ORDOS = 308,
        STR_AT_THE_EMPERORS_PALACE3 = 309,
        STR_YOU_RECEIVE_THE_ASSISTANCEYOU_REQUIRE_AND_THEN_FAIL_ME = 310,
        STR_NO_YOUR = 311,
        STR_MY_SARDAUKAR_COULD_HELP_DEFEAT_THEHARKONNEN_BUT_YOU_WASTED_THEM = 312,
        STR_WE_DIDNT_HAVE = 313,
        STR_I_WANT_NO_EXCUSESDO_NOT_FAIL_ME_AGAIN = 314,
        STR_AT_THE_EMPERORS_PALACEON_DUNE3 = 315,
        STR_I_SHOULD_HAVE_KNOWN_YOUR_HOUSESCOULDNT_STAND_UP_TO_THE_HARKONNEN = 316,
        STR_EXCELLENCY_THEY_ARE = 317,
        STR_SILENCE = 318,
        STR_YOU_ARE_TO_DEFEND_MY_PALACE = 319,
        STR_I_WILL_SHOW_YOU_HOWTO_CRUSH_THE_HARKONNEN = 320,
        STR_THANK_YOU_FOR_PLAYING_DUNE_II = 321,
        STR_SPICE_STORAGE_CAPACITY_LOW_BUILD_SILOS = 322,
        STR_NAME_AND_RANK = 323,
        STR_BATTLE = 324,
        STR_SCORE = 325,
        STR_RESUME_GAME2 = 326,
        STR_CLEAR_LIST = 327,
        STR_ARE_YOU_SURE_YOU_WANT_TO_CLEAR_THE_HIGH_SCORES = 328,
        STR_YOU = 329,
        STR_ENEMY = 330,
        STR_CREDITS_ARE_LOW_HARVEST_SPICE_FOR_MORE_CREDITS = 331,
        STR_NOT_ENOUGH_POWER_FOR_RADAR_BUILD_WINDTRAPS = 332,
        STR_REPAIR_STRUCTURE_FIRST = 333,
        STR_HALL_OF_FAME = 334,
        STR_SELECTPLACE_TOHARVEST = 335,
        STR_HALL_OF_FAME2 = 336,
        STR_THERE_ARE_NO_SAVED_GAMES_TO_LOAD = 337,
        STR_WARNING_ORIGINAL_SAVED_GAMES_ARE_INCOMPATABLE_WITH_THE_NEW_VERSION_THE_BATTLE_WILL_BE_RESTARTED = 338,
        STR_LOAD_GAME = 339,
        STR_YOU_MUST_BUILD_A_WINDTRAP_TO_PROVIDE_POWER_TO_YOUR_BASE_WITHOUT_POWER_YOUR_STRUCTURES_WILL_DECAY = 340,
        STR_CONCRETE_USE_CONCRETE_TO_MAKE_A_STURDY_FOUNDATION_FOR_YOUR_STRUCTURES = 341,
        STR_PALACE_THIS_IS_YOUR_PALACE = 342,
        STR_LIGHT_FACTORY_THE_LIGHT_FACTORY_PRODUCES_LIGHT_ATTACK_VEHICLES = 343,
        STR_HEAVY_FACTORY_THE_HEAVY_FACTORY_PRODUCES_TRACKED_VEHICLES = 344,
        STR_HITECH_FACTORY_THE_HITECH_FACTORY_PRODUCES_FLYING_VEHICLES = 345,
        STR_HOUSE_IX_THE_IX_RESEARCH_FACILITY_ADVANCES_YOUR_HOUSES_TECHNOLOGY = 346,
        STR_WOR_WOR_IS_USED_TO_TRAIN_YOUR_HEAVY_INFANTRY = 347,
        STR_CONSTRUCTION_FACILITY_ALL_STRUCTURES_ARE_BUILT_BY_THE_CONSTRUCTION_FACILITY = 348,
        STR_WINDTRAP_THE_WINDTRAP_SUPPLIES_POWER_TO_YOUR_BASE_WITHOUT_POWER_YOUR_STRUCTURES_WILL_DECAY = 349,
        STR_BARRACKS_THE_BARRACKS_IS_USED_TO_TRAIN_YOUR_LIGHT_INFANTRY = 350,
        STR_STARTPORT_THE_STARPORT_IS_USED_TO_ORDER_AND_RECEIVE_SHIPMENTS_FROM_CHOAM = 351,
        STR_SPICE_REFINERY_THE_REFINERY_CONVERTS_SPICE_INTO_CREDITS = 352,
        STR_REPAIR_FACILITY_THE_REPAIR_FACILITY_IS_USED_TO_REPAIR_YOUR_VEHICLES = 353,
        STR_WALL_THE_WALL_IS_USED_FOR_PASSIVE_DEFENSE = 354,
        STR_GUN_TURRET_THE_CANNON_TURRET_IS_USED_FOR_SHORT_RANGE_ACTIVE_DEFENSE = 355,
        STR_ROCKET_TURRET_THE_ROCKETCANNON_TURRET_IS_USED_FOR_BOTH_SHORT_AND_MEDIUM_RANGE_ACTIVE_DEFENSE = 356,
        STR_SPICE_SILO_THE_SPICE_SILO_IS_USED_TO_STORE_REFINED_SPICE = 357,
        STR_OUTPOST_THE_OUTPOST_PROVIDES_RADAR_AND_AIDS_CONTROL_OF_DISTANT_VEHICLES = 358,
        STR_THERE_ISNT_ENOUGH_OPEN_CONCRETE_TO_PLACE_THIS_STRUCTURE_YOU_MAY_PROCEED_BUT_WITHOUT_ENOUGH_CONCRETE_THE_BUILDING_WILL_NEED_REPAIRS = 359,
        STR_SAND_THIS_IS_SAND_TERRAIN_PLENTY_OF_THIS_STUFF_ON_ARRAKIS_TO_BE_SURE = 360,
        STR_SAND_DUNES_THESE_ARE_AN_UBIQUITOUS_FEATURE_OF_ARRAKIAN_LANDSCAPE = 361,
        STR_ROCK_THIS_IS_ROCK_TERRAIN_THIS_VALUABLE_TERRAIN_IS_THE_ONLY_PLACE_STRUCTURES_CAN_BE_BUILT = 362,
        STR_MOUNTAIN_MOUNTAINS_ON_ARRAKIS_ARE_RARE_AND_AN_INCONVENIENCE = 363,
        STR_SPICE_FIELD_THIS_IS_THE_SPICE_MELANGE_IT_IS_THE_MOST_PRECIOUS_SUBSTANCE_IN_THE_UNIVERSE = 364,
        STR_STRUCTURES_MUST_BE_PLACED_ON_CLEAR_ROCK_OR_CONCRETE_AND_ADJACENT_TO_ANOTHER_FRIENDLY_STRUCTURE = 365,
        STR_SEARCH_FOR_SPICE_FIELDS_TO_HARVEST = 366,
        STR_WARNING_SANDWORMS_SHAIHULUD_ROAM_DUNE_DEVOURING_ANYTHING_ON_THE_SAND = 367,
        STR_PRESENT = 368,
        STR_THE_BATTLE_FOR_ARRAKIS = 369,
        STR_THE_PLANET_ARRAKIS_KNOWN_AS_DUNE = 370,
        STR_LAND_OF_SAND = 371,
        STR_HOME_OF_THE_SPICE_MELANGE = 372,
        STR_THE_SPICE_CONTROLS_THE_EMPIRE = 373,
        STR_WHOEVER_CONTROLS_DUNECONTROLS_THE_SPICE = 374,
        STR_THE_EMPEROR_HAS_PROPOSED_ACHALLENGE_TO_EACH_OF_THE_HOUSES = 375,
        STR_THE_HOUSE_THAT_PRODUCES_THEMOST_SPICE_WILL_CONTROL_DUNE = 376,
        STR_THERE_ARE_NO_SET_TERRITORIES = 377,
        STR_AND_NO_RULES_OF_ENGAGEMENT = 378,
        STR_VAST_ARMIES_HAVE_ARRIVED = 379,
        STR_NOW_THREE_HOUSES_FIGHTFOR_CONTROL_OF_DUNE = 380,
        STR_THE_NOBLE_ATREIDES = 381,
        STR_THE_INSIDIOUS_ORDOS = 382,
        STR_AND_THE_EVIL_HARKONNEN = 383,
        STR_ONLY_ONE_HOUSE_WILL_PREVAIL = 384,
        STR_YOUR_BATTLE_FOR_DUNE_BEGINS = 385,
        STR_NOW = 386,
        STR_GREETINGS_EMPEROR = 387,
        STR_WHAT_IS_THE_MEANINGOF_THIS_INTRUSION = 388,
        STR_YOU_ARE_FORMALLY_CHARGED_WITH_CRIMESOF_TREASON_AGAINST_HOUSE_ATREIDES = 389,
        STR_THE_HOUSE_SHALL_DETERMINEYOUR_GUILT_OR_INNOCENCE = 390,
        STR_UNTIL_THEN_YOU_SHALL_NOLONGER_REIGN_AS_EMPEROR = 391,
        STR_YOU_ARE_INDEED_NOT_ENTIRELYTRUE_TO_YOUR_WORD_EMPEROR = 392,
        STR_YOU_HAVE_LIED_TO_US = 393,
        STR_WHAT_LIES_WHAT_AREYOU_TALKING_ABOUT = 394,
        STR_YOUR_LIES_OF_LOYALTYYOUR_DECEIT = 395,
        STR_A_CRIME_FOR_WHICH_YOU_WILLINDEED_PAY_DEARLY = 396,
        STR_WITH_YOUR_LIFE = 397,
        STR_NO_NO_NOOO = 398,
        STR_YOU_ARE_AWARE_EMPEROR_THAT_WEHAVE_GROWN_WEARY_OF_YOUR_GAMES = 399,
        STR_WHAT_GAMES_WHAT_AREYOU_TALKING_ABOUT = 400,
        STR_I_AM_REFERRING_TOYOUR_GAME_OF_CHESS = 401,
        STR_WE_WERE_YOUR_PAWNS_ANDDUNE_WAS_YOUR_BOARD = 402,
        STR_WE_HAVE_DECIDED_TO_TAKE_MATTERSINTO_OUR_OWN_HANDS = 403,
        STR_YOU_ARE_TO_BE_OUR_PAWNIN_OUR_GAME = 404,
        STR_HOUSE_HARKONNENFROM_THE_DARK_WORLD_OF_GIEDI_PRIME_THE_SAVAGE_HOUSE_HARKONNEN_HAS_SPREAD_ACROSS_THE_UNIVERSE_A_CRUEL_PEOPLE_THE_HARKONNEN_ARE_RUTHLESS_TOWARDS_BOTH_FRIEND_AND_FOE_IN_THEIR_FANATICAL_PURSUIT_OF_POWER = 405,
        STR_TEST_SCENARIO_WIN_TEXT_1 = 406,
        STR_TEST_SCENARIO_LOSE_TEXT_1 = 407,
        STR_TEST_SCENARIO_ADVICE_TEXT_1 = 408,
        STR_I_AM_THE_MENTAT_RADNOR_WITH_MY_GUIDANCE_YOU_MAY_BE_ABLE_TO_ASSIST_US_IN_CONQUERING_THIS_DUSTY_LITTLE_PLANET_FOR_YOUR_FIRST_TEST_YOU_WILL_BE_EXPECTED_TO_PRODUCE_1000_CREDITS_AND_NOT_A_GRANULE_LESS_YOU_MAY_EARN_CREDITS_BY_HARVESTING_SPICE_AND_WILL_NEED_TO_BUILD_A_REFINERY_TO_CONVERT_SPICE_TO_CREDITS_IF_ANY_OF_OUR_FOOLISH_ENEMIES_ATTEMPT_TO_ATTACK_YOUR_BASE_YOU_WILL_HAVE_THE_PLEASURE_OF_SEEING_THE_INVINCIBLE_HARKONNEN_TROOPS_IN_ACTION = 409,
        STR_YOU_HAVE_PLEASED_ME_CONTINUE_TO_SERVE_ME_WELL_AND_I_WILL_SEE_TO_IT_THAT_YOU_ARE_REWARDED = 410,
        STR_YOU_ARE_BENEATH_MY_CONTEMPT_DO_YOU_KNOW_WHAT_HAPPENS_TO_THOSE_WHO_HAVE_FAILED_HOUSE_HARKONNEN = 411,
        STR_FIRST_BUILD_A_TWO_BY_TWO_GROUP_OF_CONCRETE_SLABS_THEN_BUILD_A_WINDTRAP_AND_PLACE_IT_UPON_THE_CONCRETE_DO_THE_SAME_FOR_A_REFINERY_SO_THAT_YOU_CAN_REACH_YOUR_QUOTA_OF_1000_CREDITS_2 = 412,
        STR_HOUSE_HARKONNEN_HAS_GENEROUSLY_GRANTED_YOU_A_NEW_OPPORTUNITY_TO_SERVE_US_WE_WILL_NOW_ALLOW_YOU_TO_TAKE_COMMAND_IN_A_MORE_DANGEROUS_AREA_AND_ACCUMULATE_2700_CREDITS_ALTHOUGH_THE_WORTHLESS_ATREIDES_YOU_MAY_ENCOUNTER_IN_THIS_REGION_SHOULD_ALWAYS_BE_ELIMINATED_AS_A_MATTER_OF_PRINCIPLE_THE_SPICE_QUOTA_IS_YOUR_OBJECTIVE = 413,
        STR_AGAIN_YOU_HAVE_PLEASED_ME_YOU_REMIND_ME_OF_MYSELF_IN_MY_YOUTH_YOU_MAY_INDEED_HAVE_TRUE_HARKONNEN_BLOOD_COURSING_THROUGH_YOUR_VEINS = 414,
        STR_I_CANNOT_BELIEVE_THAT_YOU_WERE_INCAPABLE_OF_ACCUMULATING_YOUR_QUOTA_IT_WAS_A_SIMPLE_TASK_THAT_SHOULD_HAVE_LET_US_EXAMINE_YOUR_LEADERSHIP_CAPABILITIES_I_SUGGEST_YOU_DO_NOT_TRY_MY_PATIENCE_FURTHER = 415,
        STR_IF_YOU_HAVE_A_WINDTRAP_THEN_BUILD_A_HOUSE_OF_WOR_STRUCTURE_IN_ORDER_TO_PRODUCE_TROOPERS_I_WOULD_DO_SO_TO_PROVIDE_A_GREATER_DEFENSE_FOR_YOUR_HARVESTING_OPERATION_2 = 416,
        STR_THE_DESPISED_ORDOS_ARE_WELL_ESTABLISHED_IN_THIS_REGION_AND_ARE_HARVESTING_SPICE_THAT_SHOULD_RIGHTFULLY_BELONG_TO_HOUSE_HARKONNEN_DESTROY_THE_ORDOS_INSTALLATIONS_IN_THIS_AREA_AND_ASSERT_CONTROL_IN_THE_NAME_OF_HOUSE_HARKONNEN = 417,
        STR_CONGRATULATIONS_YOU_ARE_MAKING_PROGRESS_SEVERAL_OF_MY_ASSOCIATES_HAVE_EXPRESSED_PESSIMISTIC_THOUGHTS_CONCERNING_YOUR_ABILITIES_AND_I_AM_GLAD_THAT_YOU_HAVE_PROVED_THEM_WRONG_LET_US_SEE_NOW_IF_MY_CONFIDENCE_IN_YOU_IS_MISPLACED = 418,
        STR_I_KNEW_IT_SEVERAL_OF_MY_ASSOCIATES_HAD_EXPRESSED_CONFIDENCE_IN_YOUR_ABILITIES_BUT_I_KNEW_THAT_YOU_WERE_TOO_WEAK_TO_BE_A_HARKONNEN_COMMANDER_GUARDS_REMOVE_THIS_STENCH_BEFORE_IT_FOULS_THE_ENTIRE_PLANET = 419,
        STR_YOU_NEED_TO_BUILD_A_LIGHT_FACTORY_TO_PRODUCE_THE_WEAPONS_NECESSARY_TO_COMPLETE_YOUR_MISSION_2 = 420,
        STR_ONE_SMALL_VICTORY_DOES_NOT_WIN_THE_WAR_ANOTHER_REGION_HAS_THE_MISFORTUNE_TO_BE_INFESTED_WITH_VERMIN_FROM_HOUSE_ORDOS_AND_YOU_MUST_NOW_REPEAT_YOUR_SUCCESS_I_HAVE_MANY_DELICATE_POLITICAL_NEGOTIATIONS_ON_MY_MIND_AND_I_DONT_NEED_TO_BE_WORRYING_ABOUT_LOOSE_ENDS = 421,
        STR_WELL_DONE_THE_ORDOS_DOGS_MAKE_AMUSING_NOISES_WHEN_THEY_DIE_DO_THEY_NOT_INDEED_WHAT_GOOD_IS_OUR_LIFE_IF_WE_CANNOT_ENJOY_THE_SIMPLE_PLEASURES = 422,
        STR_IS_THIS_THE_WAY_YOU_REPAY_MY_FAITH_IN_YOU_SUCH_A_SERIOUS_LOSS_COULD_ONLY_HAVE_BEEN_DELIBERATE_YOU_ARE_UNFIT_TO_CLEAN_OUR_LATRINES_MUCH_LESS_COMMAND_OUR_GLORIOUS_TROOPERS = 423,
        STR_THE_ADDITION_OF_TANKS_IN_YOUR_FORCES_IS_ESSENTIAL_TO_YOUR_VICTORY_IN_THIS_REGION_2 = 464,
        STR_SO_THE_EMPEROR_WAS_HELPING_THE_ORDOS_IN_YOUR_LAST_MISSION_NEVER_THE_LESS_HOUSE_ATREIDES_HAS_GROWN_STRONGER_DUE_TO_OUR_NEGLIGENCE_AND_MUST_NOW_BE_TAUGHT_A_LESSON_YOU_WILL_REMOVE_ALL_THE_ATREIDES_FROM_THIS_REGION = 425,
        STR_IN_APPRECIATION_FOR_YOUR_RECENT_PERFORMANCE_I_WILL_NOW_ALLOW_YOU_TO_RESUME_THE_STRUGGLE_WITH_THE_ORDOS_AT_LEAST_THEY_WILL_PUT_UP_A_FIGHT_WORTHY_OF_OUR_TROOPERS = 426,
        STR_PERHAPS_YOU_MISS_YOUR_MOTHER_I_AM_WORRIED_ABOUT_YOU_AND_CANNOT_LET_YOU_PROGRESS_FURTHER_UNTIL_YOU_HAVE_PROVED_YOURSELF_AGAINST_THE_ATREIDES = 427,
        STR_THE_PRODUCTION_OF_CARRYALLS_WILL_GREATLY_SPEED_UP_YOUR_HARVESTING_OPERATIONS_2 = 428,
        STR_YOU_ARE_TO_PROCEED_INTO_YET_ANOTHER_LUCKLESS_REGION_DOMINATED_BY_THOSE_PESKY_ORDOS_AND_I_EXPECT_YOU_TO_OVERCOME_THIS_PARTICULARLY_TROUBLESOME_ORDOS_GROUP_WE_HAVE_ESTABLISHED_A_GOOD_REPUTATION_ON_THIS_PLANET_DO_NOT_EMBARRASS_ME_NOW = 429,
        STR_THE_VICTORS_LAURELS_FIT_YOU_WELL_MY_SON_I_MAY_EVEN_REFER_OTHER_ASPIRANTS_TO_STUDY_YOUR_TACTICS_I_WOULD_PERSONALLY_APOLOGIZE_FOR_HAVING_UNDERESTIMATED_THE_ORDOS_STRENGTH_IN_YOUR_LAST_CAMPAIGN_BUT_THAT_IS_NO_LONGER_A_RELEVANT_CONCERN_OF_OURS_IS_IT = 430,
        STR_SURELY_THIS_IS_A_TASTELESS_JOKE_HOW_COULD_YOU_MANAGE_SUCH_A_DEFEAT_I_MAY_HAVE_TO_SLIT_YOUR_THROAT_MYSELF_JUST_TO_CALM_DOWN_DID_YOU_PLAN_TO_FAIL_IN_REVENGE_FOR_SOME_IMAGINED_WRONG = 431,
        STR_I_ADVISE_YOU_TO_PRODUCE_AND_THEN_PLACE_ROCKET_TURRETS_AT_STRATEGIC_DEFENSIVE_POSITIONS_AROUND_YOUR_INSTALLATION_2 = 432,
        STR_REPORTS_OF_NEW_ATREIDES_ACTIVITY_REQUIRE_THAT_I_SEND_YOU_IMMEDIATELY_BACK_TO_THE_FRONT_LINE_YOU_DO_NOT_SEEM_TO_ENJOY_REST_AND_RELAXATION_ANYWAY_I_THINK_YOU_WOULD_PREFER_TO_CRUSH_THE_ATREIDES = 433,
        STR_CONGRATULATIONS_IT_IS_A_PLEASURE_TO_SEE_THE_WEAK_ATREIDES_DEALT_WITH_SO_SEVERELY = 434,
        STR_YOUR_FAILURE_HAS_CAUSED_ME_NO_END_OF_EMBARRASSMENT_IT_WILL_NOT_HAPPEN_AGAIN_GUARDS = 435,
        STR_THE_DEVASTATOR_IS_A_POWERFUL_ALLY_AGAINST_ENEMY_FORCES = 436,
        STR_I_HAVE_USED_MY_INFLUENCE_TO_ARRANGE_A_PALACE_FOR_YOU_A_COMMANDER_OF_YOUR_STATUS_MAY_REQUIRE_RELAXATION_OCCASIONALLY_BUT_I_EXPECT_AN_EVEN_GREATER_EFFICIENCY_ON_YOUR_PART_WILL_COME_FROM_OUR_GENEROSITY_BOTH_ATREIDES_AND_ORDOS_FORCES_EXIST_IN_THIS_REGION_AND_ALL_MUST_BE_ELIMINATED = 437,
        STR_ALTHOUGH_PRAISE_MAY_BE_DUE_YOU_WILL_NOT_HAVE_THAT_LUXURY_SERVING_HOUSE_HARKONNEN_IS_REWARD_ENOUGH_IF_YOU_NEED_FLATTERY_THEN_DIE_IN_OUR_CAUSE_WE_WILL_SPEAK_FONDLY_OF_YOU_FOREVER = 438,
        STR_GUARDS_HOW_DID_THIS_WEAKLING_GET_IN_HERE_HE_IS_A_LOSER_AND_MUST_NOT_BE_ALLOWED_TO_CORRUPT_OTHERS_IN_OUR_HOUSE_IF_THIS_HAPPENS_AGAIN_I_WILL_PERSONALLY_DISSECT_SEVERAL_OF_YOU = 439,
        STR_UTILIZE_YOUR_PALACES_SPECIAL_OPTION_WHENEVER_POSSIBLE_FOR_IT_COSTS_NOTHING_AND_IT_WILL_RECHARGE_2 = 440,
        STR_WE_HAVE_BEEN_DECEIVED_OUR_BARGAINING_IN_GOOD_FAITH_HAS_ONLY_BROUGHT_A_TREACHEROUS_HARVEST_ALL_HAVE_CONSPIRED_AGAINST_US_AND_SO_ALL_MUST_DIE_YOUR_MILITARY_SKILLS_ARE_OUR_LAST_REMAINING_HOPE_FOR_THIS_PLANET_DESTROY_ALL_REMAINING_ATREIDES_AND_ORDOS_FORCES_AND_CONQUER_THE_EMPERORS_PALACE_HE_HAS_TREATED_US_POORLY_AND_MUST_NOT_LIVE_ANOTHER_DAY = 441,
        STR_GOOD_MORNING_YOUR_LORDSHIP_AND_CONGRATULATIONS_YOU_HAVE_SERVED_ME_I_MEAN_HOUSE_HARKONNEN_WELL_OUR_HOUSE_WILL_NOT_SOON_FORGET_OUR_MOST_NOBLE_WARRIOR_I_GO_NOW_TO_RELAY_THE_NEWS_OF_YOUR_MOST_GLORIOUS_VICTORY_AND_DELIVER_YOUR_TERMS_TO_THE_EMPEROR = 442,
        STR_YOUR_DEFECTS_MUST_HAVE_BEEN_INHERITED_NOBODY_COULD_POSSIBLY_LEARN_THE_COLOSSAL_STUPIDITY_THAT_YOU_HAVE_DISPLAYED_2 = 443,
        STR_WATCH_FOR_SNEAK_ATTACKS_DEFEND_YOUR_INSTALLATIONS_WELL_2 = 444,
        STR_HOUSE_ATREIDESCALADAN_HOME_PLANET_OF_THE_ATREIDES_HAS_A_WARM_CALM_CLIMATE_AND_THE_LANDS_ARE_LUSH_AND_GREEN_THE_RICH_SOILS_AND_MILD_WEATHER_SUPPORTS_AN_EXTENSIVE_VARIETY_OF_AGRICULTURAL_ACTIVITIES_IN_RECENT_CENTURIES_INDUSTRIAL_AND_TECHNOLOGICAL_DEVELOPMENT_HAS_ADDED_TO_THE_PROSPERITY_OF_THE_CALADANIAN_PEOPLES = 445,
        STR_WIN_TEXT = 446,
        STR_LOSE_TEXT = 447,
        STR_ADVICE_TEXT = 448,
        STR_GREETINGS_I_AM_YOUR_MENTAT_CYRIL_TOGETHER_WE_WILL_PURGE_THIS_PLANET_OF_THE_FOULNESS_OF_THE_OTHER_HOUSES_THE_HIGH_COMMAND_WISHES_YOU_TO_PRODUCE_1000_CREDITS_YOU_MAY_EARN_CREDITS_BY_BUILDING_A_REFINERY_AND_HARVESTING_SPICE = 449,
        STR_CONGRATULATIONS_I_KNEW_THAT_YOU_WOULD_ACHIEVE_YOUR_GOAL_WITH_VERY_LITTLE_TROUBLE_I_LOOK_FORWARD_TO_ASSISTING_YOU_IN_FUTURE_MISSIONS = 450,
        STR_THIS_IS_VERY_DISAPPOINTING_WE_HAD_SUCH_HIGH_HOPES_FOR_YOU_WE_MUST_INSIST_THAT_YOU_TRY_AGAIN_OUR_CONFIDENCE_IS_SURELY_NOT_MISPLACED_AND_WE_LOOK_FORWARD_TO_THE_SUCCESSFUL_COMPLETION_OF_YOUR_FIRST_TEST = 451,
        STR_FIRST_BUILD_A_TWO_BY_TWO_GROUP_OF_CONCRETE_SLABS_THEN_BUILD_A_WINDTRAP_AND_PLACE_IT_UPON_THE_CONCRETE_DO_THE_SAME_FOR_A_REFINERY_SO_THAT_YOU_CAN_REACH_YOUR_QUOTA_OF_1000_CREDITS_1 = 452,
        STR_GREETINGS_I_AM_HONORED_TO_SEE_YOU_AGAIN_THE_HIGH_COMMAND_NOW_REQUIRES_THAT_YOU_PRODUCE_2700_CREDITS_IN_A_NEW_HARVESTING_AREA_UNFORTUNATELY_WE_HAVE_CONFIRMED_THE_PRESENCE_OF_AN_ORDOS_BASE_IN_THIS_REGION_GOOD_LUCK = 453,
        STR_WELL_DONE_OUR_EFFORT_TO_MAINTAIN_FAIR_PLAY_ON_THIS_PLANET_IS_OPPOSED_BY_BOTH_THE_ORDOS_AND_THE_HARKONNEN_THE_SPICE_THAT_YOU_COLLECTED_WILL_HELP_US_GREATLY_IN_OUR_EFFORTS = 454,
        STR_THE_LOSS_OF_THIS_REGION_TO_THE_ORDOS_WILL_HURT_HOUSE_ATREIDES_GREATLY_PLEASE_TRY_HARDER_NEXT_TIME_THE_SERIOUSNESS_OF_YOUR_MISSION_CANNOT_BE_OVEREMPHASIZED = 455,
        STR_IF_YOU_HAVE_A_WINDTRAP_THEN_BUILD_A_BARRACKS_IN_ORDER_TO_PRODUCE_SOLDIERS_I_WOULD_DO_SO_TO_PROVIDE_A_GREATER_DEFENSE_FOR_YOUR_HARVESTING_OPERATION_1 = 456,
        STR_THE_BATTLE_WITH_THE_OTHER_HOUSES_HAS_INTENSIFIED_AND_WE_ARE_NOW_FORCED_TO_ENGAGE_IN_SOME_SELECTED_OFFENSIVE_MANEUVERS_THE_HARKONNEN_ARE_BEING_EXTREMELY_TROUBLESOME_IN_YOUR_NEXT_REGION_AND_WE_MUST_ASK_THAT_YOU_REMOVE_THEIR_PRESENCE_FROM_THE_AREA = 457,
        STR_HURRAH_ALTHOUGH_I_PREFER_DEFENSE_A_FINE_OFFENSIVE_AGAINST_SUCH_A_DESERVING_FOE_AS_THE_HARKONNEN_IS_A_JOY_TO_BEHOLD_AND_A_REAFFIRMATION_OF_ALL_THAT_IS_GOOD_AND_RIGHT_HOUSE_ATREIDES_THANKS_YOU_FOR_YOUR_EFFORTS = 458,
        STR_IT_IS_A_CRUEL_WORLD_IS_IT_NOT_IM_NOT_SURE_IF_I_WAS_EXPECTING_DIVINE_INTERVENTION_BUT_AS_I_WATCHED_YOUR_DEFEAT_I_COULD_NOT_HELP_THINKING_HOW_UNFAIR_IT_WAS = 459,
        STR_YOU_NEED_TO_BUILD_A_LIGHT_FACTORY_TO_PRODUCE_THE_WEAPONS_NECESSARY_TO_COMPLETE_YOUR_MISSION_1 = 460,
        STR_YOUR_DEMONSTRATION_OF_MILITARY_SKILLS_NOW_FORCES_US_TO_ASSIGN_YOU_TO_ANOTHER_OFFENSIVE_CAMPAIGN_AGAINST_HOUSE_HARKONNEN_THEY_HAVE_CONTINUED_TO_ATTACK_OUR_PEACEFUL_HARVESTERS_AND_MUST_BE_REMOVED_FROM_THE_AREA = 461,
        STR_WELL_DONE_NO_TASK_SEEMS_TOO_DIFFICULT_FOR_YOU_THE_HARKONNEN_WILL_THINK_TWICE_BEFORE_ATTACKING_OUR_HARVESTERS_AGAIN = 462,
        STR_ALTHOUGH_I_UNDERSTAND_THE_PROBLEMS_CONFRONTING_YOU_I_WILL_CONFESS_THAT_I_AM_DISAPPOINTED_BY_YOUR_DEFEAT_I_AM_NOT_SURE_WHAT_WENT_WRONG_BUT_PLEASE_DO_NOT_LET_IT_HAPPEN_AGAIN = 463,
        STR_THE_ADDITION_OF_TANKS_IN_YOUR_FORCES_IS_ESSENTIAL_TO_YOUR_VICTORY_IN_THIS_REGION_1 = 464,
        STR_WELCOME_THE_RULES_SEEM_TO_HAVE_CHANGED_AS_YOU_HAVE_WITNESSED_THE_EMPEROR_HIMSELF_HAS_BEEN_AIDING_THE_EFFORTS_OF_OUR_COMPETITORS_AS_A_PART_OF_OUR_NEW_STRATEGY_WE_MUST_ASK_THAT_YOU_ELIMINATE_THE_TREACHEROUS_ORDOS_FORCES_THAT_PRESENTLY_CONTROL_THIS_REGION = 465,
        STR_CONGRATULATIONS_AGAIN_THERE_SEEMS_TO_BE_NO_LIMIT_TO_YOUR_TALENTS_I_HAVE_TO_THINK_THAT_SOME_DIVINE_FORCE_MOTIVATED_YOUR_DECISION_TO_SERVE_WITHIN_HOUSE_ATREIDES_OUR_GAIN_WAS_CERTAINLY_THE_OTHERS_LOSS = 466,
        STR_THIS_DEFEAT_OF_YOURS_WILL_NOT_LOOK_GOOD_IN_MY_REPORTS_I_AM_NOT_SURE_HOW_MUCH_LONGER_WE_CAN_AFFORD_TO_SUPPORT_YOU_THERE_ARE_MANY_OTHERS_CLAMORING_TO_TAKE_YOUR_PLACE_SO_I_SUGGEST_THAT_YOU_DO_WHATEVER_IS_REQUIRED_TO_BE_VICTORIOUS_IN_YOUR_NEXT_ENCOUNTERS = 467,
        STR_THE_PRODUCTION_OF_CARRYALLS_WILL_GREATLY_SPEED_UP_YOUR_HARVESTING_OPERATIONS_1 = 468,
        STR_AS_THE_BATTLE_FOR_THIS_PLANET_INTENSIFIES_ALL_EFFORTS_MUST_BE_TAKEN_TO_ENSURE_OUR_SUCCESS_ONCE_AGAIN_WE_MUST_CALL_UPON_YOU_TO_DESTROY_OUR_ENEMIES_IN_A_TROUBLED_SECTOR_HOUSE_HARKONNEN_MUST_BE_TAUGHT_A_LESSON_THANK_YOU_AND_GOOD_LUCK = 469,
        STR_HURRAH_WE_HAVE_THEM_ON_THE_RUN_NOW_AND_ITS_ALL_BECAUSE_OF_YOU_YOUR_SUCCESSES_HAVE_BEEN_REMARKABLE_ARE_YOU_SURE_YOU_DIDNT_GRADUATE_FROM_ONE_OF_THE_INTERGALACTIC_MILITARY_ACADEMIES = 470,
        STR_THERE_IS_A_TIME_FOR_EXCUSES_BUT_THIS_IS_NOT_IT_I_DONT_UNDERSTAND_HOW_YOU_COULD_HAVE_BUNGLED_THE_ENTIRE_CAMPAIGN_YOU_HAVE_NO_CHOICE_BUT_TO_APPLY_YOURSELF_MORE_DILIGENTLY_DURING_YOUR_NEXT_ASSIGNMENT = 471,
        STR_I_ADVISE_YOU_TO_PRODUCE_AND_THEN_PLACE_ROCKET_TURRETS_AT_STRATEGIC_DEFENSIVE_POSITIONS_AROUND_YOUR_INSTALLATION_1 = 472,
        STR_THE_BATTLE_GOES_WELL_BUT_THERE_IS_NO_TIME_TO_RELAX_WE_HAVE_AN_URGENT_NEED_FOR_YOU_TO_SUBDUE_ALL_ORDOS_FORCES_IN_THIS_REGION_PROMPTLY_OUR_ONGOING_NEGOTIATIONS_ARE_AIDED_IMMEASURABLY_BY_CORRESPONDING_VICTORIES_IN_THE_FIELD_WE_ARE_COUNTING_ON_YOU = 473,
        STR_EXCELLENT_YOUR_SKILL_SEEMS_TO_IMPROVE_WITH_EACH_ASSIGNMENT_AND_YOUR_EXPLOITS_GIVE_GREAT_CONFIDENCE_TO_THOSE_ON_THE_HOME_FRONT_KEEP_UP_THE_GOOD_WORK = 474,
        STR_MY_GOODNESS_WHAT_AN_AWFUL_DEFEAT_PERHAPS_WE_HAVE_GIVEN_YOU_TOO_MUCH_RESPONSIBILITY_IF_YOU_FAIL_AT_YOUR_NEXT_ASSIGNMENT_WE_WILL_HAVE_TO_SERIOUSLY_CONSIDER_SENDING_YOU_HOME = 475,
        STR_THE_SONIC_TANK_IS_A_POWERFUL_ALLY_AGAINST_ENEMY_TROOPS = 476,
        STR_ALTHOUGH_YOU_HAVE_EARNED_A_WELLDESERVED_REST_IM_AFRAID_THE_POLITICAL_SITUATION_REQUIRES_THAT_WE_SEND_YOU_BACK_INTO_THE_FIELD_IMMEDIATELY_BOTH_ORDOS_AND_HARKONNEN_FORCES_HAVE_BUILT_UP_TO_UNACCEPTABLE_LEVELS_IN_THIS_REGION_AND_NOW_MUST_BE_REMOVED_COMPLETELY = 477,
        STR_A_MASTERFUL_VICTORY_FRANKLY_WE_WOULD_NOT_BE_ABLE_TO_REMAIN_ON_DUNE_WITHOUT_YOUR_MILITARY_GUIDANCE_WE_HAVE_COME_TO_RELY_COMPLETELY_ON_YOU_SIR_AND_CANNOT_IMAGINE_FURTHER_SUCCESS_WITHOUT_YOU = 478,
        STR_A_DEFEAT_IS_NEVER_WELCOME_BUT_THIS_LOSS_IS_PARTICULARLY_DEVASTATING_ALL_OF_OUR_EFFORTS_MAY_SOON_PROVE_FUTILE_UNLESS_YOU_CAN_PROVIDE_A_VICTORY_IN_YOUR_NEXT_CAMPAIGN = 479,
        STR_UTILIZE_YOUR_PALACES_SPECIAL_OPTION_WHENEVER_POSSIBLE_FOR_IT_COSTS_NOTHING_AND_IT_WILL_RECHARGE_1 = 480,
        STR_YOUR_NEXT_ASSIGNMENT_WILL_DETERMINE_THE_ENTIRE_OUTCOME_OF_OUR_EFFORTS_HERE_ON_DUNE_VICTORY_WILL_NOT_COME_EASILY_IN_ADDITION_TO_DESTROYING_ALL_REMAINING_ORDOS_AND_HARKONNEN_TROOPS_YOU_ARE_ALSO_INSTRUCTED_TO_SUBDUE_EMPEROR_FREDERICKS_FORCES_ALL_OF_OUR_HOPES_AND_DREAMS_ARE_RIDING_WITH_YOU_AND_WE_HUMBLY_BEG_OF_YOU_TO_PROVIDE_ONE_FINAL_VICTORY_FOR_OUR_NOBLE_HOUSE_ATREIDES = 481,
        STR_GOOD_MORNING_YOUR_LORDSHIP_AND_CONGRATULATIONS_YOU_HAVE_SERVED_HOUSE_ATREIDES_WELL_WE_WILL_NOT_SOON_FORGET_OUR_MOST_NOBLE_WARRIOR_I_GO_NOW_TO_RELAY_THE_NEWS_OF_YOUR_MOST_GLORIOUS_VICTORY_AND_DELIVER_YOUR_TERMS_TO_THE_EMPEROR = 482,
        STR_YOUR_DEFECTS_MUST_HAVE_BEEN_INHERITED_NOBODY_COULD_POSSIBLY_LEARN_THE_COLOSSAL_STUPIDITY_THAT_YOU_HAVE_DISPLAYED_1 = 483,
        STR_WATCH_FOR_SNEAK_ATTACKS_DEFEND_YOUR_INSTALLATIONS_WELL_1 = 484,
        STR_HOUSE_ORDOSTHE_HOME_PLANET_OF_THE_ORDOS_IS_A_FRIGID_AND_ICECOVERED_WORLD_WE_PRESUME_THE_ORDOS_IMPORT_THEIR_AGRICULTURAL_AND_TECHNOLOGICAL_GOODS_FROM_NEARBY_STAR_SYSTEMS_ACTING_AS_TRADERS_AND_BROKERS_THE_ORDOS_PRODUCE_NO_PHYSICAL_PRODUCTS_OF_THEIR_OWN_AND_RELY_UPON_THEIR_MERCHANDISING_SKILLS_TO_SURVIVE = 485,
        STR_TEST_SCENARIO_WIN_TEXT_2 = 486,
        STR_TEST_SCENARIO_LOSE_TEXT_2 = 487,
        STR_TEST_SCENARIO_ADVICE_TEXT_2 = 488,
        STR_WELCOME_I_AM_YOUR_MENTAT_AND_YOU_MAY_CALL_ME_AMMON_TO_BE_OF_ANY_VALUE_TO_THE_CARTEL_YOU_MUST_PROVIDE_US_WITH_CREDITS_AS_A_TEST_WE_WILL_ASSIGN_YOU_TO_A_REGION_AND_ASK_THAT_YOU_MEET_A_PRODUCTION_QUOTA_OF_1000_CREDITS_BUILD_A_REFINERY_AND_HARVEST_THE_SPICE_IN_THE_AREA_I_AM_VERY_BUSY_BUT_YOU_MAY_CALL_UPON_ME_IF_YOU_HAVE_FURTHER_QUESTIONS = 489,
        STR_YOU_SHOW_POTENTIAL_IF_YOU_CONTINUE_TO_DISPLAY_GOOD_MANAGEMENT_SKILLS_WE_MAY_FIND_A_PERMANENT_POSITION_FOR_YOU_IN_OUR_ORGANIZATION = 490,
        STR_IT_IS_BEYOND_ME_HOW_YOU_WERE_ABLE_TO_FAIL_SUCH_AN_EASY_TASK_BECAUSE_YOU_COME_SO_HIGHLY_RECOMMENDED_WE_MAY_BE_ABLE_TO_OVERLOOK_THIS_EPISODE_IF_YOU_ARE_SUCCESSFUL_WITH_YOUR_NEXT_ASSIGNMENT = 491,
        STR_FIRST_BUILD_A_TWO_BY_TWO_GROUP_OF_CONCRETE_SLABS_THEN_BUILD_A_WINDTRAP_AND_PLACE_IT_UPON_THE_CONCRETE_DO_THE_SAME_FOR_A_REFINERY_SO_THAT_YOU_CAN_REACH_YOUR_QUOTA_OF_1000_CREDITS_3 = 492,
        STR_YOUR_QUOTA_IS_NOW_2700_CREDITS_AND_THIS_SPICE_ACCUMULATION_IS_YOUR_PRIMARY_OBJECTIVE_WE_DO_NOT_EXPECT_YOU_TO_DESTROY_THE_HARKONNEN_FORCES_IN_THE_AREA_HOWEVER_YOU_SHOULD_CONSIDER_THE_TIME_THAT_COULD_BE_SAVED_BY_APPROPRIATING_THEIR_SILOS = 493,
        STR_CONGRATULATIONS_THE_CARTEL_IS_BEGINNING_TO_NOTICE_YOU_I_THINK_YOU_WILL_DEFINITELY_HAVE_A_FUTURE_WITH_US_IF_YOU_CAN_CONTINUE_ON_THIS_TRACK = 494,
        STR_APPARENTLY_I_WAS_MISTAKEN_ABOUT_YOUR_POTENTIAL_IT_WILL_COST_US_A_GREAT_DEAL_TO_RETAKE_THIS_REGION_ONLY_COMPLETE_SUCCESS_WITH_YOUR_NEXT_ASSIGNMENT_COULD_POSSIBLY_RESURRECT_YOUR_CAREER = 495,
        STR_IF_YOU_HAVE_A_WINDTRAP_THEN_BUILD_A_BARRACKS_IN_ORDER_TO_PRODUCE_SOLDIERS_I_WOULD_DO_SO_TO_PROVIDE_A_GREATER_DEFENSE_FOR_YOUR_HARVESTING_OPERATION_3 = 496,
        STR_WE_FIND_THE_ACTIVITIES_OF_ATREIDES_TROOPS_IN_THIS_REGION_INCONVENIENT_PLEASE_REMOVE_THIS_OBSTACLE_WE_CANNOT_ALLOW_THE_MYTHICAL_VALUE_OF_FAIR_PLAY_TO_IMPEDE_OUR_PROGRESS = 497,
        STR_EXCELLENT_THERE_WERE_RUMORS_THAT_SOME_SORT_OF_MORAL_OBJECTIONS_WOULD_HINDER_YOUR_EFFECTIVENESS_I_AM_GLAD_TO_SEE_THAT_YOU_HAVE_RISEN_ABOVE_THESE_PETTY_SENSITIVITIES = 498,
        STR_YOUR_CLUMSY_MANAGEMENT_HAS_COST_US_DEARLY_THIS_IS_NOT_A_GAME_WE_CANNOT_JUST_START_OVER_EVERY_TIME_YOU_FAIL_TO_CONCENTRATE_ON_THE_TASK_BEFORE_YOU_I_SUGGEST_YOU_RID_YOURSELF_OF_ALL_OUTSIDE_INTERESTS_AND_DEVOTE_YOURSELF_COMPLETELY_TO_YOUR_NEXT_ASSIGNMENT = 499,
        STR_YOU_NEED_TO_BUILD_A_LIGHT_FACTORY_TO_PRODUCE_THE_WEAPONS_NECESSARY_TO_COMPLETE_YOUR_MISSION_3 = 500,
        STR_THE_ATREIDES_FORCES_IN_THIS_AREA_MUST_BE_ELIMINATED_AS_ALWAYS_WE_APPRECIATE_YOUR_CAPTURE_OF_ANY_ENEMY_SILOS_OR_OTHER_STRUCTURES_THAT_MIGHT_BE_SALVAGEABLE = 501,
        STR_A_MASTERFUL_VICTORY_BUT_BE_CAUTIOUS_YOUR_REPUTATION_WILL_NOW_PRECEDE_YOU_IN_FUTURE_ENGAGEMENTS_AND_YOU_MAY_FIND_YOUR_FOES_BETTER_PREPARED_FOR_YOUR_TACTICS = 502,
        STR_HOW_COULD_YOU_POSSIBLY_FAIL_SO_QUICKLY_MY_GRANDMOTHER_COULD_COMMAND_MORE_EFFICIENTLY_THAN_YOU_GET_BACK_OUT_THERE_AND_SHOW_SOME_INITIATIVE_OR_STOP_WASTING_MY_TIME = 503,
        STR_THE_ADDITION_OF_TANKS_IN_YOUR_FORCES_IS_ESSENTIAL_TO_YOUR_VICTORY_IN_THIS_REGION_3 = 504,
        STR_SO_OUR_SPIES_REVEAL_WHY_THE_SARDAUKAR_ATTACKED_US_IN_YOUR_LAST_MISSION_I_WILL_NEED_TO_LOOK_INTO_THIS_ALTHOUGH_THE_HARKONNEN_COMMANDERS_ARE_LAUGHABLY_STUPID_THEIR_MILITARY_STRENGTH_IS_A_THREAT_TO_US_IN_THIS_AREA_THEY_MUST_BE_ELIMINATED_AS_SOON_AS_POSSIBLE = 505,
        STR_CONGRATULATIONS_WOULD_BE_IN_ORDER_BUT_WE_EXPECT_THESE_RESULTS_WHEN_YOU_ARE_IN_CHARGE_YOUR_CONTINUED_SUCCESS_BRINGS_HONOR_TO_YOUR_FAMILY_AND_ALL_OF_US_IN_HOUSE_ORDOS = 506,
        STR_YOU_BUNGLING_FOOL_DID_YOU_EXPECT_YOUR_ENEMIES_TO_RUN_AWAY_AT_THE_MERE_SIGHT_OF_YOU_REMEMBER_YOUR_BRAIN_WILL_WIN_THE_WAR_NOT_YOUR_BULLETS = 507,
        STR_THE_PRODUCTION_OF_CARRYALLS_WILL_GREATLY_SPEED_UP_YOUR_HARVESTING_OPERATIONS_3 = 508,
        STR_THE_ATREIDES_HAVE_BECOME_FAR_TOO_VOCAL_IN_THIS_SECTOR_AND_WHINE_CONSTANTLY_ABOUT_THEIR_RIGHTS_WE_ORDOS_DO_NOT_HAVE_THE_LEISURE_OF_POINTLESS_CONVERSATION_AND_MUST_ASK_THAT_YOU_ELIMINATE_THIS_DISTRACTION = 509,
        STR_WELL_DONE_YOUR_SUCCESSFUL_MILITARY_CAMPAIGNS_HAVE_CREATED_A_POLITICAL_ATMOSPHERE_WELL_SUITED_TO_OUR_PARTICULAR_BARGAINING_SKILLS_KEEP_UP_THE_GOOD_WORK = 510,
        STR_DO_YOU_INSTRUCT_YOUR_TROOPS_TO_FIRE_RANDOMLY_INTO_THE_AIR_IS_THERE_ANY_PLAN_TO_YOUR_IDIOTIC_MANEUVERS_I_SUGGEST_THAT_YOU_NO_LONGER_ALLOW_YOUR_DOG_ASSIST_IN_MANAGING_YOUR_ASSIGNMENTS = 511,
        STR_I_ADVISE_YOU_TO_PRODUCE_AND_THEN_PLACE_ROCKET_TURRETS_AT_STRATEGIC_DEFENSIVE_POSITIONS_AROUND_YOUR_INSTALLATION_3 = 512,
        STR_HARKONNEN_FORCES_CONTINUE_TO_THWART_OUR_EFFORTS_IN_THIS_REGION_AND_MUST_BE_REMOVED_COMPLETELY_CRUSH_THEIR_BELOVED_TROOPERS_AND_THEY_WILL_RUN_CRYING_BACK_TO_THEIR_UGLY_MOTHERS = 513,
        STR_EXCELLENT_YOU_HAVE_WON_A_GREAT_VICTORY_FOR_HOUSE_ORDOS_DOESNT_INFLICTING_SUCH_A_HUMILIATING_DEFEAT_MAKE_YOUR_BREAKFAST_TASTE_BETTER = 514,
        STR_YOUR_FAILURE_COULD_NOT_HAVE_BEEN_TIMED_MORE_POORLY_WHAT_STATUS_AND_BARGAINING_POWER_CAN_WE_MAINTAIN_WHEN_YOU_DISPLAY_SUCH_INEPTITUDE_IN_THE_FIELD = 515,
        STR_THE_DEVIATOR_IS_A_POWERFUL_ALLY_AGAINST_ENEMY_FORCES = 516,
        STR_BOTH_ATREIDES_AND_HARKONNEN_FORCES_OPPOSE_OUR_CONTROL_OF_THIS_AREA_AND_MUST_THEREFORE_BE_DESTROYED_THE_TIME_FOR_COOPERATION_AND_COMPROMISE_HAS_PAST_AND_ALL_ENEMIES_OF_HOUSE_ORDOS_MUST_BE_ELIMINATED = 517,
        STR_I_JUST_RECEIVED_THE_WONDERFUL_NEWS_OF_YOUR_STUNNING_VICTORY_I_AM_VERY_PLEASED_AND_LOOK_FORWARD_TO_TELLING_MY_GRANDCHILDREN_THAT_IT_WAS_I_WHO_SERVED_AS_MENTAT_TO_SUCH_A_BRILLIANT_ORDOS_COMMANDER = 518,
        STR_YOU_DO_NOT_EXIST_IN_A_VACUUM_YOUR_INEPTITUDE_HAS_ENDANGERED_EVERYTHING_HOUSE_ORDOS_HAS_WORKED_TO_CREATE_HERE_ON_ARRAKIS_YOU_HAD_BETTER_THINK_ABOUT_THE_CONSEQUENCES_OF_YOUR_ACTIONS_AND_MAKE_WHATEVER_AMENDS_POSSIBLE_AS_QUICKLY_AS_YOU_CAN = 519,
        STR_UTILIZE_YOUR_PALACES_SPECIAL_OPTION_WHENEVER_POSSIBLE_FOR_IT_COSTS_NOTHING_AND_IT_WILL_RECHARGE_3 = 520,
        STR_EMPEROR_FREDERICK_HAS_JOINED_THE_LIST_OF_ORDOS_ENEMIES_AND_MUST_BE_PUNISHED_DESTROY_HIS_TROOPS_AND_ANY_ATREIDES_AND_HARKONNEN_REMNANTS_THAT_STILL_OPPOSE_US_ON_THIS_PLANET_WE_HAVE_RISKED_EVERYTHING_ON_THIS_FINAL_BATTLE_AND_CANNOT_TOLERATE_LESS_THAN_YOUR_BEST_EFFORT = 521,
        STR_GOOD_MORNING_YOUR_LORDSHIP_AND_CONGRATULATIONS_YOU_HAVE_SERVED_HOUSE_ORDOS_WELL_WE_WILL_NOT_SOON_FORGET_OUR_MOST_NOBLE_WARRIOR_I_GO_NOW_TO_RELAY_THE_NEWS_OF_YOUR_MOST_GLORIOUS_VICTORY_AND_DELIVER_YOUR_TERMS_TO_THE_EMPEROR = 522,
        STR_YOUR_DEFECTS_MUST_HAVE_BEEN_INHERITED_NOBODY_COULD_POSSIBLY_LEARN_THE_COLOSSAL_STUPIDITY_THAT_YOU_HAVE_DISPLAYED_3 = 523,
        STR_WATCH_FOR_SNEAK_ATTACKS_DEFEND_YOUR_INSTALLATIONS_WELL_3 = 524,
        STR_SECURITY_COUNT = 525,
        STR_SECURITY_TEXT_HARKONNEN = 526,
        STR_SECURITY_CORRECT_HARKONNEN = 527,
        STR_SECURITY_WRONG_HARKONNEN = 528,
        STR_SECURITY_TEXT_ATREIDES = 529,
        STR_SECURITY_CORRECT_ATREIDES = 530,
        STR_SECURITY_WRONG_ATREIDES = 531,
        STR_SECURITY_TEXT_ORDOS = 532,
        STR_SECURITY_CORRECT_ORDOS = 533,
        STR_SECURITY_WRONG_ORDOS = 534,
        STR_SECURITY_QUESTIONS = 535
    }
}
