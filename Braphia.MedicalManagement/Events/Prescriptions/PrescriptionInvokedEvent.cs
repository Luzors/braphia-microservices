namespace Braphia.MedicalManagement.Events.Prescription
{
    public class PrescriptionInvokedEvent
    {
        public Models.Prescription Prescription { get; set; }

        public PrescriptionInvokedEvent() { }

        public PrescriptionInvokedEvent(Models.Prescription prescription)
        {
            Prescription = prescription;
        }
    }
}
