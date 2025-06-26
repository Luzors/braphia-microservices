
using Braphia.UserManagement.Events;
using Braphia.UserManagement.Repositories.Interfaces;
using CsvHelper;
using MassTransit;
using System.Globalization;
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
        /// Pushes the result of the external usercsv onto our rabbitQuee
        /// </summary>
        private async Task SyncExternalUsersAsync(CancellationToken stoppingToken)
        {
            try
            {

                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                var patientRepository = scope.ServiceProvider.GetRequiredService<IPatientRepository>();
                var sendEndpointProvider = scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();

                // Gets the csv from the url + parses them inot dynamic to publish
                var httpClient = new HttpClient();
                var csvStream = await httpClient.GetStreamAsync(_externalUrl, stoppingToken);

                using var reader = new StreamReader(csvStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecords<dynamic>();
                var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:external-user-queue"));
                await sendEndpoint.SendBatch(records.Select(record => new ExternalUserFetchedEvent(JsonSerializer.Serialize(record))), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while getting external users");
            }
        }
    }
}
