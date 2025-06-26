using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Laboratory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public readonly ITestRepository _testRepository;
        public readonly IPublishEndpoint _publishEndpoint;

        public TestController(ITestRepository testRepository, IPublishEndpoint publishEndpoint)
        {
            _testRepository = testRepository ?? throw new ArgumentNullException(nameof(testRepository));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        [HttpGet(Name = "Tests")]
        [ProducesResponseType(typeof(IEnumerable<Test>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var records = await _testRepository.GetAllTestsAsync();
                if (records == null || !records.Any())
                {
                    return NotFound("No tests found");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error while fetching tests");
            }
        }

        [HttpGet("{id}", Name = "TestById")]
        [ProducesResponseType(typeof(Test), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var record = await _testRepository.GetTestByIdAsync(id);
                if (record == null)
                {
                    return NotFound($"No test found with ID {id}");
                }
                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error while fetching test");
            }
        }
        [HttpPost(Name = "Test")]
        [ProducesResponseType(typeof(Test), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] Test test)
        {
            if (test == null)
            {
                return BadRequest("Test data is null");
            }
            try
            {
                var createdTest = await _testRepository.AddTestAsync(test);
                if (createdTest == false)
                {
                    return BadRequest("Failed to create test");
                }
                // Publish an event after creating the test
                await _publishEndpoint.Publish(new { TestId = test.Id, Message = "Test created successfully" });
                return CreatedAtAction(nameof(Get), new { id = test.Id }, test);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}", Name = "UpdateTestStatus")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> Put(Guid id, [FromBody] Test test)
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
