using System;
using System.Collections.Generic;
using System.Linq;

namespace RTMP
{
    [AMFType(0x03)]
    public class AMFObject : AMFType
    {
        
        
        public override object Parse(ref byte[] bytes)
        {
            List<(AMFString, AMFType)> obj = new List<(AMFString, AMFType)>();
            while (!(bytes[0] == 0 && bytes[1] == 0 && bytes[2] == 9))
            {
                var key = new AMFString(ref bytes);
                var typeAttributes = typeof(AMFType).Assembly.GetTypes()
                    .Where(t => t.GetCustomAttributes(typeof(AMFTypeAttribute), false).Length > 0);
                var typeByte = bytes[0];
                var type = typeAttributes.First(t =>
                    ((AMFTypeAttribute)t.GetCustomAttributes(typeof(AMFTypeAttribute), false)[0]).Type == typeByte);
                bytes = bytes.Skip(1).ToArray();
                var args = new object[] { bytes };
                var amfType = (AMFType)Activator.CreateInstance(type, args);
                bytes = args[0] as byte[];
                obj.Add((key, amfType));
            }
            bytes = bytes.Skip(3).ToArray();
            return obj;
        }

        public override byte[] Serialize()
        {
            var retList = new List<byte>();
            foreach (var prop in (List<(AMFString, AMFType)>)Value)
            {
                retList.AddRange(prop.Item1.Serialize());
                var type = prop.Item2.GetType().GetCustomAttributes(typeof(AMFTypeAttribute), false)[0] as AMFTypeAttribute;
                retList.Add(type.Type);
                retList.AddRange(prop.Item2.Serialize());
            }
            retList.AddRange(new byte[] {0x00, 0x00, 0x09});

            return retList.ToArray();
        }

        public AMFObject(List<(AMFString, AMFType)> value) : base(value)
        {
        }
        
        public AMFObject(ref byte[] data) : base(ref data)
        {
        }
    }
}