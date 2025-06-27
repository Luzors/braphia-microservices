using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;
using Braphia.AppointmentManagement.Query.GetAppointmentById;
using MediatR;

public class GetAppointmentByIdHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentViewQueryModel>
{
    private readonly IAppointmentReadRepository _repository;

    public GetAppointmentByIdHandler(IAppointmentReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<AppointmentViewQueryModel> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAppointmentByIdAsync(request.AppointmentId);
    }
}