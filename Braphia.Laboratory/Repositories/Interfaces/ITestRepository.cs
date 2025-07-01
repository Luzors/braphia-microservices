using Braphia.Laboratory.Enums;
using Braphia.Laboratory.Models;

namespace Braphia.Laboratory.Repositories.Interfaces
{
    public interface ITestRepository
    {
        Task<bool> AddTestAsync(Test test);
        Task<bool> UpdateTestAsync(Test test);
        Task<bool> DeleteTestAsync(int testId);
        Task<Test?> GetTestByIdAsync(int testId);
        Task<IEnumerable<Test>> GetAllAsync();
    }
}
