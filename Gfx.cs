/* Graphics */

using System;
using System.Diagnostics;

namespace SharpDune
{
	enum Screen
	{
		SCREEN_0 = 0,
		SCREEN_1 = 1,
		SCREEN_2 = 2,
		SCREEN_3 = 3,
		SCREEN_ACTIVE = -1
	}

	class dirty_area
	{
		internal ushort left;
		internal ushort top;
		internal ushort right;
		internal ushort bottom;
	}

	class Gfx
	{
		static bool s_screen0_is_dirty = false;
		static dirty_area s_screen0_dirty_area = new dirty_area { left = 0, top = 0, right = 0, bottom = 0 };
		static uint[] g_dirty_blocks = new uint[200];

		static Screen s_screenActiveID = Screen.SCREEN_0;

		const byte GFX_SCREEN_BUFFER_COUNT = 4;
		static ushort[] s_screenBufferSize = { /*0xFA00*/0xFF00, 0xFBF4, 0xFA00, 0xFD0D/*, 0xA044*/ };
		//TODO: Use one CArray<byte>
		static byte[] s_screen1;
		static byte[] s_screen2;
		static byte[] s_screen3;

		internal const ushort SCREEN_WIDTH = 320;   /*!< Width of the screen in pixels. */
		internal const ushort SCREEN_HEIGHT = 200;  /*!< Height of the screen in pixels. */

		internal static byte[] g_paletteActive = new byte[256 * 3];
		internal static byte[] g_palette1;
		internal static byte[] g_palette2;
		internal static byte[] g_paletteMapping1;
		internal static byte[] g_paletteMapping2;

		static ushort s_tileSpacing = 0;    /* bytes to skip between each line. == SCREEN_WIDTH - 2*s_tileWidth */
		static ushort s_tileHeight = 0;     /* "icon" sprites height (lines) */
		static ushort s_tileWidth = 0;      /* "icon" sprites width in bytes. each bytes contains 2 pixels. 4 MSB = left, 4 LSB = right */
		static byte s_tileMode = 0;
		static byte s_tileByteSize = 0;     /* size in byte of one sprite pixel data = s_tileHeight * s_tileWidth / 2 */

		internal static ushort GFX_GetSize(short width, short height)
		{
			if (width < 1) width = 1;
			if (width > SCREEN_WIDTH) width = (short)SCREEN_WIDTH;
			if (height < 1) height = 1;
			if (height > SCREEN_HEIGHT) height = (short)SCREEN_HEIGHT;

			return (ushort)(width * height);
		}

		/*
		 * Get the pointer to a screenbuffer.
		 * @param screenID The screenbuffer to get.
		 * @return A pointer to the screenbuffer.
		 */
		internal static /*byte[]*/object GFX_Screen_Get_ByIndex(Screen screenID)
		{
			if (screenID == Screen.SCREEN_ACTIVE)
				screenID = s_screenActiveID;
			Debug.Assert(screenID >= 0 && (byte)screenID < GFX_SCREEN_BUFFER_COUNT);
			switch (screenID)
            {
				case Screen.SCREEN_0:
					return Sdl2Video.Video_GetFrameBuffer();
				case Screen.SCREEN_1:
					return s_screen1;
				case Screen.SCREEN_2:
					return s_screen2;
				case Screen.SCREEN_3:
					return s_screen3;
				default:
					throw new Exception($"ERROR: GFX_Screen_Get_ByIndex() parameter value '{screenID}' is invalid.");
			}
		}

		internal static void GFX_Screen_SetDirty(Screen screenID, ushort left, ushort top, ushort right, ushort bottom)
		{
			if (screenID == Screen.SCREEN_ACTIVE) screenID = s_screenActiveID;
			if (screenID != Screen.SCREEN_0) return;
			s_screen0_is_dirty = true;
			if (left < s_screen0_dirty_area.left) s_screen0_dirty_area.left = left;
			if (top < s_screen0_dirty_area.top) s_screen0_dirty_area.top = top;
			if (right > s_screen0_dirty_area.right) s_screen0_dirty_area.right = right;
			if (bottom > s_screen0_dirty_area.bottom) s_screen0_dirty_area.bottom = bottom;
		}

