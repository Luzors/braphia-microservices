using Braphia.AppointmentManagement.Events;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;

namespace Braphia.AppointmentManagement.Consumers
{
    public class MessageConsumer : IConsumer<Message>
    {
        private readonly ILogger<MessageConsumer> _logger;

        public MessageConsumer(ILogger<MessageConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<Message> context)
        {
            switch (context.Message.MessageType)
            {
                case "PostMedicalRecord":
                    var medicalRecordEvent = JsonSerializer.Deserialize<MedicalRecordsEvent>(
                        context.Message.Data.ToString() ?? string.Empty,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    HandleMedicalRecordsEvent(medicalRecordEvent);
                    break;
            }
            //TODO: Implement the missing types

            return Task.CompletedTask;
        }

        private void HandleMedicalRecordsEvent(MedicalRecordsEvent? message)
        {
            if (message == null)
            {
                _logger.LogWarning("Received a null MedicalRecordsEvent message.");
                return;
            }
            _logger.LogInformation("Received a PostMedicalRecord event for PatientId: {patientId}", message.PatientId);
        }
    }
}
