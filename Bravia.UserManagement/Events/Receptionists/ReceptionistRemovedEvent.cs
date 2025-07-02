using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Events.Receptionists
{
    public class ReceptionistRemovedEvent
    {
        public Receptionist Receptionist { get; set; }
        public ReceptionistRemovedEvent() { }

        public ReceptionistRemovedEvent(Receptionist receptionist)
        {
            Receptionist = receptionist ?? throw new ArgumentNullException(nameof(receptionist), "Receptionist cannot be null.");
        }
    }
}