		/*
		 * Copy information from a buffer to the screen.
		 * @param x The X-coordinate on the screen.
		 * @param y The Y-coordinate on the screen.
		 * @param width The width.
		 * @param height The height.
		 * @param buffer The buffer to copy from.
		 */
		internal static void GFX_CopyFromBuffer(short left, short top, ushort width, ushort height, /* uint8 * */byte[] buffer)
		{
			/* uint8 * */
			byte[] screen;
			int screenPointer = 0;
			int bufferPointer = 0;

			if (width == 0) return;
			if (height == 0) return;

			if (left < 0) left = 0;
			if (left >= SCREEN_WIDTH) left = SCREEN_WIDTH - 1;

			if (top < 0) top = 0;
			if (top >= SCREEN_HEIGHT) top = SCREEN_HEIGHT - 1;

			if (width > SCREEN_WIDTH - left) width = (ushort)(SCREEN_WIDTH - left);
			if (height > SCREEN_HEIGHT - top) height = (ushort)(SCREEN_HEIGHT - top);

			screen = (byte[])GFX_Screen_Get_ByIndex(Screen.SCREEN_0);
			screenPointer += (ushort)(top * SCREEN_WIDTH + left);

			GFX_Screen_SetDirty(Screen.SCREEN_0, (ushort)left, (ushort)top, (ushort)(left + width), (ushort)(top + height));

			while (height-- != 0)
			{
				Array.Copy(buffer, bufferPointer, screen, screenPointer, width); //memcpy(screen, buffer, width);
				screenPointer += SCREEN_WIDTH;
				bufferPointer += width;
			}
		}

		/*
		 * Copy information from the screen to a buffer.
		 * @param x The X-coordinate on the screen.
		 * @param y The Y-coordinate on the screen.
		 * @param width The width.
		 * @param height The height.
		 * @param buffer The buffer to copy to.
		 */
		internal static void GFX_CopyToBuffer(short left, short top, ushort width, ushort height, /* uint8 * */byte[] buffer)
		{
			/* uint8 * */
			byte[] screen;
			int screenPointer = 0;
			int bufferPointer = 0;

			if (width == 0) return;
			if (height == 0) return;

			if (left < 0) left = 0;
			if (left >= SCREEN_WIDTH) left = SCREEN_WIDTH - 1;

			if (top < 0) top = 0;
			if (top >= SCREEN_HEIGHT) top = SCREEN_HEIGHT - 1;

			if (width > SCREEN_WIDTH - left) width = (ushort)(SCREEN_WIDTH - left);
			if (height > SCREEN_HEIGHT - top) height = (ushort)(SCREEN_HEIGHT - top);

			screen = (byte[])GFX_Screen_Get_ByIndex(Screen.SCREEN_0);
			screenPointer += (ushort)(top * SCREEN_WIDTH + left);

			while (height-- != 0)
			{
				Array.Copy(screen, screenPointer, buffer, bufferPointer, width); //memcpy(buffer, screen, width);
				screenPointer += SCREEN_WIDTH;
				bufferPointer += width;
			}
		}

		/*
		 * Change the current active screen to the new value.
		 * @param screenID The new screen to get active.
		 * @return Old screenID that was currently active.
		 */
		internal static Screen GFX_Screen_SetActive(Screen screenID)
		{
			Screen oldScreen = s_screenActiveID;
			if (screenID != Screen.SCREEN_ACTIVE)
			{
				s_screenActiveID = screenID;
			}
			return oldScreen;
		}

		/*
		* Checks if the screen is active.
		* @param screenID The screen to check for being active
		* @return true or false.
		*/
		internal static bool GFX_Screen_IsActive(Screen screenID)
		{
			if (screenID == Screen.SCREEN_ACTIVE) return true;
			return (screenID == s_screenActiveID);
		}

