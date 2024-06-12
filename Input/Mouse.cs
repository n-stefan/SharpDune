/* Mouse */

namespace SharpDune.Input;

static class Mouse
{
    internal static ushort g_mouseLock;         /*!< Lock for when handling mouse movement. */
    internal static ushort g_mouseX;            /*!< Current X position of the mouse. */
    internal static ushort g_mouseY;            /*!< Current Y position of the mouse. */
    internal static ushort g_mousePrevX;
    internal static ushort g_mousePrevY;
    internal static byte g_prevButtonState;
    internal static ushort g_mouseClickX;
    internal static ushort g_mouseClickY;
    internal static ushort g_regionFlags;
    internal static ushort g_regionMinX;
    internal static ushort g_regionMinY;
    internal static ushort g_regionMaxX;
    internal static ushort g_regionMaxY;

    internal static byte g_mouseDisabled;       /*!< Mouse disabled flag */
    internal static byte g_mouseHiddenDepth;
    internal static byte g_mouseFileID;

    internal static bool g_mouseNoRecordedValue; /*!< used in INPUT_MOUSE_MODE_PLAY */
    internal static ushort g_mouseInputValue;
    internal static ushort g_mouseRecordedTimer;
    internal static ushort g_mouseRecordedX;
    internal static ushort g_mouseRecordedY;

    internal static byte g_mouseMode;
    internal static ushort g_inputFlags;

    internal static ushort g_mouseRegionLeft;    /*!< Region mouse can be in - left position. */
    internal static ushort g_mouseRegionRight;   /*!< Region mouse can be in - right position. */
    internal static ushort g_mouseRegionTop;     /*!< Region mouse can be in - top position. */
    internal static ushort g_mouseRegionBottom;  /*!< Region mouse can be in - bottom position. */

    /*
     * Perform handling of mouse movement iff the mouse position changed.
     * @param newButtonState New button state.
     */
    internal static void Mouse_HandleMovementIfMoved(ushort newButtonState)
    {
        if (Math.Abs((short)g_mouseX - (short)g_mousePrevX) >= 1 ||
            Math.Abs((short)g_mouseY - (short)g_mousePrevY) >= 1)
        {
            Mouse_HandleMovement(newButtonState, g_mouseX, g_mouseY);
        }
    }

    /*
     * Handle movement of the mouse.
     * @param newButtonState State of the mouse buttons.
     * @param mouseX Horizontal position of the mouse cursor.
     * @param mouseY Vertical position of the mouse cursor.
     */
    static void Mouse_HandleMovement(ushort newButtonState, ushort mouseX, ushort mouseY)
    {
        g_mouseLock = 0x1;

        g_mouseX = mouseX;
        g_mouseY = mouseY;
        if (g_mouseMode != (byte)InputMouseMode.INPUT_MOUSE_MODE_PLAY && g_mouseMode != (byte)InputMouseMode.INPUT_MOUSE_MODE_NORMAL && (g_inputFlags & (ushort)InputFlagsEnum.INPUT_FLAG_NO_CLICK) == 0)
        {
            Input_HandleInput(Mouse_CheckButtons(newButtonState));
        }

        Mouse_CheckMovement(mouseX, mouseY);
    }

    /*
     * Compare mouse button state with previous value, and report changes.
     * @param newButtonState New button state.
     * @return \c 0x2D if no change, \c 0x41 for change in first button state,
     *     \c 0x42 for change in second button state, bit 11 means 'button released'.
     */
    static ushort Mouse_CheckButtons(ushort newButtonState)
    {
        byte change;
        ushort result;

        newButtonState &= 0xFF;

        result = 0x2D;
        change = (byte)(newButtonState ^ g_prevButtonState);
        if (change == 0) return result;

        g_prevButtonState = (byte)(newButtonState & 0xFF);

        if ((change & 0x2) != 0)
        {
            result = 0x42;
            if ((newButtonState & 0x2) == 0)
            {
                result |= 0x800;    /* RELEASE */
            }
        }

        if ((change & 0x1) != 0)
        {
            result = 0x41;
            if ((newButtonState & 0x1) == 0)
            {
                result |= 0x800;    /* RELEASE */
            }
        }

        return result;
    }

