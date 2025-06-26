using Braphia.AppointmentManagement.Models.States;

namespace Braphia.AppointmentManagement.Models
{
    public class Appointment
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public Patient patient { get; set; }
        public Guid PhysicianId { get; set; }
        public Physician physician { get; set; }

        public Guid ReceptionistId { get; set; }
        public Receptionist receptionist { get; set; }

        public Guid ReferralId { get; set; }
        public Referral referral { get; set; }

        public DateTime ScheduledTime { get; set; }

        private IAppointmentState _state;

        public Appointment(Guid patientId, Guid physicianId, Guid receptionistId, Guid referralId, DateTime scheduledTime)
        {
            Id = Guid.NewGuid();
            PatientId = patientId;
            PhysicianId = physicianId;
            ReceptionistId = receptionistId;
            ReferralId = referralId;
            ScheduledTime = scheduledTime;

            SetState(new AppointmentCreated()); // Assuming AppointmentScheduledState is a valid implementation of IAppointmentState
        }

        public void SetState(IAppointmentState state)
        {
            _state = state;
            StateName = _state.Name;
        }

        public void AppointmentCreated() => _state.AppointmentCreated(this);
        public void AppointmentStarted() => _state.AppointnentStarted(this);
        public void AppointmentFinished() => _state.AppointmentFinished(this);
        public void AppointmentCancled() => _state.AppointmentCanceled(this);
        public void AppointmentReschedules(DateTime newTime) => _state.AppointmentRescheduled(this, newTime);
        public void AppointmentMissed() => _state.AppointmentMissed(this);

        public void SetScheduledTime(DateTime newTime)
        {
            ScheduledTime = newTime;
        }
    }
}

