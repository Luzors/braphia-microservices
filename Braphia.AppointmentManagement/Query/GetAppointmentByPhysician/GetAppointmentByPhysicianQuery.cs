using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using MediatR;

namespace Braphia.AppointmentManagement.Query.GetAppointmentByPhysician
{
    public class GetAppointmentsByPhysicianIdQuery : IRequest<IEnumerable<AppointmentViewQueryModel>>
    {
        public int PhysicianId { get; set; }
        public GetAppointmentsByPhysicianIdQuery(int physicianId) => PhysicianId = physicianId;
    }
}
