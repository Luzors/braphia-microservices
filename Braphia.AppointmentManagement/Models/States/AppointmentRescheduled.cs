namespace Braphia.AppointmentManagement.Models.States
{
    public class AppointmentRescheduled : IAppointmentState
    {
        public string Name => "AppointmentRescheduled";
        public void AppointmentCreated(Appointment appointment)
        {
            Console.WriteLine("Appointment rescheduled, put it back in created state");
            appointment.SetState(new AppointmentCreated());
        }
        public void AppointnentStarted(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot start an appointment that has already been rescheduled.");
        }
        public void AppointmentFinished(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot finish an appointment that has already been rescheduled.");
        }
        public void AppointmentCanceled(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot cancel an appointment that has already been rescheduled.");
        }
    
        public void AppointmentMissed(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot mark a rescheduled appointment as missed.");
        }

        void IAppointmentState.AppointmentRescheduled(Appointment appointment, DateTime newDate)
        {
            Console.WriteLine("Appointment rescheduled to a new time.");
            appointment.SetScheduledTime(newDate);
            appointment.SetState(new AppointmentRescheduled());
        }
    }
   
}
