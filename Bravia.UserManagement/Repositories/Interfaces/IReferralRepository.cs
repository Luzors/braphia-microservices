using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Repositories.Interfaces
{
    public interface IReferralRepository
    {
        Task<bool> AddReferralAsync(Referral referral);
        Task<bool> UpdateReferralAsync(Referral referral);
        Task<bool> DeleteReferralAsync(int referralId);
        Task<Referral?> GetReferralByIdAsync(int referralId);
        Task<IEnumerable<Referral>> GetAllReferralsAsync();

        Task<IEnumerable<Referral>> GetReferralsByPatientIdAsync(int patientId);
        Task<IEnumerable<Referral>> GetReferralsByGeneralPracticionerIdAsync(int generalPracticionerId);
    }
}
