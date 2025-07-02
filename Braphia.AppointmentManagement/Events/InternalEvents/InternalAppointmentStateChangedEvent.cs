using Braphia.AppointmentManagement.Enums;

namespace Braphia.AppointmentManagement.Events.InternalEvents
{
    public class InternalAppointmentStateChangedEvent
    {
        public int AppointmentId { get; set; }
        public AppointmentStateEnum NewState { get; set; }
        
    }
}
