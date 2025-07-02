namespace Braphia.UserManagement.Models
{
    public class Patient : User
    {
        public Patient() { }

        public Patient(string firstName, string lastName, string email, string phoneNumber, int gpId)
            : base(firstName, lastName, email, phoneNumber)
        {
            GeneralPracticionerId = gpId;
        }
        
        public IList<MedicalRecord> MedicalRecords { get; set; } = [];

        public IList<Referral> Referrals { get; set; } = [];

        public int? GeneralPracticionerId { get; set; }
        public GeneralPracticioner? GeneralPracticioner { get; set; }
    }
}
