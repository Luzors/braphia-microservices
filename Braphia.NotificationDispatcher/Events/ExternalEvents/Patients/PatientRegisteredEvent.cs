using Braphia.NotificationDispatcher.Models;

namespace Braphia.NotificationDispatcher.Events.ExternalEvents.Patients
{
    public class PatientRegisteredEvent
    {
        public User Patient { get; set; }
        public PatientRegisteredEvent() { }

        public PatientRegisteredEvent(User patient)
        {
            Patient = patient ?? throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
        }
    }
}
