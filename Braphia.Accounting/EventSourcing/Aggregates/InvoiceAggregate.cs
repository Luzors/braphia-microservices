using Braphia.Accounting.EventSourcing.Events;

namespace Braphia.Accounting.EventSourcing.Aggregates
{
    public class InvoiceAggregate
    {
        public Guid Id { get; private set; }
        public int PatientId { get; private set; }
        public int InsurerId { get; private set; }
        public int LabTestId { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal AmountPaid { get; private set; }
        public decimal AmountOutstanding => TotalAmount - AmountPaid;
        public string Description { get; private set; } = string.Empty;
        public DateTime CreatedDate { get; private set; }
        public bool IsFullyPaid => AmountOutstanding <= 0;

        private readonly List<IEvent> _events = new();
        public IReadOnlyList<IEvent> Events => _events.AsReadOnly();

        // For creating new invoice
        public static InvoiceAggregate CreateFromLabTest(int patientId, int insurerId, int labTestId, decimal amount, string description)
        {
            var aggregate = new InvoiceAggregate();
            var createEvent = new InvoiceCreatedEvent(Guid.NewGuid(), patientId, insurerId, labTestId, amount, description);
            
            aggregate.Apply(createEvent);
            aggregate._events.Add(createEvent);
            
            return aggregate;
        }

        // For reconstructing from events
        public static InvoiceAggregate LoadFromEvents(IEnumerable<IEvent> events)
        {
            var aggregate = new InvoiceAggregate();
            
            foreach (var @event in events)
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
            
            if (paymentAmount > AmountOutstanding)
                throw new InvalidOperationException($"Payment amount {paymentAmount:C} exceeds outstanding amount {AmountOutstanding:C}");

            var paymentEvent = new PaymentReceivedEvent(Id, insurerId, paymentAmount, paymentReference);
            Apply(paymentEvent);
            _events.Add(paymentEvent);

            // Check if fully paid after this payment
            if (IsFullyPaid)
            {
                var fullyPaidEvent = new InvoiceFullyPaidEvent(Id);
                Apply(fullyPaidEvent);
                _events.Add(fullyPaidEvent);
            }
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        private void Apply(IEvent @event)
        {
            switch (@event)
            {
                case InvoiceCreatedEvent created:
                    Id = created.InvoiceAggregateId;
                    PatientId = created.PatientId;
                    InsurerId = created.InsurerId;
                    LabTestId = created.LabTestId;
                    TotalAmount = created.Amount;
                    Description = created.Description;
                    CreatedDate = created.InvoiceDate;
                    AmountPaid = 0;
                    break;

                case PaymentReceivedEvent payment:
                    AmountPaid += payment.PaymentAmount;
                    break;

                case InvoiceFullyPaidEvent fullyPaid:
                    // Already handled by AmountPaid logic
                    break;

                default:
                    throw new NotSupportedException($"Event type {@event.GetType().Name} is not supported");
            }
        }

        private InvoiceAggregate() { }
    }
}
