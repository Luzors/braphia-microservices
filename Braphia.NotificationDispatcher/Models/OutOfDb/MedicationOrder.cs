namespace Braphia.NotificationDispatcher.Models.OutOfDb
{
    public class MedicationOrder
    {
        public int Id { get; set; }
        public int PatientId { get; set; } // Get it with RootId + usertype = patient
        public int PharmacyId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CompletedAt { get; set; }


        public MedicationOrder() { }
        public MedicationOrder(int patientId, int pharmacyId, DateTime createdAt, DateTime completedAt)
        {
            PatientId = patientId;
            PharmacyId = pharmacyId;
            CreatedAt = createdAt;
            CompletedAt = completedAt;
        }
    }
}
