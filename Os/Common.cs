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

    internal static ushort[] FromByteArrayToUshortArray(Span<byte> from)
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
    }

    internal static int SizeOf<T>(T value) =>
        value switch
        {
            sbyte _ => 1,
            byte _ => 1,
            short _ => 2,
            ushort _ => 2,
            int _ => 4,
            uint _ => 4,
            long _ => 8,
            ulong _ => 8,
            char _ => 2,
            float _ => 4,
            double _ => 8,
            decimal _ => 16,
            bool _ => 1,

            Type t when string.Equals(t.Name, "SByte", StringComparison.Ordinal) => 1,
            Type t when string.Equals(t.Name, "Byte", StringComparison.Ordinal) => 1,
            Type t when string.Equals(t.Name, "Int16", StringComparison.Ordinal) => 2,
            Type t when string.Equals(t.Name, "UInt16", StringComparison.Ordinal) => 2,
            Type t when string.Equals(t.Name, "Int32", StringComparison.Ordinal) => 4,
            Type t when string.Equals(t.Name, "UInt32", StringComparison.Ordinal) => 4,
            Type t when string.Equals(t.Name, "Int64", StringComparison.Ordinal) => 8,
            Type t when string.Equals(t.Name, "UInt64", StringComparison.Ordinal) => 8,
            Type t when string.Equals(t.Name, "Char", StringComparison.Ordinal) => 2,
            Type t when string.Equals(t.Name, "Single", StringComparison.Ordinal) => 4,
            Type t when string.Equals(t.Name, "Double", StringComparison.Ordinal) => 8,
            Type t when string.Equals(t.Name, "Decimal", StringComparison.Ordinal) => 16,
            Type t when string.Equals(t.Name, "Boolean", StringComparison.Ordinal) => 1,
            Type t when t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Length > 0 =>
                t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Sum(f => SizeOf(f.FieldType)),

            _ => -1
        };
}
