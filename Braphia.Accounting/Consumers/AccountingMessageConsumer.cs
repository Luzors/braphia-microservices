using Braphia.Accounting.Converters;
using Braphia.Accounting.Events;
using Braphia.Accounting.EventSourcing.Events;
using Braphia.Accounting.EventSourcing.Services;
using Braphia.Accounting.Models;
using Braphia.Accounting.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using System.Linq;
using System.Text.Json;

namespace Braphia.Accounting.Consumers
{
    public class AccountingMessageConsumer : IConsumer<Message>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceEventService _invoiceEventService;
        private readonly ILogger<AccountingMessageConsumer> _logger;

        public AccountingMessageConsumer(
            IPatientRepository patientRepository,
            IInvoiceRepository invoiceRepository,
            IInvoiceEventService invoiceEventService,
            ILogger<AccountingMessageConsumer> logger)
        {
            _patientRepository = patientRepository;
            _invoiceRepository = invoiceRepository;
            _invoiceEventService = invoiceEventService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            var message = context.Message;

            _logger.LogInformation("Received message in accounting of type: {MessageType} with ID: {MessageId}", message.MessageType, message.MessageId);

            if (message.MessageType == "PatientRegistered")
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

                        var success = await _patientRepository.AddPatientAsync(patient);

                        if (success)
                        {
                            _logger.LogInformation("Successfully added patient from UserManagement ID {OriginalPatientId} to accounting database with new ID {NewPatientId}",
                                patientEvent.Patient.Id, patient.Id);
                        }
                        else
                        {
                            _logger.LogError("Failed to add patient from UserManagement ID {OriginalPatientId} to accounting database", patientEvent.Patient.Id);
                            throw new InvalidOperationException($"Failed to add patient from UserManagement ID {patientEvent.Patient.Id} to accounting database");
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize PatientRegisteredEvent from message data: {Data}", message.Data.ToString());
                        throw new InvalidOperationException("Failed to deserialize PatientRegisteredEvent from message data");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing PatientCreated event: {MessageId}", message.MessageId);
                    throw;
                }
            }
            else if (message.MessageType == "TestCompleted")
            {
                try
                {
                    _logger.LogInformation("Received TestCompleted event with ID: {MessageId}", message.MessageId);
                    var serializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new DecimalJsonConverter() }
                    };
                    
                    var labTestEvent = JsonSerializer.Deserialize<TestCompletedEvent>(
                        message.Data.ToString() ?? string.Empty,
                        serializerOptions
                    );

