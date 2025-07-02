using System.ComponentModel.DataAnnotations;

namespace Braphia.UserManagement.Models.ExternalOnly
{
    public class MedicationOrder
    {
        [Key]
        public int Id { get; set; }

        // Medication with amount
        public IList<MedicationOrderItem> Items { get; set; } = [];
        public int PatientId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; } = null;

        public MedicationOrder()
        {
            Items = [];
        }

        public MedicationOrder(int patientId) : this(patientId, [])
        { }

        public MedicationOrder(int patientId, IList<MedicationOrderItem> items)
        {
            PatientId = patientId;
            Items = items ?? [];
        }

        public decimal CalculateTotalPrice()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                if (item.Medication != null)
                {
                    total += item.Medication.Price * item.Amount;
                }
            }
            return total;
        }
    }
}
