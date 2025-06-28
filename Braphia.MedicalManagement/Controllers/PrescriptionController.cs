using Braphia.MedicalManagement.Models;
using Braphia.MedicalManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.MedicalManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrescriptionController : Controller
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly ILogger<PrescriptionController> _logger;

        public PrescriptionController(IPrescriptionRepository prescriptionRepository, ILogger<PrescriptionController> logger)
        {
            _prescriptionRepository = prescriptionRepository ?? throw new ArgumentNullException(nameof(prescriptionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetAsync()
        {
            _logger.LogInformation("Fetching all prescriptions");
            try
            {
                IEnumerable<Prescription> prescriptions = await _prescriptionRepository.GetAllPrescriptionsAsync();
                return Ok(prescriptions);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt when fetching prescriptions");
                return Unauthorized("You are not authorized to access prescriptions");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when fetching prescriptions");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching prescriptions");
                return StatusCode(500, "Internal server error while fetching prescriptions");
            }
        }

        [HttpGet("{id}", Name = "GetPrescription")]
        public async Task<ActionResult<Prescription>> GetAsync(int id)
        {
            _logger.LogInformation($"Fetching prescription with ID {id}");

            if (id <= 0)
            {
                _logger.LogWarning($"Invalid prescription ID: {id}");
                return BadRequest("Prescription ID must be a positive integer");
            }

            try
            {
                Prescription prescription = await _prescriptionRepository.GetPrescriptionAsync(id);
                if (prescription == null)
                {
                    _logger.LogWarning($"Prescription with ID {id} not found");
                    return NotFound($"Prescription with ID {id} not found");
                }
                return Ok(prescription);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized access attempt for prescription ID {id}");
                return Unauthorized("You are not authorized to access this prescription");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Prescription with ID {id} not found");
                return NotFound($"Prescription with ID {id} not found");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid argument when fetching prescription ID {id}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error fetching prescription ID {id}");
                return StatusCode(500, "Internal server error while fetching prescription");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Prescription prescription)
        {
            _logger.LogInformation("Adding a new prescription: " + prescription.ToString());

            if (prescription == null)
            {
                _logger.LogWarning("Prescription data is null");
                return BadRequest("Prescription data cannot be null");
            }

            // Basic validation
            if (string.IsNullOrWhiteSpace(prescription.Medicine))
            {
                _logger.LogWarning("Prescription medicine is required");
                return BadRequest("Medicine name is required");
            }

            if (prescription.PatientId <= 0)
            {
                _logger.LogWarning($"Invalid patient ID: {prescription.PatientId}");
                return BadRequest("Valid patient ID is required");
            }

            if (prescription.PhysicianId <= 0)
            {
                _logger.LogWarning($"Invalid physician ID: {prescription.PhysicianId}");
                return BadRequest("Valid physician ID is required");
            }

            try
            {
                bool result = await _prescriptionRepository.AddPrescriptionAsync(prescription);
                if (result)
                {
                    _logger.LogInformation($"Prescription added successfully with ID {prescription.Id}");
                    return CreatedAtRoute("GetPrescription", new { id = prescription.Id }, prescription);
                }
                else
                {
                    _logger.LogError("Repository returned false when adding prescription");
                    return StatusCode(500, "Failed to add prescription");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to add prescription");
                return Unauthorized("You are not authorized to create prescriptions");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid prescription data provided");
                return BadRequest($"Invalid prescription data: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when adding prescription");
                return Conflict($"Unable to create prescription: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding prescription");
                return StatusCode(500, "Internal server error while adding prescription");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Prescription prescription)
        {
            _logger.LogInformation($"Updating prescription with ID {id}");

            if (id <= 0)
            {
                _logger.LogWarning($"Invalid prescription ID: {id}");
                return BadRequest("Prescription ID must be a positive integer");
            }

            if (prescription == null)
            {
                _logger.LogWarning("Prescription data is null");
                return BadRequest("Prescription data cannot be null");
            }

            if (id != prescription.Id)
            {
                _logger.LogWarning($"Route ID {id} does not match prescription ID {prescription.Id}");
                return BadRequest("Route ID must match prescription ID");
            }

            // Basic validation
            if (string.IsNullOrWhiteSpace(prescription.Medicine))
            {
                _logger.LogWarning("Prescription medicine is required");
                return BadRequest("Medicine name is required");
            }

            if (prescription.PatientId <= 0)
            {
                _logger.LogWarning($"Invalid patient ID: {prescription.PatientId}");
                return BadRequest("Valid patient ID is required");
            }

            if (prescription.PhysicianId <= 0)
            {
                _logger.LogWarning($"Invalid physician ID: {prescription.PhysicianId}");
                return BadRequest("Valid physician ID is required");
            }

            try
            {
                bool result = await _prescriptionRepository.UpdatePrescriptionAsync(prescription);
                if (result)
                {
                    _logger.LogInformation($"Prescription with ID {id} updated successfully");
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning($"Prescription with ID {id} not found for update");
                    return NotFound($"Prescription with ID {id} not found");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized attempt to update prescription ID {id}");
                return Unauthorized("You are not authorized to update this prescription");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Prescription with ID {id} not found for update");
                return NotFound($"Prescription with ID {id} not found");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid prescription data for update ID {id}");
                return BadRequest($"Invalid prescription data: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Invalid operation when updating prescription ID {id}");
                return Conflict($"Unable to update prescription: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error updating prescription ID {id}");
                return StatusCode(500, "Internal server error while updating prescription");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            _logger.LogInformation($"Deleting prescription with ID {id}");

            if (id <= 0)
            {
                _logger.LogWarning($"Invalid prescription ID: {id}");
                return BadRequest("Prescription ID must be a positive integer");
            }

            try
            {
                bool result = await _prescriptionRepository.DeletePrescriptionAsync(id);
                if (result)
                {
                    _logger.LogInformation($"Prescription with ID {id} deleted successfully");
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning($"Prescription with ID {id} not found for deletion");
                    return NotFound($"Prescription with ID {id} not found");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized attempt to delete prescription ID {id}");
                return Unauthorized("You are not authorized to delete this prescription");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Prescription with ID {id} not found for deletion");
                return NotFound($"Prescription with ID {id} not found");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Cannot delete prescription ID {id} due to business rules");
                return Conflict($"Cannot delete prescription: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid argument when deleting prescription ID {id}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error deleting prescription ID {id}");
                return StatusCode(500, "Internal server error while deleting prescription");
            }
        }
    }
}
