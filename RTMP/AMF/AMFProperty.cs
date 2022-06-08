using System;
using System.Collections.Generic;
using RTMP.RTMPCommandMessage;

namespace RTMP
{
    public class AMFProperty : AMFSerializeable
    {
        public AMFString Key;
        public AMFSerializeable Value;
        
        // Create implicit conversion from tuple
        public static implicit operator AMFProperty((string, object) tuple)
        {
            return new AMFProperty(tuple.Item1, tuple.Item2);
        }
        
        public AMFProperty(string key, object value)
        {
            Key = new AMFString(key);
            Value = AMF0.GetLiteralAMFClass(value);
        }

        public AMFProperty(ref byte[] data) => Parse(ref data);

        public byte[] Serialize(bool withKeyType = true)
        {
            var amfValueType = AMF0.GetLiteralAMFClass(Value);

            List<byte> result = new List<byte>(Key.Serialize(withKeyType));
            result.AddRange(amfValueType.Serialize());

            return result.ToArray();
        }

        public object GetValue => (Key, Value);

        public void Parse(ref byte[] data)
        {
            Key = new AMFString(ref data);
            
            var tempData = new object[] {data};
            Value = AMF0.GetLiteralAMFClass(tempData);
            data = (byte[])tempData[0];
        }
    }
}