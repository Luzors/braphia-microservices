namespace Braphia.UserManagement.Events
{
    public class PatientCreatedEvent
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public PatientCreatedEvent() { }

        public PatientCreatedEvent(int patientId, string firstName, string lastName, string email, string phoneNumber)
        {
            PatientId = patientId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }
    }
}
