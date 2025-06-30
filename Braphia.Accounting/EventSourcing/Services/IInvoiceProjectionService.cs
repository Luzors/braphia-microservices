using Braphia.Accounting.EventSourcing.Projections;

namespace Braphia.Accounting.EventSourcing.Services
{
    public interface IInvoiceProjectionService
    {
        Task<InsurerOutstandingBalance?> GetInsurerOutstandingBalanceAsync(int insurerId);
        Task<IEnumerable<InvoiceProjection>> GetInvoicesByInsurerAsync(int insurerId, bool? onlyOutstanding = null);
        Task<InvoiceProjection?> GetInvoiceProjectionAsync(Guid invoiceAggregateId);
        Task UpdateProjectionsFromEventAsync(IEvent @event);
    }
}
