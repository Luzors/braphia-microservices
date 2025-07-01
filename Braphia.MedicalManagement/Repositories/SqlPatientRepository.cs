using Braphia.MedicalManagement.Database;
using Braphia.MedicalManagement.Models;
using Braphia.MedicalManagement.Repositories.Interfaces;
using Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Braphia.MedicalManagement.Repositories
{
    public class SqlPatientRepository : IPatientRepository
    {
        private DBContext _context;
        public SqlPatientRepository(DBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<bool> AddPatientAsync(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
            await _context.Patient.AddAsync(patient);
            await _context.SaveChangesWithIdentityInsertAsync();
            return true;
        }

        public async Task<bool> DeletePatientAsync(int patientId)
        {
            var patient = await _context.Patient.FindAsync(patientId);
            if (patient == null)
                throw new ArgumentException($"Patient with ID {patientId} not found.");
            _context.Patient.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patient.ToListAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(int patientId)
        {
            return await _context.Patient.FirstOrDefaultAsync(p => p.Id == patientId);
        }

        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
            var existing = await _context.Patient.FindAsync(patient.Id);
            if (existing == null)
                throw new ArgumentException($"Patient with ID {patient.Id} not found.");
            existing.FirstName = patient.FirstName;
            existing.LastName = patient.LastName;
            existing.Email = patient.Email;
            existing.PhoneNumber = patient.PhoneNumber;
            existing.BirthDate = patient.BirthDate;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
