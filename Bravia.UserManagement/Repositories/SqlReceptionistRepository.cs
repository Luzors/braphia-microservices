using Braphia.UserManagement.Database;
using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Braphia.UserManagement.Repositories
{
    public class SqlReceptionistRepository : IReceptionistRepository
    {
        private DBContext _context;

        public SqlReceptionistRepository(DBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> AddReceptionistAsync(Receptionist receptionist)
        {
            if (receptionist == null) throw new ArgumentNullException(nameof(receptionist), "Receptionist cannot be null.");
            await _context.Receptionist.AddAsync(receptionist);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to add receptionist.");
            return true;
        }

        public async Task<bool> DeleteReceptionistAsync(int receptionistId)
        {
            var receptionist = await _context.Receptionist.FindAsync(receptionistId);
            if (receptionist == null) throw new KeyNotFoundException($"Receptionist with ID {receptionistId} not found.");
            _context.Receptionist.Remove(receptionist);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to delete receptionist.");
            return true;
        }

        public async Task<IEnumerable<Receptionist>> GetAllReceptionistsAsync()
        {
            return await _context.Receptionist.ToListAsync();
        }

        public async Task<Receptionist?> GetReceptionistByIdAsync(int receptionistId)
        {
            return await _context.Receptionist.FindAsync(receptionistId);
        }

        public async Task<bool> UpdateReceptionistAsync(Receptionist receptionist)
        {
            if (receptionist == null) throw new ArgumentNullException(nameof(receptionist), "Receptionist cannot be null.");
            var existing = await _context.Receptionist.FindAsync(receptionist.Id);
            if (existing == null) throw new KeyNotFoundException($"Receptionist with ID {receptionist.Id} not found.");

            _context.Entry(existing).CurrentValues.SetValues(receptionist);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to update receptionist.");
            return true;
        }
    }
}
