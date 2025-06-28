using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events
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
