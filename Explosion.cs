/* Explosion */

using SharpDune.Audio;
using SharpDune.Os;
using System.Diagnostics;
using static SharpDune.Table.TableAnimation;
using static SharpDune.Table.TableExplosion;
using static SharpDune.Table.TableLandscapeInfo;

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

		static readonly Explosion[] g_explosions = new Explosion[EXPLOSION_MAX]; /*!< Explosions. */

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

		static readonly short[] craterIconMapIndex = { -1, 2, 1 };
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

			if (g_table_landscapeInfo[type].craterType == 0) return;

			/* You cannot damage veiled tiles */
			overlayTileID = t.overlayTileID;
			if (!Sprites.Tile_IsUnveiled(overlayTileID)) return;

			iconMapIndex = craterIconMapIndex[g_table_landscapeInfo[type].craterType];
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
			animationMapID += (ushort)(g_table_landscapeInfo[Map.Map_GetLandscapeType(packed)].isSand ? 0 : 2);

			Debug.Assert(animationMapID < 16);
			CAnimation.Animation_Start(g_table_animation_map[animationMapID], e.position, 0, e.houseID, 3);
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
