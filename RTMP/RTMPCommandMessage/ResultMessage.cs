using System.Collections.Generic;

namespace RTMP.RTMPCommandMessage
{
    public class ResultMessage : CommandMessage
    {
        public ResultMessage(List<byte> data1, List<byte> data2)
        {
            List<byte> connectAmf = new List<byte>();
            connectAmf.Add(2);
            connectAmf.AddRange(new AMFString().Encode("_result"));
            connectAmf.Add(0);
            connectAmf.AddRange(new AMFNumber().Encode(1));
            
            if (data1 != null)
                connectAmf.AddRange(data1);
        }
    }
}