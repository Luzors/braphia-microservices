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
        [HttpPost("receptionist")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateReceptionist([FromBody] Receptionist receptionist)
        {
            try
            {
                var records = await _receptionistRepository.AddReceptionistAsync(receptionist);
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while adding receptionist");
            }
        }

    }
}
