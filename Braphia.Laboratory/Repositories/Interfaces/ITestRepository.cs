using Braphia.Laboratory.Enums;
using Braphia.Laboratory.Models;

namespace Braphia.Laboratory.Repositories.Interfaces
{
    public interface ITestRepository
    {
        Task<bool> AddAsync(Test test);
        Task<Test?> GetByIdAsync(int id);
        Task<IEnumerable<Test>> GetAllAsync();
        Task UpdateAsync(Test test);
    }
}
