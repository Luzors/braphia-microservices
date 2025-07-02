using Braphia.NotificationDispatcher.Models;

namespace Braphia.NotificationDispatcher.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<bool> AddNotificationAsync(Notification notification);
        Task<bool> UpdateNotificationAsync(int notificationId, Notification updatedNotification);
        Task<bool> DeleteNotificationAsync(int notificationId);
        Task<Notification?> GetNotificationByIdAsync(int notificationId);
        Task<IEnumerable<Notification>> GetAllNotificationsAsync();

        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId);
        Task<IEnumerable<Notification>> GetNotificationsByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<Notification>> GetNotificationsByLaboratoryIdAsync(int laboratoryId);
    }
}
