using Braphia.Accounting.EventSourcing;

namespace Braphia.Accounting.EventSourcing.Events
{
    public class PaymentReceivedEvent : BaseEvent
    {
        public int InsurerId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentReference { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }

        public PaymentReceivedEvent(int aggregateId, int version, int insurerId, decimal paymentAmount, string paymentReference)
            : base(aggregateId, version)
        {
            EventType = "PaymentReceived";
            InsurerId = insurerId;
            PaymentAmount = paymentAmount;
            PaymentReference = paymentReference;
            PaymentDate = DateTime.UtcNow;
        }

        // For serialization/EF
        public PaymentReceivedEvent() : base() 
        { 
            EventType = "PaymentReceived";
        }
    }
}
