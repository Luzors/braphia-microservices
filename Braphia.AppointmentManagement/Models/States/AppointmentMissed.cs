namespace Braphia.AppointmentManagement.Models.States
{
    public class AppointmentMissed : IAppointmentState
    {
        public string Name => "AppointmentMissed";
        public void AppointmentCreated(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot create an appointment that has already been missed.");
        }
        public void AppointnentStarted(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot start an appointment that has already been missed.");
        }
        public void AppointmentFinished(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot finish an appointment that has already been missed.");
        }
        public void AppointmentCanceled(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot cancel an appointment that has already been missed.");
        }
        public void AppointmentRescheduled(Appointment appointment, DateTime newTime)
        {
            Console.WriteLine("Appointment missed, rescheduling to a new time.");
            appointment.SetScheduledTime(newTime);
            appointment.SetState(new AppointmentRescheduled());
        }
    
        void IAppointmentState.AppointmentMissed(Appointment appointment)
        {
            throw new NotImplementedException();
        }
    }
}
