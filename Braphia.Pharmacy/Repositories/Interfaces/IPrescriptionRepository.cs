using Braphia.Pharmacy.Models.ExternalObjects;

namespace Braphia.Pharmacy.Repositories.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<bool> AddPrescriptionAsync(Prescription prescription);
        Task<bool> UpdatePrescriptionAsync(int prescriptionId, Prescription updatedPrescription);
        Task<bool> DeletePrescriptionAsync(int prescriptionId);
        Task<Prescription?> GetPrescriptionByIdAsync(int prescriptionId);
        Task<IEnumerable<Prescription>> GetAllPrescriptionsAsync();
        Task<IEnumerable<Prescription>> GetPrescriptionsByPatientIdAsync(int patientId);
    }
}
