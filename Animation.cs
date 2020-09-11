/* Animation */

using System.Diagnostics;

namespace SharpDune
{
    /*
	 * The valid types for command in AnimationCommandStruct.
	 */
    enum AnimationCommand
	{
		ANIMATION_STOP,                                         /*!< Gracefully stop with animation. Clean up the tiles etc. */
		ANIMATION_ABORT,                                        /*!< Abort animation. Leave it as it is. */
		ANIMATION_SET_OVERLAY_TILE,                             /*!< Set a new overlay tile. Param: the new overlay tile. */
		ANIMATION_PAUSE,                                        /*!< Pause the animation. Param: amount of ticks to pause. */
		ANIMATION_REWIND,                                       /*!< Rewind the animation.*/
		ANIMATION_PLAY_VOICE,                                   /*!< Play a voice. Param: the voice to play. */
		ANIMATION_SET_GROUND_TILE,                              /*!< Set a new ground tile. Param: the new ground tile. */
		ANIMATION_FORWARD,                                      /*!< Forward the animation. Param: how many commands to forward. */
		ANIMATION_SET_ICONGROUP                                 /*!< Set a newicongroup. Param: the new icongroup. */
	}

	/*
	 * How a single command looks like.
	 */
	class AnimationCommandStruct
	{
		internal byte command;                          /*!< The command of this command (see AnimationCommand). */
		internal ushort parameter;                      /*!< The parameter for this command. */
	}

	class Animation
	{
		internal uint tickNext;                         /*!< Which tick this Animation should be called again. */
		internal ushort tileLayout;                     /*!< Tile layout of the Animation. */
		internal byte houseID;                          /*!< House of the item being animated. */
		internal byte current;                          /*!< At which command we currently are in the Animation. */
		internal byte iconGroup;                        /*!< Which iconGroup the sprites of the Animation belongs. */
		internal AnimationCommandStruct[] commands;     /*!< List of commands for this Animation. */
		internal tile32 tile;                           /*!< Top-left tile of Animation. */
	}

	class CAnimation
	{
		const byte ANIMATION_MAX = 112;

		static Animation[] g_animations = new Animation[ANIMATION_MAX];
		static uint s_animationTimer; /*!< Timer for animations. */

		internal static AnimationCommandStruct[][] g_table_animation_structure;

