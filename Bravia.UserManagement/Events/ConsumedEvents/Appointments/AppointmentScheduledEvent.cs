using Braphia.UserManagement.Models.ExternalOnly;

namespace Braphia.UserManagement.Events.ConsumedEvents.Appointments
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
