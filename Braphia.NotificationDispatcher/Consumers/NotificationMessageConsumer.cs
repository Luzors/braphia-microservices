using Braphia.AppointmentManagement.Events;
using Braphia.NotificationDispatcher.Events.ExternalEvents;
using Braphia.NotificationDispatcher.Events.ExternalEvents.GeneralPracticioners;
using Braphia.NotificationDispatcher.Events.ExternalEvents.Patients;
using Braphia.NotificationDispatcher.Events.ExternalEvents.Pharmacies;
using Braphia.NotificationDispatcher.Events.ExternalEvents.Physicians;
using Braphia.NotificationDispatcher.Models;
using Braphia.NotificationDispatcher.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;

namespace Braphia.NotificationDispatcher.Consumers
{
    public class NotificationMessageConsumer : IConsumer<Message>
    {
        private readonly ILogger<NotificationMessageConsumer> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly ILaboratoryRepository _laboratoryRepository;
        private readonly INotificationRepository _notificationRepository;

        public NotificationMessageConsumer(
            ILogger<NotificationMessageConsumer> logger,
            IUserRepository userRepository,
            IPharmacyRepository pharmacyRepository,
            ILaboratoryRepository laboratoryRepository,
            INotificationRepository notificationRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _pharmacyRepository = pharmacyRepository;
            _laboratoryRepository = laboratoryRepository;
            _notificationRepository = notificationRepository;
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

                    var user = new User
                    {
                        RootId = patientEvent.Patient.Id,
                        FirstName = patientEvent.Patient.FirstName,
                        LastName = patientEvent.Patient.LastName,
                        UserType = UserTypeEnum.Patient,
                        Email = patientEvent.Patient.Email
                    };

                    var success = await _userRepository.AddUserAsync(user);

                    if (success)
                    {
                        _logger.LogInformation("Successfully added user from UserManagement ID {OriginalPatientId} to notification database with new ID {NewPatientId}",
                            patientEvent.Patient.Id, user.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add user from UserManagement ID {OriginalPatientId} to notification database", patientEvent.Patient.RootId);
                        throw new InvalidOperationException($"Failed to add user with ID {patientEvent.Patient.Id} to notification database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PatientCreatedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize PatientCreatedEvent from message data: {message.Data.ToString()}");
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
                    var existingUser = await _userRepository.GetUserByIdAsync(patientEvent.NewPatient.Id, UserTypeEnum.Patient);

                    if (existingUser != null)
                    {
                        existingUser.FirstName = patientEvent.NewPatient.FirstName;
                        existingUser.LastName = patientEvent.NewPatient.LastName;
                        existingUser.Email = patientEvent.NewPatient.Email;
                        var success = await _userRepository.UpdateUserAsync(existingUser.Id, existingUser);
                        if (success)
                        {
                            _logger.LogInformation("Successfully updated user from UserManagement ID {OriginalPatientId} in notification database with new ID {NewPatientId}",
                                patientEvent.NewPatient.RootId, existingUser.Id);
                        }
                        else
                        {
                            _logger.LogError("Failed to update user from UserManagement ID {OriginalPatientId} in notification database", patientEvent.NewPatient.RootId);
                            throw new InvalidOperationException($"Failed to update user with ID {patientEvent.NewPatient.Id} in notification database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No user found for PatientModified event with ID: {PatientId}", patientEvent.NewPatient.Id);
                        throw new KeyNotFoundException($"User with ID {patientEvent.NewPatient.Id} not found in notification database.");
                    }

                }
                else
                {
                    _logger.LogError("Failed to deserialize PatientModifiedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize PatientModifiedEvent from message data: {message.Data.ToString()}");
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
                    var existingUser = await _userRepository.GetUserByIdAsync(patientEvent.Patient.Id, UserTypeEnum.Patient);
                    if (existingUser != null)
                    {
                        var success = await _userRepository.DeleteUserAsync(existingUser.Id);
                        if (success)
                        {
                            _logger.LogInformation("Successfully deleted user from UserManagement ID {OriginalPatientId} in notification database", patientEvent.Patient.RootId);
                        }
                        else
                        {
                            _logger.LogError("Failed to delete user from UserManagement ID {OriginalPatientId} in notification database", patientEvent.Patient.RootId);
                            throw new InvalidOperationException($"Failed to delete user with ID {patientEvent.Patient.Id} from notification database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No user found for PatientRemoved event with ID: {PatientId}", patientEvent.Patient.Id);
                        throw new KeyNotFoundException($"User with ID {patientEvent.Patient.Id} not found in notification database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PatientRemovedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize PatientRemovedEvent from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientRemoved event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task GeneralPracticionerRegistered(Message message)
        {
            try
            {
                _logger.LogInformation("Received GeneralPracticionerRegistered event with ID: {MessageId}", message.MessageId);
                var gpEvent = JsonSerializer.Deserialize<GeneralPracticionerRegisteredEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (gpEvent != null)
                {
                    _logger.LogInformation("Deserialized GP data: ID={GPId}, Name={FirstName} {LastName}, Email={Email}",
                        gpEvent.GeneralPracticioner.Id, gpEvent.GeneralPracticioner.FirstName, gpEvent.GeneralPracticioner.LastName, gpEvent.GeneralPracticioner.Email);
                    var user = new User
                    {
                        RootId = gpEvent.GeneralPracticioner.Id,
                        FirstName = gpEvent.GeneralPracticioner.FirstName,
                        LastName = gpEvent.GeneralPracticioner.LastName,
                        UserType = UserTypeEnum.GeneralPractitioner,
                        Email = gpEvent.GeneralPracticioner.Email
                    };
                    var success = await _userRepository.AddUserAsync(user);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added GP from UserManagement ID {OriginalGPId} to notification database with new ID {NewGPId}",
                            gpEvent.GeneralPracticioner.RootId, user.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add GP from UserManagement ID {OriginalGPId} to notification database", gpEvent.GeneralPracticioner.RootId);
                        throw new InvalidOperationException($"Failed to add GP with ID {gpEvent.GeneralPracticioner.Id} to notification database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize GeneralPracticionerRegisteredEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize GeneralPracticionerRegisteredEvent from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing GeneralPracticionerRegistered event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task GeneralPracticionerModified(Message message)
        {
            try
            {
                _logger.LogInformation("Received GeneralPracticionerModified event with ID: {MessageId}", message.MessageId);
                var gpEvent = JsonSerializer.Deserialize<GeneralPracticionerModifiedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (gpEvent != null)
                {
                    _logger.LogInformation("Deserialized GP data: ID={GPId}, Name={FirstName} {LastName}, Email={Email}",
                        gpEvent.GeneralPracticioner.Id, gpEvent.GeneralPracticioner.FirstName, gpEvent.GeneralPracticioner.LastName, gpEvent.GeneralPracticioner.Email);
                    var existingUser = await _userRepository.GetUserByIdAsync(gpEvent.GeneralPracticioner.Id, UserTypeEnum.GeneralPractitioner);
                    if (existingUser != null)
                    {
                        existingUser.FirstName = gpEvent.GeneralPracticioner.FirstName;
                        existingUser.LastName = gpEvent.GeneralPracticioner.LastName;
                        existingUser.Email = gpEvent.GeneralPracticioner.Email;
                        var success = await _userRepository.UpdateUserAsync(existingUser.Id, existingUser);
                        if (success)
                        {
                            _logger.LogInformation("Successfully updated GP from UserManagement ID {OriginalGPId} in notification database with new ID {NewGPId}",
                                gpEvent.GeneralPracticioner.RootId, existingUser.Id);
                        }
                        else
                        {
                            _logger.LogError("Failed to update GP from UserManagement ID {OriginalGPId} in notification database", gpEvent.GeneralPracticioner.RootId);
                            throw new InvalidOperationException($"Failed to update GP with ID {gpEvent.GeneralPracticioner.Id} in notification database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No user found for GeneralPracticionerModified event with ID: {GPId}", gpEvent.GeneralPracticioner.Id);
                        throw new KeyNotFoundException($"User with ID {gpEvent.GeneralPracticioner.Id} not found in notification database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize GeneralPracticionerModifiedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize GeneralPracticionerModifiedEvent from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing GeneralPracticionerModified event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task GeneralPracticionerRemoved(Message message)
        {
            try
            {
                _logger.LogInformation("Received GeneralPracticionerRemoved event with ID: {MessageId}", message.MessageId);
                var gpEvent = JsonSerializer.Deserialize<GeneralPracticionerRemovedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (gpEvent != null)
                {
                    _logger.LogInformation("Deserialized GP data: ID={GPId}, Name={FirstName} {LastName}, Email={Email}",
                        gpEvent.GeneralPracticioner.Id, gpEvent.GeneralPracticioner.FirstName, gpEvent.GeneralPracticioner.LastName, gpEvent.GeneralPracticioner.Email);
                    var existingUser = await _userRepository.GetUserByIdAsync(gpEvent.GeneralPracticioner.Id, UserTypeEnum.GeneralPractitioner);
                    if (existingUser != null)
                    {
                        var success = await _userRepository.DeleteUserAsync(existingUser.Id);
                        if (success)
                        {
                            _logger.LogInformation("Successfully deleted GP from UserManagement ID {OriginalGPId} in notification database", gpEvent.GeneralPracticioner.RootId);
                        }
                        else
                        {
                            _logger.LogError("Failed to delete GP from UserManagement ID {OriginalGPId} in notification database", gpEvent.GeneralPracticioner.RootId);
                            throw new InvalidOperationException($"Failed to delete GP with ID {gpEvent.GeneralPracticioner.Id} from notification database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No user found for GeneralPracticionerRemoved event with ID: {GPId}", gpEvent.GeneralPracticioner.Id);
                        throw new KeyNotFoundException($"User with ID {gpEvent.GeneralPracticioner.Id} not found in notification database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize GeneralPracticionerRemovedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize GeneralPracticionerRemovedEvent from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing GeneralPracticionerRemoved event: {MessageId}", message.MessageId);
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
                    _logger.LogInformation("Deserialized physician data: ID={PhysicianId}, Name={FirstName} {LastName}, Email={Email}",
                        physicianEvent.Physician.Id, physicianEvent.Physician.FirstName, physicianEvent.Physician.LastName, physicianEvent.Physician.Email);
                    var user = new User
                    {
                        RootId = physicianEvent.Physician.Id,
                        FirstName = physicianEvent.Physician.FirstName,
                        LastName = physicianEvent.Physician.LastName,
                        UserType = UserTypeEnum.Physician,
                        Email = physicianEvent.Physician.Email
                    };
                    var success = await _userRepository.AddUserAsync(user);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added Physician from UserManagement ID {OriginalPhysicianId} to notification database with new ID {NewPhysicianId}",
                            physicianEvent.Physician.RootId, user.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add Physician from UserManagement ID {OriginalPhysicianId} to notification database", physicianEvent.Physician.RootId);
                        throw new InvalidOperationException($"Failed to add Physician with ID {physicianEvent.Physician.Id} to notification database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PhysicianRegisteredEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize PhysicianRegisteredEvent from message data: {message.Data.ToString()}");
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
                    _logger.LogInformation("Deserialized physician data: ID={PhysicianId}, Name={FirstName} {LastName}, Email={Email}",
                        physicianEvent.Physician.Id, physicianEvent.Physician.FirstName, physicianEvent.Physician.LastName, physicianEvent.Physician.Email);
                    var existingUser = await _userRepository.GetUserByIdAsync(physicianEvent.Physician.Id, UserTypeEnum.Physician);
                    if (existingUser != null)
                    {
                        existingUser.FirstName = physicianEvent.Physician.FirstName;
                        existingUser.LastName = physicianEvent.Physician.LastName;
                        existingUser.Email = physicianEvent.Physician.Email;
                        var success = await _userRepository.UpdateUserAsync(existingUser.Id, existingUser);
                        if (success)
                        {
                            _logger.LogInformation("Successfully updated Physician from UserManagement ID {OriginalPhysicianId} in notification database with new ID {NewPhysicianId}",
                                physicianEvent.Physician.RootId, existingUser.Id);
                        }
                        else
                        {
                            _logger.LogError("Failed to update Physician from UserManagement ID {OriginalPhysicianId} in notification database", physicianEvent.Physician.RootId);
                            throw new InvalidOperationException($"Failed to update Physician with ID {physicianEvent.Physician.Id} in notification database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No user found for PhysicianModified event with ID: {PhysicianId}", physicianEvent.Physician.Id);
                        throw new KeyNotFoundException($"User with ID {physicianEvent.Physician.Id} not found in notification database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PhysicianModifiedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize PhysicianModifiedEvent from message data: {message.Data.ToString()}");
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
                    _logger.LogInformation("Deserialized physician data: ID={PhysicianId}, Name={FirstName} {LastName}, Email={Email}",
                        physicianEvent.Physician.Id, physicianEvent.Physician.FirstName, physicianEvent.Physician.LastName, physicianEvent.Physician.Email);
                    var existingUser = await _userRepository.GetUserByIdAsync(physicianEvent.Physician.Id, UserTypeEnum.Physician);
                    if (existingUser != null)
                    {
                        var success = await _userRepository.DeleteUserAsync(existingUser.Id);
                        if (success)
                        {
                            _logger.LogInformation("Successfully deleted Physician from UserManagement ID {OriginalPhysicianId} in notification database", physicianEvent.Physician.RootId);
                        }
                        else
                        {
                            _logger.LogError("Failed to delete Physician from UserManagement ID {OriginalPhysicianId} in notification database", physicianEvent.Physician.RootId);
                            throw new InvalidOperationException($"Failed to delete Physician with ID {physicianEvent.Physician.Id} from notification database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No user found for PhysicianRemoved event with ID: {PhysicianId}", physicianEvent.Physician.Id);
                        throw new KeyNotFoundException($"User with ID {physicianEvent.Physician.Id} not found in notification database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PhysicianRemovedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize PhysicianRemovedEvent from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PhysicianRemoved event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task PharmacyRegistered(Message message)
        {
            try
            {
                _logger.LogInformation("Received PharmacyRegistered event with ID: {MessageId}", message.MessageId);
                var pharmacyEvent = JsonSerializer.Deserialize<PharmacyRegisteredEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (pharmacyEvent != null)
                {
                    _logger.LogInformation("Deserialized pharmacy data: ID={PharmacyId}, Name={Name}, Email={Email}",
                        pharmacyEvent.Pharmacy.Id, pharmacyEvent.Pharmacy.Name, pharmacyEvent.Pharmacy.Email);
                    var pharmacy = new Pharmacy
                    {
                        RootId = pharmacyEvent.Pharmacy.Id,
                        Name = pharmacyEvent.Pharmacy.Name,
                        Email = pharmacyEvent.Pharmacy.Email
                    };
                    var success = await _pharmacyRepository.AddPharmacyAsync(pharmacy);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added Pharmacy from UserManagement ID {OriginalPharmacyId} to notification database with new ID {NewPharmacyId}",
                            pharmacyEvent.Pharmacy.RootId, pharmacy.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add Pharmacy from UserManagement ID {OriginalPharmacyId} to notification database", pharmacyEvent.Pharmacy.RootId);
                        throw new InvalidOperationException($"Failed to add Pharmacy with ID {pharmacyEvent.Pharmacy.Id} to notification database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PharmacyRegisteredEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize PharmacyRegisteredEvent from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PharmacyRegistered event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task PharmacyModified(Message message)
        {
            try
            {
                _logger.LogInformation("Received PharmacyModified event with ID: {MessageId}", message.MessageId);
                var pharmacyEvent = JsonSerializer.Deserialize<PharmacyModifiedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (pharmacyEvent != null)
                {
                    _logger.LogInformation("Deserialized pharmacy data: ID={PharmacyId}, Name={Name}, Email={Email}",
                        pharmacyEvent.Pharmacy.Id, pharmacyEvent.Pharmacy.Name, pharmacyEvent.Pharmacy.Email);
                    var existingPharmacy = await _pharmacyRepository.GetPharmacyByIdAsync(pharmacyEvent.Pharmacy.Id);
                    if (existingPharmacy != null)
                    {
                        existingPharmacy.Name = pharmacyEvent.Pharmacy.Name;
                        existingPharmacy.Email = pharmacyEvent.Pharmacy.Email;
                        var success = await _pharmacyRepository.UpdatePharmacyAsync(existingPharmacy.Id, existingPharmacy);
                        if (success)
                        {
                            _logger.LogInformation("Successfully updated Pharmacy from UserManagement ID {OriginalPharmacyId} in notification database with new ID {NewPharmacyId}",
                                pharmacyEvent.Pharmacy.RootId, existingPharmacy.Id);
                        }
                        else
                        {
                            _logger.LogError("Failed to update Pharmacy from UserManagement ID {OriginalPharmacyId} in notification database", pharmacyEvent.Pharmacy.RootId);
                            throw new InvalidOperationException($"Failed to update Pharmacy with ID {pharmacyEvent.Pharmacy.Id} in notification database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No pharmacy found for PharmacyModified event with ID: {PharmacyId}", pharmacyEvent.Pharmacy.Id);
                        throw new KeyNotFoundException($"Pharmacy with ID {pharmacyEvent.Pharmacy.Id} not found in notification database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PharmacyModifiedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize PharmacyModifiedEvent from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PharmacyModified event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task PharmacyRemoved(Message message)
        {
            try
            {
                _logger.LogInformation("Received PharmacyRemoved event with ID: {MessageId}", message.MessageId);
                var pharmacyEvent = JsonSerializer.Deserialize<PharmacyRemovedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (pharmacyEvent != null)
                {
                    _logger.LogInformation("Deserialized pharmacy data: ID={PharmacyId}, Name={Name}, Email={Email}",
                        pharmacyEvent.Pharmacy.Id, pharmacyEvent.Pharmacy.Name, pharmacyEvent.Pharmacy.Email);
                    var existingPharmacy = await _pharmacyRepository.GetPharmacyByIdAsync(pharmacyEvent.Pharmacy.Id);
                    if (existingPharmacy != null)
                    {
                        var success = await _pharmacyRepository.DeletePharmacyAsync(existingPharmacy.Id);
                        if (success)
                        {
                            _logger.LogInformation("Successfully deleted Pharmacy from UserManagement ID {OriginalPharmacyId} in notification database", pharmacyEvent.Pharmacy.RootId);
                        }
                        else
                        {
                            _logger.LogError("Failed to delete Pharmacy from UserManagement ID {OriginalPharmacyId} in notification database", pharmacyEvent.Pharmacy.RootId);
                            throw new InvalidOperationException($"Failed to delete Pharmacy with ID {pharmacyEvent.Pharmacy.Id} from notification database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No pharmacy found for PharmacyRemoved event with ID: {PharmacyId}", pharmacyEvent.Pharmacy.Id);
                        throw new KeyNotFoundException($"Pharmacy with ID {pharmacyEvent.Pharmacy.Id} not found in notification database.");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PharmacyRemovedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize PharmacyRemovedEvent from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PharmacyRemoved event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task MedicationOrderCompleted(Message message)
        {
            try
            {
                _logger.LogInformation("Received MedicationOrderCompleted event with ID: {MessageId}", message.MessageId);
                var medicationEvent = JsonSerializer.Deserialize<MedicationOrderCompletedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (medicationEvent != null)
                {
                    _logger.LogInformation("Deserialized medication order data: ID={OrderId}, PatientId={PatientId}, PharmacyId={PharmacyId}",
                        medicationEvent.MedicationOrder.Id, medicationEvent.MedicationOrder.PatientId, medicationEvent.MedicationOrder.PharmacyId);

                    var user = await _userRepository.GetUserByIdAsync(medicationEvent.MedicationOrder.PatientId, UserTypeEnum.Patient) ??
                        throw new KeyNotFoundException($"User with ID {medicationEvent.MedicationOrder.PatientId} not found in notification database.");

                    Console.WriteLine(JsonSerializer.Serialize(user));
                    var notification = new Notification(title: "Medication Order Completed",
                        message: $"Medication order {medicationEvent.MedicationOrder.Id} for patient {medicationEvent.MedicationOrder.PatientId} has been completed.",
                        userId: user.Id);

                    await _notificationRepository.AddNotificationAsync(notification);
                    //Makt it warning to make it more visible in logs
                    _logger.LogWarning(notification.SendNotification());

                    _logger.LogInformation("Successfully created notification for completed medication order {OrderId}", medicationEvent.MedicationOrder.Id);
                }
                else
                {
                    _logger.LogError("Failed to deserialize MedicationOrderCompletedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize MedicationOrderCompletedEvent from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing MedicationOrderCompleted event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task AppointmentReminder(Message message)
        {
            try
            {
                _logger.LogInformation("Received AppointmentReminder event with ID: {MessageId}", message.MessageId);
                var appointmentEvent = JsonSerializer.Deserialize<AppointmentReminderEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (appointmentEvent != null)
                {
                    _logger.LogInformation("Deserialized appointment reminder data: AppointmentId={AppointmentId}, PatientId={PatientId}, ScheduledTime={ScheduledTime}",
                        appointmentEvent.AppointmentId, appointmentEvent.PatientId, appointmentEvent.ScheduledTime);
                    var user = await _userRepository.GetUserByIdAsync(appointmentEvent.PatientId, UserTypeEnum.Patient) ??
                        throw new KeyNotFoundException($"User with ID {appointmentEvent.PatientId} not found in notification database.");
                    var notification = new Notification(title: "Appointment Reminder",
                        message: $"You have an upcoming appointment scheduled for {appointmentEvent.ScheduledTime}.",
                        userId: user.Id);
                    await _notificationRepository.AddNotificationAsync(notification);
                    //Makt it warning to make it more visible in logs
                    _logger.LogWarning(notification.SendNotification());
                    _logger.LogInformation("Successfully created notification for appointment reminder for AppointmentId {AppointmentId}", appointmentEvent.AppointmentId);
                }
                else
                {
                    _logger.LogError("Failed to deserialize AppointmentReminderEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize AppointmentReminderEvent from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AppointmentReminder event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task PatientArrived(Message message)
        {
            try
            {
                _logger.LogInformation("Received PatientArrived event with ID: {MessageId}", message.MessageId);
                var patientEvent = JsonSerializer.Deserialize<PatientArrivedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (patientEvent != null)
                {
                    _logger.LogInformation("Deserialized patientarrived data: AppointmentId: {AID}, PhysicianId: {PID}",
                        patientEvent.AppointmentId, patientEvent.PhysicianId);

                    var user = await _userRepository.GetUserByIdAsync(patientEvent.PhysicianId, UserTypeEnum.Physician) ??
                        throw new KeyNotFoundException($"User with ID {patientEvent.PhysicianId} not found in notification database.");
                    var notification = new Notification(title: "Patient Arrived",
                        message: $"Patient has arrived for appointment {patientEvent.AppointmentId}.",
                        userId: user.Id);
                    await _notificationRepository.AddNotificationAsync(notification);
                    _logger.LogWarning(notification.SendNotification());
                    _logger.LogInformation("Successfully created notification for patient arrival for AppointmentId {AppointmentId}", patientEvent.AppointmentId);
                }
                else
                {
                    _logger.LogError("Failed to deserialize PatientArrivedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize PatientArrivedEvent from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientArrived event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task TestCompleted(Message message)
        {
            try
            {
                _logger.LogInformation("Received TestCompleted event with ID: {MessageId}", message.MessageId);
                var testEvent = JsonSerializer.Deserialize<TestCompletedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (testEvent != null)
                {
                    _logger.LogInformation("Deserialized test data: TestId={TestId}, PatientId={PatientId}",
                        testEvent.Test.Id, testEvent.Test.PatientId);
                    var user = await _userRepository.GetUserByIdAsync(testEvent.Test.PatientId, UserTypeEnum.Patient) ??
                        throw new KeyNotFoundException($"User with ID {testEvent.Test.PatientId} not found in notification database.");
                    var notification = new Notification(title: "Test Completed",
                        message: $"Your test {testEvent.Test.Id} has been completed with Result: {testEvent.Test.Result}.",
                        userId: user.Id);
                    await _notificationRepository.AddNotificationAsync(notification);
                    _logger.LogWarning(notification.SendNotification());
                    _logger.LogInformation("Successfully created notification for completed test {TestId}", testEvent.Test.Id);
                }
                else
                {
                    _logger.LogError("Failed to deserialize TestCompletedEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize TestCompletedEvent from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TestCompleted event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task PrescriptionWritten(Message message)
        {
            try
            {
                _logger.LogInformation("Received PrescriptionWritten event with ID: {MessageId}", message.MessageId);
                var prescriptionEvent = JsonSerializer.Deserialize<PrescriptionWrittenEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (prescriptionEvent != null)
                {
                    _logger.LogInformation("Deserialized prescription data: PrescriptionId={PrescriptionId}, PatientId={PatientId}",
                        prescriptionEvent.Prescription.Id, prescriptionEvent.Prescription.PatientId);
                    var pharmacies = await _pharmacyRepository.GetAllPharmaciesAsync();
                    if (pharmacies == null || !pharmacies.Any())
                    {
                        _logger.LogWarning("No pharmacies found in the database for PrescriptionWritten event.");
                        return;
                    }
                    foreach (var pharmacy in pharmacies)
                    {
                        _logger.LogInformation("Processing pharmacy: {PharmacyName} (ID: {PharmacyId})", pharmacy.Name, pharmacy.Id);
                        var notification = new Notification(title: "Prescription Written",
                            message: $"A new prescription {prescriptionEvent.Prescription.Id} has been written, please fulfill the order.",
                            pharmacyId: pharmacy.Id);
                        await _notificationRepository.AddNotificationAsync(notification);
                        _logger.LogWarning(notification.SendNotification());
                    }
                    _logger.LogInformation("Successfully created notifications for prescription {PrescriptionId} for all pharmacies", prescriptionEvent.Prescription.Id);
                }
                else
                {
                    _logger.LogError("Failed to deserialize PrescriptionWrittenEvent from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize PrescriptionWrittenEvent from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PrescriptionWritten event: {MessageId}", message.MessageId);
                throw;
            }
        }
    }
}