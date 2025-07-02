using Braphia.Laboratory.Models;    

namespace Braphia.Laboratory.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<bool> AddPatientAsync(Patient Patient, bool ignoreIdentity = false);
        Task<bool> UpdatePatientAsync(Patient Patient, bool ignoreIdentity = false);
        Task<bool> DeletePatientAsync(int PatientId);
        Task<Patient?> GetPatientByIdAsync(int PatientId);
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
    }
}
