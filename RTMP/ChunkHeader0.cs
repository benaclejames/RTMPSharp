using System;
using System.Linq;

namespace RTMP
{
    public class ChunkHeader0
    {
        public int Timestamp;
        public int MessageLength;
        public int TypeID;
        public int MessageStreamId;

        public ChunkHeader0(byte[] msgBytes, ref int offset)
        {
            // First 3 bytes are timestamp
            int intByteSize = sizeof(int);
            byte[] padded = new byte[intByteSize];
            Array.Copy(msgBytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded, 0, 3);
            Timestamp = BitConverter.ToInt32(padded, 0);
            offset += 3;
            
            // Next 3 bytes are message length
            padded = new byte[intByteSize];
            Array.Copy(msgBytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded, 0, 3);
            MessageLength = BitConverter.ToInt32(padded, 0);
            offset += 3;
            
            // Next byte is message type
            TypeID = msgBytes[offset];
            offset += 1;
            
            // Next 4 bytes are stream id
            MessageStreamId = BitConverter.ToInt32(msgBytes.Skip(offset).Take(4).Reverse().ToArray(), 0);
            offset += 4;
        }
        
        public ChunkHeader0(int timestamp, int messageLength, int typeID, int messageStreamId)
        {
            Timestamp = timestamp;
            MessageLength = messageLength;
            TypeID = typeID;
            MessageStreamId = messageStreamId;
        }

        public byte[] Encode()
        {
            // Do the reverse of the other constructor
            byte[] timestamp = BitConverter.GetBytes(Timestamp).Reverse().Skip(1).ToArray();
            byte[] messageLength = BitConverter.GetBytes(MessageLength).Reverse().Skip(1).ToArray();
            byte[] typeID = { (byte)TypeID };
            byte[] messageStreamId = BitConverter.GetBytes(MessageStreamId).Reverse().ToArray();
            
            // Concatenate all the bytes
            return timestamp.Concat(messageLength).Concat(typeID).Concat(messageStreamId).ToArray();
        }
    }
}