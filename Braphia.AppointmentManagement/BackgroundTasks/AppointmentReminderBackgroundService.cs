using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;
using Braphia.AppointmentManagement.Enums;
using Braphia.AppointmentManagement.Events;
using MassTransit;

public class AppointmentReminderBackgroundService : BackgroundService
{
    private readonly ILogger<AppointmentReminderBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AppointmentReminderBackgroundService(
        ILogger<AppointmentReminderBackgroundService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run the service every hour
        // While stoppingToken is not cancelled, we will check for appointments
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Create a scope to resolve dependencies
                // This ensures that we can use scoped services like repositories
                // Scope is a way to manage the lifetime of services in ASP.NET Core
                using var scope = _serviceScopeFactory.CreateScope();

                var appointmentReadRepository = scope.ServiceProvider.GetRequiredService<IAppointmentReadRepository>();
                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                //Threshold for upcoming appointments is set to 24 hours from now
                var allAppointments = await appointmentReadRepository.GetAllAppointmentsAsync();
                var now = DateTime.UtcNow;
                var threshold = now.AddHours(24);

                var upcomingAppointments = allAppointments
                    .Where(a =>
                        a.ScheduledTime > now &&
                        a.ScheduledTime <= threshold &&
                        a.State == AppointmentStateEnum.CREATED)
                    .ToList();

                foreach (var appointment in upcomingAppointments)
                {
                    var reminderEvent = new AppointmentReminderEvent
                    {
                        AppointmentId = appointment.AppointmentId,
                        PatientId = appointment.PatientId,
                        PatientEmail = appointment.PatientEmail,
                        ScheduledTime = appointment.ScheduledTime
                    };

                    await publishEndpoint.Publish(reminderEvent, stoppingToken);
                    _logger.LogInformation($"[ReminderEvent published] Appointment #{appointment.AppointmentId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while publishing reminder events.");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
