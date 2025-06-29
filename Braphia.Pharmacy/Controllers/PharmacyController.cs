using Braphia.Pharmacy.Models.ExternalObjects;
using Braphia.Pharmacy.Repositories.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Pharmacy.Controllers
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
        [ProducesResponseType(typeof(IEnumerable<Models.Pharmacy>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var records = await _pharmacyRepository.GetAllPharmaciesAsync();
                if (records == null || !records.Any())
                {
                    _logger.LogWarning("No pharmacies found in the database.");
                    return NotFound("No pharmacies found");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching pharmacies.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching pharmacies.");
                return StatusCode(500, "Internal server error while fetching pharmacies");
            }
        }

        [HttpGet("{id}", Name = "PharmacyById")]
        [ProducesResponseType(typeof(Models.Pharmacy), StatusCodes.Status200OK)]
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
                _logger.LogError(ex, "An error occurred while fetching the pharmacy.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching the pharmacy.");
                return StatusCode(500, "Internal server error while fetching the pharmacy");
            }
        }

        [HttpPost(Name = "CreatePharmacy")]
        [ProducesResponseType(typeof(Models.Pharmacy), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] Models.Pharmacy pharmacy)
        {
            if (pharmacy == null)
            {
                _logger.LogError("Pharmacy object is null.");
                return BadRequest("Pharmacy object cannot be null");
            }
            try
            {
                var createdPharmacy = await _pharmacyRepository.AddPharmacyAsync(pharmacy);
                if (!createdPharmacy)
                {
                    _logger.LogError("Failed to create pharmacy.");
                    return StatusCode(500, "Failed to create pharmacy");
                }
                return CreatedAtAction(nameof(Get), new { id = pharmacy.Id }, pharmacy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the pharmacy.");
                return StatusCode(500, "Internal server error while creating the pharmacy");
            }
        }

        [HttpDelete("{id}", Name = "DeletePharmacy")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _pharmacyRepository.DeletePharmacyAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning("No pharmacy found with ID {id} to delete.", id);
                    return NotFound($"No pharmacy found with ID {id}");
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the pharmacy.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting the pharmacy.");
                return StatusCode(500, "Internal server error while deleting the pharmacy");
            }
        }
    }
}