using Braphia.Laboratory.Models;

namespace Braphia.Laboratory.Repositories.Interfaces
{
    public interface ICentralLabotoryRepository
    {
        Task<bool> AddCentralLaboratoryAsync(CentralLaboratory centralLaboratory);
        Task<bool> UpdateCentralLaboratoryAsync(CentralLaboratory centralLaboratory);
        Task<bool> DeleteCentralLaboratoryAsync(Guid centralLaboratoryId);
        Task<CentralLaboratory?> GetCentralLaboratoryByIdAsync(Guid centralLaboratoryId);
        Task<IEnumerable<CentralLaboratory>> GetAllCentralLaboratoriesAsync();
        
    }
}
