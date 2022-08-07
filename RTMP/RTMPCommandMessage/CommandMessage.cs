namespace RTMP.RTMPCommandMessage
{
    public class CommandMessage : ChunkHeader
    {
        public CommandMessage() : base(0, 3, 0, 20, 0)
        {
        }
    }
}