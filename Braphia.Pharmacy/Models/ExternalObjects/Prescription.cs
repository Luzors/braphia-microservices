using System.ComponentModel.DataAnnotations;

namespace Braphia.Pharmacy.Models.ExternalObjects
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }
        public int RootId { get; set; }
        public int PatientId { get; set; }

        // TODO: copy from medicalmanagement <- hurry up jorn :)

        public Prescription() { }

        public Prescription(int rootId, int patientId)
        {
            RootId = rootId;
            PatientId = patientId;
        }
    }
}
