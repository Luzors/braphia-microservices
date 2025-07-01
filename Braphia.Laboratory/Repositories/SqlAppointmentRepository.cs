using Braphia.Laboratory.Database;
using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using Infrastructure.Messaging;
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
       
        public async Task<bool> AddAppointmentAsync(Appointment appointment, bool ignoreIdentity = false)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            await _context.Appointment.AddAsync(appointment);
            if (ignoreIdentity)
                await _context.SaveChangesWithIdentityInsertAsync();
            else
                await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointment.ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _context.Appointment.FirstOrDefaultAsync(a => a.Id == appointmentId);

        }

        public async Task<bool> UpdateAppointmentAsync(Appointment appointment, bool ignoreIdentity = false)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            var existing = await _context.Appointment.FindAsync(appointment.Id);
            if (existing == null)
                throw new ArgumentException($"Appointment with ID {appointment.Id} not found.");
            _context.Appointment.Update(appointment);
            if (ignoreIdentity)
                await _context.SaveChangesWithIdentityInsertAsync();
            else
                await _context.SaveChangesAsync();
            return true;
        }
    }
  
}
