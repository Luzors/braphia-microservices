using System.ComponentModel.DataAnnotations;

namespace Braphia.Pharmacy.Models
{
    public class MedicationOrder
    {
        [Key]
        public int Id { get; set; }

        // Medication with amount
        public IList<MedicationOrderItem> Items { get; set; } = [];
        public int PatientId { get; set; }
        public int PrescriptionId { get; set; }
        public int PharmacyId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; } = null;

        public MedicationOrder()
        {
            Items = [];
        }

        public MedicationOrder(int patientId, int prescriptionId, int pharmacyId) : this(patientId, prescriptionId, pharmacyId, [])
        { }

        public MedicationOrder(int patientId, int prescriptionId, int pharmacyId, IList<MedicationOrderItem> items)
        {
            PatientId = patientId;
            PrescriptionId = prescriptionId;
            Items = items ?? [];
        }

        public void AddItem(Medication medication, int amount)
        {
            if (Items.Any(i => i.Medication.Id == medication.Id))
            {
                var existingItem = Items.First(i => i.Medication.Id == medication.Id);
                existingItem.Amount += amount;
            }
            else
            {
                Items.Add(new MedicationOrderItem
                {
                    Medication = medication,
                    Amount = amount
                });
            }
        }

        public void RemoveItem(Medication medication, int amount)
        {
            var existingItem = Items.FirstOrDefault(i => i.Medication.Id == medication.Id);
            if (existingItem != null)
            {
                if (existingItem.Amount > amount)
                {
                    existingItem.Amount -= amount;
                }
                else
                {
                    Items.Remove(existingItem);
                }
            }
        }

        public void CompleteOrder()
        {
            CompletedAt = DateTime.UtcNow;
        }
    }
}
