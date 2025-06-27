using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;
using Braphia.AppointmentManagement.Query.GetAllAppointments;
using MediatR;

public class GetAllAppointmentsHandler : IRequestHandler<GetAllAppointmentsQuery, IEnumerable<AppointmentViewQueryModel>>
{
    private readonly IAppointmentReadRepository _repository;

    public GetAllAppointmentsHandler(IAppointmentReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AppointmentViewQueryModel>> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAppointmentsAsync();
    }
}