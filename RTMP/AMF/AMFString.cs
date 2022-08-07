using System;
using System.Linq;
using System.Text;
using RTMP.RTMPCommandMessage;

namespace RTMP
{
    [AMFType(0x02)]
    public class AMFString : AMFType
    {
        public override object Parse(ref byte[] bytes)
        {
            byte[] strLen = new byte[4];
            Array.Copy(bytes, 0, strLen, 2, 2);
            int len = BitConverter.ToInt32(strLen.Reverse().ToArray(), 0);
            bytes = bytes.Skip(2).ToArray();
            var str = Encoding.UTF8.GetString(bytes, 0, len);
            bytes = bytes.Skip(len).ToArray();
            return str;
        }

        public override byte[] Serialize()
        {
            byte[] strLen = BitConverter.GetBytes((short)((string)Value).Length).Reverse().ToArray();
            var strBytes = Encoding.UTF8.GetBytes((string)Value);
            var bytes = new byte[strBytes.Length + 2];
            Array.Copy(strLen, 0, bytes, 0, strLen.Length);
            Array.Copy(strBytes, 0, bytes, 2, strBytes.Length);
            return bytes;
        }
        
        public AMFString(string value) : base(value)
        {
        }
        
        public AMFString(ref byte[] bytes) : base(ref bytes)
        {
        }
    }
}