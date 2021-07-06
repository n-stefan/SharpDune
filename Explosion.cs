/* Explosion */

using System.Diagnostics;

namespace SharpDune
{
	/*
	 * The valid types for command in Explosion.
	 */
	enum ExplosionCommand
	{
		EXPLOSION_STOP,                                       /*!< Stop the Explosion. */
		EXPLOSION_SET_SPRITE,                                 /*!< Set the sprite for the Explosion. */
		EXPLOSION_SET_TIMEOUT,                                /*!< Set the timeout for the Explosion. */
		EXPLOSION_SET_RANDOM_TIMEOUT,                         /*!< Set a random timeout for the Explosion. */
		EXPLOSION_MOVE_Y_POSITION,                            /*!< Move the Y-position of the Explosion. */
		EXPLOSION_TILE_DAMAGE,                                /*!< Handle damage to a tile in a Explosion. */
		EXPLOSION_PLAY_VOICE,                                 /*!< Play a voice. */
		EXPLOSION_SCREEN_SHAKE,                               /*!< Shake the screen around. */
		EXPLOSION_SET_ANIMATION,                              /*!< Set the animation for the Explosion. */
		EXPLOSION_BLOOM_EXPLOSION                             /*!< Make a bloom explode. */
	}

	/*
	 * Types of Explosions available in the game.
	 */
	enum ExplosionType
	{
		EXPLOSION_IMPACT_SMALL = 0,
		EXPLOSION_IMPACT_MEDIUM = 1,
		EXPLOSION_IMPACT_LARGE = 2,
		EXPLOSION_IMPACT_EXPLODE = 3,
		EXPLOSION_SABOTEUR_DEATH = 4,
		EXPLOSION_SABOTEUR_INFILTRATE = 5,
		EXPLOSION_TANK_EXPLODE = 6,
		EXPLOSION_DEVIATOR_GAS = 7,
		EXPLOSION_SAND_BURST = 8,
		EXPLOSION_TANK_FLAMES = 9,
		EXPLOSION_WHEELED_VEHICLE = 10,
		EXPLOSION_DEATH_HAND = 11,
		EXPLOSION_UNUSED_12 = 12,
		EXPLOSION_SANDWORM_SWALLOW = 13,
		EXPLOSION_STRUCTURE = 14,
		EXPLOSION_SMOKE_PLUME = 15,
		EXPLOSION_ORNITHOPTER_CRASH = 16,
		EXPLOSION_CARRYALL_CRASH = 17,
		EXPLOSION_MINI_ROCKET = 18,
		EXPLOSION_SPICE_BLOOM_TREMOR = 19,

		EXPLOSIONTYPE_MAX = 20,
		EXPLOSION_INVALID = 0xFFFF
	}

	/*
	 * The layout of a single explosion command.
	 */
	class ExplosionCommandStruct
	{
		internal byte command;                                /*!< The command of the Explosion. */
		internal ushort parameter;                            /*!< The parameter of the Explosion. */
	}

	/*
	 * The layout of a Explosion.
	 */
	class Explosion
	{
		internal uint timeOut;                                /*!< Time out for the next command. */
		internal byte houseID;                                /*!< A houseID. */
		internal bool isDirty;                                /*!< Does the Explosion require a redraw next round. */
		internal byte current;                                /*!< Index in #commands pointing to the next command. */
		internal ushort spriteID;                             /*!< SpriteID. */
		internal ExplosionCommandStruct[] commands;           /*!< Commands being executed. */
		internal tile32 position;                             /*!< Position where this explosion acts. */

        internal Explosion() =>
			position = new tile32();
    }

	class CExplosion
	{
		internal const byte EXPLOSION_MAX = 32;                         /*!< The maximum amount of active explosions we can have. */

		static Explosion[] g_explosions = new Explosion[EXPLOSION_MAX]; /*!< Explosions. */

		static ExplosionCommandStruct[] s_explosion09;
		static ExplosionCommandStruct[] s_explosion15;

