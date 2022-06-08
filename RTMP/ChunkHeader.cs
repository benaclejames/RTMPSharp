using System;
using System.Collections.Generic;
using System.Linq;

namespace RTMP
{
    public class ChunkHeader
    {
        public int Timestamp;
        public int timestampDelta;
        public int MessageLength;
        public short TypeID;
        public int MessageStreamId;
        public int csid;
        public int fmt;
        
        public ChunkHeader(){}

        public ChunkHeader(int fmt, int csid, int timestamp, int messageLength, short typeID, int messageStreamId)
        {
            this.fmt = fmt;
            this.csid = csid;
            Timestamp = timestamp;
            MessageLength = messageLength;
            TypeID = typeID;
            MessageStreamId = messageStreamId;
        }

        public byte[] Encode()
        {
            // Do the reverse of the other constructor
            byte fmt = (byte) csid; 
            byte[] timestamp = BitConverter.GetBytes(Timestamp).Reverse().Skip(1).ToArray();
            byte[] messageLength = BitConverter.GetBytes(MessageLength).Reverse().Skip(1).ToArray();
            byte[] typeID = { (byte)TypeID };
            byte[] messageStreamId = BitConverter.GetBytes(MessageStreamId).Reverse().ToArray();
            
            // Concatenate all the bytes
            List<byte> bytes = new List<byte>();
            bytes.Add(fmt);
            bytes.AddRange(timestamp);
            bytes.AddRange(messageLength);
            bytes.AddRange(typeID);
            bytes.AddRange(messageStreamId);
            return bytes.ToArray();
        }
    }
}