/* Unit script routines */

using SharpDune.Audio;
using SharpDune.Include;
using SharpDune.Os;
using SharpDune.Pool;
using System;
using System.Diagnostics;
using static SharpDune.CUnit;
using static SharpDune.Script.Script;
using static SharpDune.Table.TableAnimation;
using static SharpDune.Table.TableHouseInfo;
using static SharpDune.Table.TableLandscapeInfo;
using static SharpDune.Table.TableStrings;
using static SharpDune.Table.TableStructureInfo;
using static SharpDune.Table.TableUnitInfo;
using static System.Math;

namespace SharpDune.Script
{
    class Pathfinder_Data
	{
		internal ushort packed;                                 /*!< From where we are pathfinding. */
		internal short score;                                   /*!< The total score for this route. */
		internal ushort routeSize;                              /*!< The size of this route. */
		internal byte[] buffer;                                 /*!< A buffer to store the route. */
	}

	class ScriptUnit
    {
		static readonly short[] s_mapDirection = { -64, -63, 1, 65, 64, 63, -1, -65 }; /*!< Tile index change when moving in a direction. */

		/*
		 * Create a new soldier unit.
		 *
		 * Stack: 1 - Action for the new Unit.
		 *
		 * @param script The script engine to operate on.
		 * @return 1 if a new Unit has been created, 0 otherwise.
		 */
		internal static ushort Script_Unit_RandomSoldier(ScriptEngine script)
		{
			Unit u;
			Unit nu;
			tile32 position;

			u = g_scriptCurrentUnit;

			if (Tools.Tools_Random_256() >= g_table_unitInfo[u.o.type].o.spawnChance) return 0;

			position = CTile.Tile_MoveByRandom(u.o.position, 20, true);

			nu = Unit_Create((ushort)PoolUnit.UnitIndex.UNIT_INDEX_INVALID, (byte)UnitType.UNIT_SOLDIER, u.o.houseID, position, (sbyte)Tools.Tools_Random_256());

			if (nu == null) return 0;

			nu.deviated = u.deviated;

			Unit_SetAction(nu, (ActionType)STACK_PEEK(script, 1));

			return 1;
		}

		/*
		 * Gets the best target for the current unit.
		 *
		 * Stack: 1 - How to determine the best target.
		 *
		 * @param script The script engine to operate on.
		 * @return The encoded index of the best target or 0 if none found.
		 */
		internal static ushort Script_Unit_FindBestTarget(ScriptEngine script)
		{
			Unit u;

			u = g_scriptCurrentUnit;

			return Unit_FindBestTargetEncoded(u, STACK_PEEK(script, 1));
		}

		/*
		 * Get the priority a target has for the current unit.
		 *
		 * Stack: 1 - The encoded target.
		 *
		 * @param script The script engine to operate on.
		 * @return The priority of the target.
		 */
		internal static ushort Script_Unit_GetTargetPriority(ScriptEngine script)
		{
			Unit u;
			Unit target;
			Structure s;
			ushort encoded;

			u = g_scriptCurrentUnit;
			encoded = STACK_PEEK(script, 1);

			target = Tools.Tools_Index_GetUnit(encoded);
			if (target != null) return Unit_GetTargetUnitPriority(u, target);

			s = Tools.Tools_Index_GetStructure(encoded);
			if (s == null) return 0;

			return Unit_GetTargetStructurePriority(u, s);
		}

		/*
		 * Delivery of transport, either to structure or to a tile.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return One if delivered, zero otherwise..
		 */
		internal static ushort Script_Unit_TransportDeliver(ScriptEngine script)
		{
			Unit u;
			Unit u2;

			u = g_scriptCurrentUnit;

			if (u.o.linkedID == 0xFF) return 0;
			if (Tools.Tools_Index_GetType(u.targetMove) == IndexType.IT_UNIT) return 0;

			if (Tools.Tools_Index_GetType(u.targetMove) == IndexType.IT_STRUCTURE)
			{
				StructureInfo si;
				Structure s;

				s = Tools.Tools_Index_GetStructure(u.targetMove);
				si = g_table_structureInfo[s.o.type];

				if (s.o.type == (byte)StructureType.STRUCTURE_STARPORT)
				{
					ushort ret = 0;

					if (s.state == (short)StructureState.STRUCTURE_STATE_BUSY)
					{
						s.o.linkedID = u.o.linkedID;
						u.o.linkedID = 0xFF;
						u.o.flags.inTransport = false;
						u.amount = 0;

						Unit_UpdateMap(2, u);

						Sound.Voice_PlayAtTile(24, u.o.position);

						CStructure.Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_READY);

						ret = 1;
					}

					CObject.Object_Script_Variable4_Clear(u.o);
					u.targetMove = 0;

					return ret;
				}

				if ((s.state == (short)StructureState.STRUCTURE_STATE_IDLE || (si.o.flags.busyStateIsIncoming && s.state == (short)StructureState.STRUCTURE_STATE_BUSY)) && s.o.linkedID == 0xFF)
				{
					Sound.Voice_PlayAtTile(24, u.o.position);

					Unit_EnterStructure(PoolUnit.Unit_Get_ByIndex(u.o.linkedID), s);

					CObject.Object_Script_Variable4_Clear(u.o);
					u.targetMove = 0;

					u.o.linkedID = 0xFF;
					u.o.flags.inTransport = false;
					u.amount = 0;

					Unit_UpdateMap(2, u);

					return 1;
				}

				CObject.Object_Script_Variable4_Clear(u.o);
				u.targetMove = 0;

				return 0;
			}

			if (!Map.Map_IsValidPosition(CTile.Tile_PackTile(CTile.Tile_Center(u.o.position)))) return 0;

			u2 = PoolUnit.Unit_Get_ByIndex(u.o.linkedID);

			if (!Unit_SetPosition(u2, CTile.Tile_Center(u.o.position))) return 0;

			if (u2.o.houseID == (byte)CHouse.g_playerHouseID)
			{
				Sound.Voice_PlayAtTile(24, u.o.position);
			}

			Unit_SetOrientation(u2, u.orientation[0].current, true, 0);
			Unit_SetOrientation(u2, u.orientation[0].current, true, 1);
			Unit_SetSpeed(u2, 0);

			u.o.linkedID = u2.o.linkedID;
			u2.o.linkedID = 0xFF;

			if (u.o.linkedID != 0xFF) return 1;

			u.o.flags.inTransport = false;

			CObject.Object_Script_Variable4_Clear(u.o);
			u.targetMove = 0;

			return 1;
		}

		/*
		 * Pickup a unit (either from structure or on the map). The unit that does the
		 *  picking up returns the unit to his last position.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_Pickup(ScriptEngine script)
		{
			Unit u;

			u = g_scriptCurrentUnit;

			if (u.o.linkedID != 0xFF) return 0;

			switch (Tools.Tools_Index_GetType(u.targetMove))
			{
				case IndexType.IT_STRUCTURE:
					{
						Structure s;
						Unit u2;

						s = Tools.Tools_Index_GetStructure(u.targetMove);

						/* There was nothing to pickup here */
						if (s.state != (short)StructureState.STRUCTURE_STATE_READY)
						{
							CObject.Object_Script_Variable4_Clear(u.o);
							u.targetMove = 0;
							return 0;
						}

						u.o.flags.inTransport = true;

						CObject.Object_Script_Variable4_Clear(u.o);
						u.targetMove = 0;

						u2 = PoolUnit.Unit_Get_ByIndex(s.o.linkedID);

						/* Pickup the unit */
						u.o.linkedID = (byte)(u2.o.index & 0xFF);
						s.o.linkedID = u2.o.linkedID;
						u2.o.linkedID = 0xFF;

