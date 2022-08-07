using System;
using System.Linq;

namespace RTMP
{
    [AMFType(0x01)]
    public class AMFBoolean : AMFType
    {
        public override object Parse(ref byte[] bytes)
        {
            bool ret = bytes[0] != 0x00;
            bytes = bytes.Skip(1).ToArray();
            return ret;
        }

        public override byte[] Serialize() => (bool)Value ? new byte[]{0x01} : new byte[]{0x00};

        public AMFBoolean(bool value) : base(value)
        {
        }

        public AMFBoolean(ref byte[] bytes) : base(ref bytes)
        {
        }
    }
}