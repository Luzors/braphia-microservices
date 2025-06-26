using Braphia.AppointmentManagement.Models;
using Braphia.UserManagement.Enums;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces
{
    public interface IPhysicianRepository
    {
        Task<bool> AddPhysicianAsync(Physician physician);
        Task<bool> UpdatePhysicianAsync(Physician physician);
        Task<bool> DeletePhysicianAsync(Guid physicianId);
        Task<Physician> GetPhysicianByIdAsync(Guid physicianId);
        Task<IEnumerable<Physician>> GetAllPhysiciansAsync();
        Task<IEnumerable<Physician>> GetPhysiciansBySpecializationAsync(SpecializationEnum specialization);
    }
}
