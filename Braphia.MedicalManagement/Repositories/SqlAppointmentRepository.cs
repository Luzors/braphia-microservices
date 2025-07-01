using Braphia.MedicalManagement.Database;
using Braphia.MedicalManagement.Models;
using Braphia.MedicalManagement.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.MedicalManagement.Repositories
{
    public class SqlAppointmentRepository : IAppointmentRepository
    {
        private readonly DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public SqlAppointmentRepository(DBContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<Appointment> GetAppointmentAsync(int id)
        {
            var appointment = await _context.Appointment
                .FirstOrDefaultAsync(ma => ma.Id == id);

            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {id} not found.");
            return appointment;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointment.ToListAsync();
        }

        public async Task<bool> AddAppointmentAsync(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");

            await _context.Appointment.AddAsync(appointment);
            await _context.SaveChangesWithIdentityInsertAsync();
            return true;
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {id} not found.");

            _context.Appointment.Remove(appointment);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to delete appointment.");
            return true;
        }

        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment), "Appointment cannot be null.");

            _context.Appointment.Update(appointment);
            await _context.SaveChangesWithIdentityInsertAsync();
            return true;
        }
    }
}
