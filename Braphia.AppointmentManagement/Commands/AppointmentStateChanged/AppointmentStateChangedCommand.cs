using Braphia.AppointmentManagement.Enums;
using MediatR;

namespace Braphia.AppointmentManagement.Commands.AppointmentStateChanged
{
    public class AppointmentStateChangedCommand : IRequest<int>
    {
        public int AppointmentId { get; set; }
        public AppointmentStateEnum NewState { get; set; }
        public AppointmentStateChangedCommand(int appointmentId, AppointmentStateEnum newState)
        {
            AppointmentId = appointmentId;
            NewState = newState;
        }
    
    
    }
}
