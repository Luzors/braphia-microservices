using Braphia.Pharmacy.Models.ExternalObjects;

namespace Braphia.Pharmacy.Events.ExternalEvents
{
    public class PrescriptionWrittenEvent
    {
        public Prescription Prescription { get; set; }

        public PrescriptionWrittenEvent() { }

        public PrescriptionWrittenEvent(Prescription prescription)
        {
            Prescription = prescription;
        }
    }
}
