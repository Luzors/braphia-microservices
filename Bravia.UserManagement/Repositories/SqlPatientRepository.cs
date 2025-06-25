using Braphia.UserManagement.Database;
using Braphia.UserManagement.Events;
using Braphia.UserManagement.Models;
using Braphia.UserManagement.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.UserManagement.Repositories
{
    public class SqlPatientRepository : IPatientRepository
    {
        private DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        public SqlPatientRepository(DBContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<bool> AddMedicalRecordAsync(int patientId, MedicalRecord medicalRecord)
        {
            if (medicalRecord == null)
                throw new ArgumentNullException(nameof(medicalRecord), "MedicalRecord cannot be null.");
            var patient = await _context.Patient.Include(p => p.MedicalRecords).FirstOrDefaultAsync(p => p.Id == patientId);
            if (patient == null)
                throw new ArgumentException($"Patient with ID {patientId} not found.");
            patient.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();
            await _publishEndpoint.Publish(new Message(messageType: "PostMedicalRecord", data: new MedicalRecordsEvent(patientId)));
            return true;
        }

        public async Task<bool> AddPatientAsync(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
            await _context.Patient.AddAsync(patient);
            await _context.SaveChangesAsync();

            // Patient created event
            await _publishEndpoint.Publish(new Message(
                messageType: "PatientCreated",
                data: new PatientCreatedEvent(patient)
            ));

            return true;
        }

        public async Task<bool> DeleteMedicalRecordAsync(int patientId, int medicalRecordId)
        {
            var patient = await _context.Patient.Include(p => p.MedicalRecords).FirstOrDefaultAsync(p => p.Id == patientId);
            if (patient == null)
                throw new ArgumentException($"Patient with ID {patientId} not found.");
            var record = patient.MedicalRecords.FirstOrDefault(r => r.Id == medicalRecordId);
            if (record == null)
                throw new ArgumentException($"Medical record with ID {medicalRecordId} not found for patient {patientId}.");
            patient.MedicalRecords.Remove(record);
            await _context.SaveChangesAsync();
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

        public async Task<MedicalRecord?> GetMedicalRecordByIdAsync(int patientId, int medicalRecordId)
        {
            var patient = await _context.Patient.Include(p => p.MedicalRecords).FirstOrDefaultAsync(p => p.Id == patientId);
            return patient?.MedicalRecords.FirstOrDefault(r => r.Id == medicalRecordId);
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByPatientIdAsync(int patientId)
        {
            var patient = await _context.Patient.Include(p => p.MedicalRecords).FirstOrDefaultAsync(p => p.Id == patientId);
            return patient?.MedicalRecords ?? Enumerable.Empty<MedicalRecord>();
        }

        public async Task<Patient?> GetPatientByIdAsync(int patientId)
        {
            return await _context.Patient.FirstOrDefaultAsync(p => p.Id == patientId);
        }

        public async Task<Patient?> GetPatientByFullNameAsync(string firstName, string lastName)
        {
            return await _context.Patient.FirstOrDefaultAsync(p => p.FirstName == firstName && p.LastName == lastName);
        }

        public async Task<bool> UpdateMedicalRecordAsync(int patientId, MedicalRecord medicalRecord)
        {
            if (medicalRecord == null)
                throw new ArgumentNullException(nameof(medicalRecord), "MedicalRecord cannot be null.");
            var patient = await _context.Patient.Include(p => p.MedicalRecords).FirstOrDefaultAsync(p => p.Id == patientId);
            if (patient == null)
                throw new ArgumentException($"Patient with ID {patientId} not found.");
            var record = patient.MedicalRecords.FirstOrDefault(r => r.Id == medicalRecord.Id);
            if (record == null)
                throw new ArgumentException($"Medical record with ID {medicalRecord.Id} not found for patient {patientId}.");
            record.Description = medicalRecord.Description;
            record.Date = medicalRecord.Date;
            await _context.SaveChangesAsync();
            return true;
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
