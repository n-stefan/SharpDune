/* Decoder for 'format80' files */

namespace SharpDune.Codec;

class Format80
{
    /*
     * Decode a memory fragment which is encoded with 'format80'.
     * @param dest The place the decoded fragment will be loaded.
     * @param source The encoded fragment.
     * @param destLength The length of the destination buffer.
     * @return The length of decoded data.
     */
    internal static ushort Format80_Decode(Span<byte> dest, Span<byte> source, ushort destLength, int destPointer/* = 0*/, int sourcePointer/* = 0*/)
    {
        var start = destPointer;
        var end = destPointer + destLength;

        while (destPointer != end)
        {
            byte cmd;
            ushort size;
            ushort offset;

            cmd = source[sourcePointer++];

            if (cmd == 0x80)
            {
                /* Exit */
                break;
            }
            else if ((cmd & 0x80) == 0)
            {
                /* Short move, relative */
                size = (ushort)((cmd >> 4) + 3);
                if (size > end - destPointer) size = (ushort)(end - destPointer);

                offset = (ushort)(((cmd & 0xF) << 8) + source[sourcePointer++]);

                /* This decoder assumes memcpy. As some platforms implement memcpy as memmove, this is much safer */
                for (; size > 0; size--) { dest[destPointer] = dest[(ushort)(destPointer - offset)]; destPointer++; }
            }
            else if (cmd == 0xFE)
            {
                /* Long set */
                size = source[sourcePointer++];
                size += (ushort)((source[sourcePointer++]) << 8);
                if (size > end - destPointer) size = (ushort)(end - destPointer);

                dest.Slice(destPointer, size).Fill(source[sourcePointer++]); //memset(dest, (*source++), size);
                destPointer += size;
            }
            else if (cmd == 0xFF)
            {
                /* Long move, absolute */
                size = source[sourcePointer++];
                size += (ushort)((source[sourcePointer++]) << 8);
                if (size > end - destPointer) size = (ushort)(end - destPointer);

                offset = source[sourcePointer++];
                offset += (ushort)((source[sourcePointer++]) << 8);

                /* This decoder assumes memcpy. As some platforms implement memcpy as memmove, this is much safer */
                for (; size > 0; size--) dest[destPointer++] = dest[start + offset++]; //start[offset++];
            }
            else if ((cmd & 0x40) != 0)
            {
                /* Short move, absolute */
                size = (ushort)((cmd & 0x3F) + 3);
                if (size > end - destPointer) size = (ushort)(end - destPointer);

                offset = source[sourcePointer++];
                offset += (ushort)((source[sourcePointer++]) << 8);

                /* This decoder assumes memcpy. As some platforms implement memcpy as memmove, this is much safer */
                for (; size > 0; size--) dest[destPointer++] = dest[start + offset++]; //start[offset++];
            }
            else
            {
                /* Short copy */
                size = (ushort)(cmd & 0x3F);
                if (size > end - destPointer) size = (ushort)(end - destPointer);

                /* This decoder assumes memcpy. As some platforms implement memcpy as memmove, this is much safer */
                for (; size > 0; size--) dest[destPointer++] = source[sourcePointer++];
            }
        }

        return (ushort)(destPointer - start);
    }
}
