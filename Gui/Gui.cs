/* Generic GUI */

//#define Gui_0
#define Gui_1

namespace SharpDune.Gui
{
    /*
	 * The possible selection types.
	 */
    enum SelectionType
	{
		MENTAT = 0,                               /*!< Used in most mentat screens. */
		TARGET = 1,                               /*!< Used when attacking or moving a unit, the target screen. */
		PLACE = 2,                                /*!< Used when placing a structure. */
		UNIT = 3,                                 /*!< Used when selecting a Unit. */
		STRUCTURE = 4,                            /*!< Used when selecting a Structure or nothing. */
		DEBUG = 5,                                /*!< Used when debugging scenario. */
		UNKNOWN6 = 6,                             /*!< ?? */
		INTRO = 7,                                /*!< Used in intro of the game. */

		MAX = 8
	}

	/*
	 * Factory results.
	 */
	enum FactoryResult
	{
		FACTORY_RESUME = 0,
		FACTORY_BUY = 1,
		FACTORY_UPGRADE = 2,
		FACTORY_CONTINUE = 0xFFFF
	}

	/*
	 * Factory Window Item struct.
	 */
	class FactoryWindowItem
	{
		internal ushort objectType;                     /*!< Which object is this item. */
		internal sbyte amount;                          /*!< How many are available. */
		internal short credits;                         /*!< What is the current price. */
		internal short sortPriority;                    /*!< The sorting priority. */
		internal ObjectInfo objectInfo;                 /*!< The ObjectInfo of the item. */
	}

	[StructLayout(LayoutKind.Explicit)]
	class ClippingArea
	{
		/* 0000(2) PACK */
		[FieldOffset(0)] internal ushort left;                       /*!< ?? */
		/* 0002(2) PACK */
		[FieldOffset(2)] internal ushort top;                        /*!< ?? */
		/* 0004(2) PACK */
		[FieldOffset(4)] internal ushort right;                      /*!< ?? */
		/* 0006(2) PACK */
		[FieldOffset(6)] internal ushort bottom;                     /*!< ?? */
	}

	/*
	 * Information for the selection type.
	 */
	class SelectionTypeStruct
	{
		internal sbyte[] visibleWidgets = new sbyte[20];        /*!< List of index of visible widgets, -1 terminated. */
		internal bool variable_04;                              /*!< ?? */
		internal bool variable_06;                              /*!< ?? */
		internal ushort defaultWidget;                          /*!< Index of the default Widget. */
	}

	/*
	 * Hall Of Fame data struct.
	 */
	class HallOfFameStruct
	{
		const int size = 16;
		internal char[] name = new char[6];                     /*!< Name of the entry. */
		internal ushort score;                                  /*!< Score of the entry. */
		internal ushort rank;                                   /*!< Rank of the entry. */
		internal ushort campaignID;                             /*!< Which campaign was reached. */
		internal byte houseID;                                  /*!< Which house was playing. */
		internal byte padding1;                                 /*!< Padding bytes. */
		internal ushort padding2;                               /*!< Padding bytes. */

		//TODO: Use BitConverter?
		internal byte[] ToBytes()
		{
			var bytes = new byte[size];

			//name
			for (var i = 0; i < 6; i++) bytes[i] = (byte)name[i];

			//score
			bytes[6] = (byte)(score & 0xFF);
			bytes[7] = (byte)((score >> 8) & 0xFF);

			//rank
			bytes[8] = (byte)(rank & 0xFF);
			bytes[9] = (byte)((rank >> 8) & 0xFF);

			//campaignID
			bytes[10] = (byte)(campaignID & 0xFF);
			bytes[11] = (byte)((campaignID >> 8) & 0xFF);

			//houseID
			bytes[12] = houseID;

			//padding1
			bytes[13] = padding1;

			//padding2
			bytes[14] = (byte)(padding2 & 0xFF);
			bytes[15] = (byte)((padding2 >> 8) & 0xFF);

			return bytes;
		}

		internal void FromBytes(byte[] bytes)
		{
			for (var i = 0; i < 6; i++) name[i] = (char)bytes[i];
			score = BitConverter.ToUInt16(bytes[6..8]);
			rank = BitConverter.ToUInt16(bytes[8..10]);
			campaignID = BitConverter.ToUInt16(bytes[10..12]);
			houseID = bytes[12];
			padding1 = bytes[13];
			padding2 = BitConverter.ToUInt16(bytes[14..16]);
		}

		internal static byte[] AllToBytes(HallOfFameStruct[] data)
		{
			var output = new byte[data.Length * size];
			for (var i = 0; i < data.Length; i++) Array.Copy(data[i].ToBytes(), 0, output, i * size, size);
			return output;
		}

		internal static HallOfFameStruct[] AllFromBytes(byte[] bytes)
		{
			var output = new HallOfFameStruct[bytes.Length / size];
			for (var i = 0; i < output.Length; i++)
			{
				output[i] = new HallOfFameStruct();
				output[i].FromBytes(bytes[(i * size)..((i + 1) * size)]);
			}
			return output;
		}
	}

	class StrategicMapData
	{
		/* 0000(2)   PACK */
		internal short index;      /*!< ?? */
		/* 0002(2)   PACK */
		internal short arrow;      /*!< ?? */
		/* 0004(2)   PACK */
		internal short offsetX;    /*!< ?? */
		/* 0006(2)   PACK */
		internal short offsetY;    /*!< ?? */
	}

	/* Coupling between score and rank name. */
	class RankScore
	{
		internal ushort rankString; /*!< StringID of the name of the rank. */
		internal ushort score;      /*!< Score needed to obtain the rank. */
	}

	class Gui
	{
		internal static byte[] g_remap = new byte[256];

		static readonly byte[] g_colours = new byte[16];                 /*!< Colors used for drawing chars */
		static readonly ClippingArea g_clipping = new() { left = 0, top = 0, right = Gfx.SCREEN_WIDTH - 1, bottom = Gfx.SCREEN_HEIGHT - 1 };

		static /*byte[]*/CArray<byte> s_palette1_houseColour;

		static uint s_tickCreditsAnimation;                 /*!< Next tick when credits animation needs an update. */
        static uint s_arrowAnimationTimeout;                /*!< Timeout value for the next palette change in the animation of the arrows. */
        static ushort s_arrowAnimationState;                /*!< State of the arrow animation. @see _arrowAnimationTimeout */

        /*
		 * flags used for GUI_DrawSprite()
		 */
        /* reverse X axis (void) (RTL = Right To Left) */
        const ushort DRAWSPRITE_FLAG_RTL = 0x0001;
		/* reverse Y axis (void) */
		const ushort DRAWSPRITE_FLAG_BOTTOMUP = 0x0002;
		/* Zoom (int zoom_factor_x, int zoomRatioY) UNUSED ? */
		const ushort DRAWSPRITE_FLAG_ZOOM = 0x0004;
		/* Remap (uint8* remap, int remapCount) */
		internal const ushort DRAWSPRITE_FLAG_REMAP = 0x0100;
		/* blur - SandWorm effect (void) */
		internal const ushort DRAWSPRITE_FLAG_BLUR = 0x0200;
		/* sprite has house colors (set internally, no need to be set by caller) */
		const ushort DRAWSPRITE_FLAG_SPRITEPAL = 0x0400;
		/* Set increment value for blur/sandworm effect (int) UNUSED ? */
		const ushort DRAWSPRITE_FLAG_BLURINCR = 0x1000;
		/* house colors argument (uint8 houseColors[16]) */
		internal const ushort DRAWSPRITE_FLAG_PAL = 0x2000;
		/* position relative to widget (void)*/
		internal const ushort DRAWSPRITE_FLAG_WIDGETPOS = 0x4000;
		/* position posX,posY is relative to center of sprite */
		internal const ushort DRAWSPRITE_FLAG_CENTER = 0x8000;

		internal static ushort g_productionStringID;                /*!< Descriptive text of activity of the active structure. */

		internal static bool g_structureHighHealth;
		internal static bool g_var_37B8;
		internal static byte[] g_palette_998A;

		internal static ushort g_viewportPosition;                  /*!< Top-left tile of the viewport. */
		internal static ushort g_selectionRectanglePosition;        /*!< Position of the structure selection rectangle. */

		static ushort s_mouseSpriteLeft;
		static ushort s_mouseSpriteTop;
		static ushort s_mouseSpriteWidth;
		static ushort s_mouseSpriteHeight;

		internal static ushort g_mouseSpriteHotspotX;
		internal static ushort g_mouseSpriteHotspotY;
		internal static ushort g_mouseWidth;
		internal static ushort g_mouseHeight;

		internal static ushort g_cursorSpriteID;
		internal static ushort g_cursorDefaultSpriteID;

		internal static bool g_textDisplayNeedsUpdate;
		internal static uint g_strategicRegionBits;                 /*!< Region bits at the map. */

		static uint s_ticksPlayed;
		internal static bool g_doQuitHOF;

		static readonly byte[] s_strategicMapArrowColors = new byte[24];
		static bool s_strategicMapFastForward;

		internal static ushort g_viewportMessageCounter;
		internal static string g_viewportMessageText;
		internal static ushort g_minimapPosition;
		internal static ushort g_selectionPosition;
		internal static ushort g_selectionWidth;
		internal static ushort g_selectionHeight;
		internal static short g_selectionState = 1;                 /*!< State of the selection (\c 1 is valid, \c 0 is not valid, \c <0 valid but missing some slabs. */

		static ushort[][] s_temporaryColourBorderSchema = new ushort[5][]; /*!< Temporary storage for the #s_colourBorderSchema. */

		/*!< Colours used for the border of widgets. */
		static ushort[][] s_colourBorderSchema = { //[5][4]
			new ushort[] {  26,  29,  29,  29 },
			new ushort[] {  20,  26,  16,  20 },
			new ushort[] {  20,  16,  26,  20 },
			new ushort[] { 233, 235, 232, 233 },
			new ushort[] { 233, 232, 235, 233 }
		};

		/* Colours used for the border of widgets in the hall of fame. */
		static readonly ushort[][] s_HOF_ColourBorderSchema = { //[5][4]
			new ushort[] { 226, 228, 228, 228 },
			new ushort[] { 116, 226, 105, 116 },
			new ushort[] { 116, 105, 226, 116 },
			new ushort[] { 233, 235, 232, 233 },
			new ushort[] { 233, 232, 235, 233 }
		};

		/* Mapping of scores to rank names. */
		static readonly RankScore[] _rankScores = {
			new() { rankString = 271, score = 25 }, /* "Sand Flea" */
			new() { rankString = 272, score = 50 }, /* "Sand Snake" */
			new() { rankString = 273, score = 100 }, /* "Desert Mongoose" */
			new() { rankString = 274, score = 150 }, /* "Sand Warrior" */
			new() { rankString = 275, score = 200 }, /* "Dune Trooper" */
			new() { rankString = 276, score = 300 }, /* "Squad Leader" */
			new() { rankString = 277, score = 400 }, /* "Outpost Commander" */
			new() { rankString = 278, score = 500 }, /* "Base Commander" */
			new() { rankString = 279, score = 700 }, /* "Warlord" */
			new() { rankString = 280, score = 1000 }, /* "Chief Warlord" */
			new() { rankString = 281, score = 1400 }, /* "Ruler of Arrakis" */
			new() { rankString = 282, score = 1800 }  /* "Emperor" */
		};

		internal static bool g_factoryWindowConstructionYard;
        internal static FactoryWindowItem[] g_factoryWindowItems = new FactoryWindowItem[25];

		static readonly Widget[] s_factoryWindowWidgets = new Widget[13];
		static byte[] s_factoryWindowGraymapTbl = new byte[256];
		static readonly byte[] s_factoryWindowWsaBuffer = new byte[64000];
		internal static bool g_factoryWindowStarport;
		internal static ushort g_factoryWindowBase;
		static ushort g_factoryWindowUpgradeCost;
		internal static ushort g_factoryWindowOrdered;
		internal static ushort g_factoryWindowSelected;
		internal static ushort g_factoryWindowTotal;
		internal static FactoryResult g_factoryWindowResult = FactoryResult.FACTORY_RESUME;

		static readonly byte[] blurOffsets = { 1, 3, 2, 5, 4, 3, 2, 1 };
		static ushort s_blurIndex; /* index into previous table */
        /*
		 * Draws a sprite.
		 * @param screenID On which screen to draw the sprite.
		 * @param sprite The sprite to draw.
		 * @param posX position where to draw sprite.
		 * @param posY position where to draw sprite.
		 * @param windowID The ID of the window where the drawing is done.
		 * @param flags The flags.
		 * @param ... The extra args, flags dependant.
		 *
		 * flags :
		 * 0x0001 reverse X (void)
		 * 0x0002 reverse Y (void)
		 * 0x0004 zoom (int zoom_factor_x, int zoomRatioY) UNUSED ?
		 * 0x0100 Remap (uint8* remap, int remapCount)
		 * 0x0200 blur - SandWorm effect (void)
		 * 0x0400 sprite has house colors (set internally, no need to be set by caller)
		 * 0x1000 set blur increment value (int) UNUSED ?
		 * 0x2000 house colors argument (uint8 houseColors[16])
		 * 0x4000 position relative to widget (void)
		 * 0x8000 position posX,posY is relative to center of sprite
		 *
		 * sprite data format :
		 * 00: 2 bytes = flags 0x01 = has House colors, 0x02 = NOT Format80 encoded
		 * 02: 1 byte  = height
		 * 03: 2 bytes = width
		 * 05: 1 byte  = height - duplicated (ignored)
		 * 06: 2 bytes = sprite data length, including header (ignored)
		 * 08: 2 bytes = decoded data length
		 * 0A: [16 bytes] = house colors (if flags & 0x01)
		 * [1]A: xx bytes = data (depending on flags & 0x02 : 1 = raw, 0 = Format80 encoded)
		 */
        internal static void GUI_DrawSprite(Screen screenID, /* uint8 * */byte[] sprite, short posX, short posY, ushort windowID, int flags, params object[] ap)
		{
			/* variables for blur/sandworm effect */
			ushort blurOffset = 1;
			ushort blurRandomValueIncr = 0x8B55;
			ushort blurRandomValue = 0x51EC;

			short top;
			short bottom;
			ushort width;
			ushort spriteFlags;
			short spriteHeight; /* number of sprite rows to draw */
			short tmpRowCountToDraw;
			short pixelCountPerRow; /* count of pixel to draw per row */
			short spriteWidthZoomed;    /* spriteWidth * zoomRatioX */
			short spriteWidth;  /* original sprite Width */
			short pixelSkipStart;   /* pixel count to skip at start of row */
			short pixelSkipEnd; /* pixel count to skip at end of row */
			/* uint8 * */
			byte[] remap = null;
			short remapCount = 0;
			short distY;
			ushort zoomRatioX = 0;  /* 8.8 fixed point, ie 0x0100 = 1x */
			ushort zoomRatioY = 0x100;  /* 8.8 fixed point, ie 0x0100 = 1x */
			ushort Ycounter = 0;    /* 8.8 fixed point, ie 0x0100 = 1 */
			short distX;
			/* uint8 * */
			byte[] palette = null;
			ushort spriteDecodedLength; /* if encoded with Format80 */
			var spriteBuffer = new byte[20000]; /* for sprites encoded with Format80 : maximum size for credits images is 19841, elsewere it is 3456 */

			/* uint8 * */
			byte[] buf = null;
			short count;
			short buf_incr;
			var bufPointer = 0;
			var bPointer = 0;
			var spritePointer = 0;
			var spriteSavePointer = 0;
			var apPointer = 0;

			if (sprite == null) return;

			/* read additional arguments according to the flags */

			if ((flags & DRAWSPRITE_FLAG_PAL) != 0) palette = (byte[])ap[apPointer++]; //va_arg(ap, uint8*);

			/* Remap */
			if ((flags & DRAWSPRITE_FLAG_REMAP) != 0)
			{
				remap = (byte[])ap[apPointer++]; //va_arg(ap, uint8*);
				remapCount = (short)ap[apPointer++]; //va_arg(ap, int);
				if (remapCount == 0) flags &= ~DRAWSPRITE_FLAG_REMAP;
			}

			if ((flags & DRAWSPRITE_FLAG_BLUR) != 0)
			{
				s_blurIndex = (ushort)((s_blurIndex + 1) % 8);
				blurOffset = blurOffsets[s_blurIndex];
				blurRandomValue = 0x0;
				blurRandomValueIncr = 0x100;
			}

			if ((flags & DRAWSPRITE_FLAG_BLURINCR) != 0) blurRandomValueIncr = (ushort)ap[apPointer++]; //va_arg(ap, int);

			if ((flags & DRAWSPRITE_FLAG_ZOOM) != 0)
			{
				zoomRatioX = (ushort)ap[apPointer++]; //va_arg(ap, int);
				zoomRatioY = (ushort)ap[apPointer++]; //va_arg(ap, int);
			}

			buf = Gfx.GFX_Screen_Get_ByIndex(screenID);
			bufPointer += (CWidget.g_widgetProperties[windowID].xBase << 3);

			width = (ushort)(CWidget.g_widgetProperties[windowID].width << 3);
			top = (short)CWidget.g_widgetProperties[windowID].yBase;
			bottom = (short)(top + CWidget.g_widgetProperties[windowID].height);

			if ((flags & DRAWSPRITE_FLAG_WIDGETPOS) != 0)
			{
				posY += (short)CWidget.g_widgetProperties[windowID].yBase;
			}
			else
			{
				posX -= (short)(CWidget.g_widgetProperties[windowID].xBase << 3);
			}

			spriteFlags = Endian.READ_LE_UINT16(sprite[spritePointer..]);
			spritePointer += 2;

			if ((spriteFlags & 0x1) != 0) flags |= DRAWSPRITE_FLAG_SPRITEPAL;

			spriteHeight = sprite[spritePointer++];
			spriteWidth = (short)Endian.READ_LE_UINT16(sprite[spritePointer..]);
			spritePointer += 5;
			spriteDecodedLength = Endian.READ_LE_UINT16(sprite[spritePointer..]);
			spritePointer += 2;

			spriteWidthZoomed = spriteWidth;

			if ((flags & DRAWSPRITE_FLAG_ZOOM) != 0)
			{
				spriteHeight = (short)((spriteHeight * zoomRatioY) >> 8);
				if (spriteHeight == 0) return;
				spriteWidthZoomed = (short)((spriteWidthZoomed * zoomRatioX) >> 8);
				if (spriteWidthZoomed == 0) return;
			}

			if ((flags & DRAWSPRITE_FLAG_CENTER) != 0)
			{
				posX -= (short)(spriteWidthZoomed / 2); /* posX relative to center */
				posY -= (short)(spriteHeight / 2);      /* posY relative to center */
			}

			pixelCountPerRow = spriteWidthZoomed;

			if ((spriteFlags & 0x1) != 0)
			{
				if ((flags & DRAWSPRITE_FLAG_PAL) == 0) palette = sprite[spritePointer..];
				spritePointer += 16;
			}

			if ((spriteFlags & 0x2) == 0)
			{
				Format80.Format80_Decode(spriteBuffer, sprite, spriteDecodedLength, 0, spritePointer);

				sprite = spriteBuffer;
			}

			if ((flags & DRAWSPRITE_FLAG_BOTTOMUP) == 0)
			{
				/* distance between top of window and top of sprite */
				distY = (short)(posY - top);
			}
			else
			{
				/* distance between bottom of window and bottom of sprite */
				distY = (short)(bottom - posY - spriteHeight);
			}

			if (distY < 0)
			{
				/* means the sprite begins outside the window,
				 * need to skip a few rows before drawing */
				spriteHeight += distY;
				if (spriteHeight <= 0) return;

				distY = (short)-distY;

				while (distY > 0)
				{
					/* skip a row */
					spriteSavePointer = spritePointer;
					count = spriteWidth;

					Debug.Assert((flags & 0xFF) < 4);   /* means DRAWSPRITE_FLAG_ZOOM is forbidden */
					/* so (flags & 0xFD) equals (flags & DRAWSPRITE_FLAG_RTL) */

					while (count > 0)
					{
#if Gui_1
						if (sprite[spritePointer++] == 0) count -= sprite[spritePointer++];
						else count--;
#else
						while (count != 0)
						{
							count--;
							if (sprite[spritePointer++] == 0) break;
						}
						if (sprite[spritePointer - 1] != 0 && count == 0) break;

						count -= (short)(sprite[spritePointer++] - 1);
#endif
					}

					/*buf += count * (((flags & 0xFF) == 0 || (flags & 0xFF) == 2) ? -1 : 1);*/
#if Gui_0
			if ((flags & 0xFD) == 0) buf -= count;	/* 0xFD = 1111 1101b */
			else buf += count;
#else
					if ((flags & DRAWSPRITE_FLAG_RTL) != 0) bufPointer += count;
					else bufPointer -= count;
#endif

					Ycounter += zoomRatioY;
					if ((Ycounter & 0xFF00) != 0)
					{
						distY -= (short)(Ycounter >> 8);
						Ycounter &= 0xFF;   /* keep only fractional part */
					}
				}

				if (distY < 0)
				{
					spritePointer = spriteSavePointer;

					Ycounter += (ushort)((-distY) << 8);
				}

				if ((flags & DRAWSPRITE_FLAG_BOTTOMUP) == 0) posY = top;
			}

			if ((flags & DRAWSPRITE_FLAG_BOTTOMUP) == 0)
			{
				tmpRowCountToDraw = (short)(bottom - posY); /* rows to draw */
			}
			else
			{
				tmpRowCountToDraw = (short)(posY + spriteHeight - top); /* rows to draw */
			}

			if (tmpRowCountToDraw <= 0) return; /* no row to draw */

			if (tmpRowCountToDraw < spriteHeight)
			{
				/* there are a few rows to skip at the end */
				spriteHeight = tmpRowCountToDraw;
				if ((flags & DRAWSPRITE_FLAG_BOTTOMUP) != 0) posY = top;
			}

			pixelSkipStart = 0;
			if (posX < 0)
			{
				/* skip pixels outside window */
				pixelCountPerRow += posX;
				pixelSkipStart = (short)-posX;  /* pixel count to skip at row start */
				if (pixelSkipStart >= spriteWidthZoomed) return;    /* no pixel to draw */
				posX = 0;
			}

			pixelSkipEnd = 0;
			distX = (short)(width - posX);  /* distance between left of sprite and right of window */
			if (distX <= 0) return; /* no pixel to draw */

			if (distX < pixelCountPerRow)
			{
				pixelCountPerRow = distX;
				pixelSkipEnd = (short)(spriteWidthZoomed - pixelSkipStart - pixelCountPerRow);  /* pixel count to skip at row end */
			}

			/* move pointer to 1st pixel of 1st row to draw */
			bufPointer += posY * Gfx.SCREEN_WIDTH + posX;
			if ((flags & DRAWSPRITE_FLAG_BOTTOMUP) != 0)
			{
				bufPointer += (spriteHeight - 1) * Gfx.SCREEN_WIDTH;
			}

			if ((flags & DRAWSPRITE_FLAG_RTL) != 0)
			{
				/* XCHG pixelSkipStart, pixelSkipEnd */
				var tmp = pixelSkipStart;
				pixelSkipStart = pixelSkipEnd;
				pixelSkipEnd = tmp;
				bufPointer += pixelCountPerRow - 1;
				buf_incr = -1;
			}
			else
			{
				buf_incr = 1;
			}

			bPointer = bufPointer;

			if ((flags & DRAWSPRITE_FLAG_ZOOM) != 0)
			{
				pixelSkipEnd = 0;
				pixelSkipStart = (short)((pixelSkipStart << 8) / zoomRatioX);
			}

			Debug.Assert((flags & 0xFF) < 4);

			Gfx.GFX_Screen_SetDirty(screenID,
								(ushort)((CWidget.g_widgetProperties[windowID].xBase << 3) + posX),
								(ushort)posY,
								(ushort)((CWidget.g_widgetProperties[windowID].xBase << 3) + posX + pixelCountPerRow),
								(ushort)(posY + spriteHeight));

			do
			{
				/* drawing loop */
				if ((Ycounter & 0xFF00) == 0)
				{
					while (true)
					{
						Ycounter += zoomRatioY;

						if ((Ycounter & 0xFF00) != 0) break;
						count = spriteWidth;

						while (count > 0)
						{
#if Gui_1
							if (sprite[spritePointer++] == 0) count -= sprite[spritePointer++];
							else count--;
#else
							while (count != 0)
							{
								count--;
								if (sprite[spritePointer++] == 0) break;
							}
							if (sprite[spritePointer - 1] != 0 && count == 0) break;

							count -= (short)(sprite[spritePointer++] - 1);
#endif
						}

#if Gui_0
				if ((flags & 0xFD) == 0) buf -= count;
				else buf += count;
#else
						if ((flags & DRAWSPRITE_FLAG_RTL) != 0) bufPointer += count;
						else bufPointer -= count;
#endif
					}
					spriteSavePointer = spritePointer;
				}

				count = pixelSkipStart;

				while (count > 0)
				{
#if Gui_1
					if (sprite[spritePointer++] == 0) count -= sprite[spritePointer++];
					else count--;
#else
					while (count != 0)
					{
						count--;
						if (sprite[spritePointer++] == 0) break;
					}
					if (sprite[spritePointer - 1] != 0 && count == 0) break;

					count -= (short)(sprite[spritePointer++] - 1);
#endif
				}

#if Gui_0
		if ((flags & 0xFD) == 0) buf -= count;
		else buf += count;
#else
				if ((flags & DRAWSPRITE_FLAG_RTL) != 0) bufPointer += count;
				else bufPointer -= count;
#endif

				if (spriteWidth != 0)
				{
					count += pixelCountPerRow;

					Debug.Assert((flags & 0xF00) < 0x800);
					switch (flags & 0xF00)
					{
						case 0:
							while (count > 0)
							{
								var v = sprite[spritePointer++];
								if (v == 0)
								{
									v = sprite[spritePointer++]; /* run length encoding of transparent pixels */
									if ((flags & DRAWSPRITE_FLAG_RTL) != 0) bufPointer -= v;
									else bufPointer += v;
									count -= v;
								}
								else
								{
									buf[bufPointer] = v;
									bufPointer += buf_incr;
									count--;
								}
							}
							break;

						case (DRAWSPRITE_FLAG_REMAP):   /* remap */
							while (count > 0)
							{
								var v = sprite[spritePointer++];
								if (v == 0)
								{
									v = sprite[spritePointer++]; /* run length encoding of transparent pixels */
									if ((flags & DRAWSPRITE_FLAG_RTL) != 0) bufPointer -= v;
									else bufPointer += v;
									count -= v;
								}
								else
								{
									short i;
									for (i = 0; i < remapCount; i++) v = remap[v];
									buf[bufPointer] = v;
									bufPointer += buf_incr;
									count--;
								}
							}
							break;

						case (DRAWSPRITE_FLAG_BLUR):    /* blur/Sandworm effect */
							while (count > 0)
							{
								var v = sprite[spritePointer++];
								if (v == 0)
								{
									v = sprite[spritePointer++]; /* run length encoding of transparent pixels */
									if ((flags & DRAWSPRITE_FLAG_RTL) != 0) bufPointer -= v;
									else bufPointer += v;
									count -= v;
								}
								else
								{
									blurRandomValue += blurRandomValueIncr;

									if ((blurRandomValue & 0xFF00) == 0)
									{
										buf[bufPointer] = v;
									}
									else
									{
										blurRandomValue &= 0xFF;
										buf[bufPointer] = buf[blurOffset];
									}
									bufPointer += buf_incr;
									count--;
								}
							}
							break;

						case (DRAWSPRITE_FLAG_REMAP | DRAWSPRITE_FLAG_BLUR):
						case (DRAWSPRITE_FLAG_REMAP | DRAWSPRITE_FLAG_BLUR | DRAWSPRITE_FLAG_SPRITEPAL):
							/* remap + blur ? (+ has house colors) */
							while (count > 0)
							{
								var v = sprite[spritePointer++];
								if (v == 0)
								{
									v = sprite[spritePointer++]; /* run length encoding of transparent pixels */
									if ((flags & DRAWSPRITE_FLAG_RTL) != 0) bufPointer -= v;
									else bufPointer += v;
									count -= v;
								}
								else
								{
									short i;
									v = buf[bufPointer];
									for (i = 0; i < remapCount; i++) v = remap[v];
									buf[bufPointer] = v;
									bufPointer += buf_incr;
									count--;
								}
							}
							break;

						case (DRAWSPRITE_FLAG_SPRITEPAL):   /* sprite has palette */
							while (count > 0)
							{
								var v = sprite[spritePointer++];
								if (v == 0)
								{
									v = sprite[spritePointer++]; /* run length encoding of transparent pixels */
									if ((flags & DRAWSPRITE_FLAG_RTL) != 0) bufPointer -= v;
									else bufPointer += v;
									count -= v;
								}
								else
								{
									buf[bufPointer] = palette[v];
									bufPointer += buf_incr;
									count--;
								}
							}
							break;

						case (DRAWSPRITE_FLAG_REMAP | DRAWSPRITE_FLAG_SPRITEPAL):
							/* remap +  sprite has palette */
							while (count > 0)
							{
								var v = sprite[spritePointer++];
								if (v == 0)
								{
									v = sprite[spritePointer++]; /* run length encoding of transparent pixels */
									if ((flags & DRAWSPRITE_FLAG_RTL) != 0) bufPointer -= v;
									else bufPointer += v;
									count -= v;
								}
								else
								{
									short i;
									v = palette[v];
									for (i = 0; i < remapCount; i++) v = remap[v];
									buf[bufPointer] = v;
									bufPointer += buf_incr;
									count--;
								}
							}
							break;

						case (DRAWSPRITE_FLAG_BLUR | DRAWSPRITE_FLAG_SPRITEPAL):
							/* blur/sandworm effect + sprite has palette */
							while (count > 0)
							{
								var v = sprite[spritePointer++];
								if (v == 0)
								{
									v = sprite[spritePointer++]; /* run length encoding of transparent pixels */
									if ((flags & DRAWSPRITE_FLAG_RTL) != 0) bufPointer -= v;
									else bufPointer += v;
									count -= v;
								}
								else
								{
									blurRandomValue += blurRandomValueIncr;

									if ((blurRandomValue & 0xFF00) == 0)
									{
										buf[bufPointer] = palette[v];
									}
									else
									{
										blurRandomValue &= 0x00FF;
										buf[bufPointer] = buf[blurOffset];
									}
									bufPointer += buf_incr;
									count--;
								}
							}
							break;
					}

					count += pixelSkipEnd;
					if (count != 0)
					{
						while (count > 0)
						{
#if Gui_1
							if (sprite[spritePointer++] == 0) count -= sprite[spritePointer++];
							else count--;
#else
							while (count != 0)
							{
								count--;
								if (sprite[spritePointer++] == 0) break;
							}
							if (sprite[spritePointer - 1] != 0 && count == 0) break;

							count -= (short)(sprite[spritePointer++] - 1);
#endif
						}

#if Gui_0
				if ((flags & 0xFD) == 0) buf -= count;
				else buf += count;
#else
						if ((flags & DRAWSPRITE_FLAG_RTL) != 0) bufPointer += count;
						else bufPointer -= count;
#endif
					}
				}

				if ((flags & DRAWSPRITE_FLAG_BOTTOMUP) != 0) bPointer -= Gfx.SCREEN_WIDTH;
				else bPointer += Gfx.SCREEN_WIDTH;
				bufPointer = bPointer;

				Ycounter -= 0x100;
				if ((Ycounter & 0xFF00) != 0) spritePointer = spriteSavePointer;
			} while (--spriteHeight > 0);
		}

