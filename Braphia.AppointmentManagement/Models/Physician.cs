using Braphia.UserManagement.Enums;

namespace Braphia.AppointmentManagement.Models
{
    public class Physician
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public SpecializationEnum Specialization { get; set; }
    }
}