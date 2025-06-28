using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Repositories.Interfaces
{
    public interface IMedicalAnalysisRepository
    {
        Task<MedicalAnalysis> GetMedicalAnalysisAsync(int id);

        Task<IEnumerable<MedicalAnalysis>> GetAllMedicalAnalysissAsync();

        Task<bool> AddMedicalAnalysisAsync(MedicalAnalysis medicalAnalysis);

        Task<bool> DeleteMedicalAnalysisAsync(int id);

        Task<bool> UpdateMedicalAnalysisAsync(MedicalAnalysis medicalAnalysis);
    }
}
