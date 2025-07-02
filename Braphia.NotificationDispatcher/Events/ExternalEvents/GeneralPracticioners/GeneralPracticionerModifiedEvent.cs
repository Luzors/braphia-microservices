using Braphia.NotificationDispatcher.Models;

namespace Braphia.NotificationDispatcher.Events.ExternalEvents.GeneralPracticioners
{
    public class GeneralPracticionerModifiedEvent
    {
        public int GeneralPracticionerId { get; set; }
        public User GeneralPracticioner { get; set; }
        public GeneralPracticionerModifiedEvent() { }

        public GeneralPracticionerModifiedEvent(int generalPracticionerId, User generalPracticioner)
        {
            GeneralPracticionerId = generalPracticionerId;
            GeneralPracticioner = generalPracticioner ?? throw new ArgumentNullException(nameof(generalPracticioner), "General Practicioner cannot be null.");
        }
    }
}
