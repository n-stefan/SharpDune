/* Object */

namespace SharpDune;

/*
* Flags for Object structure
*/
class ObjectFlags
{
    internal bool used;                         /*!< The Object is in use (no longer free in the pool). */
    internal bool allocated;                    /*!< The Object is allocated (created, and ready to be put on the map). */
    internal bool isNotOnMap;                   /*!< The Object is not on the map (under construction, in refinery, etc). */
    internal bool isSmoking;                    /*!< The Object has a smoke cloud coming out of it. */
    internal bool fireTwiceFlip;                /*!< Used for Unit fire twice, to keep track if it is the second shot. */
    internal bool animationFlip;                /*!< Used for Unit (bullet / missile) animation, to differ between two sprite groups. */
    internal bool bulletIsBig;                  /*!< If true, the Unit (bullet / sonic wave) is twice as big (visual only). */
    internal bool isWobbling;                   /*!< If true, the Unit will be wobbling during movement. */
    internal bool inTransport;                  /*!< The Unit is in transport (spaceport, reinforcement, harvester). */
    internal bool byScenario;                   /*!< The Unit is created by the scenario. */
    internal bool degrades;                     /*!< Structure degrades. Unit ?? */
    internal bool isHighlighted;                /*!< The Object is currently highlighted. */
    internal bool isDirty;                      /*!< If true, the Unit will be redrawn next update. */
    internal bool repairing;                    /*!< Structure is being repaired. */
    internal bool onHold;                       /*!< Structure is on hold. */
    internal bool notused_4_8000;
    internal bool isUnit;                       /*!< If true, this is an Unit, otherwise a Structure. */
    internal bool upgrading;                    /*!< Structure is being upgraded. */
    internal bool notused_6_0004;
    internal bool notused_6_0100;

    internal uint All
    {
        get
        {
            //var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            //for (var i = 0; i < fields.Length; i++) if ((bool)fields[i].GetValue(this)) value |= 1U << i;

            var value = 0U;
            if (used) value |= 1U << 0;
            if (allocated) value |= 1U << 1;
            if (isNotOnMap) value |= 1U << 2;
            if (isSmoking) value |= 1U << 3;
            if (fireTwiceFlip) value |= 1U << 4;
            if (animationFlip) value |= 1U << 5;
            if (bulletIsBig) value |= 1U << 6;
            if (isWobbling) value |= 1U << 7;
            if (inTransport) value |= 1U << 8;
            if (byScenario) value |= 1U << 9;
            if (degrades) value |= 1U << 10;
            if (isHighlighted) value |= 1U << 11;
            if (isDirty) value |= 1U << 12;
            if (repairing) value |= 1U << 13;
            if (onHold) value |= 1U << 14;
            //if (notused_4_8000) value |= 1U << 15;
            if (isUnit) value |= 1U << 16;
            if (upgrading) value |= 1U << 17;
            return value;
        }
    }
}

/*
* Data common to Structure and Unit.
*/
class CObject
{
    internal ushort index;                      /*!< The index of the Structure/Unit in the array. */
    internal byte type;                         /*!< Type of Structure/Unit. */
    internal byte linkedID;                     /*!< Structure/Unit we are linked to, or 0xFF if we are not linked to a Structure/Unit. */
    internal ObjectFlags flags;                 /*!< General flags of the Structure/Unit. */
    internal byte houseID;                      /*!< House of Structure. */
    internal byte seenByHouses;                 /*!< Bitmask of which houses have seen this object. */
    internal Tile32 position;                   /*!< Position on the map. */
    internal ushort hitpoints;                  /*!< Current hitpoints left. */
    internal ScriptEngine script;               /*!< The script engine instance of this Structure. */

    internal CObject()
    {
        flags = new ObjectFlags();
        script = new ScriptEngine();
        position = new Tile32();
    }
}

class ObjectInfoFlags
{
    internal bool hasShadow;                        /*!< If true, the Unit has a shadow below it. */
    internal bool factory;                          /*!< Structure can build other Structures or Units. */
    internal bool notOnConcrete;                    /*!< Structure cannot be build on concrete. */
    internal bool busyStateIsIncoming;              /*!< If true, the Structure has lights to indicate a Unit is incoming. This is then the BUSY state, where READY means it is processing the Unit. */
    internal bool blurTile;                         /*!< If true, this blurs the tile the Unit is on. */
    internal bool hasTurret;                        /*!< If true, the Unit has a turret seperate from his base unit. */
    internal bool conquerable;                      /*!< Structure can be invaded and subsequently conquered when hitpoints are low. */
    internal bool canBePickedUp;                    /*!< If true, it can be picked up (by a CarryAll). */
    internal bool noMessageOnDeath;                 /*!< Do not show a message (or sound) when this Structure / Unit is destroyed. */
    internal bool tabSelectable;                    /*!< Is Structure / Unit selectable by pressing tab (which cycles through all Units and Structures). */
    internal bool scriptNoSlowdown;                 /*!< If Structure / Unit is outside viewport, do not slow down scripting. */
    internal bool targetAir;                        /*!< Can target (and shoot) air units. */
    internal bool priority;                         /*!< If not set, it is never seen as any priority for Units (for auto-attack). */
}

/*
 * Data common to StructureInfo and UnitInfo.
 */
