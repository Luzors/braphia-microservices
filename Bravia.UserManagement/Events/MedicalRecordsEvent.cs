namespace Braphia.UserManagement.Events
{
    public class MedicalRecordsEvent
    {
        public int PatientId { get; set; }
        public string Action { get; set; }

        public MedicalRecordsEvent(int patientId, string action)
        {
            PatientId = patientId;
            Action = action;
        }
    }
}
