using Braphia.NotificationDispatcher.Models;

namespace Braphia.NotificationDispatcher.Repositories.Interfaces
{
    public interface IPharmacyRepository
    {
        Task<bool> AddPharmacyAsync(Pharmacy pharmacy);
        Task<bool> UpdatePharmacyAsync(int pharmacyId, Pharmacy updatedPharmacy);
        Task<bool> DeletePharmacyAsync(int pharmacyId);
        Task<Pharmacy?> GetPharmacyByIdAsync(int pharmacyId);
        Task<IEnumerable<Pharmacy>> GetAllPharmaciesAsync();
    }
}