    /*
     * If the mouse has moved, update its coordinates, and update the region flags.
     * @param mouseX New mouse X coordinate.
     * @param mouseY New mouse Y coordinate.
     */
    static void Mouse_CheckMovement(ushort mouseX, ushort mouseY)
    {
        if (g_mouseHiddenDepth == 0 && (g_mousePrevX != mouseX || g_mousePrevY != mouseY))
        {

            if ((g_regionFlags & 0xC000) != 0xC000)
            {
                GUI_Mouse_Hide();

                if ((g_regionFlags & 0x8000) == 0)
                {
                    GUI_Mouse_Show();
                    g_mousePrevX = mouseX;
                    g_mousePrevY = mouseY;
                    g_mouseLock = 0;
                    return;
                }
            }

            if (mouseX >= g_regionMinX && mouseX <= g_regionMaxX &&
                    mouseY >= g_regionMinY && mouseY <= g_regionMaxY)
            {
                g_regionFlags |= 0x4000;
            }
            else
            {
                GUI_Mouse_Show();
            }
        }

        g_mousePrevX = mouseX;
        g_mousePrevY = mouseY;
        g_mouseLock = 0;
    }

    /*
     * Initialize the mouse driver.
     */
    internal static void Mouse_Init()
    {
        g_mouseX = SCREEN_WIDTH / 2;
        g_mouseY = SCREEN_HEIGHT / 2;
        g_mouseHiddenDepth = 1;
        g_regionFlags = 0;
        g_mouseRegionRight = SCREEN_WIDTH - 1;
        g_mouseRegionBottom = SCREEN_HEIGHT - 1;

        g_mouseDisabled = 1;
        g_mouseFileID = (byte)FileMode.FILE_INVALID;

        Video_Mouse_SetPosition(g_mouseX, g_mouseY);
    }

    internal static void Mouse_SetMouseMode(byte mouseMode, string filename)
    {
        switch ((InputMouseMode)mouseMode)
        {
            default: break;

            case InputMouseMode.INPUT_MOUSE_MODE_NORMAL:
                g_mouseMode = mouseMode;
                if (g_mouseFileID != (byte)FileMode.FILE_INVALID)
                {
                    Input_Flags_ClearBits((ushort)InputFlagsEnum.INPUT_FLAG_KEY_RELEASE);
                    File_Close(g_mouseFileID);
                    g_mouseFileID = (byte)FileMode.FILE_INVALID;
                }
                g_mouseNoRecordedValue = true;
                break;

            case InputMouseMode.INPUT_MOUSE_MODE_RECORD:
                if (g_mouseFileID != (byte)FileMode.FILE_INVALID) break;

                File_Delete_Personal(filename);
                File_Create_Personal(filename);

                Tools_RandomLCG_Seed(0x1234);
                Tools_Random_Seed(0x12344321);

                g_mouseFileID = File_Open_Personal(filename, FileMode.FILE_MODE_READ_WRITE);

                g_mouseMode = mouseMode;

                Input_Flags_SetBits((ushort)InputFlagsEnum.INPUT_FLAG_KEY_RELEASE);

                Input_HandleInput(0x2D);
                break;

            case InputMouseMode.INPUT_MOUSE_MODE_PLAY:
                if (g_mouseFileID == (byte)FileMode.FILE_INVALID)
                {
                    g_mouseFileID = File_Open_Personal(filename, FileMode.FILE_MODE_READ);
                    if (g_mouseFileID == (byte)FileMode.FILE_INVALID)
                    {
                        Trace.WriteLine($"ERROR: Cannot open '{filename}', replay log is impossible.");
                        return;
                    }

                    Tools_RandomLCG_Seed(0x1234);
                    Tools_Random_Seed(0x12344321);
                }

                g_mouseNoRecordedValue = true;

                var buffer = new byte[2];

                File_Read(g_mouseFileID, ref buffer, 2);
                g_mouseInputValue = Read_LE_UInt16(buffer);
                Array.Clear(buffer, 0, 2);

                var length = File_Read(g_mouseFileID, ref buffer, 2);
                g_mouseRecordedTimer = Read_LE_UInt16(buffer);
                Array.Clear(buffer, 0, 2);

                if (length != 2) break;

                if (g_mouseInputValue is >= 0x41 and <= 0x44 or 0x2D)
                {
                    /* 0x2D == '-' 0x41 == 'A' [. . .] 0x44 == 'D' */
                    File_Read(g_mouseFileID, ref buffer, 2);
                    g_mouseRecordedX = Read_LE_UInt16(buffer);
                    Array.Clear(buffer, 0, 2);

                    length = File_Read(g_mouseFileID, ref buffer, 2);
                    g_mouseRecordedY = Read_LE_UInt16(buffer);
                    Array.Clear(buffer, 0, 2);

                    if (length == 2)
                    {
                        g_mouseX = g_mouseRecordedX;
                        g_mouseY = g_mouseRecordedY;
                        g_prevButtonState = 0;

                        GUI_Mouse_Hide_Safe();
                        GUI_Mouse_Show_Safe();

                        g_mouseNoRecordedValue = false;
                        break;
                    }
                    g_mouseNoRecordedValue = true;
                    break;
                }
                g_mouseNoRecordedValue = false;
                break;
        }

        g_timerInput = 0;
        g_mouseMode = mouseMode;
    }

