using System;
using System.Linq;
using System.Text;

namespace RTMP
{
    public class AMFString : AMFClass
    {
        public Type Type => typeof(string);

        public object Parse(byte[] bytes, ref int offset)
        {
            byte[] strLen = new byte[4];
            Array.Copy(bytes, offset, strLen, 2, 2);
            int len = BitConverter.ToInt32(strLen.Reverse().ToArray(), 0);
            var readOffset = offset + 2;
            offset += len + 2;
            return Encoding.UTF8.GetString(bytes, readOffset, len);
        }

        public byte[] Encode(string value)
        {
            // Get the length of the string in 2 bytes
            byte[] strLen = BitConverter.GetBytes((short)value.Length).Reverse().ToArray();
            var strBytes = Encoding.UTF8.GetBytes(value);
            var bytes = new byte[strBytes.Length + 2];
            Array.Copy(strLen, 0, bytes, 0, strLen.Length);
            Array.Copy(strBytes, 0, bytes, 2, strBytes.Length);
            return bytes;
        }
    }
}