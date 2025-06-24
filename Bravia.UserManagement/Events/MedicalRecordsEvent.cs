using Infrastructure.Messaging;

namespace Braphia.UserManagement.Events
{
    public class MedicalRecordsEvent: Message
    {
        public int PatientId { get; set; }
        public string Action { get; set; }

        public MedicalRecordsEvent(int patientId, string action) : base(Guid.NewGuid(), action)
        {
            Console.WriteLine("MedicalRecordsEvent constructor called with patientId: {0}, action: {1}", patientId, action);
            PatientId = patientId;
            Action = action;
        }
    }
}
