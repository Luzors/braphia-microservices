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

        public AppointmentStateEnum state { get; set; }
        public int? FollowUpAppointmentId { get; set; }
        public string? PreAppointmentQuestionnaire { get; set; }




        public Appointment() { }

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
            FollowUpAppointmentId = followUpAppointment.Id;
        }

        public void SetPreAppointmentQuestionnaire()
        {
            var DefaultQuestionnaire = new List<QuestionnaireAnswer>
            {
                new QuestionnaireAnswer { Question = "How are you feeling today?", Answer = "" },
                new QuestionnaireAnswer { Question = "Any specific concerns?", Answer = "" }
            };

            // Set default questionnaire to string
           var  questionnaireAnswers = string.Join(";", DefaultQuestionnaire.Select(q => $"{q.Question}:{q.Answer}"));

            PreAppointmentQuestionnaire = questionnaireAnswers;
        }
    }
}

