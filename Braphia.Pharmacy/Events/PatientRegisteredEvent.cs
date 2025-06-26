using Braphia.Pharmacy.Models.ExternalObjects;

namespace Braphia.Pharmacy.Events
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
