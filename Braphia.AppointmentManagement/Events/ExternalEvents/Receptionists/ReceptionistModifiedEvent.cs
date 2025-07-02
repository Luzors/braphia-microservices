using Braphia.AppointmentManagement.Models;

namespace Braphia.UserManagement.Events.Receptionists
{
    public class ReceptionistModifiedEvent
    {
        public int ReceptionistId { get; set; }
        public Receptionist NewReceptionist { get; set; }
        public ReceptionistModifiedEvent() { }

        public ReceptionistModifiedEvent(int receptionistId, Receptionist newReceptionist)
        {
            ReceptionistId = receptionistId;
            NewReceptionist = newReceptionist ?? throw new ArgumentNullException(nameof(newReceptionist), "New receptionist cannot be null.");
        }
    }
}
