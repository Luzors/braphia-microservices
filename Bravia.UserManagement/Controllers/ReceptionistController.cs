using Braphia.UserManagement.Database;
using Braphia.UserManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceptionistController : ControllerBase
    {
        private readonly ILogger<ReceptionistController> _logger;

        private readonly DBContext _dbContext;

        public ReceptionistController(ILogger<ReceptionistController> logger, DBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet(Name = "Receptionists")]
        public IEnumerable<Receptionist> Get()
        {
            _logger.LogInformation("Fetching all receptionists from the database.");
            return _dbContext.Receptionist.ToList();
        }

        [HttpGet("{id}", Name = "ReceptionistById")]
        public Receptionist? Get(int id)
        {
            _logger.LogInformation("Fetching receptionist with ID: {id} from the database.", id);
            return _dbContext.Receptionist.FirstOrDefault(
                p => p.Id == id
            );
        }

        [HttpPost(Name = "Receptionists")]
        public IActionResult Post([FromBody] Receptionist receptionist)
        {
            _logger.LogInformation("Adding a new receptionist to the database.");
            if (receptionist == null)
                return BadRequest("Receptionist cannot be null");

            _dbContext.Receptionist.Add(receptionist);
            _dbContext.SaveChanges();

            _logger.LogInformation("Receptionist with ID: {id} added successfully.", receptionist.Id);
            return CreatedAtRoute("ReceptionistById", new { id = receptionist.Id }, receptionist);
        }
    }
}
