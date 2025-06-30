using Braphia.Accounting.Events;
using Braphia.Accounting.Models;
using Braphia.Accounting.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;

namespace Braphia.Accounting.Consumers
{
    public class PatientRegisteredConsumer : IConsumer<Message>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<PatientRegisteredConsumer> _logger;

        public PatientRegisteredConsumer(IPatientRepository patientRepository, ILogger<PatientRegisteredConsumer> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
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
                        }                        else
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
        }
    }
}
