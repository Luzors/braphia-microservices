using System.ComponentModel.DataAnnotations;

namespace Braphia.MedicalManagement.Models
{
    public class Appointment
    {
        public Appointment() { }

        public Appointment(int id, DateTime dateTime)
        {
            Id = id;
            DateTime = dateTime;
        }

        [Key]
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
    }
}