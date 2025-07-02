namespace Braphia.AppointmentManagement.Events
{
    public class PatientArrivedEvent
    {
        public int AppointmentId { get; set; }
        public int PhysicianId { get; set; }

    }
}
