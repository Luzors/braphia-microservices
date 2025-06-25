using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<bool> AddPatientAsync(Patient patient);
        Task<bool> UpdatePatientAsync(Patient patient);
        Task<bool> DeletePatientAsync(int patientId);
        Task<Patient?> GetPatientByIdAsync(int patientId);
        Task<Patient?> GetPatientByFullNameAsync(string firstName, string lastName);
        Task<IEnumerable<Patient>> GetAllPatientsAsync();

        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByPatientIdAsync(int patientId);
        Task<bool> AddMedicalRecordAsync(int patientId, MedicalRecord medicalRecord);
        Task<bool> UpdateMedicalRecordAsync(int patientId, MedicalRecord medicalRecord);
        Task<bool> DeleteMedicalRecordAsync(int patientId, int medicalRecordId);
        Task<MedicalRecord?> GetMedicalRecordByIdAsync(int patientId, int medicalRecordId);
    }
}
