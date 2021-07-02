/* SDL 2 video driver */

using SDL2;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace SharpDune
{
    enum VideoScaleFilter
	{
		FILTER_NEAREST_NEIGHBOR = 0,    /*<! Default */
		FILTER_SCALE2X,                 /*<! see http://scale2x.sourceforge.net/ */
		FILTER_HQX                      /*<! see https://code.google.com/p/hqx/ */
	}

	class Sdl2Video
	{
		/* Set DUNE_ICON_DIR at compile time.  e.g. */
		/* #define DUNE_ICON_DIR "/usr/local/share/icons/hicolor/32x32/apps/" */
		const string DUNE_ICON_DIR = @".\Images";

		static VideoScaleFilter s_scale_filter;

		/* The the magnification of the screen. 2 means 640x400, 3 means 960x600, etc. */
		static int s_screen_magnification;

		static bool s_video_initialized = false;
		static bool s_video_lock = false;

		static uint[] s_palette = new uint[256];
		static bool s_screen_needrepaint = false;

		static ushort s_screenOffset = 0;   /* VGA Start Address Register */

		static bool s_full_screen = false;

		static IntPtr s_window;
		static IntPtr s_renderer;
		static IntPtr s_texture;

		static byte[] s_framebuffer;

		static byte s_keyBufferLatest = 0;

		static ushort s_mousePosX = 0;
		static ushort s_mousePosY = 0;
		static bool s_mouseButtonLeft = false;
		static bool s_mouseButtonRight = false;

		static ushort s_mouseMinX = 0;
		static ushort s_mouseMaxX = 0;
		static ushort s_mouseMinY = 0;
		static ushort s_mouseMaxY = 0;

		/* Partly copied from http://webster.cs.ucr.edu/AoA/DOS/pdf/apndxc.pdf */
		static byte[] s_SDL_keymap = {
				   0,    0,    0,    0,    0,    0,    0,    0, 0x0E, 0x0F,    0,    0,    0, 0x1C,    0,    0, /*  0x00 -  0x0F */
				   0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0, 0x01,    0,    0,    0,    0, /*  0x10 -  0x1F */
				0x39,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0, 0x33, 0x0C, 0x34, 0x35, /*  0x20 -  0x2F */
				0x0B, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A,    0,    0,    0, 0x0D,    0,    0, /*  0x30 -  0x3F */
				   0, 0x1E, 0x30, 0x2E, 0x20, 0x12, 0x21, 0x22, 0x23, 0x17, 0x24, 0x25, 0x26, 0x32, 0x31, 0x18, /*  0x40 -  0x4F */
				0x19, 0x10, 0x13, 0x1F, 0x14, 0x16, 0x2F, 0x11, 0x2D, 0x15, 0x2C,    0, 0x2B,    0,    0,    0, /*  0x50 -  0x5F */
				0x29, 0x1E, 0x30, 0x2E, 0x20, 0x12, 0x21, 0x22, 0x23, 0x17, 0x24, 0x25, 0x26, 0x32, 0x31, 0x18, /*  0x60 -  0x6F */
				0x19, 0x10, 0x13, 0x1F, 0x14, 0x16, 0x2F, 0x11, 0x2D, 0x15, 0x2C,    0,    0,    0,    0, 0x53, /*  0x70 -  0x7F */
		};

		/* see https://wiki.libsdl.org/SDLKeycodeLookup */
		static byte[] s_SDL_hikeymap = {
			0x3A,	/* 1073741881 0x40000039 SDLK_CAPSLOCK */
			0x3B,	/* 1073741882 0x4000003A SDLK_F1 */
			0x3C,	/* 1073741883 0x4000003B SDLK_F2 */
			0x3D,	/* 1073741884 0x4000003C SDLK_F3 */
			0x3E,	/* 1073741885 0x4000003D SDLK_F4 */
			0x3F,	/* 1073741886 0x4000003E SDLK_F5 */
			0x40,	/* 1073741887 0x4000003F SDLK_F6 */
			0x41,	/* 1073741888 0x40000040 SDLK_F7 */
			0x42,	/* 1073741889 0x40000041 SDLK_F8 */
			0x43,	/* 1073741890 0x40000042 SDLK_F9 */
			0x44,	/* 1073741891 0x40000043 SDLK_F10 */
			0x57,	/* 1073741892 0x40000044 SDLK_F11 */
			0x58,	/* 1073741893 0x40000045 SDLK_F12 */
			0x00,	/* 1073741894 0x40000046 SDLK_PRINTSCREEN */
			0x00,	/* 1073741895 0x40000047 SDLK_SCROLLLOCK */
			0x00,	/* 1073741896 0x40000048 SDLK_PAUSE */
			0x52,	/* 1073741897 0x40000049 SDLK_INSERT */
			0x00,	/* 1073741898 0x4000004A SDLK_HOME */
			0x49,	/* 1073741899 0x4000004B SDLK_PAGEUP */
			0x00,
			0x4F,	/* 1073741901 0x4000004D SDLK_END */
			0x51,	/* 1073741902 0x4000004E SDLK_PAGEDOWN */
			0x4D,	/* 1073741903 0x4000004F SDLK_RIGHT */
			0x4B,	/* 1073741904 0x40000050 SDLK_LEFT */
			0x50,	/* 1073741905 0x40000051 SDLK_DOWN */
			0x48,	/* 1073741906 0x40000052 SDLK_UP */
			0x00,	/* 1073741907 0x40000053 SDLK_NUMLOCKCLEAR */
			0x00,	/* 1073741908 0x40000054 SDLK_KP_DIVIDE */
			0x37,	/* 1073741909 0x40000055 SDLK_KP_MULTIPLY */
			0x4A,	/* 1073741910 0x40000056 SDLK_KP_MINUS */
			0x4E,	/* 1073741911 0x40000057 SDLK_KP_PLUS */
			0x1C,	/* 1073741912 0x40000058 SDLK_KP_ENTER */
			0x4F,	/* 1073741913 0x40000059 SDLK_KP_1 */
			0x50,	/* 1073741914 0x4000005A SDLK_KP_2 */
			0x51,	/* 1073741915 0x4000005B SDLK_KP_3 */
			0x4B,	/* 1073741916 0x4000005C SDLK_KP_4 */
			0x4C,	/* 1073741917 0x4000005D SDLK_KP_5 */
			0x4D,	/* 1073741918 0x4000005E SDLK_KP_6 */
			0x47,	/* 1073741919 0x4000005F SDLK_KP_7 */
			0x48,	/* 1073741920 0x40000060 SDLK_KP_8 */
			0x49,	/* 1073741921 0x40000061 SDLK_KP_9 */
			0x52,	/* 1073741922 0x40000062 SDLK_KP_0 */
			0x53,	/* 1073741923 0x40000063 SDLK_KP_PERIOD */
		};

		/*
		 * Change the palette with the palette supplied.
		 * @param palette The palette to replace the current with.
		 * @param from From which colour.
		 * @param length The length of the palette (in colours).
		 */
		internal static void Video_SetPalette(/*void*/byte[] palette, int from, int length)
		{
			var p = palette;
			var pPointer = 0;
			int i;

			s_video_lock = true;

			for (i = from; i < from + length; i++)
			{
				s_palette[i] = 0xff000000 /* a */
							 | (uint)((((p[pPointer] & 0x3F) * 0x41) << 12) & 0x00ff0000) /* r */
							 | (uint)((((p[pPointer + 1] & 0x3F) * 0x41) << 4) & 0x0000ff00) /* g */
							 | (uint)(((p[pPointer + 2] & 0x3F) * 0x41) >> 4); /* b */
				pPointer += 3;
			}

			s_screen_needrepaint = true;
			s_video_lock = false;
		}

		/*
		 * change the screen offset, equivalent to changing the
		 * Start Address Register on a VGA card.
		 * VGA Hardware has 4 "maps" of 64kB.
		 * @param offset The address granularity is 4bytes
		 */
		internal static void Video_SetOffset(ushort offset)
		{
			s_screenOffset = offset;
			s_screen_needrepaint = true;
		}

		/*
		 * Set the current position of the mouse.
		 * @param x The new logical X-position of the mouse.
		 * @param y The new logical Y-position of the mouse.
		 */
		internal static void Video_Mouse_SetPosition(ushort x, ushort y)
		{
			SDL.SDL_Rect rect;
			int w, h;
			float scale;

			/*
			 * We receive logical positions but SDL_WarpMouseInWindow expects physical
			 * window positions. We need to guess what SDL_RenderSetLogicalSize did
			 * exactly. Note that the values from SDL_GetRendererOutputSize are in
			 * physical units while SDL_RenderGetViewport are in logical units.
			 */
			SDL.SDL_RenderGetViewport(s_renderer, out rect);

			if (SDL.SDL_GetRendererOutputSize(s_renderer, out w, out h) != 0)
			{
				Trace.WriteLine($"ERROR: SDL_GetRendererOutputSize failed: {SDL.SDL_GetError()}");
				return;
			}

			if (rect.x != 0 && rect.y == 0)
			{
				scale = h / (float)rect.h;
			}
			else if (rect.y != 0 && rect.x == 0)
			{
				scale = w / (float)rect.w;
			}
			else
			{
				/* Guess! */
				scale = 1.0F;
			}

			SDL.SDL_WarpMouseInWindow(s_window, (int)((rect.x + (float)x) * scale), (int)((rect.y + (float)y) * scale));
		}

		internal static byte[] Video_GetFrameBuffer(/*ushort size*/)
		{
			//(void)size;
			return s_framebuffer;
		}

        /*
		 * Uninitialize the video driver.
		 */
        internal static void Video_Uninit()
		{
			s_video_initialized = false;

			//if (s_scale_filter == VideoScaleFilter.FILTER_HQX)
			//{
			//	hqxUnInit();
			//}

			s_framebuffer = null; //free(s_framebuffer);

			if (s_texture != IntPtr.Zero)
			{
				SDL.SDL_DestroyTexture(s_texture);
				s_texture = IntPtr.Zero;
			}

			if (s_renderer != IntPtr.Zero)
			{
				SDL.SDL_DestroyRenderer(s_renderer);
				s_renderer = IntPtr.Zero;
			}

			if (s_window != IntPtr.Zero)
			{
				SDL.SDL_DestroyWindow(s_window);
				s_window = IntPtr.Zero;
			}

			SDL.SDL_Quit();
		}

		static bool s_showFPS = false;
		/*
		 * Runs every tick to handle video driver updates.
		 */
		internal static void Video_Tick()
		{
			SDL.SDL_Event evt;
			var draw = true;

			if (!s_video_initialized) return;
			if (s_video_lock) return;

			s_video_lock = true;

			if (s_showFPS)
			{
				VideoFps.Video_ShowFPS((byte[])Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_0));
			}

			while (SDL.SDL_PollEvent(out evt) == 1)
			{
				byte keyup = 1;

				switch (evt.type)
				{
					case SDL.SDL_EventType.SDL_QUIT:
						{
							s_video_lock = false;
							CSharpDune.PrepareEnd();
							Environment.Exit(0);
						}
						break;

					case SDL.SDL_EventType.SDL_MOUSEMOTION:
						Video_Mouse_Move(evt.motion.x, evt.motion.y);
						break;

					case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
						if (evt.button.button == SDL.SDL_BUTTON_LEFT) Video_Mouse_Button(true, true);
						if (evt.button.button == SDL.SDL_BUTTON_RIGHT) Video_Mouse_Button(false, true);
						break;
					case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
						if (evt.button.button == SDL.SDL_BUTTON_LEFT) Video_Mouse_Button(true, false);
						if (evt.button.button == SDL.SDL_BUTTON_RIGHT) Video_Mouse_Button(false, false);
						break;

					case SDL.SDL_EventType.SDL_KEYDOWN:
					case SDL.SDL_EventType.SDL_KEYUP:
						{
							if (evt.type == SDL.SDL_EventType.SDL_KEYDOWN)
								keyup = 0;

							var sym = evt.key.keysym.sym;
							byte code = 0;
							if ((sym == SDL.SDL_Keycode.SDLK_RETURN && ((evt.key.keysym.mod & SDL.SDL_Keymod.KMOD_ALT) == SDL.SDL_Keymod.KMOD_ALT)) || sym == SDL.SDL_Keycode.SDLK_F11)
							{
								/* ALT-ENTER was pressed */
								if (keyup != 0) continue;   /* ignore key-up */
								if (SDL.SDL_SetWindowFullscreen(s_window, s_full_screen ? 0 : (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) < 0)
								{
									Trace.WriteLine($"WARNING: Failed to toggle full screen : {SDL.SDL_GetError()}");
								}
								s_full_screen = !s_full_screen;
								continue;
							}
							if (sym == SDL.SDL_Keycode.SDLK_F8)
							{
								if (keyup != 0) s_showFPS = !s_showFPS;
								continue;
							}
							if (sym == SDL.SDL_Keycode.SDLK_RSHIFT)
							{
								code = 0x36;
							}
							else if (sym == SDL.SDL_Keycode.SDLK_LSHIFT)
							{
								code = 0x2a;
							}
							else if (sym >= SDL.SDL_Keycode.SDLK_CAPSLOCK)
							{
								sym -= SDL.SDL_Keycode.SDLK_CAPSLOCK;
								if ((int)sym < s_SDL_hikeymap.Length) code = s_SDL_hikeymap[(int)sym];
							}
							else
							{
								if ((int)sym < s_SDL_keymap.Length) code = s_SDL_keymap[(int)sym];
							}
							if (code == 0)
							{
								Trace.WriteLine($"WARNING: Unhandled key scancode={evt.key.keysym.scancode} sym={evt.key.keysym.sym} {SDL.SDL_GetKeyName(evt.key.keysym.sym)}");
								continue;
							}
							Video_Key_Callback((byte)(code | (keyup != 0 ? 0x80 : 0x0)));
						}
						break;

					case SDL.SDL_EventType.SDL_WINDOWEVENT:
						if (evt.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED)
						{
							/* Clear area outside the 4:3 logical screen, if any */
							SDL.SDL_RenderClear(s_renderer);

							var rect = new SDL.SDL_Rect();
							if (SDL.SDL_RenderCopy(s_renderer, s_texture, ref rect, ref rect) != 0)
							{
								Trace.WriteLine($"ERROR: SDL_RenderCopy failed : {SDL.SDL_GetError()}");
							}

							SDL.SDL_RenderPresent(s_renderer);
							draw = false;
						}
						break;
				}
			}

			if (draw)
			{
				Video_DrawScreen();
			}

			s_video_lock = false;
		}

		/*
		 * Initialize the video driver.
		 */
		internal static bool Video_Init(int screen_magnification, VideoScaleFilter filter)
		{
			int err;
			int render_width;
			int render_height;
#if !WITHOUT_SDLIMAGE
			/*SDL.SDL_Surface*/
			IntPtr icon;
#endif //WITHOUT_SDLIMAGE
			uint window_flags = 0;

			if (s_video_initialized) return true;
			if (screen_magnification <= 0 || screen_magnification > 4)
			{
				Trace.WriteLine($"ERROR: Incorrect screen magnification factor : {screen_magnification}");
				return false;
			}
			s_scale_filter = filter;
			s_screen_magnification = screen_magnification;
			//if (filter == VideoScaleFilter.FILTER_HQX)
			//{
			//	hqxInit();
			//}

			err = SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

			if (err != 0)
			{
				Trace.WriteLine($"ERROR: Could not initialize SDL: {SDL.SDL_GetError()}");
				return false;
			}

			if (IniFile.IniFile_GetInteger("fullscreen", 0) != 0)
			{
				window_flags |= (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
				s_full_screen = true;
			}

			err = SDL.SDL_CreateWindowAndRenderer(
					Gfx.SCREEN_WIDTH * s_screen_magnification,
					Gfx.SCREEN_HEIGHT * s_screen_magnification,
					(SDL.SDL_WindowFlags)window_flags,
					out s_window,
					out s_renderer);

			if (err != 0)
			{
				Trace.WriteLine($"ERROR: Could not set resolution: {SDL.SDL_GetError()}");
				return false;
			}

			SDL.SDL_SetWindowTitle(s_window, CSharpDune.window_caption);

#if !WITHOUT_SDLIMAGE
			icon = SDL_image.IMG_Load(Path.Combine(DUNE_ICON_DIR, "opendune_32x32.png")); //"opendune.png"
			//if (icon == IntPtr.Zero) icon = SDL_image.IMG_Load(Path.Combine(DUNE_ICON_DIR, "opendune.ico")); //opendune_32x32.png
			if (icon != IntPtr.Zero)
			{
				SDL.SDL_SetWindowIcon(s_window, icon);
				SDL.SDL_FreeSurface(icon);
			}
#endif //WITHOUT_SDLIMAGE

			switch (s_scale_filter)
			{
				case VideoScaleFilter.FILTER_NEAREST_NEIGHBOR:
					/* SDL2 take care of Nearest neighbor rescaling */
					render_width = Gfx.SCREEN_WIDTH;
					render_height = Gfx.SCREEN_HEIGHT;
					break;
				case VideoScaleFilter.FILTER_SCALE2X:
				case VideoScaleFilter.FILTER_HQX:
				default:
					render_width = Gfx.SCREEN_WIDTH * s_screen_magnification;
					render_height = Gfx.SCREEN_HEIGHT * s_screen_magnification;
					break;
			}
            s_framebuffer = new byte[Gfx.SCREEN_WIDTH * (Gfx.SCREEN_HEIGHT + 4)]; //calloc(1, SCREEN_WIDTH * (SCREEN_HEIGHT + 4) * sizeof(uint8));
            if (s_framebuffer == null)
            {
				Trace.WriteLine($"ERROR: Could not allocate {Gfx.SCREEN_WIDTH * (Gfx.SCREEN_HEIGHT + 4)} bytes of memory");
                return false;
            }
            err = SDL.SDL_RenderSetLogicalSize(s_renderer, render_width, render_height);

			if (err != 0)
			{
				Trace.WriteLine($"ERROR: Could not set logical size: {SDL.SDL_GetError()}");
				return false;
			}

			s_texture = SDL.SDL_CreateTexture(s_renderer,
					SDL.SDL_PIXELFORMAT_ARGB8888,
					(int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
					render_width, render_height);

			if (s_texture == IntPtr.Zero)
			{
				Trace.WriteLine($"ERROR: Could not create texture: {SDL.SDL_GetError()}");
				return false;
			}

			SDL.SDL_ShowCursor(SDL.SDL_DISABLE);

			/* Setup SDL_RenderClear */
			SDL.SDL_SetRenderDrawColor(s_renderer, 0, 0, 0, 255);

			Video_Mouse_SetRegion(0, Gfx.SCREEN_WIDTH, 0, Gfx.SCREEN_HEIGHT);

			s_video_initialized = true;
			return true;
		}

		/*
		 * Callback wrapper for mouse actions.
		 */
		static void Video_Mouse_Callback()
		{
			if (s_scale_filter == VideoScaleFilter.FILTER_NEAREST_NEIGHBOR)
			{
				Mouse.Mouse_EventHandler(s_mousePosX, s_mousePosY, s_mouseButtonLeft, s_mouseButtonRight);
			}
			else
			{
				Mouse.Mouse_EventHandler((ushort)(s_mousePosX / s_screen_magnification), (ushort)(s_mousePosY / s_screen_magnification), s_mouseButtonLeft, s_mouseButtonRight);
			}
		}

		/*
		 * Handle the clicking of a mouse button.
		 * @param left True if the left button, otherwise the right button.
		 * @param down True if the button is down, otherwise it is up.
		 */
		static void Video_Mouse_Button(bool left, bool down)
		{
			if (left)
			{
				s_mouseButtonLeft = down;
			}
			else
			{
				s_mouseButtonRight = down;
			}

			Video_Mouse_Callback();
		}

		/*
		 * Set the region in which the mouse is allowed to move, or 0 for no limitation.
		 * @param minX The minimal X-position.
		 * @param maxX The maximal X-position.
		 * @param minY The minimal Y-position.
		 * @param maxY The maximal Y-position.
		 */
		internal static void Video_Mouse_SetRegion(ushort minX, ushort maxX, ushort minY, ushort maxY)
		{
			if (s_scale_filter == VideoScaleFilter.FILTER_NEAREST_NEIGHBOR)
			{
				s_mouseMinX = minX;
				s_mouseMaxX = maxX;
				s_mouseMinY = minY;
				s_mouseMaxY = maxY;
			}
			else
			{
				s_mouseMinX = (ushort)(minX * s_screen_magnification);
				s_mouseMaxX = (ushort)(maxX * s_screen_magnification);
				s_mouseMinY = (ushort)(minY * s_screen_magnification);
				s_mouseMaxY = (ushort)(maxY * s_screen_magnification);
			}
		}

		/*
		 * Ensure that "val" falls within "min" and "max". If not, it will be clipped
		 * to one of these boundaries.
		 * @param val Value to be champed.
		 * @param min Minimum allowed value.
		 * @param max Maximum allowed value.
		 */
		static ushort Video_Mouse_Clamp(int val, ushort min, ushort max)
		{
			if (val < 0) return 0;
			if (val < min) return min;
			if (val > max) return max;
			return (ushort)val;
		}

		/*
		 * Handle the moving of the mouse. Note: These positions can be negative when
		 * the cursor is out of the logical screen.
		 * @param x The new logical X-position of the mouse.
		 * @param y The new logical Y-position of the mouse.
		 */
		static void Video_Mouse_Move(int x, int y)
		{
			ushort rx, ry;

			rx = Video_Mouse_Clamp(x, s_mouseMinX, s_mouseMaxX);
			ry = Video_Mouse_Clamp(y, s_mouseMinY, s_mouseMaxY);

			/* If we moved, send the signal back to the window to correct for it */
			if (x != rx || y != ry)
			{
				Video_Mouse_SetPosition(rx, ry);
				return;
			}

			s_mousePosX = rx;
			s_mousePosY = ry;

			Video_Mouse_Callback();
		}

		/*
		 * Callback wrapper for key actions.
		 */
		static void Video_Key_Callback(byte key)
		{
			s_keyBufferLatest = key;
			Input.Input_EventHandler(key);
		}

		static void Video_DrawScreen()
		{
			if (!Gfx.GFX_Screen_IsDirty(Screen.SCREEN_0) && !s_screen_needrepaint) return;

			if (s_screen_magnification == 1)
			{
				Video_DrawScreen_Nearest_Neighbor();
			}
			else switch (s_scale_filter)
				{
					case VideoScaleFilter.FILTER_NEAREST_NEIGHBOR:
						Video_DrawScreen_Nearest_Neighbor();
						break;
					case VideoScaleFilter.FILTER_SCALE2X:
						//TODO
						//Video_DrawScreen_Scale2x();
						break;
					case VideoScaleFilter.FILTER_HQX:
						//TODO
						//Video_DrawScreen_Hqx();
						break;
					default:
						Trace.WriteLine("ERROR: Unsupported scale filter");
						break;
				}
			SDL.SDL_RenderPresent(s_renderer);
			Gfx.GFX_Screen_SetClean(Screen.SCREEN_0);
			s_screen_needrepaint = false;
		}

		/*
		 * This function copies the 320x200 buffer to the real screen.
		 * Scaling is done automatically.
		 */
		static void Video_DrawScreen_Nearest_Neighbor()
		{
			var gfx_screen8 = (byte[])Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_0);
			var area = Gfx.GFX_Screen_GetDirtyArea(Screen.SCREEN_0);
			IntPtr pixels;
			int pitch;
			int x, y;
			uint[] p;
			SDL.SDL_Rect rect;
			var prect = new SDL.SDL_Rect { w = Gfx.SCREEN_WIDTH, h = Gfx.SCREEN_HEIGHT }; //SDL_Rect* prect = NULL;
			var gfx_screen8Pointer = 0;
			var pPointer = 0;

			gfx_screen8Pointer += s_screenOffset << 2;
			if (SDL.SDL_LockTexture(s_texture, IntPtr.Zero, out pixels, out pitch) != 0)
			{
				Trace.WriteLine($"ERROR: Could not set lock texture: {SDL.SDL_GetError()}");
				return;
			}

			if (!s_screen_needrepaint && area != null && (area.left > 0 || area.top > 0 || area.right < Gfx.SCREEN_WIDTH || area.bottom < Gfx.SCREEN_HEIGHT))
			{
				rect.x = area.left;
				rect.y = area.top;
				rect.w = area.right - area.left;
				rect.h = area.bottom - area.top;
				prect = rect;
				pixels = IntPtr.Add(pixels, pitch * area.top);
				gfx_screen8Pointer += Gfx.SCREEN_WIDTH * area.top + area.left;
				p = new uint[area.right];
				for (y = area.top; y < area.bottom; y++)
				{
					pPointer += area.left;
					for (x = area.left; x < area.right; x++)
					{
						p[pPointer++] = s_palette[gfx_screen8[gfx_screen8Pointer++]];
					}
					gfx_screen8Pointer += Gfx.SCREEN_WIDTH - rect.w;

					unsafe { fixed (uint* first = p) Unsafe.CopyBlock(pixels.ToPointer(), first, (uint)p.Length * 4); }

					pixels = IntPtr.Add(pixels, pitch);
					pPointer = 0;
				}
				//Debug.WriteLine($"DEBUG: Dirty area : ({area.left}, {area.top}) - ({area.right}, {area.bottom})");
			}
			else
			{
				p = new uint[Gfx.SCREEN_WIDTH];
				for (y = 0; y < Gfx.SCREEN_HEIGHT; y++)
				{
					for (x = 0; x < Gfx.SCREEN_WIDTH; x++)
					{
						p[pPointer++] = s_palette[gfx_screen8[gfx_screen8Pointer++]];
					}

					unsafe { fixed (uint* first = p) Unsafe.CopyBlock(pixels.ToPointer(), first, (uint)p.Length * 4); }

					pixels = IntPtr.Add(pixels, pitch);
					pPointer = 0;
				}
			}
			SDL.SDL_UnlockTexture(s_texture);
			if (SDL.SDL_RenderCopy(s_renderer, s_texture, ref prect, ref prect) != 0)
			{
				Trace.WriteLine($"ERROR: SDL_RenderCopy failed : {SDL.SDL_GetError()}");
			}
		}

		//static uint[] truecolorbuffer = new uint[Gfx.SCREEN_WIDTH * Gfx.SCREEN_HEIGHT];
		//static void Video_DrawScreen_Scale2x()
		//{
		//	byte[] data = (byte[])Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_0);
		//	dirty_area area = Gfx.GFX_Screen_GetDirtyArea(Screen.SCREEN_0);
		//	ushort top, bottom;
		//	/*byte[]*/IntPtr pixels;
		//	int pitch;
		//	uint[] p;
		//	SDL.SDL_Rect rect, rectlock;
		//	SDL.SDL_Rect? prect = null;
		//	SDL.SDL_Rect? prectlock = null;
		//	int pPointer = 0;
		//	int dataPointer = 0;

		//	data = data[(s_screenOffset << 2)..];

		//	/* first do 8bit => 32bit pixel conversion */
		//	if (!s_screen_needrepaint && area != null && (area.left > 0 || area.top > 0 || area.right < Gfx.SCREEN_WIDTH || area.bottom < Gfx.SCREEN_HEIGHT))
		//	{
		//		int x, y;
		//		rect.x = area.left * s_screen_magnification;
		//		rect.y = area.top * s_screen_magnification;
		//		rect.w = (area.right - area.left) * s_screen_magnification;
		//		rect.h = (area.bottom - area.top) * s_screen_magnification;
		//		prect = rect;
		//		rectlock.x = 0;
		//		rectlock.y = rect.y;
		//		rectlock.w = Gfx.SCREEN_WIDTH * s_screen_magnification;
		//		rectlock.h = rect.h;
		//		prectlock = rectlock;
		//		top = area.top;
		//		bottom = area.bottom;
		//		p = truecolorbuffer[(Gfx.SCREEN_WIDTH * area.top + area.left)..];
		//		data = data[(Gfx.SCREEN_WIDTH * area.top + area.left)..];
		//		for (y = area.top; y < area.bottom; y++)
		//		{
		//			for (x = area.left; x < area.right; x++)
		//			{
		//				p[pPointer++] = s_palette[data[dataPointer++]];
		//			}
		//			dataPointer += Gfx.SCREEN_WIDTH - area.right + area.left;
		//			pPointer += Gfx.SCREEN_WIDTH - area.right + area.left;
		//		}
		//		Debug.WriteLine($"DEBUG: Dirty area : ({area.left}, {area.top}) - ({area.right}, {area.bottom})");
		//	}
		//	else
		//	{
		//		int i;
		//		p = truecolorbuffer;
		//		for (i = 0; i < Gfx.SCREEN_WIDTH * Gfx.SCREEN_HEIGHT; i++)
		//		{
		//			p[pPointer++] = s_palette[data[dataPointer++]];
		//		}
		//		top = 0;
		//		bottom = Gfx.SCREEN_HEIGHT;

		//		rectlock.y = 0;
		//	}
		//	/* then call scale2x */
		//	var prectlockValue = prectlock.Value;
		//	if (SDL.SDL_LockTexture(s_texture, ref prectlockValue, out pixels, out pitch) != 0)
		//	{
		//		Trace.WriteLine($"ERROR: Could not set lock texture: {SDL.SDL_GetError()}");
		//		return;
		//	}
		//	if (prectlock != null)
		//	{
		//		pixels -= rectlock.y * pitch;
		//	}
		//	ScaleBit.scale_part((ushort)s_screen_magnification, pixels, (ushort)pitch, truecolorbuffer, Gfx.SCREEN_WIDTH * 4, 4, Gfx.SCREEN_WIDTH, Gfx.SCREEN_HEIGHT, top, bottom);
		//	SDL.SDL_UnlockTexture(s_texture);
		//	var prectValue = prect.Value;
		//	if (SDL.SDL_RenderCopy(s_renderer, s_texture, ref prectValue, ref prectValue) != 0)
		//	{
		//		Trace.WriteLine($"ERROR: SDL_RenderCopy failed : {SDL.SDL_GetError()}");
		//	}
		//}

		//static void Video_DrawScreen_Hqx()
		//{
		//	byte[] src;
		//	/*uint[]*/IntPtr pixels;
		//	int pitch;

		//	src = (byte[])Gfx.GFX_Screen_Get_ByIndex(Screen.SCREEN_0);
		//	src = src[(s_screenOffset << 2)..];

		//	if (SDL.SDL_LockTexture(s_texture, IntPtr.Zero, out pixels, out pitch) != 0)
		//	{
		//		Trace.WriteLine($"ERROR: Could not set lock texture: {SDL.SDL_GetError()}");
		//		return;
		//	}
		//	switch (s_screen_magnification)
		//	{
		//		case 2:
		//			hq2x_8to32_rb(src, Gfx.SCREEN_WIDTH,
		//						  pixels, pitch,
		//						  Gfx.SCREEN_WIDTH, Gfx.SCREEN_HEIGHT, s_palette);
		//			break;
		//		case 3:
		//			hq3x_8to32_rb(src, Gfx.SCREEN_WIDTH,
		//						  pixels, pitch,
		//						  Gfx.SCREEN_WIDTH, Gfx.SCREEN_HEIGHT, s_palette);
		//			break;
		//		case 4:
		//			hq4x_8to32_rb(src, Gfx.SCREEN_WIDTH,
		//						  pixels, pitch,
		//						  Gfx.SCREEN_WIDTH, Gfx.SCREEN_HEIGHT, s_palette);
		//			break;
		//	}
		//	SDL.SDL_UnlockTexture(s_texture);
		//	if (SDL.SDL_RenderCopy(s_renderer, s_texture, IntPtr.Zero, IntPtr.Zero) != 0)
		//	{
		//		Trace.WriteLine($"ERROR: SDL_RenderCopy failed : {SDL.SDL_GetError()}");
		//	}
		//}
	}
}
