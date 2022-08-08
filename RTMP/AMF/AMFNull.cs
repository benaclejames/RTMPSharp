using System;

namespace RTMP
{
    [AMFType(0x05)]
    public class AMFNull : AMFType
    {
        public AMFNull() : base(null)
        {
        }

        public AMFNull(ref byte[] data) : base(ref data)
        {
        }

        public override object Parse(ref byte[] bytes)
        {
            return null;
        }

        public override byte[] Serialize()
        {
            return Array.Empty<byte>();
        }
    }
}