using System;

namespace RTMP
{
    public class IncompleteChunk
    {
        public int bytesWritten, maxSize;
        public byte[] bytes;
        
        public IncompleteChunk(int initialSize, int maxSize)
        {
            bytesWritten = 0;
            this.maxSize = maxSize;
            bytes = new byte[initialSize];
        }
        
        public void Write(byte[] data, int length)
        {
            if (bytesWritten + length > maxSize)
                throw new Exception("IncompleteChunk: maxSize exceeded");
            // Ensure we resize the array if necessary
            if (bytes.Length < bytesWritten + length)
            {
                byte[] newBytes = new byte[bytesWritten + length];
                Array.Copy(bytes, 0, newBytes, 0, bytesWritten);
                bytes = newBytes;
            }
            Array.Copy(data, 0, bytes, bytesWritten, length);
            bytesWritten += length;
        }
        
        public int WriteableBytes() { return maxSize - bytesWritten; }

        public bool IsWriteable()
        {
            return this.maxSize - (bytesWritten) > 0;
        }
    }
}