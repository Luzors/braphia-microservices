using Braphia.UserManagement.Events.ConsumedEvents.Analyses;
using Braphia.UserManagement.Events.ConsumedEvents.Appointments;
using Braphia.UserManagement.Events.ConsumedEvents.Prescriptions;
using Braphia.UserManagement.Events.ConsumedEvents.Tests;
using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using System.Text.Json;

namespace Braphia.UserManagement.Consumers
{
    public class UserManagementMessageConsumer : IConsumer<Message>
    {
        private readonly ILogger<UserManagementMessageConsumer> _logger;
        private readonly IPatientRepository _patientRepository;

        public UserManagementMessageConsumer(ILogger<UserManagementMessageConsumer> logger, IPatientRepository patientRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
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

        //TODO: Wat moet er allemaal geconsumed worden voor med-history.
        // Appointments: aanmaken, followup, state wijziging
        // Analysisses: aanmaken, updaten
        // Prescriptions: aanmaken, updaten, intrekken, voltooiten (uit pharmacy)
        // Tests: aangevraagd, voltooid
        // Voor al deze events moet die de patient uit het event halen, en aan die patient medical records toevoegen.

        private async Task ChangedExamination(Message message)
        {
            try
            {
                _logger.LogInformation("Received ChangedExamination event with ID: {MessageId}", message.MessageId);
                var examEvent = JsonSerializer.Deserialize<ChangedExaminationEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (examEvent != null)
                {
                    _logger.LogInformation("Deserialized ChangedExamination: Id={Id}, PatientId={PatientId}, Description={desc}",
                        examEvent.MedicalAnalysis.Id, examEvent.MedicalAnalysis.PatientId, examEvent.MedicalAnalysis.Description);
                    // Get the user from the event
                    var patient = await _patientRepository.GetPatientByIdAsync(examEvent.MedicalAnalysis.PatientId);
                    if (patient == null)
                    {
                        _logger.LogWarning("Patient with ID {PatientId} not found for ChangedExamination event", examEvent.MedicalAnalysis.PatientId);
                        throw new KeyNotFoundException($"Patient with ID {examEvent.MedicalAnalysis.PatientId} not found.");
                    }

                    // Create new MedicalRecord
                    var description = $"Examination changed: {examEvent.MedicalAnalysis.Description}";
                    var medicalRecord = new MedicalRecord
                    {
                        Description = description,
                        Date = DateTime.UtcNow
                    };

                    // Add the MedicalRecord to the patient
                    var success = await _patientRepository.AddMedicalRecordAsync(patient.Id, medicalRecord);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added MedicalRecord to Patient ID {PatientId}", patient.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add MedicalRecord to Patient ID {PatientId}", patient.Id);
                        throw new InvalidOperationException($"Failed to add MedicalRecord to Patient ID {patient.Id}");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize ChangedExamination from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize ChangedExamination from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ChangedExamination event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task ExaminedPatient(Message message)
        {
            try
            {
                _logger.LogInformation("Received ExaminedPatient event with ID: {MessageId}", message.MessageId);
                var examEvent = JsonSerializer.Deserialize<ExaminedPatientEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (examEvent != null)
                {
                    _logger.LogInformation("Deserialized ExaminedPatient: Id={Id}, PatientId={PatientId}, Description={desc}",
                        examEvent.MedicalAnalysis.Id, examEvent.MedicalAnalysis.PatientId, examEvent.MedicalAnalysis.Description);
                    // Get the user from the event
                    var patient = await _patientRepository.GetPatientByIdAsync(examEvent.MedicalAnalysis.PatientId);
                    if (patient == null)
                    {
                        _logger.LogWarning("Patient with ID {PatientId} not found for ExaminedPatient event", examEvent.MedicalAnalysis.PatientId);
                        throw new KeyNotFoundException($"Patient with ID {examEvent.MedicalAnalysis.PatientId} not found.");
                    }
                    // Create new MedicalRecord
                    var description = $"Examination completed: {examEvent.MedicalAnalysis.Description}";
                    var medicalRecord = new MedicalRecord
                    {
                        Description = description,
                        Date = DateTime.UtcNow
                    };
                    // Add the MedicalRecord to the patient
                    var success = await _patientRepository.AddMedicalRecordAsync(patient.Id, medicalRecord);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added MedicalRecord to Patient ID {PatientId}", patient.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add MedicalRecord to Patient ID {PatientId}", patient.Id);
                        throw new InvalidOperationException($"Failed to add MedicalRecord to Patient ID {patient.Id}");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize ExaminedPatient from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize ExaminedPatient from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ExaminedPatient event: {MessageId}", message.MessageId);
                throw;
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
                    _logger.LogInformation("Deserialized AppointmentModified: Id={Id}, PatientId={PatientId}",
                        appointmentEvent.NewAppointment.Id, appointmentEvent.NewAppointment.PatientId);
                    // Get the user from the event
                    var patient = await _patientRepository.GetPatientByIdAsync(appointmentEvent.NewAppointment.PatientId);
                    if (patient == null)
                    {
                        _logger.LogWarning("Patient with ID {PatientId} not found for AppointmentModified event", appointmentEvent.NewAppointment.PatientId);
                        throw new KeyNotFoundException($"Patient with ID {appointmentEvent.NewAppointment.PatientId} not found.");
                    }
                    // Create new MedicalRecord
                    var description = $"Appointment modified: new date {appointmentEvent.NewAppointment.ScheduledTime}, " +
                        $"With physician {appointmentEvent.NewAppointment.PhysicianId}";
                    var medicalRecord = new MedicalRecord
                    {
                        Description = description,
                        Date = DateTime.UtcNow
                    };
                    // Add the MedicalRecord to the patient
                    var success = await _patientRepository.AddMedicalRecordAsync(patient.Id, medicalRecord);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added MedicalRecord to Patient ID {PatientId}", patient.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add MedicalRecord to Patient ID {PatientId}", patient.Id);
                        throw new InvalidOperationException($"Failed to add MedicalRecord to Patient ID {patient.Id}");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize AppointmentModified from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize AppointmentModified from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AppointmentModified event: {MessageId}", message.MessageId);
                throw;
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
                    _logger.LogInformation("Deserialized AppointmentScheduled: Id={Id}, PatientId={PatientId}",
                        appointmentEvent.Appointment.Id, appointmentEvent.Appointment.PatientId);
                    // Get the user from the event
                    var patient = await _patientRepository.GetPatientByIdAsync(appointmentEvent.Appointment.PatientId);
                    if (patient == null)
                    {
                        _logger.LogWarning("Patient with ID {PatientId} not found for AppointmentScheduled event", appointmentEvent.Appointment.PatientId);
                        throw new KeyNotFoundException($"Patient with ID {appointmentEvent.Appointment.PatientId} not found.");
                    }
                    // Create new MedicalRecord
                    var description = $"Appointment scheduled: {appointmentEvent.Appointment.ScheduledTime}, " +
                        $"With physician {appointmentEvent.Appointment.PhysicianId}";
                    var medicalRecord = new MedicalRecord
                    {
                        Description = description,
                        Date = DateTime.UtcNow
                    };
                    // Add the MedicalRecord to the patient
                    var success = await _patientRepository.AddMedicalRecordAsync(patient.Id, medicalRecord);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added MedicalRecord to Patient ID {PatientId}", patient.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add MedicalRecord to Patient ID {PatientId}", patient.Id);
                        throw new InvalidOperationException($"Failed to add MedicalRecord to Patient ID {patient.Id}");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize AppointmentScheduled from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize AppointmentScheduled from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AppointmentScheduled event: {MessageId}", message.MessageId);
                throw;
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
                    _logger.LogInformation("Deserialized AppointmentScheduledFollowUp: Id={Id}, PatientId={PatientId}",
                        appointmentEvent.FollowUpAppointment.Id, appointmentEvent.FollowUpAppointment.PatientId);
                    // Get the user from the event
                    var patient = await _patientRepository.GetPatientByIdAsync(appointmentEvent.FollowUpAppointment.PatientId);
                    if (patient == null)
                    {
                        _logger.LogWarning("Patient with ID {PatientId} not found for AppointmentScheduledFollowUp event", appointmentEvent.FollowUpAppointment.PatientId);
                        throw new KeyNotFoundException($"Patient with ID {appointmentEvent.FollowUpAppointment.PatientId} not found.");
                    }
                    // Create new MedicalRecord
                    var description = $"Appointment scheduled follow-up: {appointmentEvent.FollowUpAppointment.ScheduledTime}, " +
                        $"With physician {appointmentEvent.FollowUpAppointment.PhysicianId}, Original appointment ID {appointmentEvent.AppointmentId}";
                    var medicalRecord = new MedicalRecord
                    {
                        Description = description,
                        Date = DateTime.UtcNow
                    };
                    // Add the MedicalRecord to the patient
                    var success = await _patientRepository.AddMedicalRecordAsync(patient.Id, medicalRecord);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added MedicalRecord to Patient ID {PatientId}", patient.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add MedicalRecord to Patient ID {PatientId}", patient.Id);
                        throw new InvalidOperationException($"Failed to add MedicalRecord to Patient ID {patient.Id}");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize AppointmentScheduledFollowUp from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize AppointmentScheduledFollowUp from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AppointmentScheduledFollowUp event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task MedicationOrderCompleted(Message message)
        {
            try
            {
                _logger.LogInformation("Received MedicationOrderCompleted event with ID: {MessageId}", message.MessageId);
                var prescriptionEvent = JsonSerializer.Deserialize<MedicationOrderCompletedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (prescriptionEvent != null)
                {
                    _logger.LogInformation("Deserialized MedicationOrderCompleted: Id={Id}, PatientId={PatientId}",
                        prescriptionEvent.MedicationOrder.Id, prescriptionEvent.MedicationOrder.PatientId);
                    // Get the user from the event
                    var patient = await _patientRepository.GetPatientByIdAsync(prescriptionEvent.MedicationOrder.PatientId);
                    if (patient == null)
                    {
                        _logger.LogWarning("Patient with ID {PatientId} not found for MedicationOrderCompleted event", prescriptionEvent.MedicationOrder.PatientId);
                        throw new KeyNotFoundException($"Patient with ID {prescriptionEvent.MedicationOrder.PatientId} not found.");
                    }
                    // Create new MedicalRecord
                    var description = $"Medication order completed: {string.Join(", ", prescriptionEvent.MedicationOrder.Items.Select(m => m.Medication.Name))}";
                    var medicalRecord = new MedicalRecord
                    {
                        Description = description,
                        Date = DateTime.UtcNow
                    };
                    // Add the MedicalRecord to the patient
                    var success = await _patientRepository.AddMedicalRecordAsync(patient.Id, medicalRecord);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added MedicalRecord to Patient ID {PatientId}", patient.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add MedicalRecord to Patient ID {PatientId}", patient.Id);
                        throw new InvalidOperationException($"Failed to add MedicalRecord to Patient ID {patient.Id}");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize MedicationOrderCompleted from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize MedicationOrderCompleted from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing MedicationOrderCompleted event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task PrescriptionChanged(Message message)
        {
            try
            {
                _logger.LogInformation("Received PrescriptionChanged event with ID: {MessageId}", message.MessageId);
                var prescriptionEvent = JsonSerializer.Deserialize<PrescriptionChangedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (prescriptionEvent != null)
                {
                    _logger.LogInformation("Deserialized PrescriptionChanged: Id={Id}, PatientId={PatientId}",
                        prescriptionEvent.Prescription.Id, prescriptionEvent.Prescription.PatientId);
                    // Get the user from the event
                    var patient = await _patientRepository.GetPatientByIdAsync(prescriptionEvent.Prescription.PatientId);
                    if (patient == null)
                    {
                        _logger.LogWarning("Patient with ID {PatientId} not found for PrescriptionChanged event", prescriptionEvent.Prescription.PatientId);
                        throw new KeyNotFoundException($"Patient with ID {prescriptionEvent.Prescription.PatientId} not found.");
                    }
                    // Create new MedicalRecord
                    var description = $"Prescription changed: Medication {prescriptionEvent.Prescription.Medicine}, " +
                        $"Dosage {prescriptionEvent.Prescription.Dose}{prescriptionEvent.Prescription.Unit}";
                    var medicalRecord = new MedicalRecord
                    {
                        Description = description,
                        Date = DateTime.UtcNow
                    };
                    // Add the MedicalRecord to the patient
                    var success = await _patientRepository.AddMedicalRecordAsync(patient.Id, medicalRecord);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added MedicalRecord to Patient ID {PatientId}", patient.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add MedicalRecord to Patient ID {PatientId}", patient.Id);
                        throw new InvalidOperationException($"Failed to add MedicalRecord to Patient ID {patient.Id}");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PrescriptionChanged from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize PrescriptionChanged from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PrescriptionChanged event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task PrescriptionInvoked(Message message)
        {
            try
            {
                _logger.LogInformation("Received PrescriptionInvoked event with ID: {MessageId}", message.MessageId);
                var prescriptionEvent = JsonSerializer.Deserialize<PrescriptionInvokedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (prescriptionEvent != null)
                {
                    _logger.LogInformation("Deserialized PrescriptionInvoked: Id={Id}, PatientId={PatientId}",
                        prescriptionEvent.Prescription.Id, prescriptionEvent.Prescription.PatientId);
                    // Get the user from the event
                    var patient = await _patientRepository.GetPatientByIdAsync(prescriptionEvent.Prescription.PatientId);
                    if (patient == null)
                    {
                        _logger.LogWarning("Patient with ID {PatientId} not found for PrescriptionInvoked event", prescriptionEvent.Prescription.PatientId);
                        throw new KeyNotFoundException($"Patient with ID {prescriptionEvent.Prescription.PatientId} not found.");
                    }
                    // Create new MedicalRecord
                    var description = $"Prescription invoked: Medication {prescriptionEvent.Prescription.Medicine}, " +
                        $"Dosage {prescriptionEvent.Prescription.Dose}{prescriptionEvent.Prescription.Unit}";
                    var medicalRecord = new MedicalRecord
                    {
                        Description = description,
                        Date = DateTime.UtcNow
                    };
                    // Add the MedicalRecord to the patient
                    var success = await _patientRepository.AddMedicalRecordAsync(patient.Id, medicalRecord);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added MedicalRecord to Patient ID {PatientId}", patient.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add MedicalRecord to Patient ID {PatientId}", patient.Id);
                        throw new InvalidOperationException($"Failed to add MedicalRecord to Patient ID {patient.Id}");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PrescriptionInvoked from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize PrescriptionInvoked from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PrescriptionInvoked event: {MessageId}", message.MessageId);
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
                    _logger.LogInformation("Deserialized PrescriptionWritten: Id={Id}, PatientId={PatientId}",
                        prescriptionEvent.Prescription.Id, prescriptionEvent.Prescription.PatientId);
                    // Get the user from the event
                    var patient = await _patientRepository.GetPatientByIdAsync(prescriptionEvent.Prescription.PatientId);
                    if (patient == null)
                    {
                        _logger.LogWarning("Patient with ID {PatientId} not found for PrescriptionWritten event", prescriptionEvent.Prescription.PatientId);
                        throw new KeyNotFoundException($"Patient with ID {prescriptionEvent.Prescription.PatientId} not found.");
                    }
                    // Create new MedicalRecord
                    var description = $"Prescription written: Medication {prescriptionEvent.Prescription.Medicine}, " +
                        $"Dosage {prescriptionEvent.Prescription.Dose}{prescriptionEvent.Prescription.Unit}";
                    var medicalRecord = new MedicalRecord
                    {
                        Description = description,
                        Date = DateTime.UtcNow
                    };
                    // Add the MedicalRecord to the patient
                    var success = await _patientRepository.AddMedicalRecordAsync(patient.Id, medicalRecord);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added MedicalRecord to Patient ID {PatientId}", patient.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add MedicalRecord to Patient ID {PatientId}", patient.Id);
                        throw new InvalidOperationException($"Failed to add MedicalRecord to Patient ID {patient.Id}");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize PrescriptionWritten from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize PrescriptionWritten from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PrescriptionWritten event: {MessageId}", message.MessageId);
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
                    _logger.LogInformation("Deserialized TestCompleted: Id={Id}, PatientId={PatientId}",
                        testEvent.Test.Id, testEvent.Test.PatientId);
                    // Get the user from the event
                    var patient = await _patientRepository.GetPatientByIdAsync(testEvent.Test.PatientId);
                    if (patient == null)
                    {
                        _logger.LogWarning("Patient with ID {PatientId} not found for TestCompleted event", testEvent.Test.PatientId);
                        throw new KeyNotFoundException($"Patient with ID {testEvent.Test.PatientId} not found.");
                    }
                    // Create new MedicalRecord
                    var description = $"Test completed: {testEvent.Test.Description}, Result: {testEvent.Test.Result}";
                    var medicalRecord = new MedicalRecord
                    {
                        Description = description,
                        Date = DateTime.UtcNow
                    };
                    // Add the MedicalRecord to the patient
                    var success = await _patientRepository.AddMedicalRecordAsync(patient.Id, medicalRecord);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added MedicalRecord to Patient ID {PatientId}", patient.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add MedicalRecord to Patient ID {PatientId}", patient.Id);
                        throw new InvalidOperationException($"Failed to add MedicalRecord to Patient ID {patient.Id}");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize TestCompleted from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize TestCompleted from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TestCompleted event: {MessageId}", message.MessageId);
                throw;
            }
        }

        private async Task TestRequested(Message message)
        {
            try
            {
                _logger.LogInformation("Received TestRequested event with ID: {MessageId}", message.MessageId);
                var testEvent = JsonSerializer.Deserialize<TestRequestedEvent>(
                    message.Data.ToString() ?? string.Empty,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (testEvent != null)
                {
                    _logger.LogInformation("Deserialized TestRequested: Id={Id}, PatientId={PatientId}",
                        testEvent.Test.Id, testEvent.Test.PatientId);
                    // Get the user from the event
                    var patient = await _patientRepository.GetPatientByIdAsync(testEvent.Test.PatientId);
                    if (patient == null)
                    {
                        _logger.LogWarning("Patient with ID {PatientId} not found for TestRequested event", testEvent.Test.PatientId);
                        throw new KeyNotFoundException($"Patient with ID {testEvent.Test.PatientId} not found.");
                    }
                    // Create new MedicalRecord
                    var description = $"Test requested: {testEvent.Test.Description}";
                    var medicalRecord = new MedicalRecord
                    {
                        Description = description,
                        Date = DateTime.UtcNow
                    };
                    // Add the MedicalRecord to the patient
                    var success = await _patientRepository.AddMedicalRecordAsync(patient.Id, medicalRecord);
                    if (success)
                    {
                        _logger.LogInformation("Successfully added MedicalRecord to Patient ID {PatientId}", patient.Id);
                    }
                    else
                    {
                        _logger.LogError("Failed to add MedicalRecord to Patient ID {PatientId}", patient.Id);
                        throw new InvalidOperationException($"Failed to add MedicalRecord to Patient ID {patient.Id}");
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize TestRequested from message data: {Data}", message.Data.ToString());
                    throw new JsonException($"Failed to deserialize TestRequested from message data: {message.Data.ToString()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TestRequested event: {MessageId}", message.MessageId);
                throw;
            }
        }
    }
}
