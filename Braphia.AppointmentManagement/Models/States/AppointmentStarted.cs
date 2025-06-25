namespace Braphia.AppointmentManagement.Models.States
{
    public class AppointmentStarted : IAppointmentState
    {
        public string Name => "AppointmentStarted";
        public void AppointmentCreated(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot create an appointment that has already started.");
        }
        public void AppointnentStarted(Appointment appointment)
        {
            Console.WriteLine("Appointment has started successfully.");
            appointment.SetState(new AppointmentStarted());
        }
        public void AppointmentFinished(Appointment appointment)
        {
            Console.WriteLine("Appointment finished successfully.");
            appointment.SetState(new AppointmentFinished());
        }
        public void AppointmentCanceled(Appointment appointment)
        {
            Console.WriteLine("Appointment canceled while in progress.");
            appointment.SetState(new AppointmentCancled());
        }
        public void AppointmentRescheduled(Appointment appointment, DateTime newTime)
        {
            throw new InvalidOperationException("Cannot reschedule an appointment that has already started.");
        }
        public void AppointmentMissed(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot mark an ongoing appointment as missed.");
        }

    }
}
