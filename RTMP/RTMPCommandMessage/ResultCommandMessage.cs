﻿using System.Collections.Generic;

namespace RTMP.RTMPCommandMessage
{
    public class ResultCommandMessage : CommandMessage
    {
        public List<AMFType> amf = new List<AMFType>();

        protected ResultCommandMessage(int transactionId)
        {
            amf.Add(new AMFString("_result"));
            amf.Add(new AMFNumber(transactionId));
        }

        public override byte[] Serialize()
        {
            data = AMF0.Serialize(amf);
            return base.Serialize();
        }
    }
}