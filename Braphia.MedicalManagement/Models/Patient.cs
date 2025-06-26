namespace Braphia.MedicalManagement.Models
{
    public class Patient : User
    {
        public Patient() { }

        public Patient(string firstName, string lastName, string email, string password)
            : base(firstName, lastName, email, password)
        { }

    }
}
