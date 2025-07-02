using Braphia.NotificationDispatcher.Models;

namespace Braphia.NotificationDispatcher.Events.ExternalEvents.GeneralPracticioners
{
    public class GeneralPracticionerRemovedEvent
    {
        public User GeneralPracticioner { get; set; }
        public GeneralPracticionerRemovedEvent() { }

        public GeneralPracticionerRemovedEvent(User generalPracticioner)
        {
            GeneralPracticioner = generalPracticioner ?? throw new ArgumentNullException(nameof(generalPracticioner), "GeneralPracticioner cannot be null.");
        }
    }
}
