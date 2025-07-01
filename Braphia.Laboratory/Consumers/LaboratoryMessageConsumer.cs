using Braphia.Laboratory.Events;
using Braphia.Laboratory.Events.Appointments;
using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using Braphia.Laboratory.Events.Test;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;
using Braphia.Laboratory.Converters;

namespace Braphia.Laboratory.Consumers
{
    public class LaboratoryMessageConsumer : IConsumer<Message>
    {
        private readonly IPatientRepository _patientRepository;

        private readonly ITestRepository _testRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<LaboratoryMessageConsumer> _logger;

        public LaboratoryMessageConsumer(IPatientRepository patientRepository, IAppointmentRepository appointmentRepository, ITestRepository testRepository, ILogger<LaboratoryMessageConsumer> logger)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _testRepository = testRepository ?? throw new ArgumentNullException(nameof(testRepository));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
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
            else if (message.MessageType == "AppointmentScheduled")
            {
                await AppointmentScheduled(message);
            }
            else if (message.MessageType == "AppointmentScheduledFollowUp")
            {
                await AppointmentScheduledFollowUp(message);
            }
            else if (message.MessageType == "AppointmentModified")
            {
                await AppointmentModified(message);
            }
            else if (message.MessageType == "TestRequested")
            {
                await TestRequested(message);
            }
            else
            {
                _logger.LogWarning("Received unknown message type: {MessageType}", message.MessageType);
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
                        var originalAppointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentEvent.AppointmentId);
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
                    var existingAppointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentEvent.AppointmentId);
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

        private async Task TestRequested(Message message)
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

                var testEvent = JsonSerializer.Deserialize<TestRequestedEvent>(
                        jsonData,
                        serializerOptions
                    );

                if (testEvent != null)
                {
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

                    await _testRepository.AddTestAsync(testEvent.Test, true);
                }
                else
                {
                    _logger.LogError("Failed to deserialize TestRequestedEvent from message data: {Data}", message.Data.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TestRequested event: {MessageId}", message.MessageId);
            }
        }
    }
}
