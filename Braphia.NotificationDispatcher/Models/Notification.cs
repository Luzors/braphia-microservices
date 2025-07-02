using System.ComponentModel.DataAnnotations;

namespace Braphia.NotificationDispatcher.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        
        public string Title { get; set; }
        public string Message { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }
        public int? PharmacyId { get; set; }
        public Pharmacy? Pharmacy { get; set; }
        public int? LaboratoryId { get; set; }
        public Laboratory? Laboratory { get; set; }

        public DateTime CreatedAt { get; set; }

        public Notification() { }

        public Notification(string title, string message, int? userId = null, int? pharmacyId = null, int? laboratoryId = null)
        {
            Title = title;
            Message = message;
            UserId = userId;
            PharmacyId = pharmacyId;
            LaboratoryId = laboratoryId;
            CreatedAt = DateTime.UtcNow;
        }

        public string SendNotification()
        {
            string log = $"New Notification - {CreatedAt:yyyy-MM-dd HH:mm:ss}! To: " +
                            $"{(User != null ? User.FirstName + " " + User.LastName : Pharmacy != null ? Pharmacy.Name : Laboratory != null ? Laboratory.LaboratoryName : "Unknown")}, " +
                            $"Subject: {Title}, Message: {Message}";
            return log;
        }
    }
}
