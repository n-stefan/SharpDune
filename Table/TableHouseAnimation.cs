/* House animation tables */

namespace SharpDune.Table;

class TableHouseAnimation
{
    internal static HouseAnimation_Animation[][] g_table_houseAnimation_animation = { //[HOUSEANIMATION_MAX][32]
        new HouseAnimation_Animation[] { /* 0 - intro */
        	new() { str = string.Empty, duration = 20, frameCount = 10, flags = 0x40 },
            new() { str = "INTRO1", duration = 45, frameCount = 0, flags = 0x62 },
            new() { str = string.Empty, duration = 50, frameCount = 10, flags = 0x48 },
            new() { str = "INTRO2", duration = 80, frameCount = 0, flags = 0x46 },
            new() { str = string.Empty, duration = 20, frameCount = 10, flags = 0x8 },
            new() { str = "INTRO3", duration = 100, frameCount = 0, flags = 0x46 },
            new() { str = string.Empty, duration = 30, frameCount = 10, flags = 0x0 },
            new() { str = "INTRO9", duration = 200, frameCount = 0, flags = 0x8A },
            new() { str = "INTRO10", duration = 75, frameCount = 15, flags = 0x45 },
            new() { str = "EFINALA", duration = 57, frameCount = 0, flags = 0x42 },
            new() { str = string.Empty, duration = 1, frameCount = 2, flags = 0x0 },
            new() { str = "INTRO11", duration = 63, frameCount = 0, flags = 0x4A },
            new() { str = "INTRO4", duration = 130, frameCount = 0, flags = 0x4E },
            new() { str = string.Empty, duration = 40, frameCount = 10, flags = 0x0 },
            new() { str = "INTRO6", duration = 90, frameCount = 0, flags = 0x46 },
            new() { str = "INTRO7a", duration = 55, frameCount = 0, flags = 0x402 },
            new() { str = "INTRO7b", duration = 45, frameCount = 0, flags = 0x2 },
            new() { str = "INTRO8a", duration = 30, frameCount = 0, flags = 0x402 },
            new() { str = "INTRO8b", duration = 30, frameCount = 0, flags = 0x2 },
            new() { str = "INTRO8c", duration = 50, frameCount = 0, flags = 0x12 },
            new() { str = "INTRO5", duration = 90, frameCount = 40, flags = 0x4D },
            new() { str = string.Empty, duration = 100, frameCount = 50, flags = 0x0 },
            new() { str = string.Empty, duration = 0, frameCount = 0, flags = 0x0 }
        },

        new HouseAnimation_Animation[] { /* 1 - Level 4: Harkonnen */
        	new() { str = string.Empty, duration = 50, frameCount = 10, flags = 0x0 },
            new() { str = "MEANWHIL", duration = 70, frameCount = 1, flags = 0x47 },
            new() { str = "EFINALA", duration = 60, frameCount = 0, flags = 0x42 },
            new() { str = "MEANWHIL", duration = 70, frameCount = 1, flags = 0x4B },
            new() { str = string.Empty, duration = 0, frameCount = 0, flags = 0x0 }
        },
        
        new HouseAnimation_Animation[] { /* 2 - Level 4: Atreides */
        	new() { str = string.Empty, duration = 50, frameCount = 10, flags = 0x0 },
            new() { str = "MEANWHIL", duration = 75, frameCount = 2, flags = 0x47 },
            new() { str = "EFINALA", duration = 60, frameCount = 0, flags = 0x42 },
            new() { str = "MEANWHIL", duration = 70, frameCount = 2, flags = 0x4B },
            new() { str = string.Empty, duration = 0, frameCount = 0, flags = 0x0 }
        },
        
        new HouseAnimation_Animation[] { /* 3 - Level 4: Ordos */
        	new() { str = string.Empty, duration = 50, frameCount = 10, flags = 0x0 },
            new() { str = "MEANWHIL", duration = 70, frameCount = 0, flags = 0x47 },
            new() { str = "EFINALA", duration = 60, frameCount = 0, flags = 0x42 },
            new() { str = "MEANWHIL", duration = 70, frameCount = 0, flags = 0x4B },
            new() { str = string.Empty, duration = 0, frameCount = 0, flags = 0x0 }
        },
        
        new HouseAnimation_Animation[] { /* 4 - Level 8: Harkonnen */
        	new() { str = string.Empty, duration = 50, frameCount = 10, flags = 0x0 },
            new() { str = "MEANWHIL", duration = 70, frameCount = 3, flags = 0x47 },
            new() { str = "EFINALA", duration = 85, frameCount = 0, flags = 0x42 },
            new() { str = string.Empty, duration = 0, frameCount = 0, flags = 0x0 }
        },
        
        new HouseAnimation_Animation[] { /* 5 - Level 8: Atreides */
        	new() { str = string.Empty, duration = 50, frameCount = 10, flags = 0x0 },
            new() { str = "MEANWHIL", duration = 90, frameCount = 5, flags = 0x47 },
            new() { str = "EFINALA", duration = 60, frameCount = 0, flags = 0x42 },
            new() { str = string.Empty, duration = 0, frameCount = 0, flags = 0x0 }
        },

        new HouseAnimation_Animation[] { /* 6 - Level 8: Ordos */
        	new() { str = string.Empty, duration = 50, frameCount = 10, flags = 0x0 },
            new() { str = "MEANWHIL", duration = 70, frameCount = 4, flags = 0x47 },
            new() { str = "EFINALA", duration = 75, frameCount = 0, flags = 0x42 },
            new() { str = string.Empty, duration = 0, frameCount = 0, flags = 0x0 }
        },
        
        new HouseAnimation_Animation[] { /* 7 - Level 9: Harkonnen */
        	new() { str = "HFINALA", duration = 40, frameCount = 0, flags = 0x46 },
            new() { str = "HFINALA", duration = 40, frameCount = 0, flags = 0x42 },
            new() { str = string.Empty, duration = 20, frameCount = 10, flags = 0x0 },
            new() { str = "EFINALA", duration = 60, frameCount = 0, flags = 0x42 },
            new() { str = "HFINALB", duration = 45, frameCount = 0, flags = 0x3 },
            new() { str = "EFINALB", duration = 40, frameCount = 0, flags = 0x3 },
            new() { str = "EFINALB", duration = 10, frameCount = 1, flags = 0x43 },
            new() { str = "HFINALB", duration = 20, frameCount = 0, flags = 0x3 },
            new() { str = "HFINALB", duration = 45, frameCount = 0, flags = 0x42 },
            new() { str = "HFINALC", duration = 45, frameCount = 0, flags = 0x2 },
            new() { str = string.Empty, duration = 10, frameCount = 10, flags = 0x8 },
            new() { str = string.Empty, duration = 0, frameCount = 0, flags = 0x0 }
        },
        
        new HouseAnimation_Animation[] { /* 8 - Level 9: Atreides */
        	new() { str = "AFINALA", duration = 35, frameCount = 0, flags = 0x7 },
            new() { str = "EFINALA", duration = 60, frameCount = 0, flags = 0x42 },
            new() { str = "AFINALA", duration = 40, frameCount = 0, flags = 0x42 },
            new() { str = string.Empty, duration = 16, frameCount = 10, flags = 0x0 },
            new() { str = "EFINALB", duration = 55, frameCount = 0, flags = 0x3 },
            new() { str = "EFINALB", duration = 20, frameCount = 1, flags = 0x43 },
            new() { str = "AFINALB", duration = 20, frameCount = 0, flags = 0x42 },
            new() { str = string.Empty, duration = 35, frameCount = 10, flags = 0x8 },
            new() { str = string.Empty, duration = 0, frameCount = 0, flags = 0x0 }
        },
        
        new HouseAnimation_Animation[] { /* 9 - Level 9: Ordos */
        	new() { str = "OFINALA", duration = 65, frameCount = 0, flags = 0x7 },
            new() { str = "EFINALA", duration = 60, frameCount = 0, flags = 0x42 },
            new() { str = "OFINALA", duration = 20, frameCount = 0, flags = 0x42 },
            new() { str = string.Empty, duration = 19, frameCount = 10, flags = 0x0 },
            new() { str = "OFINALB", duration = 20, frameCount = 0, flags = 0x2 },
            new() { str = string.Empty, duration = 15, frameCount = 10, flags = 0x0 },
            new() { str = "OFINALC", duration = 20, frameCount = 0, flags = 0x2 },
            new() { str = string.Empty, duration = 22, frameCount = 10, flags = 0x0 },
            new() { str = "EFINALB", duration = 25, frameCount = 0, flags = 0x3 },
            new() { str = "EFINALB", duration = 15, frameCount = 1, flags = 0x43 },
            new() { str = "OFINALD", duration = 10, frameCount = 0, flags = 0x3 },
            new() { str = "OFINALD", duration = 16, frameCount = 0, flags = 0x42 },
            new() { str = string.Empty, duration = 20, frameCount = 10, flags = 0x8 },
            new() { str = string.Empty, duration = 0, frameCount = 0, flags = 0x0 }
        }
    };

