using Braphia.AppointmentManagement.Models;

namespace Braphia.UserManagement.Events.Referrals
{
    public class ReferralSubmittedEvent
    {
        public Referral Referral { get; set; }
        public ReferralSubmittedEvent() { }

        public ReferralSubmittedEvent(Referral referral)
        {
            Referral = referral ?? throw new ArgumentNullException(nameof(referral), "Referral cannot be null.");
        }
    }
}
