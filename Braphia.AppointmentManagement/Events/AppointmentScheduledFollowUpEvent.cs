using Braphia.AppointmentManagement.Models;

namespace Braphia.AppointmentManagement.Events
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
