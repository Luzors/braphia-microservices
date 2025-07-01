using Braphia.Accounting.Database;
using Braphia.Accounting.EventSourcing.Events;
using Braphia.Accounting.EventSourcing.Projections;
using Braphia.Accounting.EventSourcing.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Accounting.EventSourcing.Services
{
    public class InvoiceProjectionService : IInvoiceProjectionService
    {
        private readonly AccountingDBContext _context;
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly ILogger<InvoiceProjectionService> _logger;

        public InvoiceProjectionService(
            AccountingDBContext context,
            IEventStoreRepository eventStoreRepository,
            ILogger<InvoiceProjectionService> logger)
        {
            _context = context;
            _eventStoreRepository = eventStoreRepository;
            _logger = logger;
        }

        public async Task<InsurerOutstandingBalance?> GetInsurerOutstandingBalanceAsync(int insurerId)
        {
            return await _context.InsurerOutstandingBalances
                .FirstOrDefaultAsync(x => x.InsurerId == insurerId);
        }

        public async Task<IEnumerable<InvoiceProjection>> GetInvoicesByInsurerAsync(int insurerId, bool? onlyOutstanding = null)
        {
            var query = _context.InvoiceProjections.Where(x => x.InsurerId == insurerId);
            
            if (onlyOutstanding.HasValue && onlyOutstanding.Value)
            {
                query = query.Where(x => !x.IsFullyPaid);
            }

            return await query
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<InvoiceProjection?> GetInvoiceProjectionAsync(Guid invoiceAggregateId)
        {
            return await _context.InvoiceProjections
                .FirstOrDefaultAsync(x => x.InvoiceAggregateId == invoiceAggregateId);
        }

        public async Task<IEnumerable<PatientInsurerEventHistory>> GetPatientInsurerEventHistoryAsync(int patientId, int? insurerId = null)
        {
            var query = _context.PatientInsurerEventHistory
                .Where(x => x.PatientId == patientId);

            if (insurerId.HasValue)
            {
                query = query.Where(x => x.InsurerId == insurerId.Value);
            }

            return await query
                .OrderByDescending(x => x.EventOccurredOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<InvoiceProjection>> GetInvoicesByPatientAndInsurerAsync(int patientId, int? insurerId = null)
        {
            var query = _context.InvoiceProjections
                .Where(x => x.PatientId == patientId);

            if (insurerId.HasValue)
            {
                query = query.Where(x => x.InsurerId == insurerId.Value);
            }

            return await query
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<PatientInsurerEventHistory?> GetPatientInsurerSummaryAsync(int patientId, int insurerId)
        {
            return await _context.PatientInsurerEventHistory
                .Where(x => x.PatientId == patientId && x.InsurerId == insurerId)
                .OrderByDescending(x => x.LastActivityDate)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateProjectionsFromEventAsync(IEvent @event)
        {
            switch (@event)
            {
                case InvoiceCreatedEvent created:
                    await HandleInvoiceCreatedAsync(created);
                    break;
                case PaymentReceivedEvent payment:
                    await HandlePaymentReceivedAsync(payment);
                    break;
                case InvoiceFullyPaidEvent fullyPaid:
                    await HandleInvoiceFullyPaidAsync(fullyPaid);
                    break;
            }

            await UpdatePatientInsurerEventHistoryAsync(@event);
        }

        public async Task UpdatePatientInsurerEventHistoryAsync(IEvent @event)
        {
            switch (@event)
            {
                case InvoiceCreatedEvent created:
                    await UpdatePatientInsurerHistoryForInvoiceCreated(created);
                    break;
                case PaymentReceivedEvent payment:
                    await UpdatePatientInsurerHistoryForPayment(payment);
                    break;
                case InvoiceFullyPaidEvent fullyPaid:
                    await UpdatePatientInsurerHistoryForFullyPaid(fullyPaid);
                    break;
            }
        }

        private async Task HandleInvoiceCreatedAsync(InvoiceCreatedEvent created)
        {
            var projection = new InvoiceProjection
            {
                InvoiceAggregateId = created.InvoiceAggregateId,
                PatientId = created.PatientId,
                InsurerId = created.InsurerId,
                LabTestId = created.LabTestId,
                TotalAmount = created.Amount,
                AmountPaid = 0,
                AmountOutstanding = created.Amount,
                Description = created.Description,
                CreatedDate = created.InvoiceDate,
                IsFullyPaid = false
            };

            // Get patient and insurer names
            var patient = await _context.Patient.FindAsync(created.PatientId);
            var insurer = await _context.Insurer.FindAsync(created.InsurerId);
            
            projection.PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}".Trim() : "Unknown";
            projection.InsurerName = insurer?.Name ?? "Unknown";

            await _context.InvoiceProjections.AddAsync(projection);
            await UpdateInsurerOutstandingBalance(created.InsurerId);
            await _context.SaveChangesAsync();
        }

        private async Task HandlePaymentReceivedAsync(PaymentReceivedEvent payment)
        {
            var projection = await _context.InvoiceProjections
                .FirstOrDefaultAsync(x => x.InvoiceAggregateId == payment.InvoiceAggregateId);

            if (projection != null)
            {
                projection.AmountPaid += payment.PaymentAmount;
                projection.AmountOutstanding = projection.TotalAmount - projection.AmountPaid;
                projection.LastPaymentDate = payment.PaymentDate;

                await UpdateInsurerOutstandingBalance(projection.InsurerId);
                await _context.SaveChangesAsync();
            }
        }

        private async Task HandleInvoiceFullyPaidAsync(InvoiceFullyPaidEvent fullyPaid)
        {
            var projection = await _context.InvoiceProjections
                .FirstOrDefaultAsync(x => x.InvoiceAggregateId == fullyPaid.InvoiceAggregateId);

            if (projection != null)
            {
                projection.IsFullyPaid = true;
                projection.AmountOutstanding = 0;

                await UpdateInsurerOutstandingBalance(projection.InsurerId);
                await _context.SaveChangesAsync();
            }
        }

        private async Task UpdatePatientInsurerHistoryForInvoiceCreated(InvoiceCreatedEvent created)
        {
            var patient = await _context.Patient.FindAsync(created.PatientId);
            var insurer = await _context.Insurer.FindAsync(created.InsurerId);

            var history = new PatientInsurerEventHistory
            {
                PatientId = created.PatientId,
                InsurerId = created.InsurerId,
                InvoiceAggregateId = created.InvoiceAggregateId,
                EventType = "InvoiceCreated",
                EventDescription = $"Invoice created for {created.Description}",
                Amount = created.Amount,
                EventOccurredOn = created.OccurredOn,
                PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}".Trim() : "Unknown",
                InsurerName = insurer?.Name ?? "Unknown",
                LastActivityDate = created.OccurredOn
            };

            // Update aggregated totals
            await UpdatePatientInsurerAggregates(created.PatientId, created.InsurerId);
            
            await _context.PatientInsurerEventHistory.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        private async Task UpdatePatientInsurerHistoryForPayment(PaymentReceivedEvent payment)
        {
            // Get the invoice aggregate to find patient info
            var aggregate = await _eventStoreRepository.GetAggregateAsync(payment.InvoiceAggregateId);
            if (aggregate == null) return;

            var patient = await _context.Patient.FindAsync(aggregate.PatientId);
            var insurer = await _context.Insurer.FindAsync(aggregate.InsurerId);

            var history = new PatientInsurerEventHistory
            {
                PatientId = aggregate.PatientId,
                InsurerId = aggregate.InsurerId,
                InvoiceAggregateId = payment.InvoiceAggregateId,
                EventType = "PaymentReceived",
                EventDescription = $"Payment received: {payment.PaymentReference}",
                Amount = payment.PaymentAmount,
                EventOccurredOn = payment.OccurredOn,
                PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}".Trim() : "Unknown",
                InsurerName = insurer?.Name ?? "Unknown",
                LastActivityDate = payment.OccurredOn
            };

            await UpdatePatientInsurerAggregates(aggregate.PatientId, aggregate.InsurerId);
            
            await _context.PatientInsurerEventHistory.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        private async Task UpdatePatientInsurerHistoryForFullyPaid(InvoiceFullyPaidEvent fullyPaid)
        {
            var aggregate = await _eventStoreRepository.GetAggregateAsync(fullyPaid.InvoiceAggregateId);
            if (aggregate == null) return;

            var patient = await _context.Patient.FindAsync(aggregate.PatientId);
            var insurer = await _context.Insurer.FindAsync(aggregate.InsurerId);

            var history = new PatientInsurerEventHistory
            {
                PatientId = aggregate.PatientId,
                InsurerId = aggregate.InsurerId,
                InvoiceAggregateId = fullyPaid.InvoiceAggregateId,
                EventType = "InvoiceFullyPaid",
                EventDescription = "Invoice fully paid",
                Amount = null,
                EventOccurredOn = fullyPaid.OccurredOn,
                PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}".Trim() : "Unknown",
                InsurerName = insurer?.Name ?? "Unknown",
                LastActivityDate = fullyPaid.OccurredOn
            };

            await UpdatePatientInsurerAggregates(aggregate.PatientId, aggregate.InsurerId);
            
            await _context.PatientInsurerEventHistory.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        private async Task UpdatePatientInsurerAggregates(int patientId, int insurerId)
        {
            var invoices = await _context.InvoiceProjections
                .Where(x => x.PatientId == patientId && x.InsurerId == insurerId)
                .ToListAsync();

            var existingHistory = await _context.PatientInsurerEventHistory
                .Where(x => x.PatientId == patientId && x.InsurerId == insurerId)
                .ToListAsync();

            // Update aggregates in the most recent history record
            foreach (var history in existingHistory)
            {
                history.TotalInvoiced = invoices.Sum(x => x.TotalAmount);
                history.TotalPaid = invoices.Sum(x => x.AmountPaid);
                history.TotalOutstanding = invoices.Sum(x => x.AmountOutstanding);
                history.TotalInvoiceCount = invoices.Count;
                history.PaidInvoiceCount = invoices.Count(x => x.IsFullyPaid);
            }
        }

        private async Task UpdateInsurerOutstandingBalance(int insurerId)
        {
            var invoices = await _context.InvoiceProjections
                .Where(x => x.InsurerId == insurerId)
                .ToListAsync();

            var outstanding = await _context.InsurerOutstandingBalances
                .FirstOrDefaultAsync(x => x.InsurerId == insurerId);

            if (outstanding == null)
            {
                var insurer = await _context.Insurer.FindAsync(insurerId);
                outstanding = new InsurerOutstandingBalance
                {
                    InsurerId = insurerId,
                    InsurerName = insurer?.Name ?? "Unknown"
                };
                await _context.InsurerOutstandingBalances.AddAsync(outstanding);
            }

            outstanding.TotalOutstanding = invoices.Sum(x => x.AmountOutstanding);
            outstanding.OutstandingInvoiceCount = invoices.Count(x => !x.IsFullyPaid);
            outstanding.LastUpdated = DateTime.UtcNow;
        }
    }
} 