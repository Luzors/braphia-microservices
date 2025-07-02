using Braphia.AppointmentManagement.Models;

namespace Braphia.UserManagement.Events.Patients
{
    public class PatientRemovedEvent
    {
        public Patient Patient { get; set; }
        public PatientRemovedEvent() { }

        public PatientRemovedEvent(Patient patient)
        {
            Patient = patient ?? throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
        }
    }
}
