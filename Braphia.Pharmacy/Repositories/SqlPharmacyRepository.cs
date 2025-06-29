using Braphia.Pharmacy.Database;
using Braphia.Pharmacy.Repositories.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Pharmacy.Repositories
{
    public class SqlPharmacyRepository : IPharmacyRepository
    {
        private readonly ILogger<SqlPharmacyRepository> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly DBContext _context;

        public SqlPharmacyRepository(DBContext context, ILogger<SqlPharmacyRepository> logger, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> AddPharmacyAsync(Models.Pharmacy pharmacy)
        {
            try
            {
                _context.Pharmacy.Add(pharmacy);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding pharmacy");
                return false;
            }
        }

        public async Task<bool> DeletePharmacyAsync(int pharmacyId)
        {
            try
            {
                var pharmacy = await _context.Pharmacy.FindAsync(pharmacyId);
                if (pharmacy == null) 
                {
                    _logger.LogWarning("Pharmacy with ID {PharmacyId} not found to delete", pharmacyId);
                    return false;
                }
                _context.Pharmacy.Remove(pharmacy);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pharmacy");
                return false;
            }
        }

        public async Task<IEnumerable<Models.Pharmacy>> GetAllPharmaciesAsync()
        {
            return await _context.Pharmacy.ToListAsync();
        }

        public async Task<Models.Pharmacy?> GetPharmacyByIdAsync(int pharmacyId)
        {
            return await _context.Pharmacy.FindAsync(pharmacyId);
        }

        public async Task<bool> UpdatePharmacyAsync(int pharmacyId, Models.Pharmacy pharmacy)
        {
            try
            {
                var existingPharmacy = await _context.Pharmacy.FindAsync(pharmacyId);
                if (existingPharmacy == null)
                {
                    _logger.LogWarning("Pharmacy with ID {PharmacyId} not found to update", pharmacyId);
                    return false;
                }
                existingPharmacy.Name = pharmacy.Name;
                existingPharmacy.Address = pharmacy.Address;
                existingPharmacy.PhoneNumber = pharmacy.PhoneNumber;
                existingPharmacy.Email = pharmacy.Email;
                _context.Pharmacy.Update(existingPharmacy);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pharmacy");
                return false;
            }
        }
    }
}
