using MediatR;

namespace Braphia.AppointmentManagement.Query.GetQuestionaireByAppointment
{
    public class GetQuestionaireByAppointmentQuery : IRequest<IEnumerable<string>>
    {
        public int AppointmentId { get; set; }
        public GetQuestionaireByAppointmentQuery(int appointmentId)
        {
            AppointmentId = appointmentId;
        }
    }
}
