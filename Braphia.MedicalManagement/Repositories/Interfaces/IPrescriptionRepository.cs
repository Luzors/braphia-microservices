using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Repositories.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<Prescription> GetPrescriptionAsync(int id);

        Task<IEnumerable<Prescription>> GetAllPrescriptionsAsync();

        Task<bool> AddPrescriptionAsync(Prescription prescription);

        Task<bool> DeletePrescriptionAsync(string id);

        Task<bool> UpdatePrescriptionAsync(Prescription prescription);
    }
}
