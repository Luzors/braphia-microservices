
namespace Braphia.AppointmentManagement.Events.InternalEvents
{
    public class InternalAppointmentRescheduledEvent 
    {
        public int AppointmentId { get; set; }
        public DateTime NewScheduledTime { get; set; }
    }
}
