using Braphia.MedicalManagement.Enums;
using System.ComponentModel.DataAnnotations;

namespace Braphia.MedicalManagement.Models
{
    public class Prescription
    {
        public Prescription() { }

        public Prescription(string medicine, string dose, UnitEnum unit, int patientId, int physicianId, int? medicalAnalysisId = null)
        {
            Medicine = medicine;
            Dose = dose;
            Unit = unit;
            PatientId = patientId;
            PhysicianId = physicianId;
            MedicalAnalysisId = medicalAnalysisId;
        }

        [Key]
        public int Id { get; set; }

        public string Medicine { get; set; }

        public string Dose { get; set; }

        public UnitEnum Unit { get; set; }

        public int PatientId { get; set; }  // Foreign Key

        public int PhysicianId { get; set; }  // Foreign Key
        
        public int? MedicalAnalysisId { get; set; }  // Foreign Key
    }
}
