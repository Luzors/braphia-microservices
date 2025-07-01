using Braphia.AppointmentManagement.Commands.AppointmentFollowUpScheduled;
using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Events;
using Braphia.AppointmentManagement.Events.InternalEvents;
using Infrastructure.Messaging;
using MassTransit;
using MediatR;

namespace Braphia.AppointmentManagement.Commands.AddAppointment
{
    public class AppointmentFollowUpScheduledCommandHandler : IRequestHandler<AppointmentFollowUpScheduledCommand, int>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPhysicianRepository _physicianRepository;
        private readonly IReceptionistRepository _receptionistRepository;
        private readonly IReferralRepository _referralRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public AppointmentFollowUpScheduledCommandHandler(
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

        public async Task<int> Handle(AppointmentFollowUpScheduledCommand request, CancellationToken cancellationToken)
        {
            var OriginalAppointment = await _appointmentRepository.GetAppointmentByIdAsync(request.OriginalAppointmentId);
            if (OriginalAppointment == null)
                throw new ArgumentException($"Original appointment with ID {request.OriginalAppointmentId} not found.");

            var followUpAppointment = new Models.Appointment(
                id: 0,
                patientId: OriginalAppointment.PatientId,
                physicianId: OriginalAppointment.PhysicianId,
                receptionistId: OriginalAppointment.ReceptionistId,
                referralId: OriginalAppointment.ReferralId,
                scheduledTime: request.ScheduledTime
            );

            var success = await _appointmentRepository.AddFollowUpAppointment( request.OriginalAppointmentId, followUpAppointment);
            if (!success)
                throw new InvalidOperationException("Failed to add appointment to the repository.");

            var physician = await _physicianRepository.GetPhysicianByIdAsync(followUpAppointment.PhysicianId);
            var receptionist = await _receptionistRepository.GetReceptionistByIdAsync(followUpAppointment.ReceptionistId);
            var referral = await _referralRepository.GetReferralByIdAsync(followUpAppointment.ReferralId);
            var patient = await _patientRepository.GetPatientByIdAsync(followUpAppointment.PatientId);

            if (patient == null)
                throw new ArgumentException($"Patient with ID not found.");
            if (physician == null)
                throw new ArgumentException($"Physician with ID not found.");
            if (receptionist == null)
                throw new ArgumentException($"Receptionist with ID not found.");
            if (referral == null)
                throw new ArgumentException($"Referral with ID not found.");


            var @event = new ScheduledFollowUpAppointmentEvent
            {
                OriginalAppointmentId = request.OriginalAppointmentId,
                AppointmentId = followUpAppointment.Id,
                PatientId = patient.Id,
                PhysicianId = physician.Id,
                ScheduledTime = followUpAppointment.ScheduledTime,
                PatientFirstName = patient.FirstName,
                PatientLastName = patient.LastName,
                PatientEmail = patient.Email,
                PatientPhoneNumber = patient.PhoneNumber,
                IsIdChecked = patient.IsIdChecked,
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
                State = followUpAppointment.state
            };
            
            var mes = new Message(@event);
            Console.WriteLine($"Publishing ScheduledFollowUpAppointmentEvent for Original Appointment ID: {request.OriginalAppointmentId}");
            await _publishEndpoint.Publish(mes, cancellationToken);


            return OriginalAppointment.Id;
        }
    }

}
