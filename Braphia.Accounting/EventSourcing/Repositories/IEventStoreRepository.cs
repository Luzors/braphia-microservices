using Braphia.Accounting.EventSourcing.Aggregates;

namespace Braphia.Accounting.EventSourcing.Repositories
{
    public interface IEventStoreRepository
    {
        Task<int> SaveEventAsync(BaseEvent eventItem);
        Task<IEnumerable<BaseEvent>> GetEventsAsync();
        Task<IEnumerable<BaseEvent>> GetEventsByAggregateIdAsync(int aggregateId);
        Task<InvoiceAggregate?> GetAggregateAsync(int aggregateId);
        Task<int> SaveEventsAsync(IEnumerable<BaseEvent> events, int aggregateId);
        Task<IEnumerable<BaseEvent>> GetEventsByInsurerIdAsync(int insurerId);
    }
}
