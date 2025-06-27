using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using Braphia.AppointmentManagement.Databases.ReadDatabase.Repository.Interface;
using Braphia.AppointmentManagement.Databases.WriteDatabase;
using Microsoft.EntityFrameworkCore;

namespace Braphia.AppointmentManagement.Databases.ReadDatabase.Repository
{
    public class SQLAppointmentReadRepository : IAppointmentReadRepository
    {
        private readonly ReadDbContext _context;

        public SQLAppointmentReadRepository(ReadDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context must be of type ReadDbContext.");
        }
        public async Task<bool> AddAppointmentAsync(AppointmentViewQueryModel appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            _context.AppointmentViewQueryModels.Add(appointment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAppointmentAsync(int appointmentId)
        {
            var appointment = await GetAppointmentByIdAsync(appointmentId);
            if (appointment == null) return false;
            _context.AppointmentViewQueryModels.Remove(appointment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<AppointmentViewQueryModel>> GetAllAppointmentsAsync()
        {
            return await _context.AppointmentViewQueryModels.ToListAsync();
        }

        public async Task<AppointmentViewQueryModel> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _context.AppointmentViewQueryModels
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId)
                ?? throw new ArgumentException($"Appointment with ID {appointmentId} not found.");
            return appointment;
        }

        public async Task<IEnumerable<AppointmentViewQueryModel>> GetAppointmentsByPatientIdAsync(int patientId)
        {   
            return await _context.AppointmentViewQueryModels
                .Where(a => a.PatientId == patientId)
                .ToListAsync() 
                ?? throw new ArgumentException($"No appointments found for Patient ID {patientId}.");
        }

        public async Task<IEnumerable<AppointmentViewQueryModel>> GetAppointmentsByPhysicianIdAsync(int physicianId)
        {
            return await _context.AppointmentViewQueryModels
                .Where(a => a.PhysicianId == physicianId)
                .ToListAsync() 
                ?? throw new ArgumentException($"No appointments found for Physician ID {physicianId}.");
        }

        public async Task<IEnumerable<AppointmentViewQueryModel>> GetAppointmentsOfTodayAsync()
        {
            var today = DateTime.Today;
            return await _context.AppointmentViewQueryModels
                .Where(a => a.ScheduledTime.Date == today)
                .ToListAsync() 
                ?? throw new ArgumentException("No appointments found for today.");
        }
        //TODO : Add all of the items from the AppointmentViewQueryModel to the UpdateAppointmentAsync method
        public async Task<bool> UpdateAppointmentAsync(AppointmentViewQueryModel appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            var existingAppointment = await GetAppointmentByIdAsync(appointment.AppointmentId);
            existingAppointment.PatientId = appointment.PatientId;
            existingAppointment.PhysicianId = appointment.PhysicianId;
            existingAppointment.ScheduledTime = appointment.ScheduledTime;
            existingAppointment.StateName = appointment.StateName;
            existingAppointment.ReferralId = appointment.ReferralId;
            _context.AppointmentViewQueryModels.Update(existingAppointment);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
