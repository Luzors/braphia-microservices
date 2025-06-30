namespace Braphia.AppointmentManagement.Models.States
{
    public class AppointmentCancled :  IAppointmentState
    {
        public string Name => "AppointmentCancled";
        public string GetName() => Name;

        public void AppointmentCreated(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot create an appointment that has already been canceled.");
        }
        public void AppointnentStarted(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot start an appointment that has already been canceled.");
        }
        public void AppointmentFinished(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot finish an appointment that has already been canceled.");
        }
        public void AppointmentCanceled(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot cancel an appointment that has already been canceled.");
        }
        public void AppointmentMissed(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot mark a canceled appointment as missed.");
        }
    }
}
