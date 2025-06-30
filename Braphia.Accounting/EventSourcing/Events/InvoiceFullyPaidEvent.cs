using Braphia.Accounting.EventSourcing;

namespace Braphia.Accounting.EventSourcing.Events
{
    public class InvoiceFullyPaidEvent : BaseEvent
    {
        public override string EventType => "InvoiceFullyPaid";
        
        public Guid InvoiceAggregateId { get; private set; }
        public DateTime PaidInFullDate { get; private set; }

        public InvoiceFullyPaidEvent(Guid invoiceAggregateId)
        {
            InvoiceAggregateId = invoiceAggregateId;
            PaidInFullDate = DateTime.UtcNow;
        }

        // For serialization
        private InvoiceFullyPaidEvent() { }
    }
}
