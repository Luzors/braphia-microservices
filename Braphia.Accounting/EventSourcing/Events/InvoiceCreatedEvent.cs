using System.ComponentModel.DataAnnotations;
using Braphia.Accounting.EventSourcing;

namespace Braphia.Accounting.EventSourcing.Events
{
    public class InvoiceCreatedEvent : BaseEvent
    {
        public int PatientId { get; set; }
        public int InsurerId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }

        public InvoiceCreatedEvent(int aggregateId, int version, int patientId, int insurerId, decimal amount, string description)
            : base(aggregateId, version)
        {
            EventType = "InvoiceCreated";
            PatientId = patientId;
            InsurerId = insurerId;
            Amount = amount;
            Description = description;
            Date = DateTime.UtcNow;
        }

        // For serialization/EF
        public InvoiceCreatedEvent() : base()
        {
            EventType = "InvoiceCreated";
        }
    }
}
