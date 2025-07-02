using Braphia.NotificationDispatcher.Models;

namespace Braphia.NotificationDispatcher.Events.ExternalEvents.Patients
{
    public class PatientRemovedEvent
    {
        public User Patient { get; set; }
        public PatientRemovedEvent() { }

        public PatientRemovedEvent(User patient)
        {
            Patient = patient ?? throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
        }
    }
}
