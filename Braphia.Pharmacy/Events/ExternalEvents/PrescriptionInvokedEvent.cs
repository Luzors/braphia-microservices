using Braphia.Pharmacy.Models.ExternalObjects;

namespace Braphia.Pharmacy.Events.ExternalEvents
{
    public class PrescriptionInvokedEvent
    {
        public Prescription Prescription { get; set; }

        public PrescriptionInvokedEvent() { }

        public PrescriptionInvokedEvent(Prescription prescription)
        {
            Prescription = prescription;
        }
    }
}
