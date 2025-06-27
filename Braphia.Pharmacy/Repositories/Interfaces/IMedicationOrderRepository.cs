namespace Braphia.Pharmacy.Repositories.Interfaces
{
    public interface IMedicationOrderRepository
    {
        Task<bool> CreateMedicationOrderAsync(Models.MedicationOrder medicationOrder);
        Task<bool> AddMedicationToMedicationOrderAsync(int medicationOrderId, Models.Medication medication, int amount);
        Task<bool> RemoveMedicationFromMedicationOrderAsync(int medicationOrderId, Models.Medication medication, int amount);
        Task<bool> CompleteMedicationOrderAsync(int medicationOrderId);
        Task<bool> UpdateMedicationOrderAsync(int medicationOrderId, Models.MedicationOrder updatedMedicationOrder);
        Task<bool> DeleteMedicationOrderAsync(int medicationOrderId);
        Task<Models.MedicationOrder?> GetMedicationOrderByIdAsync(int medicationOrderId);
        Task<IEnumerable<Models.MedicationOrder>> GetAllMedicationOrdersAsync();
    }
}
