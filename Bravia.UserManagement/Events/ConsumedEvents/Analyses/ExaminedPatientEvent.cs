using Braphia.UserManagement.Models.ExternalOnly;

namespace Braphia.UserManagement.Events.ConsumedEvents.Analyses
{
    public class ExaminedPatientEvent
    {
        public MedicalAnalysis MedicalAnalysis { get; set; }

        public ExaminedPatientEvent() { }

        public ExaminedPatientEvent(MedicalAnalysis medicalAnalysis)
        {
            MedicalAnalysis = medicalAnalysis;
        }
    }
}
