namespace Braphia.AppointmentManagement.Models
{
    public class Referral
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public DateTime ReferralDate { get; set; } = DateTime.Now;
        public string Reason { get; set; } = null!;
    }
}