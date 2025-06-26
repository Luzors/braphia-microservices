using System.ComponentModel.DataAnnotations;

namespace Braphia.MedicalManagement.Models
{
    public class MedicalAnalysis
    {
        public MedicalAnalysis() { }

        public MedicalAnalysis(int patientId, int physicianId, string description, int? appointmentId)
        {
            PatientId = patientId;
            PhysicianId = physicianId;
            Description = description;
            AppointmentId = appointmentId;
        }

        [Key]
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int PhysicianId { get; set; }

        public string Description { get; set; }

        public int? AppointmentId { get; set; }
    }
}
