using System.Collections.Generic;

namespace RTMP.RTMPCommandMessage
{
    public class ConnectMessage : CommandMessage
    {
        public ConnectMessage()
        {
            List<byte> connectAmf = new List<byte>();
            connectAmf.Add(2);
            connectAmf.AddRange(new AMFString().Encode("_result"));
            connectAmf.Add(0);
            connectAmf.AddRange(new AMFNumber().Encode(1));
                    
            connectAmf.Add(3);
            connectAmf.AddRange(new AMFString().Encode("fmsVer"));
            connectAmf.Add(2);
            connectAmf.AddRange(new AMFString().Encode("FMS/3,0,1,123"));
            connectAmf.AddRange(new AMFString().Encode("capabilities"));
            connectAmf.Add(0);
            connectAmf.AddRange(new AMFNumber().Encode(31));
            connectAmf.AddRange(new byte[]{0,0,9});
                    
            connectAmf.Add(3);
            connectAmf.AddRange(new AMFString().Encode("level"));
            connectAmf.Add(2);
            connectAmf.AddRange(new AMFString().Encode("status"));
            connectAmf.AddRange(new AMFString().Encode("code"));
            connectAmf.Add(2);
            connectAmf.AddRange(new AMFString().Encode("NetConnection.Connect.Success"));
            connectAmf.AddRange(new AMFString().Encode("description"));
            connectAmf.Add(2);
            connectAmf.AddRange(new AMFString().Encode("Connection succeeded"));
            connectAmf.AddRange(new AMFString().Encode("objectEncoding"));
            connectAmf.Add(0);
            connectAmf.AddRange(new AMFNumber().Encode(0));
            connectAmf.AddRange(new byte[]{0,0,9});
            data = connectAmf.ToArray();
        }
    }
}