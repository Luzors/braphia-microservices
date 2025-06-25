namespace Braphia.UserManagement.Models
{
    public class Patient : User
    {
        public Patient() { }

        public Patient(string firstName, string lastName, string email, string phoneNumber)
            : base(firstName, lastName, email, phoneNumber)
        { }
        
        public IList<MedicalRecord> MedicalRecords { get; set; } = [];

        public IList<Referral> Referrals { get; set; } = [];
    }
}
