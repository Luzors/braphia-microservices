using Braphia.AppointmentManagement.Models;

namespace Braphia.UserManagement.Events.Patients
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
