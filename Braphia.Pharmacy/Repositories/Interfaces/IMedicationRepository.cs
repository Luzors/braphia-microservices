namespace Braphia.Pharmacy.Repositories.Interfaces
{
    public interface IMedicationRepository
    {
        Task<bool> AddMedicationAsync(Models.Medication medication, bool ignoreIdentity = false);
        Task<bool> UpdateMedicationAsync(int medicationId, Models.Medication updatedMedication);
        Task<bool> DeleteMedicationAsync(int medicationId);
        Task<Models.Medication?> GetMedicationByIdAsync(int medicationId);
        Task<IEnumerable<Models.Medication>> GetAllMedicationsAsync();
    }
}
