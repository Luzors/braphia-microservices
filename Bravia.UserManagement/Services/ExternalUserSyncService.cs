
using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Braphia.UserManagement.Services
{
    public class ExternalUserSyncService : BackgroundService
    {
        private readonly ILogger<ExternalUserSyncService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly string _externalUrl;

        public ExternalUserSyncService(ILogger<ExternalUserSyncService> logger, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _externalUrl = configuration.GetSection("ExternalUserSync").GetValue<string>("Url") ?? throw new ArgumentNullException("ExternalUserSyncUrl");
        }

        /// <summary>
        /// Executes the background service to synchronize external users. <- lifetime is managed by .net host
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting BackgroundService for fetching external users");
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("ExternalUserSyncService is running at {StartTime}", DateTime.Now);

                try
                {
                    await SyncExternalUsersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while synchronizing external users.");
                }

                var currentTimestamp = DateTime.UtcNow;
                var nextSyncTime = DateTime.UtcNow.Date.AddDays(DateTime.UtcNow.Hour >= 2 ? 1 : 0).AddHours(2);
                var timeToNextSync = nextSyncTime - currentTimestamp;

                _logger.LogDebug("Next sync scheduled at {NextSyncTime}", nextSyncTime);
                await Task.Delay(timeToNextSync, stoppingToken);
            }
        }

        /// <summary>
        /// The actual logic to get the external csv an dparse into patients
        /// TODO: Should this put everything into a queue instead of directly into the database? 
        ///       Seems a bit overkill since it is running in the same project
        /// </summary>
        private async Task SyncExternalUsersAsync(CancellationToken stoppingToken)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var patientRepository = scope.ServiceProvider.GetRequiredService<IPatientRepository>();

            // Gets the csv from the url + parses them inot Patients using CsvHelper Nuget
            var httpClient = new HttpClient();
            var csvStream = await httpClient.GetStreamAsync(_externalUrl, stoppingToken);

            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                try
                {
                    var strBuilder = new StringBuilder();
                    var patient = new Patient()
                    {
                        FirstName = csv.GetField<string>("First Name") ?? throw new ArgumentNullException("First Name is required"),
                        LastName = csv.GetField<string>("Last Name") ?? throw new ArgumentNullException("Last Name is required"),
                        Email = strBuilder.Append(csv.GetField<string>("First Name")!.ToLower().Split("")[0])
                                          .Append('.')
                                          .Append(csv.GetField<string>("Last Name")!.ToLower())
                                          .Append("@example.com")
                                          .ToString(),
                        PhoneNumber = csv.GetField<string>("Phone Number") ?? throw new ArgumentNullException("Phone Number is required"),
                    };

                    // Check if the patient exists in the database
                    var existingPatient = await patientRepository.GetPatientByFullNameAsync(patient.FirstName, patient.LastName);
                    // If not, add the patient to the database
                    if (existingPatient == null)
                    {
                        _logger.LogInformation("Adding new patient: {FirstName} {LastName}", patient.FirstName, patient.LastName);
                        await patientRepository.AddPatientAsync(patient);
                    }
                    else
                    {
                        // If it exists, check if the data is different and update it if necessary
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
                            await patientRepository.UpdatePatientAsync(existingPatient);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error processing record: {Record}", JsonSerializer.Serialize(csv.GetRecord<object>()));
                    continue; // Skip this record and continue with the next one
                }
            }

        }
    }
}
