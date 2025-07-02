using Braphia.AppointmentManagement.Models;
using MediatR;

namespace Braphia.AppointmentManagement.Commands.QuestionnaireAnswered
{
    public class QuestionnaireAnsweredCommand : IRequest<int>
    {
        public int AppointmentId { get; set; }

        public string Answers { get; set; }
    }
}
