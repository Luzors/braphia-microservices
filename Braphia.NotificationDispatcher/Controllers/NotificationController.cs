using Braphia.NotificationDispatcher.Models;
using Braphia.NotificationDispatcher.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.NotificationDispatcher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepository, ILogger<NotificationController> logger)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "Notifications")]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var records = await _notificationRepository.GetAllNotificationsAsync();
                if (records == null || !records.Any())
                {
                    _logger.LogWarning("No notifications found in the database.");
                    return NotFound("No notifications found");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching notifications.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching notifications.");
                return StatusCode(500, "Internal server error while fetching notifications");
            }
        }

        [HttpGet("{id}", Name = "NotificationById")]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var record = await _notificationRepository.GetNotificationByIdAsync(id);
                if (record == null)
                {
                    _logger.LogWarning("No notification found with ID {id}.", id);
                    return NotFound($"No notification found with ID {id}");
                }
                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the notification.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching the notification.");
                return StatusCode(500, "Internal server error while fetching the notification");
            }
        }

        [HttpGet("by-user/{userId}", Name = "NotificationsByUser")]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser(int userId)
        {
            try
            {
                var records = await _notificationRepository.GetNotificationsByUserIdAsync(userId);
                if (records == null || !records.Any())
                {
                    _logger.LogWarning("No notifications found for user ID {userId}.", userId);
                    return NotFound($"No notifications found for user ID {userId}");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching notifications by user ID.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching notifications by user ID.");
                return StatusCode(500, "Internal server error while fetching notifications by user ID");
            }
        }

        [HttpGet("by-pharmacy/{pharmacyId}", Name = "NotificationsByPharmacy")]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPharmacy(int pharmacyId)
        {
            try
            {
                var records = await _notificationRepository.GetNotificationsByPharmacyIdAsync(pharmacyId);
                if (records == null || !records.Any())
                {
                    _logger.LogWarning("No notifications found for pharmacy ID {pharmacyId}.", pharmacyId);
                    return NotFound($"No notifications found for pharmacy ID {pharmacyId}");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching notifications by pharmacy ID.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching notifications by pharmacy ID.");
                return StatusCode(500, "Internal server error while fetching notifications by pharmacy ID");
            }
        }

        [HttpGet("by-laboratory/{laboratoryId}", Name = "NotificationsByLaboratory")]
        [ProducesResponseType(typeof(IEnumerable<Notification>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByLaboratory(int laboratoryId)
        {
            try
            {
                var records = await _notificationRepository.GetNotificationsByLaboratoryIdAsync(laboratoryId);
                if (records == null || !records.Any())
                {
                    _logger.LogWarning("No notifications found for laboratory ID {laboratoryId}.", laboratoryId);
                    return NotFound($"No notifications found for laboratory ID {laboratoryId}");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while fetching notifications by laboratory ID.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching notifications by laboratory ID.");
                return StatusCode(500, "Internal server error while fetching notifications by laboratory ID");
            }
        }

        [HttpDelete("{id}", Name = "DeleteNotification")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _notificationRepository.DeleteNotificationAsync(id);
                if (!success)
                {
                    _logger.LogWarning("No notification found with ID {id} to delete.", id);
                    return NotFound($"No notification found with ID {id}");
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the notification.");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting the notification.");
                return StatusCode(500, "Internal server error while deleting the notification");
            }
        }
    }
}
