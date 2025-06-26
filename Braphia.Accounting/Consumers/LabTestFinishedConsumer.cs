using Braphia.Accounting.Events;
using Braphia.Accounting.Models;
using Braphia.Accounting.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;

namespace Braphia.Accounting.Consumers
{
    public class LabTestFinishedConsumer : IConsumer<Message>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<LabTestFinishedConsumer> _logger;

        public LabTestFinishedConsumer(
            IPatientRepository patientRepository, 
            IInvoiceRepository invoiceRepository,
            ILogger<LabTestFinishedConsumer> logger)
        {
            _patientRepository = patientRepository;
            _invoiceRepository = invoiceRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            var message = context.Message;

            if (message.MessageType == "LabTestFinished")
            {
                try
                {
                    _logger.LogInformation("Received LabTestFinished event with ID: {MessageId}", message.MessageId);

                    var labTestEvent = JsonSerializer.Deserialize<LabTestFinishedEvent>(
                        message.Data.ToString() ?? string.Empty,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (labTestEvent != null)
                    {
                        _logger.LogInformation("Deserialized lab test data: LabTestId={LabTestId}, PatientId={PatientId}, TestType={TestType}, Cost={Cost}",
                            labTestEvent.LabTestId, labTestEvent.PatientId, labTestEvent.TestType, labTestEvent.Cost);

                        // Find the patient in the accounting database
                        var patient = await _patientRepository.GetPatientByIdAsync(labTestEvent.PatientId);
                        if (patient == null)
                        {
                            _logger.LogError("Patient with ID {PatientId} not found in accounting database", labTestEvent.PatientId);
                            return;
                        }

                        // Check if patient has an insurer
                        if (patient.InsurerId == null)
                        {
                            _logger.LogWarning("Patient {PatientId} does not have an associated insurer. Cannot create invoice.", labTestEvent.PatientId);
                            return;
                        }

                        // Create invoice for the lab test
                        var invoice = new Invoice
                        {
                            Date = labTestEvent.CompletedDate,
                            Amount = labTestEvent.Cost,
                            Description = $"Lab Test: {labTestEvent.TestType} - {labTestEvent.Description}".Trim(' ', '-'),
                            PatientId = patient.Id,
                            InsurerId = patient.InsurerId.Value
                        };

                        var success = await _invoiceRepository.AddInvoiceAsync(invoice);

                        if (success)
                        {
                            _logger.LogInformation("Successfully created invoice {InvoiceId} for lab test {LabTestId} for patient {PatientId} and insurer {InsurerId}",
                                invoice.Id, labTestEvent.LabTestId, labTestEvent.PatientId, patient.InsurerId);
                        }
                        else
                        {
                            _logger.LogError("Failed to create invoice for lab test {LabTestId} for patient {PatientId}",
                                labTestEvent.LabTestId, labTestEvent.PatientId);
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize LabTestFinishedEvent from message data: {Data}", message.Data.ToString());
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing LabTestFinished event: {MessageId}", message.MessageId);
                }
            }
        }
    }
}
