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

        [HttpGet("referrals")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReferrals()
        {
            var records = await _referralRepository.GetAllReferralsAsync();
            return Ok(records);
        }
    }
}
