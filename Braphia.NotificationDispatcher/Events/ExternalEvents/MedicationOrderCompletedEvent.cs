using Braphia.NotificationDispatcher.Models.OutOfDb;

namespace Braphia.NotificationDispatcher.Events.ExternalEvents
{
    public class MedicationOrderCompletedEvent
    {
        public MedicationOrder MedicationOrder { get; set; }
        public MedicationOrderCompletedEvent() { }

        public MedicationOrderCompletedEvent(MedicationOrder medicationOrder)
        {
            MedicationOrder = medicationOrder ?? throw new ArgumentNullException(nameof(medicationOrder), "MedicationOrder cannot be null.");
        }
    }
}
