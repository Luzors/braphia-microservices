using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events.Physicians
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
