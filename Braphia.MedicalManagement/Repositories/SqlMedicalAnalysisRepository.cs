using Braphia.MedicalManagement.Database;
using Braphia.MedicalManagement.Events;
using Braphia.MedicalManagement.Models;
using Braphia.MedicalManagement.Repositories.Interfaces;
using Infrastructure.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Braphia.MedicalManagement.Repositories
{
    public class SqlMedicalAnalysisRepository : IMedicalAnalysisRepository
    {
        private readonly DBContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public SqlMedicalAnalysisRepository(DBContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<MedicalAnalysis> GetMedicalAnalysisAsync(int id)
        {
            var medicalAnalysis = await _context.MedicalAnalysis.FindAsync(id);
            if (medicalAnalysis == null)
                throw new KeyNotFoundException($"MedicalAnalysis with ID {id} not found.");
            return medicalAnalysis;
        }

        public async Task<IEnumerable<MedicalAnalysis>> GetAllMedicalAnalysissAsync()
        {
            return await _context.MedicalAnalysis.ToListAsync();
        }

        public async Task<bool> AddMedicalAnalysisAsync(MedicalAnalysis medicalAnalysis)
        {
            if (medicalAnalysis == null)
                throw new ArgumentNullException(nameof(medicalAnalysis), "MedicalAnalysis cannot be null.");

            await _context.MedicalAnalysis.AddAsync(medicalAnalysis);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to add medicalAnalysis.");

            await _publishEndpoint.Publish(new Message(new ExaminedPatientEvent(medicalAnalysis)));
            return true;
        }

        public async Task<bool> DeleteMedicalAnalysisAsync(int id)
        {
            var medicalAnalysis = await _context.MedicalAnalysis.FindAsync(id);
            if (medicalAnalysis == null)
                throw new KeyNotFoundException($"MedicalAnalysis with ID {id} not found.");

            _context.MedicalAnalysis.Remove(medicalAnalysis);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to delete medicalAnalysis.");
            return true;
        }

        public async Task<bool> UpdateMedicalAnalysisAsync(MedicalAnalysis medicalAnalysis)
        {
            if (medicalAnalysis == null)
                throw new ArgumentNullException(nameof(medicalAnalysis), "MedicalAnalysis cannot be null.");

            _context.MedicalAnalysis.Update(medicalAnalysis);
            if (await _context.SaveChangesAsync() <= 0)
                throw new InvalidOperationException("Failed to update medicalAnalysis.");

            await _publishEndpoint.Publish(new Message(new ChangedExaminationEvent(medicalAnalysis)));

            return true;
        }
    }
}
