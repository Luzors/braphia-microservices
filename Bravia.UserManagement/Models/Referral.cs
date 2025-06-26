namespace Braphia.UserManagement.Models
{
    public class Referral
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        public int GeneralPracticionerId { get; set; }

        public DateTime ReferralDate { get; set; } = DateTime.Now;

        public string Reason { get; set; } = null!;

        public Referral() { }

        public Referral(int patientId, int generalPracticionerId, DateTime referralDate, string reason)
        {
            PatientId = patientId;
            GeneralPracticionerId = generalPracticionerId;
            ReferralDate = referralDate;
            Reason = reason;
        }
    }
}
