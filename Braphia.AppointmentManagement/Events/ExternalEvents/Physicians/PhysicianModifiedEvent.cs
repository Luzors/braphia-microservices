using Braphia.AppointmentManagement.Models;

namespace Braphia.UserManagement.Events.Physicians
{
    public class PhysicianModifiedEvent
    {
        public int PhysicianId { get; set; }
        public Physician Physician { get; set; }
        public PhysicianModifiedEvent() { }

        public PhysicianModifiedEvent(int physicianId, Physician physician)
        {
            PhysicianId = physicianId;
            Physician = physician ?? throw new ArgumentNullException(nameof(physician), "Physician cannot be null.");
        }
    }
}
