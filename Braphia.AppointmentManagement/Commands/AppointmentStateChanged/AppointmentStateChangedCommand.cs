using Braphia.AppointmentManagement.Models.States;
using MediatR;

namespace Braphia.AppointmentManagement.Commands.AppointmentStateChanged
{
    public class AppointmentStateChangedCommand : IRequest<int>
    {
        public int AppointmentId { get; set; }
        public IAppointmentState NewState { get; set; }
        public AppointmentStateChangedCommand(int appointmentId, IAppointmentState newState)
        {
            AppointmentId = appointmentId;
            NewState = newState ?? throw new ArgumentNullException(nameof(newState), "New state cannot be null.");
        }
    
    
    }
}
