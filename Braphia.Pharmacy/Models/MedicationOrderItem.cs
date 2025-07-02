using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Braphia.Pharmacy.Models
{

    public class MedicationOrderItem
    {
        [Key]
        public int Id { get; set; }

        public int MedicationOrderId { get; set; }
        [JsonIgnore]
        public MedicationOrder MedicationOrder { get; set; }

        public int MedicationId { get; set; }
        public Medication Medication { get; set; }

        public int Amount { get; set; }
    }
}
