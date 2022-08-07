using System;
using System.Collections.Generic;
using System.Linq;

namespace RTMP
{
    [AMFType(0x00)]
    public class AMFNumber : AMFType
    {
        public override object Parse(ref byte[] bytes)
        {
            var number = bytes.Take(8);
            bytes = bytes.Skip(8).ToArray();
            return BitConverter.ToDouble(number.Reverse().ToArray(), 0);
        }

        public override byte[] Serialize() => BitConverter.GetBytes((double)Value).Reverse().ToArray();

        public AMFNumber(double value) : base(value)
        {
        }
        
        public AMFNumber(ref byte[] bytes) : base(ref bytes)
        {
        }
    }
}