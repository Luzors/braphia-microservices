using Braphia.Laboratory.Models;

namespace Braphia.Laboratory.Events.Appointments
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
