using Braphia.Pharmacy.Models.ExternalObjects;
using Braphia.Pharmacy.Repositories.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Pharmacy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        public readonly ILogger<PatientController> _logger;
        public readonly IPatientRepository _patientRepository;

        public PatientController(IPatientRepository patientRepository, ILogger<PatientController> logger)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "Patients")]
        [ProducesResponseType(typeof(IEnumerable<Patient>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var records = await _patientRepository.GetAllPatientsAsync();
                if (records == null || !records.Any())
                {
                    _logger.LogWarning("No patients found in the database.");
                    return NotFound("No patients found");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching patients.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching patients.");
                return StatusCode(500, "Internal server error while fetching patients");
            }
        }

        [HttpGet("{id}", Name = "PatientById")]
        [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var record = await _patientRepository.GetPatientByIdAsync(id);
                if (record == null)
                {
                    _logger.LogWarning("No patient found with ID {id}.", id);
                    return NotFound($"No patient found with ID {id}");
                }
                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the patient by ID.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching the patient by ID.");
                return StatusCode(500, "Internal server error while fetching patient");
            }
        }
    }
}
