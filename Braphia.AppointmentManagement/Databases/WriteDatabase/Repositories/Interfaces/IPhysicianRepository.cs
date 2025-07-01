using Braphia.AppointmentManagement.Models;
using Braphia.UserManagement.Enums;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces
{
    public interface IPhysicianRepository
    {
        Task<bool> AddPhysicianAsync(Physician physician, bool ignoreIdentity = false);
        Task<bool> UpdatePhysicianAsync(Physician physician, bool ignoreIdentity = false);
        Task<bool> DeletePhysicianAsync(int physicianId);
        Task<Physician> GetPhysicianByIdAsync(int physicianId);
        Task<IEnumerable<Physician>> GetAllPhysiciansAsync();
        Task<IEnumerable<Physician>> GetPhysiciansBySpecializationAsync(SpecializationEnum specialization);
    }
}
