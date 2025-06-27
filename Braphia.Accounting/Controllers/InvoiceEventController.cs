using Braphia.Accounting.EventSourcing.Services;
using Braphia.Accounting.EventSourcing.Projections;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Accounting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceEventController : ControllerBase
    {
        private readonly IInvoiceEventService _invoiceEventService;
        private readonly IInvoiceProjectionService _projectionService;
        private readonly ILogger<InvoiceEventController> _logger;

        public InvoiceEventController(
            IInvoiceEventService invoiceEventService,
            IInvoiceProjectionService projectionService,
            ILogger<InvoiceEventController> logger)
        {
            _invoiceEventService = invoiceEventService;
            _projectionService = projectionService;
            _logger = logger;
        }

        [HttpPost("{invoiceAggregateId}/payment")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProcessPayment(
            Guid invoiceAggregateId,
            [FromBody] PaymentRequest request)
        {
            if (invoiceAggregateId == Guid.Empty)
            {
                return BadRequest("Valid invoice aggregate ID is required");
            }

            if (request == null || request.PaymentAmount <= 0)
            {
                return BadRequest("Valid payment amount is required");
            }

            if (request.InsurerId <= 0)
            {
                return BadRequest("Valid insurer ID is required");
            }

            _logger.LogInformation("Processing payment of {Amount:C} from insurer {InsurerId} for invoice {InvoiceAggregateId}",
                request.PaymentAmount, request.InsurerId, invoiceAggregateId);

            try
            {
                await _invoiceEventService.ProcessPaymentAsync(
                    invoiceAggregateId,
                    request.InsurerId,
                    request.PaymentAmount,
                    request.PaymentReference ?? string.Empty);

                var updatedInvoice = await _invoiceEventService.GetInvoiceAsync(invoiceAggregateId);
                if (updatedInvoice == null)
                {
                    return NotFound($"Invoice with ID {invoiceAggregateId} not found");
                }

                return Ok(new
                {
                    InvoiceAggregateId = updatedInvoice.Id,
                    TotalAmount = updatedInvoice.TotalAmount,
                    AmountPaid = updatedInvoice.AmountPaid,
                    AmountOutstanding = updatedInvoice.AmountOutstanding,
                    IsFullyPaid = updatedInvoice.IsFullyPaid,
                    PaymentProcessed = true
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid payment operation for invoice {InvoiceAggregateId}", invoiceAggregateId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for invoice {InvoiceAggregateId}", invoiceAggregateId);
                return StatusCode(500, "Internal server error while processing payment");
            }
        }

        [HttpGet("insurer/{insurerId}/outstanding")]
        [ProducesResponseType(typeof(InsurerOutstandingBalance), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInsurerOutstandingBalance(int insurerId)
        {
            if (insurerId <= 0)
            {
                return BadRequest("Valid insurer ID is required");
            }

            try
            {
                var outstandingBalance = await _projectionService.GetInsurerOutstandingBalanceAsync(insurerId);
                if (outstandingBalance == null)
                {
                    return NotFound($"No outstanding balance found for insurer {insurerId}");
                }

                return Ok(outstandingBalance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching outstanding balance for insurer {InsurerId}", insurerId);
                return StatusCode(500, "Internal server error while fetching outstanding balance");
            }
        }

        [HttpGet("insurer/{insurerId}/invoices")]
        [ProducesResponseType(typeof(IEnumerable<InvoiceProjection>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInsurerInvoices(int insurerId, [FromQuery] bool? onlyOutstanding = null)
        {
            if (insurerId <= 0)
            {
                return BadRequest("Valid insurer ID is required");
            }

            try
            {
                var invoices = await _projectionService.GetInvoicesByInsurerAsync(insurerId, onlyOutstanding);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching invoices for insurer {InsurerId}", insurerId);
                return StatusCode(500, "Internal server error while fetching invoices");
            }
        }
    }

    public class PaymentRequest
    {
        public int InsurerId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string? PaymentReference { get; set; }
    }
}
