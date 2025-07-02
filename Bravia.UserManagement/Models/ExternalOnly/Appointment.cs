using System.ComponentModel.DataAnnotations;

namespace Braphia.UserManagement.Models.ExternalOnly
{
    public class Appointment
    {
        public Appointment() { }

        public Appointment(int id, DateTime dateTime)
        {
            Id = id;
            ScheduledTime = dateTime;
        }

        [Key]
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int PhysicianId { get; set; }
        public DateTime ScheduledTime { get; set; }
        public int? FollowUpAppointmentId { get; set; }
    }
}