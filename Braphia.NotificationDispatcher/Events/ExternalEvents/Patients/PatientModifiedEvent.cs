using Braphia.NotificationDispatcher.Models;

namespace Braphia.NotificationDispatcher.Events.ExternalEvents.Patients
{
    public class PatientModifiedEvent
    {
        public int PatientId { get; set; }
        public User NewPatient { get; set; }
        public PatientModifiedEvent() { }

        public PatientModifiedEvent(int patientId, User newPatient)
        {
            PatientId = patientId;
            NewPatient = newPatient ?? throw new ArgumentNullException(nameof(newPatient), "New patient cannot be null.");
        }
    }
}
