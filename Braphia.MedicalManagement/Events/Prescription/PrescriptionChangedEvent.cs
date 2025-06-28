using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events
{
    public class PrescriptionChangedEvent
    {
        public Prescription Prescription { get; set; }

        public PrescriptionChangedEvent() { }

        public PrescriptionChangedEvent(Prescription prescription)
        {
            Prescription = prescription;
        }
    }
}
