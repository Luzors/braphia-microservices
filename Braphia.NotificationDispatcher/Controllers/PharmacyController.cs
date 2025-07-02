using Braphia.NotificationDispatcher.Models;
using Braphia.NotificationDispatcher.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.NotificationDispatcher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacyController : ControllerBase
    {
        public readonly ILogger<PharmacyController> _logger;
        public readonly IPharmacyRepository _pharmacyRepository;

        public PharmacyController(IPharmacyRepository pharmacyRepository, ILogger<PharmacyController> logger)
        {
            _pharmacyRepository = pharmacyRepository ?? throw new ArgumentNullException(nameof(pharmacyRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "Pharmacies")]
        [ProducesResponseType(typeof(IEnumerable<Pharmacy>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var records = await _pharmacyRepository.GetAllPharmaciesAsync();
                if (records == null || !records.Any())
                {
                    _logger.LogWarning("No pharmacys found in the database.");
                    return NotFound("No pharmacys found");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching pharmacys.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching pharmacys.");
                return StatusCode(500, "Internal server error while fetching pharmacys");
            }
        }

        [HttpGet("{id}", Name = "PharmacyById")]
        [ProducesResponseType(typeof(Pharmacy), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var record = await _pharmacyRepository.GetPharmacyByIdAsync(id);
                if (record == null)
                {
                    _logger.LogWarning("No pharmacy found with ID {id}.", id);
                    return NotFound($"No pharmacy found with ID {id}");
                }
                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the pharmacy by ID.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching the pharmacy by ID.");
                return StatusCode(500, "Internal server error while fetching pharmacy");
            }
        }
    }
}
