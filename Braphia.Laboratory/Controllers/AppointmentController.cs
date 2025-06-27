using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Braphia.Laboratory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public AppointmentController(IAppointmentRepository appointmentRepository, IPublishEndpoint publishEndpoint)
        {
            this.appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        [HttpGet(Name = "Appointments")]
        [ProducesResponseType(typeof(IEnumerable<Appointment>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var records = await appointmentRepository.GetAllAppointmentsAsync();
                if (records == null || !records.Any())
                {
                    return NotFound("No appointments found");
                }
                return Ok(records);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error while fetching appointments");
            }
        }

        [HttpGet("{id}", Name = "AppointmentById")]
        [ProducesResponseType(typeof(Appointment), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var record = await appointmentRepository.GetAppointmentByIdAsync(id);
                if (record == null)
                {
                    return NotFound($"No appointment found with ID {id}");
                }
                return Ok(record);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error while fetching appointment");
            }
        }

        [HttpPost(Name = "Appointment")]
        [ProducesResponseType(typeof(Appointment), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] Appointment appointment)
        {
            if (appointment == null)
            {
                return BadRequest("Appointment data is required");
            }
            try
            {
                var createdAppointment = await appointmentRepository.AddAppointmentAsync(appointment);
                if (createdAppointment == false)
                {
                    return BadRequest("Failed to create appointment");
                }

                return CreatedAtAction("AppointmentById", new { id = appointment.Id }, appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error while creating appointment: {ex.Message}");
            }
        }
    }
}
