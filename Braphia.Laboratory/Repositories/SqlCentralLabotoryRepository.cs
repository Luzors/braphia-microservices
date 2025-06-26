using Braphia.Laboratory.Database;
using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Laboratory.Repositories
{
    public class SqlCentralLabotoryRepository : ICentralLabotoryRepository
    {
        private readonly DBContext _context;
        public SqlCentralLabotoryRepository(DBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<bool> AddCentralLaboratoryAsync(CentralLaboratory centralLaboratory)
        {
            if (centralLaboratory == null)
                throw new ArgumentNullException(nameof(centralLaboratory), "Central Laboratory cannot be null.");
            await _context.CentralLaboratories.AddAsync(centralLaboratory);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteCentralLaboratoryAsync(Guid laboratoryId)
        {
            var laboratory = await _context.CentralLaboratories.FindAsync(laboratoryId);
            if (laboratory == null)
                throw new ArgumentException($"Central Laboratory with ID {laboratoryId} not found.");
            _context.CentralLaboratories.Remove(laboratory);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<CentralLaboratory>> GetAllCentralLaboratoriesAsync()
        {
            return await _context.CentralLaboratories.ToListAsync();
        }
        public async Task<CentralLaboratory?> GetCentralLaboratoryByIdAsync(Guid laboratoryId)
        {
            return await _context.CentralLaboratories.FirstOrDefaultAsync(l => l.Id == laboratoryId);
        }
        public async Task<bool> UpdateCentralLaboratoryAsync(CentralLaboratory centralLaboratory)
        {
            if (centralLaboratory == null)
                throw new ArgumentNullException(nameof(centralLaboratory), "Central Laboratory cannot be null.");
            var existing = await _context.CentralLaboratories.FindAsync(centralLaboratory.Id);
            if (existing == null)
                throw new ArgumentException($"Central Laboratory with ID {centralLaboratory.Id} not found.");
            existing.LaboratoryName = centralLaboratory.LaboratoryName;
            existing.Address = centralLaboratory.Address;
            existing.PhoneNumber = centralLaboratory.PhoneNumber;
            existing.Email = centralLaboratory.Email;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
