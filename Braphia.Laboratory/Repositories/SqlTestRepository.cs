using Braphia.Laboratory.Database;
using Braphia.Laboratory.Events;
using Braphia.Laboratory.Enums;
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

        public async Task<bool> AddTestAsync(Test test, bool ignoreIdentity = false)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test), "Test cannot be null.");
            await _context.Test.AddAsync(test);
            if (ignoreIdentity)
                await _context.SaveChangesWithIdentityInsertAsync();
            else
                await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Test?> GetTestByIdAsync(int id)
        {
            return await _context.Test.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Test>> GetAllAsync()
        {
            return await _context.Test.ToListAsync();
        }

        public async Task<bool> UpdateTestAsync(Test test)
        {
            _context.Test.Update(test);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTestAsync(int testId)
        {
            var test = await _context.Test.FindAsync(testId);
            if (test == null)
                return false;
            _context.Test.Remove(test);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}