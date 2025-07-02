using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events.Examination
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
