using Braphia.UserManagement.Enums;

namespace Braphia.AppointmentManagement.Models
{
    public class Physician
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public SpecializationEnum specialization { get; set; }
    }
}