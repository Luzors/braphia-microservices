namespace Braphia.AppointmentManagement.Models.States
{
    public interface IAppointmentState
    {
        string Name { get; }
        void AppointmentCreated(Appointment appointment);
        void AppointnentStarted(Appointment appointment);
        void AppointmentFinished(Appointment appointment);
        void AppointmentCanceled(Appointment appointment);
        void AppointmentRescheduled(Appointment appointment, DateTime newDate);
        void AppointmentMissed(Appointment appointment);
    }
}