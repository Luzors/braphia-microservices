using Braphia.Accounting.EventSourcing;
using Braphia.Accounting.EventSourcing.Aggregates;
using Braphia.Accounting.EventSourcing.Events;
using Braphia.Accounting.EventSourcing.Services;
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
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
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

                // Payments includen bij enkele invoice ophalen
                var paymentEvents = await _invoiceEventService.GetPaymentEventsByInvoiceIdAsync(invoiceId);
                var invoiceDto = await MapToDto(invoice, true, paymentEvents);

                return Ok(invoiceDto);
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

                // Get payment events for this invoice
                var paymentEvents = await _invoiceEventService.GetPaymentEventsByInvoiceIdAsync(invoiceId);
                
                // Create the DTO with payments included
                var invoiceDto = await MapToDto(updatedInvoice, true, paymentEvents);

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
                    invoiceDtos.Add(await MapToDto(invoice, false));
                }
                
                return Ok(invoiceDtos);
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
                    invoiceDtos.Add(await MapToDto(invoice, false));
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
                    invoiceDtos.Add(await MapToDto(invoice, false));
                }
                
                return Ok(invoiceDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices for patient {PatientId}", patientId);
                return StatusCode(500, "Internal server error while retrieving patient invoices");
            }
        }

        private async Task<InvoiceDto> MapToDto(InvoiceAggregate invoice, bool includePayments = false, IEnumerable<BaseEvent>? paymentEvents = null)
        {
            var dto = new InvoiceDto
            {
                Id = invoice.Id,
                PatientId = invoice.PatientId,
                InsurerId = invoice.InsurerId,
                LabTestId = invoice.LabTestId,
                TotalAmount = invoice.TotalAmount,
                AmountPaid = invoice.AmountPaid,
                AmountOutstanding = invoice.AmountOutstanding,
                Description = invoice.Description,
                CreatedDate = invoice.CreatedDate,
                IsFullyPaid = invoice.IsFullyPaid
            };
            
            // Add payment details only if requested
            if (includePayments)
            {
                // Fetch payment events if not provided
                var payments = paymentEvents ?? await _invoiceEventService.GetPaymentEventsByInvoiceIdAsync(invoice.Id);
                
                foreach (var evt in payments)
                {
                    if (evt is PaymentReceivedEvent paymentEvent)
                    {
                        dto.Payments.Add(new PaymentDto
                        {
                            InsurerId = paymentEvent.InsurerId,
                            PaymentAmount = paymentEvent.PaymentAmount,
                            PaymentReference = paymentEvent.PaymentReference,
                            PaymentDate = paymentEvent.PaymentDate
                        });
                    }
                }
            }
            
            return dto;
        }
    }

    public class PaymentRequest
    {
        public int InsurerId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string? PaymentReference { get; set; }
    }

    public class PaymentDto
    {
        public int InsurerId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentReference { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
    }

    // Removed InvoiceCreationRequest and InvoiceCreationResult classes
    // as they're no longer needed since invoice creation is handled through the message consumer

    public class InvoiceDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int InsurerId { get; set; }
        public int LabTestId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountOutstanding { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsFullyPaid { get; set; }
        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
    }
}
