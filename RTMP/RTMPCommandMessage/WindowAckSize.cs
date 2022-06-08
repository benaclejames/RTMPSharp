using System;
using System.Linq;

namespace RTMP.RTMPCommandMessage
{
    public class WindowAckSize : RTMPMessage
    {
        public WindowAckSize(int size) : base(0, 2, 0, 5, 0) => data = BitConverter.GetBytes(size).Reverse().ToArray();
    }
}