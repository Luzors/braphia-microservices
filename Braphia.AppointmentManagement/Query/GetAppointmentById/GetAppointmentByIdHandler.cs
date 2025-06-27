using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.WriteDatabase;
using MediatR;
using MongoDB.Driver;

namespace Braphia.AppointmentManagement.Query.GetAppointmentById
{
    public class GetAppointmentByIdHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentViewQueryModel>
    {
        private readonly ReadDbContext _context;
        public GetAppointmentByIdHandler(ReadDbContext context) => _context = context;

        public async Task<AppointmentViewQueryModel> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
        {
            var filter = Builders<AppointmentViewQueryModel>.Filter.Eq(a => a.AppointmentId, request.AppointmentId);
            return await _context.AppointmentViews.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }
    }

}
