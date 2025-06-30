using Braphia.Accounting.Converters;
using Braphia.Accounting.Events;
using Braphia.Accounting.Models;
using Braphia.Accounting.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Braphia.Accounting.Consumers
{
    public class AccountingMessageConsumer : IConsumer<Message>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ITestRepository _testRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<AccountingMessageConsumer> _logger;

        public AccountingMessageConsumer(IPatientRepository patientRepository, ITestRepository testRepository, IInvoiceRepository invoiceRepository, ILogger<AccountingMessageConsumer> logger)
        {
            _patientRepository = patientRepository;
            _testRepository = testRepository;
            _invoiceRepository = invoiceRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            var message = context.Message;

            _logger.LogInformation("Received message of type: {MessageType} with ID: {MessageId}", message.MessageType, message.MessageId);

            if (message.MessageType == "PatientRegistered")
            {
                try
                {
                    _logger.LogInformation("Received PatientRegistered event with ID: {MessageId}", message.MessageId);

                    var patientEvent = JsonSerializer.Deserialize<PatientRegisteredEvent>(
                        message.Data.ToString() ?? string.Empty,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (patientEvent != null)
                    {
                        _logger.LogInformation("Deserialized patient data: ID={RootId}, Name={FirstName} {LastName}, Email={Email}",
                            patientEvent.Patient.Id, patientEvent.Patient.FirstName, patientEvent.Patient.LastName, patientEvent.Patient.Email);

                        var patient = new Patient
                        {
                            RootId = patientEvent.Patient.Id,
                            FirstName = patientEvent.Patient.FirstName,
                            LastName = patientEvent.Patient.LastName,
                            Email = patientEvent.Patient.Email,
                            PhoneNumber = patientEvent.Patient.PhoneNumber
                        };

                        var success = await _patientRepository.AddPatientAsync(patient);

                        if (success)
                        {
                            _logger.LogInformation("Successfully added patient from UserManagement ID {OriginalPatientId} to accounting database with new ID {NewPatientId}",
                                patientEvent.Patient.Id, patient.Id);
                        }
                        else
                        {
                            _logger.LogError("Failed to add patient from UserManagement ID {OriginalPatientId} to accounting database", patientEvent.Patient.RootId);
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize PatientRegisteredEvent from message data: {Data}", message.Data.ToString());
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing PatientCreated event: {MessageId}", message.MessageId);
                }
            }
            else if (message.MessageType == "TestCompleted")
            {
                try
                {
                    _logger.LogInformation("Received TestCompleted event with ID: {MessageId}", message.MessageId);
                    // log the cost
                    _logger.LogInformation("Message data: {Data}", message.Data.ToString());

                    var labTestEvent = JsonSerializer.Deserialize<TestCompletedEvent>(
                        message.Data.ToString() ?? string.Empty,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            Converters = { new DecimalJsonConverter() }
                        }
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
                            TestType = labTestEvent.Test.TestType,
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
                        _logger.LogError("Failed to deserialize TestCompleted event from message data: {Data}", message.Data.ToString());
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing TestCompleted event: {MessageId}", message.MessageId);
                }
            }
        }
    }
}
