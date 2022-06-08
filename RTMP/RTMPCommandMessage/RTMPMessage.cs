using System;
using System.Linq;
using System.Net.Sockets;

namespace RTMP.RTMPCommandMessage
{
    public class RTMPMessage
    {
        private int fmt;
        private int csid;
        private int timestamp;
        private short typeID;
        private int messageStreamId;
        
        protected byte[] data;

        
        public RTMPMessage(int fmt, int csid, int timestamp, short typeID, int messageStreamId)
        {
            // Init all members
            this.fmt = fmt;
            this.csid = csid;
            this.timestamp = timestamp;
            this.typeID = typeID;
            this.messageStreamId = messageStreamId;
            data = Array.Empty<byte>();
        }

        public virtual void Enqueue(NetworkStream stream)
        {
            var header = new ChunkHeader(fmt, csid, timestamp, data.Length, typeID, messageStreamId);
            var chunk = header.Encode().Concat(data).ToArray();
            stream.Write(chunk, 0, chunk.Length);
        }
        
    }
}