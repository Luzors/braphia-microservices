using System.ComponentModel.DataAnnotations;

namespace Braphia.Accounting.EventSourcing
{
    public abstract class BaseEvent 
    {
        [Key]
        public int Id { get; private set; }
        public int AggregateId { get; private set; }
        public int Version { get; private set; }
        public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;
        public string EventType { get; set; } = string.Empty;

        protected BaseEvent() { }

        protected BaseEvent(int aggregateId, int version)
        {
            AggregateId = aggregateId;
            Version = version;
        }

        public void SetAggregateId(int aggregateId)
        {
            AggregateId = aggregateId;
        }

        public void SetVersion(int version)
        {
            Version = version;
        }
    }
}
