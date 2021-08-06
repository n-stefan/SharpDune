/* Decoder for 'format40' files */

namespace SharpDune.Codec
{
	class Format40
	{
		/*
		 * Decode a memory fragment which is encoded with 'format40'.
		 * @param dst The place the decoded fragment will be loaded.
		 * @param src The encoded fragment.
		 */
		internal static void Format40_Decode(Array<byte> dst, Array<byte> src)
		{
			ushort cmd;
			ushort count;

			for (; ; )
			{
				cmd = src.Arr[src.Ptr++];   /* 8 bit command code */

				if (cmd == 0)
				{
					/* XOR with value */
					for (count = src.Arr[src.Ptr++]; count > 0; count--)
					{
						/*dst.CurrInc*/dst.Arr[dst.Ptr++] ^= src.Curr; //src.Arr[src.Ptr];
					}
					src++; //src.Ptr++;
				}
				else if ((cmd & 0x80) == 0)
				{
					/* XOR with string */
					for (count = cmd; count > 0; count--)
					{
						/*dst.CurrInc*/dst.Arr[dst.Ptr++] ^= src.Arr[src.Ptr++];
					}
				}
				else if (cmd != 0x80)
				{
					/* skip bytes */
					dst += cmd & 0x7F; //dst.Ptr += cmd & 0x7F;
				}
				else
				{
					/* last byte was 0x80 : read 16 bit value */
					cmd = src.Arr[src.Ptr++];
					cmd += (ushort)((src.Arr[src.Ptr++]) << 8);

					if (cmd == 0)
						break;    /* 0x80 0x00 0x00 => exit code */

					if ((cmd & 0x8000) == 0)
					{
						/* skip bytes */
						dst += cmd; //dst.Ptr += cmd;
					}
					else if ((cmd & 0x4000) == 0)
					{
						/* XOR with string */
						for (count = (ushort)(cmd & 0x3FFF); count > 0; count--)
						{
							/*dst.CurrInc*/dst.Arr[dst.Ptr++] ^= src.Arr[src.Ptr++];
						}
					}
					else
					{
						/* XOR with value */
						for (count = (ushort)(cmd & 0x3FFF); count > 0; count--)
						{
							/*dst.CurrInc*/dst.Arr[dst.Ptr++] ^= src.Curr; //src.Arr[src.Ptr];
						}
						src++; //src.Ptr++;
					}
				}
			}
		}

		/*
		 * Copy a rectangle from a format40 compressed data source to the screen.
		 * @param base Base of the rectangle (top-left pixel).
		 * @param src Data source.
		 * @param width Width of the rectangle.
		 */
		internal static void Format40_Decode_ToScreen(byte[] dst, byte[] src, ushort width, int dstPointer/* = 0*/, int srcPointer/* = 0*/)
		{
			ushort length;
			ushort cmd;
			ushort count;
			//int srcPointer = 0;
			//int dstPointer = 0;

			length = 0;

			for (; ; )
			{
				cmd = src[srcPointer++];    /* 8 bit command code */

				if (cmd == 0)
				{
					/* fill with value */
					for (count = src[srcPointer++]; count > 0; count--)
					{
						dst[dstPointer++] = src[srcPointer];
						length++;
						if (length == width)
						{
							length = 0;
							dstPointer += SCREEN_WIDTH - width;
						}
					}
					srcPointer++;
				}
				else if ((cmd & 0x80) == 0)
				{
					/* copy string */
					for (count = (ushort)(cmd & 0x7F); count > 0; count--)
					{
						dst[dstPointer++] = src[srcPointer++];
						length++;
						if (length == width)
						{
							length = 0;
							dstPointer += SCREEN_WIDTH - width;
						}
					}
				}
				else if (cmd != 0x80)
				{
					/* skip bytes */
					dstPointer += cmd & 0x7F;
					length += (ushort)(cmd & 0x7F);
					while (length >= width)
					{
						length -= width;
						dstPointer += SCREEN_WIDTH - width;
					}
				}
				else
				{
					/* last byte was 0x80 : read 16 bit value */
					cmd = src[srcPointer++];
					cmd += (ushort)((src[srcPointer++]) << 8);

					if (cmd == 0) break;    /* 0x80 0x00 0x00 => exit code */

					if ((cmd & 0x8000) == 0)
					{
						/* skip bytes */
						dstPointer += cmd;
						length += cmd;
						while (length >= width)
						{
							length -= width;
							dstPointer += SCREEN_WIDTH - width;
						}
					}
					else if ((cmd & 0x4000) == 0)
					{
						/* copy string */
						for (count = (ushort)(cmd & 0x3FFF); count > 0; count--)
						{
							dst[dstPointer++] = src[srcPointer++];
							length++;
							if (length == width)
							{
								length = 0;
								dstPointer += SCREEN_WIDTH - width;
							}
						}
					}
					else
					{
						/* fill with value */
						for (count = (ushort)(cmd & 0x3FFF); count > 0; count--)
						{
							dst[dstPointer++] = src[srcPointer];
							length++;
							if (length == width)
							{
								length = 0;
								dstPointer += SCREEN_WIDTH - width;
							}
						}
						srcPointer++;
					}
				}
			}
		}

