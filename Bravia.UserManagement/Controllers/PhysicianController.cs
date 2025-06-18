using Braphia.UserManagement.Database;
using Braphia.UserManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhysicianController : ControllerBase
    {
        private readonly ILogger<PhysicianController> _logger;

        private readonly DBContext _dbContext;

        public PhysicianController(ILogger<PhysicianController> logger, DBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet(Name = "Physicians")]
        public IEnumerable<Physician> Get()
        {
            _logger.LogInformation("Fetching all physicians from the database.");
            return _dbContext.Physician.ToList();
        }

        [HttpGet("{id}", Name = "PhysicianById")]
        public Physician? Get(int id)
        {
            _logger.LogInformation("Fetching physician with ID: {id} from the database.", id);
            return _dbContext.Physician.FirstOrDefault(
                p => p.Id == id
            );
        }

        [HttpPost(Name = "Physicians")]
        public IActionResult Post([FromBody] Physician Physician)
        {
            _logger.LogInformation("Adding a new physician to the database.");
            if (Physician == null)
                return BadRequest("Physician cannot be null");

            _dbContext.Physician.Add(Physician);
            _dbContext.SaveChanges();

            _logger.LogInformation("Physician with ID: {id} added successfully.", Physician.Id);
            return CreatedAtRoute("PhysicianById", new { id = Physician.Id }, Physician);
        }
    }
}
