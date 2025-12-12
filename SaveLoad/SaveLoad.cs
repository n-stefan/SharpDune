/* Save Load */

namespace SharpDune.SaveLoad;

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
    internal SaveLoadType type_disk;                                 /*!< The type it is on disk. */
    internal SaveLoadType type_memory;                               /*!< The type it is in memory. */
    internal ushort count;                                           /*!< The number of elements */
    internal SaveLoadDesc[] sld;                                     /*!< The SaveLoadDesc. */
    internal SaveLoadDescCallback callback;                          /*!< The custom callback. */
    internal string member;
    internal SaveLoadDescGetter getter;
    internal SaveLoadDescSetter setter;
    //internal int offset;                                           /*!< The offset in the object, in bytes. */
    //internal int size;                                             /*!< The size of an element. */
    //internal object address;                                       /*!< The address of the element. */
}

static class SaveLoad
{
    //static int offset(Type c, string m) //(((size_t)&((c *)8)->m) - 8)
    //{
    //	var index = m.IndexOf('.');
    //	var handle = (index != -1) ?
    //		c.GetField(m[..index], BindingFlags.Instance | BindingFlags.NonPublic).FieldType.GetField(m[(index + 1)..], BindingFlags.Instance | BindingFlags.NonPublic).FieldHandle :
    //		c.GetField(m, BindingFlags.Instance | BindingFlags.NonPublic).FieldHandle;
    //	var offset = Marshal.ReadInt32(handle.Value + (4 + nint.Size)) & 0xFFFFFF;
    //	return offset;
    //}

    //static int item_size(Type c, string m) //sizeof(((c *)0)->m)
    //{
    //	var size = Common.SizeOf(c.GetField(m, BindingFlags.Instance | BindingFlags.NonPublic).FieldType);
    //	return size;
    //}

    //internal static SaveLoadDesc SLD_GENTRY<M>(SaveLoadType t, M m, SaveLoadDescSetter s) =>
    //	new SaveLoadDesc { /*offset = 0,*/ type_disk = t, type_memory = t, count = 1, sld = null, /*size = Common.SizeOf(m),*/ callback = null, address = m, setter = s };

    //internal static SaveLoadDesc SLD_GENTRY2<M>(SaveLoadType t, M m, SaveLoadType t2, SaveLoadDescSetter s) =>
    //	new SaveLoadDesc { /*offset = 0,*/ type_disk = t, type_memory = t2, count = 1, sld = null, /*size = Common.SizeOf(m),*/ callback = null, address = m, setter = s };

    //internal static SaveLoadDesc SLD_GARRAY<M>(SaveLoadType t, M m, ushort n) =>
    //	new SaveLoadDesc { /*offset = 0,*/ type_disk = t, type_memory = t, count = n, sld = null, /*size = Common.SizeOf(m) / n,*/ callback = null, address = m };

    //internal static SaveLoadDesc SLD_GCALLB<M>(SaveLoadType t, M m, SaveLoadDescCallback p) =>
    //	new SaveLoadDesc { /*offset = 0,*/ type_disk = t, type_memory = SaveLoadType.SLDT_CALLBACK, count = 1, sld = null, /*size = Common.SizeOf(m),*/ callback = p, address = m };

    //internal static SaveLoadDesc SLD_GSLD<M>(M m, SaveLoadDesc[] s) =>
    //	new SaveLoadDesc { /*offset = 0,*/ type_disk = SaveLoadType.SLDT_SLD, type_memory = SaveLoadType.SLDT_SLD, count = 1, sld = s, /*size = Common.SizeOf(m),*/ callback = null, address = m };

    /*
     * An empty entry. Just to pad bytes on disk.
     * @param t The type on disk.
     */
    internal static SaveLoadDesc SLD_EMPTY(SaveLoadType t) =>
        new() { /*offset = 0,*/ type_disk = t, type_memory = SLDT_NULL, count = 1, sld = null, /*size = 0,*/ callback = null };

    /*
     * An empty array. Just to pad bytes on disk.
     * @param t The type on disk.
     * @param n The number of elements.
     */
    internal static SaveLoadDesc SLD_EMPTY2(SaveLoadType t, ushort n) =>
        new() { /*offset = 0,*/ type_disk = t, type_memory = SLDT_NULL, count = n, sld = null, /*size = 0,*/ callback = null };

