using System.Collections.Generic;

namespace RTMP.RTMPCommandMessage
{
    public class ResultCommandMessage : CommandMessage
    {
        protected List<AMFType> amf = new List<AMFType>();

        public ResultCommandMessage(int transactionId)
        {
           amf.Add(new AMFString("_result"));
           amf.Add(new AMFNumber(transactionId));
        }

        public override byte[] Serialize()
        {
            data = AMF0.Serialize(amf);
            return base.Serialize();
        }
    }
}