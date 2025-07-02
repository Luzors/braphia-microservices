using Braphia.Pharmacy.Models.ExternalObjects;

namespace Braphia.Pharmacy.Events.ExternalEvents
{
    public class PatientModifiedEvent
    {
        public int PatientId { get; set; }
        public Patient NewPatient { get; set; }
        public PatientModifiedEvent() { }

        public PatientModifiedEvent(int patientId, Patient newPatient)
        {
            PatientId = patientId;
            NewPatient = newPatient ?? throw new ArgumentNullException(nameof(newPatient), "New patient cannot be null.");
        }
    }
}
