using Braphia.UserManagement.Database;
using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Braphia.UserManagement.Repositories
{
    public class SqlPhysicianRepository : IPhysicianRepository
    {
        private DBContext _context;
        public SqlPhysicianRepository(DBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> AddPhysicianAsync(Physician physician)
        {
            if (physician == null) throw new ArgumentNullException(nameof(physician), "Physician cannot be null.");
            await _context.Physician.AddAsync(physician);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePhysicianAsync(int physicianId)
        {
            var physician = await _context.Physician.FindAsync(physicianId) ?? throw new InvalidOperationException($"Physician with ID {physicianId} not found.");
            _context.Physician.Remove(physician);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Physician>> GetAllPhysiciansAsync()
        {
            return await _context.Physician.ToListAsync();
        }

        public async Task<Physician?> GetPhysicianByIdAsync(int physicianId)
        {
            return await _context.Physician.FindAsync(physicianId);
        }

        public async Task<bool> UpdatePhysicianAsync(Physician physician)
        {
            if (physician == null) throw new ArgumentNullException(nameof(physician), "Physician cannot be null.");
            var existing = await _context.Physician.FindAsync(physician.Id) ?? throw new InvalidOperationException($"Physician with ID {physician.Id} not found.");
            _context.Entry(existing).CurrentValues.SetValues(physician);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
