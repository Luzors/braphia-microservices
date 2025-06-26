using Braphia.Accounting.Models;
using Braphia.Accounting.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Accounting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IPatientRepository _patientRepository;
        private readonly IInsurerRepository _insurerRepository;

        public PatientController(ILogger<PatientController> logger, IPatientRepository patientRepository, IInsurerRepository insurerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _insurerRepository = insurerRepository ?? throw new ArgumentNullException(nameof(insurerRepository));
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
            _logger.LogInformation("Fetching patient with ID {id}", id);
            try
            {
                var records = await _patientRepository.GetPatientByIdAsync(id);
                if (records == null)
                {
                    _logger.LogInformation("No patient found with ID {id}", id);
                    return NotFound($"No patient found with ID {id}");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while fetching patient with ID {id}", id);
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching patient with ID {id}", id);
                return StatusCode(500, "Internal server error while fetching patient");
            }
        }

        [HttpPut("{id}/assign-insurer/{insurerId}")]
        [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
        public async Task<IActionResult> AssignInsurer(int id, int insurerId)
        {
            if (id <= 0)
            {
                return BadRequest("Valid patient ID is required");
            }

            if (insurerId <= 0)
            {
                return BadRequest("Valid insurer ID is required");
            }

            _logger.LogInformation("Assigning insurer {InsurerId} to patient {PatientId}", insurerId, id);

            try
            {
                // Check if patient exists
                var patient = await _patientRepository.GetPatientByIdAsync(id);
                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", id);
                    return NotFound($"Patient with ID {id} not found");
                }

                // Check if insurer exists
                var insurer = await _insurerRepository.GetInsurerByIdAsync(insurerId);
                if (insurer == null)
                {
                    _logger.LogWarning("Insurer with ID {InsurerId} not found", insurerId);
                    return NotFound($"Insurer with ID {insurerId} not found");
                }

                // Assign insurer to patient
                patient.InsurerId = insurerId;
                var success = await _patientRepository.UpdatePatientAsync(patient);
                
                if (success)
                {
                    _logger.LogInformation("Successfully assigned insurer {InsurerId} to patient {PatientId}", insurerId, id);
                    return Ok(patient);
                }
                else
                {
                    _logger.LogError("Failed to assign insurer {InsurerId} to patient {PatientId}", insurerId, id);
                    return StatusCode(500, "Failed to assign insurer to patient");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning insurer {InsurerId} to patient {PatientId}", insurerId, id);
                return StatusCode(500, "Internal server error while assigning insurer to patient");
            }
        }
    }
}
