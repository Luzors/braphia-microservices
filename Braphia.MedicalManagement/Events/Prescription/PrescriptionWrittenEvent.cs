namespace Braphia.MedicalManagement.Events.Prescription
{
    public class PrescriptionWrittenEvent
    {
        public Models.Prescription Prescription { get; set; }

        public PrescriptionWrittenEvent() { }

        public PrescriptionWrittenEvent(Models.Prescription prescription)
        {
            Prescription = prescription;
        }
    }
}