		static CExplosion()
        {
			unchecked
            {
				/* EXPLOSION_TANK_FLAMES */
				s_explosion09 = new ExplosionCommandStruct[] {
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 183 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 41 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_TILE_DAMAGE, parameter = 0 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 203 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_MOVE_Y_POSITION, parameter = (ushort)-80 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 168 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 169 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 170 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 168 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 169 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 170 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 168 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 169 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 170 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 168 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 169 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 170 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 168 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 169 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 170 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
				};

				/* EXPLOSION_SMOKE_PLUME */
				s_explosion15 = new ExplosionCommandStruct[] {
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 183 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 49 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_MOVE_Y_POSITION, parameter = (ushort)-80 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 184 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 180 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 181 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 182 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 181 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 180 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 181 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 182 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 181 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 180 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 181 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 182 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 181 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 180 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 181 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 182 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 181 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 180 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 181 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 182 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 181 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 180 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 181 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 182 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 181 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
					new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
				};
			}
		}

		/* EXPLOSION_IMPACT_SMALL */
		static ExplosionCommandStruct[] s_explosion00 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 153 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 153 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_IMPACT_MEDIUM */
		static ExplosionCommandStruct[] s_explosion01 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 154 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 153 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 154 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_IMPACT_LARGE */
		static ExplosionCommandStruct[] s_explosion02 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 183 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 50 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_TILE_DAMAGE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 184 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_IMPACT_EXPLODE */
		static ExplosionCommandStruct[] s_explosion03 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 183 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 49 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_TILE_DAMAGE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 184 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_SABOTEUR_DEATH */
		static ExplosionCommandStruct[] s_explosion04 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 203 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 51 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_TILE_DAMAGE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 7 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 204 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 205 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 206 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 207 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_SABOTEUR_INFILTRATE */
		static ExplosionCommandStruct[] s_explosion05 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_RANDOM_TIMEOUT, parameter = 60 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 203 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 41 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_TILE_DAMAGE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 7 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 204 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 205 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 206 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 207 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_TANK_EXPLODE */
		static ExplosionCommandStruct[] s_explosion06 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 198 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 51 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_TILE_DAMAGE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 7 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 199 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 200 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 201 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 202 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_DEVIATOR_GAS */
		static ExplosionCommandStruct[] s_explosion07 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 208 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 39 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 209 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 210 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 211 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 212 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_SAND_BURST */
		static ExplosionCommandStruct[] s_explosion08 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 156 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 40 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 7 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 157 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 158 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 157 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_TILE_DAMAGE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_WHEELED_VEHICLE */
		static ExplosionCommandStruct[] s_explosion10 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 151 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 49 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_TILE_DAMAGE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 7 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 152 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 7 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_DEATH_HAND */
		static ExplosionCommandStruct[] s_explosion11 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_RANDOM_TIMEOUT, parameter = 60 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 188 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 51 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_TILE_DAMAGE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 7 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 189 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 190 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 191 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 192 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_UNUSED_12 */
		static ExplosionCommandStruct[] s_explosion12 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 213 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 214 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 215 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 216 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 217 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 30 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_SANDWORM_SWALLOW */
		static ExplosionCommandStruct[] s_explosion13 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 218 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 219 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 220 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 221 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 15 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 222 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 30 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_STRUCTURE */
		static ExplosionCommandStruct[] s_explosion14 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_RANDOM_TIMEOUT, parameter = 60 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 188 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 51 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 7 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 189 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SCREEN_SHAKE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 190 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 191 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 192 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_ORNITHOPTER_CRASH */
		static ExplosionCommandStruct[] s_explosion16 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 203 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 49 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_ANIMATION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 204 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 207 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_CARRYALL_CRASH */
		static ExplosionCommandStruct[] s_explosion17 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 203 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 49 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_ANIMATION, parameter = 4 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 204 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 207 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_MINI_ROCKET */
		static ExplosionCommandStruct[] s_explosion18 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 183 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 54 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 184 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		/* EXPLOSION_SPICE_BLOOM_TREMOR */
		static ExplosionCommandStruct[] s_explosion19 = {
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 156 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 40 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SCREEN_SHAKE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 7 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SCREEN_SHAKE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 157 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SCREEN_SHAKE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 158 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SCREEN_SHAKE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 157 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_SCREEN_SHAKE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_TILE_DAMAGE, parameter = 0 },
			new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
		};

