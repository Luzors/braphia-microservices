using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Repositories.Interfaces
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
