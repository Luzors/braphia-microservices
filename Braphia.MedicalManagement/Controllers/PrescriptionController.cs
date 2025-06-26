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
        public async Task<IEnumerable<Prescription>> GetAsync()
        {
            _logger.LogInformation("Fetching all prescriptions");
            try
            {
                IEnumerable<Prescription> prescriptions = await _prescriptionRepository.GetAllPrescriptionsAsync();
                return prescriptions;
            }
            catch
            {
                _logger.LogError("Error fetching prescriptions");
                throw new Exception("Internal server error while fetching prescriptions");
            }

        }

        [HttpGet("{id}")]
        public async Task<Prescription> GetAsync(int id)
        {
            _logger.LogInformation($"Fetching prescription with ID {id}");
            try
            {
                Prescription prescription = await _prescriptionRepository.GetPrescriptionAsync(id);
                return prescription;
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"Prescription with ID {id} not found");
                throw new KeyNotFoundException($"Prescription with ID {id} not found");
            }
            catch
            {
                _logger.LogError("Error fetching prescription");
                throw new Exception("Internal server error while fetching prescription");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Prescription prescription)
        {
            _logger.LogInformation("Adding a new prescription");
            if (prescription == null)
            {
                _logger.LogWarning("Prescription data is null");
                return BadRequest("Prescription data cannot be null");
            }

            try
            {
                bool result = await _prescriptionRepository.AddPrescriptionAsync(prescription);
                if (result)
                {
                    _logger.LogInformation("Prescription added successfully");
                    return CreatedAtAction(nameof(GetAsync), new { id = prescription.Id }, prescription);
                }
                else
                {
                    _logger.LogError("Failed to add prescription");
                    return StatusCode(500, "Failed to add prescription");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding prescription");
                return StatusCode(500, "Internal server error while adding prescription");
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] Prescription prescription)
        {
            _logger.LogInformation($"Updating prescription with ID {prescription.Id}");
            if (prescription == null)
            {
                _logger.LogWarning("Prescription data is null");
                return BadRequest("Prescription data cannot be null");
            }

            try
            {
                bool result = await _prescriptionRepository.UpdatePrescriptionAsync(prescription);
                if (result)
                {
                    _logger.LogInformation("Prescription updated successfully");
                    return NoContent();
                }
                else
                {
                    _logger.LogError("Failed to update prescription");
                    return StatusCode(500, "Failed to update prescription");
                }
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"Prescription with ID {prescription.Id} not found");
                return NotFound($"Prescription with ID {prescription.Id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating prescription");
                return StatusCode(500, "Internal server error while updating prescription");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            _logger.LogInformation($"Deleting prescription with ID {id}");
            if (id <= 0)
            {
                _logger.LogWarning("Invalid prescription ID");
                return BadRequest("Invalid prescription ID");
            }

            try
            {
                bool result = await _prescriptionRepository.DeletePrescriptionAsync(id);
                if (result)
                {
                    _logger.LogInformation("Prescription deleted successfully");
                    return NoContent();
                }
                else
                {
                    _logger.LogError("Failed to delete prescription");
                    return StatusCode(500, "Failed to delete prescription");
                }
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning($"Prescription with ID {id} not found");
                return NotFound($"Prescription with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting prescription");
                return StatusCode(500, "Internal server error while deleting prescription");
            }
        }
    }
}
