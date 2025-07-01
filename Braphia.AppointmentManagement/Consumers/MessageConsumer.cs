using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Events;
using Braphia.AppointmentManagement.Models;
using Braphia.UserManagement.Events.Patients;
using Braphia.UserManagement.Events.Physicians;
using Braphia.UserManagement.Events.Receptionists;
using Braphia.UserManagement.Events.Referrals;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.OpenApi.Models;
using System.Text.Json;

namespace Braphia.AppointmentManagement.Consumers
{
    public class MessageConsumer : IConsumer<Message>
    {
        private readonly ILogger<MessageConsumer> _logger;
        private readonly IPatientRepository _patientRepository;
        private readonly IPhysicianRepository _physicianRepository;
        private readonly IReceptionistRepository _receptionistRepository;
        private readonly IReferralRepository _referralRepository;

        public MessageConsumer(
            ILogger<MessageConsumer> logger,
            IPatientRepository patientRepository,
            IPhysicianRepository physicianRepository,
            IReceptionistRepository receptionistRepository,
            IReferralRepository referralRepository)
        {
            _logger = logger;
            _patientRepository = patientRepository;
            _physicianRepository = physicianRepository;
            _receptionistRepository = receptionistRepository;
            _referralRepository = referralRepository;
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
                    _logger.LogInformation("Deserialized patient data: ID={Id}, Name={FirstName} {LastName}, Email={Email}",
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
                        throw new InvalidOperationException($"Failed to add patient with ID {patientEvent.Patient.Id} to the accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PatientRegistered from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize PatientRegistered event data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientRegistered event: {MessageId}", message.MessageId);
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
                    _logger.LogInformation("Updating patient with UserManagement ID {PatientId}", patientEvent.PatientId);
                    var existingPatient = await _patientRepository.GetPatientByIdAsync(patientEvent.PatientId);

                    if (existingPatient != null)
                    {
                        existingPatient.FirstName = patientEvent.NewPatient.FirstName;
                        existingPatient.LastName = patientEvent.NewPatient.LastName;
                        existingPatient.Email = patientEvent.NewPatient.Email;
                        existingPatient.PhoneNumber = patientEvent.NewPatient.PhoneNumber;

                        var success = await _patientRepository.UpdatePatientAsync(existingPatient, true);

                        if (success)
                        {
                            _logger.LogInformation("Successfully updated patient with UserManagement ID {PatientId}", patientEvent.PatientId);
                        }
                        else
                        {
                            _logger.LogError("Failed to update patient with UserManagement ID {PatientId}", patientEvent.PatientId);
                            throw new InvalidOperationException($"Failed to update patient with ID {patientEvent.PatientId} in the accounting database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Patient with UserManagement ID {PatientId} not found in medical database", patientEvent.PatientId);
                        throw new KeyNotFoundException($"Patient with ID {patientEvent.PatientId} not found in the accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PatientModifiedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize PatientModifiedEvent data.");
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
                    _logger.LogInformation("Removing patient with UserManagement ID {PatientId}", patientEvent.Patient.Id);

                    var existingPatient = await _patientRepository.GetPatientByIdAsync(patientEvent.Patient.Id);

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
                            throw new InvalidOperationException($"Failed to remove patient with ID {patientEvent.Patient.Id} from the accounting database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Patient with UserManagement ID {PatientId} not found in medical database", patientEvent.Patient.Id);
                        throw new KeyNotFoundException($"Patient with ID {patientEvent.Patient.Id} not found in the accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PatientRemovedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize PatientRemovedEvent data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientRemoved event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task PhysicianRegistered(Message message)
        {
            try
            {
                _logger.LogInformation("Received PhysicianRegistered event with ID: {MessageId}", message.MessageId);
                var physicianEvent = JsonSerializer.Deserialize<PhysicianRegisteredEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (physicianEvent != null)
                {
                    _logger.LogInformation("Deserialized physician data: ID={Id}, Name={FirstName} {LastName}, specialization={Spec}",
                        physicianEvent.Physician.Id, physicianEvent.Physician.FirstName, physicianEvent.Physician.LastName, physicianEvent.Physician.Specialization);

                    var physician = new Physician()
                    {
                        Id = physicianEvent.Physician.Id,
                        FirstName = physicianEvent.Physician.FirstName,
                        LastName = physicianEvent.Physician.LastName,
                        Specialization = physicianEvent.Physician.Specialization
                    };

                    var success = await _physicianRepository.AddPhysicianAsync(physician, true);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added physician from UserManagement ID {OriginalPhysicianId} to accounting database with new ID {NewPhysicianId}",
                            physicianEvent.Physician.Id, physician.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add physician from UserManagement ID {OriginalPhysicianId} to accounting database", physicianEvent.Physician.Id);
                        throw new InvalidOperationException($"Failed to add physician with ID {physicianEvent.Physician.Id} to the accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PhysicianRegistered from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize PhysicianRegistered event data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PhysicianRegistered event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task PhysicianModified(Message message)
        {
            try
            {
                _logger.LogInformation("Received PhysicianModified event with ID: {MessageId}", message.MessageId);
                var physicianEvent = JsonSerializer.Deserialize<PhysicianModifiedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (physicianEvent != null)
                {
                    _logger.LogInformation("Updating physician with UserManagement ID {PhysicianId}", physicianEvent.PhysicianId);
                    var existingPhysician = await _physicianRepository.GetPhysicianByIdAsync(physicianEvent.PhysicianId);
                    if (existingPhysician != null)
                    {
                        existingPhysician.FirstName = physicianEvent.Physician.FirstName;
                        existingPhysician.LastName = physicianEvent.Physician.LastName;
                        existingPhysician.Specialization = physicianEvent.Physician.Specialization;
                        var success = await _physicianRepository.UpdatePhysicianAsync(existingPhysician, true);
                        if (success)
                        {
                            _logger.LogInformation("Successfully updated physician with UserManagement ID {PhysicianId}", physicianEvent.PhysicianId);
                        }
                        else
                        {
                            _logger.LogError("Failed to update physician with UserManagement ID {PhysicianId}", physicianEvent.PhysicianId);
                            throw new InvalidOperationException($"Failed to update physician with ID {physicianEvent.PhysicianId} in the accounting database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Physician with UserManagement ID {PhysicianId} not found in medical database", physicianEvent.PhysicianId);
                        throw new KeyNotFoundException($"Physician with ID {physicianEvent.PhysicianId} not found in the accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PhysicianModifiedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize PhysicianModifiedEvent data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PhysicianModified event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task PhysicianRemoved(Message message)
        {
            try
            {
                _logger.LogInformation("Received PhysicianRemoved event with ID: {MessageId}", message.MessageId);
                var physicianEvent = JsonSerializer.Deserialize<PhysicianRemovedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (physicianEvent != null)
                {
                    _logger.LogInformation("Removing physician with UserManagement ID {PhysicianId}", physicianEvent.Physician.Id);
                    var existingPhysician = await _physicianRepository.GetPhysicianByIdAsync(physicianEvent.Physician.Id);
                    if (existingPhysician != null)
                    {
                        var success = await _physicianRepository.DeletePhysicianAsync(existingPhysician.Id);
                        if (success)
                        {
                            _logger.LogInformation("Successfully removed physician with UserManagement ID {PhysicianId} from medical database", physicianEvent.Physician.Id);
                        }
                        else
                        {
                            _logger.LogError("Failed to remove physician with UserManagement ID {PhysicianId} from medical database", physicianEvent.Physician.Id);
                            throw new InvalidOperationException($"Failed to remove physician with ID {physicianEvent.Physician.Id} from the accounting database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Physician with UserManagement ID {PhysicianId} not found in medical database", physicianEvent.Physician.Id);
                        throw new KeyNotFoundException($"Physician with ID {physicianEvent.Physician.Id} not found in the accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PhysicianRemovedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize PhysicianRemovedEvent data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PhysicianRemoved event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task ReceptionistRegistered(Message message)
        {
            try
            {
                _logger.LogInformation("Received ReceptionistRegistered event with ID: {MessageId}", message.MessageId);
                var receptionistEvent = JsonSerializer.Deserialize<ReceptionistRegisteredEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (receptionistEvent != null)
                {
                    _logger.LogInformation("Deserialized receptionist data: ID={Id}, Name={FirstName} {LastName}",
                        receptionistEvent.Receptionist.Id, receptionistEvent.Receptionist.FirstName, receptionistEvent.Receptionist.LastName);
                    var receptionist = new Receptionist()
                    {
                        Id = receptionistEvent.Receptionist.Id,
                        FirstName = receptionistEvent.Receptionist.FirstName,
                        LastName = receptionistEvent.Receptionist.LastName,
                        Email = receptionistEvent.Receptionist.Email
                    };
                    var success = await _receptionistRepository.AddReceptionistAsync(receptionist, true);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added receptionist from UserManagement ID {OriginalReceptionistId} to accounting database with new ID {NewReceptionistId}",
                            receptionistEvent.Receptionist.Id, receptionist.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add receptionist from UserManagement ID {OriginalReceptionistId} to accounting database", receptionistEvent.Receptionist.Id);
                        throw new InvalidOperationException($"Failed to add receptionist with ID {receptionistEvent.Receptionist.Id} to the accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize ReceptionistRegistered from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize ReceptionistRegistered event data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ReceptionistRegistered event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task ReceptionistModified(Message message)
        {
            try
            {
                _logger.LogInformation("Received ReceptionistModified event with ID: {MessageId}", message.MessageId);
                var receptionistEvent = JsonSerializer.Deserialize<ReceptionistModifiedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (receptionistEvent != null)
                {
                    _logger.LogInformation("Updating receptionist with UserManagement ID {ReceptionistId}", receptionistEvent.ReceptionistId);
                    var existingReceptionist = await _receptionistRepository.GetReceptionistByIdAsync(receptionistEvent.ReceptionistId);
                    if (existingReceptionist != null)
                    {
                        existingReceptionist.FirstName = receptionistEvent.NewReceptionist.FirstName;
                        existingReceptionist.LastName = receptionistEvent.NewReceptionist.LastName;
                        existingReceptionist.Email = receptionistEvent.NewReceptionist.Email;
                        var success = await _receptionistRepository.UpdateReceptionistAsync(existingReceptionist, true);
                        if (success)
                        {
                            _logger.LogInformation("Successfully updated receptionist with UserManagement ID {ReceptionistId}", receptionistEvent.ReceptionistId);
                        }
                        else
                        {
                            _logger.LogError("Failed to update receptionist with UserManagement ID {ReceptionistId}", receptionistEvent.ReceptionistId);
                            throw new InvalidOperationException($"Failed to update receptionist with ID {receptionistEvent.ReceptionistId} in the accounting database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Receptionist with UserManagement ID {ReceptionistId} not found in medical database", receptionistEvent.ReceptionistId);
                        throw new KeyNotFoundException($"Receptionist with ID {receptionistEvent.ReceptionistId} not found in the accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize ReceptionistModifiedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize ReceptionistModifiedEvent data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ReceptionistModified event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task ReceptionistRemoved(Message message)
        {
            try
            {
                _logger.LogInformation("Received ReceptionistRemoved event with ID: {MessageId}", message.MessageId);
                var receptionistEvent = JsonSerializer.Deserialize<ReceptionistRemovedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (receptionistEvent != null)
                {
                    _logger.LogInformation("Removing receptionist with UserManagement ID {ReceptionistId}", receptionistEvent.Receptionist.Id);
                    var existingReceptionist = await _receptionistRepository.GetReceptionistByIdAsync(receptionistEvent.Receptionist.Id);
                    if (existingReceptionist != null)
                    {
                        var success = await _receptionistRepository.DeleteReceptionistAsync(existingReceptionist.Id);
                        if (success)
                        {
                            _logger.LogInformation("Successfully removed receptionist with UserManagement ID {ReceptionistId} from medical database", receptionistEvent.Receptionist.Id);
                        }
                        else
                        {
                            _logger.LogError("Failed to remove receptionist with UserManagement ID {ReceptionistId} from medical database", receptionistEvent.Receptionist.Id);
                            throw new InvalidOperationException($"Failed to remove receptionist with ID {receptionistEvent.Receptionist.Id} from the accounting database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Receptionist with UserManagement ID {ReceptionistId} not found in medical database", receptionistEvent.Receptionist.Id);
                        throw new KeyNotFoundException($"Receptionist with ID {receptionistEvent.Receptionist.Id} not found in the accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize ReceptionistRemovedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize ReceptionistRemovedEvent data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ReceptionistRemoved event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task ReferralSubmitted(Message message)
        {
            try
            {
                _logger.LogInformation("Received ReferralSubmitted event with ID: {MessageId}", message.MessageId);
                var referralEvent = JsonSerializer.Deserialize<ReferralSubmittedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (referralEvent != null)
                {
                    _logger.LogInformation("Deserialized referral data: ID={Id}, PatientId={PatientId}",
                        referralEvent.Referral.Id, referralEvent.Referral.PatientId);
                    var referral = new Referral
                    {
                        Id = referralEvent.Referral.Id,
                        PatientId = referralEvent.Referral.PatientId,
                        ReferralDate = referralEvent.Referral.ReferralDate,
                        Reason = referralEvent.Referral.Reason
                    };
                    var success = await _referralRepository.AddReferralAsync(referral, true);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added referral with ID {ReferralId}", referral.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add referral with ID {ReferralId}", referral.Id);
                        throw new InvalidOperationException($"Failed to add referral with ID {referral.Id} to the accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize ReferralSubmittedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize ReferralSubmittedEvent data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ReferralSubmitted event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task ReferralModified(Message message)
        {
            try
            {
                _logger.LogInformation("Received ReferralModified event with ID: {MessageId}", message.MessageId);
                var referralEvent = JsonSerializer.Deserialize<ReferralModifiedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (referralEvent != null)
                {
                    _logger.LogInformation("Updating referral with ID {ReferralId}", referralEvent.ReferralId);
                    var existingReferral = await _referralRepository.GetReferralByIdAsync(referralEvent.ReferralId);
                    if (existingReferral != null)
                    {
                        existingReferral.Reason = referralEvent.NewReferral.Reason;
                        existingReferral.ReferralDate = referralEvent.NewReferral.ReferralDate;
                        var success = await _referralRepository.UpdateReferralAsync(existingReferral, true);
                        if (success)
                        {
                            _logger.LogInformation("Successfully updated referral with ID {ReferralId}", referralEvent.ReferralId);
                        }
                        else
                        {
                            _logger.LogError("Failed to update referral with ID {ReferralId}", referralEvent.ReferralId);
                            throw new InvalidOperationException($"Failed to update referral with ID {referralEvent.ReferralId} in the accounting database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Referral with ID {ReferralId} not found in medical database", referralEvent.ReferralId);
                        throw new KeyNotFoundException($"Referral with ID {referralEvent.ReferralId} not found in the accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize ReferralModifiedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize ReferralModifiedEvent data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ReferralModified event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task ReferralRemoved(Message message)
        {
            try
            {
                _logger.LogInformation("Received ReferralRemoved event with ID: {MessageId}", message.MessageId);
                var referralEvent = JsonSerializer.Deserialize<ReferralRemovedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (referralEvent != null)
                {
                    _logger.LogInformation("Removing referral with ID {ReferralId}", referralEvent.Referral.Id);
                    var existingReferral = await _referralRepository.GetReferralByIdAsync(referralEvent.Referral.Id);
                    if (existingReferral != null)
                    {
                        var success = await _referralRepository.DeleteReferralAsync(existingReferral.Id);
                        if (success)
                        {
                            _logger.LogInformation("Successfully removed referral with ID {ReferralId} from medical database", referralEvent.Referral.Id);
                        }
                        else
                        {
                            _logger.LogError("Failed to remove referral with ID {ReferralId} from medical database", referralEvent.Referral.Id);
                            throw new InvalidOperationException($"Failed to remove referral with ID {referralEvent.Referral.Id} from the accounting database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Referral with ID {ReferralId} not found in medical database", referralEvent.Referral.Id);
                        throw new KeyNotFoundException($"Referral with ID {referralEvent.Referral.Id} not found in the accounting database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize ReferralRemovedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException("Failed to deserialize ReferralRemovedEvent data.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ReferralRemoved event: {MessageId}", message.MessageId);
                throw;
            }
        }
    }
}