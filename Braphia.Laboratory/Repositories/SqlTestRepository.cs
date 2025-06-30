using Braphia.Laboratory.Database;
using Braphia.Laboratory.Events;
using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Infrastructure.Messaging;

namespace Braphia.Laboratory.Repositories
{
    public class SqlTestRepository : ITestRepository
    {
        private readonly DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public SqlTestRepository(DBContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<bool> AddAsync(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test), "Test cannot be null.");
            await _context.Test.AddAsync(test);
            await _context.SaveChangesAsync();

            // Patient created event
            await _publishEndpoint.Publish(new Message(
                messageType: "TestCompleted",
                data: new TestCompletedEvent(test)
            ));

            return true;
        }

        public async Task<Test?> GetByIdAsync(int id)
        {
            return await _context.Test.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Test>> GetAllAsync()
        {
            return await _context.Test.ToListAsync();
        }

        public async Task UpdateAsync(Test test)
        {
            _context.Test.Update(test);
            await _context.SaveChangesAsync();
        }
    }
}
