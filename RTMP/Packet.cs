using System;
using System.Collections.Generic;
using RTMP.RTMPCommandMessage;

namespace RTMP
{
    public static class Packet
    {
        public static int MaxChunkSize = 128;
        private static readonly Dictionary<int, ChunkHeader> PreviousHeaders = new Dictionary<int, ChunkHeader>();
        
        public static void Parse(byte[] recv, int length)
        {
            byte[] realLength = new byte[length];
            Array.Copy(recv, realLength, length);
            recv = realLength;
            while (recv.Length > 0)
            {
                var offset = 0;
                var header = new ChunkHeader(ref recv);
                FillHeader(header);

                if (header.fmt != 3)
                    PreviousHeaders[header.csid] = header;

                var lastHeader = PreviousHeaders[header.csid];

                var msgBytes = new byte[Math.Min(lastHeader.MessageLength, MaxChunkSize)];
                Array.Copy(recv, 0, msgBytes, 0, msgBytes.Length);
                byte[] newBytes = new byte[recv.Length - msgBytes.Length];
                Array.Copy(recv, msgBytes.Length, newBytes, 0, recv.Length - msgBytes.Length);
                recv = newBytes;
                
                var currentHeader = PreviousHeaders[header.csid];

                Chunk.Decode(currentHeader, msgBytes);
            }
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