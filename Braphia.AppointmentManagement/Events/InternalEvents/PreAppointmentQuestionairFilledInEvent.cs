namespace Braphia.AppointmentManagement.Events.InternalEvents
{
    public class PreAppointmentQuestionairFilledInEvent
    {
        public int AppointmentId { get; set; }
        public string answers { get; set; }
    }
}
