using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events
{
    public class PrescriptionInvokedEvent
    {
        public Prescription Prescription { get; set; }

        public PrescriptionInvokedEvent() { }

        public PrescriptionInvokedEvent(Prescription prescription)
        {
            Prescription = prescription;
        }
    }
}
