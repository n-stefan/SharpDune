/*
 * This file is part of the Scale2x project.
 *
 * Copyright (C) 2018 Thomas Bernard
 * Copyright (C) 2003, 2004 Andrea Mazzoleni
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

/*
 * This file contains an example implementation of the Scale effect
 * applyed to a generic bitmap.
 *
 * You can find an high level description of the effect at :
 *
 * http://www.scale2x.it/
 */

using System;

namespace SharpDune
{
	delegate void stage_scale2x_t(object dst0, object dst1, object src0, object src1, object src2, ushort pixel_per_row);
	delegate void stage_scale3x_t(object dst0, object dst1, object dst2, object src0, object src1, object src2, ushort pixel_per_row);
	delegate void stage_scale2x4_t(object dst0, object dst1, object dst2, object dst3, object src0, object src1, object src2, ushort pixel_per_row);

	[Flags]
	enum Scale2xOption
    {
		SCALE2X_OPTION_DEFAULT = 0,
		SCALE2X_OPTION_NO_SSE2 = 0x001
	}

	class stage_scale_funcs
	{
		internal stage_scale2x_t stage_scale2x;
		internal stage_scale3x_t stage_scale2x3;
		internal stage_scale2x4_t stage_scale2x4;
		internal stage_scale3x_t stage_scale3x;
	}

	class ScaleBit
	{
		static Scale2xOption scale_options = Scale2xOption.SCALE2X_OPTION_DEFAULT;

		/*
		 * Apply the Scale effect on a bitmap.
		 * This function is simply a common interface for ::scale2x(), ::scale3x() and ::scale4x().
		 * \param scale Scale factor. 2, 203 (fox 2x3), 204 (for 2x4), 3 or 4.
		 * \param void_dst Pointer at the first pixel of the destination bitmap.
		 * \param dst_slice Size in bytes of a destination bitmap row.
		 * \param void_src Pointer at the first pixel of the source bitmap.
		 * \param src_slice Size in bytes of a source bitmap row.
		 * \param pixel Bytes per pixel of the source and destination bitmap.
		 * \param width Horizontal size in pixels of the source bitmap.
		 * \param height Vertical size in pixels of the source bitmap.
		 */
		internal static void scale_part(ushort scale, object void_dst, ushort dst_slice, object void_src, ushort src_slice, ushort pixel, ushort width, ushort height, ushort top, ushort bottom)
		{
			stage_scale_funcs funcs = new stage_scale_funcs();

			switch (pixel)
			{
				case 1:
					if ((scale_options & Scale2xOption.SCALE2X_OPTION_NO_SSE2) == Scale2xOption.SCALE2X_OPTION_NO_SSE2)
					{
						funcs.stage_scale2x = (stage_scale2x_t)scale2x_8_def;
						funcs.stage_scale2x3 = (stage_scale3x_t)scale2x3_8_def;
						funcs.stage_scale2x4 = (stage_scale2x4_t)scale2x4_8_def;
					}
					else
					{
						funcs.stage_scale2x = (stage_scale2x_t)scale2x_8_sse2;
						funcs.stage_scale2x3 = (stage_scale3x_t)scale2x3_8_sse2;
						funcs.stage_scale2x4 = (stage_scale2x4_t)scale2x4_8_sse2;
					}
					funcs.stage_scale3x = (stage_scale3x_t)scale3x_8_def;
					break;
				case 2:
					if ((scale_options & Scale2xOption.SCALE2X_OPTION_NO_SSE2) == Scale2xOption.SCALE2X_OPTION_NO_SSE2)
					{
						funcs.stage_scale2x = (stage_scale2x_t)scale2x_16_def;
						funcs.stage_scale2x3 = (stage_scale3x_t)scale2x3_16_def;
						funcs.stage_scale2x4 = (stage_scale2x4_t)scale2x4_16_def;
					}
					else
					{
						funcs.stage_scale2x = (stage_scale2x_t)scale2x_16_sse2;
						funcs.stage_scale2x3 = (stage_scale3x_t)scale2x3_16_sse2;
						funcs.stage_scale2x4 = (stage_scale2x4_t)scale2x4_16_sse2;
					}
					funcs.stage_scale3x = (stage_scale3x_t)scale3x_16_def;
					break;
				case 4:
					if ((scale_options & Scale2xOption.SCALE2X_OPTION_NO_SSE2) == Scale2xOption.SCALE2X_OPTION_NO_SSE2)
					{
						funcs.stage_scale2x = (stage_scale2x_t)scale2x_32_def;
						funcs.stage_scale2x3 = (stage_scale3x_t)scale2x3_32_def;
						funcs.stage_scale2x4 = (stage_scale2x4_t)scale2x4_32_def;
					}
					else
					{
						funcs.stage_scale2x = (stage_scale2x_t)scale2x_32_sse2;
						funcs.stage_scale2x3 = (stage_scale3x_t)scale2x3_32_sse2;
						funcs.stage_scale2x4 = (stage_scale2x4_t)scale2x4_32_sse2;
					}
					funcs.stage_scale3x = (stage_scale3x_t)scale3x_32_def;
					break;
				default:
					return;
			}
			switch (scale)
			{
				case 202:
				case 2:
					scale2x(funcs, void_dst, dst_slice, void_src, src_slice, width, height, top, bottom);
					break;
				case 203:
					scale2x3(funcs, void_dst, dst_slice, void_src, src_slice, width, height);
					break;
				case 204:
					scale2x4(funcs, void_dst, dst_slice, void_src, src_slice, width, height);
					break;
				case 303:
				case 3:
					scale3x(funcs, void_dst, dst_slice, void_src, src_slice, width, height, top, bottom);
					break;
				case 404:
				case 4:
					scale4x(funcs, void_dst, dst_slice, void_src, src_slice, pixel, width, height, top, bottom);
					break;
			}
		}
	}
}