		/*
		 * Draw a tile on the screen.
		 * @param tileID The tile to draw.
		 * @param x The x-coordinate to draw the sprite.
		 * @param y The y-coordinate to draw the sprite.
		 * @param houseID The house the sprite belongs (for recolouring).
		 */
		internal static void GFX_DrawTile(ushort tileID, ushort x, ushort y, byte houseID)
		{
			int i, j;
			byte[] icon_palette;
			byte[] wArray;
			int wPointer = 0;
			byte[] rArray;
			int rPointer = 0;
			byte[] local_palette = new byte[16];

			Debug.Assert(houseID < (byte)HouseType.HOUSE_MAX);

			if (s_tileMode == 4) return;

			icon_palette = Sprites.g_iconRPAL[(Sprites.g_iconRTBL[tileID] << 4)..]; //Sprites.g_iconRPAL + (Sprites.g_iconRTBL[tileID] << 4);

			if (houseID != 0)
			{
				/* Remap colors for the right house */
				for (i = 0; i < 16; i++)
				{
					byte colour = icon_palette[i];

					/* ENHANCEMENT -- Dune2 recolours too many colours, causing clear graphical glitches in the IX building */
					if ((colour & 0xF0) == 0x90)
					{
						if (colour <= 0x96 || !CSharpDune.g_dune2_enhanced) colour += (byte)(houseID << 4);
					}
					local_palette[i] = colour;
				}
				icon_palette = local_palette;
			}

			wArray = (byte[])GFX_Screen_GetActive();
			wPointer += (ushort)(y * SCREEN_WIDTH + x);
			rArray = Sprites.g_tilesPixels[(tileID * s_tileByteSize)..];

			/* tiles with transparent pixels : [1 : 33] U [108 : 122] and 124
			 * palettes 1 to 18 and 22 and 24 */
			/*if (tileID <= 33 || (tileID >= 108 && tileID <= 124)) {*/
			/* We've found that all "transparent" icons/tiles have 0 (transparent) as color 0 */
			if (icon_palette[0] == 0)
			{
				for (j = 0; j < s_tileHeight; j++)
				{
					for (i = 0; i < s_tileWidth; i++)
					{
						byte left = icon_palette[rArray[rPointer] >> 4];
						byte right = icon_palette[rArray[rPointer] & 0xF];
						rPointer++;

						if (left != 0) wArray[wPointer] = left;
						wPointer++;
						if (right != 0) wArray[wPointer] = right;
						wPointer++;
					}
					wPointer += s_tileSpacing;
				}
			}
			else
			{
				for (j = 0; j < s_tileHeight; j++)
				{
					for (i = 0; i < s_tileWidth; i++)
					{
						wArray[wPointer++] = icon_palette[rArray[rPointer] >> 4];
						wArray[wPointer++] = icon_palette[rArray[rPointer] & 0xF];
						rPointer++;
					}
					wPointer += s_tileSpacing;
				}
			}
		}

		/*
		 * Get the codesegment of the active screen buffer.
		 * @return The codesegment of the screen buffer.
		 */
		internal static /*byte[]*/object GFX_Screen_GetActive() =>
			GFX_Screen_Get_ByIndex(s_screenActiveID);

		/*
		 * Set a new palette for the screen.
		 * @param palette The palette in RGB order.
		 */
		internal static void GFX_SetPalette(byte[] palette)
		{
			int from, to;

			for (from = 0; from < 256; from++)
			{
				if (palette[from * 3] != g_paletteActive[from * 3] ||
				   palette[from * 3 + 1] != g_paletteActive[from * 3 + 1] ||
				   palette[from * 3 + 2] != g_paletteActive[from * 3 + 2]) break;
			}
			if (from >= 256)
			{
				Trace.WriteLine("WARNING: Useless GFX_SetPalette() call");
				return;
			}
			for (to = 255; to > from; to--)
			{
				if (palette[to * 3] != g_paletteActive[to * 3] ||
				   palette[to * 3 + 1] != g_paletteActive[to * 3 + 1] ||
				   palette[to * 3 + 2] != g_paletteActive[to * 3 + 2]) break;
			}
			Sdl2Video.Video_SetPalette(palette[(3 * from)..], from, to - from + 1);

			Array.Copy(palette, 3 * from, g_paletteActive, 3 * from, (to - from + 1) * 3); //memcpy(g_paletteActive + 3 * from, palette + 3 * from, (to - from + 1) * 3);
		}

