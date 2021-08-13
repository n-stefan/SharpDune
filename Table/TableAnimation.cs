/* Animation file table */

namespace SharpDune.Table;

class TableAnimation
{
    internal static AnimationCommandStruct[][] g_table_animation_unitMove = { //[8][8]
    	new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 0 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 4 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 1 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 5 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 2 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 6 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 3 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 7 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 0 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 4 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 1 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 5 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 2 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 6 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 3 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 7 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        }
    };

    internal static AnimationCommandStruct[][] g_table_animation_unitScript1 = { //[4][8]
    	new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 0 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 1 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 0 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 4 },
            new() { command = (byte)AnimationCommand.ANIMATION_PLAY_VOICE, parameter = 35 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 5 },
            new() { command = (byte)AnimationCommand.ANIMATION_PLAY_VOICE, parameter = 35 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        }
    };

    internal static AnimationCommandStruct[][] g_table_animation_unitScript2 = { //[4][8]
    	new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 2 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 3 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 2 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 4 },
            new() { command = (byte)AnimationCommand.ANIMATION_PLAY_VOICE, parameter = 35 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 5 },
            new() { command = (byte)AnimationCommand.ANIMATION_PLAY_VOICE, parameter = 35 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        }
    };

    internal static AnimationCommandStruct[][] g_table_animation_map = { //[16][8]
    	new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 1 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 2 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 1 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 2 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 0 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 0 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 4 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 5 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 4 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 5 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 3 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 3 },
            new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        },
        new AnimationCommandStruct[] {
            new() { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
        }
    };

    internal static AnimationCommandStruct[][] g_table_animation_structure;

    static TableAnimation()
    {
        unchecked
        {
            g_table_animation_structure = new AnimationCommandStruct[29][] { //[29][16]
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 1 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 300 },
                    new() { command = (byte)AnimationCommand.ANIMATION_ABORT, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 0 },
                    new() { command = (byte)AnimationCommand.ANIMATION_ABORT, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 300 },
                    new() { command = (byte)AnimationCommand.ANIMATION_ABORT, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 4 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 6 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 7 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 8 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 9 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 7 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 6 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 4 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_FORWARD, parameter = (ushort)-4 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 4 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 6 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 7 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_FORWARD, parameter = (ushort)-4 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 4 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 6 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 7 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_FORWARD, parameter = (ushort)-4 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 4 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 4 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_FORWARD, parameter = (ushort)-4 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 6 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 7 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 8 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 9 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 8 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 9 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 6 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
                    new() { command = (byte)AnimationCommand.ANIMATION_FORWARD, parameter = (ushort)-4 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 6 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 9 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 8 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
                    new() { command = (byte)AnimationCommand.ANIMATION_FORWARD, parameter = (ushort)-4 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                },
                new AnimationCommandStruct[] {
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
                    new() { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
                    new() { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
                }
            };
        }
    }
}