    internal static HouseAnimation_Subtitle[][] g_table_houseAnimation_subtitle = { //[HOUSEANIMATION_MAX][32]
        new HouseAnimation_Subtitle[] { /* 0 - intro */
        	new() {
                stringID = (ushort)Text.STR_PRESENT,
                colour = 0,
                animationID = 0,
                top = 94,
                waitFadein = 1,
                paletteFadein = 2,
                waitFadeout = 7,
                paletteFadeout = 2
            },
            new() {
                stringID = (ushort)Text.STR_NULL,
                colour = 0,
                animationID = 1,
                top = 154,
                waitFadein = 45,
                paletteFadein = 1,
                waitFadeout = 1,
                paletteFadeout = 1
            },
            new() {
                stringID = (ushort)Text.STR_THE_BATTLE_FOR_ARRAKIS,
                colour = 0,
                animationID = 2,
                top = 104,
                waitFadein = 1,
                paletteFadein = 5,
                waitFadeout = 7,
                paletteFadeout = 64
            },
            new() {
                stringID = (ushort)Text.STR_THE_PLANET_ARRAKIS_KNOWN_AS_DUNE,
                colour = 0,
                animationID = 3,
                top = 154,
                waitFadein = 3,
                paletteFadein = 5,
                waitFadeout = 35,
                paletteFadeout = 5
            },
            new() {
                stringID = (ushort)Text.STR_LAND_OF_SAND,
                colour = 0,
                animationID = 5,
                top = 154,
                waitFadein = 1,
                paletteFadein = 4,
                waitFadeout = 16,
                paletteFadeout = 4
            },
            new() {
                stringID = (ushort)Text.STR_HOME_OF_THE_SPICE_MELANGE,
                colour = 0,
                animationID = 5,
                top = 154,
                waitFadein = 3,
                paletteFadein = 4,
                waitFadeout = 16,
                paletteFadeout = 4
            },
            new() {
                stringID = (ushort)Text.STR_THE_SPICE_CONTROLS_THE_EMPIRE,
                colour = 0,
                animationID = 7,
                top = 154,
                waitFadein = 1,
                paletteFadein = 5,
                waitFadeout = 25,
                paletteFadeout = 5
            },
            new() {
                stringID = (ushort)Text.STR_WHOEVER_CONTROLS_DUNECONTROLS_THE_SPICE,
                colour = 0,
                animationID = 7,
                top = 154,
                waitFadein = 2,
                paletteFadein = 5,
                waitFadeout = 27,
                paletteFadeout = 9
            },
            new() {
                stringID = (ushort)Text.STR_THE_EMPEROR_HAS_PROPOSED_ACHALLENGE_TO_EACH_OF_THE_HOUSES,
                colour = 0,
                animationID = 8,
                top = 154,
                waitFadein = 1,
                paletteFadein = 5,
                waitFadeout = 10,
                paletteFadeout = 5
            },
            new() {
                stringID = (ushort)Text.STR_THE_HOUSE_THAT_PRODUCES_THEMOST_SPICE_WILL_CONTROL_DUNE,
                colour = 4,
                animationID = 9,
                top = 154,
                waitFadein = 1,
                paletteFadein = 4,
                waitFadeout = 24,
                paletteFadeout = 4
            },
            new() {
                stringID = (ushort)Text.STR_THERE_ARE_NO_SET_TERRITORIES,
                colour = 4,
                animationID = 11,
                top = 154,
                waitFadein = 1,
                paletteFadein = 4,
                waitFadeout = 20,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_AND_NO_RULES_OF_ENGAGEMENT,
                colour = 4,
                animationID = 11,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 20,
                paletteFadeout = 4
            },
            new() {
                stringID = (ushort)Text.STR_VAST_ARMIES_HAVE_ARRIVED,
                colour = 0,
                animationID = 12,
                top = 154,
                waitFadein = 10,
                paletteFadein = 3,
                waitFadeout = 30,
                paletteFadeout = 4
            },
            new() {
                stringID = (ushort)Text.STR_NOW_THREE_HOUSES_FIGHTFOR_CONTROL_OF_DUNE,
                colour = 0,
                animationID = 13,
                top = 85,
                waitFadein = 0,
                paletteFadein = 2,
                waitFadeout = 10,
                paletteFadeout = 1
            },
            new() {
                stringID = (ushort)Text.STR_THE_NOBLE_ATREIDES,
                colour = 0,
                animationID = 14,
                top = 154,
                waitFadein = 32,
                paletteFadein = 2,
                waitFadeout = 35,
                paletteFadeout = 7
            },
            new() {
                stringID = (ushort)Text.STR_THE_INSIDIOUS_ORDOS,
                colour = 0,
                animationID = 15,
                top = 154,
                waitFadein = 5,
                paletteFadein = 2,
                waitFadeout = 26,
                paletteFadeout = 7
            },
            new() {
                stringID = (ushort)Text.STR_AND_THE_EVIL_HARKONNEN,
                colour = 0,
                animationID = 17,
                top = 154,
                waitFadein = 1,
                paletteFadein = 2,
                waitFadeout = 34,
                paletteFadeout = 7
            },
            new() {
                stringID = (ushort)Text.STR_ONLY_ONE_HOUSE_WILL_PREVAIL,
                colour = 0,
                animationID = 20,
                top = 154,
                waitFadein = 1,
                paletteFadein = 4,
                waitFadeout = 29,
                paletteFadeout = 5
            },
            new() {
                stringID = (ushort)Text.STR_YOUR_BATTLE_FOR_DUNE_BEGINS,
                colour = 0,
                animationID = 21,
                top = 85,
                waitFadein = 0,
                paletteFadein = 3,
                waitFadeout = 20,
                paletteFadeout = 0
            },
            new() {
                stringID = (ushort)Text.STR_NOW,
                colour = 0,
                animationID = 21,
                top = 85,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 3,
                paletteFadeout = 15
            },
            new() {
                stringID = 65535,
                colour = 65535,
                animationID = 0,
                top = 0,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 0,
                paletteFadeout = 0
            }
        },

        new HouseAnimation_Subtitle[] { /* 1 - Level 4: Harkonnen */
		    new() {
                stringID = (ushort)Text.STR_AT_THE_EMPERORS_PALACE3,
                colour = 0,
                animationID = 0,
                top = 85,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 6,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_YOU_RECEIVE_THE_ASSISTANCEYOU_REQUIRE_AND_THEN_FAIL_ME,
                colour = 4,
                animationID = 1,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 11,
                paletteFadeout = 2
            },
            new() {
                stringID = (ushort)Text.STR_NO_YOUR,
                colour = 2,
                animationID = 1,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 7,
                paletteFadeout = 0
            },
            new() {
                stringID = (ushort)Text.STR_MY_SARDAUKAR_COULD_HELP_DEFEAT_THEHARKONNEN_BUT_YOU_WASTED_THEM,
                colour = 4,
                animationID = 2,
                top = 154,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 22,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_WE_DIDNT_HAVE,
                colour = 2,
                animationID = 3,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 4,
                paletteFadeout = 0
            },
            new() {
                stringID = (ushort)Text.STR_I_WANT_NO_EXCUSESDO_NOT_FAIL_ME_AGAIN,
                colour = 4,
                animationID = 3,
                top = 154,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 99,
                paletteFadeout = 99
            },
            new() {
                stringID = 65535,
                colour = 65535,
                animationID = 0,
                top = 0,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 0,
                paletteFadeout = 0
            }
        },

        new HouseAnimation_Subtitle[] { /* 2 - Level 4: Atreides */
		    new() {
                stringID = (ushort)Text.STR_AT_THE_EMPERORS_PALACE,
                colour = 1,
                animationID = 0,
                top = 85,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 6,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_YOU_OF_ALL_PEOPLE_SHOULDUNDERSTAND_THE_IMPORTANCE_OF_VICTORY,
                colour = 4,
                animationID = 1,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 11,
                paletteFadeout = 2
            },
            new() {
                stringID = (ushort)Text.STR_YES_YOUR_EXCELLENCY_I,
                colour = 0,
                animationID = 1,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 7,
                paletteFadeout = 0
            },
            new() {
                stringID = (ushort)Text.STR_YOU_LET_THE_ATREIDES_DEFEATYOU_AND_MY_SARDAUKAR,
                colour = 4,
                animationID = 2,
                top = 154,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 22,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_I_DID_NOT_LET,
                colour = 0,
                animationID = 3,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 4,
                paletteFadeout = 0
            },
            new() {
                stringID = (ushort)Text.STR_I_WILL_NOT_ALLOW_IT_TO_HAPPEN_AGAIN,
                colour = 4,
                animationID = 3,
                top = 154,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 99,
                paletteFadeout = 99
            },
            new() {
                stringID = 65535,
                colour = 65535,
                animationID = 0,
                top = 0,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 0,
                paletteFadeout = 0
            }
        },

        new HouseAnimation_Subtitle[] { /* 3 - Level 4: Ordos */
		    new() {
                stringID = (ushort)Text.STR_AT_THE_EMPERORS_PALACE2,
                colour = 2,
                animationID = 0,
                top = 85,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 6,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_I_CANNOT_BELIEVE_THEINCOMPETENCE_I_SEE_BEFORE_ME,
                colour = 4,
                animationID = 1,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 11,
                paletteFadeout = 2
            },
            new() {
                stringID = (ushort)Text.STR_YOUR_HIGHNESS,
                colour = 1,
                animationID = 1,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 7,
                paletteFadeout = 0
            },
            new() {
                stringID = (ushort)Text.STR_I_GIVE_YOU_MY_SARDAUKAR_TO_ASSISTAGAINST_THE_ORDOS_AND_YOU_FAILED_ME,
                colour = 4,
                animationID = 2,
                top = 154,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 22,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_WE,
                colour = 1,
                animationID = 3,
                top = 154,
                waitFadein = 1,
                paletteFadein = 1,
                waitFadeout = 3,
                paletteFadeout = 0
            },
            new() {
                stringID = (ushort)Text.STR_DO_NOT_FAIL_ME_AGAINYOU_ARE_DISMISSED,
                colour = 4,
                animationID = 3,
                top = 154,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 99,
                paletteFadeout = 99
            },
            new() {
                stringID = 65535,
                colour = 65535,
                animationID = 0,
                top = 0,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 0,
                paletteFadeout = 0
            }
        },

        new HouseAnimation_Subtitle[] { /* 4 - Level 8: Harkonnen */
		    new() {
                stringID = (ushort)Text.STR_AT_THE_EMPERORS_PALACEON_DUNE3,
                colour = 0,
                animationID = 0,
                top = 85,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 6,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_I_SHOULD_HAVE_KNOWN_YOUR_HOUSESCOULDNT_STAND_UP_TO_THE_HARKONNEN,
                colour = 4,
                animationID = 1,
                top = 154,
                waitFadein = 1,
                paletteFadein = 2,
                waitFadeout = 12,
                paletteFadeout = 1
            },
            new() {
                stringID = (ushort)Text.STR_EXCELLENCY_THEY_ARE,
                colour = 2,
                animationID = 1,
                top = 154,
                waitFadein = 0,
                paletteFadein = 1,
                waitFadeout = 6,
                paletteFadeout = 0
            },
            new() {
                stringID = (ushort)Text.STR_SILENCE,
                colour = 4,
                animationID = 1,
                top = 154,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 4,
                paletteFadeout = 0
            },
            new() {
                stringID = (ushort)Text.STR_YOU_ARE_TO_DEFEND_MY_PALACE,
                colour = 4,
                animationID = 2,
                top = 154,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 22,
                paletteFadeout = 0
            },
            new() {
                stringID = (ushort)Text.STR_I_WILL_SHOW_YOU_HOWTO_CRUSH_THE_HARKONNEN,
                colour = 4,
                animationID = 2,
                top = 154,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 99,
                paletteFadeout = 99
            },
            new() {
                stringID = 65535,
                colour = 65535,
                animationID = 0,
                top = 0,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 0,
                paletteFadeout = 0
            }
        },

        new HouseAnimation_Subtitle[] { /* 5 - Level 8: Atreides */
		    new() {
                stringID = (ushort)Text.STR_AT_THE_EMPERORS_PALACEON_DUNE,
                colour = 1,
                animationID = 0,
                top = 85,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 6,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_FOOLSI_GAVE_YOU_WEAPONS_AND_TROOPS,
                colour = 4,
                animationID = 1,
                top = 154,
                waitFadein = 1,
                paletteFadein = 2,
                waitFadeout = 7,
                paletteFadeout = 1
            },
            new() {
                stringID = (ushort)Text.STR_AND_STILL_YOU_FAILTO_DEFEAT_THE_ATREIDES,
                colour = 4,
                animationID = 1,
                top = 154,
                waitFadein = 0,
                paletteFadein = 1,
                waitFadeout = 8,
                paletteFadeout = 1
            },
            new() {
                stringID = (ushort)Text.STR_BUT_EXCELL,
                colour = 0,
                animationID = 1,
                top = 154,
                waitFadein = 0,
                paletteFadein = 3,
                waitFadeout = 4,
                paletteFadeout = 0
            },
            new() {
                stringID = (ushort)Text.STR_ENOUGH_TOGETHER_WE_MUST_MAKESURE_THE_ATREIDES_DO_NOT_SUCCEED,
                colour = 4,
                animationID = 2,
                top = 154,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 99,
                paletteFadeout = 99
            },
            new() {
                stringID = 65535,
                colour = 65535,
                animationID = 0,
                top = 0,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 0,
                paletteFadeout = 0
            }
        },

        new HouseAnimation_Subtitle[] { /* 6 - Level 8: Ordos */
		    new() {
                stringID = (ushort)Text.STR_AT_THE_EMPERORS_PALACEON_DUNE2,
                colour = 2,
                animationID = 0,
                top = 85,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 6,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_THE_ORDOS_WERE_NOTSUPPOSED_TO_GET_THIS_FAR,
                colour = 4,
                animationID = 1,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 11,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_YOUR_HIGHNESS_LET_US_EXPLAIN,
                colour = 1,
                animationID = 1,
                top = 154,
                waitFadein = 1,
                paletteFadein = 2,
                waitFadeout = 8,
                paletteFadeout = 0
            },
            new() {
                stringID = (ushort)Text.STR_NO_MORE_EXPLANATIONSYOU_ARE_TO_DEFEND_MY_PALACE,
                colour = 4,
                animationID = 2,
                top = 154,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 23,
                paletteFadeout = 0
            },
            new() {
                stringID = (ushort)Text.STR_ONLY_TOGETHER_WILLWE_DEFEAT_THE_ORDOS,
                colour = 4,
                animationID = 2,
                top = 154,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 99,
                paletteFadeout = 99
            },
            new() {
                stringID = 65535,
                colour = 65535,
                animationID = 0,
                top = 0,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 0,
                paletteFadeout = 0
            }
        },

        new HouseAnimation_Subtitle[] { /* 7 - Level 9: Harkonnen */
		    new() {
                stringID = (ushort)Text.STR_YOU_ARE_INDEED_NOT_ENTIRELYTRUE_TO_YOUR_WORD_EMPEROR,
                colour = 0,
                animationID = 0,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 9,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_YOU_HAVE_LIED_TO_US,
                colour = 0,
                animationID = 1,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 14,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_WHAT_LIES_WHAT_AREYOU_TALKING_ABOUT,
                colour = 4,
                animationID = 3,
                top = 154,
                waitFadein = 2,
                paletteFadein = 3,
                waitFadeout = 22,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_YOUR_LIES_OF_LOYALTYYOUR_DECEIT,
                colour = 0,
                animationID = 4,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 17,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_A_CRIME_FOR_WHICH_YOU_WILLINDEED_PAY_DEARLY,
                colour = 0,
                animationID = 5,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 17,
                paletteFadeout = 1
            },
            new() {
                stringID = (ushort)Text.STR_WITH_YOUR_LIFE,
                colour = 0,
                animationID = 5,
                top = 154,
                waitFadein = 1,
                paletteFadein = 1,
                waitFadeout = 26,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_NO_NO_NOOO,
                colour = 4,
                animationID = 8,
                top = 154,
                waitFadein = 2,
                paletteFadein = 2,
                waitFadeout = 10,
                paletteFadeout = 3
            },
            new() {
                stringID = 65535,
                colour = 65535,
                animationID = 0,
                top = 0,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 0,
                paletteFadeout = 0
            }
        },

        new HouseAnimation_Subtitle[] { /* 8 - Level 9: Atreides */
		    new() {
                stringID = (ushort)Text.STR_GREETINGS_EMPEROR,
                colour = 1,
                animationID = 0,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 14,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_WHAT_IS_THE_MEANINGOF_THIS_INTRUSION,
                colour = 4,
                animationID = 1,
                top = 154,
                waitFadein = 2,
                paletteFadein = 3,
                waitFadeout = 39,
                paletteFadeout = 2
            },
            new() {
                stringID = (ushort)Text.STR_YOU_ARE_FORMALLY_CHARGED_WITH_CRIMESOF_TREASON_AGAINST_HOUSE_ATREIDES,
                colour = 1,
                animationID = 2,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 32,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_THE_HOUSE_SHALL_DETERMINEYOUR_GUILT_OR_INNOCENCE,
                colour = 1,
                animationID = 4,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 14,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_UNTIL_THEN_YOU_SHALL_NOLONGER_REIGN_AS_EMPEROR,
                colour = 1,
                animationID = 4,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 32,
                paletteFadeout = 3
            },
            new() {
                stringID = 65535,
                colour = 65535,
                animationID = 0,
                top = 0,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 0,
                paletteFadeout = 0
            }
        },

        new HouseAnimation_Subtitle[] { /* 9 - Level 9: Ordos */
		    new() {
                stringID = (ushort)Text.STR_YOU_ARE_AWARE_EMPEROR_THAT_WEHAVE_GROWN_WEARY_OF_YOUR_GAMES,
                colour = 2,
                animationID = 0,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 18,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_WHAT_GAMES_WHAT_AREYOU_TALKING_ABOUT,
                colour = 4,
                animationID = 1,
                top = 154,
                waitFadein = 2,
                paletteFadein = 3,
                waitFadeout = 22,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_I_AM_REFERRING_TOYOUR_GAME_OF_CHESS,
                colour = 2,
                animationID = 2,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 15,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_WE_WERE_YOUR_PAWNS_ANDDUNE_WAS_YOUR_BOARD,
                colour = 2,
                animationID = 4,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 25,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_WE_HAVE_DECIDED_TO_TAKE_MATTERSINTO_OUR_OWN_HANDS,
                colour = 2,
                animationID = 6,
                top = 154,
                waitFadein = 1,
                paletteFadein = 3,
                waitFadeout = 22,
                paletteFadeout = 3
            },
            new() {
                stringID = (ushort)Text.STR_YOU_ARE_TO_BE_OUR_PAWNIN_OUR_GAME,
                colour = 2,
                animationID = 8,
                top = 154,
                waitFadein = 3,
                paletteFadein = 3,
                waitFadeout = 40,
                paletteFadeout = 3
            },
            new() {
                stringID = 65535,
                colour = 65535,
                animationID = 0,
                top = 0,
                waitFadein = 0,
                paletteFadein = 0,
                waitFadeout = 0,
                paletteFadeout = 0
            }
        }
    };

