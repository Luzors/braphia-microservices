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
       
        // TODO: verwijderen want appointment wordt overgenomen vanuit event
        public async Task<bool> AddAppointmentAsync(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            await _context.Appointment.AddAsync(appointment);
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

        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");
            var existing = await _context.Appointment.FindAsync(appointment.Id);
            if (existing == null)
                throw new ArgumentException($"Appointment with ID {appointment.Id} not found.");
            existing.AppointmentDate = appointment.AppointmentDate;
            existing.PatientId = appointment.PatientId;
            await _context.SaveChangesAsync();
            return true;
        }
    }
  
}
