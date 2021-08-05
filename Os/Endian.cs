/* Os-independent endian detection */

namespace SharpDune.Os
{
	class Endian
	{
        static ushort Endian_BSwap16(ushort x) =>
			(ushort)((x & 0xFF00) >> 8 | (x & 0x00FF) << 8);

		static uint Endian_BSwap32(uint x) =>
			(x & 0xFF000000) >> 24 | (x & 0x00FF0000) >> 8 | (x & 0x0000FF00) << 8 | (x & 0x000000FF) << 24;

		internal static uint HToBE32(uint x) =>
			Endian_BSwap32(x);
		
		internal static uint BEToH32(uint x) =>
			Endian_BSwap32(x);
		
		internal static ushort BEToH16(ushort x) =>
			Endian_BSwap16(x);

		internal static ushort Read_LE_UInt16(byte[] p) =>
			(ushort)(p[0] | (p[1] << 8));

		internal static uint Read_LE_UInt32(byte[] p) =>
			(uint)(p[0] | (p[1] << 8) | (p[2] << 16) | (p[3] << 24));

		internal static uint Read_BE_UInt32(byte[] p) =>
			(uint)((p[0] << 24) | (p[1] << 16) | (p[2] << 8) | p[3]);

		internal static void Write_LE_UInt16(byte[] p, ushort value, int i = 0)
		{
			p[i] = (byte)(value & 0xFF);
			p[i + 1] = (byte)((value >> 8) & 0xFF);
		}
	}
}