    internal static HouseAnimation_SoundEffect[][] g_table_houseAnimation_soundEffect = { //[HOUSEANIMATION_MAX][90]
        new HouseAnimation_SoundEffect[] { /* 0 - intro */
        	new() { animationID = 5, voiceID = 108, wait = 12 },
            new() { animationID = 7, voiceID = 117, wait = 19 },
            new() { animationID = 7, voiceID = 109, wait = 33 },
            new() { animationID = 7, voiceID = 110, wait = 52 },
            new() { animationID = 12, voiceID = 105, wait = 2 },
            new() { animationID = 12, voiceID = 106, wait = 4 },
            new() { animationID = 12, voiceID = 106, wait = 20 },
            new() { animationID = 12, voiceID = 111, wait = 22 },
            new() { animationID = 12, voiceID = 106, wait = 37 },
            new() { animationID = 12, voiceID = 117, wait = 38 },
            new() { animationID = 14, voiceID = 112, wait = 21 },
            new() { animationID = 14, voiceID = 112, wait = 22 },
            new() { animationID = 14, voiceID = 112, wait = 23 },
            new() { animationID = 14, voiceID = 112, wait = 24 },
            new() { animationID = 14, voiceID = 112, wait = 25 },
            new() { animationID = 14, voiceID = 112, wait = 26 },
            new() { animationID = 14, voiceID = 112, wait = 27 },
            new() { animationID = 14, voiceID = 112, wait = 28 },
            new() { animationID = 14, voiceID = 112, wait = 29 },
            new() { animationID = 14, voiceID = 112, wait = 30 },
            new() { animationID = 14, voiceID = 112, wait = 31 },
            new() { animationID = 14, voiceID = 113, wait = 32 },
            new() { animationID = 14, voiceID = 113, wait = 33 },
            new() { animationID = 14, voiceID = 113, wait = 34 },
            new() { animationID = 14, voiceID = 112, wait = 51 },
            new() { animationID = 14, voiceID = 112, wait = 52 },
            new() { animationID = 14, voiceID = 112, wait = 53 },
            new() { animationID = 14, voiceID = 112, wait = 54 },
            new() { animationID = 14, voiceID = 112, wait = 55 },
            new() { animationID = 14, voiceID = 112, wait = 56 },
            new() { animationID = 14, voiceID = 112, wait = 57 },
            new() { animationID = 14, voiceID = 112, wait = 58 },
            new() { animationID = 14, voiceID = 112, wait = 59 },
            new() { animationID = 14, voiceID = 112, wait = 60 },
            new() { animationID = 14, voiceID = 112, wait = 61 },
            new() { animationID = 14, voiceID = 113, wait = 62 },
            new() { animationID = 14, voiceID = 113, wait = 63 },
            new() { animationID = 15, voiceID = 114, wait = 5 },
            new() { animationID = 15, voiceID = 114, wait = 9 },
            new() { animationID = 15, voiceID = 116, wait = 19 },
            new() { animationID = 16, voiceID = 114, wait = 3 },
            new() { animationID = 16, voiceID = 114, wait = 13 },
            new() { animationID = 17, voiceID = 112, wait = 2 },
            new() { animationID = 17, voiceID = 112, wait = 3 },
            new() { animationID = 17, voiceID = 112, wait = 4 },
            new() { animationID = 17, voiceID = 115, wait = 5 },
            new() { animationID = 17, voiceID = 112, wait = 7 },
            new() { animationID = 17, voiceID = 115, wait = 8 },
            new() { animationID = 17, voiceID = 112, wait = 9 },
            new() { animationID = 17, voiceID = 112, wait = 10 },
            new() { animationID = 17, voiceID = 112, wait = 11 },
            new() { animationID = 17, voiceID = 112, wait = 12 },
            new() { animationID = 17, voiceID = 112, wait = 13 },
            new() { animationID = 17, voiceID = 112, wait = 14 },
            new() { animationID = 18, voiceID = 112, wait = 3 },
            new() { animationID = 18, voiceID = 112, wait = 4 },
            new() { animationID = 18, voiceID = 112, wait = 5 },
            new() { animationID = 18, voiceID = 115, wait = 6 },
            new() { animationID = 18, voiceID = 112, wait = 7 },
            new() { animationID = 18, voiceID = 112, wait = 8 },
            new() { animationID = 18, voiceID = 112, wait = 9 },
            new() { animationID = 18, voiceID = 112, wait = 10 },
            new() { animationID = 18, voiceID = 112, wait = 11 },
            new() { animationID = 18, voiceID = 112, wait = 12 },
            new() { animationID = 18, voiceID = 112, wait = 13 },
            new() { animationID = 18, voiceID = 112, wait = 14 },
            new() { animationID = 18, voiceID = 112, wait = 15 },
            new() { animationID = 19, voiceID = 112, wait = 2 },
            new() { animationID = 19, voiceID = 112, wait = 3 },
            new() { animationID = 19, voiceID = 112, wait = 4 },
            new() { animationID = 19, voiceID = 112, wait = 5 },
            new() { animationID = 19, voiceID = 112, wait = 6 },
            new() { animationID = 19, voiceID = 112, wait = 7 },
            new() { animationID = 19, voiceID = 112, wait = 8 },
            new() { animationID = 19, voiceID = 112, wait = 9 },
            new() { animationID = 19, voiceID = 112, wait = 10 },
            new() { animationID = 19, voiceID = 112, wait = 11 },
            new() { animationID = 19, voiceID = 112, wait = 12 },
            new() { animationID = 19, voiceID = 112, wait = 13 },
            new() { animationID = 19, voiceID = 112, wait = 14 },
            new() { animationID = 19, voiceID = 112, wait = 15 },
            new() { animationID = 19, voiceID = 118, wait = 18 },
            new() { animationID = 19, voiceID = 119, wait = 28 },
            new() { animationID = 22, voiceID = 255, wait = 0 }
        },

        new HouseAnimation_SoundEffect[] { /* 1 - Level 4: Harkonnen */
			new() { animationID = 4, voiceID = 255, wait = 0 }
        },

        new HouseAnimation_SoundEffect[] { /* 2 - Level 4: Atreides */
			new() { animationID = 4, voiceID = 255, wait = 0 }
        },

        new HouseAnimation_SoundEffect[] { /* 3 - Level 4: Ordos */
			new() { animationID = 4, voiceID = 255, wait = 0 }
        },

        new HouseAnimation_SoundEffect[] { /* 4 - Level 8: Harkonnen */
			new() { animationID = 4, voiceID = 255, wait = 0 }
        },

        new HouseAnimation_SoundEffect[] { /* 5 - Level 8: Atreides */
			new() { animationID = 4, voiceID = 255, wait = 0 }
        },

        new HouseAnimation_SoundEffect[] { /* 6 - Level 8: Ordos */
			new() { animationID = 4, voiceID = 255, wait = 0 }
        },

        new HouseAnimation_SoundEffect[] { /* 7 - Level 9: Harkonnen */
			new() { animationID = 8, voiceID = 69, wait = 2 },
            new() { animationID = 8, voiceID = 65, wait = 10 },
            new() { animationID = 8, voiceID = 68, wait = 12 },
            new() { animationID = 8, voiceID = 69, wait = 36 },
            new() { animationID = 9, voiceID = 65, wait = 0 },
            new() { animationID = 9, voiceID = 68, wait = 2 },
            new() { animationID = 9, voiceID = 66, wait = 25 },
            new() { animationID = 11, voiceID = 255, wait = 0 }
        },

        new HouseAnimation_SoundEffect[] { /* 8 - Level 9: Atreides */
			new() { animationID = 8, voiceID = 255, wait = 0 }
        },

        new HouseAnimation_SoundEffect[] { /* 9 - Level 9: Ordos */
			new() { animationID = 2, voiceID = 67, wait = 2 },
            new() { animationID = 6, voiceID = 67, wait = 1 },
            new() { animationID = 13, voiceID = 255, wait = 0 }
        }
    };
}
