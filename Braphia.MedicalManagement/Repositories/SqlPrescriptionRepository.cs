using Braphia.MedicalManagement.Database;
using Braphia.MedicalManagement.Events.Prescription;
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

            // Validate that the MedicalAnalysis exists
            var medicalAnalysis = await _context.MedicalAnalysis
                .Include(ma => ma.Prescriptions)
                .FirstOrDefaultAsync(ma => ma.Id == prescription.MedicalAnalysisId);

            if (medicalAnalysis == null)
                throw new KeyNotFoundException($"Medical Analysis with ID {prescription.MedicalAnalysisId} not found.");

            // Add the prescription to context first
            await _context.Prescription.AddAsync(prescription);

            // Add the prescription to the medical analysis collection
            medicalAnalysis.Prescriptions.Add(prescription);

            // Verify the relationship was established
            if (!medicalAnalysis.Prescriptions.Contains(prescription))
                throw new InvalidOperationException("Failed to establish relationship between prescription and medical analysis.");

            // Save all changes at once - EF will handle the relationships
            var changesSaved = await _context.SaveChangesAsync();
            if (changesSaved <= 0)
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
