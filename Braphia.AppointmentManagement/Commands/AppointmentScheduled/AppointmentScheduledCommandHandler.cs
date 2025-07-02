using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Events;
using Braphia.AppointmentManagement.Events.InternalEvents;
using Infrastructure.Messaging;
using MassTransit;
using MediatR;

namespace Braphia.AppointmentManagement.Commands.AddAppointment
{
    public class AppointmentScheduledCommandHandler : IRequestHandler<AppointmentScheduledCommand, int>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPhysicianRepository _physicianRepository;
        private readonly IReceptionistRepository _receptionistRepository;
        private readonly IReferralRepository _referralRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public AppointmentScheduledCommandHandler(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IPhysicianRepository physicianRepository,
            IReceptionistRepository receptionistRepository,
            IReferralRepository referralRepository,
            IPublishEndpoint publishEndpoint,
            ISendEndpointProvider sendEndpointProvider)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _physicianRepository = physicianRepository;
            _receptionistRepository = receptionistRepository;
            _referralRepository = referralRepository;
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task<int> Handle(AppointmentScheduledCommand request, CancellationToken cancellationToken)
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:internal-event-queue"));
            var appointment = new Models.Appointment(
                id: 0,
                patientId: request.PatientId,
                physicianId: request.PhysicianId,
                receptionistId: request.ReceptionistId,
                referralId: request.ReferralId,
                scheduledTime: request.ScheduledTime
            );

            Console.WriteLine($"Creating appointment for Patient ID: {request.PatientId}, Physician ID: {request.PhysicianId}, Scheduled Time: {request.ScheduledTime}");
            Console.WriteLine($"Appointment State: {appointment.state}");

            //Can be put in a separate method if needed
            // Sets the Pre-Appointment Questionnaire for the appointment to preset values
            appointment.SetPreAppointmentQuestionnaire();

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

            var @event = new Events.InternalEvents.InternalAppointmentScheduledEvent
            {
                AppointmentId = appointment.Id,
                PatientId = patient.Id,
                PhysicianId = physician.Id,
                ScheduledTime = appointment.ScheduledTime,
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
                State = appointment.state,
                PreAppointmentQuestionnaire = appointment.PreAppointmentQuestionnaire
            };
            
            var mes = new Message(@event);
            await sendEndpoint.Send(mes, cancellationToken);

            return appointment.Id;
        }
    }

}
