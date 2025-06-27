using System.ComponentModel.DataAnnotations;

namespace Braphia.Laboratory.Models
{
    public class Appointment
    {
        public Appointment() { }

        [Key]
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime AppointmentDate { get; set; }
        // TODO public int? RootId { get; set; } 
    }
}
