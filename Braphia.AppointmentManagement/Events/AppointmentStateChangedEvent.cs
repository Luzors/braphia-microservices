using Braphia.AppointmentManagement.Enums;

namespace Braphia.AppointmentManagement.Events
{
    public class AppointmentStateChangedEvent
    {
        public int AppointmentId { get; set; }
        public AppointmentStateEnum NewState { get; set; }
        
    }
}
