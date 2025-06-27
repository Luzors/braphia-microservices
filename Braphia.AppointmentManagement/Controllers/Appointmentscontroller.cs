using Braphia.AppointmentManagement.Commands.AddAppointment;
using Braphia.AppointmentManagement.Databases.WriteDatabase;
using Braphia.AppointmentManagement.Query.GetAllAppointments;
using Braphia.AppointmentManagement.Query.GetAppointmentById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.AppointmentManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ReadDbContext _readDb;

        public AppointmentsController(IMediator mediator, ReadDbContext readDb)
        {
            _mediator = mediator;
            _readDb = readDb;
        }

        // POST: api/appointments
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreatedCommand command)
        {
            var appointmentId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointmentId }, new { appointmentId });
        }

        // GET: api/appointments
        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var results = await _mediator.Send(new GetAllAppointmentsQuery());
            return results != null ? Ok(results) : NotFound("No appointments found.");
        }

        // GET: api/appointments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetAppointmentByIdQuery(id));
            return result != null ? Ok(result) : NotFound();
        }
    }
}
