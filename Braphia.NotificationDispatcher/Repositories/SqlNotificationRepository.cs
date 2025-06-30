using Braphia.NotificationDispatcher.Database;
using Braphia.NotificationDispatcher.Models;
using Braphia.NotificationDispatcher.Repositories.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.NotificationDispatcher.Repositories
{
    public class SqlNotificationRepository : INotificationRepository
    {
        private readonly ILogger<SqlNotificationRepository> _logger;
        private readonly DBContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public SqlNotificationRepository(ILogger<SqlNotificationRepository> logger, DBContext dbContext, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<bool> AddNotificationAsync(Notification notification)
        {
            try
            {
                await _dbContext.Notification.AddAsync(notification);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding notification");
                return false;
            }
        }

        public async Task<bool> UpdateNotificationAsync(int notificationId, Notification updatedNotification)
        {
            try
            {
                var existingNotification = await _dbContext.Notification.FindAsync(notificationId);
                if (existingNotification == null) return false;
                existingNotification.Title = updatedNotification.Title;
                existingNotification.Message = updatedNotification.Message;
                existingNotification.UserId = updatedNotification.UserId;
                existingNotification.PharmacyId = updatedNotification.PharmacyId;
                existingNotification.LaboratoryId = updatedNotification.LaboratoryId;
                existingNotification.CreatedAt = DateTime.UtcNow;
                _dbContext.Notification.Update(existingNotification);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification");
                return false;
            }
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            try
            {
                var notification = await _dbContext.Notification.FindAsync(notificationId);
                if (notification == null) return false;
                _dbContext.Notification.Remove(notification);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification");
                return false;
            }
        }

        public async Task<Notification?> GetNotificationByIdAsync(int notificationId)
        {
            return await _dbContext.Notification.FindAsync(notificationId);
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
        {
            return await _dbContext.Notification.ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _dbContext.Notification
                .Where(n => n.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByPharmacyIdAsync(int pharmacyId)
        {
            return await _dbContext.Notification
                .Where(n => n.PharmacyId == pharmacyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByLaboratoryIdAsync(int laboratoryId)
        {
            return await _dbContext.Notification
                .Where(n => n.LaboratoryId == laboratoryId)
                .ToListAsync();
        }
    }
}
