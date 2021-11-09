/* Explosion file table */

namespace SharpDune.Table;

class TableExplosion
{
    /* EXPLOSION_IMPACT_SMALL */
    static readonly ExplosionCommandStruct[] s_explosion00 = {
        new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 153 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 153 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
    };

    /* EXPLOSION_IMPACT_MEDIUM */
    static readonly ExplosionCommandStruct[] s_explosion01 = {
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
    static readonly ExplosionCommandStruct[] s_explosion02 = {
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
    static readonly ExplosionCommandStruct[] s_explosion03 = {
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
    static readonly ExplosionCommandStruct[] s_explosion04 = {
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
    static readonly ExplosionCommandStruct[] s_explosion05 = {
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
    static readonly ExplosionCommandStruct[] s_explosion06 = {
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
    static readonly ExplosionCommandStruct[] s_explosion07 = {
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
    static readonly ExplosionCommandStruct[] s_explosion08 = {
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
    static readonly ExplosionCommandStruct[] s_explosion10 = {
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
    static readonly ExplosionCommandStruct[] s_explosion11 = {
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
    static readonly ExplosionCommandStruct[] s_explosion12 = {
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
    static readonly ExplosionCommandStruct[] s_explosion13 = {
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
    static readonly ExplosionCommandStruct[] s_explosion14 = {
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
    static readonly ExplosionCommandStruct[] s_explosion16 = {
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
    static readonly ExplosionCommandStruct[] s_explosion17 = {
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
    static readonly ExplosionCommandStruct[] s_explosion18 = {
        new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 183 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 54 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 184 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_STOP, parameter = 0 }
    };

    /* EXPLOSION_SPICE_BLOOM_TREMOR */
    static readonly ExplosionCommandStruct[] s_explosion19 = {
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

    /* EXPLOSION_TANK_FLAMES */
    static readonly ExplosionCommandStruct[] s_explosion09 = {
        new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 183 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 41 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_BLOOM_EXPLOSION, parameter = 0 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_TILE_DAMAGE, parameter = 0 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 203 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_SET_TIMEOUT, parameter = 3 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_MOVE_Y_POSITION, parameter = 65456 /*(ushort)-80*/ },
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
    static readonly ExplosionCommandStruct[] s_explosion15 = {
        new() { command = (byte)ExplosionCommand.EXPLOSION_SET_SPRITE, parameter = 183 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_PLAY_VOICE, parameter = 49 },
        new() { command = (byte)ExplosionCommand.EXPLOSION_MOVE_Y_POSITION, parameter = 65456 /*(ushort)-80*/ },
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

    internal static readonly ExplosionCommandStruct[][] g_table_explosion = { //[EXPLOSIONTYPE_MAX]
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
}
