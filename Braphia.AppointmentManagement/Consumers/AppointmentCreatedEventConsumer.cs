using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;
using Braphia.AppointmentManagement.Events.InternalEvents;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;

namespace Braphia.AppointmentManagement.Consumers
{
    public class AppointmentCreatedEventConsumer : IConsumer<Message>
    {
        private readonly SQLAppointmentReadRepository _readRepo;
        private readonly ILogger<AppointmentCreatedEventConsumer> _logger;

        public AppointmentCreatedEventConsumer(SQLAppointmentReadRepository readRepo, ILogger<AppointmentCreatedEventConsumer> logger)
        {
            _readRepo = readRepo;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            _logger.LogInformation(
                JsonSerializer.Serialize(context.Message));           
            var type = context.Message.MessageType;
            var data = context.Message.Data.ToString() ?? string.Empty;

            switch(type)
            {
                case "AppointmentCreated":
                    _logger.LogInformation("AppointmentCreated");
                    _logger.LogInformation(type);
                    var createdEvent = JsonSerializer.Deserialize<AppointmentCreatedEvent>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    _logger.LogInformation(createdEvent.ToString());
                    await HandleAppointmentCreatedAsync(createdEvent);
                    break;

                case "AppointmentRescheduled":
                    _logger.LogInformation("AppointmentResc");
                    _logger.LogInformation(type);
                    var rescheduledEvent = JsonSerializer.Deserialize<AppointmentRescheduledEvent>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await HandleAppointmentRescheduledAsync(rescheduledEvent);
                    break;

                case "UserCheckId"
                :
                    _logger.LogInformation("UserCheckId");
                    _logger.LogInformation(type);
                    var userCheck = JsonSerializer.Deserialize<UserCheckIdEvent>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await HandleUserCheckId(userCheck);
                    break;

                case "AppointmentStateChanged":
                    var newState = JsonSerializer.Deserialize<AppointmentStateChangedEvent>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await HandleStateChange(newState);
                    break;

                case "ScheduledFollowUpAppointment":
                    var followUpEvent = JsonSerializer.Deserialize<ScheduledFollowUpAppointmentEvent>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await HandleFollowUpAppointment(followUpEvent);
                    break;
                default:
                    _logger.LogWarning("Unhandled message type: {MessageType}", type);
                    break;
            }
        }
        private async Task HandleAppointmentCreatedAsync(AppointmentCreatedEvent? evt)
        {
            if (evt == null)
            {
                _logger.LogWarning("Received null AppointmentCreatedEvent.");
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
                ScheduledTime = evt.ScheduledTime
            };

            await _readRepo.AddAppointmentAsync(viewModel);
            _logger.LogInformation("Processed AppointmentCreatedEvent for ID {AppointmentId}", evt.AppointmentId);
        }

        private async Task HandleAppointmentRescheduledAsync(AppointmentRescheduledEvent? evt)
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

        private async Task HandleUserCheckId(UserCheckIdEvent? evt)
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

        private async Task HandleStateChange(AppointmentStateChangedEvent? evt)
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

        private async Task HandleFollowUpAppointment(ScheduledFollowUpAppointmentEvent? evt)
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

            await _readRepo.AddFollowUpAppointment( viewModel, evt.AppointmentId);
        }
    }

}
