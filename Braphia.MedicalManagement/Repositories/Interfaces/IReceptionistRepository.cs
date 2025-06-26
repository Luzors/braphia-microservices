using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Repositories.Interfaces
{
    public interface IReceptionistRepository
    {
        Task<bool> AddReceptionistAsync(Receptionist receptionist);
        Task<bool> UpdateReceptionistAsync(Receptionist receptionist);
        Task<bool> DeleteReceptionistAsync(int receptionistId);
        Task<Receptionist?> GetReceptionistByIdAsync(int receptionistId);
        Task<IEnumerable<Receptionist>> GetAllReceptionistsAsync();
    }
}
