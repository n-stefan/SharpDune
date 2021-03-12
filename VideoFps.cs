/* Display FPS in top right of the screen */

namespace SharpDune
{
    class VideoFps
    {
		delegate void Video_ShowFPS_Proc(byte[] screen, ushort x, byte digit);

        internal static void Video_ShowFPS(byte[] screen) =>
            Video_ShowFPS_2(screen, 320, null);

		static uint[] s_previousTimeStamps = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		static byte s_previousTimeStampsIndex = 0;
		static void Video_ShowFPS_2(byte[] screen, int bytes_per_row, Video_ShowFPS_Proc drawchar)
		{
			uint timeStamp;

			timeStamp = Timer.Timer_GetTime();
			if (s_previousTimeStamps[s_previousTimeStampsIndex] > 0
					&& timeStamp != s_previousTimeStamps[s_previousTimeStampsIndex])
			{
				int x, i;
				/* calculate average frames per 1000sec on the 16 last time measures */
				var kfps = 16000000 / (timeStamp - s_previousTimeStamps[s_previousTimeStampsIndex]);
				for (x = 320 - 4; kfps > 0; kfps /= 10, x -= 4)
				{
					/* draw the digits */
					if (drawchar != null)
						drawchar(screen, (ushort)x, (byte)(kfps % 10));
					else
						Video_ShowFPS_DrawChar(screen, bytes_per_row, (ushort)x, (byte)(kfps % 10));
				}
				if (drawchar == null)
				{
					for (i = 0; i < 5; i++) screen[x + 2 + i * bytes_per_row] = 0;
				}
			}
			s_previousTimeStamps[s_previousTimeStampsIndex] = timeStamp;
			s_previousTimeStampsIndex = (byte)((s_previousTimeStampsIndex + 1) & 0x0f);
		}

		static byte[] fontdigits = { 0167, 044, 0135, 0155, 056, 0153, 0173, 045, 0177, 0157 };
		static byte[] fonttestsegments = { 03, 01, 05, 02, 0, 04, 032, 010, 054, 020, 0, 040, 0120, 0100, 0140 };
		static void Video_ShowFPS_DrawChar(byte[] screen, int bytes_per_row, ushort x, byte digit)
		{
			int i;
			var segments = fontdigits[digit];
			var offset = 0;
			for (i = 0; i < 15; i++)
			{
				screen[x + offset] = (byte)(((segments & fonttestsegments[i]) == fonttestsegments[i]) ? 15 : 0);
				offset++;
				if ((i % 3) == 2)
				{
					screen[x + offset] = 0;
					offset += bytes_per_row - 3;
				}
			}
		}
	}
}
