using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Events;
using MassTransit;
using MediatR;

namespace Braphia.AppointmentManagement.Commands.AddAppointment
{
    public class AppointmentCreatedCommandHandler : IRequestHandler<AppointmentCreatedCommand, int>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public AppointmentCreatedCommandHandler(
            IAppointmentRepository appointmentRepository,
            IPublishEndpoint publishEndpoint)
        {
            _appointmentRepository = appointmentRepository ??
                throw new ArgumentNullException(nameof(appointmentRepository));
            _publishEndpoint = publishEndpoint ??
                throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<int> Handle(AppointmentCreatedCommand request, CancellationToken cancellationToken)
        {
            var appointment = new Models.Appointment(
                id: 0,
                patientId: request.PatientId,
                physicianId: request.PhysicianId,
                receptionistId: request.ReceptionistId,
                referralId: request.ReferralId,
                scheduledTime: request.ScheduledTime
            );

            var success = await _appointmentRepository.AddAppointmentAsync(appointment);
            if (!success)
            {
                throw new InvalidOperationException("Failed to add appointment to the repository.");
            }

            // Publiseer event naar MassTransit voor read database update
            await _publishEndpoint.Publish(new AppointmentCreatedEvent
            {
                AppointmentId = appointment.Id,
                PatientId = appointment.PatientId,
                PhysicianId = appointment.PhysicianId,
                ScheduledTime = appointment.ScheduledTime,
                PatientFirstName = "",
                PatientLastName = "",
                PatientEmail = "",
                PatientPhoneNumber = "",
                PhysicianFirstName = "",
                PhysicianLastName = "",
                PhysicianSpecialization = 0,
                ReceptionistId = appointment.ReceptionistId,
                ReceptionistFirstName = "",
                ReceptionistLastName = "",
                ReceptionistEmail = "",
                ReferralId = appointment.ReferralId,
                ReferralDate = DateTime.UtcNow,
                ReferralReason = "",
                StateName = "Scheduled"
            }, cancellationToken);

            return appointment.Id;
        }
    }
}
