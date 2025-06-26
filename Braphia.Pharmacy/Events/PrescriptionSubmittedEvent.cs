using Braphia.Pharmacy.Models.ExternalObjects;

namespace Braphia.Pharmacy.Events
{
    public class PrescriptionSubmittedEvent
    {
        public Prescription Prescription { get; set; }
        public PrescriptionSubmittedEvent() { }

        public PrescriptionSubmittedEvent(Prescription prescription)
        {
            Prescription = prescription ?? throw new ArgumentNullException(nameof(prescription), "Prescription cannot be null.");
        }
    }
}
