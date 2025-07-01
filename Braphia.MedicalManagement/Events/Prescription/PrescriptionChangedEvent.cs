namespace Braphia.MedicalManagement.Events.Prescription
{
    public class PrescriptionChangedEvent
    {
        public Models.Prescription Prescription { get; set; }

        public PrescriptionChangedEvent() { }

        public PrescriptionChangedEvent(Models.Prescription prescription)
        {
            Prescription = prescription;
        }
    }
}
