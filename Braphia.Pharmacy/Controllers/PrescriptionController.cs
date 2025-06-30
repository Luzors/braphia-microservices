using Braphia.Pharmacy.Models.ExternalObjects;
using Braphia.Pharmacy.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        public readonly ILogger<PrescriptionController> _logger;
        public readonly IPrescriptionRepository _prescriptionRepository;

        public PrescriptionController(IPrescriptionRepository prescriptionRepository, ILogger<PrescriptionController> logger)
        {
            _prescriptionRepository = prescriptionRepository ?? throw new ArgumentNullException(nameof(prescriptionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "Prescriptions")]
        [ProducesResponseType(typeof(IEnumerable<Prescription>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var records = await _prescriptionRepository.GetAllPrescriptionsAsync();
                if (records == null || !records.Any())
                {
                    _logger.LogWarning("No prescriptions found in the database.");
                    return NotFound("No prescriptions found");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching prescriptions.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching prescriptions.");
                return StatusCode(500, "Internal server error while fetching prescriptions");
            }
        }

        [HttpGet("{id}", Name = "PrescriptionById")]
        [ProducesResponseType(typeof(Prescription), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var record = await _prescriptionRepository.GetPrescriptionByIdAsync(id);
                if (record == null)
                {
                    _logger.LogWarning("No prescription found with ID {id}.", id);
                    return NotFound($"No prescription found with ID {id}");
                }
                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the prescription by ID.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching the prescription by ID.");
                return StatusCode(500, "Internal server error while fetching prescription");
            }
        }
    }
}