						if (s.o.linkedID == 0xFF) CStructure.Structure_SetState(s, (short)StructureState.STRUCTURE_STATE_IDLE);

						/* Check if the unit has a return-to position or try to find spice in case of a harvester */
						if (u2.targetLast.x != 0 || u2.targetLast.y != 0)
						{
							u.targetMove = Tools.Tools_Index_Encode(CTile.Tile_PackTile(u2.targetLast), IndexType.IT_TILE);
						}
						else if (u2.o.type == (byte)UnitType.UNIT_HARVESTER && Unit_GetHouseID(u2) != (byte)CHouse.g_playerHouseID)
						{
							u.targetMove = Tools.Tools_Index_Encode(Map.Map_SearchSpice(CTile.Tile_PackTile(u.o.position), 20), IndexType.IT_TILE);
						}

						Unit_UpdateMap(2, u);

						return 1;
					}

				case IndexType.IT_UNIT:
					{
						Unit u2;
						Structure s = null;
						var find = new PoolFindStruct();
						short minDistance = 0;

						u2 = Tools.Tools_Index_GetUnit(u.targetMove);

						if (!u2.o.flags.allocated) return 0;

						find.houseID = Unit_GetHouseID(u);
						find.index = 0xFFFF;
						find.type = 0xFFFF;

						/* Find closest refinery / repair station */
						while (true)
						{
							Structure s2;
							short distance;

							s2 = PoolStructure.Structure_Find(find);
							if (s2 == null) break;

							distance = (short)CTile.Tile_GetDistanceRoundedUp(s2.o.position, u.o.position);

							if (u2.o.type == (byte)UnitType.UNIT_HARVESTER)
							{
								if (s2.o.type != (byte)StructureType.STRUCTURE_REFINERY || s2.state != (short)StructureState.STRUCTURE_STATE_IDLE || s2.o.script.variables[4] != 0) continue;
								if (minDistance != 0 && distance >= minDistance) break;
								minDistance = distance;
								s = s2;
								break;
							}

							if (s2.o.type != (byte)StructureType.STRUCTURE_REPAIR || s2.state != (short)StructureState.STRUCTURE_STATE_IDLE || s2.o.script.variables[4] != 0) continue;

							if (minDistance != 0 && distance >= minDistance) continue;
							minDistance = distance;
							s = s2;
						}

						if (s == null) return 0;

						/* Deselect the unit as it is about to be picked up */
						if (u2 == g_unitSelected) Unit_Select(null);

						/* Pickup the unit */
						u.o.linkedID = (byte)(u2.o.index & 0xFF);
						u.o.flags.inTransport = true;

						Unit_UpdateMap(0, u2);

						Unit_Hide(u2);

						/* Set where we are going to */
						CObject.Object_Script_Variable4_Link(Tools.Tools_Index_Encode(u.o.index, IndexType.IT_UNIT), Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE));
						u.targetMove = u.o.script.variables[4];

						Unit_UpdateMap(2, u);

						if (u2.o.type != (byte)UnitType.UNIT_HARVESTER) return 0;

						/* Check if we want to return to this spice field later */
						if (Map.Map_SearchSpice(CTile.Tile_PackTile(u2.o.position), 2) == 0)
						{
							u2.targetPreLast.x = 0;
							u2.targetPreLast.y = 0;
							u2.targetLast.x = 0;
							u2.targetLast.y = 0;
						}

						return 0;
					}

				default: return 0;
			}
		}

		/*
		 * Stop the Unit.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_Stop(ScriptEngine script)
		{
			Unit u;

			u = g_scriptCurrentUnit;

			Unit_SetSpeed(u, 0);

			Unit_UpdateMap(2, u);

			return 0;
		}

		/*
		 * Set the speed of a Unit.
		 *
		 * Stack: 1 - The new speed of the Unit.
		 *
		 * @param script The script engine to operate on.
		 * @return The new speed; it might differ from the value given.
		 */
		internal static ushort Script_Unit_SetSpeed(ScriptEngine script)
		{
			Unit u;
			ushort speed;

			u = g_scriptCurrentUnit;
			speed = CMath.clamp(STACK_PEEK(script, 1), 0, 255);

			if (!u.o.flags.byScenario) speed = (ushort)(speed * 192 / 256);

			Unit_SetSpeed(u, speed);

			return u.speed;
		}

		/*
		 * Change the sprite (offset) of the unit.
		 *
		 * Stack: 1 - The new sprite offset.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_SetSprite(ScriptEngine script)
		{
			Unit u;

			u = g_scriptCurrentUnit;

			u.spriteOffset = (sbyte)-(STACK_PEEK(script, 1) & 0xFF);

			Unit_UpdateMap(2, u);

			return 0;
		}

		/*
		 * Move the Unit to the target, and keep repeating this function till we
		 *  arrived there. When closing in on the target it will slow down the Unit.
		 * It is wise to only use this function on Carry-Alls.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return 1 if arrived, 0 if still busy.
		 */
		internal static ushort Script_Unit_MoveToTarget(ScriptEngine script)
		{
			Unit u;
			ushort delay;
			tile32 tile;
			ushort distance;
			sbyte orientation;
			short diff;

			u = g_scriptCurrentUnit;

			if (u.targetMove == 0) return 0;

			tile = Tools.Tools_Index_GetTile(u.targetMove);

			distance = CTile.Tile_GetDistance(u.o.position, tile);

			if ((short)distance < 128)
			{
				Unit_SetSpeed(u, 0);

				u.o.position.x += (ushort)CMath.clamp((short)(tile.x - u.o.position.x), -16, 16);
				u.o.position.y += (ushort)CMath.clamp((short)(tile.y - u.o.position.y), -16, 16);

				Unit_UpdateMap(2, u);

				if ((short)distance < 32) return 1;

				script.delay = 2;

				script.script--;
				return 0;
			}

			orientation = CTile.Tile_GetDirection(u.o.position, tile);

			Unit_SetOrientation(u, orientation, false, 0);

			diff = (short)Abs(orientation - u.orientation[0].current);
			if (diff > 128) diff = (short)(256 - diff);

			Unit_SetSpeed(u, (ushort)((Max(Min(distance / 8, 255), 25) * (255 - diff) + 128) / 256));

			delay = (ushort)Max((short)distance / 1024, 1);

			Unit_UpdateMap(2, u);

			if (delay != 0)
			{
				script.delay = delay;

				script.script--;
			}

			return 0;
		}

		/*
		 * Kill a unit. When it was a saboteur, expect a big explosion.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_Die(ScriptEngine script)
		{
			UnitInfo ui;
			Unit u;

			u = g_scriptCurrentUnit;
			ui = g_table_unitInfo[u.o.type];

			Unit_Remove(u);

			if (ui.movementType != (ushort)MovementType.MOVEMENT_WINGER)
			{
				ushort credits;

				credits = (ushort)Max(ui.o.buildCredits / 100, 1);

				if (u.o.houseID == (byte)CHouse.g_playerHouseID)
				{
					CScenario.g_scenario.killedAllied++;
					CScenario.g_scenario.score -= credits;
				}
				else
				{
					CScenario.g_scenario.killedEnemy++;
					CScenario.g_scenario.score += credits;
				}
			}

			Unit_HouseUnitCount_Remove(u);

			if (u.o.type != (byte)UnitType.UNIT_SABOTEUR) return 0;

			Map.Map_MakeExplosion((ushort)ExplosionType.EXPLOSION_SABOTEUR_DEATH, u.o.position, 300, 0);
			return 0;
		}

		/*
		 * Make an explosion at the coordinates of the unit.
		 *  It does damage to the surrounding units based on the unit.
		 *
		 * Stack: 1 - Explosion type
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_ExplosionSingle(ScriptEngine script)
		{
			Unit u;

			u = g_scriptCurrentUnit;

			Map.Map_MakeExplosion(STACK_PEEK(script, 1), u.o.position, g_table_unitInfo[u.o.type].o.hitpoints, Tools.Tools_Index_Encode(u.o.index, IndexType.IT_UNIT));
			return 0;
		}

		/*
		 * Make 8 explosions: 1 at the unit, and 7 around him.
		 * It does damage to the surrounding units with predefined damage, but
		 *  anonymous.
		 *
		 * Stack: 1 - The radius of the 7 explosions.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_ExplosionMultiple(ScriptEngine script)
		{
			Unit u;
			byte i;

			u = g_scriptCurrentUnit;

			Map.Map_MakeExplosion((ushort)ExplosionType.EXPLOSION_DEATH_HAND, u.o.position, Tools.Tools_RandomLCG_Range(25, 50), 0);

			for (i = 0; i < 7; i++)
			{
				Map.Map_MakeExplosion((ushort)ExplosionType.EXPLOSION_DEATH_HAND, CTile.Tile_MoveByRandom(u.o.position, STACK_PEEK(script, 1), false), Tools.Tools_RandomLCG_Range(75, 150), 0);
			}

			return 0;
		}

		/*
		 * Makes the current unit fire a bullet (or eat its target).
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 1 if the current unit fired/eat, 0 otherwise.
		 */
		internal static ushort Script_Unit_Fire(ScriptEngine script)
		{
			UnitInfo ui;
			Unit u;
			ushort target;
			UnitType typeID;
			ushort distance;
			bool fireTwice;
			ushort damage;

			u = g_scriptCurrentUnit;

			target = u.targetAttack;
			if (target == 0 || !Tools.Tools_Index_IsValid(target)) return 0;

			if (u.o.type != (byte)UnitType.UNIT_SANDWORM && target == Tools.Tools_Index_Encode(CTile.Tile_PackTile(u.o.position), IndexType.IT_TILE)) u.targetAttack = 0;

			if (u.targetAttack != target)
			{
				Unit_SetTarget(u, target);
				return 0;
			}

			ui = g_table_unitInfo[u.o.type];

			if (u.o.type != (byte)UnitType.UNIT_SANDWORM && u.orientation[ui.o.flags.hasTurret ? 1 : 0].speed != 0) return 0;

			if (Tools.Tools_Index_GetType(target) == IndexType.IT_TILE && CObject.Object_GetByPackedTile(Tools.Tools_Index_GetPackedTile(target)) != null) Unit_SetTarget(u, target);

			if (u.fireDelay != 0) return 0;

			distance = CObject.Object_GetDistanceToEncoded(u.o, target);

			if ((short)(ui.fireDistance << 8) < (short)distance) return 0;

			if (u.o.type != (byte)UnitType.UNIT_SANDWORM && (Tools.Tools_Index_GetType(target) != IndexType.IT_UNIT || g_table_unitInfo[Tools.Tools_Index_GetUnit(target).o.type].movementType != (ushort)MovementType.MOVEMENT_WINGER))
			{
				short diff = 0;
				sbyte orientation;

				orientation = CTile.Tile_GetDirection(u.o.position, Tools.Tools_Index_GetTile(target));

				diff = (short)Abs(u.orientation[ui.o.flags.hasTurret ? 1 : 0].current - orientation);
				if (ui.movementType == (ushort)MovementType.MOVEMENT_WINGER) diff /= 8;

				if (diff >= 8) return 0;
			}

			damage = ui.damage;
			typeID = (UnitType)ui.bulletType;

			fireTwice = ui.flags.firesTwice && u.o.hitpoints > ui.o.hitpoints / 2;

			if ((u.o.type == (byte)UnitType.UNIT_TROOPERS || u.o.type == (byte)UnitType.UNIT_TROOPER) && (short)distance > 512) typeID = UnitType.UNIT_MISSILE_TROOPER;

			switch (typeID)
			{
				case UnitType.UNIT_SANDWORM:
					{
						Unit u2;

						Unit_UpdateMap(0, u);

						u2 = Tools.Tools_Index_GetUnit(target);

						if (u2 != null)
						{
							u2.o.script.variables[1] = 0xFFFF;
							Unit_RemovePlayer(u2);
							Unit_HouseUnitCount_Remove(u2);
							Unit_Remove(u2);
						}

						Map.Map_MakeExplosion(ui.explosionType, u.o.position, 0, 0);

						Sound.Voice_PlayAtTile(63, u.o.position);

						Unit_UpdateMap(1, u);

						u.amount--;

						script.delay = 12;

						if ((sbyte)u.amount < 1) Unit_SetAction(u, ActionType.ACTION_DIE);
					}
					break;

				case UnitType.UNIT_MISSILE_TROOPER:
				case UnitType.UNIT_MISSILE_ROCKET:
				case UnitType.UNIT_MISSILE_TURRET:
				case UnitType.UNIT_MISSILE_DEVIATOR:
				case UnitType.UNIT_BULLET:
				case UnitType.UNIT_SONIC_BLAST:
					{
						if (typeID == UnitType.UNIT_MISSILE_TROOPER)
							damage -= (ushort)(damage / 4);

						Unit bullet;

						bullet = Unit_CreateBullet(u.o.position, typeID, Unit_GetHouseID(u), damage, target);

						if (bullet == null) return 0;

						bullet.originEncoded = Tools.Tools_Index_Encode(u.o.index, IndexType.IT_UNIT);

						Sound.Voice_PlayAtTile((short)ui.bulletSound, u.o.position);

						Unit_Deviation_Decrease(u, 20);
					}
					break;

				default: break;
			}

			u.fireDelay = Tools.Tools_AdjustToGameSpeed((ushort)(ui.fireDelay * 2), 1, 0xFFFF, true);

			if (fireTwice)
			{
				u.o.flags.fireTwiceFlip = !u.o.flags.fireTwiceFlip;
				if (u.o.flags.fireTwiceFlip) u.fireDelay = Tools.Tools_AdjustToGameSpeed(5, 1, 10, true);
			}
			else
			{
				u.o.flags.fireTwiceFlip = false;
			}

			u.fireDelay += (ushort)(Tools.Tools_Random_256() & 1);

			Unit_UpdateMap(2, u);

			return 1;
		}

		/*
		 * Set the orientation of a unit.
		 *
		 * Stack: 1 - New orientation for unit.
		 *
		 * @param script The script engine to operate on.
		 * @return The current orientation of the unit (it will move to the requested over time).
		 */
		internal static ushort Script_Unit_SetOrientation(ScriptEngine script)
		{
			Unit u;

			u = g_scriptCurrentUnit;

			Unit_SetOrientation(u, (sbyte)STACK_PEEK(script, 1), false, 0);

			return (ushort)u.orientation[0].current;
		}

		/*
		 * Rotate the unit to aim at the enemy.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return 0 if the enemy is no longer there or if we are looking at him, 1 otherwise.
		 */
		internal static ushort Script_Unit_Rotate(ScriptEngine script)
		{
			UnitInfo ui;
			Unit u;
			ushort index;
			sbyte current;
			tile32 tile;
			sbyte orientation;

			u = g_scriptCurrentUnit;
			ui = g_table_unitInfo[u.o.type];

			if (ui.movementType != (ushort)MovementType.MOVEMENT_WINGER && (u.currentDestination.x != 0 || u.currentDestination.y != 0)) return 1;

			index = (ushort)(ui.o.flags.hasTurret ? 1 : 0);

			/* Check if we are already rotating */
			if (u.orientation[index].speed != 0) return 1;
			current = u.orientation[index].current;

			if (!Tools.Tools_Index_IsValid(u.targetAttack)) return 0;

			/* Check where we should rotate to */
			tile = Tools.Tools_Index_GetTile(u.targetAttack);
			orientation = CTile.Tile_GetDirection(u.o.position, tile);

			/* If we aren't already looking at it, rotate */
			if (orientation == current) return 0;
			Unit_SetOrientation(u, orientation, false, index);

			return 1;
		}

		/*
		 * Get the direction to a tile or our current direction.
		 *
		 * Stack: 1 - An encoded tile to get the direction to.
		 *
		 * @param script The script engine to operate on.
		 * @return The direction to the encoded tile if valid, otherwise our current orientation.
		 */
		internal static ushort Script_Unit_GetOrientation(ScriptEngine script)
		{
			Unit u;
			ushort encoded;

			u = g_scriptCurrentUnit;
			encoded = STACK_PEEK(script, 1);

			if (Tools.Tools_Index_IsValid(encoded))
			{
				tile32 tile;

				tile = Tools.Tools_Index_GetTile(encoded);

				return (ushort)CTile.Tile_GetDirection(u.o.position, tile);
			}

			return (ushort)u.orientation[0].current;
		}

		/*
		 * Set the new destination of the unit.
		 *
		 * Stack: 1 - An encoded index where to move to.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_SetDestination(ScriptEngine script)
		{
			Unit u;
			ushort encoded;

			u = g_scriptCurrentUnit;
			encoded = STACK_PEEK(script, 1);

			if (encoded == 0 || !Tools.Tools_Index_IsValid(encoded))
			{
				u.targetMove = 0;
				return 0;
			}

			if (u.o.type == (byte)UnitType.UNIT_HARVESTER)
			{
				Structure s;

				s = Tools.Tools_Index_GetStructure(encoded);
				if (s == null)
				{
					u.targetMove = encoded;
					u.route[0] = 0xFF;
					return 0;
				}

				if (s.o.script.variables[4] != 0) return 0;
			}

			Unit_SetDestination(u, encoded);
			return 0;
		}

		/*
		 * Set a new target, and rotate towards him if needed.
		 *
		 * Stack: 1 - An encoded tile of the unit/tile to target.
		 *
		 * @param script The script engine to operate on.
		 * @return The new target.
		 */
		internal static ushort Script_Unit_SetTarget(ScriptEngine script)
		{
			Unit u;
			ushort target;
			tile32 tile;
			sbyte orientation;

			u = g_scriptCurrentUnit;

			target = STACK_PEEK(script, 1);

			if (target == 0 || !Tools.Tools_Index_IsValid(target))
			{
				u.targetAttack = 0;
				return 0;
			}

			tile = Tools.Tools_Index_GetTile(target);

			orientation = CTile.Tile_GetDirection(u.o.position, tile);

			u.targetAttack = target;
			if (!g_table_unitInfo[u.o.type].o.flags.hasTurret)
			{
				u.targetMove = target;
				Unit_SetOrientation(u, orientation, false, 0);
			}
			Unit_SetOrientation(u, orientation, false, 1);

			return u.targetAttack;
		}

		/*
		 * Sets the action for the current unit.
		 *
		 * Stack: 1 - The action.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_SetAction(ScriptEngine script)
		{
			Unit u;
			ActionType action;

			u = g_scriptCurrentUnit;

			action = (ActionType)STACK_PEEK(script, 1);

			if (u.o.houseID == (byte)CHouse.g_playerHouseID && action == ActionType.ACTION_HARVEST && u.nextActionID != (byte)ActionType.ACTION_INVALID) return 0;

			Unit_SetAction(u, action);

			return 0;
		}

		/*
		 * Sets the action for the current unit to default.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_SetActionDefault(ScriptEngine script)
		{
			Unit u;

			u = g_scriptCurrentUnit;

			Unit_SetAction(u, (ActionType)g_table_unitInfo[u.o.type].o.actionsPlayer[3]);

			return 0;
		}

		/*
		 * Set the current destination of a Unit, bypassing any pathfinding.
		 * It is wise to only use this function on Carry-Alls.
		 *
		 * Stack: 1 - An encoded tile, the destination.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_SetDestinationDirect(ScriptEngine script)
		{
			Unit u;
			ushort encoded;

			encoded = STACK_PEEK(script, 1);

			if (!Tools.Tools_Index_IsValid(encoded)) return 0;

			u = g_scriptCurrentUnit;

			if ((u.currentDestination.x == 0 && u.currentDestination.y == 0) || g_table_unitInfo[u.o.type].flags.isNormalUnit)
			{
				u.currentDestination = Tools.Tools_Index_GetTile(encoded);
			}

			Unit_SetOrientation(u, CTile.Tile_GetDirection(u.o.position, u.currentDestination), false, 0);

			return 0;
		}

		/*
		 * Get information about the unit, like hitpoints, current target, etc.
		 *
		 * Stack: 1 - Which information you would like.
		 *
		 * @param script The script engine to operate on.
		 * @return The information you requested.
		 */
		internal static ushort Script_Unit_GetInfo(ScriptEngine script)
		{
			UnitInfo ui;
			Unit u;

			u = g_scriptCurrentUnit;
			ui = g_table_unitInfo[u.o.type];

			switch (STACK_PEEK(script, 1))
			{
				case 0x00: return (ushort)(u.o.hitpoints * 256 / ui.o.hitpoints);
				case 0x01: return (ushort)(Tools.Tools_Index_IsValid(u.targetMove) ? u.targetMove : 0);
				case 0x02: return (ushort)(ui.fireDistance << 8);
				case 0x03: return u.o.index;
				case 0x04: return (ushort)u.orientation[0].current;
				case 0x05: return u.targetAttack;
				case 0x06:
					if (u.originEncoded == 0 || u.o.type == (byte)UnitType.UNIT_HARVESTER) Unit_FindClosestRefinery(u);
					return u.originEncoded;
				case 0x07: return u.o.type;
				case 0x08: return Tools.Tools_Index_Encode(u.o.index, IndexType.IT_UNIT);
				case 0x09: return u.movingSpeed;
				case 0x0A: return (ushort)Abs(u.orientation[0].target - u.orientation[0].current);
				case 0x0B: return (ushort)((u.currentDestination.x == 0 && u.currentDestination.y == 0) ? 0 : 1);
				case 0x0C: return (ushort)(u.fireDelay == 0 ? 1 : 0);
				case 0x0D: return (ushort)(ui.flags.explodeOnDeath ? 1 : 0);
				case 0x0E: return Unit_GetHouseID(u);
				case 0x0F: return (ushort)(u.o.flags.byScenario ? 1 : 0);
				case 0x10: return (ushort)u.orientation[ui.o.flags.hasTurret ? 1 : 0].current;
				case 0x11: return (ushort)Abs(u.orientation[ui.o.flags.hasTurret ? 1 : 0].target - u.orientation[ui.o.flags.hasTurret ? 1 : 0].current);
				case 0x12: return (ushort)((ui.movementType & 0x40) == 0 ? 0 : 1);
				case 0x13: return (ushort)((u.o.seenByHouses & (1 << (byte)CHouse.g_playerHouseID)) == 0 ? 0 : 1);
				default: return 0;
			}
		}

		/*
		 * Get the score to enter this tile from a direction.
		 *
		 * @param packed The packed tile.
		 * @param direction The direction we move on this tile.
		 * @return 256 if tile is not accessable, or a score for entering otherwise.
		 */
		static short Script_Unit_Pathfind_GetScore(ushort packed, byte orient8)
		{
			short res;
			Unit u;

			if (g_scriptCurrentUnit == null) return 0;

			u = g_scriptCurrentUnit;

			if (CSharpDune.g_dune2_enhanced)
			{
				res = Unit_GetTileEnterScore(u, packed, orient8);
			}
			else
			{
				res = Unit_GetTileEnterScore(u, packed, (ushort)(orient8 << 5));
			}

			if (res == -1) res = 256;

			return res;
		}

		static readonly sbyte[] directionOffset = { 0, 0, 1, 2, 3, -2, -1, 0 };
		/*
		 * Smoothen the route found by the pathfinder.
		 * @param data The found route to smoothen.
		 */
		static void Script_Unit_Pathfinder_Smoothen(Pathfinder_Data data)
		{
			ushort packed;
			/*byte[]*/
			var bufferFrom = new CArray<byte> { Arr = data.buffer };
			/*byte[]*/
			var bufferTo = new CArray<byte> { Arr = data.buffer };
			//int bufferFromPointer = 0;
			//int bufferToPointer = 0;

			data.buffer[data.routeSize] = 0xFF;
			packed = data.packed;

			if (data.routeSize > 1)
			{
				bufferTo.Ptr = 1; //bufferTo = data.buffer[1..];

				while (bufferTo.Arr[bufferTo.Ptr] != 0xFF)
				{
					sbyte direction;
					byte dir;

					bufferFrom.Ptr = bufferTo.Ptr - 1; //bufferFrom = bufferTo[(bufferToPointer - 1)..];

					while (bufferFrom.Arr[bufferFrom.Ptr] == 0xFE && !Common.AreArraysEqual(bufferFrom.Arr, bufferFrom.Ptr, data.buffer, 0, Min(bufferFrom.Arr.Length, data.buffer.Length))) bufferFrom.Ptr--;

					if (bufferFrom.Arr[bufferFrom.Ptr] == 0xFE)
					{
						bufferTo.Ptr++;
						continue;
					}

					direction = (sbyte)((bufferTo.Arr[bufferTo.Ptr] - bufferFrom.Arr[bufferFrom.Ptr]) & 0x7);
					direction = directionOffset[direction];

					/* The directions are opposite of each other, so they can both be removed */
					if (direction == 3)
					{
						bufferFrom.Arr[bufferFrom.Ptr] = 0xFE;
						bufferTo.Arr[bufferTo.Ptr] = 0xFE;

						bufferTo.Ptr++;
						continue;
					}

					/* The directions are close to each other, so follow */
					if (direction == 0)
					{
						packed += (ushort)s_mapDirection[bufferFrom.Arr[bufferFrom.Ptr]];
						bufferTo.Ptr++;
						continue;
					}

					if ((bufferFrom.Arr[bufferFrom.Ptr] & 0x1) != 0)
					{
						dir = (byte)((bufferFrom.Arr[bufferFrom.Ptr] + (direction < 0 ? -1 : 1)) & 0x7);

						/* If we go 45 degrees with 90 degree difference, we can also go straight */
						if (Abs(direction) == 1)
						{
							if (Script_Unit_Pathfind_GetScore((ushort)(packed + s_mapDirection[dir]), dir) <= 255)
							{
								bufferTo.Arr[bufferTo.Ptr] = dir;
								bufferFrom.Arr[bufferFrom.Ptr] = dir;
							}
							packed += (ushort)s_mapDirection[bufferFrom.Arr[bufferFrom.Ptr]];
							bufferTo.Ptr++;
							continue;
						}
					}
					else
					{
						dir = (byte)((bufferFrom.Arr[bufferFrom.Ptr] + direction) & 0x7);
					}

					/* In these cases we can do with 1 direction change less, so remove one */
					bufferTo.Arr[bufferTo.Ptr] = dir;
					bufferFrom.Arr[bufferFrom.Ptr] = 0xFE;

					/* Walk back one tile */
					while (bufferFrom.Arr[bufferFrom.Ptr] == 0xFE && bufferFrom.Ptr != 0/*data.buffer != bufferFrom.Arr*/) bufferFrom.Ptr--;
					if (bufferFrom.Arr[bufferFrom.Ptr] != 0xFE)
					{
						packed += (ushort)s_mapDirection[(bufferFrom.Arr[bufferFrom.Ptr] + 4) & 0x7];
					}
					else
					{
						packed = data.packed;
					}
				}
			}

			bufferFrom.Arr = data.buffer;
			bufferFrom.Ptr = 0;
			bufferTo.Arr = data.buffer;
			bufferTo.Ptr = 0;
			packed = data.packed;
			data.score = 0;
			data.routeSize = 0;

			/* Build the new improved route, without gaps */
			for (; bufferTo.Arr[bufferTo.Ptr] != 0xFF; bufferTo.Ptr++)
			{
				if (bufferTo.Arr[bufferTo.Ptr] == 0xFE) continue;

				packed += (ushort)s_mapDirection[bufferTo.Arr[bufferTo.Ptr]];
				data.score += Script_Unit_Pathfind_GetScore(packed, bufferTo.Arr[bufferTo.Ptr]);
				data.routeSize++;
				bufferFrom.Arr[bufferFrom.Ptr++] = bufferTo.Arr[bufferTo.Ptr];
			}

			data.routeSize++;
			bufferFrom.Arr[bufferFrom.Ptr] = 0xFF;
		}

		/*
		 * Try to connect two tiles (packedDst and data->packed) via a simplistic algorithm.
		 * @param packedDst The tile to try to get to.
		 * @param data Information about the found route, and the start point.
		 * @param searchDirection The search direction (1 for clockwise, -1 for counterclockwise).
		 * @param directionStart The direction to start looking at.
		 * @return True if a route was found.
		 */
		static bool Script_Unit_Pathfinder_Connect(ushort packedDst, Pathfinder_Data data, sbyte searchDirection, byte directionStart)
		{
			ushort packedNext;
			ushort packedCur;
			byte[] buffer;
			ushort bufferSize;
			var bufferPointer = 0;

			packedCur = data.packed;
			buffer = data.buffer;
			bufferSize = 0;

			while (bufferSize < 100)
			{
				var direction = directionStart;

				while (true)
				{
					/* Look around us, first in the start direction, for a valid tile */
					direction = (byte)((direction + searchDirection) & 0x7);

					/* In case we are directly looking at our destination tile, we are pretty much done */
					if ((direction & 0x1) != 0 && (packedCur + s_mapDirection[(direction + searchDirection) & 0x7]) == packedDst)
					{
						direction = (byte)((direction + searchDirection) & 0x7);
						packedNext = (ushort)(packedCur + s_mapDirection[direction]);
						break;
					}
					else
					{
						/* If we are back to our start direction, we didn't find a route */
						if (direction == directionStart) return false;

						/* See if the tile next to us is a valid position */
						packedNext = (ushort)(packedCur + s_mapDirection[direction]);
						if (Script_Unit_Pathfind_GetScore(packedNext, direction) <= 255) break;
					}
				}

				buffer[bufferPointer++] = direction;
				bufferSize++;

				/* If we found the destination, smooth the route and we are done */
				if (packedNext == packedDst)
				{
					buffer[bufferPointer] = 0xFF;
					data.routeSize = bufferSize;
					Script_Unit_Pathfinder_Smoothen(data);
					data.routeSize--;
					return true;
				}

				/* If we return to our start tile, we didn't find a route */
				if (data.packed == packedNext) return false;

				/* Now look at the next tile, starting 3 directions back */
				directionStart = (byte)((direction - searchDirection * 3) & 0x7);
				packedCur = packedNext;
			}

			/* We ran out of search space and didn't find a route */
			return false;
		}

		/*
		 * Try to find a path between two points.
		 *
		 * @param packedSrc The start point.
		 * @param packedDst The end point.
		 * @param buffer The buffer to store the route in.
		 * @param bufferSize The size of the buffer.
		 * @return A struct with information about the found route.
		 */
		static Pathfinder_Data Script_Unit_Pathfinder(ushort packedSrc, ushort packedDst, /*object*/byte[] buffer, short bufferSize)
		{
			ushort packedCur;
			var res = new Pathfinder_Data
			{
				packed = packedSrc,
				score = 0,
				routeSize = 0,
				buffer = buffer
			};

			res.buffer[0] = 0xFF;

			bufferSize--;

			packedCur = packedSrc;
			while (res.routeSize < bufferSize)
			{
				byte direction;
				ushort packedNext;
				short score;

				if (packedCur == packedDst) break;

				/* Try going directly to the destination tile */
				direction = (byte)(CTile.Tile_GetDirectionPacked(packedCur, packedDst) / 32);
				packedNext = (ushort)(packedCur + s_mapDirection[direction]);

				/* Check for valid movement towards the tile */
				score = Script_Unit_Pathfind_GetScore(packedNext, direction);
				if (score <= 255)
				{
					res.buffer[res.routeSize++] = direction;
					res.score += score;
				}
				else
				{
					byte dir;
					var foundCounterclockwise = false;
					var foundClockwise = false;
					short routeSize;
					Pathfinder_Data[] routes = { new(), new() }; //new Pathfinder_Data[2];
					byte[][] routesBuffer = { new byte[102], new byte[102] }; //new byte[2][]; //[2][102]
					Pathfinder_Data bestRoute;

					while (true)
					{
						if (packedNext == packedDst) break;

						/* Find the first valid tile on the (direct) route. */
						dir = (byte)(CTile.Tile_GetDirectionPacked(packedNext, packedDst) / 32);
						packedNext += (ushort)s_mapDirection[dir];
						if (Script_Unit_Pathfind_GetScore(packedNext, dir) > 255) continue;

						/* Try to find a connection between our last valid tile and the new valid tile */
						routes[1].packed = packedCur;
						routes[1].score = 0;
						routes[1].routeSize = 0;
						routes[1].buffer = routesBuffer[0];
						foundCounterclockwise = Script_Unit_Pathfinder_Connect(packedNext, routes[1], -1, direction);

						routes[0].packed = packedCur;
						routes[0].score = 0;
						routes[0].routeSize = 0;
						routes[0].buffer = routesBuffer[1];
						foundClockwise = Script_Unit_Pathfinder_Connect(packedNext, routes[0], 1, direction);

						if (foundCounterclockwise || foundClockwise) break;

						do
						{
							if (packedNext == packedDst) break;

							dir = (byte)(CTile.Tile_GetDirectionPacked(packedNext, packedDst) / 32);
							packedNext += (ushort)s_mapDirection[dir];
						} while (Script_Unit_Pathfind_GetScore(packedNext, dir) <= 255);
					}

					if (foundCounterclockwise || foundClockwise)
					{
						/* Find the best (partial) route */
						if (!foundClockwise)
						{
							bestRoute = routes[1];
						}
						else if (!foundCounterclockwise)
						{
							bestRoute = routes[0];
						}
						else
						{
							bestRoute = routes[routes[1].score < routes[0].score ? 1 : 0];
						}

						/* Calculate how much more we can copy into our own buffer */
						routeSize = (short)Min(bufferSize - res.routeSize, bestRoute.routeSize);
						if (routeSize <= 0) break;

						/* Copy the rest into our own buffer */
						Buffer.BlockCopy(bestRoute.buffer, 0, res.buffer, res.routeSize, routeSize); //memcpy(&res.buffer[res.routeSize], bestRoute->buffer, routeSize);
						res.routeSize += (ushort)routeSize;
						res.score += bestRoute.score;
					}
					else
					{
						/* Means we didn't find a route. packedNext is now equal to packedDst */
						break;
					}
				}

				packedCur = packedNext;
			}

			if (res.routeSize < bufferSize) res.buffer[res.routeSize++] = 0xFF;

			Script_Unit_Pathfinder_Smoothen(res);

			return res;
		}

		/*
		 * Calculate the route to a tile.
		 *
		 * Stack: 1 - An encoded tile to calculate the route to.
		 *
		 * @param script The script engine to operate on.
		 * @return 0 if we arrived on location, 1 otherwise.
		 */
		internal static ushort Script_Unit_CalculateRoute(ScriptEngine script)
		{
			Unit u;
			ushort encoded;
			ushort packedSrc;
			ushort packedDst;

			u = g_scriptCurrentUnit;
			encoded = STACK_PEEK(script, 1);

			if (u.currentDestination.x != 0 || u.currentDestination.y != 0 || !Tools.Tools_Index_IsValid(encoded)) return 1;

			packedSrc = CTile.Tile_PackTile(u.o.position);
			packedDst = Tools.Tools_Index_GetPackedTile(encoded);

			if (packedDst == packedSrc)
			{
				u.route[0] = 0xFF;
				u.targetMove = 0;
				return 0;
			}

			if (u.route[0] == 0xFF)
			{
				Pathfinder_Data res;
				var buffer = new byte[42];

				res = Script_Unit_Pathfinder(packedSrc, packedDst, buffer, 40);

				Buffer.BlockCopy(res.buffer, 0, u.route, 0, Min(res.routeSize, (ushort)14)); //memcpy(u->route, res.buffer, min(res.routeSize, 14));

				if (u.route[0] == 0xFF)
				{
					u.targetMove = 0;
					if (u.o.type == (byte)UnitType.UNIT_SANDWORM)
					{
						script.delay = 720;
					}
				}
			}
			else
			{
				ushort distance;

				distance = CTile.Tile_GetDistancePacked(packedDst, packedSrc);
				if (distance < 14) u.route[distance] = 0xFF;
			}

			if (u.route[0] == 0xFF) return 1;

			if (u.orientation[0].current != (sbyte)(u.route[0] * 32))
			{
				Unit_SetOrientation(u, (sbyte)(u.route[0] * 32), false, 0);
				return 1;
			}

			if (!Unit_StartMovement(u))
			{
				u.route[0] = 0xFF;
				return 0;
			}

			Buffer.BlockCopy(u.route, 1, u.route, 0, 13); //memmove(&u->route[0], &u->route[1], 13);
			u.route[13] = 0xFF;
			return 1;
		}

		/*
		 * Move the unit to the first available structure it can find of the required
		 *  type.
		 *
		 * Stack: 1 - Type of structure.
		 *
		 * @param script The script engine to operate on.
		 * @return An encoded structure index.
		 */
		internal static ushort Script_Unit_MoveToStructure(ScriptEngine script)
		{
			Unit u;
			var find = new PoolFindStruct();

			u = g_scriptCurrentUnit;

			if (u.o.linkedID != 0xFF)
			{
				Structure s;

				s = Tools.Tools_Index_GetStructure(PoolUnit.Unit_Get_ByIndex(u.o.linkedID).originEncoded);

				if (s != null && s.state == (short)StructureState.STRUCTURE_STATE_IDLE && s.o.script.variables[4] == 0)
				{
					ushort encoded;

					encoded = Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);

					CObject.Object_Script_Variable4_Link(Tools.Tools_Index_Encode(u.o.index, IndexType.IT_UNIT), encoded);

					u.targetMove = u.o.script.variables[4];

					return encoded;
				}
			}

			find.houseID = Unit_GetHouseID(u);
			find.index = 0xFFFF;
			find.type = STACK_PEEK(script, 1);

			while (true)
			{
				Structure s;
				ushort encoded;

				s = PoolStructure.Structure_Find(find);
				if (s == null) break;

				if (s.state != (short)StructureState.STRUCTURE_STATE_IDLE) continue;
				if (s.o.script.variables[4] != 0) continue;

				encoded = Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);

				CObject.Object_Script_Variable4_Link(Tools.Tools_Index_Encode(u.o.index, IndexType.IT_UNIT), encoded);

				u.targetMove = encoded;

				return encoded;
			}

			return 0;
		}

		/*
		 * Gets the amount of the unit linked to current unit, or current unit if not linked.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The amount.
		 */
		internal static ushort Script_Unit_GetAmount(ScriptEngine script)
		{
			Unit u;

			u = g_scriptCurrentUnit;

			if (u.o.linkedID == 0xFF) return u.amount;

			return PoolUnit.Unit_Get_ByIndex(u.o.linkedID).amount;
		}

		/*
		 * Checks if the current unit is in transport.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return True if the current unit is in transport.
		 */
		internal static ushort Script_Unit_IsInTransport(ScriptEngine script)
		{
			Unit u;

			u = g_scriptCurrentUnit;

			return (ushort)(u.o.flags.inTransport ? 1 : 0);
		}

		/*
		 * Start the animation on the current tile.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 1. Always.
		 */
		internal static ushort Script_Unit_StartAnimation(ScriptEngine script)
		{
			Unit u;
			ushort animationUnitID;
			ushort position;

			u = g_scriptCurrentUnit;

			position = CTile.Tile_PackTile(CTile.Tile_Center(u.o.position));
			CAnimation.Animation_Stop_ByTile(position);

			animationUnitID = (ushort)(g_table_landscapeInfo[Map.Map_GetLandscapeType(CTile.Tile_PackTile(u.o.position))].isSand ? 0 : 1);
			if (u.o.script.variables[1] == 1) animationUnitID += 2;

			Map.g_map[position].houseID = Unit_GetHouseID(u);

			Debug.Assert(animationUnitID < 4);
			if (g_table_unitInfo[u.o.type].displayMode == (ushort)DisplayMode.INFANTRY_3_FRAMES)
			{
				CAnimation.Animation_Start(g_table_animation_unitScript1[animationUnitID], u.o.position, 0, Unit_GetHouseID(u), 4);
			}
			else
			{
				CAnimation.Animation_Start(g_table_animation_unitScript2[animationUnitID], u.o.position, 0, Unit_GetHouseID(u), 4);
			}

			return 1;
		}

		/*
		 * Call a UnitType and make it go to the current unit. In general, type should
		 *  be a Carry-All for this to make any sense.
		 *
		 * Stack: 1 - An unit type.
		 *
		 * @param script The script engine to operate on.
		 * @return An encoded unit index.
		 */
		internal static ushort Script_Unit_CallUnitByType(ScriptEngine script)
		{
			Unit u;
			Unit u2;
			ushort encoded;
			ushort encoded2;

			u = g_scriptCurrentUnit;

			if (u.o.script.variables[4] != 0) return u.o.script.variables[4];
			if (!g_table_unitInfo[u.o.type].o.flags.canBePickedUp || u.deviated != 0) return 0;

			encoded = Tools.Tools_Index_Encode(u.o.index, IndexType.IT_UNIT);

			u2 = Unit_CallUnitByType((UnitType)STACK_PEEK(script, 1), Unit_GetHouseID(u), encoded, false);
			if (u2 == null) return 0;

			encoded2 = Tools.Tools_Index_Encode(u2.o.index, IndexType.IT_UNIT);

			CObject.Object_Script_Variable4_Link(encoded, encoded2);
			u2.targetMove = encoded;

			return encoded2;
		}

		/*
		 * Unknown function 2552.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_Unknown2552(ScriptEngine script)
		{
			Unit u;
			Unit u2;

			u = g_scriptCurrentUnit;
			if (u.o.script.variables[4] == 0) return 0;

			u2 = Tools.Tools_Index_GetUnit(u.o.script.variables[4]);
			if (u2 == null || u2.o.type != (byte)UnitType.UNIT_CARRYALL) return 0;

			CObject.Object_Script_Variable4_Clear(u.o);
			u2.targetMove = 0;

			return 0;
		}

		/*
		 * Finds a structure.
		 *
		 * Stack: 1 - A structure type.
		 *
		 * @param script The script engine to operate on.
		 * @return An encoded structure index, or 0 if none found.
		 */
		internal static ushort Script_Unit_FindStructure(ScriptEngine script)
		{
			Unit u;
			var find = new PoolFindStruct();

			u = g_scriptCurrentUnit;

			find.houseID = Unit_GetHouseID(u);
			find.index = 0xFFFF;
			find.type = STACK_PEEK(script, 1);

			while (true)
			{
				Structure s;

				s = PoolStructure.Structure_Find(find);
				if (s == null) break;
				if (s.state != (short)StructureState.STRUCTURE_STATE_IDLE) continue;
				if (s.o.linkedID != 0xFF) continue;
				if (s.o.script.variables[4] != 0) continue;

				return Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE);
			}

			return 0;
		}

		/*
		 * Displays the "XXX XXX destroyed." message for the current unit.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_DisplayDestroyedText(ScriptEngine script)
		{
			UnitInfo ui;
			Unit u;

			u = g_scriptCurrentUnit;
			ui = g_table_unitInfo[u.o.type];

			if (Config.g_config.language == (byte)Language.FRENCH)
			{
				Gui.Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_S_S_DESTROYED), 0, CString.String_Get_ByIndex(ui.o.stringID_abbrev), g_table_houseInfo[Unit_GetHouseID(u)].name);
			}
			else
			{
				Gui.Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_S_S_DESTROYED), 0, g_table_houseInfo[Unit_GetHouseID(u)].name, CString.String_Get_ByIndex(ui.o.stringID_abbrev));
			}

			return 0;
		}

		/*
		 * Removes fog around the current unit.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_RemoveFog(ScriptEngine script)
		{
			Unit u;

			u = g_scriptCurrentUnit;
			Unit_RemoveFog(u);
			return 0;
		}

		/*
		 * Make the current unit harvest spice.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return ??.
		 */
		internal static ushort Script_Unit_Harvest(ScriptEngine script)
		{
			Unit u;
			ushort packed;
			ushort type;

			u = g_scriptCurrentUnit;

			if (u.o.type != (byte)UnitType.UNIT_HARVESTER) return 0;
			if (u.amount >= 100) return 0;

			packed = CTile.Tile_PackTile(u.o.position);

			type = Map.Map_GetLandscapeType(packed);
			if (type != (ushort)LandscapeType.LST_SPICE && type != (ushort)LandscapeType.LST_THICK_SPICE) return 0;

			u.amount += (byte)(Tools.Tools_Random_256() & 1);
			u.o.flags.inTransport = true;

			Unit_UpdateMap(2, u);

			if (u.amount > 100) u.amount = 100;

			if ((Tools.Tools_Random_256() & 0x1F) != 0) return 1;

			Map.Map_ChangeSpiceAmount(packed, -1);

			return 0;
		}

		/*
		 * Check if the given tile is a valid destination. In case of for example
		 *  a carry-all it checks if the unit carrying can be placed on destination.
		 * In case of structures, it checks if you can walk into it.
		 *
		 * Stack: 1 - An encoded tile, indicating the destination.
		 *
		 * @param script The script engine to operate on.
		 * @return ??.
		 */
		internal static ushort Script_Unit_IsValidDestination(ScriptEngine script)
		{
			Unit u;
			Unit u2;
			ushort encoded;
			ushort index;

			u = g_scriptCurrentUnit;
			encoded = STACK_PEEK(script, 1);
			index = Tools.Tools_Index_Decode(encoded);

			switch (Tools.Tools_Index_GetType(encoded))
			{
				case IndexType.IT_TILE:
					if (!Map.Map_IsValidPosition(index)) return 1;
					if (u.o.linkedID == 0xFF) return 1;
					u2 = PoolUnit.Unit_Get_ByIndex(u.o.linkedID);
					u2.o.position = Tools.Tools_Index_GetTile(encoded);
					if (!Unit_IsTileOccupied(u2)) return 0;
					u2.o.position.x = 0xFFFF;
					u2.o.position.y = 0xFFFF;
					return 1;

				case IndexType.IT_STRUCTURE:
					{
						Structure s;

						s = PoolStructure.Structure_Get_ByIndex(index);
						if (s.o.houseID == Unit_GetHouseID(u)) return 0;
						if (u.o.linkedID == 0xFF) return 1;
						u2 = PoolUnit.Unit_Get_ByIndex(u.o.linkedID);
						return (ushort)(Unit_IsValidMovementIntoStructure(u2, s) != 0 ? 1 : 0);
					}

				default: return 1;
			}
		}

		/*
		 * Get a random tile around the Unit.
		 *
		 * Stack: 1 - An encoded index of a tile, completely ignored, as long as it is a tile.
		 *
		 * @param script The script engine to operate on.
		 * @return An encoded tile, or 0.
		 */
		internal static ushort Script_Unit_GetRandomTile(ScriptEngine script)
		{
			Unit u;
			tile32 tile;

			u = g_scriptCurrentUnit;

			if (Tools.Tools_Index_GetType(STACK_PEEK(script, 1)) != IndexType.IT_TILE) return 0;

			tile = CTile.Tile_MoveByRandom(u.o.position, 80, true);

			return Tools.Tools_Index_Encode(CTile.Tile_PackTile(tile), IndexType.IT_TILE);
		}

		/*
		 * Perform a random action when we are sitting idle, like rotating around.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_IdleAction(ScriptEngine script)
		{
			Unit u;
			ushort random;
			ushort movementType;
			ushort i;

			u = g_scriptCurrentUnit;

			random = Tools.Tools_RandomLCG_Range(0, 10);
			movementType = g_table_unitInfo[u.o.type].movementType;

			if (movementType != (ushort)MovementType.MOVEMENT_FOOT && movementType != (ushort)MovementType.MOVEMENT_TRACKED && movementType != (ushort)MovementType.MOVEMENT_WHEELED) return 0;

			if (movementType == (ushort)MovementType.MOVEMENT_FOOT && random > 8)
			{
				u.spriteOffset = (sbyte)(Tools.Tools_Random_256() & 0x3F);
				Unit_UpdateMap(2, u);
			}

			if (random > 2) return 0;

			/* Ensure the order of Tools_Random_256() calls. */
			i = (ushort)((Tools.Tools_Random_256() & 1) == 0 ? 1 : 0);
			Unit_SetOrientation(u, (sbyte)Tools.Tools_Random_256(), false, i);

			return 0;
		}

		/*
		 * Makes the current unit to go to the closest structure of the given type.
		 *
		 * Stack: 1 - The type of the structure.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 1 if and only if a structure has been found.
		 */
		internal static ushort Script_Unit_GoToClosestStructure(ScriptEngine script)
		{
			Unit u;
			Structure s = null;
			var find = new PoolFindStruct();
			ushort distanceMin = 0;

			u = g_scriptCurrentUnit;

			find.houseID = Unit_GetHouseID(u);
			find.index = 0xFFFF;
			find.type = STACK_PEEK(script, 1);

			while (true)
			{
				Structure s2;
				ushort distance;

				s2 = PoolStructure.Structure_Find(find);

				if (s2 == null) break;
				if (s2.state != (short)StructureState.STRUCTURE_STATE_IDLE) continue;
				if (s2.o.linkedID != 0xFF) continue;
				if (s2.o.script.variables[4] != 0) continue;

				distance = CTile.Tile_GetDistanceRoundedUp(s2.o.position, u.o.position);

				if (distance >= distanceMin && distanceMin != 0) continue;

				distanceMin = distance;
				s = s2;
			}

			if (s == null) return 0;

			Unit_SetAction(u, ActionType.ACTION_MOVE);
			Unit_SetDestination(u, Tools.Tools_Index_Encode(s.o.index, IndexType.IT_STRUCTURE));

			return 1;
		}

		static readonly sbyte[] offsets = { 0, -1, -64, -65 };
		/*
		 * Transform an MCV into Construction Yard.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return 1 if and only if the transformation succeeded.
		 */
		internal static ushort Script_Unit_MCVDeploy(ScriptEngine script)
		{
			Unit u;
			Structure s;
			ushort i;

			u = g_scriptCurrentUnit;

			Unit_UpdateMap(0, u);

			for (i = 0; i < 4; i++)
			{
				s = CStructure.Structure_Create((ushort)PoolStructure.StructureIndex.STRUCTURE_INDEX_INVALID, (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD, Unit_GetHouseID(u), (ushort)(CTile.Tile_PackTile(u.o.position) + offsets[i]));

				if (s != null)
				{
					Unit_Remove(u);
					return 1;
				}
			}

			if (Unit_GetHouseID(u) == (byte)CHouse.g_playerHouseID)
			{
				Gui.Gui.GUI_DisplayText(CString.String_Get_ByIndex(Text.STR_UNIT_IS_UNABLE_TO_DEPLOY_HERE), 0);
			}

			Unit_UpdateMap(1, u);

			return 0;
		}

		/*
		 * Get the best target around you. Only considers units on sand.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return An encoded unit index, or 0.
		 */
		internal static ushort Script_Unit_Sandworm_GetBestTarget(ScriptEngine script)
		{
			Unit u;
			Unit u2;

			u = g_scriptCurrentUnit;

			u2 = Unit_Sandworm_FindBestTarget(u);
			if (u2 == null) return 0;

			return Tools.Tools_Index_Encode(u2.o.index, IndexType.IT_UNIT);
		}

		/*
		 * Unknown function 2BD5.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return ??.
		 */
		internal static ushort Script_Unit_Unknown2BD5(ScriptEngine script)
		{
			Unit u;

			u = g_scriptCurrentUnit;

			switch (Tools.Tools_Index_GetType(u.o.script.variables[4]))
			{
				case IndexType.IT_UNIT:
					{
						Unit u2;

						u2 = Tools.Tools_Index_GetUnit(u.o.script.variables[4]);

						if (Tools.Tools_Index_Encode(u.o.index, IndexType.IT_UNIT) == u2.o.script.variables[4] && u2.o.houseID == u.o.houseID) return 1;

						u2.targetMove = 0;
					}
					break;

				case IndexType.IT_STRUCTURE:
					{
						Structure s;

						s = Tools.Tools_Index_GetStructure(u.o.script.variables[4]);
						if (Tools.Tools_Index_Encode(u.o.index, IndexType.IT_UNIT) == s.o.script.variables[4] && s.o.houseID == u.o.houseID) return 1;
					}
					break;

				default: break;
			}

			CObject.Object_Script_Variable4_Clear(u.o);
			return 0;
		}

		/*
		 * Blink the unit for 32 ticks.
		 *
		 * Stack: *none*.
		 *
		 * @param script The script engine to operate on.
		 * @return The value 0. Always.
		 */
		internal static ushort Script_Unit_Blink(ScriptEngine script)
		{
			Unit u;

			u = g_scriptCurrentUnit;
			u.blinkCounter = 32;
			return 0;
		}
	}
}