    /*
     * A normal entry.
     * @param c The class.
     * @param t The type on disk / in memory.
     * @param m The member of the class.
     */
    internal static SaveLoadDesc SLD_ENTRY(/*Type c,*/ SaveLoadType t, string m) =>
        new() { member = m, /*offset = offset(c, m),*/ type_disk = t, type_memory = t, count = 1, sld = null, /*size = item_size(c, m),*/ callback = null };

    internal static SaveLoadDesc SLD_GENTRY(SaveLoadType t, SaveLoadDescGetter g, SaveLoadDescSetter s) =>
        new() { /*offset = 0,*/ type_disk = t, type_memory = t, count = 1, sld = null, /*size = Common.SizeOf(m),*/ callback = null, getter = g, setter = s };

    /*
     * A full entry.
     * @param c The class.
     * @param t The type on disk.
     * @param m The member of the class.
     * @param t2 The type in memory.
     */
    internal static SaveLoadDesc SLD_ENTRY2(/*Type c,*/ SaveLoadType t, string m, SaveLoadType t2) =>
        new() { member = m, /*offset = offset(c, m),*/ type_disk = t, type_memory = t2, count = 1, sld = null, /*size = item_size(c, m),*/ callback = null };

    internal static SaveLoadDesc SLD_GENTRY2(SaveLoadType t, SaveLoadType t2, SaveLoadDescGetter g, SaveLoadDescSetter s) =>
        new() { /*offset = 0,*/ type_disk = t, type_memory = t2, count = 1, sld = null, /*size = Common.SizeOf(m),*/ callback = null, getter = g, setter = s };

    /*
     * A normal array.
     * @param c The class.
     * @param t The type on disk / in memory.
     * @param m The member of the class.
     * @param n The number of elements.
     */
    internal static SaveLoadDesc SLD_ARRAY(/*Type c,*/ SaveLoadType t, string m, ushort n) =>
        new() { member = m, /*offset = offset(c, m),*/ type_disk = t, type_memory = t, count = n, sld = null, /*size = item_size(c, m) / n,*/ callback = null };

    internal static SaveLoadDesc SLD_GARRAY(SaveLoadType t, SaveLoadDescGetter g, SaveLoadDescSetter s, ushort n) =>
        new() { /*offset = 0,*/ type_disk = t, type_memory = t, count = n, sld = null, /*size = Common.SizeOf(m) / n,*/ callback = null, getter = g, setter = s };

    /*
     * A callback entry.
     * @param c The class.
     * @param t The type on disk.
     * @param m The member of the class.
     * @param p The callback.
     */
    internal static SaveLoadDesc SLD_CALLB(/*Type c,*/ SaveLoadType t, string m, SaveLoadDescCallback p) =>
        new() { member = m, /*offset = offset(c, m),*/ type_disk = t, type_memory = SLDT_CALLBACK, count = 1, sld = null, /*size = item_size(c, m),*/ callback = p };

    internal static SaveLoadDesc SLD_GCALLB(SaveLoadType t, SaveLoadDescGetter g, SaveLoadDescCallback p) =>
        new() { /*offset = 0,*/ type_disk = t, type_memory = SLDT_CALLBACK, count = 1, sld = null, /*size = Common.SizeOf(m),*/ callback = p, getter = g };

    /* Indicates end of array. */
    internal static SaveLoadDesc SLD_END() =>
        new() { /*offset = 0,*/ type_disk = SLDT_NULL, type_memory = SLDT_NULL, count = 0, sld = null, /*size = 0,*/ callback = null };

    /*
     * A struct entry.
     * @param c The class.
     * @param m The member of the class.
     * @param s The SaveLoadDesc.
     */
    internal static SaveLoadDesc SLD_SLD(/*Type c,*/ string m, SaveLoadDesc[] s) =>
        new() { member = m, /*offset = offset(c, m),*/ type_disk = SLDT_SLD, type_memory = SLDT_SLD, count = 1, sld = s, /*size = item_size(c, m),*/ callback = null };

