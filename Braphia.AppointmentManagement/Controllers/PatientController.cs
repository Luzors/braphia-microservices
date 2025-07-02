using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.AppointmentManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;

        public PatientController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        [HttpGet("patients")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> GetPatients()
        {
            var records = await _patientRepository.GetAllPatientsAsync();
            return Ok(records);
        }
    }
}
