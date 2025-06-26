using Braphia.Accounting.Models;

namespace Braphia.Accounting.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<bool> AddInvoiceAsync(Invoice invoice);
        Task<bool> UpdateInvoiceAsync(Invoice invoice);
        Task<bool> DeleteInvoiceAsync(int invoiceId);
        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<IEnumerable<Invoice>> GetInvoicesByPatientIdAsync(int patientId);
        Task<IEnumerable<Invoice>> GetInvoicesByInsurerIdAsync(int insurerId);
    }
}
