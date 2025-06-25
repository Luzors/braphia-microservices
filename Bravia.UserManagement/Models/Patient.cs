namespace Braphia.UserManagement.Models
{
    public class Patient : User
    {
        public Patient() { }

        public Patient(string firstName, string lastName, string email, string phoneNumber)
            : base(firstName, lastName, email, phoneNumber)
        { }

        public List<MedicalRecord> MedicalRecords { get; set; } = [];
    }
}
