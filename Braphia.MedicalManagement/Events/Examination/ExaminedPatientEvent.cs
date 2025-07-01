using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events.Examination
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
