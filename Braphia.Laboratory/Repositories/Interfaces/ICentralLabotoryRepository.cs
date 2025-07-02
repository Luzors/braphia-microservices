using Braphia.Laboratory.Models;

namespace Braphia.Laboratory.Repositories.Interfaces
{
    public interface ICentralLabotoryRepository
    {
        Task<bool> AddCentralLaboratoryAsync(CentralLaboratory centralLaboratory);
        Task<bool> UpdateCentralLaboratoryAsync(CentralLaboratory centralLaboratory);
        Task<bool> DeleteCentralLaboratoryAsync(int id);
        Task<CentralLaboratory?> GetCentralLaboratoryByIdAsync(int id);
        Task<IEnumerable<CentralLaboratory>> GetAllCentralLaboratoriesAsync();
        
    }
}
