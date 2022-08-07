using System.Collections.Generic;

namespace RTMP.RTMPCommandMessage
{
    public class ConnectMessage : ResultCommandMessage
    {
        public ConnectMessage() : base(1)
        {
            amf.AddRange(new List<AMFType>
            {
                new AMFObject(new List<(AMFString, AMFType)>
                {
                    (new AMFString("fmsVer"), new AMFString("FMS/3,0,1,123")),
                    (new AMFString("capabilities"), new AMFNumber(31)),
                    (new AMFString("mode"), new AMFString("live")),
                    (new AMFString("objectEncoding"), new AMFNumber(0))
                }),
                new AMFObject(new List<(AMFString, AMFType)>
                {
                    (new AMFString("level"), new AMFString("status")),
                    (new AMFString("code"), new AMFString("NetConnection.Connect.Success")),
                    (new AMFString("description"), new AMFString("Connection succeeded")),
                    (new AMFString("objectEncoding"), new AMFNumber(0))
                })
            });
        }
    }
}