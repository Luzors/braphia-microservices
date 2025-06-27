using MediatR;

namespace Braphia.AppointmentManagement.Commands.AddAppointment
{
    public class AppointmentCreatedCommand : IRequest<int>
    {
        public int PatientId { get; set; }
        public int PhysicianId { get; set; }
        public int ReceptionistId { get; set; }
        public int ReferralId { get; set; }
        public DateTime ScheduledTime { get; set; }
    }
}
