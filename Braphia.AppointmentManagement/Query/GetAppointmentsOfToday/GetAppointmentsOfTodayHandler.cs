using MediatR;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;

public class GetAppointmentsOfTodayHandler : IRequestHandler<GetAppointmentsOfTodayQuery, IEnumerable<AppointmentViewQueryModel>>
{
    private readonly IAppointmentReadRepository _repository;

    public GetAppointmentsOfTodayHandler(IAppointmentReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AppointmentViewQueryModel>> Handle(GetAppointmentsOfTodayQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAppointmentsOfTodayAsync();
    }
}
