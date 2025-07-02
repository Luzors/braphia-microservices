using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Laboratory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CentralLaboratoryController : ControllerBase
    {
        private readonly ICentralLabotoryRepository _centralLaboratory;
        private readonly ILogger<CentralLaboratoryController> _logger;

        public CentralLaboratoryController(ICentralLabotoryRepository centralLabotory, ILogger<CentralLaboratoryController> logger)
        {
            _centralLaboratory = centralLabotory ?? throw new ArgumentNullException(nameof(centralLabotory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "CentralLaboratory")]
        [ProducesResponseType(typeof(IEnumerable<CentralLaboratory>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var records = await _centralLaboratory.GetAllCentralLaboratoriesAsync();
                if (records == null || !records.Any())
                {
                    return NotFound("No patients found");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching central laboratories");
                return StatusCode(500, "Internal server error while fetching patients");
            }
        }

        [HttpGet("{id}", Name = "GetCentralLaboratoryById")]
        [ProducesResponseType(typeof(IEnumerable<CentralLaboratory>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var record = await _centralLaboratory.GetCentralLaboratoryByIdAsync(id);
                if (record == null)
                {
                    return NotFound($"No central laboratory found with ID {id}");
                }
                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching central laboratory with ID {Id}", id);
                return StatusCode(500, "Internal server error while fetching central laboratory");
            }

        }

        [HttpPost(Name = "CentralLaboratory")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] CentralLaboratory centralLaboratory)
        {
            try
            {
                var records = await _centralLaboratory.AddCentralLaboratoryAsync(centralLaboratory);
                return CreatedAtRoute("GetCentralLaboratoryById", new { id = centralLaboratory.Id }, centralLaboratory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding central laboratory");
                return StatusCode(500, "Internal server error while adding patient");
            }
        }
    }
}
