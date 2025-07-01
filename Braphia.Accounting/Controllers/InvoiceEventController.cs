using Braphia.Accounting.EventSourcing.Aggregates;
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
        
        // Note: Direct invoice creation via API is removed
        // Invoices are now created only through the message consumer when lab tests are completed

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

                return Ok(MapToDto(invoice));
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

                return Ok(MapToDto(updatedInvoice));
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
                return Ok(invoices.Select(MapToDto));
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
                return Ok(invoices.Select(MapToDto));
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
                return Ok(invoices.Select(MapToDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices for patient {PatientId}", patientId);
                return StatusCode(500, "Internal server error while retrieving patient invoices");
            }
        }

        private InvoiceDto MapToDto(InvoiceAggregate invoice)
        {
            return new InvoiceDto
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
        }
    }

    public class PaymentRequest
    {
        public int InsurerId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string? PaymentReference { get; set; }
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
    }
}
