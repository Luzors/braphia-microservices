using Braphia.Pharmacy.Events.ExternalEvents;
using Braphia.Pharmacy.Models.ExternalObjects;
using Braphia.Pharmacy.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Braphia.Pharmacy.Consumers
{
    public class PharmacyMessageConsumer : IConsumer<Message>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly ILogger<PharmacyMessageConsumer> _logger;

        public PharmacyMessageConsumer(IPatientRepository patientRepository, ILogger<PharmacyMessageConsumer> logger, IPrescriptionRepository prescriptionRepository)
        {
            _patientRepository = patientRepository;
            _logger = logger;
            _prescriptionRepository = prescriptionRepository;
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
                        Id = patientEvent.Patient.Id,
                        FirstName = patientEvent.Patient.FirstName,
                        LastName = patientEvent.Patient.LastName,
                        Email = patientEvent.Patient.Email,
                        PhoneNumber = patientEvent.Patient.PhoneNumber
                    };

                    var success = await _patientRepository.AddPatientAsync(patient, true);

                    if (success)
                    {
                        _logger.LogInformation("Successfully added patient from UserManagement ID {OriginalPatientId}",
                            patient.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add patient from UserManagement ID {OriginalPatientId} to accounting database", patientEvent.Patient.Id);
                        throw new InvalidOperationException($"Failed to add patient with ID {patientEvent.Patient.Id} to accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PatientCreatedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize PatientCreatedEvent from message data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientCreated event: {MessageId}", message.MessageId);
                throw;
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
                        Id = patientEvent.NewPatient.Id,
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
                        throw new InvalidOperationException($"Failed to update patient with ID {patient.Id} in accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PatientModifiedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize PatientModifiedEvent from message data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientModified event: {MessageId}", message.MessageId);
                throw;
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
                        throw new InvalidOperationException($"Failed to remove patient with ID {patientEvent.Patient.Id} from accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PatientRemovedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize PatientRemovedEvent from message data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientRemoved event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task PrescriptionWritten(Message message)
        {
            try
            {
                _logger.LogInformation("Received PrescriptionCreated event with ID: {MessageId}", message.MessageId);
                var prescriptionEvent = JsonSerializer.Deserialize<PrescriptionWrittenEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (prescriptionEvent != null)
                {
                    _logger.LogInformation("Deserialized prescription data: ID={PrescriptionId}, PatientId={PatientId}, Medication={Medication}, Dosage={Dosage}",
                        prescriptionEvent.Prescription.Id, prescriptionEvent.Prescription.PatientId, prescriptionEvent.Prescription.Medicine, prescriptionEvent.Prescription.Dose);
                    var prescription = new Prescription
                    {
                        Id = prescriptionEvent.Prescription.Id,
                        PatientId = prescriptionEvent.Prescription.PatientId,
                        Medicine = prescriptionEvent.Prescription.Medicine,
                        Dose = prescriptionEvent.Prescription.Dose,
                        Unit = prescriptionEvent.Prescription.Unit
                    };
                    var success = await _prescriptionRepository.AddPrescriptionAsync(prescription, true);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added prescription with ID {PrescriptionId}", prescription.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add prescription with ID {PrescriptionId}", prescription.Id);
                        throw new InvalidOperationException($"Failed to add prescription with ID {prescription.Id} to accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PrescriptionWrittenEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize PrescriptionWrittenEvent from message data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PrescriptionWritten event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task PrescriptionChanged(Message message)
        {
            try
            {
                _logger.LogInformation("Received PrescriptionModified event with ID: {MessageId}", message.MessageId);
                var prescriptionEvent = JsonSerializer.Deserialize<PrescriptionChangedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (prescriptionEvent != null)
                {
                    _logger.LogInformation("Deserialized prescription data: ID={PrescriptionId}, PatientId={PatientId}, Medication={Medication}, Dosage={Dosage}",
                        prescriptionEvent.Prescription.Id, prescriptionEvent.Prescription.PatientId, prescriptionEvent.Prescription.Medicine, prescriptionEvent.Prescription.Dose);
                    var prescription = new Prescription
                    {
                        Id = prescriptionEvent.Prescription.Id,
                        PatientId = prescriptionEvent.Prescription.PatientId,
                        Medicine = prescriptionEvent.Prescription.Medicine,
                        Dose = prescriptionEvent.Prescription.Dose,
                        Unit = prescriptionEvent.Prescription.Unit
                    };
                    var success = await _prescriptionRepository.UpdatePrescriptionAsync(prescription.Id, prescription);
                    if (success)
                    {
                        _logger.LogInformation("Successfully updated prescription with ID {PrescriptionId}", prescription.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to update prescription with ID {PrescriptionId}", prescription.Id);
                        throw new InvalidOperationException($"Failed to update prescription with ID {prescription.Id} in accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PrescriptionChangedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize PrescriptionChangedEvent from message data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PrescriptionModified event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task PrescriptionInvoked(Message message)
        {
            try
            {
                _logger.LogInformation("Received PrescriptionRemoved event with ID: {MessageId}", message.MessageId);
                var prescriptionEvent = JsonSerializer.Deserialize<PrescriptionInvokedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (prescriptionEvent != null)
                {
                    _logger.LogInformation("Deserialized prescription data: ID={PrescriptionId}, PatientId={PatientId}, Medication={Medication}, Dosage={Dosage}",
                        prescriptionEvent.Prescription.Id, prescriptionEvent.Prescription.PatientId, prescriptionEvent.Prescription.Medicine, prescriptionEvent.Prescription.Dose);
                    var success = await _prescriptionRepository.DeletePrescriptionAsync(prescriptionEvent.Prescription.Id);
                    if (success)
                    {
                        _logger.LogInformation("Successfully removed prescription with ID {PrescriptionId}", prescriptionEvent.Prescription.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to remove prescription with ID {PrescriptionId}", prescriptionEvent.Prescription.Id);
                        throw new InvalidOperationException($"Failed to remove prescription with ID {prescriptionEvent.Prescription.Id} from accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PrescriptionRemovedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize PrescriptionRemovedEvent from message data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PrescriptionRemoved event: {MessageId}", message.MessageId);
                throw;
            }
        }
    }
}