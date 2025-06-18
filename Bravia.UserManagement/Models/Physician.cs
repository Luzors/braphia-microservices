using Braphia.UserManagement.Enums;

namespace Braphia.UserManagement.Models
{
    public class Physician : User
    {
        public Physician() { }
        public Physician(string firstName, string lastName, string email, string phoneNumber)
            : base(firstName, lastName, email, phoneNumber)
        { }

        public SpecializationEnum Specialization { get; set; }
    }
}
