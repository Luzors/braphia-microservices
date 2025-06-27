using Braphia.AppointmentManagement.Models.States;
using Braphia.AppointmentManagement.Query.Abstractions;
using Braphia.UserManagement.Enums;

namespace Braphia.AppointmentManagement.Databases.ReadDatabase.Models
{
    public class AppointmentViewQueryModel : IQueryModel
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public string PatientEmail { get; set; }
        public string PatientPhoneNumber { get; set; }
        public int PhysicianId { get; set; }
        public string PhysicianFirstName { get; set; }
        public string PhysicianLastName { get; set; }
        public SpecializationEnum PhysicianSpecialization { get; set; }
        public int ReceptionistId { get; set; }
        public string ReceptionistFirstName { get; set; }
        public string ReceptionistLastName { get; set; }
        public string ReceptionistEmail { get; set; }
        public int ReferralId { get; set; }
        public DateTime ReferralDate { get; set; } 
        public string ReferralReason { get; set; } 
        public DateTime ScheduledTime { get; set; }
        public string StateName { get; set; }

    }



}

