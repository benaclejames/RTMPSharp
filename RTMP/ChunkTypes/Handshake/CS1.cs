using System;
using System.Linq;

namespace RTMP
{
    // Initializer, sorta
    public class CS1
    {
        public const int Length = 1536;
        public byte[] randData;

        public byte[] time;

        public CS1(byte[] data)
        {
            // First 4 bytes correspond to the timestamp, which we use as the epoch for all future chunks
            time = data.Take(4).ToArray();

            // Next 1528 bytes are random
            randData = data.Skip(8).Take(1528).ToArray();
        }
    }
}