using System;
using System.Linq;

namespace RTMP
{
    public class ControlMessageHandler
    {
        public static void Handle(byte[] chunk, ChunkHeader header)
        {
            switch (header.TypeID)
            {
                case 1: // Set Chunk Size
                    var chunkSizeBytes = chunk.Take(4).Reverse().ToArray();
                    Packet.MaxChunkSize = BitConverter.ToInt32(chunkSizeBytes, 0);
                    Console.WriteLine("Set Chunk Size: " + Packet.MaxChunkSize);
                    break;
                
                case 20:    // AMF0
                    //data = new AMFMessage(ref chunk);
                    Console.WriteLine("AMF0");
                    break;
            }
        }
    }
}