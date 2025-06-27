namespace Braphia.AppointmentManagement.Models.States
{
    public class AppointmentCreated : IAppointmentState
    {
        public string Name => "AppointmentCreated";
        public string GetName() => Name;
        void IAppointmentState.AppointmentCreated(Appointment appointment)
        {
            Console.WriteLine("Appointment created successfully.");
            appointment.SetState(new AppointmentCreated());
        }
        public void AppointnentStarted(Appointment appointment)
        {
            
            appointment.SetState(new AppointmentStarted());
        }
        public void AppointmentFinished(Appointment appointment)
        {
            
            throw new InvalidOperationException("Cannot finish an appointment that hasn't started.");
        }
        public void AppointmentCanceled(Appointment appointment)
        {
            
            Console.WriteLine("Appointment canceled.");
            appointment.SetState(new AppointmentCancled());
        }
        public void AppointmentRescheduled(Appointment appointment, DateTime newTime)
        {
            appointment.SetScheduledTime(newTime);
            appointment.SetState(new AppointmentRescheduled());
        }
        public void AppointmentMissed(Appointment appointment)
        {
            // Transition to Missed state
            appointment.SetState(new AppointmentMissed());
        }

        
    }    
    
}
