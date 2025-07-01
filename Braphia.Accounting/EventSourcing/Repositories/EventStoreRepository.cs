using Braphia.Accounting.Database;
using Braphia.Accounting.EventSourcing.Aggregates;
using Braphia.Accounting.EventSourcing.Events;
using Braphia.Accounting.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Braphia.Accounting.EventSourcing.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly AccountingDBContext _context;
        private readonly ILogger<EventStoreRepository> _logger;

        public EventStoreRepository(AccountingDBContext context, ILogger<EventStoreRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<IEvent> events, int expectedVersion)
        {
            var eventsList = events.ToList();
            if (!eventsList.Any()) return;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check current version
                var currentVersion = await _context.EventStore
                    .Where(e => e.AggregateId == aggregateId)
                    .MaxAsync(e => (int?)e.Version) ?? 0;

                if (currentVersion != expectedVersion)
                {
                    throw new InvalidOperationException(
                        $"Concurrency conflict. Expected version {expectedVersion}, but current version is {currentVersion}");
                }

                // Save events
                var version = expectedVersion;
                foreach (var @event in eventsList)
                {
                    version++;
                    var eventStore = new EventStore
                    {
                        EventId = @event.EventId,
                        AggregateId = aggregateId,
                        EventType = @event.EventType,
                        EventData = JsonSerializer.Serialize(@event, @event.GetType()),
                        OccurredOn = @event.OccurredOn,
                        Version = version
                    };

                    await _context.EventStore.AddAsync(eventStore);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Saved {EventCount} events for aggregate {AggregateId}", 
                    eventsList.Count, aggregateId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to save events for aggregate {AggregateId}", aggregateId);
                throw;
            }
        }

        public async Task<IEnumerable<IEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStores = await _context.EventStore
                .Where(e => e.AggregateId == aggregateId)
                .OrderBy(e => e.Version)
                .ToListAsync();

            var events = new List<IEvent>();
            foreach (var eventStore in eventStores)
            {
                var eventType = GetEventType(eventStore.EventType);
                if (eventType != null)
                {
                    var eventObject = JsonSerializer.Deserialize(eventStore.EventData, eventType) as IEvent;
                    if (eventObject != null)
                    {
                        events.Add(eventObject);
                    }
                }
            }

            return events;
        }

        public async Task<InvoiceAggregate?> GetAggregateAsync(Guid aggregateId)
        {
            var events = await GetEventsAsync(aggregateId);
            if (!events.Any()) return null;

            return InvoiceAggregate.LoadFromEvents(events);
        }

        public async Task<IEnumerable<IEvent>> GetEventsByPatientAndInsurerAsync(int patientId, int? insurerId = null)
        {
            var query = _context.EventStore.AsQueryable();

            // Filter events by patient and insurer by examining the event data
            var eventStores = await query
                .Where(e => e.EventType == "InvoiceCreated" || e.EventType == "PaymentReceived" || e.EventType == "InvoiceFullyPaid")
                .OrderBy(e => e.OccurredOn)
                .ToListAsync();

            var events = new List<IEvent>();
            foreach (var eventStore in eventStores)
            {
                var eventType = GetEventType(eventStore.EventType);
                if (eventType != null)
                {
                    var eventObject = JsonSerializer.Deserialize(eventStore.EventData, eventType) as IEvent;
                    if (eventObject != null)
                    {
                        // Filter by patient and insurer
                        bool shouldInclude = false;
                        if (eventObject is InvoiceCreatedEvent createdEvent)
                        {
                            shouldInclude = createdEvent.PatientId == patientId &&
                                          (insurerId == null || createdEvent.InsurerId == insurerId);
                        }
                        else if (eventObject is PaymentReceivedEvent paymentEvent)
                        {
                            // For payment events, we need to check if they belong to relevant invoices
                            var relatedAggregate = await GetAggregateAsync(paymentEvent.InvoiceAggregateId);
                            shouldInclude = relatedAggregate != null &&
                                          relatedAggregate.PatientId == patientId &&
                                          (insurerId == null || relatedAggregate.InsurerId == insurerId);
                        }
                        else if (eventObject is InvoiceFullyPaidEvent fullyPaidEvent)
                        {
                            // For fully paid events, check related aggregate
                            var relatedAggregate = await GetAggregateAsync(fullyPaidEvent.InvoiceAggregateId);
                            shouldInclude = relatedAggregate != null &&
                                          relatedAggregate.PatientId == patientId &&
                                          (insurerId == null || relatedAggregate.InsurerId == insurerId);
                        }

                        if (shouldInclude)
                        {
                            events.Add(eventObject);
                        }
                    }
                }
            }

            return events;
        }

        private static Type? GetEventType(string eventType)
        {
            return eventType switch
            {
                "InvoiceCreated" => typeof(InvoiceCreatedEvent),
                "PaymentReceived" => typeof(PaymentReceivedEvent),
                "InvoiceFullyPaid" => typeof(InvoiceFullyPaidEvent),
                _ => null
            };
        }
    }
} 