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

                await _testRepository.AddTestAsync(test);
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

                // Update test with completion data
                test.CompletedDate = DateTime.UtcNow;
                test.Result = result.Trim();

                bool updateResult = await _testRepository.UpdateTestAsync(test);
                if (updateResult)
                {
                    _logger.LogInformation($"Test with ID {id} completed successfully");
                    return Ok(new
                    {
                        message = "Test completed successfully",
                        testId = id,
                        completedDate = test.CompletedDate,
                        result = test.Result
                    });
                }
                else
                {
                    _logger.LogError("Failed to update test with ID {Id}", id);
                    return StatusCode(500, "Failed to complete test");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized attempt to complete test ID {id}");
                return Unauthorized("You are not authorized to complete this test");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Test with ID {id} not found");
                return NotFound($"Test with ID {id} not found");
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

        [HttpPut("{id}", Name = "UpdateTestStatus")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> Put(int id, [FromBody] Test test)
        {
            if (test == null || test.Id != id)
            {
                return BadRequest("Test data is null or ID mismatch");
            }
            try
            {
                var updated = await _testRepository.UpdateTestAsync(test);
                if (!updated)
                {
                    return NotFound($"No test found with ID {id} to update");
                }
                // Publish an event after updating the test
                await _publishEndpoint.Publish(new { TestId = id, Message = "Test updated successfully" });
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
