using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;
using Braphia.AppointmentManagement.Databases.WriteDatabase;
using MongoDB.Driver;

namespace Braphia.AppointmentManagement.Databases.ReadDatabase.Repository
{
    public class AppointmentReadRepository : IMongoAppointmentReadRepository
    {
        private readonly ReadDbContext _context;
        public AppointmentReadRepository(ReadDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddAppointmentAsync(AppointmentViewQueryModel appointment)
        {
            await _context.AppointmentViews.InsertOneAsync(appointment);
            return true;
        }
        public async Task<bool> UpdateAppointmentAsync(AppointmentViewQueryModel appointment)
        {
            var filter = Builders<AppointmentViewQueryModel>.Filter.Eq(a => a.AppointmentId, appointment.AppointmentId);
            var result = await _context.AppointmentViews.ReplaceOneAsync(filter, appointment);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
        public async Task<bool> DeleteAppointmentAsync(int appointmentId)
        {
            var filter = Builders<AppointmentViewQueryModel>.Filter.Eq(a => a.AppointmentId, appointmentId);
            var result = await _context.AppointmentViews.DeleteOneAsync(filter);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
        //public async Task<AppointmentViewQueryModel> GetAppointmentByIdAsync(Guid appointmentId)
        //{
        //    var filter = Builders<AppointmentViewQueryModel>.Filter.Eq(a => a.AppointmentId, appointmentId);
        //    return await _context.AppointmentViews.Find(filter).FirstOrDefaultAsync();
        //}
        //public async Task<IEnumerable<AppointmentViewQueryModel>> GetAllAppointmentsAsync()
        //{
        //    return await _context.AppointmentViews.Find(_ => true).ToListAsync();
        //}
        //public async Task<IEnumerable<AppointmentViewQueryModel>> GetAppointmentsByPatientIdAsync(Guid patientId)
        //{
        //    var filter = Builders<AppointmentViewQueryModel>.Filter.Eq(a => a.PatientId, patientId);
        //    return await _context.AppointmentViews.Find(filter).ToListAsync();
        //}
        //public async Task<IEnumerable<AppointmentViewQueryModel>> GetAppointmentsByPhysicianIdAsync(Guid physicianId)
        //{
        //    var filter = Builders<AppointmentViewQueryModel>.Filter.Eq(a => a.PhysicianId, physicianId);
        //    return await _context.AppointmentViews.Find(filter).ToListAsync();
        //}
        //public async Task<IEnumerable<AppointmentViewQueryModel>> GetAppointmentsOfTodayAsync()
        //{
        //    var today = DateTime.UtcNow.Date;
        //    var filter = Builders<AppointmentViewQueryModel>.Filter.Gte(a => a.ScheduledTime, today) &
        //                 Builders<AppointmentViewQueryModel>.Filter.Lt(a => a.ScheduledTime, today.AddDays(1));
        //    return await _context.AppointmentViews.Find(filter).ToListAsync();
        //}
    }
}
