using Braphia.NotificationDispatcher.Database;
using Braphia.NotificationDispatcher.Repositories.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.NotificationDispatcher.Repositories
{
    public class SqlLaboratoryRepository : ILaboratoryRepository
    {
        private readonly ILogger<SqlLaboratoryRepository> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly DBContext _context;

        public SqlLaboratoryRepository(DBContext context, ILogger<SqlLaboratoryRepository> logger, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> AddLaboratoryAsync(Models.Laboratory laboratory)
        {
            try
            {
                _context.Laboratory.Add(laboratory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding laboratory");
                return false;
            }
        }

        public async Task<bool> UpdateLaboratoryAsync(int laboratoryId, Models.Laboratory updatedLaboratory)
        {
            try
            {
                var existingLaboratory = await _context.Laboratory.FindAsync(laboratoryId);
                if (existingLaboratory == null)
                {
                    _logger.LogWarning("Laboratory with ID {LaboratoryId} not found for update", laboratoryId);
                    return false;
                }
                existingLaboratory.LaboratoryName = updatedLaboratory.LaboratoryName;
                existingLaboratory.Email = updatedLaboratory.Email;
                _context.Laboratory.Update(existingLaboratory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating laboratory");
                return false;
            }
        }

        public async Task<bool> DeleteLaboratoryAsync(int laboratoryId)
        {
            try
            {
                var laboratory = await _context.Laboratory.FindAsync(laboratoryId);
                if (laboratory == null)
                {
                    _logger.LogWarning("Laboratory with ID {LaboratoryId} not found to delete", laboratoryId);
                    return false;
                }
                _context.Laboratory.Remove(laboratory);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting laboratory");
                return false;
            }
        }

        public async Task<Models.Laboratory?> GetLaboratoryByIdAsync(int laboratoryId)
        {
            return await _context.Laboratory.FindAsync(laboratoryId);
        }

        public async Task<IEnumerable<Models.Laboratory>> GetAllLaboratoriesAsync()
        {
            return await _context.Laboratory.ToListAsync();
        }
    }
}
