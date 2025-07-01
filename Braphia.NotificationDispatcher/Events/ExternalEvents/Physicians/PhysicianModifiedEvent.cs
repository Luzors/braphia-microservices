using Braphia.NotificationDispatcher.Models;

namespace Braphia.NotificationDispatcher.Events.ExternalEvents.Physicians
{
    public class PhysicianModifiedEvent
    {
        public int PhysicianId { get; set; }
        public User Physician { get; set; }
        public PhysicianModifiedEvent() { }

        public PhysicianModifiedEvent(int physicianId, User physician)
        {
            PhysicianId = physicianId;
            Physician = physician ?? throw new ArgumentNullException(nameof(physician), "Physician cannot be null.");
        }
    }
}
