using Braphia.UserManagement.Database;
using Braphia.UserManagement.Events.GeneralPracticioners;
using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.UserManagement.Repositories
{
    public class SqlGeneralPracticionerRepository : IGeneralPracticionerRepository
    {
        private DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        public SqlGeneralPracticionerRepository(DBContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<bool> AddGeneralPracticionerAsync(GeneralPracticioner generalPracticioner)
        {
            if (generalPracticioner == null)
                throw new ArgumentNullException(nameof(generalPracticioner), "GeneralPracticioner cannot be null.");
            await _context.GeneralPracticioner.AddAsync(generalPracticioner);
            await _context.SaveChangesAsync();
            await _publishEndpoint.Publish(new Message(new GeneralPracticionerRegisteredEvent(generalPracticioner)));

            return true;
        }

        public async Task<bool> DeleteGeneralPracticionerAsync(int generalPracticionerId)
        {
            var generalPracticioner = await _context.GeneralPracticioner.FirstOrDefaultAsync(gp => gp.Id == generalPracticionerId) ?? throw new ArgumentException($"GeneralPracticioner with ID {generalPracticionerId} not found.");
            _context.GeneralPracticioner.Remove(generalPracticioner);
            await _context.SaveChangesAsync();
            await _publishEndpoint.Publish(new Message(new GeneralPracticionerRemovedEvent(generalPracticioner)));
            return true;
        }

        public async Task<IEnumerable<GeneralPracticioner>> GetAllGeneralPracticionersAsync()
        {
            return await _context.GeneralPracticioner.ToListAsync();
        }

        public async Task<GeneralPracticioner?> GetGeneralPracticionerByIdAsync(int generalPracticionerId)
        {
            return await _context.GeneralPracticioner.FirstOrDefaultAsync(gp => gp.Id == generalPracticionerId);
        }

        public async Task<bool> UpdateGeneralPracticionerAsync(GeneralPracticioner generalPracticioner)
        {
            if (generalPracticioner == null)
                throw new ArgumentNullException(nameof(generalPracticioner), "GeneralPracticioner cannot be null.");
            var existingGeneralPracticioner = await _context.GeneralPracticioner.FirstOrDefaultAsync(gp => gp.Id == generalPracticioner.Id) ?? throw new ArgumentException($"GeneralPracticioner with ID {generalPracticioner.Id} not found.");
            existingGeneralPracticioner.FirstName = generalPracticioner.FirstName;
            existingGeneralPracticioner.LastName = generalPracticioner.LastName;
            existingGeneralPracticioner.Email = generalPracticioner.Email;
            existingGeneralPracticioner.PhoneNumber = generalPracticioner.PhoneNumber;
            await _context.SaveChangesAsync();
            await _publishEndpoint.Publish(new Message(new GeneralPracticionerModifiedEvent(existingGeneralPracticioner.Id, existingGeneralPracticioner)));
            return true;
        }
    }
}
