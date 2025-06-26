using Braphia.Pharmacy.Database;
using Braphia.Pharmacy.Models.ExternalObjects;
using Braphia.Pharmacy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Pharmacy.Repositories
{
    public class SqlPatientRepository : IPatientRepository
    {
        private readonly ILogger<SqlPatientRepository> _logger;
        private readonly DBContext _context;

        public SqlPatientRepository(DBContext context, ILogger<SqlPatientRepository> logger)
        {
            _context = context;
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

        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
            try
            {
                _context.Patient.Update(patient);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
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