class ObjectInfo
{
    internal ushort stringID_abbrev;                 /*!< StringID of abbreviated name of Structure / Unit. */
    internal string name;                            /*!< Pointer to name of Structure / Unit. */
    internal ushort stringID_full;                   /*!< StringID of full name of Structure / Unit. */
    internal string wsa;                             /*!< Pointer to name of .wsa file. */
    internal ObjectInfoFlags flags;                  /*!< General flags of the ObjectInfo. */
    internal ushort spawnChance;                     /*!< Chance of spawning a Unit (if Structure: on destroying of Structure). */
    internal ushort hitpoints;                       /*!< Default hitpoints for this Structure / Unit. */
    internal ushort fogUncoverRadius;                /*!< Radius of fog to uncover. */
    internal ushort spriteID;                        /*!< SpriteID of Structure / Unit. */
    internal ushort buildCredits;                    /*!< How much credits it cost to build this Structure / Unit. Upgrading is 50% of this value. */
    internal ushort buildTime;                       /*!< Time required to build this Structure / Unit. */
    internal ushort availableCampaign;               /*!< In which campaign (starting at 1) this Structure / Unit is available. */
    internal uint structuresRequired;                /*!< Which structures are required before this Structure / Unit is available. */
    internal byte sortPriority;                      /*!< ?? */
    internal byte upgradeLevelRequired;              /*!< Which level of upgrade the Structure / Unit has to have before this is avialable. */
    internal ushort[] actionsPlayer = new ushort[4]; /*!< Actions for player Structure / Unit. */
    internal sbyte available;                        /*!< If this Structure / Unit is ordered (Starport) / available (Rest). 1+=yes (volume), 0=no, -1=upgrade-first. */
    internal ushort hintStringID;                    /*!< StringID of the hint shown for this Structure / Unit. */
    internal ushort priorityBuild;                   /*!< The amount of priority a Structure / Unit has when a new Structure / Unit has to be build. */
    internal ushort priorityTarget;                  /*!< The amount of priority a Structure / Unit has when being targetted. */
    internal byte availableHouse;                    /*!< To which house this Structure / Unit is available. */
}

class Object
{
    /*
     * Clear variable4 in a safe (and recursive) way from an object.
     * @param object The Oject to clear variable4 of.
     */
    internal static void Object_Script_Variable4_Clear(CObject obj)
    {
        CObject objectVariable;
        var encoded = obj.script.variables[4];

        if (encoded == 0) return;

        objectVariable = Tools_Index_GetObject(encoded);

        Object_Script_Variable4_Set(obj, 0);
        Object_Script_Variable4_Set(objectVariable, 0);
    }

    /*
     * Set in a safe way the new value for variable4.
     * @param o The Object to set variable4 for.
     * @param index The encoded index to set it to.
     */
    internal static void Object_Script_Variable4_Set(CObject o, ushort encoded)
    {
        StructureInfo si;
        CStructure s;

        if (o == null) return;

        o.script.variables[4] = encoded;

        if (o.flags.isUnit) return;

        si = g_table_structureInfo[o.type];
        if (!si.o.flags.busyStateIsIncoming) return;

        s = Structure_Get_ByIndex(o.index);
        if (Structure_GetLinkedUnit(s) != null) return;

        Structure_SetState(s, (short)((encoded == 0) ? StructureState.STRUCTURE_STATE_IDLE : StructureState.STRUCTURE_STATE_BUSY));
    }

    /*
     * Link two variable4 values to eachother, and clean up existing values if
     *  needed.
     * @param encodedFrom From where the link goes.
     * @param encodedTo To where the link goes.
     */
    internal static void Object_Script_Variable4_Link(ushort encodedFrom, ushort encodedTo)
    {
        CObject objectFrom;
        CObject objectTo;

        if (!Tools_Index_IsValid(encodedFrom)) return;
        if (!Tools_Index_IsValid(encodedTo)) return;

        objectFrom = Tools_Index_GetObject(encodedFrom);
        objectTo = Tools_Index_GetObject(encodedTo);

        if (objectFrom == null) return;
        if (objectTo == null) return;

        if (objectFrom.script.variables[4] != objectTo.script.variables[4])
        {
            Object_Script_Variable4_Clear(objectFrom);
            Object_Script_Variable4_Clear(objectTo);
        }
        if (objectFrom.script.variables[4] != 0) return;

        Object_Script_Variable4_Set(objectFrom, encodedTo);
        Object_Script_Variable4_Set(objectTo, encodedFrom);

        return;
    }

    /*
     * Get the object on the given packed tile.
     * @param packed The packed tile to get the object from.
     * @return The object.
     */
    internal static CObject Object_GetByPackedTile(ushort packed)
    {
        CTile t;

        if (Tile_IsOutOfMap(packed)) return null;

        t = g_map[packed];
        if (t.hasUnit) return Unit_Get_ByIndex((ushort)(t.index - 1)).o;
        if (t.hasStructure) return Structure_Get_ByIndex((ushort)(t.index - 1)).o;
        return null;
    }

    /*
     * Gets the distance from the given object to the given encoded index.
     * @param o The object.
     * @param encoded The encoded index.
     * @return The distance.
     */
    internal static ushort Object_GetDistanceToEncoded(CObject o, ushort encoded)
    {
        CStructure s;
        Tile32 position;

        s = Tools_Index_GetStructure(encoded);

        if (s != null)
        {
            ushort packed;

            position = s.o.position;
            packed = Tile_PackTile(position);

            /* ENHANCEMENT -- Originally this was o->type, where 'o' refers to a unit. */
            packed += g_table_structure_layoutEdgeTiles[g_table_structureInfo[s.o.type].layout][(Orientation_Orientation256ToOrientation8((byte)Tile_GetDirection(o.position, position)) + 4) & 7];

            position = Tile_UnpackTile(packed);
        }
        else
        {
            position = Tools_Index_GetTile(encoded);
        }

        return Tile_GetDistance(o.position, position);
    }
}