    /*
     * Set the region in which the mouse can move.
     * @note This limits the mouse movement in the hardware.
     *
     * @param left The left side of the region.
     * @param top The top side of the region.
     * @param right The right side of the region.
     * @param bottom The bottom side of the region.
     */
    internal static void Mouse_SetRegion(ushort left, ushort top, ushort right, ushort bottom)
    {
        if (left > right)
        {
            (right, left) = (left, right);
        }
        if (top > bottom)
        {
            (bottom, top) = (top, bottom);
        }

        left = Clamp(left, 0, SCREEN_WIDTH - 1);
        right = Clamp(right, 0, SCREEN_WIDTH - 1);
        top = Clamp(top, 0, SCREEN_HEIGHT - 1);
        bottom = Clamp(bottom, 0, SCREEN_HEIGHT - 1);

        g_mouseRegionLeft = left;
        g_mouseRegionRight = right;
        g_mouseRegionTop = top;
        g_mouseRegionBottom = bottom;

        Video_Mouse_SetRegion(left, right, top, bottom);
    }

    /*
     * Handle the new mouse event.
     */
    internal static void Mouse_EventHandler(ushort mousePosX, ushort mousePosY, bool mouseButtonLeft, bool mouseButtonRight)
    {
        var newButtonState = (byte)((mouseButtonLeft ? 0x1 : 0x0) | (mouseButtonRight ? 0x2 : 0x0));

        if (g_mouseDisabled == 0)
        {
            if (g_mouseMode == (byte)InputMouseMode.INPUT_MOUSE_MODE_NORMAL && (g_inputFlags & (ushort)InputFlagsEnum.INPUT_FLAG_NO_CLICK) == 0)
            {
                Input_HandleInput(Mouse_CheckButtons(newButtonState));
            }

            if (g_mouseMode != (byte)InputMouseMode.INPUT_MOUSE_MODE_PLAY && g_mouseLock == 0)
            {
                Mouse_HandleMovement(newButtonState, mousePosX, mousePosY);
            }
        }
    }

    /*
     * Test whether the mouse cursor is at the border or inside the given rectangle.
     * @param left Left edge.
     * @param top  Top edge.
     * @param right Right edge.
     * @param bottom Bottom edge.
     * @return Mouse is at the border or inside the rectangle.
     */
    internal static ushort Mouse_InsideRegion(short left, short top, short right, short bottom)
    {
        short mx, my;
        ushort inside;

        while (g_mouseLock != 0) SleepIdle();
        g_mouseLock++;

        mx = (short)g_mouseX;
        my = (short)g_mouseY;

        inside = (ushort)((mx < left || mx > right || my < top || my > bottom) ? 0 : 1);

        g_mouseLock--;
        return inside;
    }
}
