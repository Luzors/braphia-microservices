using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.AppointmentManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralController : ControllerBase
    {
        private readonly IReferralRepository _referralRepository;

        public ReferralController (IReferralRepository referralRepository)
        {
            _referralRepository = referralRepository;
        }

        [HttpPost("referral")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateReferral([FromBody] Referral referral)
        {
            try
            {
                var records = await _referralRepository.AddReferralAsync(referral);
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while adding referral");
            }
        }
    }
}
