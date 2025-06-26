using Braphia.MedicalManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace Braphia.MedicalManagement.Models
{
    public class Prescription
    {
        public Prescription() { }

        public Prescription(string medicine, string dose, UnitEnum unit, Patient writtenFor, Physician writtenBy)
        {
            Medicine = medicine;
            Dose = dose;
            Unit = unit;
            WrittenFor = writtenFor;
            WrittenBy = writtenBy;
        }

        [Key]
        public int Id { get; set; }

        public string Medicine { get; set; }

        public string Dose { get; set; }

        public UnitEnum Unit { get; set; }

        public Patient WrittenFor { get; set; }

        public Physician WrittenBy { get; set; }
        
        public MedicalAnalysis MedicalAnalysis { get; set; }
    }
}
