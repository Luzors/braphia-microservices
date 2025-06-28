using Braphia.UserManagement.Database;
using Braphia.UserManagement.Events.Receptionists;
using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.UserManagement.Repositories
{
    public class SqlReceptionistRepository : IReceptionistRepository
    {
        private DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public SqlReceptionistRepository(DBContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<bool> AddReceptionistAsync(Receptionist receptionist)
        {
            if (receptionist == null) throw new ArgumentNullException(nameof(receptionist), "Receptionist cannot be null.");
            await _context.Receptionist.AddAsync(receptionist);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to add receptionist.");
            await _publishEndpoint.Publish(new Message(new ReceptionistRegisteredEvent(receptionist)));
            return true;
        }

        public async Task<bool> DeleteReceptionistAsync(int receptionistId)
        {
            var receptionist = await _context.Receptionist.FindAsync(receptionistId) ?? throw new KeyNotFoundException($"Receptionist with ID {receptionistId} not found.");
            _context.Receptionist.Remove(receptionist);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to delete receptionist.");
            await _publishEndpoint.Publish(new Message(new ReceptionistRemovedEvent(receptionist)));
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
            var existing = await _context.Receptionist.FindAsync(receptionist.Id) ?? throw new KeyNotFoundException($"Receptionist with ID {receptionist.Id} not found.");
            _context.Entry(existing).CurrentValues.SetValues(receptionist);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to update receptionist.");
            await _publishEndpoint.Publish(new Message(new ReceptionistModifiedEvent(receptionist.Id, receptionist)));
            return true;
        }
    }
}
