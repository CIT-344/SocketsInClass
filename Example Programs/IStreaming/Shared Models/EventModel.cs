using System;
using System.Collections.Generic;
using System.Text;

namespace IStreaming.Shared_Models
{
    public class EventModel
    {
        public String EventName;
        public Action<Object> Work;
        public Type DataType;
    }
}
