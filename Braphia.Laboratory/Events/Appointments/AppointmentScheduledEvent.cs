using Braphia.Laboratory.Models;

namespace Braphia.Laboratory.Events.Appointments
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
