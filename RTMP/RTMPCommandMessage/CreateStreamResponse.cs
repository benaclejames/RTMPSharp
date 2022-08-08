namespace RTMP.RTMPCommandMessage
{
    public class CreateStreamResponse : ResultCommandMessage
    {
        public CreateStreamResponse(int transactionId) : base(transactionId)
        {
            amf.Add(new AMFNull());
            amf.Add(new AMFNumber(0));
        }
    }
}