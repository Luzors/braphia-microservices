namespace Braphia.AppointmentManagement.Models.States
{
    public interface IAppointmentState
    {
        string Name { get; }
        string GetName() => Name;
        void AppointmentCreated(Appointment appointment);
        void AppointnentStarted(Appointment appointment);
        void AppointmentFinished(Appointment appointment);
        void AppointmentCanceled(Appointment appointment);
        void AppointmentMissed(Appointment appointment);
    }
}