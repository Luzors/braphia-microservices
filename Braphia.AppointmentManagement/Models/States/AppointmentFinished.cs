namespace Braphia.AppointmentManagement.Models.States
{
    public class AppointmentFinished : IAppointmentState
    {
        public string Name => "AppointmentFinished";
        public void AppointmentCreated(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot create an appointment that has already finished.");
        }
   
        void IAppointmentState.AppointmentFinished(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot finish an already finished appointment");
        }
        public void AppointnentStarted(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot finish an already finished appointment");
        }
        public void AppointmentCanceled(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot cancel an appointment that has already finished.");
        }
        public void AppointmentRescheduled(Appointment appointment, DateTime newTime)
        {
            throw new InvalidOperationException("Cannot reschedule an appointment that has already finished.");
        }
        public void AppointmentMissed(Appointment appointment)
        {
            throw new InvalidOperationException("Cannot mark a finished appointment as missed.");
        }

        
    }
}