		static ExplosionCommandStruct[][] g_table_explosion = { //[EXPLOSIONTYPE_MAX]
			s_explosion00,
			s_explosion01,
			s_explosion02,
			s_explosion03,
			s_explosion04,
			s_explosion05,
			s_explosion06,
			s_explosion07,
			s_explosion08,
			s_explosion09,
			s_explosion10,
			s_explosion11,
			s_explosion12,
			s_explosion13,
			s_explosion14,
			s_explosion15,
			s_explosion16,
			s_explosion17,
			s_explosion18,
			s_explosion19
		};

		static uint s_explosionTimer;                               /*!< Timeout value for next explosion activity. */

        /*
		 * Timer tick for explosions.
		 */
        internal static void Explosion_Tick()
		{
			byte i;

			if (s_explosionTimer > Timer.g_timerGUI) return;
			s_explosionTimer += 10000;

			for (i = 0; i < EXPLOSION_MAX; i++)
			{
				Explosion e;

				e = g_explosions[i];

				if (e.commands == null) continue;

				if (e.timeOut <= Timer.g_timerGUI)
				{
					var parameter = e.commands[e.current].parameter;
					ushort command = e.commands[e.current].command;

					e.current++;

					switch ((ExplosionCommand)command)
					{
						default:
						case ExplosionCommand.EXPLOSION_STOP: Explosion_Func_Stop(e, parameter); break;

						case ExplosionCommand.EXPLOSION_SET_SPRITE: Explosion_Func_SetSpriteID(e, parameter); break;
						case ExplosionCommand.EXPLOSION_SET_TIMEOUT: Explosion_Func_SetTimeout(e, parameter); break;
						case ExplosionCommand.EXPLOSION_SET_RANDOM_TIMEOUT: Explosion_Func_SetRandomTimeout(e, parameter); break;
						case ExplosionCommand.EXPLOSION_MOVE_Y_POSITION: Explosion_Func_MoveYPosition(e, parameter); break;
						case ExplosionCommand.EXPLOSION_TILE_DAMAGE: Explosion_Func_TileDamage(e, parameter); break;
						case ExplosionCommand.EXPLOSION_PLAY_VOICE: Explosion_Func_PlayVoice(e, parameter); break;
						case ExplosionCommand.EXPLOSION_SCREEN_SHAKE: Explosion_Func_ScreenShake(e, parameter); break;
						case ExplosionCommand.EXPLOSION_SET_ANIMATION: Explosion_Func_SetAnimation(e, parameter); break;
						case ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION: Explosion_Func_BloomExplosion(e, parameter); break;
					}
				}

				if (e.commands == null || e.timeOut > s_explosionTimer) continue;

				s_explosionTimer = e.timeOut;
			}
		}

		/*
		 * Update the tile a Explosion is on.
		 * @param type Are we introducing (0) or updating (2) the tile.
		 * @param e The Explosion in question.
		 */
		static void Explosion_Update(ushort type, Explosion e)
		{
			if (e == null) return;

			if (type == 1 && e.isDirty) return;

			e.isDirty = type != 0;

			Map.Map_UpdateAround(24, e.position, null, Map.g_functions[2][type]);
		}

		/*
		 * Stop performing an explosion.
		 * @param e The Explosion to end.
		 * @param parameter Unused parameter.
		 */
		static void Explosion_Func_Stop(Explosion e, ushort parameter)
		{
			Map.g_map[CTile.Tile_PackTile(e.position)].hasExplosion = false;

			Explosion_Update(0, e);

			e.commands = null;
		}

