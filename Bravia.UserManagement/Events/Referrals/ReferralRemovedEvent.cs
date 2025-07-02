using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Events.Referrals
{
    public class ReferralRemovedEvent
    {
        public Referral Referral { get; set; }
        public ReferralRemovedEvent() { }

        public ReferralRemovedEvent(Referral referral)
        {
            Referral = referral ?? throw new ArgumentNullException(nameof(referral), "Referral cannot be null.");
        }
    }
}
