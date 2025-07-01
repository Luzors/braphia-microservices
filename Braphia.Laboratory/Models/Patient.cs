using System.ComponentModel.DataAnnotations;

namespace Braphia.Laboratory.Models
{
    public class Patient
    {
        public Patient() { }

        public Patient(int rootId, string firstName, string lastName, string email, string phoneNumber)
        {
            RootId = rootId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        [Key]
        public int Id { get; set; }
        public int RootId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        // tests
        public List<Test> Tests { get; set; } = new List<Test>();
    }
}
