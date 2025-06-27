using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.WriteDatabase;
using MediatR;
using MongoDB.Driver;

namespace Braphia.AppointmentManagement.Query.GetAppointmentByPhysician
{
    public class GetAppointmentsByPhysicianIdHandler : IRequestHandler<GetAppointmentsByPhysicianIdQuery, IEnumerable<AppointmentViewQueryModel>>
    {
        private readonly ReadDbContext _context;
        public GetAppointmentsByPhysicianIdHandler(ReadDbContext context) => _context = context;

        public async Task<IEnumerable<AppointmentViewQueryModel>> Handle(GetAppointmentsByPhysicianIdQuery request, CancellationToken cancellationToken)
        {
            var filter = Builders<AppointmentViewQueryModel>.Filter.Eq(a => a.PhysicianId, request.PhysicianId);
            return await _context.AppointmentViews.Find(filter).ToListAsync(cancellationToken);
        }
    }
}
