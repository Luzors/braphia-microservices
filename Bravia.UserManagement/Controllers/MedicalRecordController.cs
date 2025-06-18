using Braphia.UserManagement.Database;
using Braphia.UserManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Braphia.UserManagement.Controllers
{
    [Route("api/[controller]/{patientId}")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly ILogger<MedicalRecordController> _logger;

        private readonly DBContext _dbContext;

        public MedicalRecordController(ILogger<MedicalRecordController> logger, DBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet(Name = "MedicalRecordsByPatientId")]
        public IEnumerable<MedicalRecord> Get(int patientId)
        {
            _logger.LogInformation("Fetching medical records for patient with ID {patientId}", patientId);
            return _dbContext.Patient.Include(p => p.MedicalRecords)
                                     .Where(p => p.Id == patientId)
                                     .SelectMany(p => p.MedicalRecords)
                                     .ToList();
        }

        [HttpPost(Name = "AddMedicalRecord")]
        public IActionResult Post(int patientId, [FromBody] MedicalRecord medicalRecord)
        {
            _logger.LogInformation("Adding medical record for patient with ID {patientId}", patientId);
            if (medicalRecord == null)
                return BadRequest("MedicalRecord cannot be null");

            var patient = _dbContext.Patient.FirstOrDefault(p => p.Id == patientId);
            if (patient == null)
                return NotFound($"Patient with ID {patientId} not found");

            patient.MedicalRecords.Add(medicalRecord);
            _dbContext.SaveChanges();
            _logger.LogInformation("Medical record added for patient with ID {patientId}", patientId);

            return CreatedAtRoute("MedicalRecordsByPatientId", new { patientId = patient.Id }, medicalRecord);
        }

        [HttpDelete("{recordId}", Name = "DeleteMedicalRecord")]
        public IActionResult Delete(int patientId, int recordId)
        {
            _logger.LogInformation("Deleting medical record with ID {recordId} for patient with ID {patientId}", recordId, patientId);
            var patient = _dbContext.Patient.Include(p => p.MedicalRecords)
                                            .FirstOrDefault(p => p.Id == patientId);
            if (patient == null)
                return NotFound($"Patient with ID {patientId} not found");

            var medicalRecord = patient.MedicalRecords.FirstOrDefault(mr => mr.Id == recordId);
            if (medicalRecord == null)
                return NotFound($"MedicalRecord with ID {recordId} not found for Patient ID {patientId}");

            patient.MedicalRecords.Remove(medicalRecord);
            _dbContext.SaveChanges();
            _logger.LogInformation("Medical record with ID {recordId} deleted for patient with ID {patientId}", recordId, patientId);

            return NoContent();
        }
    }
}
