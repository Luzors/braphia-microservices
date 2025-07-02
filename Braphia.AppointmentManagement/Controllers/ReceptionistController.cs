using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.AppointmentManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceptionistController : ControllerBase
    {
        private readonly IReceptionistRepository _receptionistRepository;

        public ReceptionistController(IReceptionistRepository receptionistRepository)
        {
            _receptionistRepository = receptionistRepository;
        }
        
        [HttpGet("receptionist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReceptionists()
        {
            var records = await _receptionistRepository.GetAllReceptionistsAsync();
            return Ok(records);
        }

    }
}
