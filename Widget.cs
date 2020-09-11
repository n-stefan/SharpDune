/* Widget */

using System;
using System.Diagnostics;
using static System.Math;

namespace SharpDune
{
    delegate bool ClickProc(Widget widget);
	delegate void ScrollbarDrawProc(Widget widget);

	/*
	 * Types of DrawMode available in the game.
	 */
	enum DrawMode
	{
		DRAW_MODE_NONE = 0,                      /*!< Draw nothing. */
		DRAW_MODE_SPRITE = 1,                    /*!< Draw a sprite. */
		DRAW_MODE_TEXT = 2,                      /*!< Draw text. */
		DRAW_MODE_UNKNOWN3 = 3,
		DRAW_MODE_CUSTOM_PROC = 4,               /*!< Draw via a custom defined function. */
		DRAW_MODE_WIRED_RECTANGLE = 5,           /*!< Draw a wired rectangle. */
		DRAW_MODE_XORFILLED_RECTANGLE = 6,       /*!< Draw a filled rectangle using xor. */

		DRAW_MODE_MAX = 7
	}

	class WidgetFlags
	{
		internal bool requiresClick;                    /*!< Requires click. */
		internal bool notused1;
		internal bool clickAsHover;                     /*!< Click as hover. */
		internal bool invisible;                        /*!< Widget is invisible. */
		internal bool greyWhenInvisible;                /*!< Make the widget grey out when made invisible, instead of making it invisible. */
		internal bool noClickCascade;                   /*!< Don't cascade the click event to any other widgets. */
		internal bool loseSelect;                       /*!< Lose select when leave. */
		internal bool notused2;
		internal /*uint*/byte buttonFilterLeft;         /*!< Left button filter. */
		internal /*uint*/byte buttonFilterRight;        /*!< Right button filter. */

		internal void Set(ushort flags)
		{
			requiresClick = (flags & 0x0001) == 0x0001; // ? true : false;
			notused1 = (flags & 0x0002) == 0x0002; // ? true : false;
			clickAsHover = (flags & 0x0004) == 0x0004; // ? true : false;
			invisible = (flags & 0x0008) == 0x0008; // ? true : false;
			greyWhenInvisible = (flags & 0x0010) == 0x0010; // ? true : false;
			noClickCascade = (flags & 0x0020) == 0x0020; // ? true : false;
			loseSelect = (flags & 0x0040) == 0x0040; // ? true : false;
			notused2 = (flags & 0x0080) == 0x0080; // ? true : false;
			buttonFilterLeft = (byte)((flags >> 8) & 0x0f);
			buttonFilterRight = (byte)((flags >> 12) & 0x0f);
		}
	}

	class WidgetState
	{
		internal bool selected;                         /*!< Selected. */
		internal bool hover1;                           /*!< Hover. */
		internal bool hover2;                           /*!< Hover. */
		internal bool selectedLast;                     /*!< Last Selected. */
		internal bool hover1Last;                       /*!< Last Hover. */
		internal bool hover2Last;                       /*!< Last Hover. */
		internal bool notused;
		internal bool keySelected;                      /*!< Key Selected. */
		internal ulong buttonState;                     /*!< Button state. */
	}

	/*
    * The parameter for a given DrawMode.
    */
	class WidgetDrawParameter
	{
		internal ushort spriteID;                               /*!< Parameter for DRAW_MODE_UNKNOWN3. */
		internal /*void*/byte[] sprite;                         /*!< Parameter for DRAW_MODE_SPRITE. */
		internal string text;                                   /*!< Parameter for DRAW_MODE_TEXT. */
		internal Action<Widget> proc;                           /*!< Parameter for DRAW_MODE_CUSTOM_PROC. */
	}

	/* Widget properties. */
	class WidgetProperties
	{
		internal ushort xBase;                                  /*!< Horizontal base coordinate divided by 8. */
		internal ushort yBase;                                  /*!< Vertical base coordinate. */
		internal ushort width;                                  /*!< Width of the widget divided by 8. */
		internal ushort height;                                 /*!< Height of the widget. */
		internal byte fgColourBlink;                            /*!< Foreground colour for 'blink'. */
		internal byte fgColourNormal;                           /*!< Foreground colour for 'normal'. */
		internal byte fgColourSelected;                         /*!< Foreground colour when 'selected' */

		internal WidgetProperties Clone() =>
			(WidgetProperties)MemberwiseClone();
	}

	/*
    * A Widget as stored in the memory.
    */
	class Widget
	{
		internal Widget next;									/*!< Next widget in the list. */
		internal ushort index;                                  /*!< Index of the widget. */
		internal ushort shortcut;                               /*!< What key triggers this widget. */
		internal ushort shortcut2;                              /*!< What key (also) triggers this widget. */
		internal byte drawModeNormal;                           /*!< Draw mode when normal. */
		internal byte drawModeSelected;                         /*!< Draw mode when selected. */
		internal byte drawModeDown;                             /*!< Draw mode when down. */
		internal WidgetFlags flags;                             /*!< General flags of the Widget. */
		internal WidgetDrawParameter drawParameterNormal;       /*!< Draw parameter when normal. */
		internal WidgetDrawParameter drawParameterSelected;     /*!< Draw parameter when selected. */
		internal WidgetDrawParameter drawParameterDown;         /*!< Draw parameter when down. */
		internal ushort parentID;                               /*!< Parent window we are nested in. */
		internal short offsetX;                                 /*!< X position from parent we are at, in pixels. */
		internal short offsetY;                                 /*!< Y position from parent we are at, in pixels. */
		internal ushort width;                                  /*!< Width of widget in pixels. */
		internal ushort height;                                 /*!< Height of widget in pixels. */
		internal byte fgColourNormal;                           /*!< Foreground colour for draw proc when normal. */
		internal byte bgColourNormal;                           /*!< Background colour for draw proc when normal. */
		internal byte fgColourSelected;                         /*!< Foreground colour for draw proc when selected. */
		internal byte bgColourSelected;                         /*!< Background colour for draw proc when selected. */
		internal byte fgColourDown;                             /*!< Foreground colour for draw proc when down. */
		internal byte bgColourDown;                             /*!< Background colour for draw proc when down. */
		internal WidgetState state;                             /*!< State of the Widget. */
		internal ClickProc clickProc;                           /*!< Function to execute when widget is pressed. */
		//TODO: Make generic?
		internal object data;									/*!< If non-NULL, it points to WidgetScrollbar or HallOfFameData belonging to this widget. */
		internal ushort stringID;                               /*!< Strings to print on the widget. Index above 0xFFF2 are special. */

        internal Widget()
        {
			flags = new WidgetFlags();
			state = new WidgetState();
			drawParameterNormal = new WidgetDrawParameter();
			drawParameterSelected = new WidgetDrawParameter();
			drawParameterDown = new WidgetDrawParameter();
		}
	}

	/*
	 * Static information per WidgetClick type.
	 */
	class WidgetInfo
	{
		internal short index;              /*!< ?? */
		internal ClickProc clickProc;      /*!< Function to execute when widget is pressed. */
		internal short shortcut;           /*!< ?? */
		internal ushort flags;             /*!< ?? */
		internal short spriteID;           /*!< ?? */
		internal ushort offsetX;           /*!< ?? */
		internal ushort offsetY;           /*!< ?? */
		internal ushort width;             /*!< only used if spriteID < 0 */
		internal ushort height;            /*!< only used if spriteID < 0 */
		internal ushort stringID;          /*!< ?? */
	}

