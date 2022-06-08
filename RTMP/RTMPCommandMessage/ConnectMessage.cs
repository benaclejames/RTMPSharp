using System.Collections.Generic;
using System.Net.Sockets;

namespace RTMP.RTMPCommandMessage
{
    public class ConnectMessage : ResultCommandMessage
    {
        public ConnectMessage() : base(1)
        {
        }
        
        public override void Enqueue(NetworkStream stream)
        {
            amf.Add(new List<AMFProperty>()
            {
                ("fmsVer", "FMS/3,0,1,123"),
                ("capabilities", (double)31),
                ("mode", "live"),
                ("objectEncoding", (double)0)
            });
            
            amf.Add(new List<AMFProperty>()
            {
                ("level", "status"),
                ("code", "NetConnection.Connect.Success"),
                ("description", "Connection succeeded"),
                ("objectEncoding", (double)0)
            });
            
            base.Enqueue(stream);
        }
    }
}