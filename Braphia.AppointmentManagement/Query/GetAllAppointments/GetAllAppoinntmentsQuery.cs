using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using MediatR;

namespace Braphia.AppointmentManagement.Query.GetAllAppointments
{
    public class GetAllAppointmentsQuery : IRequest<IEnumerable<AppointmentViewQueryModel>> { }
}
