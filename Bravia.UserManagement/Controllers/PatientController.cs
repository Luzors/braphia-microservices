using Braphia.UserManagement.Database;
using Braphia.UserManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;

        private readonly DBContext _dbContext;

        public PatientController(ILogger<PatientController> logger, DBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet(Name = "Patients")]
        public IEnumerable<Patient> Get()
        {
            _logger.LogInformation("Fetching all patients");
            return _dbContext.Patient.ToList();
        }

        [HttpGet("{id}", Name = "PatientById")]
        public Patient? Get(int id)
        {
            _logger.LogInformation("Fetching patient with ID {id}", id);
            return _dbContext.Patient.FirstOrDefault(
                p => p.Id == id
            );
        }

        [HttpPost(Name = "Patients")]
        public IActionResult Post([FromBody] Patient patient)
        {
            _logger.LogInformation("Adding new patient");
            if (patient == null)
            {
                return BadRequest("Patient cannot be null");
            }
            _dbContext.Patient.Add(patient);
            _dbContext.SaveChanges();
            _logger.LogInformation("Patient with ID {id} created", patient.Id);
            return CreatedAtRoute("PatientById", new { id = patient.Id }, patient);
        }
    }
}
