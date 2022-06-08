using System;
using System.Collections.Generic;
using System.Linq;

namespace RTMP
{
    public static class Packet
    {
        public static int MaxChunkSize = 128;
        private static readonly Dictionary<int, ChunkHeader> PreviousHeaders = new Dictionary<int, ChunkHeader>();
        
        public static void Parse(List<byte> recv, int length)
        {
            var current = 0;
            while (current < length)
            {
                var offset = 0;
                var header = ReadHeader(recv.ToArray(), ref offset);
                current += offset;
                recv.RemoveRange(0, offset);
                FillHeader(header);

                if (header.fmt != 3)
                    PreviousHeaders[header.csid] = header;

                var lastHeader = PreviousHeaders[header.csid];

                var msgBytes = new byte[Math.Min(lastHeader.MessageLength, MaxChunkSize)];
                Array.Copy(recv.ToArray(), 0, msgBytes, 0, msgBytes.Length);
                current += msgBytes.Length;
                recv.RemoveRange(0, msgBytes.Length);

                var currentHeader = PreviousHeaders[header.csid];

                Chunk.Decode(currentHeader, msgBytes);
            }
        }

        private static ChunkHeader ReadHeader(byte[] bytes, ref int offset)
        {
            ChunkHeader header = new ChunkHeader();
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
            
            header.csid = csid;
            header.fmt = fmt;

            switch (fmt)
            {
                case 0:
                    int intByteSize = sizeof(int);
                    byte[] padded = new byte[intByteSize];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded, 0, 3);
                    header.Timestamp = BitConverter.ToInt32(padded, 0);
                    offset += 3;
                    
                    padded = new byte[intByteSize];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded, 0, 3);
                    header.MessageLength = BitConverter.ToInt32(padded, 0);
                    offset += 3;
                    
                    header.TypeID = (short)(bytes[offset] & 0xff);
                    offset += 1;
                    
                    header.MessageStreamId = BitConverter.ToInt32(bytes.Skip(offset).Take(4).Reverse().ToArray(), 0);
                    offset += 4;
                    break;
                
                case 1:
                    int intByteSize2 = sizeof(int);
                    byte[] padded2 = new byte[intByteSize2];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded2, 0, 3);
                    header.Timestamp = BitConverter.ToInt32(padded2, 0);
                    offset += 3;
                    
                    padded2 = new byte[intByteSize2];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded2, 0, 3);
                    header.MessageLength = BitConverter.ToInt32(padded2, 0);
                    offset += 3;
                    
                    header.TypeID = (short)(bytes[offset] & 0xff);
                    offset += 1;
                    break;
                
                case 2:
                    int intByteSize3 = sizeof(int);
                    byte[] padded3 = new byte[intByteSize3];
                    Array.Copy(bytes.Skip(offset).Take(3).Reverse().ToArray(), 0, padded3, 0, 3);
                    header.Timestamp = BitConverter.ToInt32(padded3, 0);
                    offset += 3;
                    break;
            }

            return header;
        }

        private static void FillHeader(ChunkHeader header)
        {
            if (!PreviousHeaders.ContainsKey(header.csid))
                return;
            ChunkHeader previousHeader = PreviousHeaders[header.csid];
            if (previousHeader == null)
                return;

            switch (header.fmt)
            {
                case 1:
                    header.MessageStreamId = previousHeader.MessageStreamId;
                    break;
                case 2:
                    header.MessageLength = previousHeader.MessageLength;
                    header.MessageStreamId = previousHeader.MessageStreamId;
                    header.TypeID = previousHeader.TypeID;
                    break;
                case 3:
                    header.MessageStreamId = previousHeader.MessageStreamId;
                    header.TypeID = previousHeader.TypeID;
                    header.Timestamp = previousHeader.Timestamp;
                    header.timestampDelta = previousHeader.timestampDelta;
                    break;
            }
        }
    }
}