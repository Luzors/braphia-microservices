using Braphia.Accounting.Models;

namespace Braphia.Accounting.Events
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
