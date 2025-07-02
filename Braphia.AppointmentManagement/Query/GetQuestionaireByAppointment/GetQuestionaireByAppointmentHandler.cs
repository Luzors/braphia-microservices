using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;
using MediatR;

namespace Braphia.AppointmentManagement.Query.GetQuestionaireByAppointment
{
    public class GetQuestionaireByAppointmentHandler : IRequestHandler<GetQuestionaireByAppointmentQuery, IEnumerable<string>>
    {
        private readonly IAppointmentReadRepository _repository;

        public GetQuestionaireByAppointmentHandler(IAppointmentReadRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository), "Repository cannot be null.");
        }
        public async Task<IEnumerable<string>> Handle(GetQuestionaireByAppointmentQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");

            var questionnaire = await _repository.GetQuestionareAsync(request.AppointmentId);
            
            if (questionnaire == null || !questionnaire.Any())
                throw new ArgumentException($"No questionnaire found for appointment with ID {request.AppointmentId}.");

            return questionnaire;
        }

    }

}
