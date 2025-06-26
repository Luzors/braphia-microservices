using Braphia.AppointmentManagement.Models;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces
{
    public interface IReferralRepository
    {
        Task<bool> AddReferralAsync(Referral referral);
        Task<bool> UpdateReferralAsync(Referral referral);
        Task<bool> DeleteReferralAsync(Guid referralId);
        Task<Referral> GetReferralByIdAsync(Guid referralId);
        Task<IEnumerable<Referral>> GetAllReferralsAsync();
        Task<IEnumerable<Referral>> GetReferralsByPatientIdAsync(Guid patientId);
    }
}
