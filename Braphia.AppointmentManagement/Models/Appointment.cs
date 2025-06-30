using Braphia.AppointmentManagement.Enums;

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

        public AppointmentStateEnum state;
        public int? FollowUpAppointmentId { get; set; }
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

            state = AppointmentStateEnum.CREATED;
        }

       public void StartAppointment()
        {
            state = AppointmentStateEnum.STARTED;
        }

        public void FinishAppointment()
        {
            state = AppointmentStateEnum.FINISHED;
        }

        public void CancelAppointment()
        {
            state = AppointmentStateEnum.CANCELED;
        }

        public void AppointmentMissed()
        {
            state = AppointmentStateEnum.MISSED;
        }

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

