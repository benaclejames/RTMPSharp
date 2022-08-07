using System;
using System.Collections.Generic;
using RTMP.RTMPCommandMessage;

namespace RTMP
{
    public class PacketHandler
    {
        public readonly RTMPClient Parent;

        private int _maxChunkSize = 128;
        private readonly Dictionary<int, ChunkHeader> _previousHeaders = new Dictionary<int, ChunkHeader>();

        public PacketHandler(RTMPClient parentClient) => Parent = parentClient;
        
        public void Parse(byte[] recv, int length)
        {
            var realLength = new byte[length];
            Array.Copy(recv, realLength, length);
            recv = realLength;
            while (recv.Length > 0)
            {
                var header = new ChunkHeader(ref recv);
                FillHeader(header);

                if (header.HeaderFormat != 3)
                    _previousHeaders[header.ChunkStreamId] = header;

                var lastHeader = _previousHeaders[header.ChunkStreamId];

                var msgBytes = new byte[Math.Min(lastHeader.MessageLength, _maxChunkSize)];
                Array.Copy(recv, 0, msgBytes, 0, msgBytes.Length);
                var newBytes = new byte[recv.Length - msgBytes.Length];
                Array.Copy(recv, msgBytes.Length, newBytes, 0, recv.Length - msgBytes.Length);
                recv = newBytes;

                var currentHeader = _previousHeaders[header.ChunkStreamId];

                Chunk.Decode(this, currentHeader, msgBytes);
            }
        }

        private void FillHeader(ChunkHeader header)
        {
            if (!_previousHeaders.ContainsKey(header.ChunkStreamId))
                return;
            var previousHeader = _previousHeaders[header.ChunkStreamId];
            if (previousHeader == null)
                return;

            switch (header.HeaderFormat)
            {
                case 1:
                    header.MessageStreamId = previousHeader.MessageStreamId;
                    break;
                case 2:
                    header.MessageLength = previousHeader.MessageLength;
                    header.MessageStreamId = previousHeader.MessageStreamId;
                    header.TypeId = previousHeader.TypeId;
                    break;
                case 3:
                    header.MessageStreamId = previousHeader.MessageStreamId;
                    header.TypeId = previousHeader.TypeId;
                    header.Timestamp = previousHeader.Timestamp;
                    header.TimestampDelta = previousHeader.TimestampDelta;
                    break;
            }
        }
        
        public void SetChunkSize(int newSize, bool notifyClient = false)
        {
            _maxChunkSize = newSize;
            
            if (notifyClient)
                Parent.EnqueueSend(new SetChunkSize(newSize));
            
            Parent.Log("Chunk size set to " + newSize);
        }
    }
}