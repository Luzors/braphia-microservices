using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Events;
using Infrastructure.Messaging;
using MassTransit;
using MediatR;

namespace Braphia.AppointmentManagement.Commands.AppointmentStateChanged
{
    public class AppointmentStateChangedCommandHandler : IRequestHandler<AppointmentStateChangedCommand, int>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public AppointmentStateChangedCommandHandler(IAppointmentRepository appointmentRepository, IPublishEndpoint publishEndpoint)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository), "Appointment repository cannot be null.");
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint), "Publish endpoint cannot be null.");
        }
        public async Task<int> Handle(AppointmentStateChangedCommand request, CancellationToken cancellationToken)
        {
            if (request.NewState == null)
            {
                throw new ArgumentNullException(nameof(request.NewState), "New state cannot be null.");
            }
            bool result = await _appointmentRepository.UpdateAppointmentStateAsync(request.AppointmentId, request.NewState);
            if (!result)
            {
                throw new InvalidOperationException($"Failed to update state for appointment with ID {request.AppointmentId}.");
            }

            var @event = new AppointmentStateChangedEvent
            {
                AppointmentId = request.AppointmentId,
                NewState = request.NewState
            };

            var message = new Message(@event);

            await _publishEndpoint.Publish(message, cancellationToken);

            return request.AppointmentId;

        }
    }
    
}
