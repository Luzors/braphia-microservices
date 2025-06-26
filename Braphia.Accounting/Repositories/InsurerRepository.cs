using Braphia.Accounting.Database;
using Braphia.Accounting.Models;
using Braphia.Accounting.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Accounting.Repositories
{
    public class InsurerRepository : IInsurerRepository
    {
        private readonly AccountingDBContext _context;

        public InsurerRepository(AccountingDBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddInsurerAsync(Insurer insurer)
        {
            try
            {
                _context.Insurer.Add(insurer);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateInsurerAsync(Insurer insurer)
        {
            try
            {
                _context.Insurer.Update(insurer);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteInsurerAsync(int insurerId)
        {
            try
            {
                var insurer = await _context.Insurer.FindAsync(insurerId);
                if (insurer == null) return false;

                _context.Insurer.Remove(insurer);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }        public async Task<Insurer?> GetInsurerByIdAsync(int insurerId)
        {
            return await _context.Insurer
                .Include(i => i.Patients)
                .Include(i => i.Invoices)
                .FirstOrDefaultAsync(i => i.Id == insurerId);
        }

        public async Task<IEnumerable<Insurer>> GetAllInsurersAsync()
        {
            return await _context.Insurer
                .Include(i => i.Patients)
                .Include(i => i.Invoices)
                .ToListAsync();
        }
    }
}