    internal static SaveLoadDesc SLD_GSLD(SaveLoadDescGetter g, SaveLoadDesc[] s) =>
        new() { /*offset = 0,*/ type_disk = SLDT_SLD, type_memory = SLDT_SLD, count = 1, sld = s, /*size = Common.SizeOf(m),*/ callback = null, getter = g };

    /*
     * A struct array.
     * @param c The class.
     * @param m The member of the class.
     * @param s The SaveLoadDesc.
     * @param n The number of elements.
     */
    internal static SaveLoadDesc SLD_SLD2(/*Type c,*/ string m, SaveLoadDesc[] s, ushort n) =>
        new() { member = m, /*offset = offset(c, m),*/ type_disk = SLDT_SLD, type_memory = SLDT_SLD, count = n, sld = s, /*size = item_size(c, m) / n,*/ callback = null };

    /*
     * Get the length of the struct how it would be on disk.
     * @param sld The description of the struct.
     * @return The length of the struct on disk.
     */
    internal static uint SaveLoad_GetLength(SaveLoadDesc[] sld)
    {
        uint length = 0;
        var i = 0;

        while (sld[i].type_disk != SLDT_NULL)
        {
            length += sld[i].type_disk switch
            {
                SLDT_NULL => 0,
                SLDT_CALLBACK => 0,
                SLDT_UINT8 => (uint)sizeof(byte) * sld[i].count,
                SLDT_UINT16 => (uint)sizeof(ushort) * sld[i].count,
                SLDT_UINT32 => (uint)sizeof(uint) * sld[i].count,
                SLDT_INT8 => (uint)sizeof(sbyte) * sld[i].count,
                SLDT_INT16 => (uint)sizeof(short) * sld[i].count,
                SLDT_INT32 => (uint)sizeof(int) * sld[i].count,
                SLDT_SLD => SaveLoad_GetLength(sld[i].sld) * sld[i].count,
                _ => 0
            };
            i++;
        }

        return length;
    }

    private static void FillArray(Array array, ArrayList values)
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
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var type = obj?.GetType();
        var sb = new StringBuilder();
        var values = new ArrayList();
        FieldInfo field = null;
        int c = 0, index = -1;
        ReadOnlySpan<char> member;
        object value, ptr, subPtr;

