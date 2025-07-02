using Braphia.AppointmentManagement.Models;

namespace Braphia.UserManagement.Events.Physicians
{
    public class PhysicianRemovedEvent
    {
        public Physician Physician { get; set; }
        public PhysicianRemovedEvent() { }

        public PhysicianRemovedEvent(Physician physician)
        {
            Physician = physician ?? throw new ArgumentNullException(nameof(physician), "Physician cannot be null.");
        }
    }
}
