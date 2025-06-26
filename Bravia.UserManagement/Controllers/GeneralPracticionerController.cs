using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralPracticionerController : ControllerBase
    {
        private readonly ILogger<GeneralPracticionerController> _logger;
        private readonly IGeneralPracticionerRepository _generalPracticionerRepository;
        private readonly IReferralRepository _referralRepository;

        public GeneralPracticionerController(ILogger<GeneralPracticionerController> logger, IGeneralPracticionerRepository generalPracticionerRepository, IReferralRepository referralRepository)
        {
            _logger = logger;
            _generalPracticionerRepository = generalPracticionerRepository ?? throw new ArgumentNullException(nameof(generalPracticionerRepository));
            _referralRepository = referralRepository ?? throw new ArgumentNullException(nameof(referralRepository));
        }

        [HttpGet(Name = "GeneralPracticioners")]
        [ProducesResponseType(typeof(IEnumerable<GeneralPracticioner>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Fetching all generalPracticioners from the database.");

            try
            {
                var records = await _generalPracticionerRepository.GetAllGeneralPracticionersAsync();
                if (records == null || !records.Any())
                {
                    _logger.LogInformation("No generalPracticioners found in the database.");
                    return NotFound("No generalPracticioners found.");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error fetching generalPracticioners from the database.");
                return BadRequest("Invalid request while fetching generalPracticioners");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching generalPracticioners from the database.");
                return StatusCode(500, "Internal server error while fetching generalPracticioners");
            }
        }

        [HttpGet("{id}", Name = "GeneralPracticionerById")]
        [ProducesResponseType(typeof(GeneralPracticioner), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("Fetching generalPracticioner with ID: {id} from the database.", id);

            try
            {
                var records = await _generalPracticionerRepository.GetGeneralPracticionerByIdAsync(id);
                if (records == null)
                {
                    _logger.LogInformation("GeneralPracticioner with ID {id} not found in the database.", id);
                    return NotFound($"GeneralPracticioner with ID {id} not found.");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error fetching generalPracticioner with ID {id} from the database.", id);
                return BadRequest("Invalid request while fetching generalPracticioner");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching generalPracticioner with ID {id} from the database.", id);
                return StatusCode(500, "Internal server error while fetching generalPracticioner");
            }
        }

        [HttpPost(Name = "GeneralPracticioners")]
        [ProducesResponseType(typeof(GeneralPracticioner), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] GeneralPracticioner generalPracticioner)
        {
            _logger.LogInformation("Adding a new generalPracticioner to the database.");

            try
            {
                var records = await _generalPracticionerRepository.AddGeneralPracticionerAsync(generalPracticioner);
                if (!records)
                {
                    _logger.LogWarning("Failed to add generalPracticioner to the database.");
                    return BadRequest("Failed to add generalPracticioner");
                }
                _logger.LogInformation("GeneralPracticioner with ID: {id} added successfully.", generalPracticioner.Id);
                return CreatedAtRoute("GeneralPracticionerById", new { id = generalPracticioner.Id }, generalPracticioner);

            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error adding generalPracticioner to the database.");
                return BadRequest("Invalid request while adding generalPracticioner");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding generalPracticioner to the database.");
                return StatusCode(500, "Internal server error while adding generalPracticioner");
            }
        }

        [HttpGet("{id}/referrals", Name = "GeneralPracticionerReferrals")]
        [ProducesResponseType(typeof(IEnumerable<Referral>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReferrals(int id)
        {
            _logger.LogInformation("Fetching referrals for generalPracticioner with ID: {id} from the database.", id);
            try
            {
                var records = await _referralRepository.GetReferralsByGeneralPracticionerIdAsync(id);
                if (records == null || !records.Any())
                {
                    _logger.LogInformation("No referrals found for generalPracticioner with ID {id}.", id);
                    return NotFound($"No referrals found for generalPracticioner with ID {id}.");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error fetching referrals for generalPracticioner with ID {id} from the database.", id);
                return BadRequest("Invalid request while fetching referrals");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching referrals for generalPracticioner with ID {id} from the database.", id);
                return StatusCode(500, "Internal server error while fetching referrals");
            }
        }

        [HttpDelete("{id}", Name = "DeleteGeneralPracticioner")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting generalPracticioner with ID: {id}", id);
            try
            {
                var deleted = await _generalPracticionerRepository.DeleteGeneralPracticionerAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning("Failed to delete generalPracticioner with ID: {id}", id);
                    return NotFound($"GeneralPracticioner with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting generalPracticioner with ID: {id}", id);
                return StatusCode(500, "Internal server error while deleting generalPracticioner");
            }
        }
    }
}