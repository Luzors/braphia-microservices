using Braphia.Accounting.Models;
using Braphia.Accounting.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Accounting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceRepository invoiceRepository, ILogger<InvoiceController> logger)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetAllInvoices()
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllInvoicesAsync();
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all invoices");
                return StatusCode(500, "Internal server error while retrieving invoices");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoiceById(int id)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(id);
                if (invoice == null)
                {
                    return NotFound($"Invoice with ID {id} not found");
                }
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice with ID {InvoiceId}", id);
                return StatusCode(500, "Internal server error while retrieving invoice");
            }
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoicesByPatientId(int patientId)
        {
            try
            {
                var invoices = await _invoiceRepository.GetInvoicesByPatientIdAsync(patientId);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices for patient {PatientId}", patientId);
                return StatusCode(500, "Internal server error while retrieving patient invoices");
            }
        }

        [HttpGet("insurer/{insurerId}")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoicesByInsurerId(int insurerId)
        {
            try
            {
                var invoices = await _invoiceRepository.GetInvoicesByInsurerIdAsync(insurerId);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices for insurer {InsurerId}", insurerId);
                return StatusCode(500, "Internal server error while retrieving insurer invoices");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Invoice>> CreateInvoice([FromBody] Invoice invoice)
        {
            if (invoice == null)
            {
                return BadRequest("Invoice data is required");
            }

            if (invoice.PatientId <= 0)
            {
                return BadRequest("Valid PatientId is required");
            }

            if (invoice.InsurerId <= 0)
            {
                return BadRequest("Valid InsurerId is required");
            }

            if (invoice.Amount <= 0)
            {
                return BadRequest("Amount must be greater than 0");
            }

            if (string.IsNullOrWhiteSpace(invoice.Description))
            {
                return BadRequest("Description is required");
            }

            try
            {
                var success = await _invoiceRepository.AddInvoiceAsync(invoice);
                if (success)
                {
                    _logger.LogInformation("Successfully created invoice with ID {InvoiceId} for insurer {InsurerId}", invoice.Id, invoice.InsurerId);
                    return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.Id }, invoice);
                }
                else
                {
                    _logger.LogError("Failed to create invoice for patient {PatientId}", invoice.PatientId);
                    return StatusCode(500, "Failed to create invoice");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice for patient {PatientId}", invoice.PatientId);
                return StatusCode(500, "Internal server error while creating invoice");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateInvoice(int id, [FromBody] Invoice invoice)
        {
            if (invoice == null)
            {
                return BadRequest("Invoice data is required");
            }

            if (id != invoice.Id)
            {
                return BadRequest("ID mismatch between route and body");
            }

            if (invoice.PatientId <= 0)
            {
                return BadRequest("Valid PatientId is required");
            }

            if (invoice.InsurerId <= 0)
            {
                return BadRequest("Valid InsurerId is required");
            }

            if (invoice.Amount <= 0)
            {
                return BadRequest("Amount must be greater than 0");
            }

            if (string.IsNullOrWhiteSpace(invoice.Description))
            {
                return BadRequest("Description is required");
            }

            try
            {
                var existingInvoice = await _invoiceRepository.GetInvoiceByIdAsync(id);
                if (existingInvoice == null)
                {
                    return NotFound($"Invoice with ID {id} not found");
                }

                var success = await _invoiceRepository.UpdateInvoiceAsync(invoice);
                if (success)
                {
                    _logger.LogInformation("Successfully updated invoice with ID {InvoiceId}", id);
                    return NoContent();
                }
                else
                {
                    _logger.LogError("Failed to update invoice with ID {InvoiceId}", id);
                    return StatusCode(500, "Failed to update invoice");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating invoice with ID {InvoiceId}", id);
                return StatusCode(500, "Internal server error while updating invoice");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInvoice(int id)
        {
            try
            {
                var existingInvoice = await _invoiceRepository.GetInvoiceByIdAsync(id);
                if (existingInvoice == null)
                {
                    return NotFound($"Invoice with ID {id} not found");
                }

                var success = await _invoiceRepository.DeleteInvoiceAsync(id);
                if (success)
                {
                    _logger.LogInformation("Successfully deleted invoice with ID {InvoiceId}", id);
                    return NoContent();
                }
                else
                {
                    _logger.LogError("Failed to delete invoice with ID {InvoiceId}", id);
                    return StatusCode(500, "Failed to delete invoice");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting invoice with ID {InvoiceId}", id);
                return StatusCode(500, "Internal server error while deleting invoice");
            }
        }
    }
}
