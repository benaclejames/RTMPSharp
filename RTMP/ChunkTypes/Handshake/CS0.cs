using System;

namespace RTMP
{
    // RTMP Version Request
    public class CS0
    {
        public const int Length = 1;

        public int RTMPVersion;

        public CS0(byte[] ver)
        {
            // We should've only got 1 byte for this so throw an exception if we didn't
            if (ver.Length != 1)
                throw new Exception("Invalid version length");

            RTMPVersion = ver[0];
        }
    }
}