using System;
using System.Collections.Generic;
using System.Linq;

namespace RTMP
{
    public class ChunkHeader : RTMPSerializeable
    {
        public int ChunkStreamId;
        protected byte[] data;
        public int HeaderFormat;
        public int MessageLength;
        public int MessageStreamId;
        public int Timestamp;
        public int TimestampDelta;
        public short TypeId;

        public ChunkHeader(ref byte[] bytes) => Parse(ref bytes);

        protected ChunkHeader(int headerFormat, int chunkStreamId, int timestamp, short typeID, int messageStreamId)
        {
            HeaderFormat = headerFormat;
            ChunkStreamId = chunkStreamId;
            Timestamp = timestamp;
            TypeId = typeID;
            MessageStreamId = messageStreamId;
        }

        public virtual byte[] Serialize()
        {
            // Basic chunk header is 2 bits for the header format, and then the rest of the byte for the chunk stream id.
            byte basicChunkHeader = (byte)((byte)HeaderFormat << 6 | (byte)(ChunkStreamId & 0b_00111111));
            var timestamp = BitConverter.GetBytes(Timestamp).Reverse().Skip(1).ToArray();
            var messageLength = BitConverter.GetBytes(data.Length).Reverse().Skip(1).ToArray();
            byte[] typeID = { (byte)TypeId };
            var messageStreamId = BitConverter.GetBytes(MessageStreamId).Reverse().ToArray();

            // Concatenate all the bytes
            var bytes = new List<byte>();
            bytes.Add(basicChunkHeader);
            bytes.AddRange(timestamp);
            bytes.AddRange(messageLength);
            bytes.AddRange(typeID);
            bytes.AddRange(messageStreamId);
            return bytes.Concat(data).ToArray();
        }

        public object Parse(ref byte[] bytes)
        {
            var offset = 0;
            var fmtByte = bytes[offset];
            offset++;
            var fmt = (fmtByte & 0xff) >> 6;
            var csid = fmtByte & 0x3f;

            if (csid == 0)
            {
                csid = bytes[offset] & (0xff + 64);
                offset++;
            }
            else if (csid == 1)
            {
                var secondByte = bytes[offset];
                var thirdByte = bytes[offset + 1];
                csid = (thirdByte & 0xff) << (8 + (secondByte & 0xff) + 64);
                offset += 2;
            }

            ChunkStreamId = csid;
            HeaderFormat = fmt;

            switch (fmt)
            {
                case 0:
                    var intByteSize = sizeof(int);
                    var padded = new byte[intByteSize];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded, 0, 3);
                    Timestamp = BitConverter.ToInt32(padded, 0);
                    offset += 3;

                    padded = new byte[intByteSize];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded, 0, 3);
                    MessageLength = BitConverter.ToInt32(padded, 0);
                    offset += 3;

                    TypeId = (short)(bytes[offset] & 0xff);
                    offset += 1;

                    MessageStreamId = BitConverter.ToInt32(bytes.Skip(offset).Take(4).Reverse().ToArray(), 0);
                    offset += 4;
                    break;

                case 1:
                    var intByteSize2 = sizeof(int);
                    var padded2 = new byte[intByteSize2];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded2, 0, 3);
                    Timestamp = BitConverter.ToInt32(padded2, 0);
                    offset += 3;

                    padded2 = new byte[intByteSize2];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded2, 0, 3);
                    MessageLength = BitConverter.ToInt32(padded2, 0);
                    offset += 3;

                    TypeId = (short)(bytes[offset] & 0xff);
                    offset += 1;
                    break;

                case 2:
                    var intByteSize3 = sizeof(int);
                    var padded3 = new byte[intByteSize3];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded3, 0, 3);
                    Timestamp = BitConverter.ToInt32(padded3, 0);
                    offset += 3;
                    break;
            }

            var newBytes = new byte[bytes.Length - offset];
            Array.Copy(bytes, offset, newBytes, 0, bytes.Length - offset);
            bytes = newBytes;
            return this;
        }
    }
}