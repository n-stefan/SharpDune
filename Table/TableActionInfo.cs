/* ActionInfo file table */

namespace SharpDune.Table;

static class TableActionInfo
{
    internal static ActionInfo[] g_table_actionInfo = [ //[ACTION_MAX]
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
    ];
}
