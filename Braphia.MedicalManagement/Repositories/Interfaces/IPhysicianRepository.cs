using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Repositories.Interfaces
{
    public interface IPhysicianRepository
    {
        Task<bool> AddPhysicianAsync(Physician Physician);
        Task<bool> UpdatePhysicianAsync(Physician Physician);
        Task<bool> DeletePhysicianAsync(int PhysicianId);
        Task<Physician?> GetPhysicianByIdAsync(int PhysicianId);
        Task<IEnumerable<Physician>> GetAllPhysiciansAsync();
    }
}
