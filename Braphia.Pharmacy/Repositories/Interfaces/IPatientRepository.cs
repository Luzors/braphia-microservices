using Braphia.Pharmacy.Models.ExternalObjects;

namespace Braphia.Pharmacy.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<bool> AddPatientAsync(Patient patient, bool ignoreIdentity = false);
        Task<bool> UpdatePatientAsync(int patientId, Patient patient);
        Task<bool> DeletePatientAsync(int patientId);
        Task<Patient?> GetPatientByIdAsync(int patientId);
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
    }
}
