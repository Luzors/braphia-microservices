using Braphia.Pharmacy.Database;
using Braphia.Pharmacy.Models;
using Braphia.Pharmacy.Repositories.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Pharmacy.Repositories
{
    public class SqlMedicationRepository : IMedicationRepository
    {
        private readonly ILogger<SqlMedicationRepository> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly DBContext _context;

        public SqlMedicationRepository(DBContext context, ILogger<SqlMedicationRepository> logger, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> AddMedicationAsync(Medication medication)
        {
            try
            {
                _context.Medication.Add(medication);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding medication");
                return false;
            }
        }

        public async Task<bool> DeleteMedicationAsync(int medicationId)
        {
            try
            {
                var medication = await _context.Medication.FindAsync(medicationId);
                if (medication == null)
                {
                    _logger.LogWarning("Medication with ID {MedicationId} not found to delete", medicationId);
                    return false;
                }
                _context.Medication.Remove(medication);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medication");
                return false;
            }
        }

        public async Task<IEnumerable<Medication>> GetAllMedicationsAsync()
        {
            return await _context.Medication.ToListAsync();
        }

        public async Task<Medication?> GetMedicationByIdAsync(int medicationId)
        {
            return await _context.Medication.FindAsync(medicationId);
        }

        public async Task<bool> UpdateMedicationAsync(int medicationId, Medication updatedMedication)
        {
            try
            {
                var existingMedication = await _context.Medication.FindAsync(medicationId);
                if (existingMedication == null)
                {
                    _logger.LogWarning("Medication with ID {MedicationId} not found for update", medicationId);
                    return false;
                }
                existingMedication.Name = updatedMedication.Name;
                existingMedication.Description = updatedMedication.Description;
                existingMedication.Dosage = updatedMedication.Dosage;
                existingMedication.Price = updatedMedication.Price;
                _context.Medication.Update(existingMedication);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating medication with ID {MedicationId}", medicationId);
                return false;
            }
        }
    }
}
