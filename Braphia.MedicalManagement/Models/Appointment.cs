using System;
using System.ComponentModel.DataAnnotations;

namespace Braphia.MedicalManagement.Models
{
    public class Appointment
    {
        public Appointment() { }

        public Appointment(int rootId, DateTime dateTime)
        {
            RootId = rootId;
            DateTime = dateTime;
        }

        [Key]
        public int Id { get; set; }
        public int RootId { get; set; }
        public DateTime DateTime { get; set; }
    }
}