using MediatR;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;

public class GetAppointmentsByPhysicianIdHandler : IRequestHandler<GetAppointmentsByPhysicianIdQuery, IEnumerable<AppointmentViewQueryModel>>
{
    private readonly IAppointmentReadRepository _repository;

    public GetAppointmentsByPhysicianIdHandler(IAppointmentReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AppointmentViewQueryModel>> Handle(GetAppointmentsByPhysicianIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAppointmentsByPhysicianIdAsync(request.PhysicianId);
    }
}
