using System.ComponentModel.DataAnnotations;

namespace Braphia.Accounting.Models
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
        public int? InsurerId { get; set; }
        public Insurer? Insurer { get; set; }
    }
}
