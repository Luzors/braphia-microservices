using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Events;
using Braphia.AppointmentManagement.Query.GetAppointmentsWithin24Hours;
using Infrastructure.Messaging;
using MassTransit;
using MediatR;

public class AppointmentReminderBackgroundService : BackgroundService
{
    private readonly ILogger<AppointmentReminderBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMediator _mediator;
    private IList<AppointmentViewQueryModel> _sendAppointments;

    public AppointmentReminderBackgroundService(
        ILogger<AppointmentReminderBackgroundService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _sendAppointments = [];
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       
        _logger.LogInformation("CheckAppointmentTime");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _sendAppointments = [.. _sendAppointments.Where(a => a.ScheduledTime > DateTime.UtcNow)];
                // Create a scope to resolve dependencies
                // This ensures that we can use scoped services like repositories
                // Scope is a way to manage the lifetime of services in ASP.NET Core
                using var scope = _serviceScopeFactory.CreateScope();

                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var results = await mediator.Send(new GetAppointmentsWithin24HoursQuery());
                foreach (var appointment in results)
                {
                    var match = _sendAppointments.FirstOrDefault(x => x.AppointmentId == appointment.AppointmentId);
                    if (match == null)
                    {
                        var reminderEvent = new AppointmentReminderEvent
                        {
                            AppointmentId = appointment.AppointmentId,
                            PatientId = appointment.PatientId,
                            PatientEmail = appointment.PatientEmail,
                            ScheduledTime = appointment.ScheduledTime
                        };

                        _sendAppointments.Add(appointment);

                        await publishEndpoint.Publish(new Message(reminderEvent), stoppingToken);
                        _logger.LogInformation($"[ReminderEvent sent] Appointment #{appointment.AppointmentId} for patient {appointment.PatientFirstName} {appointment.PatientLastName} at {appointment.ScheduledTime}");
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
            
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
