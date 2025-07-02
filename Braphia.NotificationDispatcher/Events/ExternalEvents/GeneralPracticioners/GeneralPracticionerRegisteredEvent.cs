using Braphia.NotificationDispatcher.Models;

namespace Braphia.NotificationDispatcher.Events.ExternalEvents.GeneralPracticioners
{
    public class GeneralPracticionerRegisteredEvent
    {
        public User GeneralPracticioner { get; set; }
        public GeneralPracticionerRegisteredEvent() { }

        public GeneralPracticionerRegisteredEvent(User generalPracticioner)
        {
            GeneralPracticioner = generalPracticioner ?? throw new ArgumentNullException(nameof(generalPracticioner), "GeneralPracticioner cannot be null.");
        }
    }
}
