using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;
using MediatR;

namespace Braphia.AppointmentManagement.Query.GetAppointmentsWithin24Hours
{
    public class GetAppointmentsWithin24HoursHandler : IRequestHandler<GetAppointmentsWithin24HoursQuery, IEnumerable<AppointmentViewQueryModel>>
    {
        private readonly IAppointmentReadRepository _repository;
        public GetAppointmentsWithin24HoursHandler(IAppointmentReadRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository), "Repository cannot be null.");
        }
        public async Task<IEnumerable<AppointmentViewQueryModel>> Handle(GetAppointmentsWithin24HoursQuery request, CancellationToken cancellationToken)
        {
            var allAppointments = await _repository.GetAllAppointmentsAsync();

            var now = DateTime.UtcNow;
            var in24Hours = now.AddHours(24);

            return allAppointments
                .Where(a =>
                    a.ScheduledTime > now &&
                    a.ScheduledTime <= in24Hours &&
                    a.State == Enums.AppointmentStateEnum.CREATED)
                .ToList();
        }
    
    }
}