		/*
		 * Put a pixel on the screen.
		 * @param x The X-coordinate on the screen.
		 * @param y The Y-coordinate on the screen.
		 * @param colour The colour of the pixel to put on the screen.
		 */
		internal static void GFX_PutPixel(ushort x, ushort y, byte colour)
		{
			if (y >= SCREEN_HEIGHT) return;
			if (x >= SCREEN_WIDTH) return;

			((byte[])GFX_Screen_GetActive())[y * SCREEN_WIDTH + x] = colour;
		}

		/*
		 * Copy information from one screenbuffer to the other.
		 * @param xSrc The X-coordinate on the source.
		 * @param ySrc The Y-coordinate on the source.
		 * @param xDst The X-coordinate on the destination.
		 * @param yDst The Y-coordinate on the destination.
		 * @param width The width.
		 * @param height The height.
		 * @param screenSrc The ID of the source screen.
		 * @param screenDst The ID of the destination screen.
		 */
		internal static void GFX_Screen_Copy(short xSrc, short ySrc, short xDst, short yDst, short width, short height, Screen screenSrc, Screen screenDst)
		{
			byte[] src;
			byte[] dst;
			int srcPointer = 0;
			int dstPointer = 0;

			if (xSrc >= SCREEN_WIDTH) return;
			if (xSrc < 0) xSrc = 0;

			if (ySrc >= SCREEN_HEIGHT) return;
			if (ySrc < 0) ySrc = 0;

			if (xDst >= SCREEN_WIDTH) return;
			if (xDst < 0) xDst = 0;

			if ((yDst + height) > SCREEN_HEIGHT)
			{
				height = (short)(SCREEN_HEIGHT - 1 - yDst);
			}
			if (height <= 0) return;

			if (yDst >= SCREEN_HEIGHT) return;
			if (yDst < 0) yDst = 0;

			if (width <= 0 || width > SCREEN_WIDTH) return;

			src = (byte[])GFX_Screen_Get_ByIndex(screenSrc);
			dst = (byte[])GFX_Screen_Get_ByIndex(screenDst);

			srcPointer += (ushort)(xSrc + ySrc * SCREEN_WIDTH);
			dstPointer += (ushort)(xDst + yDst * SCREEN_WIDTH);

			GFX_Screen_SetDirty(screenDst, (ushort)xDst, (ushort)yDst, (ushort)(xDst + width), (ushort)(yDst + height));

			if (width == SCREEN_WIDTH)
			{
				Array.Copy(src, srcPointer, dst, dstPointer, height * SCREEN_WIDTH); //memmove(dst, src, height * SCREEN_WIDTH);
			}
			else
			{
				while (height-- != 0)
				{
					Array.Copy(src, srcPointer, dst, dstPointer, width); //memmove(dst, src, width);
					dstPointer += SCREEN_WIDTH;
					srcPointer += SCREEN_WIDTH;
				}
			}
		}

		/*
		 * Returns the size of a screenbuffer.
		 * @param screenID The screenID to get the size of.
		 * @return Some size value.
		 */
		internal static ushort GFX_Screen_GetSize_ByIndex(Screen screenID)
		{
			if (screenID == Screen.SCREEN_ACTIVE)
				screenID = s_screenActiveID;
			Debug.Assert(screenID >= 0 && (byte)screenID < GFX_SCREEN_BUFFER_COUNT);
			return s_screenBufferSize[(int)screenID];
		}