		/*
		 * Set the SpriteID of the Explosion.
		 * @param e The Explosion to change.
		 * @param spriteID The new SpriteID for the Explosion.
		 */
		static void Explosion_Func_SetSpriteID(Explosion e, ushort spriteID)
		{
			e.spriteID = spriteID;

			Explosion_Update(2, e);
		}

		/*
		 * Set timeout for next the activity of \a e.
		 * @param e The Explosion to change.
		 * @param value The new timeout value.
		 */
		static void Explosion_Func_SetTimeout(Explosion e, ushort value) =>
			e.timeOut = Timer.g_timerGUI + value;

		/*
		 * Set timeout for next the activity of \a e to a random value up to \a value.
		 * @param e The Explosion to change.
		 * @param value The maximum amount of timeout.
		 */
		static void Explosion_Func_SetRandomTimeout(Explosion e, ushort value) =>
			e.timeOut = Timer.g_timerGUI + Tools.Tools_RandomLCG_Range(0, value);

		/*
		 * Set position at the left of a row.
		 * @param e The Explosion to change.
		 * @param row Row number.
		 */
		static void Explosion_Func_MoveYPosition(Explosion e, ushort row) =>
			e.position.y += row;

		static short[] craterIconMapIndex = { -1, 2, 1 };
		/*
		 * Handle damage to a tile, removing spice, removing concrete, stuff like that.
		 * @param e The Explosion to handle damage on.
		 * @param parameter Unused parameter.
		 */
		static void Explosion_Func_TileDamage(Explosion e, ushort parameter)
		{
			ushort packed;
			ushort type;
			Tile t;
			short iconMapIndex;
			ushort overlayTileID;
			ushort[] iconMap;

			packed = CTile.Tile_PackTile(e.position);

			if (!Map.Map_IsPositionUnveiled(packed)) return;

			type = Map.Map_GetLandscapeType(packed);

			if (type == (ushort)LandscapeType.LST_STRUCTURE || type == (ushort)LandscapeType.LST_DESTROYED_WALL) return;

			t = Map.g_map[packed];

			if (type == (ushort)LandscapeType.LST_CONCRETE_SLAB)
			{
				t.groundTileID = Map.g_mapTileID[packed];
				Map.Map_Update(packed, 0, false);
			}

			if (Map.g_table_landscapeInfo[type].craterType == 0) return;

			/* You cannot damage veiled tiles */
			overlayTileID = t.overlayTileID;
			if (!Sprites.Tile_IsUnveiled(overlayTileID)) return;

			iconMapIndex = craterIconMapIndex[Map.g_table_landscapeInfo[type].craterType];
			iconMap = Sprites.g_iconMap[Sprites.g_iconMap[iconMapIndex]..];

			if (iconMap[0] <= overlayTileID && overlayTileID <= iconMap[10])
			{
				/* There already is a crater; make it bigger */
				overlayTileID -= iconMap[0];
				if (overlayTileID < 4) overlayTileID += 2;
			}
			else
			{
				/* Randomly pick 1 of the 2 possible craters */
				overlayTileID = (ushort)(Tools.Tools_Random_256() & 1);
			}

			/* Reduce spice if there is any */
			Map.Map_ChangeSpiceAmount(packed, -1);

			/* Boom a bloom if there is one */
			if (t.groundTileID == Sprites.g_bloomTileID)
			{
				Map.Map_Bloom_ExplodeSpice(packed, (byte)CHouse.g_playerHouseID);
				return;
			}

			/* Update the tile with the crater */
			t.overlayTileID = (ushort)(overlayTileID + iconMap[0]);
			Map.Map_Update(packed, 0, false);
		}

		/*
		 * Play a voice for a Explosion.
		 * @param e The Explosion to play the voice on.
		 * @param voiceID The voice to play.
		 */
		static void Explosion_Func_PlayVoice(Explosion e, ushort voiceID) =>
			Sound.Voice_PlayAtTile((short)voiceID, e.position);

