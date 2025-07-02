using MediatR;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;

public class GetAppointmentsByPhysicianIdQuery : IRequest<IEnumerable<AppointmentViewQueryModel>>
{
    public int PhysicianId { get; }

    public GetAppointmentsByPhysicianIdQuery(int physicianId)
    {
        PhysicianId = physicianId;
    }
}
