using System;
using System.Collections.Generic;
using System.Linq;

namespace RTMP
{
    public class AMFMessage
    {
        public List<object> Objects = new List<object>();
        
        private static Dictionary<byte, Type> TypeMap = new Dictionary<byte, Type>()
        {
            {0x00, typeof(double)},
            {0x01, typeof(bool)},
            {0x02, typeof(string)},
            {0x03, typeof(object)}
        };
        
        public static AMFClass GetAMFClass(Type type)
        {
            // Get all classes that implement AMFClass
            var classes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes()).Where(t => typeof(AMFClass).IsAssignableFrom(t) && t.IsClass);
            
            // Cast all of these classes to AMFClass interface
            var amfClasses = classes.Select(t => (AMFClass)Activator.CreateInstance(t));
            
            // Return the first class that matches the type
            return amfClasses.FirstOrDefault(c => c.Type == type);
        }

        public static object Read(byte[] bytes, ref int offset, Type type = null)
        {
            AMFClass amfClass = GetAMFClass(type ?? TypeMap[bytes[offset]]);
            if (type == null)
                offset++;
            return amfClass.Parse(bytes, ref offset);
        }

        public AMFMessage(byte[] amfBytes, ref int offset)
        {
            Objects.Add(Read(amfBytes, ref offset));
            Objects.Add(Read(amfBytes, ref offset));
            Objects.Add(Read(amfBytes, ref offset));
        }
    }
}