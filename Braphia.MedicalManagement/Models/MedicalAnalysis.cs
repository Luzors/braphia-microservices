using System.ComponentModel.DataAnnotations;

namespace Braphia.MedicalManagement.Models
{
    public class MedicalAnalysis
    {
        public MedicalAnalysis() { }

        public MedicalAnalysis(Patient patient, Physician physician)
        {
            Patient = patient;
            Physician = physician;
        }

        [Key]
        public int Id { get; set; }

        public Patient Patient { get; set; }
        public Physician Physician { get; set; }
    }
}
