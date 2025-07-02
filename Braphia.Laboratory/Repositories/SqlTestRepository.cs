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
            if (test == null)
            {
            Console.WriteLine("UpdateTestAsync: Provided test is null.");
            throw new ArgumentNullException(nameof(test), "Test cannot be null.");
            }

            try
            {
                Console.WriteLine("UpdateTestAsync: Provided test is null." + test);
            _context.Test.Update(test);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to update test.");
            Console.WriteLine($"UpdateTestAsync: Test with Id {test.Id} updated successfully.");
            return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
            Console.WriteLine($"UpdateTestAsync: Concurrency exception for Test Id {test.Id}: {ex.Message}");
            throw;
            }
            catch (Exception ex)
            {
            Console.WriteLine($"UpdateTestAsync: Error updating Test Id {test.Id}: {ex.Message}");
            throw;
            }
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