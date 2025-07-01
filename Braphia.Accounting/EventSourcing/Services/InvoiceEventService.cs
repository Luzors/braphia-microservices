using Braphia.Accounting.EventSourcing.Aggregates;
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
            _eventStoreRepository = eventStoreRepository;
            _logger = logger;
        }

        public async Task<Guid> CreateInvoiceFromLabTestAsync(int patientId, int insurerId, int labTestId, decimal amount, string description)
        {
            _logger.LogInformation("Creating invoice for patient {PatientId}, insurer {InsurerId}, test {LabTestId}, amount {Amount:C}",
                patientId, insurerId, labTestId, amount);

            var aggregate = InvoiceAggregate.CreateFromLabTest(patientId, insurerId, labTestId, amount, description);
            
            await _eventStoreRepository.SaveEventsAsync(aggregate.Id, aggregate.Events, 0);
            
            _logger.LogInformation("Created invoice aggregate {AggregateId} for patient {PatientId} and insurer {InsurerId}",
                aggregate.Id, patientId, insurerId);

            return aggregate.Id;
        }

        public async Task ProcessPaymentAsync(Guid invoiceAggregateId, int insurerId, decimal paymentAmount, string paymentReference)
        {
            _logger.LogInformation("Processing payment of {Amount:C} from insurer {InsurerId} for invoice {InvoiceAggregateId}",
                paymentAmount, insurerId, invoiceAggregateId);

            var aggregate = await _eventStoreRepository.GetAggregateAsync(invoiceAggregateId);
            if (aggregate == null)
            {
                throw new InvalidOperationException($"Invoice aggregate {invoiceAggregateId} not found");
            }

            var currentVersion = await GetCurrentVersionAsync(invoiceAggregateId);
            
            aggregate.ReceivePayment(insurerId, paymentAmount, paymentReference);
            
            await _eventStoreRepository.SaveEventsAsync(invoiceAggregateId, aggregate.Events, currentVersion);
            
            _logger.LogInformation("Processed payment for invoice {InvoiceAggregateId}, new outstanding amount: {Outstanding:C}",
                invoiceAggregateId, aggregate.AmountOutstanding);
        }

        public async Task<InvoiceAggregate?> GetInvoiceAsync(Guid invoiceAggregateId)
        {
            return await _eventStoreRepository.GetAggregateAsync(invoiceAggregateId);
        }

        private async Task<int> GetCurrentVersionAsync(Guid aggregateId)
        {
            var events = await _eventStoreRepository.GetEventsAsync(aggregateId);
            return events.Count();
        }
    }
} 