using System.ComponentModel.DataAnnotations;

namespace Braphia.Pharmacy.Models.ExternalObjects
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }
        public int RootId { get; set; }
        public int PatientId { get; set; }
        public string Medicine { get; set; }
        public string Dose { get; set; }
        public UnitEnum Unit { get; set; }

        public Prescription() { }

        public Prescription(int rootId, int patientId, string medicine, string dose, UnitEnum unit)
        {
            RootId = rootId;
            PatientId = patientId;
            Medicine = medicine;
            Dose = dose;
            Unit = unit;
        }
    }
}
