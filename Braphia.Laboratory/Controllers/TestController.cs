using Braphia.Laboratory.Events;
using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Braphia.Laboratory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public readonly ITestRepository _testRepository;
        public readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger, ITestRepository testRepository, IPublishEndpoint publishEndpoint)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _testRepository = testRepository ?? throw new ArgumentNullException(nameof(testRepository));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTests()
        {
            _logger.LogInformation("Fetching all tests");

            try
            {
                var tests = await _testRepository.GetAllAsync();
                if (tests == null || !tests.Any())
                {
                    _logger.LogInformation("No tests found");
                    return NotFound("No tests found.");
                }

                return Ok(tests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all tests");
                return StatusCode(500, "Internal server error while fetching tests");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTestById(int id)
        {
            _logger.LogInformation("Fetching test with ID {Id}", id);

            try
            {
                var test = await _testRepository.GetByIdAsync(id);
                if (test == null)
                {
                    _logger.LogInformation("Test with ID {Id} not found", id);
                    return NotFound("Test not found.");
                }

                return Ok(test);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching test with ID {Id}", id);
                return StatusCode(500, "Internal server error while fetching test");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTest([FromBody] Test test)
        {
            _logger.LogInformation("Creating a new test");

            try
            {
                if (test == null)
                {
                    _logger.LogWarning("Test data is null");
                    return BadRequest("Test data is required.");
                }
                
                _logger.LogInformation("Received test data: {TestData}", test);

                test.CompletedDate = null;
                test.Result = null;

                _logger.LogInformation("Received test data no date res: {TestData}", test);

                await _testRepository.AddAsync(test);
                _logger.LogInformation("Test created successfully with ID {Id}", test.Id);
                return CreatedAtAction(nameof(CreateTest), new { id = test.Id }, test);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while creating test");
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test");
                return StatusCode(500, "Internal server error while creating test");
            }
        }

        [HttpPut("CompleteTest/{id}")]
        public async Task<IActionResult> CompleteTest(int id, [FromBody] string result)
        {
            _logger.LogInformation("Completing test with ID {Id}", id);

            try
            {
                var test = await _testRepository.GetByIdAsync(id);
                if (test == null)
                {
                    _logger.LogWarning("Test with ID {Id} not found", id);
                    return NotFound("Test not found.");
                }

                test.CompletedDate = DateTime.UtcNow;
                test.Result = result;

                await _testRepository.UpdateAsync(test);
                _logger.LogInformation("Test with ID {Id} completed successfully", id);

                // Stuur TestCompletedEvent
                await _publishEndpoint.Publish(new Message(
                    messageType: "TestCompleted",
                    data: new TestCompletedEvent(test)
                ));

                return Ok(test);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while completing test with ID {Id}", id);
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing test with ID {Id}", id);
                return StatusCode(500, "Internal server error while completing test");
            }
        }
    }
}
