using Braphia.AppointmentManagement.Models;

namespace Braphia.UserManagement.Events.Referrals
{
    public class ReferralModifiedEvent
    {
        public int ReferralId { get; set; }
        public Referral NewReferral { get; set; }
        public ReferralModifiedEvent() { }

        public ReferralModifiedEvent(int referralId, Referral newReferral)
        {
            ReferralId = referralId;
            NewReferral = newReferral ?? throw new ArgumentNullException(nameof(newReferral), "New referral cannot be null.");
        }
    }
}
