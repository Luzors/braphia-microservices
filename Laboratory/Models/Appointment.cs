namespace Laboratory.Models
{
    public class Appointment
    {
        public Guid Id { get; set; }
        
        public DateTime AppointmentDate { get; set; }

        public Appointment() { }

        public Appointment(DateTime appointmentDate)
        {
            Id = Guid.NewGuid();
            AppointmentDate = appointmentDate;
        }
    }
}
