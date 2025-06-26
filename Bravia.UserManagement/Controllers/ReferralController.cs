using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferralController : ControllerBase
    {
        private readonly ILogger<ReferralController> _logger;
        private readonly IReferralRepository _referralRepository;

        public ReferralController(ILogger<ReferralController> logger, IReferralRepository referralRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _referralRepository = referralRepository ?? throw new ArgumentNullException(nameof(referralRepository));
        }

        [HttpGet(Name = "Referrals")]
        [ProducesResponseType(typeof(IEnumerable<Referral>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Fetching all referrals");
            try
            {
                var records = await _referralRepository.GetAllReferralsAsync();
                if (records == null || !records.Any())
                {
                    _logger.LogInformation("No referrals found");
                    return NotFound("No referrals found");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while fetching referrals");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching referrals");
                return StatusCode(500, "Internal server error while fetching referrals");
            }
        }

        [HttpGet("{id}", Name = "ReferralById")]
        [ProducesResponseType(typeof(Referral), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("Fetching referral with ID {id}", id);
            try
            {
                var record = await _referralRepository.GetReferralByIdAsync(id);
                if (record == null)
                {
                    _logger.LogInformation("No referral found with ID {id}", id);
                    return NotFound($"No referral found with ID {id}");
                }
                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while fetching referral");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching referral");
                return StatusCode(500, "Internal server error while fetching referral");
            }
        }

        [HttpPost(Name = "AddReferral")]
        [ProducesResponseType(typeof(Referral), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] Referral referral)
        {
            if (referral == null)
            {
                _logger.LogWarning("Referral object is null");
                return BadRequest("Referral object cannot be null");
            }
            try
            {
                var result = await _referralRepository.AddReferralAsync(referral);
                if (result)
                {
                    _logger.LogInformation("Referral added successfully");
                    return CreatedAtAction(nameof(Get), new { id = referral.Id }, referral);
                }
                else
                {
                    _logger.LogError("Failed to add referral");
                    return StatusCode(500, "Failed to add referral");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding referral");
                return StatusCode(500, "Internal server error while adding referral");
            }
        }

        [HttpDelete("{id}", Name = "DeleteReferral")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting referral with ID {id}", id);
            try
            {
                var result = await _referralRepository.DeleteReferralAsync(id);
                if (result)
                {
                    _logger.LogInformation("Referral with ID {id} deleted successfully", id);
                    return NoContent();
                }
                else
                {
                    _logger.LogError("Failed to delete referral with ID {id}", id);
                    return NotFound($"No referral found with ID {id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting referral with ID {id}", id);
                return StatusCode(500, "Internal server error while deleting referral");
            }
        }
    }
    }
