namespace RTMP.RTMPCommandMessage
{
    public interface AMFSerializeable
    {
        byte[] Serialize(bool withKey = true);
        object GetValue { get; }
    }
}