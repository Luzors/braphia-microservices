using Braphia.UserManagement.Events;
using MassTransit;

namespace Braphia.UserManagement.Consumers
{
    public class MedicalRecordsEventConsumer : IConsumer<MedicalRecordsEvent>
    {
        private readonly ILogger<MedicalRecordsEventConsumer> _logger;

        public MedicalRecordsEventConsumer(ILogger<MedicalRecordsEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<MedicalRecordsEvent> context)
        {
            _logger.LogWarning("OMG! We've gotten an medicalrecordsevent: PatientId: {patientId}", context.Message.PatientId);
            //TODO: Actually consume this thingy

            return Task.CompletedTask;
        }
    }
}
