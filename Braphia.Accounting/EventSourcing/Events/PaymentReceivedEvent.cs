using Braphia.Accounting.EventSourcing;

namespace Braphia.Accounting.EventSourcing.Events
{
    public class PaymentReceivedEvent : BaseEvent
    {
        public override string EventType => "PaymentReceived";
        
        public Guid InvoiceAggregateId { get; private set; }
        public int InsurerId { get; private set; }
        public decimal PaymentAmount { get; private set; }
        public string PaymentReference { get; private set; }
        public DateTime PaymentDate { get; private set; }

        public PaymentReceivedEvent(Guid invoiceAggregateId, int insurerId, decimal paymentAmount, string paymentReference)
        {
            InvoiceAggregateId = invoiceAggregateId;
            InsurerId = insurerId;
            PaymentAmount = paymentAmount;
            PaymentReference = paymentReference;
            PaymentDate = DateTime.UtcNow;
        }

        // For serialization
        private PaymentReceivedEvent() { }
    }
}
