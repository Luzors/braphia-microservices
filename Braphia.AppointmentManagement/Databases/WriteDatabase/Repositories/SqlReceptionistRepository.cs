using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories
{
    public class SQLReceptionistRepository : IReceptionistRepository
    {
        private readonly DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public SQLReceptionistRepository(DBContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context must be of type WriteDbContext.");
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }
        public async Task<bool> AddReceptionistAsync(Receptionist receptionist)
        {
            if (receptionist == null)
                throw new ArgumentNullException(nameof(receptionist), "Receptionist cannot be null.");
            await _context.Receptionists.AddAsync(receptionist);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateReceptionistAsync(Receptionist receptionist)
        {
            if (receptionist == null)
                throw new ArgumentNullException(nameof(receptionist), "Receptionist cannot be null.");
            var existingReceptionist = await _context.Receptionists.FindAsync(receptionist.Id)
                ?? throw new ArgumentException($"Receptionist with ID {receptionist.Id} not found.");
            existingReceptionist.FirstName = receptionist.FirstName;
            existingReceptionist.LastName = receptionist.LastName;
            existingReceptionist.Email = receptionist.Email;

            _context.Receptionists.Update(existingReceptionist);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteReceptionistAsync(int receptionistId)
        {
            var receptionist = await GetReceptionistByIdAsync(receptionistId);
            if (receptionist == null) return false;
            _context.Receptionists.Remove(receptionist);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Receptionist> GetReceptionistByIdAsync(int receptionistId)
        {
            return await _context.Receptionists.FindAsync(receptionistId);
        }
        public async Task<IEnumerable<Receptionist>> GetAllReceptionistsAsync()
        {
            return await _context.Receptionists.ToListAsync() 
                   ?? throw new ArgumentException("No receptionists found in the database.");
        }
    }
}