		static CAnimation()
        {
			unchecked
            {
				g_table_animation_structure = new AnimationCommandStruct[29][] { //[29][16]
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 1 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 300 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_ABORT, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 0 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_ABORT, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 300 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_ABORT, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 4 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 6 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 7 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 8 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 9 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 7 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 6 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 4 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_FORWARD, parameter = (ushort)-4 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 4 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 6 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 7 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_FORWARD, parameter = (ushort)-4 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 4 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 6 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 7 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_FORWARD, parameter = (ushort)-4 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 4 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 4 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_FORWARD, parameter = (ushort)-4 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 6 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 7 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 8 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 9 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 8 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 9 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 6 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_FORWARD, parameter = (ushort)-4 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 5 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 6 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 9 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 8 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 60 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_FORWARD, parameter = (ushort)-4 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					},
					new AnimationCommandStruct[] {
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 2 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_GROUND_TILE, parameter = 3 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 30 },
						new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_REWIND, parameter = 0 }
					}
				};
			}
        }

		internal static AnimationCommandStruct[][] g_table_animation_map = { //[16][8]
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 1 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 2 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 1 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 2 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 0 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 0 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 4 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 5 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 4 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 5 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 3 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 3 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			}
		};

		internal static AnimationCommandStruct[][] g_table_animation_unitMove = { //[8][8]
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 0 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 4 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 1 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 5 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 2 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 6 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 3 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 7 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 0 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 4 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 1 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 5 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 2 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 6 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 3 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 7 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			}
		};

		internal static AnimationCommandStruct[][] g_table_animation_unitScript1 = { //[4][8]
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 0 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 1 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 0 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 4 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PLAY_VOICE, parameter = 35 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 5 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PLAY_VOICE, parameter = 35 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			}
		};

		internal static AnimationCommandStruct[][] g_table_animation_unitScript2 = { //[4][8]
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 2 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 3 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 2 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 4 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PLAY_VOICE, parameter = 35 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			},
			new AnimationCommandStruct[] {
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_SET_OVERLAY_TILE, parameter = 5 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PLAY_VOICE, parameter = 35 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_PAUSE, parameter = 600 },
				new AnimationCommandStruct { command = (byte)AnimationCommand.ANIMATION_STOP, parameter = 0 }
			}
		};

		/*
		 * Stop with this Animation.
		 * @param animation The Animation to stop.
		 * @param parameter Not used.
		 */
		static void Animation_Func_Stop(Animation animation, short parameter)
		{
			ushort[] layout = CStructure.g_table_structure_layoutTiles[animation.tileLayout];
			ushort layoutTileCount = CStructure.g_table_structure_layoutTileCount[animation.tileLayout];
			ushort packed = CTile.Tile_PackTile(animation.tile);
			int i;
			int layoutPointer = 0;
			//VARIABLE_NOT_USED(parameter);

			Map.g_map[packed].hasAnimation = false;
			animation.commands = null;

			for (i = 0; i < layoutTileCount; i++)
			{
				ushort position = (ushort)(packed + layout[layoutPointer++]);
				Tile t = Map.g_map[position];

				if (animation.tileLayout != 0)
				{
					t.groundTileID = Map.g_mapTileID[position];
				}

				if (Map.Map_IsPositionUnveiled(position))
				{
					t.overlayTileID = 0;
				}

				Map.Map_Update(position, 0, false);
			}
		}

		/*
		 * Abort this Animation.
		 * @param animation The Animation to abort.
		 * @param parameter Not used.
		 */
		static void Animation_Func_Abort(Animation animation, short parameter)
		{
			ushort packed = CTile.Tile_PackTile(animation.tile);
			//VARIABLE_NOT_USED(parameter);

			Map.g_map[packed].hasAnimation = false;
			animation.commands = null;

			Map.Map_Update(packed, 0, false);
		}

		/*
		 * Set the overlay sprite of the tile.
		 * @param animation The Animation for which we change the overlay sprite.
		 * @param parameter The TileID to which the overlay sprite is set.
		 */
		static void Animation_Func_SetOverlayTile(Animation animation, short parameter)
		{
			ushort packed = CTile.Tile_PackTile(animation.tile);
			Tile t = Map.g_map[packed];
			Debug.Assert(parameter >= 0);

			if (!Map.Map_IsPositionUnveiled(packed)) return;

			t.overlayTileID = Sprites.g_iconMap[Sprites.g_iconMap[animation.iconGroup] + parameter];
			t.houseID = animation.houseID;

			Map.Map_Update(packed, 0, false);
		}

		/*
		 * Pause the animation for a few ticks.
		 * @param animation The Animation to pause.
		 * @param parameter How many ticks it should pause.
		 * @note Delays are randomly delayed with [0..3] ticks.
		 */
		static void Animation_Func_Pause(Animation animation, short parameter)
		{
			Debug.Assert(parameter >= 0);

			animation.tickNext = (uint)(Timer.g_timerGUI + parameter + (Tools.Tools_Random_256() % 4));
		}

		/*
		 * Rewind the animation.
		 * @param animation The Animation to rewind.
		 * @param parameter Not used.
		 */
		static void Animation_Func_Rewind(Animation animation, short parameter)
		{
			//VARIABLE_NOT_USED(parameter);

			animation.current = 0;
		}

		/*
		 * Play a Voice on the tile of animation.
		 * @param animation The Animation which gives the position the voice plays at.
		 * @param parameter The VoiceID to play.
		 */
		static void Animation_Func_PlayVoice(Animation animation, short parameter) =>
			Sound.Voice_PlayAtTile(parameter, animation.tile);

		/*
		 * Set the ground sprite of the tile.
		 * @param animation The Animation for which we change the ground sprite.
		 * @param parameter The offset in the iconGroup to which the ground sprite is set.
		 */
		static void Animation_Func_SetGroundTile(Animation animation, short parameter)
		{
			ushort[] specialMap = new ushort[1];
			ushort[] iconMap;
			ushort[] layout = CStructure.g_table_structure_layoutTiles[animation.tileLayout];
			ushort layoutTileCount = CStructure.g_table_structure_layoutTileCount[animation.tileLayout];
			ushort packed = CTile.Tile_PackTile(animation.tile);
			int i;
			int layoutPointer = 0;
			int iconMapPointer = 0;

			iconMap = Sprites.g_iconMap[(Sprites.g_iconMap[animation.iconGroup] + layoutTileCount * parameter)..];

			/* Some special case for turrets */
			if ((parameter > 1) && (animation.iconGroup == (byte)IconMapEntries.ICM_ICONGROUP_BASE_DEFENSE_TURRET || animation.iconGroup == (byte)IconMapEntries.ICM_ICONGROUP_BASE_ROCKET_TURRET))
			{
				Structure s = CStructure.Structure_Get_ByPackedTile(packed);
				Debug.Assert(s != null);
				Debug.Assert(layoutTileCount == 1);

				specialMap[0] = (ushort)(s.rotationSpriteDiff + Sprites.g_iconMap[Sprites.g_iconMap[animation.iconGroup]] + 2);
				iconMap = specialMap;
			}

			for (i = 0; i < layoutTileCount; i++)
			{
				ushort position = (ushort)(packed + layout[layoutPointer++]);
				ushort tileID = iconMap[iconMapPointer++];
				Tile t = Map.g_map[position];

				if (t.groundTileID == tileID) continue;
				t.groundTileID = tileID;
				t.houseID = animation.houseID;

				if (Map.Map_IsPositionUnveiled(position))
				{
					t.overlayTileID = 0;
				}

				Map.Map_Update(position, 0, false);

				Map.Map_MarkTileDirty(position);
			}
		}

		/*
		 * Forward the current Animation with the given amount of steps.
		 * @param animation The Animation to forward.
		 * @param parameter With what value you want to forward the Animation.
		 * @note Forwarding with 1 is just the next instruction, making this command a NOP.
		 */
		static void Animation_Func_Forward(Animation animation, short parameter) =>
			animation.current += (byte)(parameter - 1);

		/*
		 * Set the IconGroup of the Animation.
		 * @param animation The Animation to change.
		 * @param parameter To what value IconGroup should change.
		 */
		static void Animation_Func_SetIconGroup(Animation animation, short parameter)
		{
			Debug.Assert(parameter >= 0);

			animation.iconGroup = (byte)parameter;
		}

		/*
		 * Stop an Animation on a tile, if any.
		 * @param packed The tile to check for animation on.
		 */
		internal static void Animation_Stop_ByTile(ushort packed)
		{
			Animation[] animation = g_animations;
			Tile t = Map.g_map[packed];
			int i;

			if (!t.hasAnimation) return;

			for (i = 0; i < ANIMATION_MAX; i++)
			{ //, animation++) {
				if (animation[i].commands == null) continue;
				if (CTile.Tile_PackTile(animation[i].tile) != packed) continue;

				Animation_Func_Stop(animation[i], 0);
				return;
			}
		}

		/*
		 * Start an Animation.
		 * @param commands List of commands for the Animation.
		 * @param tile The tile to do the Animation on.
		 * @param layout The layout of tiles for the Animation.
		 * @param houseID The house of the item being Animation.
		 * @param iconGroup In which IconGroup the sprites of the Animation belongs.
		 */
		internal static void Animation_Start(AnimationCommandStruct[] commands, tile32 tile, ushort tileLayout, byte houseID, byte iconGroup)
		{
			Animation[] animation = g_animations;
			ushort packed = CTile.Tile_PackTile(tile);
			Tile t;
			int i;

			t = Map.g_map[packed];
			Animation_Stop_ByTile(packed);

			for (i = 0; i < ANIMATION_MAX; i++)
			{ //, animation++) {
				if (animation[i].commands != null) continue;

				animation[i].tickNext = Timer.g_timerGUI;
				animation[i].tileLayout = tileLayout;
				animation[i].houseID = houseID;
				animation[i].current = 0;
				animation[i].iconGroup = iconGroup;
				animation[i].commands = commands;
				animation[i].tile = tile;

				s_animationTimer = 0;

				t.houseID = houseID;
				t.hasAnimation = true;
				return;
			}
		}

		/*
		 * Check all Animations if they need changing.
		 */
		internal static void Animation_Tick()
		{
			Animation[] animation = g_animations;
			int i;

			if (s_animationTimer > Timer.g_timerGUI) return;
			s_animationTimer += 10000;

			for (i = 0; i < ANIMATION_MAX; i++)
			{ //, animation++) {
				if (animation[i].commands == null) continue;

				if (animation[i].tickNext <= Timer.g_timerGUI)
				{
					AnimationCommandStruct commands = animation[i].commands[animation[i].current];
					short parameter = (short)commands.parameter;
					Debug.Assert((parameter & 0x0800) == 0 || (parameter & 0xF000) != 0); /* Validate if the compiler sign-extends correctly */

					animation[i].current++;

					switch ((AnimationCommand)commands.command)
					{
						case AnimationCommand.ANIMATION_STOP:
						default: Animation_Func_Stop(animation[i], parameter); break;

						case AnimationCommand.ANIMATION_ABORT: Animation_Func_Abort(animation[i], parameter); break;
						case AnimationCommand.ANIMATION_SET_OVERLAY_TILE: Animation_Func_SetOverlayTile(animation[i], parameter); break;
						case AnimationCommand.ANIMATION_PAUSE: Animation_Func_Pause(animation[i], parameter); break;
						case AnimationCommand.ANIMATION_REWIND: Animation_Func_Rewind(animation[i], parameter); break;
						case AnimationCommand.ANIMATION_PLAY_VOICE: Animation_Func_PlayVoice(animation[i], parameter); break;
						case AnimationCommand.ANIMATION_SET_GROUND_TILE: Animation_Func_SetGroundTile(animation[i], parameter); break;
						case AnimationCommand.ANIMATION_FORWARD: Animation_Func_Forward(animation[i], parameter); break;
						case AnimationCommand.ANIMATION_SET_ICONGROUP: Animation_Func_SetIconGroup(animation[i], parameter); break;
					}

					if (animation[i].commands == null) continue;
				}

				if (animation[i].tickNext < s_animationTimer) s_animationTimer = animation[i].tickNext;
			}
		}

		internal static void Animation_Init()
        {
			for (var i = 0; i < g_animations.Length; i++) g_animations[i] = new Animation(); //memset(g_animations, 0, ANIMATION_MAX * sizeof(Animation));
		}	
	}
}
