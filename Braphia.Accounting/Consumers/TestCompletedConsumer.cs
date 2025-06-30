using Braphia.Accounting.Events;
using Braphia.Accounting.Models;
using Braphia.Accounting.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;

namespace Braphia.Accounting.Consumers
{
    public class TestCompletedConsumer : IConsumer<Message>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ITestRepository _testRepository;
        private readonly ILogger<TestCompletedConsumer> _logger;

        public TestCompletedConsumer(
            IPatientRepository patientRepository, 
            IInvoiceRepository invoiceRepository,
            ITestRepository testRepository,
            ILogger<TestCompletedConsumer> logger)
        {
            _patientRepository = patientRepository;
            _invoiceRepository = invoiceRepository;
            _testRepository = testRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            var message = context.Message;

            if (message.MessageType == "TestCompletedEvent")
            {
                try
                {
                    _logger.LogInformation("Received TestCompletedEvent event with ID: {MessageId}", message.MessageId);

                    var labTestEvent = JsonSerializer.Deserialize<TestCompletedEvent>(
                        message.Data.ToString() ?? string.Empty,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (labTestEvent != null)
                    {
                        _logger.LogInformation("Deserialized lab test data: Id={Id} LabTestId={LabTestId}, PatientId={PatientId}, TestType={TestType}, Cost={Cost}",
                            labTestEvent.Test.Id, labTestEvent.Test.RootId, labTestEvent.Test.PatientId, labTestEvent.Test.TestType, labTestEvent.Test.Cost);

                        // Find the patient in the accounting database
                        var patient = await _patientRepository.GetPatientByIdAsync(labTestEvent.Test.PatientId);
                        if (patient == null)
                        {
                            _logger.LogError("Patient with ID {PatientId} not found in accounting database", labTestEvent.Test.PatientId);
                            return;
                        }

                        // Check if patient has an insurer
                        if (patient.InsurerId == null)
                        {
                            _logger.LogWarning("Patient {PatientId} does not have an associated insurer. Cannot create invoice.", labTestEvent.Test.PatientId);
                            return;
                        }

                        // Create invoice for the lab test
                        var invoice = new Invoice
                        {
                            Date = labTestEvent.Test.CompletedDate,
                            Amount = labTestEvent.Test.Cost,
                            Description = $"Lab Test: {labTestEvent.Test.TestType} - {labTestEvent.Test.Description}".Trim(' ', '-'),
                            PatientId = patient.Id,
                            InsurerId = patient.InsurerId.Value
                        };

                        var success = await _invoiceRepository.AddInvoiceAsync(invoice);

                        if (success)
                        {
                            _logger.LogInformation("Successfully created invoice {InvoiceId} for lab test {LabTestId} for patient {PatientId} and insurer {InsurerId}",
                                invoice.Id, labTestEvent.Test.Id, labTestEvent.Test.PatientId, patient.InsurerId);
                        }
                        else
                        {
                            _logger.LogError("Failed to create invoice for lab test {LabTestId} for patient {PatientId}",
                                labTestEvent.Test.Id, labTestEvent.Test.PatientId);
                        }

                        // Save the test in the TestRepository
                        var test = new Test
                        {
                            RootId = labTestEvent.Test.Id,
                            PatientId = labTestEvent.Test.PatientId,
                            TestType = labTestEvent.Test.TestType.ToString(),
                            Description = labTestEvent.Test.Description,
                            Result = labTestEvent.Test.Result,
                            Cost = labTestEvent.Test.Cost,
                            CompletedDate = labTestEvent.Test.CompletedDate
                        };

                        var testAdded = await _testRepository.AddTestAsync(test);

                        if (testAdded)
                        {
                            _logger.LogInformation("Successfully added test {RootId} to accounting database", test.RootId);
                        }
                        else
                        {
                            _logger.LogError("Failed to add test {RootId} to accounting database", test.RootId);
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
