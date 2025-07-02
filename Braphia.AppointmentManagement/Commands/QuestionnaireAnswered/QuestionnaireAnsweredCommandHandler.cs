using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Events.InternalEvents;
using Infrastructure.Messaging;
using MassTransit;
using MediatR;

namespace Braphia.AppointmentManagement.Commands.QuestionnaireAnswered
{
    public class QuestionnaireAnsweredCommandHandler : IRequestHandler<QuestionnaireAnsweredCommand, int>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly  ISendEndpointProvider _sendEndpointProvider;

        public QuestionnaireAnsweredCommandHandler(IAppointmentRepository appointmentRepository, IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository), "Appointment repository cannot be null.");
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint), "Publish endpoint cannot be null.");
            _sendEndpointProvider = sendEndpointProvider;
        }
        public async Task<int> Handle(QuestionnaireAnsweredCommand request, CancellationToken cancellationToken)
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:internal-event-queue"));
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");
            }
            Console.WriteLine($"Handling QuestionnaireAnsweredCommand for Appointment ID: {request.AppointmentId}");
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(request.AppointmentId);
            if (appointment == null)
            {
                throw new ArgumentNullException( "Appointment cannot be null.");

            }
            appointment.PreAppointmentQuestionnaire = request.Answers;
            appointment.PreAppointmentQuestionnaireFilled();
            Console.WriteLine(appointment.IsPreAppointmentQuestionnaireFilled);
            var result = await _appointmentRepository.UpdateAppointmentAsync(appointment);
            if (result == null)
            {
                throw new ArgumentNullException("Result cannot be null.");

            }

            var @event = new InternalPreAppointmentQuestionairFilledInEvent
            {
                AppointmentId = request.AppointmentId,
                answers = request.Answers,
            };
            var message = new Message(@event);

            await sendEndpoint.Send(message,cancellationToken);

            return request.AppointmentId; 


        }
    }
}
