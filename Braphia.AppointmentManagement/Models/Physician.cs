using Braphia.UserManagement.Enums;

namespace Braphia.AppointmentManagement.Models
{
    public class Physician
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public SpecializationEnum Specialization { get; set; }

        public Physician() { }
        public Physician(int id, string firstName, string lastName, SpecializationEnum specialization)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Specialization = specialization;
        }
    }
}