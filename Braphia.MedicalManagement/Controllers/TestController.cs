using Braphia.MedicalManagement.Models;
using Braphia.MedicalManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.MedicalManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly ITestRepository _testRepository;
        private readonly ILogger<TestController> _logger;

        public TestController(ITestRepository testRepository, ILogger<TestController> logger)
        {
            _testRepository = testRepository ?? throw new ArgumentNullException(nameof(testRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Test>>> GetAsync()
        {
            _logger.LogInformation("Fetching all tests");
            try
            {
                IEnumerable<Test> tests = await _testRepository.GetAllTestsAsync();
                return Ok(tests);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt when fetching tests");
                return Unauthorized("You are not authorized to access tests");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when fetching tests");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching tests");
                return StatusCode(500, "Internal server error while fetching tests");
            }
        }

        [HttpGet("{id}", Name = "GetTest")]
        public async Task<ActionResult<Test>> GetAsync(int id)
        {
            _logger.LogInformation($"Fetching test with ID {id}");

            if (id <= 0)
            {
                _logger.LogWarning($"Invalid test ID: {id}");
                return BadRequest("Test ID must be a positive integer");
            }

            try
            {
                Test test = await _testRepository.GetTestAsync(id);
                if (test == null)
                {
                    _logger.LogWarning($"Test with ID {id} not found");
                    return NotFound($"Test with ID {id} not found");
                }
                return Ok(test);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized access attempt for test ID {id}");
                return Unauthorized("You are not authorized to access this test");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Test with ID {id} not found");
                return NotFound($"Test with ID {id} not found");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid argument when fetching test ID {id}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error fetching test ID {id}");
                return StatusCode(500, "Internal server error while fetching test");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Test test)
        {
            _logger.LogInformation("Adding a new test: " + test.ToString());

            if (test == null)
            {
                _logger.LogWarning("Test data is null");
                return BadRequest("Test data cannot be null");
            }

            // Basic validation
            if (test.PatientId <= 0)
            {
                _logger.LogWarning($"Invalid patient ID: {test.PatientId}");
                return BadRequest("Valid patient ID is required");
            }

            try
            {
                test.CompletedDate = null;
                test.Result = null;

                bool result = await _testRepository.AddTestAsync(test);
                if (result)
                {
                    _logger.LogInformation($"Test added successfully with ID {test.Id}");
                    return CreatedAtRoute("GetTest", new { id = test.Id }, test);
                }
                else
                {
                    _logger.LogError("Repository returned false when adding test");
                    return StatusCode(500, "Failed to add test");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to add test");
                return Unauthorized("You are not authorized to create tests");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid test data provided");
                return BadRequest($"Invalid test data: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when adding test");
                return Conflict($"Unable to create test: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding test");
                return StatusCode(500, "Internal server error while adding test");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Test test)
        {
            _logger.LogInformation($"Updating test with ID {id}");

            if (id <= 0)
            {
                _logger.LogWarning($"Invalid test ID: {id}");
                return BadRequest("Test ID must be a positive integer");
            }

            if (test == null)
            {
                _logger.LogWarning("Test data is null");
                return BadRequest("Test data cannot be null");
            }

            try
            {
                bool result = await _testRepository.UpdateTestAsync(test);
                if (result)
                {
                    _logger.LogInformation($"Test with ID {id} updated successfully");
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning($"Test with ID {id} not found for update");
                    return NotFound($"Test with ID {id} not found");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized attempt to update test ID {id}");
                return Unauthorized("You are not authorized to update this test");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Test with ID {id} not found for update");
                return NotFound($"Test with ID {id} not found");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid test data for update ID {id}");
                return BadRequest($"Invalid test data: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Invalid operation when updating test ID {id}");
                return Conflict($"Unable to update test: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error updating test ID {id}");
                return StatusCode(500, "Internal server error while updating test");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            _logger.LogInformation($"Deleting test with ID {id}");

            if (id <= 0)
            {
                _logger.LogWarning($"Invalid test ID: {id}");
                return BadRequest("Test ID must be a positive integer");
            }

            try
            {
                bool result = await _testRepository.DeleteTestAsync(id);
                if (result)
                {
                    _logger.LogInformation($"Test with ID {id} deleted successfully");
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning($"Test with ID {id} not found for deletion");
                    return NotFound($"Test with ID {id} not found");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized attempt to delete test ID {id}");
                return Unauthorized("You are not authorized to delete this test");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Test with ID {id} not found for deletion");
                return NotFound($"Test with ID {id} not found");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Cannot delete test ID {id} due to business rules");
                return Conflict($"Cannot delete test: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid argument when deleting test ID {id}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error deleting test ID {id}");
                return StatusCode(500, "Internal server error while deleting test");
            }
        }
    }
}
