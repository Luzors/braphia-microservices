namespace Braphia.UserManagement.Models
{
    public class Patient : User
    {
        public Patient() { }

        public Patient(string firstName, string lastName, string email, string password)
            : base(firstName, lastName, email, password)
        { }

        public List<MedicalRecord> MedicalRecords { get; set; } = [];
    }
}
