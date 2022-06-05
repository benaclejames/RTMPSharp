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
    }
}