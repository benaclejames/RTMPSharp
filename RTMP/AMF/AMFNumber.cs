using System;
using System.Linq;

namespace RTMP
{
    [AMFType(0x00)]
    public class AMFNumber : AMFType
    {
        public AMFNumber(double value) : base(value)
        {
        }

        public AMFNumber(ref byte[] bytes) : base(ref bytes)
        {
        }

        public override object Parse(ref byte[] bytes)
        {
            var number = bytes.Take(8);
            bytes = bytes.Skip(8).ToArray();
            return BitConverter.ToDouble(number.Reverse().ToArray(), 0);
        }

        public override byte[] Serialize()
        {
            return BitConverter.GetBytes((double)Value).Reverse().ToArray();
        }
    }
}