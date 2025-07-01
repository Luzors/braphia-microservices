namespace Braphia.NotificationDispatcher.Events.ExternalEvents
{
    public class PatientArrivedEvent
    {
        public int AppointmentId { get; set; }
        public int PhysicianId { get; set; }

    }
}
