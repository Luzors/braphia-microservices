namespace Braphia.AppointmentManagement.Events
{
    public class AppointmentReminderEvent
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientEmail { get; set; }
        public DateTime ScheduledTime { get; set; }
    }
}
