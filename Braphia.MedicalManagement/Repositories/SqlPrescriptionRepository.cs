using Braphia.MedicalManagement.Database;
using Braphia.MedicalManagement.Models;
using Braphia.MedicalManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Braphia.MedicalManagement.Repositories
{
    public class SqlPrescriptionRepository : IPrescriptionRepository
    {
        private readonly DBContext _context;

        public SqlPrescriptionRepository(DBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Prescription> GetPrescriptionAsync(int id)
        {
            var prescription = await _context.Prescription.FindAsync(id);
            if (prescription == null)
                throw new KeyNotFoundException($"Prescription with ID {id} not found.");
            return prescription;
        }

        public async Task<IEnumerable<Prescription>> GetAllPrescriptionsAsync()
        {
            return await _context.Prescription.ToListAsync();
        }

        public async Task<bool> AddPrescriptionAsync(Prescription prescription)
        {
            if (prescription == null)
                throw new ArgumentNullException(nameof(prescription), "Prescription cannot be null.");

            await _context.Prescription.AddAsync(prescription);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to add prescription.");
            return true;
        }

        public async Task<bool> DeletePrescriptionAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id), "ID cannot be null or whitespace.");

            var prescription = await _context.Prescription.FindAsync(id);
            if (prescription == null)
                throw new KeyNotFoundException($"Prescription with ID {id} not found.");

            _context.Prescription.Remove(prescription);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to delete prescription.");
            return true;
        }

        public async Task<bool> UpdatePrescriptionAsync(Prescription prescription)
        {
            if (prescription == null)
                throw new ArgumentNullException(nameof(prescription), "Prescription cannot be null.");

            var existing = await _context.Prescription.FindAsync(prescription.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Prescription with ID {prescription.Id} not found.");

            _context.Entry(existing).CurrentValues.SetValues(prescription);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to update prescription.");
            return true;
        }
    }
}
