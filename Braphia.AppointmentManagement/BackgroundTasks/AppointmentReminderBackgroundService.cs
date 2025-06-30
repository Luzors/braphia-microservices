using MassTransit;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;
using Braphia.AppointmentManagement.Events;

namespace Braphia.AppointmentManagement.BackgroundServices
{
    public class AppointmentReminderBackgroundService : BackgroundService
    {
        private readonly ILogger<AppointmentReminderBackgroundService> _logger;
        private readonly IAppointmentReadRepository _appointmentReadRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public AppointmentReminderBackgroundService(
            ILogger<AppointmentReminderBackgroundService> logger,
            IAppointmentReadRepository appointmentReadRepository,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _appointmentReadRepository = appointmentReadRepository;
            _publishEndpoint = publishEndpoint;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var allAppointments = await _appointmentReadRepository.GetAllAppointmentsAsync();
                    var now = DateTime.UtcNow;
                    var threshold = now.AddHours(24);

                    var upcomingAppointments = allAppointments
                        .Where(a =>
                            a.ScheduledTime > now &&
                            a.ScheduledTime <= threshold &&
                            a.State == Enums.AppointmentStateEnum.CREATED)
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

                        await _publishEndpoint.Publish(reminderEvent, stoppingToken);
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
}
