using System;
using System.Linq;

namespace RTMP
{
    public class ControlMessageHandler
    {
        public static void Handle(PacketHandler packetHandler, byte[] chunk, ChunkHeader header)
        {
            switch (header.TypeId)
            {
                case 1: // Set Chunk Size
                    var chunkSizeBytes = chunk.Take(4).Reverse().ToArray();
                    var newSize = BitConverter.ToInt32(chunkSizeBytes, 0);
                    packetHandler.SetChunkSize(newSize);
                    break;
            }
        }
    }
}