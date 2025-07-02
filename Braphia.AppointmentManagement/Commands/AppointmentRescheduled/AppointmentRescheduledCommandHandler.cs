using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Events;
using Braphia.AppointmentManagement.Events.InternalEvents;
using Infrastructure.Messaging;
using MassTransit;
using MediatR;

namespace Braphia.AppointmentManagement.Commands.AppointmentRescheduled
{
    public class AppointmentRescheduledCommandHandler : IRequestHandler<AppointmentRescheduledCommand, int>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPublishEndpoint _publishEndpoint; 
       
        
        public AppointmentRescheduledCommandHandler(
            IAppointmentRepository appointmentRepository,
            IPublishEndpoint publishEndpoint)
        {
            _appointmentRepository = appointmentRepository;
            _publishEndpoint = publishEndpoint;
           
        }
        //De wijziging van tijd staat niet goed in de read database, hij lijkt wel de datum van vandaag te pakken
        public async Task<int> Handle(AppointmentRescheduledCommand request, CancellationToken cancellationToken)
        {

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(request.AppointmentId);
            if (appointment == null)
                throw new ArgumentException($"Appointment with ID {request.AppointmentId} not found.");

            
            appointment.ScheduledTime = request.NewScheduledTime;

            var success = await _appointmentRepository.UpdateAppointmentAsync(appointment);
            if (!success)
                throw new InvalidOperationException("Failed to update appointment in the repository.");

            var @event = new AppointmentRescheduledEvent
            {
                AppointmentId = appointment.Id,
                NewScheduledTime = request.NewScheduledTime
            };

            var mes = new Message(@event);


            await _publishEndpoint.Publish(mes);

            return appointment.Id;


        }
    }
}
