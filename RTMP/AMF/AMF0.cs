using System;
using System.Collections.Generic;
using System.Linq;
using RTMP.RTMPCommandMessage;

namespace RTMP
{
    public class AMF0
    {
        public List<AMFSerializeable> Objects = new List<AMFSerializeable>();
        
        public static Dictionary<byte, Type> TypeMap = new Dictionary<byte, Type>()
        {
            {0x00, typeof(double)},
            {0x01, typeof(bool)},
            {0x02, typeof(string)},
            {0x03, typeof(List<AMFProperty>)},
            {0x05, null}
        };
        
        public static AMFSerializeable GetLiteralAMFClass<T>(T initValue)
        {
            // Get all classes that implement AMFClass
            var classes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes()).Where(t => IsSubclassOfRawGeneric(typeof(AMFType<>), t) 
                                                          && t.BaseType.GenericTypeArguments.Length > 0 && t.BaseType.GenericTypeArguments[0] == initValue.GetType());

            // Return the first class that matches the type
            return (AMFSerializeable) Activator.CreateInstance(classes.First(), args: initValue);
        }
        
        public static AMFSerializeable GetLiteralAMFClass(object[] args)
        {
            var type = TypeMap[((byte[]) args[0])[0]];
            
            // Get the first class that implements this type as a template 
            var targClass = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes()).First(t => IsSubclassOfRawGeneric(typeof(AMFType<>), t)
                                                          && t.BaseType.GenericTypeArguments.Length > 0 && t.BaseType.GenericTypeArguments[0] == type);

            return (AMFSerializeable)Activator.CreateInstance(targClass, args);
        }
        
        static bool IsSubclassOfRawGeneric(Type generic, Type toCheck) {
            while (toCheck != null && toCheck != typeof(object)) {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur) {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static byte[] Serialize(List<AMFSerializeable> amf)
        {
            var bytes = new List<byte>();
            foreach (var message in amf)
                bytes.AddRange(message.Serialize());

            return bytes.ToArray();
        }

        public void Add<T>(T value)
        {
            Objects.Add(GetLiteralAMFClass(value));
        }

        public static AMF0 Decode(byte[] amfBytes)
        {
            int offset = 0;
            var amf = new AMF0();
            var args = new object[] { amfBytes };
            while (offset < ((byte[])args[0]).Length)
                try
                {
                    amf.Objects.Add(GetLiteralAMFClass(args));
                }
                catch (Exception e)
                {
                    break;
                }

            return amf;
        }

    }
}