		/*
		 * Xor a rectangle from a format40 compressed data source to the screen.
		 * @param base Base of the rectangle (top-left pixel).
		 * @param src Data source.
		 * @param width Width of the rectangle.
		 */
		internal static void Format40_Decode_XorToScreen(byte[] dst, byte[] src, ushort width, int dstPointer/* = 0*/, int srcPointer/* = 0*/)
		{
			ushort length;
			ushort cmd;
			ushort count;
			//int srcPointer = 0;
			//int dstPointer = 0;

			length = 0;

			for (; ; )
			{
				cmd = src[srcPointer++];    /* 8 bit command code */

				if (cmd == 0)
				{
					/* XOR with value */
					for (count = src[srcPointer++]; count > 0; count--)
					{
						dst[dstPointer++] ^= src[srcPointer];
						length++;
						if (length == width)
						{
							length = 0;
							dstPointer += SCREEN_WIDTH - width;
						}
					}
					srcPointer++;
				}
				else if ((cmd & 0x80) == 0)
				{
					/* XOR with string */
					for (count = cmd; count > 0; count--)
					{
						dst[dstPointer++] ^= src[srcPointer++];
						length++;
						if (length == width)
						{
							length = 0;
							dstPointer += SCREEN_WIDTH - width;
						}
					}
				}
				else if (cmd != 0x80)
				{
					/* skip bytes */
					dstPointer += cmd & 0x7F;
					length += (ushort)(cmd & 0x7F);
					while (length >= width)
					{
						length -= width;
						dstPointer += SCREEN_WIDTH - width;
					}
				}
				else
				{
					/* last byte was 0x80 : read 16 bit value */
					cmd = src[srcPointer++];
					cmd += (ushort)((src[srcPointer++]) << 8);

					if (cmd == 0) break;    /* 0x80 0x00 0x00 => exit code */

					if ((cmd & 0x8000) == 0)
					{
						/* skip bytes */
						dstPointer += cmd;
						length += cmd;
						while (length >= width)
						{
							length -= width;
							dstPointer += SCREEN_WIDTH - width;
						}
					}
					else if ((cmd & 0x4000) == 0)
					{
						/* XOR with string */
						for (count = (ushort)(cmd & 0x3FFF); count > 0; count--)
						{
							dst[dstPointer++] ^= src[srcPointer++];
							length++;
							if (length == width)
							{
								length = 0;
								dstPointer += SCREEN_WIDTH - width;
							}
						}
					}
					else
					{
						/* XOR with value */
						for (count = (ushort)(cmd & 0x3FFF); count > 0; count--)
						{
							dst[dstPointer++] ^= src[srcPointer];
							length++;
							if (length == width)
							{
								length = 0;
								dstPointer += SCREEN_WIDTH - width;
							}
						}
						srcPointer++;
					}
				}
			}
		}
	}
}
