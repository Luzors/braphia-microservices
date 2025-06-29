using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;
using Braphia.AppointmentManagement.Events;
using Braphia.AppointmentManagement.Models.States;
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
            var type = context.Message.MessageType;
            var data = context.Message.Data.ToString() ?? string.Empty;

            switch(type)
            {
                case "AppointmentCreated":
                    var createdEvent = JsonSerializer.Deserialize<AppointmentCreatedEvent>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await HandleAppointmentCreatedAsync(createdEvent);
                    break;

                case "AppointmentRescheduled":
                    var rescheduledEvent = JsonSerializer.Deserialize<AppointmentRescheduledEvent>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await HandleAppointmentRescheduledAsync(rescheduledEvent);
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

            var viewModel = new AppointmentViewQueryModel
            {
                AppointmentId = evt.AppointmentId,
                PatientId = evt.PatientId,
                PatientFirstName = evt.PatientFirstName,
                PatientLastName = evt.PatientLastName,
                PatientEmail = evt.PatientEmail,
                PatientPhoneNumber = evt.PatientPhoneNumber,
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
                StateName = evt.StateName,
                ScheduledTime = evt.ScheduledTime
            };

            await _readRepo.AddAppointmentAsync(viewModel);
            _logger.LogInformation("Processed AppointmentCreatedEvent for ID {AppointmentId}", evt.AppointmentId);
        }

        private async Task HandleAppointmentRescheduledAsync(AppointmentRescheduledEvent? evt)
        {
            if (evt == null)
            {
                _logger.LogWarning("Received null AppointmentRescheduledEvent.");
                return;
            }

            var appointment = await _readRepo.GetAppointmentByIdAsync(evt.AppointmentId);
            appointment.ScheduledTime = evt.NewScheduledTime;

            await _readRepo.UpdateAppointmentAsync(appointment);
            _logger.LogInformation("Processed AppointmentRescheduledEvent for ID {AppointmentId}", evt.AppointmentId);
        }
    }

}
