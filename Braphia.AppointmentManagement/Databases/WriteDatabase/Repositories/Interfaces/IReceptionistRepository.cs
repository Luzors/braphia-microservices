using Braphia.AppointmentManagement.Models;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces
{
    public interface IReceptionistRepository
    {
        Task<bool> AddReceptionistAsync(Receptionist receptionist);
        Task<bool> UpdateReceptionistAsync(Receptionist receptionist);
        Task<bool> DeleteReceptionistAsync(int receptionistId);
        Task<Receptionist> GetReceptionistByIdAsync(int receptionistId);
        Task<IEnumerable<Receptionist>> GetAllReceptionistsAsync();
    }
}
