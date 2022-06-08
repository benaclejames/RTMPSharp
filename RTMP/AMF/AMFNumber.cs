using System;
using System.Collections.Generic;
using System.Linq;

namespace RTMP
{
    public class AMFNumber : AMFType<double>
    {
        public override double Parse(ref byte[] bytes)
        {
            var number = bytes.Take(8);
            bytes = bytes.Skip(8).ToArray();
            return BitConverter.ToDouble(number.Reverse().ToArray(), 0);
        }

        public override byte[] Serialize(bool withKey = true)
        {
            var retArr = new List<byte>();
            if (withKey)
                retArr.Add(TypeByte);
            retArr.AddRange(BitConverter.GetBytes(Value).Reverse());
            return retArr.ToArray();
        }

        public AMFNumber(double value) : base(value)
        {
        }
        
        public AMFNumber(ref byte[] data) : base(0)
        {
            if (data[0] == TypeByte)
                data = data.Skip(1).ToArray();
            
            Value = Parse(ref data);
        }
    }
}