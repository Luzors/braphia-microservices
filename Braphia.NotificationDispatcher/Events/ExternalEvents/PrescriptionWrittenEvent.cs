using Braphia.NotificationDispatcher.Models.OutOfDb;

namespace Braphia.NotificationDispatcher.Events.ExternalEvents
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
