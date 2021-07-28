/* LandscapeInfo file table */

namespace SharpDune.Table
{
    class TableLandscapeInfo
    {
        internal static LandscapeInfo[] g_table_landscapeInfo = { //[LST_MAX]
            new() { /* 0 / LST_NORMAL_SAND */
		        movementSpeed = new byte[] { 112, 112, 112, 160, 255, 192 },
                letUnitWobble = false,
                isValidForStructure = false,
                isSand = true,
                isValidForStructure2 = false,
                canBecomeSpice = true,
                craterType = 1,
                radarColour = 88,
                spriteID = 37
            },

            new() { /* 1 / LST_PARTIAL_ROCK */
		        movementSpeed = new byte[] { 160, 112, 112, 64, 255, 0 },
                letUnitWobble = true,
                isValidForStructure = false,
                isSand = false,
                isValidForStructure2 = false,
                canBecomeSpice = false,
                craterType = 1,
                radarColour = 28,
                spriteID = 39
            },

            new() { /* 2 / LST_ENTIRELY_DUNE */
		        movementSpeed = new byte[] { 112, 160, 160, 160, 255, 192 },
                letUnitWobble = false,
                isValidForStructure = false,
                isSand = true,
                isValidForStructure2 = false,
                canBecomeSpice = true,
                craterType = 1,
                radarColour = 92,
                spriteID = 41
            },

            new() { /* 3 / LST_PARTIAL_DUNE */
		        movementSpeed = new byte[] { 112, 160, 160, 160, 255, 192 },
                letUnitWobble = false,
                isValidForStructure = false,
                isSand = true,
                isValidForStructure2 = false,
                canBecomeSpice = true,
                craterType = 1,
                radarColour = 89,
                spriteID = 43
            },

            new() { /* 4 / LST_ENTIRELY_ROCK */
		        movementSpeed = new byte[] { 112, 160, 160, 112, 255, 0 },
                letUnitWobble = true,
                isValidForStructure = true,
                isSand = false,
                isValidForStructure2 = true,
                canBecomeSpice = false,
                craterType = 2,
                radarColour = 30,
                spriteID = 45
            },

            new() { /* 5 / LST_MOSTLY_ROCK */
		        movementSpeed = new byte[] { 160, 160, 160, 160, 255, 0 },
                letUnitWobble = true,
                isValidForStructure = true,
                isSand = false,
                isValidForStructure2 = true,
                canBecomeSpice = false,
                craterType = 2,
                radarColour = 29,
                spriteID = 47
            },

            new() { /* 6 / LST_ENTIRELY_MOUNTAIN */
		        movementSpeed = new byte[] { 64, 0, 0, 0, 255, 0 },
                letUnitWobble = true,
                isValidForStructure = false,
                isSand = false,
                isValidForStructure2 = false,
                canBecomeSpice = false,
                craterType = 0,
                radarColour = 12,
                spriteID = 49
            },

            new() { /* 7 / LST_PARTIAL_MOUNTAIN */
		        movementSpeed = new byte[] { 64, 0, 0, 0, 255, 0 },
                letUnitWobble = true,
                isValidForStructure = false,
                isSand = false,
                isValidForStructure2 = false,
                canBecomeSpice = false,
                craterType = 0,
                radarColour = 133,
                spriteID = 51
            },

            new() { /* 8 / LST_SPICE */
		        movementSpeed = new byte[] { 112, 160, 160, 160, 255, 192 },
                letUnitWobble = false,
                isValidForStructure = false,
                isSand = true,
                isValidForStructure2 = false,
                canBecomeSpice = true,
                craterType = 1,
                radarColour = 215, /* was 88, but is changed on startup */
		        spriteID = 53  /* was 37, but is changed on startup */
	        },

            new() { /* 9 / LST_THICK_SPICE */
		        movementSpeed = new byte[] { 112, 160, 160, 160, 255, 192 },
                letUnitWobble = true,
                isValidForStructure = false,
                isSand = true,
                isValidForStructure2 = false,
                canBecomeSpice = true,
                craterType = 1,
                radarColour = 216, /* was 88, but is changed on startup */
		        spriteID = 53  /* was 37, but is changed on startup */
	        },

            new() { /* 10 / LST_CONCRETE_SLAB */
		        movementSpeed = new byte[] { 255, 255, 255, 255, 255, 0 },
                letUnitWobble = false,
                isValidForStructure = true,
                isSand = false,
                isValidForStructure2 = false,
                canBecomeSpice = false,
                craterType = 2,
                radarColour = 133,
                spriteID = 51
            },

            new() { /* 11 / LST_WALL */
		        movementSpeed = new byte[] { 0, 0, 0, 0, 255, 0 },
                letUnitWobble = false,
                isValidForStructure = false,
                isSand = false,
                isValidForStructure2 = false,
                canBecomeSpice = false,
                craterType = 0,
                radarColour = 65535,
                spriteID = 31
            },

            new() { /* 12 / LST_STRUCTURE */
		        movementSpeed = new byte[] { 0, 0, 0, 0, 255, 0 },
                letUnitWobble = false,
                isValidForStructure = false,
                isSand = false,
                isValidForStructure2 = false,
                canBecomeSpice = false,
                craterType = 0,
                radarColour = 65535,
                spriteID = 31
            },

            new() { /* 13 / LST_DESTROYED_WALL */
		        movementSpeed = new byte[] { 160, 160, 160, 160, 255, 0 },
                letUnitWobble = true,
                isValidForStructure = true,
                isSand = false,
                isValidForStructure2 = true,
                canBecomeSpice = false,
                craterType = 2,
                radarColour = 29,
                spriteID = 47
            },

            new() { /* 14 / LST_BLOOM_FIELD */
		        movementSpeed = new byte[] { 112, 112, 112, 160, 255, 192 },
                letUnitWobble = false,
                isValidForStructure = false,
                isSand = true,
                isValidForStructure2 = false,
                canBecomeSpice = true,
                craterType = 1,
                radarColour = 50,
                spriteID = 57
            }
        };
    }
}