                    if (labTestEvent != null)
                    {
                        _logger.LogInformation("Deserialized lab test data: Id={Id}, PatientId={PatientId}, TestType={TestType}, Cost={Cost}",
                            labTestEvent.Test.Id, labTestEvent.Test.PatientId, labTestEvent.Test.TestType, labTestEvent.Test.Cost);

                        // Find the patient in the accounting database
                        var patient = await _patientRepository.GetPatientByIdAsync(labTestEvent.Test.PatientId);
                        if (patient == null)
                        {
                            _logger.LogError("Patient with ID {PatientId} not found in accounting database", labTestEvent.Test.PatientId);
                            throw new InvalidOperationException($"Patient with ID {labTestEvent.Test.PatientId} not found in accounting database");
                        }

                        // Check if patient has an insurer
                        if (patient.InsurerId == null)
                        {
                            _logger.LogWarning("Patient {PatientId} does not have an associated insurer. Cannot create invoice.", labTestEvent.Test.PatientId);
                            throw new InvalidOperationException($"Patient {labTestEvent.Test.PatientId} does not have an associated insurer. Cannot create invoice.");
                        }

                        // Create invoice through event sourcing service
                        string description = $"Lab Test: {labTestEvent.Test.TestType} - {labTestEvent.Test.Description}".Trim(' ', '-');

                        try
                        {
                            int invoiceId = await _invoiceEventService.CreateInvoiceAsync(
                                patient.Id,
                                patient.InsurerId.Value,
                                labTestEvent.Test.Cost,
                                description);

                            _logger.LogInformation("Successfully created invoice {InvoiceId} through event sourcing for lab test for patient {PatientId} and insurer {InsurerId}",
                                invoiceId, patient.Id, patient.InsurerId.Value);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to create invoice through event sourcing for lab test for patient {PatientId}",
                                patient.Id);
                            throw new InvalidOperationException($"Failed to create invoice through event sourcing for lab test for patient {patient.Id}", ex);
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize TestCompleted event from message data: {Data}", message.Data.ToString());
                        throw new InvalidOperationException("Failed to deserialize TestCompleted event from message data");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing TestCompleted event: {MessageId}", message.MessageId);
                    throw;
                }
            }
            else if (message.MessageType == "MedicationOrderCompleted")
            {
                try
                {
                    _logger.LogInformation("Received MedicationOrderCompleted event with ID: {MessageId}", message.MessageId);
                    var serializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new DecimalJsonConverter() }
                    };

                    var medicationOrderEvent = JsonSerializer.Deserialize<MedicationOrderCompletedEvent>(
                        message.Data.ToString() ?? string.Empty,
                        serializerOptions
                    );

                    if (medicationOrderEvent != null)
                    {
                        _logger.LogInformation("Deserialized medication order data: Id={Id}, PatientId={PatientId}, TotalCost={TotalCost}",
                            medicationOrderEvent.MedicationOrder.Id, medicationOrderEvent.MedicationOrder.PatientId, medicationOrderEvent.MedicationOrder.CalculateTotalPrice());

                        var patient = await _patientRepository.GetPatientByIdAsync(medicationOrderEvent.MedicationOrder.PatientId);
                        if (patient == null)
                        {
                            _logger.LogError("Patient with ID {PatientId} not found in accounting database", medicationOrderEvent.MedicationOrder.PatientId);
                            throw new InvalidOperationException($"Patient with ID {medicationOrderEvent.MedicationOrder.PatientId} not found in accounting database");
                        }

                        if (patient.InsurerId == null)
                        {
                            _logger.LogWarning("Patient {PatientId} does not have an associated insurer. Cannot create invoice.", medicationOrderEvent.MedicationOrder.PatientId);
                            throw new InvalidOperationException($"Patient {medicationOrderEvent.MedicationOrder.PatientId} does not have an associated insurer. Cannot create invoice.");
                        }

                        string description = $"Medication Order - {medicationOrderEvent.MedicationOrder.Id} - {string.Join(", ", medicationOrderEvent.MedicationOrder.Items.Select(x => x.Medication.Name))}".Trim(' ', '-');
                        try
                        {
                            int invoiceId = await _invoiceEventService.CreateInvoiceAsync(
                                patient.Id,
                                patient.InsurerId.Value,
                                medicationOrderEvent.MedicationOrder.CalculateTotalPrice(),
                                description);
                            _logger.LogInformation("Successfully created invoice {InvoiceId} through event sourcing for medication order {MedicationOrderId} for patient {PatientId} and insurer {InsurerId}",
                                invoiceId, medicationOrderEvent.MedicationOrder.Id, patient.Id, patient.InsurerId.Value);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to create invoice through event sourcing for medication order {MedicationOrderId} for patient {PatientId}",
                                medicationOrderEvent.MedicationOrder.Id, patient.Id);
                            throw new InvalidOperationException($"Failed to create invoice through event sourcing for medication order {medicationOrderEvent.MedicationOrder.Id} for patient {patient.Id}", ex);
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize MedicationOrderCompleted event from message data: {Data}", message.Data.ToString());
                        throw new InvalidOperationException("Failed to deserialize MedicationOrderCompleted event from message data");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing MedicationOrderCompleted event: {MessageId}", message.MessageId);
                    throw;
                }
            }
        }
    }
}