	class WindowDescWidget
	{
		internal ushort stringID;										/*!< String of the Widget. */
		internal ushort offsetX;                                        /*!< Offset in X-position of the Widget (relative to Window). */
		internal ushort offsetY;                                        /*!< Offset in Y-position of the Widget (relative to Window). */
		internal ushort width;                                          /*!< Width of the Widget. */
		internal ushort height;                                         /*!< Height of the Widget. */
		internal ushort labelStringId;                                  /*!< Label of the Widget. */
		internal ushort shortcut2;                                      /*!< The shortcut to trigger the Widget. */
	}

	/*
	 * Static information per WidgetClick type.
	 */
	class WindowDesc
	{
		internal ushort index;                                                  /*!< Index of the Window. */
		internal short stringID;                                                /*!< String for the Window. */
		internal bool addArrows;                                                /*!< If true, arrows are added to the Window. */
		internal byte widgetCount;                                              /*!< Amount of widgets following. */
		internal WindowDescWidget[] widgets = new WindowDescWidget[7];          /*!< The Widgets belonging to the Window. */
	}

	/*
	 * Scrollbar information as stored in the memory.
	 */
	class WidgetScrollbar
	{
		internal Widget parent;                               /*!< Parent widget we belong to. */
		internal ushort size;                                 /*!< Size (in pixels) of the scrollbar. */
		internal ushort position;                             /*!< Current position of the scrollbar. */
		internal ushort scrollMax;                            /*!< Maximum position of the scrollbar cursor. */
		internal ushort scrollPageSize;                       /*!< Amount of elements to scroll per page. */
		internal ushort scrollPosition;                       /*!< Current position of the scrollbar cursor. */
		internal byte pressed;                                /*!< If non-zero, the scrollbar is currently pressed. */
		internal byte dirty;                                  /*!< If non-zero, the scrollbar is dirty (requires repaint). */
		internal ushort pressedPosition;                      /*!< Position where we clicked on the scrollbar when pressed. */
		internal ScrollbarDrawProc drawProc;                  /*!< Draw proc (called on every draw). Can be null. */
	}

	class CWidget
	{
		/* Layout and other properties of the widgets. */
		internal static WidgetProperties[] g_widgetProperties = { //[22]
			/* x   y   w    h   p4  norm sel */
			new WidgetProperties { xBase =  0, yBase =   0, width = 40, height = 200, fgColourBlink =  15, fgColourNormal =  12, fgColourSelected = 0 }, /*  0 */
			new WidgetProperties { xBase =  1, yBase =  75, width = 29, height =  70, fgColourBlink =  15, fgColourNormal =  15, fgColourSelected = 0 }, /*  1 */
			new WidgetProperties { xBase =  0, yBase =  40, width = 30, height = 160, fgColourBlink =  15, fgColourNormal =  20, fgColourSelected = 0 }, /*  2 */
			new WidgetProperties { xBase = 32, yBase = 136, width =  8, height =  64, fgColourBlink =  15, fgColourNormal =  12, fgColourSelected = 0 }, /*  3 */
			new WidgetProperties { xBase = 32, yBase =  44, width =  8, height =   9, fgColourBlink =  29, fgColourNormal = 116, fgColourSelected = 0 }, /*  4 */
			new WidgetProperties { xBase = 32, yBase =   4, width =  8, height =   9, fgColourBlink =  29, fgColourNormal = 116, fgColourSelected = 0 }, /*  5 */
			new WidgetProperties { xBase = 32, yBase =  42, width =  8, height =  82, fgColourBlink =  15, fgColourNormal =  20, fgColourSelected = 0 }, /*  6 */
			new WidgetProperties { xBase =  1, yBase =  21, width = 38, height =  14, fgColourBlink =  12, fgColourNormal = 116, fgColourSelected = 0 }, /*  7 */
			new WidgetProperties { xBase = 16, yBase =  48, width = 23, height = 112, fgColourBlink =  15, fgColourNormal = 233, fgColourSelected = 0 }, /*  8 */
			new WidgetProperties { xBase =  2, yBase = 176, width = 36, height =  11, fgColourBlink =  15, fgColourNormal =  20, fgColourSelected = 0 }, /*  9 */
			new WidgetProperties { xBase =  0, yBase =  40, width = 40, height = 160, fgColourBlink =  29, fgColourNormal =  20, fgColourSelected = 0 }, /* 10 */
			new WidgetProperties { xBase = 16, yBase =  48, width = 23, height = 112, fgColourBlink =  29, fgColourNormal =  20, fgColourSelected = 0 }, /* 11 */
			new WidgetProperties { xBase =  9, yBase =  80, width = 22, height = 112, fgColourBlink =  29, fgColourNormal = 116, fgColourSelected = 0 }, /* 12 */
			new WidgetProperties { xBase = 12, yBase = 140, width = 16, height =  42, fgColourBlink = 236, fgColourNormal = 233, fgColourSelected = 0 }, /* 13 */
			new WidgetProperties { xBase =  2, yBase =  89, width = 36, height =  60, fgColourBlink =   0, fgColourNormal =   0, fgColourSelected = 0 }, /* 14 */
			new WidgetProperties { xBase =  4, yBase = 110, width = 32, height =  12, fgColourBlink = 232, fgColourNormal = 235, fgColourSelected = 0 }, /* 15 */
			new WidgetProperties { xBase =  5, yBase =  48, width = 30, height = 134, fgColourBlink =   0, fgColourNormal =   0, fgColourSelected = 0 }, /* 16 */
			new WidgetProperties { xBase =  3, yBase =  36, width = 36, height = 148, fgColourBlink =   0, fgColourNormal =   0, fgColourSelected = 0 }, /* 17 */
			new WidgetProperties { xBase =  1, yBase =  72, width = 38, height =  52, fgColourBlink =   0, fgColourNormal =   0, fgColourSelected = 0 }, /* 18 */
			new WidgetProperties { xBase =  0, yBase =   0, width =  0, height =   0, fgColourBlink =   0, fgColourNormal =   0, fgColourSelected = 0 }, /* 19 */
			new WidgetProperties { xBase =  2, yBase =  24, width = 36, height = 152, fgColourBlink =  12, fgColourNormal =  12, fgColourSelected = 0 }, /* 20 */
			new WidgetProperties { xBase =  1, yBase =   6, width = 12, height =   3, fgColourBlink =   0, fgColourNormal =  15, fgColourSelected = 6 }  /* 21 */
		};

		internal static ushort g_curWidgetIndex;          /*!< Index of the currently selected widget in #g_widgetProperties. */
		internal static ushort g_curWidgetXBase;          /*!< Horizontal base position of the currently selected widget. */
		internal static ushort g_curWidgetYBase;          /*!< Vertical base position of the currently selected widget. */
		internal static ushort g_curWidgetWidth;          /*!< Width of the currently selected widget. */
		internal static ushort g_curWidgetHeight;         /*!< Height of the currently selected widget. */
		internal static byte g_curWidgetFGColourBlink;    /*!< Blinking colour of the currently selected widget. */
		internal static byte g_curWidgetFGColourNormal;   /*!< Normal colour of the currently selected widget. */

		internal static Widget g_widgetLinkedListHead;
		internal static Widget g_widgetLinkedListTail;
		internal static Widget g_widgetInvoiceTail;

		static bool s_widgetReset; /*!< If true, the widgets will be redrawn. */

		internal static Widget g_widgetMentatFirst;
		internal static Widget g_widgetMentatTail;
		internal static Widget g_widgetMentatScrollUp;
		internal static Widget g_widgetMentatScrollDown;
		internal static Widget g_widgetMentatScrollbar;

