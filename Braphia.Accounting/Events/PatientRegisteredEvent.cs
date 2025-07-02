using Braphia.Accounting.Models;

namespace Braphia.Accounting.Events
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
