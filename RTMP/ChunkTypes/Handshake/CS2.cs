using System.Linq;

namespace RTMP
{
    public class CS2
    {
        public const int Length = 1536;
        
        // Timestamp must match the recv or sent timestamp in cs1
        public byte[] Time = new byte[4];
        
        // Timestamp of the time c1 was read
        public byte[] Time2 = new byte[4];
        
        // Random 1528 byte echo
        public byte[] randData = new byte[1528];

        public CS2(byte[] data)
        {
            // Set the time
            Time = data.Take(4).ToArray();
            
            // Set the time2
            Time2 = data.Skip(4).Take(4).ToArray();
            
            // Set the random data
            randData = data.Skip(8).Take(1528).ToArray();     
        }
    }
}