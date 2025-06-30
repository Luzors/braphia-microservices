using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<bool> AddPatientAsync(Patient Patient);
        Task<bool> UpdatePatientAsync(Patient Patient);
        Task<bool> DeletePatientAsync(int PatientId);
        Task<Patient?> GetPatientByIdAsync(int PatientId);
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
    }
}
