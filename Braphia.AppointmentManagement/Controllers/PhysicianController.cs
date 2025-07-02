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
        
        [HttpGet("physicians")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPhysicians()
        {
            var records = await _physicianRepository.GetAllPhysiciansAsync();
            return Ok(records);
        }
    }
}
