using System;
using System.Collections.Generic;
using System.Text;

namespace IStreaming.Shared_Models
{
    public interface IEventStorage
    {
        List<EventModel> Events
        {
            get;
            set;
        }

        void AddEvent(EventModel Event);
        void GetEvent(String EventName);
    }
}
