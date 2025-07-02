using Braphia.AppointmentManagement.Models;

namespace Braphia.AppointmentManagement.Events
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
