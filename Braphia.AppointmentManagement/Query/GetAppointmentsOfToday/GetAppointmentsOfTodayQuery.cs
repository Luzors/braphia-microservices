using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using MediatR;

namespace Braphia.AppointmentManagement.Query.GetAppointmentsOfToday
{
    public class GetAppointmentsOfTodayQuery : IRequest<IEnumerable<AppointmentViewQueryModel>> { }
}
