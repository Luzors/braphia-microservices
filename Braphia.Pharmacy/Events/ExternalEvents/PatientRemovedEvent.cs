using Braphia.Pharmacy.Models.ExternalObjects;

namespace Braphia.Pharmacy.Events.ExternalEvents
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
