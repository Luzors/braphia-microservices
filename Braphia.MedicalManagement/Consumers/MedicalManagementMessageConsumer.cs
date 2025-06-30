using Braphia.MedicalManagement.Events;
using Braphia.MedicalManagement.Events.Patients;
using Braphia.MedicalManagement.Models;
using Braphia.MedicalManagement.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;

namespace Braphia.MedicalManagement.Consumers
{
    public class MedicalManagementMessageConsumer : IConsumer<Message>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<MedicalManagementMessageConsumer> _logger;

        public MedicalManagementMessageConsumer(IPatientRepository patientRepository, ILogger<MedicalManagementMessageConsumer> logger)
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

        public async Task PatientCreated(Message message)
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
                    _logger.LogInformation("Updating patient with UserManagement ID {PatientId}", patientEvent.PatientId);

                    // Get the existing patient by RootId (which maps to UserManagement ID)
                    var existingPatients = await _patientRepository.GetAllPatientsAsync();
                    var existingPatient = existingPatients.FirstOrDefault(p => p.RootId == patientEvent.PatientId);

                    if (existingPatient != null)
                    {
                        // Update the existing patient with new data
                        existingPatient.FirstName = patientEvent.NewPatient.FirstName;
                        existingPatient.LastName = patientEvent.NewPatient.LastName;
                        existingPatient.Email = patientEvent.NewPatient.Email;
                        existingPatient.PhoneNumber = patientEvent.NewPatient.PhoneNumber;
                        existingPatient.BirthDate = patientEvent.NewPatient.BirthDate;

                        var success = await _patientRepository.UpdatePatientAsync(existingPatient);

                        if (success)
                        {
                            _logger.LogInformation("Successfully updated patient with UserManagement ID {PatientId}", patientEvent.PatientId);
                        }
                        else
                        {
                            _logger.LogError("Failed to update patient with UserManagement ID {PatientId}", patientEvent.PatientId);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Patient with UserManagement ID {PatientId} not found in medical database", patientEvent.PatientId);
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
                    _logger.LogInformation("Removing patient with UserManagement ID {PatientId}", patientEvent.Patient.Id);

                    // Get the existing patient by RootId (which maps to UserManagement ID)
                    var existingPatients = await _patientRepository.GetAllPatientsAsync();
                    var existingPatient = existingPatients.FirstOrDefault(p => p.RootId == patientEvent.Patient.Id);

                    if (existingPatient != null)
                    {
                        var success = await _patientRepository.DeletePatientAsync(existingPatient.Id);

                        if (success)
                        {
                            _logger.LogInformation("Successfully removed patient with UserManagement ID {PatientId} from medical database", patientEvent.Patient.Id);
                        }
                        else
                        {
                            _logger.LogError("Failed to remove patient with UserManagement ID {PatientId} from medical database", patientEvent.Patient.Id);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Patient with UserManagement ID {PatientId} not found in medical database", patientEvent.Patient.Id);
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
