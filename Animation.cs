/* Animation */

namespace SharpDune;

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

class CAnimation
{
    internal uint tickNext;                         /*!< Which tick this Animation should be called again. */
    internal ushort tileLayout;                     /*!< Tile layout of the Animation. */
    internal byte houseID;                          /*!< House of the item being animated. */
    internal byte current;                          /*!< At which command we currently are in the Animation. */
    internal byte iconGroup;                        /*!< Which iconGroup the sprites of the Animation belongs. */
    internal AnimationCommandStruct[] commands;     /*!< List of commands for this Animation. */
    internal Tile32 tile;                           /*!< Top-left tile of Animation. */
}

static class Animation
{
    const byte ANIMATION_MAX = 112;

    static readonly CAnimation[] g_animations = new CAnimation[ANIMATION_MAX];
    static uint s_animationTimer; /*!< Timer for animations. */

    /*
     * Stop with this Animation.
     * @param animation The Animation to stop.
     * @param parameter Not used.
     */
    static void Animation_Func_Stop(CAnimation animation)
    {
        var layout = g_table_structure_layoutTiles[animation.tileLayout];
        var layoutTileCount = g_table_structure_layoutTileCount[animation.tileLayout];
        var packed = Tile_PackTile(animation.tile);
        int i;
        var layoutPointer = 0;

        g_map[packed].hasAnimation = false;
        animation.commands = null;

        for (i = 0; i < layoutTileCount; i++)
        {
            var position = (ushort)(packed + layout[layoutPointer++]);
            var t = g_map[position];

            if (animation.tileLayout != 0)
            {
                t.groundTileID = g_mapTileID[position];
            }

            if (Map_IsPositionUnveiled(position))
            {
                t.overlayTileID = 0;
            }

            Map_Update(position, 0, false);
        }
    }

    /*
     * Abort this Animation.
     * @param animation The Animation to abort.
     * @param parameter Not used.
     */
    static void Animation_Func_Abort(CAnimation animation)
    {
        var packed = Tile_PackTile(animation.tile);

        g_map[packed].hasAnimation = false;
        animation.commands = null;

        Map_Update(packed, 0, false);
    }

    /*
     * Set the overlay sprite of the tile.
     * @param animation The Animation for which we change the overlay sprite.
     * @param parameter The TileID to which the overlay sprite is set.
     */
    static void Animation_Func_SetOverlayTile(CAnimation animation, short parameter)
    {
        var packed = Tile_PackTile(animation.tile);
        var t = g_map[packed];
        Debug.Assert(parameter >= 0);

        if (!Map_IsPositionUnveiled(packed)) return;

        t.overlayTileID = g_iconMap[g_iconMap[animation.iconGroup] + parameter];
        t.houseID = animation.houseID;

        Map_Update(packed, 0, false);
    }

    /*
     * Pause the animation for a few ticks.
     * @param animation The Animation to pause.
     * @param parameter How many ticks it should pause.
     * @note Delays are randomly delayed with [0. . .3] ticks.
     */
    static void Animation_Func_Pause(CAnimation animation, short parameter)
    {
        Debug.Assert(parameter >= 0);

        animation.tickNext = (uint)(g_timerGUI + parameter + (Tools_Random_256() % 4));
    }

    /*
     * Rewind the animation.
     * @param animation The Animation to rewind.
     * @param parameter Not used.
     */
    static void Animation_Func_Rewind(CAnimation animation) =>
        animation.current = 0;

    /*
     * Play a Voice on the tile of animation.
     * @param animation The Animation which gives the position the voice plays at.
     * @param parameter The VoiceID to play.
     */
    static void Animation_Func_PlayVoice(CAnimation animation, short parameter) =>
        Voice_PlayAtTile(parameter, animation.tile);

