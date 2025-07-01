using Braphia.MedicalManagement.Converters;
using Braphia.MedicalManagement.Events.Appointments;
using Braphia.MedicalManagement.Events.Patients;
using Braphia.MedicalManagement.Events.Physicians;
using Braphia.MedicalManagement.Events.Tests;
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
        private readonly IPhysicianRepository _physicianRepository;

        private readonly ITestRepository _testRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<MedicalManagementMessageConsumer> _logger;

        public MedicalManagementMessageConsumer(IPatientRepository patientRepository,
                                                IPhysicianRepository physicianRepository,
                                                IAppointmentRepository appointmentRepository,
                                                ITestRepository testRepository,
                                                ILogger<MedicalManagementMessageConsumer> logger)
        {
            _patientRepository = patientRepository;
            _physicianRepository = physicianRepository;
            _appointmentRepository = appointmentRepository;
            _testRepository = testRepository;
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
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PatientRegistered from message data: {Data}", message.Data.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PatientRegistered event: {MessageId}", message.MessageId);
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
                    _logger.LogInformation("Deserialized physician data: ID={Id}, Name={FirstName} {LastName}, Email={Email}",
                        physicianEvent.Physician.Id, physicianEvent.Physician.FirstName, physicianEvent.Physician.LastName, physicianEvent.Physician.Email);

                    var physician = new Physician()
                    {
                        Id = physicianEvent.Physician.Id,
                        FirstName = physicianEvent.Physician.FirstName,
                        LastName = physicianEvent.Physician.LastName,
                        Email = physicianEvent.Physician.Email,
                        PhoneNumber = physicianEvent.Physician.PhoneNumber
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
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PhysicianRegistered event: {MessageId}", message.MessageId);
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
                        existingPhysician.Email = physicianEvent.Physician.Email;
                        existingPhysician.PhoneNumber = physicianEvent.Physician.PhoneNumber;
                        existingPhysician.BirthDate = physicianEvent.Physician.BirthDate;
                        var success = await _physicianRepository.UpdatePhysicianAsync(existingPhysician);
                        if (success)
                        {
                            _logger.LogInformation("Successfully updated physician with UserManagement ID {PhysicianId}", physicianEvent.PhysicianId);
                        }
                        else
                        {
                            _logger.LogError("Failed to update physician with UserManagement ID {PhysicianId}", physicianEvent.PhysicianId);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Physician with UserManagement ID {PhysicianId} not found in medical database", physicianEvent.PhysicianId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PhysicianModified event: {MessageId}", message.MessageId);
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
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Physician with UserManagement ID {PhysicianId} not found in medical database", physicianEvent.Physician.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PhysicianRemoved event: {MessageId}", message.MessageId);
            }
        }

        private async Task AppointmentScheduled(Message message)
        {
            try
            {
                _logger.LogInformation("Received AppointmentScheduled event with ID: {MessageId}", message.MessageId);
                var appointmentEvent = JsonSerializer.Deserialize<AppointmentScheduledEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (appointmentEvent != null)
                {
                    _logger.LogInformation("Deserialized appointment data: ID={Id}, PatientId={PatientId}, PhysicianId={PhysicianId}, ScheduledTime={ScheduledTime}",
                        appointmentEvent.Appointment.Id, appointmentEvent.Appointment.PatientId, appointmentEvent.Appointment.PhysicianId, appointmentEvent.Appointment.ScheduledTime);
                    var appointment = new Appointment
                    {
                        Id = appointmentEvent.Appointment.Id,
                        PatientId = appointmentEvent.Appointment.PatientId,
                        PhysicianId = appointmentEvent.Appointment.PhysicianId,
                        ScheduledTime = appointmentEvent.Appointment.ScheduledTime,
                        FollowUpAppointmentId = null
                    };
                    var success = await _appointmentRepository.AddAppointmentAsync(appointment, true);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added appointment with ID {AppointmentId} to medical database", appointment.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add appointment with ID {AppointmentId} to medical database", appointment.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AppointmentScheduled event: {MessageId}", message.MessageId);
            }
        }

        private async Task AppointmentScheduledFollowUp(Message message)
        {
            try
            {
                _logger.LogInformation("Received AppointmentScheduledFollowUp event with ID: {MessageId}", message.MessageId);
                var appointmentEvent = JsonSerializer.Deserialize<AppointmentScheduledFollowUpEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (appointmentEvent != null)
                {
                    _logger.LogInformation("Deserialized follow-up appointment data: original ID={OriginalAppointmentId}, FollowUpId={FollowUpAppointmentId}",
                        appointmentEvent.AppointmentId, appointmentEvent.FollowUpAppointment.Id);

                    var followUpAppointment = new Appointment()
                    {
                        Id = appointmentEvent.FollowUpAppointment.Id,
                        PatientId = appointmentEvent.FollowUpAppointment.PatientId,
                        PhysicianId = appointmentEvent.FollowUpAppointment.PhysicianId,
                        ScheduledTime = appointmentEvent.FollowUpAppointment.ScheduledTime,
                        FollowUpAppointmentId = null
                    };

                    var success = await _appointmentRepository.AddAppointmentAsync(followUpAppointment, true);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added follow-up appointment with ID {FollowUpAppointmentId} to medical database", followUpAppointment.Id);
                        // Update the original appointment with the follow-up appointment ID
                        var originalAppointment = await _appointmentRepository.GetAppointmentAsync(appointmentEvent.AppointmentId);
                        if (originalAppointment != null)
                        {
                            originalAppointment.FollowUpAppointmentId = followUpAppointment.Id;
                            await _appointmentRepository.UpdateAppointmentAsync(originalAppointment, true);
                            _logger.LogInformation("Updated original appointment with ID {OriginalAppointmentId} to include follow-up appointment ID {FollowUpAppointmentId}",
                                appointmentEvent.AppointmentId, followUpAppointment.Id);
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to add follow-up appointment with ID {FollowUpAppointmentId} to medical database", followUpAppointment.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AppointmentScheduledFollowUp event: {MessageId}", message.MessageId);
            }
        }

        private async Task AppointmentModified(Message message)
        {
            try
            {
                _logger.LogInformation("Received AppointmentModified event with ID: {MessageId}", message.MessageId);
                var appointmentEvent = JsonSerializer.Deserialize<AppointmentModifiedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (appointmentEvent != null)
                {
                    _logger.LogInformation("Updating appointment with ID {AppointmentId}", appointmentEvent.AppointmentId);
                    var existingAppointment = await _appointmentRepository.GetAppointmentAsync(appointmentEvent.AppointmentId);
                    if (existingAppointment != null)
                    {
                        existingAppointment.PatientId = appointmentEvent.NewAppointment.PatientId;
                        existingAppointment.PhysicianId = appointmentEvent.NewAppointment.PhysicianId;
                        existingAppointment.ScheduledTime = appointmentEvent.NewAppointment.ScheduledTime;
                        existingAppointment.FollowUpAppointmentId = appointmentEvent.NewAppointment.FollowUpAppointmentId;
                        var success = await _appointmentRepository.UpdateAppointmentAsync(existingAppointment, true);
                        if (success)
                        {
                            _logger.LogInformation("Successfully updated appointment with ID {AppointmentId}", appointmentEvent.AppointmentId);
                        }
                        else
                        {
                            _logger.LogError("Failed to update appointment with ID {AppointmentId}", appointmentEvent.AppointmentId);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Appointment with ID {AppointmentId} not found in medical database", appointmentEvent.AppointmentId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AppointmentModified event: {MessageId}", message.MessageId);
            }
        }

        private async Task TestCompleted(Message message)
        {
            try
            {
                _logger.LogInformation("Received TestCompleted event with ID: {MessageId}", message.MessageId);

                var jsonData = message.Data.ToString() ?? string.Empty;
                _logger.LogInformation("Message data: {Data}", jsonData);

                var serializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new DecimalJsonConverter() }
                };

                var testEvent = JsonSerializer.Deserialize<TestCompletedEvent>(jsonData, serializerOptions);

                if (testEvent != null)
                {
                    // Get the existing test from Medical Management database
                    var existingTest = await _testRepository.GetTestAsync(testEvent.Test.Id);

                    if (existingTest != null)
                    {
                        // Only update the fields that should be updated from the lab
                        existingTest.Result = testEvent.Test.Result;
                        existingTest.CompletedDate = testEvent.Test.CompletedDate;
                        existingTest.Cost = testEvent.Test.Cost; // If this field exists

                        // DON'T update foreign keys like MedicalAnalysisId, PatientId, etc.
                        // Keep the existing relationships from Medical Management

                        await _testRepository.UpdateTestAsync(existingTest);

                        _logger.LogInformation("Successfully updated test with ID {TestId} with completion data", existingTest.Id);
                    }
                    else
                    {
                        _logger.LogWarning("Test with ID {TestId} not found in Medical Management database", testEvent.Test.Id);
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize TestCompletedEvent from message data: {Data}", jsonData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TestCompleted event: {MessageId}", message.MessageId);
            }
        }
    }
}