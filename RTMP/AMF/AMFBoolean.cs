using System;

namespace RTMP
{
    public class AMFBoolean : AMFClass
    {
        public Type Type => typeof(bool);
        public object Parse(byte[] bytes, ref int offset)
        {
            offset++;
            return bytes[offset-1] != 0x00;
        }
    }
}