		/*
		 * Shake the screen.
		 * @param e The Explosion.
		 * @param parameter Unused parameter.
		 */
		static void Explosion_Func_ScreenShake(Explosion e, ushort parameter)
		{
			int i;

			Debug.WriteLine($"DEBUG: Explosion_Func_ScreenShake({e}, {parameter})");

			for (i = 0; i < 2; i++)
			{
				Sleep.msleep(30);
				Sdl2Video.Video_SetOffset(320);
				Sleep.msleep(30);
				Sdl2Video.Video_SetOffset(0);
			}
		}

		/*
		 * Set the animation of a Explosion.
		 * @param e The Explosion to change.
		 * @param animationMapID The animation map to use.
		 */
		static void Explosion_Func_SetAnimation(Explosion e, ushort animationMapID)
		{
			ushort packed;

			packed = CTile.Tile_PackTile(e.position);

			if (CStructure.Structure_Get_ByPackedTile(packed) != null) return;

			animationMapID += (ushort)(Tools.Tools_Random_256() & 0x1);
			animationMapID += (ushort)(Map.g_table_landscapeInfo[Map.Map_GetLandscapeType(packed)].isSand ? 0 : 2);

			Debug.Assert(animationMapID < 16);
			CAnimation.Animation_Start(CAnimation.g_table_animation_map[animationMapID], e.position, 0, e.houseID, 3);
		}

		/*
		 * Check if there is a bloom at the location, and make it explode if needed.
		 * @param e The Explosion to perform the explosion on.
		 * @param parameter Unused parameter.
		 */
		static void Explosion_Func_BloomExplosion(Explosion e, ushort parameter)
		{
			ushort packed;

			packed = CTile.Tile_PackTile(e.position);

			if (Map.g_map[packed].groundTileID != Sprites.g_bloomTileID) return;

			Map.Map_Bloom_ExplodeSpice(packed, (byte)CHouse.g_playerHouseID);
		}

		/*
		 * Start a Explosion on a tile.
		 * @param explosionType Type of Explosion.
		 * @param position The position to use for init.
		 */
		internal static void Explosion_Start(ushort explosionType, tile32 position)
		{
			ExplosionCommandStruct[] commands;
			ushort packed;
			byte i;

			if (explosionType > (ushort)ExplosionType.EXPLOSION_SPICE_BLOOM_TREMOR) return;
			commands = g_table_explosion[explosionType];

			packed = CTile.Tile_PackTile(position);

			Explosion_StopAtPosition(packed);

			for (i = 0; i < EXPLOSION_MAX; i++)
			{
				Explosion e;

				e = g_explosions[i];

				if (e.commands != null) continue;

				e.commands = commands;
				e.current = 0;
				e.spriteID = 0;
				e.position = position;
				e.isDirty = false;
				e.timeOut = Timer.g_timerGUI;
				s_explosionTimer = 0;
				Map.g_map[packed].hasExplosion = true;

				break;
			}
		}

		/*
		 * Stop any Explosion at position \a packed.
		 * @param packed A packed position where no activities should take place (any more).
		 */
		static void Explosion_StopAtPosition(ushort packed)
		{
			Tile t;
			byte i;

			t = Map.g_map[packed];

			if (!t.hasExplosion) return;

			for (i = 0; i < EXPLOSION_MAX; i++)
			{
				Explosion e;

				e = g_explosions[i];

				if (e.commands == null || CTile.Tile_PackTile(e.position) != packed) continue;

				Explosion_Func_Stop(e, 0);
			}
		}

		internal static Explosion Explosion_Get_ByIndex(int i)
		{
			Debug.Assert(0 <= i && i < EXPLOSION_MAX);

			return g_explosions[i];
		}

		internal static void Explosion_Init()
		{
			for (var i = 0; i < g_explosions.Length; i++) g_explosions[i] = new Explosion(); //memset(g_explosions, 0, EXPLOSION_MAX * sizeof(Explosion));
		}
	}
}
