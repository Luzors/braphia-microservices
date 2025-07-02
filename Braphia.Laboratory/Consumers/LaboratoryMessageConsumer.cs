using Braphia.Laboratory.Events;
using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using Braphia.Laboratory.Events.Test;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;
using Braphia.Laboratory.Events.Tests;
using Braphia.Laboratory.Converters;

namespace Braphia.Laboratory.Consumers
{
    public class LaboratoryMessageConsumer : IConsumer<Message>
    {
        private readonly IPatientRepository _patientRepository;

        private readonly ITestRepository _testRepository;
        private readonly ILogger<LaboratoryMessageConsumer> _logger;

        public LaboratoryMessageConsumer(IPatientRepository patientRepository, ITestRepository testRepository, ILogger<LaboratoryMessageConsumer> logger)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _testRepository = testRepository ?? throw new ArgumentNullException(nameof(testRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            var message = context.Message;

            if (message.MessageType == "PatientRegistered")
            {
                try
                {
                    _logger.LogInformation("Received PatientCreated event with ID: {MessageId}", message.MessageId);

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
                            Id = patientEvent.Patient.Id,
                            FirstName = patientEvent.Patient.FirstName,
                            LastName = patientEvent.Patient.LastName,
                            Email = patientEvent.Patient.Email,
                            PhoneNumber = patientEvent.Patient.PhoneNumber
                        };

                        var success = await _patientRepository.AddPatientAsync(patient, true);

                        if (success)
                        {
                            _logger.LogInformation("Successfully added patient from UserManagement ID {OriginalPatientId} to accounting database with new ID {NewPatientId}",
                                patientEvent.Patient.Id, patient.Id);
                        }
                        else
                        {
                            _logger.LogError("Failed to add patient from UserManagement ID {OriginalPatientId} to accounting database", patientEvent.Patient.Id);
                            throw new InvalidOperationException($"Failed to add patient with ID {patientEvent.Patient.Id} to the database.");
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize PatientRegisteredEvent from message data: {Data}", message.Data.ToString());
                        throw new JsonException("Failed to deserialize PatientRegisteredEvent from message data.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing PatientCreated event: {MessageId}", message.MessageId);
                    throw;
                }
            }
            else if (message.MessageType == "TestRequested")
            {
                await TestRequested(message);
            }
            else if (message.MessageType == "TestChanged")
            {
                await TestChanged(message);
            }
            else if (message.MessageType == "TestRemoved")
            {
                await TestRemoved(message);
            }
            else
            {
                _logger.LogWarning("Received unknown message type: {MessageType}", message.MessageType);
            }
        }

        private async Task TestRequested(Message message)
        {
            try
            {
                _logger.LogInformation("Received TestCompleted event with ID: {MessageId}", message.MessageId);
                var serializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new DecimalJsonConverter() }
                };
                
                var testEvent = JsonSerializer.Deserialize<TestRequestedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    serializerOptions
                );

                if (testEvent != null)
                {

                    var succes = await _testRepository.AddTestAsync(testEvent.Test, true);
                    if (succes)
                    {
                        _logger.LogInformation("Successfully added test with ID {TestId} to medical database", testEvent.Test.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add test with ID {TestId} to medical database", testEvent.Test.Id);
                        throw new InvalidOperationException($"Failed to add test with ID {testEvent.Test.Id} to the database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize TestRequestedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize TestRequestedEvent from message data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TestRequested event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task TestChanged(Message message)
        {
            try
            {
                _logger.LogInformation("Received TestChanged event with ID: {MessageId}", message.MessageId);
                var serializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new DecimalJsonConverter() }
                };
                
                var testEvent = JsonSerializer.Deserialize<TestChangedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    serializerOptions
                );

                if (testEvent != null)
                {
                    _logger.LogInformation("Updating test with ID {TestId}", testEvent.Test.Id);
                    var existingTest = await _testRepository.GetTestByIdAsync(testEvent.Test.Id);

                    if (existingTest != null)
                    {
                        // Update the properties of the existing tracked entity
                        existingTest.TestType = testEvent.Test.TestType;
                        existingTest.Description = testEvent.Test.Description;
                        existingTest.Cost = testEvent.Test.Cost;
                        existingTest.PatientId = testEvent.Test.PatientId;
                        existingTest.Result = testEvent.Test.Result;
                        existingTest.CompletedDate = testEvent.Test.CompletedDate;

                        // Update the existing entity (not the deserialized one)
                        var success = await _testRepository.UpdateTestAsync(existingTest);

                        if (success)
                        {
                            _logger.LogInformation("Successfully updated test with ID {TestId}", existingTest.Id);
                        }
                        else
                        {
                            _logger.LogError("Failed to update test with ID {TestId}", existingTest.Id);
                            throw new InvalidOperationException($"Failed to update test with ID {existingTest.Id} in the database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Test with ID {TestId} not found in laboratory database", testEvent.Test.Id);
                        throw new KeyNotFoundException($"Test with ID {testEvent.Test.Id} not found in the database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize TestChangedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize TestChangedEvent from message data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TestChanged event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task TestRemoved(Message message)
        {
            try
            {
                _logger.LogInformation("Received TestRemoved event with ID: {MessageId}", message.MessageId);
                var serializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new DecimalJsonConverter() }
                };
                
                var testEvent = JsonSerializer.Deserialize<TestRemovedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    serializerOptions
                );

                if (testEvent != null)
                {
                    _logger.LogInformation("Removing test with ID {TestId}", testEvent.Test.Id);
                    var existingTest = await _testRepository.GetTestByIdAsync(testEvent.Test.Id);
                    if (existingTest != null)
                    {
                        var success = await _testRepository.DeleteTestAsync(existingTest.Id);
                        if (success)
                        {
                            _logger.LogInformation("Successfully removed test with ID {TestId}", testEvent.Test.Id);
                        }
                        else
                        {
                            _logger.LogError("Failed to remove test with ID {TestId}", testEvent.Test.Id);
                            throw new InvalidOperationException($"Failed to remove test with ID {testEvent.Test.Id} from the database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Test with ID {TestId} not found in medical database", testEvent.Test.Id);
                        throw new KeyNotFoundException($"Test with ID {testEvent.Test.Id} not found in the database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize TestRemovedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize TestRemovedEvent from message data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TestRemoved event: {MessageId}", message.MessageId);
                throw;
            }
        }
    }
}
