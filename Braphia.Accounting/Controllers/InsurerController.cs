using Braphia.Accounting.Models;
using Braphia.Accounting.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Accounting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsurerController : ControllerBase
    {
        private readonly IInsurerRepository _insurerRepository;
        private readonly ILogger<InsurerController> _logger;

        public InsurerController(IInsurerRepository insurerRepository, ILogger<InsurerController> logger)
        {
            _insurerRepository = insurerRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Insurer>>> GetAllInsurers()
        {
            try
            {
                var insurers = await _insurerRepository.GetAllInsurersAsync();
                return Ok(insurers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all insurers");
                return StatusCode(500, "Internal server error while retrieving insurers");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Insurer>> GetInsurerById(int id)
        {
            try
            {
                var insurer = await _insurerRepository.GetInsurerByIdAsync(id);
                if (insurer == null)
                {
                    return NotFound($"Insurer with ID {id} not found");
                }
                return Ok(insurer);
            }
            catch (Exception ex)
            {                _logger.LogError(ex, "Error retrieving insurer with ID {InsurerId}", id);
                return StatusCode(500, "Internal server error while retrieving insurer");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Insurer>> CreateInsurer([FromBody] Insurer insurer)
        {
            if (insurer == null)
            {
                return BadRequest("Insurer data is required");
            }

            if (string.IsNullOrWhiteSpace(insurer.Name))
            {
                return BadRequest("Insurer name is required");
            }

            if (string.IsNullOrWhiteSpace(insurer.ContactEmail))
            {
                return BadRequest("Contact email is required");
            }

            try
            {
                var success = await _insurerRepository.AddInsurerAsync(insurer);
                if (success)
                {
                    _logger.LogInformation("Successfully created insurer with ID {InsurerId}", insurer.Id);
                    return CreatedAtAction(nameof(GetInsurerById), new { id = insurer.Id }, insurer);
                }
                else
                {
                    _logger.LogError("Failed to create insurer {InsurerName}", insurer.Name);
                    return StatusCode(500, "Failed to create insurer");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating insurer {InsurerName}", insurer.Name);
                return StatusCode(500, "Internal server error while creating insurer");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateInsurer(int id, [FromBody] Insurer insurer)
        {
            if (insurer == null)
            {
                return BadRequest("Insurer data is required");
            }

            if (id != insurer.Id)
            {
                return BadRequest("ID mismatch between route and body");
            }

            if (string.IsNullOrWhiteSpace(insurer.Name))
            {
                return BadRequest("Insurer name is required");
            }

            if (string.IsNullOrWhiteSpace(insurer.ContactEmail))
            {
                return BadRequest("Contact email is required");
            }

            try
            {
                var existingInsurer = await _insurerRepository.GetInsurerByIdAsync(id);
                if (existingInsurer == null)
                {
                    return NotFound($"Insurer with ID {id} not found");
                }

                var success = await _insurerRepository.UpdateInsurerAsync(insurer);
                if (success)
                {
                    _logger.LogInformation("Successfully updated insurer with ID {InsurerId}", id);
                    return NoContent();
                }
                else
                {
                    _logger.LogError("Failed to update insurer with ID {InsurerId}", id);
                    return StatusCode(500, "Failed to update insurer");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating insurer with ID {InsurerId}", id);
                return StatusCode(500, "Internal server error while updating insurer");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInsurer(int id)
        {
            try
            {
                var existingInsurer = await _insurerRepository.GetInsurerByIdAsync(id);
                if (existingInsurer == null)
                {
                    return NotFound($"Insurer with ID {id} not found");
                }

                var success = await _insurerRepository.DeleteInsurerAsync(id);
                if (success)
                {
                    _logger.LogInformation("Successfully deleted insurer with ID {InsurerId}", id);
                    return NoContent();
                }
                else
                {
                    _logger.LogError("Failed to delete insurer with ID {InsurerId}", id);
                    return StatusCode(500, "Failed to delete insurer");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting insurer with ID {InsurerId}", id);
                return StatusCode(500, "Internal server error while deleting insurer");
            }
        }
    }
}
