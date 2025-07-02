using Braphia.Accounting.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Braphia.Accounting.Repositories.Interfaces
{
    public interface ITestRepository
    {
        Task<bool> AddTestAsync(Test test);
        Task<Test?> GetTestByRootIdAsync(int rootId);
        Task<IEnumerable<Test>> GetAllTestsAsync();
    }
}
