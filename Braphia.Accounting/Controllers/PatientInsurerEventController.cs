using Braphia.Accounting.EventSourcing.Services;
using Braphia.Accounting.EventSourcing.Projections;
using Braphia.Accounting.EventSourcing.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Accounting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientInsurerEventController : ControllerBase
    {
        private readonly IInvoiceProjectionService _projectionService;
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly ILogger<PatientInsurerEventController> _logger;

        public PatientInsurerEventController(
            IInvoiceProjectionService projectionService,
            IEventStoreRepository eventStoreRepository,
            ILogger<PatientInsurerEventController> logger)
        {
            _projectionService = projectionService;
            _eventStoreRepository = eventStoreRepository;
            _logger = logger;
        }

        [HttpGet("patient/{patientId}/history")]
        [ProducesResponseType(typeof(IEnumerable<PatientInsurerEventHistory>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPatientEventHistory(int patientId, [FromQuery] int? insurerId = null)
        {
            if (patientId <= 0)
            {
                return BadRequest("Valid patient ID is required");
            }

            try
            {
                var history = await _projectionService.GetPatientInsurerEventHistoryAsync(patientId, insurerId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching event history for patient {PatientId}, insurer {InsurerId}", 
                    patientId, insurerId);
                return StatusCode(500, "Internal server error while fetching event history");
            }
        }

        [HttpGet("patient/{patientId}/invoices")]
        [ProducesResponseType(typeof(IEnumerable<InvoiceProjection>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPatientInvoices(int patientId, [FromQuery] int? insurerId = null)
        {
            if (patientId <= 0)
            {
                return BadRequest("Valid patient ID is required");
            }

            try
            {
                var invoices = await _projectionService.GetInvoicesByPatientAndInsurerAsync(patientId, insurerId);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching invoices for patient {PatientId}, insurer {InsurerId}", 
                    patientId, insurerId);
                return StatusCode(500, "Internal server error while fetching invoices");
            }
        }

        [HttpGet("patient/{patientId}/insurer/{insurerId}/summary")]
        [ProducesResponseType(typeof(PatientInsurerEventHistory), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPatientInsurerSummary(int patientId, int insurerId)
        {
            if (patientId <= 0 || insurerId <= 0)
            {
                return BadRequest("Valid patient ID and insurer ID are required");
            }

            try
            {
                var summary = await _projectionService.GetPatientInsurerSummaryAsync(patientId, insurerId);
                if (summary == null)
                {
                    return NotFound($"No event history found for patient {patientId} and insurer {insurerId}");
                }

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching summary for patient {PatientId}, insurer {InsurerId}", 
                    patientId, insurerId);
                return StatusCode(500, "Internal server error while fetching summary");
            }
        }

        [HttpGet("patient/{patientId}/events")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPatientEvents(int patientId, [FromQuery] int? insurerId = null)
        {
            if (patientId <= 0)
            {
                return BadRequest("Valid patient ID is required");
            }

            try
            {
                var events = await _eventStoreRepository.GetEventsByPatientAndInsurerAsync(patientId, insurerId);
                
                // Convert events to a more readable format
                var eventSummaries = events.Select(e => new
                {
                    EventId = e.EventId,
                    EventType = e.EventType,
                    OccurredOn = e.OccurredOn,
                    EventData = (object)(e switch
                    {
                        Braphia.Accounting.EventSourcing.Events.InvoiceCreatedEvent created => new
                        {
                            InvoiceId = created.InvoiceAggregateId,
                            PatientId = created.PatientId,
                            InsurerId = created.InsurerId,
                            Amount = created.Amount,
                            Description = created.Description
                        },
                        Braphia.Accounting.EventSourcing.Events.PaymentReceivedEvent payment => new
                        {
                            InvoiceId = payment.InvoiceAggregateId,
                            InsurerId = payment.InsurerId,
                            PaymentAmount = payment.PaymentAmount,
                            PaymentReference = payment.PaymentReference
                        },
                        Braphia.Accounting.EventSourcing.Events.InvoiceFullyPaidEvent fullyPaid => new
                        {
                            InvoiceId = fullyPaid.InvoiceAggregateId
                        },
                        _ => new { Message = "Unknown event type" }
                    })
                }).ToList();

                return Ok(eventSummaries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching events for patient {PatientId}, insurer {InsurerId}", 
                    patientId, insurerId);
                return StatusCode(500, "Internal server error while fetching events");
            }
        }
    }
} 