		/*
		 * Clears the screen.
		 */
		internal static void GFX_ClearScreen(Screen screenID)
		{
			Array.Fill<byte>((byte[])GFX_Screen_Get_ByIndex(screenID), 0, 0, SCREEN_WIDTH * SCREEN_HEIGHT); //memset(GFX_Screen_Get_ByIndex(screenID), 0, SCREEN_WIDTH * SCREEN_HEIGHT);
			GFX_Screen_SetDirty(screenID, 0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
		}

		/*
		 * Clears the given memory block.
		 * @param index The memory block.
		 */
		internal static void GFX_ClearBlock(Screen index)
		{
			Array.Fill<byte>((byte[])GFX_Screen_Get_ByIndex(index), 0, 0, GFX_Screen_GetSize_ByIndex(index)); //memset(GFX_Screen_Get_ByIndex(index), 0, GFX_Screen_GetSize_ByIndex(index));
			GFX_Screen_SetDirty(index, 0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
		}

		/*
		 * Initialize the GFX system.
		 */
		internal static void GFX_Init()
		{
			/* init g_paletteActive with invalid values so first GFX_SetPalette() will be ok */
			Array.Fill<byte>(g_paletteActive, 0xff, 0, 3 * 256); //memset(g_paletteActive, 0xff, 3*256);

			s_screen1 = new byte[s_screenBufferSize[(int)Screen.SCREEN_1] + s_screenBufferSize[(int)Screen.SCREEN_2] + s_screenBufferSize[(int)Screen.SCREEN_3]];
			s_screen2 = new byte[s_screenBufferSize[(int)Screen.SCREEN_2] + s_screenBufferSize[(int)Screen.SCREEN_3]];
			s_screen3 = new byte[s_screenBufferSize[(int)Screen.SCREEN_3]];

			s_screenActiveID = Screen.SCREEN_0;
		}

		/*
		 * Uninitialize the GFX system.
		 */
		internal static void GFX_Uninit()
		{
			s_screen1 = null;
			s_screen2 = null;
			s_screen3 = null;
		}

		/*
		 * Get a pixel on the screen.
		 * @param x The X-coordinate on the screen.
		 * @param y The Y-coordinate on the screen.
		 * @return The colour of the pixel.
		 */
		internal static byte GFX_GetPixel(ushort x, ushort y)
		{
			if (y >= SCREEN_HEIGHT) return 0;
			if (x >= SCREEN_WIDTH) return 0;

			return ((byte[])GFX_Screen_GetActive())[y * SCREEN_WIDTH + x];
		}

		/*
		 * Copy information from one screenbuffer to the other.
		 * @param xSrc The X-coordinate on the source.
		 * @param ySrc The Y-coordinate on the source.
		 * @param xDst The X-coordinate on the destination.
		 * @param yDst The Y-coordinate on the destination.
		 * @param width The width.
		 * @param height The height.
		 * @param screenSrc The ID of the source screen.
		 * @param screenDst The ID of the destination screen.
		 * @param skipNull Wether to skip pixel colour 0.
		 */
		internal static void GFX_Screen_Copy2(short xSrc, short ySrc, short xDst, short yDst, short width, short height, Screen screenSrc, Screen screenDst, bool skipNull)
		{
			byte[] src;
			byte[] dst;
			int srcPointer = 0;
			int dstPointer = 0;

			if (xSrc >= SCREEN_WIDTH) return;
			if (xSrc < 0)
			{
				xDst += xSrc;
				width += xSrc;
				xSrc = 0;
			}

			if (ySrc >= SCREEN_HEIGHT) return;
			if (ySrc < 0)
			{
				yDst += ySrc;
				height += ySrc;
				ySrc = 0;
			}

			if (xDst >= SCREEN_WIDTH) return;
			if (xDst < 0)
			{
				xSrc += xDst;
				width += xDst;
				xDst = 0;
			}

			if (yDst >= SCREEN_HEIGHT) return;
			if (yDst < 0)
			{
				ySrc += yDst;
				height += yDst;
				yDst = 0;
			}

			if (SCREEN_WIDTH - xSrc - width < 0) width = (short)(SCREEN_WIDTH - xSrc);
			if (SCREEN_HEIGHT - ySrc - height < 0) height = (short)(SCREEN_HEIGHT - ySrc);
			if (SCREEN_WIDTH - xDst - width < 0) width = (short)(SCREEN_WIDTH - xDst);
			if (SCREEN_HEIGHT - yDst - height < 0) height = (short)(SCREEN_HEIGHT - yDst);

			if (xSrc < 0 || xSrc >= SCREEN_WIDTH) return;
			if (xDst < 0 || xDst >= SCREEN_WIDTH) return;
			if (ySrc < 0 || ySrc >= SCREEN_HEIGHT) return;
			if (yDst < 0 || yDst >= SCREEN_HEIGHT) return;
			if (width < 0 || width >= SCREEN_WIDTH) return;
			if (height < 0 || height >= SCREEN_HEIGHT) return;

			GFX_Screen_SetDirty(screenDst, (ushort)xDst, (ushort)yDst, (ushort)(xDst + width), (ushort)(yDst + height));

			src = (byte[])GFX_Screen_Get_ByIndex(screenSrc);
			dst = (byte[])GFX_Screen_Get_ByIndex(screenDst);

			srcPointer += xSrc + ySrc * SCREEN_WIDTH;
			dstPointer += xDst + yDst * SCREEN_WIDTH;

			while (height-- != 0)
			{
				if (skipNull)
				{
					ushort i;
					for (i = 0; i < width; i++)
					{
						if (src[srcPointer + i] != 0) dst[dstPointer + i] = src[srcPointer + i];
					}
				}
				else
				{
					Array.Copy(src, srcPointer, dst, dstPointer, width); //memmove(dst, src, width);
				}
				dstPointer += SCREEN_WIDTH;
				srcPointer += SCREEN_WIDTH;
			}
		}

		/*
		 * Initialize sprite information.
		 *
		 * @param widthSize Value between 0 and 2, indicating the width of the sprite. x8 to get actuel width of sprite
		 * @param heightSize Value between 0 and 2, indicating the width of the sprite. x8 to get actuel width of sprite
		 */
		internal static void GFX_Init_TilesInfo(ushort widthSize, ushort heightSize)
		{
			/* NOTE : shouldn't it be (heightSize < 3 && widthSize < 3) ??? */
			if (widthSize == heightSize && widthSize < 3)
			{
				s_tileMode = (byte)(widthSize & 2);

				s_tileWidth = (ushort)(widthSize << 2);
				s_tileHeight = (ushort)(heightSize << 3);
				s_tileSpacing = (ushort)(SCREEN_WIDTH - s_tileHeight);
				s_tileByteSize = (byte)(s_tileWidth * s_tileHeight);
			}
			else
			{
				/* NOTE : is it dead code ? */
				/* default to 8x8 sprites */
				s_tileMode = 4;
				s_tileByteSize = 8 * 4;

				s_tileWidth = 4;
				s_tileHeight = 8;
				s_tileSpacing = 312;
			}
		}

		internal static bool GFX_Screen_IsDirty(Screen screenID)
		{
			if (screenID == Screen.SCREEN_ACTIVE) screenID = s_screenActiveID;
			if (screenID != Screen.SCREEN_0) return true;
			return s_screen0_is_dirty;
		}

		internal static dirty_area GFX_Screen_GetDirtyArea(Screen screenID)
		{
			if (screenID == Screen.SCREEN_ACTIVE) screenID = s_screenActiveID;
			if (screenID != Screen.SCREEN_0) return null;
			return s_screen0_dirty_area;
		}

		internal static void GFX_Screen_SetClean(Screen screenID)
		{
			if (screenID == Screen.SCREEN_ACTIVE) screenID = s_screenActiveID;
			if (screenID != Screen.SCREEN_0) return;
			s_screen0_is_dirty = false;
			s_screen0_dirty_area.left = 0xffff;
			s_screen0_dirty_area.top = 0xffff;
			s_screen0_dirty_area.right = 0;
			s_screen0_dirty_area.bottom = 0;
			Array.Fill<uint>(g_dirty_blocks, 0, 0, g_dirty_blocks.Length); //memset(g_dirty_blocks, 0, sizeof(g_dirty_blocks));
		}
	}
}