		internal static Widget[] g_table_windowWidgets = {
			new Widget {
				next = null,
				index = 30,
				shortcut = 0,
				shortcut2 = 0,
				drawModeNormal = 1,
				drawModeSelected = 1,
				drawModeDown = 1,
				flags = new WidgetFlags { /* flags */
					requiresClick = true,
					notused1 = true,
					clickAsHover = false,
					invisible = false,
					greyWhenInvisible = false,
					noClickCascade = false,
					loseSelect = true,
					notused2 = false,
					buttonFilterLeft = 4,
					buttonFilterRight = 4
				},
				drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
				parentID = 0,
				offsetX = 0,
				offsetY = 0,
				width = 0,
				height = 0,
				fgColourNormal = 15,
				bgColourNormal = 12,
				fgColourSelected = 15,
				bgColourSelected = 12,
				fgColourDown = 15,
				bgColourDown = 12,
				state = new WidgetState { /* state */
					selected = false,
					hover1 = false,
					hover2 = false,
					selectedLast = false,
					hover1Last = false,
					hover2Last = false,
					notused = false,
					keySelected = false,
					buttonState = 0
				},
				clickProc = null,
				data = null,
				stringID = (ushort)Text.STR_NULL
			},
			new Widget {
				next = null,
				index = 31,
				shortcut = 0,
				shortcut2 = 0,
				drawModeNormal = 1,
				drawModeSelected = 1,
				drawModeDown = 1,
				flags = new WidgetFlags { /* flags */
					requiresClick = true,
					notused1 = true,
					clickAsHover = false,
					invisible = false,
					greyWhenInvisible = false,
					noClickCascade = false,
					loseSelect = true,
					notused2 = false,
					buttonFilterLeft = 4,
					buttonFilterRight = 4
				},
				drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
				parentID = 0,
				offsetX = 0,
				offsetY = 0,
				width = 0,
				height = 0,
				fgColourNormal = 15,
				bgColourNormal = 12,
				fgColourSelected = 15,
				bgColourSelected = 12,
				fgColourDown = 15,
				bgColourDown = 12,
				state = new WidgetState { /* state */
					selected = false,
					hover1 = false,
					hover2 = false,
					selectedLast = false,
					hover1Last = false,
					hover2Last = false,
					notused = false,
					keySelected = false,
					buttonState = 0
				},
				clickProc = null,
				data = null,
				stringID = (ushort)Text.STR_NULL
			},
			new Widget {
				next = null,
				index = 32,
				shortcut = 0,
				shortcut2 = 0,
				drawModeNormal = 1,
				drawModeSelected = 1,
				drawModeDown = 1,
				flags = new WidgetFlags { /* flags */
					requiresClick = true,
					notused1 = true,
					clickAsHover = false,
					invisible = false,
					greyWhenInvisible = false,
					noClickCascade = false,
					loseSelect = true,
					notused2 = false,
					buttonFilterLeft = 4,
					buttonFilterRight = 4
				},
				drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
				parentID = 0,
				offsetX = 0,
				offsetY = 0,
				width = 0,
				height = 0,
				fgColourNormal = 15,
				bgColourNormal = 12,
				fgColourSelected = 15,
				bgColourSelected = 12,
				fgColourDown = 15,
				bgColourDown = 12,
				state = new WidgetState { /* state */
					selected = false,
					hover1 = false,
					hover2 = false,
					selectedLast = false,
					hover1Last = false,
					hover2Last = false,
					notused = false,
					keySelected = false,
					buttonState = 0
				},
				clickProc = null,
				data = null,
				stringID = (ushort)Text.STR_NULL
			},
			new Widget {
				next = null,
				index = 33,
				shortcut = 0,
				shortcut2 = 0,
				drawModeNormal = 1,
				drawModeSelected = 1,
				drawModeDown = 1,
				flags = new WidgetFlags { /* flags */
					requiresClick = true,
					notused1 = true,
					clickAsHover = false,
					invisible = false,
					greyWhenInvisible = false,
					noClickCascade = false,
					loseSelect = true,
					notused2 = false,
					buttonFilterLeft = 4,
					buttonFilterRight = 4
				},
				drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
				parentID = 0,
				offsetX = 0,
				offsetY = 0,
				width = 0,
				height = 0,
				fgColourNormal = 15,
				bgColourNormal = 12,
				fgColourSelected = 15,
				bgColourSelected = 12,
				fgColourDown = 15,
				bgColourDown = 12,
				state = new WidgetState { /* state */
					selected = false,
					hover1 = false,
					hover2 = false,
					selectedLast = false,
					hover1Last = false,
					hover2Last = false,
					notused = false,
					keySelected = false,
					buttonState = 0
				},
				clickProc = null,
				data = null,
				stringID = (ushort)Text.STR_NULL
			},
			new Widget {
				next = null,
				index = 34,
				shortcut = 0,
				shortcut2 = 0,
				drawModeNormal = 1,
				drawModeSelected = 1,
				drawModeDown = 1,
				flags = new WidgetFlags { /* flags */
					requiresClick = true,
					notused1 = true,
					clickAsHover = false,
					invisible = false,
					greyWhenInvisible = false,
					noClickCascade = false,
					loseSelect = true,
					notused2 = false,
					buttonFilterLeft = 4,
					buttonFilterRight = 4
				},
				drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
				parentID = 0,
				offsetX = 0,
				offsetY = 0,
				width = 0,
				height = 0,
				fgColourNormal = 15,
				bgColourNormal = 12,
				fgColourSelected = 15,
				bgColourSelected = 12,
				fgColourDown = 15,
				bgColourDown = 12,
				state = new WidgetState { /* state */
					selected = false,
					hover1 = false,
					hover2 = false,
					selectedLast = false,
					hover1Last = false,
					hover2Last = false,
					notused = false,
					keySelected = false,
					buttonState = 0
				},
				clickProc = null,
				data = null,
				stringID = (ushort)Text.STR_NULL
			},
			new Widget {
				next = null,
				index = 35,
				shortcut = 0,
				shortcut2 = 0,
				drawModeNormal = 1,
				drawModeSelected = 1,
				drawModeDown = 1,
				flags = new WidgetFlags { /* flags */
					requiresClick = true,
					notused1 = true,
					clickAsHover = false,
					invisible = false,
					greyWhenInvisible = false,
					noClickCascade = false,
					loseSelect = true,
					notused2 = false,
					buttonFilterLeft = 4,
					buttonFilterRight = 4
				},
				drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
				parentID = 0,
				offsetX = 0,
				offsetY = 0,
				width = 0,
				height = 0,
				fgColourNormal = 15,
				bgColourNormal = 12,
				fgColourSelected = 15,
				bgColourSelected = 12,
				fgColourDown = 15,
				bgColourDown = 12,
				state = new WidgetState { /* state */
					selected = false,
					hover1 = false,
					hover2 = false,
					selectedLast = false,
					hover1Last = false,
					hover2Last = false,
					notused = false,
					keySelected = false,
					buttonState = 0
				},
				clickProc = null,
				data = null,
				stringID = (ushort)Text.STR_NULL
			},
			new Widget {
				next = null,
				index = 36,
				shortcut = 0,
				shortcut2 = 0,
				drawModeNormal = 1,
				drawModeSelected = 1,
				drawModeDown = 1,
				flags = new WidgetFlags { /* flags */
					requiresClick = true,
					notused1 = true,
					clickAsHover = false,
					invisible = false,
					greyWhenInvisible = false,
					noClickCascade = false,
					loseSelect = true,
					notused2 = false,
					buttonFilterLeft = 4,
					buttonFilterRight = 4
				},
				drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
				parentID = 0,
				offsetX = 0,
				offsetY = 0,
				width = 0,
				height = 0,
				fgColourNormal = 15,
				bgColourNormal = 12,
				fgColourSelected = 15,
				bgColourSelected = 12,
				fgColourDown = 15,
				bgColourDown = 12,
				state = new WidgetState { /* state */
					selected = false,
					hover1 = false,
					hover2 = false,
					selectedLast = false,
					hover1Last = false,
					hover2Last = false,
					notused = false,
					keySelected = false,
					buttonState = 0
				},
				clickProc = null,
				data = null,
				stringID = (ushort)Text.STR_NULL
			},
			new Widget {
				next = null,
				index = 37,
				shortcut = 96,
				shortcut2 = 0,
				drawModeNormal = 1,
				drawModeSelected = 1,
				drawModeDown = 1,
				flags = new WidgetFlags { /* flags */
					requiresClick = true,
					notused1 = true,
					clickAsHover = true,
					invisible = false,
					greyWhenInvisible = false,
					noClickCascade = false,
					loseSelect = false,
					notused2 = true,
					buttonFilterLeft = 4,
					buttonFilterRight = 4
				},
				drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
				parentID = 17,
				offsetX = 128,
				offsetY = 20,
				width = 25,
				height = 16,
				fgColourNormal = 15,
				bgColourNormal = 12,
				fgColourSelected = 15,
				bgColourSelected = 12,
				fgColourDown = 15,
				bgColourDown = 12,
				state = new WidgetState { /* state */
					selected = false,
					hover1 = false,
					hover2 = false,
					selectedLast = false,
					hover1Last = false,
					hover2Last = false,
					notused = false,
					keySelected = false,
					buttonState = 0
				},
				clickProc = null,
				data = null,
				stringID = (ushort)Text.STR_NULL
			},
			new Widget {
				next = null,
				index = 38,
				shortcut = 98,
				shortcut2 = 0,
				drawModeNormal = 1,
				drawModeSelected = 1,
				drawModeDown = 1,
				flags = new WidgetFlags { /* flags */
					requiresClick = true,
					notused1 = true,
					clickAsHover = true,
					invisible = false,
					greyWhenInvisible = false,
					noClickCascade = false,
					loseSelect = false,
					notused2 = true,
					buttonFilterLeft = 4,
					buttonFilterRight = 4
				},
				drawParameterNormal = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterSelected = new WidgetDrawParameter(), //{ 0x0 },
				drawParameterDown = new WidgetDrawParameter(), //{ 0x0 },
				parentID = 17,
				offsetX = 128,
				offsetY = 126,
				width = 25,
				height = 16,
				fgColourNormal = 15,
				bgColourNormal = 12,
				fgColourSelected = 15,
				bgColourSelected = 12,
				fgColourDown = 15,
				bgColourDown = 12,
				state = new WidgetState { /* state */
					selected = false,
					hover1 = false,
					hover2 = false,
					selectedLast = false,
					hover1Last = false,
					hover2Last = false,
					notused = false,
					keySelected = false,
					buttonState = 0
				},
				clickProc = null,
				data = null,
				stringID = (ushort)Text.STR_NULL
			}
		};

