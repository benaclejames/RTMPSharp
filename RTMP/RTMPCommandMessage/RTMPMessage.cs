using System;
using System.Linq;

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

        public byte[] Encode()
        {
            var header = new ChunkHeader(fmt, csid, timestamp, data.Length, typeID, messageStreamId);
            return header.Encode().Concat(data).ToArray();
        }
    }
}