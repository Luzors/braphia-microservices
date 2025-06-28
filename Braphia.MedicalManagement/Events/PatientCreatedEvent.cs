using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events
{
    public class PatientCreatedEvent
    {
        public Patient Patient { get; set; }

        public PatientCreatedEvent() { }

        public PatientCreatedEvent(Patient patient)
        {
            Patient = patient;
        }
    }
}