		/*
         * Select a widget as current widget.
         * @param index %Widget number to select.
         * @return Index of the previous selected widget.
         */
		internal static ushort Widget_SetCurrentWidget(ushort index)
		{
			ushort oldIndex = g_curWidgetIndex;
			g_curWidgetIndex = index;

			g_curWidgetXBase = g_widgetProperties[index].xBase;
			g_curWidgetYBase = g_widgetProperties[index].yBase;
			g_curWidgetWidth = g_widgetProperties[index].width;
			g_curWidgetHeight = g_widgetProperties[index].height;
			g_curWidgetFGColourBlink = g_widgetProperties[index].fgColourBlink;
			g_curWidgetFGColourNormal = g_widgetProperties[index].fgColourNormal;

			return oldIndex;
		}

		/*
         * Find an existing Widget by the index number. It matches the first hit, and
         *  returns that widget to you.
         * @param w The first widget to start searching from.
         * @param index The index of the widget you are looking for.
         * @return The widget, or NULL if not found.
         */
		internal static Widget GUI_Widget_Get_ByIndex(Widget w, ushort index)
		{
			if (index == 0) return w;

			while (w != null)
			{
				if (w.index == index) return w;
				w = GUI_Widget_GetNext(w);
			}

			return null;
		}

		internal static Widget GUI_Widget_GetNext(Widget w)
		{
			if (w.next == null) return null;
			return w.next;
		}

		/*
         * Make the widget invisible.
         * @param w The widget to make invisible.
         */
		internal static void GUI_Widget_MakeInvisible(Widget w)
		{
			if (w == null || w.flags.invisible) return;
			w.flags.invisible = true;

			GUI_Widget_Draw(w);
		}

		/*
         * Make the widget visible.
         * @param w The widget to make visible.
         */
		internal static void GUI_Widget_MakeVisible(Widget w)
		{
			if (w == null || !w.flags.invisible) return;
			w.flags.invisible = false;

			GUI_Widget_Draw(w);
		}

		/*
         * Reset the Widget to a normal state (not selected, not clicked).
         *
         * @param w The widget to reset.
         * @param clickProc Wether to execute the widget clickProc.
         */
		internal static void GUI_Widget_MakeNormal(Widget w, bool clickProc)
		{
			if (w == null || w.flags.invisible) return;

			w.state.selectedLast = w.state.selected;
			w.state.hover1Last = w.state.hover2;

			w.state.selected = false;
			w.state.hover1 = false;
			w.state.hover2 = false;

			GUI_Widget_Draw(w);

			if (!clickProc || w.clickProc == null) return;

			w.clickProc(w);
		}

		/*
         * Draw a widget to the display.
         *
         * @param w The widget to draw.
         */
		internal static void GUI_Widget_Draw(Widget w)
		{
			ushort positionLeft, positionRight;
			ushort positionTop, positionBottom;
			ushort offsetX, offsetY;
			ushort drawMode;
			byte fgColour, bgColour;
			WidgetDrawParameter drawParam;

			if (w == null) return;

			if (w.flags.invisible)
			{
				if (!w.flags.greyWhenInvisible) return;

				GUI_Widget_DrawBlocked(w, 12);
				return;
			}

			if (!w.state.hover2)
			{
				if (!w.state.selected)
				{
					drawMode = w.drawModeNormal;
					drawParam = w.drawParameterNormal;
					fgColour = w.fgColourNormal;
					bgColour = w.bgColourNormal;
				}
				else
				{
					drawMode = w.drawModeSelected;
					drawParam = w.drawParameterSelected;
					fgColour = w.fgColourSelected;
					bgColour = w.bgColourSelected;
				}
			}
			else
			{
				drawMode = w.drawModeDown;
				drawParam = w.drawParameterDown;
				fgColour = w.fgColourDown;
				bgColour = w.bgColourDown;
			}

			offsetX = (ushort)w.offsetX;
			if (w.offsetX < 0)
			{
				offsetX = (ushort)((g_widgetProperties[w.parentID].width << 3) + w.offsetX);
			}
			positionLeft = (ushort)((g_widgetProperties[w.parentID].xBase << 3) + offsetX);
			positionRight = (ushort)(positionLeft + w.width - 1);

			offsetY = (ushort)w.offsetY;
			if (w.offsetY < 0)
			{
				offsetY = (ushort)(g_widgetProperties[w.parentID].height + w.offsetY);
			}
			positionTop = (ushort)(g_widgetProperties[w.parentID].yBase + offsetY);
			positionBottom = (ushort)(positionTop + w.height - 1);

			Debug.Assert(drawMode < (ushort)DrawMode.DRAW_MODE_MAX);
			if (drawMode != (ushort)DrawMode.DRAW_MODE_NONE && drawMode != (ushort)DrawMode.DRAW_MODE_CUSTOM_PROC && Gfx.GFX_Screen_IsActive(Screen.SCREEN_0))
			{
				Gui.GUI_Mouse_Hide_InRegion(positionLeft, positionTop, positionRight, positionBottom);
			}

			switch (drawMode)
			{
				case (ushort)DrawMode.DRAW_MODE_NONE: break;

				case (ushort)DrawMode.DRAW_MODE_SPRITE:
					{
						Gui.GUI_DrawSprite(Screen.SCREEN_ACTIVE, drawParam.sprite, (short)offsetX, (short)offsetY, w.parentID, Gui.DRAWSPRITE_FLAG_REMAP | Gui.DRAWSPRITE_FLAG_WIDGETPOS, Gui.g_remap, (short)1);
					}
					break;

				case (ushort)DrawMode.DRAW_MODE_TEXT:
					{
						Gui.GUI_DrawText(drawParam.text, (short)positionLeft, (short)positionTop, fgColour, bgColour);
					}
					break;

				case (ushort)DrawMode.DRAW_MODE_UNKNOWN3:
					{
						Gfx.GFX_DrawTile(drawParam.spriteID, positionLeft, positionTop, (byte)HouseType.HOUSE_HARKONNEN);
					}
					break;

				case (ushort)DrawMode.DRAW_MODE_CUSTOM_PROC:
					{
						if (drawParam.proc == null) return;
						drawParam.proc(w);
					}
					break;

				case (ushort)DrawMode.DRAW_MODE_WIRED_RECTANGLE:
					{
						Gui.GUI_DrawWiredRectangle(positionLeft, positionTop, positionRight, positionBottom, fgColour);
					}
					break;

				case (ushort)DrawMode.DRAW_MODE_XORFILLED_RECTANGLE:
					{
						Gui.GUI_DrawXorFilledRectangle((short)positionLeft, (short)positionTop, (short)positionRight, (short)positionBottom, fgColour);
					}
					break;
			}

			if (drawMode != (ushort)DrawMode.DRAW_MODE_NONE && drawMode != (ushort)DrawMode.DRAW_MODE_CUSTOM_PROC && Gfx.GFX_Screen_IsActive(Screen.SCREEN_0))
			{
				Gui.GUI_Mouse_Show_InRegion();
			}
		}

