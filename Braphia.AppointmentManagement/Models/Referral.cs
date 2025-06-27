namespace Braphia.AppointmentManagement.Models
{
    public class Referral
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime ReferralDate { get; set; } = DateTime.Now;
        public string Reason { get; set; } = null!;

        public Referral(int id, int patientId, string reason)
        {
            Id = id;
            PatientId = patientId;
            Reason = reason;
        }
    }
}