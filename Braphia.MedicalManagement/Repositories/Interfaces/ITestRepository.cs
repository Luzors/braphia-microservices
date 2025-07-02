using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Repositories.Interfaces
{
    public interface ITestRepository
    {
        Task<Test> GetTestAsync(int id);

        Task<IEnumerable<Test>> GetAllTestsAsync();

        Task<bool> AddTestAsync(Test test);

        Task<bool> DeleteTestAsync(int id);

        Task<bool> UpdateTestAsync(Test test);
    }
}
