namespace Braphia.MedicalManagement.Models
{
    public class Receptionist : User
    {
        public Receptionist() { }
        public Receptionist(string firstName, string lastName, string email, string phoneNumber)
            : base(firstName, lastName, email, phoneNumber)
        { }
    }
}
