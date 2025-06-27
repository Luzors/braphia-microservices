using Braphia.Pharmacy.Database;
using Braphia.Pharmacy.Models.ExternalObjects;
using Braphia.Pharmacy.Repositories.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Pharmacy.Repositories
{
    public class SqlPatientRepository : IPatientRepository
    {
        private readonly ILogger<SqlPatientRepository> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly DBContext _context;

        public SqlPatientRepository(DBContext context, ILogger<SqlPatientRepository> logger, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> AddPatientAsync(Patient patient)
        {
            try
            {
                _context.Patient.Add(patient);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdatePatientAsync(int patientId, Patient patient)
        {
            try
            {
                var existingPatient = await _context.Patient.FindAsync(patientId);
                if (existingPatient == null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found for update", patientId);
                    return false;
                }
                existingPatient.RootId = patient.RootId;
                existingPatient.FirstName = patient.FirstName;
                existingPatient.LastName = patient.LastName;
                existingPatient.Email = patient.Email;
                existingPatient.PhoneNumber = patient.PhoneNumber;
                _context.Patient.Update(existingPatient);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                _logger.LogError("Error updating patient with ID {PatientId}", patientId);
                return false;
            }
        }

        public async Task<bool> DeletePatientAsync(int patientId)
        {
            try
            {
                var patient = await _context.Patient.FindAsync(patientId);
                if (patient == null) return false;

                _context.Patient.Remove(patient);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Patient?> GetPatientByIdAsync(int patientId)
        {
            return await _context.Patient.FindAsync(patientId);
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patient.ToListAsync();
        }
    }
}