		/*
		 * Draw a chess-pattern filled rectangle over the widget.
		 *
		 * @param w The widget to draw.
		 * @param colour The colour of the chess pattern.
		 */
		static void GUI_Widget_DrawBlocked(Widget w, byte colour)
		{
			if (Gfx.GFX_Screen_IsActive(Screen.SCREEN_0))
			{
				Gui.GUI_Mouse_Hide_InRegion((ushort)w.offsetX, (ushort)w.offsetY, (ushort)(w.offsetX + w.width), (ushort)(w.offsetY + w.height));
			}

			Gui.GUI_DrawSprite(Screen.SCREEN_ACTIVE, w.drawParameterNormal.sprite, w.offsetX, w.offsetY, w.parentID, 0);

			Gui.GUI_DrawBlockedRectangle(w.offsetX, w.offsetY, (short)w.width, (short)w.height, colour);

			if (Gfx.GFX_Screen_IsActive(Screen.SCREEN_0))
			{
				Gui.GUI_Mouse_Show_InRegion();
			}
		}

		/*
		 * Make the Widget selected.
		 *
		 * @param w The widget to make selected.
		 * @param clickProc Wether to execute the widget clickProc.
		 */
		internal static void GUI_Widget_MakeSelected(Widget w, bool clickProc)
		{
			if (w == null || w.flags.invisible) return;

			w.state.selectedLast = w.state.selected;

			w.state.selected = true;

			GUI_Widget_Draw(w);

			if (!clickProc || w.clickProc == null) return;

			w.clickProc(w);
		}

		/* This is for a US AT keyboard layout */
		static byte[] shortcuts = {
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, /*  0 -  7 */
			0x0f, 0x10, 0x00, 0x00, 0x00, 0x2b, 0x00, 0x00, /*  8 - 15 : Backspace, Tab, return */
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, /* 16 - 23 */
			0x00, 0x00, 0x00, 0x6e, 0x00, 0x00, 0x00, 0x00, /* 24 - 31 : ESCAPE */
			0x3d, 0x02, 0x29, 0x04, 0x05, 0x06, 0x08, 0x29, /* 32 - 39 : SPACE !1 '" #3 $4 %5 &7 '" */
			0x0a, 0x0b, 0x64, 0x6a, 0x35, 0x0c, 0x36, 0x5f, /* 40 - 47 : (9 )0 KP* KP+ ,< -_ . / */
			0x0b, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, /* 48 - 55 : 0 1 2 3 4 5 6 7 */
			0x09, 0x0a, 0x28, 0x28, 0x35, 0x0d, 0x36, 0x41, /* 56 - 63 : 8 9 :; :; <, =+ >. ?/ */
			0x03, 0x1f, 0x32, 0x30, 0x21, 0x13, 0x22, 0x23, /* 64 - 71 : @2 A B C D E F G */
			0x24, 0x18, 0x25, 0x26, 0x27, 0x34, 0x33, 0x19, /* 72 - 79 : H I J K L M N O */
			0x1a, 0x11, 0x14, 0x20, 0x15, 0x17, 0x31, 0x12, /* 80 - 87 : P Q R S T U V W */
			0x2f, 0x16, 0x2e, 0x1b, 0x1d, 0x1c, 0x07, 0x0c, /* 88 - 95 : X Y Z [ \ ] ^6 _- */
			0x01, 0x1f, 0x32, 0x30, 0x21, 0x13, 0x22, 0x23, /* 96 -103 : ` a b c d e f g */
			0x24, 0x18, 0x25, 0x26, 0x27, 0x34, 0x33, 0x19, /*104 -111 : h i j k l m n o */
			0x1a, 0x11, 0x14, 0x20, 0x15, 0x17, 0x31, 0x12, /*112 -119 : p q r s t u v w */
			0x2f, 0x16, 0x2e, 0x1b, 0x1d, 0x1c, 0x01, 0x00, /*120 -127 : x y z { | } ~ */
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, /*128 -135 */
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x79, /*136 -143 : F10 */
			0x78, 0x77, 0x76, 0x75, 0x74, 0x73, 0x72, 0x71, /*144 -151 : F9 F8 F7 F6 F5 F4 F3 F2 */
			0x70, 0x79, 0x78, 0x77, 0x76, 0x75, 0x74, 0x73, /*152 -159 : F1 F10 F9 F8 F7 F6 F5 F4 */
			0x72, 0x71, 0x70, 0x79, 0x78, 0x77, 0x76, 0x75, /*160 -167 : F3 F2 F1 F10 F9 F8 F7 F6 */
			0x74, 0x73, 0x72, 0x71, 0x70, 0x4c, 0x4b, 0x56, /*168 -175 : F5 F4 F3 F2 F1 DELETE INSERT RIGHT */
			0x54, 0x51, 0x00, 0x59, 0x61, 0x4f, 0x00, 0x55, /*176 -183 : DOWN END  KP/ KP5 END  PGUP */
			0x53, 0x50, 0x00, 0x00, 0x79, 0x78, 0x77, 0x76, /*184 -191 : UP HOME   F10 F9 F8 F7 */
			0x75, 0x74, 0x73, 0x72, 0x71, 0x70, 0x41, 0x42, /*192 -199 : F6 F5 F4 F3 F2 F1 LEFT_MOUSEB RIGHT_MOUSEB */
			0x43, 0x44, 0x45, 0x46, 0x47, 0x48              /*200 -205 : ??? ??? ??? ??? ??? ??? */
		};
		/*
		 * Get shortcut key for the given char.
		 *
		 * @param c The (ASCII) char to get the shortcut for.
		 * @return The shortcut key. (Dune II key code)
		 */
		internal static byte GUI_Widget_GetShortcut(byte c)
		{
			if (c < shortcuts.Length/*sizeof(shortcuts)*/) return shortcuts[c];
			else return 0;
		}

