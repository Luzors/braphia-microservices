using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Laboratory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CentralLaboratoryController : ControllerBase
    {
        private readonly ICentralLabotoryRepository _centralLabotory;
        private readonly IPublishEndpoint _publishEndpoint;

        public CentralLaboratoryController(ICentralLabotoryRepository centralLabotory, IPublishEndpoint publishEndpoint)
        {
            _centralLabotory = centralLabotory ?? throw new ArgumentNullException(nameof(centralLabotory));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        [HttpGet(Name = "CentralLaboratory")]
        [ProducesResponseType(typeof(IEnumerable<CentralLaboratory>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var records = await _centralLabotory.GetAllCentralLaboratoriesAsync();
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
                return StatusCode(500, "Internal server error while fetching patients");
            }
        }

        [HttpGet(Name = "centralLaboratoryById")]
        [ProducesResponseType(typeof(IEnumerable<CentralLaboratory>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var record = await _centralLabotory.GetCentralLaboratoryByIdAsync(id);
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
                return StatusCode(500, "Internal server error while fetching central laboratory");
            }

        }

        [HttpPost(Name = "CentralLaboratory")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] CentralLaboratory centralLaboratory)
        {
            try
            {
                var records = await _centralLabotory.AddCentralLaboratoryAsync(centralLaboratory);
                return CreatedAtRoute("centralLaboratoryById", new { id = centralLaboratory.Id }, centralLaboratory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error while adding patient");
            }
        }
    }
}
