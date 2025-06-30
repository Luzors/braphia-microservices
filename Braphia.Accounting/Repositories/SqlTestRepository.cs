using Braphia.Accounting.Database;
using Braphia.Accounting.Models;
using Braphia.Accounting.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Accounting.Repositories
{
    public class SqlTestRepository : ITestRepository
    {
        private readonly AccountingDBContext _context;

        public SqlTestRepository(AccountingDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> AddTestAsync(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test), "Test cannot be null.");

            await _context.Test.AddAsync(test);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Test?> GetTestByRootIdAsync(int rootId)
        {
            return await _context.Test.FirstOrDefaultAsync(t => t.RootId == rootId);
        }

        public async Task<IEnumerable<Test>> GetAllTestsAsync()
        {
            return await _context.Test.ToListAsync();
        }
    }
}
