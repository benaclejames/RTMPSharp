using System;
using System.Linq;
using System.Text;
using RTMP.RTMPCommandMessage;

namespace RTMP
{
    public class AMFString : AMFType<string>
    {
        public override string Parse(ref byte[] bytes)
        {
            byte[] strLen = new byte[4];
            Array.Copy(bytes, 0, strLen, 2, 2);
            int len = BitConverter.ToInt32(strLen.Reverse().ToArray(), 0);
            bytes = bytes.Skip(2).ToArray();
            var str = Encoding.UTF8.GetString(bytes, 0, len);
            bytes = bytes.Skip(len).ToArray();
            return str;
        }

        public override byte[] Serialize(bool withKey = true)
        {
            byte[] strLen = BitConverter.GetBytes((short)Value.Length).Reverse().ToArray();
            var strBytes = Encoding.UTF8.GetBytes(Value);
            var bytes = new byte[strBytes.Length + (withKey ? 3 : 2)];
            if (withKey)
                bytes[0] = TypeByte;
            Array.Copy(strLen, 0, bytes, withKey ? 1 : 0, strLen.Length);
            Array.Copy(strBytes, 0, bytes, withKey ? 3 : 2, strBytes.Length);
            return bytes;
        }

        public AMFString(string value) : base(value)
        {
        }
        
        public AMFString(ref byte[] data) : base("")
        {
            if (data[0] == TypeByte)
                data = data.Skip(1).ToArray();
            
            Value = Parse(ref data);
        }
    }
}