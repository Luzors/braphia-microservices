using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.WriteDatabase;
using MediatR;
using MongoDB.Driver;

namespace Braphia.AppointmentManagement.Query.GetAppointmentsOfToday
{
    public class GetAppointmentsOfTodayHandler : IRequestHandler<GetAppointmentsOfTodayQuery, IEnumerable<AppointmentViewQueryModel>>
    {
        private readonly ReadDbContext _context;
        public GetAppointmentsOfTodayHandler(ReadDbContext context) => _context = context;

        public async Task<IEnumerable<AppointmentViewQueryModel>> Handle(GetAppointmentsOfTodayQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var filter = Builders<AppointmentViewQueryModel>.Filter.Gte(a => a.ScheduledTime, today) &
                         Builders<AppointmentViewQueryModel>.Filter.Lt(a => a.ScheduledTime, today.AddDays(1));
            return await _context.AppointmentViews.Find(filter).ToListAsync(cancellationToken);
        }
    }
}
