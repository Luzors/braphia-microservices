using Braphia.AppointmentManagement.Models;

namespace Braphia.AppointmentManagement.Commands.QuestionnaireAnswered
{
    public class QuestionnaireAnsweredCommand
    {
        public int AppointmentId { get; set; }

        public string Answers { get; set; }
    }
}
