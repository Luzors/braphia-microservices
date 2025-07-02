using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Events;
using Braphia.AppointmentManagement.Events.InternalEvents;
using Infrastructure.Messaging;
using MassTransit;
using MassTransit.Transports;
using MediatR;

namespace Braphia.AppointmentManagement.Commands.AppointmentRescheduled
{
    public class AppointmentRescheduledCommandHandler : IRequestHandler<AppointmentRescheduledCommand, int>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public AppointmentRescheduledCommandHandler(
            IAppointmentRepository appointmentRepository,
            IPublishEndpoint publishEndpoint,
            ISendEndpointProvider sendEndpointProvider)
        {
            _appointmentRepository = appointmentRepository;
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task<int> Handle(AppointmentRescheduledCommand request, CancellationToken cancellationToken)
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:internal-event-queue"));

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(request.AppointmentId);
            if (appointment == null)
                throw new ArgumentException($"Appointment with ID {request.AppointmentId} not found.");

            
            appointment.ScheduledTime = request.NewScheduledTime;

            var success = await _appointmentRepository.UpdateAppointmentAsync(appointment);
            if (!success)
                throw new InvalidOperationException("Failed to update appointment in the repository.");

            var @event = new InternalAppointmentRescheduledEvent
            {
                AppointmentId = appointment.Id,
                NewScheduledTime = request.NewScheduledTime
            };

            var mes = new Message(@event);

            await sendEndpoint.Send(mes, cancellationToken);

            return appointment.Id;
        }
    }
}
