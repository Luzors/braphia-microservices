using Braphia.UserManagement.Models.ExternalOnly;

namespace Braphia.UserManagement.Events.ConsumedEvents.Prescriptions
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
