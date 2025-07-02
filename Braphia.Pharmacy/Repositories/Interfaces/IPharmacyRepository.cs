namespace Braphia.Pharmacy.Repositories.Interfaces
{
    public interface IPharmacyRepository
    {
        Task<bool> AddPharmacyAsync(Models.Pharmacy pharmacy, bool ignoreIdentity = false);
        Task<bool> UpdatePharmacyAsync(int pharmacyId, Models.Pharmacy updatedPharmacy);
        Task<bool> DeletePharmacyAsync(int pharmacyId);
        Task<Models.Pharmacy?> GetPharmacyByIdAsync(int pharmacyId);
        Task<IEnumerable<Models.Pharmacy>> GetAllPharmaciesAsync();
    }
}
