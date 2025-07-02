using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Braphia.Pharmacy.Models.ExternalObjects;

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
        public Prescription? Prescription { get; set; }
        public int PharmacyId { get; set; }
        public Pharmacy? Pharmacy { get; set; }
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
            if (CompletedAt != null)
                throw new InvalidOperationException("Cannot add items to a completed order.");
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
            if (CompletedAt != null)
                throw new InvalidOperationException("Cannot remove items from a completed order.");
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
            if (CompletedAt != null)
                throw new InvalidOperationException("Cannot complete an already completed order");
            if (Items.Count == 0)
            {
                throw new InvalidOperationException("Cannot complete an order with no items.");
            }
            if (Items.Any(i => i.Amount <= 0))
            {
                throw new InvalidOperationException("Cannot complete an order with items that have zero or negative amounts.");
            }

            var requiredMedicine = Prescription?.Medicine;
            if (requiredMedicine != null)
            {
                if (!Items.Any(i => i.Medication.Name == requiredMedicine))
                {
                    throw new InvalidOperationException($"Order must contain the prescribed medicine: {requiredMedicine}.");
                }
                CompletedAt = DateTime.UtcNow;
            }
            else
            {
                throw new InvalidOperationException("Prescription is not set for this order.");
            }
        }
    }
}
