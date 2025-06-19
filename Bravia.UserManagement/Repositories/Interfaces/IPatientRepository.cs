using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<bool> AddPatientAsync(Patient Patient);
        Task<bool> UpdatePatientAsync(Patient Patient);
        Task<bool> DeletePatientAsync(int PatientId);
        Task<Patient?> GetPatientByIdAsync(int PatientId);
        Task<IEnumerable<Patient>> GetAllPatientsAsync();

        Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByPatientIdAsync(int patientId);
        Task<bool> AddMedicalRecordAsync(int patientId, MedicalRecord medicalRecord);
        Task<bool> UpdateMedicalRecordAsync(int patientId, MedicalRecord medicalRecord);
        Task<bool> DeleteMedicalRecordAsync(int patientId, int medicalRecordId);
        Task<MedicalRecord?> GetMedicalRecordByIdAsync(int patientId, int medicalRecordId);
    }
}
