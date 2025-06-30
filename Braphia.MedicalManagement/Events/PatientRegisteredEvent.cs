using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events
{
    public class PatientRegisteredEvent
    {
        public Patient Patient { get; set; }

        public PatientRegisteredEvent() { }

        public PatientRegisteredEvent(Patient patient)
        {
            Patient = patient;
        }
    }
}
