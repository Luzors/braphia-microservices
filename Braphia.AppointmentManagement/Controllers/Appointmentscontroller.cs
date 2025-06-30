using Braphia.AppointmentManagement.Commands.AddAppointment;
using Braphia.AppointmentManagement.Commands.AppointmentRescheduled;
using Braphia.AppointmentManagement.Commands.AppointmentStateChanged;
using Braphia.AppointmentManagement.Commands.UserCheckId;
using Braphia.AppointmentManagement.Databases.WriteDatabase;
using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Enums;
using Braphia.AppointmentManagement.Models;
using Braphia.AppointmentManagement.Query.GetAllAppointments;
using Braphia.AppointmentManagement.Query.GetAppointmentById;
using Braphia.AppointmentManagement.Query.GetAppointmentIdChecked;
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

        //Post : Api/changeState/{id}
        [HttpPost("changeState/{id}")]
        public async Task<IActionResult> ChangeState(int id, [FromBody] string newState)
        {
            // This method will change the state of the appointment based on the provided ID and new state
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound($"Appointment with ID {id} not found.");
            }
            switch (newState.ToLower())
            {
                case "started":
                    var r = await _mediator.Send(new GetAppointmentIdCheckedQuery(id));
                    if (r == null)
                    {
                        return NotFound($"Appointment with ID {id} not found in read database.");
                    }
                    //check if id is checked in read database
                    if (!r)
                    {
                        return BadRequest($"Appointment with ID {id} is not checked.");
                    }
                    // Compare the state name directly using the `Name` property
                    if (appointment.state != AppointmentStateEnum.CREATED)
                    {
                        return BadRequest($"Appointment with ID {id} is not in the correct state to start the appointment.");
                    }
                    appointment.StartAppointment();
                    break;
                case "finished":
                   
                    // Check if state is started in read database
                    if(appointment.state != AppointmentStateEnum.STARTED)
                    {
                        return BadRequest($"Appointment with ID {id} is not in the correct state to finish the appointment.");
                    }

                    appointment.FinishAppointment();
                    break;
                case "canceled":
                    // Check if state is not started or finished in read database
                    if (appointment.state == AppointmentStateEnum.STARTED || appointment.state == AppointmentStateEnum.FINISHED)
                    {
                        return BadRequest($"Appointment with ID {id} cannot be canceled as it is already started or finished.");
                    }
                    appointment.CancelAppointment();
                    break;
                case "missed":
                    // Check if state is created
                    if (appointment.state != AppointmentStateEnum.CREATED)
                    {
                        return BadRequest($"Appointment with ID {id} is not in the correct state to mark as missed.");
                    }
                    appointment.AppointmentMissed();
                    break;
                default:
                    return BadRequest("Invalid state provided.");
            }

            var command = new AppointmentStateChangedCommand(appointment.Id, appointment.state);
            var result = await _mediator.Send(command);
            return Ok(appointment.Id);
        }

 
        [HttpPost("checkId")]
        public async Task<IActionResult> CheckId(UserCheckIdCommand userCheckIdCommand)
        {
            // This method will turn the isidchecked property of the appointment to true
            Console.WriteLine($"Checking ID for appointment with ID: {userCheckIdCommand.UserId}");
            var result = await _mediator.Send(userCheckIdCommand);

            // Ensure 'result' is treated as a boolean by checking its value explicitly
            return result > 0 ? Ok("ID checked successfully.") : BadRequest("Failed to check ID.");
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
