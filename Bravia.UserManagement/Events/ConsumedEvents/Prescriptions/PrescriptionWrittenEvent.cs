using Braphia.UserManagement.Models.ExternalOnly;

namespace Braphia.UserManagement.Events.ConsumedEvents.Prescriptions
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
