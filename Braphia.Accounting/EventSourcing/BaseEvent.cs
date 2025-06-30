namespace Braphia.Accounting.EventSourcing
{
    public abstract class BaseEvent : IEvent
    {
        public Guid EventId { get; private set; } = Guid.NewGuid();
        public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;
        public abstract string EventType { get; }
    }
}