		/*
		 * Allocates a widget.
		 *
		 * @param index The index for the allocated widget.
		 * @param shortcut The shortcut for the allocated widget.
		 * @param offsetX The x position for the allocated widget.
		 * @param offsetY The y position for the allocated widget.
		 * @param spriteID The sprite to draw on the allocated widget (0xFFFF for none).
		 * @param stringID The string to print on the allocated widget.
		 * @return The allocated widget.
		 */
		internal static Widget GUI_Widget_Allocate(ushort index, ushort shortcut, ushort offsetX, ushort offsetY, ushort spriteID, ushort stringID)
		{
			Widget w;
			byte drawMode;
			WidgetDrawParameter drawParam1 = new WidgetDrawParameter();
			WidgetDrawParameter drawParam2 = new WidgetDrawParameter();

			w = new Widget(); //(Widget *)calloc(1, sizeof(Widget));
			w.index = index;
			w.shortcut = shortcut;
			w.shortcut2 = shortcut;
			w.parentID = 0;
			w.fgColourSelected = 0xB;
			w.bgColourSelected = 0xC;
			w.fgColourNormal = 0xF;
			w.bgColourNormal = 0xC;
			w.stringID = stringID;
			w.offsetX = (short)offsetX;
			w.offsetY = (short)offsetY;

			w.flags.requiresClick = true;
			w.flags.clickAsHover = true;
			w.flags.loseSelect = true;
			w.flags.buttonFilterLeft = 4;
			w.flags.buttonFilterRight = 4;

			switch ((short)spriteID + 4)
			{
				case 0:
					drawMode = (byte)DrawMode.DRAW_MODE_CUSTOM_PROC;
					drawParam1.proc = WidgetDraw.GUI_Widget_SpriteButton_Draw;
					drawParam2.proc = WidgetDraw.GUI_Widget_SpriteButton_Draw;
					break;

				case 1:
					drawMode = (byte)DrawMode.DRAW_MODE_CUSTOM_PROC;
					drawParam1.proc = WidgetDraw.GUI_Widget_SpriteTextButton_Draw;
					drawParam2.proc = WidgetDraw.GUI_Widget_SpriteTextButton_Draw;

					if (stringID == (ushort)Text.STR_NULL) break;

					if (CString.String_Get_ByIndex(stringID) != null) w.shortcut = GUI_Widget_GetShortcut((byte)CString.String_Get_ByIndex(stringID)[0]);
					if (stringID == (ushort)Text.STR_CANCEL) w.shortcut2 = 'n';
					break;

				case 2:
					drawMode = (byte)DrawMode.DRAW_MODE_CUSTOM_PROC;
					drawParam1.proc = WidgetDraw.GUI_Widget_TextButton2_Draw;
					drawParam2.proc = WidgetDraw.GUI_Widget_TextButton2_Draw;
					break;

				case 3:
					drawMode = (byte)DrawMode.DRAW_MODE_NONE;
					drawParam1.spriteID = 0;
					drawParam2.spriteID = 0;
					break;

				default:
					drawMode = (byte)DrawMode.DRAW_MODE_SPRITE;
					drawParam1.sprite = Sprites.g_sprites[spriteID];
					drawParam2.sprite = Sprites.g_sprites[spriteID + 1];

					if (drawParam1.sprite == null) break;

					w.width = Sprites.Sprite_GetWidth(drawParam1.sprite);
					w.height = Sprites.Sprite_GetHeight(drawParam1.sprite);
					break;
			}

			w.drawModeSelected = drawMode;
			w.drawModeDown = drawMode;
			w.drawModeNormal = drawMode;
			w.drawParameterNormal = drawParam1;
			w.drawParameterDown = drawParam2;
			w.drawParameterSelected = (spriteID == 0x19) ? drawParam2 : drawParam1;

			return w;
		}

		/*
		 * Insert a widget into a list of widgets.
		 * @param w1 Widget to which the other widget is added.
		 * @param w2 Widget which is added to the first widget (ordered by index).
		 * @return The first widget of the chain.
		 */
		internal static Widget GUI_Widget_Insert(Widget w1, Widget w2)
		{
			Widget first;
			Widget prev;

			if (w1 == null) return w2;
			if (w2 == null) return w1;

			if (w2.index <= w1.index)
			{
				w2.next = w1;
				return w2;
			}

			first = w1;
			prev = w1;

			while (w2.index > w1.index && w1.next != null)
			{
				prev = w1;
				w1 = w1.next;
			}

			if (w2.index > w1.index)
			{
				w1 = GUI_Widget_Link(first, w2);
			}
			else
			{
				prev.next = w2;
				w2.next = w1;
			}

			s_widgetReset = true;

			return first;
		}