		/*
		 * Change the selection type.
		 * @param selectionType The new selection type.
		 */
		internal static void GUI_ChangeSelectionType(ushort selectionType)
		{
			Screen oldScreenID;

			if (selectionType == (ushort)SelectionType.UNIT && CUnit.g_unitSelected == null)
			{
				selectionType = (ushort)SelectionType.STRUCTURE;
			}

			if (selectionType == (ushort)SelectionType.STRUCTURE && CUnit.g_unitSelected != null)
			{
				CUnit.g_unitSelected = null;
			}

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);

			if (CSharpDune.g_selectionType != selectionType)
			{
				var oldSelectionType = CSharpDune.g_selectionType;

				Timer.Timer_SetTimer(TimerType.TIMER_GAME, false);

				CSharpDune.g_selectionType = selectionType;
				CSharpDune.g_selectionTypeNew = selectionType;
				g_var_37B8 = true;

				switch (oldSelectionType)
				{
					case (ushort)SelectionType.PLACE:
					case (ushort)SelectionType.TARGET:
					case (ushort)SelectionType.STRUCTURE:
						if (oldSelectionType == (ushort)SelectionType.PLACE)
							Map.Map_SetSelection(CStructure.g_structureActivePosition);

						g_cursorDefaultSpriteID = 0;
						GUI_DisplayText(null, -1);
						break;

					case (ushort)SelectionType.UNIT:
						if (CUnit.g_unitSelected != null && selectionType != (ushort)SelectionType.TARGET && selectionType != (ushort)SelectionType.UNIT)
						{
							CUnit.Unit_UpdateMap(2, CUnit.g_unitSelected);
							CUnit.g_unitSelected = null;
						}
						break;

					default:
						break;
				}

				if (g_table_selectionType[oldSelectionType].variable_04 && g_table_selectionType[selectionType].variable_06)
				{
					CSharpDune.g_viewport_forceRedraw = true;
					CSharpDune.g_viewport_fadein = true;

					GUI_DrawInterfaceAndRadar(Screen.NO0);
				}

				CWidget.Widget_SetCurrentWidget(g_table_selectionType[selectionType].defaultWidget);

				if (CWidget.g_curWidgetIndex != 0)
				{
					WidgetDraw.GUI_Widget_DrawBorder(CWidget.g_curWidgetIndex, 0, false);
				}

				if (selectionType != (ushort)SelectionType.MENTAT)
				{
					var w = CWidget.g_widgetLinkedListHead;

					while (w != null)
					{
						var s = g_table_selectionType[selectionType].visibleWidgets;
						var sPointer = 0;

						w.state.selected = false;
						w.flags.invisible = true;

						for (; s[sPointer] != -1; sPointer++)
						{
							if (s[sPointer] == w.index)
							{
								w.flags.invisible = false;
								break;
							}
						}

						CWidget.GUI_Widget_Draw(w);
						w = CWidget.GUI_Widget_GetNext(w);
					}

					WidgetDraw.GUI_Widget_DrawAll(CWidget.g_widgetLinkedListHead);
					g_textDisplayNeedsUpdate = true;
				}

				switch (CSharpDune.g_selectionType)
				{
					case (ushort)SelectionType.MENTAT:
						if (oldSelectionType != (ushort)SelectionType.INTRO)
						{
							g_cursorSpriteID = 0;

							Sprites.Sprites_SetMouseSprite(0, 0, Sprites.g_sprites[0]);
						}

						CWidget.Widget_SetCurrentWidget(g_table_selectionType[selectionType].defaultWidget);
						break;

					case (ushort)SelectionType.TARGET:
						CStructure.g_structureActivePosition = g_selectionPosition;
						WidgetDraw.GUI_Widget_ActionPanel_Draw(true);

						g_cursorDefaultSpriteID = 5;

						Timer.Timer_SetTimer(TimerType.TIMER_GAME, true);
						break;

					case (ushort)SelectionType.PLACE:
						CUnit.Unit_Select(null);
						WidgetDraw.GUI_Widget_ActionPanel_Draw(true);

						Map.Map_SetSelectionSize(g_table_structureInfo[CStructure.g_structureActiveType].layout);

						Timer.Timer_SetTimer(TimerType.TIMER_GAME, true);
						break;

					case (ushort)SelectionType.UNIT:
						WidgetDraw.GUI_Widget_ActionPanel_Draw(true);

						Timer.Timer_SetTimer(TimerType.TIMER_GAME, true);
						break;

					case (ushort)SelectionType.STRUCTURE:
						WidgetDraw.GUI_Widget_ActionPanel_Draw(true);

						Timer.Timer_SetTimer(TimerType.TIMER_GAME, true);
						break;

					default: break;
				}
			}

			Gfx.GFX_Screen_SetActive(oldScreenID);
		}

		/*
		 * Show the mouse on the screen. Copy the screen behind the mouse in a safe
		 * buffer.
		 */
		internal static void GUI_Mouse_Show()
		{
			int left, top;

			if (Mouse.g_mouseDisabled == 1) return;
			if (Mouse.g_mouseHiddenDepth == 0 || --Mouse.g_mouseHiddenDepth != 0) return;

			left = Mouse.g_mouseX - g_mouseSpriteHotspotX;
			top = Mouse.g_mouseY - g_mouseSpriteHotspotY;

			s_mouseSpriteLeft = (ushort)((left < 0) ? 0 : (left >> 3));
			s_mouseSpriteTop = (ushort)((top < 0) ? 0 : top);

			s_mouseSpriteWidth = g_mouseWidth;
			if ((left >> 3) + g_mouseWidth >= Gfx.SCREEN_WIDTH / 8) s_mouseSpriteWidth -= (ushort)((left >> 3) + g_mouseWidth - Gfx.SCREEN_WIDTH / 8);

			s_mouseSpriteHeight = g_mouseHeight;
			if (top + g_mouseHeight >= Gfx.SCREEN_HEIGHT) s_mouseSpriteHeight -= (ushort)(top + g_mouseHeight - Gfx.SCREEN_HEIGHT);

			if (Sprites.g_mouseSpriteBuffer != null)
			{
				Gfx.GFX_CopyToBuffer((short)(s_mouseSpriteLeft * 8), (short)s_mouseSpriteTop, (ushort)(s_mouseSpriteWidth * 8), s_mouseSpriteHeight, Sprites.g_mouseSpriteBuffer);
			}

			GUI_DrawSprite(Screen.NO0, Sprites.g_mouseSprite, (short)left, (short)top, 0, 0);
		}

		/*
		 * Hide the mouse from the screen. Do this by copying the mouse buffer back to
		 * the screen.
		 */
		internal static void GUI_Mouse_Hide()
		{
			if (Mouse.g_mouseDisabled == 1) return;

			if (Mouse.g_mouseHiddenDepth == 0 && s_mouseSpriteWidth != 0)
			{
				if (Sprites.g_mouseSpriteBuffer != null)
				{
					Gfx.GFX_CopyFromBuffer((short)(s_mouseSpriteLeft * 8), (short)s_mouseSpriteTop, (ushort)(s_mouseSpriteWidth * 8), s_mouseSpriteHeight, Sprites.g_mouseSpriteBuffer);
				}

				s_mouseSpriteWidth = 0;
			}

			Mouse.g_mouseHiddenDepth++;
		}

		/*
		 * Display a hint to the user. Only show each hint exactly once.
		 *
		 * @param stringID The string of the hint to show.
		 * @param spriteID The sprite to show with the hint.
		 * @return Zero or the return value of GUI_DisplayModalMessage.
		 */
		internal static ushort GUI_DisplayHint(ushort stringID, ushort spriteID)
		{
			//uint hintsShown;
			uint mask;
			ushort hint;

			if (CSharpDune.g_debugGame || stringID == (ushort)Text.STR_NULL || Config.g_gameConfig.hints == 0 || CSharpDune.g_selectionType == (ushort)SelectionType.MENTAT) return 0;

			hint = (ushort)(stringID - Text.STR_YOU_MUST_BUILD_A_WINDTRAP_TO_PROVIDE_POWER_TO_YOUR_BASE_WITHOUT_POWER_YOUR_STRUCTURES_WILL_DECAY);

			Debug.Assert(hint < 64);

			if (hint < 32)
			{
				mask = (uint)(1 << hint);
				if ((CSharpDune.g_hintsShown1 & mask) != 0) return 0;
				CSharpDune.g_hintsShown1 |= mask;
			}
			else
			{
				mask = (uint)(1 << (hint - 32));
				if ((CSharpDune.g_hintsShown2 & mask) != 0) return 0;
				CSharpDune.g_hintsShown2 |= mask;
			}

			//if ((hintsShown & mask) != 0) return 0;
			//hintsShown |= mask;

			return GUI_DisplayModalMessage(CString.String_Get_ByIndex(stringID), spriteID);
		}

		static string textBuffer; //char[768]
		/*
		 * Displays a message and waits for a user action.
		 * @param str The text to display.
		 * @param spriteID The sprite to draw (0xFFFF for none).
		 * @param ... The args for the text.
		 * @return ??
		 */
		internal static ushort GUI_DisplayModalMessage(string str, uint spriteID, params object[] ap)
		{
			ushort oldWidgetId;
			ushort ret;
			Screen oldScreenID;
			byte[] screenBackup;

			textBuffer = new StringBuilder().AppendFormat(CSharpDune.Culture, str, ap).ToString(); //vsnprintf(textBuffer, sizeof(textBuffer), str, ap);

			GUI_Mouse_Hide_Safe();

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO0);

			GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x22);

			oldWidgetId = CWidget.Widget_SetCurrentWidget(1);

			CWidget.g_widgetProperties[1].height = (ushort)(CFont.g_fontCurrent.height * Max(GUI_SplitText(ref textBuffer, (ushort)(((CWidget.g_curWidgetWidth - ((spriteID == 0xFFFF) ? 2 : 7)) << 3) - 6), '\r'), (ushort)3) + 18);

			CWidget.Widget_SetCurrentWidget(1);

			screenBackup = new byte[Gfx.GFX_GetSize((short)(CWidget.g_curWidgetWidth * 8), (short)CWidget.g_curWidgetHeight)];
			//malloc(Gfx.GFX_GetSize((short)(CWidget.g_curWidgetWidth * 8), (short)CWidget.g_curWidgetHeight));

			if (screenBackup != null)
			{
				Gfx.GFX_CopyToBuffer((short)(CWidget.g_curWidgetXBase * 8), (short)CWidget.g_curWidgetYBase, (ushort)(CWidget.g_curWidgetWidth * 8), CWidget.g_curWidgetHeight, screenBackup);
			}

			WidgetDraw.GUI_Widget_DrawBorder(1, 1, true /*1*/);

			if (spriteID != 0xFFFF)
			{
				GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[spriteID], 7, 8, 1, DRAWSPRITE_FLAG_WIDGETPOS);
				GUI_Widget_SetProperties(1, (ushort)(CWidget.g_curWidgetXBase + 5), (ushort)(CWidget.g_curWidgetYBase + 8), (ushort)(CWidget.g_curWidgetWidth - 7), (ushort)(CWidget.g_curWidgetHeight - 16));
			}
			else
			{
				GUI_Widget_SetProperties(1, (ushort)(CWidget.g_curWidgetXBase + 1), (ushort)(CWidget.g_curWidgetYBase + 8), (ushort)(CWidget.g_curWidgetWidth - 2), (ushort)(CWidget.g_curWidgetHeight - 16));
			}

			CWidget.g_curWidgetFGColourNormal = 0;

			GUI_DrawText(textBuffer, (short)(CWidget.g_curWidgetXBase << 3), (short)CWidget.g_curWidgetYBase, CWidget.g_curWidgetFGColourBlink, CWidget.g_curWidgetFGColourNormal);

			Gfx.GFX_SetPalette(Gfx.g_palette1);

			GUI_Mouse_Show_Safe();

			for (Timer.g_timerTimeout = 30; Timer.g_timerTimeout != 0; Sleep.sleepIdle())
			{
				GUI_PaletteAnimate();
			}

			Input.Input.Input_History_Clear();

			do
			{
				GUI_PaletteAnimate();

				ret = Input.Input.Input_WaitForValidInput();
				Sleep.sleepIdle();
			} while (ret == 0 || (ret & 0x800) != 0);

			Input.Input.Input_HandleInput(0x841);

			GUI_Mouse_Hide_Safe();

			if (spriteID != 0xFFFF)
			{
				GUI_Widget_SetProperties(1, (ushort)(CWidget.g_curWidgetXBase - 5), (ushort)(CWidget.g_curWidgetYBase - 8), (ushort)(CWidget.g_curWidgetWidth + 7), (ushort)(CWidget.g_curWidgetHeight + 16));
			}
			else
			{
				GUI_Widget_SetProperties(1, (ushort)(CWidget.g_curWidgetXBase - 1), (ushort)(CWidget.g_curWidgetYBase - 8), (ushort)(CWidget.g_curWidgetWidth + 2), (ushort)(CWidget.g_curWidgetHeight + 16));
			}

			if (screenBackup != null)
			{
				Gfx.GFX_CopyFromBuffer((short)(CWidget.g_curWidgetXBase * 8), (short)CWidget.g_curWidgetYBase, (ushort)(CWidget.g_curWidgetWidth * 8), CWidget.g_curWidgetHeight, screenBackup);
			}

			CWidget.Widget_SetCurrentWidget(oldWidgetId);

			if (screenBackup != null)
			{
				screenBackup = null; //free(screenBackup);
			}
			else
			{
				CSharpDune.g_viewport_forceRedraw = true;
			}

			Gfx.GFX_Screen_SetActive(oldScreenID);

			GUI_Mouse_Show_Safe();

			return ret;
		}

		static uint displayTimer;          /* Timeout value for next update of the display. */
        static ushort textOffset;              /* Vertical position of text being scrolled. */
		static bool scrollInProgress;          /* Text is being scrolled (and partly visible to the user). */
		static string displayLine1; //char[80] /* Current line being displayed. */
		static string displayLine2; //char[80] /* Next line (if scrollInProgress, it is scrolled up). */
		static string displayLine3; //char[80] /* Next message to display (after scrolling next line has finished). */
		static short line1Importance;          /* Importance of the displayed line of text. */
		static short line2Importance;          /* Importance of the next line of text. */
		static short line3Importance;          /* Importance of the next message. */
		static byte fgColour1;                 /* Foreground colour current line. */
		static byte fgColour2;                 /* Foreground colour next line. */
		static byte fgColour3;                 /* Foreground colour next message. */
		/*
		 * Display a text.
		 * @param str The text to display. If \c NULL, update the text display (scroll text, and/or remove it on time out).
		 * @param importance Importance of the new text. Value \c -1 means remove all text lines, \c -2 means drop all texts in buffer but not yet displayed.
		 *                   Otherwise, it is the importance of the message (if supplied). Higher numbers mean displayed sooner.
		 * @param ... The args for the text.
		 */
		internal static void GUI_DisplayText(string str, int importance, params object[] ap)
		{
			string buffer; //char[80]		  /* Formatting buffer of new message. */

			buffer = string.Empty; //buffer[0] = '\0';

			if (str != null)
			{
				buffer = new StringBuilder().AppendFormat(CSharpDune.Culture, str, ap).ToString(); //vsnprintf(buffer, sizeof(buffer), str, ap);
			}

			if (importance == -1) /* Remove all displayed lines. */
			{
				line1Importance = -1;
				line2Importance = -1;
				line3Importance = -1;

				displayLine1 = string.Empty; //displayLine1[0] = '\0';
				displayLine2 = string.Empty; //displayLine2[0] = '\0';
				displayLine3 = string.Empty; //displayLine3[0] = '\0';

				scrollInProgress = false;
				displayTimer = 0;
				return;
			}

			if (importance == -2) /* Remove next line and next message. */
			{
				if (!scrollInProgress)
				{
					line2Importance = -1;
					displayLine2 = string.Empty; //displayLine2[0] = '\0';
				}
				line3Importance = -1;
				displayLine3 = string.Empty; //displayLine3[0] = '\0';
			}

			if (scrollInProgress)
			{
				ushort oldWidgetId;
				ushort height;

				if (buffer != string.Empty) //buffer[0] != '\0'
				{
					if (string.Compare(buffer, displayLine2, StringComparison.OrdinalIgnoreCase) != 0 && importance >= line3Importance) //strcasecmp(buffer, displayLine2) != 0
					{
						displayLine3 = buffer; //strncpy(displayLine3, buffer, sizeof(displayLine3));
						line3Importance = (short)importance;
					}
				}
				if (displayTimer > Timer.g_timerGUI) return;

				oldWidgetId = CWidget.Widget_SetCurrentWidget(7);

				if (g_textDisplayNeedsUpdate)
				{
					var oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);

					GUI_DrawFilledRectangle(0, 0, Gfx.SCREEN_WIDTH - 1, 23, CWidget.g_curWidgetFGColourNormal);

					GUI_DrawText_Wrapper(displayLine2, (short)(CWidget.g_curWidgetXBase << 3), 2, fgColour2, 0, 0x012);
					GUI_DrawText_Wrapper(displayLine1, (short)(CWidget.g_curWidgetXBase << 3), 13, fgColour1, 0, 0x012);

					g_textDisplayNeedsUpdate = false;

					Gfx.GFX_Screen_SetActive(oldScreenID);
				}

				GUI_Mouse_Hide_InWidget(7);

				if (textOffset + CWidget.g_curWidgetHeight > 24)
				{
					height = (ushort)(24 - textOffset);
				}
				else
				{
					height = CWidget.g_curWidgetHeight;
				}

				GUI_Screen_Copy((short)CWidget.g_curWidgetXBase, (short)textOffset, (short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetWidth, (short)height, Screen.NO1, Screen.NO0);
				GUI_Mouse_Show_InWidget();

				CWidget.Widget_SetCurrentWidget(oldWidgetId);

				if (textOffset != 0)
				{
					if (line3Importance <= line2Importance)
					{
						displayTimer = Timer.g_timerGUI + 1;
					}
					textOffset--;
					return;
				}

				/* Finished scrolling, move line 2 to line 1. */
				displayLine1 = displayLine2; //strncpy(displayLine1, displayLine2, sizeof(displayLine1));
				fgColour1 = fgColour2;
				line1Importance = (short)((line2Importance != 0) ? line2Importance - 1 : 0);

				/* And move line 3 to line 2. */
				displayLine2 = displayLine3; //strncpy(displayLine2, displayLine3, sizeof(displayLine2));
				line2Importance = line3Importance;
				fgColour2 = fgColour3;
				displayLine3 = string.Empty; //displayLine3[0] = '\0';

				line3Importance = -1;
				g_textDisplayNeedsUpdate = true;
				displayTimer = (uint)(Timer.g_timerGUI + (line2Importance <= line1Importance ? 900 : 1));
				scrollInProgress = false;
				return;
			}

			if (buffer != string.Empty) //buffer[0] != '\0'
			{
				/* If new line arrived, different from every line that is in the display buffers, and more important than existing messages,
				 * insert it at the right place.
				 */
				if (string.Compare(buffer, displayLine1, StringComparison.OrdinalIgnoreCase) != 0 && //strcasecmp(buffer, displayLine1) != 0
					string.Compare(buffer, displayLine2, StringComparison.OrdinalIgnoreCase) != 0 && //strcasecmp(buffer, displayLine2) != 0
					string.Compare(buffer, displayLine3, StringComparison.OrdinalIgnoreCase) != 0) //strcasecmp(buffer, displayLine3) != 0
				{
					if (importance >= line2Importance)
					{
						/* Move line 2 to line 2 to make room for the new line. */
						displayLine3 = displayLine2; //strncpy(displayLine3, displayLine2, sizeof(displayLine3));
						fgColour3 = fgColour2;
						line3Importance = line2Importance;
						/* Copy new line to line 2. */
						displayLine2 = buffer; //strncpy(displayLine2, buffer, sizeof(displayLine2));
						fgColour2 = 12;
						line2Importance = (short)importance;

					}
					else if (importance >= line3Importance)
					{
						/* Copy new line to line 3. */
						displayLine3 = buffer; //strncpy(displayLine3, buffer, sizeof(displayLine3));
						line3Importance = (short)importance;
						fgColour3 = 12;
					}
				}
			}
			else
			{
				if (displayLine1 == string.Empty && displayLine2 == string.Empty) return; //displayLine1[0] == '\0' && displayLine2[0] == '\0'
			}

			if (line2Importance <= line1Importance && displayTimer >= Timer.g_timerGUI) return;

			scrollInProgress = true;
			textOffset = 10;
			displayTimer = 0;
		}

		static short displayedarg12low = -1;
		static short displayedarg2mid = -1;
		/*
		 * Draw a string to the screen, and so some magic.
		 *
		 * @param string The string to draw.
		 * @param left The most left position where to draw the string.
		 * @param top The most top position where to draw the string.
		 * @param fgColour The foreground colour of the text.
		 * @param bgColour The background colour of the text.
		 * @param flags The flags of the string.
		 *
		 * flags :
		 * 0x0001 : font 6p
		 * 0x0002 : font 8p
		 * 0x0010 : style ?
		 * 0x0020 : style ?
		 * 0x0030 : style ?
		 * 0x0040 : style ?
		 * 0x0100 : align center
		 * 0x0200 : align right
		 */
		internal static void GUI_DrawText_Wrapper(string str, short left, short top, byte fgColour, byte bgColour, int flags, params object[] ap)
		{
			string textBuffer; //char[240]

			var arg12low = (byte)(flags & 0x0F);   /* font : 1 => 6p, 2 => 8p */
			var arg2mid = (byte)(flags & 0xF0);    /* style */

			if ((arg12low != displayedarg12low && arg12low != 0) || str == null)
			{
				switch (arg12low)
				{
					case 1: CFont.Font_Select(CFont.g_fontNew6p); break;
					case 2: CFont.Font_Select(CFont.g_fontNew8p); break;
					default: CFont.Font_Select(CFont.g_fontNew8p); break;
				}

				displayedarg12low = arg12low;
			}

			if ((arg2mid != displayedarg2mid && arg2mid != 0) || str == null)
			{
				var colours = new byte[16];
				//memset(colours, 0, sizeof(colours));

				switch (arg2mid)
				{
					case 0x0010:
						colours[2] = 0;
						colours[3] = 0;
						CFont.g_fontCharOffset = -2;
						break;

					case 0x0020:
						colours[2] = 12;
						colours[3] = 0;
						CFont.g_fontCharOffset = -1;
						break;

					case 0x0030:
						colours[2] = 12;
						colours[3] = 12;
						CFont.g_fontCharOffset = -1;
						break;

					case 0x0040:
						colours[2] = 232;
						colours[3] = 0;
						CFont.g_fontCharOffset = -1;
						break;
				}

				colours[0] = bgColour;
				colours[1] = fgColour;
				colours[4] = 6;

				GUI_InitColors(colours, 0, /*lengthof(colours)*/(byte)(colours.Length - 1));

				displayedarg2mid = arg2mid;
			}

			if (str == null) return;

			textBuffer = new StringBuilder().AppendFormat(CSharpDune.Culture, str, ap).ToString(); //vsnprintf(textBuffer, sizeof(textBuffer), string, ap);

			switch (flags & 0x0F00)
			{
				case 0x100:
					left -= (short)(CFont.Font_GetStringWidth(textBuffer) / 2);
					break;

				case 0x200:
					left -= (short)CFont.Font_GetStringWidth(textBuffer);
					break;
			}

			GUI_DrawText(textBuffer, left, top, fgColour, bgColour);
		}

		/*
		 * Sets the activity description to the correct string for the active structure.
		 * @see g_productionStringID
		 */
		internal static void GUI_UpdateProductionStringID()
		{
			var s = CStructure.Structure_Get_ByPackedTile(g_selectionPosition);

			if (s == null) return;

			g_productionStringID = (ushort)Text.STR_NULL;

			if (!g_table_structureInfo[s.o.type].o.flags.factory)
			{
				if (s.o.type == (byte)StructureType.STRUCTURE_PALACE) g_productionStringID = (ushort)(Text.STR_LAUNCH + g_table_houseInfo[s.o.houseID].specialWeapon - 1);
				return;
			}

			if (s.o.flags.upgrading)
			{
				g_productionStringID = (ushort)Text.STR_UPGRADINGD_DONE;
				return;
			}

			if (s.o.linkedID == 0xFF)
			{
				g_productionStringID = (ushort)Text.STR_BUILD_IT;
				return;
			}

			if (s.o.flags.onHold)
			{
				g_productionStringID = (ushort)Text.STR_ON_HOLD;
				return;
			}

			if (s.countDown != 0)
			{
				g_productionStringID = (ushort)Text.STR_D_DONE;
				return;
			}

			if (s.o.type == (byte)StructureType.STRUCTURE_CONSTRUCTION_YARD)
			{
				g_productionStringID = (ushort)Text.STR_PLACE_IT;
				return;
			}

			g_productionStringID = (ushort)Text.STR_COMPLETED;
		}

		/*
		 * Sets the colours to be used when drawing chars.
		 * @param colours The colours to use.
		 * @param min The index of the first colour to set.
		 * @param max The index of the last colour to set.
		 */
		internal static void GUI_InitColors(byte[] colours, byte first, byte last)
		{
			byte i;

			first &= 0xF;
			last &= 0xF;

			if (last < first || colours == null) return;

			for (i = first; i < last + 1; i++) g_colours[i] = colours[i];
		}

		/*
		 * Show the mouse if needed. Should be used in combination with
		 *  #GUI_Mouse_Hide_InRegion().
		 */
		internal static void GUI_Mouse_Show_InRegion()
		{
			byte counter;

			while (Mouse.g_mouseLock != 0) Sleep.sleepIdle();
			Mouse.g_mouseLock++;

			counter = (byte)(Mouse.g_regionFlags & 0xFF);
			if (counter == 0 || --counter != 0)
			{
				Mouse.g_regionFlags = (ushort)((Mouse.g_regionFlags & 0xFF00) | (counter & 0xFF));
				Mouse.g_mouseLock--;
				return;
			}

			if ((Mouse.g_regionFlags & 0x4000) != 0)
			{
				GUI_Mouse_Show();
			}

			Mouse.g_regionFlags = 0;
			Mouse.g_mouseLock--;
		}

		/*
		 * Hide the mouse when it is inside the specified region. Works with
		 *  #GUI_Mouse_Show_InRegion(), which only calls #GUI_Mouse_Show() when
		 *  mouse was really hidden.
		 */
		internal static void GUI_Mouse_Hide_InRegion(ushort left, ushort top, ushort right, ushort bottom)
		{
			int minx, miny;
			int maxx, maxy;

			minx = left - ((g_mouseWidth - 1) << 3) + g_mouseSpriteHotspotX;
			if (minx < 0) minx = 0;

			miny = top - g_mouseHeight + g_mouseSpriteHotspotY;
			if (miny < 0) miny = 0;

			maxx = right + g_mouseSpriteHotspotX;
			if (maxx > Gfx.SCREEN_WIDTH - 1) maxx = Gfx.SCREEN_WIDTH - 1;

			maxy = bottom + g_mouseSpriteHotspotY;
			if (maxy > Gfx.SCREEN_HEIGHT - 1) maxy = Gfx.SCREEN_HEIGHT - 1;

			while (Mouse.g_mouseLock != 0) Sleep.sleepIdle();
			Mouse.g_mouseLock++;

			if (Mouse.g_regionFlags == 0)
			{
				Mouse.g_regionMinX = (ushort)minx;
				Mouse.g_regionMinY = (ushort)miny;
				Mouse.g_regionMaxX = (ushort)maxx;
				Mouse.g_regionMaxY = (ushort)maxy;
			}

			if (minx > Mouse.g_regionMinX) Mouse.g_regionMinX = (ushort)minx;
			if (miny > Mouse.g_regionMinY) Mouse.g_regionMinY = (ushort)miny;
			if (maxx < Mouse.g_regionMaxX) Mouse.g_regionMaxX = (ushort)maxx;
			if (maxy < Mouse.g_regionMaxY) Mouse.g_regionMaxY = (ushort)maxy;

			if ((Mouse.g_regionFlags & 0x4000) == 0 &&
				 Mouse.g_mouseX >= Mouse.g_regionMinX &&
				 Mouse.g_mouseX <= Mouse.g_regionMaxX &&
				 Mouse.g_mouseY >= Mouse.g_regionMinY &&
				 Mouse.g_mouseY <= Mouse.g_regionMaxY)
			{
				GUI_Mouse_Hide();

				Mouse.g_regionFlags |= 0x4000;
			}

			Mouse.g_regionFlags |= 0x8000;
			Mouse.g_regionFlags = (ushort)((Mouse.g_regionFlags & 0xFF00) | (((Mouse.g_regionFlags & 0x00FF) + 1) & 0xFF));

			Mouse.g_mouseLock--;
		}

		/*
		 * Draw a string to the screen.
		 *
		 * @param string The string to draw.
		 * @param left The most left position where to draw the string.
		 * @param top The most top position where to draw the string.
		 * @param fgColour The foreground colour of the text.
		 * @param bgColour The background colour of the text.
		 */
		internal static void GUI_DrawText(string str, short left, short top, byte fgColour, byte bgColour)
		{
			var colours = new byte[2];
			ushort x;
			ushort y;
			string s;

			if (CFont.g_fontCurrent == null) return;

			if (left < 0) left = 0;
			if (top < 0) top = 0;
			if (left > Gfx.SCREEN_WIDTH) return;
			if (top > Gfx.SCREEN_HEIGHT) return;

			colours[0] = bgColour;
			colours[1] = fgColour;

			GUI_InitColors(colours, 0, 1);

			s = str;
			x = (ushort)left;
			y = (ushort)top;
			var i = 0;
			while (i < s.Length) //while (*s != '\0') {
			{
				ushort width;

				if (s[i] == '\n' || s[i] == '\r')
				{
					x = (ushort)left;
					y += CFont.g_fontCurrent.height;

					while (s[i] == '\n' || s[i] == '\r') i++;
				}

				width = CFont.Font_GetCharWidth(s[i]);

				if (x + width > Gfx.SCREEN_WIDTH)
				{
					x = (ushort)left;
					y += CFont.g_fontCurrent.height;
				}
				if (y > Gfx.SCREEN_HEIGHT) break;

				GUI_DrawChar(s[i], x, y);

				x += width;
				i++;
			}
		}

		/*
		 * Draw a char on the screen.
		 *
		 * @param c The char to draw.
		 * @param x The most left position where to draw the string.
		 * @param y The most top position where to draw the string.
		 */
		static void GUI_DrawChar(char c, ushort x, ushort y)
		{
			var screen = (byte[])Gfx.GFX_Screen_GetActive();

			FontChar fc;

			ushort remainingWidth;
			byte i;
			byte j;

			if (CFont.g_fontCurrent == null) return;

			fc = CFont.g_fontCurrent.chars[c];
			if (fc.data == null) return;

			if (x >= Gfx.SCREEN_WIDTH || (x + fc.width) > Gfx.SCREEN_WIDTH) return;
			if (y >= Gfx.SCREEN_HEIGHT || (y + CFont.g_fontCurrent.height) > Gfx.SCREEN_HEIGHT) return;

			Gfx.GFX_Screen_SetDirty(Screen.ACTIVE, x, y, (ushort)(x + fc.width), (ushort)(y + CFont.g_fontCurrent.height));
			x += (ushort)(y * Gfx.SCREEN_WIDTH);
			remainingWidth = (ushort)(Gfx.SCREEN_WIDTH - fc.width);

			if (g_colours[0] != 0)
			{
				for (j = 0; j < fc.unusedLines; j++)
				{
					for (i = 0; i < fc.width; i++) screen[x++] = g_colours[0];
					x += remainingWidth;
				}
			}
			else
			{
				x += (ushort)(fc.unusedLines * Gfx.SCREEN_WIDTH);
			}

			if (fc.usedLines == 0) return;

			for (j = 0; j < fc.usedLines; j++)
			{
				for (i = 0; i < fc.width; i++)
				{
					var data = fc.data[j * fc.width + i];

					if (g_colours[data & 0xF] != 0) screen[x] = g_colours[data & 0xF];
					x++;
				}
				x += remainingWidth;
			}

			if (g_colours[0] == 0) return;

			for (j = (byte)(fc.unusedLines + fc.usedLines); j < CFont.g_fontCurrent.height; j++)
			{
				for (i = 0; i < fc.width; i++) screen[x++] = g_colours[0];
				x += remainingWidth;
			}
		}

		/*
		 * Draws a chess-pattern filled rectangle.
		 * @param left The X-position of the rectangle.
		 * @param top The Y-position of the rectangle.
		 * @param width The width of the rectangle.
		 * @param height The height of the rectangle.
		 * @param colour The colour of the rectangle.
		 */
		internal static void GUI_DrawBlockedRectangle(short left, short top, short width, short height, byte colour)
		{
			byte[] screen;
			var screenPointer = 0;

			if (width <= 0) return;
			if (height <= 0) return;
			if (left >= Gfx.SCREEN_WIDTH) return;
			if (top >= Gfx.SCREEN_HEIGHT) return;

			if (left < 0)
			{
				if (left + width <= 0) return;
				width += left;
				left = 0;
			}
			if (top < 0)
			{
				if (top + height <= 0) return;
				height += top;
				top = 0;
			}

			if (left + width >= Gfx.SCREEN_WIDTH)
			{
				width = (short)(Gfx.SCREEN_WIDTH - left);
			}
			if (top + height >= Gfx.SCREEN_HEIGHT)
			{
				height = (short)(Gfx.SCREEN_HEIGHT - top);
			}

			screen = (byte[])Gfx.GFX_Screen_GetActive();
			screenPointer += (ushort)(top * Gfx.SCREEN_WIDTH + left);

			for (; height > 0; height--)
			{
				int i = width;

				if ((height & 1) != (width & 1))
				{
					screenPointer++;
					i--;
				}

				for (; i > 0; i -= 2)
				{
					screen[screenPointer] = colour;
					screenPointer += 2;
				}

				screenPointer += (ushort)(Gfx.SCREEN_WIDTH - width - (height & 1));
			}
		}

		/*
		 * Draw a wired rectangle.
		 * @param left The left position of the rectangle.
		 * @param top The top position of the rectangle.
		 * @param right The right position of the rectangle.
		 * @param bottom The bottom position of the rectangle.
		 * @param colour The colour of the rectangle.
		 */
		internal static void GUI_DrawWiredRectangle(ushort left, ushort top, ushort right, ushort bottom, byte colour)
		{
			GUI_DrawLine((short)left, (short)top, (short)right, (short)top, colour);
			GUI_DrawLine((short)left, (short)bottom, (short)right, (short)bottom, colour);
			GUI_DrawLine((short)left, (short)top, (short)left, (short)bottom, colour);
			GUI_DrawLine((short)right, (short)top, (short)right, (short)bottom, colour);

			Gfx.GFX_Screen_SetDirty(Screen.ACTIVE, left, top, (ushort)(right + 1), (ushort)(bottom + 1));
		}

		/*
		 * Draw a filled rectangle using xor.
		 * @param left The left position of the rectangle.
		 * @param top The top position of the rectangle.
		 * @param right The right position of the rectangle.
		 * @param bottom The bottom position of the rectangle.
		 * @param colour The colour of the rectangle.
		 */
		internal static void GUI_DrawXorFilledRectangle(short left, short top, short right, short bottom, byte colour)
		{
			ushort x;
			ushort y;
			ushort height;
			ushort width;

			var screen = (byte[])Gfx.GFX_Screen_GetActive();
			var screenPointer = 0;

			if (left >= Gfx.SCREEN_WIDTH) return;
			if (left < 0) left = 0;

			if (top >= Gfx.SCREEN_HEIGHT) return;
			if (top < 0) top = 0;

			if (right >= Gfx.SCREEN_WIDTH) right = Gfx.SCREEN_WIDTH - 1;
			if (right < 0) right = 0;

			if (bottom >= Gfx.SCREEN_HEIGHT) bottom = Gfx.SCREEN_HEIGHT - 1;
			if (bottom < 0) bottom = 0;

			if (left > right) return;
			if (top > bottom) return;

			screenPointer += (ushort)(left + top * Gfx.SCREEN_WIDTH);
			width = (ushort)(right - left + 1);
			height = (ushort)(bottom - top + 1);
			for (y = 0; y < height; y++)
			{
				for (x = 0; x < width; x++)
				{
					screen[screenPointer++] ^= colour;
				}
				screenPointer += (ushort)(Gfx.SCREEN_WIDTH - width);
			}
		}

		/*
		 * Draw a filled rectangle.
		 * @param left The left position of the rectangle.
		 * @param top The top position of the rectangle.
		 * @param right The right position of the rectangle.
		 * @param bottom The bottom position of the rectangle.
		 * @param colour The colour of the rectangle.
		 */
		internal static void GUI_DrawFilledRectangle(short left, short top, short right, short bottom, byte colour)
		{
			ushort x;
			ushort y;
			ushort height;
			ushort width;

			var screen = (byte[])Gfx.GFX_Screen_GetActive();
			var screenPointer = 0;

			if (left >= Gfx.SCREEN_WIDTH) return;
			if (left < 0) left = 0;

			if (top >= Gfx.SCREEN_HEIGHT) return;
			if (top < 0) top = 0;

			if (right >= Gfx.SCREEN_WIDTH) right = Gfx.SCREEN_WIDTH - 1;
			if (right < 0) right = 0;

			if (bottom >= Gfx.SCREEN_HEIGHT) bottom = Gfx.SCREEN_HEIGHT - 1;
			if (bottom < 0) bottom = 0;

			if (left > right) return;
			if (top > bottom) return;

			screenPointer += (ushort)(left + top * Gfx.SCREEN_WIDTH);
			width = (ushort)(right - left + 1);
			height = (ushort)(bottom - top + 1);
			for (y = 0; y < height; y++)
			{
				/* TODO : use memset() */
				for (x = 0; x < width; x++)
				{
					screen[screenPointer++] = colour;
				}
				screenPointer += (ushort)(Gfx.SCREEN_WIDTH - width);
			}

			Gfx.GFX_Screen_SetDirty(Screen.ACTIVE, (ushort)left, (ushort)top, (ushort)(right + 1), (ushort)(bottom + 1));
		}

		/*
		 * Draw a border.
		 *
		 * @param left Left position of the border.
		 * @param top Top position of the border.
		 * @param width Width of the border.
		 * @param height Height of the border.
		 * @param colourSchemaIndex Index of the colourSchema used.
		 * @param fill True if you want the border to be filled.
		 */
		internal static void GUI_DrawBorder(ushort left, ushort top, ushort width, ushort height, ushort colourSchemaIndex, bool fill)
		{
			ushort[] colourSchema;

			if (!fill) Gfx.GFX_Screen_SetDirty(Screen.ACTIVE, left, top, (ushort)(left + width), (ushort)(top + height));

			width -= 1;
			height -= 1;

			colourSchema = s_colourBorderSchema[colourSchemaIndex];

			if (fill) GUI_DrawFilledRectangle((short)left, (short)top, (short)(left + width), (short)(top + height), (byte)(colourSchema[0] & 0xFF));

			GUI_DrawLine((short)left, (short)(top + height), (short)(left + width), (short)(top + height), (byte)(colourSchema[1] & 0xFF));
			GUI_DrawLine((short)(left + width), (short)top, (short)(left + width), (short)(top + height), (byte)(colourSchema[1] & 0xFF));
			GUI_DrawLine((short)left, (short)top, (short)(left + width), (short)top, (byte)(colourSchema[2] & 0xFF));
			GUI_DrawLine((short)left, (short)top, (short)left, (short)(top + height), (byte)(colourSchema[2] & 0xFF));

			Gfx.GFX_PutPixel(left, (ushort)(top + height), (byte)(colourSchema[3] & 0xFF));
			Gfx.GFX_PutPixel((ushort)(left + width), top, (byte)(colourSchema[3] & 0xFF));
		}

		/*
		 * Draws a line from (x1, y1) to (x2, y2) using given colour.
		 * @param x1 The X-coordinate of the begin of the line.
		 * @param y1 The Y-coordinate of the begin of the line.
		 * @param x2 The X-coordinate of the end of the line.
		 * @param y2 The Y-coordinate of the end of the line.
		 * @param colour The colour to use to draw the line.
		 */
		internal static void GUI_DrawLine(short x1, short y1, short x2, short y2, byte colour)
		{
			var screen = (byte[])Gfx.GFX_Screen_GetActive();
			var screenPointer = 0;
			short increment = 1;

			if (x1 < g_clipping.left || x1 > g_clipping.right || y1 < g_clipping.top || y1 > g_clipping.bottom || x2 < g_clipping.left || x2 > g_clipping.right || y2 < g_clipping.top || y2 > g_clipping.bottom)
			{
				while (true)
				{
					var clip1 = GetNeededClipping(x1, y1);
					var clip2 = GetNeededClipping(x2, y2);

					if (clip1 == 0 && clip2 == 0) break;
					if ((clip1 & clip2) != 0) return;

					switch (clip1)
					{
						case 1: case 9: ClipTop(ref x1, ref y1, x2, y2); break;
						case 2: case 6: ClipBottom(ref x1, ref y1, x2, y2); break;
						case 4: case 5: ClipLeft(ref x1, ref y1, x2, y2); break;
						case 8: case 10: ClipRight(ref x1, ref y1, x2, y2); break;
						default:
							switch (clip2)
							{
								case 1: case 9: ClipTop(ref x2, ref y2, x1, y1); break;
								case 2: case 6: ClipBottom(ref x2, ref y2, x1, y1); break;
								case 4: case 5: ClipLeft(ref x2, ref y2, x1, y1); break;
								case 8: case 10: ClipRight(ref x2, ref y2, x1, y1); break;
								default: break;
							}
							break;
					}
				}
			}

			y2 -= y1;

			if (y2 == 0)
			{
				if (x1 >= x2)
				{
					var x = x1;
					x1 = x2;
					x2 = x;
				}

				x2 -= (short)(x1 - 1);

				screenPointer += y1 * Gfx.SCREEN_WIDTH + x1;

				Array.Fill(screen, colour, screenPointer, x2); //memset(screen, colour, x2);

				return;
			}

			if (y2 < 0)
			{
				var x = x1;
				x1 = x2;
				x2 = x;
				y2 = (short)-y2;
				y1 -= y2;
			}

			screenPointer += y1 * Gfx.SCREEN_WIDTH;

			x2 -= x1;
			if (x2 == 0)
			{
				screenPointer += x1;

				while (y2-- != 0)
				{
					screen[screenPointer] = colour;
					screenPointer += Gfx.SCREEN_WIDTH;
				}

				return;
			}

			if (x2 < 0)
			{
				x2 = (short)-x2;
				increment = -1;
			}

			if (x2 < y2)
			{
				var full = y2;
				var half = (short)(y2 / 2);
				screenPointer += x1;
				while (true)
				{
					screen[screenPointer] = colour;
					if (y2-- == 0) return;
					screenPointer += Gfx.SCREEN_WIDTH;
					half -= x2;
					if (half < 0)
					{
						half += full;
						screenPointer += increment;
					}
				}
			}
			else
			{
				var full = x2;
				var half = (short)(x2 / 2);
				screenPointer += x1;
				while (true)
				{
					screen[screenPointer] = colour;
					if (x2-- == 0) return;
					screenPointer += increment;
					half -= y2;
					if (half < 0)
					{
						half += full;
						screenPointer += Gfx.SCREEN_WIDTH;
					}
				}
			}
		}

		static readonly ushort[] l_info = { 293, 52, 24, 7, 1, 0, 0, 0, 4, 5, 8 };
		internal static void GUI_DrawProgressbar(ushort current, ushort max)
		{
			ushort width;
			ushort height;
			ushort colour;

			l_info[7] = max;
			l_info[6] = current;

			if (current > max) current = max;
			if (max < 1) max = 1;

			width = l_info[2];
			height = l_info[3];

			/* 0 = Horizontal, 1 = Vertial */
			if (l_info[5] == 0)
			{
				width = (ushort)(current * width / max);
				if (width < 1) width = 1;
			}
			else
			{
				height = (ushort)(current * height / max);
				if (height < 1) height = 1;
			}

			colour = l_info[8];
			if (current <= max / 2) colour = l_info[9];
			if (current <= max / 4) colour = l_info[10];

			if (current != 0 && width == 0) width = 1;
			if (current != 0 && height == 0) height = 1;

			if (height != 0)
			{
				GUI_DrawBorder((ushort)(l_info[0] - 1), (ushort)(l_info[1] - 1), (ushort)(l_info[2] + 2), (ushort)(l_info[3] + 2), 1, true);
			}

			if (width != 0)
			{
				GUI_DrawFilledRectangle((short)l_info[0], (short)(l_info[1] + l_info[3] - height), (short)(l_info[0] + width - 1), (short)(l_info[1] + l_info[3] - 1), (byte)colour);
			}
		}

		/*
		 * Show the mouse if needed. Should be used in combination with
		 *  GUI_Mouse_Hide_InWidget().
		 */
		internal static void GUI_Mouse_Show_InWidget() => GUI_Mouse_Show_InRegion();

		/*
		 * Hide the mouse when it is inside the specified widget. Works with
		 *  #GUI_Mouse_Show_InWidget(), which only calls #GUI_Mouse_Show() when
		 *  mouse was really hidden.
		 * @param widgetIndex The index of the widget to check on.
		 */
		internal static void GUI_Mouse_Hide_InWidget(ushort widgetIndex)
		{
			ushort left, top;
			ushort width, height;

			left = (ushort)(CWidget.g_widgetProperties[widgetIndex].xBase << 3);
			top = CWidget.g_widgetProperties[widgetIndex].yBase;
			width = (ushort)(CWidget.g_widgetProperties[widgetIndex].width << 3);
			height = CWidget.g_widgetProperties[widgetIndex].height;

			GUI_Mouse_Hide_InRegion(left, top, (ushort)(left + width - 1), (ushort)(top + height - 1));
		}

		/*
		 * Wrapper around GFX_Screen_Copy. Protects against wrong input values.
		 * @param xSrc The X-coordinate on the source divided by 8.
		 * @param ySrc The Y-coordinate on the source.
		 * @param xDst The X-coordinate on the destination divided by 8.
		 * @param yDst The Y-coordinate on the destination.
		 * @param width The width divided by 8.
		 * @param height The height.
		 * @param screenSrc The ID of the source screen.
		 * @param screenDst The ID of the destination screen.
		 */
		internal static void GUI_Screen_Copy(short xSrc, short ySrc, short xDst, short yDst, short width, short height, Screen screenSrc, Screen screenDst)
		{
			if (width > Gfx.SCREEN_WIDTH / 8) width = Gfx.SCREEN_WIDTH / 8;
			if (height > Gfx.SCREEN_HEIGHT) height = (short)Gfx.SCREEN_HEIGHT;

			if (xSrc < 0)
			{
				xDst -= xSrc;
				width += xSrc;
				xSrc = 0;
			}

			if (xSrc >= Gfx.SCREEN_WIDTH / 8 || xDst >= Gfx.SCREEN_WIDTH / 8) return;

			if (xDst < 0)
			{
				xSrc -= xDst;
				width += xDst;
				xDst = 0;
			}

			if (ySrc < 0)
			{
				yDst -= ySrc;
				height += ySrc;
				ySrc = 0;
			}

			if (yDst < 0)
			{
				ySrc -= yDst;
				height += yDst;
				yDst = 0;
			}

			Gfx.GFX_Screen_Copy((short)(xSrc * 8), ySrc, (short)(xDst * 8), yDst, (short)(width * 8), height, screenSrc, screenDst);
		}

		/*
		 * The safe version of GUI_Mouse_Hide(). It waits for a mouselock before doing
		 *  anything.
		 */
		internal static void GUI_Mouse_Hide_Safe()
		{
			while (Mouse.g_mouseLock != 0) Sleep.sleepIdle();
			if (Mouse.g_mouseDisabled == 1) return;
			Mouse.g_mouseLock++;

			GUI_Mouse_Hide();

			Mouse.g_mouseLock--;
		}

		/*
		 * The safe version of GUI_Mouse_Show(). It waits for a mouselock before doing
		 *  anything.
		 */
		internal static void GUI_Mouse_Show_Safe()
		{
			while (Mouse.g_mouseLock != 0) Sleep.sleepIdle();
			if (Mouse.g_mouseDisabled == 1) return;
			Mouse.g_mouseLock++;

			GUI_Mouse_Show();

			Mouse.g_mouseLock--;
		}

		static void GUI_Widget_SetProperties(ushort index, ushort xpos, ushort ypos, ushort width, ushort height)
		{
			CWidget.g_widgetProperties[index].xBase = xpos;
			CWidget.g_widgetProperties[index].yBase = ypos;
			CWidget.g_widgetProperties[index].width = width;
			CWidget.g_widgetProperties[index].height = height;

			if (CWidget.g_curWidgetIndex == index) CWidget.Widget_SetCurrentWidget(index);
		}

		/*
		 * Draw the interface (borders etc etc) and radar on the screen.
		 * @param screenID The screen to draw the radar on. if NO0, NO1 is used as back buffer
		 */
		internal static void GUI_DrawInterfaceAndRadar(Screen screenID)
		{
			var find = new PoolFindStruct();
			Screen oldScreenID;
			Widget w;

			oldScreenID = Gfx.GFX_Screen_SetActive((screenID == Screen.NO0) ? Screen.NO1 : screenID);

			CSharpDune.g_viewport_forceRedraw = true;

			Sprites.Sprites_LoadImage("SCREEN.CPS", Screen.NO1, null);
			GUI_DrawSprite(Screen.NO1, Sprites.g_sprites[11], 192, 0, 0, 0); /* "Credits" */

			GUI_Palette_RemapScreen(0, 0, Gfx.SCREEN_WIDTH, Gfx.SCREEN_HEIGHT, Screen.NO1, g_remap);

			g_textDisplayNeedsUpdate = true;

			Viewport.GUI_Widget_Viewport_RedrawMap(Screen.ACTIVE);

			GUI_DrawScreen(Screen.ACTIVE);

			WidgetDraw.GUI_Widget_ActionPanel_Draw(true);

			w = CWidget.GUI_Widget_Get_ByIndex(CWidget.g_widgetLinkedListHead, 1);
			CWidget.GUI_Widget_Draw(w);

			w = CWidget.GUI_Widget_Get_ByIndex(CWidget.g_widgetLinkedListHead, 2);
			CWidget.GUI_Widget_Draw(w);

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			while (true)
			{
				Structure s;

				s = PoolStructure.Structure_Find(find);
				if (s == null) break;
				if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;

				CStructure.Structure_UpdateMap(s);
			}

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.index = 0xFFFF;
			find.type = 0xFFFF;

			while (true)
			{
				Unit u;

				u = PoolUnit.Unit_Find(find);
				if (u == null) break;

				CUnit.Unit_UpdateMap(1, u);
			}

			if (screenID == Screen.NO0)
			{
				Gfx.GFX_Screen_SetActive(Screen.NO0);

				GUI_Mouse_Hide_Safe();

				GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, Screen.NO1, Screen.NO0);
				GUI_DrawCredits((byte)CHouse.g_playerHouseID, (ushort)((CHouse.g_playerCredits == 0xFFFF) ? 2 : 1));
				GUI_SetPaletteAnimated(Gfx.g_palette1, 15);

				GUI_Mouse_Show_Safe();
			}

			Gfx.GFX_Screen_SetActive(oldScreenID);

			GUI_DrawCredits((byte)CHouse.g_playerHouseID, 2);

			Input.Input.Input_History_Clear();
		}

		/*
		 * Splits the given text in lines of maxwidth width using the given delimiter.
		 * @param text The text to split.
		 * @param maxwidth The maximum width the text will have.
		 * @param delimiter The char used as delimiter.
		 * @return The number of lines.
		 */
		internal static ushort GUI_SplitText(ref string text, ushort maxwidth, char delimiter)
		{
			ushort lines = 0;
			var i = 0;

			if (text == null) return 0;

			var str = text.ToArray();

			while (i < str.Length && str[i] != '\0')
			{
				ushort width = 0;

				lines++;

				while (i < str.Length && width < maxwidth && str[i] != delimiter && str[i] != '\r' && str[i] != '\0') width += CFont.Font_GetCharWidth(str[i++]);

				if (width >= maxwidth)
				{
					while (str[i] != 0x20 && str[i] != delimiter && str[i] != '\r' && str[i] != '\0') width -= CFont.Font_GetCharWidth(str[i--]);
				}

				if (i < str.Length && str[i] != '\0') str[i++] = delimiter;
			}

			text = new string(str);

			return lines;
		}

		static uint timerAnimation;
        static uint timerSelection;
        static uint timerToggle;
        static bool animationToggle;
        static ushort selectionStateColour = 15;
		static ushort toggleColour = 12;
		/*
		 * Animate the palette. Only works for some colours or something
		 */
		internal static void GUI_PaletteAnimate()
		{
			var shouldSetPalette = false;

			if (timerAnimation < Timer.g_timerGUI)
			{
				/* make the repair button flash */
				ushort colour;

				colour = (ushort)((!g_structureHighHealth && animationToggle) ? 6 : 15);
				if (!Common.AreArraysEqual(Gfx.g_palette1, 3 * 239, Gfx.g_palette1, 3 * colour, 3))
				{ //memcmp(g_palette1 + 3 * 239, g_palette1 + 3 * colour, 3) != 0
					Array.Copy(Gfx.g_palette1, 3 * colour, Gfx.g_palette1, 3 * 239, 3); //memcpy(g_palette1 + 3 * 239, g_palette1 + 3 * colour, 3);
					shouldSetPalette = true;
				}

				animationToggle = !animationToggle;
				timerAnimation = Timer.g_timerGUI + 60;
			}

			if (timerSelection < Timer.g_timerGUI && CSharpDune.g_selectionType != (ushort)SelectionType.MENTAT)
			{
				/* selection color */
				GUI_Palette_ShiftColour(Gfx.g_palette1, 255, selectionStateColour);
				GUI_Palette_ShiftColour(Gfx.g_palette1, 255, selectionStateColour);
				GUI_Palette_ShiftColour(Gfx.g_palette1, 255, selectionStateColour);

				if (!GUI_Palette_ShiftColour(Gfx.g_palette1, 255, selectionStateColour))
				{
					if (selectionStateColour == 13)
					{
						selectionStateColour = 15;

						if (CSharpDune.g_selectionType == (ushort)SelectionType.PLACE)
						{
							if (g_selectionState != 0)
							{
								selectionStateColour = (ushort)((g_selectionState < 0) ? 5 : 15);
							}
							else
							{
								selectionStateColour = 6;
							}
						}
					}
					else
					{
						selectionStateColour = 13;
					}
				}

				shouldSetPalette = true;

				timerSelection = Timer.g_timerGUI + 3;
			}

			if (timerToggle < Timer.g_timerGUI)
			{
				/* windtrap color */
				GUI_Palette_ShiftColour(Gfx.g_palette1, 223, toggleColour);

				if (!GUI_Palette_ShiftColour(Gfx.g_palette1, 223, toggleColour))
				{
					toggleColour = (ushort)((toggleColour == 12) ? 10 : 12);
				}

				shouldSetPalette = true;

				timerToggle = Timer.g_timerGUI + 5;
			}

			if (shouldSetPalette) Gfx.GFX_SetPalette(Gfx.g_palette1);

			Sound.Sound_StartSpeech();
		}

		/*
		 * Shift the given colour toward the reference color.
		 * Increment(or decrement) each component (R, G, B) until
		 * they equal thoses of the reference color.
		 *
		 * @param palette The palette to work on.
		 * @param colour The colour to modify.
		 * @param reference The colour to use as reference.
		 * @return true if the colour now equals the reference.
		 */
		static bool GUI_Palette_ShiftColour(byte[] palette, ushort colour, ushort reference)
		{
			var ret = false;
			ushort i;

			colour *= 3;
			reference *= 3;

			for (i = 0; i < 3; i++)
			{
				if (palette[reference] != palette[colour])
				{
					ret = true;
					palette[colour] += (byte)((palette[colour] > palette[reference]) ? -1 : 1);
				}
				colour++;
				reference++;
			}

			return ret;
		}

		/*
		 * Get how the given point must be clipped.
		 * @param x The X-coordinate of the point.
		 * @param y The Y-coordinate of the point.
		 * @return A bitset.
		 */
		static ushort GetNeededClipping(short x, short y)
		{
			ushort flags = 0;

			if (y < g_clipping.top) flags |= 0x1;
			if (y > g_clipping.bottom) flags |= 0x2;
			if (x < g_clipping.left) flags |= 0x4;
			if (x > g_clipping.right) flags |= 0x8;

			return flags;
		}

		/*
		 * Applies top clipping to a line.
		 * @param x1 Pointer to the X-coordinate of the begin of the line.
		 * @param y1 Pointer to the Y-coordinate of the begin of the line.
		 * @param x2 The X-coordinate of the end of the line.
		 * @param y2 The Y-coordinate of the end of the line.
		 */
		static void ClipTop(ref short x1, ref short y1, short x2, short y2)
		{
			x1 += (short)((x2 - x1) * (g_clipping.top - y1) / (y2 - y1));
			y1 = (short)g_clipping.top;
		}

		/*
		 * Applies bottom clipping to a line.
		 * @param x1 Pointer to the X-coordinate of the begin of the line.
		 * @param y1 Pointer to the Y-coordinate of the begin of the line.
		 * @param x2 The X-coordinate of the end of the line.
		 * @param y2 The Y-coordinate of the end of the line.
		 */
		static void ClipBottom(ref short x1, ref short y1, short x2, short y2)
		{
			x1 += (short)((x2 - x1) * (y1 - g_clipping.bottom) / (y1 - y2));
			y1 = (short)g_clipping.bottom;
		}

		/*
		 * Applies left clipping to a line.
		 * @param x1 Pointer to the X-coordinate of the begin of the line.
		 * @param y1 Pointer to the Y-coordinate of the begin of the line.
		 * @param x2 The X-coordinate of the end of the line.
		 * @param y2 The Y-coordinate of the end of the line.
		 */
		static void ClipLeft(ref short x1, ref short y1, short x2, short y2)
		{
			y1 += (short)((y2 - y1) * (g_clipping.left - x1) / (x2 - x1));
			x1 = (short)g_clipping.left;
		}

		/*
		 * Applies right clipping to a line.
		 * @param x1 Pointer to the X-coordinate of the begin of the line.
		 * @param y1 Pointer to the Y-coordinate of the begin of the line.
		 * @param x2 The X-coordinate of the end of the line.
		 * @param y2 The Y-coordinate of the end of the line.
		 */
		static void ClipRight(ref short x1, ref short y1, short x2, short y2)
		{
			y1 += (short)((y2 - y1) * (x1 - g_clipping.right) / (x1 - x2));
			x1 = (short)g_clipping.right;
		}

		/*
		 * Remap all the colours in the region with the ones indicated by the remap palette.
		 * @param left The left of the region to remap.
		 * @param top The top of the region to remap.
		 * @param width The width of the region to remap.
		 * @param height The height of the region to remap.
		 * @param screenID The screen to do the remapping on.
		 * @param remap The pointer to the remap palette.
		 */
		internal static void GUI_Palette_RemapScreen(ushort left, ushort top, ushort width, ushort height, Screen screenID, byte[] remap)
		{
			var screen = Gfx.GFX_Screen_Get_ByIndex(screenID);
			var screenPointer = 0;

			screenPointer += top * Gfx.SCREEN_WIDTH + left;
			for (; height > 0; height--)
			{
				int i;
				for (i = width; i > 0; i--)
				{
					var pixel = screen[screenPointer];
					screen[screenPointer++] = remap[pixel];
				}
				screenPointer += Gfx.SCREEN_WIDTH - width;
			}
		}

		static ushort creditsAnimation;          /* How many credits are shown in current animation of credits. */
        static short creditsAnimationOffset;     /* Offset of the credits for the animation of credits. */
        /*
		 * Draw the credits on the screen, and animate it when the value is changing.
		 * @param houseID The house to display the credits from.
		 * @param mode The mode of displaying. 0 = animate, 1 = force draw, 2 = reset.
		 */
        internal static void GUI_DrawCredits(byte houseID, ushort mode)
		{
			Screen oldScreenID;
			ushort oldWidgetId;
			House h;
			string charCreditsOld; //char[7];
			string charCreditsNew; //char[7];
			int i;
			short creditsDiff;
			int creditsNew;
			int creditsOld;
			short offset;

			if (s_tickCreditsAnimation > Timer.g_timerGUI && mode == 0) return;
			s_tickCreditsAnimation = Timer.g_timerGUI + 1;

			h = PoolHouse.House_Get_ByIndex(houseID);

			if (mode == 2)
			{
				CHouse.g_playerCredits = h.credits;
				creditsAnimation = h.credits;
			}

			if (mode == 0 && h.credits == creditsAnimation && creditsAnimationOffset == 0) return;

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);

			oldWidgetId = CWidget.Widget_SetCurrentWidget(4);

			creditsDiff = (short)(h.credits - creditsAnimation);
			if (creditsDiff != 0)
			{
				var diff = (short)(creditsDiff / 4);
				if (diff == 0) diff = (short)(creditsDiff < 0 ? -1 : 1);
				if (diff > 128) diff = 128;
				if (diff < -128) diff = -128;
				creditsAnimationOffset += diff;
			}
			else
			{
				creditsAnimationOffset = 0;
			}

			if (creditsDiff != 0 && (creditsAnimationOffset < -7 || creditsAnimationOffset > 7))
			{
				CDriver.Driver_Sound_Play((short)(creditsDiff > 0 ? 52 : 53), 0xFF);
			}

			if (creditsAnimationOffset < 0 && creditsAnimation == 0) creditsAnimationOffset = 0;

			creditsAnimation += (ushort)(creditsAnimationOffset / 8);

			if (creditsAnimationOffset > 0) creditsAnimationOffset &= 7;
			if (creditsAnimationOffset < 0) creditsAnimationOffset = (short)-((-creditsAnimationOffset) & 7);

			creditsOld = creditsAnimation;
			creditsNew = creditsAnimation;
			offset = 1;

			if (creditsAnimationOffset < 0)
			{
				creditsOld -= 1;
				if (creditsOld < 0) creditsOld = 0;

				offset -= 8;
			}

			if (creditsAnimationOffset > 0)
			{
				creditsNew += 1;
			}

			GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[12], 0, 0, 4, DRAWSPRITE_FLAG_WIDGETPOS);

			CHouse.g_playerCredits = (ushort)creditsOld;

			charCreditsOld = string.Format(CSharpDune.Culture, "{0, 6}", creditsOld); //snprintf(charCreditsOld, sizeof(charCreditsOld), "%6d", creditsOld);
			charCreditsNew = string.Format(CSharpDune.Culture, "{0, 6}", creditsNew); //snprintf(charCreditsNew, sizeof(charCreditsNew), "%6d", creditsNew);

			for (i = 0; i < 6; i++)
			{
				var left = (ushort)(i * 10 + 4);
				ushort spriteID;

				spriteID = (ushort)((charCreditsOld[i] == ' ') ? 13 : charCreditsOld[i] - 34);

				if (charCreditsOld[i] != charCreditsNew[i])
				{
					GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[spriteID], (short)left, (short)(offset - creditsAnimationOffset), 4, DRAWSPRITE_FLAG_WIDGETPOS);
					if (creditsAnimationOffset == 0) continue;

					spriteID = (ushort)((charCreditsNew[i] == ' ') ? 13 : charCreditsNew[i] - 34);

					GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[spriteID], (short)left, (short)(offset + 8 - creditsAnimationOffset), 4, DRAWSPRITE_FLAG_WIDGETPOS);
				}
				else
				{
					GUI_DrawSprite(Screen.ACTIVE, Sprites.g_sprites[spriteID], (short)left, 1, 4, DRAWSPRITE_FLAG_WIDGETPOS);
				}
			}

			if (!Gfx.GFX_Screen_IsActive(oldScreenID))
			{
				GUI_Mouse_Hide_InWidget(5);
				GUI_Screen_Copy((short)CWidget.g_curWidgetXBase, (short)CWidget.g_curWidgetYBase, (short)CWidget.g_curWidgetXBase, (short)(CWidget.g_curWidgetYBase - 40), (short)CWidget.g_curWidgetWidth, (short)CWidget.g_curWidgetHeight, Screen.ACTIVE, oldScreenID);
				GUI_Mouse_Show_InWidget();
			}

			Gfx.GFX_Screen_SetActive(oldScreenID);

			CWidget.Widget_SetCurrentWidget(oldWidgetId);
		}

		static uint s_timerViewportMessage;
        /*
		 * Draw the screen.
		 * This also handles animation tick and other viewport related activity.
		 * @param screenID The screen to draw on.
		 */
        internal static void GUI_DrawScreen(Screen screenID)
		{
			var hasScrolled = false;
			Screen oldScreenID;
			ushort xpos;

			if (CSharpDune.g_selectionType == (ushort)SelectionType.MENTAT) return;
			if (CSharpDune.g_selectionType == (ushort)SelectionType.DEBUG) return;
			if (CSharpDune.g_selectionType == (ushort)SelectionType.UNKNOWN6) return;
			if (CSharpDune.g_selectionType == (ushort)SelectionType.INTRO) return;

			oldScreenID = Gfx.GFX_Screen_SetActive(screenID);

			if (!Gfx.GFX_Screen_IsActive(Screen.NO0)) CSharpDune.g_viewport_forceRedraw = true;

			CExplosion.Explosion_Tick();
			CAnimation.Animation_Tick();
			CUnit.Unit_Sort();

			if (!CSharpDune.g_viewport_forceRedraw && g_viewportPosition != g_minimapPosition)
			{
				var viewportX = CTile.Tile_GetPackedX(g_viewportPosition);
				var viewportY = CTile.Tile_GetPackedY(g_viewportPosition);
				var xOffset = (short)(CTile.Tile_GetPackedX(g_minimapPosition) - viewportX); /* Horizontal offset between viewport and minimap. */
				var yOffset = (short)(CTile.Tile_GetPackedY(g_minimapPosition) - viewportY); /* Vertical offset between viewport and minmap. */

				/* Overlap remaining in tiles. */
				var xOverlap = (short)(15 - Abs(xOffset));
				var yOverlap = (short)(10 - Abs(yOffset));

				short x, y;

				if (xOverlap < 1 || yOverlap < 1)
				{
					CSharpDune.g_viewport_forceRedraw = true;
				}
				else if (!CSharpDune.g_viewport_forceRedraw && (xOverlap != 15 || yOverlap != 10))
				{
					Map.Map_SetSelectionObjectPosition(0xFFFF);
					hasScrolled = true;

					GUI_Mouse_Hide_InWidget(2);

					GUI_Screen_Copy((short)Max(-xOffset << 1, 0), (short)(40 + Max(-yOffset << 4, 0)), (short)Max(0, xOffset << 1), (short)(40 + Max(0, yOffset << 4)), (short)(xOverlap << 1), (short)(yOverlap << 4), Screen.NO0, Screen.NO1);
				}
				else
				{
					CSharpDune.g_viewport_forceRedraw = true;
				}

				xOffset = Max((short)0, xOffset);
				yOffset = Max((short)0, yOffset);

				for (y = 0; y < 10; y++)
				{
					var mapYBase = (ushort)((y + viewportY) << 6);

					for (x = 0; x < 15; x++)
					{
						if (x >= xOffset && (xOffset + xOverlap) > x && y >= yOffset && (yOffset + yOverlap) > y && !CSharpDune.g_viewport_forceRedraw) continue;

						Map.Map_Update((ushort)(x + viewportX + mapYBase), 0, true);
					}
				}
			}

			if (hasScrolled)
			{
				Map.Map_SetSelectionObjectPosition(0xFFFF);

				for (xpos = 0; xpos < 14; xpos++)
				{
					var v = (ushort)(g_minimapPosition + xpos + 6 * 64);

					Tools.BitArray_Set(Map.g_dirtyViewport, v);
					Tools.BitArray_Set(Map.g_dirtyMinimap, v);

					Map.g_dirtyViewportCount++;
				}
			}

			g_minimapPosition = g_viewportPosition;
			g_selectionRectanglePosition = g_selectionPosition;

			if (g_viewportMessageCounter != 0 && s_timerViewportMessage < Timer.g_timerGUI)
			{
				g_viewportMessageCounter--;
				s_timerViewportMessage = Timer.g_timerGUI + 60;

				for (xpos = 0; xpos < 14; xpos++)
				{
					Map.Map_Update((ushort)(g_viewportPosition + xpos + 6 * 64), 0, true);
				}
			}

			Viewport.GUI_Widget_Viewport_Draw(CSharpDune.g_viewport_forceRedraw, hasScrolled, !Gfx.GFX_Screen_IsActive(Screen.NO0));

			CSharpDune.g_viewport_forceRedraw = false;

			Gfx.GFX_Screen_SetActive(oldScreenID);

			Map.Map_SetSelectionObjectPosition(g_selectionRectanglePosition);
			Map.Map_UpdateMinimapPosition(g_minimapPosition, false);

			GUI_Mouse_Show_InWidget();
		}

		/*
		 * Set a new palette, but animate it in slowly.
		 * @param palette The new palette.
		 * @param ticksOfAnimation The amount of ticks it should take.
		 */
		internal static void GUI_SetPaletteAnimated(byte[] palette, short ticksOfAnimation)
		{
			bool progress;
			short diffPerTick;
			short tickSlice;
			uint timerCurrent;
			short highestDiff;
			short ticks;
			ushort tickCurrent;
			var data = new byte[256 * 3];
			int i;

			if (palette == null) return;

			Array.Copy(Gfx.g_paletteActive, data, 256 * 3); //memcpy(data, g_paletteActive, 256 * 3);

			highestDiff = 0;
			for (i = 0; i < 256 * 3; i++)
			{
				var diff = (short)(palette[i] - data[i]);
				highestDiff = Max(highestDiff, Abs(diff));
			}

			ticks = (short)(ticksOfAnimation << 8);
			if (highestDiff != 0) ticks /= highestDiff;

			/* Find a nice value to change every timeslice */
			tickSlice = ticks;
			diffPerTick = 1;
			while (diffPerTick <= highestDiff && ticks < (2 << 8))
			{
				ticks += tickSlice;
				diffPerTick++;
			}

			tickCurrent = 0;
			timerCurrent = Timer.g_timerSleep;

			for (; ; )
			{
				progress = false;   /* will be set true if any color is changed */

				tickCurrent += (ushort)ticks;
				timerCurrent += (uint)(tickCurrent >> 8);
				tickCurrent &= 0xFF;

				for (i = 0; i < 256 * 3; i++)
				{
					short goal = palette[i];
					short current = data[i];

					if (goal == current) continue;

					progress = true;
					if (goal > current)
					{
						current += diffPerTick;
						if (current > goal) current = goal;
					}
					else
					{
						current -= diffPerTick;
						if (current < goal) current = goal;
					}
					data[i] = (byte)current;
				}

				/* if no color was changed, the target palette has been reached */
				if (!progress) break;

				Gfx.GFX_SetPalette(data);

				while (Timer.g_timerSleep < timerCurrent) Sleep.sleepIdle();
			}
		}

		/*
		 * Sets the clipping area.
		 * @param left The left clipping.
		 * @param top The top clipping.
		 * @param right The right clipping.
		 * @param bottom The bottom clipping.
		 */
		internal static void GUI_SetClippingArea(ushort left, ushort top, ushort right, ushort bottom)
		{
			g_clipping.left = left;
			g_clipping.top = top;
			g_clipping.right = right;
			g_clipping.bottom = bottom;
		}

		/*
		 * Fade in parts of the screen from one screenbuffer to the other screenbuffer.
		 * @param xSrc The X-position to start in the source screenbuffer divided by 8.
		 * @param ySrc The Y-position to start in the source screenbuffer.
		 * @param xDst The X-position to start in the destination screenbuffer divided by 8.
		 * @param yDst The Y-position to start in the destination screenbuffer.
		 * @param width The width of the screen to copy divided by 8.
		 * @param height The height of the screen to copy.
		 * @param screenSrc The ID of the source screen.
		 * @param screenDst The ID of the destination screen.
		 */
		internal static void GUI_Screen_FadeIn(ushort xSrc, ushort ySrc, ushort xDst, ushort yDst, ushort width, ushort height, Screen screenSrc, Screen screenDst)
		{
			var offsetsY = new ushort[100];
			var offsetsX = new ushort[40];
			int x, y;

			if (screenDst == Screen.NO0)
			{
				GUI_Mouse_Hide_InRegion((ushort)(xDst << 3), yDst, (ushort)((xDst + width) << 3), (ushort)(yDst + height));
			}

			height /= 2;

			for (x = 0; x < width; x++) offsetsX[x] = (ushort)x;
			for (y = 0; y < height; y++) offsetsY[y] = (ushort)y;

			for (x = 0; x < width; x++)
			{
				ushort index;
				ushort temp;

				index = Tools.Tools_RandomLCG_Range(0, (ushort)(width - 1));

				temp = offsetsX[index];
				offsetsX[index] = offsetsX[x];
				offsetsX[x] = temp;
			}

			for (y = 0; y < height; y++)
			{
				ushort index;
				ushort temp;

				index = Tools.Tools_RandomLCG_Range(0, (ushort)(height - 1));

				temp = offsetsY[index];
				offsetsY[index] = offsetsY[y];
				offsetsY[y] = temp;
			}

			for (y = 0; y < height; y++)
			{
				var y2 = (ushort)y;
				for (x = 0; x < width; x++)
				{
					ushort offsetX, offsetY;

					offsetX = offsetsX[x];
					offsetY = offsetsY[y2];

					GUI_Screen_Copy((short)(xSrc + offsetX), (short)(ySrc + offsetY * 2), (short)(xDst + offsetX), (short)(yDst + offsetY * 2), 1, 2, screenSrc, screenDst);

					y2++;
					if (y2 == height) y2 = 0;
				}

				/* XXX -- This delays the system so you can in fact see the animation */
				if ((y % 4) == 0) Timer.Timer_Sleep(1);
			}

			if (screenDst == Screen.NO0)
			{
				GUI_Mouse_Show_InRegion();
			}
		}

		internal static void GUI_ClearScreen(Screen screenID) =>
			Gfx.GFX_ClearScreen(screenID);

		/*
		 * Create the remap palette for the givern house.
		 * @param houseID The house ID.
		 */
		internal static void GUI_Palette_CreateRemap(byte houseID)
		{
			short i;
			short loc4;
			short loc6;
			byte[] remap;
			//int remapPointer = 0;

			remap = g_remap;
			for (i = 0; i < 0x100; i++/*, remapPointer++*/)
			{
				remap[i/*remapPointer*/] = (byte)(i & 0xFF);

				loc6 = (short)(i / 16);
				loc4 = (short)(i % 16);
				if (loc6 == 9 && loc4 <= 6)
				{
					remap[i/*remapPointer*/] = (byte)((houseID << 4) + 0x90 + loc4);
				}
			}
		}

		/*
		 * Creates a palette mapping: colour -> colour + reference * intensity.
		 *
		 * @param palette The palette to create the mapping for.
		 * @param colours The resulting mapping.
		 * @param reference The colour to use as reference.
		 * @param intensity The intensity to use.
		 */
		internal static void GUI_Palette_CreateMapping(byte[] palette, byte[] colours, byte reference, byte intensity)
		{
			ushort index;

			if (palette == null || colours == null) return;

			colours[0] = 0;

			for (index = 1; index < 256; index++)
			{
				ushort i;
				var red = (byte)(palette[3 * index + 0] - (((palette[3 * index + 0] - palette[3 * reference + 0]) * (intensity / 2)) >> 7));
				var blue = (byte)(palette[3 * index + 1] - (((palette[3 * index + 1] - palette[3 * reference + 1]) * (intensity / 2)) >> 7));
				var green = (byte)(palette[3 * index + 2] - (((palette[3 * index + 2] - palette[3 * reference + 2]) * (intensity / 2)) >> 7));
				var colour = reference;
				ushort sumMin = 0xFFFF;

				for (i = 1; i < 256; i++)
				{
					ushort sum = 0;

					sum += (ushort)((palette[3 * i + 0] - red) * (palette[3 * i + 0] - red));
					sum += (ushort)((palette[3 * i + 1] - blue) * (palette[3 * i + 1] - blue));
					sum += (ushort)((palette[3 * i + 2] - green) * (palette[3 * i + 2] - green));

					if (sum > sumMin) continue;
					if ((i != reference) && (i == index)) continue;

					sumMin = sum;
					colour = (byte)(i & 0xFF);
				}

				colours[index] = colour;
			}
		}

		internal static ushort GUI_StrategicMap_Show(ushort campaignID, bool win)
		{
			ushort scenarioID;
			ushort previousCampaignID;
			ushort x;
			ushort y;
			Screen oldScreenID;
			var palette = new byte[3 * 256];
			var loc316 = new byte[12];

			if (campaignID == 0) return 1;

			Timer.Timer_Sleep(10);
			Sound.Music_Play(0x1D);

			Array.Fill<byte>(palette, 0, 0, 256 * 3); //memset(palette, 0, 256 * 3);

			previousCampaignID = (ushort)(campaignID - (win ? 1 : 0));
			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO2);

			GUI_SetPaletteAnimated(palette, 15);

			Mouse.Mouse_SetRegion(8, 24, 311, 143);

			GUI_Mouse_SetPosition(160, 84);

			Sprites.Sprites_LoadImage("MAPMACH.CPS", Screen.NO2, g_palette_998A);

			GUI_Palette_RemapScreen(0, 0, Gfx.SCREEN_WIDTH, Gfx.SCREEN_HEIGHT, Screen.NO2, g_remap);

			x = 0;
			y = 0;

			switch (CHouse.g_playerHouseID)
			{
				case HouseType.HOUSE_HARKONNEN:
					x = 0;
					y = 152;
					break;

				default:
					x = 33;
					y = 152;
					break;

				case HouseType.HOUSE_ORDOS:
					x = 1;
					y = 24;
					break;
			}

			Array.Copy(Gfx.g_palette1, 251 * 3, loc316, 0, 12); //memcpy(loc316, Gfx.g_palette1 + 251 * 3, 12);
			Array.Copy(Gfx.g_palette1, (144 + ((int)CHouse.g_playerHouseID * 16)) * 3, s_strategicMapArrowColors, 0, 4 * 3); //memcpy(s_strategicMapArrowColors, Gfx.g_palette1 + (144 + (CHouse.g_playerHouseID * 16)) * 3, 4 * 3);
			Array.Copy(s_strategicMapArrowColors, 0, s_strategicMapArrowColors, 4 * 3, 4 * 3); //memcpy(s_strategicMapArrowColors + 4 * 3, s_strategicMapArrowColors, 4 * 3);

			GUI_Screen_Copy((short)x, (short)y, 0, 152, 7, 40, Screen.NO2, Screen.NO2);
			GUI_Screen_Copy((short)x, (short)y, 33, 152, 7, 40, Screen.NO2, Screen.NO2);

			switch ((Language)Config.g_config.language)
			{
				case Language.GERMAN:
					GUI_Screen_Copy(1, 120, 1, 0, 38, 24, Screen.NO2, Screen.NO2);
					break;

				case Language.FRENCH:
					GUI_Screen_Copy(1, 96, 1, 0, 38, 24, Screen.NO2, Screen.NO2);
					break;

				default: break;
			}

			GUI_DrawFilledRectangle(8, 24, 311, 143, 12);

			GUI_Mouse_Hide_Safe();
			GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, Screen.NO2, Screen.NO0);
			GUI_SetPaletteAnimated(Gfx.g_palette1, 15);
			GUI_Mouse_Show_Safe();

			s_strategicMapFastForward = false;

			if (win && campaignID == 1)
			{
				Sprites.Sprites_LoadImage("PLANET.CPS", Screen.NO1, g_palette_998A);

				GUI_StrategicMap_DrawText(CString.String_Get_ByIndex(Text.STR_THREE_HOUSES_HAVE_COME_TO_DUNE));

				GUI_Screen_FadeIn2(8, 24, 304, 120, Screen.NO1, Screen.NO0, 0, false);

				Input.Input.Input_History_Clear();

				Sprites.Sprites_CPS_LoadRegionClick();

				for (Timer.g_timerTimeout = 120; Timer.g_timerTimeout != 0; Sleep.sleepIdle())
				{
					if (GUI_StrategicMap_FastForwardToggleWithESC()) break;
				}

				Sprites.Sprites_LoadImage("DUNEMAP.CPS", Screen.NO1, g_palette_998A);

				GUI_StrategicMap_DrawText(CString.String_Get_ByIndex(Text.STR_TO_TAKE_CONTROL_OF_THE_LAND));

				GUI_Screen_FadeIn2(8, 24, 304, 120, Screen.NO1, Screen.NO0, (ushort)(GUI_StrategicMap_FastForwardToggleWithESC() ? 0 : 1), false);

				for (Timer.g_timerTimeout = 60; Timer.g_timerTimeout != 0; Sleep.sleepIdle())
				{
					if (GUI_StrategicMap_FastForwardToggleWithESC()) break;
				}

				GUI_StrategicMap_DrawText(CString.String_Get_ByIndex(Text.STR_THAT_HAS_BECOME_DIVIDED));
			}
			else
			{
				Sprites.Sprites_CPS_LoadRegionClick();
			}

			Sprites.Sprites_LoadImage("DUNERGN.CPS", Screen.NO1, g_palette_998A);

			Gfx.GFX_Screen_SetActive(Screen.NO1);

			GUI_StrategicMap_PrepareRegions(previousCampaignID);

			if (GUI_StrategicMap_FastForwardToggleWithESC())
			{
				GUI_Screen_Copy(1, 24, 1, 24, 38, 120, Screen.NO1, Screen.NO0);
			}
			else
			{
				GUI_Screen_FadeIn2(8, 24, 304, 120, Screen.NO1, Screen.NO0, 0, false);
			}

			GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, Screen.NO0, Screen.NO1);

			if (campaignID != previousCampaignID) GUI_StrategicMap_ShowProgression(campaignID);

			GUI_Mouse_Show_Safe();

			if (Sprites.g_regions[0] >= campaignID)
			{
				GUI_StrategicMap_DrawText(CString.String_Get_ByIndex(Text.STR_SELECT_YOUR_NEXT_REGION));

				scenarioID = GUI_StrategicMap_ScenarioSelection(campaignID);
			}
			else
			{
				scenarioID = 0;
			}

			CDriver.Driver_Music_FadeOut();

			Gfx.GFX_Screen_SetActive(oldScreenID);

			Mouse.Mouse_SetRegion(0, 0, Gfx.SCREEN_WIDTH - 1, Gfx.SCREEN_HEIGHT - 1);

			Input.Input.Input_History_Clear();

			Array.Copy(loc316, 0, Gfx.g_palette1, 251 * 3, 12); //memcpy(Gfx.g_palette1 + 251 * 3, loc316, 12);

			GUI_SetPaletteAnimated(palette, 15);

			GUI_Mouse_Hide_Safe();
			GUI_ClearScreen(Screen.NO0);
			GUI_Mouse_Show_Safe();

			Gfx.GFX_SetPalette(Gfx.g_palette1);

			return scenarioID;
		}

		static readonly byte[][] l_houses = { //[3][3]
			/* x, y, shortcut */
			new byte[] { 16, 56, 31 }, /* A */
			new byte[] { 112, 56, 25 }, /* O */
			new byte[] { 208, 56, 36 }, /* H */
		};
		/*
		 * Show pick house screen.
		 */
		internal static byte GUI_PickHouse()
		{
			Screen oldScreenID;
			Widget w = null;
			var palette = new byte[3 * 256];
			ushort i;
			HouseType houseID;

			houseID = HouseType.HOUSE_MERCENARY;

			//memset(palette, 0, 256 * 3);

			CDriver.Driver_Voice_Play(null, 0xFF);

			Sound.Voice_LoadVoices(5);

			for (; ; Sleep.sleepIdle())
			{
				ushort yes_no;

				for (i = 0; i < 3; i++)
				{
					Widget w2;

					w2 = CWidget.GUI_Widget_Allocate((ushort)(i + 1), l_houses[i][2], l_houses[i][0], l_houses[i][1], 0xFFFF, 0);

					//memset(w2.flags, 0, sizeof(w2.flags));
					w2.flags.loseSelect = true;
					w2.flags.buttonFilterLeft = 1;
					w2.flags.buttonFilterRight = 1;
					w2.width = 96;
					w2.height = 104;

					w = CWidget.GUI_Widget_Link(w, w2);
				}

				Sprites.Sprites_LoadImage(CString.String_GenerateFilename("HERALD"), Screen.NO1, null);

				GUI_Mouse_Hide_Safe();
				GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, Screen.NO1, Screen.NO0);
				GUI_SetPaletteAnimated(Gfx.g_palette1, 15);
				GUI_Mouse_Show_Safe();

				for (houseID = HouseType.HOUSE_INVALID; houseID == HouseType.HOUSE_INVALID; Sleep.sleepIdle())
				{
					var key = CWidget.GUI_Widget_HandleEvents(w);

					GUI_PaletteAnimate();

					if ((key & 0x800) != 0) key = 0;

					switch (key)
					{
						case 0x8001: houseID = HouseType.HOUSE_ATREIDES; break;
						case 0x8002: houseID = HouseType.HOUSE_ORDOS; break;
						case 0x8003: houseID = HouseType.HOUSE_HARKONNEN; break;
						default: break;
					}
				}

				GUI_Mouse_Hide_Safe();

				if (Config.g_enableVoices)
				{ // != 0) {
					Sound.Sound_Output_Feedback((ushort)(houseID + 62));

					while (Sound.Sound_StartSpeech()) Sleep.sleepIdle();
				}

				while (w != null)
				{
					var next = w.next;

					w = null; //free(w);

					w = next;
				}

				GUI_SetPaletteAnimated(palette, 15);

				if (CSharpDune.g_debugSkipDialogs || CSharpDune.g_debugScenario)
				{
					Debug.WriteLine("DEBUG: Skipping House selection confirmation.");
					break;
				}

				w = CWidget.GUI_Widget_Link(w, CWidget.GUI_Widget_Allocate(1, CWidget.GUI_Widget_GetShortcut((byte)CString.String_Get_ByIndex(Text.STR_YES)[0]), 168, 168, 373, 0));
				w = CWidget.GUI_Widget_Link(w, CWidget.GUI_Widget_Allocate(2, CWidget.GUI_Widget_GetShortcut((byte)CString.String_Get_ByIndex(Text.STR_NO)[0]), 240, 168, 375, 0));

				CHouse.g_playerHouseID = HouseType.HOUSE_MERCENARY;

				oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO0);

				GUI_Mouse_Show_Safe();

				//strncpy(g_readBuffer, String_Get_ByIndex(STR_HOUSE_HARKONNENFROM_THE_DARK_WORLD_OF_GIEDI_PRIME_THE_SAVAGE_HOUSE_HARKONNEN_HAS_SPREAD_ACROSS_THE_UNIVERSE_A_CRUEL_PEOPLE_THE_HARKONNEN_ARE_RUTHLESS_TOWARDS_BOTH_FRIEND_AND_FOE_IN_THEIR_FANATICAL_PURSUIT_OF_POWER + houseID * 40), g_readBufferSize);
				var text = CString.String_Get_ByIndex((ushort)(Text.STR_HOUSE_HARKONNENFROM_THE_DARK_WORLD_OF_GIEDI_PRIME_THE_SAVAGE_HOUSE_HARKONNEN_HAS_SPREAD_ACROSS_THE_UNIVERSE_A_CRUEL_PEOPLE_THE_HARKONNEN_ARE_RUTHLESS_TOWARDS_BOTH_FRIEND_AND_FOE_IN_THEIR_FANATICAL_PURSUIT_OF_POWER + (byte)houseID * 40));
				CSharpDune.g_readBuffer = CSharpDune.Encoding.GetBytes(text);
				Mentat.GUI_Mentat_Show(text, CHouse.House_GetWSAHouseFilename((byte)houseID), null);

				Sprites.Sprites_LoadImage(CString.String_GenerateFilename("MISC"), Screen.NO1, Gfx.g_palette1);

				GUI_Mouse_Hide_Safe();

				GUI_Screen_Copy(0, 0, 0, 0, 26, 24, Screen.NO1, Screen.NO0);

				GUI_Screen_Copy(0, (short)(24 * ((byte)houseID + 1)), 26, 0, 13, 24, Screen.NO1, Screen.NO0);

				WidgetDraw.GUI_Widget_DrawAll(w);

				GUI_Mouse_Show_Safe();

				for (; ; Sleep.sleepIdle())
				{
					yes_no = Mentat.GUI_Mentat_Loop(CHouse.House_GetWSAHouseFilename((byte)houseID), null, null, true, w);

					if ((yes_no & 0x8000) != 0) break;
				}

				if (yes_no == 0x8001)
				{
					CDriver.Driver_Music_FadeOut();
				}
				else
				{
					GUI_SetPaletteAnimated(palette, 15);
				}

				while (w != null)
				{
					var next = w.next;

					w = null; //free(w);

					w = next;
				}

				Load.Load_Palette_Mercenaries();
				Sprites.Sprites_LoadTiles();

				Gfx.GFX_Screen_SetActive(oldScreenID);

				while (CDriver.Driver_Voice_IsPlaying()) Sleep.sleepIdle();

				if (yes_no == 0x8001) break;
			}

			Sound.Music_Play(0);

			GUI_Palette_CreateRemap((byte)houseID);

			Input.Input.Input_History_Clear();

			GUI_Mouse_Show_Safe();

			GUI_SetPaletteAnimated(palette, 15);

			return (byte)houseID;
		}

		/*
		 * Shows the stats at end of scenario.
		 * @param killedAllied The amount of destroyed allied units.
		 * @param killedEnemy The amount of destroyed enemy units.
		 * @param destroyedAllied The amount of destroyed allied structures.
		 * @param destroyedEnemy The amount of destroyed enemy structures.
		 * @param harvestedAllied The amount of spice harvested by allies.
		 * @param harvestedEnemy The amount of spice harvested by enemies.
		 * @param score The base score.
		 * @param houseID The houseID of the player.
		 */
		internal static void GUI_EndStats_Show(ushort killedAllied, ushort killedEnemy, ushort destroyedAllied, ushort destroyedEnemy, ushort harvestedAllied, ushort harvestedEnemy, short score, byte houseID)
		{
			Screen oldScreenID;
			ushort statsBoxCount;
			ushort textLeft;    /* text left position */
			ushort statsBarWidth;   /* available width to draw the score bars */
			ushort i;
			(ushort value, ushort increment)[][] scores =
			{
				new (ushort value, ushort increment)[2], new (ushort value, ushort increment)[2], new (ushort value, ushort increment)[2]
			};

			s_ticksPlayed = ((Timer.g_timerGame - CSharpDune.g_tickScenarioStart) / 3600) + 1;

			//TODO: Remove
			//s_ticksPlayed = 5;
			//killedAllied = 0;
			//killedEnemy = 2;
			//destroyedAllied = 0;
			//destroyedEnemy = 0;
			//harvestedAllied = 707;
			//harvestedEnemy = 0;
			//score = 68;

			score = (short)Update_Score(score, harvestedAllied, harvestedEnemy, houseID);

			/* 1st scenario doesn't have the "Building destroyed" stats */
			statsBoxCount = (ushort)((CSharpDune.g_scenarioID == 1) ? 2 : 3);

			GUI_Mouse_Hide_Safe();

			GUI_ChangeSelectionType((ushort)SelectionType.MENTAT);

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);

			GUI_HallOfFame_DrawBackground((ushort)score, false);

			GUI_DrawTextOnFilledRectangle(CString.String_Get_ByIndex(Text.STR_SPICE_HARVESTED_BY), 83);
			GUI_DrawTextOnFilledRectangle(CString.String_Get_ByIndex(Text.STR_UNITS_DESTROYED_BY), 119);
			if (CSharpDune.g_scenarioID != 1) GUI_DrawTextOnFilledRectangle(CString.String_Get_ByIndex(Text.STR_BUILDINGS_DESTROYED_BY), 155);

			textLeft = (ushort)(19 + Max(CFont.Font_GetStringWidth(CString.String_Get_ByIndex(Text.STR_YOU)), CFont.Font_GetStringWidth(CString.String_Get_ByIndex(Text.STR_ENEMY))));
			statsBarWidth = (ushort)(261 - textLeft);

			for (i = 0; i < statsBoxCount; i++)
			{
				GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_YOU), (short)(textLeft - 4), (short)(92 + (i * 36)), 0xF, 0, 0x221);
				GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_ENEMY), (short)(textLeft - 4), (short)(101 + (i * 36)), 0xF, 0, 0x221);
			}

			Sound.Music_Play((ushort)(17 + Tools.Tools_RandomLCG_Range(0, 5)));

			GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, Screen.NO1, Screen.NO0);

			Input.Input.Input_History_Clear();

			scores[0][0].value = harvestedAllied;
			scores[0][1].value = harvestedEnemy;
			scores[1][0].value = killedEnemy;
			scores[1][1].value = killedAllied;
			scores[2][0].value = destroyedEnemy;
			scores[2][1].value = destroyedAllied;

			for (i = 0; i < statsBoxCount; i++)
			{
				ushort scoreIncrement;

				/* You */
				if (scores[i][0].value > 65000) scores[i][0].value = 65000;
				/* Enemy */
				if (scores[i][1].value > 65000) scores[i][1].value = 65000;

				scoreIncrement = (ushort)(1 + (Max(scores[i][0].value, scores[i][1].value) / statsBarWidth));

				scores[i][0].increment = scoreIncrement;
				scores[i][1].increment = scoreIncrement;
			}

			GUI_EndStats_Sleep(45);
			GUI_HallOfFame_DrawRank((ushort)score, true);
			GUI_EndStats_Sleep(45);

			for (i = 0; i < statsBoxCount; i++)
			{
				ushort j;

				GUI_HallOfFame_Tick();

				for (j = 0; j < 2; j++)
				{   /* 0 : You, 1 : Enemy */
					byte colour;
					ushort posX;
					ushort posY;
					ushort calculatedScore;

					GUI_HallOfFame_Tick();

					colour = (byte)((j == 0) ? 255 : 209);
					posX = textLeft;
					posY = (ushort)(93 + (i * 36) + (j * 9));

					for (calculatedScore = 0; calculatedScore < scores[i][j].value; calculatedScore += scores[i][j].increment)
					{
						GUI_DrawFilledRectangle(271, (short)posY, 303, (short)(posY + 5), 226);
						GUI_DrawText_Wrapper("{0}", 287, (short)(posY - 1), 0x14, 0, 0x121, calculatedScore);

						GUI_HallOfFame_Tick();

						Timer.g_timerTimeout = 1;

						GUI_DrawLine((short)posX, (short)posY, (short)posX, (short)(posY + 5), colour);

						posX++;

						GUI_DrawLine((short)posX, (short)(posY + 1), (short)posX, (short)(posY + 6), 12);   /* shadow */

						Gfx.GFX_Screen_Copy2((short)textLeft, (short)posY, (short)textLeft, (short)posY, 304, 7, Screen.NO1, Screen.NO0, false);

						CDriver.Driver_Sound_Play(52, 0xFF);

						GUI_EndStats_Sleep((ushort)Timer.g_timerTimeout);
					}

					GUI_DrawFilledRectangle(271, (short)posY, 303, (short)(posY + 5), 226);
					GUI_DrawText_Wrapper("{0}", 287, (short)(posY - 1), 0xF, 0, 0x121, scores[i][j].value);

					Gfx.GFX_Screen_Copy2((short)textLeft, (short)posY, (short)textLeft, (short)posY, 304, 7, Screen.NO1, Screen.NO0, false);

					CDriver.Driver_Sound_Play(38, 0xFF);

					GUI_EndStats_Sleep(12);
				}

				GUI_EndStats_Sleep(60);
			}

			GUI_Mouse_Show_Safe();

			Input.Input.Input_History_Clear();

			for (; ; Sleep.sleepIdle())
			{
				GUI_HallOfFame_Tick();
				if (Input.Input.Input_Keyboard_NextKey() != 0) break;
			}

			Input.Input.Input_History_Clear();

			GUI_HallOfFame_Show((ushort)score);

			Array.Fill<byte>(Gfx.g_palette1, 0, 255 * 3, 3); //memset(g_palette1 + 255 * 3, 0, 3);

			Gfx.GFX_Screen_SetActive(oldScreenID);

			CDriver.Driver_Music_FadeOut();
		}

		internal static void GUI_HallOfFame_Show(ushort score)
		{
			ushort width;
			ushort editLine;
			Widget w;
			byte fileID;
			var data = new HallOfFameStruct[8];
			byte[] encodedBytes;

			GUI_Mouse_Hide_Safe();

			if (score == 0xFFFF)
			{
				if (!CFile.File_Exists_Personal("SAVEFAME.DAT"))
				{
					GUI_Mouse_Show_Safe();
					return;
				}
				s_ticksPlayed = 0;
			}

			//data = (HallOfFameStruct[])Gfx.GFX_Screen_Get_ByIndex(Screen.NO2);

			if (!CFile.File_Exists_Personal("SAVEFAME.DAT"))
			{
				ushort written;

				for (var i = 0; i < data.Length; i++) data[i] = new HallOfFameStruct(); //memset(data, 0, 128);

				encodedBytes = GUI_HallOfFame_Encode(data);

				fileID = CFile.File_Open_Personal("SAVEFAME.DAT", FileMode.FILE_MODE_WRITE);
				written = (ushort)CFile.File_Write(fileID, /*HallOfFameStruct.AllToBytes(data)*/encodedBytes, 128);
				CFile.File_Close(fileID);

				if (written != 128) return;
			}

			encodedBytes = new byte[128];
			CFile.File_ReadBlockFile_Personal("SAVEFAME.DAT", /*HallOfFameStruct.AllToBytes(data)*/encodedBytes, 128);

			data = GUI_HallOfFame_Decode(/*data*/encodedBytes);

			GUI_HallOfFame_DrawBackground(score, true);

			if (score == 0xFFFF)
			{
				editLine = 0;
			}
			else
			{
				editLine = GUI_HallOfFame_InsertScore(data, score);
			}

			width = GUI_HallOfFame_DrawData(data, false);

			GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, Screen.NO1, Screen.NO0);

			if (editLine != 0)
			{
				WidgetProperties backupProperties;
				string name;

				name = new string(data[editLine - 1].name).Replace("\0", string.Empty);

				backupProperties = CWidget.g_widgetProperties[19].Clone(); //memcpy(&backupProperties, &g_widgetProperties[19], sizeof(WidgetProperties));

				CWidget.g_widgetProperties[19].xBase = 4;
				CWidget.g_widgetProperties[19].yBase = (ushort)((editLine - 1) * 11 + 90);
				CWidget.g_widgetProperties[19].width = (ushort)(width / 8);
				CWidget.g_widgetProperties[19].height = 11;
				CWidget.g_widgetProperties[19].fgColourBlink = 6;
				CWidget.g_widgetProperties[19].fgColourNormal = 116;

				GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x22);

				while (name == string.Empty)
				{
					Screen oldScreenID;

					oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO0);
					CWidget.Widget_SetAndPaintCurrentWidget(19);
					Gfx.GFX_Screen_SetActive(oldScreenID);

					EditBox.GUI_EditBox(ref name, 5, 19, null, GUI_HallOfFame_Tick, false);

					if (name == string.Empty) continue;
				}

				data[editLine - 1].name = name.PadRight(6, '\0').ToArray();

				CWidget.g_widgetProperties[19] = backupProperties.Clone(); //memcpy(&g_widgetProperties[19], &backupProperties, sizeof(WidgetProperties));

				GUI_HallOfFame_DrawData(data, true);

				encodedBytes = GUI_HallOfFame_Encode(data);

				fileID = CFile.File_Open_Personal("SAVEFAME.DAT", FileMode.FILE_MODE_WRITE);
				CFile.File_Write(fileID, /*HallOfFameStruct.AllToBytes(data)*/encodedBytes, 128);
				CFile.File_Close(fileID);
			}

			GUI_Mouse_Show_Safe();

			w = GUI_HallOfFame_CreateButtons(data);

			Input.Input.Input_History_Clear();

			Gfx.GFX_Screen_SetActive(Screen.NO0);

			for (g_doQuitHOF = false; !g_doQuitHOF; Sleep.sleepIdle())
			{
				CWidget.GUI_Widget_HandleEvents(w);
			}

			GUI_HallOfFame_DeleteButtons(w);

			Input.Input.Input_History_Clear();

			if (score == 0xFFFF) return;

			Array.Fill<byte>(Gfx.g_palette1, 0, 255 * 3, 3); //memset(g_palette1 + 255 * 3, 0, 3);
		}

		static uint l_timerNext2;
        static short colouringDirection = 1;
		static ushort GUI_HallOfFame_Tick()
		{
			if (l_timerNext2 >= Timer.g_timerGUI) return 0;
			l_timerNext2 = Timer.g_timerGUI + 2;

			if (s_palette1_houseColour.Curr >= 63)
			{
				colouringDirection = -1;
			}
			else if (s_palette1_houseColour.Curr <= 35)
			{
				colouringDirection = 1;
			}

			s_palette1_houseColour.Curr += (byte)colouringDirection;

			Gfx.g_palette1[255 * 3 + s_palette1_houseColour.Ptr] = s_palette1_houseColour.Curr;

			Gfx.GFX_SetPalette(Gfx.g_palette1);

			return 0;
		}

		static void GUI_EndStats_Sleep(ushort delay)
		{
			for (Timer.g_timerTimeout = delay; Timer.g_timerTimeout != 0; Sleep.sleepIdle())
			{
				GUI_HallOfFame_Tick();
			}
		}

		static bool GUI_StrategicMap_FastForwardToggleWithESC()
		{
			if (Input.Input.Input_Keyboard_NextKey() == 0) return s_strategicMapFastForward;

			if (Input.Input.Input_WaitForValidInput() != 0x1B) return s_strategicMapFastForward;

			s_strategicMapFastForward = !s_strategicMapFastForward;

			Input.Input.Input_History_Clear();

			return s_strategicMapFastForward;
		}

		static uint l_timerNext;
        static void GUI_StrategicMap_DrawText(string str)
		{
			Screen oldScreenID;
			ushort y;

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);

			GUI_Screen_Copy(8, 165, 8, 186, 24, 14, Screen.NO0, Screen.NO1);

			GUI_DrawFilledRectangle(64, 172, 255, 185, Gfx.GFX_GetPixel(64, 186));

			GUI_DrawText_Wrapper(str, 64, 175, 12, 0, 0x12);

			while (Timer.g_timerGUI + 90 < l_timerNext) Sleep.sleepIdle();

			for (y = 185; y > 172; y--)
			{
				GUI_Screen_Copy(8, (short)y, 8, 165, 24, 14, Screen.NO1, Screen.NO0);

				for (Timer.g_timerTimeout = 3; Timer.g_timerTimeout != 0; Sleep.sleepIdle())
				{
					if (GUI_StrategicMap_FastForwardToggleWithESC()) break;
				}
			}

			l_timerNext = Timer.g_timerGUI + 90;

			Gfx.GFX_Screen_SetActive(oldScreenID);
		}

		/*
		 * Draws a string on a filled rectangle.
		 * @param string The string to draw.
		 * @param top The most top position where to draw the string.
		 */
		static void GUI_DrawTextOnFilledRectangle(string str, ushort top)
		{
			ushort halfWidth;

			GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x121);

			halfWidth = (ushort)((CFont.Font_GetStringWidth(str) / 2) + 4);

			GUI_DrawFilledRectangle((short)(Gfx.SCREEN_WIDTH / 2 - halfWidth), (short)top, (short)(Gfx.SCREEN_WIDTH / 2 + halfWidth), (short)(top + 6), 116);
			GUI_DrawText_Wrapper(str, Gfx.SCREEN_WIDTH / 2, (short)top, 0xF, 0, 0x121);
		}

		/*
		 * Set the mouse to the given position on the screen.
		 *
		 * @param x The new X-position of the mouse.
		 * @param y The new Y-position of the mouse.
		 */
		static void GUI_Mouse_SetPosition(ushort x, ushort y)
		{
			while (Mouse.g_mouseLock != 0) Sleep.sleepIdle();
			Mouse.g_mouseLock++;

			if (x < Mouse.g_mouseRegionLeft) x = Mouse.g_mouseRegionLeft;
			if (x > Mouse.g_mouseRegionRight) x = Mouse.g_mouseRegionRight;
			if (y < Mouse.g_mouseRegionTop) y = Mouse.g_mouseRegionTop;
			if (y > Mouse.g_mouseRegionBottom) y = Mouse.g_mouseRegionBottom;

			Mouse.g_mouseX = x;
			Mouse.g_mouseY = y;

			VideoSdl2.Video_Mouse_SetPosition(x, y);

			if (Mouse.g_mouseX != Mouse.g_mousePrevX || Mouse.g_mouseY != Mouse.g_mousePrevY)
			{
				GUI_Mouse_Hide();
				GUI_Mouse_Show();
			}

			Mouse.g_mouseLock--;
		}

		static void GUI_StrategicMap_ReadHouseRegions(byte houseID, ushort campaignID)
		{
			string key; //char[4];
			string buffer; //char[100]
			string groupText; //char[16];
			var bufferPointer = 0;

			key = g_table_houseInfo[houseID].name; //strncpy(key, g_table_houseInfo[houseID].name, 3);
														  //key[3] = '\0';

			groupText = $"GROUP{campaignID}"; //snprintf(groupText, sizeof(groupText), "GROUP%d", campaignID);

			if ((buffer = Ini.Ini_GetString(groupText, key, null, Sprites.g_fileRegionINI)) == null) return;

			while (buffer[bufferPointer] != Environment.NewLine[0])
			{ //*s != '\0'
				var region = ushort.Parse(buffer[bufferPointer].ToString(), CSharpDune.Culture);

				if (region != 0) Sprites.g_regions[region] = houseID;

				while (buffer[bufferPointer] != Environment.NewLine[0])
				{ //*s != '\0'
					if (buffer[bufferPointer++] == ',') break;
				}
			}
		}

		static void GUI_StrategicMap_PrepareRegions(ushort campaignID)
		{
			ushort i;

			for (i = 0; i < campaignID; i++)
			{
				GUI_StrategicMap_ReadHouseRegions((byte)HouseType.HOUSE_HARKONNEN, (ushort)(i + 1));
				GUI_StrategicMap_ReadHouseRegions((byte)HouseType.HOUSE_ATREIDES, (ushort)(i + 1));
				GUI_StrategicMap_ReadHouseRegions((byte)HouseType.HOUSE_ORDOS, (ushort)(i + 1));
				GUI_StrategicMap_ReadHouseRegions((byte)HouseType.HOUSE_SARDAUKAR, (ushort)(i + 1));
			}

			for (i = 0; i < Sprites.g_regions[0]; i++)
			{
				if (Sprites.g_regions[i + 1] == 0xFFFF) continue;

				GUI_StrategicMap_DrawRegion((byte)Sprites.g_regions[i + 1], (ushort)(i + 1), false);
			}
		}

		/*
		 * Fade in parts of the screen from one screenbuffer to the other screenbuffer.
		 * @param x The X-position in the source and destination screenbuffers.
		 * @param y The Y-position in the source and destination screenbuffers.
		 * @param width The width of the screen to copy.
		 * @param height The height of the screen to copy.
		 * @param screenSrc The ID of the source screen.
		 * @param screenDst The ID of the destination screen.
		 * @param delay The delay.
		 * @param skipNull Wether to copy pixels with colour 0.
		 */
		internal static void GUI_Screen_FadeIn2(short x, short y, short width, short height, Screen screenSrc, Screen screenDst, ushort delay, bool skipNull)
		{
			Screen oldScreenID;
			ushort i;
			ushort j;

			var columns = new ushort[Gfx.SCREEN_WIDTH];
			var rows = new ushort[Gfx.SCREEN_HEIGHT];

			Debug.Assert(width <= Gfx.SCREEN_WIDTH);
			Debug.Assert(height <= Gfx.SCREEN_HEIGHT);

			if (screenDst == 0)
			{
				GUI_Mouse_Hide_InRegion((ushort)x, (ushort)y, (ushort)(x + width), (ushort)(y + height));
			}

			for (i = 0; i < width; i++) columns[i] = i;
			for (i = 0; i < height; i++) rows[i] = i;

			for (i = 0; i < width; i++)
			{
				ushort tmp;

				j = Tools.Tools_RandomLCG_Range(0, (ushort)(width - 1));

				tmp = columns[j];
				columns[j] = columns[i];
				columns[i] = tmp;
			}

			for (i = 0; i < height; i++)
			{
				ushort tmp;

				j = Tools.Tools_RandomLCG_Range(0, (ushort)(height - 1));

				tmp = rows[j];
				rows[j] = rows[i];
				rows[i] = tmp;
			}

			oldScreenID = Gfx.GFX_Screen_SetActive(screenDst);

			for (j = 0; j < height; j++)
			{
				var j2 = j;

				for (i = 0; i < width; i++)
				{
					byte colour;
					var curX = (ushort)(x + columns[i]);
					var curY = (ushort)(y + rows[j2]);

					if (++j2 >= height) j2 = 0;

					Gfx.GFX_Screen_SetActive(screenSrc);

					colour = Gfx.GFX_GetPixel(curX, curY);

					Gfx.GFX_Screen_SetActive(screenDst);

					if (skipNull && colour == 0) continue;

					Gfx.GFX_PutPixel(curX, curY, colour);
				}
				Gfx.GFX_Screen_SetDirty(screenDst, (ushort)x, (ushort)y, (ushort)(x + width), (ushort)(y + height));

				Timer.Timer_Sleep(delay);
			}

			if (screenDst == 0)
			{
				GUI_Mouse_Show_InRegion();
			}

			Gfx.GFX_Screen_SetActive(oldScreenID);
		}

		static void GUI_HallOfFame_DrawBackground(ushort score, bool hallOfFame)
		{
			Screen oldScreenID;
			ushort xSrc;
			ushort colour;
			ushort offset;

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);

			Sprites.Sprites_LoadImage("FAME.CPS", Screen.NO1, g_palette_998A);

			xSrc = 1;
			if (CHouse.g_playerHouseID <= HouseType.HOUSE_ORDOS)
			{
				xSrc = (ushort)(((byte)CHouse.g_playerHouseID * 56 + 8) / 8);
			}

			GUI_Screen_Copy((short)xSrc, 136, 0, 8, 7, 56, Screen.NO1, Screen.NO1);

			if (CHouse.g_playerHouseID > HouseType.HOUSE_ORDOS)
			{
				xSrc += 7;
			}

			GUI_Screen_Copy((short)xSrc, 136, 33, 8, 7, 56, Screen.NO1, Screen.NO1);

			GUI_DrawFilledRectangle(8, 136, 175, 191, 116);

			if (hallOfFame)
			{
				GUI_DrawFilledRectangle(8, 80, 311, 191, 116);
				if (score != 0xFFFF) GUI_HallOfFame_DrawRank(score, false);
			}
			else
			{
				Gfx.GFX_Screen_Copy2(8, 80, 8, 116, 304, 36, Screen.NO1, Screen.NO1, false);
				if (CSharpDune.g_scenarioID != 1) Gfx.GFX_Screen_Copy2(8, 80, 8, 152, 304, 36, Screen.NO1, Screen.NO1, false);
			}

			if (score != 0xFFFF)
			{
				string buffer; //char[64];

				//snprintf(buffer, sizeof(buffer), String_Get_ByIndex(STR_TIME_DH_DM), s_ticksPlayed / 60, s_ticksPlayed % 60);
				buffer = string.Format(CSharpDune.Culture, CString.String_Get_ByIndex(Text.STR_TIME_DH_DM), s_ticksPlayed / 60, s_ticksPlayed % 60);

				if (s_ticksPlayed < 60)
				{
					//char* hours = strchr(buffer, '0');
					//while (*hours != ' ') memmove(hours, hours + 1, strlen(hours));
					buffer = buffer.Replace("0h", string.Empty);
				}

				/* "Score: %d" */
				GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_SCORE_D), 72, 15, 15, 0, 0x22, score);
				GUI_DrawText_Wrapper(buffer, 248, 15, 15, 0, 0x222);
				/* "You have attained the rank of" */
				GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_YOU_HAVE_ATTAINED_THE_RANK_OF), Gfx.SCREEN_WIDTH / 2, 38, 15, 0, 0x122);
			}
			else
			{
				/* "Hall of Fame" */
				GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_HALL_OF_FAME2), Gfx.SCREEN_WIDTH / 2, 15, 15, 0, 0x122);
			}

			switch (CHouse.g_playerHouseID)
			{
				case HouseType.HOUSE_HARKONNEN:
					colour = 149;
					offset = 0;
					break;

				default:
					colour = 165;
					offset = 2;
					break;

				case HouseType.HOUSE_ORDOS:
					colour = 181;
					offset = 1;
					break;
			}

			s_palette1_houseColour = new CArray<byte> { Arr = Gfx.g_palette1[(colour * 3)..(colour * 3 + 3)] };
			Buffer.BlockCopy(s_palette1_houseColour.Arr, 0, Gfx.g_palette1, 255 * 3, 3);
			s_palette1_houseColour += offset;

			if (!hallOfFame) GUI_HallOfFame_Tick();

			Gfx.GFX_Screen_SetActive(oldScreenID);
		}

		internal static ushort GUI_HallOfFame_DrawData(HallOfFameStruct[] data, bool show)
		{
			Screen oldScreenID;
			string scoreString;
			string battleString;
			ushort width = 0;
			ushort offsetY;
			ushort scoreX;
			ushort battleX;
			byte i;

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);
			GUI_DrawFilledRectangle(8, 80, 311, 178, 116);
			GUI_DrawText_Wrapper(null, 0, 0, 0, 0, 0x22);

			battleString = CString.String_Get_ByIndex(Text.STR_BATTLE);
			scoreString = CString.String_Get_ByIndex(Text.STR_SCORE);

			scoreX = (ushort)(320 - CFont.Font_GetStringWidth(scoreString) / 2 - 12);
			battleX = (ushort)(scoreX - CFont.Font_GetStringWidth(scoreString) / 2 - 8 - CFont.Font_GetStringWidth(battleString) / 2);
			offsetY = 80;

			GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_NAME_AND_RANK), 32, (short)offsetY, 8, 0, 0x22);
			GUI_DrawText_Wrapper(battleString, (short)battleX, (short)offsetY, 8, 0, 0x122);
			GUI_DrawText_Wrapper(scoreString, (short)scoreX, (short)offsetY, 8, 0, 0x122);

			offsetY = 90;
			for (i = 0; i < 8; i++, offsetY += 11)
			{
				string buffer; //char[81];
				string p1, p2, name;

				if (data[i].score == 0) break;

				if (Config.g_config.language == (byte)Language.FRENCH)
				{
					p1 = CString.String_Get_ByIndex(_rankScores[data[i].rank].rankString);
					p2 = g_table_houseInfo[data[i].houseID].name;
				}
				else
				{
					p1 = g_table_houseInfo[data[i].houseID].name;
					p2 = CString.String_Get_ByIndex(_rankScores[data[i].rank].rankString);
				}
				name = new string(data[i].name).Replace("\0", string.Empty);
				buffer = $"{name}, {p1} {p2}"; //snprintf(buffer, sizeof(buffer), "%s, %s %s", data[i].name, p1, p2);

				if (name == string.Empty)
				{
					width = (ushort)(battleX - 36 - CFont.Font_GetStringWidth(buffer));
				}
				else
				{
					GUI_DrawText_Wrapper(buffer, 32, (short)offsetY, 15, 0, 0x22);
				}

				GUI_DrawText_Wrapper("{0}.", 24, (short)offsetY, 15, 0, 0x222, i + 1);
				GUI_DrawText_Wrapper("{0}", (short)battleX, (short)offsetY, 15, 0, 0x122, data[i].campaignID);
				GUI_DrawText_Wrapper("{0}", (short)scoreX, (short)offsetY, 15, 0, 0x122, data[i].score);
			}

			if (show)
			{
				GUI_Mouse_Hide_Safe();
				GUI_Screen_Copy(1, 80, 1, 80, 38, 100, Screen.NO1, Screen.NO0);
				GUI_Mouse_Show_Safe();
			}

			Gfx.GFX_Screen_SetActive(oldScreenID);

			return width;
		}

		static ushort GUI_HallOfFame_GetRank(ushort score)
		{
			byte i;

			for (i = 0; i < _rankScores.Length; i++)
			{
				if (_rankScores[i].score > score) break;
			}

			return Min(i, (ushort)(_rankScores.Length - 1));
		}

		static void GUI_HallOfFame_DrawRank(ushort score, bool fadeIn)
		{
			GUI_DrawText_Wrapper(CString.String_Get_ByIndex(_rankScores[GUI_HallOfFame_GetRank(score)].rankString), Gfx.SCREEN_WIDTH / 2, 49, 6, 0, 0x122);

			if (!fadeIn) return;

			GUI_Screen_FadeIn(10, 49, 10, 49, 20, 12, Screen.NO1, Screen.NO0);
		}

		static void GUI_StrategicMap_DrawRegion(byte houseId, ushort region, bool progressive)
		{
			string key; //char[4];
			string buffer; //char[81]
			short x;
			short y;
			byte[] sprite;

			GUI_Palette_CreateRemap(houseId);

			key = region.ToString(CSharpDune.Culture); //sprintf(key, "%hu", region);

			buffer = Ini.Ini_GetString("PIECES", key, null, Sprites.g_fileRegionINI);

			var temp = buffer.Split(",");
			x = short.Parse(temp[0], CSharpDune.Culture);
			y = short.Parse(temp[1], CSharpDune.Culture);
			//sscanf(buffer, "%hd,%hd", &x, &y);

			sprite = Sprites.g_sprites[477 + region];

			GUI_DrawSprite(Screen.NO1, sprite, (short)(x + 8), (short)(y + 24), 0, DRAWSPRITE_FLAG_REMAP, g_remap, (short)1);

			if (!progressive) return;

			GUI_Screen_FadeIn2((short)(x + 8), (short)(y + 24), Sprites.Sprite_GetWidth(sprite), Sprites.Sprite_GetHeight(sprite),
				Screen.NO1, Screen.NO0, (ushort)(GUI_StrategicMap_FastForwardToggleWithESC() ? 0 : 1), false);
		}

		static ushort GUI_StrategicMap_ScenarioSelection(ushort campaignID)
		{
			ushort count;
			string key; //char[6];
			bool loop;
			var hasRegions = false;
			string category; //char[16];
			var data = new StrategicMapData[20];
			ushort scenarioID = 0;
			ushort region = 0;
			ushort i;

			GUI_Palette_CreateRemap((byte)CHouse.g_playerHouseID);

			category = $"GROUP{campaignID:X}"; //sprintf(category, "GROUP%hu", campaignID);

			for (i = 0; i < data.Length; i++) data[i] = new StrategicMapData(); //memset(data, 0, 20 * sizeof(StrategicMapData));

			for (i = 0; i < 20; i++)
			{
				string buffer; //char[81]

				key = $"REG{(ushort)(i + 1):X}"; //sprintf(key, "REG%hu", (ushort)(i + 1));

				if ((buffer = Ini.Ini_GetString(category, key, null, Sprites.g_fileRegionINI)) == null) break;

				var temp = buffer.Split(",");
				data[i].index = short.Parse(temp[0], CSharpDune.Culture);
				data[i].arrow = short.Parse(temp[1], CSharpDune.Culture);
				data[i].offsetX = short.Parse(temp[2], CSharpDune.Culture);
				data[i].offsetY = short.Parse(temp[3], CSharpDune.Culture);
				//sscanf(buffer, "%hd,%hd,%hd,%hd", &data[i].index, &data[i].arrow, &data[i].offsetX, &data[i].offsetY);

				if (!GUI_StrategicMap_IsRegionDone((ushort)data[i].index)) hasRegions = true;

				Gfx.GFX_Screen_Copy2(data[i].offsetX, data[i].offsetY, (short)(i * 16), 152, 16, 16, Screen.NO1, Screen.NO1, false);
				Gfx.GFX_Screen_Copy2(data[i].offsetX, data[i].offsetY, (short)(i * 16), 0, 16, 16, Screen.NO1, Screen.NO1, false);
				GUI_DrawSprite(Screen.NO1, Sprites.g_sprites[505 + data[i].arrow], (short)(i * 16), 152, 0, DRAWSPRITE_FLAG_REMAP, g_remap, (short)1);
			}

			count = i;

			if (!hasRegions)
			{
				/* This campaign has no available regions left; reset all regions for this campaign */
				for (i = 0; i < count; i++)
				{
					GUI_StrategicMap_SetRegionDone((ushort)data[i].index, false);
				}
			}
			else
			{
				/* Mark all regions that are already done as not-selectable */
				for (i = 0; i < count; i++)
				{
					if (GUI_StrategicMap_IsRegionDone((ushort)data[i].index)) data[i].index = 0;
				}
			}

			GUI_Mouse_Hide_Safe();

			for (i = 0; i < count; i++)
			{
				if (data[i].index == 0) continue;

				Gfx.GFX_Screen_Copy2((short)(i * 16), 152, data[i].offsetX, data[i].offsetY, 16, 16, Screen.NO1, Screen.NO0, false);
			}

			GUI_Mouse_Show_Safe();
			Input.Input.Input_History_Clear();

			for (loop = true; loop; Sleep.sleepIdle())
			{
				region = (ushort)GUI_StrategicMap_ClickedRegion();

				if (region == 0) continue;

				for (i = 0; i < count; i++)
				{
					GUI_StrategicMap_AnimateArrows();

					if (data[i].index == region)
					{
						loop = false;
						scenarioID = i;
						break;
					}
				}
			}

			GUI_StrategicMap_SetRegionDone(region, true);

			GUI_StrategicMap_DrawText(string.Empty);

			GUI_StrategicMap_AnimateSelected(region, data);

			scenarioID += (ushort)((campaignID - 1) * 3 + 2);

			if (campaignID > 7) scenarioID--;
			if (campaignID > 8) scenarioID--;

			return scenarioID;
		}

		/*
		 * Return if a region has already been done.
		 * @param region Region to obtain.
		 * @return True if and only if the region has already been done.
		 */
		static bool GUI_StrategicMap_IsRegionDone(ushort region) =>
			(g_strategicRegionBits & (1 << region)) != 0;

		/*
		 * Set or reset if a region of the strategic map is already done.
		 * @param region Region to change.
		 * @param set Region must be set or reset.
		 */
		static void GUI_StrategicMap_SetRegionDone(ushort region, bool set)
		{
			if (set)
			{
				g_strategicRegionBits |= (uint)(1 << region);
			}
			else
			{
				g_strategicRegionBits &= (uint)~(1 << region);
			}
		}

		static short GUI_StrategicMap_ClickedRegion()
		{
			ushort key;

			GUI_StrategicMap_AnimateArrows();

			if (Input.Input.Input_Keyboard_NextKey() == 0) return 0;

			key = Input.Input.Input_WaitForValidInput();
			if (key != 0xC6 && key != 0xC7) return 0;

			return Sprites.g_fileRgnclkCPS[(Mouse.g_mouseClickY - 24) * 304 + Mouse.g_mouseClickX - 8];
		}

		static void GUI_StrategicMap_AnimateArrows()
		{
			if (s_arrowAnimationTimeout >= Timer.g_timerGUI) return;
			s_arrowAnimationTimeout = Timer.g_timerGUI + 7;

			s_arrowAnimationState = (ushort)((s_arrowAnimationState + 1) % 4);

			Array.Copy(s_strategicMapArrowColors, s_arrowAnimationState * 3, Gfx.g_palette1, 251 * 3, 4 * 3); //memcpy(g_palette1 + 251 * 3, s_strategicMapArrowColors + s_arrowAnimationState * 3, 4 * 3);

			Gfx.GFX_SetPalette(Gfx.g_palette1);
		}

		static void GUI_StrategicMap_ShowProgression(ushort campaignID)
		{
			string key; //char[10];
			string category; //char[10];
			string buf; //char[100]
			ushort i;
			string[] parts;

			category = $"GROUP{campaignID:X}"; //sprintf(category, "GROUP%hu", campaignID);

			for (i = 0; i < 6; i++)
			{
				var houseID = (byte)(((byte)CHouse.g_playerHouseID + i) % 6);

				key = g_table_houseInfo[houseID].name[0..3]; //strncpy(key, g_table_houseInfo[houseID].name, 3);
																	//key[3] = '\0';

				if ((buf = Ini.Ini_GetString(category, key, null, Sprites.g_fileRegionINI)) == null) continue;

				parts = buf.Split(",");

				for (var j = 0; j < parts.Length; j++)
				{
					var region = ushort.Parse(parts[j], CSharpDune.Culture);

					if (region != 0)
					{
						string buffer; //char[81]

						key = $"{CString.g_languageSuffixes[Config.g_config.language]}TXT{region}"; //sprintf(key, "%sTXT%d", g_languageSuffixes[g_config.language], region);

						if ((buffer = Ini.Ini_GetString(category, key, null, Sprites.g_fileRegionINI)) != null)
						{
							GUI_StrategicMap_DrawText(buffer);
						}

						GUI_StrategicMap_DrawRegion(houseID, region, true);
					}
				}
			}

			GUI_StrategicMap_DrawText(string.Empty);
		}

		/*
		 * Updates the score.
		 * @param score The base score.
		 * @param harvestedAllied Pointer to the total amount of spice harvested by allies.
		 * @param harvestedEnemy Pointer to the total amount of spice harvested by enemies.
		 * @param houseID The houseID of the player.
		 */
		static ushort Update_Score(short score, ushort harvestedAllied, ushort harvestedEnemy, byte houseID)
		{
			var find = new PoolFindStruct();
			ushort targetTime;
			ushort sumHarvestedAllied = 0;
			ushort sumHarvestedEnnemy = 0;
			uint tmp;

			if (score < 0) score = 0;

			find.houseID = houseID;
			find.type = 0xFFFF;
			find.index = 0xFFFF;

			while (true)
			{
				Structure s;

				s = PoolStructure.Structure_Find(find);
				if (s == null) break;
				if (s.o.type == (byte)StructureType.STRUCTURE_SLAB_1x1 || s.o.type == (byte)StructureType.STRUCTURE_SLAB_2x2 || s.o.type == (byte)StructureType.STRUCTURE_WALL) continue;

				score += (short)(g_table_structureInfo[s.o.type].o.buildCredits / 100);
			}

			CSharpDune.g_validateStrictIfZero++;

			find.houseID = (byte)HouseType.HOUSE_INVALID;
			find.type = (ushort)UnitType.UNIT_HARVESTER;
			find.index = 0xFFFF;

			while (true)
			{
				Unit u;

				u = PoolUnit.Unit_Find(find);
				if (u == null) break;

				if (CHouse.House_AreAllied(CUnit.Unit_GetHouseID(u), (byte)CHouse.g_playerHouseID))
				{
					sumHarvestedAllied += (ushort)(u.amount * 7);
				}
				else
				{
					sumHarvestedEnnemy += (ushort)(u.amount * 7);
				}
			}

			CSharpDune.g_validateStrictIfZero--;

			tmp = (uint)(harvestedEnemy + sumHarvestedEnnemy);
			harvestedEnemy = (ushort)((tmp > 65000) ? 65000 : (tmp & 0xFFFF));

			tmp = (uint)(harvestedAllied + sumHarvestedAllied);
			harvestedAllied = (ushort)((tmp > 65000) ? 65000 : (tmp & 0xFFFF));

			score += (short)(PoolHouse.House_Get_ByIndex(houseID).credits / 100);

			if (score < 0) score = 0;

			targetTime = (ushort)(CSharpDune.g_campaignID * 45);

			if (s_ticksPlayed < targetTime)
			{
				score += (short)(targetTime - s_ticksPlayed);
			}

			return (ushort)score;
		}

		static Widget GUI_HallOfFame_CreateButtons(HallOfFameStruct[] data)
		{
			string resumeString;
			string clearString;
			Widget wClear;
			Widget wResume;
			ushort width;

			s_temporaryColourBorderSchema = s_colourBorderSchema; //memcpy(s_temporaryColourBorderSchema, s_colourBorderSchema, sizeof(s_colourBorderSchema));
			s_colourBorderSchema = s_HOF_ColourBorderSchema; //memcpy(s_colourBorderSchema, s_HOF_ColourBorderSchema, sizeof(s_colourBorderSchema));

			resumeString = CString.String_Get_ByIndex(Text.STR_RESUME_GAME2);
			clearString = CString.String_Get_ByIndex(Text.STR_CLEAR_LIST);

			width = (ushort)(Max(CFont.Font_GetStringWidth(resumeString), CFont.Font_GetStringWidth(clearString)) + 6);

			/* "Clear List" */
			wClear = CWidget.GUI_Widget_Allocate(100, clearString[0], (ushort)(160 - width - 18), 180, 0xFFFE, (ushort)Text.STR_CLEAR_LIST);
			wClear.width = width;
			wClear.height = 10;
			wClear.clickProc = WidgetClick.GUI_Widget_HOF_ClearList_Click;
			//memset(&wClear.flags, 0, sizeof(wClear.flags));
			wClear.flags.requiresClick = true;
			wClear.flags.clickAsHover = true;
			wClear.flags.loseSelect = true;
			wClear.flags.notused2 = true;
			wClear.flags.buttonFilterLeft = 4;
			wClear.flags.buttonFilterRight = 4;
			wClear.data = data;

			/* "Resume Game" */
			wResume = CWidget.GUI_Widget_Allocate(101, resumeString[0], 178, 180, 0xFFFE, (ushort)Text.STR_RESUME_GAME2);
			wResume.width = width;
			wResume.height = 10;
			wResume.clickProc = WidgetClick.GUI_Widget_HOF_Resume_Click;
			//memset(&wResume.flags, 0, sizeof(wResume.flags));
			wResume.flags.requiresClick = true;
			wResume.flags.clickAsHover = true;
			wResume.flags.loseSelect = true;
			wResume.flags.notused2 = true;
			wResume.flags.buttonFilterLeft = 4;
			wResume.flags.buttonFilterRight = 4;
			wResume.data = data;

			return CWidget.GUI_Widget_Insert(wClear, wResume);
		}

		static void GUI_HallOfFame_DeleteButtons(Widget w)
		{
			while (w != null)
			{
				var next = w.next;
				w = null; //free(w);
				w = next;
			}

			s_colourBorderSchema = s_temporaryColourBorderSchema; //memcpy(s_colourBorderSchema, s_temporaryColourBorderSchema, sizeof(s_temporaryColourBorderSchema));
		}

		static byte[] GUI_HallOfFame_Encode(HallOfFameStruct[] data)
		{
			byte i;
			byte[] d;
			var dPointer = 0;

			d = HallOfFameStruct.AllToBytes(data);

			for (/*d = (uint8*)data,*/ i = 0; i < 128; i++, dPointer++) d[dPointer] = (byte)((d[dPointer] + i) ^ 0xA7);

			return d;
		}

		static HallOfFameStruct[] GUI_HallOfFame_Decode(byte[] d)
		{
			byte i;
			var dPointer = 0;

			for (/*d = (uint8*)data,*/ i = 0; i < 128; i++, dPointer++) d[dPointer] = (byte)((d[dPointer] ^ 0xA7) - i);

			return HallOfFameStruct.AllFromBytes(d);
		}

		static ushort GUI_HallOfFame_InsertScore(HallOfFameStruct[] data, ushort score)
		{
			ushort i;
			for (i = 0; i < 8; i++/*, data++*/)
			{
				if (data[i].score >= score) continue;

				Array.Copy(data, i, data, i + 1, data.Length - i - 1); //memmove(data + 1, data, 128);
				data[i] = new HallOfFameStruct
				{
					//memset(data->name, 0, 6);
					score = score,
					houseID = (byte)CHouse.g_playerHouseID,
					rank = GUI_HallOfFame_GetRank(score),
					campaignID = CSharpDune.g_campaignID
				};

				return (ushort)(i + 1);
			}

			return 0;
		}

		static readonly ushort[] gameSpeedStrings = {
			(ushort)Text.STR_SLOWEST,
			(ushort)Text.STR_SLOW,
			(ushort)Text.STR_NORMAL,
			(ushort)Text.STR_FAST,
			(ushort)Text.STR_FASTEST
		};
		internal static string GUI_String_Get_ByIndex(short stringID)
		{
			switch (stringID)
			{
				case -5:
				case -4:
				case -3:
				case -2:
				case -1:
					{
						var s = WidgetClick.g_savegameDesc[Abs(stringID + 1)];
						if (string.IsNullOrEmpty(s)) return null;
						return s;
					}

				case -10:
					stringID = (short)((Config.g_gameConfig.music != 0) ? Text.STR_ON : Text.STR_OFF);
					break;

				case -11:
					stringID = (short)((Config.g_gameConfig.sounds != 0) ? Text.STR_ON : Text.STR_OFF);
					break;

				case -12:
					stringID = (short)gameSpeedStrings[Config.g_gameConfig.gameSpeed];
					break;

				case -13:
					stringID = (short)((Config.g_gameConfig.hints != 0) ? Text.STR_ON : Text.STR_OFF);
					break;

				case -14:
					stringID = (short)((Config.g_gameConfig.autoScroll != 0) ? Text.STR_ON : Text.STR_OFF);
					break;

				default: break;
			}

			return CString.String_Get_ByIndex((ushort)stringID);
		}

		/*
		 * Display the window where you can order/build stuff for a structure.
		 * @param isConstructionYard True if this is for a construction yard.
		 * @param isStarPort True if this is for a starport.
		 * @param upgradeCost Cost of upgrading the structure.
		 * @return Unknown value.
		 */
		internal static FactoryResult GUI_DisplayFactoryWindow(bool isConstructionYard, bool isStarPort, ushort upgradeCost)
		{
			Screen oldScreenID;
			var backup = new byte[3];

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO0);

			Buffer.BlockCopy(Gfx.g_palette1, 255 * 3, backup, 0, 3); //memcpy(backup, g_palette1 + 255 * 3, 3);

			g_factoryWindowConstructionYard = isConstructionYard; /* always same value as g_factoryWindowConstructionYard */
			g_factoryWindowStarport = isStarPort;
			g_factoryWindowUpgradeCost = upgradeCost;
			g_factoryWindowOrdered = 0;

			GUI_FactoryWindow_Init();

			GUI_FactoryWindow_UpdateSelection(true);

			for (g_factoryWindowResult = FactoryResult.FACTORY_CONTINUE; g_factoryWindowResult == FactoryResult.FACTORY_CONTINUE; Sleep.sleepIdle())
			{
				ushort evt;

				GUI_DrawCredits((byte)CHouse.g_playerHouseID, 0);

				GUI_FactoryWindow_UpdateSelection(false);

				evt = CWidget.GUI_Widget_HandleEvents(CWidget.g_widgetInvoiceTail);

				if (evt == 0x6E) WidgetClick.GUI_Production_ResumeGame_Click(null);

				GUI_PaletteAnimate();
			}

			GUI_DrawCredits((byte)CHouse.g_playerHouseID, 1);

			Gfx.GFX_Screen_SetActive(oldScreenID);

			GUI_FactoryWindow_B495_0F30();

			Buffer.BlockCopy(backup, 0, Gfx.g_palette1, 255 * 3, 3); //memcpy(g_palette1 + 255 * 3, backup, 3);

			Gfx.GFX_SetPalette(Gfx.g_palette1);

			/* Visible credits have to be reset, as it might not be the real value */
			CHouse.g_playerCredits = 0xFFFF;

			return g_factoryWindowResult;
		}

		static uint paletteChangeTimer;
		static sbyte paletteColour;
		static sbyte paletteChange;
		/*
		 * Update the selection in the factory window.
		 * If \a selectionChanged, it draws the rectangle around the new entry.
		 * In addition, the palette colour of the rectangle is slowly changed back and
		 * forth between white and the house colour by palette changes, thus giving it
		 * the appearance of glowing.
		 * @param selectionChanged User has selected a new thing to build.
		 */
		internal static void GUI_FactoryWindow_UpdateSelection(bool selectionChanged)
		{
			if (selectionChanged)
			{
				ushort y;

				Array.Fill<byte>(Gfx.g_palette1, 0x3F, 255 * 3, 3); //memset(g_palette1 + 255 * 3, 0x3F, 3);

				/* calling GFX_SetPalette() now is useless as it will be done at the end of the function */
				/*GFX_SetPalette(g_palette1);*/

				paletteChangeTimer = 0;
				paletteColour = 0;
				paletteChange = 8;

				y = (ushort)(g_factoryWindowSelected * 32 + 24);

				GUI_Mouse_Hide_Safe();
				GUI_DrawWiredRectangle(71, (ushort)(y - 1), 104, (ushort)(y + 24), 255);
				GUI_DrawWiredRectangle(72, y, 103, (ushort)(y + 23), 255);
				GUI_Mouse_Show_Safe();
			}
			else
			{
				if (paletteChangeTimer > Timer.g_timerGUI) return;
			}

			paletteChangeTimer = Timer.g_timerGUI + 3;
			paletteColour += paletteChange;

			if (paletteColour < 0 || paletteColour > 63)
			{
				paletteChange = (sbyte)-paletteChange;
				paletteColour += paletteChange;
				return;
			}

			switch (CHouse.g_playerHouseID)
			{
				case HouseType.HOUSE_HARKONNEN:
					Gfx.g_palette1[255 * 3 + 1] = (byte)paletteColour;
					Gfx.g_palette1[255 * 3 + 2] = (byte)paletteColour;
					break;

				case HouseType.HOUSE_ATREIDES:
					Gfx.g_palette1[255 * 3 + 0] = (byte)paletteColour;
					Gfx.g_palette1[255 * 3 + 1] = (byte)paletteColour;
					break;

				case HouseType.HOUSE_ORDOS:
					Gfx.g_palette1[255 * 3 + 0] = (byte)paletteColour;
					Gfx.g_palette1[255 * 3 + 2] = (byte)paletteColour;
					break;

				default: break;
			}

			Gfx.GFX_SetPalette(Gfx.g_palette1);
		}

		internal static void GUI_FactoryWindow_B495_0F30()
		{
			GUI_Mouse_Hide_Safe();
			Gfx.GFX_Screen_Copy2(69, (short)(((g_factoryWindowSelected + 1) * 32) + 5), 69, (short)((g_factoryWindowSelected * 32) + 21), 38, 30, Screen.NO1, Screen.NO0, false);
			GUI_Mouse_Show_Safe();
		}

		static readonly byte[] xSrc = { 0, 0, 16, 0, 0, 0 }; //HOUSE_MAX
		static readonly byte[] ySrc = { 8, 152, 48, 0, 0, 0 }; //HOUSE_MAX
		static void GUI_FactoryWindow_Init()
		{
			Screen oldScreenID;
			/* void* *//*WSAObject*/
			(WSAHeader header, CArray<byte> buffer) wsa;
			short i;
			ObjectInfo oi;

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);

			Sprites.Sprites_LoadImage("CHOAM.CPS", Screen.NO1, null);
			GUI_DrawSprite(Screen.NO1, Sprites.g_sprites[11], 192, 0, 0, 0); /* "Credits" */

			GUI_Palette_RemapScreen(0, 0, Gfx.SCREEN_WIDTH, Gfx.SCREEN_HEIGHT, Screen.NO1, g_remap);

			GUI_Screen_Copy(xSrc[(int)CHouse.g_playerHouseID], ySrc[(int)CHouse.g_playerHouseID], 0, 8, 7, 40, Screen.NO1, Screen.NO1);
			GUI_Screen_Copy(xSrc[(int)CHouse.g_playerHouseID], ySrc[(int)CHouse.g_playerHouseID], 0, 152, 7, 40, Screen.NO1, Screen.NO1);

			GUI_FactoryWindow_CreateWidgets();
			GUI_FactoryWindow_LoadGraymapTbl();
			GUI_FactoryWindow_InitItems();

			for (i = (short)g_factoryWindowTotal; i < 4; i++) CWidget.GUI_Widget_MakeInvisible(CWidget.GUI_Widget_Get_ByIndex(CWidget.g_widgetInvoiceTail, (ushort)(i + 46)));

			for (i = 0; i < 4; i++)
			{
				var item = GUI_FactoryWindow_GetItem(i);

				if (item == null) continue;

				oi = item.objectInfo;
				if (oi.available == -1)
				{
					GUI_DrawSprite(Screen.NO1, Sprites.g_sprites[oi.spriteID], 72, (short)(24 + i * 32), 0, DRAWSPRITE_FLAG_REMAP, s_factoryWindowGraymapTbl, (short)1);
				}
				else
				{
					GUI_DrawSprite(Screen.NO1, Sprites.g_sprites[oi.spriteID], 72, (short)(24 + i * 32), 0, 0);
				}
			}

			g_factoryWindowBase = 0;
			g_factoryWindowSelected = 0;

			oi = g_factoryWindowItems[0].objectInfo;

			wsa = Wsa.WSA_LoadFile(oi.wsa, s_factoryWindowWsaBuffer, (uint)s_factoryWindowWsaBuffer.Length, false);
			Wsa.WSA_DisplayFrame(wsa, 0, 128, 48, Screen.NO1);
			Wsa.WSA_Unload(wsa);

			GUI_Mouse_Hide_Safe();
			GUI_Screen_Copy(0, 0, 0, 0, Gfx.SCREEN_WIDTH / 8, (short)Gfx.SCREEN_HEIGHT, Screen.NO1, Screen.NO0);
			GUI_Mouse_Show_Safe();

			GUI_DrawFilledRectangle(64, 0, 112, Gfx.SCREEN_HEIGHT - 1, Gfx.GFX_GetPixel(72, 23));

			GUI_FactoryWindow_PrepareScrollList();

			Gfx.GFX_Screen_SetActive(Screen.NO0);

			GUI_FactoryWindow_DrawDetails();

			GUI_DrawCredits((byte)CHouse.g_playerHouseID, 1);

			Gfx.GFX_Screen_SetActive(oldScreenID);
		}

		internal static FactoryWindowItem GUI_FactoryWindow_GetItem(short offset)
		{
			offset += (short)g_factoryWindowBase;

			if (offset < 0 || offset >= g_factoryWindowTotal) return null;

			return g_factoryWindowItems[offset];
		}

		static uint GUI_FactoryWindow_LoadGraymapTbl()
		{
			byte fileID;

			fileID = CFile.File_Open("GRAYRMAP.TBL", FileMode.FILE_MODE_READ);
			CFile.File_Read(fileID, ref s_factoryWindowGraymapTbl, 256);
			CFile.File_Close(fileID);

			return 256;
		}

		static uint GUI_FactoryWindow_CreateWidgets()
		{
			ushort i;
			ushort count = 0;
			var wi = g_table_factoryWidgetInfo;
			var w = s_factoryWindowWidgets;

			for (i = 0; i < 13; i++/*, wi++*/)
			{
				w[i] = new Widget(); //memset(w, 0, 13 * sizeof(Widget));

				if ((i == 8 || i == 9 || i == 10 || i == 12) && !g_factoryWindowStarport) continue;
				if (i == 11 && g_factoryWindowStarport) continue;
				if (i == 7 && g_factoryWindowUpgradeCost == 0) continue;

				count++;

				w[i].index = (ushort)(i + 46);
				//memset(&w.state, 0, sizeof(w.state));
				w[i].offsetX = (short)wi[i].offsetX;
				w[i].offsetY = (short)wi[i].offsetY;
				w[i].flags.Set(wi[i].flags);
				w[i].shortcut = (ushort)((wi[i].shortcut < 0) ? Abs(wi[i].shortcut) : CWidget.GUI_Widget_GetShortcut((byte)CString.String_Get_ByIndex((ushort)wi[i].shortcut)[0]));
				w[i].clickProc = wi[i].clickProc;
				w[i].width = wi[i].width;
				w[i].height = wi[i].height;

				if (wi[i].spriteID < 0)
				{
					w[i].drawModeNormal = (byte)DrawMode.DRAW_MODE_NONE;
					w[i].drawModeSelected = (byte)DrawMode.DRAW_MODE_NONE;
					w[i].drawModeDown = (byte)DrawMode.DRAW_MODE_NONE;
				}
				else
				{
					w[i].drawModeNormal = (byte)DrawMode.DRAW_MODE_SPRITE;
					w[i].drawModeSelected = (byte)DrawMode.DRAW_MODE_SPRITE;
					w[i].drawModeDown = (byte)DrawMode.DRAW_MODE_SPRITE;
					w[i].drawParameterNormal.sprite = Sprites.g_sprites[wi[i].spriteID];
					w[i].drawParameterSelected.sprite = Sprites.g_sprites[wi[i].spriteID + 1];
					w[i].drawParameterDown.sprite = Sprites.g_sprites[wi[i].spriteID + 1];
				}

				if (i != 0)
				{
					CWidget.g_widgetInvoiceTail = CWidget.GUI_Widget_Link(CWidget.g_widgetInvoiceTail, w[i]);
				}
				else
				{
					CWidget.g_widgetInvoiceTail = w[i];
				}

				//w++;
			}

			WidgetDraw.GUI_Widget_DrawAll(CWidget.g_widgetInvoiceTail);

			return count;
		}

		internal static void GUI_FactoryWindow_DrawDetails()
		{
			Screen oldScreenID;
			var item = GUI_FactoryWindow_GetItem((short)g_factoryWindowSelected);
			var oi = item.objectInfo;
			/* void* *//*WSAObject*/
			(WSAHeader header, CArray<byte> buffer) wsa;

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);

			wsa = Wsa.WSA_LoadFile(oi.wsa, s_factoryWindowWsaBuffer, (uint)s_factoryWindowWsaBuffer.Length, false);
			Wsa.WSA_DisplayFrame(wsa, 0, 128, 48, Screen.NO1);
			Wsa.WSA_Unload(wsa);

			if (g_factoryWindowConstructionYard)
			{
				StructureInfo si;
				short x = 288;
				short y = 136;
				byte[] sprite;
				ushort width;
				ushort i;
				ushort j;

				GUI_DrawSprite(Screen.NO1, Sprites.g_sprites[64], x, y, 0, 0);
				x++;
				y++;

				sprite = Sprites.g_sprites[24];
				width = (ushort)(Sprites.Sprite_GetWidth(sprite) + 1);
				si = g_table_structureInfo[item.objectType];

				for (j = 0; j < g_table_structure_layoutSize[si.layout].height; j++)
				{
					for (i = 0; i < g_table_structure_layoutSize[si.layout].width; i++)
					{
						GUI_DrawSprite(Screen.NO1, sprite, (short)(x + i * width), (short)(y + j * width), 0, 0);
					}
				}
			}

			if (oi.available == -1)
			{
				GUI_Palette_RemapScreen(128, 48, 184, 112, Screen.NO1, s_factoryWindowGraymapTbl);

				if (g_factoryWindowStarport)
				{
					GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_OUT_OF_STOCK), 220, 99, 6, 0, 0x132);
				}
				else
				{
					GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_NEED_STRUCTURE_UPGRADE), 220, 94, 6, 0, 0x132);

					if (g_factoryWindowUpgradeCost != 0)
					{
						GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_UPGRADE_COST_D), 220, 104, 6, 0, 0x132, g_factoryWindowUpgradeCost);
					}
					else
					{
						GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_REPAIR_STRUCTURE_FIRST), 220, 104, 6, 0, 0x132);
					}
				}
			}
			else
			{
				if (g_factoryWindowStarport)
				{
					GUI_Screen_Copy(16, 99, 16, 160, 23, 9, Screen.NO1, Screen.NO1);
					GUI_Screen_Copy(16, 99, 16, 169, 23, 9, Screen.NO1, Screen.NO1);
					GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_OUT_OF_STOCK), 220, 169, 6, 0, 0x132);
					GUI_Screen_Copy(16, 99, 16, 178, 23, 9, Screen.NO1, Screen.NO1);
					GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_UNABLE_TO_CREATE_MORE), 220, 178, 6, 0, 0x132);

					GUI_FactoryWindow_UpdateDetails(item);
				}
			}

			GUI_Mouse_Hide_Safe();
			GUI_Screen_Copy(16, 48, 16, 48, 23, 112, Screen.NO1, oldScreenID);
			GUI_Mouse_Show_Safe();

			Gfx.GFX_Screen_SetActive(oldScreenID);

			GUI_FactoryWindow_DrawCaption(null);
		}

		internal static void GUI_FactoryWindow_PrepareScrollList()
		{
			FactoryWindowItem item;

			GUI_Mouse_Hide_Safe();
			GUI_Screen_Copy(9, 24, 9, 40, 4, 128, Screen.NO0, Screen.NO1);
			GUI_Mouse_Show_Safe();

			item = GUI_FactoryWindow_GetItem(-1);

			if (item != null)
			{
				var oi = item.objectInfo;

				if (oi.available == -1)
				{
					GUI_DrawSprite(Screen.NO1, Sprites.g_sprites[oi.spriteID], 72, 8, 0, DRAWSPRITE_FLAG_REMAP, s_factoryWindowGraymapTbl, 1);
				}
				else
				{
					GUI_DrawSprite(Screen.NO1, Sprites.g_sprites[oi.spriteID], 72, 8, 0, 0);
				}
			}
			else
			{
				GUI_Screen_Copy(9, 32, 9, 24, 4, 8, Screen.NO1, Screen.NO1);
			}

			item = GUI_FactoryWindow_GetItem(4);

			if (item != null)
			{
				var oi = item.objectInfo;

				if (oi.available == -1)
				{
					GUI_DrawSprite(Screen.NO1, Sprites.g_sprites[oi.spriteID], 72, 168, 0, DRAWSPRITE_FLAG_REMAP, s_factoryWindowGraymapTbl, 1);
				}
				else
				{
					GUI_DrawSprite(Screen.NO1, Sprites.g_sprites[oi.spriteID], 72, 168, 0, 0);
				}
			}
			else
			{
				GUI_Screen_Copy(9, 0, 9, 168, 4, 8, Screen.NO1, Screen.NO1);
			}
		}

		internal static void GUI_FactoryWindow_DrawCaption(string caption)
		{
			Screen oldScreenID;

			oldScreenID = Gfx.GFX_Screen_SetActive(Screen.NO1);

			GUI_DrawFilledRectangle(128, 21, 310, 35, 116);

			if (!string.IsNullOrWhiteSpace(caption))
			{
				GUI_DrawText_Wrapper(caption, 128, 23, 12, 0, 0x12);
			}
			else
			{
				var item = GUI_FactoryWindow_GetItem((short)g_factoryWindowSelected);
				var oi = item.objectInfo;
				ushort width;

				GUI_DrawText_Wrapper(CString.String_Get_ByIndex(oi.stringID_full), 128, 23, 12, 0, 0x12);

				width = CFont.Font_GetStringWidth(CString.String_Get_ByIndex(Text.STR_COST_999));
				GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_COST_3D), (short)(310 - width), 23, 12, 0, 0x12, item.credits);

				if (g_factoryWindowStarport)
				{
					width += (ushort)(CFont.Font_GetStringWidth(CString.String_Get_ByIndex(Text.STR_QTY_99)) + 2);
					GUI_DrawText_Wrapper(CString.String_Get_ByIndex(Text.STR_QTY_2D), (short)(310 - width), 23, 12, 0, 0x12, item.amount);
				}
			}

			GUI_Mouse_Hide_Safe();
			if (oldScreenID == Screen.NO0) Gfx.GFX_Screen_Copy2(128, 21, 128, 21, 182, 14, Screen.NO1, oldScreenID, false);
			GUI_Mouse_Show_Safe();

			Gfx.GFX_Screen_SetActive(oldScreenID);
		}

		internal static void GUI_FactoryWindow_UpdateDetails(FactoryWindowItem item)
		{
			short y;
			var oi = item.objectInfo;
			var type = item.objectType;

			/* check the available units and unit count limit */
			if (oi.available == -1) return;

			y = 160;
			if (oi.available <= item.amount) y = 169;
			else if (CSharpDune.g_starPortEnforceUnitLimit && g_table_unitInfo[type].movementType != (ushort)MovementType.MOVEMENT_WINGER && g_table_unitInfo[type].movementType != (ushort)MovementType.MOVEMENT_SLITHER)
			{
				var h = CHouse.g_playerHouse;
				if (h.unitCount >= h.unitCountMax) y = 178;
			}
			GUI_Mouse_Hide_Safe();
			GUI_Screen_Copy(16, y, 16, 99, 23, 9, Screen.NO1, Screen.ACTIVE);
			GUI_Mouse_Show_Safe();
		}

		/*
		 * Draw a string to the screen using a fixed width for each char.
		 *
		 * @param string The string to draw.
		 * @param left The most left position where to draw the string.
		 * @param top The most top position where to draw the string.
		 * @param fgColour The foreground colour of the text.
		 * @param bgColour The background colour of the text.
		 * @param charWidth The width of a char.
		 */
		internal static void GUI_DrawText_Monospace(string str, ushort left, ushort top, byte fgColour, byte bgColour, ushort charWidth)
		{
			for (var i = 0; i < str.Length; i++)
			{
				GUI_DrawText(str[i].ToString(), (short)left, (short)top, fgColour, bgColour);
				left += charWidth;
			}
		}

		static void GUI_StrategicMap_AnimateSelected(ushort selected, StrategicMapData[] data)
		{
			string key; //char[4]
			string buffer; //char[81]
			short x;
			short y;
			byte[] sprite;
			ushort width;
			ushort height;
			ushort i;

			GUI_Palette_CreateRemap((byte)CHouse.g_playerHouseID);

			for (i = 0; i < 20; i++)
			{
				GUI_StrategicMap_AnimateArrows();

				if (data[i].index == 0 || data[i].index == selected) continue;

				GUI_Mouse_Hide_Safe();
				Gfx.GFX_Screen_Copy2((short)(i * 16), 0, data[i].offsetX, data[i].offsetY, 16, 16, Screen.NO1, Screen.NO0, false);
				GUI_Mouse_Show_Safe();
			}

			key = selected.ToString("D", CSharpDune.Culture); //sprintf(key, "%d", selected);

			buffer = Ini.Ini_GetString("PIECES", key, null, Sprites.g_fileRegionINI);

			var temp = buffer.Split(",");
			x = short.Parse(temp[0], CSharpDune.Culture);
			y = short.Parse(temp[1], CSharpDune.Culture);
			//sscanf(buffer, "%hd,%hd", &x, &y);

			sprite = Sprites.g_sprites[477 + selected];
			width = Sprites.Sprite_GetWidth(sprite);
			height = Sprites.Sprite_GetHeight(sprite);

			x += 8;
			y += 24;

			GUI_Mouse_Hide_Safe();
			Gfx.GFX_Screen_Copy2(x, y, 16, 16, (short)width, (short)height, Screen.NO0, Screen.NO1, false);
			GUI_Mouse_Show_Safe();

			Gfx.GFX_Screen_Copy2(16, 16, 176, 16, (short)width, (short)height, Screen.NO1, Screen.NO1, false);

			GUI_DrawSprite(Screen.NO1, sprite, 16, 16, 0, DRAWSPRITE_FLAG_REMAP, g_remap, (short)1);

			for (i = 0; i < 20; i++)
			{
				GUI_StrategicMap_AnimateArrows();

				if (data[i].index != selected) continue;

				GUI_DrawSprite(Screen.NO1, Sprites.g_sprites[505 + data[i].arrow], (short)(data[i].offsetX + 16 - x), (short)(data[i].offsetY + 16 - y), 0, DRAWSPRITE_FLAG_REMAP, g_remap, (short)1);
			}

			for (i = 0; i < 4; i++)
			{
				GUI_Mouse_Hide_Safe();
				Gfx.GFX_Screen_Copy2((short)((i % 2 == 0) ? 16 : 176), 16, x, y, (short)width, (short)height, Screen.NO1, Screen.NO0, false);
				GUI_Mouse_Show_Safe();

				for (Timer.g_timerTimeout = 20; Timer.g_timerTimeout != 0; Sleep.sleepIdle())
				{
					GUI_StrategicMap_AnimateArrows();
				}
			}
		}

		static void GUI_FactoryWindow_InitItems()
		{
			g_factoryWindowTotal = 0;
			g_factoryWindowSelected = 0;
			g_factoryWindowBase = 0;

			for (var i = 0; i < g_factoryWindowItems.Length; i++) g_factoryWindowItems[i] = new FactoryWindowItem(); //memset(g_factoryWindowItems, 0, 25 * sizeof(FactoryWindowItem));

			if (g_factoryWindowStarport)
			{
				var seconds = (ushort)((Timer.g_timerGame - CSharpDune.g_tickScenarioStart) / 60);
				var seed = (ushort)((seconds / 60) + CSharpDune.g_scenarioID + CHouse.g_playerHouseID);
				seed *= seed;

				Tools.Tools_RandomLCG_Seed(seed);
			}

			if (!g_factoryWindowConstructionYard)
			{
				ushort i;

				for (i = 0; i < (ushort)UnitType.UNIT_MAX; i++)
				{
					var oi = g_table_unitInfo[i].o;

					if (oi.available == 0) continue;

					g_factoryWindowItems[g_factoryWindowTotal].objectInfo = oi;
					g_factoryWindowItems[g_factoryWindowTotal].objectType = i;

					if (g_factoryWindowStarport)
					{
						g_factoryWindowItems[g_factoryWindowTotal].credits = (short)GUI_FactoryWindow_CalculateStarportPrice(oi.buildCredits);
					}
					else
					{
						g_factoryWindowItems[g_factoryWindowTotal].credits = (short)oi.buildCredits;
					}

					g_factoryWindowItems[g_factoryWindowTotal].sortPriority = oi.sortPriority;

					g_factoryWindowTotal++;
				}
			}
			else
			{
				ushort i;

				for (i = 0; i < (ushort)StructureType.STRUCTURE_MAX; i++)
				{
					var oi = g_table_structureInfo[i].o;

					if (oi.available == 0) continue;

					g_factoryWindowItems[g_factoryWindowTotal].objectInfo = oi;
					g_factoryWindowItems[g_factoryWindowTotal].objectType = i;
					g_factoryWindowItems[g_factoryWindowTotal].credits = (short)oi.buildCredits;
					g_factoryWindowItems[g_factoryWindowTotal].sortPriority = oi.sortPriority;

					if (i == 0 || i == 1) g_factoryWindowItems[g_factoryWindowTotal].sortPriority = (short)(0x64 + i);

					g_factoryWindowTotal++;
				}
			}

			if (g_factoryWindowTotal == 0)
			{
				GUI_DisplayModalMessage("ERROR: No items in construction list!", 0xFFFF);
				CSharpDune.PrepareEnd();
				Environment.Exit(0);
			}

			Array.Sort(g_factoryWindowItems, (a, b) => b.sortPriority - a.sortPriority);
			//qsort(g_factoryWindowItems, g_factoryWindowTotal, sizeof(FactoryWindowItem), GUI_FactoryWindow_Sorter);
		}

		static ushort GUI_FactoryWindow_CalculateStarportPrice(ushort credits)
		{
			credits = (ushort)((credits / 10) * 4 + (credits / 10) * (Tools.Tools_RandomLCG_Range(0, 6) + Tools.Tools_RandomLCG_Range(0, 6)));

			return Min(credits, (ushort)999);
		}
	}
}
