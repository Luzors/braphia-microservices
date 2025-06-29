using Braphia.Pharmacy.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationController : ControllerBase
    {
        public readonly ILogger<MedicationController> _logger;
        public readonly IMedicationRepository _medicationRepository;
        public MedicationController(IMedicationRepository medicationRepository, ILogger<MedicationController> logger)
        {
            _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "Medications")]
        [ProducesResponseType(typeof(IEnumerable<Models.Medication>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var records = await _medicationRepository.GetAllMedicationsAsync();
                if (records == null || !records.Any())
                {
                    _logger.LogWarning("No medications found in the database.");
                    return NotFound("No medications found");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching medications.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching medications.");
                return StatusCode(500, "Internal server error while fetching medications");
            }
        }

        [HttpGet("{id}", Name = "MedicationById")]
        [ProducesResponseType(typeof(Models.Medication), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var record = await _medicationRepository.GetMedicationByIdAsync(id);
                if (record == null)
                {
                    _logger.LogWarning("No medication found with ID {id}.", id);
                    return NotFound($"No medication found with ID {id}");
                }
                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching medication by ID.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching medication by ID.");
                return StatusCode(500, "Internal server error while fetching medication by ID");
            }
        }

        [HttpPost(Name = "CreateMedication")]
        [ProducesResponseType(typeof(Models.Medication), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] Models.Medication medication)
        {
            if (medication == null)
            {
                _logger.LogError("Medication object is null.");
                return BadRequest("Medication object cannot be null");
            }
            try
            {
                var createdMedication = await _medicationRepository.AddMedicationAsync(medication);
                if (!createdMedication)
                {
                    _logger.LogError("Failed to create medication.");
                    return BadRequest("Failed to create medication");
                }
                return CreatedAtAction(nameof(Get), new { id = medication.Id }, medication);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while creating medication.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating medication.");
                return StatusCode(500, "Internal server error while creating medication");
            }
        }

        [HttpDelete("{id}", Name = "DeleteMedication")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _medicationRepository.DeleteMedicationAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning("No medication found with ID {id} to delete.", id);
                    return NotFound($"No medication found with ID {id}");
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting medication.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting medication.");
                return StatusCode(500, "Internal server error while deleting medication");
            }
        }
    }
}
