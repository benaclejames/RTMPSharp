using System;
using System.Linq;

namespace RTMP.RTMPCommandMessage
{
    public class SetPeerBandwidth : RTMPMessage
    {
        public SetPeerBandwidth(int size, byte limitType) : base(0, 2, 0, 6, 0)
        {
            var tempData = BitConverter.GetBytes(size).Reverse().ToList();
            tempData.Add(limitType);
            data = tempData.ToArray();
        }
    }
}