using Braphia.UserManagement.Models.ExternalOnly;

namespace Braphia.UserManagement.Events.ConsumedEvents.Prescriptions
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
