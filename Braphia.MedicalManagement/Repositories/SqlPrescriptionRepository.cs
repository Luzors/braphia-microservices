using Braphia.MedicalManagement.Database;
using Braphia.MedicalManagement.Events;
using Braphia.MedicalManagement.Models;
using Braphia.MedicalManagement.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.MedicalManagement.Repositories
{
    public class SqlPrescriptionRepository : IPrescriptionRepository
    {
        private readonly DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public SqlPrescriptionRepository(DBContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<Prescription> GetPrescriptionAsync(int id)
        {
            var prescription = await _context.Prescription.FindAsync(id);
            if (prescription == null)
                throw new KeyNotFoundException($"Prescription with ID {id} not found.");
            return prescription;
        }

        public async Task<IEnumerable<Prescription>> GetAllPrescriptionsAsync()
        {
            return await _context.Prescription.ToListAsync();
        }

        public async Task<bool> AddPrescriptionAsync(Prescription prescription)
        {
            if (prescription == null)
                throw new ArgumentNullException(nameof(prescription), "Prescription cannot be null.");

            await _context.Prescription.AddAsync(prescription);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to add prescription.");

            await _publishEndpoint.Publish(new Message(new PrescriptionWrittenEvent(prescription)));
            return true;
        }

        public async Task<bool> DeletePrescriptionAsync(int id)
        {
            var prescription = await _context.Prescription.FindAsync(id);
            if (prescription == null)
                throw new KeyNotFoundException($"Prescription with ID {id} not found.");

            _context.Prescription.Remove(prescription);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to delete prescription.");
            
            await _publishEndpoint.Publish(new Message(new PrescriptionInvokedEvent(prescription)));

            return true;
        }

        public async Task<bool> UpdatePrescriptionAsync(Prescription prescription)
        {
            if (prescription == null)
                throw new ArgumentNullException(nameof(prescription), "Prescription cannot be null.");

            _context.Prescription.Update(prescription);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to update prescription.");
            
            await _publishEndpoint.Publish(new Message(new PrescriptionChangedEvent(prescription)));
            
            return true;
        }
    }
}
