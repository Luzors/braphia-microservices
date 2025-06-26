using Braphia.Accounting.Database;
using Braphia.Accounting.Models;
using Braphia.Accounting.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Accounting.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AccountingDBContext _context;

        public PatientRepository(AccountingDBContext context)
        {
            _context = context;
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
            return await _context.Patient
                .Include(p => p.Insurer)
                .FirstOrDefaultAsync(p => p.Id == patientId);
        }public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patient.ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetPatientsByInsurerIdAsync(int insurerId)
        {
            return await _context.Patient
                .Where(p => p.InsurerId == insurerId)
                .ToListAsync();
        }
    }
}
