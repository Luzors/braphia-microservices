using Braphia.MedicalManagement.Models;
using Braphia.MedicalManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.MedicalManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MedicalAnalysisController : Controller
    {
        private readonly IMedicalAnalysisRepository _medicalAnalysisRepository;
        private readonly ILogger<MedicalAnalysisController> _logger;

        public MedicalAnalysisController(IMedicalAnalysisRepository medicalAnalysisRepository, ILogger<MedicalAnalysisController> logger)
        {
            _medicalAnalysisRepository = medicalAnalysisRepository ?? throw new ArgumentNullException(nameof(medicalAnalysisRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalAnalysis>>> GetAsync()
        {
            _logger.LogInformation("Fetching all medicalAnalysiss");
            try
            {
                IEnumerable<MedicalAnalysis> medicalAnalysiss = await _medicalAnalysisRepository.GetAllMedicalAnalysissAsync();
                return Ok(medicalAnalysiss);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt when fetching medicalAnalysiss");
                return Unauthorized("You are not authorized to access medicalAnalysiss");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when fetching medicalAnalysiss");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching medicalAnalysiss");
                return StatusCode(500, "Internal server error while fetching medicalAnalysiss");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalAnalysis>> GetAsync(int id)
        {
            _logger.LogInformation($"Fetching medicalAnalysis with ID {id}");

            if (id <= 0)
            {
                _logger.LogWarning($"Invalid medicalAnalysis ID: {id}");
                return BadRequest("MedicalAnalysis ID must be a positive integer");
            }

            try
            {
                MedicalAnalysis medicalAnalysis = await _medicalAnalysisRepository.GetMedicalAnalysisAsync(id);
                if (medicalAnalysis == null)
                {
                    _logger.LogWarning($"MedicalAnalysis with ID {id} not found");
                    return NotFound($"MedicalAnalysis with ID {id} not found");
                }
                return Ok(medicalAnalysis);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized access attempt for medicalAnalysis ID {id}");
                return Unauthorized("You are not authorized to access this medicalAnalysis");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"MedicalAnalysis with ID {id} not found");
                return NotFound($"MedicalAnalysis with ID {id} not found");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid argument when fetching medicalAnalysis ID {id}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error fetching medicalAnalysis ID {id}");
                return StatusCode(500, "Internal server error while fetching medicalAnalysis");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] MedicalAnalysis medicalAnalysis)
        {
            _logger.LogInformation($"Adding a new medicalAnalysis for PatientId: {medicalAnalysis.PatientId}, PhysicianId: {medicalAnalysis.PhysicianId}");

            if (medicalAnalysis == null)
            {
                _logger.LogWarning("MedicalAnalysis data is null");
                return BadRequest("MedicalAnalysis data cannot be null");
            }

            // Basic validation
            if (medicalAnalysis.PatientId <= 0)
            {
                _logger.LogWarning($"Invalid patient ID: {medicalAnalysis.PatientId}");
                return BadRequest("Valid patient ID is required");
            }

            if (medicalAnalysis.PhysicianId <= 0)
            {
                _logger.LogWarning($"Invalid physician ID: {medicalAnalysis.PhysicianId}");
                return BadRequest("Valid physician ID is required");
            }

            try
            {
                bool result = await _medicalAnalysisRepository.AddMedicalAnalysisAsync(medicalAnalysis);
                if (result)
                {
                    _logger.LogInformation($"MedicalAnalysis added successfully with ID {medicalAnalysis.Id}");
                    return StatusCode(201, medicalAnalysis);
                }
                else
                {
                    _logger.LogError("Repository returned false when adding medicalAnalysis");
                    return StatusCode(500, "Failed to add medicalAnalysis");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to add medicalAnalysis");
                return Unauthorized("You are not authorized to create medicalAnalysiss");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid medicalAnalysis data provided");
                return BadRequest($"Invalid medicalAnalysis data: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when adding medicalAnalysis");
                return Conflict($"Unable to create medicalAnalysis: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding medicalAnalysis");
                return StatusCode(500, "Internal server error while adding medicalAnalysis");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] MedicalAnalysis medicalAnalysis)
        {
            _logger.LogInformation($"Updating medicalAnalysis with ID {id}");

            if (id <= 0)
            {
                _logger.LogWarning($"Invalid medicalAnalysis ID: {id}");
                return BadRequest("MedicalAnalysis ID must be a positive integer");
            }

            if (medicalAnalysis == null)
            {
                _logger.LogWarning("MedicalAnalysis data is null");
                return BadRequest("MedicalAnalysis data cannot be null");
            }

            if (id != medicalAnalysis.Id)
            {
                _logger.LogWarning($"Route ID {id} does not match medicalAnalysis ID {medicalAnalysis.Id}");
                return BadRequest("Route ID must match medicalAnalysis ID");
            }

            // Basic validation
            if (medicalAnalysis.PatientId <= 0)
            {
                _logger.LogWarning($"Invalid patient ID: {medicalAnalysis.PatientId}");
                return BadRequest("Valid patient ID is required");
            }

            if (medicalAnalysis.PhysicianId <= 0)
            {
                _logger.LogWarning($"Invalid physician ID: {medicalAnalysis.PhysicianId}");
                return BadRequest("Valid physician ID is required");
            }

            try
            {
                bool result = await _medicalAnalysisRepository.UpdateMedicalAnalysisAsync(medicalAnalysis);
                if (result)
                {
                    _logger.LogInformation($"MedicalAnalysis with ID {id} updated successfully");
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning($"MedicalAnalysis with ID {id} not found for update");
                    return NotFound($"MedicalAnalysis with ID {id} not found");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized attempt to update medicalAnalysis ID {id}");
                return Unauthorized("You are not authorized to update this medicalAnalysis");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"MedicalAnalysis with ID {id} not found for update");
                return NotFound($"MedicalAnalysis with ID {id} not found");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid medicalAnalysis data for update ID {id}");
                return BadRequest($"Invalid medicalAnalysis data: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Invalid operation when updating medicalAnalysis ID {id}");
                return Conflict($"Unable to update medicalAnalysis: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error updating medicalAnalysis ID {id}");
                return StatusCode(500, "Internal server error while updating medicalAnalysis");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            _logger.LogInformation($"Deleting medicalAnalysis with ID {id}");

            if (id <= 0)
            {
                _logger.LogWarning($"Invalid medicalAnalysis ID: {id}");
                return BadRequest("MedicalAnalysis ID must be a positive integer");
            }

            try
            {
                bool result = await _medicalAnalysisRepository.DeleteMedicalAnalysisAsync(id);
                if (result)
                {
                    _logger.LogInformation($"MedicalAnalysis with ID {id} deleted successfully");
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning($"MedicalAnalysis with ID {id} not found for deletion");
                    return NotFound($"MedicalAnalysis with ID {id} not found");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized attempt to delete medicalAnalysis ID {id}");
                return Unauthorized("You are not authorized to delete this medicalAnalysis");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"MedicalAnalysis with ID {id} not found for deletion");
                return NotFound($"MedicalAnalysis with ID {id} not found");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Cannot delete medicalAnalysis ID {id} due to business rules");
                return Conflict($"Cannot delete medicalAnalysis: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid argument when deleting medicalAnalysis ID {id}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error deleting medicalAnalysis ID {id}");
                return StatusCode(500, "Internal server error while deleting medicalAnalysis");
            }
        }
    }
}
