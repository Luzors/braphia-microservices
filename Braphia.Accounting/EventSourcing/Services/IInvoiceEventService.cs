using Braphia.Accounting.EventSourcing.Aggregates;

namespace Braphia.Accounting.EventSourcing.Services
{
    public interface IInvoiceEventService
    {
        Task<Guid> CreateInvoiceFromLabTestAsync(int patientId, int insurerId, int labTestId, decimal amount, string description);
        Task ProcessPaymentAsync(Guid invoiceAggregateId, int insurerId, decimal paymentAmount, string paymentReference);
        Task<InvoiceAggregate?> GetInvoiceAsync(Guid invoiceAggregateId);
    }
}