		static Widget l_widget_selected = null;
		static Widget l_widget_last = null;
		static ushort l_widget_button_state = 0x0;
		/*
		 * Check a widget for events like 'hover' or 'click'. Also check the keyboard
		 *  buffer if there was any key which should active us.
		 *
		 * @param w The widget to handle events for. If the widget has a valid next
		 *   pointer, those widgets are handled too.
		  * @return The last key pressed, or 0 if the key pressed was handled (or if
		 *   there was no key press).
		 */
		internal static ushort GUI_Widget_HandleEvents(Widget w)
		{
			ushort mouseX, mouseY;
			ushort buttonState;
			ushort returnValue;
			ushort key;
			bool fakeClick;

			/* Get the key from the buffer, if there was any key pressed */
			key = 0;
			if (Input.Input_IsInputAvailable() != 0)
			{
				key = Input.Input_Wait();
			}

			if (w == null) return (ushort)(key & 0x7FFF);

			/* First time this window is being drawn? */
			if (w != l_widget_last || s_widgetReset)
			{
				l_widget_last = w;
				l_widget_selected = null;
				l_widget_button_state = 0x0;
				s_widgetReset = false;

				/* Check for left click */
				if (Input.Input_Test(0x41) != 0) l_widget_button_state |= 0x0200;

				/* Check for right click */
				if (Input.Input_Test(0x42) != 0) l_widget_button_state |= 0x2000;

				/* Draw all the widgets */
				for (; w != null; w = GUI_Widget_GetNext(w))
				{
					GUI_Widget_Draw(w);
				}
			}

			mouseX = Mouse.g_mouseX;
			mouseY = Mouse.g_mouseY;

			buttonState = 0;
			if (Mouse.g_mouseDisabled == 0)
			{
				ushort buttonStateChange = 0;

				/* See if the key was a mouse button action */
				if ((key & 0x8000) != 0)
				{
					if ((key & 0x00FF) == 0xC7) buttonStateChange = 0x1000;
					if ((key & 0x00FF) == 0xC6) buttonStateChange = 0x0100;
				}
				else
				{
					if ((key & 0x00FF) == 0x42) buttonStateChange = 0x1000;
					if ((key & 0x00FF) == 0x41) buttonStateChange = 0x0100;
				}

				/* Mouse button up */
				if ((key & 0x0800) != 0)
				{
					buttonStateChange <<= 2;
				}

				if (buttonStateChange != 0)
				{
					mouseX = Mouse.g_mouseClickX;
					mouseY = Mouse.g_mouseClickY;
				}

				/* Disable when release, enable when click */
				l_widget_button_state &= (ushort)(~((buttonStateChange & 0x4400) >> 1));
				l_widget_button_state |= (ushort)((buttonStateChange & 0x1100) << 1);

				buttonState |= buttonStateChange;
				buttonState |= l_widget_button_state;
				buttonState |= (ushort)((l_widget_button_state << 2) ^ 0x8800);
			}

			w = l_widget_last;
			if (l_widget_selected != null)
			{
				w = l_widget_selected;

				if (w.flags.invisible)
				{
					l_widget_selected = null;
				}
			}

			returnValue = 0;
			for (; w != null; w = GUI_Widget_GetNext(w))
			{
				ushort positionX, positionY;
				bool triggerWidgetHover;
				bool widgetHover;
				bool widgetClick;

				if (w.flags.invisible) continue;

				/* Store the previous button state */
				w.state.selectedLast = w.state.selected;
				w.state.hover1Last = w.state.hover1;

				positionX = (ushort)w.offsetX;
				if (w.offsetX < 0) positionX += (ushort)(g_widgetProperties[w.parentID].width << 3);
				positionX += (ushort)(g_widgetProperties[w.parentID].xBase << 3);

				positionY = (ushort)w.offsetY;
				if (w.offsetY < 0) positionY += g_widgetProperties[w.parentID].height;
				positionY += g_widgetProperties[w.parentID].yBase;

				widgetHover = false;
				w.state.keySelected = false;

				/* Check if the mouse is inside the widget */
				if (positionX <= mouseX && mouseX <= positionX + w.width && positionY <= mouseY && mouseY <= positionY + w.height)
				{
					widgetHover = true;
				}

				/* Check if there was a keypress for the widget */
				if ((key & 0x7F) != 0 && ((key & 0x7F) == w.shortcut || (key & 0x7F) == w.shortcut2))
				{
					widgetHover = true;
					w.state.keySelected = true;
					key = 0;

					buttonState = 0;
					if ((key & 0x7F) == w.shortcut2) buttonState = (ushort)((w.flags.buttonFilterRight) << 12);
					if (buttonState == 0) buttonState = (ushort)((w.flags.buttonFilterLeft) << 8);

					l_widget_selected = w;
				}

				/* Update the hover state */
				w.state.hover1 = false;
				w.state.hover2 = false;
				if (widgetHover)
				{
					/* Button pressed, and click is hover */
					if ((buttonState & 0x3300) != 0 && w.flags.clickAsHover && (w == l_widget_selected || l_widget_selected == null))
					{
						w.state.hover1 = true;
						w.state.hover2 = true;

						/* If we don't have a selected widget yet, this will be the one */
						if (l_widget_selected == null)
						{
							l_widget_selected = w;
						}
					}
					/* No button pressed, and click not is hover */
					if ((buttonState & 0x8800) != 0 && !w.flags.clickAsHover)
					{
						w.state.hover1 = true;
						w.state.hover2 = true;
					}
				}

				/* Check if we should trigger the hover activation */
				triggerWidgetHover = widgetHover;
				if (l_widget_selected != null && l_widget_selected.flags.loseSelect)
				{
					triggerWidgetHover = (l_widget_selected == w) ? true : false;
				}

				widgetClick = false;
				if (triggerWidgetHover)
				{
					byte buttonLeftFiltered;
					byte buttonRightFiltered;

					/* We click this widget for the first time */
					if ((buttonState & 0x1100) != 0 && l_widget_selected == null)
					{
						l_widget_selected = w;
						key = 0;
					}

					buttonLeftFiltered = (byte)((buttonState >> 8) & w.flags.buttonFilterLeft);
					buttonRightFiltered = (byte)((buttonState >> 12) & w.flags.buttonFilterRight);

					/* Check if we want to consider this as click */
					if ((buttonLeftFiltered != 0 || buttonRightFiltered != 0) && (widgetHover || !w.flags.requiresClick))
					{

						if (Convert.ToBoolean(buttonLeftFiltered & 1) || Convert.ToBoolean(buttonRightFiltered & 1))
						{
							/* Widget click */
							w.state.selected = !w.state.selected;
							returnValue = (ushort)(w.index | 0x8000);
							widgetClick = true;

							if (w.flags.clickAsHover)
							{
								w.state.hover1 = true;
								w.state.hover2 = true;
							}
							l_widget_selected = w;
						}
						else if (Convert.ToBoolean(buttonLeftFiltered & 2) || Convert.ToBoolean(buttonRightFiltered & 2))
                        {
							/* Widget was already clicked */
							if (!w.flags.clickAsHover)
							{
								w.state.hover1 = true;
								w.state.hover2 = true;
							}
							if (!w.flags.requiresClick) widgetClick = true;
						}
						else if (Convert.ToBoolean(buttonLeftFiltered & 4) || Convert.ToBoolean(buttonRightFiltered & 4))
                        {
							/* Widget release */
							if (!w.flags.requiresClick || (w.flags.requiresClick && w == l_widget_selected))
							{
								w.state.selected = !w.state.selected;
								returnValue = (ushort)(w.index | 0x8000);
								widgetClick = true;
							}

							if (!w.flags.clickAsHover)
							{
								w.state.hover1 = false;
								w.state.hover2 = false;
							}
						}
						else
						{
							/* Widget was already released */
							if (w.flags.clickAsHover)
							{
								w.state.hover1 = true;
								w.state.hover2 = true;
							}
							if (!w.flags.requiresClick) widgetClick = true;
						}
					}
				}

				fakeClick = false;
				/* Check if we are hovering and have mouse button down */
				if (widgetHover && (buttonState & 0x2200) != 0)
				{
					w.state.hover1 = true;
					w.state.hover2 = true;

					if (!w.flags.clickAsHover && !w.state.selected)
					{
						fakeClick = true;
						w.state.selected = true;
					}
				}

				/* Check if we are not pressing a button */
				if ((buttonState & 0x8800) == 0x8800)
				{
					l_widget_selected = null;

					if (!widgetHover || w.flags.clickAsHover)
					{
						w.state.hover1 = false;
						w.state.hover2 = false;
					}
				}

				if (!widgetHover && l_widget_selected == w && !w.flags.loseSelect)
				{
					l_widget_selected = null;
				}

				/* When the state changed, redraw */
				if (w.state.selected != w.state.selectedLast || w.state.hover1 != w.state.hover1Last)
				{
					GUI_Widget_Draw(w);
				}

				/* Reset click state when we were faking it */
				if (fakeClick)
				{
					w.state.selected = false;
				}

				if (widgetClick)
				{
					w.state.buttonState = (ulong)(buttonState >> 8);

					/* If Click was successful, don't handle any other widgets */
					if (w.clickProc != null && w.clickProc(w)) break;

					/* On click, don't handle any other widgets */
					if (w.flags.noClickCascade) break;
				}

				/* If we are selected and we lose selection on leave, don't try other widgets */
				if (w == l_widget_selected && w.flags.loseSelect) break;
			}

			if (returnValue != 0) return returnValue;
			return (ushort)(key & 0x7FFF);
		}

		/*
		 * Link a widget to another widget, where the new widget is linked at the end
		 *  of the list of the first widget.
		 * @param w1 Widget to which the other widget is added.
		 * @param w2 Widget which is added to the first widget (at the end of his chain).
		 * @return The first widget of the chain.
		 */
		internal static Widget GUI_Widget_Link(Widget w1, Widget w2)
		{
			Widget first = w1;

			s_widgetReset = true;

			if (w2 == null) return w1;
			w2.next = null;
			if (w1 == null) return w2;

			while (w1.next != null) w1 = w1.next;

			w1.next = w2;
			return first;
		}

		/*
		 * Draw the exterior of the currently selected widget.
		 */
		internal static void Widget_PaintCurrentWidget() =>
			Gui.GUI_DrawFilledRectangle((short)(g_curWidgetXBase << 3), (short)g_curWidgetYBase, (short)(((g_curWidgetXBase + g_curWidgetWidth) << 3) - 1), (short)(g_curWidgetYBase + g_curWidgetHeight - 1), g_curWidgetFGColourNormal);

