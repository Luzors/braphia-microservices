using Braphia.AppointmentManagement.Models;

namespace Braphia.AppointmentManagement.Events
{
    public class AppointmentModifiedEvent
    {
        public int AppointmentId { get; set; }
        public Appointment NewAppointment { get; set; }

        public AppointmentModifiedEvent(int appointmentId, Appointment newAppointment)
        {
            AppointmentId = appointmentId;
            NewAppointment = newAppointment ?? throw new ArgumentNullException(nameof(newAppointment), "New appointment cannot be null.");
        }
    }
}
