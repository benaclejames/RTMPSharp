using System;
using System.Collections.Generic;
using System.Linq;

namespace RTMP
{
    public class AMFObject : AMFType<List<AMFProperty>>
    {
        public override List<AMFProperty> Parse(ref byte[] bytes)
        {
            while (bytes[2] != 0x09)
                Value.Add(new AMFProperty(ref bytes));

            // remove the last 3
            bytes = bytes.Skip(3).ToArray();
            return Value;
        }

        public override byte[] Serialize(bool withKey = true)
        {
            var retList = new List<byte>();
            retList.Add(0x03);
            foreach (var prop in Value)
            {
                retList.AddRange(prop.Key.Serialize(false));
                retList.AddRange(prop.Value.Serialize());
            }
            retList.AddRange(new byte[] {0x00, 0x00, 0x09});

            return retList.ToArray();
        }

        public AMFObject(List<AMFProperty> value) : base(value)
        {
        }
        
        public AMFObject(ref byte[] data) : base(new List<AMFProperty>())
        {
            data = data.Skip(1).ToArray();
            
            Value = Parse(ref data);
        }
    }
}