/* Compiler-independent common statements */

namespace SharpDune.Os;

class Common
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

            Type t when t.Name == "SByte" => 1,
            Type t when t.Name == "Byte" => 1,
            Type t when t.Name == "Int16" => 2,
            Type t when t.Name == "UInt16" => 2,
            Type t when t.Name == "Int32" => 4,
            Type t when t.Name == "UInt32" => 4,
            Type t when t.Name == "Int64" => 8,
            Type t when t.Name == "UInt64" => 8,
            Type t when t.Name == "Char" => 2,
            Type t when t.Name == "Single" => 4,
            Type t when t.Name == "Double" => 8,
            Type t when t.Name == "Decimal" => 16,
            Type t when t.Name == "Boolean" => 1,
            Type t when t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Length > 0 =>
                t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Sum(f => SizeOf(f.FieldType)),

            _ => -1
        };
}
