using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTMP
{
    public class Chunk
    {
        public BasicHeader header;
        public static ChunkHeader0 lastMessageHeader;
        public object data;

        public static ChunkHeader0 GetMessageHeader(BasicHeader basic, byte[] headerBytes, ref int offset)
        {
            switch (basic.ChunkHeaderFormat)
            {
                case 0:
                    lastMessageHeader = new ChunkHeader0(headerBytes, ref offset);
                    return lastMessageHeader;
                case 3:
                    return lastMessageHeader;
            }

            return null;
        }
        
        public Chunk(byte[] chunk, ref int offset)
        {
            header = new BasicHeader(chunk, ref offset);
            var messageHeader = GetMessageHeader(header, chunk, ref offset);
            
            if (header.CSID == 2) // Control Messages
            {
                ControlMessageHandler.Handle(chunk, messageHeader, ref offset);
                return;
            }
            
            if (messageHeader.TypeID == 20) // AMF Encoding
            {
                var msg = new AMFMessage(chunk, ref offset);
                if ((string) msg.Objects[0] == "connect")
                {
                    List<byte> newChunk = new List<byte>();
                    newChunk.AddRange(new BasicHeader(0, 2).Encode());
                    newChunk.AddRange(new ChunkHeader0(0, 4, 5, 0).Encode());
                    newChunk.AddRange(BitConverter.GetBytes(5000000).Reverse());
                    RTMPClient.stream.Write(newChunk.ToArray(), 0, newChunk.Count);
                    
                    newChunk.Clear();
                    newChunk.AddRange(new BasicHeader(0, 2).Encode());
                    newChunk.AddRange(new ChunkHeader0(0, 5, 6, 0).Encode());
                    newChunk.AddRange(BitConverter.GetBytes(5000000).Reverse());
                    newChunk.Add(1);
                    RTMPClient.stream.Write(newChunk.ToArray(), 0, newChunk.Count);
                    
                    newChunk.Clear();
                    newChunk.AddRange(new BasicHeader(0, 2).Encode());
                    newChunk.AddRange(new ChunkHeader0(0, 4, 1, 0).Encode());
                    newChunk.AddRange(BitConverter.GetBytes(5000).Reverse());
                    RTMPClient.stream.Write(newChunk.ToArray(), 0, newChunk.Count);

                    newChunk.Clear();

                    List<byte> data = new List<byte>();
                    data.Add(2);
                    data.AddRange(new AMFString().Encode("_result"));
                    data.Add(0);
                    data.AddRange(new AMFNumber().Encode(1));
                    
                    data.Add(3);
                    data.AddRange(new AMFString().Encode("fmsVer"));
                    data.Add(2);
                    data.AddRange(new AMFString().Encode("FMS/3,0,1,123"));
                    data.AddRange(new AMFString().Encode("capabilities"));
                    data.Add(0);
                    data.AddRange(new AMFNumber().Encode(31));
                    data.AddRange(new byte[]{0,0,9});
                    
                    data.Add(3);
                    data.AddRange(new AMFString().Encode("level"));
                    data.Add(2);
                    data.AddRange(new AMFString().Encode("status"));
                    data.AddRange(new AMFString().Encode("code"));
                    data.Add(2);
                    data.AddRange(new AMFString().Encode("NetConnection.Connect.Success"));
                    data.AddRange(new AMFString().Encode("description"));
                    data.Add(2);
                    data.AddRange(new AMFString().Encode("Connection succeeded"));
                    data.AddRange(new AMFString().Encode("objectEncoding"));
                    data.Add(0);
                    data.AddRange(new AMFNumber().Encode(0));
                    data.AddRange(new byte[]{0,0,9});
                    
                    newChunk.AddRange(new BasicHeader(0, 3).Encode());
                    newChunk.AddRange(new ChunkHeader0(0, data.Count, 20, 0).Encode());
                    newChunk.AddRange(data);
                    RTMPClient.stream.Write(newChunk.ToArray(), 0, newChunk.Count);
                }
                return;
            }
            
            throw new Exception("Not implemented");
        }
    }
}