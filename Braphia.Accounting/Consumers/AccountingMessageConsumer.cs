using Braphia.Accounting.Converters;
using Braphia.Accounting.Events;
using Braphia.Accounting.EventSourcing.Events;
using Braphia.Accounting.EventSourcing.Services;
using Braphia.Accounting.Models;
using Braphia.Accounting.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;

namespace Braphia.Accounting.Consumers
{
    public class AccountingMessageConsumer : IConsumer<Message>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ITestRepository _testRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceEventService _invoiceEventService;
        private readonly ILogger<AccountingMessageConsumer> _logger;

        public AccountingMessageConsumer(
            IPatientRepository patientRepository, 
            ITestRepository testRepository, 
            IInvoiceRepository invoiceRepository, 
            IInvoiceEventService invoiceEventService,
            ILogger<AccountingMessageConsumer> logger)
        {
            _patientRepository = patientRepository;
            _testRepository = testRepository;
            _invoiceRepository = invoiceRepository;
            _invoiceEventService = invoiceEventService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            var message = context.Message;

            _logger.LogInformation("Received message in accounting of type: {MessageType} with ID: {MessageId}", message.MessageType, message.MessageId);

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
                            throw new InvalidOperationException($"Failed to add patient from UserManagement ID {patientEvent.Patient.Id} to accounting database");
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize PatientRegisteredEvent from message data: {Data}", message.Data.ToString());
                        throw new InvalidOperationException("Failed to deserialize PatientRegisteredEvent from message data");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing PatientCreated event: {MessageId}", message.MessageId);
                    throw new InvalidOperationException($"Error processing PatientCreated event: {message.MessageId}", ex);
                }
            }
            else if (message.MessageType == "TestCompleted")
            {
                try
                {
                    _logger.LogInformation("Received TestCompleted event with ID: {MessageId}", message.MessageId);
                    // log the cost
                    var jsonData = message.Data.ToString() ?? string.Empty;
                    _logger.LogInformation("Message data: {Data}", jsonData);

                    var serializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new DecimalJsonConverter() }
                    };
                    
                    var labTestEvent = JsonSerializer.Deserialize<TestCompletedEvent>(
                        jsonData,
                        serializerOptions
                    );

                    if (labTestEvent != null)
                    {
                        // First parse the original cost string to see what it should be
                        try {
                            // Try to extract the cost directly from the JSON to verify the original value
                            using (JsonDocument doc = JsonDocument.Parse(jsonData))
                            {
                                if (doc.RootElement.TryGetProperty("test", out var testElement) && 
                                    testElement.TryGetProperty("cost", out var costElement))
                                {
                                    string rawCost = costElement.ToString();
                                    _logger.LogInformation("Raw cost value from JSON: '{RawCost}'", rawCost);
                                    
                                    if (decimal.TryParse(rawCost, 
                                        System.Globalization.NumberStyles.Any, 
                                        System.Globalization.CultureInfo.InvariantCulture, 
                                        out decimal parsedCost))
                                    {
                                        _logger.LogInformation("Parsed cost directly from JSON: {ParsedCost}", parsedCost);
                                    }
                                }
                            }
                        }
                        catch (Exception ex) {
                            _logger.LogWarning(ex, "Error parsing raw cost value from JSON");
                        }
                        
                        _logger.LogInformation("Deserialized lab test data: Id={Id} LabTestId={LabTestId}, PatientId={PatientId}, TestType={TestType}, Cost={Cost}",
                            labTestEvent.Test.Id, labTestEvent.Test.RootId, labTestEvent.Test.PatientId, labTestEvent.Test.TestType, labTestEvent.Test.Cost);

                        // Find the patient in the accounting database
                        var patient = await _patientRepository.GetPatientByIdAsync(labTestEvent.Test.PatientId);
                        if (patient == null)
                        {
                            _logger.LogError("Patient with ID {PatientId} not found in accounting database", labTestEvent.Test.PatientId);
                            throw new InvalidOperationException($"Patient with ID {labTestEvent.Test.PatientId} not found in accounting database");
                        }

                        // Check if patient has an insurer
                        if (patient.InsurerId == null)
                        {
                            _logger.LogWarning("Patient {PatientId} does not have an associated insurer. Cannot create invoice.", labTestEvent.Test.PatientId);
                            throw new InvalidOperationException($"Patient {labTestEvent.Test.PatientId} does not have an associated insurer. Cannot create invoice.");
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
                            throw new InvalidOperationException($"Failed to add test {test.RootId} to accounting database");
                        }

                        // Create invoice through event sourcing service
                        string description = $"Lab Test: {labTestEvent.Test.TestType} - {labTestEvent.Test.Description}".Trim(' ', '-');

                        try
                        {
                            int invoiceId = await _invoiceEventService.CreateInvoiceAsync(
                                patient.Id,
                                patient.InsurerId.Value,
                                test.Id,
                                labTestEvent.Test.Cost,
                                description);

                            _logger.LogInformation("Successfully created invoice {InvoiceId} through event sourcing for lab test {LabTestId} for patient {PatientId} and insurer {InsurerId}",
                                invoiceId, test.Id, patient.Id, patient.InsurerId.Value);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to create invoice through event sourcing for lab test {LabTestId} for patient {PatientId}",
                                test.Id, patient.Id);
                            throw new InvalidOperationException($"Failed to create invoice through event sourcing for lab test {test.Id} for patient {patient.Id}", ex);
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize TestCompleted event from message data: {Data}", message.Data.ToString());
                        throw new InvalidOperationException("Failed to deserialize TestCompleted event from message data");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing TestCompleted event: {MessageId}", message.MessageId);
                    throw new InvalidOperationException($"Error processing TestCompleted event: {message.MessageId}", ex);
                }
            }
        }
    }
}
