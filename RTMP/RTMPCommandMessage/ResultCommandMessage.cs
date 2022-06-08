using System.Net.Sockets;

namespace RTMP.RTMPCommandMessage
{
    public class ResultCommandMessage : CommandMessage
    {
        protected AMF0 amf = new AMF0();

        public ResultCommandMessage(int transactionId)
        {
            amf.Add("_result");
            amf.Add((double)transactionId);
        }
        
        public override void Enqueue(NetworkStream stream)
        {
            data = AMF0.Serialize(amf.Objects);
            base.Enqueue(stream);
        }
    }
}