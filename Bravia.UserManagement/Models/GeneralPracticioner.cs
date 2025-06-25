namespace Braphia.UserManagement.Models
{
    public class GeneralPracticioner : User
    {
        public IList<Referral> Referrals { get; set; } = [];

        public GeneralPracticioner() { }
        public GeneralPracticioner(string firstName, string lastName, string email, string phoneNumber)
            : base(firstName, lastName, email, phoneNumber)
        { }
    }
}
