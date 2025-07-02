using System.ComponentModel.DataAnnotations;

namespace Braphia.UserManagement.Models.ExternalOnly
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
        public Patient? Patient { get; set; }

        public int PhysicianId { get; set; }
        public Physician? Physician { get; set; }

        public string Description { get; set; } = string.Empty;

        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();

        public List<Test> Tests { get; set; } = new List<Test>();
    }
}
