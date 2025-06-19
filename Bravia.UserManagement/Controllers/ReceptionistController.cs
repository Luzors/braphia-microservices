using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceptionistController : ControllerBase
    {
        private readonly ILogger<ReceptionistController> _logger;
        private readonly IReceptionistRepository _receptionistRepository;

        public ReceptionistController(ILogger<ReceptionistController> logger, IReceptionistRepository receptionistRepository)
        {
            _logger = logger;
            _receptionistRepository = receptionistRepository ?? throw new ArgumentNullException(nameof(receptionistRepository));
        }

        [HttpGet(Name = "Receptionists")]
        [ProducesResponseType(typeof(IEnumerable<Receptionist>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Fetching all receptionists from the database.");

            try
            {
                var records = await _receptionistRepository.GetAllReceptionistsAsync();
                if (records == null || !records.Any())
                {
                    _logger.LogInformation("No receptionists found in the database.");
                    return NotFound("No receptionists found.");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error fetching receptionists from the database.");
                return BadRequest("Invalid request while fetching receptionists");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching receptionists from the database.");
                return StatusCode(500, "Internal server error while fetching receptionists");
            }
        }

        [HttpGet("{id}", Name = "ReceptionistById")]
        [ProducesResponseType(typeof(Receptionist), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("Fetching receptionist with ID: {id} from the database.", id);

            try
            {
                var record = await _receptionistRepository.GetReceptionistByIdAsync(id);
                if (record == null)
                {
                    _logger.LogInformation("Receptionist with ID {id} not found in the database.", id);
                    return NotFound($"Receptionist with ID {id} not found.");
                }
                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error fetching receptionist with ID {id} from the database.", id);
                return BadRequest("Invalid request while fetching receptionist");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching receptionist with ID {id} from the database.", id);
                return StatusCode(500, "Internal server error while fetching receptionist");
            }
        }

        [HttpPost(Name = "Receptionists")]
        [ProducesResponseType(typeof(Receptionist), StatusCodes.Status201Created)]

        public async Task<IActionResult> Post([FromBody] Receptionist receptionist)
        {
            _logger.LogInformation("Adding a new receptionist to the database.");

            try
            {
                var records = await _receptionistRepository.AddReceptionistAsync(receptionist);
                if (!records)
                {
                    _logger.LogWarning("Failed to add receptionist to the database.");
                    return BadRequest("Failed to add receptionist");
                }
                _logger.LogInformation("Receptionist with ID: {id} added successfully.", receptionist.Id);
                return CreatedAtRoute("ReceptionistById", new { id = receptionist.Id }, receptionist);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error adding receptionist to the database.");
                return BadRequest("Invalid request while adding receptionist");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding receptionist to the database.");
                return StatusCode(500, "Internal server error while adding receptionist");
            }
        }
    }
}
