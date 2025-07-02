using Braphia.Accounting.Database;
using Braphia.Accounting.EventSourcing.Aggregates;
using Braphia.Accounting.EventSourcing.Events;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Accounting.EventSourcing.Repositories
{
    public class SqlEventStoreRepository : IEventStoreRepository
    {
        private readonly AccountingDBContext _context;
        private readonly ILogger<SqlEventStoreRepository> _logger;

        public SqlEventStoreRepository(
            AccountingDBContext context,
            ILogger<SqlEventStoreRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<InvoiceAggregate?> GetAggregateAsync(int aggregateId)
        {
            var events = await GetEventsByAggregateIdAsync(aggregateId);
            
            if (!events.Any())
                return null;

            return InvoiceAggregate.LoadFromHistory(events);
        }

        public async Task<IEnumerable<BaseEvent>> GetEventsAsync()
        {
            return await _context.Event
                .OrderBy(e => e.AggregateId)
                .ThenBy(e => e.Version)
                .ToListAsync();
        }

        public async Task<IEnumerable<BaseEvent>> GetEventsByAggregateIdAsync(int aggregateId)
        {
            return await _context.Event
                .Where(e => e.AggregateId == aggregateId)
                .OrderBy(e => e.Version)
                .ToListAsync();
        }

        public async Task<IEnumerable<BaseEvent>> GetEventsByInsurerIdAsync(int insurerId)
        {
            // Get all InvoiceCreatedEvents for this insurer
            var invoiceCreatedEvents = await _context.Event
                .OfType<InvoiceCreatedEvent>()
                .Where(e => e.InsurerId == insurerId)
                .ToListAsync();

            // Get the aggregate IDs for these events
            var aggregateIds = invoiceCreatedEvents.Select(e => e.AggregateId).Distinct().ToList();

            // Get all events for these aggregates (including payments)
            var allEvents = await _context.Event
                .Where(e => aggregateIds.Contains(e.AggregateId))
                .OrderBy(e => e.AggregateId)
                .ThenBy(e => e.Version)
                .ToListAsync();

            return allEvents;
        }

        public async Task<int> SaveEventAsync(BaseEvent eventItem)
        {           
            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem), "Event cannot be null.");
            }

            try
            {
                await _context.Event.AddAsync(eventItem);
                await _context.SaveChangesAsync();
                
                return eventItem.Id; // Return the auto-generated ID
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while saving the event");
                throw new Exception("An error occurred while saving the event.", ex);
            }
        }

        public async Task<int> SaveEventsAsync(IEnumerable<BaseEvent> events, int aggregateId)
        {
            if (events == null || !events.Any())
                return 0;
            
            // For new aggregates (with negative temp ID), we need to establish the real ID
            bool isNewAggregate = aggregateId < 0;
            int newAggregateId = aggregateId;
            
            // Convert to list to avoid multiple enumeration
            var eventsList = events.ToList();
            
            foreach (var evt in eventsList)
            {
                if (isNewAggregate && evt is InvoiceCreatedEvent)
                {
                    // For the first event of a new aggregate, we need to generate a real ID
                    // First, ensure the event has the correct ID before saving
                    evt.SetAggregateId(0); // Use 0 so the database will assign the next available ID
                    var result = await SaveEventAsync(evt);
                    newAggregateId = evt.Id; // Use the event's ID as the aggregate ID
                    
                    // Update the AggregateId to match the new event ID
                    evt.SetAggregateId(newAggregateId);
                    
                    // Update the event in the database with the corrected AggregateId
                    await _context.SaveChangesAsync();
                    
                    isNewAggregate = false;
                }
                else
                {
                    // For existing aggregates or subsequent events, use the established ID
                    if (newAggregateId != evt.AggregateId)
                    {
                        evt.SetAggregateId(newAggregateId);
                    }
                    await SaveEventAsync(evt);
                }
            }

            return newAggregateId;
        }
        
        // For backward compatibility, adapts the aggregate-based API to the event-based API
        public Task<int> SaveAggregateAsync(InvoiceAggregate aggregate)
        {
            return SaveEventsAsync(aggregate.UncommittedEvents, aggregate.Id);
        }
    }
}