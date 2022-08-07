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
                var msg = AMF0.Decode(data);
                string commandType = (string)msg[0].Value;
                if (commandType == "connect")
                {
                    var winAckSize = new WindowAckSize(5000000);
                    var peerBandwidth = new SetPeerBandwidth(5000000, 1);
                    var setChunkSize = new SetChunkSize(5000);
                    var connectMsg = new ConnectMessage();

                    RTMPClient.EnqueueSend(winAckSize);
                    RTMPClient.EnqueueSend(peerBandwidth);
                    RTMPClient.EnqueueSend(setChunkSize);
                    RTMPClient.EnqueueSend(connectMsg);
                    return;
                }
                //var command = (string) msg.Objects[0].GetValue;
                /* Console.WriteLine("Received AMF0 Command: " + command);
                 if (command == "connect")
                 {
                     var winAckSize = new WindowAckSize(5000000);
                     var peerBandwidth = new SetPeerBandwidth(5000000, 1);
                     var setChunkSize = new SetChunkSize(5000);
                     var connectMsg = new ConnectMessage();
                     
                     winAckSize.Enqueue(RTMPClient.stream);
                     peerBandwidth.Enqueue(RTMPClient.stream);
                     setChunkSize.Enqueue(RTMPClient.stream);
                     connectMsg.Enqueue(RTMPClient.stream);
                 }*/

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