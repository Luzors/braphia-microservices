using Braphia.Accounting.Database;
using Braphia.Accounting.Models;
using Braphia.Accounting.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Accounting.Repositories
{
    public class SqlInvoiceRepository : IInvoiceRepository
    {
        private readonly AccountingDBContext _context;

        public SqlInvoiceRepository(AccountingDBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddInvoiceAsync(Invoice invoice)
        {
            try
            {
                _context.Invoice.Add(invoice);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateInvoiceAsync(Invoice invoice)
        {
            try
            {
                _context.Invoice.Update(invoice);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteInvoiceAsync(int invoiceId)
        {
            try
            {
                var invoice = await _context.Invoice.FindAsync(invoiceId);
                if (invoice == null) return false;

                _context.Invoice.Remove(invoice);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }        public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
        {
            return await _context.Invoice
                .FirstOrDefaultAsync(i => i.Id == invoiceId);
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            return await _context.Invoice
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByPatientIdAsync(int patientId)
        {
            return await _context.Invoice
                .Where(i => i.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByInsurerIdAsync(int insurerId)
        {
            return await _context.Invoice
                .Where(i => i.InsurerId == insurerId)
                .ToListAsync();
        }
    }
}