    /*
     * Set the ground sprite of the tile.
     * @param animation The Animation for which we change the ground sprite.
     * @param parameter The offset in the iconGroup to which the ground sprite is set.
     */
    static void Animation_Func_SetGroundTile(CAnimation animation, short parameter)
    {
        var specialMap = new ushort[1];
        Span<ushort> iconMap;
        var layout = g_table_structure_layoutTiles[animation.tileLayout];
        var layoutTileCount = g_table_structure_layoutTileCount[animation.tileLayout];
        var packed = Tile_PackTile(animation.tile);
        int i;
        var layoutPointer = 0;
        var iconMapPointer = 0;

        iconMap = g_iconMap.AsSpan(g_iconMap[animation.iconGroup] + layoutTileCount * parameter);

        /* Some special case for turrets */
        if ((parameter > 1) && (animation.iconGroup == (byte)IconMapEntries.ICM_ICONGROUP_BASE_DEFENSE_TURRET || animation.iconGroup == (byte)IconMapEntries.ICM_ICONGROUP_BASE_ROCKET_TURRET))
        {
            var s = Structure_Get_ByPackedTile(packed);
            Debug.Assert(s != null);
            Debug.Assert(layoutTileCount == 1);

            specialMap[0] = (ushort)(s.rotationSpriteDiff + g_iconMap[g_iconMap[animation.iconGroup]] + 2);
            iconMap = specialMap;
        }

        for (i = 0; i < layoutTileCount; i++)
        {
            var position = (ushort)(packed + layout[layoutPointer++]);
            var tileID = iconMap[iconMapPointer++];
            var t = g_map[position];

            if (t.groundTileID == tileID) continue;
            t.groundTileID = tileID;
            t.houseID = animation.houseID;

            if (Map_IsPositionUnveiled(position))
            {
                t.overlayTileID = 0;
            }

            Map_Update(position, 0, false);

            Map_MarkTileDirty(position);
        }
    }

    /*
     * Forward the current Animation with the given amount of steps.
     * @param animation The Animation to forward.
     * @param parameter With what value you want to forward the Animation.
     * @note Forwarding with 1 is just the next instruction, making this command a NOP.
     */
    static void Animation_Func_Forward(CAnimation animation, short parameter) =>
        animation.current += (byte)(parameter - 1);

    /*
     * Set the IconGroup of the Animation.
     * @param animation The Animation to change.
     * @param parameter To what value IconGroup should change.
     */
    static void Animation_Func_SetIconGroup(CAnimation animation, short parameter)
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
        var animation = g_animations;
        var t = g_map[packed];
        int i;

        if (!t.hasAnimation) return;

        for (i = 0; i < ANIMATION_MAX; i++)
        { //, animation++) {
            if (animation[i].commands == null) continue;
            if (Tile_PackTile(animation[i].tile) != packed) continue;

            Animation_Func_Stop(animation[i]);
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
    internal static void Animation_Start(AnimationCommandStruct[] commands, Tile32 tile, ushort tileLayout, byte houseID, byte iconGroup)
    {
        var animation = g_animations;
        var packed = Tile_PackTile(tile);
        CTile t;
        int i;

        t = g_map[packed];
        Animation_Stop_ByTile(packed);

        for (i = 0; i < ANIMATION_MAX; i++)
        { //, animation++) {
            if (animation[i].commands != null) continue;

            animation[i].tickNext = g_timerGUI;
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
        var animation = g_animations;
        int i;

        if (s_animationTimer > g_timerGUI) return;
        s_animationTimer += 10000;

        for (i = 0; i < ANIMATION_MAX; i++)
        { //, animation++) {
            if (animation[i].commands == null) continue;

            if (animation[i].tickNext <= g_timerGUI)
            {
                var commands = animation[i].commands[animation[i].current];
                var parameter = (short)commands.parameter;
                Debug.Assert((parameter & 0x0800) == 0 || (parameter & 0xF000) != 0); /* Validate if the compiler sign-extends correctly */

                animation[i].current++;

                switch ((AnimationCommand)commands.command)
                {
                    case AnimationCommand.ANIMATION_STOP:
                    default: Animation_Func_Stop(animation[i]); break;

                    case AnimationCommand.ANIMATION_ABORT: Animation_Func_Abort(animation[i]); break;
                    case AnimationCommand.ANIMATION_SET_OVERLAY_TILE: Animation_Func_SetOverlayTile(animation[i], parameter); break;
                    case AnimationCommand.ANIMATION_PAUSE: Animation_Func_Pause(animation[i], parameter); break;
                    case AnimationCommand.ANIMATION_REWIND: Animation_Func_Rewind(animation[i]); break;
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
        for (var i = 0; i < g_animations.Length; i++) g_animations[i] = new CAnimation(); //memset(g_animations, 0, ANIMATION_MAX * sizeof(Animation));
    }
}
