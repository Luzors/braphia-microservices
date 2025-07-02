using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Models;
using MediatR;

namespace Braphia.AppointmentManagement.Query.GetAppointmentsWithin24Hours
{
    public class GetAppointmentsWithin24HoursQuery : IRequest<IEnumerable<AppointmentViewQueryModel>>
    {
    }
}
