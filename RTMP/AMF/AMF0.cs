using System;
using System.Collections.Generic;
using System.Linq;

namespace RTMP
{
    public class AMF0
    {
        public static List<AMFType> Decode(byte[] bytes)
        {
            // Get all types that implement the amftype attribute
            var typeAttributes = typeof(AMFType).Assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(AMFTypeAttribute), false).Length > 0);

            // Recursively find the type byte, then find the correct AMFType and decode it.
            var types = new List<AMFType>();
            
            while (bytes.Length > 0)
            {
                var type = typeAttributes.First(t => ((AMFTypeAttribute)t.GetCustomAttributes(typeof(AMFTypeAttribute), false)[0]).Type == bytes[0]);
                bytes = bytes.Skip(1).ToArray();
                // Create a new instance by passing the bytes to the constructor as a ref
                var args = new object[] { bytes };
                var amfType = (AMFType)Activator.CreateInstance(type, args);
                bytes = args[0] as byte[];
                types.Add(amfType);
            }
            return types;
        }
        
        public static byte[] Serialize(List<AMFType> amf)
          {
              var bytes = new List<byte>();
              foreach (var message in amf)
              {
                  // Get the AMFTypeAttribute from the type and append the byte
                  AMFTypeAttribute attr = message.GetType().GetCustomAttributes(false)[0] as AMFTypeAttribute;
                  bytes.Add(attr.Type);
                  bytes.AddRange(message.Serialize());
              }

              return bytes.ToArray();
          }
    }
}