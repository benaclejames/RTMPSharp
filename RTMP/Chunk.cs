using System;
using System.Collections.Generic;
using System.Linq;
using RTMP.RTMPCommandMessage;

namespace RTMP
{
    public static class Chunk
    {
        public static void Decode(ChunkHeader header, byte[] data)
        {
            if (header.csid == 2) // Control Messages
            {
                ControlMessageHandler.Handle(data, header);
                return;
            }
            
            if (header.TypeID == 20) // AMF Encoding
            {
                int offset = 0;
                var msg = new AMFMessage(data, ref offset);
                Console.WriteLine("Recv AMF0 Message: " + (string) msg.Objects[0]);
                if ((string) msg.Objects[0] == "connect")
                {
                    List<byte> connectChunk = new List<byte>();
                    connectChunk.AddRange(new ChunkHeader(0, 2, 0, 4, 5, 0).Encode());
                    connectChunk.AddRange(BitConverter.GetBytes(5000000).Reverse());
                    RTMPClient.stream.Write(connectChunk.ToArray(), 0, connectChunk.Count);
                    
                    connectChunk.Clear();
                    connectChunk.AddRange(new ChunkHeader(0, 2, 0, 5, 6, 0).Encode());
                    connectChunk.AddRange(BitConverter.GetBytes(5000000).Reverse());
                    connectChunk.Add(1);
                    RTMPClient.stream.Write(connectChunk.ToArray(), 0, connectChunk.Count);
                    
                    connectChunk.Clear();
                    connectChunk.AddRange(new ChunkHeader(0, 2, 0, 4, 1, 0).Encode());
                    connectChunk.AddRange(BitConverter.GetBytes(5000).Reverse());
                    RTMPClient.stream.Write(connectChunk.ToArray(), 0, connectChunk.Count);

                    connectChunk.Clear();
                    
                    var connectMsg = new ConnectMessage().Encode();
                    RTMPClient.stream.Write(connectMsg, 0, connectMsg.Length);
                }

                if ((string) msg.Objects[0] == "createStream")
                {
                    List<byte> createStreamAMF = new List<byte>();
                    List<byte> createStreamChunk = new List<byte>();
                    createStreamAMF.Add(2);
                    createStreamAMF.AddRange(new AMFString().Encode("_result"));
                    createStreamAMF.Add(0);
                    createStreamAMF.AddRange(new AMFNumber().Encode((double)msg.Objects[1]));
                    createStreamAMF.Add(5);
                    createStreamAMF.Add(0);
                    createStreamAMF.AddRange(new AMFNumber().Encode(5));
                    createStreamChunk.AddRange(new ChunkHeader(0, 3, 0, createStreamAMF.Count, 20, 0).Encode());
                    createStreamChunk.AddRange(createStreamAMF);
                    RTMPClient.stream.Write(createStreamChunk.ToArray(), 0, createStreamChunk.Count);
                }

                if ((string) msg.Objects[0] == "publish")
                {
                    List<byte> publishAMF = new List<byte>();
                    List<byte> publishChunk = new List<byte>();
                    publishAMF.Add(2);
                    publishAMF.AddRange(new AMFString().Encode("onStatus"));
                    publishAMF.Add(0);
                    publishAMF.AddRange(new AMFNumber().Encode(0));
                    publishAMF.Add(5);

                    publishAMF.Add(3);
                    publishAMF.AddRange(new AMFString().Encode("level"));
                    publishAMF.Add(2);
                    publishAMF.AddRange(new AMFString().Encode("status"));
                    publishAMF.AddRange(new AMFString().Encode("code"));
                    publishAMF.Add(2);
                    publishAMF.AddRange(new AMFString().Encode("NetStream.Publish.Start"));
                    publishAMF.AddRange(new AMFString().Encode("description"));
                    publishAMF.Add(2);
                    publishAMF.AddRange(new AMFString().Encode("Start publishing"));
                    publishAMF.AddRange(new byte[] {0, 0, 9});

                    publishChunk.AddRange(new ChunkHeader(0, 3, 0, publishAMF.Count, 20, 0).Encode());
                    publishChunk.AddRange(publishAMF);
                    RTMPClient.stream.Write(publishChunk.ToArray(), 0, publishChunk.Count);
                    return;
                }
            }
        }
    }
}