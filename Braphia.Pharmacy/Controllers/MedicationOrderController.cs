using Braphia.Pharmacy.Models;
using Braphia.Pharmacy.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationOrderController : ControllerBase
    {
        private readonly ILogger<MedicationOrderController> _logger;
        private readonly IMedicationOrderRepository _medicationOrderRepository;
        private readonly IMedicationRepository _medicationRepository;

        public MedicationOrderController(ILogger<MedicationOrderController> logger, IMedicationOrderRepository medicationOrderRepository, IMedicationRepository medicationRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _medicationOrderRepository = medicationOrderRepository ?? throw new ArgumentNullException(nameof(medicationOrderRepository));
            _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));
        }

        [HttpGet(Name = "GetMedicationOrders")]
        [ProducesResponseType(typeof(IEnumerable<MedicationOrder>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var records = await _medicationOrderRepository.GetAllMedicationOrdersAsync();
                if (records == null || !records.Any())
                {
                    _logger.LogWarning("No medication orders found in the database.");
                    return NotFound("No medication orders found");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching medication orders.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching medication orders.");
                return StatusCode(500, "Internal server error while fetching medication orders");
            }
        }

        [HttpGet("{id}", Name = "GetMedicationOrderById")]
        [ProducesResponseType(typeof(MedicationOrder), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var record = await _medicationOrderRepository.GetMedicationOrderByIdAsync(id);
                if (record == null)
                {
                    _logger.LogWarning("No medication order found with ID {id}.", id);
                    return NotFound($"No medication order found with ID {id}");
                }
                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching medication order by ID.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching medication order by ID.");
                return StatusCode(500, "Internal server error while fetching medication order by ID");
            }
        }

        [HttpPost(Name = "CreateMedicationOrder")]
        [ProducesResponseType(typeof(MedicationOrder), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] MedicationOrder medicationOrder)
        {
            if (medicationOrder == null)
            {
                _logger.LogError("Medication order is null.");
                return BadRequest("Medication order cannot be null");
            }
            try
            {
                var created = await _medicationOrderRepository.CreateMedicationOrderAsync(medicationOrder);
                if (!created)
                {
                    _logger.LogError("Failed to create medication order.");
                    return StatusCode(500, "Failed to create medication order");
                }
                return CreatedAtAction(nameof(Get), new { id = medicationOrder.Id }, medicationOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating medication order.");
                return StatusCode(500, "Internal server error while creating medication order");
            }
        }

        [HttpDelete("{id}", Name = "DeleteMedicationOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _medicationOrderRepository.DeleteMedicationOrderAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning("No medication order found with ID {id} to delete.", id);
                    return NotFound($"No medication order found with ID {id}");
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting medication order.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting medication order.");
                return StatusCode(500, "Internal server error while deleting medication order");
            }
        }

        [HttpPut("{id}/complete", Name = "CompleteMedicationOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Complete(int id)
        {
            try
            {
                var completed = await _medicationOrderRepository.CompleteMedicationOrderAsync(id);
                if (!completed)
                {
                    _logger.LogWarning("No medication order found with ID {id} to complete.", id);
                    return NotFound($"No medication order found with ID {id}");
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while completing medication order.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while completing medication order.");
                return StatusCode(500, "Internal server error while completing medication order");
            }
        }

        [HttpPost("{id}/medication", Name = "AddMedicationToMedicationOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> AddMedicationToOrder(int id, [FromQuery] int medicationId, [FromQuery] int amount)
        {
            if (amount <= 0)
            {
                _logger.LogError("Amount must be greater than zero.");
                return BadRequest("Amount must be greater than zero");
            }
            try
            {
                var medication = await _medicationRepository.GetMedicationByIdAsync(medicationId);
                if (medication == null)
                {
                    _logger.LogWarning("No medication found with ID {medicationId}.", medicationId);
                    return NotFound($"No medication found with ID {medicationId}");
                }
                var added = await _medicationOrderRepository.AddMedicationToMedicationOrderAsync(id, medication, amount);
                if (!added)
                {
                    _logger.LogError("Failed to add medication to medication order with ID {id}.", id);
                    return StatusCode(500, "Failed to add medication to medication order");
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while adding medication to medication order.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding medication to medication order.");
                return StatusCode(500, "Internal server error while adding medication to medication order");
            }
        }

        [HttpDelete("{id}/medication", Name = "RemoveMedicationFromMedicationOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveMedicationFromOrder(int id, [FromQuery] int medicationId, [FromQuery] int amount)
        {
            if (amount <= 0)
            {
                _logger.LogError("Amount must be greater than zero.");
                return BadRequest("Amount must be greater than zero");
            }
            try
            {
                var medication = await _medicationRepository.GetMedicationByIdAsync(medicationId);
                if (medication == null)
                {
                    _logger.LogWarning("No medication found with ID {medicationId}.", medicationId);
                    return NotFound($"No medication found with ID {medicationId}");
                }
                var removed = await _medicationOrderRepository.RemoveMedicationFromMedicationOrderAsync(id, medication, amount);
                if (!removed)
                {
                    _logger.LogError("Failed to remove medication from medication order with ID {id}.", id);
                    return StatusCode(500, "Failed to remove medication from medication order");
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while removing medication from medication order.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while removing medication from medication order.");
                return StatusCode(500, "Internal server error while removing medication from medication order");
            }
        }
    }
}
