using System.ComponentModel.DataAnnotations;

namespace Braphia.Pharmacy.Models.ExternalObjects
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
