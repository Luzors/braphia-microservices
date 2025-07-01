using Braphia.Accounting.EventSourcing.Aggregates;

namespace Braphia.Accounting.EventSourcing.Repositories
{
    public interface IEventStoreRepository
    {
        Task SaveEventsAsync(Guid aggregateId, IEnumerable<IEvent> events, int expectedVersion);
        Task<IEnumerable<IEvent>> GetEventsAsync(Guid aggregateId);
        Task<InvoiceAggregate?> GetAggregateAsync(Guid aggregateId);
    }
}
