using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Repositories.Interfaces
{
    public interface IGeneralPracticionerRepository
    {
        Task<bool> AddGeneralPracticionerAsync(GeneralPracticioner generalPracticioner);
        Task<bool> UpdateGeneralPracticionerAsync(GeneralPracticioner generalPracticioner);
        Task<bool> DeleteGeneralPracticionerAsync(int generalPracticionerId);
        Task<GeneralPracticioner?> GetGeneralPracticionerByIdAsync(int generalPracticionerId);
        Task<IEnumerable<GeneralPracticioner>> GetAllGeneralPracticionersAsync();
    }
}
