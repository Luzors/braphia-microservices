using Braphia.MedicalManagement.Models;
using Braphia.MedicalManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.MedicalManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : Controller
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IPatientRepository patientRepository, ILogger<PatientController> logger)
        {
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAsync()
        {
            _logger.LogInformation("Fetching all patients");
            try
            {
                IEnumerable<Patient> patients = await _patientRepository.GetAllPatientsAsync();
                return Ok(patients);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt when fetching patients");
                return Unauthorized("You are not authorized to access patients");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when fetching patients");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching patients");
                return StatusCode(500, "Internal server error while fetching patients");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetAsync(int id)
        {
            _logger.LogInformation($"Fetching patient with ID {id}");
            
            if (id <= 0)
            {
                _logger.LogWarning($"Invalid patient ID: {id}");
                return BadRequest("Patient ID must be a positive integer");
            }

            try
            {
                Patient? patient = await _patientRepository.GetPatientByIdAsync(id);
                if (patient == null)
                {
                    _logger.LogWarning($"Patient with ID {id} not found");
                    return NotFound($"Patient with ID {id} not found");
                }
                return Ok(patient);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized access attempt for patient ID {id}");
                return Unauthorized("You are not authorized to access this patient");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Patient with ID {id} not found");
                return NotFound($"Patient with ID {id} not found");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid argument when fetching patient ID {id}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error fetching patient ID {id}");
                return StatusCode(500, "Internal server error while fetching patient");
            }
        }
    }
}