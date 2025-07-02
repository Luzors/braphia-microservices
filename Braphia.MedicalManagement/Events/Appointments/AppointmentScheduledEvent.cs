using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events.Appointments
{
    public class AppointmentScheduledEvent
    {
        public Appointment Appointment { get; set; }

        public AppointmentScheduledEvent(Appointment appointment)
        {
            Appointment = appointment;
        }
    }
}
