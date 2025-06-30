using Braphia.MedicalManagement.Database;
using Braphia.MedicalManagement.Events;
using Braphia.MedicalManagement.Models;
using Braphia.MedicalManagement.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.MedicalManagement.Repositories
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

        public async Task<Test> GetTestAsync(int id)
        {
            var test = await _context.Test.FindAsync(id);
            if (test == null)
                throw new KeyNotFoundException($"Test with ID {id} not found.");
            return test;
        }

        public async Task<IEnumerable<Test>> GetAllTestsAsync()
        {
            return await _context.Test.ToListAsync();
        }

        public async Task<bool> AddTestAsync(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test), "Test cannot be null.");

            await _context.Test.AddAsync(test);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to add test.");

            return true;
        }

        public async Task<bool> DeleteTestAsync(int id)
        {
            var test = await _context.Test.FindAsync(id);
            if (test == null)
                throw new KeyNotFoundException($"Test with ID {id} not found.");

            _context.Test.Remove(test);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to delete test.");

            return true;
        }

        public async Task<bool> UpdateTestAsync(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test), "Test cannot be null.");

            _context.Test.Update(test);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to update test.");
                        
            return true;
        }
    }
}
