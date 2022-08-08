using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

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

        public ChunkHeader(ref Stream stream) => Parse(ref stream);

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
            throw new NotImplementedException();
        }

        public object Parse(ref Stream stream)
        {
            var fmtByte = stream.ReadByte();
            var fmt = (fmtByte & 0xff) >> 6;
            // Ensure fmt is 0-3
            if (fmt > 3)
                throw new Exception("Invalid header format");
            var csid = fmtByte & 0x3f;

            if (csid == 0)
            {
                csid = stream.ReadByte() & (0xff + 64);
            }
            else if (csid == 1)
            {
                var secondByte = stream.ReadByte();
                var thirdByte = stream.ReadByte();
                csid = (thirdByte & 0xff) << (8 + (secondByte & 0xff) + 64);
            }

            ChunkStreamId = csid;
            HeaderFormat = fmt;

            switch (fmt)
            {
                case 0:
                    var intByteSize = sizeof(int);
                    var padded = new byte[intByteSize];
                    var meme = new byte[3];
                    stream.Read(meme, 0, 3);
                    Array.Copy(meme.Reverse().ToArray(), 0, padded, 0, 3);
                    Timestamp = BitConverter.ToInt32(padded, 0);
                    if (Timestamp == 0xffffff)
                        Console.WriteLine("AAAA");

                    padded = new byte[intByteSize];
                    meme = new byte[3];
                    stream.Read(meme, 0, 3);
                    Array.Copy(meme.Reverse().ToArray(), 0, padded, 0, 3);
                    MessageLength = BitConverter.ToInt32(padded, 0);

                    TypeId = (short)(stream.ReadByte() & 0xff);

                    byte[] messageStreamId = new byte[4];
                    stream.Read(messageStreamId, 0, 4);
                    MessageStreamId = BitConverter.ToInt32(messageStreamId.Reverse().ToArray(), 0);
                    
                    break;

                case 1:
                    var intByteSize2 = sizeof(int);
                    var padded2 = new byte[intByteSize2];
                    var meme2 = new byte[3];
                    stream.Read(meme2, 0, 3);
                    Array.Copy(meme2.Reverse().ToArray(), 0, padded2, 0, 3);
                    Timestamp = BitConverter.ToInt32(padded2, 0);
                    if (Timestamp == 0xffffff)
                        Console.WriteLine("AAAA");

                    padded2 = new byte[intByteSize2];
                    meme2 = new byte[3];
                    stream.Read(meme2, 0, 3);
                    Array.Copy(meme2.Reverse().ToArray(), 0, padded2, 0, 3);
                    MessageLength = BitConverter.ToInt32(padded2, 0);

                    TypeId = (short)(stream.ReadByte() & 0xff);
                    break;

                case 2:
                    var intByteSize3 = sizeof(int);
                    var padded3 = new byte[intByteSize3];
                    var meme3 = new byte[3];
                    stream.Read(meme3, 0, 3);
                    Array.Copy(meme3.Reverse().ToArray(), 0, padded3, 0, 3);
                    Timestamp = BitConverter.ToInt32(padded3, 0);
                    if (Timestamp == 0xffffff)
                        Console.WriteLine("AAAA");
                    break;
            }

            return this;
        }
    }
}