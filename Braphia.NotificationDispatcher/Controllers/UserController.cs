using Braphia.NotificationDispatcher.Models;
using Braphia.NotificationDispatcher.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.NotificationDispatcher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly ILogger<UserController> _logger;
        public readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "Users")]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var records = await _userRepository.GetAllUsersAsync();
                if (records == null || !records.Any())
                {
                    _logger.LogWarning("No users found in the database.");
                    return NotFound("No users found");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching users.");
                return StatusCode(500, "Internal server error while fetching users");
            }
        }

        [HttpGet("{id}", Name = "UserById")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var record = await _userRepository.GetUserByIdAsync(id);
                if (record == null)
                {
                    _logger.LogWarning("No user found with ID {id}.", id);
                    return NotFound($"No user found with ID {id}");
                }
                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the user by ID.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching the user by ID.");
                return StatusCode(500, "Internal server error while fetching user");
            }
        }
    }
}