		/*
		 * Select a widget as current widget and draw its exterior.
		 * @param index %Widget number to select.
		 * @return Index of the previous selected widget.
		 */
		internal static ushort Widget_SetAndPaintCurrentWidget(ushort index)
		{
			index = Widget_SetCurrentWidget(index);

			Widget_PaintCurrentWidget();

			return index;
		}

		/*
		 * Allocate a scroll button for the Mentat screen scroll bar.
		 * @return Allocated widget.
		 */
		internal static Widget GUI_Widget_AllocateScrollBtn(ushort index, ushort parentID, ushort offsetX, ushort offsetY, byte[] sprite1, byte[] sprite2, Widget widget2, bool isDown)
		{
			Widget w;

			w = new Widget(); //(Widget*)calloc(1, sizeof(Widget));

			w.index = index;
			w.parentID = parentID;
			w.offsetX = (short)offsetX;
			w.offsetY = (short)offsetY;

			w.drawModeNormal = (byte)DrawMode.DRAW_MODE_SPRITE;
			w.drawModeDown = (byte)DrawMode.DRAW_MODE_SPRITE;
			w.drawModeSelected = (byte)DrawMode.DRAW_MODE_SPRITE;

			w.width = (ushort)(Sprites.Sprite_GetWidth(sprite1) * 8);
			w.height = Sprites.Sprite_GetHeight(sprite1);

			w.flags.requiresClick = true;
			w.flags.clickAsHover = true;
			w.flags.loseSelect = true;
			w.flags.buttonFilterLeft = 1;
			w.flags.buttonFilterRight = 1;

			w.drawParameterNormal.sprite = sprite1;
			w.drawParameterSelected.sprite = sprite1;
			w.drawParameterDown.sprite = sprite2;

			if (isDown)
			{
				w.clickProc = WidgetClick.GUI_Widget_Scrollbar_ArrowDown_Click;
			}
			else
			{
				w.clickProc = WidgetClick.GUI_Widget_Scrollbar_ArrowUp_Click;
			}

			w.data = widget2.data;
			return w;
		}

		/*
		 * Allocate a #Widget and a #WidgetScrollbar.
		 * @param index Index of the new widget.
		 * @param parentID Parent ID of the new widget.
		 * @param offsetX Horizontal offset of the new widget.
		 * @param offsetY Vertical offset of the new widget.
		 * @param width Width of the new widget.
		 * @param height Height of the new widget.
		 * @param drawProc Procedure for drawing.
		 * @return Address of the new widget.
		 */
		internal static Widget GUI_Widget_Allocate_WithScrollbar(ushort index, ushort parentID, ushort offsetX, ushort offsetY, short width, short height, ScrollbarDrawProc drawProc)
		{
			Widget w;
			WidgetScrollbar ws;

			w = new Widget(); //(Widget*)calloc(1, sizeof(Widget));

			w.index = index;
			w.parentID = parentID;
			w.offsetX = (short)offsetX;
			w.offsetY = (short)offsetY;
			w.width = (ushort)width;
			w.height = (ushort)height;

			w.fgColourSelected = 10;
			w.bgColourSelected = 12;

			w.fgColourNormal = 15;
			w.bgColourNormal = 12;

			w.flags.buttonFilterLeft = 7;
			w.flags.loseSelect = true;

			w.state.hover2Last = true;

			w.drawModeNormal = (byte)DrawMode.DRAW_MODE_CUSTOM_PROC;
			w.drawModeSelected = (byte)DrawMode.DRAW_MODE_CUSTOM_PROC;

			w.drawParameterNormal.proc = WidgetDraw.GUI_Widget_Scrollbar_Draw;
			w.drawParameterSelected.proc = WidgetDraw.GUI_Widget_Scrollbar_Draw;
			w.clickProc = WidgetClick.GUI_Widget_Scrollbar_Click;

			ws = new WidgetScrollbar(); //(WidgetScrollbar*)calloc(1, sizeof(WidgetScrollbar));

			w.data = ws;

			ws.parent = w;

			ws.scrollMax = 1;
			ws.scrollPageSize = 1;
			ws.scrollPosition = 0;
			ws.pressed = 0;
			ws.dirty = 0;

			ws.drawProc = drawProc;

			GUI_Widget_Scrollbar_CalculateSize(ws);
			GUI_Widget_Scrollbar_CalculatePosition(ws);

			return w;
		}

		internal static void GUI_Widget_Free_WithScrollbar(Widget w)
		{
			if (w == null) return;

			w.data = null; //free(w->data);
			w = null; //free(w);
		}

		internal static ushort GUI_Widget_Scrollbar_CalculatePosition(WidgetScrollbar scrollbar)
		{
			Widget w;
			ushort position;

			w = scrollbar.parent;
			if (w == null) return 0xFFFF;

			position = (ushort)(scrollbar.scrollMax - scrollbar.scrollPageSize);

			if (position != 0) position = (ushort)(scrollbar.scrollPosition * (Max(w.width, w.height) - 2 - scrollbar.size) / position);

			if (scrollbar.position != position)
			{
				scrollbar.position = position;
				scrollbar.dirty = 1;
			}

			return position;
		}

		internal static ushort GUI_Widget_Scrollbar_CalculateScrollPosition(WidgetScrollbar scrollbar)
		{
			Widget w;

			w = scrollbar.parent;
			if (w == null) return 0xFFFF;

			scrollbar.scrollPosition = (ushort)(scrollbar.position * (scrollbar.scrollMax - scrollbar.scrollPageSize) / (Max(w.width, w.height) - 2 - scrollbar.size));

			return scrollbar.scrollPosition;
		}

		static ushort GUI_Widget_Scrollbar_CalculateSize(WidgetScrollbar scrollbar)
		{
			Widget w;
			ushort size;

			w = scrollbar.parent;

			if (w == null) return 0;

			size = (ushort)(scrollbar.scrollPageSize * (Max(w.width, w.height) - 2) / scrollbar.scrollMax);

			if (scrollbar.size != size)
			{
				scrollbar.size = size;
				scrollbar.dirty = 1;
			}

			return size;
		}

		/*
		 * Get scrollbar position.
		 * @param w Widget.
		 * @return Scrollbar position, or \c 0xFFFF if no widget supplied.
		 */
		internal static ushort GUI_Get_Scrollbar_Position(Widget w)
		{
			WidgetScrollbar ws;

			if (w == null) return 0xFFFF;

			ws = (WidgetScrollbar)w.data;
			return ws.scrollPosition;
		}

		internal static ushort GUI_Widget_Scrollbar_Init(Widget w, short scrollMax, short scrollPageSize, short scrollPosition)
		{
			ushort position;
			WidgetScrollbar scrollbar;

			if (w == null) return 0xFFFF;

			position = GUI_Get_Scrollbar_Position(w);
			scrollbar = (WidgetScrollbar)w.data;

			if (scrollMax > 0) scrollbar.scrollMax = (ushort)scrollMax;
			if (scrollPageSize >= 0) scrollbar.scrollPageSize = (ushort)Min(scrollPageSize, scrollbar.scrollMax);
			if (scrollPosition >= 0) scrollbar.scrollPosition = (ushort)Min(scrollPosition, scrollbar.scrollMax - scrollbar.scrollPageSize);

			GUI_Widget_Scrollbar_CalculateSize(scrollbar);
			GUI_Widget_Scrollbar_CalculatePosition(scrollbar);
			WidgetDraw.GUI_Widget_Scrollbar_Draw(w);

            scrollbar.drawProc?.Invoke(w);

            return position;
		}
	}
}
