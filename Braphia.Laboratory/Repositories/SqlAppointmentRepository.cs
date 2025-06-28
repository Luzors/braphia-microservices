using Braphia.Laboratory.Database;
using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Laboratory.Repositories
{
    public class SqlAppointmentRepository : IAppointmentRepository
    {
        private readonly DBContext _context;
        public SqlAppointmentRepository(DBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
       
        public async Task<bool> AddAppointmentAsync(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAppointmentAsync(Guid appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);

            if (appointment == null)
                throw new ArgumentException($"Appointment with ID {appointmentId} not found.");

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments.ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId)
        {
            return await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);

        }

        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            var existing = await _context.Appointments.FindAsync(appointment.Id);
            if (existing == null)
                throw new ArgumentException($"Appointment with ID {appointment.Id} not found.");
            existing.AppointmentDate = appointment.AppointmentDate;
            await _context.SaveChangesAsync();
            return true;
        }
    }
  
}
