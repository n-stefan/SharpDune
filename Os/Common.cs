/* Compiler-independent common statements */

namespace SharpDune.Os;

static class Common
{
    internal static bool AreArraysEqual(Span<byte> array1, int index1, Span<byte> array2, int index2, int count)
    {
        var i = 0;
        while (i < count)
        {
            if (array1[index1 + i] == array2[index2 + i]) i++;
            else return false;
        }
        return true;
    }

    internal static ushort[] ByteArrayToUshortArray(Span<byte> from)
    {
        var i = 0;
        var j = 0;
        var to = new ushort[from.Length / 2];
        while (i < from.Length && j < to.Length)
        {
            to[j] = (ushort)(from[i] | (from[i + 1] << 8));
            i += 2;
            j++;
        }
        return to;

        //var to = new ushort[from.Length / 2];
        //Buffer.BlockCopy(from.ToArray(), 0, to, 0, from.Length);
        //return to;
    }

    internal static string UshortArrayToString(ushort[] array, int offset)
    {
        var index = Array.FindIndex(array, offset, x => x == '\0');
        var bytes = new byte[(index - offset) * sizeof(ushort)];
        Buffer.BlockCopy(array, offset, bytes, 0, bytes.Length);
        return SharpDune.Encoding.GetString(bytes);
    }

    internal static int SizeOf<T>(T value) =>
        value switch
        {
            Type t when string.Equals(t.Name, "SByte", StringComparison.Ordinal) => sizeof(sbyte),
            Type t when string.Equals(t.Name, "Byte", StringComparison.Ordinal) => sizeof(byte),
            Type t when string.Equals(t.Name, "Int16", StringComparison.Ordinal) => sizeof(short),
            Type t when string.Equals(t.Name, "UInt16", StringComparison.Ordinal) => sizeof(ushort),
            Type t when string.Equals(t.Name, "Int32", StringComparison.Ordinal) => sizeof(int),
            Type t when string.Equals(t.Name, "UInt32", StringComparison.Ordinal) => sizeof(uint),
            Type t when string.Equals(t.Name, "Int64", StringComparison.Ordinal) => sizeof(long),
            Type t when string.Equals(t.Name, "UInt64", StringComparison.Ordinal) => sizeof(ulong),
            Type t when string.Equals(t.Name, "Char", StringComparison.Ordinal) => sizeof(char),
            Type t when string.Equals(t.Name, "Single", StringComparison.Ordinal) => sizeof(float),
            Type t when string.Equals(t.Name, "Double", StringComparison.Ordinal) => sizeof(double),
            Type t when string.Equals(t.Name, "Decimal", StringComparison.Ordinal) => sizeof(decimal),
            Type t when string.Equals(t.Name, "Boolean", StringComparison.Ordinal) => sizeof(bool),
            Type t when t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Length > 0 =>
                t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Sum(f => SizeOf(f.FieldType)),

            _ => -1
        };
}
