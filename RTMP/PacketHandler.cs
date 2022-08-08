using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using RTMP.RTMPCommandMessage;

namespace RTMP
{
    public class PacketHandler
    {
        public readonly RTMPClient Parent;

        private int _maxChunkSize = 128;
        private readonly Dictionary<int, ChunkHeader> _previousHeaders = new Dictionary<int, ChunkHeader>();
        private readonly Dictionary<int, IncompleteChunk> _incompleteChunks = new Dictionary<int, IncompleteChunk>();

        public PacketHandler(RTMPClient parentClient) => Parent = parentClient;

        private int state = 0;
        private int currentStreamId;
        IncompleteChunk currentPayload = null;
        
        // Create an add or replace function
        private void AddOrReplaceHeader(int streamId, ChunkHeader header)
        {
            if (_previousHeaders.ContainsKey(streamId))
                _previousHeaders[streamId] = header;
            else
                _previousHeaders.Add(streamId, header);
        }
        
        private void AddOrReplacePayload(int streamId, IncompleteChunk chunk)
        {
            if (_incompleteChunks.ContainsKey(streamId))
                _incompleteChunks[streamId] = chunk;
            else
                _incompleteChunks.Add(streamId, chunk);
        }

        public void Parse(Stream stream)
        {
            // Parse has two states, header and data. Data can only be read after header.
            // Header is read first, then stored in the _previousHeaders dictionary.

            if (state == 0)
            {
                var header = new ChunkHeader(ref stream);
                FillHeader(ref header);

                currentStreamId = header.ChunkStreamId;
                Console.WriteLine("CSID: " + currentStreamId+", MsgType: "+(header.TypeId == 9 ? "Video" : "Audio")+", Length: " + header.MessageLength);

                if (header.HeaderFormat != 3)
                {
                    var buffer = new IncompleteChunk(header.MessageLength, header.MessageLength);
                    AddOrReplacePayload(header.ChunkStreamId, buffer);
                    AddOrReplaceHeader(header.ChunkStreamId, header);
                }

                currentPayload = _incompleteChunks.ContainsKey(header.ChunkStreamId)
                    ? _incompleteChunks[header.ChunkStreamId]
                    : null;
                if (currentPayload == null)
                {
                    var previousHeader = _previousHeaders[header.ChunkStreamId];
                    Console.WriteLine("Current payload null, previous header: "+previousHeader);
                    currentPayload =  new IncompleteChunk(previousHeader.MessageLength, previousHeader.MessageLength);
                    AddOrReplacePayload(header.ChunkStreamId, currentPayload);
                    Console.WriteLine("Current payload assigned as "+currentPayload);
                }

                state = 1;
            }
            else if (state == 1)
            {
                byte[] finalBytes = new byte[Math.Min(currentPayload.WriteableBytes(), _maxChunkSize)];
                var readBytes = stream.Read(finalBytes, 0, finalBytes.Length);
                if (finalBytes.Length != readBytes)
                    throw new Exception("Did not read all bytes");
                currentPayload.Write(finalBytes, finalBytes.Length);
                state = 0;

                if (currentPayload.IsWriteable())
                    return;

                _incompleteChunks.Remove(currentStreamId);

                var header = _previousHeaders[currentStreamId];

                Chunk.Decode(this, header, currentPayload.bytes);
            }
        }

        private void FillHeader(ref ChunkHeader header)
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