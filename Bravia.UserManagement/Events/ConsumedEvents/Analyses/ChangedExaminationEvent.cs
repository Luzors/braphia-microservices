using Braphia.UserManagement.Models.ExternalOnly;

namespace Braphia.UserManagement.Events.ConsumedEvents.Analyses
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
