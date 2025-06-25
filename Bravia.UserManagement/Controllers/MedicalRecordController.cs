using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.UserManagement.Controllers
{
    [Route("api/[controller]/{patientId}")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly ILogger<MedicalRecordController> _logger;
        private readonly IPatientRepository _patientRepository;

        public MedicalRecordController(ILogger<MedicalRecordController> logger, IPatientRepository patientRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        }

        [HttpGet(Name = "MedicalRecordsByPatientId")]
        [ProducesResponseType(typeof(IEnumerable<MedicalRecord>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(int patientId)
        {
            _logger.LogInformation("Fetching medical records for patient with ID {patientId}", patientId);
            try
            {
                var records = await _patientRepository.GetMedicalRecordsByPatientIdAsync(patientId);
                if (records == null || !records.Any())
                {
                    _logger.LogInformation("No medical records found for patient with ID {patientId}", patientId);
                    return NotFound($"No medical records found for Patient ID {patientId}");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while fetching medical records for patient with ID {patientId}", patientId);
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching medical records for patient with ID {patientId}", patientId);
                return StatusCode(500, "Internal server error while fetching medical records");
            }
        }

        [HttpPost(Name = "AddMedicalRecord")]
        [ProducesResponseType(typeof(MedicalRecord), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(int patientId, [FromBody] MedicalRecord medicalRecord)
        {
            _logger.LogInformation("Adding medical record for patient with ID {patientId}", patientId);
            try
            {
                var success = await _patientRepository.AddMedicalRecordAsync(patientId, medicalRecord);
                if (!success)
                {
                    _logger.LogWarning("Failed to add medical record for patient with ID {patientId}", patientId);
                    return BadRequest($"Failed to add medical record for Patient ID {patientId}");
                }

                _logger.LogInformation("Medical record added for patient with ID {patientId}", patientId);
                return CreatedAtRoute("MedicalRecordsByPatientId", new { patientId }, medicalRecord);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while fetching medical records for patient with ID {patientId}", patientId);
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding medical record for patient with ID {patientId}", patientId);
                return StatusCode(500, "Internal server error while adding medical record");
            }

        }

        [HttpDelete("{recordId}", Name = "DeleteMedicalRecord")]
        public async Task<IActionResult> Delete(int patientId, int recordId)
        {
            _logger.LogInformation("Deleting medical record with ID {recordId} for patient with ID {patientId}", recordId, patientId);
            try
            {
                var success = await _patientRepository.DeleteMedicalRecordAsync(patientId, recordId);
                if (!success)
                {
                    _logger.LogWarning("Failed to delete medical record with ID {recordId} for patient with ID {patientId}", recordId, patientId);
                    return NotFound($"Medical record with ID {recordId} not found for Patient ID {patientId}");
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while fetching medical records for patient with ID {patientId}", patientId);
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medical record with ID {recordId} for patient with ID {patientId}", recordId, patientId);
                return StatusCode(500, "Internal server error while deleting medical record");
            }

            _logger.LogInformation("Medical record with ID {recordId} deleted for patient with ID {patientId}", recordId, patientId);

            return NoContent();
        }

        [HttpGet("{recordId}", Name = "MedicalRecordById")]
        [ProducesResponseType(typeof(MedicalRecord), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int patientId, int recordId)
        {
            _logger.LogInformation("Fetching medical record with ID {recordId} for patient with ID {patientId}", recordId, patientId);
            try
            {
                var record = await _patientRepository.GetMedicalRecordByIdAsync(patientId, recordId);
                if (record == null)
                {
                    _logger.LogInformation("No medical record found with ID {recordId} for patient with ID {patientId}", recordId, patientId);
                    return NotFound($"No medical record found with ID {recordId} for Patient ID {patientId}");
                }
                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while fetching medical record with ID {recordId} for patient with ID {patientId}", recordId, patientId);
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching medical record with ID {recordId} for patient with ID {patientId}", recordId, patientId);
                return StatusCode(500, "Internal server error while fetching medical record");
            }
        }

    }
}
