using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Repositories.Interfaces
{
    public interface IPhysicianRepository
    {
        Task<bool> AddPhysicianAsync(Physician physician);
        Task<bool> UpdatePhysicianAsync(Physician physician);
        Task<bool> DeletePhysicianAsync(int physicianId);
        Task<Physician?> GetPhysicianByIdAsync(int physicianId);
        Task<IEnumerable<Physician>> GetAllPhysiciansAsync();
    }
}
