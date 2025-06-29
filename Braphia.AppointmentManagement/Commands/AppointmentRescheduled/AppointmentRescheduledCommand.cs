using MediatR;

namespace Braphia.AppointmentManagement.Commands.AppointmentRescheduled
{
    public class AppointmentRescheduledCommand : IRequest<int>
    {
        public int AppointmentId { get; set; }
        public DateTime NewScheduledTime { get; set; }

    }
}
