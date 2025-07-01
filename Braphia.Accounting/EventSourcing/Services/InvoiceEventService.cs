using Braphia.Accounting.EventSourcing.Aggregates;
using Braphia.Accounting.EventSourcing.Events;
using Braphia.Accounting.EventSourcing.Repositories;

namespace Braphia.Accounting.EventSourcing.Services
{
    public class InvoiceEventService : IInvoiceEventService
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly ILogger<InvoiceEventService> _logger;

        public InvoiceEventService(
            IEventStoreRepository eventStoreRepository,
            ILogger<InvoiceEventService> logger)
        {
            _eventStoreRepository = eventStoreRepository ?? throw new ArgumentNullException(nameof(eventStoreRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> CreateInvoiceAsync(int patientId, int insurerId, int labTestId, decimal amount, string description)
        {
            if (patientId <= 0)
                throw new ArgumentException("Patient ID must be positive", nameof(patientId));

            if (insurerId <= 0)
                throw new ArgumentException("Insurer ID must be positive", nameof(insurerId));

            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            try
            {
                var invoice = InvoiceAggregate.CreateNew(patientId, insurerId, labTestId, amount, description);
                
                var aggregateId = await _eventStoreRepository.SaveEventsAsync(invoice.UncommittedEvents, invoice.Id);
                
                // Update the ID of the aggregate to match the real ID returned from the repository
                invoice.SetId(aggregateId);
                invoice.MarkEventsAsCommitted(); // Clear the uncommitted events as they've been saved
                
                _logger.LogInformation("Created new invoice with ID {InvoiceId} for patient {PatientId} and insurer {InsurerId}", 
                    aggregateId, patientId, insurerId);
                
                return aggregateId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice for patient {PatientId}", patientId);
                throw;
            }
        }

        public async Task<InvoiceAggregate?> GetInvoiceAsync(int invoiceAggregateId)
        {
            if (invoiceAggregateId <= 0)
                throw new ArgumentException("Invoice ID must be positive", nameof(invoiceAggregateId));

            try
            {
                return await _eventStoreRepository.GetAggregateAsync(invoiceAggregateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting invoice {InvoiceId}", invoiceAggregateId);
                throw;
            }
        }

        public async Task<IEnumerable<InvoiceAggregate>> GetInvoicesByInsurerAsync(int insurerId)
        {
            if (insurerId <= 0)
                throw new ArgumentException("Insurer ID must be positive", nameof(insurerId));

            try
            {
                var events = await _eventStoreRepository.GetEventsByInsurerIdAsync(insurerId);
                
                // Group events by aggregate ID and reconstruct invoices
                var invoices = InvoiceAggregate.GetInvoicesForInsurer(events, insurerId);
                
                return invoices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting invoices for insurer {InsurerId}", insurerId);
                throw;
            }
        }

        public async Task<bool> ProcessPaymentAsync(int invoiceAggregateId, int insurerId, decimal paymentAmount, string paymentReference)
        {
            if (invoiceAggregateId <= 0)
                throw new ArgumentException("Invoice ID must be positive", nameof(invoiceAggregateId));

            if (insurerId <= 0)
                throw new ArgumentException("Insurer ID must be positive", nameof(insurerId));

            if (paymentAmount <= 0)
                throw new ArgumentException("Payment amount must be positive", nameof(paymentAmount));

            try
            {
                var invoice = await _eventStoreRepository.GetAggregateAsync(invoiceAggregateId);
                
                if (invoice == null)
                    throw new InvalidOperationException($"Invoice with ID {invoiceAggregateId} not found");

                invoice.ReceivePayment(insurerId, paymentAmount, paymentReference);
                
                await _eventStoreRepository.SaveEventsAsync(invoice.UncommittedEvents, invoice.Id);
                invoice.MarkEventsAsCommitted(); // Clear the uncommitted events as they've been saved
                
                _logger.LogInformation("Processed payment of {Amount:C} for invoice {InvoiceId} from insurer {InsurerId}",
                    paymentAmount, invoiceAggregateId, insurerId);
                
                return true;
            }
            catch (Exception ex) when (ex is not InvalidOperationException && ex is not ArgumentException)
            {
                _logger.LogError(ex, "Error processing payment for invoice {InvoiceId}", invoiceAggregateId);
                throw;
            }
        }

        public async Task<IEnumerable<InvoiceAggregate>> GetAllInvoicesAsync()
        {
            try
            {
                var allEvents = await _eventStoreRepository.GetEventsAsync();
                
                // Group events by aggregate ID
                var aggregateIds = allEvents
                    .Where(e => e is InvoiceCreatedEvent)
                    .Select(e => e.AggregateId)
                    .Distinct();

                // Reconstruct each aggregate
                var invoices = new List<InvoiceAggregate>();
                foreach (var aggregateId in aggregateIds)
                {
                    var events = allEvents.Where(e => e.AggregateId == aggregateId);
                    var invoice = InvoiceAggregate.LoadFromHistory(events);
                    invoices.Add(invoice);
                }
                
                return invoices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all invoices");
                throw;
            }
        }

        public async Task<IEnumerable<InvoiceAggregate>> GetInvoicesByPatientIdAsync(int patientId)
        {
            if (patientId <= 0)
                throw new ArgumentException("Patient ID must be positive", nameof(patientId));

            try
            {
                // Get all events
                var allEvents = await _eventStoreRepository.GetEventsAsync();
                
                // Find InvoiceCreatedEvents for this patient
                var invoiceCreatedEvents = allEvents
                    .OfType<InvoiceCreatedEvent>()
                    .Where(e => e.PatientId == patientId);
                
                // Group by aggregate ID
                var aggregateIds = invoiceCreatedEvents.Select(e => e.AggregateId).Distinct();
                
                // Reconstruct each aggregate
                var invoices = new List<InvoiceAggregate>();
                foreach (var aggregateId in aggregateIds)
                {
                    var events = allEvents.Where(e => e.AggregateId == aggregateId);
                    var invoice = InvoiceAggregate.LoadFromHistory(events);
                    invoices.Add(invoice);
                }
                
                return invoices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices for patient {PatientId}", patientId);
                throw;
            }
        }
    }
}
