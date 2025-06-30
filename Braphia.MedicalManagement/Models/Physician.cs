using Braphia.MedicalManagement.Enums;

namespace Braphia.MedicalManagement.Models
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
