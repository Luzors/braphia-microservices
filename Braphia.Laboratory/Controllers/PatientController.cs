using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Laboratory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IPatientRepository _patientRepository;

        public PatientController(ILogger<PatientController> logger, IPatientRepository patientRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        }

        [HttpGet(Name = "Patients")]
        [ProducesResponseType(typeof(IEnumerable<Patient>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Fetching all patients");

            try
            {
                var records = await _patientRepository.GetAllPatientsAsync();
                if (records == null || !records.Any())
                {
                    _logger.LogInformation("No patients found");
                    return NotFound("No patients found");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while fetching patients");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching patients");
                return StatusCode(500, "Internal server error while fetching patients");
            }
        }

        [HttpGet("{id}", Name = "PatientById")]
        [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("Fetching patient with ID {Id}", id);
            try
            {
                var records = await _patientRepository.GetPatientByIdAsync(id);
                if (records == null)
                {
                    _logger.LogInformation("No patient found with ID {Id}", id);
                    return NotFound($"No patient found with ID {id}");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while fetching patient with ID {Id}", id);
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching patient with ID {Id}", id);
                return StatusCode(500, "Internal server error while fetching patient");
            }
        }
    }
}