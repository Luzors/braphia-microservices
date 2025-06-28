using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events
{
    public class MedicalAnalysisCreatedEvent
    {
        public MedicalAnalysis MedicalAnalysis { get; set; }

        public MedicalAnalysisCreatedEvent() { }

        public MedicalAnalysisCreatedEvent(MedicalAnalysis medicalAnalysis)
        {
            MedicalAnalysis = medicalAnalysis;
        }
    }
}
