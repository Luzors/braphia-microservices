using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Events.Physicians
{
    public class PhysicianRegisteredEvent
    {
        public Physician Physician { get; set; }
        public PhysicianRegisteredEvent() { }

        public PhysicianRegisteredEvent(Physician physician)
        {
            Physician = physician ?? throw new ArgumentNullException(nameof(physician), "Physician cannot be null.");
        }
    }
}
