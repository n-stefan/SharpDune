/* WindowDesc file table */

namespace SharpDune.Table;

class TableWindowDesc
{
    internal static WindowDesc g_saveLoadWindowDesc = new() {
        index = 17,
        stringID = (short)Text.STR_SELECT_A_SAVED_GAME_TO_LOAD,
        addArrows = true,
        widgetCount = 6,
        widgets = new WindowDescWidget[] {
            new() {
                stringID = 65535 /*(ushort)-1*/, /* First savegame name. */
                offsetX = 16,
                offsetY = 39,
                width = 256,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new() {
                stringID = 65534 /*(ushort)-2*/, /* Second savegame name. */
                offsetX = 16,
                offsetY = 56,
                width = 256,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new() {
                stringID = 65533 /*(ushort)-3*/, /* Third savegame name. */
                offsetX = 16,
                offsetY = 73,
                width = 256,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new() {
                stringID = 65532 /*(ushort)-4*/, /* Fourth savegame name. */
                offsetX = 16,
                offsetY = 90,
                width = 256,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new() {
                stringID = 65531 /*(ushort)-5*/, /* Fifth savegame name. */
                offsetX = 16,
                offsetY = 107,
                width = 256,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new() {
                stringID = (ushort)Text.STR_CANCEL,
                offsetX = 176,
                offsetY = 126,
                width = 96,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 110
            },
            new() {
                stringID = (ushort)Text.STR_NULL,
                offsetX = 0,
                offsetY = 0,
                width = 0,
                height = 0,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            }
        }
    };

    internal static WindowDesc g_gameControlWindowDesc = new() {
        index = 16,
        stringID = (short)Text.STR_GAME_CONTROLS,
        addArrows = false,
        widgetCount = 6,
        widgets = new WindowDescWidget[] {
            new()
            {
                stringID = 65526 /*(ushort)-10*/, /* Music state. */
                offsetX = 152,
                offsetY = 22,
                width = 80,
                height = 15,
                labelStringId = (ushort)Text.STR_MUSIC_IS,
                shortcut2 = 0
            },
            new()
            {
                stringID = 65525 /*(ushort)-11*/, /* Sound state. */
                offsetX = 152,
                offsetY = 39,
                width = 80,
                height = 15,
                labelStringId = (ushort)Text.STR_SOUNDS_ARE,
                shortcut2 = 0
            },
            new()
            {
                stringID = 65524 /*(ushort)-12*/, /* Game speed. */
                offsetX = 152,
                offsetY = 56,
                width = 80,
                height = 15,
                labelStringId = (ushort)Text.STR_GAME_SPEED,
                shortcut2 = 0
            },
            new()
            {
                stringID = 65523 /*(ushort)-13*/, /* Hints state */
                offsetX = 152,
                offsetY = 73,
                width = 80,
                height = 15,
                labelStringId = (ushort)Text.STR_HINTS_ARE,
                shortcut2 = 0
            },
            new()
            {
                stringID = 65522 /*(ushort)-14*/, /* Autoscroll state */
                offsetX = 152,
                offsetY = 90,
                width = 80,
                height = 15,
                labelStringId = (ushort)Text.STR_AUTO_SCROLL_IS,
                shortcut2 = 0
            },
            new()
            {
                stringID = (ushort)Text.STR_PREVIOUS,
                offsetX = 96,
                offsetY = 110,
                width = 136,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 110
            },
            new()
            {
                stringID = (ushort)Text.STR_NULL,
                offsetX = 0,
                offsetY = 0,
                width = 0,
                height = 0,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            }
        }
    };

    internal static WindowDesc g_optionsWindowDesc = new()
    {
        index = 16,
        stringID = (short)Text.STR_DUNE_II_THE_BATTLE_FOR_ARRAKIS,
        addArrows = false,
        widgetCount = 7,
        widgets = new WindowDescWidget[] {
            new() {
                stringID = (ushort)Text.STR_LOAD_A_GAME,
                offsetX = 16,
                offsetY = 23,
                width = 208,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new() {
                stringID = (ushort)Text.STR_SAVE_THIS_GAME,
                offsetX = 16,
                offsetY = 40,
                width = 208,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new() {
                stringID = (ushort)Text.STR_GAME_CONTROLS,
                offsetX = 16,
                offsetY = 57,
                width = 208,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new() {
                stringID = (ushort)Text.STR_RESTART_SCENARIO,
                offsetX = 16,
                offsetY = 74,
                width = 208,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new() {
                stringID = (ushort)Text.STR_PICK_ANOTHER_HOUSE,
                offsetX = 16,
                offsetY = 91,
                width = 208,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new() {
                stringID = (ushort)Text.STR_CONTINUE_GAME,
                offsetX = 120,
                offsetY = 110,
                width = 104,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 110
            },
            new() {
                stringID = (ushort)Text.STR_QUIT_PLAYING,
                offsetX = 16,
                offsetY = 110,
                width = 104,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            }
        }
    };

    internal static WindowDesc g_yesNoWindowDesc = new()
    {
        index = 18,
        stringID = (short)Text.STR_ARE_YOU_SURE_YOU_WANT_TO_QUIT_PLAYING,
        addArrows = false,
        widgetCount = 2,
        widgets = new WindowDescWidget[] {
            new()
            {
                stringID = (ushort)Text.STR_YES,
                offsetX = 8,
                offsetY = 30,
                width = 72,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new()
            {
                stringID = (ushort)Text.STR_NO,
                offsetX = 224,
                offsetY = 30,
                width = 72,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new()
            {
                stringID = (ushort)Text.STR_NULL,
                offsetX = 0,
                offsetY = 0,
                width = 0,
                height = 0,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new()
            {
                stringID = (ushort)Text.STR_NULL,
                offsetX = 0,
                offsetY = 0,
                width = 0,
                height = 0,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new()
            {
                stringID = (ushort)Text.STR_NULL,
                offsetX = 0,
                offsetY = 0,
                width = 0,
                height = 0,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new()
            {
                stringID = (ushort)Text.STR_NULL,
                offsetX = 0,
                offsetY = 0,
                width = 0,
                height = 0,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new()
            {
                stringID = (ushort)Text.STR_NULL,
                offsetX = 0,
                offsetY = 0,
                width = 0,
                height = 0,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            }
        }
    };

    internal static WindowDesc g_savegameNameWindowDesc = new()
    {
        index = 14,
        stringID = (short)Text.STR_ENTER_A_DESCRIPTION_OF_YOUR_SAVED_GAME,
        addArrows = false,
        widgetCount = 2,
        widgets = new WindowDescWidget[] {
            new() {
                stringID = (ushort)Text.STR_SAVE,
                offsetX = 8,
                offsetY = 38,
                width = 72,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 43
            },
            new() {
                stringID = (ushort)Text.STR_CANCEL,
                offsetX = 208,
                offsetY = 38,
                width = 72,
                height = 15,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 110
            },
            new() {
                stringID = (ushort)Text.STR_NULL,
                offsetX = 0,
                offsetY = 0,
                width = 0,
                height = 0,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new() {
                stringID = (ushort)Text.STR_NULL,
                offsetX = 0,
                offsetY = 0,
                width = 0,
                height = 0,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new() {
                stringID = (ushort)Text.STR_NULL,
                offsetX = 0,
                offsetY = 0,
                width = 0,
                height = 0,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new() {
                stringID = (ushort)Text.STR_NULL,
                offsetX = 0,
                offsetY = 0,
                width = 0,
                height = 0,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            },
            new() {
                stringID = (ushort)Text.STR_NULL,
                offsetX = 0,
                offsetY = 0,
                width = 0,
                height = 0,
                labelStringId = (ushort)Text.STR_NULL,
                shortcut2 = 0
            }
        }
    };
}
