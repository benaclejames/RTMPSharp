namespace RTMP
{
    public interface RTMPSerializeable
    {
        byte[] Serialize();
        object Parse(ref byte[] bytes);
    }
}