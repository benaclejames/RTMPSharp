using System.Collections.Generic;

namespace RTMP.RTMPCommandMessage
{
    public class OnStatusResponse : CommandMessage
    {
        public List<AMFType> amf = new List<AMFType>();

        public OnStatusResponse(AMFObject info)
        {
            amf.Add(new AMFString("onStatus"));
            amf.Add(new AMFNumber(0));
            amf.Add(new AMFNull());
            amf.Add(info);
        }

        public override byte[] Serialize()
        {
            data = AMF0.Serialize(amf);
            return base.Serialize();
        }
    }
}