using System;
using System.Linq;
using RTMP.RTMPCommandMessage;

namespace RTMP
{
    public abstract class AMFType<T> : AMFSerializeable
    {
        public Type Type { get; }
        public byte TypeByte { get; }
        public T Value { get; protected set; }
        public AMFType(T value)
        {
            Type = typeof(T);
            TypeByte = AMF0.TypeMap.First(t => t.Value == typeof(T)).Key;
            Value = value;
        }

        public AMFType(ref byte[] data)
        {
            var info = AMF0.TypeMap[data[0]];
            Type = info;
            TypeByte = data[0];

            // Create an instance of that class
            Value = Parse(ref data);
        }
        public abstract T Parse(ref byte[] bytes);
        public abstract byte[] Serialize(bool withKey = true);
        public object GetValue => Value;
    }
}