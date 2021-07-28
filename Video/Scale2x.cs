/*
 * This file is part of the Scale2x project.
 *
 * Copyright (C) 2001, 2002, 2003, 2004 Andrea Mazzoleni
 * Copyright (C) 2015 Thomas Bernard
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
 * This file contains C, SSE2 and Altivec implementation of the Scale2x effect.
 *
 * You can find an high level description of the effect at :
 *
 * http://www.scale2x.it/
 */

using System.Diagnostics;

namespace SharpDune.Video
{
	class Scale2x
	{
		/*
         * Scale by a factor of 2 a row of pixels of 8 bits.
         * The function is implemented in C.
         * The pixels over the left and right borders are assumed of the same color of
         * the pixels on the border.
         * Note that the implementation is optimized to write data sequentially to
         * maximize the bandwidth on video memory.
         * \param src0 Pointer at the first pixel of the previous row.
         * \param src1 Pointer at the first pixel of the current row.
         * \param src2 Pointer at the first pixel of the next row.
         * \param count Length in pixels of the src0, src1 and src2 rows.
         * It must be at least 2.
         * \param dst0 First destination row, double length in pixels.
         * \param dst1 Second destination row, double length in pixels.
         */
		void scale2x_8_def(byte[] dst0, byte[] dst1, byte[] src0, byte[] src1, byte[] src2, ushort count)
		{
#if USE_SCALE_RANDOMWRITE
	        scale2x_8_def_whole(dst0, dst1, src0, src1, src2, count);
#else
			scale2x_8_def_border(dst0, src0, src1, src2, count);
			scale2x_8_def_border(dst1, src2, src1, src0, count);
#endif
		}

#if !USE_SCALE_RANDOMWRITE
		static void scale2x_8_def_border(byte[] dst, byte[] src0, byte[] src1, byte[] src2, ushort count)
		{
			Debug.Assert(count >= 2);

			/* first pixel */
			if (src0[0] != src2[0] && src1[0] != src1[1])
			{
				dst[0] = src1[0] == src0[0] ? src0[0] : src1[0];
				dst[1] = src1[1] == src0[0] ? src0[0] : src1[0];
			}
			else
			{
				dst[0] = src1[0];
				dst[1] = src1[0];
			}
			++src0;
			++src1;
			++src2;
			dst += 2;

			/* central pixels */
			count -= 2;
			while (count)
			{
				if (src0[0] != src2[0] && src1[-1] != src1[1])
				{
					dst[0] = src1[-1] == src0[0] ? src0[0] : src1[0];
					dst[1] = src1[1] == src0[0] ? src0[0] : src1[0];
				}
				else
				{
					dst[0] = src1[0];
					dst[1] = src1[0];
				}

				++src0;
				++src1;
				++src2;
				dst += 2;
				--count;
			}

			/* last pixel */
			if (src0[0] != src2[0] && src1[-1] != src1[0])
			{
				dst[0] = src1[-1] == src0[0] ? src0[0] : src1[0];
				dst[1] = src1[0] == src0[0] ? src0[0] : src1[0];
			}
			else
			{
				dst[0] = src1[0];
				dst[1] = src1[0];
			}
		}
#endif
	}
}
