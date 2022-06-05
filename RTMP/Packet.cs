using System.Collections.Generic;

namespace RTMP
{
    public class Packet
    {
        public static int MaxChunkSize = 128;
        public readonly List<Chunk> Chunks = new List<Chunk>();

        public Packet(byte[] recv, int length)
        {
            int offset = 0;
            // while offset is less than length by 4 or more bytes
            while (offset < length-4)
            {
                Chunks.Add(new Chunk(recv, ref offset));
            }
        }
    }
}