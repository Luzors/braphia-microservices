using Braphia.UserManagement.Events;
using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.UserManagement.Controllers
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

        [HttpPost(Name = "Patients")]
        [ProducesResponseType(typeof(Patient), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] Patient patient)
        {
            _logger.LogInformation("Adding new patient");
            try
            {
                var records = await _patientRepository.AddPatientAsync(patient);
                _logger.LogInformation("Patient with ID {id} created", patient.Id);                
                return CreatedAtRoute("PatientById", new { id = patient.Id }, patient);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while adding patient");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding patient");
                return StatusCode(500, "Internal server error while adding patient");
            }
        }
    }
}
