using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories
{
    public class SqlAppointmentRepository : IAppointmentRepository
    {
        private readonly WriteDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        public SqlAppointmentRepository(WriteDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context must be of type WriteDbContext.");
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }
        public async Task<bool> AddAppointmentAsync(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            await _context.appointments.AddAsync(appointment);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateAppointmentAsync(int id, Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            var existingAppointment = await _context.appointments.FindAsync(id)
                ?? throw new ArgumentException($"Appointment with ID {appointment.Id} not found.");
            existingAppointment.PatientId = appointment.PatientId;
            existingAppointment.PhysicianId = appointment.PhysicianId;
            existingAppointment.ScheduledTime = appointment.ScheduledTime;
            existingAppointment.state = appointment.state;
            existingAppointment.ReferralId = appointment.ReferralId;

            _context.appointments.Update(existingAppointment);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteAppointmentAsync(int appointmentId)
        {
            var appointment = await GetAppointmentByIdAsync(appointmentId);
            if (appointment == null) return false;
            _context.appointments.Remove(appointment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _context.appointments
                .Include(a => a.patient)
                .Include(a => a.physician)
                .Include(a => a.referral)
                .Include(a => a.receptionist)
                .FirstOrDefaultAsync(a => a.Id == appointmentId)
                ?? await _context.appointments
                .FindAsync(appointmentId);
            return appointment ?? throw new ArgumentException($"Appointment with ID {appointmentId} not found.");
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.appointments.ToListAsync()
                   ?? throw new ArgumentException("No appointments found in the database.");
        }
        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            return await _context.appointments
                .Where(a => a.PatientId == patientId)
                .Include(a => a.patient)
                .Include(a => a.physician)
                .Include(a => a.referral)
                .Include(a => a.receptionist)
                .ToListAsync()
                ?? throw new ArgumentException($"No appointments found for patient with ID {patientId}.");
        }

        public async Task<bool> AddFollowUpAppointmentAsync(int appointmentId, Appointment followUpAppointment)
        {
            if (followUpAppointment == null)
                throw new ArgumentNullException(nameof(followUpAppointment), "Follow-up appointment cannot be null.");
            var existingAppointment = await GetAppointmentByIdAsync(appointmentId);
            if (existingAppointment == null)
                throw new ArgumentException($"Appointment with ID {appointmentId} not found.");
            existingAppointment.SetFollowUpAppointment(followUpAppointment);
            _context.appointments.Update(existingAppointment);
            return await _context.SaveChangesAsync() > 0;

        }
    }
}
