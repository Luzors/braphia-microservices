using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Events
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
