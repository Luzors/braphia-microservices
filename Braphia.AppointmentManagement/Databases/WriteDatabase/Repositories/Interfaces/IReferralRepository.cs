using Braphia.AppointmentManagement.Models;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces
{
    public interface IReferralRepository
    {
        Task<bool> AddReferralAsync(Referral referral);
        Task<bool> UpdateReferralAsync(Referral referral);
        Task<bool> DeleteReferralAsync(int referralId);
        Task<Referral> GetReferralByIdAsync(int referralId);
        Task<IEnumerable<Referral>> GetAllReferralsAsync();
        Task<IEnumerable<Referral>> GetReferralsByPatientIdAsync(int patientId);
    }
}
