using Braphia.NotificationDispatcher.Models;

namespace Braphia.NotificationDispatcher.Repositories.Interfaces
{
    public interface ILaboratoryRepository
    {
        Task<bool> AddLaboratoryAsync(Laboratory laboratory);
        Task<bool> UpdateLaboratoryAsync(int laboratoryId, Laboratory updatedLaboratory);
        Task<bool> DeleteLaboratoryAsync(int laboratoryId);
        Task<Laboratory?> GetLaboratoryByIdAsync(int laboratoryId);
        Task<IEnumerable<Laboratory>> GetAllLaboratoriesAsync();
    }
}
