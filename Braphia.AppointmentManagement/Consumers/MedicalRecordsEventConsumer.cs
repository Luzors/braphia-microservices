using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;

namespace Braphia.AppointmentManagement.Consumers
{
    public class MedicalRecordsEventConsumer : IConsumer<Message>
    {
        private readonly ILogger<MedicalRecordsEventConsumer> _logger;

        public MedicalRecordsEventConsumer(ILogger<MedicalRecordsEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<Message> context)
        {
            _logger.LogInformation("Message: ", JsonSerializer.Serialize(context.Message, new JsonSerializerOptions { WriteIndented = true }));
            _logger.LogInformation("Consuming message of type {messageType} with ID {messageId}", context.Message.MessageType, context.Message.MessageId);
            switch (context.Message.MessageType)
            {
                case "PostMedicalRecord":
                    _logger.LogInformation("Received a PostMedicalRecord event for PatientId: {patientId}", context.Message.MessageId);
                    break;
            }
            //TODO: Actually consume this thingy

            return Task.CompletedTask;
        }
    }
}
