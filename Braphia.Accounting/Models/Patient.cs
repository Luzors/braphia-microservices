using System.ComponentModel.DataAnnotations;

namespace Braphia.Accounting.Models
{
    public class Patient
    {
        public Patient() { }

        public Patient(string firstName, string lastName, string email, string phoneNumber)
        { 
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }
        
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
