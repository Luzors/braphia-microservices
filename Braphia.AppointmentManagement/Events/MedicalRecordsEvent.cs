namespace Braphia.AppointmentManagement.Events
{
    public class MedicalRecordsEvent
    {
        public int PatientId { get; set; }

        public MedicalRecordsEvent() { }

        public MedicalRecordsEvent(int patientId)
        {
            PatientId = patientId;
        }
    }
}
