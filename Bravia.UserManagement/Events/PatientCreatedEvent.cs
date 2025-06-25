using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Events
{
    public class PatientCreatedEvent
    {
        public Patient Patient { get; set; }
        public PatientCreatedEvent() { }

        public PatientCreatedEvent(Patient patient)
        {
            Patient = patient ?? throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
        }
    }
}
