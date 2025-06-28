using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events
{
    public class ChangedExaminationEvent
    {
        public MedicalAnalysis MedicalAnalysis { get; set; }

        public ChangedExaminationEvent() { }

        public ChangedExaminationEvent(MedicalAnalysis medicalAnalysis)
        {
            MedicalAnalysis = medicalAnalysis;
        }
    }
}
