using Braphia.Accounting.Events;
using Braphia.Accounting.Models;
using Braphia.Accounting.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;

namespace Braphia.Accounting.Consumers
{
    public class PatientCreatedConsumer : IConsumer<Message>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<PatientCreatedConsumer> _logger;

        public PatientCreatedConsumer(IPatientRepository patientRepository, ILogger<PatientCreatedConsumer> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            var message = context.Message;
            
            if (message.MessageType == "PatientCreated")
            {
                try
                {
                    _logger.LogInformation("Received PatientCreated event with ID: {MessageId}", message.MessageId);
                    
                    var patientEvent = JsonSerializer.Deserialize<PatientCreatedEvent>(
                        message.Data.ToString() ?? string.Empty,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    
                    if (patientEvent != null)
                    {
                        _logger.LogInformation("Deserialized patient data: ID={RootId}, Name={FirstName} {LastName}, Email={Email}", 
                            patientEvent.Patient.RootId, patientEvent.Patient.FirstName, patientEvent.Patient.LastName, patientEvent.Patient.Email);
                          
                        var patient = new Patient
                        {
                            FirstName = patientEvent.Patient.FirstName,
                            LastName = patientEvent.Patient.LastName,
                            Email = patientEvent.Patient.Email,
                            PhoneNumber = patientEvent.Patient.PhoneNumber
                        };

                        var success = await _patientRepository.AddPatientAsync(patient);
                        
                        if (success)
                        {
                            _logger.LogInformation("Successfully added patient from UserManagement ID {OriginalPatientId} to accounting database with new ID {NewPatientId}", 
                                patientEvent.Patient.RootId, patient.Id);
                        }                        else
                        {
                            _logger.LogError("Failed to add patient from UserManagement ID {OriginalPatientId} to accounting database", patientEvent.Patient.RootId);
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize PatientCreatedEvent from message data: {Data}", message.Data.ToString());
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing PatientCreated event: {MessageId}", message.MessageId);
                }
            }
        }
    }
}
