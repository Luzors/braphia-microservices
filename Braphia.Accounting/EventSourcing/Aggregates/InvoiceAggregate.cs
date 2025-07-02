using System.ComponentModel.DataAnnotations;
using Braphia.Accounting.EventSourcing.Events;
using Braphia.Accounting.Events;

namespace Braphia.Accounting.EventSourcing.Aggregates
{
    public class InvoiceAggregate
    {
        [Key]
        public int Id { get; private set; }
        public int PatientId { get; private set; }
        public int InsurerId { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal AmountPaid { get; private set; }
        public decimal AmountOutstanding => TotalAmount - AmountPaid;
        public string Description { get; private set; } = string.Empty;
        public DateTime CreatedDate { get; private set; }
        public bool IsFullyPaid => AmountOutstanding <= 0;
        public int Version { get; private set; }

        private readonly List<BaseEvent> _uncommittedEvents = new();
        public IReadOnlyList<BaseEvent> UncommittedEvents => _uncommittedEvents.AsReadOnly();

        private InvoiceAggregate() { }

        // For creating new invoice
        public static InvoiceAggregate CreateNew(int patientId, int insurerId, decimal amount, string description)
        {
            // Use a temporary ID for the aggregate, will be replaced with actual DB ID
            int tempAggregateId = -1;
            var aggregate = new InvoiceAggregate();
            
            var createEvent = new InvoiceCreatedEvent(tempAggregateId, 1, patientId, insurerId, amount, description);
            aggregate.Apply(createEvent);
            aggregate._uncommittedEvents.Add(createEvent);

            return aggregate;
        }

        // For reconstructing from events
        public static InvoiceAggregate LoadFromHistory(IEnumerable<BaseEvent> events)
        {
            var aggregate = new InvoiceAggregate();
            
            foreach (var @event in events.OrderBy(e => e.Version))
            {
                aggregate.Apply(@event);
            }
            
            return aggregate;
        }

        public void ReceivePayment(int insurerId, decimal paymentAmount, string paymentReference)
        {
            if (insurerId != InsurerId)
                throw new InvalidOperationException($"Payment from insurer {insurerId} not allowed for invoice belonging to insurer {InsurerId}");
            
            if (paymentAmount <= 0)
                throw new ArgumentException("Payment amount must be positive");

            if (IsFullyPaid)
                throw new InvalidOperationException("Cannot process payment for a fully paid invoice");
            
            if (paymentAmount > AmountOutstanding)
                throw new InvalidOperationException($"Payment amount {paymentAmount:C} exceeds outstanding amount {AmountOutstanding:C}");

            var nextVersion = Version + 1;
            var paymentEvent = new PaymentReceivedEvent(Id, nextVersion, insurerId, paymentAmount, paymentReference);
            Apply(paymentEvent);
            _uncommittedEvents.Add(paymentEvent);
        }

        public void AdjustInvoiceAmount(int insurerId, decimal adjustmentAmount, string reason, string reference)
        {
            if (insurerId != InsurerId)
                throw new InvalidOperationException($"Amount adjustment from insurer {insurerId} not allowed for invoice belonging to insurer {InsurerId}");
            
            if (adjustmentAmount == 0)
                throw new ArgumentException("Adjustment amount cannot be zero");
            
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Adjustment reason is required");

            if (IsFullyPaid)
                throw new InvalidOperationException("Cannot adjust amount for a fully paid invoice");

            // Prevent negative total amounts
            if (TotalAmount + adjustmentAmount < 0)
                throw new InvalidOperationException($"Adjustment amount {adjustmentAmount:C} would result in negative total amount");

            var nextVersion = Version + 1;
            var adjustmentEvent = new InvoiceAmountAdjustedEvent(Id, nextVersion, adjustmentAmount, reason, reference, insurerId);
            Apply(adjustmentEvent);
            _uncommittedEvents.Add(adjustmentEvent);
        }

        public void MarkEventsAsCommitted()
        {
            _uncommittedEvents.Clear();
        }

        // Method to update the ID when a new aggregate has been saved
        public void SetId(int id)
        {
            Id = id;
        }

        private void Apply(BaseEvent @event)
        {
            switch (@event)
            {
                case InvoiceCreatedEvent created:
                    Id = created.AggregateId;
                    PatientId = created.PatientId;
                    InsurerId = created.InsurerId;
                    TotalAmount = created.Amount;
                    Description = created.Description;
                    CreatedDate = created.Date;
                    AmountPaid = 0;
                    Version = created.Version;
                    break;

                case PaymentReceivedEvent payment:
                    AmountPaid += payment.PaymentAmount;
                    Version = payment.Version;
                    break;

                case InvoiceAmountAdjustedEvent adjustment:
                    TotalAmount += adjustment.AdjustmentAmount;
                    Version = adjustment.Version;
                    break;

                default:
                    throw new NotSupportedException($"Event type {@event.GetType().Name} is not supported");
            }
        }

        public static IEnumerable<InvoiceAggregate> GetInvoicesForInsurer(IEnumerable<BaseEvent> events, int insurerId)
        {
            return events
                .Where(e => e is InvoiceCreatedEvent created && created.InsurerId == insurerId)
                .GroupBy(e => e.AggregateId)
                .Select(group => LoadFromHistory(events.Where(e => e.AggregateId == group.Key)))
                .Where(invoice => !invoice.IsFullyPaid); // Only return unpaid invoices
        }
    }
}
