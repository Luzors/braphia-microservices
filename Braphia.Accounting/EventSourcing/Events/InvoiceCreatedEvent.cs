using Braphia.Accounting.EventSourcing;

namespace Braphia.Accounting.EventSourcing.Events
{
    public class InvoiceCreatedEvent : BaseEvent
    {
        public override string EventType => "InvoiceCreated";
        
        public Guid InvoiceAggregateId { get; private set; }
        public int PatientId { get; private set; }
        public int InsurerId { get; private set; }
        public int LabTestId { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; }
        public DateTime InvoiceDate { get; private set; }

        public InvoiceCreatedEvent(Guid invoiceAggregateId, int patientId, int insurerId, int labTestId, decimal amount, string description)
        {
            InvoiceAggregateId = invoiceAggregateId;
            PatientId = patientId;
            InsurerId = insurerId;
            LabTestId = labTestId;
            Amount = amount;
            Description = description;
            InvoiceDate = DateTime.UtcNow;
        }

        // For serialization
        private InvoiceCreatedEvent() { }
    }
}
