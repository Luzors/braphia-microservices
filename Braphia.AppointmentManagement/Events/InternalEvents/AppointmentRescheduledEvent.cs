using MediatR;

namespace Braphia.AppointmentManagement.Events.InternalEvents
{
    public class AppointmentRescheduledEvent 
    {
        public int AppointmentId { get; set; }
        public DateTime NewScheduledTime { get; set; }
    }
}
