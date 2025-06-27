using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Models;
using Braphia.UserManagement.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories
{
    public class SqlPhysicianRepository : IPhysicianRepository
    {
        private readonly DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        public SqlPhysicianRepository(DBContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context must be of type WriteDbContext.");
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }
        public async Task<bool> AddPhysicianAsync(Physician physician)
        {
            if (physician == null)
                throw new ArgumentNullException(nameof(physician), "Physician cannot be null.");
            await _context.physicians.AddAsync(physician);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdatePhysicianAsync(Physician physician)
        {
            if (physician == null)
                throw new ArgumentNullException(nameof(physician), "Physician cannot be null.");
            var existingPhysician = await _context.physicians.FindAsync(physician.Id)
                ?? throw new ArgumentException($"Physician with ID {physician.Id} not found.");
            existingPhysician.FirstName = physician.FirstName;
            existingPhysician.LastName = physician.LastName;
            existingPhysician.Specialization = physician.Specialization;
            _context.physicians.Update(existingPhysician);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeletePhysicianAsync(int physicianId)
        {
            var physician = await GetPhysicianByIdAsync(physicianId);
            if (physician == null) return false;
            _context.physicians.Remove(physician);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Physician> GetPhysicianByIdAsync(int physicianId)
        {
            return await _context.physicians.FindAsync(physicianId);
        }
        public async Task<IEnumerable<Physician>> GetAllPhysiciansAsync()
        {
            return await _context.physicians.ToListAsync()
                   ?? throw new ArgumentException("No physicians found in the database.");
        }
        public async Task<IEnumerable<Physician>> GetPhysiciansBySpecializationAsync(SpecializationEnum specialization)
        {
            return await _context.physicians
                .Where(p => p.Specialization == specialization)
                .ToListAsync() 
                ?? throw new ArgumentException($"No physicians found with specialization {specialization}.");
        }

    }
    }
