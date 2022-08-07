using System;
using System.Collections.Generic;
using System.Linq;

namespace RTMP
{
    public class ChunkHeader : RTMPSerializeable
    {
        public int Timestamp;
        public int timestampDelta;
        public int MessageLength;
        public short TypeID;
        public int MessageStreamId;
        public int csid;
        public int fmt;
        public byte[] data;

        public ChunkHeader(ref byte[] bytes) => Parse(ref bytes);

        public ChunkHeader(int fmt, int csid, int timestamp, short typeID, int messageStreamId)
        {
            this.fmt = fmt;
            this.csid = csid;
            Timestamp = timestamp;
            TypeID = typeID;
            MessageStreamId = messageStreamId;
        }

        public virtual byte[] Serialize()
        {
            // Do the reverse of the other constructor
            byte basicChunkHeader = (byte)csid;
            byte[] timestamp = BitConverter.GetBytes(Timestamp).Reverse().Skip(1).ToArray();
            byte[] messageLength = BitConverter.GetBytes(data.Length).Reverse().Skip(1).ToArray();
            byte[] typeID = { (byte)TypeID };
            byte[] messageStreamId = BitConverter.GetBytes(MessageStreamId).Reverse().ToArray();
            
            // Concatenate all the bytes
            List<byte> bytes = new List<byte>();
            bytes.Add(basicChunkHeader);
            bytes.AddRange(timestamp);
            bytes.AddRange(messageLength);
            bytes.AddRange(typeID);
            bytes.AddRange(messageStreamId);
            return bytes.Concat(data).ToArray();
        }

        public object Parse(ref byte[] bytes)
        {
            int offset = 0;
            byte fmtByte = bytes[offset];
            offset++;
            var fmt = (fmtByte & 0xff) >> 6;
            var csid = (fmtByte & 0x3f);
            
            if (csid == 0)
            {
                csid = bytes[offset] & 0xff + 64;
                offset++;
            }
            else if (csid == 1)
            {
                var secondByte = bytes[offset];
                var thirdByte = bytes[offset + 1];
                csid = (thirdByte & 0xff) << 8 + (secondByte & 0xff) + 64;
                offset += 2;
            }
            
            this.csid = csid;
            this.fmt = fmt;

            switch (fmt)
            {
                case 0:
                    int intByteSize = sizeof(int);
                    byte[] padded = new byte[intByteSize];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded, 0, 3);
                    Timestamp = BitConverter.ToInt32(padded, 0);
                    offset += 3;
                    
                    padded = new byte[intByteSize];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded, 0, 3);
                    MessageLength = BitConverter.ToInt32(padded, 0);
                    offset += 3;
                    
                    TypeID = (short)(bytes[offset] & 0xff);
                    offset += 1;
                    
                    MessageStreamId = BitConverter.ToInt32(bytes.Skip(offset).Take(4).Reverse().ToArray(), 0);
                    offset += 4;
                    break;
                
                case 1:
                    int intByteSize2 = sizeof(int);
                    byte[] padded2 = new byte[intByteSize2];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded2, 0, 3);
                    Timestamp = BitConverter.ToInt32(padded2, 0);
                    offset += 3;
                    
                    padded2 = new byte[intByteSize2];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded2, 0, 3);
                    MessageLength = BitConverter.ToInt32(padded2, 0);
                    offset += 3;
                    
                    TypeID = (short)(bytes[offset] & 0xff);
                    offset += 1;
                    break;
                
                case 2:
                    int intByteSize3 = sizeof(int);
                    byte[] padded3 = new byte[intByteSize3];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded3, 0, 3);
                    Timestamp = BitConverter.ToInt32(padded3, 0);
                    offset += 3;
                    break;
            }
            
            byte[] newBytes = new byte[bytes.Length - offset];
            Array.Copy(bytes, offset, newBytes, 0, bytes.Length - offset);
            bytes = newBytes;
            return this;
        }
    }
}