using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events.Appointments
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
