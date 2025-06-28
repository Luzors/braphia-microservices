using Braphia.Pharmacy.Database;
using Braphia.Pharmacy.Models.ExternalObjects;
using Braphia.Pharmacy.Repositories.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Pharmacy.Repositories
{
    public class SqlPrescriptionRepository : IPrescriptionRepository
    {
        private readonly ILogger<SqlPrescriptionRepository> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly DBContext _context;

        public SqlPrescriptionRepository(DBContext context, ILogger<SqlPrescriptionRepository> logger, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> AddPrescriptionAsync(Prescription prescription)
        {
            try
            {
                _context.Prescription.Add(prescription);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding prescription");
                return false;
            }
        }

        public async Task<bool> DeletePrescriptionAsync(int prescriptionId)
        {
            try
            {
                var prescription = await _context.Prescription.FindAsync(prescriptionId);
                if (prescription == null)
                {
                    _logger.LogWarning("Prescription with ID {PrescriptionId} not found to delete", prescriptionId);
                    return false;
                }
                _context.Prescription.Remove(prescription);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting prescription");
                return false;
            }
        }

        public async Task<IEnumerable<Prescription>> GetAllPrescriptionsAsync()
        {
            return await _context.Prescription.ToListAsync();
        }

        public async Task<Prescription?> GetPrescriptionByIdAsync(int prescriptionId)
        {
            return await _context.Prescription.FindAsync(prescriptionId);
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByPatientIdAsync(int patientId)
        {
            return await _context.Prescription
                .Where(p => p.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<bool> UpdatePrescriptionAsync(int prescriptionId, Prescription prescription)
        {
            try
            {
                var existingPrescription = await _context.Prescription.FindAsync(prescriptionId);
                if (existingPrescription == null)
                {
                    _logger.LogWarning("Prescription with ID {PrescriptionId} not found to update", prescriptionId);
                    return false;
                }
                existingPrescription.RootId = prescription.RootId;
                existingPrescription.PatientId = prescription.PatientId;
                _context.Prescription.Update(existingPrescription);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating prescription");
                return false;
            }
        }
    }
}
