using Braphia.AppointmentManagement.Commands.AddAppointment;
using Braphia.AppointmentManagement.Commands.AppointmentFollowUpScheduled;
using Braphia.AppointmentManagement.Commands.AppointmentRescheduled;
using Braphia.AppointmentManagement.Commands.AppointmentStateChanged;
using Braphia.AppointmentManagement.Commands.QuestionnaireAnswered;
using Braphia.AppointmentManagement.Commands.UserCheckId;
using Braphia.AppointmentManagement.Databases.WriteDatabase;
using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Enums;
using Braphia.AppointmentManagement.Events.InternalEvents;
using Braphia.AppointmentManagement.Models;
using Braphia.AppointmentManagement.Query.GetAllAppointments;
using Braphia.AppointmentManagement.Query.GetAppointmentById;
using Braphia.AppointmentManagement.Query.GetAppointmentIdChecked;
using Braphia.AppointmentManagement.Query.GetAppointmentsByPatient;
using Braphia.AppointmentManagement.Query.GetQuestionaireByAppointment;
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
                        return BadRequest($"User of Appointment ID {id} is not checked.");
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

        // POST: api/appointments/checkId
        [HttpPost("checkId")]
        public async Task<IActionResult> CheckId(UserCheckIdCommand userCheckIdCommand)
        {
            // This method will turn the isidchecked property of the appointment to true
            Console.WriteLine($"Checking ID for appointment with ID: {userCheckIdCommand.UserId}");
            var result = await _mediator.Send(userCheckIdCommand);

            // Ensure 'result' is treated as a boolean by checking its value explicitly
            return result > 0 ? Ok("ID checked successfully.") : BadRequest("Failed to check ID.");
        }

        // POST: api/appointments/followup
        [HttpPost("followup")]
        public async Task<IActionResult> CreateFollowUpAppointment([FromBody] AppointmentFollowUpScheduledCommand command)
        {
            
            var appointmentId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = appointmentId }, new { appointmentId });
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

        // GET: api/appointments/patient/123
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(int patientId)
        {
            var result = await _mediator.Send(new GetAppointmentsByPatientIdQuery(patientId));
            if (result != null)
            {
                

                // Create a modified result with the desired properties
                var modifiedResult = result.Select(appointment => new
                {
                    appointment.Id,
                    appointment.PatientFirstName,
                    appointment.PatientLastName,
                    appointment.PhysicianFirstName,
                    appointment.PhysicianLastName,
                    appointment.PhysicianSpecialization,
                    appointment.ReceptionistFirstName,
                    appointment.ReceptionistLastName,
                    appointment.ReferralReason,
                    appointment.ScheduledTime,
                    state = appointment.State == AppointmentStateEnum.CREATED ? "CREATED" :
                            appointment.State == AppointmentStateEnum.STARTED ? "STARTED" :
                            appointment.State == AppointmentStateEnum.FINISHED ? "FINISHED" :
                            appointment.State == AppointmentStateEnum.CANCELED ? "CANCELED" :
                            appointment.State == AppointmentStateEnum.MISSED ? "MISSED" : "UNKNOWN"
                }).ToList();

                return Ok(modifiedResult);
            }
            return NotFound();
        }

        [HttpGet("{appointmentId}/questionnaire")]
        public async Task<IActionResult> GetPreAppointmentQuestionnaire(int appointmentId)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
                return NotFound($"Appointment with ID {appointmentId} not found.");

            var questionaire = await _mediator.Send(new GetQuestionaireByAppointmentQuery(appointmentId));
            if(appointment.IsPreAppointmentQuestionnaireFilled == false)
            {
                if (questionaire != null && questionaire.Any())
                {
                    var questionnaireString = string.Join(";", questionaire);
                    return Ok(questionnaireString);
                }
                else
                {
                    return NotFound($"No questionnaire found for appointment with ID {appointmentId}.");
                }
            }
            return questionaire != null ? Ok(questionaire) : NotFound($"No questionnaire found for appointment with ID {appointmentId}.");
        }

        [HttpPost("questionnaire")]
        public async Task<IActionResult> SubmitPreAppointmentQuestionnaire([FromBody] QuestionnaireAnsweredCommand command)
        {
            var appointmentId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = appointmentId }, new { appointmentId });
        }












    }
}
