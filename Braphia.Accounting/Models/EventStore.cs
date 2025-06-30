using System.ComponentModel.DataAnnotations;

namespace Braphia.Accounting.Models
{
    public class EventStore
    {
        [Key]
        public long Id { get; set; } // Auto-increment voor ordering
        
        public Guid EventId { get; set; } // Uniek per event
        public Guid AggregateId { get; set; } // Uniek per invoice
        public string EventType { get; set; } = string.Empty;
        public string EventData { get; set; } = string.Empty; // JSON serialized event
        public DateTime OccurredOn { get; set; }
        public int Version { get; set; } // Version van het aggregate
    }
}
