using Braphia.UserManagement.Database;
using Braphia.UserManagement.Events.Physicians;
using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.UserManagement.Repositories
{
    public class SqlPhysicianRepository : IPhysicianRepository
    {
        private DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        public SqlPhysicianRepository(DBContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<bool> AddPhysicianAsync(Physician physician)
        {
            if (physician == null) throw new ArgumentNullException(nameof(physician), "Physician cannot be null.");
            await _context.Physician.AddAsync(physician);
            await _context.SaveChangesAsync();
            await _publishEndpoint.Publish(new Message(new PhysicianRegisteredEvent(physician)));
            return true;
        }

        public async Task<bool> DeletePhysicianAsync(int physicianId)
        {
            var physician = await _context.Physician.FindAsync(physicianId) ?? throw new InvalidOperationException($"Physician with ID {physicianId} not found.");
            _context.Physician.Remove(physician);
            await _context.SaveChangesAsync();
            await _publishEndpoint.Publish(new Message(new PhysicianRemovedEvent(physician)));
            return true;
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
            await _context.SaveChangesAsync();
            await _publishEndpoint.Publish(new Message(new PhysicianModifiedEvent(physician.Id, physician)));
            return true;
        }
    }
}
