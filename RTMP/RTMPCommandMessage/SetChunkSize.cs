using System;
using System.Linq;

namespace RTMP.RTMPCommandMessage
{
    public class SetChunkSize : ChunkHeader
    {
        public SetChunkSize(int chunkSize) : base(0, 2, 0, 1, 0)
        {
            data = BitConverter.GetBytes(chunkSize).Reverse().ToArray();
        }
    }
}