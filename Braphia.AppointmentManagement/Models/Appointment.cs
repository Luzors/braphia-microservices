using Braphia.AppointmentManagement.Models.States;

namespace Braphia.AppointmentManagement.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient patient { get; set; }
        public int PhysicianId { get; set; }
        public Physician physician { get; set; }

        public int ReceptionistId { get; set; }
        public Receptionist receptionist { get; set; }

        public int ReferralId { get; set; }
        public Referral referral { get; set; }

        public DateTime ScheduledTime { get; set; }

        public IAppointmentState state;
        public int FollowUpAppointmentId { get; set; }
        public Appointment FollowUpAppointment { get; set; }

        public Appointment(){}

        public Appointment(int id, int patientId, int physicianId, int receptionistId, int referralId, DateTime scheduledTime)
        {
            Id = id;
            PatientId = patientId;
            PhysicianId = physicianId;
            ReceptionistId = receptionistId;
            ReferralId = referralId;
            ScheduledTime = scheduledTime;

            SetState(new AppointmentCreated()); // Assuming AppointmentScheduledState is a valid implementation of IAppointmentState
        }

        public void SetState(IAppointmentState state)
        {
            state = state;
        }

        public void AppointmentCreated() => state.AppointmentCreated(this);
        public void AppointmentStarted() => state.AppointnentStarted(this);
        public void AppointmentFinished() => state.AppointmentFinished(this);
        public void AppointmentCancled() => state.AppointmentCanceled(this);
        public void AppointmentReschedules(DateTime newTime) => state.AppointmentRescheduled(this, newTime);
        public void AppointmentMissed() => state.AppointmentMissed(this);

        public void SetScheduledTime(DateTime newTime)
        {
            ScheduledTime = newTime;
        }

        public void SetFollowUpAppointment(Appointment followUpAppointment)
        {
            FollowUpAppointment = followUpAppointment;
            FollowUpAppointmentId = followUpAppointment.Id;
        }
    }
}

