using Braphia.UserManagement.Models.ExternalOnly;

namespace Braphia.UserManagement.Events.ConsumedEvents.Appointments
{
    public class AppointmentScheduledFollowUpEvent
    {
        public int AppointmentId { get; set; }
        public Appointment FollowUpAppointment { get; set; }

        public AppointmentScheduledFollowUpEvent(int appointmentId, Appointment followUpAppointment)
        {
            AppointmentId = appointmentId;
            FollowUpAppointment = followUpAppointment;
        }
    }
}
