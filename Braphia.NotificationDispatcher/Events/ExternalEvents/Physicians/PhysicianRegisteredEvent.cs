using Braphia.NotificationDispatcher.Models;

namespace Braphia.NotificationDispatcher.Events.ExternalEvents.Physicians
{
    public class PhysicianRegisteredEvent
    {
        public User Physician { get; set; }
        public PhysicianRegisteredEvent() { }

        public PhysicianRegisteredEvent(User physician)
        {
            Physician = physician ?? throw new ArgumentNullException(nameof(physician), "Physician cannot be null.");
        }
    }
}
