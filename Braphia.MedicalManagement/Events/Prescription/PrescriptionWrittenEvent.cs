using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events
{
    public class PrescriptionWrittenEvent
    {
        public Prescription Prescription { get; set; }

        public PrescriptionWrittenEvent() { }

        public PrescriptionWrittenEvent(Prescription prescription)
        {
            Prescription = prescription;
        }
    }
}
