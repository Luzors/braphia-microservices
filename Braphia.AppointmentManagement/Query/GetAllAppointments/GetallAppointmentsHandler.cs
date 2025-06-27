using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.WriteDatabase;
using MediatR;
using MongoDB.Driver;

namespace Braphia.AppointmentManagement.Query.GetAllAppointments
{
    public class GetAllAppointmentsHandler : IRequestHandler<GetAllAppointmentsQuery, IEnumerable<AppointmentViewQueryModel>>
    {
        private readonly ReadDbContext _context;
        public GetAllAppointmentsHandler(ReadDbContext context) => _context = context;

        public async Task<IEnumerable<AppointmentViewQueryModel>> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
            => await _context.AppointmentViews.Find(_ => true).ToListAsync(cancellationToken);
    }

}
