using Braphia.Laboratory.Database;
using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Braphia.Laboratory.Repositories
{
    public class SqlPatientRepository : IPatientRepository
    {
        private readonly DBContext _context;

        public SqlPatientRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddPatientAsync(Patient patient)
        {
            try
            {
                _context.Patients.Add(patient);
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
                _context.Patients.Update(patient);
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
                var patient = await _context.Patients.FindAsync(patientId);
                if (patient == null) return false;

                _context.Patients.Remove(patient);
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
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == patientId);
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patients.ToListAsync();
        }

    }
}
