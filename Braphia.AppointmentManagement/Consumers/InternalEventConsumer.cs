using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository;
using Braphia.AppointmentManagement.Events.InternalEvents;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;

namespace Braphia.AppointmentManagement.Consumers
{
    public class InternalEventConsumer : IConsumer<Message>
    {
        private readonly SQLAppointmentReadRepository _readRepo;
        private readonly ILogger<InternalEventConsumer> _logger;

        public InternalEventConsumer(SQLAppointmentReadRepository readRepo, ILogger<InternalEventConsumer> logger)
        {
            _readRepo = readRepo;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {

            var type = context.Message.MessageType;
            var data = context.Message.Data.ToString() ?? string.Empty;

            switch (type)
            {
                case "AppointmentScheduled":

                    var createdEvent = JsonSerializer.Deserialize<Events.InternalEvents.InternalAppointmentScheduledEvent>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await HandleAppointmentScheduledAsync(createdEvent);
                    break;

                case "AppointmentRescheduled":

                    var rescheduledEvent = JsonSerializer.Deserialize<InternalAppointmentRescheduledEvent>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await HandleAppointmentRescheduledAsync(rescheduledEvent);
                    break;

                case "UserCheckId"
                :
                    var userCheck = JsonSerializer.Deserialize<InternalUserCheckIdEvent>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await HandleUserCheckId(userCheck);
                    break;

                case "AppointmentStateChanged":
                    var newState = JsonSerializer.Deserialize<InternalAppointmentStateChangedEvent>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await HandleStateChange(newState);
                    break;

                case "ScheduledFollowUpAppointment":
                    var followUpEvent = JsonSerializer.Deserialize<InternalScheduledFollowUpAppointmentEvent>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await HandleFollowUpAppointment(followUpEvent);
                    break;

                case "PreAppointmentQuestionairFilledIn":
                    var preQuestionnaireEvent = JsonSerializer.Deserialize<InternalPreAppointmentQuestionairFilledInEvent>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await HandlePreQuestionaireFilledIn(preQuestionnaireEvent);
                    break;
                default:
                    _logger.LogWarning("Unhandled message type: {MessageType}", type);
                    break;
            }
        }
        private async Task HandleAppointmentScheduledAsync(InternalAppointmentScheduledEvent evt)
        {
            if (evt == null)
            {
                _logger.LogWarning("Received null AppointmentScheduledEvent.");
                return;
            }

            _logger.LogDebug(evt.ToString());

            var viewModel = new AppointmentViewQueryModel
            {
                AppointmentId = evt.AppointmentId,
                PatientId = evt.PatientId,
                PatientFirstName = evt.PatientFirstName,
                PatientLastName = evt.PatientLastName,
                PatientEmail = evt.PatientEmail,
                PatientPhoneNumber = evt.PatientPhoneNumber,
                IsIdChecked = evt.IsIdChecked,
                PhysicianId = evt.PhysicianId,
                PhysicianFirstName = evt.PhysicianFirstName,
                PhysicianLastName = evt.PhysicianLastName,
                PhysicianSpecialization = evt.PhysicianSpecialization,
                ReceptionistId = evt.ReceptionistId,
                ReceptionistFirstName = evt.ReceptionistFirstName,
                ReceptionistLastName = evt.ReceptionistLastName,
                ReceptionistEmail = evt.ReceptionistEmail,
                ReferralId = evt.ReferralId,
                ReferralDate = evt.ReferralDate,
                ReferralReason = evt.ReferralReason,
                State = evt.State,
                ScheduledTime = evt.ScheduledTime,
                PreAppointmentQuestionnaire = evt.PreAppointmentQuestionnaire ?? string.Empty
            };

            await _readRepo.AddAppointmentAsync(viewModel);
        }

        private async Task HandleAppointmentRescheduledAsync(InternalAppointmentRescheduledEvent? evt)
        {
            _logger.LogInformation("HandleResc");
            if (evt == null)
            {
                _logger.LogWarning("Received null AppointmentRescheduledEvent.");
                return;
            }
            _logger.LogInformation(evt.NewScheduledTime.ToString());
            var appointment = await _readRepo.GetAppointmentByIdAsync(evt.AppointmentId);
            appointment.ScheduledTime = evt.NewScheduledTime;

            _logger.LogInformation(appointment.ToString());
            await _readRepo.UpdateAppointmentAsync(appointment);
            _logger.LogInformation("Processed AppointmentRescheduledEvent for ID {AppointmentId}", evt.AppointmentId);
        }

        private async Task HandleUserCheckId(InternalUserCheckIdEvent? evt)
        {
            if (evt == null)
            {
                _logger.LogWarning("Received null UserCheckIdEvent.");
                return;
            }
            _logger.LogInformation("Received UserCheckIdEvent for ID {UserId}", evt.UserId);
            var results = await _readRepo.UserIdChecked(evt.UserId);
            if (results != null)
            {
                _logger.LogInformation("User with ID {UserId} exists in the database.", evt.UserId);
            }
            else
            {
                _logger.LogWarning("User with ID {UserId} does not exist in the database.", evt.UserId);
            }
        }

        private async Task HandleStateChange(InternalAppointmentStateChangedEvent? evt)
        {
            if (evt == null)
            {
                _logger.LogWarning("Received null AppointmentStateChangedEvent.");
                return;
            }
            _logger.LogInformation("Received AppointmentStateChangedEvent for Appointment ID {AppointmentId} with new state {NewState}", evt.AppointmentId, evt.NewState);
            var appointment = await _readRepo.GetAppointmentByIdAsync(evt.AppointmentId);
            appointment.State = evt.NewState;
            await _readRepo.UpdateAppointmentAsync(appointment);
            _logger.LogInformation("Updated state for Appointment ID {AppointmentId} to {NewState}", evt.AppointmentId, evt.NewState);
        }

        private async Task HandleFollowUpAppointment(InternalScheduledFollowUpAppointmentEvent? evt)
        {
            if (evt == null)
            {
                _logger.LogWarning("Received null ScheduledFollowUpAppointmentEvent.");
                return;
            }

            var viewModel = new AppointmentViewQueryModel
            {
                AppointmentId = evt.AppointmentId,
                PatientId = evt.PatientId,
                PatientFirstName = evt.PatientFirstName,
                PatientLastName = evt.PatientLastName,
                PatientEmail = evt.PatientEmail,
                PatientPhoneNumber = evt.PatientPhoneNumber,
                IsIdChecked = evt.IsIdChecked,
                PhysicianId = evt.PhysicianId,
                PhysicianFirstName = evt.PhysicianFirstName,
                PhysicianLastName = evt.PhysicianLastName,
                PhysicianSpecialization = evt.PhysicianSpecialization,
                ReceptionistId = evt.ReceptionistId,
                ReceptionistFirstName = evt.ReceptionistFirstName,
                ReceptionistLastName = evt.ReceptionistLastName,
                ReceptionistEmail = evt.ReceptionistEmail,
                ReferralId = evt.ReferralId,
                ReferralDate = evt.ReferralDate,
                ReferralReason = evt.ReferralReason,
                State = evt.State,
                ScheduledTime = evt.ScheduledTime
            };

            await _readRepo.AddFollowUpAppointment(viewModel, evt.OriginalAppointmentId);
        }

        private async Task HandlePreQuestionaireFilledIn(InternalPreAppointmentQuestionairFilledInEvent? evt)
        {
            if (evt == null)
            {
                _logger.LogWarning("Received null PreAppointmentQuestionairFilledInEvent.");
                return;
            }
            var appointment = await _readRepo.GetAppointmentByIdAsync(evt.AppointmentId);
            if (appointment == null)
            {
                _logger.LogWarning("No appointment found for ID {AppointmentId}", evt.AppointmentId);
                return;
            }
            appointment.IsPreAppointmentQuestionnaireFilled = true;
            appointment.PreAppointmentQuestionnaire = evt.answers;
            await _readRepo.UpdateAppointmentAsync(appointment);
            _logger.LogInformation("Updated Pre-Appointment Questionnaire for Appointment ID {AppointmentId}", evt.AppointmentId);
        }
    }
}
