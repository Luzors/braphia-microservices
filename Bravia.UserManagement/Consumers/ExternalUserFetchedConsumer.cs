using Braphia.UserManagement.Events;
using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using MassTransit;
using System.Collections;
using System.Text;
using System.Text.Json;

namespace Braphia.UserManagement.Consumers
{
    /// <summary>
    /// This consumer responds to events from the external csv fetch. 
    /// However it could still be improved since now both the publish and consume happens in the same project, 
    /// in reality the publish side should be done in a different project / app so the queue will continue to fill up 
    /// with changes while the consumer is down for whatever reason
    /// </summary>
    public class ExternalUserFetchedConsumer : IConsumer<ExternalUserFetchedEvent>
    {
        private readonly ILogger<ExternalUserFetchedConsumer> _logger;
        private readonly IPatientRepository _patientRepository;

        public ExternalUserFetchedConsumer(ILogger<ExternalUserFetchedConsumer> logger, IPatientRepository patientRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository), "Patient Repository cannot be null.");
        }

        public async Task Consume(ConsumeContext<ExternalUserFetchedEvent> context)
        {
            try
            {
                // Convert object into patient
                var result = JsonSerializer.Deserialize<JsonElement>(context.Message.ExternalUserData);
                var emailBuilder = new StringBuilder();
                var patient = new Patient()
                {
                    FirstName = result.GetProperty("First Name").GetString() ?? throw new ArgumentNullException("First Name is Missing"),
                    LastName = result.GetProperty("Last Name").GetString() ?? throw new ArgumentNullException("Last Name is Missing"),
                    Email = emailBuilder.Append(result.GetProperty("First Name").GetString()!.ToLower())
                                .Append('.')
                                .Append(result.GetProperty("Last Name").GetString()!.ToLower())
                                .Append("@example.com").ToString(),
                    PhoneNumber = result.GetProperty("Phone Number").GetString() ?? throw new ArgumentNullException("Last Name is Missing")
                };
                // Check if the patient exists in the database
                var existingPatient = await _patientRepository.GetPatientByFullNameAsync(patient.FirstName, patient.LastName);
                // If not, add the patient to the database
                if (existingPatient == null)
                {
                    _logger.LogInformation("Adding new patient: {FirstName} {LastName}", patient.FirstName, patient.LastName);
                    await _patientRepository.AddPatientAsync(patient);
                }
                else
                {
                    // If it exists, check if the data is different and update it if necessary (don't update id or enums)
                    var changed = false;
                    foreach (var property in typeof(Patient).GetProperties())
                    {
                        if (property.Name == nameof(Patient.Id) || typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                            continue;

                        var existingValue = property.GetValue(existingPatient);
                        var newValue = property.GetValue(patient);

                        if (!Equals(existingValue, newValue))
                        {
                            property.SetValue(existingPatient, newValue);
                            changed = true;
                        }
                    }
                    if (changed)
                        await _patientRepository.UpdatePatientAsync(existingPatient);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error processing record: {Record}", JsonSerializer.Serialize(context.Message.ExternalUserData));
                throw;
            }
        }
    }
}
