namespace Braphia.AppointmentManagement.Events.InternalEvents
{
    public class InternalPreAppointmentQuestionairFilledInEvent
    {
        public int AppointmentId { get; set; }
        public string answers { get; set; }
    }
}
