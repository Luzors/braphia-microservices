using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Events;
using MassTransit;

namespace Braphia.AppointmentManagement.Commands.AddAppointment
{
    public class AppointmentCreatedCommandHandler : IConsumer<AppointmentCreatedCommand>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        public AppointmentCreatedCommandHandler(IAppointmentRepository appointmentRepository, IPublishEndpoint publishEndpoint)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository), "Appointment repository cannot be null.");
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<AppointmentCreatedCommand> context)
        {
            var command = context.Message;

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command), "Command cannot be null.");
            }

            var appointment = new Models.Appointment(
                id: 0, 
                patientId: command.PatientId,
                physicianId: command.PhysicianId,
                receptionistId: command.ReceptionistId,
                referralId: command.ReferralId,
                scheduledTime: command.ScheduledTime
            );

            var success = await _appointmentRepository.AddAppointmentAsync(appointment);
            if (!success)
            {
                throw new InvalidOperationException("Failed to add appointment to the repository.");
            }
            await _publishEndpoint.Publish(new AppointmentCreatedEvent
            {
                AppointmentId = appointment.Id,
                PatientId = appointment.PatientId,
                PhysicianId = appointment.PhysicianId,
                ScheduledTime = appointment.ScheduledTime
            });

        }
    }
}
