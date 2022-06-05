using System;
using System.Linq;

namespace RTMP
{
    public class ControlMessageHandler
    {
        public static void Handle(byte[] chunk, ChunkHeader0 header, ref int offset)
        {
            switch (header.TypeID)
            {
                case 1: // Set Chunk Size
                    var chunkSizeBytes = chunk.Skip(offset).Take(4).Reverse().ToArray();
                    offset += 4;
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