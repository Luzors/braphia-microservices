using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.AppointmentManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhysicianController : ControllerBase
    {
        private readonly IPhysicianRepository _physicianRepository;

        public PhysicianController(IPhysicianRepository physicianRepository)
        {
            _physicianRepository = physicianRepository;
        }
        [HttpPost("physician")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePhysician([FromBody] Physician physician)
        {
            try
            {
                var records = await _physicianRepository.AddPhysicianAsync(physician);
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while adding physician");
            }
        }
    }
}
