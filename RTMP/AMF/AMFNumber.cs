using System;
using System.Linq;

namespace RTMP
{
    public class AMFNumber : AMFClass
    {
        public Type Type => typeof(double);
        public object Parse(byte[] bytes, ref int offset)
        {
            byte[] number = bytes.Skip(offset).Take(8).ToArray();
            offset += 8;
            return BitConverter.ToDouble(number.Reverse().ToArray(), 0);
        }
    }
}