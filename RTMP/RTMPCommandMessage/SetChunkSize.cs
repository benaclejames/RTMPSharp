using System;
using System.Linq;

namespace RTMP.RTMPCommandMessage
{
    public class SetChunkSize : RTMPMessage
    {
        public SetChunkSize(int chunkSize) : base(0, 2, 0, 1, 0)
        {
            data = BitConverter.GetBytes(chunkSize).Reverse().ToArray();
        }
    }
}