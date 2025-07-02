using Braphia.Laboratory.Database;
using Braphia.Laboratory.Models;
using Braphia.Laboratory.Repositories.Interfaces;
using Infrastructure.Messaging;
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

        public async Task<bool> AddPatientAsync(Patient patient, bool ignoreIdentity = false)
        {
            try
            {
                _context.Patient.Add(patient);
                if (ignoreIdentity)
                    await _context.SaveChangesWithIdentityInsertAsync();
                else
                    await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdatePatientAsync(Patient patient, bool ignoreIdentity = false)
        {
            try
            {
                _context.Patient.Update(patient);
                if (ignoreIdentity)
                    await _context.SaveChangesWithIdentityInsertAsync();
                else
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
                .Include(p => p.Tests)
                .FirstOrDefaultAsync(p => p.Id == patientId);               
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patient.ToListAsync();
        }

    }
}
