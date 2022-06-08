using System;
using System.Linq;

namespace RTMP
{
    public class AMFBoolean : AMFType<bool>
    {
        public override bool Parse(ref byte[] bytes)
        {
            bool ret = bytes[0] != 0x00;
            bytes = bytes.Skip(1).ToArray();
            return ret;
        }

        public override byte[] Serialize(bool withKey = true)
        {
            return Value ? new byte[]{0x01, 0x01} : new byte[]{0x01, 0x00};
        }

        public AMFBoolean(bool value) : base(value)
        {
        }
        
        public AMFBoolean(ref byte[] data) : base(false)
        {
            if (data[0] == TypeByte)
                data = data.Skip(1).ToArray();
            
            Value = Parse(ref data);
        }
    }
}