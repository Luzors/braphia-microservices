using Braphia.AppointmentManagement.Models;

namespace Braphia.UserManagement.Events.Receptionists
{
    public class ReceptionistRegisteredEvent
    {
        public Receptionist Receptionist { get; set; }
        public ReceptionistRegisteredEvent() { }

        public ReceptionistRegisteredEvent(Receptionist receptionist)
        {
            Receptionist = receptionist ?? throw new ArgumentNullException(nameof(receptionist), "Receptionist cannot be null.");
        }
    }
}
