using System;

namespace RTMP
{
    // Make new attribute that dictates the amf type byte
    public class AMFTypeAttribute : Attribute
    {
        public AMFTypeAttribute(byte type)
        {
            Type = type;
        }

        public byte Type { get; }
    }

    public abstract class AMFType : RTMPSerializeable
    {
        public AMFType(object value)
        {
            Value = value;
        }

        public AMFType(ref byte[] data)
        {
            Value = Parse(ref data);
        }

        public object Value { get; }
        public abstract object Parse(ref byte[] bytes);
        public abstract byte[] Serialize();
    }
}