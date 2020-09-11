/* Os-independent endian detection */

namespace SharpDune
{
	class Endian
	{
		//#define __BYTE_ORDER __LITTLE_ENDIAN
		//#define __LITTLE_ENDIAN 1234

		const int __BYTE_ORDER = 1234;

		static ushort endian_bswap16(ushort x) =>
			(ushort)((x & 0xFF00) >> 8 | (x & 0x00FF) << 8);

		static uint endian_bswap32(uint x) =>
			(x & 0xFF000000) >> 24 | (x & 0x00FF0000) >> 8 | (x & 0x0000FF00) << 8 | (x & 0x000000FF) << 24;

		internal static uint HTOBE32(uint x) =>
			endian_bswap32(x);
		
		internal static uint BETOH32(uint x) =>
			endian_bswap32(x);
		
		ushort HTOBE16(ushort x) =>
			endian_bswap16(x);
		
		internal static ushort BETOH16(ushort x) =>
			endian_bswap16(x);
		
		uint HTOLE32(uint x) =>
			x;
		
		uint LETOH32(uint x) =>
			x;
		
		ushort HTOLE16(ushort x) =>
			x;
		
		ushort LETOH16(ushort x) =>
			x;

		internal static ushort READ_LE_UINT16(byte[] p) =>
			(ushort)(p[0] | (p[1] << 8));

		internal static uint READ_LE_UINT32(byte[] p) =>
			(uint)(p[0] | (p[1] << 8) | (p[2] << 16) | (p[3] << 24));

		internal static uint READ_BE_UINT32(byte[] p) =>
			(uint)((p[0] << 24) | (p[1] << 16) | (p[2] << 8) | p[3]);

		internal static void WRITE_LE_UINT16(byte[] p, ushort value, int i = 0)
		{
			p[i] = (byte)(value & 0xFF);
			p[i + 1] = (byte)((value >> 8) & 0xFF);
		}
	}
}
