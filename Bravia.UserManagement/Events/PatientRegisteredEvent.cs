using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Events
{
    public class PatientRegisteredEvent
    {
        public Patient Patient { get; set; }
        public PatientRegisteredEvent() { }

        public PatientRegisteredEvent(Patient patient)
        {
            Patient = patient ?? throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
        }
    }
}
