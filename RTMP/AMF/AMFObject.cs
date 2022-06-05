using System;
using System.Collections.Generic;

namespace RTMP
{
    public class AMFObject : AMFClass
    {
        public Type Type => typeof(object);

        public Dictionary<string, object> Values = new Dictionary<string, object>();
        private readonly AMFString _stringReader = new AMFString();

        public void ReadPair(byte[] bytes, ref int offset, string overrideName = null)
        {
            string key = overrideName ?? (string) _stringReader.Parse(bytes, ref offset);
            object value = AMFMessage.Read(bytes, ref offset);

            Values.Add(key, value);
        }

        public object Parse(byte[] bytes, ref int offset)
        {
            // While the next two bytes are not 0x00
            while (bytes[offset+2] != 0x09)
                ReadPair(bytes, ref offset);

            return Values;
        }
    }
}