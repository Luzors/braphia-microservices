using Braphia.Laboratory.Models;

namespace Braphia.Laboratory.Events
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