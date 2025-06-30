using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Events;
using Infrastructure.Messaging;
using MassTransit;
using MediatR;

namespace Braphia.AppointmentManagement.Commands.AddAppointment
{
    public class AppointmentCreatedCommandHandler : IRequestHandler<AppointmentCreatedCommand, int>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPhysicianRepository _physicianRepository;
        private readonly IReceptionistRepository _receptionistRepository;
        private readonly IReferralRepository _referralRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public AppointmentCreatedCommandHandler(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IPhysicianRepository physicianRepository,
            IReceptionistRepository receptionistRepository,
            IReferralRepository referralRepository,
            IPublishEndpoint publishEndpoint)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _physicianRepository = physicianRepository;
            _receptionistRepository = receptionistRepository;
            _referralRepository = referralRepository;
            _publishEndpoint = publishEndpoint;
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
                throw new InvalidOperationException("Failed to add appointment to the repository.");

            var patient = await _patientRepository.GetPatientByIdAsync(request.PatientId);
            var physician = await _physicianRepository.GetPhysicianByIdAsync(request.PhysicianId);
            var receptionist = await _receptionistRepository.GetReceptionistByIdAsync(request.ReceptionistId);
            var referral = await _referralRepository.GetReferralByIdAsync(request.ReferralId);

            if (patient == null)
                throw new ArgumentException($"Patient with ID {request.PatientId} not found.");
            if (physician == null)
                throw new ArgumentException($"Physician with ID {request.PhysicianId} not found.");
            if (receptionist == null)
                throw new ArgumentException($"Receptionist with ID {request.ReceptionistId} not found.");
            if (referral == null)
                throw new ArgumentException($"Referral with ID {request.ReferralId} not found.");

            var @event = new AppointmentCreatedEvent
            {
                AppointmentId = appointment.Id,
                PatientId = patient.Id,
                PhysicianId = physician.Id,
                ScheduledTime = appointment.ScheduledTime,
                PatientFirstName = patient.FirstName,
                PatientLastName = patient.LastName,
                PatientEmail = patient.Email,
                PatientPhoneNumber = patient.PhoneNumber,
                PhysicianFirstName = physician.FirstName,
                PhysicianLastName = physician.LastName,
                PhysicianSpecialization = physician.Specialization,
                ReceptionistId = receptionist.Id,
                ReceptionistFirstName = receptionist.FirstName,
                ReceptionistLastName = receptionist.LastName,
                ReceptionistEmail = receptionist.Email,
                ReferralId = referral.Id,
                ReferralDate = referral.ReferralDate,
                ReferralReason = referral.Reason,
                StateName = "Created"
            };
            var mes = new Message(@event);
            await _publishEndpoint.Publish(mes, cancellationToken);

            return appointment.Id;
        }
    }

}
