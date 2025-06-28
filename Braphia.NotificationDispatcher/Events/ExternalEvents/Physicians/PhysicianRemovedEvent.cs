using Braphia.NotificationDispatcher.Models;

namespace Braphia.NotificationDispatcher.Events.ExternalEvents.Physicians
{
    public class PhysicianRemovedEvent
    {
        public User Physician { get; set; }
        public PhysicianRemovedEvent() { }

        public PhysicianRemovedEvent(User physician)
        {
            Physician = physician ?? throw new ArgumentNullException(nameof(physician), "Physician cannot be null.");
        }
    }
}
