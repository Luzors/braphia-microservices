using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Events;
using Infrastructure.Messaging;
using MassTransit;
using MediatR;

namespace Braphia.AppointmentManagement.Commands.UserCheckId
{
    public class UserCheckIdCommandHandler : IRequestHandler<UserCheckIdCommand, int>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public UserCheckIdCommandHandler(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IPublishEndpoint publishEndpoint)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository), "Appointment repository cannot be null.");
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository), "Patient repository cannot be null.");
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint), "Publish endpoint cannot be null.");
        }
        public async Task<int> Handle(UserCheckIdCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Checking ID for user with ID: {request.UserId}");
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");
            }
            var patient = await  _patientRepository.GetPatientByIdAsync(request.UserId);
            Console.WriteLine($"Patient found: {patient != null}");
            if (patient == null)
            {
                throw new ArgumentException($"Patient with ID {request.UserId} not found.");
            }
            var succes = await _patientRepository.setIdChecked(request.UserId);
            Console.WriteLine($"ID checked successfully: {succes}");
            if (succes == null)
            {
                throw new InvalidOperationException($"Failed to set ID checked for user with ID {request.UserId}.");
            }

            var @event = new UserCheckIdEvent
            {
                UserId = request.UserId,
            };

            var message = new Message(@event);

            await _publishEndpoint.Publish(message, cancellationToken);

            return request.UserId; // Return true to indicate the operation was successful

        }
    }
}
