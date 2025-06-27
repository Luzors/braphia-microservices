using System.ComponentModel.DataAnnotations;

namespace Braphia.Pharmacy.Models
{
    public class MedicationOrder
    {
        [Key]
        public int Id { get; set; }

        // Medication with amount
        public IDictionary<Medication, int> Items { get; set; }
        public int PatientId { get; set; }
        public int PrescriptionId { get; set; }
        public int PharmacyId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; } = null;

        public MedicationOrder()
        {
            Items = new Dictionary<Medication, int>();
        }

        public MedicationOrder(int patientId, int prescriptionId, int pharmacyId) : this(patientId, prescriptionId, pharmacyId, new Dictionary<Medication, int>())
        { }

        public MedicationOrder(int patientId, int prescriptionId, int pharmacyId, IDictionary<Medication, int> items)
        {
            PatientId = patientId;
            PrescriptionId = prescriptionId;
            Items = items ?? new Dictionary<Medication, int>();
        }

        public void AddItem(Medication medication, int amount)
        {
            if (Items.ContainsKey(medication))
                Items[medication] += amount;
            else
                Items[medication] = amount;
        }

        public void RemoveItem(Medication medication, int amount)
        {
            if (Items.ContainsKey(medication))
            {
                Items[medication] -= amount;
                if (Items[medication] <= 0)
                    Items.Remove(medication);
            }
        }

        public void CompleteOrder()
        {
            CompletedAt = DateTime.UtcNow;
        }
    }
}
