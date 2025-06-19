using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhysicianController : ControllerBase
    {
        private readonly ILogger<PhysicianController> _logger;
        private readonly IPhysicianRepository _physicianRepository;

        public PhysicianController(ILogger<PhysicianController> logger, IPhysicianRepository physicianRepository)
        {
            _logger = logger;
            _physicianRepository = physicianRepository ?? throw new ArgumentNullException(nameof(physicianRepository));
        }

        [HttpGet(Name = "Physicians")]
        [ProducesResponseType(typeof(IEnumerable<Physician>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Fetching all physicians from the database.");

            try
            {
                var records = await _physicianRepository.GetAllPhysiciansAsync();
                if (records == null || !records.Any())
                {
                    _logger.LogInformation("No physicians found in the database.");
                    return NotFound("No physicians found.");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error fetching physicians from the database.");
                return BadRequest("Invalid request while fetching physicians");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching physicians from the database.");
                return StatusCode(500, "Internal server error while fetching physicians");
            }
        }

        [HttpGet("{id}", Name = "PhysicianById")]
        [ProducesResponseType(typeof(Physician), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("Fetching physician with ID: {id} from the database.", id);

            try
            {
                var records = await _physicianRepository.GetPhysicianByIdAsync(id);
                if (records == null)
                {
                    _logger.LogInformation("Physician with ID {id} not found in the database.", id);
                    return NotFound($"Physician with ID {id} not found.");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error fetching physician with ID {id} from the database.", id);
                return BadRequest("Invalid request while fetching physician");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching physician with ID {id} from the database.", id);
                return StatusCode(500, "Internal server error while fetching physician");
            }
        }

        [HttpPost(Name = "Physicians")]
        [ProducesResponseType(typeof(Physician), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] Physician Physician)
        {
            _logger.LogInformation("Adding a new physician to the database.");

            try
            {
                var records = await _physicianRepository.AddPhysicianAsync(Physician);
                if (!records)
                {
                    _logger.LogWarning("Failed to add physician to the database.");
                    return BadRequest("Failed to add physician");
                }
                _logger.LogInformation("Physician with ID: {id} added successfully.", Physician.Id);
                return CreatedAtRoute("PhysicianById", new { id = Physician.Id }, Physician);

            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error fetching medical records for patient with ID {patientId}", Physician.Id);
                return BadRequest("Invalid request while fetching medical records");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding physician to the database.");
                return StatusCode(500, "Internal server error while adding physician");
            }
        }
    }
}
