using System;

namespace RTMP
{
    public interface AMFClass
    {
        Type Type { get; }
        object Parse(byte[] bytes, ref int offset);
    }
}