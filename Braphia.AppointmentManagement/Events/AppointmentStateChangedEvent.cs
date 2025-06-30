using Braphia.AppointmentManagement.Models.States;

namespace Braphia.AppointmentManagement.Events
{
    public class AppointmentStateChangedEvent
    {
        public int AppointmentId { get; set; }
        public IAppointmentState NewState { get; set; }
        
    }
}
