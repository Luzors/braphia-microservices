using Braphia.Laboratory.Models;

namespace Braphia.Laboratory.Repositories.Interfaces
{
    public interface ITestRepository
    {
        Task<bool> AddTestAsync(Test test);
        Task<bool> UpdateTestAsync(Test test);
        Task<bool> DeleteTestAsync(Guid testId);
        Task<Test?> GetTestByIdAsync(Guid testId);
        Task<IEnumerable<Test>> GetAllTestsAsync();
  
        
    }
}
