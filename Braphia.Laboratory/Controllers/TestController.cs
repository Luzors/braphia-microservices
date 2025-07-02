using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Braphia.Laboratory.Events.Tests;

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
                var test = await _testRepository.GetTestByIdAsync(id);
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

        [HttpPut("{id}/Complete")]
        public async Task<IActionResult> CompleteTest(int id, [FromBody] CompleteTestDto body)
        {
            _logger.LogInformation($"Completing test with ID {id}");

            // Validate input parameters
            if (id <= 0)
            {
                _logger.LogWarning($"Invalid test ID: {id}");
                return BadRequest("Test ID must be a positive integer");
            }

            if (string.IsNullOrWhiteSpace(body.Result))
            {
                _logger.LogWarning("Test result is null or empty");
                return BadRequest("Test result cannot be null or empty");
            }

            string result = body.Result.Trim();

            try
            {
                Test test = await _testRepository.GetTestByIdAsync(id);
                if (test == null)
                {
                    _logger.LogWarning("Test with ID {Id} not found", id);
                    return NotFound($"Test with ID {id} not found");
                }
                if (test.CompletedDate.HasValue)
                {
                    _logger.LogWarning("Test with ID {Id} is already completed", id);
                    return Conflict("Test is already completed");
                }
                if (test.Result != null)
                {
                    _logger.LogWarning("Test with ID {Id} already has a result", id);
                    return Conflict("Test already has a result");
                }

                // Update test with completion data
                test.CompletedDate = DateTime.UtcNow;
                test.Result = result.Trim();

                bool updateResult = await _testRepository.UpdateTestAsync(test);
                _logger.LogInformation("Test with ID {Id} completed successfully", id);

                await _publishEndpoint.Publish(new Message(new TestCompletedEvent(test)));
                return Ok(test);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid argument when completing test ID {id}");
                return BadRequest($"Invalid test data: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Invalid operation when completing test ID {id}");
                return Conflict($"Unable to complete test: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error completing test ID {id}");
                return StatusCode(500, "Internal server error while completing test");
            }
        }
    }
}
