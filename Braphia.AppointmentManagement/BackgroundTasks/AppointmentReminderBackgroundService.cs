using Braphia.AppointmentManagement.Events;
using Braphia.AppointmentManagement.Query.GetAppointmentsWithin24Hours;
using MassTransit;
using MediatR;

public class AppointmentReminderBackgroundService : BackgroundService
{
    private readonly ILogger<AppointmentReminderBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMediator _mediator;

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
        _logger.LogInformation("CheckAppointmentTime");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Create a scope to resolve dependencies
                // This ensures that we can use scoped services like repositories
                // Scope is a way to manage the lifetime of services in ASP.NET Core
                using var scope = _serviceScopeFactory.CreateScope();

                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var notificationsSend = new List<int>();


                var results = await mediator.Send(new GetAppointmentsWithin24HoursQuery());
     

                _logger.LogInformation("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                _logger.LogInformation(results.ToString());

                foreach (var appointment in results)
                {
                    var match = notificationsSend.FirstOrDefault(x => x == appointment.AppointmentId);
                    if (match == null)
                    {
                        var reminderEvent = new AppointmentReminderEvent
                        {
                            AppointmentId = appointment.AppointmentId,
                            PatientId = appointment.PatientId,
                            PatientEmail = appointment.PatientEmail,
                            ScheduledTime = appointment.ScheduledTime
                        };

                        notificationsSend.Add(appointment.AppointmentId);

                        await publishEndpoint.Publish(reminderEvent, stoppingToken);
                        _logger.LogInformation($"[ReminderEvent published] Appointment #{appointment.AppointmentId}");
                        _logger.LogInformation($"[ReminderEvent published] Patient #{appointment.PatientId}");
                        _logger.LogInformation($"[ReminderEvent published] ScheduledTime {appointment.ScheduledTime}");
                    }
                    else
                    {
                        _logger.LogInformation($"[ReminderEvent already sent] Appointment #{appointment.AppointmentId}");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while publishing reminder events.");
            }
            //check elke minuut
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            //await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
