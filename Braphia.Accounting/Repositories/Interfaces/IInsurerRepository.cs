using Braphia.Accounting.Models;

namespace Braphia.Accounting.Repositories.Interfaces
{
    public interface IInsurerRepository
    {
        Task<bool> AddInsurerAsync(Insurer insurer);
        Task<bool> UpdateInsurerAsync(Insurer insurer);
        Task<bool> DeleteInsurerAsync(int insurerId);
        Task<Insurer?> GetInsurerByIdAsync(int insurerId);
        Task<IEnumerable<Insurer>> GetAllInsurersAsync();
    }
}
