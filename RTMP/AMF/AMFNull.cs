using System;

namespace RTMP
{
    public class AMFNull : AMFClass
    {
        public Type Type => typeof(Nullable);
        public object Parse(byte[] bytes, ref int offset)
        {
            return null;
        }
    }
}