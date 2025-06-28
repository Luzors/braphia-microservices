using Braphia.Pharmacy.Events.ExternalEvents;
using Braphia.Pharmacy.Models.ExternalObjects;
using Braphia.Pharmacy.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;

namespace Braphia.Pharmacy.Consumers
{
    public class MessageConsumer : IConsumer<Message>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<MessageConsumer> _logger;

        public MessageConsumer(IPatientRepository patientRepository, ILogger<MessageConsumer> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            var message = context.Message;

            _logger.LogInformation("Received message of type: {MessageType} with ID: {MessageId}", message.MessageType, message.MessageId);
            // Dynamic method invocation, looks cool :P
            var method = GetType().GetMethod(message.MessageType, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (method != null)
            {
                var result = method.Invoke(this, [message]);
                if (result is Task task)
                    await task;
            }
            else
                _logger.LogDebug("No handler found for message type: {MessageType}", message.MessageType);
        }

        private async Task PatientRegistered(Message message)
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
                    _logger.LogInformation("Deserialized patient data: ID={PatientId}, Name={FirstName} {LastName}, Email={Email}",
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
                            patientEvent.Patient.RootId, patient.Id);
                    }
                    else
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

        private async Task PatientModified(Message message)
        {
            try
            {
                _logger.LogInformation("Received PatientModified event with ID: {MessageId}", message.MessageId);
                var patientEvent = JsonSerializer.Deserialize<PatientModifiedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (patientEvent != null)
                {
                    _logger.LogInformation("Deserialized patient data: ID={PatientId}, Name={FirstName} {LastName}, Email={Email}",
                        patientEvent.NewPatient.Id, patientEvent.NewPatient.FirstName, patientEvent.NewPatient.LastName, patientEvent.NewPatient.Email);
                    var patient = new Patient
                    {
                        RootId = patientEvent.NewPatient.Id,
                        FirstName = patientEvent.NewPatient.FirstName,
                        LastName = patientEvent.NewPatient.LastName,
                        Email = patientEvent.NewPatient.Email,
                        PhoneNumber = patientEvent.NewPatient.PhoneNumber
                    };
                    var success = await _patientRepository.UpdatePatientAsync(patient.Id, patient);
                    if (success)
                    {
                        _logger.LogInformation("Successfully updated patient with ID {PatientId}", patient.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to update patient with ID {PatientId}", patient.Id);
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PatientModifiedEvent from message data: {Data}", message.Data.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientModified event: {MessageId}", message.MessageId);
            }
        }

        private async Task PatientRemoved(Message message)
        {
            try
            {
                _logger.LogInformation("Received PatientRemoved event with ID: {MessageId}", message.MessageId);
                var patientEvent = JsonSerializer.Deserialize<PatientRemovedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (patientEvent != null)
                {
                    _logger.LogInformation("Deserialized patient data: ID={PatientId}, Name={FirstName} {LastName}, Email={Email}",
                        patientEvent.Patient.Id, patientEvent.Patient.FirstName, patientEvent.Patient.LastName, patientEvent.Patient.Email);
                    var success = await _patientRepository.DeletePatientAsync(patientEvent.Patient.Id);
                    if (success)
                    {
                        _logger.LogInformation("Successfully removed patient with ID {PatientId}", patientEvent.Patient.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to remove patient with ID {PatientId}", patientEvent.Patient.Id);
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PatientRemovedEvent from message data: {Data}", message.Data.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientRemoved event: {MessageId}", message.MessageId);
            }
        }
    }
}
