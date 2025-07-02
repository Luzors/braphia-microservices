using Braphia.Pharmacy.Models.ExternalObjects;

namespace Braphia.Pharmacy.Events.ExternalEvents
{
    public class PrescriptionChangedEvent
    {
        public Prescription Prescription { get; set; }

        public PrescriptionChangedEvent() { }

        public PrescriptionChangedEvent(Prescription prescription)
        {
            Prescription = prescription;
        }
    }
}
