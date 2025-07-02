using MediatR;

namespace Braphia.AppointmentManagement.Commands.AppointmentFollowUpScheduled
{
    public class AppointmentFollowUpScheduledCommand : IRequest<int>
    {
        public int OriginalAppointmentId { get; set; }
        public DateTime ScheduledTime { get; set; }
    }
}
