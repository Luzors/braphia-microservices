using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.WriteDatabase;
using MediatR;
using MongoDB.Driver;

namespace Braphia.AppointmentManagement.Query.GetAppointmentsByPatient
{
    public class GetAppointmentsByPatientIdHandler : IRequestHandler<GetAppointmentsByPatientIdQuery, IEnumerable<AppointmentViewQueryModel>>
    {
        private readonly ReadDbContext _context;
        public GetAppointmentsByPatientIdHandler(ReadDbContext context) => _context = context;

        public async Task<IEnumerable<AppointmentViewQueryModel>> Handle(GetAppointmentsByPatientIdQuery request, CancellationToken cancellationToken)
        {
            var filter = Builders<AppointmentViewQueryModel>.Filter.Eq(a => a.PatientId, request.PatientId);
            return await _context.AppointmentViews.Find(filter).ToListAsync(cancellationToken);
        }
    }

}
