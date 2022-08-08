using System.Collections.Generic;
using RTMP.RTMPCommandMessage;

namespace RTMP
{
    public static class Chunk
    {
        public static void Decode(PacketHandler callingHandler, ChunkHeader header, byte[] data)
        {
            if (header.ChunkStreamId == 2) // Control Messages
            {
                ControlMessageHandler.Handle(callingHandler, data, header);
                return;
            }

            if (header.TypeId == 20) // AMF Encoding
            {
                var msg = AMF0.Decode(data);
                var commandType = (string)msg[0].Value;
                callingHandler.Parent.Log("Received Command: " + commandType);
                switch (commandType) 
                {
                    case "connect":
                        callingHandler.Parent.EnqueueSend(new WindowAckSize(5000000));
                        callingHandler.Parent.EnqueueSend(new SetPeerBandwidth(5000000, 1));
                        callingHandler.Parent.EnqueueSend(new SetChunkSize(5000));

                        var connectMsg = new ConnectMessage();
                        callingHandler.Parent.EnqueueSend(connectMsg);
                        break;
                    
                    case "createStream":
                        var createStreamMsg = new CreateStreamResponse((int)((double)msg[1].Value));
                        callingHandler.Parent.EnqueueSend(createStreamMsg);
                        break;
                    
                    case "publish":
                        var onPublishMessage = new OnStatusResponse(new AMFObject(new List<(AMFString, AMFType)>
                        {
                            (new AMFString("code"), new AMFString("NetStream.Publish.Start")),
                            (new AMFString("level"), new AMFString("status")),
                            (new AMFString("description"), new AMFString("Started publishing stream."))
                        }));
                        callingHandler.Parent.EnqueueSend(onPublishMessage);
                        break;
                }

                /* if ((string) msg.Objects[0] == "createStream")
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
                 }*/
            }
        }
    }
}