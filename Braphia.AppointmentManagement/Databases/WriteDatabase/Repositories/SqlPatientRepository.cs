using Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories.Interfaces;
using Braphia.AppointmentManagement.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase.Repositories
{
    public class SqlPatientRepository : IPatientRepository
    {
        private readonly DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        public SqlPatientRepository(DBContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context must be of type WriteDbContext.");
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }
        public async Task<bool> AddPatientAsync(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
            await _context.Patients.AddAsync(patient);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
            var existingPatient = await _context.Patients.FindAsync(patient.Id)
                ?? throw new ArgumentException($"Patient with ID {patient.Id} not found.");
            existingPatient.FirstName = patient.FirstName;
            existingPatient.LastName = patient.LastName;
            existingPatient.Email = patient.Email;
            existingPatient.PhoneNumber = patient.PhoneNumber;
            _context.Patients.Update(existingPatient);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeletePatientAsync(int patientId)
        {
            var patient = await GetPatientByIdAsync(patientId);
            if (patient == null) return false;
            _context.Patients.Remove(patient);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Patient> GetPatientByIdAsync(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            return patient ?? throw new ArgumentException($"Patient with ID {patientId} not found.");
        }
        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patients.ToListAsync()
                   ?? throw new ArgumentException("No patients found in the database.");
        }

        public async Task<bool> isIdChecked(int patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null)
            {
                throw new ArgumentException($"Patient with ID {patientId} not found.");
            }
            return patient.IsIdChecked;
        }

        public async Task<bool> setIdChecked(int patientId)
        {
            Console.WriteLine("SetIdChecked called");
            var patient = await GetPatientByIdAsync(patientId);
            Console.WriteLine($"Patient ID: {patientId}, IsIdChecked: {patient?.IsIdChecked}");
            if (patient == null)
            {
                throw new ArgumentException($"Patient with ID {patientId} not found.");
            }
            patient.IsIdChecked = true;
            var updated = await UpdatePatientAsync(patient);
            ;
            Console.WriteLine($"Patient ID: {patientId}, IsIdChecked after update]");
            return true;
        }
    }
}
