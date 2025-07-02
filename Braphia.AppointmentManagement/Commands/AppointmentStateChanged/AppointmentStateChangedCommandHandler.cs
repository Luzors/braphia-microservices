using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Enums;
using Braphia.AppointmentManagement.Events;
using Braphia.AppointmentManagement.Events.InternalEvents;
using Infrastructure.Messaging;
using MassTransit;
using MediatR;

namespace Braphia.AppointmentManagement.Commands.AppointmentStateChanged
{
    public class AppointmentStateChangedCommandHandler : IRequestHandler<AppointmentStateChangedCommand, int>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public AppointmentStateChangedCommandHandler(IAppointmentRepository appointmentRepository, IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository), "Appointment repository cannot be null.");
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint), "Publish endpoint cannot be null.");
            _sendEndpointProvider = sendEndpointProvider;
        }
        public async Task<int> Handle(AppointmentStateChangedCommand request, CancellationToken cancellationToken)
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:internal-event-queue"));
            if (request.NewState == null)
            {
                throw new ArgumentNullException(nameof(request.NewState), "New state cannot be null.");
            }
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(request.AppointmentId);
            if (appointment == null) throw new InvalidOperationException($"Appointment with ID {request.AppointmentId} not found.");
            bool result = await _appointmentRepository.UpdateAppointmentStateAsync(request.AppointmentId, request.NewState);
            if (!result)
            {
                throw new InvalidOperationException($"Failed to update state for appointment with ID {request.AppointmentId}.");
            }

            var @event = new InternalAppointmentStateChangedEvent
            {
                AppointmentId = request.AppointmentId,
                NewState = request.NewState
            };

            var message = new Message(@event);

            await sendEndpoint.Send(message, cancellationToken);

            //send PatientArrivedEvent to the bus
            if (request.NewState == AppointmentStateEnum.STARTED)
            {
                var patientArrivedEvent = new PatientArrivedEvent
                {
                    AppointmentId = request.AppointmentId,
                    PhysicianId = appointment.PhysicianId
                };
                var patientArrivedMessage = new Message(patientArrivedEvent);
                await _publishEndpoint.Publish(patientArrivedMessage);
            }

            return request.AppointmentId;

        }
    }
    
}
