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

        public async Task<bool> AddFollowUpAppointment(AppointmentViewQueryModel appointment, int originalAppointmentId)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Follow-up appointment cannot be null.");
            var originalAppointment = await GetAppointmentByIdAsync(originalAppointmentId);
            if (originalAppointment == null)
                throw new ArgumentException($"Original appointment with ID {originalAppointmentId} not found.");
            var followUpAppointment = await AddAppointmentAsync(appointment);
            if (!followUpAppointment)
                throw new InvalidOperationException("Failed to add follow-up appointment.");

            originalAppointment.FollowUpAppointmentId = appointment.AppointmentId;

            var result = await UpdateAppointmentAsync(originalAppointment);
            if (!result)
                throw new InvalidOperationException("Failed to update original appointment with follow-up appointment ID.");
            return result;
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
        public async Task<bool> UpdateAppointmentAsync(AppointmentViewQueryModel appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            var existingAppointment = await GetAppointmentByIdAsync(appointment.AppointmentId);
            existingAppointment.PatientId = appointment.PatientId;
            existingAppointment.PatientFirstName = appointment.PatientFirstName;
            existingAppointment.PatientLastName = appointment.PatientLastName;
            existingAppointment.PatientEmail = appointment.PatientEmail;
            existingAppointment.PatientPhoneNumber = appointment.PatientPhoneNumber;
            existingAppointment.PhysicianId = appointment.PhysicianId;
            existingAppointment.PhysicianFirstName = appointment.PhysicianFirstName;
            existingAppointment.PhysicianLastName = appointment.PhysicianLastName;
            existingAppointment.PhysicianSpecialization = appointment.PhysicianSpecialization;
            existingAppointment.ReceptionistId = appointment.ReceptionistId;
            existingAppointment.ReceptionistFirstName = appointment.ReceptionistFirstName;
            existingAppointment.ReceptionistLastName = appointment.ReceptionistLastName;
            existingAppointment.ReceptionistEmail = appointment.ReceptionistEmail;
            existingAppointment.ScheduledTime = appointment.ScheduledTime;
            existingAppointment.State = appointment.State;
            existingAppointment.ReferralId = appointment.ReferralId;
            existingAppointment.ReferralDate = appointment.ReferralDate;
            existingAppointment.ReferralReason = appointment.ReferralReason;

            _context.AppointmentViewQueryModels.Update(existingAppointment);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UserIdChecked (int userId)
        {
            //update all appointments where the userId is equal to the given userId
            var appointments = await _context.AppointmentViewQueryModels
                .Where(a => a.PatientId == userId)
                .ToListAsync();
            if (appointments == null || !appointments.Any())
                {
                throw new ArgumentException($"No appointments found for User ID {userId}.");
            }
            // update appointment IsIdChecked to true
            foreach (var appointment in appointments)
            {
                appointment.IsIdChecked = true;
                _context.AppointmentViewQueryModels.Update(appointment);
            }
            return await _context.SaveChangesAsync() > 0;

        }
    }
}
