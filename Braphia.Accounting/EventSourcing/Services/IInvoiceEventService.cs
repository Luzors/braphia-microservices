using Braphia.Accounting.EventSourcing.Aggregates;

namespace Braphia.Accounting.EventSourcing.Services
{
    public interface IInvoiceEventService
    {
        Task<int> CreateInvoiceAsync(int patientId, int insurerId, int labTestId, decimal amount, string description);
        Task<bool> ProcessPaymentAsync(int invoiceAggregateId, int insurerId, decimal paymentAmount, string paymentReference);
        Task<InvoiceAggregate?> GetInvoiceAsync(int invoiceAggregateId);
        Task<IEnumerable<InvoiceAggregate>> GetInvoicesByInsurerAsync(int insurerId);
        Task<IEnumerable<BaseEvent>> GetPaymentEventsByInvoiceIdAsync(int invoiceAggregateId);
        Task<IEnumerable<InvoiceAggregate>> GetAllInvoicesAsync();
        Task<IEnumerable<InvoiceAggregate>> GetInvoicesByPatientIdAsync(int patientId);
    }
}
