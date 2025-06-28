using Braphia.NotificationDispatcher.Models;
using Braphia.NotificationDispatcher.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.NotificationDispatcher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaboratoryController : ControllerBase
    {
        public readonly ILogger<LaboratoryController> _logger;
        public readonly ILaboratoryRepository _laboratoryRepository;

        public LaboratoryController(ILaboratoryRepository laboratoryRepository, ILogger<LaboratoryController> logger)
        {
            _laboratoryRepository = laboratoryRepository ?? throw new ArgumentNullException(nameof(laboratoryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "Laboratories")]
        [ProducesResponseType(typeof(IEnumerable<Laboratory>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var records = await _laboratoryRepository.GetAllLaboratoriesAsync();
                if (records == null || !records.Any())
                {
                    _logger.LogWarning("No laboratorys found in the database.");
                    return NotFound("No laboratorys found");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching laboratorys.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching laboratorys.");
                return StatusCode(500, "Internal server error while fetching laboratorys");
            }
        }

        [HttpGet("{id}", Name = "LaboratoryById")]
        [ProducesResponseType(typeof(Laboratory), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var record = await _laboratoryRepository.GetLaboratoryByIdAsync(id);
                if (record == null)
                {
                    _logger.LogWarning("No laboratory found with ID {id}.", id);
                    return NotFound($"No laboratory found with ID {id}");
                }
                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the laboratory by ID.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching the laboratory by ID.");
                return StatusCode(500, "Internal server error while fetching laboratory");
            }
        }
    }
}
