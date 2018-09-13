using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Shared_Models
{
    public class Communication_Model
    {
        public readonly String EventName;
        public readonly DateTime DateReceived;
        public readonly Object Body;

        public Communication_Model(String EventName, Object Body)
        {
            this.EventName = EventName;
            this.DateReceived = DateTime.Now;
            this.Body = Body;
        }
    }
}
