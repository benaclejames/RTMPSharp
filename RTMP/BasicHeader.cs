using System;
using System.Linq;

namespace RTMP
{
    public class BasicHeader
    {
        public int ChunkHeaderFormat;
        public int CSID;
        
        public BasicHeader(int chunkHeaderFormat, int csid)
        {
            ChunkHeaderFormat = chunkHeaderFormat;
            CSID = csid;
        }

        public byte[] Encode()
        {
            // Encode the ChunkHeaderFormat into the first 2 bits of the first byte and the CSID into the last 6 bits of the first byte
            return new[] { (byte)((ChunkHeaderFormat << 6) | CSID) };
        }
        
        public BasicHeader(byte[] chunk, ref int offset)
        {
            // Convert the first two bits of the first byte to an int
            ChunkHeaderFormat = chunk[offset] >> 6;
            
            // If the last 6 bits of the first byte equal 0, the headerType is 2.
            // If they equal 1, the headerType is 3.
            // If not, the headerType is 1.
            
            // Get the last 6 bits of the first byte
            int last6Bits = chunk[offset] & 0x3F;

            offset++;

            if (last6Bits == 0)
            {
                CSID = chunk[offset] + 64;
                offset++;
            }
            else if (last6Bits == 1)
            {
                CSID = chunk[offset+1]*256 + chunk[offset] + 64;
                offset += 2;
            }
            else
            {
                CSID = last6Bits;
            }
        }
    }
}