        while (sld[c].type_disk != SLDT_NULL)
        {
            value = 0;
            ptr = null;

            member = sld[c].member;
            if (type != null && member != default)
            {
                index = member.IndexOf('.');

                field = (index != -1) ?
                    type.GetField(member.Slice(0, index).ToString(), flags).FieldType.GetField(member.Slice(index + 1).ToString(), flags) :
                    type.GetField(member.ToString(), flags);

                ptr = (index != -1) ?
                    type.GetField(member.Slice(0, index).ToString(), flags).GetValue(obj) :
                    obj;
            }
            else
            {
                ptr = sld[c].getter?.Invoke(); //sld[c].address;
            }

            for (var i = 0; i < sld[c].count; i++)
            {
                //void* ptr = (sld->address == NULL ? ((uint8*)object) + sld->offset : (uint8*)sld->address) + i * sld->size;

                switch (sld[c].type_disk)
                {
                    case SLDT_CALLBACK:
                    case SLDT_SLD:
                    case SLDT_NULL:
                        value = 0;
                        break;

                    case SLDT_UINT8:
                        value = fp.ReadByte(); //if (fread(&v, sizeof(uint8), 1, fp) != 1) return false;
                        break;

                    case SLDT_UINT16:
                        value = fp.ReadUInt16(); //if (!CFile.fread_le_uint16(ref v, (FileStream)fp.BaseStream)) return false;
                        break;

                    case SLDT_UINT32:
                        value = fp.ReadUInt32(); //if (!CFile.fread_le_uint32(ref v, (FileStream)fp.BaseStream)) return false;
                        break;

                    case SLDT_INT8:
                        value = fp.ReadSByte(); //if (fread(&v, sizeof(int8), 1, fp) != 1) return false;
                        break;

                    case SLDT_INT16:
                        value = fp.ReadInt16(); //if (!CFile.fread_le_int16(ref v, (FileStream)fp.BaseStream)) return false;
                        break;

                    case SLDT_INT32:
                        value = fp.ReadInt32(); //if (!CFile.fread_le_int32(ref v, (FileStream)fp.BaseStream)) return false;
                        break;

                    case SLDT_INVALID:
                    default:
                        Trace.WriteLine("ERROR: Error in Save/Load structure descriptions");
                        return false;
                }

                switch (sld[c].type_memory)
                {
                    case SLDT_NULL:
                        break;

                    case SLDT_UINT8:
                    case SLDT_UINT16:
                    case SLDT_UINT32:
                    case SLDT_INT8:
                    case SLDT_INT16:
                    case SLDT_INT32:
                        if (field != null)
                        {
                            if (field.FieldType == typeof(string))
                            {
                                sb.Append(Convert.ToChar(value, Culture));
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
                                field.SetValue(ptr, (sld[c].type_memory == SLDT_UINT8) ? Convert.ToByte(value, Culture) : value);
                                if (ptr is ValueType && index != -1)
                                {
                                    type.GetField(member.Slice(0, index).ToString(), flags).SetValue(obj, ptr);
                                }
                            }
                        }
                        else
                        {
                            sld[c].setter?.Invoke(value, i);
                        }
                        break;

                    case SLDT_HOUSEFLAGS:
                        {
                            var v = Convert.ToUInt32(value, Culture);
                            var f = new HouseFlags
                            {
                                used = (v & 0x01) == 0x01,
                                human = (v & 0x02) == 0x02,
                                doneFullScaleAttack = (v & 0x04) == 0x04,
                                isAIActive = (v & 0x08) == 0x08,
                                radarActivated = (v & 0x10) == 0x10,
                                unused_0020 = false
                            };
                            field.SetValue(ptr, f);
                        }
                        break;

                    case SLDT_OBJECTFLAGS:
                        {
                            var v = Convert.ToUInt32(value, Culture);
                            var f = new ObjectFlags
                            {
                                used = (v & 0x01) == 0x01,
                                allocated = (v & 0x02) == 0x02,
                                isNotOnMap = (v & 0x04) == 0x04,
                                isSmoking = (v & 0x08) == 0x08,
                                fireTwiceFlip = (v & 0x10) == 0x10,
                                animationFlip = (v & 0x20) == 0x20,
                                bulletIsBig = (v & 0x40) == 0x40,
                                isWobbling = (v & 0x80) == 0x80,
                                inTransport = (v & 0x0100) == 0x0100,
                                byScenario = (v & 0x0200) == 0x0200,
                                degrades = (v & 0x0400) == 0x0400,
                                isHighlighted = (v & 0x0800) == 0x0800,
                                isDirty = (v & 0x1000) == 0x1000,
                                repairing = (v & 0x2000) == 0x2000,
                                onHold = (v & 0x4000) == 0x4000,
                                notused_4_8000 = false,
                                isUnit = (v & 0x010000) == 0x010000,
                                upgrading = (v & 0x020000) == 0x020000,
                                notused_6_0004 = false,
                                notused_6_0100 = false
                            };
                            field.SetValue(ptr, f);
                        }
                        break;

                    case SLDT_TEAMFLAGS:
                        {
                            var f = new TeamFlags
                            {
                                used = (Convert.ToUInt32(value, Culture) & 0x01) == 0x01,
                                notused_0002 = false
                            };
                            field.SetValue(ptr, f);
                        }
                        break;

                    case SLDT_SLD:
                        subPtr = (field != null && field.FieldType.IsArray) ?
                            (field.GetValue(ptr) as Array).GetValue(i) :
                            (field != null && field.FieldType.IsClass) ?
                            field.GetValue(ptr) :
                            ptr;

                        if (!SaveLoad_Load(sld[c].sld, fp, subPtr)) return false;
                        break;

                    case SLDT_CALLBACK:
                        sld[c].callback(obj, Convert.ToUInt32(value, Culture), true);
                        break;

                    case SLDT_INVALID:
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
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var type = obj?.GetType();
        FieldInfo field = null;
        int c = 0, index;
        ReadOnlySpan<char> member;
        object value;
        object[] values;

        while (sld[c].type_disk != SLDT_NULL)
        {
            value = 0;

            member = sld[c].member;
            if (type != null && member != default)
            {
                index = member.IndexOf('.');

                field = (index != -1) ?
                    type.GetField(member.Slice(0, index).ToString(), flags).FieldType.GetField(member.Slice(index + 1).ToString(), flags) :
                    type.GetField(member.ToString(), flags);

                value = (field.FieldType == typeof(string)) ?
                    (field.GetValue(obj) as string).PadRight(sld[c].count, '\0') :
                    (index != -1) ?
                    field.GetValue(type.GetField(member.Slice(0, index).ToString(), flags).GetValue(obj)) :
                    field.GetValue(obj);
            }
            else
            {
                value = sld[c].getter?.Invoke(); //sld[c].address;
            }

            if (value != null)
            {
                values = sld[c].count == 1 ?
                    [value] :
                    [.. ((IEnumerable)value).Cast<object>()];

                if (values[0] is IEnumerable)
                    values = [.. values.SelectMany(o => ((IEnumerable)o).Cast<object>())];
            }
            else
            {
                values = new object[sld[c].count];
            }

            for (var i = 0; i < sld[c].count; i++)
            {
                //void* ptr = (sld->address == NULL ? ((uint8*)object) + sld->offset : (uint8*)sld->address) + i * sld->size;

                switch (sld[c].type_memory)
                {
                    case SLDT_NULL:
                        values[i] = 0;
                        break;

                    case SLDT_UINT8:
                    case SLDT_UINT16:
                    case SLDT_UINT32:
                    case SLDT_INT8:
                    case SLDT_INT16:
                    case SLDT_INT32:
                        break;

                    case SLDT_HOUSEFLAGS:
                        values[i] = ((HouseFlags)values[i]).All; //ptr;
                        break;

                    case SLDT_OBJECTFLAGS:
                        values[i] = ((ObjectFlags)values[i]).All; //ptr;
                        break;

                    case SLDT_TEAMFLAGS:
                        values[i] = Convert.ToUInt32(((TeamFlags)values[i]).used); //ptr;
                        break;

                    case SLDT_SLD:
                        //var subObj = sld[c].address ?? (value as Array).GetValue(i);
                        //var subObj = typeof(Array).IsAssignableFrom(value.GetType()) ?
                        //	(value as Array).GetValue(i) :
                        //	value;
                        if (!SaveLoad_Save(sld[c].sld, fp, values[i]/*subObj*//*ptr*/)) return false;
                        break;

                    case SLDT_CALLBACK:
                        values[i] = sld[c].callback(obj, 0, false);
                        break;

                    case SLDT_INVALID:
                        Trace.WriteLine("ERROR: Error in Save/Load structure descriptions");
                        return false;
                }

                switch (sld[c].type_disk)
                {
                    case SLDT_CALLBACK:
                    case SLDT_SLD:
                    case SLDT_NULL:
                        break;

                    case SLDT_UINT8:
                        try
                        {
                            fp.Write(Convert.ToByte(values[i], Culture)); //if (fwrite(&v, sizeof(uint8), 1, fp) != 1) return false;
                        }
                        catch (OverflowException)
                        {
                            fp.Write((byte)(ushort)values[i]);
                        }
                        break;

                    case SLDT_UINT16:
                        fp.Write(Convert.ToUInt16(values[i], Culture)); //if (!CFile.fwrite_le_uint16(v, fp)) return false;
                        break;

                    case SLDT_UINT32:
                        fp.Write(Convert.ToUInt32(values[i], Culture)); //if (!CFile.fwrite_le_uint32(v, fp)) return false;
                        break;

                    case SLDT_INT8:
                        try
                        {
                            fp.Write(Convert.ToSByte(values[i], Culture)); //if (fwrite(&v, sizeof(int8), 1, fp) != 1) return false;
                        }
                        catch (OverflowException)
                        {
                            fp.Write((sbyte)(ushort)values[i]);
                        }
                        break;

                    case SLDT_INT16:
                        fp.Write(Convert.ToInt16(values[i], Culture)); //if (!CFile.fwrite_le_int16(v, fp)) return false;
                        break;

                    case SLDT_INT32:
                        fp.Write(Convert.ToInt32(values[i], Culture)); //if (!CFile.fwrite_le_int32(v, fp)) return false;
                        break;

                    case SLDT_INVALID:
                    default:
                        Trace.WriteLine("ERROR: Error in Save/Load structure descriptions");
                        return false;
                }
            }

            c++;
        }

        return true;
    }
}
