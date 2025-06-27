using Braphia.AppointmentManagement.Models;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<bool> AddPatientAsync(Patient patient);
        Task<bool> UpdatePatientAsync(Patient patient);
        Task<bool> DeletePatientAsync(int patientId);
        Task<Patient> GetPatientByIdAsync(int patientId);
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
     
    }
}
