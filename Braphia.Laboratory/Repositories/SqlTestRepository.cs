using Braphia.Laboratory.Database;
using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Laboratory.Repositories
{
    public class SqlTestRepository : ITestRepository
    {
        private readonly DBContext _context;
        public SqlTestRepository(DBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<bool> AddTestAsync(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test), "Test cannot be null.");
            await _context.Tests.AddAsync(test);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateTestAsync(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test), "Test cannot be null.");
            var existing = await _context.Tests.FindAsync(test.Id);
            if (existing == null)
                throw new ArgumentException($"Test with ID {test.Id} not found.");
            existing.TestName = test.TestName;
            existing.TestType = test.TestType;
            existing.CreatedAt = test.CreatedAt;
            existing.FinishedAt = test.FinishedAt;
            existing.TestStatus = test.TestStatus;
            existing.Description = test.Description;
            existing.CentralLaboratoryId = test.CentralLaboratoryId;
            existing.AppointmentId = test.AppointmentId;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteTestAsync(Guid testId)
        {
            var test = await _context.Tests.FindAsync(testId);
            if (test == null)
                throw new ArgumentException($"Test with ID {testId} not found.");
            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Test?> GetTestByIdAsync(Guid testId)
        {
            return await _context.Tests.FirstOrDefaultAsync(t => t.Id == testId);
        }
        public async Task<IEnumerable<Test>> GetAllTestsAsync()
        {
            return await _context.Tests.ToListAsync();
        }
       
    }
}
