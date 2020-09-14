/* Save Load */

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SharpDune
{
    delegate uint SaveLoadDescCallback(object obj, uint value, bool loading); //Func<object, uint, bool, uint>
	delegate object SaveLoadDescGetter();
	delegate void SaveLoadDescSetter(object value, int index);

	/*
	 * Types of storage we support / understand.
	 */
	enum SaveLoadType
	{
		SLDT_INVALID,                                           /*!< Invalid value. */
		SLDT_UINT8,                                             /*!< 8bit unsigned integer. */
		SLDT_UINT16,                                            /*!< 16bit unsigned integer. */
		SLDT_UINT32,                                            /*!< 32bit unsigned integer. */

		SLDT_INT8,                                              /*!< 8bit signed integer. */
		SLDT_INT16,                                             /*!< 16bit signed integer. */
		SLDT_INT32,                                             /*!< 32bit signed integer. */

		SLDT_CALLBACK,                                          /*!< A callback handler. */
		SLDT_SLD,                                               /*!< A SaveLoadDesc. */

		SLDT_HOUSEFLAGS,                                        /*!< flags for House struct */
		SLDT_OBJECTFLAGS,                                       /*!< flags for Object struct */
		SLDT_TEAMFLAGS,                                         /*!< flags for Team struct */

		SLDT_NULL                                               /*!< Not stored. */
	}

	/*
	 * Table definition for SaveLoad descriptors.
	 */
	class SaveLoadDesc
	{
		//internal int offset;                                           /*!< The offset in the object, in bytes. */
		internal SaveLoadType type_disk;                                 /*!< The type it is on disk. */
		internal SaveLoadType type_memory;                               /*!< The type it is in memory. */
		internal ushort count;                                           /*!< The number of elements */
		internal SaveLoadDesc[] sld;                                     /*!< The SaveLoadDesc. */
		//internal int size;                                             /*!< The size of an element. */
		internal SaveLoadDescCallback callback;                          /*!< The custom callback. */
		//internal object address;                                         /*!< The address of the element. */
		internal string member;
		internal SaveLoadDescGetter getter;
		internal SaveLoadDescSetter setter;
	}

	class SaveLoad
	{
		//static readonly Type obj = typeof(Object);
		//static readonly Type house = typeof(House);
		//static readonly Type structure = typeof(Structure);
		//static readonly Type unit = typeof(Unit);
		//static readonly Type team = typeof(Team);
		//static readonly Type scenario = typeof(Scenario);
		//static readonly Type reinforcement = typeof(Reinforcement);
		//static readonly Type scriptEngine = typeof(ScriptEngine);
		//static readonly Type dir24 = typeof(dir24);

		static SaveLoadDesc[] g_saveScriptEngine = {
			SLD_ENTRY(/*scriptEngine,*/ SaveLoadType.SLDT_UINT16, "delay"),
			SLD_CALLB(/*scriptEngine,*/ SaveLoadType.SLDT_UINT32, "script", Info.SaveLoad_Script_Script),
			SLD_EMPTY(SaveLoadType.SLDT_UINT32),
			SLD_ENTRY(/*scriptEngine,*/ SaveLoadType.SLDT_UINT16, "returnValue"),
			SLD_ENTRY(/*scriptEngine,*/ SaveLoadType.SLDT_UINT8, "framePointer"),
			SLD_ENTRY(/*scriptEngine,*/ SaveLoadType.SLDT_UINT8, "stackPointer"),
			SLD_ARRAY(/*scriptEngine,*/ SaveLoadType.SLDT_UINT16, "variables", 5),
			SLD_ARRAY(/*scriptEngine,*/ SaveLoadType.SLDT_UINT16, "stack", 15),
			SLD_ENTRY(/*scriptEngine,*/ SaveLoadType.SLDT_UINT8, "isSubroutine"),
			SLD_END()
		};

		static SaveLoadDesc[] g_saveObject = {
			SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT16, "index"),
			SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT8, "type"),
			SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT8, "linkedID"),
			SLD_ENTRY2(/*obj,*/ SaveLoadType.SLDT_UINT32, "flags", SaveLoadType.SLDT_OBJECTFLAGS),
			SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT8, "houseID"),
			SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT8, "seenByHouses"),
			SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT16, "position.x"),
			SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT16, "position.y"),
			SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT16, "hitpoints"),
			SLD_SLD(/*obj,*/ "script", g_saveScriptEngine),
			SLD_END()
		};

		static SaveLoadDesc[] s_saveHouse = {
			SLD_ENTRY2(/*house,*/ SaveLoadType.SLDT_UINT16, "index", SaveLoadType.SLDT_UINT8),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "harvestersIncoming"),
			SLD_ENTRY2(/*house,*/ SaveLoadType.SLDT_UINT16, "flags", SaveLoadType.SLDT_HOUSEFLAGS),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "unitCount"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "unitCountMax"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "unitCountEnemy"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "unitCountAllied"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT32, "structuresBuilt"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "credits"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "creditsStorage"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "powerProduction"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "powerUsage"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "windtrapCount"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "creditsQuota"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "palacePosition.x"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "palacePosition.y"),
			SLD_EMPTY(SaveLoadType.SLDT_UINT16),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "timerUnitAttack"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "timerSandwormAttack"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "timerStructureAttack"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "starportTimeLeft"),
			SLD_ENTRY(/*house,*/ SaveLoadType.SLDT_UINT16, "starportLinkedID"),
			SLD_ARRAY(/*house,*/ SaveLoadType.SLDT_UINT16, "ai_structureRebuild", 10),
			SLD_END()
		};

		static SaveLoadDesc[] s_saveStructure = {
			SLD_SLD(/*structure,*/ "o", g_saveObject),
			SLD_ENTRY(/*structure,*/ SaveLoadType.SLDT_UINT16, "creatorHouseID"),
			SLD_ENTRY(/*structure,*/ SaveLoadType.SLDT_UINT16, "rotationSpriteDiff"),
			SLD_EMPTY(SaveLoadType.SLDT_UINT8),
			SLD_ENTRY(/*structure,*/ SaveLoadType.SLDT_UINT16, "objectType"),
			SLD_ENTRY(/*structure,*/ SaveLoadType.SLDT_UINT8, "upgradeLevel"),
			SLD_ENTRY(/*structure,*/ SaveLoadType.SLDT_UINT8, "upgradeTimeLeft"),
			SLD_ENTRY(/*structure,*/ SaveLoadType.SLDT_UINT16, "countDown"),
			SLD_ENTRY(/*structure,*/ SaveLoadType.SLDT_UINT16, "buildCostRemainder"),
			SLD_ENTRY(/*structure,*/ SaveLoadType.SLDT_INT16, "state"),
			SLD_ENTRY(/*structure,*/ SaveLoadType.SLDT_UINT16, "hitpointsMax"),
			SLD_END()
		};

		static SaveLoadDesc[] s_saveUnitOrientation = {
			SLD_ENTRY(/*dir24,*/ SaveLoadType.SLDT_INT8, "speed"),
			SLD_ENTRY(/*dir24,*/ SaveLoadType.SLDT_INT8, "target"),
			SLD_ENTRY(/*dir24,*/ SaveLoadType.SLDT_INT8, "current"),
			SLD_END()
		};

		static SaveLoadDesc[] s_saveUnit = {
			SLD_SLD(/*unit,*/ "o", g_saveObject),
			SLD_EMPTY(SaveLoadType.SLDT_UINT16),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, "currentDestination.x"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, "currentDestination.y"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, "originEncoded"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, "actionID"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, "nextActionID"),
			SLD_ENTRY2(/*unit,*/ SaveLoadType.SLDT_UINT8, "fireDelay", SaveLoadType.SLDT_UINT16),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, "distanceToDestination"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, "targetAttack"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, "targetMove"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, "amount"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, "deviated"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, "targetLast.x"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, "targetLast.y"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, "targetPreLast.x"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, "targetPreLast.y"),
			SLD_SLD2(/*unit,*/ "orientation", s_saveUnitOrientation, 2),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, "speedPerTick"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, "speedRemainder"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, "speed"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, "movingSpeed"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, "wobbleIndex"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_INT8, "spriteOffset"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, "blinkCounter"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, "team"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, "timer"),
			SLD_ARRAY(/*unit,*/ SaveLoadType.SLDT_UINT8, "route", 14),
			SLD_END()
		};

		static SaveLoadDesc[] s_saveUnitNewIndex = {
			SLD_ENTRY(/*obj,*/ SaveLoadType.SLDT_UINT16, "index"),
			SLD_END()
		};

		static SaveLoadDesc[] s_saveUnitNew = {
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT16, "fireDelay"),
			SLD_ENTRY(/*unit,*/ SaveLoadType.SLDT_UINT8, "deviatedHouse"),
			SLD_EMPTY(SaveLoadType.SLDT_UINT8),
			SLD_EMPTY2(SaveLoadType.SLDT_UINT16, 6),
			SLD_END()
		};

        static SaveLoadDesc[] s_saveTeam = {
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, "index"),
			SLD_ENTRY2(/*team,*/ SaveLoadType.SLDT_UINT16, "flags", SaveLoadType.SLDT_TEAMFLAGS),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, "members"),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, "minMembers"),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, "maxMembers"),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, "movementType"),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, "action"),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, "actionStart"),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT8, "houseID"),
			SLD_EMPTY2(SaveLoadType.SLDT_UINT8, 3),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, "position.x"),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, "position.y"),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, "targetTile"),
			SLD_ENTRY(/*team,*/ SaveLoadType.SLDT_UINT16, "target"),
			SLD_SLD(/*team,*/ "script", g_saveScriptEngine),
			SLD_END()
		};

		static SaveLoadDesc[] s_saveReinforcement = {
			SLD_ENTRY(/*reinforcement,*/ SaveLoadType.SLDT_UINT16, "unitID"),
			SLD_ENTRY(/*reinforcement,*/ SaveLoadType.SLDT_UINT16, "locationID"),
			SLD_ENTRY(/*reinforcement,*/ SaveLoadType.SLDT_UINT16, "timeLeft"),
			SLD_ENTRY(/*reinforcement,*/ SaveLoadType.SLDT_UINT16, "timeBetween"),
			SLD_ENTRY(/*reinforcement,*/ SaveLoadType.SLDT_UINT16, "repeat"),
			SLD_END()
		};

		internal static SaveLoadDesc[] g_saveScenario = {
			SLD_ENTRY(/*scenario,*/ SaveLoadType.SLDT_UINT16, "score"),
			SLD_ENTRY(/*scenario,*/ SaveLoadType.SLDT_UINT16, "winFlags"),
			SLD_ENTRY(/*scenario,*/ SaveLoadType.SLDT_UINT16, "loseFlags"),
			SLD_ENTRY(/*scenario,*/ SaveLoadType.SLDT_UINT32, "mapSeed"),
			SLD_ENTRY(/*scenario,*/ SaveLoadType.SLDT_UINT16, "mapScale"),
			SLD_ENTRY(/*scenario,*/ SaveLoadType.SLDT_UINT16, "timeOut"),
			SLD_ARRAY(/*scenario,*/ SaveLoadType.SLDT_UINT8, "pictureBriefing", 14),
			SLD_ARRAY(/*scenario,*/ SaveLoadType.SLDT_UINT8, "pictureWin", 14),
			SLD_ARRAY(/*scenario,*/ SaveLoadType.SLDT_UINT8, "pictureLose", 14),
			SLD_ENTRY(/*scenario,*/ SaveLoadType.SLDT_UINT16, "killedAllied"),
			SLD_ENTRY(/*scenario,*/ SaveLoadType.SLDT_UINT16, "killedEnemy"),
			SLD_ENTRY(/*scenario,*/ SaveLoadType.SLDT_UINT16, "destroyedAllied"),
			SLD_ENTRY(/*scenario,*/ SaveLoadType.SLDT_UINT16, "destroyedEnemy"),
			SLD_ENTRY(/*scenario,*/ SaveLoadType.SLDT_UINT16, "harvestedAllied"),
			SLD_ENTRY(/*scenario,*/ SaveLoadType.SLDT_UINT16, "harvestedEnemy"),
			SLD_SLD2(/*scenario,*/ "reinforcement", s_saveReinforcement, 16),
			SLD_END()
		};

		//static int offset(Type c, string m) //(((size_t)&((c *)8)->m) - 8)
		//{
		//	var index = m.IndexOf('.');
		//	var handle = (index != -1) ?
		//		c.GetField(m[..index], BindingFlags.Instance | BindingFlags.NonPublic).FieldType.GetField(m[(index + 1)..], BindingFlags.Instance | BindingFlags.NonPublic).FieldHandle :
		//		c.GetField(m, BindingFlags.Instance | BindingFlags.NonPublic).FieldHandle;
		//	var offset = Marshal.ReadInt32(handle.Value + (4 + IntPtr.Size)) & 0xFFFFFF;
		//	return offset;
		//}

		//static int item_size(Type c, string m) //sizeof(((c *)0)->m)
		//{
		//	var size = Common.SizeOf(c.GetField(m, BindingFlags.Instance | BindingFlags.NonPublic).FieldType);
		//	return size;
		//}

		/*
         * An empty entry. Just to pad bytes on disk.
         * @param t The type on disk.
         */
		internal static SaveLoadDesc SLD_EMPTY(SaveLoadType t) =>
			new SaveLoadDesc { /*offset = 0,*/ type_disk = t, type_memory = SaveLoadType.SLDT_NULL, count = 1, sld = null, /*size = 0,*/ callback = null };

		/*
         * An empty array. Just to pad bytes on disk.
         * @param t The type on disk.
         * @param n The number of elements.
         */
		internal static SaveLoadDesc SLD_EMPTY2(SaveLoadType t, ushort n) =>
			new SaveLoadDesc { /*offset = 0,*/ type_disk = t, type_memory = SaveLoadType.SLDT_NULL, count = n, sld = null, /*size = 0,*/ callback = null };

		/*
         * A normal entry.
         * @param c The class.
         * @param t The type on disk / in memory.
         * @param m The member of the class.
         */
		internal static SaveLoadDesc SLD_ENTRY(/*Type c,*/ SaveLoadType t, string m) =>
			new SaveLoadDesc { member = m, /*offset = offset(c, m),*/ type_disk = t, type_memory = t, count = 1, sld = null, /*size = item_size(c, m),*/ callback = null };

		internal static SaveLoadDesc SLD_GENTRY(SaveLoadType t, SaveLoadDescGetter g, SaveLoadDescSetter s) =>
			new SaveLoadDesc { /*offset = 0,*/ type_disk = t, type_memory = t, count = 1, sld = null, /*size = Common.SizeOf(m),*/ callback = null, getter = g, setter = s };

		//internal static SaveLoadDesc SLD_GENTRY<M>(SaveLoadType t, M m, SaveLoadDescSetter s) =>
		//	new SaveLoadDesc { /*offset = 0,*/ type_disk = t, type_memory = t, count = 1, sld = null, /*size = Common.SizeOf(m),*/ callback = null, address = m, setter = s };

		/*
         * A full entry.
         * @param c The class.
         * @param t The type on disk.
         * @param m The member of the class.
         * @param t2 The type in memory.
         */
		internal static SaveLoadDesc SLD_ENTRY2(/*Type c,*/ SaveLoadType t, string m, SaveLoadType t2) =>
			new SaveLoadDesc { member = m, /*offset = offset(c, m),*/ type_disk = t, type_memory = t2, count = 1, sld = null, /*size = item_size(c, m),*/ callback = null };

		internal static SaveLoadDesc SLD_GENTRY2(SaveLoadType t, SaveLoadType t2, SaveLoadDescGetter g, SaveLoadDescSetter s) =>
			new SaveLoadDesc { /*offset = 0,*/ type_disk = t, type_memory = t2, count = 1, sld = null, /*size = Common.SizeOf(m),*/ callback = null, getter = g, setter = s };

		//internal static SaveLoadDesc SLD_GENTRY2<M>(SaveLoadType t, M m, SaveLoadType t2, SaveLoadDescSetter s) =>
		//	new SaveLoadDesc { /*offset = 0,*/ type_disk = t, type_memory = t2, count = 1, sld = null, /*size = Common.SizeOf(m),*/ callback = null, address = m, setter = s };

		/*
         * A normal array.
         * @param c The class.
         * @param t The type on disk / in memory.
         * @param m The member of the class.
         * @param n The number of elements.
         */
		internal static SaveLoadDesc SLD_ARRAY(/*Type c,*/ SaveLoadType t, string m, ushort n) =>
			new SaveLoadDesc { member = m, /*offset = offset(c, m),*/ type_disk = t, type_memory = t, count = n, sld = null, /*size = item_size(c, m) / n,*/ callback = null };

		internal static SaveLoadDesc SLD_GARRAY(SaveLoadType t, SaveLoadDescGetter g, SaveLoadDescSetter s, ushort n) =>
			new SaveLoadDesc { /*offset = 0,*/ type_disk = t, type_memory = t, count = n, sld = null, /*size = Common.SizeOf(m) / n,*/ callback = null, getter = g, setter = s };

		//internal static SaveLoadDesc SLD_GARRAY<M>(SaveLoadType t, M m, ushort n) =>
		//	new SaveLoadDesc { /*offset = 0,*/ type_disk = t, type_memory = t, count = n, sld = null, /*size = Common.SizeOf(m) / n,*/ callback = null, address = m };

		/*
         * A callback entry.
         * @param c The class.
         * @param t The type on disk.
         * @param m The member of the class.
         * @param p The callback.
         */
		internal static SaveLoadDesc SLD_CALLB(/*Type c,*/ SaveLoadType t, string m, SaveLoadDescCallback p) =>
			new SaveLoadDesc { member = m, /*offset = offset(c, m),*/ type_disk = t, type_memory = SaveLoadType.SLDT_CALLBACK, count = 1, sld = null, /*size = item_size(c, m),*/ callback = p };

		internal static SaveLoadDesc SLD_GCALLB(SaveLoadType t, SaveLoadDescGetter g, SaveLoadDescCallback p) =>
			new SaveLoadDesc { /*offset = 0,*/ type_disk = t, type_memory = SaveLoadType.SLDT_CALLBACK, count = 1, sld = null, /*size = Common.SizeOf(m),*/ callback = p, getter = g };

		//internal static SaveLoadDesc SLD_GCALLB<M>(SaveLoadType t, M m, SaveLoadDescCallback p) =>
		//	new SaveLoadDesc { /*offset = 0,*/ type_disk = t, type_memory = SaveLoadType.SLDT_CALLBACK, count = 1, sld = null, /*size = Common.SizeOf(m),*/ callback = p, address = m };

		/* Indicates end of array. */
		internal static SaveLoadDesc SLD_END() =>
			new SaveLoadDesc { /*offset = 0,*/ type_disk = SaveLoadType.SLDT_NULL, type_memory = SaveLoadType.SLDT_NULL, count = 0, sld = null, /*size = 0,*/ callback = null };

		/*
		 * A struct entry.
		 * @param c The class.
		 * @param m The member of the class.
		 * @param s The SaveLoadDesc.
		 */
		static SaveLoadDesc SLD_SLD(/*Type c,*/ string m, SaveLoadDesc[] s) =>
			new SaveLoadDesc { member = m, /*offset = offset(c, m),*/ type_disk = SaveLoadType.SLDT_SLD, type_memory = SaveLoadType.SLDT_SLD, count = 1, sld = s, /*size = item_size(c, m),*/ callback = null };

		internal static SaveLoadDesc SLD_GSLD(SaveLoadDescGetter g, SaveLoadDesc[] s) =>
			new SaveLoadDesc { /*offset = 0,*/ type_disk = SaveLoadType.SLDT_SLD, type_memory = SaveLoadType.SLDT_SLD, count = 1, sld = s, /*size = Common.SizeOf(m),*/ callback = null, getter = g };

		//internal static SaveLoadDesc SLD_GSLD<M>(M m, SaveLoadDesc[] s) =>
		//	new SaveLoadDesc { /*offset = 0,*/ type_disk = SaveLoadType.SLDT_SLD, type_memory = SaveLoadType.SLDT_SLD, count = 1, sld = s, /*size = Common.SizeOf(m),*/ callback = null, address = m };

		/*
		 * A struct array.
		 * @param c The class.
		 * @param m The member of the class.
		 * @param s The SaveLoadDesc.
		 * @param n The number of elements.
		 */
		static SaveLoadDesc SLD_SLD2(/*Type c,*/ string m, SaveLoadDesc[] s, ushort n) =>
			new SaveLoadDesc { member = m, /*offset = offset(c, m),*/ type_disk = SaveLoadType.SLDT_SLD, type_memory = SaveLoadType.SLDT_SLD, count = n, sld = s, /*size = item_size(c, m) / n,*/ callback = null };

		/*
		 * Get the length of the struct how it would be on disk.
		 * @param sld The description of the struct.
		 * @return The length of the struct on disk.
		 */
		internal static uint SaveLoad_GetLength(SaveLoadDesc[] sld)
		{
			uint length = 0;
			int i = 0;

			while (sld[i].type_disk != SaveLoadType.SLDT_NULL)
			{
				switch (sld[i].type_disk)
				{
					case SaveLoadType.SLDT_NULL: length += 0; break;
					case SaveLoadType.SLDT_CALLBACK: length += 0; break;
					case SaveLoadType.SLDT_UINT8: length += (uint)Common.SizeOf(typeof(byte)) * sld[i].count; break;
					case SaveLoadType.SLDT_UINT16: length += (uint)Common.SizeOf(typeof(ushort)) * sld[i].count; break;
					case SaveLoadType.SLDT_UINT32: length += (uint)Common.SizeOf(typeof(uint)) * sld[i].count; break;
					case SaveLoadType.SLDT_INT8: length += (uint)Common.SizeOf(typeof(sbyte)) * sld[i].count; break;
					case SaveLoadType.SLDT_INT16: length += (uint)Common.SizeOf(typeof(short)) * sld[i].count; break;
					case SaveLoadType.SLDT_INT32: length += (uint)Common.SizeOf(typeof(int)) * sld[i].count; break;
					case SaveLoadType.SLDT_SLD: length += SaveLoad_GetLength(sld[i].sld) * sld[i].count; break;
					default: length += 0; break;
				}
				i++;
			}

			return length;
		}

		static void FillArray(Array array, ArrayList values)
		{
			var index = 0;
			if (array.GetValue(0) is Array)
			{
				for (var i = 0; i < array.Length; i++)
				{
					var subArray = (Array)array.GetValue(i);
				    for (var j = 0; j < subArray.Length; j++)
				    {
						subArray.SetValue(values[index++], j);
					}
				}
			}
			else
			{
				for (var i = 0; i < array.Length; i++)
				{
					array.SetValue(values[index++], i);
				}
			}
		}

		/*
		 * Load from a file into a struct.
		 * @param sld The description of the struct.
		 * @param fp The file to read from.
		 * @param object The object instance to read to.
		 * @return True if and only if the reading was successful.
		 */
		internal static bool SaveLoad_Load(SaveLoadDesc[] sld, BinaryReader fp, object obj)
		{
			var flags = BindingFlags.Instance | BindingFlags.NonPublic;
			var c = 0;
			var type = obj?.GetType();
			var sb = new StringBuilder();
			var values = new ArrayList();
			FieldInfo field = null;
			string member;
			var index = -1;

			while (sld[c].type_disk != SaveLoadType.SLDT_NULL)
			{
				object/*uint*/ value = 0;
				object ptr = null;
				object subPtr;

				member = sld[c].member;
				if (type != null && member != null)
				{
					index = member.IndexOf('.');

					field = (index != -1) ?
						type.GetField(member[..index], flags).FieldType.GetField(member[(index + 1)..], flags) :
						type.GetField(member, flags);

					ptr = (index != -1) ?
						type.GetField(member[..index], flags).GetValue(obj) :
						obj;
				}
				else
				{
					ptr = sld[c].getter?.Invoke(); //sld[c].address;
				}

				for (var i = 0; i < sld[c].count; i++)
				{
					//void* ptr = (sld->address == NULL ? ((uint8*)object) + sld->offset : (uint8*)sld->address) + i * sld->size;
					//object ptr = (sld[c].address == null ? ((byte[])obj)[sld[c].offset..] : (byte[])sld[c].address)[(i * sld[c].size)..];
					//object ptr = sld[c].address == null ? ((byte[])obj)[(sld[c].offset + i * sld[c].size)..] : ((byte[])sld[c].address)[(i * sld[c].size)..];
					
					switch (sld[c].type_disk)
					{
						case SaveLoadType.SLDT_CALLBACK:
						case SaveLoadType.SLDT_SLD:
						case SaveLoadType.SLDT_NULL:
							value = 0;
							break;

						case SaveLoadType.SLDT_UINT8:
							value = fp.ReadByte(); //if (fread(&v, sizeof(uint8), 1, fp) != 1) return false;
							break;

						case SaveLoadType.SLDT_UINT16:
							value = fp.ReadUInt16(); //if (!CFile.fread_le_uint16(ref v, (FileStream)fp.BaseStream)) return false;
							break;

						case SaveLoadType.SLDT_UINT32:
							value = fp.ReadUInt32(); //if (!CFile.fread_le_uint32(ref v, (FileStream)fp.BaseStream)) return false;
							break;

						case SaveLoadType.SLDT_INT8:
							value = fp.ReadSByte(); //if (fread(&v, sizeof(int8), 1, fp) != 1) return false;
							break;

						case SaveLoadType.SLDT_INT16:
							value = fp.ReadInt16(); //if (!CFile.fread_le_int16(ref v, (FileStream)fp.BaseStream)) return false;
							break;

						case SaveLoadType.SLDT_INT32:
							value = fp.ReadInt32(); //if (!CFile.fread_le_int32(ref v, (FileStream)fp.BaseStream)) return false;
							break;

						case SaveLoadType.SLDT_INVALID:
						default:
							Trace.WriteLine("ERROR: Error in Save/Load structure descriptions");
							return false;
					}

					switch (sld[c].type_memory)
					{
						case SaveLoadType.SLDT_NULL:
							break;

						case SaveLoadType.SLDT_UINT8: //ptr = (byte)value;
						case SaveLoadType.SLDT_UINT16: //ptr = (ushort)value;
						case SaveLoadType.SLDT_UINT32: //ptr = value;
						case SaveLoadType.SLDT_INT8: //ptr = (sbyte)value;
						case SaveLoadType.SLDT_INT16: //ptr = (short)value;
						case SaveLoadType.SLDT_INT32: //ptr = (int)value;
							if (field != null)
                            {
								if (field.FieldType == typeof(string))
								{
									sb.Append(Convert.ToChar(value));
									if (i == sld[c].count - 1)
									{
										field.SetValue(ptr, sb.Replace("\0", string.Empty).ToString());
										sb.Clear();
									}
								}
								else if (field.FieldType.IsArray)
                                {
									values.Add(value);
									if (i == sld[c].count - 1)
									{
										FillArray((Array)field.GetValue(ptr), values);
										values.Clear();
									}
								}
								else
								{
									field.SetValue(ptr, (sld[c].type_memory == SaveLoadType.SLDT_UINT8) ? Convert.ToByte(value) : value);
									if (ptr is ValueType && index != -1)
									{
										type.GetField(member[..index], flags).SetValue(obj, ptr);
									}
								}
                            }
							else
                            {
								sld[c].setter?.Invoke(value, i);
							}
							break;

						case SaveLoadType.SLDT_HOUSEFLAGS:
							{
								var v = Convert.ToUInt32(value);
                                var f = new HouseFlags
                                {
                                    used = (v & 0x01) == 0x01, //? true : false;
                                    human = (v & 0x02) == 0x02, //? true : false;
                                    doneFullScaleAttack = (v & 0x04) == 0x04, //? true : false;
                                    isAIActive = (v & 0x08) == 0x08, //? true : false;
                                    radarActivated = (v & 0x10) == 0x10, //? true : false;
                                    unused_0020 = false
                                };
                                field.SetValue(ptr, f);
							}
							break;

						case SaveLoadType.SLDT_OBJECTFLAGS:
							{
								var v = Convert.ToUInt32(value);
                                var f = new ObjectFlags
                                {
                                    used = (v & 0x01) == 0x01, //? true : false;
                                    allocated = (v & 0x02) == 0x02, //? true : false;
                                    isNotOnMap = (v & 0x04) == 0x04, //? true : false;
                                    isSmoking = (v & 0x08) == 0x08, //? true : false;
                                    fireTwiceFlip = (v & 0x10) == 0x10, //? true : false;
                                    animationFlip = (v & 0x20) == 0x20, //? true : false;
                                    bulletIsBig = (v & 0x40) == 0x40, //? true : false;
                                    isWobbling = (v & 0x80) == 0x80, //? true : false;
                                    inTransport = (v & 0x0100) == 0x0100, //? true : false;
                                    byScenario = (v & 0x0200) == 0x0200, //? true : false;
                                    degrades = (v & 0x0400) == 0x0400, //? true : false;
                                    isHighlighted = (v & 0x0800) == 0x0800, //? true : false;
                                    isDirty = (v & 0x1000) == 0x1000, //? true : false;
                                    repairing = (v & 0x2000) == 0x2000, //? true : false;
                                    onHold = (v & 0x4000) == 0x4000, //? true : false;
                                    notused_4_8000 = false,
                                    isUnit = (v & 0x010000) == 0x010000, //? true : false;
                                    upgrading = (v & 0x020000) == 0x020000, //? true : false;
                                    notused_6_0004 = false,
                                    notused_6_0100 = false
                                };
								field.SetValue(ptr, f);
							}
							break;

						case SaveLoadType.SLDT_TEAMFLAGS:
							{
								var f = new TeamFlags
								{
									used = (Convert.ToUInt32(value) & 0x01) == 0x01, //? true : false;
									notused_0002 = false
								};
								field.SetValue(ptr, f);
							}
							break;

						case SaveLoadType.SLDT_SLD:
							subPtr = (field != null && field.FieldType.IsArray) ?
								(field.GetValue(ptr) as Array).GetValue(i) :
								(field != null && field.FieldType.IsClass) ?
								field.GetValue(ptr) :
								ptr;

							if (!SaveLoad_Load(sld[c].sld, fp, subPtr)) return false;
							break;

						case SaveLoadType.SLDT_CALLBACK:
							sld[c].callback(obj, Convert.ToUInt32(value), true);
							break;

						case SaveLoadType.SLDT_INVALID:
							Trace.WriteLine("ERROR: Error in Save/Load structure descriptions");
							return false;
					}
				}

				c++;
			}

			return true;
		}

		/*
		 * Save from a struct to a file.
		 * @param sld The description of the struct.
		 * @param fp The file to write to.
		 * @param object The object instance to write from.
		 * @return True if and only if the writing was successful.
		 */
		internal static bool SaveLoad_Save(SaveLoadDesc[] sld, BinaryWriter fp, object obj)
		{
			var flags = BindingFlags.Instance | BindingFlags.NonPublic;
			var c = 0;
			var type = obj?.GetType();
			FieldInfo field = null;
			string member;
			int index;

			while (sld[c].type_disk != SaveLoadType.SLDT_NULL)
			{
				object/*uint*/ value = 0;
				object[] values;

				member = sld[c].member;
				if (type != null && member != null)
				{
					index = member.IndexOf('.');
					
					field = (index != -1) ?
						type.GetField(member[..index], flags).FieldType.GetField(member[(index + 1)..], flags) :
						type.GetField(member, flags);

					value = (field.FieldType == typeof(string)) ?
						(field.GetValue(obj) as string).PadRight(sld[c].count, '\0') :
						(index != -1) ?
						field.GetValue(type.GetField(member[..index], flags).GetValue(obj)) :
						field.GetValue(obj);
				}
				else
                {
					value = sld[c].getter?.Invoke(); //sld[c].address;
				}

				if (value != null)
                {
					values = sld[c].count == 1 ?
						new object[] { value } :
						((IEnumerable)value).Cast<object>().ToArray();

					if (values[0] is IEnumerable)
						values = values.SelectMany(o => ((IEnumerable)o).Cast<object>()).ToArray();
                }
				else
                {
					values = new object[sld[c].count];
				}

				for (var i = 0; i < sld[c].count; i++)
				{
					//void* ptr = (sld->address == NULL ? ((uint8*)object) + sld->offset : (uint8*)sld->address) + i * sld->size;
					//object ptr = sld[c].address == null ? ((byte[])obj)[(sld[c].offset + i * sld[c].size)..] : ((byte[])sld[c].address)[(i * sld[c].size)..];

					switch (sld[c].type_memory)
					{
						case SaveLoadType.SLDT_NULL:
							values[i] = 0;
							break;

						case SaveLoadType.SLDT_UINT8: //value = ((byte[])ptr)[0];
						case SaveLoadType.SLDT_UINT16: //value = ((ushort[])ptr)[0];
						case SaveLoadType.SLDT_UINT32: //value = ((uint[])ptr)[0];
						case SaveLoadType.SLDT_INT8: //value = (uint)((sbyte[])ptr)[0];
						case SaveLoadType.SLDT_INT16: //value = (uint)((short[])ptr)[0];
						case SaveLoadType.SLDT_INT32: //value = (uint)((int[])ptr)[0];
							break;

						case SaveLoadType.SLDT_HOUSEFLAGS:
							//{
								values[i] = ((HouseFlags)values[i]).all; //ptr;
								//value = (uint)(Convert.ToByte(f.used) | (Convert.ToByte(f.human) << 1) | (Convert.ToByte(f.doneFullScaleAttack) << 2) | (Convert.ToByte(f.isAIActive) << 3) | (Convert.ToByte(f.radarActivated) << 4));
							//}
							break;

						case SaveLoadType.SLDT_OBJECTFLAGS:
							//{
								values[i] = ((ObjectFlags)values[i]).all; //ptr;
								//value = (uint)(Convert.ToByte(f.used) | (Convert.ToByte(f.allocated) << 1) | (Convert.ToByte(f.isNotOnMap) << 2) | (Convert.ToByte(f.isSmoking) << 3) | (Convert.ToByte(f.fireTwiceFlip) << 4) | (Convert.ToByte(f.animationFlip) << 5) | (Convert.ToByte(f.bulletIsBig) << 6) | (Convert.ToByte(f.isWobbling) << 7) | (Convert.ToByte(f.inTransport) << 8) | (Convert.ToByte(f.byScenario) << 9) | (Convert.ToByte(f.degrades) << 10) | (Convert.ToByte(f.isHighlighted) << 11) | (Convert.ToByte(f.isDirty) << 12) | (Convert.ToByte(f.repairing) << 13) | (Convert.ToByte(f.onHold) << 14) | (Convert.ToByte(f.isUnit) << 16) | (Convert.ToByte(f.upgrading) << 17));
							//}
							break;

						case SaveLoadType.SLDT_TEAMFLAGS:
							values[i] = Convert.ToUInt32(((TeamFlags)values[i]).used); //ptr;
							break;

						case SaveLoadType.SLDT_SLD:
							//var subObj = sld[c].address ?? (value as Array).GetValue(i);
							//var subObj = typeof(Array).IsAssignableFrom(value.GetType()) ?
							//	(value as Array).GetValue(i) :
							//	value;
							if (!SaveLoad_Save(sld[c].sld, fp, values[i]/*subObj*//*ptr*/)) return false;
							break;

						case SaveLoadType.SLDT_CALLBACK:
							values[i] = sld[c].callback(obj, 0, false);
							break;

						case SaveLoadType.SLDT_INVALID:
							Trace.WriteLine("ERROR: Error in Save/Load structure descriptions");
							return false;
					}

					switch (sld[c].type_disk)
					{
						case SaveLoadType.SLDT_CALLBACK:
						case SaveLoadType.SLDT_SLD:
						case SaveLoadType.SLDT_NULL:
							break;

						case SaveLoadType.SLDT_UINT8:
							fp.Write(Convert.ToByte(values[i])); //if (fwrite(&v, sizeof(uint8), 1, fp) != 1) return false;
							break;

						case SaveLoadType.SLDT_UINT16:
							fp.Write(Convert.ToUInt16(values[i])); //if (!CFile.fwrite_le_uint16(v, fp)) return false;
							break;

						case SaveLoadType.SLDT_UINT32:
							fp.Write(Convert.ToUInt32(values[i])); //if (!CFile.fwrite_le_uint32(v, fp)) return false;
							break;

						case SaveLoadType.SLDT_INT8:
							sbyte v;
							try
							{
							    v = Convert.ToSByte(values[i]);
							}
							catch (OverflowException)
							{
								v = (sbyte)(ushort)values[i];
							}
							fp.Write(v); //if (fwrite(&v, sizeof(int8), 1, fp) != 1) return false;
							break;

						case SaveLoadType.SLDT_INT16:
							fp.Write(Convert.ToInt16(values[i])); //if (!CFile.fwrite_le_int16(v, fp)) return false;
							break;

						case SaveLoadType.SLDT_INT32:
							fp.Write(Convert.ToInt32(values[i])); //if (!CFile.fwrite_le_int32(v, fp)) return false;
							break;

						default:
						case SaveLoadType.SLDT_INVALID:
							Trace.WriteLine("ERROR: Error in Save/Load structure descriptions");
							return false;
					}
				}

				c++;
			}

			return true;
		}

		/*
		 * Load all Structures from a file.
		 * @param fp The file to load from.
		 * @param length The length of the data chunk.
		 * @return True if and only if all bytes were read successful.
		 */
		internal static bool Structure_Load(BinaryReader fp, uint length)
		{
			while (length > 0)
			{
				//Structure s;
				Structure sl;

				sl = new Structure(); //memset(&sl, 0, sizeof(sl));

				/* Read the next Structure from disk */
				if (!SaveLoad_Load(s_saveStructure, fp, sl)) return false;

				length -= SaveLoad_GetLength(s_saveStructure);

				sl.o.script.scriptInfo = Script.g_scriptStructure;
				//sl.o.script.script.Arr = Script.g_scriptStructure.start.Arr[sl.o.script.script.Ptr..];
				if (sl.upgradeTimeLeft == 0) sl.upgradeTimeLeft = (byte)(CStructure.Structure_IsUpgradable(sl) ? 100 : 0);

				/* Get the Structure from the pool */
				/*s = */CStructure.Structure_Set_ByIndex(sl/*.o.index*/);
				//if (s == null) return false;

				/* Copy over the data */
				//s = sl;
			}
			if (length != 0) return false;

			CStructure.Structure_Recount();

			return true;
		}

		/*
		 * Load all Units from a file.
		 * @param fp The file to load from.
		 * @param length The length of the data chunk.
		 * @return True if and only if all bytes were read successful.
		 */
		internal static bool Unit_Load(BinaryReader fp, uint length)
		{
			while (length > 0)
			{
				//Unit u;
				Unit ul;

				ul = new Unit(); //memset(&ul, 0, sizeof(ul));

				/* Read the next Structure from disk */
				if (!SaveLoad_Load(s_saveUnit, fp, ul)) return false;

				length -= SaveLoad_GetLength(s_saveUnit);

				ul.o.script.scriptInfo = Script.g_scriptUnit;
				//ul.o.script.script.Arr = Script.g_scriptUnit.start.Arr[ul.o.script.script.Ptr..];
				ul.o.script.delay = 0;
				ul.timer = 0;
				ul.o.seenByHouses |= (byte)(1 << ul.o.houseID);

				/* In case the new ODUN chunk is not available, Ordos is always the one who deviated */
				if (ul.deviated != 0) ul.deviatedHouse = (byte)HouseType.HOUSE_ORDOS;

				/* ENHANCEMENT -- Due to wrong parameter orders of Unit_Create in original Dune2,
				 *  it happened that units exists with houseID 13. This in fact are Trikes with
				 *  the wrong houseID. So remove those units completely from the savegame. */
				if (ul.o.houseID == 13) continue;

				/* Get the Structure from the pool */
				/*u = */CUnit.Unit_Set_ByIndex(ul/*.o.index*/);
				//if (u == null) return false;

				/* Copy over the data */
				//u = ul;
			}
			if (length != 0) return false;

			CUnit.Unit_Recount();

			return true;
		}

		/*
		 * Load all new information of Units from a file.
		 * @param fp The file to load from.
		 * @param length The length of the data chunk.
		 * @return True if and only if all bytes were read successful.
		 */
		internal static bool UnitNew_Load(BinaryReader fp, uint length)
		{
			while (length > 0)
			{
				Unit u;
				Object o = new Object();

				/* Read the next index from disk */
				if (!SaveLoad_Load(s_saveUnitNewIndex, fp, o)) return false;

				length -= SaveLoad_GetLength(s_saveUnitNewIndex);

				/* Get the Unit from the pool */
				u = CUnit.Unit_Get_ByIndex(o.index);
				if (u == null) return false;

				/* Read the "new" information for this unit */
				if (!SaveLoad_Load(s_saveUnitNew, fp, u)) return false;

				length -= SaveLoad_GetLength(s_saveUnitNew);
			}
			if (length != 0) return false;

			return true;
		}

		/*
		 * Load all Houses from a file.
		 * @param fp The file to load from.
		 * @param length The length of the data chunk.
		 * @return True if and only if all bytes were read successful.
		 */
		internal static bool House_Load(BinaryReader fp, uint length)
		{
			while (length > 0)
			{
				//House h;
				House hl;

				hl = new House(); //memset(&hl, 0, sizeof(hl));

				/* Read the next House from disk */
				if (!SaveLoad_Load(s_saveHouse, fp, hl)) return false;

				length -= SaveLoad_GetLength(s_saveHouse);

				/* Create the House in the pool */
				/*h = */CHouse.House_Allocate(hl/*.index*/);
				//if (h == null) return false;

				/* Copy over the data */
				//h = hl;

				/* See if it is a human house */
				if (hl.flags.human)
				{
					CHouse.g_playerHouseID = (HouseType)hl.index;
					CHouse.g_playerHouse = hl;

					if (hl.starportLinkedID != 0xFFFF && hl.starportTimeLeft == 0) hl.starportTimeLeft = 1;
				}
			}
			if (length != 0) return false;

			return true;
		}

		/*
		 * Load all Houses from a file.
		 * @param fp The file to load from.
		 * @param length The length of the data chunk.
		 * @return True if and only if all bytes were read successful.
		 */
		internal static bool House_LoadOld(BinaryReader fp, uint length)
		{
			while (length > 0)
			{
				House hl = null;

				/* Read the next House from disk */
				if (!SaveLoad_Load(s_saveHouse, fp, hl)) return false;

				/* See if it is a human house */
				if (hl.flags.human)
				{
					CHouse.g_playerHouseID = (HouseType)hl.index;
					break;
				}

				length -= SaveLoad_GetLength(s_saveHouse);
			}
			if (length == 0) return false;

			return true;
		}

		/*
		 * Load all Teams from a file.
		 * @param fp The file to load from.
		 * @param length The length of the data chunk.
		 * @return True if and only if all bytes were read successful.
		 */
		internal static bool Team_Load(BinaryReader fp, uint length)
		{
			while (length > 0)
			{
				//Team t;
				Team tl;

				tl = new Team(); //memset(&tl, 0, sizeof(tl));

				/* Read the next Structure from disk */
				if (!SaveLoad_Load(s_saveTeam, fp, tl)) return false;

				length -= SaveLoad_GetLength(s_saveTeam);

				tl.script.scriptInfo = Script.g_scriptTeam;
				//tl.script.script.Arr = Script.g_scriptTeam.start.Arr[tl.script.script.Ptr..];

				/* Get the Structure from the pool */
				/*t = */CTeam.Team_Set_ByIndex(tl/*.index*/);
				//if (t == null) return false;

				/* Copy over the data */
				//t = tl;
			}
			if (length != 0) return false;

			CTeam.Team_Recount();

			return true;
		}

		/*
		 * Load all Tiles from a file.
		 * @param fp The file to load from.
		 * @param length The length of the data chunk.
		 * @return True if and only if all bytes were read successful.
		 */
		internal static bool Map_Load(FileStream fp, uint length)
		{
			ushort i;

			for (i = 0; i < 0x1000; i++)
			{
				Tile t = Map.g_map[i];

				t.isUnveiled = false;
				t.overlayTileID = Sprites.g_veiledTileID;
			}

			while (length >= Common.SizeOf(typeof(ushort)) + 4/*Common.SizeOf(typeof(Tile))*/)
			{
				Tile t;

				length -= (uint)(Common.SizeOf(typeof(ushort)) + 4/*Common.SizeOf(typeof(Tile))*/);

				if (!CFile.fread_le_uint16(ref i, fp)) return false;
				if (i >= 0x1000) return false;

				t = Map.g_map[i];
				if (!fread_tile(t, fp)) return false;

				if (Map.g_mapTileID[i] != t.groundTileID)
				{
					Map.g_mapTileID[i] |= 0x8000;
				}
			}
			if (length != 0) return false;

			return true;
		}

		/*
		 * Load a Tile structure to a file (Little endian)
		 *
		 * @param t The tile to read
		 * @param fp The stream
		 * @return True if the tile was loaded successfully
		 */
		static bool fread_tile(Tile t, FileStream fp)
		{
			byte[] buffer = new byte[4];

			if (fp.Read(buffer, 0, 4) != 4) return false; //(fread(buffer, 1, 4, fp) != 4)

			t.groundTileID = (ushort)(buffer[0] | ((buffer[1] & 1) << 8));
			t.overlayTileID = (ushort)(buffer[1] >> 1);
			t.houseID = (byte)(buffer[2] & 0x07);
			t.isUnveiled = (buffer[2] & 0x08) == 0x08; //? true : false;
			t.hasUnit = (buffer[2] & 0x10) == 0x10; //? true : false;
			t.hasStructure = (buffer[2] & 0x20) == 0x20; //? true : false;
			t.hasAnimation = (buffer[2] & 0x40) == 0x40; //? true : false;
			t.hasExplosion = (buffer[2] & 0x80) == 0x80; //? true : false;
			t.index = buffer[3];
			return true;
		}

		/*
		 * Save all Houses to a file.
		 * @param fp The file to save to.
		 * @return True if and only if all bytes were written successful.
		 */
		internal static bool House_Save(BinaryWriter fp)
		{
			PoolFindStruct find = new PoolFindStruct();

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.type = 0xFFFF;
			find.index = 0xFFFF;

			while (true)
			{
				House h;

				h = CHouse.House_Find(find);
				if (h == null) break;

				if (!SaveLoad_Save(s_saveHouse, fp, h)) return false;
			}

			return true;
		}

		/*
		 * Save all Units to a file. It converts pointers to indices where needed.
		 * @param fp The file to save to.
		 * @return True if and only if all bytes were written successful.
		 */
		internal static bool Unit_Save(BinaryWriter fp)
		{
			PoolFindStruct find = new PoolFindStruct();

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.type = 0xFFFF;
			find.index = 0xFFFF;

			while (true)
			{
				Unit u;

				u = CUnit.Unit_Find(find);
				if (u == null) break;

				if (!SaveLoad_Save(s_saveUnit, fp, u)) return false;
			}

			return true;
		}

		/*
		 * Save all new Units information to a file. It converts pointers to indices
		 *   where needed.
		 * @param fp The file to save to.
		 * @return True if and only if all bytes were written successful.
		 */
		internal static bool UnitNew_Save(BinaryWriter fp)
		{
			PoolFindStruct find = new PoolFindStruct();

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.type = 0xFFFF;
			find.index = 0xFFFF;

			while (true)
			{
				Unit u;

				u = CUnit.Unit_Find(find);
				if (u == null) break;

				if (!SaveLoad_Save(s_saveUnitNewIndex, fp, u.o)) return false;
				if (!SaveLoad_Save(s_saveUnitNew, fp, u)) return false;
			}

			return true;
		}

		/*
		 * Save all Structures to a file. It converts pointers to indices where needed.
		 * @param fp The file to save to.
		 * @return True if and only if all bytes were written successful.
		 */
		internal static bool Structure_Save(BinaryWriter fp)
		{
			PoolFindStruct find = new PoolFindStruct();

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.type = 0xFFFF;
			find.index = 0xFFFF;

			while (true)
			{
				Structure s;

				s = CStructure.Structure_Find(find);
				if (s == null) break;

				if (!SaveLoad_Save(s_saveStructure, fp, s)) return false;
			}

			return true;
		}

		/*
		 * Save all Teams to a file. It converts pointers to indices where needed.
		 * @param fp The file to save to.
		 * @return True if and only if all bytes were written successful.
		 */
		internal static bool Team_Save(BinaryWriter fp)
		{
			PoolFindStruct find = new PoolFindStruct();

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.type = 0xFFFF;
			find.index = 0xFFFF;

			while (true)
			{
				Team t;

				t = CTeam.Team_Find(find);
				if (t == null) break;

				if (!SaveLoad_Save(s_saveTeam, fp, t)) return false;
			}

			return true;
		}

		/*
		 * Save all Tiles to a file.
		 * @param fp The file to save to.
		 * @return True if and only if all bytes were written successful.
		 */
		internal static bool Map_Save(BinaryWriter fp)
		{
			ushort i;

			for (i = 0; i < 0x1000; i++)
			{
				Tile tile = Map.g_map[i];

				/* If there is nothing on the tile, not unveiled, and it is equal to the mapseed generated tile, don't store it */
				if (!tile.isUnveiled && !tile.hasStructure && !tile.hasUnit && !tile.hasAnimation && !tile.hasExplosion && (Map.g_mapTileID[i] & 0x8000) == 0 && Map.g_mapTileID[i] == tile.groundTileID) continue;

				/* Store the index, then the tile itself */
				if (!CFile.fwrite_le_uint16(i, fp)) return false;
				if (!fwrite_tile(tile, fp)) return false;
			}

			return true;
		}

		/*
		 * Save a Tile structure to a file (Little endian)
		 *
		 * @param t The tile to save
		 * @param fp The stream
		 * @return True if the tile was saved successfully
		 */
		static bool fwrite_tile(Tile t, BinaryWriter fp)
		{
			byte value = t.houseID;
			if (t.isUnveiled) value |= 1 << 3;
			if (t.hasUnit) value |= 1 << 4;
			if (t.hasStructure) value |= 1 << 5;
			if (t.hasAnimation) value |= 1 << 6;
			if (t.hasExplosion) value |= 1 << 7;

			byte[] buffer = new byte[4];
			buffer[0] = (byte)(t.groundTileID & 0xff);
			buffer[1] = (byte)((t.groundTileID >> 8) | (t.overlayTileID << 1));
			buffer[2] = value;
			buffer[3] = (byte)t.index;

			fp.Write(buffer); //if (fwrite(buffer, 1, 4, fp) != 4) return false;
			
			return true;

			//byte[] buffer = new byte[4];
			//buffer[0] = (byte)(t.groundTileID & 0xff);
			//buffer[1] = (byte)((t.groundTileID >> 8) | (t.overlayTileID << 1));
			//buffer[2] = (byte)(t.houseID | (Convert.ToByte(t.isUnveiled) << 3) | (Convert.ToByte(t.hasUnit) << 4) | (Convert.ToByte(t.hasStructure) << 5) | (Convert.ToByte(t.hasAnimation) << 6) | (Convert.ToByte(t.hasExplosion) << 7));
			//buffer[3] = (byte)t.index;

			//fp.Write(buffer); //if (fwrite(buffer, 1, 4, fp) != 4) return false;

			//return true;
		}
	}
}
