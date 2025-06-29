using Braphia.AppointmentManagement.Commands.AddAppointment;
using Braphia.AppointmentManagement.Commands.AppointmentRescheduled;
using Braphia.AppointmentManagement.Databases.WriteDatabase;
using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Models;
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
        private readonly IAppointmentRepository _appointmentRepository;




        public AppointmentsController(IMediator mediator, ReadDbContext readDb, IAppointmentRepository appointmentRepository)
        {
            _mediator = mediator;
            _readDb = readDb;
            _appointmentRepository = appointmentRepository;
        }

        // POST: api/appointments
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentCreatedCommand command)
        {
            //Mediator will handle the command and publish the event
            //Will send the command to the matching handler
            var appointmentId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = appointmentId }, new { appointmentId });
        }
        [HttpPost("GetAllFromWrite")]
        public async Task<IActionResult> GetAllFromWriteDb()
        {
            var results = await _appointmentRepository.GetAllAppointmentsAsync();
            return results != null ? Ok(results) : NotFound("No appointments found in write database.");
        }

        [HttpPost("Reschedule")]
        public async Task<IActionResult> RescheduleAppointment([FromBody] AppointmentRescheduledCommand command)
        {
            //Mediator will handle the command and publish the event
            //Will send the command to the matching handler
            var appointmentId = await _mediator.Send(command);
            return Ok(new { appointmentId });
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

        // GET: api/appointments/physician/123
        [HttpGet("physician/{physicianId}")]
        public async Task<IActionResult> GetByPhysician(int physicianId)
        {
            var result = await _mediator.Send(new GetAppointmentsByPhysicianIdQuery(physicianId));
            return result != null ? Ok(result) : NotFound();
        }

        // GET: api/appointments/today
        [HttpGet("today")]
        public async Task<IActionResult> GetAppointmentsOfToday()
        {
            var result = await _mediator.Send(new GetAppointmentsOfTodayQuery());
            return result != null ? Ok(result) : NotFound();
        }


        

        

        

        

    }
}
