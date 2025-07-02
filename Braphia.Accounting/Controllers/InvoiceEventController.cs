using Braphia.Accounting.EventSourcing;
using Braphia.Accounting.EventSourcing.Aggregates;
using Braphia.Accounting.EventSourcing.Events;
using Braphia.Accounting.EventSourcing.Services;
using Braphia.Accounting.Events;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Accounting.Controllers
{
    [Route("api/invoice")]
    [ApiController]
    public class InvoiceEventController : ControllerBase
    {
        private readonly IInvoiceEventService _invoiceEventService;
        private readonly ILogger<InvoiceEventController> _logger;

        public InvoiceEventController(
            IInvoiceEventService invoiceEventService,
            ILogger<InvoiceEventController> logger)
        {
            _invoiceEventService = invoiceEventService ?? throw new ArgumentNullException(nameof(invoiceEventService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        [HttpGet("{invoiceId}")]
        [ProducesResponseType(typeof(InvoiceEventHistoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInvoice(int invoiceId)
        {
            if (invoiceId <= 0)
            {
                return BadRequest("Valid invoice ID is required");
            }

            try
            {
                var invoice = await _invoiceEventService.GetInvoiceAsync(invoiceId);
                
                if (invoice == null)
                {
                    return NotFound($"Invoice with ID {invoiceId} not found");
                }

                // Get all events for complete history
                var allEvents = await _invoiceEventService.GetAllEventsByInvoiceIdAsync(invoiceId);
                
                var eventHistoryDto = new InvoiceEventHistoryDto
                {
                    InvoiceId = invoiceId,
                    PatientId = invoice.PatientId,
                    InsurerId = invoice.InsurerId,
                    TotalAmount = invoice.TotalAmount,
                    AmountPaid = invoice.AmountPaid,
                    AmountOutstanding = invoice.AmountOutstanding,
                    Description = invoice.Description,
                    CreatedDate = invoice.CreatedDate,
                    Events = allEvents.OrderBy(e => e.Version).Select(e => MapEventToDto(e)).ToList()
                };

                return Ok(eventHistoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice {InvoiceId}", invoiceId);
                return StatusCode(500, "Internal server error while retrieving invoice");
            }
        }

        [HttpPost("{invoiceId}/payment")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProcessPayment(int invoiceId, [FromBody] PaymentRequest request)
        {
            if (invoiceId <= 0)
            {
                return BadRequest("Valid invoice ID is required");
            }

            if (request == null || request.PaymentAmount <= 0)
            {
                return BadRequest("Valid payment amount is required");
            }

            if (request.InsurerId <= 0)
            {
                return BadRequest("Valid insurer ID is required");
            }

            _logger.LogInformation("Processing payment of {Amount:C} from insurer {InsurerId} for invoice {InvoiceId}",
                request.PaymentAmount, request.InsurerId, invoiceId);

            try
            {
                bool success = await _invoiceEventService.ProcessPaymentAsync(
                    invoiceId,
                    request.InsurerId,
                    request.PaymentAmount,
                    request.PaymentReference ?? string.Empty);

                var updatedInvoice = await _invoiceEventService.GetInvoiceAsync(invoiceId);
                if (updatedInvoice == null)
                {
                    return NotFound($"Invoice with ID {invoiceId} not found");
                }

                // Create the DTO without payments - just basic invoice info
                var invoiceDto = MapToDto(updatedInvoice);

                return Ok(invoiceDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid payment operation for invoice {InvoiceId}", invoiceId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for invoice {InvoiceId}", invoiceId);
                return StatusCode(500, "Internal server error while processing payment");
            }
        }

        [HttpPost("{invoiceId}/adjustment")]
        [ProducesResponseType(typeof(InvoiceEventHistoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AdjustInvoiceAmount(int invoiceId, [FromBody] InvoiceAmountAdjustmentRequest request)
        {
            if (invoiceId <= 0)
            {
                return BadRequest("Valid invoice ID is required");
            }

            if (request == null || request.AdjustmentAmount == 0)
            {
                return BadRequest("Valid adjustment amount is required (cannot be zero)");
            }

            if (request.InsurerId <= 0)
            {
                return BadRequest("Valid insurer ID is required");
            }

            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return BadRequest("Adjustment reason is required");
            }

            _logger.LogInformation("Adjusting invoice amount by {Amount:C} from insurer {InsurerId} for invoice {InvoiceId}. Reason: {Reason}",
                request.AdjustmentAmount, request.InsurerId, invoiceId, request.Reason);

            try
            {
                bool success = await _invoiceEventService.AdjustInvoiceAmountAsync(
                    invoiceId,
                    request.InsurerId,
                    request.AdjustmentAmount,
                    request.Reason,
                    request.Reference ?? string.Empty);

                var updatedInvoice = await _invoiceEventService.GetInvoiceAsync(invoiceId);
                if (updatedInvoice == null)
                {
                    return NotFound($"Invoice with ID {invoiceId} not found");
                }

                // Get all events for complete history
                var allEvents = await _invoiceEventService.GetAllEventsByInvoiceIdAsync(invoiceId);
                
                var eventHistoryDto = new InvoiceEventHistoryDto
                {
                    InvoiceId = invoiceId,
                    PatientId = updatedInvoice.PatientId,
                    InsurerId = updatedInvoice.InsurerId,
                    TotalAmount = updatedInvoice.TotalAmount,
                    AmountPaid = updatedInvoice.AmountPaid,
                    AmountOutstanding = updatedInvoice.AmountOutstanding,
                    Description = updatedInvoice.Description,
                    CreatedDate = updatedInvoice.CreatedDate,
                    Events = allEvents.OrderBy(e => e.Version).Select(e => MapEventToDto(e)).ToList()
                };

                return Ok(eventHistoryDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid adjustment operation for invoice {InvoiceId}", invoiceId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adjusting invoice amount for invoice {InvoiceId}", invoiceId);
                return StatusCode(500, "Internal server error while adjusting invoice amount");
            }
        }



        [HttpGet("insurer/{insurerId}/invoices")]
        [ProducesResponseType(typeof(IEnumerable<InvoiceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInsurerInvoices(int insurerId)
        {
            if (insurerId <= 0)
            {
                return BadRequest("Valid insurer ID is required");
            }

            try
            {
                // Get all unpaid invoices for the insurer
                var invoices = await _invoiceEventService.GetInvoicesByInsurerAsync(insurerId);
                
                // Create DTOs without payments
                var invoiceDtos = new List<InvoiceDto>();
                foreach (var invoice in invoices)
                {
                    invoiceDtos.Add(MapToDto(invoice));
                }
                
                return Ok(new 
                {
                    totalAmountOutstanding = invoiceDtos.Sum(i => i.AmountOutstanding),
                    invoices = invoiceDtos

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching invoices for insurer {InsurerId}", insurerId);
                return StatusCode(500, "Internal server error while fetching invoices");
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InvoiceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllInvoices()
        {
            try
            {
                var invoices = await _invoiceEventService.GetAllInvoicesAsync();
                
                // Create DTOs without payments
                var invoiceDtos = new List<InvoiceDto>();
                foreach (var invoice in invoices)
                {
                    invoiceDtos.Add(MapToDto(invoice));
                }
                
                return Ok(invoiceDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all invoices");
                return StatusCode(500, "Internal server error while retrieving invoices");
            }
        }

        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(typeof(IEnumerable<InvoiceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInvoicesByPatientId(int patientId)
        {
            if (patientId <= 0)
            {
                return BadRequest("Valid patient ID is required");
            }

            try
            {
                var invoices = await _invoiceEventService.GetInvoicesByPatientIdAsync(patientId);
                
                // Create DTOs without payments
                var invoiceDtos = new List<InvoiceDto>();
                foreach (var invoice in invoices)
                {
                    invoiceDtos.Add(MapToDto(invoice));
                }
                
                return Ok(invoiceDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices for patient {PatientId}", patientId);
                return StatusCode(500, "Internal server error while retrieving patient invoices");
            }
        }

        private InvoiceDto MapToDto(InvoiceAggregate invoice)
        {
            var dto = new InvoiceDto
            {
                Id = invoice.Id,
                PatientId = invoice.PatientId,
                InsurerId = invoice.InsurerId,
                TotalAmount = invoice.TotalAmount,
                AmountPaid = invoice.AmountPaid,
                AmountOutstanding = invoice.AmountOutstanding,
                Description = invoice.Description,
                CreatedDate = invoice.CreatedDate,
                IsFullyPaid = invoice.IsFullyPaid
            };
            
            return dto;
        }

        private EventDto MapEventToDto(BaseEvent evt)
        {
            return evt switch
            {
                InvoiceCreatedEvent created => new EventDto
                {
                    EventType = "Invoice Created",
                    Description = $"Invoice created for {created.Description}",
                    Amount = created.Amount,
                    Date = created.OccurredOn,
                    Version = created.Version
                },
                PaymentReceivedEvent payment => new EventDto
                {
                    EventType = "Payment Received",
                    Description = $"Payment from insurer {payment.InsurerId}: {payment.PaymentReference}",
                    Amount = payment.PaymentAmount,
                    Date = payment.PaymentDate,
                    Version = payment.Version
                },
                InvoiceAmountAdjustedEvent adjustment => new EventDto
                {
                    EventType = "Invoice Amount Adjusted",
                    Description = $"Amount adjusted by {adjustment.AdjustmentAmount:C}: {adjustment.Reason}",
                    Amount = adjustment.AdjustmentAmount,
                    Date = adjustment.OccurredOn,
                    Version = adjustment.Version
                },
                _ => new EventDto
                {
                    EventType = "Unknown",
                    Description = $"Unknown event type: {evt.GetType().Name}",
                    Amount = 0,
                    Date = evt.OccurredOn,
                    Version = evt.Version
                }
            };
        }
    }

    public class PaymentRequest
    {
        public int InsurerId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string? PaymentReference { get; set; }
    }

    public class InvoiceAmountAdjustmentRequest
    {
        public int InsurerId { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Reference { get; set; }
    }

    public class PaymentDto
    {
        public int InsurerId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentReference { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? ReversalReason { get; set; }
        public string? OriginalPaymentReference { get; set; }
    }

    public class EventDto
    {
        public string EventType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int Version { get; set; }
    }

    public class InvoiceEventHistoryDto
    {
        public int InvoiceId { get; set; }
        public int PatientId { get; set; }
        public int InsurerId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountOutstanding { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public List<EventDto> Events { get; set; } = new List<EventDto>();
    }

    // Removed InvoiceCreationRequest and InvoiceCreationResult classes
    // as they're no longer needed since invoice creation is handled through the message consumer

    public class InvoiceDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int InsurerId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountOutstanding { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsFullyPaid { get; set; }
        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
        public List<EventDto> EventHistory { get; set; } = new List<EventDto>();
    }
}
